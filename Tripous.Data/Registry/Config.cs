namespace Tripous.Data;

[TypeStore]
public enum ConfigScope
{
    System = 0,
    Company = 1,
    User = 2
}

[TypeStore]
public enum ConfigValueKind
{
    String = 0,
    Integer = 1,
    Boolean = 2,
    Date = 3,
    Time = 4,
    Double = 5,
    Decimal = 6,
    Lookup = 7,
    Enum = 8,
    Memo = 50,
    Object = 100,
}

/// <summary>
/// Describes a configuration property definition.
///
/// A configuration property definition contains only metadata,
/// such as display information, security requirements, value type,
/// and default value. Actual values are stored separately and may
/// exist at different scopes (System, Company, User).
///
/// Examples:
///     Trade.DefaultPaymentMethodId
///     Trade.DefaultPaymentTermId
///     Ui.Theme
///     Sales.LineGridLayout
/// </summary>
public class ConfigPropertyDef: BaseDef
{
    /// <summary>
    /// Gets or sets the UI group name used when displaying
    /// configuration properties in configuration dialogs.
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// Gets or sets the minimum user level required
    /// to view or modify this configuration property.
    /// </summary>
    public UserLevel SecurityLevel { get; set; }
    /// <summary>
    /// Gets or sets the value kind of the property.
    /// Determines how the value is edited, validated,
    /// serialized and displayed.
    /// </summary>
    public ConfigValueKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the default value used when
    /// no stored value exists.
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// Gets or sets an optional type name.
    ///
    /// Usage depends on the Kind:
    ///
    /// Lookup:
    ///     Lookup source name.
    ///
    /// Enum:
    ///     Enum lookup name or enum type name.
    ///
    /// Object:
    ///     CLR type name or serializer key.
    ///
    /// Ignored for simple scalar value kinds.
    /// </summary>
    public string TypeName { get; set; }
    /// <summary>
    /// A callback that is invoked when a value is applied to this property.
    /// </summary>
    public Action<ConfigPropertyDef, string> ApplyValueFunc { get; set; }
}

/// <summary>
/// Provides access to application configuration values.
///
/// Configuration property definitions are registered in code and stored
/// in the Properties collection. Actual values are stored separately and
/// may exist at different scopes:
///
/// - User
/// - Company
/// - System
///
/// Effective values are resolved using the following order:
///
/// User -> Company -> System -> DefaultValue
///
/// Company values are resolved using DbConfig.CompanyId.
/// User values are resolved using the current application user.
/// </summary>
static public class Config
{
    static SqlStore fStore;
    static SysConfigModule fModule;

    // ● private
    static T JsonToObject<T>(ConfigPropertyDef Def, string JsonText) where T: class
    {
        if (string.IsNullOrWhiteSpace(JsonText))
            return null;

        string ClassName = string.IsNullOrWhiteSpace(Def.TypeName) ? typeof(T).FullName : Def.TypeName;
        T Result = TypeStore.CreateInstance<T>(ClassName);
        Json.PopulateObject(Result, JsonText);
        return Result;
    }
    
    static string NormalizeOwnerKey(ConfigScope Scope, string OwnerKey)
    {
        return Scope == ConfigScope.System ? "" : OwnerKey;
    }
    static ConfigPropertyDef GetDef(string Name)
    {
        return DataRegistry.ConfigProperties.Get(Name);
    }
    static string SelectStoredValue(ConfigPropertyDef Def, ConfigScope Scope, string OwnerKey)
    {
        string FieldName = Def.Kind == ConfigValueKind.Memo || Def.Kind == ConfigValueKind.Object ? "TextValue" : "Value";
        string SqlText = $"""
                          select {FieldName}
                          from SYS_CONFIG
                          where ScopeId = :ScopeId
                            and OwnerKey = :OwnerKey
                            and Name = :Name
                          """;

        object Result = Module.Store.SelectResult(SqlText, null, new Dictionary<string, object>()
        {
            ["ScopeId"] = (int)Scope,
            ["OwnerKey"] = OwnerKey,
            ["Name"] = Def.Name,
        });

        return Result == null || Result == DBNull.Value ? null : Result.ToString();
    }
    static void InsertOrUpdateStoredValue(ConfigPropertyDef Def, string Value, ConfigScope Scope, string OwnerKey)
    {
        bool IsTextValue = Def.Kind == ConfigValueKind.Memo || Def.Kind == ConfigValueKind.Object;
        string SqlText = """
                         select Id
                         from SYS_CONFIG
                         where ScopeId = :ScopeId
                           and OwnerKey = :OwnerKey
                           and Name = :Name
                         """;

        object Id = Module.Store.SelectResult(SqlText, null, new Dictionary<string, object>()
        {
            ["ScopeId"] = (int)Scope,
            ["OwnerKey"] = OwnerKey,
            ["Name"] = Def.Name,
        });

        if (Sys.IsNull(Id))
            Module.Insert();
        else
            Module.Edit(Id);

        DataRow Row = Module.CurrentRow;
        Row["ScopeId"] = (int)Scope;
        Row["OwnerKey"] = OwnerKey;
        Row["Name"] = Def.Name;
        Row["Value"] = IsTextValue ? DBNull.Value : Value;
        Row["TextValue"] = IsTextValue ? Value : DBNull.Value;
        Row["ModifiedAt"] = Module.Store.GetServerDateTime();
        Row["ModifiedBy"] = Sys.GetCurrentAppUserId();

        Module.Commit();
    }
    
    // ● public
    /// <summary>
    /// Returns the effective value of a configuration property.
    /// Resolution order: User, Company, System, DefaultValue.
    /// <para>This method returns the value associated with the current Company and current User.</para>
    /// </summary>
    static public string GetValue(string Name)
    {
        return GetValue(Name, DbConfig.CompanyId.ToString(), Sys.GetCurrentAppUserName());
    }
    /// <summary>
    /// Returns the effective value of a configuration property using the specified company and user context.
    /// Resolution order: User, Company, System, DefaultValue.
    /// </summary>
    static public string GetValue(string Name, string CompanyId, string UserName)
    {
        string Result;
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            Result = GetValue(Name, ConfigScope.User, UserName);
            if (Result != null)
                return Result;
        }
        if (!string.IsNullOrWhiteSpace(CompanyId))
        {
            Result = GetValue(Name, ConfigScope.Company, CompanyId);
            if (Result != null)
                return Result;
        }
        Result = GetValue(Name, ConfigScope.System, "");
        if (Result != null)
            return Result;
        return GetDef(Name).DefaultValue;
    }
    /// <summary>
    /// Returns the value stored at the specified scope.
    /// No effective value resolution takes place.
    /// </summary>
    static public string GetValue(string Name, ConfigScope Scope, string OwnerKey)
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        return SelectStoredValue(Def, Scope, OwnerKey);
    }
    
    /// <summary>
    /// Returns the effective object value of a configuration property.
    /// Resolution order: User, Company, System, DefaultValue.
    /// </summary>
    static public T GetObjectValue<T>(string Name) where T: class
    {
        return GetObjectValue<T>(Name, DbConfig.CompanyId.ToString(), Sys.GetCurrentAppUserName());
    }
    /// <summary>
    /// Returns the effective object value of a configuration property using the specified company and user context.
    /// Resolution order: User, Company, System, DefaultValue.
    /// </summary>
    static public T GetObjectValue<T>(string Name, string CompanyId, string UserName) where T: class
    {
        ConfigPropertyDef Def = GetDef(Name);
        string JsonText = GetValue(Name, CompanyId, UserName);
        return JsonToObject<T>(Def, JsonText);
    }
    /// <summary>
    /// Returns the object value stored at the specified scope.
    /// No effective value resolution takes place.
    /// </summary>
    static public T GetObjectValue<T>(string Name, ConfigScope Scope, string OwnerKey) where T: class
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        string JsonText = SelectStoredValue(Def, Scope, OwnerKey);
        return JsonToObject<T>(Def, JsonText);
    }
    
    /// <summary>
    /// Stores a value at the specified scope.
    /// Creates or updates the corresponding configuration entry.
    /// </summary>
    static public void SetValue(string Name, string Value, ConfigScope Scope, string OwnerKey)
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        InsertOrUpdateStoredValue(Def, Value, Scope, OwnerKey);
        
        Def.ApplyValueFunc?.Invoke(Def, Value);
    }
    /// <summary>
    /// Stores a value associated with the current User.
    /// Creates or updates the corresponding configuration entry.
    /// </summary>
    static public void SetUserValue(string Name, string Value)
    {
        ConfigScope Scope = ConfigScope.User;
        string OwnerKey = Sys.GetCurrentAppUserName();
        SetValue(Name, Value, Scope, OwnerKey);
    }

    // ● properties
    /// <summary>
    /// Gets the SQL store used by the Config system.
    /// </summary>
    static public SqlStore Store => fStore ??= SqlStores.CreateDefaultSqlStore();
    static public SysConfigModule Module => fModule ??= DataRegistry.CreateModule("SysConfig") as SysConfigModule;
}
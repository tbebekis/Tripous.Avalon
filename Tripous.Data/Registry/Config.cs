/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Specifies the target organizational layer or visibility context for a configuration property.
/// </summary>
[TypeStore]
public enum ConfigScope
{
    /// <summary>
    /// Global application-wide configuration layer.
    /// </summary>
    System = 0,
    /// <summary>
    /// Tenant or company-specific configuration layer.
    /// </summary>
    Company = 1,
    /// <summary>
    /// End-user personalized configuration layer.
    /// </summary>
    User = 2
}

/// <summary>
/// Specifies the configuration scopes supported by a configuration property definition.
/// </summary>
[Flags]
public enum ConfigScopeFlags
{
    /// <summary>
    /// No configuration scope is supported.
    /// </summary>
    None = 0,
    /// <summary>
    /// The System configuration scope is supported.
    /// </summary>
    System = 1,
    /// <summary>
    /// The Company configuration scope is supported.
    /// </summary>
    Company = 2,
    /// <summary>
    /// The User configuration scope is supported.
    /// </summary>
    User = 4,
    /// <summary>
    /// All configuration scopes are supported.
    /// </summary>
    All = System | Company | User
}

/// <summary>
/// Specifies the structural classification or serialization type of a configuration entry value.
/// </summary>
[TypeStore]
public enum ConfigValueKind
{
    /// <summary>
    /// Text or string primitive scalar value.
    /// </summary>
    String = 0,
    /// <summary>
    /// Whole 32-bit signed integer numeric value.
    /// </summary>
    Integer = 1,
    /// <summary>
    /// Boolean conditional true or false status flag.
    /// </summary>
    Boolean = 2,
    /// <summary>
    /// Calendar date value representation.
    /// </summary>
    Date = 3,
    /// <summary>
    /// Daily time-of-day clock measurement representation.
    /// </summary>
    Time = 4,
    /// <summary>
    /// Double precision binary floating-point representation.
    /// </summary>
    Double = 5,
    /// <summary>
    /// Precise fixed-point decimal financial numeric value.
    /// </summary>
    Decimal = 6,
    /// <summary>
    /// Relational key identifier linking to external lookup source tables.
    /// </summary>
    Lookup = 7,
    /// <summary>
    /// Discrete numeric token tracking an external registered code list.
    /// </summary>
    Enum = 8,
    /// <summary>
    /// Large multi-line plain text or structural data payload.
    /// </summary>
    Memo = 50,
    /// <summary>
    /// Complex structured custom object serialized descriptor mapping.
    /// </summary>
    Object = 100,
}

/// <summary>
/// Describes a configuration property definition.
/// <para>
/// A configuration property definition contains only metadata,
/// such as display information, security requirements, value type,
/// and default value. Actual values are stored separately and may
/// exist at different scopes (System, Company, User).
/// </para>
/// <para>
/// Examples:
/// * Trade.DefaultPaymentMethodId
/// * Trade.DefaultPaymentTermId
/// * Ui.Theme
/// * Sales.LineGridLayout
/// </para>
/// </summary>
public class ConfigPropertyDef : BaseDef
{
    // ● public methods
    /// <summary>
    /// Returns true when this definition supports the specified configuration scope.
    /// </summary>
    public bool SupportsScope(ConfigScope Scope)
    {
        ConfigScopeFlags Flag = Scope switch
        {
            ConfigScope.System => ConfigScopeFlags.System,
            ConfigScope.Company => ConfigScopeFlags.Company,
            ConfigScope.User => ConfigScopeFlags.User,
            _ => ConfigScopeFlags.None
        };
        return (Scopes & Flag) == Flag;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the UI group name used when displaying configuration properties in configuration dialogs.
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// Gets or sets the minimum user level required to view or modify this configuration property.
    /// </summary>
    public UserLevel SecurityLevel { get; set; }
    /// <summary>
    /// Gets or sets the value kind of the property. Determines how the value is edited, validated, serialized and displayed.
    /// </summary>
    public ConfigValueKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the default value used when no stored value exists.
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// Gets or sets the configuration scopes where this property is visible and editable.
    /// </summary>
    public ConfigScopeFlags Scopes { get; set; } = ConfigScopeFlags.All;
    /// <summary>
    /// Gets or sets an optional type name mapping descriptor based on property classification rules.
    /// <para>
    /// Usage depends on the Kind:
    /// * Lookup: Lookup source name.
    /// * Enum: Enum lookup name or enum type name.
    /// * Object: CLR type name or serializer key.
    /// Ignored for simple scalar value kinds.
    /// </para>
    /// </summary>
    public string TypeName { get; set; }
    /// <summary>
    /// Gets or sets the class name of a desktop editor used for complex configuration values.
    /// </summary>
    public string EditorClassName { get; set; }
    /// <summary>
    /// Gets or sets a callback that is invoked when a value is applied to this property.
    /// </summary>
    public Action<ConfigPropertyDef, string> ApplyValueFunc { get; set; }
}

/// <summary>
/// Provides access to application configuration values.
/// <para>
/// Configuration property definitions are registered in code and stored
/// in the Properties collection. Actual values are stored separately and
/// may exist at different scopes:
/// * User
/// * Company
/// * System
/// </para>
/// <para>
/// Effective values are resolved using the following order:
/// User -> Company -> System -> DefaultValue
/// </para>
/// <para>
/// Company values are resolved using DbConfig.CompanyId.
/// User values are resolved using the current application user.
/// </para>
/// </summary>
static public class Config
{
    // ● constants
    /// <summary>
    /// The configuration property name used as the row limit for DataModule list SELECTs.
    /// </summary>
    public const string SSelectListRowLimit = "SelectListRowLimit";
    /// <summary>
    /// The configuration property name used to enable the DataForm FactBox pane.
    /// </summary>
    public const string SShowDataFormFactBoxPane = "ShowDataFormFactBoxPane";
    /// <summary>
    /// The configuration property name used to warn before executing non-SELECT SQL statements.
    /// </summary>
    public const string SShowWarningOnExecStatements = "ShowWarningOnExecStatements";
    /// <summary>
    /// The configuration property name used to auto-insert missing string resource keys.
    /// </summary>
    public const string SSysStrResAutoInsertMissingKeys = "SysStrRes.AutoInsertMissingKeys";

    // ● private fields
    static SqlStore fStore;
    static SysConfigModule fModule;

    // ● private methods
    /// <summary>
    /// Deserializes a json string token matrix into a strongly-typed instance layout of type <typeparamref name="T"/>.
    /// </summary>
    static T JsonToObject<T>(ConfigPropertyDef Def, string JsonText) where T : class
    {
        if (string.IsNullOrWhiteSpace(JsonText))
            return null;

        string ClassName = string.IsNullOrWhiteSpace(Def.TypeName) ? typeof(T).FullName : Def.TypeName;
        T Result = TypeStore.CreateInstance<T>(ClassName);
        Json.PopulateObject(Result, JsonText);
        return Result;
    }
    /// <summary>
    /// Normalizes the specific context descriptor tracking key depending on operational scope visibility.
    /// </summary>
    static string NormalizeOwnerKey(ConfigScope Scope, string OwnerKey)
    {
        return Scope == ConfigScope.System ? "" : OwnerKey;
    }
    /// <summary>
    /// Fetches the structural registration descriptor mapping tracking rules for the identified config record name.
    /// </summary>
    static ConfigPropertyDef GetDef(string Name)
    {
        return DataRegistry.ConfigProperties.Get(Name);
    }
    /// <summary>
    /// Executes targeting select queries to read specific config values directly from backend system table structures.
    /// </summary>
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
    /// <summary>
    /// Checks existence states to perform direct transaction script insert updates on persistent data row instances.
    /// </summary>
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

    // ● static public methods
    /// <summary>
    /// Returns the effective value of a configuration property.
    /// <para>Resolution order: User, Company, System, DefaultValue.</para>
    /// <para>This method returns the value associated with the current Company and current User.</para>
    /// </summary>
    static public string GetValue(string Name)
    {
        return GetValue(Name, DbConfig.CompanyId.ToString(), Sys.GetCurrentAppUserName());
    }
    /// <summary>
    /// Returns the effective value of a configuration property using the specified company and user context.
    /// <para>Resolution order: User, Company, System, DefaultValue.</para>
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
    /// Returns the value stored at the specified scope. No effective value resolution takes place.
    /// </summary>
    static public string GetValue(string Name, ConfigScope Scope, string OwnerKey)
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        return SelectStoredValue(Def, Scope, OwnerKey);
    }
    /// <summary>
    /// Returns the effective object value of a configuration property.
    /// <para>Resolution order: User, Company, System, DefaultValue.</para>
    /// </summary>
    static public T GetObjectValue<T>(string Name) where T : class
    {
        return GetObjectValue<T>(Name, DbConfig.CompanyId.ToString(), Sys.GetCurrentAppUserName());
    }
    /// <summary>
    /// Returns the effective object value of a configuration property using the specified company and user context.
    /// <para>Resolution order: User, Company, System, DefaultValue.</para>
    /// </summary>
    static public T GetObjectValue<T>(string Name, string CompanyId, string UserName) where T : class
    {
        ConfigPropertyDef Def = GetDef(Name);
        string JsonText = GetValue(Name, CompanyId, UserName);
        return JsonToObject<T>(Def, JsonText);
    }
    /// <summary>
    /// Returns the object value stored at the specified scope. No effective value resolution takes place.
    /// </summary>
    static public T GetObjectValue<T>(string Name, ConfigScope Scope, string OwnerKey) where T : class
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        string JsonText = SelectStoredValue(Def, Scope, OwnerKey);
        return JsonToObject<T>(Def, JsonText);
    }
    /// <summary>
    /// Stores a value at the specified scope, creating or updating the corresponding configuration entry.
    /// </summary>
    static public void SetValue(string Name, string Value, ConfigScope Scope, string OwnerKey)
    {
        ConfigPropertyDef Def = GetDef(Name);
        OwnerKey = NormalizeOwnerKey(Scope, OwnerKey);
        InsertOrUpdateStoredValue(Def, Value, Scope, OwnerKey);
        
        Def.ApplyValueFunc?.Invoke(Def, Value);
    }
    /// <summary>
    /// Stores a value associated with the current User, creating or updating the corresponding configuration entry.
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
    /// <summary>
    /// Gets the underlying configuration module provider instance layer.
    /// </summary>
    static public SysConfigModule Module => fModule ??= DataRegistry.CreateModule("SysConfig") as SysConfigModule;
}

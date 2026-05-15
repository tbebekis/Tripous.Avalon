namespace Tripous.Data;

/// <summary>
/// Registry of the data layer.
/// </summary>
static public class DataRegistry
{
    // ● private
    static ModuleDef AddModuleInternal(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false)
    {
        ModuleDef Result = new();
        Result.Name = Name;
        Result.GuidOids = DbConfig.GuidOids;
        Result.TitleKey = TitleKey;
        Result.ClassName = !string.IsNullOrWhiteSpace(ClassName)? ClassName: typeof(DataModule).FullName;
        Result.Table.Name = Name;
        Result.IsSingleSelect = IsSingleSelect;

        SelectDef SelectDef = new();
        SelectDef.Name = Sys.DEFAULT;
        SelectDef.SqlText = !string.IsNullOrWhiteSpace(ListSelectSql) ? ListSelectSql : $"select * from {Name}";
        Result.SelectList.Add(SelectDef);
        
        DataRegistry.Modules.Add(Result);
        return Result;
    }
    static ModuleDef AddLookupListModuleInternal(string TableName, string Name, string TitleKey)
    {
        ModuleDef Result = AddModule(Name: Name, TitleKey: TitleKey, IsSingleSelect: true);
        Result.UseFilters = false;
        
        SelectDef SelectDef = Result.SelectList[0];
        SelectDef.DisplayLabels["Name"] = Name;
        
        TableDef Table = Result.Table;
        Table.Name = TableName;
        Table.KeyField = "Id";

        if (Result.GuidOids)
            Table.AddStringId("Id", FieldFlags.Required | FieldFlags.Visible);  
        else
            Table.AddIntegerId("Id", FieldFlags.Required | FieldFlags.Visible);  
        
        Table.AddString("Name", 96, TitleKey: "Name", Flags: FieldFlags.Required | FieldFlags.Visible);
 
        return Result;
    }
    static void CheckModule(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(ModuleDef)}. No '{nameof(Name)}' is provided.");
        if (DataRegistry.Modules.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(ModuleDef)}. '{Name}' is already registered.");
    }

    static LookupSource AddLookupSourceInternal(string Name, Type EnumType, string TableName, string SqlText, bool UseNullItem)
    {
        if (EnumType == null && string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(SqlText))
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. No '{nameof(EnumType)}' or  '{nameof(TableName)}' or  '{nameof(SqlText)}' is provided.");

        string EnumTypeName = EnumType != null ? EnumType.FullName : null;
        
        if (string.IsNullOrWhiteSpace(EnumTypeName))
        {
            if (string.IsNullOrWhiteSpace(SqlText) && string.IsNullOrWhiteSpace(TableName))
                TableName = Name;

            if (string.IsNullOrWhiteSpace(SqlText))
                SqlText = $"select * from {TableName}";
        }
        
        LookupSource Result = new();
        Result.Name = Name;
        Result.UseNullItem = UseNullItem;
        if (EnumType != null)
            Result.EnumTypeName = EnumType.FullName;
        Result.TableName = TableName;
        Result.SqlText = SqlText;
        DataRegistry.LookupSources.Add(Result);
        return Result;
    }
    static void CheckLookupSource(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. No '{nameof(Name)}' is provided.");
        if (LookupSources.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. '{Name}' is already registered.");
    }
    static void CheckLookupSource(string Name, Type EnumType)
    {
        CheckLookupSource(Name);
        if (EnumType == null)
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. No '{nameof(EnumType)}' is provided.");
        if (!EnumType.IsEnum)
            throw new TripousDataException($"Cannot add a {nameof(LookupSource)}. Type {EnumType.FullName} is not an enum type");
    }
    static void CheckLookupSourceWithTableName(string Name, string TableName)
    {
        CheckLookupSource(Name);
        if (string.IsNullOrWhiteSpace(TableName))
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. No '{nameof(TableName)}' is provided.");
    }
    static void CheckLookupSourceWithSql(string Name, string SqlText)
    {
        CheckLookupSource(Name);
        if (string.IsNullOrWhiteSpace(SqlText))
            throw new TripousException($"Cannot add a {nameof(LookupSource)}. No '{nameof(SqlText)}' is provided.");
    }

    static void CheckLocator(string Name, string KeyField)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. No '{nameof(Name)}' is provided.");
        if (Locators.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. '{Name}' is already registered.");
    }
    static LocatorDef AddLocatorInternal(string Name, string SourceTableName, string KeyField, string ClassName = null)
    {
        LocatorDef Result = new();
        Result.Name = Name;
        Result.SourceTableName = SourceTableName;
        Result.KeyField = KeyField;
        Result.ClassName = ClassName;
        Locators.Add(Result);
        return Result;
    }

    
    // ● modules
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public ModuleDef AddModule(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false)
    {
        CheckModule(Name);
        ModuleDef Result = AddModuleInternal(Name, TitleKey, ClassName, ListSelectSql, IsSingleSelect); 
        return Result;
    }
    /// <summary>
    /// Adds a module definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public ModuleDef AddOrGetModule(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false)
    {
        ModuleDef Result = Modules.Find(Name);
        if (Result == null)
            Result = AddModuleInternal(Name, TitleKey, ClassName, ListSelectSql, IsSingleSelect); 
        return Result;
    }
 
    // ● lookup list modules
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string Name) => AddLookupListModule(Name, Name, null);
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string Name, string TitleKey) => AddLookupListModule(Name, Name, TitleKey);
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string TableName, string Name, string TitleKey)
    {
        CheckModule(Name);
        ModuleDef Result = AddLookupListModuleInternal(TableName, Name, TitleKey);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public ModuleDef AddOrGetLookupListModule(string TableName, string Name, string TitleKey)
    {
        ModuleDef Result = Modules.Find(Name);
        if (Result == null)
            Result = AddLookupListModuleInternal(TableName, Name, TitleKey); 
        return Result;
    }
    
    // ● lookup sources
    /// <summary>
    /// Adds a lookup source.
    /// <para>The <see cref="EnumType"/> is used as the source.</para>
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupSource AddLookupSource(Type EnumType, bool UseNullItem = false) => AddLookupSource(EnumType.FullName, EnumType, UseNullItem);
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupSource AddLookupSource(string Name, Type EnumType, bool UseNullItem = false)
    {
        CheckLookupSource(Name, EnumType);
        LookupSource Result = AddLookupSourceInternal(Name, EnumType, TableName: null, SqlText: null, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupSource AddLookupSourceWithTableName(string Name, string TableName = null, bool UseNullItem = false)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            TableName = Name;
        
        CheckLookupSourceWithTableName(Name, TableName);
        LookupSource Result = AddLookupSourceInternal(Name, EnumType: null, TableName: TableName, SqlText: null, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupSource AddLookupSourceWithSql(string Name, string SqlText = null, bool UseNullItem = false)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            SqlText = $"select * from {Name}";
        
        CheckLookupSourceWithSql(Name, SqlText);
        LookupSource Result = AddLookupSourceInternal(Name, EnumType: null, TableName: null, SqlText: SqlText, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupSource AddOrGetLookupSource(string Name, Type EnumType, string TableName, string SqlText, bool UseNullItem)
    {
        LookupSource Result = LookupSources.Find(Name);
        if (Result == null)
            Result = AddLookupSourceInternal(Name, EnumType, TableName, SqlText, UseNullItem);
        return Result;
    }
    
    // ● locators
    /// <summary>
    /// Adds a locator definition.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LocatorDef AddLocator(string Name, string SourceTableName, string KeyField, string ClassName = null)
    {
        CheckLocator(Name, KeyField);
        LocatorDef Result = AddLocatorInternal(Name, SourceTableName, KeyField, ClassName);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LocatorDef AddOrGetLocator(string Name, string SourceTableName, string KeyField, string ClassName = null)
    {
        LocatorDef Result = Locators.Find(Name);
        if (Result == null)
            Result = AddLocatorInternal(Name, SourceTableName, KeyField, ClassName);
        return Result;
    }

    // ● create 
    /// <summary>
    /// Creates and returns a <see cref="DataModule"/> based on its registered name.
    /// </summary>
    static public DataModule CreateModule(string Name) => Modules.Get(Name).Create();
    
    // ● properties
    /// <summary>
    /// The list of locator definitions.
    /// </summary>
    static public DefList<LocatorDef> Locators { get; } = new();
    /// <summary>
    /// The list of module definitions
    /// </summary>
    static public DefList<ModuleDef> Modules { get; } = new();
    /// <summary>
    /// The list of lookup sources definitions
    /// </summary>
    static public DefList<LookupSource> LookupSources { get; } = new();
}


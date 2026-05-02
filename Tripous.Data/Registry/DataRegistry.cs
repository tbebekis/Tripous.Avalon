namespace Tripous.Data;

static public class DataRegistry
{
    // ● modules
    static public ModuleDef AddModule(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousArgumentNullException(nameof(Name));
        if (DataRegistry.Modules.Contains(Name))
            throw new TripousException($"{nameof(ModuleDef)} '{Name}' is already registered.");

        ModuleDef Result = new();
        Result.Name = Name;
        Result.GuidOids = SysConfig.GuidOids;
        Result.TitleKey = !string.IsNullOrWhiteSpace(TitleKey) ? TitleKey : Name.ToPlural();
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
 
    // ● lookup list modules
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string Name) => AddLookupListModule(Name, Name, null);
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string Name, string TitleKey) => AddLookupListModule(Name, Name, TitleKey);
    /// <summary>
    /// A list module with just Id and Name as fields in its table.
    /// <para>NOTE: The name of the module is the list TableName</para>
    /// </summary>
    static public ModuleDef AddLookupListModule(string TableName, string Name, string TitleKey)
    {
        ModuleDef Result = AddModule(Name: Name, TitleKey: TitleKey, IsSingleSelect: true);
        
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
    
    // ● lookup sources
    /// <summary>
    /// Adds a lookup source.
    /// <para>The <see cref="EnumType"/> is used as the source.</para>
    /// </summary>
    static public LookupSource AddLookupSource(Type EnumType, bool UseNullItem = false) => AddLookupSource(EnumType.FullName, EnumType, UseNullItem);
    /// <summary>
    /// Adds a lookup source.
    /// <para>If <see cref="EnumType"/> is not null, the it is used as the source.</para>
    /// <para>Else the <see cref="Name"/> is used as the <see cref="LookupSource.TableName"/></para>
    /// </summary>
    static public LookupSource AddLookupSource(string Name, Type EnumType = null, bool UseNullItem = false)
    {
        if (EnumType != null && !EnumType.IsEnum)
            throw new TripousDataException($"Type {EnumType.FullName} is not an enum type");

        LookupSource Result = new();
        Result.Name = Name;
        Result.UseNullItem = UseNullItem;
        if (EnumType != null)
            Result.EnumTypeName = EnumType.FullName;
        DataRegistry.LookupSources.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds a lookup source.
    /// </summary>
    static public LookupSource AddLookupSourceWithTableName(string Name, string TableName, bool UseNullItem = false)
    {
        LookupSource Result = new();
        Result.Name = Name;
        Result.UseNullItem = UseNullItem;
        Result.TableName = TableName;
        DataRegistry.LookupSources.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds a lookup source.
    /// </summary>
    static public LookupSource AddLookupSourceWithSql(string Name, string SqlText, bool UseNullItem = false)
    {
        LookupSource Result = new();
        Result.Name = Name;
        Result.UseNullItem = UseNullItem;
        Result.SqlText = SqlText;
        DataRegistry.LookupSources.Add(Result);
        return Result;
    }
 
    // ● create module
    static public DataModule CreateModule(string Name) => Modules.Get(Name).Create();
    
    // ● properties
    static public DefList<LocatorDef> Locators { get; } = new();
    static public DefList<ModuleDef> Modules { get; } = new();
    static public DefList<LookupSource> LookupSources { get; } = new();
}


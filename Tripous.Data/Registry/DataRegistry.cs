/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

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
            Table.AddStringId("Id", FieldFlags.Required | FieldFlags.Hidden);
        else
            Table.AddIntegerId("Id", FieldFlags.Required | FieldFlags.Hidden);
        
        Table.AddString("Name", 96, TitleKey: "Name", Flags: FieldFlags.Required);
 
        return Result;
    }
    static void CheckModule(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(ModuleDef)}. No '{nameof(Name)}' is provided.");
        if (DataRegistry.Modules.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(ModuleDef)}. '{Name}' is already registered.");
    }

    static LookupDef AddLookupInternal(string Name, Type EnumType, string TableName, string SqlText, string FormName, bool UseNullItem)
    {
        if (EnumType == null && string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(SqlText))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(EnumType)}' or  '{nameof(TableName)}' or  '{nameof(SqlText)}' is provided.");

        string EnumTypeName = EnumType != null ? EnumType.FullName : null;
        
        if (string.IsNullOrWhiteSpace(EnumTypeName))
        {
            if (string.IsNullOrWhiteSpace(SqlText) && string.IsNullOrWhiteSpace(TableName))
                TableName = Name;

            if (string.IsNullOrWhiteSpace(SqlText))
                SqlText = $"select * from {TableName}";
        }
        
        LookupDef Result = new();
        Result.Name = Name;
        Result.UseNullItem = UseNullItem;
        if (EnumType != null)
            Result.EnumTypeName = EnumType.FullName;
        Result.TableName = TableName;
        Result.SqlText = SqlText;
        Result.Form = FormName;
        DataRegistry.Lookups.Add(Result);
        return Result;
    }
    static void CheckLookup(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(Name)}' is provided.");
        if (Lookups.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. '{Name}' is already registered.");
    }
    static void CheckLookup(string Name, Type EnumType)
    {
        CheckLookup(Name);
        if (EnumType == null)
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(EnumType)}' is provided.");
        if (!EnumType.IsEnum)
            throw new TripousDataException($"Cannot add a {nameof(LookupDef)}. Type {EnumType.FullName} is not an enum type");
    }
    static void CheckLookupWithTableName(string Name, string TableName)
    {
        CheckLookup(Name);
        if (string.IsNullOrWhiteSpace(TableName))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(TableName)}' is provided.");
    }
    static void CheckLookupWithSql(string Name, string SqlText)
    {
        CheckLookup(Name);
        if (string.IsNullOrWhiteSpace(SqlText))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(SqlText)}' is provided.");
    }

    static void CheckLocator(string Name, string KeyField)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. No '{nameof(Name)}' is provided.");
        if (Locators.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. '{Name}' is already registered.");
    }
    static LocatorDef AddLocatorInternal(string Name, string SourceTableName, string KeyField, string ClassName, string FormName)
    {
        LocatorDef Result = new();
        Result.Name = Name;
        Result.SourceTableName = SourceTableName;
        Result.KeyField = KeyField;
        Result.ClassName = ClassName;
        Result.Form = FormName;
        Locators.Add(Result);
        return Result;
    }
    
    static void CheckCodeProvider(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(CodeProviderDef)}. No '{nameof(Name)}' is provided.");
        if (CodeProviders.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(CodeProviderDef)}. '{Name}' is already registered.");
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
    
    // ● lookups - enum types
    /// <summary>
    /// Adds a lookup source.
    /// <para>The <see cref="EnumType"/> is used as the source.</para>
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupDef AddLookupSource(Type EnumType, bool UseNullItem = false) => AddLookupSource(EnumType.FullName, EnumType, UseNullItem);
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupDef AddLookupSource(string Name, Type EnumType, bool UseNullItem = false)
    {
        CheckLookup(Name, EnumType);
        LookupDef Result = AddLookupInternal(Name, EnumType, TableName: null, SqlText: null, FormName: null, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupDef AddOrGetLookupSource(Type EnumType, bool UseNullItem = false)
    {
        return AddOrGetLookup(EnumType.FullName, EnumType, TableName: "", SqlText: "", FormName: "", UseNullItem);
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupDef AddOrGetLookupSource(string Name, Type EnumType, bool UseNullItem = false)
    {
        return AddOrGetLookup(Name, EnumType, TableName: "", SqlText: "", FormName: "", UseNullItem);
    }
    
    // ● lookups - with table name
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupDef AddLookupWithTableName(string Name, string TableName = null, string FormName = null, bool UseNullItem = false)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            TableName = Name;
        
        CheckLookupWithTableName(Name, TableName);
        LookupDef Result = AddLookupInternal(Name, EnumType: null, TableName: TableName, SqlText: null, FormName: FormName, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupDef AddOrGetLookupWithTableName(string Name, string TableName = null, string FormName = null, bool UseNullItem = false)
    {
        return AddOrGetLookup(Name, EnumType: null, TableName: TableName, SqlText: "", FormName: FormName, UseNullItem);
    }
    
    // ● lookups - with SELECT Sql
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupDef AddLookupWithSql(string Name, string SqlText = null, string FormName = null, bool UseNullItem = false)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            SqlText = $"select * from {Name}";
        
        CheckLookupWithSql(Name, SqlText);
        LookupDef Result = AddLookupInternal(Name, EnumType: null, TableName: null, SqlText: SqlText, FormName: FormName, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupDef AddOrGetLookupWithSql(string Name, string SqlText = null, string FormName = null, bool UseNullItem = false)
    {
        return AddOrGetLookup(Name, EnumType: null, TableName: "", SqlText: SqlText, FormName: FormName, UseNullItem);
    }
    
    // ● lookups - add or get
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LookupDef AddOrGetLookup(string Name, Type EnumType, string TableName, string SqlText, string FormName, bool UseNullItem)
    {
        LookupDef Result = Lookups.Find(Name);
        if (Result == null)
            Result = AddLookupInternal(Name, EnumType, TableName, SqlText, FormName: FormName, UseNullItem);
        return Result;
    }
    
    // ● locators
    /// <summary>
    /// Adds a locator definition.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LocatorDef AddLocator(string Name, string SourceTableName, string KeyField, string ClassName = null, string FormName = null)
    {
        CheckLocator(Name, KeyField);
        LocatorDef Result = AddLocatorInternal(Name, SourceTableName, KeyField, ClassName, FormName);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public LocatorDef AddOrGetLocator(string Name, string SourceTableName, string KeyField, string ClassName = null, string FormName = null)
    {
        LocatorDef Result = Locators.Find(Name);
        if (Result == null)
            Result = AddLocatorInternal(Name, SourceTableName, KeyField, ClassName, FormName);
        return Result;
    }
    /// <summary>
    /// Locators are not part of module, so we need a way to update references.
    /// </summary>
    static public void UpdateLocatorReferences()
    {
        foreach (LocatorDef LocatorDef in Locators)
            LocatorDef.UpdateReferences();
    }

    // ● code providers
    /// <summary>
    /// Adds a definition.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public CodeProviderDef AddCodeProvider(string Name)
    {
        CheckCodeProvider(Name);
        CodeProviderDef Result = new CodeProviderDef() { Name = Name };
        CodeProviders.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public CodeProviderDef AddOrGetCodeProvider(string Name)
    {
        CodeProviderDef Result = CodeProviders.Find(Name);
        if (Result == null)
            Result = AddCodeProvider(Name);
        return Result;
    }
    
    // ● create 
    /// <summary>
    /// Creates and returns a <see cref="DataModule"/> based on its registered name.
    /// </summary>
    static public DataModule CreateModule(string Name, bool InitializeToo = true) => Modules.Get(Name).Create(InitializeToo);
    
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
    static public DefList<LookupDef> Lookups { get; } = new();
    /// <summary>
    /// The list of code providers.
    /// </summary>
    static public DefList<CodeProviderDef> CodeProviders { get; } = new();
}

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
    static ModuleDef AddModuleInternal(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false, UserLevel SecurityLevel = UserLevel.None)
    {
        ModuleDef Result = new();
        Result.Name = Name;
        Result.GuidOids = DbConfig.GuidOids;
        Result.TitleKey = TitleKey;
        Result.ClassName = !string.IsNullOrWhiteSpace(ClassName)? ClassName: typeof(DataModule).FullName;
        Result.Table.Name = Name;
        Result.IsSingleSelect = IsSingleSelect;
        Result.SecurityLevel = SecurityLevel;

        SelectDef SelectDef = new();
        SelectDef.Name = Sys.DEFAULT;
        SelectDef.SqlText = !string.IsNullOrWhiteSpace(ListSelectSql) ? ListSelectSql : $"select * from {Name}";
        Result.SelectList.Add(SelectDef);
        
        DataRegistry.Modules.Add(Result);
        return Result;
    }
    static void UpdateModule(ModuleDef ModuleDef, string TitleKey, string ClassName, string ListSelectSql, bool? IsSingleSelect, UserLevel? SecurityLevel)
    {
        if (TitleKey != null)
            ModuleDef.TitleKey = TitleKey;
        if (ClassName != null)
            ModuleDef.ClassName = ClassName;
        if (ListSelectSql != null)
            ModuleDef.SelectList[0].SqlText = ListSelectSql;
        if (IsSingleSelect.HasValue)
            ModuleDef.IsSingleSelect = IsSingleSelect.Value;
        if (SecurityLevel.HasValue)
            ModuleDef.SecurityLevel = SecurityLevel.Value;
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

    static LookupDef AddLookupInternal(string Name, Type EnumType, string TableName, string SqlText, string ClassName, string FormName, bool UseNullItem)
    {
        if (EnumType == null && string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(SqlText) && string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(EnumType)}' or  '{nameof(TableName)}' or  '{nameof(SqlText)}' or  '{nameof(ClassName)}' is provided.");

        string EnumTypeName = EnumType != null ? EnumType.FullName : null;
        
        if (string.IsNullOrWhiteSpace(EnumTypeName) && string.IsNullOrWhiteSpace(ClassName))
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
        Result.ClassName = ClassName;
        Result.Form = FormName;
        DataRegistry.Lookups.Add(Result);
        return Result;
    }
    static void UpdateLookup(LookupDef LookupDef, Type EnumType, string TableName, string SqlText, string ClassName, string FormName, bool? UseNullItem)
    {
        if (EnumType != null)
        {
            LookupDef.EnumTypeName = EnumType.FullName;
            LookupDef.TableName = null;
            LookupDef.SqlText = null;
            LookupDef.ClassName = null;
        }
        else if (TableName != null)
        {
            LookupDef.EnumTypeName = null;
            LookupDef.TableName = TableName;
            LookupDef.SqlText = $"select * from {TableName}";
            LookupDef.ClassName = null;
        }
        else if (SqlText != null)
        {
            LookupDef.EnumTypeName = null;
            LookupDef.TableName = null;
            LookupDef.SqlText = SqlText;
            LookupDef.ClassName = null;
        }
        else if (ClassName != null)
        {
            LookupDef.EnumTypeName = null;
            LookupDef.TableName = null;
            LookupDef.SqlText = null;
            LookupDef.ClassName = ClassName;
        }

        if (FormName != null)
            LookupDef.Form = FormName;
        if (UseNullItem.HasValue)
            LookupDef.UseNullItem = UseNullItem.Value;
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
    static void CheckLookupWithClassName(string Name, string ClassName)
    {
        CheckLookup(Name);
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException($"Cannot add a {nameof(LookupDef)}. No '{nameof(ClassName)}' is provided.");
    }

    static void CheckLocator(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. No '{nameof(Name)}' is provided.");
        if (Locators.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(LocatorDef)}. '{Name}' is already registered.");
    }
    static LocatorDef AddLocatorInternal(string Name, string Source, string ClassName, string KeyField, string FormName, string WebFormName)
    {
        LocatorDef Result = new();
        Result.Name = Name;
        Result.Source = Source;
        Result.ClassName = ClassName;
        Result.KeyField = KeyField;
        Result.Form = FormName;
        Result.WebForm = WebFormName;
        Locators.Add(Result);
        return Result;
    }
    static void UpdateLocator(LocatorDef LocatorDef, string Source, string ClassName, string KeyField, string FormName, string WebFormName)
    {
        if (Source != null)
            LocatorDef.Source = Source;
        if (ClassName != null)
            LocatorDef.ClassName = ClassName;
        if (KeyField != null)
            LocatorDef.KeyField = KeyField;
        if (FormName != null)
            LocatorDef.Form = FormName;
        if (WebFormName != null)
            LocatorDef.WebForm = WebFormName;
    }
    
    static void CheckCodeProvider(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(CodeProviderDef)}. No '{nameof(Name)}' is provided.");
        if (CodeProviders.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(CodeProviderDef)}. '{Name}' is already registered.");
    }
    static void CheckDocumentHandler(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(DocumentHandlerDef)}. No '{nameof(Name)}' is provided.");
        if (DocumentHandlers.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(DocumentHandlerDef)}. '{Name}' is already registered.");
    }
    
    static void CheckConfigProperty(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(ConfigPropertyDef)}. No '{nameof(Name)}' is provided.");
        if (ConfigProperties.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(ConfigPropertyDef)}. '{Name}' is already registered.");
    }
    static void UpdateConfigProperty(ConfigPropertyDef Def, string TitleKey, string GroupName, UserLevel? SecurityLevel, ConfigValueKind? Kind, string DefaultValue, string TypeName, string EditorClassName, ConfigScopeFlags? Scopes)
    {
        if (TitleKey != null)
            Def.TitleKey = TitleKey;
        if (GroupName != null)
            Def.GroupName = GroupName;
        if (SecurityLevel.HasValue)
            Def.SecurityLevel = SecurityLevel.Value;
        if (Kind.HasValue)
            Def.Kind = Kind.Value;
        if (DefaultValue != null)
            Def.DefaultValue = DefaultValue;
        if (Scopes.HasValue)
            Def.Scopes = Scopes.Value;
        if (TypeName != null)
            Def.TypeName = TypeName;
        if (EditorClassName != null)
            Def.EditorClassName = EditorClassName;
    }
    
    
    // ● modules
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public ModuleDef AddModule(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool IsSingleSelect = false, UserLevel SecurityLevel = UserLevel.None)
    {
        CheckModule(Name);
        ModuleDef Result = AddModuleInternal(Name, TitleKey, ClassName, ListSelectSql, IsSingleSelect, SecurityLevel);
        return Result;
    }
    /// <summary>
    /// Adds or updates a module definition.
    /// <para>Existing child definitions and collections are preserved.</para>
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public ModuleDef AddOrUpdateModule(string Name, string TitleKey = null, string ClassName = null, string ListSelectSql = null, bool? IsSingleSelect = null, UserLevel? SecurityLevel = null)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(ModuleDef)}. No '{nameof(Name)}' is provided.");

        ModuleDef Result = Modules.Find(Name);
        if (Result == null)
            Result = AddModuleInternal(Name, TitleKey, ClassName, ListSelectSql, IsSingleSelect ?? false, SecurityLevel ?? UserLevel.None);
        else
            UpdateModule(Result, TitleKey, ClassName, ListSelectSql, IsSingleSelect, SecurityLevel);
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
    /// Adds or updates a lookup list module definition.
    /// <para>Existing child definitions and collections are preserved.</para>
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public ModuleDef AddOrUpdateLookupListModule(string TableName, string Name, string TitleKey)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            throw new TripousException($"Cannot add or update a {nameof(ModuleDef)}. No '{nameof(TableName)}' is provided.");
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(ModuleDef)}. No '{nameof(Name)}' is provided.");

        ModuleDef Result = Modules.Find(Name);
        if (Result == null)
            Result = AddLookupListModuleInternal(TableName, Name, TitleKey);
        else
        {
            Result.Table.Name = TableName;
            Result.IsSingleSelect = true;
            Result.UseFilters = false;
            if (TitleKey != null)
                Result.TitleKey = TitleKey;
        }
        return Result;
    }
    
    // ● lookups - enum types
    /// <summary>
    /// Adds a lookup source.
    /// <para>The enum type is used as the source.</para>
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
        LookupDef Result = AddLookupInternal(Name, EnumType, TableName: null, SqlText: null, ClassName: null, FormName: null, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds or updates an enum lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookupSource(Type EnumType, bool? UseNullItem = null)
    {
        if (EnumType == null)
            throw new TripousException($"Cannot add or update a {nameof(LookupDef)}. No '{nameof(EnumType)}' is provided.");
        return AddOrUpdateLookup(EnumType.FullName, EnumType, TableName: null, SqlText: null, ClassName: null, FormName: null, UseNullItem);
    }
    /// <summary>
    /// Adds or updates an enum lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookupSource(string Name, Type EnumType, bool? UseNullItem = null)
    {
        if (EnumType == null)
            throw new TripousException($"Cannot add or update a {nameof(LookupDef)}. No '{nameof(EnumType)}' is provided.");
        return AddOrUpdateLookup(Name, EnumType, TableName: null, SqlText: null, ClassName: null, FormName: null, UseNullItem);
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
        LookupDef Result = AddLookupInternal(Name, EnumType: null, TableName: TableName, SqlText: null, ClassName: null, FormName: FormName, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds or updates a table lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookupWithTableName(string Name, string TableName = null, string FormName = null, bool? UseNullItem = null)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            TableName = Name;
        return AddOrUpdateLookup(Name, EnumType: null, TableName: TableName, SqlText: null, ClassName: null, FormName: FormName, UseNullItem);
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
        LookupDef Result = AddLookupInternal(Name, EnumType: null, TableName: null, SqlText: SqlText, ClassName: null, FormName: FormName, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds or updates a SQL lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookupWithSql(string Name, string SqlText = null, string FormName = null, bool? UseNullItem = null)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            SqlText = $"select * from {Name}";
        return AddOrUpdateLookup(Name, EnumType: null, TableName: null, SqlText: SqlText, ClassName: null, FormName: FormName, UseNullItem);
    }

    // ● lookups - with class name
    /// <summary>
    /// Adds a lookup source.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LookupDef AddLookupWithClassName(string Name, string ClassName, string FormName = null, bool UseNullItem = false)
    {
        CheckLookupWithClassName(Name, ClassName);
        LookupDef Result = AddLookupInternal(Name, EnumType: null, TableName: null, SqlText: null, ClassName: ClassName, FormName: FormName, UseNullItem: UseNullItem);
        return Result;
    }
    /// <summary>
    /// Adds or updates a class lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookupWithClassName(string Name, string ClassName, string FormName = null, bool? UseNullItem = null)
    {
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException($"Cannot add or update a {nameof(LookupDef)}. No '{nameof(ClassName)}' is provided.");
        return AddOrUpdateLookup(Name, EnumType: null, TableName: null, SqlText: null, ClassName: ClassName, FormName: FormName, UseNullItem);
    }
    
    // ● lookups - add or update
    /// <summary>
    /// Adds or updates a lookup definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LookupDef AddOrUpdateLookup(string Name, Type EnumType, string TableName, string SqlText, string ClassName, string FormName, bool? UseNullItem)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(LookupDef)}. No '{nameof(Name)}' is provided.");
        if (EnumType != null && !EnumType.IsEnum)
            throw new TripousDataException($"Cannot add or update a {nameof(LookupDef)}. Type {EnumType.FullName} is not an enum type");
        if (EnumType == null && string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(SqlText) && string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException($"Cannot add or update a {nameof(LookupDef)}. No source is provided.");

        LookupDef Result = Lookups.Find(Name);
        if (Result == null)
            Result = AddLookupInternal(Name, EnumType, TableName, SqlText, ClassName, FormName, UseNullItem ?? false);
        else
            UpdateLookup(Result, EnumType, TableName, SqlText, ClassName, FormName, UseNullItem);
        return Result;
    }
    
    // ● locators
    /// <summary>
    /// Adds a locator definition.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public LocatorDef AddLocator(string Name, string Source = null, string ClassName = null, string KeyField = null, string FormName = null, string WebFormName = null)
    {
        CheckLocator(Name);
        LocatorDef Result = AddLocatorInternal(Name, Source, ClassName, KeyField, FormName, WebFormName);
        return Result;
    }
    /// <summary>
    /// Adds or updates a locator definition.
    /// <para>Existing field definitions and collections are preserved.</para>
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public LocatorDef AddOrUpdateLocator(string Name, string Source = null, string ClassName = null, string KeyField = null, string FormName = null, string WebFormName = null)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(LocatorDef)}. No '{nameof(Name)}' is provided.");

        LocatorDef Result = Locators.Find(Name);
        if (Result == null)
            Result = AddLocatorInternal(Name, Source, ClassName, KeyField, FormName, WebFormName);
        else
            UpdateLocator(Result, Source, ClassName, KeyField, FormName, WebFormName);
        return Result;
    }
    /// <summary>
    /// Returns a locator definition, if any, else null.
    /// </summary>
    static public LocatorDef FindLocator(string Name) => Locators.Find(Name);
    /// <summary>
    /// Returns a locator definition, if any, else exception.
    /// </summary>
    static public LocatorDef GetLocator(string Name) => Locators.Get(Name);
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
    /// Adds or returns a code provider definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public CodeProviderDef AddOrUpdateCodeProvider(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(CodeProviderDef)}. No '{nameof(Name)}' is provided.");

        CodeProviderDef Result = CodeProviders.Find(Name);
        if (Result == null)
            Result = AddCodeProvider(Name);
        return Result;
    }
    
    // ● document handlers
    /// <summary>
    /// Adds a definition.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public DocumentHandlerDef AddDocumentHandler(string Name, string ClassName)
    {
        CheckDocumentHandler(Name);
        DocumentHandlerDef Result = new DocumentHandlerDef() { Name = Name, ClassName = ClassName };
        DocumentHandlers.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds or updates a document handler definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public DocumentHandlerDef AddOrUpdateDocumentHandler(string Name, string ClassName)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(DocumentHandlerDef)}. No '{nameof(Name)}' is provided.");
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException($"Cannot add or update a {nameof(DocumentHandlerDef)}. No '{nameof(ClassName)}' is provided.");

        DocumentHandlerDef Result = DocumentHandlers.Find(Name);
        if (Result == null)
            Result = AddDocumentHandler(Name, ClassName);
        else if (ClassName != null)
            Result.ClassName = ClassName;
        return Result;
    }
    
    // ● config properties
    /// <summary>
    /// Adds a configuration property definition.
    /// If the definition exists, an exception is thrown.
    /// </summary>
    static public ConfigPropertyDef AddConfigProperty(string Name, string TitleKey = null, string GroupName = null, UserLevel SecurityLevel = UserLevel.Admin, ConfigValueKind Kind = ConfigValueKind.String, string DefaultValue = null, string TypeName = null, string EditorClassName = null, ConfigScopeFlags Scopes = ConfigScopeFlags.All)
    {
        CheckConfigProperty(Name);
        ConfigPropertyDef Result = new();
        Result.Name = Name;
        Result.TitleKey = TitleKey;
        Result.GroupName = GroupName;
        Result.SecurityLevel = SecurityLevel;
        Result.Kind = Kind;
        Result.DefaultValue = DefaultValue;
        Result.Scopes = Scopes;
        Result.TypeName = TypeName;
        Result.EditorClassName = EditorClassName;
        ConfigProperties.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds or updates a configuration property definition.
    /// NOTE: When the definition already exists, non-null parameters and nullable enum parameters with a value update its scalar properties.
    /// </summary>
    static public ConfigPropertyDef AddOrUpdateConfigProperty(string Name, string TitleKey = null, string GroupName = null, UserLevel? SecurityLevel = null, ConfigValueKind? Kind = null, string DefaultValue = null, string TypeName = null, string EditorClassName = null, ConfigScopeFlags? Scopes = null)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(ConfigPropertyDef)}. No '{nameof(Name)}' is provided.");
        ConfigPropertyDef Result = ConfigProperties.Find(Name);
        if (Result == null)
            Result = AddConfigProperty(Name, TitleKey, GroupName, SecurityLevel ?? UserLevel.Admin, Kind ?? ConfigValueKind.String, DefaultValue, TypeName, EditorClassName, Scopes ?? ConfigScopeFlags.All);
        else
            UpdateConfigProperty(Result, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, TypeName, EditorClassName, Scopes);
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
    /// <summary>
    /// The list of document handlers
    /// </summary>
    static public DefList<DocumentHandlerDef> DocumentHandlers { get; } = new();
    /// <summary>
    /// The list of configuration property definitions.
    /// </summary>
    static public DefList<ConfigPropertyDef> ConfigProperties { get; } = new();
}

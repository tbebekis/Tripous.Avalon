namespace PasswordManager.Data;

/// <summary>
/// Registers version 1 descriptors.
/// </summary>
public partial class RegistryVersion1 : RegistryVersion
{
    // ● private
    /// <summary>
    /// Registers the category lookup module.
    /// </summary>
    private void RegisterCategoryModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Name
                            ,DisplayOrder
                         from
                             Category
                         order by
                             DisplayOrder
                         """;
        ModuleDef Module = DataRegistry.AddModule("Category", TitleKey: "Categories", ListSelectSql: SqlText, IsSingleSelect: true);
        Module.GuidOids = false;
        TableDef Table = Module.Table;
        Table.Name = "Category";
        Table.AddIntegerId();
        Table.AddString("Name", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddInteger("DisplayOrder", Flags: FieldFlags.Required);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the credential module.
    /// </summary>
    private void RegisterCredentialModule()
    {
        string SqlText = """
                         select
                             c.Id
                            ,c.Title
                            ,c.UserName
                            ,c.Url
                            ,cat.Name as Category
                            ,c.CreatedAt
                            ,c.UpdatedAt
                         from
                             Credential c
                                left join Category cat on cat.Id = c.CategoryId
                         order by
                             c.Title
                         """;
        ModuleDef Module = DataRegistry.AddModule("Credential", TitleKey: "Credentials", ClassName: typeof(CredentialDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "Credential";
        Table.AddId();
        Table.AddIntegerLookupId("CategoryId", "Category", TitleKey: "Category", Flags: FieldFlags.Required);
        Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("UserName", 128, Flags: FieldFlags.Searchable);
        Table.AddString("Url", 512, Flags: FieldFlags.Searchable);
        Table.AddTextBlob("Password", Flags: FieldFlags.Memo).SetMemo();
        Table.AddTextBlob("Notes", Flags: FieldFlags.Memo).SetMemo();
        Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Title", FieldName: "c.Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("UserName", FieldName: "c.UserName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Url", FieldName: "c.Url", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Category", FieldName: "cat.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("UpdatedAt", FieldName: "c.UpdatedAt", FilterDataType: DataFieldType.DateTime, ConditionOp: ConditionOp.Between);
    }
    /// <summary>
    /// Registers the system configuration module used by ConfigDialog.
    /// </summary>
    private void RegisterSysConfigModule()
    {
        string SqlText = """
                         select
                             Id
                            ,ScopeId
                            ,case
                                when ScopeId = 0 then 'System'
                                when ScopeId = 1 then 'Company'
                                when ScopeId = 2 then 'User'
                                else ''
                             end as ConfigScope
                            ,OwnerKey
                            ,Name
                            ,Value
                            ,ModifiedAt
                            ,ModifiedBy
                         from
                             SYS_CONFIG
                         order by
                             Name
                         """;
        ModuleDef Module = DataRegistry.AddModule("SysConfig", ClassName: typeof(SysConfigModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "SYS_CONFIG";
        Table.AddId();
        Table.AddEnumLookupId("ScopeId", "ConfigScope", TypeStore.Get("ConfigScope"), Flags: FieldFlags.Required);
        Table.AddString("OwnerKey", 96);
        Table.AddString("Name", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Value", 512);
        Table.AddTextBlob("TextValue");
        Table.AddDateTime("ModifiedAt", Flags: FieldFlags.ReadOnlyUI);
        Table.AddString("ModifiedBy", 40, Flags: FieldFlags.ReadOnlyUI);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("ConfigScope", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the system log module.
    /// </summary>
    private void RegisterLogModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Year
                            ,Month
                            ,DayOfMonth
                            ,LogTime
                            ,User
                            ,Host
                            ,Level
                            ,Source
                            ,Scope
                            ,EventId
                         from
                             SYS_LOG
                         order by
                             Year desc,
                             Month desc,
                             DayOfMonth desc,
                             LogTime desc
                         """;
        ModuleDef Module = DataRegistry.AddModule("Log", TitleKey: "Log", ClassName: typeof(LogDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "SYS_LOG";
        Table.AddId();
        Table.AddInteger("Year", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddInteger("Month", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddInteger("DayOfMonth", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("LogTime", 20, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("User", 96, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("Host", 96, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("Level", 96, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("Source", 512, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("Scope", 512, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("EventId", 96, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddTextBlob("Message", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI).SetLargeMemo();
    }

    // ● public
    /// <summary>
    /// Registers lookup definitions.
    /// </summary>
    public override void RegisterLookupSources()
    {
        LookupDef Lookup = DataRegistry.AddLookupWithTableName("Category", "Category", FormName: "Category");
        Lookup.ValueField = "Id";
        Lookup.DisplayField = "Name";
    }
    /// <summary>
    /// Registers module definitions.
    /// </summary>
    public override void RegisterModules()
    {
        RegisterLogModule();
        RegisterSysConfigModule();
        RegisterCategoryModule();
        RegisterCredentialModule();
    }
    /// <summary>
    /// Registers form definitions.
    /// </summary>
    public override void RegisterForms()
    {
        DesktopRegistry.AddForm("Category", TitleKey: "Categories", Module: "Category", Group: "Vault");
        DesktopRegistry.AddForm("Credential", TitleKey: "Credentials", Module: "Credential", Group: "Vault");
    }
    /// <summary>
    /// Registers sample configuration property definitions.
    /// </summary>
    public override void RegisterConfigProperties()
    {
        DataRegistry.AddConfigProperty("PasswordManager.MasterSalt", TitleKey: "Master Salt", GroupName: "Vault Security", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.String, DefaultValue: "");
        DataRegistry.AddConfigProperty("PasswordManager.MasterHash", TitleKey: "Master Verifier Hash", GroupName: "Vault Security", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.String, DefaultValue: "");
        DataRegistry.AddConfigProperty("PasswordManager.KdfIterations", TitleKey: "KDF Iterations", GroupName: "Vault Security", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Integer, DefaultValue: "100000");
        DataRegistry.AddConfigProperty("PasswordManager.MinimumPasswordLength", TitleKey: "Minimum Master Password Length", GroupName: "Vault Security", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Integer, DefaultValue: "8");
        DataRegistry.AddConfigProperty("PasswordManager.AutoOpenCredentialList", TitleKey: "Auto Open Credential List", GroupName: "Password Manager", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Boolean, DefaultValue: "true");
    }

    // ● properties
    /// <summary>
    /// Gets the registry version number.
    /// </summary>
    public override int VersionNumber => 1;
}

namespace MiniCrm.Data;

/// <summary>
/// Registers version 1 descriptors.
/// </summary>
public partial class RegistryVersion1 : RegistryVersion
{
    // ● private
    /// <summary>
    /// Registers the application user system module.
    /// </summary>
    private void RegisterAppUserModule()
    {
        string SqlText = """
                         select
                             Id
                            ,UserName
                            ,FullName
                            ,UserLevelId
                            ,case
                                when UserLevelId = 0 then 'None'
                                when UserLevelId = 1 then 'Guest'
                                when UserLevelId = 2 then 'User'
                                when UserLevelId = 4 then 'Admin'
                                when UserLevelId = 8 then 'ClientApp'
                                when UserLevelId = 256 then 'Service'
                                when UserLevelId = 4096 then 'God'
                                else ''
                             end as UserLevel
                            ,CultureCode
                            ,Email
                            ,Phone
                            ,LastLoginAt
                            ,PasswordChangedAt
                            ,IsActive
                         from
                             SYS_APP_USER
                         order by
                             UserName
                         """;
        ModuleDef Module = DataRegistry.AddModule("AppUser", TitleKey: "Users", ClassName: typeof(AppUserDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true, SecurityLevel: UserLevel.Admin);
        TableDef Table = Module.Table;
        Table.Name = "SYS_APP_USER";
        Table.AddId();
        Table.AddString("UserName", 64, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit);
        Table.AddString("Password", 512, Flags: FieldFlags.Required | FieldFlags.Hidden);
        Table.AddString("Salt", 256, Flags: FieldFlags.Required | FieldFlags.Hidden);
        Table.AddString("FullName", 96, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddEnumLookupId("UserLevelId", "UserLevel", TypeStore.Get("UserLevel"), Flags: FieldFlags.Required);
        Table.AddString("CultureCode", 16);
        Table.AddString("Email", 96);
        Table.AddString("Phone", 40);
        Table.AddDateTime("LastLoginAt", Flags: FieldFlags.ReadOnlyUI);
        Table.AddDateTime("PasswordChangedAt", Flags: FieldFlags.ReadOnlyUI);
        Table.AddBoolean("IsActive", Flags: FieldFlags.Required);
        Table.AddTextBlob("Remarks");
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("UserName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("FullName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("UserLevel", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("IsActive", FilterDataType: DataFieldType.Boolean);
    }
    /// <summary>
    /// Registers the system configuration module.
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
        ModuleDef Module = DataRegistry.AddModule("SysConfig", TitleKey: "Config", ClassName: typeof(SysConfigModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "SYS_CONFIG";
        Table.AddId();
        Table.AddEnumLookupId("ScopeId", "ConfigScope", TypeStore.Get("ConfigScope"), Flags: FieldFlags.Required);
        Table.AddString("OwnerKey", 96);
        Table.AddString("Name", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Value", 512);
        Table.AddTextBlob("TextValue");
        Table.AddDateTime("ModifiedAt", Flags: FieldFlags.ReadOnlyUI);
        Table.AddStringLookupId("ModifiedBy", "SYS_APP_USER", Flags: FieldFlags.ReadOnlyUI);
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
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("User", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Host", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Level", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Source", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Scope", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the number series system module.
    /// </summary>
    private void RegisterNumberSeriesModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Code
                            ,Name
                            ,Pattern
                            ,ResetPeriodId
                            ,case
                                when ResetPeriodId = 0 then 'None'
                                when ResetPeriodId = 1 then 'Year'
                                when ResetPeriodId = 2 then 'Semester'
                                when ResetPeriodId = 3 then 'Quarter'
                                when ResetPeriodId = 4 then 'Month'
                                when ResetPeriodId = 5 then 'Week'
                                when ResetPeriodId = 6 then 'Day'
                                else ''
                             end as ResetPeriod
                            ,NextNumber
                            ,LastResetValue
                            ,IsActive
                         from
                             SYS_NUMBER_SERIES
                         order by
                             Code
                         """;
        ModuleDef Module = DataRegistry.AddModule("NumberSeries", TitleKey: "Number Series", ClassName: typeof(CodeProviderModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "SYS_NUMBER_SERIES";
        Table.AddId();
        Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("Name", 96, Flags: FieldFlags.Required);
        Table.AddString("Pattern", 64, Flags: FieldFlags.Required);
        Table.AddEnumLookupId("ResetPeriodId", "ResetPeriod", TypeStore.Get("ResetPeriod"), Flags: FieldFlags.Required);
        Table.AddInteger("NextNumber", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI);
        Table.AddString("LastResetValue", 16, Flags: FieldFlags.ReadOnlyUI);
        Table.AddBoolean("IsActive", Flags: FieldFlags.Required);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Code", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("ResetPeriod", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the string resource system module.
    /// </summary>
    private void RegisterResourceStringsModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Lang
                            ,ResKey
                         from
                             SYS_STR_RES
                         order by
                             Lang,
                             ResKey
                         """;
        ModuleDef Module = DataRegistry.AddModule("ResourceStrings", TitleKey: "String Resources", ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "SYS_STR_RES";
        Table.AddId();
        Table.AddString("Lang", 12, Flags: FieldFlags.Required);
        Table.AddString("ResKey", 96, Flags: FieldFlags.Required);
        Table.AddTextBlob("ResValue", Flags: FieldFlags.Required).SetMemo();
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Lang", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("ResKey", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the activity type lookup module.
    /// </summary>
    private void RegisterActivityTypeModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Name
                            ,DisplayOrder
                         from
                             ActivityType
                         order by
                             DisplayOrder
                         """;
        ModuleDef Module = DataRegistry.AddModule("ActivityType", TitleKey: "Activity Types", ListSelectSql: SqlText, IsSingleSelect: true);
        Module.GuidOids = false;
        TableDef Table = Module.Table;
        Table.Name = "ActivityType";
        Table.AddIntegerId();
        Table.AddString("Name", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddInteger("DisplayOrder", Flags: FieldFlags.Required);
    }
    /// <summary>
    /// Registers the customer master module.
    /// </summary>
    private void RegisterCustomerModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Code
                            ,Name
                            ,Email
                            ,Phone
                            ,City
                            ,IsActive
                            ,UpdatedAt
                         from
                             Customer
                         order by
                             Name
                         """;
        ModuleDef Module = DataRegistry.AddModule("Customer", TitleKey: "Customers", ClassName: typeof(MiniCrmDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "Customer";
        Table.AddId();
        Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetCodeProviderName("CUSTOMER");
        Table.AddString("Name", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Email", 96);
        Table.AddString("Phone", 40);
        Table.AddString("Website", 128);
        Table.AddString("City", 96);
        Table.AddBoolean("IsActive", Flags: FieldFlags.Required);
        Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddTextBlob("Notes").SetMemo();

        TableDef Contact = Table.AddDetail("Contact", "Id", "CustomerId");
        Contact.AddId();
        Contact.AddString("CustomerId", 40, Flags: FieldFlags.Required | FieldFlags.Hidden);
        Contact.AddString("FirstName", 64, Flags: FieldFlags.Required);
        Contact.AddString("LastName", 64, Flags: FieldFlags.Required);
        Contact.AddString("JobTitle", 96);
        Contact.AddString("Email", 96);
        Contact.AddString("Phone", 40);
        Contact.AddBoolean("IsPrimaryContact", Flags: FieldFlags.Required);
        Contact.AddTextBlob("Notes").SetMemo();

        TableDef Activity = Table.AddDetail("Activity", "Id", "CustomerId");
        Activity.AddId();
        Activity.AddString("CustomerId", 40, Flags: FieldFlags.Required | FieldFlags.Hidden);
        FieldDef Field = Activity.AddString("ContactId", 40);
        Field.Locator = "Contact";
        TableDef ContactJoin = Activity.AddJoin("ContactId", "Contact", "Contact", "Contact", "Id");
        ContactJoin.AddString("FirstName", 64);
        ContactJoin.AddString("LastName", 64);
        Activity.AddDateTime("ActivityDate", Flags: FieldFlags.Required);
        Activity.AddIntegerLookupId("ActivityTypeId", "ActivityType", TitleKey: "Activity Type", Flags: FieldFlags.Required);
        Activity.AddString("Subject", 128, Flags: FieldFlags.Required);
        Activity.AddString("Description", 4000, Flags: FieldFlags.Memo);
        Activity.AddBoolean("IsClosed", Flags: FieldFlags.Required);

        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Code", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("City", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("IsActive", FilterDataType: DataFieldType.Boolean);
    }
    /// <summary>
    /// Registers the contact module.
    /// </summary>
    private void RegisterContactModule()
    {
        string SqlText = """
                         select
                             Contact.Id
                            ,Contact.CustomerId
                            ,Customer.Name as Customer
                            ,Contact.FirstName
                            ,Contact.LastName
                            ,Contact.JobTitle
                            ,Contact.Email
                            ,Contact.Phone
                            ,Contact.IsPrimaryContact
                         from
                             Contact
                                left join Customer on Customer.Id = Contact.CustomerId
                         order by
                             Customer.Name,
                             Contact.LastName,
                             Contact.FirstName
                         """;
        ModuleDef Module = DataRegistry.AddModule("Contact", TitleKey: "Contacts", ClassName: typeof(MiniCrmDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "Contact";
        Table.AddId();
        FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
        Field.Locator = "Customer";
        TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
        CustomerJoin.AddString("Code", 40);
        CustomerJoin.AddString("Name", 128);
        Table.AddString("FirstName", 64, Flags: FieldFlags.Required);
        Table.AddString("LastName", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("JobTitle", 96);
        Table.AddString("Email", 96);
        Table.AddString("Phone", 40);
        Table.AddBoolean("IsPrimaryContact", Flags: FieldFlags.Required);
        Table.AddTextBlob("Notes").SetMemo();
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Customer", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("LastName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("IsPrimaryContact", FilterDataType: DataFieldType.Boolean);
    }
    /// <summary>
    /// Registers the activity module.
    /// </summary>
    private void RegisterActivityModule()
    {
        SqlProvider Provider = Db.GetDefaultConnectionInfo().GetSqlProvider();
        string ContactDisplaySql = Provider.Concat("Contact.FirstName", "' '", "Contact.LastName");
        string SqlText = $"""
                         select
                             Activity.Id
                            ,Activity.CustomerId
                            ,Customer.Name as Customer
                            ,Activity.ContactId
                            ,{ContactDisplaySql} as Contact
                            ,Activity.ActivityDate
                            ,ActivityType.Name as ActivityType
                            ,Activity.Subject
                            ,Activity.IsClosed
                         from
                             Activity
                                left join Customer on Customer.Id = Activity.CustomerId
                                left join Contact on Contact.Id = Activity.ContactId
                                left join ActivityType on ActivityType.Id = Activity.ActivityTypeId
                         order by
                             Activity.ActivityDate desc,
                             Activity.Subject
                         """;
        ModuleDef Module = DataRegistry.AddModule("Activity", TitleKey: "Activities", ClassName: typeof(MiniCrmDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        TableDef Table = Module.Table;
        Table.Name = "Activity";
        Table.AddId();
        FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
        Field.Locator = "Customer";
        TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
        CustomerJoin.AddString("Code", 40);
        CustomerJoin.AddString("Name", 128);
        Field = Table.AddString("ContactId", 40, TitleKey: "Contact");
        Field.Locator = "Contact";
        TableDef ContactJoin = Table.AddJoin("ContactId", "Contact", "Contact", "Contact", "Id");
        ContactJoin.AddString("FirstName", 64);
        ContactJoin.AddString("LastName", 64);
        Table.AddDateTime("ActivityDate", Flags: FieldFlags.Required);
        Table.AddIntegerLookupId("ActivityTypeId", "ActivityType", TitleKey: "Activity Type", Flags: FieldFlags.Required);
        Table.AddString("Subject", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
        Table.AddBoolean("IsClosed", Flags: FieldFlags.Required);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Customer", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Contact", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("ActivityDate", FilterDataType: DataFieldType.DateTime, ConditionOp: ConditionOp.Between);
        SelectDef.AddFilter("ActivityType", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("IsClosed", FilterDataType: DataFieldType.Boolean);
    }

    // ● public
    /// <summary>
    /// Registers lookup definitions.
    /// </summary>
    public override void RegisterLookupSources()
    {
        DataRegistry.AddOrUpdateCodeProvider("CUSTOMER");
        LookupDef Lookup = DataRegistry.AddLookupWithTableName("SYS_APP_USER", "SYS_APP_USER");
        Lookup.ValueField = "Id";
        Lookup.DisplayField = "UserName";
        Lookup = DataRegistry.AddLookupWithTableName("ActivityType", "ActivityType", FormName: "ActivityType");
        Lookup.ValueField = "Id";
        Lookup.DisplayField = "Name";
    }
    /// <summary>
    /// Registers locator definitions.
    /// </summary>
    public override void RegisterLocators()
    {
        LocatorDef Locator = DataRegistry.AddOrUpdateLocator2("Customer", Source: "Customer", KeyField: "Id", FormName: "Customer");
        Locator.Add("Id", DataFieldType.String);
        Locator.Add("Code", DataFieldType.String);
        Locator.Add("Name", DataFieldType.String);
        Locator.AddResultFields("Id", "Code", "Name");
        Locator.AddSearchFields("Code", "Name");

        Locator = DataRegistry.AddOrUpdateLocator2("Contact", Source: "Contact", KeyField: "Id", FormName: "Contact");
        Locator.Add("Id", DataFieldType.String);
        Locator.Add("CustomerId", DataFieldType.String);
        Locator.Add("FirstName", DataFieldType.String);
        Locator.Add("LastName", DataFieldType.String);
        Locator.AddResultFields("Id", "CustomerId", "FirstName", "LastName");
        Locator.AddSearchFields("FirstName", "LastName");
    }
    /// <summary>
    /// Registers module definitions.
    /// </summary>
    public override void RegisterModules()
    {
        RegisterLogModule();
        RegisterNumberSeriesModule();
        RegisterResourceStringsModule();
        RegisterAppUserModule();
        RegisterSysConfigModule();
        RegisterActivityTypeModule();
        RegisterCustomerModule();
        RegisterContactModule();
        RegisterActivityModule();
    }
    /// <summary>
    /// Registers form definitions.
    /// </summary>
    public override void RegisterForms()
    {
        DesktopRegistry.AddForm("AppUser", TitleKey: "Users", Module: "AppUser", ClassName: typeof(AppUserForm).FullName, Group: "System", IsReadOnly: false);
        DesktopRegistry.AddForm("Log", TitleKey: "Log", Module: "Log", Group: "System");
        DesktopRegistry.AddForm("NumberSeries", TitleKey: "Number Series", Module: "NumberSeries", Group: "System");
        DesktopRegistry.AddForm("ActivityType", TitleKey: "Activity Types", Module: "ActivityType", Group: "Setup");
        DesktopRegistry.AddForm("Customer", TitleKey: "Customers", Module: "Customer", Group: "CRM");
        DesktopRegistry.AddForm("Contact", TitleKey: "Contacts", Module: "Contact", Group: "CRM");
        DesktopRegistry.AddForm("Activity", TitleKey: "Activities", Module: "Activity", Group: "CRM");
    }
    /// <summary>
    /// Registers sample configuration property definitions.
    /// </summary>
    public override void RegisterConfigProperties()
    {
        DataRegistry.AddConfigProperty("MiniCrm.AutoOpenCustomerList", TitleKey: "Auto Open Customer List", GroupName: "Mini CRM", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Boolean, DefaultValue: "true");
        DataRegistry.AddConfigProperty("MiniCrm.DefaultActivityTypeId", TitleKey: "Default Activity Type", GroupName: "Mini CRM", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Integer, DefaultValue: "1");
        DataRegistry.AddConfigProperty("UseUsers", TitleKey: "Use Users", GroupName: "Mini CRM", SecurityLevel: UserLevel.Admin, Kind: ConfigValueKind.Boolean, DefaultValue: "false");
    }

    // ● properties
    /// <summary>
    /// Gets the registry version number.
    /// </summary>
    public override int VersionNumber => 1;
}

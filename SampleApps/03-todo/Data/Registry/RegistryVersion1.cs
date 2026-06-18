namespace ToDo.Data;

/// <summary>
/// Registers version 1 descriptors.
/// </summary>
public partial class RegistryVersion1 : RegistryVersion
{
    // ● private
    /// <summary>
    /// Registers the status lookup module.
    /// </summary>
    private void RegisterTodoStatusModule()
    {
        string SqlText = """
                         select
                             Id
                            ,Name
                            ,DisplayOrder
                         from
                             TodoStatus
                         order by
                             DisplayOrder
        """;
        ModuleDef Module = DataRegistry.AddModule("TodoStatus", TitleKey: "Statuses", ListSelectSql: SqlText, IsSingleSelect: true);
        Module.GuidOids = false;
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);

        TableDef Table = Module.Table;
        Table.Name = "TodoStatus";
        Table.AddIntegerId();
        Table.AddString("Name", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddInteger("DisplayOrder", Flags: FieldFlags.Required);
    }
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
        ModuleDef Module = DataRegistry.AddModule("AppUser", TitleKey: "Users", ListSelectSql: SqlText, IsSingleSelect: true, SecurityLevel: UserLevel.Admin);
        TableDef Table = Module.Table;
        Table.Name = "SYS_APP_USER";
        Table.AddId();
        Table.AddString("UserName", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Password", 512, Flags: FieldFlags.Required | FieldFlags.Hidden);
        Table.AddString("Salt", 256, Flags: FieldFlags.Required | FieldFlags.Hidden);
        Table.AddString("FullName", 96, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddEnumLookupId("UserLevelId", "UserLevel", TypeStore.Get("UserLevel"), Flags: FieldFlags.Required);
        Table.AddString("CultureCode", 16);
        Table.AddString("Email", 96);
        Table.AddString("Phone", 40);
        Table.AddDateTime("LastLoginAt");
        Table.AddDateTime("PasswordChangedAt");
        Table.AddBoolean("IsActive", Flags: FieldFlags.Required);
        Table.AddTextBlob("Remarks");
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("UserName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("FullName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("UserLevel", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("IsActive", FilterDataType: DataFieldType.Boolean);
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
        Table.AddInteger("Year", Flags: FieldFlags.Required);
        Table.AddInteger("Month", Flags: FieldFlags.Required);
        Table.AddInteger("DayOfMonth", Flags: FieldFlags.Required);
        Table.AddString("LogTime", 20, Flags: FieldFlags.Required);
        Table.AddString("User", 96, Flags: FieldFlags.Required);
        Table.AddString("Host", 96, Flags: FieldFlags.Required);
        Table.AddString("Level", 96, Flags: FieldFlags.Required);
        Table.AddString("Source", 512, Flags: FieldFlags.Required);
        Table.AddString("Scope", 512, Flags: FieldFlags.Required);
        Table.AddString("EventId", 96, Flags: FieldFlags.Required);
        Table.AddTextBlob("Message", Flags: FieldFlags.Required).SetLargeMemo();
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("User", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Host", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Level", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Source", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Scope", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Registers the task master module.
    /// </summary>
    private void RegisterTodoTaskModule()
    {
        // ● The default list SELECT shows task rows together with the status display name.
        // ● It is intentionally simple SQL so the SelectDef and filters are easy to inspect.
        string SqlText = """
                         select
                             t.Id
                            ,t.Title
                            ,s.Name as Status
                            ,t.DueDate
                            ,t.Priority
                            ,t.IsDone
                            ,t.CreatedAt
                            ,t.UpdatedAt
                         from
                             TodoTask t
                                left join TodoStatus s on s.Id = t.TodoStatusId
                         order by
                             t.IsDone,
                             t.Priority desc,
                             t.DueDate,
                             t.Title
                         """;
        ModuleDef Module = DataRegistry.AddModule("TodoTask", TitleKey: "ToDo", ClassName: typeof(ToDoDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Title", FieldName: "t.Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("Status", FieldName: "s.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("DueDate", FieldName: "t.DueDate", FilterDataType: DataFieldType.Date, ConditionOp: ConditionOp.Between);
        // ● Boolean database fields are integer-backed 0/1 values.
        // ● The filter UI displays them as All/True/False and emits field = 1/0.
        SelectDef.AddFilter("IsDone", FieldName: "t.IsDone", FilterDataType: DataFieldType.Boolean);

        // ● TableDef describes the editable table, not the list SELECT result.
        // ● TodoStatusId is stored as an integer but displayed through the TodoStatus lookup.
        TableDef Table = Module.Table;
        Table.Name = "TodoTask";
        Table.AddId();
        Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
        Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
        Table.AddDate("DueDate");
        Table.AddDateTime("CompletedAt", Flags: FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddInteger("Priority", Flags: FieldFlags.Required);
        // ● TableDef.AddBoolean() creates an integer-backed 0/1 boolean field.
        Table.AddBoolean("IsDone");
        Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
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
        Table.AddStringLookupId("ModifiedBy", "SYS_APP_USER", Flags: FieldFlags.ReadOnlyUI);
        SelectDef SelectDef = Module.SelectList[0];
        SelectDef.AddFilter("Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("ConfigScope", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }

    // ● public
    /// <summary>
    /// Registers lookup definitions.
    /// </summary>
    public override void RegisterLookupSources()
    {
        // ● LookupDef describes where lookup items come from.
        // ● This lookup uses a normal table as its source.
        // ● LookupSource is the runtime object that loads rows and creates LookupItem values.
        // ● The default LookupSource can load from a table name, SQL text, enum or DataTable.
        // ● A larger application may register a custom LookupSource class with AddLookupWithClassName().
        LookupDef Lookup = DataRegistry.AddLookupWithTableName("TodoStatus", "TodoStatus", FormName: "TodoStatus");
        Lookup.ValueField = "Id";
        Lookup.DisplayField = "Name";

        Lookup = DataRegistry.AddLookupWithTableName("SYS_APP_USER", "SYS_APP_USER");
        Lookup.ValueField = "Id";
        Lookup.DisplayField = "UserName";
    }
    /// <summary>
    /// Registers module definitions.
    /// </summary>
    public override void RegisterModules()
    {
        RegisterLogModule();
        RegisterAppUserModule();
        RegisterSysConfigModule();
        RegisterTodoStatusModule();
        RegisterTodoTaskModule();
    }
    /// <summary>
    /// Registers form definitions.
    /// </summary>
    public override void RegisterForms()
    {
        DesktopRegistry.AddForm("TodoStatus", TitleKey: "Statuses", Module: "TodoStatus", Group: "Modules");
        DesktopRegistry.AddForm("TodoTask", TitleKey: "ToDo", Module: "TodoTask", Group: "Modules");
    }
    /// <summary>
    /// Registers sample configuration property definitions.
    /// </summary>
    public override void RegisterConfigProperties()
    {
        DataRegistry.AddConfigProperty("ToDo.DefaultPriority", TitleKey: "Default ToDo Priority", GroupName: "ToDo", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Integer, DefaultValue: "1");
        DataRegistry.AddConfigProperty("ToDo.ShowCompletedTasks", TitleKey: "Show Completed Tasks", GroupName: "ToDo", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Boolean, DefaultValue: "true");
        DataRegistry.AddConfigProperty("ToDo.AutoOpenTaskList", TitleKey: "Auto Open Task List", GroupName: "ToDo", SecurityLevel: UserLevel.None, Kind: ConfigValueKind.Boolean, DefaultValue: "true");
    }

    // ● properties
    /// <summary>
    /// Gets the registry version number.
    /// </summary>
    public override int VersionNumber => 1;
}

# ToDo

This is the third Tripous sample application.

Purpose

- Use SQLite.
- Use a master table.
- Use a status lookup table.
- Register multiple modules.
- Register multiple forms.
- Register a table-backed LookupDef.
- Register SelectDef filters.
- Use the default DataForm grid and item editing UI.
- Show more DataModule patterns.
- Avoid RegBuilder.

What this sample teaches

- How a more real-world Tripous desktop app is structured.
- How a lookup table participates in editing a master record.
- How LookupDef and LookupSource work together.
- How SelectDef exposes list views and filters.
- How TableDef fields drive grid columns and item editors.
- How DataForm uses MemTable instances.
- How ItemPage creates the edit UI.
- How AppFormPagerHandler hosts sidebar and content forms.
- How AppHost remains the application orchestrator.

Main scenario

- TodoStatus is the lookup/status table.
- TodoTask is the master table.
- TodoTask.TodoStatusId points to TodoStatus.Id.
- The TodoTask DataForm shows Status as a lookup editor.
- The TodoTask list SELECT joins TodoStatus so the list grid can show the status name.
- The sidebar contains General and Modules command groups.
- The Modules group contains both Statuses and ToDo.

Files

- ToDo.csproj
- Program.cs
- App.axaml
- App.axaml.cs
- HiddenMainWindow.cs
- MainWindow.axaml
- MainWindow.axaml.cs
- AppHost/AppHost.cs
- AppHost/AppHost.Startup.cs
- AppHost/AppHost.Commands.cs
- AppHost/AppHost.Ui.cs
- Data/DataModules/ToDoDataModule.cs
- Data/Registry/SchemaVersion1.cs
- Data/Registry/RegistryVersion.cs
- Data/Registry/RegistryVersion1.cs
- Data/Registry/Registry.cs
- ReadMe.txt

AppHost

- AppHost is the central static class of this application.
- AppHost owns the startup sequence.
- AppHost owns the hidden startup window reference.
- AppHost owns the real MainWindow reference.
- AppHost owns the AppFormPagerHandler references.
- AppHost creates or loads database connection settings.
- AppHost creates the SQLite database when needed.
- AppHost starts schema registration and descriptor registration.
- AppHost registers toolbar and sidebar commands.
- AppHost shows CommandTreeViewForm in the left sidebar.
- AppHost reads ToDo.AutoOpenTaskList from Config.
- AppHost opens TodoTask automatically when ToDo.AutoOpenTaskList is true.

Startup sequence

- App creates HiddenMainWindow.
- HiddenMainWindow becomes Avalonia desktop MainWindow.
- HiddenMainWindow.Opened calls AppHost.Start().
- AppHost initializes SysConfig.
- AppHost loads or creates SQLite connection settings.
- AppHost creates the SQLite database when needed.
- Registry.RegisterSchemas() registers SchemaVersion1.
- Schemas.Execute() creates SYS_APP_USER, SYS_CONFIG, SYS_LOG, TodoStatus and TodoTask.
- SchemaVersion1 seed statements insert default statuses.
- AppHost creates the default SqlStore.
- AppHost.InitializeLibraries() is empty here, but shows where multi-assembly applications initialize central static classes.
- TypeStore.RegisterLoadedAssemblies() scans loaded assemblies so Tripous can create registered types by name.
- Registry.RegisterDescriptors() registers lookups, modules, forms and configuration property definitions.
- AppHost registers toolbar/sidebar commands.
- AppHost creates the real MainWindow.
- Ui.MainWindow is changed from HiddenMainWindow to MainWindow.
- MainWindow is shown.

Configuration

- SYS_APP_USER, SYS_CONFIG and SYS_LOG are standard system tables used by Tripous applications.
- SYS_APP_USER stores application user rows.
- SYS_CONFIG stores values edited by ConfigDialog.
- SYS_LOG stores database-backed log rows.
- Config property definitions live in DataRegistry.ConfigProperties.
- Actual values are stored as rows in SYS_CONFIG.
- Scalar values use Value.
- Memo and object values use TextValue.
- The effective value resolution order is User, Company, System, DefaultValue.
- SysConfigModule is the DataModule used internally by Config.
- This sample registers three ToDo configuration properties:
- ToDo.DefaultPriority
- ToDo.ShowCompletedTasks
- ToDo.AutoOpenTaskList
- ToDo.DefaultPriority is used by ToDoDataModule when inserting a new task.
- ToDo.AutoOpenTaskList is used by AppHost.InitializeUi().
- ToDo.ShowCompletedTasks is an educational placeholder for this sample.
- Application Settings lets the user edit values at User, Company or System scope.

SchemaVersion1

- SchemaVersion1 inherits from SchemaVersionDef.
- RegisterInternal() registers tables and seed statements.
- Version.AddTable() registers CREATE TABLE statements.
- Version.AddStatementAfter() inserts default TodoStatus rows after table creation.
- TodoStatus uses an integer primary key because it is a small lookup table.
- SYS_APP_USER is included as the standard application user table.
- SYS_CONFIG is included because ConfigDialog needs a table-backed SysConfigModule.
- SYS_LOG is included as the standard database log table.
- TodoTask uses the default string Guid-style Id.
- TodoTask has a foreign key to TodoStatus.
- Later versions may add tables, alter tables or add seed data.

Database column tokens

- @NVARCHAR(size) maps to provider-specific Unicode text.
- @DATE maps to provider-specific date.
- @DATE_TIME maps to provider-specific date-time.
- @BOOL maps to the provider-specific integer boolean SQL type.
- @NOT_NULL maps to provider-specific NOT NULL.
- @NULL maps to provider-specific NULL.
- Use @DATE_TIME, not @DATETIME.

Boolean database fields

- In Tripous, boolean database fields and boolean FieldDef fields are integer-backed 0/1 fields.
- DataFieldType.Boolean means an integer-backed boolean FieldDef value.
- Boolean filters use an All/True/False editor and emit SQL values 1/0.
- Boolean filters are validated against the real SELECT schema and must be backed by integer-compatible columns.
- TableDef.AddBoolean() creates an integer-backed FieldDef that the UI displays as a boolean editor.
- @BOOL maps through each SqlProvider.BoolSql property to the provider-specific integer boolean SQL type.
- Code should assign 0 or 1 to these fields when setting default database values.

LookupDef

- LookupDef describes a lookup list.
- A lookup list supplies values for an editor, usually a ComboBox.
- A LookupDef may load from a table name.
- A LookupDef may load from custom SQL.
- A LookupDef may load from an enum type.
- A LookupDef may load from a custom LookupSource class.
- This sample uses DataRegistry.AddLookupWithTableName().
- The lookup name is TodoStatus.
- The source table is TodoStatus.
- ValueField is Id.
- DisplayField is Name.
- Form is TodoStatus, so UI code may know which form edits lookup rows.

LookupSource

- LookupSource is the runtime object that loads lookup items.
- The default LookupSource can select from a table.
- The default LookupSource can execute custom SQL.
- The default LookupSource can load enum values.
- The default LookupSource can load from a DataTable.
- A custom LookupSource class can override loading behavior.
- Custom LookupSource classes are useful when lookup items come from code, services, calculated data or special filtering rules.
- A custom LookupSource class is registered with DataRegistry.AddLookupWithClassName().

ModuleDef

- ModuleDef describes a data module.
- TodoStatus has a ModuleDef.
- TodoTask has a ModuleDef.
- A ModuleDef owns a TableDef.
- A ModuleDef owns one or more SelectDef objects.
- A ModuleDef may point to a custom DataModule class.
- TodoTask points to ToDoDataModule.
- TodoStatus uses the default Tripous.Data.DataModule.

SelectDef

- SelectDef describes a list SELECT.
- The list SELECT feeds the DataForm grid.
- TodoTask SelectDef joins TodoStatus to show the Status name.
- SelectDef.AddFilter() registers list filters.
- TodoTask has filters for Title, Status, DueDate and IsDone.
- The IsDone filter uses DataFieldType.Boolean and is displayed as an All/True/False filter.
- TodoStatus has a Name filter.
- A module may have multiple SelectDef entries.
- Each SelectDef may have different SQL and different filters.
- The list toolbar can switch between SelectDef entries when more than one exists.

TableDef

- TableDef describes the editable item table.
- TableDef is not necessarily the same shape as the list SELECT.
- TodoTask list SELECT includes Status from a join.
- TodoTask TableDef describes only the TodoTask editable fields.
- AddIntegerLookupId() creates an integer foreign key lookup field.
- AddString() creates text fields.
- AddDate() creates date fields.
- AddDateTime() creates date-time fields.
- AddBoolean() creates integer-backed 0/1 boolean fields.
- FieldFlags.Memo creates a multiline editor.
- FieldFlags.Searchable marks a field as useful in search/filter scenarios.
- FieldFlags.ReadOnlyUI makes a field visible but not editable by the user.

MemTable

- MemTable is Tripous' DataTable-derived runtime table.
- DataModule creates MemTable instances from TableDef.
- DataForm binds grids and item controls to MemTable data.
- Insert creates a new DataRow in the item MemTable.
- Save commits MemTable changes through TableSet and SqlStore.
- Cancel rejects MemTable changes.
- This sample leaves MemTable mostly behind the DataModule/DataForm APIs, but it is the in-memory data surface under the UI.

DataModule patterns

- ToDoDataModule inherits from DataModule.
- ToDoDataModule overrides SetDefaultValues().
- SetDefaultValues assigns TodoStatusId, Priority, IsDone, CreatedAt and UpdatedAt.
- SetDefaultValues also manages CompletedAt when IsDone changes.
- Data rules belong in DataModule, not in the UI.
- The UI should show and edit data; the DataModule should own data behavior.

ItemPage and ItemUi

- DataForm is the full list/edit form.
- ItemPage is the default edit page inside a DataForm.
- ItemPage uses UiItemContext to know the ModuleDef, TableDef, DataModule and binding helpers.
- UiItemPage creates the visible item layout from TableDef fields.
- UiFactory creates common controls used by generated item layouts.
- ItemBinder binds controls to MemTable columns.
- UiItemDetails creates detail grids when a TableDef has detail tables.
- UiFieldInfo, UiTableInfo and UiItemInfo store runtime UI metadata.
- This sample has no detail table yet, but it uses the same ItemPage pipeline.

AppForm and AppFormPagerHandler

- AppForm is the base class for Tripous desktop user controls that behave like forms.
- DataForm and CommandTreeViewForm inherit from AppForm.
- AppFormPagerHandler wraps a TabControl.
- AppFormPagerHandler shows AppForm instances as TabItem pages.
- SideBarHandler hosts CommandTreeViewForm in the left sidebar.
- ContentHandler hosts TodoStatus and TodoTask forms in the right content area.
- AppFormPagerHandler reuses already open forms by FormId.
- AppFormPagerHandler handles middle-click close on tabs.
- Middle-click close honors AppForm.ClosableByUser.

Sidebar and toolbar

- The left sidebar uses CommandTreeViewForm.
- CommandTreeViewForm displays AppRegistry.MenuCommands.
- General contains ShowAppFolder, Application Settings, ConnectionInfo, ToggleLog, ClearLog, Log Sql and Exit.
- Modules contains Statuses and ToDo.
- The main toolbar uses AppRegistry.ToolBarCommands.
- The ToDo Modal toolbar button opens TodoTask as a modal dialog.

Operational commands

- Application Settings opens the Tripous ConfigDialog.
- ConnectionInfo opens DbConnectionEditDialog for the DEFAULT connection.
- Log Sql toggles Db.Settings.LogSqlStatements.
- SQL statement logging writes SQL commands to the log through the Tripous data layer.
- These commands are common in larger Tripous desktop applications because they help diagnostics and deployment checks.

Manual test

- Start the application.
- Check that ToDo opens automatically.
- Check that the sidebar contains General and Modules.
- Open Modules / Statuses.
- Check that status rows exist.
- Open Modules / ToDo.
- Press Insert.
- Enter a title.
- Select a status.
- Enter a due date.
- Enter a priority.
- Save the task.
- Check that CreatedAt and UpdatedAt are assigned.
- Edit the task.
- Toggle IsDone.
- Save the task.
- Check that CompletedAt is assigned.
- Edit the task again.
- Clear IsDone.
- Save the task.
- Check that CompletedAt is cleared.
- Use the Title filter.
- Use the Status filter.
- Use the DueDate filter.
- Use the IsDone filter.
- Press ToDo Modal.
- Check that ToDo opens as a modal dialog.
- Close the modal dialog.
- Press ShowAppFolder.
- Check that the base application folder opens.
- Press Application Settings.
- Check that the configuration dialog opens.
- Press ConnectionInfo.
- Check that the DEFAULT SQLite connection opens.
- Press Log Sql.
- Run a list refresh or save operation.
- Check that SQL logging changes state in the log.

Database reset

- Close the application.
- Open the application folder.
- Delete todo.db3 from the Data folder.
- Start the application again.
- The schema and default statuses are created again.

Next sample

- The next sample should add configuration, services, security-related code, settings and import/export patterns.

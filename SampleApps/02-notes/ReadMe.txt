# Notes

This is the second Tripous sample application.

Purpose

- Use SQLite.
- Create one table.
- Register one module.
- Register one form.
- Use one DataModule.
- Define the schema and descriptors by hand.
- Avoid RegBuilder.
- Explain the registration and XXXDef logic of Tripous.

What this sample teaches

- How startup uses a hidden window first.
- How AppHost controls application startup.
- How the real MainWindow is shown only after startup completes.
- How SchemaVersion1 defines a database table.
- How Registry registers schema versions and descriptors.
- How TableDef describes fields.
- How ModuleDef describes data access and list selection.
- How SelectDef describes a list SELECT and its filters.
- How FormDef connects a UI form to a module.
- How AppFormPagerHandler hosts data forms.
- How LogBox is connected to a TextBox.
- How ToolBar uses registered commands.
- How inline event handlers can call private methods.
- How a sample can use default Tripous forms while still using custom modules.

Files

- Notes.csproj
- Program.cs
- App.axaml
- App.axaml.cs
- HiddenMainWindow.cs
- MainWindow.axaml
- MainWindow.axaml.cs
- AppHost/AppHost.cs
- AppHost/AppHost.Startup.cs
- AppHost/AppHost.Commands.cs
- Data/DataModules/NotesDataModule.cs
- Data/Registry/SchemaVersion1.cs
- Data/Registry/RegistryVersion.cs
- Data/Registry/RegistryVersion1.cs
- Data/Registry/Registry.cs
- ReadMe.txt

AppHost

- AppHost is the central static class of this application.
- AppHost is the application orchestrator.
- AppHost owns the startup sequence.
- AppHost owns the hidden startup window reference.
- AppHost owns the real MainWindow reference.
- AppHost owns the AppFormPagerHandler references.
- AppHost shows the command tree in the left sidebar.
- AppHost creates or loads database connection settings.
- AppHost creates the SQLite database when needed.
- AppHost starts schema registration and descriptor registration.
- AppHost registers commands.
- AppHost changes Ui.MainWindow from the hidden window to the real MainWindow only after startup is ready.

AppForm

- AppForm is the base class for Tripous desktop user controls that behave like application forms.
- AppForm is a UserControl, not a Window.
- AppForm can be hosted in a TabItem.
- AppForm can also be hosted in a modal AppFormDialog.
- DataForm inherits from AppForm.
- CommandTreeViewForm also inherits from AppForm.
- AppForm has a FormContext.
- FormContext describes how the form is opened, who owns it and where it is hosted.
- AppForm has a ClosableByUser property.
- When ClosableByUser is true, a tab-hosted form can be closed by the user.
- When ClosableByUser is false, the user cannot close it with middle-click.

AppFormPagerHandler

- AppFormPagerHandler wraps a TabControl.
- AppFormPagerHandler shows AppForm instances as TabItem pages.
- ShowAppForm() shows a generic AppForm.
- ShowDataForm() shows a DataForm registered in DesktopRegistry.
- AppFormPagerHandler reuses an already open form by FormId.
- AppFormPagerHandler selects the form tab after opening or reusing it.
- AppFormPagerHandler handles middle-click close on tabs.
- Middle-click close honors AppForm.ClosableByUser.
- This sample uses two AppFormPagerHandler instances.
- SideBarHandler hosts CommandTreeViewForm in the left sidebar.
- ContentHandler hosts the Notes DataForm in the right content area.

AppFormDialog

- AppFormDialog is a Window that hosts an AppForm modally.
- It is useful when a form should be temporary and modal instead of tab-hosted.
- DataFormContext.ShowFormModal() is the simple helper for showing a registered DataForm modally.
- This sample has a Notes Modal toolbar command.
- Notes Modal opens the same Note registration as a modal dialog.
- This demonstrates that the same registered DataForm can be hosted either in a tab or in a dialog.

Startup sequence

- App creates HiddenMainWindow.
- HiddenMainWindow becomes Avalonia desktop MainWindow.
- HiddenMainWindow.Opened calls AppHost.Start().
- AppHost initializes SysConfig.
- AppHost loads or creates SQLite connection settings.
- AppHost creates the SQLite database when needed.
- Registry.RegisterSchemas() registers SchemaVersion1.
- Schemas.Execute() creates the Note table.
- AppHost creates the default SqlStore.
- AppHost.InitializeLibraries() is empty here, but shows where multi-assembly applications initialize central static classes.
- TypeStore.RegisterLoadedAssemblies() scans loaded assemblies so Tripous can create registered types by name.
- Registry.RegisterDescriptors() registers ModuleDef and FormDef.
- AppHost registers toolbar commands.
- AppHost registers menu/sidebar command groups.
- AppHost creates the real MainWindow.
- Ui.MainWindow is changed from HiddenMainWindow to MainWindow.
- MainWindow is shown.

Hidden window

- The hidden window exists so early dialogs have an owner before the real MainWindow is ready.
- The hidden window is placed far outside the visible screen.
- This follows the same pattern as tERP.
- After the real MainWindow closes, the hidden window is closed too.

Sidebar forms

- The left sidebar hosts AppForm user controls too.
- This sample shows CommandTreeViewForm in the sidebar.
- Sidebar forms should not be closable by the user.
- CommandTreeViewForm sets ClosableByUser to false.
- When ClosableByUser is false, middle-click on the tab does not close the form.
- This keeps the command tree permanently available.

Content forms

- The right content area hosts normal working forms.
- This sample shows the Notes DataForm in the content area.
- Content forms can be closed by their Close toolbar button.
- Content forms can also be closed with middle-click on their tab.

Database connection

- Tripous stores connection settings in DbConnections.json.
- This sample creates a connection automatically if DbConnections.json does not exist or contains no connections.
- The connection name is Default.
- Default is the normal connection name Tripous uses when no specific connection is requested.
- SchemaVersionDef.ConnectionName returns DbConfig.DefaultConnectionName by default.
- ModuleDef.ConnectionName also uses the default connection when not specified.

DbConnections.json example

```json
{
  "List": [
    {
      "Name": "Default",
      "DbServerType": "Sqlite",
      "ConnectionString": "Data Source=[Data]/notes.db3",
      "UserName": "",
      "Password": "",
      "CommandTimeoutSeconds": 60
    }
  ]
}
```

[Data] token

- [Data] is a Tripous path token.
- In desktop applications it points to SysConfig.AppDataFolderPath.
- By default this is under the application configuration folder.
- On Linux it is usually under ~/.config/Notes/Data.
- This sample uses [Data]/notes.db3.
- The toolbar has a ShowAppFolder button that opens SysConfig.AppFolderPath.
- The base application folder contains DbConnections.json and the Data folder.

SchemaVersion1

- SchemaVersion1 inherits from SchemaVersionDef.
- SchemaVersionDef.Register() creates or finds the runtime Schema and SchemaVersion objects.
- RegisterInternal() is where the version registers tables, views and statements.
- Version.AddTable() registers a CREATE TABLE statement.
- Version.AddView() registers a CREATE VIEW statement.
- Version.AddStatementBefore() registers SQL to execute before tables and views are created.
- Version.AddStatementAfter() registers SQL to execute after tables and views are created.
- AddStatementBefore() is useful for preparation SQL.
- AddStatementAfter() is useful for seed data, number series rows, default rows or post-creation updates.
- A later SchemaVersion2 may add new tables.
- A later SchemaVersion2 may execute ALTER TABLE statements.
- A later SchemaVersion2 may only insert seed data.
- This sample has only SchemaVersion1 because it is intentionally small.

Database column tokens

- @NVARCHAR(size) maps to provider-specific Unicode text.
- @VARCHAR(size) maps to provider-specific non-Unicode text.
- @DATE maps to provider-specific date.
- @DATE_TIME maps to provider-specific date-time.
- @BOOL maps to the provider-specific integer boolean SQL type.
- @DECIMAL maps to provider-specific decimal.
- @DECIMAL_(p,s) maps to provider-specific decimal with precision and scale.
- @FLOAT maps to provider-specific floating-point number.
- @BLOB maps to provider-specific binary/blob storage.
- @BLOB_TEXT maps to provider-specific text blob.
- @NBLOB_TEXT maps to provider-specific Unicode text blob.
- @NOT_NULL maps to provider-specific NOT NULL.
- @NULL maps to provider-specific NULL.
- Use @DATE_TIME, not @DATETIME.

Boolean database fields

- In Tripous, boolean database fields and boolean FieldDef fields are integer-backed 0/1 fields.
- DataFieldType.Boolean means an integer-backed boolean value.
- TableDef.AddBoolean() creates an integer-backed FieldDef that the UI displays as a boolean editor.
- @BOOL maps through each SqlProvider.BoolSql property to the provider-specific integer boolean SQL type.
- Code should assign 0 or 1 to these fields when setting default database values.

Registry

- Registry is a static partial class.
- Registry is the application entry point for schema and descriptor registration.
- Registry.RegisterSchemas() registers schema versions.
- Registry.RegisterDescriptors() registers modules, forms, lookups, locators and other descriptors.
- This sample has one RegistryVersion1.
- tERP has generated RegistryVersion classes because it uses RegBuilder.
- This sample writes RegistryVersion1 by hand to teach the descriptors directly.

ModuleDef

- ModuleDef describes a data module.
- ModuleDef has a name, title, class name, table definition and list SELECT definitions.
- DataRegistry.AddModule() registers a ModuleDef.
- The ClassName points to the DataModule type.
- In this sample ClassName points to NotesDataModule.
- If ClassName is not supplied, Tripous.Data.DataModule is used.
- A ModuleDef may have multiple SelectDef entries.
- Each SelectDef may have a different SQL query.
- Each SelectDef may have its own filters.
- The DataForm list toolbar can use the registered SelectDef entries.
- This sample registers one list SELECT.
- The list SELECT has two filters.
- The Title filter uses Contains.
- The CreatedAt filter uses Between.

SelectDef

- SelectDef describes a list SELECT.
- SelectDef.SqlText is the SELECT statement.
- SelectDef.FilterDefs contains the list filters.
- SelectDef.AddFilter() adds a filter manually.
- FilterDataType tells Tripous what editor to create.
- ConditionOp tells Tripous what SQL condition to generate.
- String filters often use Contains.
- Date and DateTime filters often use Between.
- Decimal filters often use GreaterOrEqual or Between.

TableDef

- TableDef describes the editable item table.
- TableDef fields are FieldDef objects.
- Tripous uses TableDef to create the in-memory MemTable.
- Tripous uses TableDef to build insert/update/delete SQL.
- Tripous.Desktop uses TableDef to create editors in ItemPage.
- Table.AddId() adds the primary key field.
- Table.AddString() adds a string field.
- Table.AddInteger() adds an integer field.
- Table.AddDecimal() adds a decimal field.
- Table.AddDouble() adds a double field.
- Table.AddDate() adds a date field.
- Table.AddDateTime() adds a date-time field.
- Table.AddBoolean() adds an integer-backed boolean field.
- Table.AddBlob() adds a binary field.
- Table.AddTextBlob() adds a text blob field.
- Table.AddStringLookupId() and Table.AddIntegerLookupId() add lookup fields.
- Table.AddJoin() adds a joined lookup table description.
- Table.AddDetail() adds a master-detail table description.

Field flags

- Required marks a field as required.
- Hidden hides a field from the UI.
- Searchable marks a field as a good filter/search candidate.
- Memo creates a multiline editor.
- LargeMemo creates a larger standalone memo editor.
- Boolean displays a checkbox-style editor.
- ReadOnlyUI prevents editing in the UI.
- ReadOnlyEdit prevents editing after insert.
- ReadOnly prevents editing more strictly.

FormDef

- FormDef describes a desktop form registration.
- DesktopRegistry.AddForm() registers a FormDef.
- FormDef.Module connects the form to a ModuleDef.
- If ClassName is not supplied, Tripous.Desktop.DataForm is used.
- If ItemClassName is not supplied, Tripous.Desktop.ItemPage is used.
- This sample uses the default DataForm and ItemPage.
- Larger applications may register custom form and item page classes.

DataModule

- DataModule owns the runtime data behavior.
- A DataModule has a ModuleDef.
- A DataModule creates MemTable objects from TableDef.
- A DataModule performs insert, edit, delete, save and cancel operations.
- NotesDataModule overrides SetDefaultValues().
- CreatedAt and UpdatedAt are assigned in the DataModule.
- This keeps data defaults out of the UI layer.

TypeStore

- TypeStore creates registered types by name.
- ModuleDef.ClassName and FormDef.ClassName rely on TypeStore.
- TypeStore can only discover types from loaded assemblies.
- In a single-assembly sample this is simple.
- In a multi-assembly application an AppHost.InitializeLibraries() method is a good place to touch central static classes in other assemblies.
- Even fake Initialize() methods can be useful when they force .NET to load those assemblies before TypeStore.RegisterLoadedAssemblies().

Manual test

- Start the application.
- Check that the Notes form opens automatically.
- Press the ShowAppFolder toolbar button.
- Check that the base application folder opens.
- Press the Notes Modal toolbar button.
- Check that the Notes form opens in a modal dialog.
- Close the modal dialog.
- Check that the left sidebar contains General and Modules groups.
- Double-click Modules / Notes.
- Check that the Notes form opens in the content area.
- Press Insert or use the form insert command.
- Enter a title.
- Enter body text.
- Save the note.
- Check that CreatedAt and UpdatedAt are assigned.
- Edit the note.
- Change the title or body.
- Save the note.
- Check that UpdatedAt changes.
- Toggle IsPinned.
- Save the note.
- Check that pinned notes appear first in the list.
- Type part of the title in the Title filter.
- Execute the list filter.
- Check that only matching notes appear.
- Enter a CreatedAt date range.
- Execute the list filter.
- Check that only notes in that range appear.
- Use the toolbar Notes button.
- Use the toolbar Toggle Log button.
- Use File / Toggle Log.
- Use File / Clear Log.
- Use the toolbar Exit button.

Database reset

- Close the application.
- Open the application folder.
- Delete notes.db3.
- Start the application again.
- The schema is created again from SchemaVersion1.

Next sample

- The next sample should add master data, status lookup, filters, grid editing, LookupSource, SelectDef and more DataModule patterns.

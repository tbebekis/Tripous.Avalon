# Mini CRM

This is the fourth Tripous sample application.

Purpose

- Use SQLite.
- Use multiple application tables.
- Use multiple modules and forms.
- Use a master/detail form.
- Use lookup fields.
- Use locator fields.
- Use system modules.
- Avoid RegBuilder.
- Show how a small real-world Tripous desktop application is registered by hand.

What this sample teaches

- Customer, Contact and Activity application tables.
- ActivityType as a small lookup table.
- Customer as the master module.
- Contact and Activity as detail tables inside Customer.
- Contact and Activity as standalone modules too.
- LocatorDef and locator-backed FieldDef fields.
- LookupDef for small reference lists.
- SelectDef filters for list grids.
- TableDef detail registration.
- DataModule default value patterns.
- AppFormPagerHandler usage in sidebar and content areas.
- Application settings through SYS_CONFIG.

Main scenario

- Customer is the main business entity.
- Contact belongs to Customer.
- Activity belongs to Customer and may also point to Contact.
- ActivityType contains small fixed values such as Call, Email, Meeting and Task.
- Customer form shows Contact and Activity detail grids.
- Contacts and Activities also have their own list forms.
- The sidebar contains General commands and a Modules group.
- The application starts maximized.
- The real MainWindow is shown only after startup, schema execution and descriptor registration are complete.

Files

- MiniCrm.csproj
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
- Data/DataModules/MiniCrmDataModule.cs
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
- AppHost loads or creates DbConnections.json.
- AppHost creates the SQLite database when needed.
- AppHost starts schema registration and descriptor registration.
- AppHost registers toolbar, menu and sidebar commands.
- AppHost shows CommandTreeViewForm in the left sidebar.
- AppHost reads MiniCrm.AutoOpenCustomerList from Config.
- AppHost opens the Customer form automatically when MiniCrm.AutoOpenCustomerList is true.

Startup sequence

- App creates HiddenMainWindow.
- HiddenMainWindow becomes Avalonia desktop MainWindow.
- HiddenMainWindow.Opened calls AppHost.Start().
- AppHost initializes SysConfig.
- AppHost loads or creates SQLite connection settings.
- AppHost creates the SQLite database when needed.
- Registry.RegisterSchemas() registers SchemaVersion1.
- Schemas.Execute() creates the database tables.
- SchemaVersion1 seed statements insert default ActivityType and SYS_NUMBER_SERIES rows.
- AppHost creates the default SqlStore.
- AppHost.InitializeLibraries() is empty here, but shows where multi-assembly applications initialize central static classes.
- TypeStore.RegisterLoadedAssemblies() scans loaded assemblies so Tripous can create registered types by name.
- Registry.RegisterDescriptors() registers lookups, locators, modules, forms and configuration properties.
- AppHost registers toolbar and sidebar commands.
- AppHost creates an admin user when SYS_APP_USER is empty.
- AppHost reads UseUsers from Config.
- AppHost automatically logs in the first active admin user when UseUsers is false.
- AppHost shows LoginDialog when UseUsers is true.
- AppHost creates the real MainWindow.
- Ui.MainWindow is changed from HiddenMainWindow to MainWindow.
- MainWindow is shown.

Database connection

- DbConnections.json stores database connection definitions.
- The default connection has Name = Default.
- Tripous uses the Default connection when no other connection is specified.
- This sample creates a SQLite database at [Data]/mini-crm.db3.
- [Data] is a Tripous folder token.
- [Data] resolves to the application Data folder under SysConfig.AppFolderPath.
- The Application Folder command opens SysConfig.AppFolderPath.

DbConnections.json example

{
  "List": [
    {
      "Name": "Default",
      "DbServerType": "Sqlite",
      "ConnectionString": "Data Source=[Data]/mini-crm.db3"
    }
  ]
}

SchemaVersion1

- SchemaVersion1 inherits from SchemaVersionDef.
- RegisterInternal() registers tables and seed statements.
- Version.AddTable() registers CREATE TABLE statements.
- Version.AddStatementAfter() inserts seed rows after table creation.
- Version 1 inserts ActivityType rows.
- Version 1 inserts SYS_NUMBER_SERIES rows.
- Version 1 inserts sample Customer, Contact and Activity rows.
- The Customer Code field uses the CUSTOMER CodeProvider.
- The seed data sets the next customer number to 3 because C-0001 and C-0002 are already inserted.
- Later versions may add new tables.
- Later versions may alter existing tables.
- Later versions may add seed data.
- AddStatementBefore() can run SQL before a version creates or alters tables.
- AddStatementAfter() can run SQL after a version creates or alters tables.
- Schema versions are useful for database migration, seed data, system defaults and data correction scripts.

Tables

- SYS_APP_USER stores application users.
- SYS_CONFIG stores application settings.
- SYS_LOG stores database log rows.
- SYS_NUMBER_SERIES stores code generation definitions.
- SYS_STR_RES stores string resources for future localization work.
- ActivityType stores small fixed activity types.
- Customer stores customer rows.
- Contact stores customer contact rows.
- Activity stores customer activity rows.

Database column tokens

- @NVARCHAR(size) maps to provider-specific Unicode text.
- @DATE maps to provider-specific date.
- @DATE_TIME maps to provider-specific date-time.
- @NBLOB_TEXT maps to provider-specific text blob storage.
- @BOOL maps to the provider-specific integer boolean SQL type.
- @NOT_NULL maps to provider-specific NOT NULL.
- @NULL maps to provider-specific NULL.
- Use @DATE_TIME, not @DATETIME.

Boolean database fields

- In Tripous, boolean database fields and boolean FieldDef fields are integer-backed 0/1 fields.
- DataFieldType.Boolean means an integer-backed boolean FieldDef value.
- TableDef.AddBoolean() creates an integer-backed FieldDef that the UI displays as a boolean editor.
- @BOOL maps through each SqlProvider.BoolSql property to the provider-specific integer boolean SQL type.
- Boolean filters use an All, True and False editor and emit SQL values 1 or 0.
- Boolean filters are validated against the real SELECT schema and must be backed by integer-compatible columns.
- Code should assign 0 or 1 to these fields when setting default database values.

LookupDef

- LookupDef describes a small value list.
- A lookup list usually feeds a ComboBox editor.
- A LookupDef may load from a table name.
- A LookupDef may load from custom SQL.
- A LookupDef may load from an enum type.
- A LookupDef may load from a custom LookupSource class.
- This sample registers ActivityType as a table-backed lookup.
- Activity.ActivityTypeId uses ActivityType as a lookup field.
- Lookup fields are best for small stable lists such as type, status, category and priority.

Locator

- A Locator is a searchable selector for an entity reference.
- A Locator is used when a ComboBox is not enough.
- A Locator is usually used for large or important tables such as Customer, Product, Account or Contact.
- A LocatorDef describes where the selectable rows come from.
- A LocatorDef may use a table name.
- A LocatorDef may use custom SQL.
- A LocatorDef declares the key field of the selected row.
- A LocatorDef declares the visible/searchable columns shown to the user.
- A LocatorDef may point to a FormDef, so the UI can know which form manages the selected entity.
- FieldDef.Locator connects an editable field to a LocatorDef.
- LocatorDef.Add() can map a source column to a target field through TargetField.
- When the user selects a locator row, Tripous copies mapped source values into target fields.
- The Customer locator maps Customer.Id to CustomerId.
- The Contact locator maps Contact.Id to ContactId.
- The Contact locator also maps Contact.CustomerId to CustomerId.
- That second Contact mapping lets an Activity select a Contact and also receive the matching CustomerId.
- A locator field should normally be backed by a TableDef join when the UI must show source-table display fields.
- Without the matching join, the key value may be assigned but the LocatorBox display fields may remain empty.
- This is why locators and joins are usually registered together.
- Lookup is for small lists.
- Locator is for searchable entity selection.

Locator and join mapping

- A locator has two jobs.
- The first job is to assign the key value, such as Customer.Id into Contact.CustomerId.
- The second job is to show human-readable values, such as Customer.Code and Customer.Name, in the LocatorBox sub-textboxes.
- The key assignment can work without a join.
- The visible sub-textboxes need matching joined fields in the target TableDef.
- TableDef.AddJoin() describes how the editable table joins to the referenced table.
- The own key field is the field in the editable table, such as Contact.CustomerId.
- The foreign table is the source table, such as Customer.
- The foreign alias controls the generated display aliases.
- A join alias Customer produces display field aliases such as Customer__Code and Customer__Name.
- A join alias Contact produces display field aliases such as Contact__FirstName and Contact__LastName.
- The LocatorDef visible fields should use aliases that match the join aliases.
- The locator key field maps to the editable key field, such as Customer.Id to CustomerId.
- The visible locator fields map to joined display fields, such as Customer.Code to Customer__Code.
- TableDef.CreateLocatorTargetFieldMap() builds this mapping at runtime.
- Locator.Assign() uses the mapping when the user selects a row.
- LocatorBox.RefreshTargetBoxes() uses the same mapping to display the selected row.
- If the locator alias and the join alias do not match, the key may be set but the display text boxes may not refresh.
- If the locator has visible fields but the TableDef has no matching join fields, the locator cannot display those values reliably.
- For Contact.CustomerId, the TableDef has a Customer join and the Customer locator uses Customer__Code and Customer__Name.
- For Activity.CustomerId, the TableDef has a Customer join and the Customer locator uses Customer__Code and Customer__Name.
- For Activity.ContactId, the TableDef has a Contact join and the Contact locator uses Contact__FirstName and Contact__LastName.
- The Contact locator also carries Contact.CustomerId as a hidden mapping so selecting a contact can fill Activity.CustomerId.
- A practical rule is that every locator field with visible columns should have a TableDef.AddJoin() with the same alias used by LocatorDef.Add().

ModuleDef

- ModuleDef describes a business module.
- A ModuleDef owns a TableDef.
- A ModuleDef owns one or more SelectDef objects.
- A ModuleDef may point to a custom DataModule class.
- This sample registers AppUser, SysConfig, Log, NumberSeries, ResourceStrings, ActivityType, Customer, Contact and Activity modules.
- SysConfig and ResourceStrings are registered as modules but are not shown as forms in the sidebar.
- Customer, Contact and Activity point to MiniCrmDataModule.
- AppUser points to AppUserDataModule.
- ActivityType uses the default Tripous.Data.DataModule.
- A module may have multiple list SELECT statements, each with its own filters.

FormDef

- FormDef connects a UI form name with a ModuleDef.
- DesktopRegistry.AddForm() registers forms shown in the Modules sidebar group.
- This sample has System, Setup and CRM form groups.
- System contains Users, Log and Number Series.
- Config and String Resources are not shown in the sidebar.
- CRM contains Customers, Contacts and Activities.
- Forms shown in the sidebar use AppForm.ClosableByUser = false.
- Content forms can be closed by the Close button or by middle-clicking the tab.

TableDef

- TableDef describes the editable item table.
- TableDef is not necessarily the same shape as the list SELECT.
- Customer list SELECT is a simple Customer list.
- Contact list SELECT joins Customer to show the customer name.
- Activity list SELECT joins Customer, Contact and ActivityType.
- AddString() creates text fields.
- AddIntegerLookupId() creates integer foreign key lookup fields.
- AddBoolean() creates integer-backed 0/1 boolean fields.
- AddDateTime() creates date-time fields.
- AddTextBlob().SetMemo() creates memo editors.
- TableDef.AddDetail() registers detail tables for the item page.
- Customer registers Contact and Activity as detail tables.
- Code fields are read-only in the UI.
- Customer.Code is generated by the CUSTOMER CodeProvider.
- Log fields are read-only in the UI.
- CreatedAt, UpdatedAt, LastLoginAt and PasswordChangedAt are read-only in the UI.

CodeProvider

- A CodeProvider generates sequential business codes.
- A CodeProvider is not the primary key.
- The primary key remains the Id field.
- The generated code is a user-visible business value such as C-0001.
- CodeProvider definitions are registered in DataRegistry.CodeProviders.
- This sample registers the CUSTOMER CodeProvider.
- The backing table is SYS_NUMBER_SERIES.
- SYS_NUMBER_SERIES.Code is the provider name.
- SYS_NUMBER_SERIES.Name is the display name.
- SYS_NUMBER_SERIES.Pattern controls code formatting.
- SYS_NUMBER_SERIES.ResetPeriodId controls when numbering resets.
- SYS_NUMBER_SERIES.NextNumber stores the next integer to use.
- SYS_NUMBER_SERIES.LastResetValue stores the last reset key.
- SYS_NUMBER_SERIES.IsActive enables or disables the series.
- A FieldDef uses CodeProvider to declare that its value must be generated by a named provider.
- In this sample, Customer.Code uses SetCodeProviderName("CUSTOMER").
- The same field is marked ReadOnlyEdit and ReadOnlyUI, because the user should not type the generated code.
- During commit, DataModule detects the configured CodeProvider.
- DataModule locks and increments the matching SYS_NUMBER_SERIES row.
- DataModule formats the next number with the Pattern.
- DataModule writes the generated value into the Code field before saving the row.
- Version 1 seeds C-0001 and C-0002 as sample customers.
- The CUSTOMER row in SYS_NUMBER_SERIES starts with NextNumber = 3 so the next new customer receives C-0003.

SelectDef

- SelectDef describes a list SELECT.
- The list SELECT feeds the DataForm grid.
- SelectDef.AddFilter() registers list filters.
- Customer filters by Code, Name, City and IsActive.
- Contact filters by Customer, LastName and IsPrimaryContact.
- Activity filters by Customer, Contact, ActivityDate, ActivityType and IsClosed.
- Hand-written SELECT statements should use SqlProvider helper methods for provider-specific SQL.
- This sample uses SqlProvider.Concat() for the Activity Contact display column because string concatenation differs between SQLite, SQL Server, MySQL and other providers.
- Boolean filters use the All, True and False editor.
- DateTime filters can use range conditions such as Between.

DataModule

- MiniCrmDataModule sets default values for new rows.
- AppUserDataModule creates the default admin user and loads users for login.
- AppUserDataModule gives newly inserted users the default password changeme.
- AppUserDataModule changes passwords for the current user and for administrator-managed users.
- The main toolbar has Change Password for the current user.
- The Users form has Set Password for the selected user.
- New Customer rows default IsActive to 1.
- New Customer rows default CreatedAt and UpdatedAt to DateTime.Now.
- New Contact rows default IsPrimaryContact to 0.
- New Activity rows default ActivityDate to DateTime.Now.
- New Activity rows default ActivityTypeId from MiniCrm.DefaultActivityTypeId.
- New Activity rows default IsClosed to 0.
- DataModules must not show UI or wait for user interaction.

AppFormPagerHandler

- AppFormPagerHandler manages a TabControl that hosts AppForm instances.
- The left handler hosts sidebar forms.
- The right handler hosts content forms.
- The sidebar handler shows CommandTreeViewForm.
- The content handler shows DataForm instances.
- AppHost keeps both handlers so commands can show forms from anywhere.

AppForm

- AppForm is the base class for forms hosted by AppFormPagerHandler.
- DataForm is an AppForm that displays a registered ModuleDef.
- CommandTreeViewForm is an AppForm that displays registered command groups.
- AppForm.ClosableByUser controls whether the user may close the form tab.
- Sidebar forms should normally set ClosableByUser to false.
- Content forms can be closed by the Close button or by middle-clicking the tab.
- The toolbar button Customer Modal shows the Customer DataForm as a modal dialog to demonstrate modal AppForm usage.

Configuration

- Config property definitions live in DataRegistry.ConfigProperties.
- Actual values are stored as rows in SYS_CONFIG.
- Scalar values use Value.
- Memo and object values use TextValue.
- The effective value resolution order is User, Company, System, DefaultValue.
- SysConfigModule is the DataModule used internally by Config.
- MiniCrm.AutoOpenCustomerList controls whether Customer opens at startup.
- MiniCrm.DefaultActivityTypeId controls the default ActivityTypeId used by MiniCrmDataModule.
- UseUsers controls login mode.
- When UseUsers is false, Mini CRM automatically logs in the first active user.
- When UseUsers is true, Mini CRM shows LoginDialog.
- The first generated admin account is admin/admin.
- Application Settings lets the user edit values at User, Company or System scope.

Manual test checklist

- Start the application.
- Confirm the main window opens maximized.
- Confirm the sample customers, contacts and activities exist.
- Confirm the sidebar contains General and Modules.
- Confirm Config and String Resources are not shown in Modules.
- Confirm Users, Log and Number Series are shown under System.
- Open Users.
- Edit a user row.
- Press Set Password.
- Confirm the selected user password can be changed.
- Open Application Settings.
- Confirm Mini CRM settings are visible.
- Set UseUsers to true.
- Restart the application.
- Login as admin with password admin.
- Press Change Password from the main toolbar.
- Confirm the current user password can be changed.
- Set UseUsers back to false when login testing is done.
- Close Application Settings with Esc.
- Open Customers.
- Insert a customer.
- Enter Code, Name, Email, Phone and City.
- Save the customer.
- Reopen the customer.
- Add a Contact detail row.
- Add an Activity detail row.
- Save the customer again.
- Open Contacts from the sidebar.
- Confirm the contact appears with its customer name.
- Insert a contact from the standalone Contacts form.
- Use the Customer locator to select the customer.
- Save the contact.
- Open Activities from the sidebar.
- Insert an activity.
- Use the Customer locator to select a customer.
- Use the Contact locator to select a contact.
- Confirm the Contact locator also fills CustomerId when possible.
- Select an ActivityType.
- Save the activity.
- Test Customer filters.
- Test Contact filters.
- Test Activity filters.
- Test boolean filters with All, True and False.
- Toggle SQL logging and confirm SQL appears in the log area.
- Open the application folder and confirm DbConnections.json and Data exist.

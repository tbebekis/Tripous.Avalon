# Forms

`FormDef` is a presentation descriptor.

It belongs to `Tripous.Desktop`, not to the core `Tripous.Data` model.

Applications that use only `Tripous` or `Tripous.Data`, such as services, background workers, web applications or WASM applications, do not need to register `FormDef` objects.

Desktop applications register forms so the presentation layer can open modules through a standard descriptor.

## Purpose

A form declaration tells the desktop layer:

- The form registration name.
- The module used by the form.
- The display title.
- The menu or command group.
- The form class to instantiate.
- The item page class to use.
- Whether the form is read-only.
- The security level required for access.

The form does not define the database schema.

The form does not define module fields.

It points to an already registered `ModuleDef`.

## Registering Forms

Forms are registered in `DesktopRegistry`.

```csharp
DesktopRegistry.AddForm("TodoTask", TitleKey: "ToDo", Module: "TodoTask", Group: "Modules");
```

The simplest form declaration is enough when the default Tripous data form is used.

The `Name` and `Module` are often the same.

```csharp
DesktopRegistry.AddForm("Customer", TitleKey: "Customers", Module: "Customer", Group: "CRM");
```

The form named `Customer` opens the module named `Customer`.

## Form To Module Relationship

`FormDef.Module` stores the registration name of the module used by the form.

The module must be registered before form references are resolved.

This is why the sample registry registers modules before forms:

```csharp
Version.RegisterModules();
Version.RegisterForms();
DesktopRegistry.Forms.UpdateReferences();
```

The form descriptor provides presentation metadata. The module descriptor provides data metadata.

## Default Data Forms

When `ClassName` is omitted, `FormDef` uses the default `DataForm` class.

```csharp
DesktopRegistry.AddForm("Activity", TitleKey: "Activities", Module: "Activity", Group: "CRM");
```

This is the normal case for modules that can be edited with the standard Tripous data form.

The default data form uses the module metadata:

- List SQL from `SelectDef`.
- Filters from `SelectDef`.
- Editable table structure from `TableDef`.
- Field metadata from `FieldDef`.
- Lookups and locators from their registered descriptors.

## List And Item Parts

The standard `DataForm` has two main parts.

- The list part.
- The item part.

The list part is used for browsing rows.

It uses the module list SQL and the module select definitions.

Typical list responsibilities include:

- Displaying rows.
- Applying filters.
- Selecting the current row.
- Opening a row for editing.
- Starting insert operations.
- Starting delete operations.

Filters belong to the list part.

In the desktop presentation, filters are shown in the left filter area and are built from `SelectDef` filter declarations.

The item part is used for viewing and editing one row.

It uses the module `TableDef` and `FieldDef` metadata.

Typical item responsibilities include:

- Displaying field editors.
- Inserting rows.
- Editing rows.
- Saving changes.
- Canceling changes.
- Deleting rows.
- Displaying detail tables.
- Using lookup and locator editors.

This split is important because list metadata and item metadata come from different descriptors.

- The list part is driven mainly by `SelectDef`.
- The item part is driven mainly by `TableDef` and `FieldDef`.

## Form Commands

Forms can be opened through commands generated from `FormDef`.

```csharp
foreach (FormDef FormDef in DesktopRegistry.Forms)
{
    Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
    cmdModules.Commands.Add(Cmd);
}
```

Applications may also register their own toolbar commands.

```csharp
Command cmdToDo = Command.Create("ToDo", "book_open.png", Cmd => ContentHandler.ShowDataForm("TodoTask"));
Command cmdToDoModal = Command.CreateAsync("ToDo Modal", "application_go.png", async Cmd => await ShowToDoModal());

AppRegistry.ToolBarCommands.AddRange([cmdToDo, cmdToDoModal]);
```

Custom forms may extend their own toolbar by overriding `CreateToolBar()`.

```csharp
protected override bool CreateToolBar()
{
    if (!base.CreateToolBar())
        return false;

    btnSetPassword = ToolBar.AddButton("change_password.png", "Set Password", async () => await SetPassword());
    ToolBar.PlaceControlAfter(btnSave, btnSetPassword);
    return true;
}
```

This keeps normal form behavior in the base `DataForm` while allowing application-specific actions.

## Custom Forms

A form may use a custom class.

```csharp
DesktopRegistry.AddForm("AppUser", TitleKey: "Users", Module: "AppUser", ClassName: typeof(AppUserForm).FullName, Group: "System", IsReadOnly: false);
```

This pattern is useful when the default data form is not enough.

Typical reasons include:

- Custom security behavior.
- Custom item editing.
- Special toolbar commands.
- Additional validation or workflow.
- Specialized presentation controls.

The custom form still points to a module. It does not replace module declaration.

## Item Page Class

`FormDef.ItemClassName` controls the item part class used by the form.

When omitted, it defaults to the standard `ItemPage`.

Applications may set `ItemClassName` when they need a custom editor inside a standard data form.

```csharp
FormDef Form = DesktopRegistry.AddForm("Customer", TitleKey: "Customers", Module: "Customer", Group: "CRM");
Form.ItemClassName = typeof(CustomerItemPage).FullName;
```

This keeps the form descriptor standard while replacing the item editor.

## Groups

`Group` is used when creating command groups, menus or navigation groups.

Examples from the samples:

```csharp
DesktopRegistry.AddForm("ActivityType", TitleKey: "Activity Types", Module: "ActivityType", Group: "Setup");
DesktopRegistry.AddForm("Customer", TitleKey: "Customers", Module: "Customer", Group: "CRM");
DesktopRegistry.AddForm("Log", TitleKey: "Log", Module: "Log", Group: "System");
```

The group is presentation metadata.

It helps the application organize forms for the user.

## Read-Only Forms

`IsReadOnly` marks a form as read-only.

```csharp
DesktopRegistry.AddForm("Log", TitleKey: "Log", Module: "Log", Group: "System", IsReadOnly: true);
```

Read-only form behavior belongs to the presentation layer.

Database constraints and business rules still belong to the database, data module and application logic.

## Security Level

`SecurityLevel` restricts access to the form.

```csharp
DesktopRegistry.AddForm("AppUser", TitleKey: "Users", Module: "AppUser", Group: "System", SecurityLevel: UserLevel.Admin);
```

The form security level controls whether a user may access the form in the desktop layer.

A module may also have its own `SecurityLevel`.

In that case, both descriptors should be treated as part of the access policy.

## Commands From Forms

`FormDef` can create a command that opens the form.

```csharp
Command Cmd = Form.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
```

This is how a registered form can become a menu item, toolbar button or navigation command.

The command uses:

- The form name.
- The form title.
- The form security level.
- The provided execution callback.
- The optional image file name.

## Reference Resolution

Forms refer to modules by name.

Those references are resolved after all forms and modules have been registered.

```csharp
DesktopRegistry.Forms.UpdateReferences();
```

This call belongs at the end of descriptor registration.

## Practical Rule

When declaring forms manually:

- Register modules first.
- Register forms after modules.
- Use the same form name and module name when there is no reason to separate them.
- Omit `ClassName` when the standard `DataForm` is enough.
- Use `ClassName` for a custom form.
- Use `ItemClassName` for a custom item page inside a standard form.
- Use `Group` to organize forms in menus and navigation.
- Use `IsReadOnly` for presentation-level read-only behavior.
- Use `SecurityLevel` for presentation-level access control.
- Call `DesktopRegistry.Forms.UpdateReferences()` after registration.

## Related Articles

- [Modules](modules.md)
- [Declaration Order](declaration-order.md)
- [Application Host](application-host.md)
- [Tables and Fields](tables-and-fields.md)

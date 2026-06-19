# Form Registration

Tripous.Desktop uses form registrations to connect UI forms with data modules.

A form registration is a `FormDef` stored in `DesktopRegistry.Forms`.

## Basic Registration

The usual registration is short.

```csharp
DesktopRegistry.AddForm(
    "Note",
    TitleKey: "Notes",
    Module: "Note",
    Group: "Samples");
```

This creates a form named `Note`, connected to the data module named `Note`.

If no custom class names are supplied, Tripous uses:

- `DataForm` as the form class;
- `ItemPage` as the item editor class;
- `ReferenceContextMenu` as the reference context menu class.

## FormDef

`FormDef` contains the desktop metadata for a form.

Important properties:

- `Name`, the form registry name.
- `TitleKey`, the title localization key.
- `Module`, the related `ModuleDef` name.
- `ClassName`, the `DataForm` class name.
- `ItemClassName`, the `ItemPage` class name.
- `ReferenceMenuClassName`, the reference context menu class name.
- `Group`, the navigation or menu group.
- `IsReadOnly`, whether editing is disabled.
- `SecurityLevel`, the minimum user level required to access the form.

The most important connection is `FormDef.Module`.

```csharp
FormDef FormDef = DesktopRegistry.Forms.Get("Customer");
ModuleDef ModuleDef = DataRegistry.Modules.Get(FormDef.Module);
```

## AddOrUpdateForm

Generated projects usually use `AddOrUpdateForm()`.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "Customer",
    TitleKey: "Customer",
    Module: "Customer",
    Group: "Sales");
```

When a form already exists, non-empty values update the existing definition. This is useful when generated registrations are later extended by hand-written partial registration code.

## Custom Form Class

Use `ClassName` when a form needs custom behavior but still follows the `DataForm` lifecycle.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "SalesInvoice",
    TitleKey: "SalesInvoice",
    Module: "SalesInvoice",
    ClassName: "SalesInvoiceForm",
    Group: "Sales");
```

The class is created through `TypeStore`, so it may be a full type name or an assembly-qualified type name.

```csharp
DataForm Form = DesktopRegistry.CreateDataForm("SalesInvoice");
```

## Custom Item Page

Use `ItemClassName` when the main form is still a normal `DataForm`, but the item editor needs custom controls or custom behavior.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "PurchaseInvoice",
    TitleKey: "PurchaseInvoice",
    Module: "PurchaseInvoice",
    ClassName: "PurchaseInvoiceForm",
    Group: "Purchases",
    ItemClassName: "TradeItemPage");
```

During `DataForm` initialization, Tripous creates the item page from `FormDef.ItemClassName`, assigns the `DataForm`, binds the page, and applies column visibility settings.

## Read-Only Forms

Use `IsReadOnly` for forms that should display data but not edit it.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "StockMovement",
    TitleKey: "StockMovement",
    Module: "StockMovement",
    Group: "Inventory",
    IsReadOnly: true);
```

This is common for balance, movement, log and other derived or audit-oriented forms.

## Security

`SecurityLevel` controls whether a user may access a form.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "AppUser",
    TitleKey: "AppUser",
    Module: "AppUser",
    ClassName: "AppUserForm",
    Group: "Setup",
    SecurityLevel: UserLevel.Admin);
```

`FormDef.CanAccess()` checks the current user level against the form security level.

## Form Commands

A registered form can create a command that opens it.

```csharp
foreach (FormDef FormDef in DesktopRegistry.Forms)
{
    Command Cmd = FormDef.CreateShowCommand(ShowFormFunc, ImageFileName: "table.png");
    AppRegistry.MenuCommands.Add(Cmd);
}
```

Application shells can group these commands by `FormDef.Group`.

## Form Context

When a `DataForm` is opened, `DataFormContext` resolves the registration.

It loads:

- the `FormDef`;
- the related `ModuleDef`;
- the `DataModule` instance;
- the form title;
- the caller control.

This context is what allows a form registration to drive both the UI and the data module lifecycle.

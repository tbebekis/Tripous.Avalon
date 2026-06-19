# Overview

Tripous.Desktop is the Avalonia UI layer of Tripous.

It sits above Tripous.Data and turns registered data descriptors into usable desktop forms, item pages, grids, lookup controls, locator controls, dialogs, menus and commands.

## Main Entry Points

- `Ui`
- `UiGlobalSettings`
- `DesktopRegistry`
- `FormDef`
- `AppForm`
- `DataForm`
- `ItemPage`
- `UiFactory`
- `AvaloniaAssets`
- `DesktopExceptionHandler`

## Ui

`Ui` is the central static helper class for the desktop layer.

It provides:

- access to the main window;
- file dialogs;
- input box dialog;
- UI thread dispatching;
- wait cursor helpers;
- tree view helpers;
- debug output through `LogBox` when available.

Example:

```csharp
Ui.MainWindow = MainWindow;

Ui.Post(() =>
{
    Ui.Debug("Background work completed.");
});
```

`Ui.Debug()` writes to `LogBox` when the log box is initialized. Otherwise, it writes to the normal debug output.

## UiGlobalSettings

`Ui.Settings` contains desktop layout and behavior settings.

```csharp
Ui.Settings.FormColumnCount = 2;
Ui.Settings.FormColumnWidth = 420;
Ui.Settings.FormMemoRowCount = 3;
Ui.Settings.DetailGridMinHeight = 240;
Ui.Settings.ShowDataFormLog = true;
```

Important settings:

- `FormColumnCount`
- `FormColumnWidth`
- `FormMemoRowCount`
- `FormMaxControlsPerColumn`
- `FormImageHeight`
- `FormImageStretch`
- `ShowIdColumnsInGrids`
- `DetailGridMinHeight`
- `ShowDataFormLog`

These settings affect generated item forms, detail grids and the DataForm log panel.

## Desktop Registry

Forms are registered in `DesktopRegistry.Forms` as `FormDef` instances.

```csharp
DesktopRegistry.AddForm(
    "Customer",
    TitleKey: "Customers",
    Module: "Customer",
    Group: "CRM");
```

A `FormDef` connects a desktop form to a data module.

It also defines:

- the `DataForm` class;
- the `ItemPage` class;
- the reference context menu class;
- the navigation group;
- read-only mode;
- required user level.

```csharp
DataForm Form = DesktopRegistry.CreateDataForm("Customer");
```

## AppForm And DataForm

`AppForm` is the base user control for application forms.

`DataForm` is the standard data-aware form. It hosts browser/item views, talks to a `DataModule`, executes insert/edit/delete/save/cancel workflows, and coordinates UI state.

Most application CRUD screens use `DataForm`.

## ItemPage

`ItemPage` is the item editor part of a `DataForm`.

It creates the editing controls for the top table and the detail grids. It also owns the current detail grid command handling, such as add/delete row actions for detail and subdetail grids.

## Generated Item UI

Tripous.Desktop can generate editing UI from `ModuleDef`, `TableDef` and `FieldDef`.

The descriptor metadata controls:

- which fields are visible;
- field captions;
- field order and grouping;
- control kind;
- lookup and locator behavior;
- memo and image controls;
- detail grid creation.

This is why the same Data Registry descriptors are important for both data access and desktop UI generation.

## Bindings And Grids

The desktop layer binds Avalonia controls to `DataRowView`, `MemTable`, `TableSet` and module context objects.

The grid binding code handles:

- column creation;
- visible and hidden fields;
- lookup and locator columns;
- row selection;
- detail grid synchronization.

## Locators And Reference Menus

Locator controls use Tripous.Data locator descriptors to search related records and assign reference values.

Reference controls can also show a context menu for common reference actions, such as opening the referenced form.

## Commands Menus And Dialogs

Tripous.Desktop includes command, menu and toolbar helpers used by application shells and data forms.

Dialog helpers cover common modal workflows, while `DesktopExceptionHandler` centralizes desktop exception handling and writes useful diagnostics to `LogBox`.

## LogBox

`LogBox` is a lightweight UI log target used by the desktop layer.

It is used by:

- `Ui.Debug()`;
- `DataForm` logging;
- locator diagnostics;
- desktop exception handling.

It is intentionally small, but useful while testing generated forms, locators and data workflows.

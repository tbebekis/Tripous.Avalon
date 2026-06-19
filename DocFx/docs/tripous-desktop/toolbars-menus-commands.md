# Toolbars, Menus And Commands

Tripous uses a small command model and a set of Avalonia helpers for toolbars and menus.
The core command metadata lives in `Tripous.Command`, while the Desktop layer turns commands and callbacks into UI controls.

## Main Types

- `Command` is the named application command descriptor.
- `AppRegistry` stores global menu and toolbar commands.
- `ToolBar` creates Avalonia toolbar controls.
- `MenuExtensions` creates menu items and checkable menu items.
- `GridCommand` is a separate command type for detail grid toolbars.
- `ReferenceContextMenu` is a separate menu type for lookup and locator references.

## Command

`Command` is a named application action.
It may execute a synchronous callback, execute an asynchronous callback, open a form, or contain child commands.

```csharp
Command Cmd = Command.Create(
    "Customers",
    "customer.png",
    Command =>
    {
        ShowCustomers();
        return null;
    });
```

Async commands use `CreateAsync()`.

```csharp
Command Cmd = Command.CreateAsync(
    "Import",
    "database_go.png",
    async Command =>
    {
        await ImportData();
        return null;
    });
```

Form commands store the form name in `Form`.

```csharp
Command Cmd = Command.CreateForm(
    "CustomerList",
    "Customer",
    ImageFileName: "customer.png");
```

Important command members are:

- `Name`, the command identifier.
- `Title`, inherited from `BaseDef`, used as the user-facing title.
- `ImageFileName`, used by toolbar buttons and command views.
- `Form`, the form opened by the command.
- `Commands`, child commands.
- `IsToggle`, used when a toolbar command should become a toggle button.
- `SecurityLevel`, used by `CanAccess()`.
- `CanExecuteFunc`, `ExecuteFunc`, and `ExecuteAsyncFunc`.

## AppRegistry

`AppRegistry` keeps global command lists.

```csharp
AppRegistry.ToolBarCommands.Add(Cmd);
AppRegistry.MenuCommands.Add(Cmd);
```

It can also search all registered commands, including child commands.

```csharp
Command Cmd = AppRegistry.GetCommand("Customers");
```

Use `MenuCommands` for application menus and `ToolBarCommands` for application-level toolbar commands.

## ToolBar Helper

`ToolBar` is a helper around a `StackPanel`.
It creates common toolbar controls and adds them to the panel.

```csharp
ToolBar ToolBar = new();
ToolBar.Panel = pnlToolBar;

ToolBar.AddButton(
    "table_refresh.png",
    "Refresh",
    async () => await RefreshList());
```

The helper supports:

- normal buttons.
- async buttons.
- drop-down buttons.
- toggle buttons.
- separators.
- text boxes.
- text blocks.
- labels.
- combo boxes.
- check boxes.
- `Command` buttons.

Images are loaded through `AvaloniaAssets.FindImage()`.
If no image is found, the button may show the tooltip text as content.

## Adding Commands To Toolbars

`ToolBar.Add(Command)` creates a button from a `Command`.
When `Command.IsToggle` is true, it creates a `ToggleButton`.

```csharp
ToolBar ToolBar = new();
ToolBar.Panel = pnlToolBar;
ToolBar.Add(Cmd);
```

`ToolBar.AddRange()` adds multiple commands.

```csharp
ToolBar.AddRange(AppRegistry.ToolBarCommands);
```

The created button stores the command in `Button.Tag`.
The command stores the button in `Command.Tag`.

## Repositioning Toolbar Controls

`ToolBar` can move controls after they have been added.
This is useful when a derived form wants to insert a button near an existing one.

```csharp
Button Button = ToolBar.AddButton("print.png", "Print", Print);
ToolBar.PlaceControlAfter(btnRefresh, Button);
```

There are matching methods for placing controls or separators before or after another toolbar item.

## DataForm Toolbar

`DataForm` builds its main toolbar with `ToolBar`.
The default buttons dispatch `DataFormAction` values through `Execute()`.

```csharp
ToolBar = new();
ToolBar.Panel = pnlToolBar;

btnList = ToolBar.AddButton(
    "table.png",
    "List (F5)",
    async () => await Execute(DataFormAction.List));

btnSave = ToolBar.AddButton(
    "disk.png",
    "Save (Ctrl+S)",
    async () => await Execute(DataFormAction.Save));
```

`DataForm` also creates a select-list toolbar with the select combo box, Execute button, and Clear Filter button.

## Menus

`MenuExtensions` adds compact helpers for Avalonia menu item lists.

```csharp
MenuItem FileMenu = MainMenu.Items.AddMenuItem("File");

FileMenu.AddMenuItem("Open", OpenFile);
FileMenu.AddSeparator();
FileMenu.AddMenuItem("Close", CloseForm);
```

Checkable menu items are supported too.

```csharp
FileMenu.AddCheckBoxMenuItem(
    "Show Id Columns",
    IsChecked: true,
    ToggleIdColumns);
```

Menu item helpers support both direct click handlers and simple `Action` callbacks.
They also allow a `Tag` value when the caller needs to attach command metadata or state.

## Command Tree View

`CommandTreeViewForm` is a small utility form for viewing and executing command trees.
Its toolbar uses the same `ToolBar` helper for Expand, Collapse, and Execute actions.

This is useful when commands are hierarchical through `Command.Commands`.

## Separate Command Systems

Tripous.Desktop also has focused command systems for specific UI surfaces.

- `GridCommand` is used by detail grid toolbars.
- `ReferenceMenuCommandContext` is used by reference context menus.
- `DataFormAction` is used by `DataForm` actions and keyboard shortcuts.

These are separate from `Tripous.Command`.
Use the focused command type when working inside that surface, and use `Command` for application-level commands.

## Practical Notes

- Use `Command` and `AppRegistry` for application-wide commands.
- Use `ToolBar` for creating toolbar controls in forms.
- Use `MenuExtensions` for simple menus and context menus.
- Use `GridCommand` for detail grid row actions.
- Keep UI command callbacks in the Desktop layer.

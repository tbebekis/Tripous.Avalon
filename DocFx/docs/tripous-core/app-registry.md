# AppRegistry And Command

`Command` represents a named application action.
`AppRegistry` is the global place where applications register menu and toolbar commands.

Together they provide a simple command model that can be used by desktop UI code, command trees, menus, and toolbars.

## Command Basics

A command has a `Name`, a localized title through `BaseDef`, an optional image, and optional execution callbacks.

```csharp
Command cmdAppFolder = Command.Create(
    "ShowAppFolder",
    "folder.png",
    Cmd =>
    {
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
        return null;
    });
```

The command name is the stable identifier.
The image file name is used by UI code when the command is displayed in a toolbar, menu, or command tree.

## Synchronous And Asynchronous Commands

Use `Command.Create()` for synchronous callbacks.

```csharp
Command cmdExit = Command.Create(
    "Exit",
    "door_out.png",
    Cmd =>
    {
        MainWindow.Close();
        return null;
    });
```

Use `Command.CreateAsync()` for asynchronous callbacks.

```csharp
Command cmdSettings = Command.CreateAsync(
    "Application Settings",
    "setting_tools.png",
    async Cmd =>
    {
        await ConfigDialog.ShowModal(MainWindow);
        return null;
    });
```

Both forms support `CanExecuteFunc`.

```csharp
cmdSettings.CanExecuteFunc = Cmd => Sys.User != null;
```

`Execute()` runs the synchronous callback.
`ExecuteAsync()` runs the asynchronous callback when one exists, otherwise it falls back to the synchronous callback.

## Form Commands

A command may point to a form by name.
This is useful when a descriptor or UI layer knows how to open forms.

```csharp
Command cmdCustomers = Command.CreateForm(
    "Customers",
    "Customer",
    ImageFileName: "table.png");
```

Desktop descriptors also create commands for forms.
Sample applications use this pattern to build a Modules menu from registered forms.

```csharp
Command cmdModules = new("Modules");

foreach (FormDef FormDef in DesktopRegistry.Forms)
{
    Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
    cmdModules.Commands.Add(Cmd);
}
```

## Command Groups

A command may contain child commands.
This turns the command into a menu or command-tree group.

```csharp
Command cmdGeneral = new("General");

cmdGeneral.Commands.AddRange([
    cmdAppFolder,
    cmdSettings,
    cmdExit
]);
```

`Command.HasChildren` returns true when a command has child commands.
`AppRegistry.GetCommandsAll()` walks both menu and toolbar command lists and includes child commands too.

```csharp
List<Command> Commands = AppRegistry.GetCommandsAll();
```

## AppRegistry

Applications register commands during startup.

```csharp
AppRegistry.MenuCommands.AddRange([cmdGeneral, cmdModules]);
AppRegistry.ToolBarCommands.AddRange([cmdCustomers, cmdAppFolder, cmdExit]);
```

`MenuCommands` are intended for menus and command trees.
`ToolBarCommands` are intended for toolbar surfaces.

Sample desktop applications create their toolbar directly from `AppRegistry.ToolBarCommands`.

```csharp
fToolBar = new();
fToolBar.Panel = pnlToolBar;
fToolBar.AddRange(AppRegistry.ToolBarCommands);
```

## Finding Commands

Use `FindCommand()` when a missing command is allowed.

```csharp
Command Group = AppRegistry.FindCommand("Views");

if (Group == null)
{
    Group = new Command("Views");
    AppRegistry.MenuCommands.Add(Group);
}
```

Use `GetCommand()` when the command is required.

```csharp
Command Exit = AppRegistry.GetCommand("Exit");
```

`CommandExists()` is a convenience method for checking by name.

```csharp
bool Exists = AppRegistry.CommandExists("Exit");
```

## Security Level

Commands may require a minimum `UserLevel`.

```csharp
Command cmdRegenerateDatabase = Command.CreateAsync(
    "Regenerate Database",
    "database_refresh.png",
    async Cmd =>
    {
        await RegenerateDatabase();
        return null;
    });

cmdRegenerateDatabase.SecurityLevel = UserLevel.Admin;
```

`CanAccess()` checks whether a user may see or execute the command.

```csharp
bool CanShow = cmdRegenerateDatabase.CanAccess(Sys.User);
```

Sample applications use this to filter commands before adding them to menus and toolbars.

```csharp
AppRegistry.ToolBarCommands.AddRange(Commands.Where(CanAccess));
```

## Toggle Commands

`IsToggle` marks a command as a Boolean toggle.
The command still executes like any other command; the flag lets UI code display it as a toggle action.

```csharp
Command cmdToggleLogSqlStatements = Command.Create(
    "Log Sql",
    "file_extension_log.png",
    Cmd =>
    {
        MainWindow.ToggleLogSqlStatements();
        return null;
    });

cmdToggleLogSqlStatements.IsToggle = true;
```

## Serialization

`Command` derives from `BaseDef`, but it is not meant to be serialized as a descriptor declaration.
Its `IsSerializable` property returns false.

Commands often hold delegates such as `ExecuteFunc` and `ExecuteAsyncFunc`.
Those callbacks are runtime objects and cannot be represented meaningfully in JSON.

## When To Use It

Use `Command` and `AppRegistry` for application actions that should appear in menus, toolbars, or command trees.

- Open a form.
- Show a dialog.
- Toggle a panel.
- Clear a log.
- Open an application folder.
- Run an administrative action.

Do not use `Command` as a general business operation object.
It is an application/UI command descriptor with runtime callbacks, not a domain service.

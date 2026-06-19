# Creating Tripous Avalonia Applications

This walkthrough explains the usual startup shape of a Tripous Avalonia desktop application.
The reference implementation is the `TemplateApp` folder at the solution root.

The template contains two projects:

- `TemplateApp/TemplateApp`, the Avalonia executable.
- `TemplateApp/TemplateApp.Data`, the data and registry library.

## Startup Shape

A Tripous Avalonia application usually starts in this order:

- configure `Sys`.
- load or create database connection settings.
- create the database when needed.
- register schema versions.
- execute schema creation.
- create the default `SqlStore`.
- initialize application libraries.
- register loaded types.
- register descriptors.
- register commands.
- create the real main window.
- initialize desktop exception handling.

In the template this flow lives in `AppHost.Start()`.

```csharp
InitializeConfigs();
await LoadConnectionStrings();
await CreateDatabase();
Registry.RegisterSchemas();
Schemas.Execute();
Store = SqlStores.CreateDefaultSqlStore();
InitializeLibraries();
TypeStore.RegisterLoadedAssemblies();
Registry.RegisterDescriptors();
RegisterCommands();
```

The hidden startup window is used as the owner for early dialogs.
The real `MainWindow` appears after configuration and registration are complete.

## Configure Sys

`InitializeConfigs()` sets the application mode and the main assembly.

```csharp
SysConfig.ApplicationMode = ApplicationMode.Desktop;
SysConfig.MainAssembly = typeof(AppHost).Assembly;
SysConfig.AppName = "TemplateApp";
SysConfig.CompanyName = "Tripous";
```

These values affect application folders, settings files, type discovery and desktop startup behavior.

## Database Connection

The template creates a default SQLite connection when no connection settings exist.

```csharp
DbConnectionInfo Result = new();
Result.Name = Sys.DEFAULT;
Result.DbServerType = DbServerType.Sqlite;
Result.ConnectionString = string.Format(
    DbServerType.Sqlite.GetTemplateConnectionString(),
    "[Data]/template-app.db3");
```

Applications may replace this with a first-run dialog, a fixed connection, or a connection editor workflow.

## Schema And Registry

`TemplateApp.Data` owns the registry code.
The executable calls it during startup.

```csharp
Registry.RegisterSchemas();
Schemas.Execute();
Registry.RegisterDescriptors();
```

The empty template registry already has the expected hooks:

- `RegisterLookupSources()`
- `RegisterLocators()`
- `RegisterModules()`
- `RegisterForms()`
- `RegisterConfigProperties()`

After descriptor registration, references are updated.

```csharp
DataRegistry.Modules.UpdateReferences();
DesktopRegistry.Forms.UpdateReferences();
```

## TypeStore

Tripous uses `TypeStore` when descriptors refer to classes by type name.
Register loaded assemblies after application libraries are initialized.

```csharp
InitializeLibraries();
TypeStore.RegisterLoadedAssemblies();
```

`TemplateDataLib.Initialize()` is intentionally empty.
It exists as a stable place to force the data library to load and to initialize future static services.

## Main Window

`MainWindow` contains:

- a main menu.
- a toolbar.
- a sidebar pager.
- a content pager.
- a log text box.

The window initializes `LogBox`, creates `AppFormPagerHandler` instances, creates menu and toolbar controls, and then calls `AppHost.InitializeUi()`.

```csharp
LogBox.Initialize(edtLog);
fSideBarHandler = new AppFormPagerHandler(pagerSideBar);
fContentHandler = new AppFormPagerHandler(pagerContent);
AppHost.InitializeUi(fSideBarHandler, fContentHandler);
```

The sidebar opens `CommandTreeViewForm`, which displays registered commands.

## Commands

Commands are registered in `AppHost.Commands.cs`.
The template includes basic commands for:

- opening the application folder.
- editing application settings.
- editing connection settings.
- toggling the log panel.
- clearing the log.
- toggling SQL statement logging.
- exiting the application.

Module commands are created from registered forms.

```csharp
foreach (FormDef FormDef in DesktopRegistry.Forms)
{
    Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
    cmdModules.Commands.Add(Cmd);
}
```

When an application adds forms to `DesktopRegistry.Forms`, they appear in the command tree and can be shown in the content pager.

## Adding The First Module

To turn the template into a real application:

- add schema versions or schema scripts in `TemplateApp.Data`.
- register schema versions in `RegisterSchemas()`.
- register modules and tables in `RegisterModules()`.
- register forms in `RegisterForms()`.
- add application-specific commands in `RegisterCommands()`.
- optionally auto-open a form in `InitializeUi()`.

The descriptor articles under Manual Application Declaration and Automatic Application Declaration describe the two ways to create those registrations.

## Practical Notes

- Keep data declarations in `TemplateApp.Data`.
- Keep Avalonia windows, commands and UI startup in `TemplateApp`.
- Do not show UI from data modules or data library code.
- Keep `AppHost` as the startup coordinator.
- Let `Registry` be the single place that registers schemas and descriptors.

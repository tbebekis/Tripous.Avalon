# Template Tripous Avalonia Application

`TemplateApp` is a minimal reference application for new Tripous Avalonia desktop projects.
It is intentionally empty, but it includes the startup and registration structure a real application needs.

## Folder Layout

```text
TemplateApp/
  TemplateApp/
    AppHost/
      AppHost.cs
      AppHost.Startup.cs
      AppHost.Commands.cs
      AppHost.Ui.cs
    App.axaml
    App.axaml.cs
    GlobalUsings.cs
    HiddenMainWindow.cs
    MainWindow.axaml
    MainWindow.axaml.cs
    Program.cs
    TemplateApp.csproj
  TemplateApp.Data/
    Registry/
      Registry.cs
    GlobalUsings.cs
    TemplateApp.Data.csproj
    TemplateDataLib.cs
```

## Projects

`TemplateApp` is the executable Avalonia application.
It references:

- `Tripous`
- `Tripous.Data`
- `Tripous.Desktop`
- `Tripous.Logging`
- `TemplateApp.Data`

`TemplateApp.Data` is the application data library.
It references the Tripous libraries and contains the descriptor registry.

## TemplateApp.Data

The data library owns the `Registry` class.
This keeps schema and descriptor registration out of the UI project.

```csharp
static public class Registry
{
    static public void RegisterSchemas()
    {
    }

    static public void RegisterDescriptors()
    {
        RegisterLookupSources();
        RegisterLocators();
        RegisterModules();
        RegisterForms();
        RegisterConfigProperties();

        DataRegistry.Modules.UpdateReferences();
        DesktopRegistry.Forms.UpdateReferences();
    }
}
```

The private registration methods are empty by design.
They are placeholders for the application declarations.

## AppHost

`AppHost` is the application coordinator.
It is split into partial files:

- `AppHost.cs`, shared state and logging.
- `AppHost.Startup.cs`, startup sequence.
- `AppHost.Commands.cs`, command registration.
- `AppHost.Ui.cs`, UI page initialization.

Important shared properties are:

- `HiddenMainWindow`
- `MainWindow`
- `AvaloniaDesktop`
- `SideBarHandler`
- `ContentHandler`
- `Store`

## Hidden Startup Window

The application starts with `HiddenMainWindow`.
This gives early dialogs an owner while the real application window is not ready yet.

```csharp
Desktop.MainWindow = AppHost.HiddenMainWindow;
Desktop.MainWindow.Opened += async (Sender, Args) => await AppHost.Start(Desktop);
```

After configuration and registration, `AppHost` creates and shows `MainWindow`.

## Main Window

The template main window contains:

- a menu.
- a toolbar.
- a sidebar tab control.
- a content tab control.
- a log panel.

`MainWindow` initializes `LogBox`, creates the two `AppFormPagerHandler` instances, and lets `AppHost` initialize sidebar pages.

## Registry Hooks

Use these hooks in `TemplateApp.Data/Registry/Registry.cs`.

```csharp
static void RegisterLookupSources()
{
}

static void RegisterLocators()
{
}

static void RegisterModules()
{
}

static void RegisterForms()
{
}

static void RegisterConfigProperties()
{
}
```

Manual applications fill these methods directly.
RegBuilder-based applications can call generated registry version code from these methods.

## Command Hooks

Use `AppHost.Commands.cs` for application commands.

The template already registers general commands and creates module commands from `DesktopRegistry.Forms`.

```csharp
Command cmdModules = new("Modules");
foreach (FormDef FormDef in DesktopRegistry.Forms)
{
    Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
    cmdModules.Commands.Add(Cmd);
}
```

Add application-specific command groups beside the general and module groups.

## Startup Checklist

When creating a new application from the template:

- rename namespaces from `TemplateApp`.
- set `SysConfig.AppName`.
- set `SysConfig.CompanyName`.
- choose the default connection behavior.
- register schema versions.
- register modules and forms.
- add application commands.
- decide whether a data form should auto-open at startup.

## Practical Notes

- Use the template as a reference, not as generated code.
- Keep the executable project focused on UI and startup.
- Keep data descriptors in the data project.
- Keep `Registry` as the single registration entry point.
- Keep `AppHost` as the single startup coordinator.

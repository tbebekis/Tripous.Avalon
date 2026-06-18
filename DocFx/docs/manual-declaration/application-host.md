# Application Host

`AppHost` is an application convention used by the sample applications.

It is not a Tripous core requirement.

Tripous does not require a class named `AppHost`, and the framework does not search for one automatically. The samples use `AppHost` because it gives the application a clear place to coordinate startup, registration, shared state and command setup.

## Purpose

An application host is a composition root.

It is the place where the application wires together:

- Tripous configuration.
- Database connection settings.
- Database creation.
- Schema registration and execution.
- Descriptor registration.
- Type discovery.
- Command registration.
- Main application state.
- Presentation startup, when the application has a UI.

Keeping those responsibilities in one explicit place makes startup easier to read and easier to debug.

## Not A Core Requirement

The `AppHost` class belongs to the application.

It is not part of the Tripous framework contract.

An application may choose a different name or a different structure:

- `Startup`
- `ApplicationBootstrapper`
- `Program`
- Dependency injection setup
- A host builder used by a service or web application

The important idea is not the class name.

The important idea is that the application should have a clear startup boundary where Tripous is configured and declarations are registered in the correct order.

## Sample Structure

The samples split `AppHost` into partial files.

- `AppHost.cs` keeps shared application state.
- `AppHost.Startup.cs` contains startup and registration flow.
- `AppHost.Commands.cs` registers commands.
- `AppHost.Ui.cs` contains presentation helpers.

This split is only for organization. It keeps the startup class readable as the sample grows.

## Shared State

The sample `AppHost.cs` stores application-wide runtime objects.

```csharp
static public partial class AppHost
{
    static public HiddenMainWindow HiddenMainWindow { get; private set; }
    static public MainWindow MainWindow { get; private set; }
    static public IClassicDesktopStyleApplicationLifetime AvaloniaDesktop { get; private set; }
    static public AppFormPagerHandler SideBarHandler { get; private set; }
    static public AppFormPagerHandler ContentHandler { get; private set; }
    static public SqlStore Store { get; private set; }
}
```

This is convenient for small sample applications.

Larger applications may replace this with dependency injection, service containers or explicit constructor parameters.

## Startup Flow

The main responsibility of `AppHost` is startup coordination.

The samples use this order:

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

This keeps the order visible.

- Configuration is initialized first.
- Connection settings are loaded before database work.
- The database is created before schema execution.
- Schema declarations are registered before `Schemas.Execute()`.
- Descriptors are registered after the schema exists.
- Commands are registered after descriptors are available.

This is the same order described in [Declaration Order](declaration-order.md).

## Configuration Initialization

The samples initialize central Tripous settings before doing any database or registration work.

```csharp
static void InitializeConfigs()
{
    SysConfig.ApplicationMode = ApplicationMode.Desktop;
    SysConfig.MainAssembly = typeof(AppHost).Assembly;
}
```

The exact settings depend on the application type.

A service application, web application or WASM application may use a different application mode or a different hosting setup.

The key point is that central Tripous configuration belongs early in startup.

## Database Setup

The samples keep database setup explicit.

```csharp
await LoadConnectionStrings();
await CreateDatabase();
Registry.RegisterSchemas();
Schemas.Execute();
Store = SqlStores.CreateDefaultSqlStore();
```

This sequence makes the database lifecycle clear:

- Load connection settings.
- Create a database if the provider supports it and it does not exist.
- Register schema versions.
- Execute pending schema work.
- Create the default `SqlStore`.

The store is created after schema execution because application code usually expects the database to be ready when the store is used.

## Descriptor Registration

After the database schema exists, the host registers descriptors.

```csharp
InitializeLibraries();
TypeStore.RegisterLoadedAssemblies();
Registry.RegisterDescriptors();
```

`InitializeLibraries()` is a sample hook for multi-assembly applications.

It can force assemblies to load before `TypeStore.RegisterLoadedAssemblies()` scans loaded types.

`Registry.RegisterDescriptors()` then registers modules, tables, fields, lookups, locators, forms and configuration properties.

## Presentation Startup

The sample applications are Avalonia desktop applications, so their `AppHost` also creates and shows the main window.

```csharp
RegisterCommands();
MainWindow = new MainWindow();
Ui.MainWindow = MainWindow;
MainWindow.Show();
```

This is presentation-specific code.

It is not required by Tripous.Data and it would look different in a service, web or WASM application.

For desktop applications, keeping presentation startup after registration ensures that forms, commands and descriptors are ready before the user can interact with the application.

## Why Use This Convention

The `AppHost` convention is useful because it:

- Makes startup order visible.
- Keeps framework configuration out of forms.
- Keeps schema and descriptor registration in one flow.
- Gives commands a clear registration point.
- Gives the application one place for shared runtime state.
- Scales from small samples to larger applications.

The convention is intentionally simple.

It provides structure without hiding the startup sequence.

## Related Articles

- [Overview](overview.md)
- [Declaration Order](declaration-order.md)
- [Modules](modules.md)
- [Forms](forms.md)

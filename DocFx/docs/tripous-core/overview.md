# Overview

Tripous Core is the foundation layer of Tripous.
It contains the small framework services and helper types used by the Data and Desktop layers.

Core does not know about databases, forms, or Avalonia controls.
Instead, it provides the shared infrastructure that those higher layers depend on.

## What Tripous Core Provides

Tripous Core covers a few broad areas:

- Application bootstrap and global settings.
- JSON serialization helpers.
- Dynamic objects and descriptor infrastructure.
- Type discovery and runtime type creation.
- Application command registration.
- Localization hooks.
- Embedded resource helpers.
- Observable collections.
- Security and password helpers.
- Common extension methods.

These pieces are intentionally small.
They are not separate frameworks; they are the common vocabulary used by the rest of Tripous.

## Application Foundation

`Sys`, `SysConfig`, and `SysGlobalSettings` form the first Core entry point.
They provide application-wide paths, startup configuration, global settings, current user context, logging hooks, and general utility methods.

```csharp
SysConfig.AppName = "TinyERP";
SysConfig.CompanyName = "Tripous";

Sys.Initialize();
```

The Data and Desktop layers assume this foundation exists.
For that reason, application startup normally initializes Core configuration before databases, registries, forms, or UI services.

## JSON And Settings

Tripous Core centralizes JSON handling through `Json`.
Settings classes normally derive from `SettingsBase`.

```csharp
public class AppSettings : SettingsBase
{
    public string CultureCode { get; set; } = "en-US";
}
```

This gives applications a consistent way to save and load JSON-backed settings under the application folder.

## Descriptor Infrastructure

Tripous uses descriptors to declare metadata.
Core provides the common descriptor contract through `IDef`, `BaseDef`, `DefList<T>`, and `ReadOnlyDefList<T>`.

```csharp
public class ReportDef : BaseDef
{
    public string SqlText { get; set; }
}
```

The Data layer builds on this model with modules, tables, fields, lookups, locators, and select definitions.
The Desktop layer builds on it with form and grid descriptors.

This is one of the central design ideas in Tripous:

- Declarations are plain descriptor objects.
- Descriptor lists provide name-based lookup.
- JSON can save and restore descriptor graphs.
- `UpdateReferences()` reconnects runtime links after loading.

## Runtime Type Discovery

Tripous often stores type names in descriptors.
Core resolves those names through `TypeStore` and `AppAssemblies`.

```csharp
[TypeStore]
public class CustomerForm : DataForm
{
}
```

At startup, applications register loaded types.

```csharp
TypeStore.RegisterLoadedAssemblies();
```

Later, Tripous can create instances by name.

```csharp
DataForm Form = TypeStore.CreateInstance<DataForm>("CustomerForm");
```

This is what allows generated or manual declarations to refer to forms, item pages, handlers, resolvers, and enum types without hard-coded constructor calls.

## Commands And UI Actions

Core defines `Command` and `AppRegistry`.
Desktop applications use them to register menu and toolbar actions.

```csharp
Command cmdExit = Command.Create(
    "Exit",
    "door_out.png",
    Cmd =>
    {
        MainWindow.Close();
        return null;
    });

AppRegistry.ToolBarCommands.Add(cmdExit);
```

The Core layer only defines the command model.
The Desktop layer decides how those commands appear in menus, toolbars, and command trees.

## Localization

`Texts` and `ILocalizer` provide a simple localization gateway.
Descriptors store title keys, and `Texts` turns those keys into display text.

```csharp
string Title = Texts.L("CustomerName");
```

If no localizer is configured, Tripous still produces readable fallback text such as `Customer Name`.
Applications may assign `Texts.Current` to provide real translations.

## Utility Helpers

Core also contains focused helper types:

- `DynamicClass` for dictionary-backed dynamic objects.
- `ResourceFiles` for embedded resource access.
- `TripousList<T>` for observable lists with validation hooks.
- `Sec` and `Passwords` for password hashing, encryption helpers, password generation, and validation.
- Extension methods for strings, lists, dictionaries, types, assemblies, streams, exceptions, and date/time values.

These helpers appear throughout Tripous code, but they remain independent from the higher layers.

## How To Read This Section

Start with the first articles in this order:

- `Sys, SysConfig and SysGlobalSettings`.
- `Json`.
- `SettingsBase`.
- `BaseDef and IDef`.
- `DefList and ReadOnlyDefList`.
- `TypeStore`.

Those articles explain the concepts that appear again in Tripous.Data and Tripous.Desktop.
The remaining articles cover the supporting helpers used around those core concepts.

# Framework Layers

Tripous is organized as a small number of clearly defined framework layers.

Each layer has a specific responsibility, its own namespace, its own configuration infrastructure and, where appropriate, its own registry system.

This structure is intentionally simple.

The objective is to make the framework easy to understand, navigate and extend without introducing unnecessary architectural complexity.

## A Small Number Of Layers

Many frameworks organize functionality into large numbers of namespaces, sub-namespaces and architectural subdivisions.

Tripous takes a different approach.

The framework prefers a small number of well-defined layers rather than a deeply fragmented structure.

This decision is deliberate.

While extensive namespace hierarchies may appear organized, they often increase navigation costs and make systems harder to understand.

A developer should not need to memorize dozens of namespaces in order to understand a framework.

The primary layers of Tripous are:

- `Tripous`
- `Tripous.Data`
- `Tripous.Logging`
- `Tripous.Desktop`

Together, these layers provide the foundation for building data-centric applications.

## Namespace As Architectural Boundary

In Tripous, a namespace is more than a naming convention.

Each major namespace represents an architectural boundary.

A namespace groups together concepts that belong to the same responsibility area.

This helps developers build a clear mental model of the framework.

When working with data access, developers primarily interact with `Tripous.Data`.

When working with desktop applications, they primarily interact with `Tripous.Desktop`.

This separation keeps responsibilities clear while avoiding unnecessary complexity.

## Common Layer Structure

Each major layer follows a similar organizational pattern.

A layer typically contains:

- A central static entry point
- A configuration class
- A global settings class
- A registry, when appropriate

Because the structure is consistent, developers can quickly understand how different parts of the framework are organized.

## The Core Layer

The `Tripous` namespace contains the framework core.

Important types include:

- `Sys`
- `SysConfig`
- `SysGlobalSettings`
- `AppRegistry`

`Sys` acts as the central entry point for core framework services.

`Sys.Settings` provides access to the application's global system settings through a `SysGlobalSettings` instance.

`AppRegistry` contains application-wide descriptors and registrations that belong to the framework core.

## The Data Layer

The `Tripous.Data` namespace contains the data access and metadata infrastructure.

Important types include:

- `Db`
- `DbConfig`
- `DbGlobalSettings`
- `DataRegistry`

`Db` acts as the central entry point for database-related functionality.

`Db.Settings` provides access to global database settings through a `DbGlobalSettings` instance.

`DataRegistry` contains registrations related to data infrastructure, including metadata definitions, lookup sources, locators, document handlers and other data-oriented descriptors.

## The Desktop Layer

The `Tripous.Desktop` namespace contains the desktop application layer.

The current implementation is based on Avalonia UI.

Important types include:

- `Ui`
- `UiConfig`
- `UiGlobalSettings`
- `DesktopRegistry`

`Ui` acts as the central entry point for desktop-related functionality.

`Ui.Settings` provides access to global user interface settings through a `UiGlobalSettings` instance.

`DesktopRegistry` contains registrations related to forms, commands and desktop infrastructure.

## The Logging Layer

The `Tripous.Logging` namespace contains the framework's logging infrastructure.

Important types include:

- `Logger`
- `LogGlobalSettings`

`Logger` acts as the central entry point for logging functionality.

`Logger.Settings` provides access to logging configuration through a `LogGlobalSettings` instance.

Unlike other layers, the logging layer does not require a registry because its responsibilities are considerably narrower.

## Registries And Definitions

One of the defining characteristics of Tripous is the use of registries and definition classes.

The framework frequently describes application structure using descriptor objects rather than procedural code.

Most descriptor classes derive from a common base class.

```csharp
public class BaseDef
{
    public virtual string Name { get; set; }
    public virtual string TitleKey { get; set; }
    public virtual string Title => Texts.L(TitleKey);
}
```

Examples of descriptor classes include:

- `ModuleDef`
- `FormDef`
- `LookupDef`
- `LocatorDef`
- `CommandDef`
- `ConfigPropertyDef`

A descriptor is not a business object.

It is a metadata object that describes part of an application's structure.

Registries act as centralized repositories for these descriptors.

A simplified view of the `DataRegistry` illustrates the idea.

```csharp
static public class DataRegistry
{
    static public DefList<ModuleDef> Modules { get; }
    static public DefList<LookupDef> Lookups { get; }
    static public DefList<LocatorDef> Locators { get; }

    static public ModuleDef AddModule(...)
    static public LookupDef AddLookupSource(...)
    static public LocatorDef AddLocator(...)
}
```

The framework uses registries to discover, manage and retrieve application definitions at runtime.

This approach improves discoverability, extensibility and consistency throughout the framework.

It also supports plugin-style extension without requiring widespread code modifications.

## Type Discovery

Descriptors often reference implementation classes using type names.

For example, a module definition may reference a data module class, a form definition may reference a desktop form class and a document definition may reference a document handler class.

To support this model, Tripous includes a centralized type registry called `TypeStore`.

`TypeStore` is responsible for discovering, registering, resolving and instantiating application types.

Types become discoverable by decorating them with the `TypeStoreAttribute`.

```csharp
[TypeStore]
public class SalesOrderForm: DataEntryForm
{
}
```

During application startup, assemblies are scanned and eligible types are registered automatically.

Later, the framework can resolve or create instances dynamically.

```csharp
Type FormType = TypeStore.Resolve("SalesOrderForm");

object Form = TypeStore.CreateInstance("SalesOrderForm");
```

This mechanism is one of the foundations of the framework's declarative architecture.

Definitions can reference classes by name while the framework remains responsible for locating and instantiating the corresponding implementation types.

Together, descriptors, registries and the type store form the metadata infrastructure that drives much of the framework's runtime behavior.

## Why Static Entry Points?

Developers sometimes associate static classes with poor design.

Tripous does not share that assumption.

The framework uses static entry points deliberately and in a controlled manner.

Some concepts are naturally application-wide:

- Configuration
- Global settings
- Registries
- Logging infrastructure
- Database provider services
- Type discovery

Representing such concepts through central static entry points makes them visible, predictable and easy to locate.

Examples include:

- `Sys`
- `Db`
- `Ui`
- `Logger`

These classes are intended to expose infrastructure services.

They are not intended to contain application-specific business logic.

## Infrastructure, Not Business Logic

The central static classes are part of the framework's infrastructure.

They provide access to services, settings and registrations that belong to the framework itself.

Business logic remains the responsibility of application code.

This distinction is important.

The framework provides mechanisms.

Applications provide behavior.

Keeping those responsibilities separate helps preserve clarity and maintainability.

## Dependency Direction

The framework layers follow a simple dependency model.

Higher layers may depend on lower layers.

Lower layers should not depend on higher layers.

As a result:

- `Tripous` forms the foundation.
- `Tripous.Data` builds upon the foundation.
- `Tripous.Logging` provides supporting infrastructure.
- `Tripous.Desktop` builds upon the lower layers to provide desktop application functionality.

This dependency structure helps keep the framework modular and maintainable.

## Simplicity As A Design Goal

The overall architecture of Tripous reflects one of the framework's core principles:

Simplicity over complexity.

The framework intentionally favors a small number of clearly defined layers, predictable entry points and centralized infrastructure.

The goal is not to create an elaborate architecture.

The goal is to create an architecture that developers can understand, navigate and maintain for many years.

## Related Articles

- [What Is Tripous?](what-is-tripous.md)
- [History and Evolution](history-and-evolution.md)
- [Design Philosophy](design-philosophy.md)
- [Tripous and Data](tripous-and-data.md)

 

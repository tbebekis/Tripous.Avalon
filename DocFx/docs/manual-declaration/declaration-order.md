# Declaration Order

Manual declaration is explicit, but the order still matters.

Tripous registration has two separate phases:

- Schema registration and execution.
- Descriptor registration and reference resolution.

The schema phase prepares the database. The descriptor phase tells Tripous how the application works with that database.

## Startup Order

The sample applications follow this startup order:

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

The important steps are:

- `InitializeConfigs()` prepares application-level Tripous settings.
- `LoadConnectionStrings()` loads or creates database connection settings.
- `CreateDatabase()` creates the physical database when needed.
- `Registry.RegisterSchemas()` registers schema versions.
- `Schemas.Execute()` creates or upgrades the database schema.
- `SqlStores.CreateDefaultSqlStore()` creates the default store after the database exists.
- `InitializeLibraries()` gives the application a place to initialize external assemblies.
- `TypeStore.RegisterLoadedAssemblies()` makes loaded types discoverable.
- `Registry.RegisterDescriptors()` registers modules, fields, forms, lookups, locators and settings.
- `RegisterCommands()` registers application commands after descriptors are available.

The database schema is executed before descriptors are registered because descriptors describe database objects that must already exist.

## Schema Order

Schema versions are registered first.

```csharp
static public void RegisterSchemas()
{
    foreach (SchemaVersionDef Version in fSchemaVersionList)
        Version.Register();
}
```

Each `SchemaVersionDef` registers its tables and seed statements.

The schema executor then compares registered schema versions with the database version and executes the missing schema work.

In the samples this happens through:

```csharp
Registry.RegisterSchemas();
Schemas.Execute();
```

This keeps schema creation separate from descriptor registration.

## Descriptor Order

After the schema is ready, descriptors are registered.

The sample registry uses this order:

```csharp
static public void RegisterDescriptors()
{
    foreach (RegistryVersion Version in fRegistryVersionList)
    {
        Version.RegisterLookupSources();
        Version.RegisterLocators();
        Version.RegisterModules();
        Version.RegisterForms();
        Version.RegisterConfigProperties();
    }

    DataRegistry.Modules.UpdateReferences();
    DesktopRegistry.Forms.UpdateReferences();
}
```

The order is intentional.

- Lookup sources are registered before modules because fields may reference lookups by name.
- Locators are registered before modules because fields may reference locators by name.
- Modules are registered before forms because forms point to modules.
- Forms are registered after modules because they expose modules to a presentation layer.
- Configuration property definitions may be registered with the rest of the descriptor metadata.
- `UpdateReferences()` resolves descriptor links after all names are known.

This allows descriptors to refer to each other by name while still keeping registration readable.

## Registry Versions

The samples group descriptors into `RegistryVersion` classes.

```csharp
public class RegistryVersion
{
    public virtual void RegisterModules()
    {
    }

    public virtual void RegisterForms()
    {
    }

    public virtual void RegisterLookupSources()
    {
    }

    public virtual void RegisterLocators()
    {
    }

    public virtual void RegisterConfigProperties()
    {
    }

    public virtual int VersionNumber => -1;
}
```

This is an application convention, not a Tripous core requirement.

It keeps registration code organized by application version and makes it easier to inspect what was added over time.

## Reference Resolution

Many declarations refer to other declarations by name.

Examples:

- A lookup field references a `LookupDef`.
- A locator field references a `LocatorDef`.
- A form references a `ModuleDef`.
- A detail table references a master table.
- A joined field references a joined table.

Those references cannot be fully resolved until all descriptors have been registered.

For that reason the samples call:

```csharp
DataRegistry.Modules.UpdateReferences();
DesktopRegistry.Forms.UpdateReferences();
```

This final step connects descriptor objects after registration is complete.

## Practical Rule

Use this rule when writing manual registration:

- Register database schema first.
- Execute schema creation or upgrade.
- Register lookup sources.
- Register locators.
- Register modules and their tables, fields, select definitions and filters.
- Register forms or other presentation descriptors.
- Register configuration property definitions.
- Resolve descriptor references.
- Register commands after descriptors exist.

This order keeps startup predictable and makes registration failures easier to diagnose.

## Related Articles

- [Overview](overview.md)
- [Application Host](application-host.md)
- [Modules](modules.md)
- [Lookups](lookups.md)
- [Locators](locators.md)

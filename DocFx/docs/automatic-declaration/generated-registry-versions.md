# Generated Registry Versions

Generated registry version files register Tripous application descriptors for a specific application version.

They are the descriptor side of automatic declaration.

Schema versions describe database structure.

Registry versions describe application registration.

## Artifact Set

For a version `N`, the builder generates:

- `RegistryVersionN.cs`
- `RegistryVersionN.Modules.cs`
- `RegistryVersionN.Forms.cs`
- `RegistryVersionN.Lookups.cs`
- `RegistryVersionN.Locators.cs`
- `RegistryVersionN.CodeProviders.cs`

The files are partial class files for the same registry version class.

For example:

```csharp
public partial class RegistryVersion2: RegistryVersion
{
    public override int VersionNumber { get; } = 2;
}
```

## Base Class

TinyERP uses a handwritten `RegistryVersion` base class.

It defines the registration methods each generated version may override:

```csharp
public class RegistryVersion
{
    public virtual void RegisterModules() {}
    public virtual void RegisterForms() {}
    public virtual void RegisterLookups() {}
    public virtual void RegisterLookupSources() {}
    public virtual void RegisterLocators() {}
    public virtual void RegisterCodeProviders() {}
    public virtual int VersionNumber { get; } = -1;
}
```

The generated partial files override only the methods they need.

## Version Number

`RegistryVersionN.cs` contains the version number.

```csharp
public override int VersionNumber { get; } = 2;
```

This keeps descriptor registration aligned with the application version that produced it.

The registry version number does not execute database schema.

It identifies a batch of descriptor declarations.

## Partial Files

The builder splits descriptor registration by responsibility.

`RegistryVersionN.Modules.cs` registers:

- modules
- table descriptors
- list select definitions
- joins
- fields
- filters

`RegistryVersionN.Forms.cs` registers:

- forms
- form classes
- groups
- item page classes
- read-only form declarations

`RegistryVersionN.Lookups.cs` registers:

- table-backed lookups
- enum-backed lookup sources
- class-backed lookup sources

`RegistryVersionN.Locators.cs` registers:

- locator definitions
- base locator table mapping
- locator form references
- locator class names

`RegistryVersionN.CodeProviders.cs` registers:

- normal code provider names
- draft code provider names

## Application Registration

The application decides which registry versions participate in startup.

TinyERP coordinates this in `Registry.cs`.

`Registry.cs` is handwritten application code.

Adding a new generated registry version to the list is the developer's responsibility.

The same rule applies to generated schema versions in `SchemaVersionList`.

```csharp
RegistryVersionList.AddRange([
    new RegistryVersion1(),
    new RegistryVersion2()
]);
```

During descriptor registration, TinyERP iterates the list in order.

```csharp
foreach (RegistryVersion Version in RegistryVersionList)
{
    Version.RegisterLookups();
    Version.RegisterLookupSources();
    Version.RegisterLocators();
    Version.RegisterCodeProviders();
    Version.RegisterModules();
    Version.RegisterForms();
}
```

The order matters.

Lookups and locators are registered before modules and forms that may reference them.

## Generated And Handwritten Code

Generated registry versions contain structural declarations.

Handwritten registry files may still extend or adjust those declarations after generated registration.

TinyERP calls handwritten update methods after generated descriptor registration:

- `UpdateLookups()`
- `UpdateLocators()`
- `UpdateForms()`
- `UpdateModules()`

This is the preferred extension model.

Generated files provide the baseline.

Handwritten files add application-specific adjustments.

## Manual Edits

Generated registry version files are output.

Do not edit them manually.

When generated descriptor registration needs to change:

- change the `Schema.sql` metadata
- run the Registration Builder
- review the generated diff
- place manual extensions in handwritten files

## Practical Rule

Use generated registry versions to keep structural descriptors synchronized with schema metadata.

Use handwritten registry files for deliberate application-specific behavior.

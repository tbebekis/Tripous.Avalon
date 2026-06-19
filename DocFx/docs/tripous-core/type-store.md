# TypeStore

`TypeStore` is the central registry Tripous uses when an application type must be found by name.
It is the bridge between declaration metadata and real .NET types.

The usual pattern is:

- Mark a type with `[TypeStore]`.
- Load the assembly that contains the type.
- Register loaded assemblies.
- Resolve or create the type later by name.

## Discoverable Types

A type becomes discoverable when it is decorated with `TypeStoreAttribute`.

```csharp
/// <summary>
/// Represents a custom price resolver.
/// </summary>
[TypeStore]
public class PriceResolver : IPriceResolver
{
}
```

`TypeStoreAttribute` can be used on classes, enums, interfaces, and structs.
Tripous scans assemblies and registers only the marked types.

## Application Startup

Applications normally register loaded assemblies during startup, after the required libraries have been initialized.

```csharp
InitializeLibraries();
TypeStore.RegisterLoadedAssemblies();
Registry.RegisterDescriptors();
```

This is why many sample libraries contain a small `Initialize()` method.
Calling such a method forces .NET to load the assembly before `TypeStore.RegisterLoadedAssemblies()` scans the currently loaded assemblies.

```csharp
/// <summary>
/// Initializes the library.
/// </summary>
static public void Initialize()
{
}
```

Without loading the assembly first, the types may exist in the project but still be invisible to `TypeStore`.

## Resolving A Type

`TypeStore.Get()` returns a type or throws when it cannot be found.
This is useful when the type is required.

```csharp
Type EnumType = TypeStore.Get("UserLevel");

Table.AddEnumLookupId("UserLevelId", "UserLevel", EnumType, Flags: FieldFlags.Required);
```

Generated registry code uses this pattern for enum-backed lookup fields.
The schema can refer to the enum by name, while the runtime resolves the actual enum type.

`TypeStore.Find()` is softer.
It returns the type when it exists, otherwise it returns `null`.

```csharp
Type ClassType = TypeStore.Find("PriceResolver");

if (ClassType != null)
{
    // Use the resolved type.
}
```

## Creating Instances

The main practical use of `TypeStore` is creating objects from class names stored in descriptors.

```csharp
DataForm Form = TypeStore.CreateInstance<DataForm>(ClassName);
```

Tripous.Desktop uses this pattern when a `FormDef` creates a data form.
The descriptor stores the class name, and `TypeStore` creates the actual form class.

```csharp
public DataForm Create() => TypeStore.CreateInstance<DataForm>(ClassName);
```

The same pattern is used for `ItemPage` classes and reference context menus.

```csharp
ItemPage Page = TypeStore.CreateInstance<ItemPage>(FormDef.ItemClassName);
ReferenceContextMenu Menu = TypeStore.CreateInstance<ReferenceContextMenu>(ReferenceMenuClassName);
```

## Base Type Checks

`CreateInstance<T>()` and `Resolve()` can check that the resolved type is assignable to an expected base type or interface.
That protects descriptor-driven code from creating an incompatible type.

```csharp
IPriceResolver Resolver = TypeStore.CreateInstance<IPriceResolver>(ClassName);
```

If `ClassName` points to a type that does not implement `IPriceResolver`, `TypeStore` throws.

## Name Resolution

`TypeStore` tries multiple ways to resolve a name:

- `Type.GetType()`.
- Exact full name lookup in the registered store.
- Simple class name lookup among registered types.
- Application assembly lookup through `AppAssemblies`.

Simple names are convenient, but they must be unique.
If more than one registered type has the same simple class name, `TypeStore` throws and reports the matching full names.

## When To Use It

Use `TypeStore` when application metadata stores type names instead of direct type references.

- Form descriptors that create forms.
- Item page descriptors.
- Reference context menus.
- Enum-backed lookup fields.
- Resolver or handler classes selected by configuration.

Do not use it when normal dependency injection or direct construction is clearer.
`TypeStore` is most useful at the declaration boundary, where Tripous turns metadata into runtime objects.

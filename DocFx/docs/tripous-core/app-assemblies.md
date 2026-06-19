# AppAssemblies

`AppAssemblies` identifies the assemblies that belong to the application.
Tripous uses it when it has to search loaded assemblies without scanning every framework, database provider, or UI dependency.

The class is mainly used by `TypeStore`.
When `TypeStore` cannot resolve a class name from its own registered types, it asks `AppAssemblies` to search application assemblies.

## Application Assemblies

An application normally has many assemblies loaded:

- Tripous assemblies.
- Application assemblies.
- .NET assemblies.
- Avalonia assemblies.
- Database provider assemblies.
- Other third-party dependencies.

Most discovery code wants only the first two groups.
`AppAssemblies.GetApplicationAssemblies()` returns loaded assemblies after excluding known framework and third-party names.

```csharp
List<Assembly> Assemblies = AppAssemblies.GetApplicationAssemblies();

foreach (Assembly Assembly in Assemblies)
{
    Type[] Types = Assembly.GetTypesSafe();
}
```

`Sys` exposes the same helper as a convenience method.

```csharp
List<Assembly> Assemblies = Sys.GetApplicationAssemblies();
```

## Exclusion Rules

`AppAssemblies` excludes assemblies whose names start with known prefixes such as `System`, `Microsoft`, `Avalonia`, `mscorlib`, and `netstandard`.
It also excludes assemblies whose names contain known provider or rendering names such as `Npgsql`, `MySql`, `Oracle`, `SkiaSharp`, and `HarfBuzzSharp`.

Applications can add more exclusion rules when needed.

```csharp
AppAssemblies.AddExcludeStart("MyCompany.Tools");
AppAssemblies.AddExcludeContaining("DesignTime");
```

Use prefix exclusions for whole assembly families.
Use containing exclusions for smaller fragments that may appear in the middle of an assembly name.

## Finding A Class Type

`FindApplicationClassType()` searches application assemblies for a class.
It first tries the full type name and then the simple class name.

```csharp
Type FormType = AppAssemblies.FindApplicationClassType("SalesOrderForm", typeof(DataForm));
```

The optional base type limits the search to compatible classes.
In the example above, a matching type must be assignable to `DataForm`.

This is the fallback used by `TypeStore`.

```csharp
Type ClassType = TypeStore.Resolve("SalesOrderForm", typeof(DataForm));
```

If `TypeStore` does not already know the type, it can still find it through `AppAssemblies`, as long as the assembly is loaded and considered an application assembly.

## Simple Names Must Be Unique

Simple class names are convenient in descriptors, but they must be unique across application assemblies.
If more than one class has the same simple name, `FindApplicationClassType()` throws and reports all matching full names.

Use full type names when there is any chance of ambiguity.

```csharp
Type FormType = AppAssemblies.FindApplicationClassType("MyApp.Forms.SalesOrderForm", typeof(DataForm));
```

## When To Use It

Most application code does not call `AppAssemblies` directly.
It usually uses higher-level APIs such as `TypeStore`.

Call `AppAssemblies` directly when writing infrastructure code that must inspect application assemblies.

- Type discovery.
- Descriptor registration.
- Plugin-style scanning.
- Diagnostics that list application types.

The important condition is that the assembly must already be loaded.
`AppAssemblies` does not load project references by itself; it filters and scans what the current `AppDomain` already contains.

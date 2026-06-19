# ResourceFiles

`ResourceFiles` is a helper for reading embedded resource files from an assembly.
It wraps the standard .NET manifest resource APIs and gives Tripous code a simpler way to find resources by folder and file name.

Use it when files are compiled into an assembly as embedded resources and must be read at runtime.

## Embedded Resource Names

.NET stores embedded resources by manifest resource name.
That name usually combines the default namespace, folder path, and file name.

For example, a file like this:

```text
Sql/Init.sql
```

may become a manifest resource named like this:

```text
MyApp.Sql.Init.sql
```

`ResourceFiles` helps avoid hard-coding that full string everywhere.

## Listing Resources

Use `GetResourceFilePaths()` when you need to inspect the resources embedded in an assembly.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

string[] ResourcePaths = ResourceFiles.GetResourceFilePaths(Assembly);

foreach (string ResourcePath in ResourcePaths)
{
    Console.WriteLine(ResourcePath);
}
```

This is useful during diagnostics when a resource cannot be found.

## Finding A Resource

`FindResourcePath()` returns the full manifest resource path.
The simple overload matches by suffix, using folder path and file name.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

string ResourcePath = ResourceFiles.FindResourcePath(Assembly, "Sql", "Init.sql");
```

The overload with `BaseNamespace` expects the exact generated resource name.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

string ResourcePath = ResourceFiles.FindResourcePath(Assembly, "MyApp", "Sql", "Init.sql");
```

Use the exact overload when the base namespace is known and stable.
Use the simpler overload when the caller only knows the folder and file name.

## Checking Existence

Use `ResourceFileExists()` before reading a resource when a missing file is acceptable.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

bool Exists = ResourceFiles.ResourceFileExists(Assembly, "Sql", "Init.sql");

if (Exists)
{
    string Sql = ResourceFiles.GetResourceFileText(Assembly, "Sql", "Init.sql");
}
```

The method uses the same simple suffix matching as the simple `FindResourcePath()` overload.

## Reading Text

`GetResourceFileText()` reads the resource bytes and returns UTF-8 text.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

string Sql = ResourceFiles.GetResourceFileText(Assembly, "MyApp", "Sql", "Init.sql");
```

There is also a simple overload without the base namespace.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

string Sql = ResourceFiles.GetResourceFileText(Assembly, "Sql", "Init.sql");
```

If the resource is not found, the method returns an empty string.
That is convenient for optional text resources, but required resources should usually be checked explicitly.

```csharp
string Sql = ResourceFiles.GetResourceFileText(Assembly, "Sql", "Init.sql");

if (string.IsNullOrWhiteSpace(Sql))
    throw new TripousException("Required resource not found: Sql/Init.sql");
```

## Reading Bytes Or Streams

Use `GetResourceFileData()` for binary resources.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

byte[] Data = ResourceFiles.GetResourceFileData(Assembly, "Images", "Logo.png");
```

Use `GetResourceFileStream()` when the caller wants to consume the resource as a stream.

```csharp
Assembly Assembly = typeof(AppHost).Assembly;

using Stream Stream = ResourceFiles.GetResourceFileStream(Assembly, "MyApp", "Images", "Logo.png");

if (Stream == null)
    throw new TripousException("Required resource not found: Images/Logo.png");
```

The stream belongs to the caller and should be disposed.

## When To Use It

Use `ResourceFiles` for small files that naturally travel with the assembly.

- SQL scripts.
- Metadata files.
- Text templates.
- Small binary assets.
- Diagnostic resource inspection.

Do not use it for user-editable files or files that must be replaced without recompiling the application.
Those belong in normal file storage, not embedded resources.

## Notes

The assembly parameter is important.
`ResourceFiles` reads resources from the assembly you pass to it.

```csharp
Assembly Assembly = typeof(SomeTypeInTheTargetAssembly).Assembly;
```

Use a type that lives in the assembly containing the embedded resource.
Passing a type from another assembly is a common reason for missing resources.

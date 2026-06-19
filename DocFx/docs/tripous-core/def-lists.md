# DefList And ReadOnlyDefList

`DefList<T>` is the standard Tripous collection for descriptors.
It is a named, observable list where every item implements `IDef`.

`ReadOnlyDefList<T>` is a thin read-only wrapper around a `DefList<T>`.
It exposes lookup and enumeration without exposing mutation methods.

## Named Descriptor Lists

Descriptors are usually addressed by `Name`.
`DefList<T>` builds that rule into the collection.

```csharp
DefList<Command> Commands = new();

Commands.Add(new Command { Name = "Save" });
Commands.Add(new Command { Name = "Delete" });

Command SaveCommand = Commands.Find("Save");
```

Names are compared case-insensitively.
That keeps registry lookups stable even when casing differs between declarations.

## Duplicate Protection

By default, a `DefList<T>` does not allow duplicate names.

```csharp
DefList<Command> Commands = new();

Commands.Add(new Command { Name = "Save" });
Commands.Add(new Command { Name = "Save" });
```

The second `Add()` throws.
This is important for registries, because names are used as identifiers.

When duplicates are intentionally needed, set `AllowDuplicateNames`.

```csharp
DefList<SqlFilterDef> Filters = new();

Filters.AllowDuplicateNames = true;
```

Tripous uses this for cases such as SQL filters, where the same field name may appear more than once in a range condition.

## Find, Get And Indexer

Use `Find()` when a missing descriptor is allowed.

```csharp
ModuleDef Module = DataRegistry.Modules.Find("Customer");

if (Module != null)
{
    // Use the module.
}
```

Use `Get()` or the indexer when the descriptor is required.

```csharp
ModuleDef Module = DataRegistry.Modules.Get("Customer");
FieldDef Field = Module.Table.Fields["Code"];
```

`Get()` throws when the descriptor is missing.
That is usually better for required registry objects, because the error is raised close to the problem.

## Find Or Add

`FindOrdAdd()` looks for a descriptor by name and creates it when it is missing.

```csharp
DefList<FieldDef> Fields = new();

FieldDef Code = Fields.FindOrdAdd("Code");
Code.DataType = SqlDataType.String;
```

The new item is created with `Activator.CreateInstance<T>()`, so `T` must have a parameterless constructor.

`FindOrAddRange()` applies the same pattern to multiple names.

```csharp
List<FieldDef> Fields = Table.Fields.FindOrAddRange(["Code", "Name", "Notes"]);
```

Use this pattern when building descriptors programmatically.
For validation or read-only lookup, prefer `Find()` or `Get()`.

## Ordering

`InsertBefore()` and `InsertAfter()` help place descriptors relative to an existing descriptor.

```csharp
Table.Fields.InsertAfter("Code", new FieldDef { Name = "Name" });
Table.Fields.InsertBefore("Notes", new FieldDef { Name = "IsActive" });
```

`Sort()` orders items by `TitleKey`.

```csharp
DataRegistry.Modules.Sort();
```

This is useful for display-oriented lists.
For declaration order, do not sort unless order is intentionally not meaningful.

## Validation And References

`CheckDescriptors()` calls `CheckDescriptor()` on every item.

```csharp
DataRegistry.Modules.CheckDescriptors();
```

`UpdateReferences()` calls `UpdateReferences()` on every item.
`JsonLoaded()` calls `UpdateReferences()` after JSON deserialization.

```csharp
DefList<TableDef> Tables = Json.Deserialize<DefList<TableDef>>(JsonText);

Tables.UpdateReferences();
```

This matches the `BaseDef` pattern:

- Load descriptor data.
- Reconnect runtime references.
- Validate required values.

## Where Tripous Uses DefList

`DefList<T>` appears throughout the registry system.

- `AppRegistry.MenuCommands`.
- `AppRegistry.ToolBarCommands`.
- `DataRegistry.Modules`.
- `DataRegistry.Lookups`.
- `DataRegistry.Locators`.
- `DataRegistry.CodeProviders`.
- `DesktopRegistry.Forms`.
- `TableDef.Fields`, `Joins`, `Stocks`, and `Details`.
- `LocatorDef.Fields`.

That common list type is what lets Tripous use the same lookup, validation, JSON loading, and duplicate-name behavior across different descriptor categories.

## ReadOnlyDefList

`ReadOnlyDefList<T>` wraps a `DefList<T>` and exposes only read operations.

```csharp
ReadOnlyDefList<ModuleDef> Modules = new(DataRegistry.Modules);

ModuleDef Module = Modules.Get("Customer");
int Count = Modules.Count;
```

It supports:

- `Contains()`.
- `Find()`.
- `Get()`.
- Enumeration.
- Indexer lookup.
- `Count`.

Use it when an API should expose descriptors for lookup but should not allow callers to add, remove, or reorder them.

## When To Use It

Use `DefList<T>` when the items are Tripous descriptors and must be found by name.

```csharp
DefList<ReportDef> Reports = new();
```

Use a normal list when the items are not descriptors or when duplicate values and position are the only important concerns.

```csharp
List<string> Names = new();
```

The distinction matters.
`DefList<T>` is not just a collection; it carries descriptor-specific rules.

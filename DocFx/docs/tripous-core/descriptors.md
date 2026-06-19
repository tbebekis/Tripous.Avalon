# BaseDef And IDef

`IDef` and `BaseDef` define the common contract for Tripous descriptors.
A descriptor is a metadata object that describes something the framework can later use: a table, field, module, form, command, lookup, locator, or SQL definition.

The important idea is that descriptors are not runtime services.
They are structured declarations.

## Descriptor Contract

`IDef` is the common interface.
It gives every descriptor the same basic shape.

```csharp
IDef Def = Registry.Find("Customer");

string Name = Def.Name;
string Title = Def.Title;

Def.CheckDescriptor();
```

Every descriptor has:

- `Name`, the stable descriptor identifier.
- `TitleKey`, the localization key.
- `Title`, the localized display title.
- `CheckDescriptor()`, for validation.
- `Assign()`, `Clone()`, and `Clear()`, for descriptor copying.
- `UpdateReferences()`, for reconnecting object references after loading.

## BaseDef

Most Tripous descriptors inherit from `BaseDef`.
It implements `IDef`, `IJsonLoadable`, and `INotifyPropertyChanged`.

```csharp
/// <summary>
/// Represents a custom descriptor.
/// </summary>
public class ReportDef : BaseDef
{
    // ● properties
    /// <summary>
    /// Gets or sets the SQL statement.
    /// </summary>
    public string SqlText { get; set; }
}
```

`BaseDef` supplies the standard behavior.
Derived descriptors usually add only the properties and validation rules they need.

## Name And Title

`Name` is the technical identifier.
If no name is assigned, `BaseDef.Name` falls back to the full type name.

```csharp
ReportDef Def = new();

Def.Name = "CustomerReport";
```

`TitleKey` is the localization key.
If it is empty, `BaseDef` uses `Name`.

```csharp
Def.TitleKey = "CustomerReport";

string Title = Def.Title;
```

`Title` is resolved through `Texts.L(TitleKey)`.
That means descriptor classes can keep localization as keys, while the application controls the final displayed text.

## Validation

`CheckDescriptor()` verifies that a descriptor is complete.
`BaseDef` checks only `Name`.
Derived descriptors override the method and add their own required rules.

```csharp
/// <summary>
/// Represents a custom descriptor.
/// </summary>
public class ReportDef : BaseDef
{
    // ● public
    /// <summary>
    /// Checks whether this descriptor is fully defined.
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(SqlText))
            Sys.Throw(Texts.GS("E_ReportDef_NoSqlText", "ReportDef must have SQL text."));
    }

    // ● properties
    /// <summary>
    /// Gets or sets the SQL statement.
    /// </summary>
    public string SqlText { get; set; }
}
```

This pattern is used throughout Tripous.Data.
For example, table, field, module, and select descriptors validate their required names, aliases, SQL text, table references, or field lists.

## Assign, Clone And Clear

`BaseDef` uses the Tripous `Json` helper to copy property values between descriptors.

```csharp
ReportDef Source = new();

Source.Name = "CustomerReport";
Source.SqlText = "select * from Customer";

ReportDef Target = new();

Target.Assign(Source);
```

`Clone()` creates a new descriptor of the same runtime type and copies values into it.

```csharp
ReportDef Copy = Source.Clone() as ReportDef;
```

`Clear()` assigns values from a new empty instance.

```csharp
Source.Clear();
```

These methods are useful in descriptor editors, registry manipulation, tests, and JSON-based configuration workflows.

## JSON Loading And References

`BaseDef` implements `IJsonLoadable`.
When a descriptor is loaded from JSON, `JsonLoaded()` calls `UpdateReferences()`.

```csharp
public virtual void JsonLoaded() => UpdateReferences();
```

`UpdateReferences()` exists for descriptors that store both names and object references.
After JSON loading, the names are present but runtime references may need to be rebuilt.

Typical examples are descriptors that contain child tables, fields, filters, or links to other descriptors.

```csharp
/// <summary>
/// Updates internal references after loading.
/// </summary>
public override void UpdateReferences()
{
    base.UpdateReferences();

    foreach (ReportColumnDef Column in Columns)
        Column.Owner = this;
}
```

Use this method when a descriptor must repair parent links, cached references, or child ownership after deserialization.

## Property Change Notifications

`BaseDef` implements `INotifyPropertyChanged`.
Changing `Name` raises notifications for:

- `Name`.
- `TitleKey`.
- `Title`.

Changing `TitleKey` raises notifications for:

- `TitleKey`.
- `Title`.

This is useful when descriptor editors or UI tools bind directly to descriptor objects.

## Common Derived Descriptors

Tripous uses `BaseDef` across multiple layers.

- `Command` in Tripous Core.
- `SelectSql` in Tripous.Data.
- `ModuleDef`, `TableDef`, `FieldDef`, and `SelectDef` in Tripous.Data.
- `LookupDef` and `LocatorDef` in Tripous.Data.
- `FormDef` and `GridColumnDef` in Tripous.Desktop.

That common base is what allows registry code, descriptor lists, editors, JSON loading, and validation to treat many different metadata objects in the same way.

## When To Derive From BaseDef

Derive from `BaseDef` when the type is a Tripous descriptor.

- It has a stable `Name`.
- It has a localizable title.
- It may be stored in descriptor lists.
- It may be serialized to JSON.
- It needs descriptor validation.
- It participates in registry or declaration workflows.

Do not derive from `BaseDef` for runtime services, UI controls, data rows, or business logic classes.
Those are not descriptors, even if they also have names or titles.

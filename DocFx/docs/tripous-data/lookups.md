# Lookups

Lookups provide value/display lists for fields.
They are used for combo boxes, lookup grid columns, display conversion, and reference fields that should show a friendly text instead of a raw id.

A lookup has two parts:

- `LookupDef`, the registered definition.
- `LookupSource`, the runtime object that loads `LookupItem` values.

## LookupDef

`LookupDef` describes where lookup items come from.
It is registered in `DataRegistry.Lookups`.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName(
    "Country",
    "Country",
    FormName: "Country");

Lookup.ValueField = "Id";
Lookup.DisplayField = "Name";
```

`ValueField` is the value written to the data row.
`DisplayField` is the text shown to the user.

The defaults are:

- `ValueField = "Id"`.
- `DisplayField = "Name"`.

## Lookup Sources

A lookup source may come from:

- A table name.
- A SELECT statement.
- An enum type.
- A custom `LookupSource` class.
- A manually loaded `DataTable`.

Table source:

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName(
    "PaymentMethod",
    "PaymentMethod",
    FormName: "PaymentMethod");
```

SQL source:

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithSql(
    "ActiveCustomer",
    @"
select
    Id,
    Name
from Customer
where IsActive = :IsActive");
```

Enum source:

```csharp
LookupDef Lookup = DataRegistry.AddLookupSource(
    "DocumentStatus",
    typeof(DocumentStatus));
```

Custom source:

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithClassName(
    "ModuleName",
    typeof(DocumentModuleLookupSource).FullName);
```

Use the `AddOrUpdate...` variants in generated or repeatable registration code.

## LookupSource

`LookupSource` is the runtime loader.
It is created from a `LookupDef`.

```csharp
LookupSource Source = Lookup.Create();
List<LookupItem> Items = Source.GetList();
```

`GetList()` loads the list the first time it is called.
Depending on `LookupDef`, it may execute SQL, select from a table, load enum values, or call a custom source.

```csharp
LookupItem Item = Source.FindItem(Value);
```

`FindItem()` compares both raw values and invariant string forms.
This helps when database providers return equivalent values with different CLR types.

## LookupItem

`LookupItem` represents one entry.

```csharp
LookupItem Item = new(
    Value: 1,
    DisplayText: "Open");
```

It contains:

- `Value`, the stored value.
- `DisplayText`, the text shown to the user.
- `IsNullItem`, true for an empty item.
- `Row`, the original source row when loaded from a `DataTable`.

`ToString()` returns `DisplayText`, which is why combo boxes can display it directly.

## Null Item

Set `UseNullItem` when the list should include an empty first item.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName(
    "Country",
    "Country",
    UseNullItem: true);
```

The runtime list then starts with:

```csharp
new LookupItem(null, string.Empty, IsNullItem: true);
```

This is useful for optional fields.

## Field Lookup

A field becomes a lookup field by setting `LookupSource`.
The helper methods do that directly.

```csharp
Table.AddIntegerLookupId(
    "CountryId",
    LookupSource: "Country",
    TitleKey: "Country");
```

String id lookup:

```csharp
Table.AddStringLookupId(
    "UserId",
    LookupSource: "SYS_APP_USER",
    TitleKey: "User");
```

The field stores the id.
The lookup provides the display text.

## Lookup Snapshots

Some tables keep snapshot fields from the lookup source.
For example, an order may store both `CustomerId` and a snapshot of the customer name.

When a lookup item is selected, Tripous can assign snapshot fields from the source row.

```csharp
Table.AssignLookupSnapshots(
    TargetRow,
    LookupField,
    Source,
    Item);
```

Desktop bindings and grid bindings call this when lookup values change.

## Custom LookupSource

Create a custom `LookupSource` when the list is not a simple table, SQL, or enum.

```csharp
public class DocumentModuleLookupSource : LookupSource
{
    // ● public
    /// <summary>
    /// Returns the document module lookup items.
    /// </summary>
    public override List<LookupItem> GetList()
    {
        if (List == null)
            List = new();

        if (List.Count == 0)
        {
            foreach (DocumentHandlerDef HandlerDef in DataRegistry.DocumentHandlers)
                List.Add(new LookupItem(HandlerDef.Name, HandlerDef.Name));
        }

        return List;
    }
}
```

Register it by class name.

```csharp
DataRegistry.AddLookupWithClassName(
    "ModuleName",
    typeof(DocumentModuleLookupSource).FullName);
```

## Desktop Use

Tripous.Desktop uses lookups in:

- Combo box bindings.
- DataGrid lookup columns.
- Lookup display converters.
- Reference context menus.

For example, a field with `LookupSource = "Country"` can become a combo box column automatically.
The user sees the lookup display text, while the data row stores the lookup value.

## When To Use A Lookup

Use a lookup when:

- The source list is small or moderate.
- The user should choose from known values.
- A combo box or lookup grid column is appropriate.
- The stored value is an id but the UI should show a name.

For large searchable reference tables, use a locator instead.

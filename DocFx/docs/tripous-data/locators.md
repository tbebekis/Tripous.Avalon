# Locators

Locators are searchable selectors for reference data.
They are used when a field stores a single key value, but the user needs to search and see more information before selecting it.

Use a lookup for small value lists.
Use a locator for larger tables such as customers, products, persons, documents, or finance movements.

## LocatorDef

`LocatorDef` is the registered descriptor.
It defines the source table or SELECT, the key field, the visible/searchable fields, and the form used to open the selected item.

Simple table locator:

```csharp
LocatorDef Locator = DataRegistry.AddLocator(
    "Customer",
    SourceTableName: "Customer",
    KeyField: "Id",
    FormName: "Customer");
```

Add fields that the locator should return or display.

```csharp
Locator.Add(
    "Id",
    DataFieldType.String,
    TargetField: "CustomerId",
    Alias: "Customer__Id",
    TitleKey: null,
    IsVisible: false,
    IsSearchable: false);

Locator.Add(
    "Code",
    DataFieldType.String,
    TargetField: null,
    Alias: "Customer__Code",
    TitleKey: null,
    IsVisible: true,
    IsSearchable: true);

Locator.Add(
    "Name",
    DataFieldType.String,
    TargetField: null,
    Alias: "Customer__Name",
    TitleKey: null,
    IsVisible: true,
    IsSearchable: true);
```

`Name` is the source field.
`Alias` is the field name exposed by the locator SELECT.
`TargetField` is the target table field that receives the source value.

## SQL Locators

For joins, filters, or role-specific sources, use explicit SQL.

```csharp
string SqlText = @"
select
     P.Id as Id,
     P.Code as Code,
     P.Name as Name
from Person P
inner join PersonRole PR on PR.PersonId = P.Id
where P.IsActive = 1";

LocatorDef Locator = DataRegistry.AddLocatorWithSql(
    "Person",
    SqlText,
    KeyField: "Id",
    FormName: "Person");
```

When `SelectSql` is supplied, it should not contain the user search WHERE.
The locator adds the search WHERE at execution time.

## Field Binding

A field uses a locator by setting `FieldDef.Locator`.

```csharp
FieldDef Field = Table.AddString(
    "CustomerId",
    40,
    Flags: FieldFlags.Required);

Field.Locator = "Customer";
```

Desktop bindings use this metadata to create locator controls or locator grid columns.

## Runtime Locator

`LocatorDef.Create()` creates the runtime `Locator`.

```csharp
Locator Locator = DataRegistry.Locators.Get("Customer").Create();
```

The user search term must end with the trigger character `?`.

```csharp
bool ShouldSearch = Locator.ContainsSearchTrigger("acme?");
```

Executing a locator fills `SourceTable`.

```csharp
LocatorSearchResult Result = Locator.Execute("acme?");

if (Result.IsSingleRow)
{
    DataRow SourceRow = Result.SourceTable.Rows[0];
}
```

If too many rows are returned, `TooManyRows` is true and the source table is cleared.
The limit comes from `Db.Settings.LocatorMaximumDropDownRows`.

The minimum search text length comes from `Db.Settings.LocatorMinimumSearchTextLength`.

## Assigning Values

After a source row is selected, `Locator.Assign()` copies the key and mapped fields to the target row.

```csharp
Dictionary<string, string> TargetFieldMap = Table.CreateLocatorTargetFieldMap(
    Field,
    Locator.LocatorDef);

Locator.Assign(
    SourceRow,
    TargetRow,
    KeyFieldName: "CustomerId",
    TargetFieldMap: TargetFieldMap);
```

The key field receives `Locator.KeyValue`.
Other locator fields may update snapshot fields such as customer code or customer name.

Passing a null source row clears the key and mapped target fields.

```csharp
Locator.Assign(
    null,
    TargetRow,
    KeyFieldName: "CustomerId",
    TargetFieldMap: TargetFieldMap);
```

## Target Field Mapping

The target field mapping is one of the important locator features.
When a source row is selected, the locator does not only assign the selected key.
It may also assign related display values to target fields.

There are two common cases.

## Virtual Join Fields

The first case is a locator used together with a join.
The target fields may be virtual fields produced by the join, not actual database columns of the target table.

For example, a `Customer` join may expose:

- `Customer__Code`.
- `Customer__Name`.

The locator fields use the same aliases.

```csharp
Locator.Add(
    "Code",
    DataFieldType.String,
    TargetField: null,
    Alias: "Customer__Code",
    TitleKey: null,
    IsVisible: true,
    IsSearchable: true);

Locator.Add(
    "Name",
    DataFieldType.String,
    TargetField: null,
    Alias: "Customer__Name",
    TitleKey: null,
    IsVisible: true,
    IsSearchable: true);
```

When the locator is used from a reference field such as `CustomerId`, Tripous can match locator field aliases to join field aliases.
This lets the UI refresh related display fields immediately after selection.

These join fields are not persisted by themselves.
They are display fields derived from the related source table.

## Snapshot Target Fields

The second case is a locator that writes selected values to real database fields.
These fields are snapshots.

For example, a trade line may store the product id and also the product code and name at the time of selection.

```sql
ProductId @NVARCHAR(40) @NULL,      -- Locator Product
ProductCode @NVARCHAR(40) @NULL,    -- Snapshot Product.Code
ProductName @NVARCHAR(128) @NULL,   -- Snapshot Product.Name
```

The descriptor keeps that meaning.

```csharp
Table.AddString("ProductId", 40);
Table.AddString("ProductCode", 40).SetSnapshotOf("Product.Code");
Table.AddString("ProductName", 128).SetSnapshotOf("Product.Name");

TableDef Product = Table.AddJoin(
    "ProductId",
    "Product",
    "Product",
    "Id");

Table.GetField("ProductId").Locator = "Product";
Product.AddString("Code", 40);
Product.AddString("Name", 96);
```

Here locator selection writes:

- `Product.Id` to `ProductId`.
- `Product.Code` to `ProductCode`.
- `Product.Name` to `ProductName`.

The join fields describe where values come from.
The snapshot fields describe where values are stored.

Snapshot fields are actual persisted fields of the target table.
They are used when the selected related value must be preserved historically.

## Mapping Resolution

`TableDef.CreateLocatorTargetFieldMap()` resolves the target fields.

The key rules are:

- The reference field receives the selected key value.
- Tripous tries to match locator field names or aliases to fields in the related join.
- If a matching snapshot field exists for that join field, the snapshot field is preferred.
- If `TargetField` is set on the locator field, it can explicitly name the target field.
- If no better mapping is found, the locator field alias may be used as the target field name by the binding layer.

This is why locator aliases, join aliases, and snapshot metadata should follow the same naming convention.

## LocatorFieldDef

`LocatorFieldDef` describes one source field.

Important properties:

- `Name`, the source field name.
- `Alias`, the result column name.
- `TargetField`, the target table field to update.
- `DataType`, the field data type.
- `IsVisible`, whether the user sees it.
- `IsSearchable`, whether it participates in search.
- `DisplayWidth`, UI width hint.

Only string fields are useful as searchable fields because the default search WHERE uses `LIKE`.

## Generated SELECT

When a locator has only `SourceTableName`, Tripous builds a SELECT automatically.
For example, a customer locator may become:

```sql
select
  Customer.Id as Id,
  Customer.Code as Customer__Code,
  Customer.Name as Customer__Name
from Customer
```

When `OrderBy` is set, it is added to the generated SELECT.

```csharp
Locator.OrderBy = "Name";
```

For more complex SQL, provide `SelectSql` through `AddLocatorWithSql()`.

## Events

`Locator.AnyEvent` allows client code to customize locator behavior.

```csharp
Locator.AnyEvent += (Sender, Args) =>
{
    if (Args.EventType == LocatorEventType.AddToWhere)
        Args.SelectSql.AddToWhere("IsActive = 1");
};
```

Event types include:

- `AddToWhere`, before execution, to extend the SELECT.
- `SelectSourceTable`, to execute custom loading.
- `SetupSourceTable`, to customize columns.
- `FilterSourceTable`, to apply a `DataView` filter.

## Cascading Locators

A locator can depend on another locator.
Set `Master` and `DetailKey`.

```csharp
CityLocator.Master = CountryLocator;
CityLocator.DetailKey = "CountryId";
```

When the source table is loaded, the detail locator filters its `SourceTable.DataView` by the master key value.

## Where It Is Used

Locators are used by:

- `LocatorBox`, for item forms.
- `GridLocatorBox`, for grids.
- Desktop item bindings.
- DataGrid locator columns.
- Reference context menus.
- RegBuilder-generated locator metadata.

The important distinction is that a locator searches a source table and returns one key value, while also being able to display and copy extra fields that help the user identify the row.

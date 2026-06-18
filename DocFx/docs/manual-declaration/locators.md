# Locators

Locators are searchable selectors for related rows.

A locator is used when a field stores a reference to another table and the user needs help finding the correct row.

Example:

- `Contact.CustomerId` stores a customer id.
- The `Customer` locator lets the user search customers by code or name.
- The selected customer id is written back to `Contact.CustomerId`.

## LocatorDef

`LocatorDef` is the descriptor.

It declares:

- The locator name.
- The source table or source select.
- The source key field.
- The fields returned by the search.
- Which fields are visible.
- Which fields are searchable.
- Which returned fields map to target fields.
- The form that can open the source table.

The runtime `Locator` uses this descriptor to search rows and return selected values.

## Registering A Locator

The `04-mini-crm` sample registers a `Customer` locator.

```csharp
LocatorDef Locator = DataRegistry.AddLocator("Customer", "Customer", "Id", FormName: "Customer");
Locator.Add("Id", DataFieldType.String, TargetField: "CustomerId", Alias: "Customer__Id", TitleKey: null, IsVisible: false, IsSearchable: false);
Locator.Add("Code", DataFieldType.String, TargetField: null, Alias: "Customer__Code", TitleKey: null, IsVisible: true, IsSearchable: true);
Locator.Add("Name", DataFieldType.String, TargetField: null, Alias: "Customer__Name", TitleKey: null, IsVisible: true, IsSearchable: true);
```

This means:

- The locator name is `Customer`.
- The source table is `Customer`.
- The source key field is `Id`.
- The related form is `Customer`.
- `Id` is returned but hidden.
- `Code` and `Name` are visible and searchable.
- The selected `Id` value is written to `CustomerId`.

## Locator Fields

Each `Locator.Add()` call creates a `LocatorFieldDef`.

Important field properties are:

- `Name` is the source field name.
- `DataType` is the source field data type.
- `TargetField` is the field in the target table that receives the selected value.
- `Alias` is the returned column alias.
- `TitleKey` is the display title key.
- `IsVisible` controls whether the field is visible.
- `IsSearchable` controls whether the field can be searched.
- `DisplayWidth` can provide a preferred display width.

Searchable fields should normally be string fields.

## Using A Locator In A Table

A `FieldDef` references a locator by name.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";
```

The field stores the selected key.

The locator controls how the user searches for and selects that key.

## Locator And Join Together

Locator fields often appear together with joins.

The locator handles search and selection.

The join describes related display fields.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";

TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
CustomerJoin.AddString("Code", 40);
CustomerJoin.AddString("Name", 128);
```

This describes two related things:

- `CustomerId` stores the selected customer id.
- The `Customer` locator is used to find the customer.
- The join exposes related customer fields such as `Code` and `Name`.

The locator does not replace the join.

The join does not replace the locator.

They serve different parts of the same reference workflow.

## Target Field Mapping

When a row is selected, the locator copies values from the selected source row to the target row.

There are two common mapping cases.

### Join Target Fields

The first case is a locator used together with a join.

The target fields may be fields produced by the join.

For example, a `Customer` join may expose:

- `Customer__Code`
- `Customer__Name`

The locator fields use the same aliases.

```csharp
Locator.Add("Code", DataFieldType.String, TargetField: null, Alias: "Customer__Code", TitleKey: null, IsVisible: true, IsSearchable: true);
Locator.Add("Name", DataFieldType.String, TargetField: null, Alias: "Customer__Name", TitleKey: null, IsVisible: true, IsSearchable: true);
```

When the locator is used from a table field such as `CustomerId`, Tripous can match the locator field aliases to the join field aliases.

This is useful when the selected row should immediately refresh related display fields.

### Snapshot Target Fields

The second case is a locator that writes selected values to real table fields.

These fields are snapshots.

In tERP, `TradeLine` stores the product id and also stores the product code and name at the time of selection.

```sql
ProductId @NVARCHAR(40) @NULL,                       -- Locator Product
ProductCode @NVARCHAR(40) @NULL,                     -- Snapshot Product.Code
ProductName @NVARCHAR(128) @NULL,                    -- Snapshot Product.Name
```

The generated declaration keeps the same meaning.

```csharp
tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true).SetSnapshotOf("Product.Code");
tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetSnapshotOf("Product.Name");
TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
tblTradeLine.Fields.Get("ProductId").Locator = "Product";
tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI);
tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required);
```

Here the locator selection writes:

- `Product.Id` to `TradeLine.ProductId`.
- `Product.Code` to `TradeLine.ProductCode`.
- `Product.Name` to `TradeLine.ProductName`.

The join fields describe where the values come from.

The snapshot fields describe where the values are stored.

Snapshot fields are persisted fields of the target table.

Join aliases are related display fields.

### Mapping Resolution

The mapping is resolved in this order:

- The reference field receives the selected key value.
- Tripous tries to map locator field names or aliases to fields in the related join.
- If a matching snapshot field exists for that join field, the snapshot field is preferred.
- If `TargetField` is set on the locator field, it can explicitly name the target field.
- If no better mapping is found, the locator field alias may be used as the target field name.

This is why locator aliases, join aliases and snapshot metadata should use the same naming convention.

## Multiple Target Fields

A locator may return more than one value to the target table.

The `Contact` locator in `04-mini-crm` maps both `ContactId` and `CustomerId`.

```csharp
Locator = DataRegistry.AddLocator("Contact", "Contact", "Id", FormName: "Contact");
Locator.Add("Id", DataFieldType.String, TargetField: "ContactId", Alias: "Contact__Id", TitleKey: null, IsVisible: false, IsSearchable: false);
Locator.Add("CustomerId", DataFieldType.String, TargetField: "CustomerId", Alias: "Contact__CustomerId", TitleKey: null, IsVisible: false, IsSearchable: false);
Locator.Add("FirstName", DataFieldType.String, TargetField: null, Alias: "Contact__FirstName", TitleKey: null, IsVisible: true, IsSearchable: true);
Locator.Add("LastName", DataFieldType.String, TargetField: null, Alias: "Contact__LastName", TitleKey: null, IsVisible: true, IsSearchable: true);
```

This is useful when selecting one row should also update related hidden fields.

## Table-Based And SQL-Based Locators

Most locators are table-based.

```csharp
DataRegistry.AddLocator("Customer", "Customer", "Id", FormName: "Customer");
```

Tripous can construct the select from the source table and locator fields.

A locator may also be SQL-based.

```csharp
LocatorDef Locator = DataRegistry.AddLocatorWithSql("ActiveCustomer", @"
select
    Id,
    Code,
    Name
from Customer
where IsActive = 1
", "Id", FormName: "Customer");
```

SQL-based locators are useful when the source rows need filtering, joins or calculated columns.

The select should not contain a dynamic search `where` clause. Locator search logic adds search conditions using the locator field metadata.

## Locator Versus Lookup

A lookup is best for a small known list.

Examples:

- Status.
- Category.
- Activity type.
- Enum value.

A locator is best for a larger searchable table.

Examples:

- Customer.
- Contact.
- Product.
- Document.

Use a lookup when the user should choose from a short list of values.

Use a locator when the user needs search fields, visible columns and a selected row key.

## Registration Order

Locators should be registered before modules.

The sample registry uses this order:

```csharp
Version.RegisterLookupSources();
Version.RegisterLocators();
Version.RegisterModules();
```

Modules may declare fields that reference locator names.

Those references are resolved later by:

```csharp
DataRegistry.Modules.UpdateReferences();
```

## Practical Rule

When declaring locators manually:

- Use locators for searchable related tables.
- Register locators before modules.
- Set the source table and key field clearly.
- Add at least one visible searchable field.
- Map the key field to the target foreign key field.
- Use locator aliases that match join aliases, such as `Customer__Code`.
- Use snapshot fields when values must be stored, such as `ProductCode` with `SetSnapshotOf("Product.Code")`.
- Hide key fields when they are only technical identifiers.
- Use aliases that remain unique when joins are involved.
- Use SQL-based locators only when table-based locators are not enough.
- Use joins when the module also needs related display fields.

## Related Articles

- [Lookups](lookups.md)
- [Modules](modules.md)
- [Tables and Fields](tables-and-fields.md)
- [Select Definitions](select-definitions.md)

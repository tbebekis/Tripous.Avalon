# Generated Locators

Generated locators are written to `RegistryVersionN.Locators.cs`.

They register locator definitions in `DataRegistry.Locators2`.

Locators are used for searchable reference selection, usually for larger or more complex reference sets than simple lookups.

## Generated Method

The generated file overrides `RegisterLocators()`.

Example:

```csharp
public override void RegisterLocators()
{
    DataRegistry.AddOrUpdateLocator2("Person", Source: "Person", KeyField: "Id", FormName: "Person", WebFormName: "Person");
    DataRegistry.AddOrUpdateLocator2("Product", Source: "Product", KeyField: "Id", FormName: "Product", WebFormName: "Product");
    DataRegistry.AddOrUpdateLocator2("PaymentSettlementFinanceMovement", Source: "FinanceMovement", KeyField: "Id", ClassName: "tERP.Data.PaymentSettlementFinanceMovementLocator", FormName: "FinanceMovement", WebFormName: "FinanceMovement");
}
```

`AddOrUpdateLocator2()` receives:

- locator name
- source table name or source SQL text
- source key field
- optional locator class name
- optional desktop form name
- optional web form name

## Source Metadata

Locator generation starts from field metadata.

Examples:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
FinanceMovementId @NVARCHAR(40) @NOT_NULL, -- Locator PaymentSettlementFinanceMovement ClassName:tERP.Data.PaymentSettlementFinanceMovementLocator
CustomerId @NVARCHAR(40) @NOT_NULL, -- Locator Customer Form:Person WebForm:Person
```

If the locator name is omitted, the builder resolves it from the foreign key referenced table.

If `ClassName:`, `Form:`, or `WebForm:` is used, the locator name is required.

## Base Registration

The generated locator file registers the base locator definition.

Example:

```csharp
DataRegistry.AddOrUpdateLocator2("Product", Source: "Product", KeyField: "Id", FormName: "Product", WebFormName: "Product");
```

This means:

- locator name: `Product`
- source table: `Product`
- key field: `Id`
- related desktop form: `Product`
- related web form: `Product`

The generated locator registration is intentionally minimal.

Custom locator fields, custom SELECT SQL, search behavior and display behavior may still be configured in handwritten code.

## Field Registration

Locator source registration and field registration are separate.

`RegistryVersionN.Locators.cs` registers the locator.

`RegistryVersionN.Modules.cs` marks fields that use it.

Example:

```csharp
tblTradeLine.Fields.Get("ProductId").Locator = "Product";
```

The field stores the locator name.

The locator definition describes how that name resolves to a searchable source.

## Locator Name Resolution

The locator name follows these rules:

- `-- Locator` uses the foreign key referenced table as the locator name.
- `-- Locator Product` uses `Product` as the locator name.
- `-- Locator Product ClassName:ProductLocator` uses `Product` and registers `ProductLocator`.
- `-- Locator Customer Form:Person WebForm:Person` uses `Customer` and sets reference forms explicitly.

If the field has no usable foreign key and no explicit locator name, the builder cannot infer the source safely.

## Source Table Resolution

The source table is normally resolved from the field foreign key.

Example:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
FOREIGN KEY (ProductId) REFERENCES Product(Id)
```

Generated registration:

```csharp
DataRegistry.AddOrUpdateLocator2("Product", Source: "Product", KeyField: "Id", FormName: "Product", WebFormName: "Product");
```

When the locator name differs from the source table, the generated registration keeps both values.

Example:

```csharp
DataRegistry.AddOrUpdateLocator2("Supplier", Source: "ProductSupplier", KeyField: "Id");
```

Here `Supplier` is the locator name and `ProductSupplier` is the source table.

## ClassName Convention

`ClassName:` registers a custom locator class.

Example metadata:

```sql
FinanceMovementId @NVARCHAR(40) @NOT_NULL, -- Locator PaymentSettlementFinanceMovement ClassName:tERP.Data.PaymentSettlementFinanceMovementLocator
```

Generated registration:

```csharp
DataRegistry.AddOrUpdateLocator2("PaymentSettlementFinanceMovement", Source: "FinanceMovement", KeyField: "Id", ClassName: "tERP.Data.PaymentSettlementFinanceMovementLocator", FormName: "FinanceMovement", WebFormName: "FinanceMovement");
```

The class is handwritten application code.

The generated code only references it by name.

## Join Alias Convention

When a locator field points to a joined source table, generated join fields use this alias convention:

```text
JOIN_ALIAS__FIELD_NAME
```

Example generated join:

```csharp
TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
tblProduct.AddId("Id");
tblProduct.AddString("Code");
tblProduct.AddString("Name");
```

Generated aliases:

```text
Id    -> Product__Id
Code  -> Product__Code
Name  -> Product__Name
```

The list SELECT uses the same convention:

```sql
COALESCE(Product.Code, '') as Product__Code
```

## Locator Field Names

Locator field names normally identify source fields on the joined source table.

For a product locator, preferred field names are:

```csharp
LocatorDef.Add("Id");
LocatorDef.Add("Code");
LocatorDef.Add("Name");
```

They mean:

```text
Product.Id
Product.Code
Product.Name
```

They do not mean `ProductId`, `ProductCode` or `ProductName` on the owning table.

## Snapshot Target Fields

Snapshot fields are persisted copies of related source fields.

Example schema:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
ProductCode @NVARCHAR(40) @NOT_NULL, -- Snapshot Product.Code
ProductName @NVARCHAR(128) @NOT_NULL, -- Snapshot Product.Name
```

Generated table fields:

```csharp
tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false).SetSnapshotOf("Product.Code");
tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.Required).SetNullable(false).SetSnapshotOf("Product.Name");
```

The locator value mapping is based on the source field path.

```text
Locator Code -> Product.Code -> ProductCode
Locator Name -> Product.Name -> ProductName
```

The target field names may differ from the source field names.

The `Snapshot` metadata connects them.

## Runtime Display Fields

If there is no snapshot target field, the UI may use generated join aliases.

```text
Locator Code -> Product.Code -> Product__Code
Locator Name -> Product.Name -> Product__Name
```

So there are two common target cases:

- generated join fields such as `Product__Code` and `Product__Name`
- persisted snapshot fields such as `ProductCode` and `ProductName`

Both cases still start from the same source fields:

```text
Product.Code
Product.Name
```

## Custom SELECT Convention

A custom locator SELECT should preferably return source field names.

Preferred:

```sql
select
     P.Id as Id
    ,P.Code as Code
    ,P.Name as Name
from Product P
```

Then locator fields can remain:

```csharp
LocatorDef.Add("Id");
LocatorDef.Add("Code");
LocatorDef.Add("Name");
```

## Explicit Alias Convention

Do not declare `ProductCode` and `ProductName` as locator field names merely because those are the target snapshot fields.

Those names do not identify fields in the `Product` source table.

If a custom locator SELECT must return target-style column names, keep the source field name and set an explicit alias.

```csharp
LocatorDef.Add("Code", DataFieldType.String, TargetField: null, Alias: "ProductCode", TitleKey: null, IsVisible: true, IsSearchable: true);
LocatorDef.Add("Name", DataFieldType.String, TargetField: null, Alias: "ProductName", TitleKey: null, IsVisible: true, IsSearchable: true);
```

Then the SELECT may return:

```sql
select
     P.Id as Id
    ,P.Code as ProductCode
    ,P.Name as ProductName
from Product P
```

This keeps the locator definition connected to the source fields while allowing different returned column names.

## Generated Baseline

Generated locator registrations are base declarations.

They do not replace handwritten locator configuration.

Use handwritten code when a locator needs:

- custom fields
- custom search SQL
- custom display columns
- custom search rules
- custom selection behavior

Do not edit `RegistryVersionN.Locators.cs` manually.

Change schema metadata and regenerate, or extend the locator in handwritten code.

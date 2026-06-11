# RegBuilder

Parses `.sql` schema files written in Tripous neutral SQL syntax and generates Tripous registration source code.

The input `.sql` file acts as a **declarative module definition language**.

**Reads:** table headers · field inline metadata · foreign key relations  
**Generates:** ordered schema SQL · `ModuleDef` · `FormDef` · `LookupSource` · `LocatorDef` · select definitions · table registration code

---


## Quick Overview

```
Input:  .sql file with special comments (metadata)
          ↓
RegBuilder
          ↓
Output: • Ordered CREATE TABLE SQL (RDBMS-neutral)
        • C# registration code
        • Module/Form/Lookup/Locator definitions
```

**Key features:**
- RDBMS-neutral type tokens (`@NVARCHAR`, `@DECIMAL`, `@DATE_TIME`, etc.)
- Automatic dependency resolution (correct table creation order)
- Metadata-driven module/group organization
- Built-in support for Lookups, Locators, Enums, and Auto-code fields

---

## The Input Schema File

### Anatomy of a Table Definition

```sql
/*---------------------------------------------------
Table: Product
Module: Product
Group: Inventory
Form: DataForm
-----------------------------------------------------
Description of what this table represents
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,
    Code @NVARCHAR(40) @NOT_NULL,           -- Code PRD-XXXX
    Name @NVARCHAR(96) @NOT_NULL,
    CategoryId @NVARCHAR(40) @NOT_NULL,     -- Lookup
    Price @DECIMAL @NOT_NULL,
    IsActive @BOOL DEFAULT 1 @NOT_NULL,
    CreatedAt @DATE_TIME @NOT_NULL,

    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);
```


## Core Concepts

### Group

UI/navigation classification. Organizes modules into business areas. Does **not** define table relationships.

```
People       → Person, ContactType
Inventory    → Product, Warehouse, Category
Sales        → Trade, Customer
```

Only **top tables** declare `Group`.

### Module

A business object with its complete table tree. Always has one **top table** (declares `Module`) and zero or more **detail tables** (connected via `Master` field).

```
Product               ← top table (declares Module)
    ProductBarcode    ← detail (Master field → Product)
    ProductSupplier
    ProductPrice
    BillOfMaterial
        BillOfMaterialLine
```

Only top tables declare `Module` and `Group`.  
Lookup tables are typically standalone single-table modules.

**Hierarchy:**
```
Group → Module → Top Table → Detail Tables
```

---

## Processing

1. Parses table headers, fields, and foreign keys
2. Resolves dependencies and calculates creation order
3. Rebuilds schema SQL in dependency order
4. Injects `CreationOrder` into rebuilt headers
5. Generates Tripous registration source code

Result available at `SchemaParserResult.SchemaSql`.

> `CreationOrder` is **generated output** — not required in the input schema.

---

## Schema Structure

Each table block contains:

```
/*--- header metadata ---*/
CREATE TABLE {TableName} (
    field definitions    -- METADATA -- comment
    foreign keys
)
```

---

## Header Syntax

```sql
/*---------------------------------------------------
Table:  TABLE_NAME
Module: Default | MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
Code:   Code [Draft] [Pattern] [ProviderName]
ListWhere: SQL_CONDITION

Module: MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
Code:   Code [Draft] [Pattern] [ProviderName]
ListWhere: SQL_CONDITION
FieldGroups: Address, Billing, Notes

IsLookup | NotUiVisible | IsReadOnly
IsSingleSelect | NoFilters | NoCascadeDeletes | NoGuidOids
-----------------------------------------------------
  comments / examples
----------------------------------------------------*/
```

### Required
- `Table` — always required

### Header order
- `Table` must be the first metadata entry.
- If the table is a top table of one or more modules, `Table` is followed by one or more module blocks.
- Table-level metadata such as `FieldGroups` and boolean flags follows the module blocks.
- Free text comments may follow after the metadata section separator.

### Module block
Each `Module` line starts a new module block. The following `Group`, optional `Form`, optional `ItemPage`, optional `DetailOrder`, optional `ListWhere`, and optional `Code` belong to that module block. A module block is complete when the next `Module` line starts or when non-module header metadata begins.

A module block has the following entries, in this order:

```sql
Module: Default | MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
DetailOrder: PARENT_TABLE_NAME=DETAIL_TABLE_NAME, DETAIL_TABLE_NAME
Code: Code [Draft] [Pattern] [ProviderName]
ListWhere: SQL_CONDITION
```

- `Module` is required.
- `Group` is required.
- `Form` is optional.
- `ItemPage` is optional.
- `DetailOrder` is optional.
- `ListWhere` is optional.
- `Code` is optional.
- If `Form` is omitted, the form name defaults to the module name and the form class defaults to `DataForm`.
- If `ItemPage` is omitted, the item page class defaults to `ItemPage`.
- `DetailOrder` defines the preferred order of a parent's direct child detail tabs. It may appear multiple times for different parent tables. Details not listed remain at the end in declaration order.
- Example: `DetailOrder: Trade=TradeLine, TradeTax` and `DetailOrder: BillOfMaterial=BillOfMaterialLine, BillOfMaterialCost`.
- `ListWhere` adds a module-specific condition to the generated list SELECT. Write only the condition, without the `WHERE` keyword.
- `ListWhere` may reference the top table and generated join aliases.
- If class names are omitted, default `DataModule`, `DataForm`, and `ItemPage` types are used.
- If `Code` is omitted, field `-- Code` metadata is used as fallback.

### Multiple module example

```sql
/*---------------------------------------------------
Table: Trade

Module: SalesOrder SalesOrderDataModule
Group: Sales Orders
Form: SalesOrder TradeForm
ItemPage: TradeItemPage
DetailOrder: Trade=TradeLine, TradeTax
Code: Draft SO-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesOrder'

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales Invoices
Form: SalesInvoice TradeForm
ItemPage: TradeItemPage
Code: Draft SI-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesInvoice'

Module: SalesCreditNote SalesCreditNoteDataModule
Group: Sales Credit Notes
Form: SalesCreditNote TradeForm
ItemPage: TradeItemPage
Code: Draft SCN-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesCreditNote'

FieldGroups: Dates, Party, Organization, Payment, Billing, Shipping, Relations, Amounts, Status, Audit, Notes

NoCascadeDeletes
NoGuidOids
----------------------------------------------------*/
```

### Boolean flags
Presence = `true`, absence = `false`.

| Flag               | Default   |
| ------------------ | --------- |
| `IsLookup`         | heuristic |
| `NotUiVisible`     | false     |
| `IsReadOnly`       | false     |
| `IsSingleSelect`   | false     |
| `NoFilters`        | false     |
| `NoCascadeDeletes` | false     |
| `NoGuidOids`       | false     |

### Module syntax
```
Module: Default                        → ModuleName = TableName
Module: Default LogDataModule
Module: Customer
Module: Customer CustomerDataModule
```
If `ModuleClassName` is omitted → default `DataModule` type.

### Form syntax
```
Form: DataForm                         → FormName = ModuleName, FormClassName = DataForm
Form: Customer                         → FormName = Customer, FormClassName = DataForm
Form: Customer CustomerDataForm
```
If `Form` is omitted → `FormName = ModuleName`, `FormClassName = DataForm`.
To declare `FORM_CLASS_NAME`, `FORM_NAME` must also be declared.

### ItemPage syntax
```
ItemPage: ItemPage                     → ItemPageClassName = ItemPage
ItemPage: CustomerItemPage
```
If `ItemPage` is omitted → `ItemPageClassName = ItemPage`.

### ListWhere syntax

```sql
ListWhere: DocumentType.ModuleName = 'SalesOrder'
ListWhere: Trade.IsActive = 1
```

The RegBuilder appends the condition as a `where` clause after the generated joins. Do not include the `WHERE` keyword. Only one `ListWhere` is allowed per module block.

### FieldGroups syntax
Field grouping is defined at table level through an optional header entry:

```sql
FieldGroups: Address, Billing, Notes
```

The `General` group is a built-in system group and always exists. Any field that does not explicitly declare a `Group` metadata modifier is automatically assigned to `General`.

If `FieldGroups` is present, it defines the display order of the generated ItemPage expanders. `General` is always inserted as the first group, even if not declared. Therefore, the previous example is treated internally as:

```text
General, Address, Billing, Notes
```

Fields may declare a group using:

```sql
-- Group Address
```

Group matching is case-insensitive. The display name keeps the first spelling declared or referenced. Any group referenced by a field but not listed in `FieldGroups` is appended after the declared groups in order of first appearance. If `FieldGroups` is omitted entirely, all groups are ordered by first appearance, with `General` remaining first.

---

## Field Metadata Syntax

```sql
FieldName TYPE, -- METADATA -- plain comment
```

The first `--` begins metadata. A second `--` separates metadata from plain comment.
Multiple metadata entries are separated with `;`.
A metadata entry enclosed in square brackets is parsed as comma-separated `FieldFlags`.

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General; [ReadOnlyUI, ReadOnlyEdit] -- customer code
Code @NVARCHAR(40) @NOT_NULL, -- Code Draft SO-YYYY-XXXXXX SALES_ORDER
TradeStatusId int default 0 @NOT_NULL, -- Enum TradeStatus; [ReadOnlyUI]
ProductCode @NVARCHAR(40) @NULL, -- Snapshot Product.Code
SupplierCode @NVARCHAR(96) @NULL, -- TitleKey Supplier Product Code
Notes @BLOB_TEXT, -- Memo; Group Notes -- short notes
Remarks @BLOB_TEXT, -- LargeMemo; Group Notes -- long notes
Photo @BLOB, -- [Image] -- product photo
```

### Metadata keywords

| Keyword      | Syntax                                                                                                    | Meaning                                             |
| ------------ | --------------------------------------------------------------------------------------------------------- | --------------------------------------------------- |
| `Master`     | `Master` / `Master OneToOne`                                                                              | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup`     | `Lookup [LOOKUP_NAME] [TableName:TABLE_NAME \| EnumName:ENUM_NAME \| ClassName:LOOKUP_SOURCE_CLASS_NAME]` | Small in-memory reference selector                  |
| `Enum`       | `Enum [EnumName]`                                                                                         | Enum-backed selector                                |
| `Locator`    | `Locator [LOCATOR_NAME] [ClassName:LOCATOR_CLASS_NAME]`                                                   | Searchable large reference selector                 |
| `Code`       | `Code [Draft] [Pattern] [ProviderName]`                                                                   | Auto-generated code field                           |
| `Memo`       | `Memo`                                                                                                    | Text field with Memo flag                           |
| `LargeMemo`  | `LargeMemo`                                                                                               | Text blob with LargeMemo flag                       |
| `Group`      | `Group GroupName`                                                                                         | Field UI group                                      |
| `Snapshot`   | `Snapshot TableName.FieldName`                                                                            | Persisted copy of a related source field            |
| `TitleKey`   | `TitleKey KeyOrText`                                                                                      | Field title resource key or fallback text           |
| `FieldFlags` | `[Flag1, Flag2]`                                                                                          | Adds `FieldFlags` values to the field               |

`Memo` and `LargeMemo` are mutually exclusive.
`FieldFlags` names are parsed from the `FieldFlags` enum. Common values are `Hidden`, `ReadOnly`, `ReadOnlyUI`, `ReadOnlyEdit`, `Required`, `Boolean`, `Memo`, `LargeMemo`, `Image`, `ImagePath`, `NoInsertUpdate`, `ForeignKey`, `Extra`, and `Searchable`.
Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.
For `FieldFlags`, square brackets are part of the actual schema syntax.
`Snapshot` requires an existing source table and field. The generated `FieldDef` receives `SnapshotOf`.
`TitleKey` applies the remaining metadata text up to the next `;` through `FieldDef.SetTitleKey()`.

**Name resolution when omitted:**

| Keyword         | Default name                 |
| --------------- | ---------------------------- |
| `Lookup`        | FK referenced table          |
| `Enum`          | field name minus `Id` suffix |
| `Locator`       | FK referenced table          |
| field `Code` Provider | TableName              |
| header `Code` Provider | ModuleName            |
| `Code` Pattern  | `XXX-XXX`                    |

---

## Lookup

`LOOKUP_NAME` is the name of the `LookupDef` registered in `DataRegistry.Lookups`.

```sql
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer
PersonId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer TableName:Person
TradeTypeId int @NOT_NULL, -- Lookup TradeType EnumName:TradeType
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
```

- If `LOOKUP_NAME` is omitted, it is resolved from the foreign key referenced table, or from the field name without the `Id` suffix.
- If `TableName:`, `EnumName:`, or `ClassName:` is used, `LOOKUP_NAME` is required.
- `TableName:` registers the lookup with `DataRegistry.AddOrUpdateLookupWithTableName()`.
- `EnumName:` registers the lookup with `DataRegistry.AddOrUpdateLookupSource()`.
- `ClassName:` registers the lookup with `DataRegistry.AddOrUpdateLookupWithClassName()`.

A table is identified as **lookup** when:
- header contains `IsLookup`, **or**
- it is a top table whose native fields match exactly one of:

```
Id, Name
Id, Code, Name
Id, Name, IsActive
Id, Code, Name, IsActive
```

Explicit `IsLookup` overrides the heuristic.

A **LookupSource** registration is generated when any of the following is true:
- table is identified as lookup
- any field declares `-- Lookup`, `-- Lookup LOOKUP_NAME`, or `-- Lookup LOOKUP_NAME SourceKind:SourceValue`

---

## Locator

Locator fields use a searchable control (not a simple dropdown).

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product ClassName:ProductLocator
```

- `LOCATOR_NAME` is the name of the `LocatorDef` registered in `DataRegistry.Locators`.
- If `LOCATOR_NAME` is omitted, it is resolved from the foreign key referenced table.
- If `ClassName:` is used, `LOCATOR_NAME` is required.
- `ClassName:` is passed to `DataRegistry.AddOrUpdateLocator()`.

The following declaration:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product ClassName:ProductLocator
```

generates:

```csharp
DataRegistry.AddOrUpdateLocator("Product", "Product", "Id", ClassName: "ProductLocator");
```

The builder generates **base registration only**. The developer is expected to further configure:
- `LocatorFieldDefs`
- `SelectSql` / custom joins
- custom search and return fields

**Joins:** the builder uses the FK to find the referenced table and creates a join.  
**Extra fields** are materialized on the owning `TableDef` using the alias convention:

```
ProductId       → persisted FK
Product__Code   → display/runtime (non-persistent)
Product__Name   → display/runtime (non-persistent)
```

In grids, raw FK fields are hidden; alias fields are shown instead.

### Join Field Aliases and Locator Fields

Unless the developer provides a custom alias, a field added to a join table receives an alias using this convention:

```text
JOIN_ALIAS__FIELD_NAME
```

For example:

```csharp
TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
tblProduct.AddId("Id");
tblProduct.AddString("Code");
tblProduct.AddString("Name");
```

The join alias is `Product`, so the generated field aliases are:

```text
Id    → Product__Id
Code  → Product__Code
Name  → Product__Name
```

The important rule is that `LocatorFieldDef.Name` normally identifies the original field name in the joined source table, not the alias of a snapshot field in the owning table.

For the `Product` join, locator fields should normally be declared as:

```csharp
LocatorDef.Add("Id");
LocatorDef.Add("Code");
LocatorDef.Add("Name");
```

A custom locator SELECT should expose compatible columns:

```sql
select
     P.Id as Id
    ,P.Code as Code
    ,P.Name as Name
from Product P
```

The UI resolves each locator field in two steps:

- It matches locator field `Code` to join field `Product.Code`.
- It uses the join field alias `Product__Code` for display, or finds a snapshot field such as `ProductCode` declared with `SnapshotOf("Product.Code")`.

For example:

```csharp
tblTradeLine.AddString("ProductCode").SetSnapshotOf("Product.Code");
tblTradeLine.AddString("ProductName").SetSnapshotOf("Product.Name");
```

The resulting mappings are:

```text
Locator Code → Product.Code → ProductCode
Locator Name → Product.Name → ProductName
```

Do not declare `ProductCode` and `ProductName` as locator field names merely because those are the target snapshot field names. They do not identify fields in the `Product` join and the UI cannot resolve the locator columns through the default matching rules.

When the custom SELECT must use different column names, keep the locator source name and set an explicit alias:

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

Using the original source names `Code` and `Name` in both the locator definition and the SELECT is the preferred and simplest form.

---

## Code Provider

```sql
Code @NVARCHAR(40) @NOT_NULL, -- Code SO-YYYY-XXXXXX SALES_ORDER
Code @NVARCHAR(40) @NOT_NULL, -- Code Draft SO-YYYY-XXXXXX SALES_ORDER

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SI-YYYY-XXXXXX
```

Discovered providers are stored in `SchemaParserResult.CodeProviderPatterns`:

```csharp
Dictionary<string, string>  // Key = ProviderName, Value = Pattern
```

Same provider name with different patterns → **parsing error**.

Generated output:
```csharp
FieldDef.CodeProvider = "SALES_ORDER";
DataRegistry.AddOrUpdateCodeProvider("SALES_ORDER");
Version.AddStatementAfter("INSERT INTO SYS_NUMBER_SERIES ...");
```

Draft output:

```csharp
FieldDef.CodeProvider = "DRAFT-SALES_ORDER";
DataRegistry.AddOrUpdateCodeProvider("DRAFT-SALES_ORDER");
DataRegistry.AddOrUpdateCodeProvider("SALES_ORDER");
```

`Code Draft SO-YYYY-XXXXXX SALES_ORDER` generates two code provider patterns:
- `DRAFT-SALES_ORDER` with pattern `DRAFT-SO-YYYY-XXXXXX`
- `SALES_ORDER` with pattern `SO-YYYY-XXXXXX`

Document modules with one module per table may use field `-- Code Draft PATTERN PROVIDER_NAME`.
Document modules with multiple modules on the same table should use module header `Code: Draft PATTERN PROVIDER_NAME`.
If `ProviderName` is omitted in a header `Code:`, the provider name defaults to the module name.
If header `Code:` is omitted, field `-- Code` metadata is used as fallback for that module.
Snapshot document modules should not declare `-- Code`; this is the schema author's responsibility, even when the table has a `DocumentTypeId` field.

Generated `SchemaVersionN.cs` files add `INSERT INTO SYS_NUMBER_SERIES` statements through `SchemaVersion.AddStatementAfter()`.
Each statement supplies all non-nullable fields, uses `MemTable.GenId()` for `Id`, and runs after the version's `CREATE TABLE` statements.
Duplicate `Code` values are expected to fail through the table unique constraint.

---

## Cheat Sheet

For a compact syntax reference, see [RegBuilder-Cheat-Sheet.md](RegBuilder-Cheat-Sheet.md).

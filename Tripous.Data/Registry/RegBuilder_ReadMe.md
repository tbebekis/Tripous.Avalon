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
Group: Inventory
Module: Product
Form: Default
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
Form:   Default | FORM_NAME  [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]

Module: MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   Default | FORM_NAME  [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]
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
Each `Module` line starts a new module block. The following `Group` and optional `Form` belong to that module block. A module block is complete when the next `Module` line starts or when non-module header metadata begins.

A module block has the following entries, in this order:

```sql
Module: Default | MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   Default | FORM_NAME [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]
```

- `Module` is required.
- `Group` is required.
- `Form` is optional.
- If `Form` is omitted, default form registration behavior is used.
- If class names are omitted, default `DataModule`, `DataForm`, and `ItemPage` types are used.

### Multiple module example

```sql
/*---------------------------------------------------
Table: Trade

Module: SalesOrder SalesOrderDataModule
Group: Sales Orders
Form: SalesOrder TradeForm TradeItemPage

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales Invoices
Form: SalesInvoice TradeForm TradeItemPage

Module: SalesCreditNote SalesCreditNoteDataModule
Group: Sales Credit Notes
Form: SalesCreditNote TradeForm TradeItemPage

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
Form: Default                          → FormName = ModuleName
Form: Default LogDataForm
Form: Default LogDataForm LogItemPage
Form: Customer CustomerDataForm CustomerItemPage
```
If `Form` is omitted → default form registration behavior.  
If class names are omitted → default `DataForm` / `ItemPage` types.

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

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General -- customer code
Notes @BLOB_TEXT, -- Memo; Group Notes -- short notes
Remarks @BLOB_TEXT, -- LargeMemo; Group Notes -- long notes
```

### Metadata keywords

| Keyword     | Syntax                          | Meaning                                             |
| ----------- | ------------------------------- | --------------------------------------------------- |
| `Master`    | `Master` / `Master OneToOne`    | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup`    | `Lookup [SourceName]`           | Small in-memory reference selector                  |
| `Enum`      | `Enum [EnumName]`               | Enum-backed selector                                |
| `Locator`   | `Locator [LocatorName]`         | Searchable large reference selector                 |
| `Code`      | `Code [Pattern] [ProviderName]` | Auto-generated code field                           |
| `Memo`      | `Memo`                          | Text field with Memo flag                           |
| `LargeMemo` | `LargeMemo`                     | Text blob with LargeMemo flag                       |
| `Group`     | `Group GroupName`               | Field UI group                                      |

`Memo` and `LargeMemo` are mutually exclusive.
Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.

**Name resolution when omitted:**

| Keyword         | Default name                 |
| --------------- | ---------------------------- |
| `Lookup`        | FK referenced table          |
| `Enum`          | field name minus `Id` suffix |
| `Locator`       | FK referenced table          |
| `Code` Provider | TableName                    |
| `Code` Pattern  | `XXX-XXX`                    |

---

## Lookup

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
- any field declares `-- Lookup` or `-- Lookup [SourceName]`

---

## Locator

Locator fields use a searchable control (not a simple dropdown).

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

---

## Code Provider

```sql
Code @NVARCHAR(40) @NOT_NULL, -- Code SO-YYYY-XXXXXX SALES_ORDER
```

Discovered providers are stored in `SchemaParserResult.CodeProviderPatterns`:

```csharp
Dictionary<string, string>  // Key = ProviderName, Value = Pattern
```

Same provider name with different patterns → **parsing error**.

Generated output:
```csharp
FieldDef.CodeProvider = "SALES_ORDER";
DataRegistry.AddCodeProvider("SALES_ORDER");
```

On startup, missing `SYS_NumberSeries` rows are created automatically. Existing rows are never overwritten.

---

# Appendix

## Header Syntax

```sql
/*---------------------------------------------------
Table:  TABLE_NAME
Module: Default | MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   Default | FORM_NAME  [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]

Module: MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   Default | FORM_NAME  [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]
FieldGroups: Address, Billing, Notes

IsLookup 
NotUiVisible 
IsReadOnly
IsSingleSelect 
NoFilters 
NoCascadeDeletes 
NoGuidOids
-----------------------------------------------------
  comments / examples
----------------------------------------------------*/
```


### Required
- `Table` — always required

### Module blocks
- `Table` must be the first metadata entry.
- A top table may declare one or more module blocks.
- Each `Module` line starts a new module block.
- The following `Group` and optional `Form` belong to that module block.
- A module block is complete when the next `Module` line starts or when non-module header metadata begins.
- Module block order is `Module`, `Group`, `Form`.
- `Group` is required for each module block.

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


## Field Syntax

```sql
FieldName TYPE, -- METADATA -- plain comment
```

The first `--` begins metadata. A second `--` separates metadata from plain comment.
Multiple metadata entries are separated with `;`.

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General -- customer code
Notes @BLOB_TEXT, -- Memo; Group Notes -- short notes
Remarks @BLOB_TEXT, -- LargeMemo; Group Notes -- long notes
```

### Metadata keywords

| Keyword     | Syntax                          | Meaning                                             |
| ----------- | ------------------------------- | --------------------------------------------------- |
| `Master`    | `Master` / `Master OneToOne`    | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup`    | `Lookup [SourceName]`           | Small in-memory reference selector                  |
| `Enum`      | `Enum [EnumName]`               | Enum-backed selector                                |
| `Locator`   | `Locator [LocatorName]`         | Searchable large reference selector                 |
| `Code`      | `Code [Pattern] [ProviderName]` | Auto-generated code field                           |
| `Memo`      | `Memo`                          | Text field with Memo flag                           |
| `LargeMemo` | `LargeMemo`                     | Text blob with LargeMemo flag                       |
| `Group`     | `Group GroupName`               | Field UI group                                      |

`Memo` and `LargeMemo` are mutually exclusive.
Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.

**Name resolution when omitted:**

| Keyword         | Default name                 |
| --------------- | ---------------------------- |
| `Lookup`        | FK referenced table          |
| `Enum`          | field name minus `Id` suffix |
| `Locator`       | FK referenced table          |
| `Code` Provider | TableName                    |
| `Code` Pattern  | `XXX-XXX`                    |


## SQL Type Tokens

RDBMS-neutral tokens replaced at `CREATE TABLE` time:

| Token            | Meaning                      |
| ---------------- | ---------------------------- |
| `@NVARCHAR(n)`   | Unicode string               |
| `@VARCHAR(n)`    | ASCII string                 |
| `@DECIMAL`       | Decimal number               |
| `@DECIMAL_(p,s)` | Decimal with precision/scale |
| `@FLOAT`         | Float                        |
| `@DATE`          | Date                         |
| `@DATE_TIME`     | DateTime                     |
| `@BOOL`          | Boolean                      |
| `@BLOB`          | Binary blob                  |
| `@BLOB_TEXT`     | ASCII text blob              |
| `@NBLOB_TEXT`    | Unicode text blob            |
| `@NOT_NULL`      | NOT NULL constraint          |
| `@NULL`          | NULL constraint              |
| `@AUTO_INC`      | Auto-increment PK            |

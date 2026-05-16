# Registration Builder

`RegBuilder` parses `.sql` schema files written with the server-neutral SQL syntax of Tripous and generates Tripous registration source code.

The input schema acts as a declarative ERP/module definition language.

The builder reads:

- table header metadata
- field inline metadata
- foreign key relations

and generates:

- ordered schema SQL
- `ModuleDef` registrations
- `FormDef` registrations
- `LookupSource` registrations
- `LocatorDef` registrations
- select definitions
- table registration code

---

# Processing Model

The source `.sql` file is the declarative input.

The builder:

- parses table headers, fields and foreign keys
- resolves table dependencies
- calculates table creation order
- rebuilds the schema SQL in dependency order
- injects generated `CreationOrder` metadata into the rebuilt headers
- generates Tripous registration source code

`CreationOrder` is generated output metadata.

It is not required in the initial input schema.

The rebuilt ordered schema is returned through:

```text
SchemaParserResult.SchemaSql
```

---

# Schema Structure

Each table consists of:

- one metadata header
- one `CREATE TABLE` statement
- field definitions
- optional inline field metadata
- foreign keys

Conceptually:

```text
Table Header
CREATE TABLE
Field Definitions
Inline Metadata
Foreign Keys
```

---

# Table Header Metadata

## Input Header Syntax

```sql
/*---------------------------------------------------
Table: TABLE_NAME
Group: GROUP_NAME
Module: Default|MODULE_NAME [MODULE_CLASS_NAME]
Form: Default|FORM_NAME [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]

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

## Generated Header Metadata

The builder adds:

```text
CreationOrder: NUMBER
```

to the rebuilt ordered schema returned by `SchemaParserResult.SchemaSql`.

## Required Input Metadata

- `Table`

## Top Table Metadata

Only top tables use:

- `Module`
- `Group`
- `Form`

## Optional Top Table Metadata

- `Form`
- `IsLookup`
- `NotUiVisible`
- `IsReadOnly`
- `IsSingleSelect`
- `NoFilters`
- `NoCascadeDeletes`
- `NoGuidOids`

---

# Header Boolean Flags

Boolean header options are presence flags.

Examples:

```text
IsLookup
NotUiVisible
IsReadOnly
IsSingleSelect
NoFilters
NoCascadeDeletes
NoGuidOids
```

Meaning:

```text
existence = true
absence = false
```

---

# Default Values

Default module values:

```text
UiVisible = true
IsReadOnly = false
IsSingleSelect = false
UseFilters = true
CascadeDeletes = true
GuidOids = true
```

`IsLookup` is determined by heuristic unless explicitly declared.

Lookup is a table property, not a module property.

---

# Module Header Rules

## Syntax

```text
Module: Default|MODULE_NAME [MODULE_CLASS_NAME]
```

## Default Module

`Default` means:

```text
ModuleName = TableName
```

## Examples

```text
Module: Default

Module: Default LogDataModule

Module: Customer

Module: Customer CustomerDataModule
```

If `ModuleClassName` is omitted, generated code uses the default `DataModule` type.

---

# Form Header Rules

## Syntax

```text
Form: Default|FORM_NAME [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]
```

## Default Form

`Default` means:

```text
FormName = ModuleName
```

## Examples

```text
Form: Default

Form: Default LogDataForm

Form: Default LogDataForm LogItemPage

Form: Customer

Form: Customer CustomerDataForm CustomerItemPage
```

If `Form` is omitted, the builder uses its current/default form registration behavior.

If `FormClassName` is omitted, generated code uses the default `DataForm` type.

If `ItemPageClassName` is omitted, generated code uses the default `ItemPage` type.

---

# Lookup Rules

## Lookup Tables

A table is lookup when:

- the header contains `IsLookup`
- or it is a top table and its native fields are exactly one of:

```text
Id, Name
Id, Code, Name
Id, Name, IsActive
Id, Code, Name, IsActive
```

Explicit `IsLookup` overrides the heuristic.

## Lookup Fields

A field becomes a lookup field when its first inline metadata comment contains:

```text
Lookup
```

Extended syntax:

```text
Lookup [LookupSourceName]
```

Examples:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup
```

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup Currency
```

Rules:

```text
LookupSourceName omitted
    => FK.ReferenceTable

LookupSourceName specified
    => explicit lookup source name
```

Lookup fields are intended for small in-memory reference datasets.

Lookup source forms are resolved automatically from the referenced table metadata.

Conceptually:

```text
LookupSource
    -> TableName
    -> Table Form
    -> LookupSource.Form
```

## Lookup Registration

A `LookupSource` registration is generated when:

- a table is identified as lookup
- a field is declared with `Lookup` metadata

Therefore lookup registration is generated when any of the following is true:

- the table header contains `IsLookup`
- the table matches the lookup field heuristic
- a field contains `-- Lookup`
- a field contains `-- Lookup [LookupSourceName]`

---

# Enum Rules

A field becomes an enum-backed selector when its first inline metadata comment contains:

```text
Enum
```

Extended syntax:

```text
Enum [EnumName]
```

Rules:

```text
EnumName omitted
    => field name without Id suffix
```

---

# Locator Rules

## Locator Fields

A field becomes a locator field when its first inline metadata comment contains:

```text
Locator
```

Extended syntax:

```text
Locator [LocatorName]
```

Examples:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator
```

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator PersonCustomer
```

Rules:

```text
LocatorName omitted
    => FK.ReferenceTable

LocatorName specified
    => explicit locator definition name
```

Locator forms are resolved automatically from the referenced table metadata.

Conceptually:

```text
LocatorDef
    -> SourceTableName
    -> Table Form
    -> LocatorDef.Form
```

## Locator Registration

The builder performs base locator registration only.

Complex locator behavior is intentionally left to the application developer.

Usually the developer additionally configures:

- `LocatorFieldDefs`
- `SelectSql`
- custom joins
- custom search behavior
- custom return fields

The builder provides the registration infrastructure, not a complete locator implementation.

## Locator Joins

When a field has `-- Locator`, the builder uses its foreign key to find the referenced table.

Example:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator

FOREIGN KEY (ProductId) REFERENCES Product(Id)
```

The builder creates a join from the owning table to the referenced table.

Conceptual generated join:

```text
OwnKeyField = ProductId
ForeignTable = Product
ForeignAlias = Product
ForeignPrimaryKey = Id
```

If the same foreign table is joined more than once, aliases are required.

Example:

```text
CustomerId      -> Customer
ManagerId       -> Manager
SalesPersonId   -> SalesPerson
```

The alias acts as a namespace for generated extra fields.

## Locator Extra Fields

Locator returned fields are materialized as extra fields in the owning `TableDef`.

Naming convention:

```text
JoinAlias__SourceField
```

Example:

```text
Product__Code
Product__Name
```

Meaning:

```text
ProductId      <- Product.Id
Product__Code  <- Product.Code
Product__Name  <- Product.Name
```

`ProductId` is persisted.

Alias-based extra fields are non-persistent display/runtime fields.

## Locator In Forms

A locator field is displayed using a searchable locator control.

Example:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator
```

may display:

```text
Customer.Code
Customer.Name
```

User selection writes:

```text
CustomerId <- Customer.Id
```

and updates extra alias-based fields.

## Locator In Grids

Raw FK fields are normally hidden.

Example generated fields:

```text
ProductId
Product__Code
Product__Name
```

Grid displays:

```text
Product__Code
Product__Name
```

instead of the raw FK value.

---

# Field Metadata

Field inline comments may contain both metadata and plain comments.

Syntax:

```text
-- METADATA -- COMMENT
```

Example:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
```

Rules:

```text
text before the second --
    => field metadata

text after the second --
    => plain field comment
```

## Master

The table is a detail table and this field points to the master table.

`Master OneToOne` designates a single-row detail table.

## Lookup

Defines a small in-memory reference selector.

Syntax:

```text
Lookup [LookupSourceName]
```

If omitted:

```text
LookupSourceName
    => FK.ReferenceTable
```

## Enum

Defines an enum-backed selector.

Syntax:

```text
Enum [EnumName]
```

If omitted:

```text
EnumName
    => field name without Id suffix
```

## Locator

Defines a searchable large reference selector.

Syntax:

```text
Locator [LocatorName]
```

If omitted:

```text
LocatorName
    => FK.ReferenceTable
```

## LargeMemo

Generates a text blob field with `LargeMemo` flag.

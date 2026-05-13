# Registration Builder

This application parses `.sql` schema files written using the server-neutral SQL syntax of Tripous.

Based on metadata comments declared:

* above `CREATE TABLE` statements
* and next to field definitions

the builder generates Tripous registration source code such as:

* `ModuleDef`
* `FormDef`
* `LocatorDef`
* lookup registrations
* locator registrations
* select definitions
* table registration code

The schema file acts as a declarative ERP/module definition language.

---

# Registration Schema Parsing Rules

## General

The schema file is the single declarative source for:

* create table SQL
* table metadata
* module metadata
* form metadata
* custom runtime type metadata
* master/detail relations
* lookup behavior
* locator behavior
* generated registration source code

Each table is declared as:

* one header comment
* followed by one `CREATE TABLE` block

---

# Table Header

## Header Format

```sql
/*---------------------------------------------------
Table: TABLE_NAME
Group: GROUP_NAME
Module: Default|MODULE_NAME [MODULE_CLASS_NAME]
Form: Default|FORM_NAME [FORM_CLASS_NAME] [ITEM_PAGE_CLASS_NAME]
Master: MASTER_TABLE_NAME

IsLookup
NotUiVisible
IsReadOnly

IsSingleSelect
NoFilters
NoCascadeDeletes
NoGuidOids

CreationOrder: NUMBER
-----------------------------------------------------
    comments / examples
----------------------------------------------------*/
```

## Required

* `Table`
* `CreationOrder`

## Only Top Tables Have

* `Module`
* `Group`
* `Form`

## Only Detail Tables Have

* `Master`

## Optional, On Top Tables Only

* `Form`
* `IsLookup`
* `NotUiVisible`
* `IsReadOnly`
* `IsSingleSelect`
* `NoFilters`
* `NoCascadeDeletes`
* `NoGuidOids`

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

* existence = true
* absence = false

---

# Module Defaults

Default values:

```text
UiVisible = true
IsReadOnly = false
IsSingleSelect = false
UseFilters = true
CascadeDeletes = true
GuidOids = true
```

`IsLookup` is determined by heuristic unless explicitly declared.

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

If `Form` is omitted, builder uses current/default form registration behavior.

If `FormClassName` is omitted, generated code uses the default `DataForm` type.

If `ItemPageClassName` is omitted, generated code uses the default `ItemPage` type.

---

# Read Only Rule

Header may contain:

```text
IsReadOnly
```

Example:

```sql
/*---------------------------------------------------
Table: SYS_LOG
Group: Log
Module: Log LogDataModule
Form: Default LogDataForm LogItemPage
IsReadOnly
CreationOrder: 100
----------------------------------------------------*/
```

Meaning:

```text
FormDef.IsReadOnly = true
```

Typical usage:

* system forms
* logs
* audit/history tables
* administration views

---

# Lookup Table Rules

A table is lookup when:

1. Header contains `IsLookup`
2. Or it is a top table and its native fields are exactly one of:

```text
Id, Name
Id, Code, Name
Id, Name, IsActive
Id, Code, Name, IsActive
```

Lookup heuristic is overridden by explicit header flags.

Lookup is a table property, not a module property.

---

# Lookup Field Rules

A field becomes a lookup field when its first inline metadata comment contains:

```text
Lookup
```

Example:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
```

Lookup fields are intended for small in-memory reference datasets, such as:

* countries
* currencies
* units of measure
* tax offices
* payment methods

Generated behavior:

* the field is registered as lookup id field
* the lookup source is derived from the foreign key reference table when available
* otherwise the lookup source is derived from the field name without the `Id` suffix

Typical generated methods:

* `AddStringLookupId()`
* `AddIntegerLookupId()`

---

# Locator Field Rules

A field becomes a locator field when its first inline metadata comment contains:

```text
Locator
```

Extended syntax:

```text
Locator [LocatorName] [(DisplayFields | SearchFields | ReturnFields)]
```

Examples:

```sql
-- Locator
```

```sql
-- Locator Customer
```

```sql
-- Locator Customer(Code, Name)
```

```sql
-- Locator Customer(Code, Name | Default | Id, Code, Name, BirthDay)
```

Rules:

```text
second token after Locator
    => LocatorName

(...) section
    => field definitions
```

Default rules:

```text
LocatorName
    => FK.ReferenceTable when omitted

SearchFields
    => DisplayFields when omitted or Default

ReturnFields
    => Id + DisplayFields when omitted or Default
```

Examples:

```text
Locator Customer(Code, Name)
```

means:

```text
DisplayFields = Code, Name
SearchFields = Code, Name
ReturnFields = Id, Code, Name
```

and:

```text
Locator Customer(Code, Name | Default | Id, Code, Name, BirthDay)
```

means:

```text
DisplayFields = Code, Name
SearchFields = Code, Name
ReturnFields = Id, Code, Name, BirthDay
```

Example on a top table:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator
```

Example on a detail table:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
```

Locator fields are intended for large searchable reference datasets, such as:

* customers
* products
* persons
* projects
* documents

A locator is not a dropdown lookup.

A locator is a search-assisted relation binding mechanism.

It locates a row in a large reference table and returns the selected key and display fields.

---

# Locator Definition Rules

A `LocatorDef` describes how to search and display a large reference table.

A locator definition contains:

```text
Name
TableName
KeyField
DisplayFields
SearchFields
ReturnFields
```

Default rules:

```text
KeyField = Id
SearchFields = DisplayFields, when omitted
ReturnFields = DisplayFields, when omitted
```

`LocatorDef` does not describe where returned values are assigned.

Assignment is handled by table joins and naming conventions.

---

# Locator Join Rules

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

If the same foreign table is joined more than once, aliases are required to keep generated field names unique.

The alias acts as a namespace for generated extra fields.

---

# Locator Extra Field Naming Convention

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

For a field:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
```

and a referenced table:

```text
Product(Id, Code, Name)
```

the generated table fields may be:

```text
ProductId
Product__Code
Product__Name
```

Meaning:

```text
ProductId      <- Product.Id
Product__Code  <- Product.Code
Product__Name  <- Product.Name
```

`ProductId` is the persisted foreign key field.

`Product__Code` and `Product__Name` are non-persistent extra display fields.

---

# Locator In Forms

In a form, a locator field is displayed by a locator control.

For example:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator
```

may be displayed as a composite control with searchable boxes such as:

```text
Customer.Code
Customer.Name
```

When the user selects a customer, the locator writes:

```text
CustomerId <- Customer.Id
```

and updates any available alias-based extra fields.

---

# Locator In Grids

In a grid, the raw foreign key field is normally hidden.

For example, a detail table:

```text
TradeLines
    Id
    TradeId
    ProductId
```

may be expanded in the `MemTable` / `TableDef` as:

```text
TradeLines
    Id
    TradeId
    ProductId
    Product__Code
    Product__Name
```

The grid displays:

```text
Product__Code
Product__Name
```

instead of the raw `ProductId`.

When the user selects a product, the locator writes:

```text
ProductId      <- Product.Id
Product__Code  <- Product.Code
Product__Name  <- Product.Name
```

---

# Locator Select SQL Rules

Locator SQL can be generated from `LocatorDef` metadata.

Conceptual method:

```text
GenerateLocatorSelectSql(TableAlias)
```

Example locator:

```text
LocatorDef Product
    TableName = Product
    KeyField = Id
    DisplayFields = Code, Name
```

Generated SQL with alias `Product`:

```sql
select
    Product.Code,
    Product.Name
from
    Product Product
```

If `ReturnFields` is explicitly defined, those fields are selected instead.

The table alias is important because the same table may be used more than once in the same module.

---

# UI Visibility Rules

Default:

```text
UiVisible = true
```

If header contains:

```text
NotUiVisible
```

then:

```text
UiVisible = false
```

Typical usage:

* hidden tables
* system tables
* implementation-only tables

---

# Module Option Rules

Default values:

```text
IsSingleSelect = false
UseFilters = true
CascadeDeletes = true
GuidOids = true
```

Flags:

```text
IsSingleSelect
    -> IsSingleSelect = true

NoFilters
    -> UseFilters = false

NoCascadeDeletes
    -> CascadeDeletes = false

NoGuidOids
    -> GuidOids = false
```

---

# Table Body

`CreateTableSql` preserves:

* original header
* original `CREATE TABLE` text
* comments

Parser ignores:

* block comments inside `CREATE TABLE`
* example text outside `CREATE TABLE`
* constraints when parsing fields

---

# Create Order

`CreationOrder` is used for executing `CREATE TABLE` statements.

Foreign key dependency sorting may additionally be used as validation.

---

# Field Comment Metadata

For field lines, the first inline SQL comment may contain metadata.

Example:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
```

Metadata keyword:

```text
Lookup
```

Remaining text after the second `--` is plain comment.

---

# Recognized Field Metadata Keywords

```text
Master [OneToOne]
Lookup
Enum
Locator
Correlation Lookup
Correlation Locator
LargeMemo
```

---

# Field Metadata Meaning

## Master

The table is a detail table and this field points to the master table.

`Master OneToOne` designates a single-row detail table.

## Lookup

Defines a small in-memory reference selector.

Used for small datasets.

Generates lookup id field registration.

## Enum

Generates:

* `AddEnumLookupId()`

Enum type is derived from the field name without the `Id` suffix.

## Locator

Defines a searchable large reference selector.

Used for large datasets.

Generates locator-related metadata, joins and extra display fields.

## Correlation Lookup

Junction/correlation field pointing to a lookup table.

## Correlation Locator

Junction/correlation field pointing to a non-lookup/master table.

## LargeMemo

Generates a text blob field with `LargeMemo` flag.

---

# Foreign Keys

Foreign key clauses are parsed for:

* dependencies
* select joins
* validation
* lookup source resolution
* locator table resolution

Foreign keys do not alone decide lookup or locator behavior.

Lookup and locator behavior comes from field metadata.

Foreign keys provide the referenced table and referenced key field.

---

# Module Rules

A module is created from a top table header.

Header values define:

* top table
* module group
* module name
* module class name
* form name
* form class name
* item page class name

Details are attached using:

* `Master` header
* `Master` field metadata
* foreign keys

A module tree consists of:

* one top table
* zero or more detail tables
* nested details

---

# Form Rules

A form is created from a top table header.

If `Form` is omitted:

* builder uses default form registration behavior

If `Form` exists:

* `FormName` comes from `Form`
* `Default` means `FormName = ModuleName`
* `FormClassName` is optional
* `ItemPageClassName` is optional

Generated `FormDef` supports:

* `FormName`
* `ModuleName`
* `Group`
* `ClassName`
* `ItemClassName`
* `IsReadOnly`

---

# Select Rules

`ListSelectSql` is generated from the top table.

It may include `LEFT JOIN` clauses for display fields.

Filter fields are taken from generated select aliases.

## Filterable Types

* string
* numeric
* date
* datetime
* boolean

## Not Filterable

* blobs
* text blobs
* raw Id fields unless explicitly needed later

---

# Example: Custom Log Module

```sql
/*---------------------------------------------------
Table: SYS_LOG
Module: Log LogDataModule
Group: Log
Form: Default LogDataForm LogItemPage
IsReadOnly
CreationOrder: 100
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,
    Year int @NOT_NULL,
    Month int @NOT_NULL,
    DayOfMonth int @NOT_NULL,
    LogTime @NVARCHAR(20) @NOT_NULL,
    User @NVARCHAR(96) @NOT_NULL,
    Host @NVARCHAR(96) @NOT_NULL,
    Level @NVARCHAR(96) @NOT_NULL,
    Source @NVARCHAR(512) @NOT_NULL,
    Scope @NVARCHAR(512) @NOT_NULL,
    EventId @NVARCHAR(96) @NOT_NULL,
    Message @NBLOB_TEXT @NOT_NULL
    )
```

Generated behavior:

* module name = `Log`
* module class = `LogDataModule`
* form name = `Log`
* form class = `LogDataForm`
* item page class = `LogItemPage`
* form is read-only

---

# Example: Locator Field

```sql
/*---------------------------------------------------
Table: TradeLine
Master: Trade
CreationOrder: 500
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,
    TradeId @NVARCHAR(40) @NOT_NULL,      -- Master
    ProductId @NVARCHAR(40) @NOT_NULL,    -- Locator
    Qty @DECIMAL @NOT_NULL,

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
    )
```

Generated behavior:

* `ProductId` remains the persisted foreign key
* a join to `Product` is created
* extra display fields are generated using the join alias
* grid/form locator controls use the locator metadata

Example extra fields:

```text
Product__Code
Product__Name
```

Example assignments after user selection:

```text
ProductId      <- Product.Id
Product__Code  <- Product.Code
Product__Name  <- Product.Name
```

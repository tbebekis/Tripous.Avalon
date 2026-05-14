# Registration Builder

This application parses `.sql` schema files written using the server-neutral SQL syntax of Tripous.

Based on metadata comments declared:

* above `CREATE TABLE` statements
* next to field definitions
* inside metadata definition blocks

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
* enum behavior
* generated registration source code

Each table is declared as:

* one header comment
* followed by one `CREATE TABLE` block

---

# Global Metadata Definition Blocks

The schema may contain global metadata definition blocks.

Example:

```text
Locators begin
Customer (Code, Name | Default | Id, Code, Name)
Product (Code, Name)
Locators end

Enums begin
WarehouseType
TradeType
Enums end
````

Blocks are parsed before table parsing.

---

# Locator Definition Block

## Syntax

```text
Locators begin
LocatorName (DisplayFields | SearchFields | ReturnFields)
Locators end
```

## Examples

```text
Locators begin

Customer (Code, Name)

Product (Code, Name | Default | Id, Code, Name, VatRateId)

Person (Code, LastName, FirstName)

Locators end
```

---

# Locator Definition Rules

A locator definition describes how a locator searches and displays a referenced table.

A locator definition contains:

```text
Name
DisplayFields
SearchFields
ReturnFields
```

Default rules:

```text
SearchFields
    => DisplayFields

ReturnFields
    => Id + DisplayFields
```

`Default` means:

```text
use default generated behavior
```

Example:

```text
Customer (Code, Name)
```

means:

```text
DisplayFields = Code, Name
SearchFields = Code, Name
ReturnFields = Id, Code, Name
```

Example:

```text
Customer (Code, Name | Default | Id, Code, Name, VatRateId)
```

means:

```text
DisplayFields = Code, Name
SearchFields = Code, Name
ReturnFields = Id, Code, Name, VatRateId
```

---

# Enum Definition Block

## Syntax

```text
Enums begin
EnumName
Enums end
```

## Example

```text
Enums begin

WarehouseType
TradeType
VatMode

Enums end
```

The enum definition block allows future extensibility for enum metadata.

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
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup
```

Lookup fields are intended for small in-memory reference datasets.

---

# Locator Field Rules

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
-- Locator
```

```sql
-- Locator Customer
```

Rules:

```text
LocatorName omitted
    => FK.ReferenceTable

LocatorName specified
    => explicit locator definition name
```

Examples:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator
```

means:

```text
LocatorName = Customer
```

and:

```sql
CustomerId @NVARCHAR(40) @NULL, -- Locator PersonCustomer
```

means:

```text
LocatorName = PersonCustomer
```

---

# Locator Runtime Rules

Locator runtime behavior is determined using:

```text
1. explicit locator definition block
2. otherwise generated defaults
```

Meaning:

```text
Locators block
    overrides defaults
```

If no explicit locator definition exists:

```text
DisplayFields
    => generated defaults

SearchFields
    => DisplayFields

ReturnFields
    => Id + DisplayFields
```

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

If the same foreign table is joined more than once, aliases are required.

Example:

```text
CustomerId      -> Customer
ManagerId       -> Manager
SalesPersonId   -> SalesPerson
```

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

Meaning:

```text
ProductId      <- Product.Id
Product__Code  <- Product.Code
Product__Name  <- Product.Name
```

`ProductId` is persisted.

Alias-based extra fields are non-persistent display/runtime fields.

---

# Locator In Forms

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

---

# Locator In Grids

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

# Recognized Field Metadata Keywords

```text
Master [OneToOne]
Lookup
Enum [EnumName]
Locator [LocatorName]
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

## Correlation Lookup

Junction/correlation field pointing to a lookup table.

## Correlation Locator

Junction/correlation field pointing to a non-lookup/master table.

## LargeMemo

Generates a text blob field with `LargeMemo` flag.




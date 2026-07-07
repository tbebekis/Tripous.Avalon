# Metadata Comments

Metadata comments are the Tripous syntax used by the Registration Builder inside `Schema.sql` files.

They let a SQL table definition also describe application declarations.

The Registration Builder parses those comments and generates C# registration code.

## Comment Types

There are two kinds of metadata comments:

- Header metadata comments.
- Field metadata comments.

Header metadata appears before a `CREATE TABLE` statement.

Field metadata appears after a field definition.

```sql
/*---------------------------------------------------
Table: Product
Module: Product ProductDataModule
Group: Inventory
Form: Product ProductForm
FilterFields: Code, Name
IsLookup
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,        -- Code PRD-XXXX
    Name @NVARCHAR(96) @NOT_NULL,
    Notes @NBLOB_TEXT @NULL              -- LargeMemo; Group Notes
    )
```

The header describes table and module intent.

The field comments describe field-level behavior.

## Top Table Rule

A module has one and only one top table.

Only the top table declares module-level metadata.

Examples:

- `Module`
- `Group`
- `Form`
- `ItemPage`
- `DetailOrder`
- `Code`
- `ListWhere`
- `FilterFields`

Detail and subdetail tables do not declare module metadata unless they are also exposed as separate top-level modules.

This keeps module ownership clear.

## Header Syntax

General header shape:

```sql
/*---------------------------------------------------
Table: TABLE_NAME
Module: Default | MODULE_NAME [MODULE_CLASS_NAME]
Group: GROUP_NAME
Form: DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
DetailOrder: PARENT_TABLE_NAME=DETAIL_TABLE_NAME, DETAIL_TABLE_NAME
Code: Code [Draft] [Pattern] [ProviderName]
ListWhere: SQL_CONDITION
FilterFields: Field1, Field2, FieldN
FieldGroups: Address, Billing, Notes

IsLookup
NotUiVisible
IsReadOnly
IsSingleSelect
NoFilters
NoCascadeDeletes
NoGuidOids
-----------------------------------------------------
free text comments
----------------------------------------------------*/
```

`Table` is always required.

`Table` must be the first metadata entry.

If the table is a top table, `Table` is followed by one or more module blocks.

Table-level metadata such as `FieldGroups` and boolean flags follows the module blocks.

## Module Blocks

Each `Module` line starts a module block.

The following entries belong to that module block until the next `Module` line or until non-module header metadata begins.

Module block order:

- `Module`
- `Group`
- `Form`
- `ItemPage`
- `DetailOrder`
- `Code`
- `ListWhere`
- `FilterFields`

`Module` and `Group` are required for each module block.

The other entries are optional.

## Multiple Modules On One Table

A single top table may declare multiple modules.

This is useful when the same physical table stores multiple document types.

Example:

```sql
/*---------------------------------------------------
Table: Trade

Module: SalesOrder SalesOrderDataModule
Group: Sales
Form: SalesOrder SalesOrderForm
ItemPage: TradeItemPage
DetailOrder: Trade=TradeLine, TradeTax
DetailOrder: TradeLine=TradeLineTax
Code: Draft SO-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesOrder'
FilterFields: Code, TradeDate, Person__Name, TradeStatus, TotalAmount

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
Form: SalesInvoice SalesInvoiceForm
ItemPage: TradeItemPage
DetailOrder: Trade=TradeLine, TradeTax
DetailOrder: TradeLine=TradeLineTax
Code: Draft SINV-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesInvoice'
FilterFields: Code, TradeDate, Person__Name, TradeStatus, TotalAmount

FieldGroups: Totals, Billing, Shipping, Organization, Relations, Audit, Notes
----------------------------------------------------*/
```

Each module gets its own generated `ModuleDef`, list SQL, filters and form registration.

The table remains one physical table.

## Module Syntax

```sql
Module: Default
Module: Default LogDataModule
Module: Customer
Module: Customer CustomerDataModule
```

Rules:

- `Default` means the module name defaults to the table name.
- If the module class name is omitted, the default `DataModule` type is used.
- If a module class name is declared, the generated `ModuleDef` references it.

## Form Syntax

```sql
Form: DataForm
Form: Customer
Form: Customer CustomerDataForm
```

Rules:

- `Form: DataForm` means default form registration for the module.
- If `Form` is omitted, the form name defaults to the module name and the form class defaults to `DataForm`.
- To declare a form class name, the form name must also be declared.

## ItemPage Syntax

```sql
ItemPage: ItemPage
ItemPage: CustomerItemPage
```

`ItemPage` declares an item page class name.

If omitted, the default item page class is `ItemPage`.

## DetailOrder Syntax

```sql
DetailOrder: Trade=TradeLine, TradeTax
DetailOrder: TradeLine=TradeLineTax
```

`DetailOrder` controls the preferred display order of direct child detail tabs.

Rules:

- It belongs to the current module block.
- It may appear multiple times for different parent tables.
- Details not listed remain at the end in declaration order.

## Code Syntax

Header-level code syntax:

```sql
Code: Code [Draft] [Pattern] [ProviderName]
Code: Draft SO-YYYY-XXXXXX
Code: Draft SO-YYYY-XXXXXX SALES_ORDER
```

Field-level code syntax:

```sql
Code @NVARCHAR(40) @NOT_NULL, -- Code SO-YYYY-XXXXXX SALES_ORDER
Code @NVARCHAR(40) @NOT_NULL, -- Code Draft SO-YYYY-XXXXXX SALES_ORDER
```

Rules:

- Header `Code` belongs to the current module block.
- If header `Code` omits `ProviderName`, provider name defaults to the module name.
- If header `Code` is omitted, field `-- Code` metadata is used as fallback.
- `Draft` generates a `DRAFT-` provider and the normal provider.
- Code provider patterns generate `SYS_NUMBER_SERIES` seed statements in generated schema versions.

## ListWhere Syntax

```sql
ListWhere: DocumentType.ModuleName = 'SalesOrder'
ListWhere: Trade.IsActive = 1
```

Rules:

- It belongs to the current module block.
- Write only the condition.
- Do not include the `WHERE` keyword.
- Only one `ListWhere` is allowed per module block.
- It may reference the top table and generated join aliases.

The Registration Builder appends it as a `where` condition after generated joins.

## FilterFields Syntax

```sql
FilterFields: Code, TradeDate, Person__Name, TradeStatus
```

Rules:

- It belongs to the current module block.
- Names are resolved against the final generated list select columns.
- Join aliases such as `Person__Name` may be used.
- Enum display columns such as `TradeStatus` may be used.
- Unknown names are validation errors.
- Duplicate names are validation errors.
- Only one `FilterFields` declaration is allowed per module block.
- If omitted, automatic filter generation is used.

## FieldGroups Syntax

```sql
FieldGroups: Address, Billing, Notes
```

`FieldGroups` is table-level metadata.

It defines item page group display order.

The `General` group is built in and always exists.

Fields without explicit `Group` metadata are assigned to `General`.

Field group matching is case-insensitive.

If `FieldGroups` is present, `General` is still inserted as the first group even when it is not declared.

Example:

```text
FieldGroups: Address, Billing, Notes
```

is treated internally as:

```text
General, Address, Billing, Notes
```

Any group referenced by a field but not listed in `FieldGroups` is appended after the declared groups in order of first appearance.

If `FieldGroups` is omitted, groups are ordered by first appearance, with `General` remaining first.

Fields may declare a group like this:

```sql
Street @NVARCHAR(96) @NULL, -- Group Address
Remarks @NBLOB_TEXT @NULL, -- LargeMemo; Group Notes
```

## Header Boolean Flags

Boolean flags are true when present and false when absent.

| Flag | Default |
| --- | --- |
| `IsLookup` | heuristic |
| `NotUiVisible` | false |
| `IsReadOnly` | false |
| `IsSingleSelect` | false |
| `NoFilters` | false |
| `NoCascadeDeletes` | false |
| `NoGuidOids` | false |

Example:

```sql
IsLookup
NoCascadeDeletes
NoGuidOids
```

## Field Metadata Syntax

Field metadata appears after the first `--`.

A second `--` separates metadata from a plain human comment.

Multiple metadata entries are separated with `;`.

```sql
FieldName TYPE, -- METADATA -- plain comment
```

Examples:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General; [ReadOnlyUI, ReadOnlyEdit] -- customer code
TradeStatusId int default 0 @NOT_NULL, -- Enum TradeStatus; [ReadOnlyUI]
ProductCode @NVARCHAR(40) @NULL, -- Snapshot Product.Code
SupplierCode @NVARCHAR(96) @NULL, -- TitleKey Supplier Product Code
Notes @BLOB_TEXT @NULL, -- Memo; Group Notes -- short notes
Remarks @NBLOB_TEXT @NULL, -- LargeMemo; Group Notes -- long notes
Photo @BLOB @NULL, -- [Image] -- product photo
```

## Field Metadata Keywords

| Keyword | Syntax | Meaning |
| --- | --- | --- |
| `Master` | `Master` or `Master OneToOne` | Foreign key to parent table. |
| `Lookup` | `Lookup [LOOKUP_NAME] [TableName:TABLE_NAME | EnumName:ENUM_NAME | ClassName:LOOKUP_SOURCE_CLASS_NAME]` | Small reference selector. |
| `Enum` | `Enum [EnumName]` | Enum-backed selector. |
| `Locator` | `Locator [LOCATOR_NAME] [ClassName:LOCATOR_CLASS_NAME] [Form:FORM_NAME] [WebForm:WEB_FORM_NAME]` | Searchable reference selector. |
| `Code` | `Code [Draft] [Pattern] [ProviderName]` | Auto-generated code field. |
| `Memo` | `Memo` | Text field with memo behavior. |
| `LargeMemo` | `LargeMemo` | Text blob with large memo behavior. |
| `Group` | `Group GroupName` | Field UI group. |
| `Snapshot` | `Snapshot TableName.FieldName` | Persisted copy of a related source field. |
| `TitleKey` | `TitleKey KeyOrText` | Field title resource key or fallback text. |
| `FieldFlags` | `[Flag1, Flag2]` | Adds `FieldFlags` values to the field. |

Notes:

- `Memo` and `LargeMemo` are mutually exclusive.
- `FieldFlags` names are parsed from the `FieldFlags` enum.
- For `FieldFlags`, square brackets are part of the actual schema syntax.
- In syntax descriptions, square brackets mean optional arguments unless the keyword is `FieldFlags`.
- `Snapshot` requires an existing source table and field.
- `TitleKey` uses the remaining text up to the next `;`.

## Common FieldFlags

Common `FieldFlags` values include:

- `Hidden`
- `ReadOnly`
- `ReadOnlyUI`
- `ReadOnlyEdit`
- `Required`
- `Boolean`
- `Memo`
- `LargeMemo`
- `Image`
- `ImagePath`
- `NoInsertUpdate`
- `ForeignKey`
- `Extra`
- `Searchable`

## Default Name Resolution

When names are omitted, the Registration Builder uses conventions.

| Metadata | Default name |
| --- | --- |
| `Lookup` | Foreign key referenced table |
| `Enum` | Field name minus `Id` suffix |
| `Locator` | Foreign key referenced table |
| field `Code` provider | Table name |
| header `Code` provider | Module name |
| `Code` pattern | `XXXXXX` |

## Lookup Metadata

Examples:

```sql
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer
PersonId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer TableName:Person
TradeTypeId int @NOT_NULL, -- Lookup TradeType EnumName:TradeType
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
```

Rules:

- If `LOOKUP_NAME` is omitted, it is resolved from the foreign key referenced table or from the field name without `Id`.
- If `TableName:`, `EnumName:` or `ClassName:` is used, `LOOKUP_NAME` is required.
- `TableName:` generates table-backed lookup registration.
- `EnumName:` generates enum lookup source registration.
- `ClassName:` generates class-backed lookup source registration.

## Locator Metadata

Examples:

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product ClassName:ProductLocator
CustomerId @NVARCHAR(40) @NOT_NULL, -- Locator Customer Form:Person WebForm:Person
PaymentId @NVARCHAR(40) @NOT_NULL, -- Locator Payment ClassName:PaymentLocator Form:CustomerReceipt WebForm:CustomerReceipt
```

Rules:

- If `LOCATOR_NAME` is omitted, it is resolved from the foreign key referenced table.
- If `ClassName:`, `Form:`, or `WebForm:` is used, `LOCATOR_NAME` is required.
- The builder generates `LocatorDef2` registration.
- The generated table field references the locator name.
- `Form:` is the desktop reference form name used by locator reference menus.
- `WebForm:` is the web reference form name used by web locator reference menus.
- For FK-backed generated locators, if `Form:` is omitted, the builder uses the referenced table form.
- For FK-backed generated locators, if `WebForm:` is omitted, the builder uses the referenced table web form, falling back to `Form:`.
- At runtime, `LocatorDef2.Form` falls back to the locator name and `LocatorDef2.WebForm` falls back to `Form`.
- For table-backed locators where the locator name is the referenced module/form name, the defaults are usually enough.
- For custom SQL, service, or cross-module locators, prefer explicit `Form:` and `WebForm:`.

Join aliases use this convention:

```text
JOIN_ALIAS__FIELD_NAME
```

Example:

```text
Product__Code
Product__Name
```

Snapshot fields may receive locator values when they point to the same joined source field.

```sql
ProductCode @NVARCHAR(40) @NULL, -- Snapshot Product.Code
ProductName @NVARCHAR(96) @NULL, -- Snapshot Product.Name
```

The preferred locator field names are the source field names, such as `Code` and `Name`.

When a custom locator SELECT returns different column names, keep the source name and set an explicit alias in code.

```csharp
LocatorDef.Add("Code", DataFieldType.String, TargetField: null, Alias: "ProductCode", TitleKey: null, IsVisible: true, IsSearchable: true);
LocatorDef.Add("Name", DataFieldType.String, TargetField: null, Alias: "ProductName", TitleKey: null, IsVisible: true, IsSearchable: true);
```

Then the SELECT may return the aliased columns:

```sql
select
     P.Id as Id
    ,P.Code as ProductCode
    ,P.Name as ProductName
from Product P
```

## Table Lookup Detection

A table is identified as a lookup when:

- Its header contains `IsLookup`, or
- it is a top table whose native fields match one of the known lookup shapes.

Common lookup shapes:

```text
Id, Name
Id, Code, Name
Id, Name, IsActive
Id, Code, Name, IsActive
```

Explicit `IsLookup` overrides the heuristic.

## Practical Rule

When writing metadata comments:

- Put module metadata only on top tables.
- Keep module blocks ordered and easy to scan.
- Use `ListWhere` without the `WHERE` keyword.
- Use `FilterFields` names from the final generated list select.
- Keep field metadata close to the field it describes.
- Separate multiple field metadata entries with `;`.
- Use a second `--` for plain human comments.
- Use `Lookup` for small selectors.
- Use `Locator` for searchable references.
- Use `Snapshot` for persisted copies of related fields.
- Use header `Code` for multi-module document tables.
- Do not put business behavior in metadata comments.

## Related Articles

- [Schema.sql Files](schema-sql-files.md)
- [Registration Builder](reg-builder.md)
- [Generated Modules](generated-modules.md)
- [Generated Lookups](generated-lookups.md)
- [Generated Locators](generated-locators.md)
- [Generated Code Providers](generated-code-providers.md)

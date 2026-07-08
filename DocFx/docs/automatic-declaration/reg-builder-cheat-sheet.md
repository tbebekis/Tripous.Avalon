# RegBuilder Cheat Sheet

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
FilterFields: Field1, Field2, FieldN

Module: MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
Code:   Code [Draft] [Pattern] [ProviderName]
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
  comments / examples
----------------------------------------------------*/
```

## Required

- `Table` - always required

## Module Blocks

- `Table` must be the first metadata entry.
- A top table may declare one or more module blocks.
- Each `Module` line starts a new module block.
- The following `Group`, optional `Form`, optional `ItemPage`, optional `DetailOrder`, optional `Code`, optional `ListWhere`, and optional `FilterFields` belong to that module block.
- A module block is complete when the next `Module` line starts or when non-module header metadata begins.
- Module block order is `Module`, `Group`, `Form`, `ItemPage`, `DetailOrder`, `Code`, `ListWhere`, `FilterFields`.
- `Group` is required for each module block.
- If `Form` is omitted, the form name defaults to the module name and the form class defaults to `DataForm`.
- If `ItemPage` is omitted, the item page class defaults to `ItemPage`.
- `DetailOrder: Trade=TradeLine, TradeTax` orders the direct child detail tabs of `Trade`. Multiple declarations are allowed for different parent tables. Unlisted details remain at the end.
- Example nested order: `DetailOrder: BillOfMaterial=BillOfMaterialLine, BillOfMaterialCost`.
- `ListWhere` adds a module-specific condition to the generated list SELECT. Write only the condition, without `WHERE`.
- `ListWhere` may reference the top table and generated join aliases.
- `FilterFields` limits generated filters to the specified List SELECT column names and preserves their declared order.
- Without `FilterFields`, the existing automatic filter generation is used.
- `Code` is optional and uses the same syntax as field `-- Code`.
- If header `Code:` omits `ProviderName`, provider name defaults to `ModuleName`.
- If header `Code:` is omitted, field `-- Code` metadata is used as fallback for that module.

## Form Syntax

```sql
Form: DataForm
Form: Customer
Form: Customer CustomerDataForm
```

- `Form: DataForm` means default form registration for the module.
- To declare `FORM_CLASS_NAME`, `FORM_NAME` must also be declared.

## ItemPage Syntax

```sql
ItemPage: ItemPage
ItemPage: CustomerItemPage
```

- `ItemPage` has no name, only class name.

## ListWhere Syntax

```sql
ListWhere: DocumentType.ModuleName = 'SalesOrder'
ListWhere: Trade.IsActive = 1
```

- `ListWhere` is optional and belongs to the current module block.
- The RegBuilder appends it as a `where` clause after the generated joins.
- Do not include the `WHERE` keyword.
- Only one `ListWhere` is allowed per module block.

## FilterFields Syntax

```sql
FilterFields: Code, TradeDate, Person__Name, TradeStatus
```

- `FilterFields` is optional and belongs to the current module block.
- Names are resolved against the final generated List SELECT columns, including join aliases and enum display columns.
- Unknown and duplicate names are validation errors.
- Only one `FilterFields` declaration is allowed per module block.
- When omitted, filters are generated automatically.

## Boolean Flags

Presence = `true`, absence = `false`.

| Flag | Default |
| --- | --- |
| `IsLookup` | heuristic |
| `NotUiVisible` | false |
| `IsReadOnly` | false |
| `IsSingleSelect` | false |
| `NoFilters` | false |
| `NoCascadeDeletes` | false |
| `NoGuidOids` | false |

## Field Syntax

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

## Metadata Keywords

| Keyword | Syntax | Meaning |
| --- | --- | --- |
| `Master` | `Master` / `Master OneToOne` | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup` | `Lookup [LOOKUP_NAME] [TableName:TABLE_NAME \| EnumName:ENUM_NAME \| ClassName:LOOKUP_SOURCE_CLASS_NAME]` | Small in-memory reference selector |
| `Enum` | `Enum [EnumName]` | Enum-backed selector |
| `Locator` | `Locator [LOCATOR_NAME] [ClassName:LOCATOR_CLASS_NAME] [Form:FORM_NAME] [WebForm:WEB_FORM_NAME]` | Searchable large reference selector |
| `Code` | `Code [Draft] [Pattern] [ProviderName]` | Auto-generated code field |
| `Memo` | `Memo` | Text field with Memo flag |
| `LargeMemo` | `LargeMemo` | Text blob with LargeMemo flag |
| `Group` | `Group GroupName` | Field UI group |
| `Snapshot` | `Snapshot TableName.FieldName` | Persisted copy of a related source field |
| `TitleKey` | `TitleKey KeyOrText` | Field title resource key or fallback text |
| `FieldFlags` | `[Flag1, Flag2]` | Adds `FieldFlags` values to the field |

- `Memo` and `LargeMemo` are mutually exclusive.
- `FieldFlags` names are parsed from the `FieldFlags` enum.
- Common `FieldFlags`: `Hidden`, `ReadOnly`, `ReadOnlyUI`, `ReadOnlyEdit`, `Required`, `Boolean`, `Memo`, `LargeMemo`, `Image`, `ImagePath`, `NoInsertUpdate`, `ForeignKey`, `Extra`, `Searchable`.
- Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.
- For `FieldFlags`, square brackets are part of the actual schema syntax.
- `Snapshot` requires an existing source table and field and generates `SetSnapshotOf()`.
- `TitleKey` uses the remaining text up to the next `;` and generates `SetTitleKey()`.
- `Code Draft PATTERN PROVIDER_NAME` generates `DRAFT-PROVIDER_NAME` with `DRAFT-PATTERN` and also the normal provider.
- Multi-module document tables should use header `Code: Draft PATTERN PROVIDER_NAME`; snapshot document modules should not declare `-- Code`.
- Code provider patterns generate `SchemaVersion.AddStatementAfter()` inserts into `SYS_NUMBER_SERIES`.

## Name Resolution

| Keyword | Default name |
| --- | --- |
| `Lookup` | FK referenced table |
| `Enum` | field name minus `Id` suffix |
| `Locator` | FK referenced table |
| field `Code` Provider | TableName |
| header `Code` Provider | ModuleName |
| `Code` Pattern | `XXXXXX` |

## Lookup Syntax

```sql
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup
CustomerId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer
PersonId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer TableName:Person
TradeTypeId int @NOT_NULL, -- Lookup TradeType EnumName:TradeType
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
```

- `LOOKUP_NAME` is the name of the `LookupDef` in `DataRegistry.Lookups`.
- If `LOOKUP_NAME` is omitted, it is resolved from the FK referenced table or from the field name without `Id`.
- If `TableName:`, `EnumName:`, or `ClassName:` is used, `LOOKUP_NAME` is required.
- `TableName:` uses `DataRegistry.AddOrUpdateLookupWithTableName()`.
- `EnumName:` uses `DataRegistry.AddOrUpdateLookupSource()`.
- `ClassName:` uses `DataRegistry.AddOrUpdateLookupWithClassName()`.

## Locator Syntax

```sql
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product
ProductId @NVARCHAR(40) @NOT_NULL, -- Locator Product ClassName:ProductLocator
CustomerId @NVARCHAR(40) @NOT_NULL, -- Locator Customer Form:Person WebForm:Person
PaymentId @NVARCHAR(40) @NOT_NULL, -- Locator Payment ClassName:PaymentLocator Form:CustomerReceipt WebForm:CustomerReceipt
```

- `LOCATOR_NAME` is the name of the locator definition in `DataRegistry.Locators`.
- If `LOCATOR_NAME` is omitted, it is resolved from the FK referenced table.
- If `ClassName:`, `Form:`, or `WebForm:` is used, `LOCATOR_NAME` is required.
- `ClassName:` is passed to `DataRegistry.AddOrUpdateLocator()`.
- `Form:` is the desktop reference form name used by locator reference menus.
- `WebForm:` is the web reference form name used by web locator reference menus.
- For FK-backed generated locators, if `Form:` is omitted, the builder uses the referenced table form.
- For FK-backed generated locators, if `WebForm:` is omitted, the builder uses the referenced table web form, falling back to `Form:`.
- At runtime, `LocatorDef.Form` falls back to the locator name and `LocatorDef.WebForm` falls back to `Form`.
- For table-backed locators where the locator name is the referenced module/form name, the defaults are usually enough.
- For custom SQL, service, or cross-module locators, prefer explicit `Form:` and `WebForm:`.

Example generated registration:

```csharp
DataRegistry.AddOrUpdateLocator("Product", Source: "Product", KeyField: "Id", ClassName: "ProductLocator", FormName: "Product", WebFormName: "Product");
```

## Join Aliases and Locator Fields

Default join field alias:

```text
JOIN_ALIAS__FIELD_NAME
```

Example:

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

Declare locator fields using the original joined-table field names:

```csharp
LocatorDef.Add("Id");
LocatorDef.Add("Code");
LocatorDef.Add("Name");
```

Preferred custom SELECT:

```sql
select
     P.Id as Id
    ,P.Code as Code
    ,P.Name as Name
from Product P
```

The UI matches:

```text
Locator Code -> Product.Code -> Product__Code
Locator Name -> Product.Name -> Product__Name
```

When snapshot fields exist:

```text
Locator Code -> Product.Code -> ProductCode
Locator Name -> Product.Name -> ProductName
```

Do not use `ProductCode` or `ProductName` as `LocatorFieldDef.Name` unless those are actual join field names. If the SELECT returns those column names, declare locator fields with `Name = "Code"` / `Name = "Name"` and explicit aliases `ProductCode` / `ProductName`.

## SQL Type Tokens

RDBMS-neutral tokens replaced at `CREATE TABLE` time:

| Token | Meaning |
| --- | --- |
| `@NVARCHAR(n)` | Unicode string |
| `@VARCHAR(n)` | ASCII string |
| `@DECIMAL` | Decimal number |
| `@DECIMAL_(p,s)` | Decimal with precision/scale |
| `@FLOAT` | Float |
| `@DATE` | Date |
| `@DATE_TIME` | DateTime |
| `@BOOL` | Boolean |
| `@BLOB` | Binary blob |
| `@BLOB_TEXT` | ASCII text blob |
| `@NBLOB_TEXT` | Unicode text blob |
| `@NOT_NULL` | NOT NULL constraint |
| `@NULL` | NULL constraint |
| `@AUTO_INC` | Auto-increment PK |

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

Module: MODULE_NAME [MODULE_CLASS_NAME]
Group:  GROUP_NAME
Form:   DataForm | FORM_NAME [FORM_CLASS_NAME]
ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME
Code:   Code [Draft] [Pattern] [ProviderName]
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
- The following `Group`, optional `Form`, and optional `ItemPage` belong to that module block.
- A module block is complete when the next `Module` line starts or when non-module header metadata begins.
- Module block order is `Module`, `Group`, `Form`, `ItemPage`, `Code`.
- `Group` is required for each module block.
- If `Form` is omitted, the form name defaults to the module name and the form class defaults to `DataForm`.
- If `ItemPage` is omitted, the item page class defaults to `ItemPage`.
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

## Boolean Flags

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
A metadata entry enclosed in square brackets is parsed as comma-separated `FieldFlags`.

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup -- default currency
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup DocumentModule ClassName:DocumentModuleLookupSource
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General; [ReadOnlyUI, ReadOnlyEdit] -- customer code
Code @NVARCHAR(40) @NOT_NULL, -- Code Draft SO-YYYY-XXXXXX SALES_ORDER
TradeStatusId int default 0 @NOT_NULL, -- Enum TradeStatus; [ReadOnlyUI]
Notes @BLOB_TEXT, -- Memo; Group Notes -- short notes
Remarks @BLOB_TEXT, -- LargeMemo; Group Notes -- long notes
Photo @BLOB, -- [Image] -- product photo
```

## Metadata Keywords

| Keyword      | Syntax                                                                                                    | Meaning                                             |
| ------------ | --------------------------------------------------------------------------------------------------------- | --------------------------------------------------- |
| `Master`     | `Master` / `Master OneToOne`                                                                              | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup`     | `Lookup [LOOKUP_NAME] [TableName:TABLE_NAME \| EnumName:ENUM_NAME \| ClassName:LOOKUP_SOURCE_CLASS_NAME]` | Small in-memory reference selector                  |
| `Enum`       | `Enum [EnumName]`                                                                                         | Enum-backed selector                                |
| `Locator`    | `Locator [LocatorName]`                                                                                   | Searchable large reference selector                 |
| `Code`       | `Code [Draft] [Pattern] [ProviderName]`                                                                   | Auto-generated code field                           |
| `Memo`       | `Memo`                                                                                                    | Text field with Memo flag                           |
| `LargeMemo`  | `LargeMemo`                                                                                               | Text blob with LargeMemo flag                       |
| `Group`      | `Group GroupName`                                                                                         | Field UI group                                      |
| `FieldFlags` | `[Flag1, Flag2]`                                                                                          | Adds `FieldFlags` values to the field               |

- `Memo` and `LargeMemo` are mutually exclusive.
- `FieldFlags` names are parsed from the `FieldFlags` enum.
- Common `FieldFlags`: `Hidden`, `ReadOnly`, `ReadOnlyUI`, `ReadOnlyEdit`, `Required`, `Boolean`, `Memo`, `LargeMemo`, `Image`, `ImagePath`, `NoInsertUpdate`, `ForeignKey`, `Extra`, `Searchable`.
- Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.
- For `FieldFlags`, square brackets are part of the actual schema syntax.
- `Code Draft PATTERN PROVIDER_NAME` generates `DRAFT-PROVIDER_NAME` with `DRAFT-PATTERN` and also the normal provider.
- Multi-module document tables should use header `Code: Draft PATTERN PROVIDER_NAME`; snapshot document modules should not declare `-- Code`.
- Code provider patterns generate `SchemaVersion.AddStatementAfter()` inserts into `SYS_NUMBER_SERIES`.

## Name Resolution

| Keyword                | Default name                 |
| ---------------------- | ---------------------------- |
| `Lookup`               | FK referenced table          |
| `Enum`                 | field name minus `Id` suffix |
| `Locator`              | FK referenced table          |
| field `Code` Provider  | TableName                    |
| header `Code` Provider | ModuleName                   |
| `Code` Pattern         | `XXX-XXX`                    |

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

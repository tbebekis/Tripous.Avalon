# RegBuilder Cheat Sheet

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

## Required

- `Table` - always required

## Module Blocks

- `Table` must be the first metadata entry.
- A top table may declare one or more module blocks.
- Each `Module` line starts a new module block.
- The following `Group` and optional `Form` belong to that module block.
- A module block is complete when the next `Module` line starts or when non-module header metadata begins.
- Module block order is `Module`, `Group`, `Form`.
- `Group` is required for each module block.

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
Code @NVARCHAR(40) @NOT_NULL, -- Code CUS-XXXX; Group General; [ReadOnlyUI, ReadOnlyEdit] -- customer code
Notes @BLOB_TEXT, -- Memo; Group Notes -- short notes
Remarks @BLOB_TEXT, -- LargeMemo; Group Notes -- long notes
Photo @BLOB, -- [Image] -- product photo
```

## Metadata Keywords

| Keyword      | Syntax                          | Meaning                                             |
| ------------ | ------------------------------- | --------------------------------------------------- |
| `Master`     | `Master` / `Master OneToOne`    | FK to parent table. `OneToOne` = single-row detail. |
| `Lookup`     | `Lookup [SourceName]`           | Small in-memory reference selector                  |
| `Enum`       | `Enum [EnumName]`               | Enum-backed selector                                |
| `Locator`    | `Locator [LocatorName]`         | Searchable large reference selector                 |
| `Code`       | `Code [Pattern] [ProviderName]` | Auto-generated code field                           |
| `Memo`       | `Memo`                          | Text field with Memo flag                           |
| `LargeMemo`  | `LargeMemo`                     | Text blob with LargeMemo flag                       |
| `Group`      | `Group GroupName`               | Field UI group                                      |
| `FieldFlags` | `[Flag1, Flag2]`                | Adds `FieldFlags` values to the field               |

- `Memo` and `LargeMemo` are mutually exclusive.
- `FieldFlags` names are parsed from the `FieldFlags` enum.
- Common `FieldFlags`: `Hidden`, `ReadOnly`, `ReadOnlyUI`, `ReadOnlyEdit`, `Required`, `Boolean`, `Memo`, `LargeMemo`, `Image`, `ImagePath`, `NoInsertUpdate`, `ForeignKey`, `Extra`, `Searchable`.
- Square brackets in this specification mean optional arguments and are not part of the actual schema syntax.
- For `FieldFlags`, square brackets are part of the actual schema syntax.

## Name Resolution

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

# Code Providers

Code providers generate human-readable document or entity codes.

Typical examples are customer codes, product codes, sales order numbers and draft document numbers.

## Main Types

- `CodeProviderDef` is the registered definition.
- `CodeProviderEntry` is a runtime entry loaded from the number series table.
- `CodeProviderModule` is the data module for editing number series rows.
- `FieldDef.CodeProvider` connects a table field to a provider.
- `DataModule` assigns the next code during commit.

## Number Series Table

The actual counters are stored in the system number series table.

The default table name is `SYS_NUMBER_SERIES`, controlled by `DbConfig.SysNumberSeriesTableName`.

The important fields are:

- `Code`, the unique provider name.
- `Name`, the display name.
- `Pattern`, the generated code pattern.
- `ResetPeriodId`, the reset period.
- `NextNumber`, the next numeric value.
- `LastResetValue`, the last period marker used for reset.
- `IsActive`, whether the provider is enabled.

Example rows:

```text
Code                 Pattern
CUSTOMER             C-XXXXXX
Product              P-XXXXXX
DRAFT-SalesOrder     DRAFT-SO-YYYY-XXXXXX
SalesInvoice         SI-YYYY-XXXXXX
```

## Registering A Provider

Providers are registered in `DataRegistry.CodeProviders`.

```csharp
DataRegistry.AddOrUpdateCodeProvider("CUSTOMER");
```

RegBuilder can generate provider registrations from schema metadata. It can also add `SYS_NUMBER_SERIES` seed rows as schema `StatementsAfter`.

## Connecting A Field

A table field uses a provider through `FieldDef.CodeProvider`.

```csharp
Table.AddString(
    "Code",
    MaxLength: 40,
    Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI)
    .SetCodeProviderName("CUSTOMER");
```

When a `DataModule` initializes, it checks the top table `Code` field. If that field has a code provider name, the module loads the matching `CodeProviderDef`.

## Automatic Assignment

Code assignment happens automatically during commit.

The important rule is that Tripous assigns a code only when:

- the row is newly inserted;
- the table has a `Code` column;
- the current code value is empty;
- the field has a registered code provider.

The assignment runs inside the commit transaction, just before rows are posted. This keeps the generated code and the saved row in the same transaction.

## Locked Increment

`DataModule.GetNextCodeLocked()` reads the provider row with a provider-specific locked select.

It then:

- loads the row into `CodeProviderEntry`;
- checks whether the period must reset;
- updates `NextNumber` and `LastResetValue`;
- formats the final code.

This is important for multi-user applications because two users must not receive the same code.

## Pattern Syntax

`CodeProviderEntry.Format()` replaces date tokens and numeric `X` tokens.

Supported tokens:

- `YYYY`, four digit year.
- `YY`, two digit year.
- `MM`, month.
- `DD`, day.
- `WW`, ISO week.
- `Q`, quarter.
- `S`, semester.
- `X`, numeric digit.

Example:

```text
Pattern: SO-YYYY-XXXXXX
Number : 123
Result : SO-2026-000123
```

All `X` tokens participate in the numeric part, even when separators exist.

```text
Pattern: INV-XXX-XXX
Number : 123
Result : INV-000-123
```

## Reset Periods

`ResetPeriod` controls when numbering starts again from `1`.

- `None`
- `Year`
- `Semester`
- `Quarter`
- `Month`
- `Week`
- `Day`

The pattern must contain the date tokens required by the reset period.

- `Year` requires `YYYY` or `YY`.
- `Semester` requires year and `S`.
- `Quarter` requires year and `Q`.
- `Month` requires year and `MM`.
- `Week` requires year and `WW`.
- `Day` requires year, `MM` and `DD`.

## Draft Providers

Some workflows use separate draft providers, normally prefixed with `DRAFT-`.

Example:

```text
DRAFT-SalesOrder
DRAFT-PurchaseInvoice
DRAFT-StockTrade
```

This keeps temporary draft numbering separate from final official numbering.

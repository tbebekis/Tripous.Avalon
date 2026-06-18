# Generated Code Providers

Code providers generate sequential codes for fields such as document numbers, customer codes or product codes.

The Registration Builder discovers code provider metadata from field comments and module headers.

Generated code provider work appears in two places:

- `RegistryVersionN.CodeProviders.cs` registers provider names in `DataRegistry`.
- `SchemaVersionN.cs` seeds `SYS_NUMBER_SERIES` rows with provider patterns.

## Field Code

Field-level syntax:

```sql
Code @NVARCHAR(40) @NOT_NULL, -- Code SO-YYYY-XXXXXX SALES_ORDER
Code @NVARCHAR(40) @NOT_NULL, -- Code Draft SO-YYYY-XXXXXX SALES_ORDER
```

If the provider name is omitted, it defaults to the table name.

If the pattern is omitted, it defaults to `XXXXXX`.

The generated field registration stores the resolved provider name on the `FieldDef`.

```csharp
FieldDef.CodeProvider = "SALES_ORDER";
```

In generated module code this appears through chained field configuration.

```csharp
tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false).SetCodeProviderName("Asset");
```

## Header Code

Header-level syntax:

```sql
Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
Code: Draft SI-YYYY-XXXXXX
```

Header `Code` belongs to the current module block.

If the provider name is omitted, it defaults to the module name.

This is the preferred form for multi-module document tables.

For shared document tables, each module can declare its own code provider while the physical `Code` field remains one field on the shared table.

Example:

```sql
Module: SalesOrder SalesOrderDataModule
Code: Draft SO-YYYY-XXXXXX

Module: SalesInvoice SalesInvoiceDataModule
Code: Draft SINV-YYYY-XXXXXX
```

The field-level `-- Code` metadata is then used as fallback only for module blocks without header `Code:`.

## Draft Codes

`Draft` generates two providers:

- `DRAFT-PROVIDER_NAME`
- `PROVIDER_NAME`

For example:

```sql
Code: Draft SO-YYYY-XXXXXX SALES_ORDER
```

generates:

- `DRAFT-SALES_ORDER` with pattern `DRAFT-SO-YYYY-XXXXXX`
- `SALES_ORDER` with pattern `SO-YYYY-XXXXXX`

## Provider Patterns

Discovered providers are stored in `SchemaParserResult.CodeProviderPatterns`.

The dictionary key is the provider name.

The dictionary value is the code pattern.

The same provider name with a different pattern is a parsing error.

This prevents one provider name from being seeded with conflicting number formats.

## Registry Version Output

`RegistryVersionN.CodeProviders.cs` registers provider names.

Example:

```csharp
public override void RegisterCodeProviders()
{
    DataRegistry.AddOrUpdateCodeProvider("SalesInvoice");
    DataRegistry.AddOrUpdateCodeProvider("DRAFT-SalesInvoice");
}
```

The registry registration tells Tripous that the provider exists.

It does not seed the database number series row.

## Schema Version Output

Generated schema version files add `SYS_NUMBER_SERIES` seed statements through `SchemaVersion.AddStatementAfter()`.

The statements run after the version's `CREATE TABLE` statements.

They supply required fields and use `MemTable.GenId()` for generated ids.

Duplicate code values are expected to fail through the table unique constraint.

Example generated statement:

```sql
INSERT INTO SYS_NUMBER_SERIES
(Id, Code, Name, Pattern, ResetPeriodId, NextNumber, LastResetValue, IsActive)
VALUES
('{MemTable.GenId()}', 'SalesInvoice', 'SalesInvoice', 'SINV-YYYY-XXXXXX', 0, 1, NULL, 1)
```

Draft provider example:

```sql
INSERT INTO SYS_NUMBER_SERIES
(Id, Code, Name, Pattern, ResetPeriodId, NextNumber, LastResetValue, IsActive)
VALUES
('{MemTable.GenId()}', 'DRAFT-SalesInvoice', 'DRAFT-SalesInvoice', 'DRAFT-SINV-YYYY-XXXXXX', 0, 1, NULL, 1)
```

The Registration Builder uses `Version.AddStatementAfter(SqlText)` only for these generated number series seed rows.

It does not provide general metadata for arbitrary post-table SQL statements.

## Generated Field Assignment

The provider assigned to a generated field depends on context.

For simple tables:

- field `-- Code PATTERN PROVIDER_NAME` assigns `PROVIDER_NAME`
- field `-- Code Draft PATTERN PROVIDER_NAME` assigns `DRAFT-PROVIDER_NAME`

For module header code:

- `Code: PATTERN PROVIDER_NAME` assigns `PROVIDER_NAME`
- `Code: Draft PATTERN PROVIDER_NAME` assigns `DRAFT-PROVIDER_NAME`

This means inserted draft documents start with the draft provider.

Application behavior may later replace the draft code with the normal provider code when the document is finalized.

## Practical Rules

- Use field `-- Code` for simple one-module tables.
- Use header `Code:` for multi-module document tables.
- Use `Draft` when documents need temporary draft numbers.
- Do not declare `-- Code` on snapshot document modules.
- Keep provider names stable after data exists.
- Do not reuse the same provider name with different patterns.
- Do not edit generated code provider files manually.

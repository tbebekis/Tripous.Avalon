# Generated Schema Versions

Generated schema version files register database structure for a specific application schema version.

The Registration Builder creates one `SchemaVersionN.cs` file for each configured schema version.

For example:

```text
SchemaVersion1.cs
SchemaVersion2.cs
```

## Purpose

A generated schema version describes what database objects belong to a version.

It mainly registers:

- tables
- provider-neutral `CREATE TABLE` statements
- statements that run after table creation
- code provider seed rows

It does not execute the schema by itself.

It registers schema descriptors that are later executed by the schema execution system.

## Class Shape

A generated schema version derives from `SchemaVersionDef`.

Example:

```csharp
public partial class SchemaVersion2: SchemaVersionDef
{
    public override int VersionNumber { get; } = 2;
}
```

`VersionNumber` connects the generated class to the corresponding schema version.

During registration, `SchemaVersionDef.Register()` finds or creates the target `Schema` and `SchemaVersion`, then calls the generated internal registration code.

## Table Registration

For each generated table, the file contains a private registration method.

The method creates a provider-neutral SQL statement and adds it to the version.

Example:

```csharp
void RegisterTable_DocumentType()
{
    string TableName = "DocumentType";
    string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL
    )
";
    Version.AddTable(SqlText);
}
```

`Version.AddTable()` stores the SQL statement as a schema table item.

The actual SQL token translation happens later through the active `SqlProvider`.

## Provider-Neutral SQL

Generated schema SQL keeps Tripous provider-neutral tokens.

Examples:

- `@NVARCHAR(40)`
- `@DECIMAL`
- `@DATE`
- `@DATE_TIME`
- `@BOOL`
- `@NOT_NULL`
- `@NULL`

This lets the same generated schema version target the relational database engines supported by Tripous.

## Creation Order

The Registration Builder resolves dependencies between tables before generating schema code.

Foreign keys determine table dependency order.

The generated `RegisterInternal()` method calls table registration methods in creation order.

This matters because referenced tables must normally exist before tables that reference them.

`CreationOrder` may appear in rebuilt generated schema text, but it is not required in the input `Schema.sql` file.

## Statements After Table Creation

`SchemaVersion` supports statements that run after table creation.

The API is:

```csharp
Version.AddStatementAfter(SqlText);
```

When a developer writes schema registration manually, this API may be used for any statement that must run after table creation.

For generated schema versions, the Registration Builder uses it for generated code provider seed rows.

It is not a general metadata feature that creates arbitrary post-table statements from `Schema.sql`.

## Code Provider Seed Rows

When the schema metadata contains `Code` declarations, the builder generates seed statements for `SYS_NUMBER_SERIES`.

Those statements are added with:

```csharp
Version.AddStatementAfter(SqlText);
```

These statements are stored in `SchemaVersion.StatementsAfter`.

They are executed after the version's table creation statements.

Example metadata:

```sql
Code @NVARCHAR(40) @NOT_NULL, -- Code AST-XXXXXX Asset
```

Generated schema output inserts a number series row:

```sql
INSERT INTO SYS_NUMBER_SERIES
(Id, Code, Name, Pattern, ResetPeriodId, NextNumber, LastResetValue, IsActive)
VALUES
('{MemTable.GenId()}', 'Asset', 'Asset', 'AST-XXXXXX', 0, 1, NULL, 1)
```

Draft code providers generate a draft series and a normal series.

## Registration From The Application

The application decides which schema versions are registered.

TinyERP coordinates this in `Registry.cs`.

`Registry.cs` is handwritten application code.

Adding a new generated schema version to the list is the developer's responsibility.

```csharp
SchemaVersionList.AddRange([
    new SchemaVersion1(),
    new SchemaVersion2()
]);
```

Then it registers each version:

```csharp
foreach (SchemaVersionDef Version in SchemaVersionList)
    Version.Register();
```

Registration builds the in-memory schema descriptor list.

Schema execution is a separate step.

## Database Recreation

Changing a generated schema version can require database recreation during development.

Examples:

- adding fields
- removing fields
- changing field types
- changing constraints
- changing foreign keys

Generated schema versions describe the desired structure.

They do not automatically migrate existing application data.

Depending on the provider and connection settings, Tripous may be able to recreate the database.

Otherwise the developer must recreate the database or drop affected tables manually.

## Manual Edits

Generated schema version files are output.

Do not edit them manually.

When a generated schema version needs to change:

- change the `Schema.sql` file
- change metadata comments if needed
- run the Registration Builder again
- review the generated diff

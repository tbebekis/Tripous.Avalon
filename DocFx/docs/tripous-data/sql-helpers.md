# SQL Helpers

Tripous.Data contains several small SQL helper types.
They support the SQL-first model without hiding SQL behind an ORM.

These helpers are used by `SqlProvider`, `SqlStore`, descriptors, schema execution, filters, locators, and data modules.

## SqlHelper

`SqlHelper` contains general SQL formatting and parsing helpers.

It can extract table names from simple SQL text.

```csharp
string TableName = SqlHelper.ExtractTableName(@"
create table Customer (
    Id @NVARCHAR(40) @NOT_NULL
)");
```

It can also format field paths and aliases.

```csharp
string FieldPath = SqlHelper.FieldPath("Customer", "Name");
string Alias = SqlHelper.FieldAlias("Customer", "Name");
```

The result of `FieldPath()` is:

```text
Customer.Name
```

The result of `FieldAlias()` uses the Tripous field alias separator:

```text
Customer__Name
```

`SqlHelper` also contains date/time and value formatting helpers.

```csharp
string DateText = SqlHelper.DateToStr(DateTime.Today, Quoted: true);
string ValueText = SqlHelper.Format(123.45);
string IdText = SqlHelper.FormatId(Id);
```

Use these helpers for framework-generated SQL fragments.
For normal application SQL values, prefer parameters instead of string formatting.

## Mask Helpers

`NormalizeMask()` and related methods convert simple search text to SQL `LIKE` fragments.

```csharp
string WherePart = SqlHelper.NormalizeMask("cust*");
```

The result is a `LIKE` expression using `%`.

```sql
like 'cust%'
```

`IsMasked()` checks whether text contains `%`, `?`, or `*`.

```csharp
bool IsMasked = SqlHelper.IsMasked("cust*");
```

These helpers are useful in filter and locator code, where users may type search masks.

## SqlParamScanner

`SqlParamScanner` scans SQL text for Tripous parameters.
Tripous parameters use the global `:` prefix.

```csharp
List<SqlParamRef> Refs = SqlParamScanner.Scan(@"
select *
from Customer
where Code = :Code
  and IsActive = :IsActive");
```

The scanner ignores:

- String literals.
- Line comments.
- Block comments.
- PostgreSQL-style `::` casts.

This allows `SqlProvider` to find real parameter references before replacing them with provider-native parameter tokens.

## SqlParam And SqlParams

`SqlParam` represents one parameter.
`SqlParams` is a small collection of parameters.

```csharp
SqlParams Params = new();

Params
    .Add("Code", "CUST-001")
    .Add("IsActive", true);

MemTable Table = Store.Select(@"
select *
from Customer
where Code = :Code
  and IsActive = :IsActive",
    Params);
```

Most code does not need to create `SqlParams` manually.
`SqlProvider.CreateSqlParams()` can build parameters from argument lists, dictionaries, rows, arrays, or an existing `SqlParams` instance.

Use `SqlParams` directly when explicit parameter objects make the code clearer.

## SqlTypeTokens

`SqlTypeTokens` defines the RDBMS-neutral type placeholders used in schema SQL.

Common tokens include:

- `@PRIMARY_KEY`.
- `@AUTO_INC`.
- `@VARCHAR`.
- `@NVARCHAR`.
- `@DECIMAL`.
- `@DATE`.
- `@DATE_TIME`.
- `@BOOL`.
- `@BLOB`.
- `@NOT_NULL`.
- `@NULL`.

Example:

```sql
create table Customer (
    Id @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CreatedAt @DATE_TIME @NOT_NULL
)
```

`SqlProvider.ReplaceDataTypePlaceholders()` translates those tokens to native SQL for the target RDBMS.

This is one of the important pieces behind Tripous support for 6 RDBMS.

## SqlValueProviders

`SqlValueProviders` replaces known SQL variables and fills default row values.

SQL text may contain variables such as:

```sql
select *
from Customer
where CompanyId = :@CompanyId
```

The internal provider can replace values such as:

- `:@CompanyId`.
- `:@AppDate`.
- `:@SysDate`.
- `:@SysTime`.

It can also fill `DataRow` values for default keywords such as:

- `CompanyId`.
- `EmptyString`.
- `AppDate`.
- `SysDate`.
- `SysTime`.
- `NetUserName`.
- `Guid`.
- `DbServerTime`.

```csharp
SqlValueProviders.Process(Row, Store);
```

Applications can register custom value providers by implementing `ISqlValueProvider`.

```csharp
SqlValueProviders.Add(new MySqlValueProvider());
```

Use custom providers when application-specific keywords must be resolved outside the framework.

## SqlCache

`SqlCache` stores schema `DataTable` objects per connection and statement name.
It is used when Tripous needs the native schema of a SELECT statement.

```csharp
DataTable Schema = Store.GetNativeSchemaFromSelect(
    "CustomerList",
    "select Id, Name from Customer");
```

Internally, `SqlStore` checks `SqlCache` before querying the database again.

The statement name must be unique within the connection.
If two different SQL statements use the same statement name, schema caching can return the wrong schema.

```csharp
SqlCache.Clear();
```

Use `Clear()` when schema-related metadata should be rebuilt.

## TableSqls

`TableSqls` groups the SQL statements needed for table operations.

It contains statements such as:

- `SelectSql`.
- `DeleteSql`.
- `SelectByMasterIdSql`.
- `SelectRowSql`.
- `InsertRowSql`.
- `UpdateRowSql`.
- `DeleteRowSql`.

`MemTable` and posting code use these statements when loading and saving table rows.

```csharp
Table.Sqls.SelectRowSql = "select * from Customer where Id = :Id";
Table.Sqls.UpdateRowSql = "update Customer set Name = :Name where Id = :Id";
```

`DisplayLabels` can also define visible fields and title keys for dropdown-style displays.

## SqlStatementBuilder

`SqlStatementBuilder` can generate table SQL statements from database metadata.
It reads the native table schema through `SqlStore` and fills a `TableSqls` instance.

```csharp
SqlStatementBuilder.BuildSql(
    "CustomerModule",
    Table,
    Store,
    IsTopTable: true);
```

It generates SQL such as:

- Select all rows.
- Select one row by primary key.
- Insert row.
- Update row.
- Delete row.

This is useful when a `MemTable` needs standard row SQL based on the actual database schema.

## When To Use SQL Helpers

Use these helpers when writing infrastructure code around SQL:

- Build provider-neutral schema SQL.
- Scan parameters.
- Generate table statements.
- Format SQL fragments.
- Resolve Tripous SQL variables.
- Cache native SELECT schemas.
- Fill default row values.

For ordinary business queries, keep SQL explicit and use `SqlStore` with parameters.

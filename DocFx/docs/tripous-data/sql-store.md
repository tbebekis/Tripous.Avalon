# SqlStore And Transactions

`SqlStore` is the main runtime facade for executing SQL through a configured database connection.
It sits above `SqlProvider` and gives application code a simpler API for SELECT statements, executable SQL, scalar results, metadata, id generation, and transactions.

Most application code should use `SqlStore` instead of calling `SqlProvider` directly.

## Creating A Store

The default store uses the default connection.

```csharp
SqlStore Store = SqlStores.CreateDefaultSqlStore();
```

`Db.DefaultStore` exposes a lazy default store too.

```csharp
SqlStore Store = Db.DefaultStore;
```

For a named connection, create a store by connection name.

```csharp
SqlStore Store = SqlStores.CreateSqlStore("Reports");
```

Or by connection info.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetConnectionInfo("Reports");
SqlStore Store = SqlStores.CreateSqlStore(ConnectionInfo);
```

The store keeps both the `DbConnectionInfo` and the `SqlProvider`.

```csharp
SqlProvider Provider = Store.Provider;
DbConnectionInfo ConnectionInfo = Store.ConnectionInfo;
```

## Selecting Rows

`Select()` executes SQL and returns a `MemTable`.

```csharp
MemTable Table = Store.Select(@"
select
    Id,
    Code,
    Name
from Customer
where IsActive = :IsActive",
    true);
```

Tripous uses `:` as the global parameter prefix.
The provider converts it to the native prefix before execution.

## Loading An Existing MemTable

`SelectTo()` loads rows into an existing `MemTable`.

```csharp
MemTable Table = new();

int Count = Store.SelectTo(Table, @"
select
    Id,
    Name
from Customer");
```

Use this when the table object already exists and should be reused.

## Executing SQL

`ExecSql()` executes INSERT, UPDATE, DELETE, DDL, or other non-query SQL.

```csharp
int RowsAffected = Store.ExecSql(@"
update Customer
set Name = :Name
where Id = :Id",
    Name,
    Id);
```

It can also execute a list of SQL statements inside a single transaction.

```csharp
Store.ExecSql([
    "delete from CustomerTag",
    "delete from Customer"
]);
```

Use this only when the order is clear and all statements should succeed or fail together.

## Scalar Results

Use `SelectResults()` when the first row is needed.

```csharp
DataRow Row = Store.SelectResults(
    "select * from Customer where Id = :Id",
    Id);
```

Use `SelectResult()` when a single value is needed.

```csharp
object Name = Store.SelectResult(
    "select Name from Customer where Id = :Id",
    string.Empty,
    Id);
```

Use `IntegerResult()` for integer scalar queries.

```csharp
int Count = Store.IntegerResult(
    "select count(*) from Customer",
    0);
```

## Parameter Sources

Many `SqlStore` methods accept `params object[]`.
The parameters may be supplied as:

- A normal argument list.
- A `DataRow`.
- An `IDictionary`.
- An `IList` or array.
- A `SqlParams` instance.

Argument list:

```csharp
Store.ExecSql(
    "update Customer set Name = :Name where Id = :Id",
    Name,
    Id);
```

Dictionary:

```csharp
Dictionary<string, object> Params = new();

Params["Name"] = Name;
Params["Id"] = Id;

Store.ExecSql(
    "update Customer set Name = :Name where Id = :Id",
    Params);
```

Data row:

```csharp
Store.ExecSql(
    "update Customer set Name = :Name where Id = :Id",
    Row);
```

The SQL text is scanned for parameter names and the provider binds values accordingly.

## Transactions

Use `BeginTransactionContext()` when multiple operations must run in the same transaction.
`SqlTransactionContext` owns both the connection and the transaction.

```csharp
using SqlTransactionContext Context = Store.BeginTransactionContext();

try
{
    Store.ExecSql(
        Context.Transaction,
        "insert into Customer (Id, Name) values (:Id, :Name)",
        Id,
        Name);

    Store.ExecSql(
        Context.Transaction,
        "insert into CustomerLog (CustomerId, Message) values (:CustomerId, :Message)",
        Id,
        "Customer created");

    Context.Commit();
}
catch
{
    Context.Rollback();
    throw;
}
```

If a context is disposed while the transaction is still active, it attempts to roll back.
Still, explicit `Commit()` and `Rollback()` make the transaction boundary clear.

Do not keep only a raw `DbTransaction` without its owning connection.
Use `SqlTransactionContext` for new internal transaction code.

## Transaction Callbacks

For shorter transaction batches, `ExecSql(Action<DbTransaction>)` can run a callback inside a transaction.

```csharp
Store.ExecSql(Transaction =>
{
    Store.ExecSql(Transaction, Sql1, Params1);
    Store.ExecSql(Transaction, Sql2, Params2);
});
```

Use the explicit `SqlTransactionContext` form when the operation is complex or needs clearer error handling.

## Id Generation

`SqlStore` exposes provider-backed id helpers.

```csharp
int NextId = Store.NextId("Customer");
```

For generator-based databases, this delegates to provider generator logic.
For identity-based databases, `LastId()` can return the last inserted identity value inside a transaction.

```csharp
int LastId = Store.LastId(Transaction, "Customer");
```

Use these only when the table and provider strategy require numeric ids.
Many Tripous applications use GUID string ids instead, controlled by `DbConfig.GuidOids`.

## Metadata Helpers

`SqlStore` can read database metadata.

```csharp
List<string> TableNames = Store.GetTableNames();
List<string> FieldNames = Store.GetFieldNames("Customer");
bool Exists = Store.TableExists("Customer");
```

It can also get the native schema of a SELECT statement without loading data.

```csharp
DataTable Schema = Store.GetNativeSchemaFromSelect(
    "CustomerList",
    "select Id, Name from Customer");
```

Tripous uses this in descriptor and filter generation code.

## Creating Tables

`CreateTable()` executes a CREATE TABLE statement if the table does not already exist.
The SQL may contain Tripous data type placeholders.

```csharp
bool Created = Store.CreateTable(@"
create table Customer (
    Id @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL
)");
```

Before execution, the provider replaces data type placeholders with native SQL.
If the provider supports generators and the connection has `AutoCreateGenerators` enabled, a table generator may also be created.

## Dangerous Helpers

`ResetTable()` deletes all rows and resets the table generator or identity value where possible.

```csharp
Store.ResetTable("Customer");
```

Use it only in controlled setup, tests, or administrative workflows.
It is not a normal application operation.

## When To Use SqlStore

Use `SqlStore` when code needs to work with SQL through a configured connection.

- Run SELECT statements.
- Execute INSERT, UPDATE, DELETE, or DDL.
- Read scalar values.
- Run explicit transactions.
- Read database metadata.
- Create tables from provider-neutral SQL.
- Use provider-backed id helpers.

Use `SqlProvider` directly only for provider infrastructure.
Use `DataModule` when the operation belongs to a registered business module.

# SqlProvider And SqlProviders

`SqlProvider` is the database-specific execution and translation layer of Tripous.Data.
`SqlProviders` is the static registry that exposes one provider instance for each supported RDBMS.

Tripous.Data uses SQL directly, but different database engines still have different parameter prefixes, data types, identifier delimiters, generators, identity behavior, and database creation rules.
Those differences live in `SqlProvider` implementations.

## Supported Providers

`SqlProviders` exposes lazy singleton instances for the 6 supported RDBMS engines:

- `SqlProviders.Sqlite`.
- `SqlProviders.MsSql`.
- `SqlProviders.Firebird`.
- `SqlProviders.PostgreSql`.
- `SqlProviders.MySql`.
- `SqlProviders.Oracle`.

You normally get a provider from a connection definition.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
SqlProvider Provider = ConnectionInfo.GetSqlProvider();
```

Or directly by server type:

```csharp
SqlProvider Provider = SqlProviders.GetSqlProvider(DbServerType.PostgreSql);
```

## What A Provider Does

A `SqlProvider` handles the database-specific work behind a `SqlStore`.

It is responsible for:

- Creating connections and commands.
- Preparing SQL parameters.
- Translating Tripous global parameter prefixes to native prefixes.
- Replacing RDBMS-neutral data type tokens.
- Executing SELECT and non-SELECT statements.
- Handling transactions.
- Creating databases where supported.
- Reading schema metadata.
- Creating or resetting generators and identity values where supported.
- Formatting provider-specific SQL fragments.

Application code usually talks to `SqlStore`.
`SqlStore` delegates the provider-specific details to its `Provider`.

```csharp
SqlStore Store = Db.DefaultStore;

MemTable Table = Store.Select("select * from Customer where IsActive = :IsActive", true);
```

## Parameter Translation

Tripous SQL uses `:` as the global parameter prefix.

```sql
select * from Customer where IsActive = :IsActive
```

Before execution, the provider scans the SQL, creates `DbParameter` objects, and replaces the global prefix with its native prefix.

Examples:

- SQL Server: `@IsActive`.
- PostgreSQL: `@IsActive`.
- MySQL: `@IsActive`.
- Firebird: `@IsActive`.
- SQLite: `:IsActive`.
- Oracle: `:IsActive`.

This lets application SQL keep one parameter style while the provider adapts it for the target database engine.

## Parameter Sources

`SqlProvider.CreateSqlParams()` can build parameters from several source forms.

Plain argument list:

```csharp
Store.Select(
    "select * from Customer where Code = :Code and IsActive = :IsActive",
    "CUST-001",
    true);
```

Dictionary:

```csharp
Dictionary<string, object> Params = new();

Params["Code"] = "CUST-001";
Params["IsActive"] = true;

Store.Select(
    "select * from Customer where Code = :Code and IsActive = :IsActive",
    Params);
```

Data row:

```csharp
Store.ExecSql(
    "update Customer set Name = :Name where Id = :Id",
    Row);
```

The provider scans the SQL text and binds values by parameter name.

## Data Type Tokens

Schema code may use RDBMS-neutral type tokens.
`SqlProvider.ReplaceDataTypePlaceholders()` turns those tokens into native SQL.

```sql
Id @NVARCHAR(40) @NOT_NULL,
CreatedAt @DATE_TIME @NOT_NULL
```

The final SQL differs by database engine.
This is one of the mechanisms that lets Tripous keep one schema declaration model while supporting 6 RDBMS.

## Connection And Database Operations

Providers create and open native `DbConnection` instances.

```csharp
using DbConnection Connection = Provider.OpenConnection(ConnectionInfo);
```

They can also check whether a connection can be opened.

```csharp
bool CanConnect = Provider.CanConnect(ConnectionInfo.ConnectionString);
```

Some providers can create databases from a connection string.

```csharp
if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
{
    Provider.CreateDatabase(ConnectionString);
}
```

Creation support depends on the database engine and on the permissions of the user in the connection string.

## Capabilities

Provider capability properties let higher-level code adjust behavior.

```csharp
bool SupportsTransactions = Provider.SupportsTransactions;
bool CanCreateDatabases = Provider.CanCreateDatabases;
bool SupportsGenerators = Provider.SupportsGenerators;
bool SupportsAutoIncFields = Provider.SupportsAutoIncFields;
```

Examples:

- Oracle supports generators and does not support auto-increment fields in the same way as SQL Server or MySQL.
- Firebird and PostgreSQL support generators or sequences.
- SQLite and SQL Server support database creation from the provider implementation.

Schema and store code use these flags when creating tables, generators, and identity behavior.

## Identifier Delimiters

Each provider defines object name delimiters.

```csharp
string Quoted = Provider.QuoteName("Customer");
```

Examples:

- SQL Server uses `[Customer]`.
- MySQL uses `` `Customer` ``.
- Most other providers use `"Customer"`.

Use provider helpers when infrastructure code must generate SQL object names.

## Connection String Adapter

Each provider has a `ConnectionStringAdapter`.
Desktop connection editing uses adapters to parse, display, and rebuild connection strings in a provider-aware way.

```csharp
DbConAdapter Adapter = Provider.ConnectionStringAdapter;
```

This keeps connection editing separate from SQL execution.

## When To Use SqlProvider Directly

Most application code should use `SqlStore`.
Use `SqlProvider` directly when code needs provider-specific infrastructure behavior.

- Create or verify a database.
- Read provider capabilities.
- Translate data type tokens.
- Inspect native metadata.
- Build provider-specific SQL fragments.
- Work with connection string adapters.

For normal SELECT, INSERT, UPDATE, DELETE, and transaction work, prefer `SqlStore`.

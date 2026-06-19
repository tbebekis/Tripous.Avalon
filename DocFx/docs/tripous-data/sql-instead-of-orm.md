# SQL Instead of ORM

Tripous.Data is SQL-centric.
It does not try to hide relational databases behind an object graph.

Instead, it keeps SQL visible and combines it with descriptors, metadata, stores, modules, and provider-specific translation.

This is a deliberate design choice.
Tripous does not avoid ORM because ORM ideas are unknown.
It chooses SQL because relational databases already have a powerful, explicit, and widely understood language.

## The Basic Trade-Off

ORM frameworks usually start from classes and object graphs.
They then generate or translate database access behind those objects.

Tripous starts from relational structures and SQL.
It then adds framework services around them:

- Schema declarations.
- SQL providers for different RDBMS dialects.
- Descriptor metadata.
- Data modules.
- Lookup and locator definitions.
- Generated or manual registration.
- `MemTable` row buffers.
- Transaction-aware stores.

The goal is not to pretend the database is not there.
The goal is to make database work predictable and productive.

## SQL Is The Contract

In Tripous.Data, SQL statements are first-class values.
They appear in module definitions, select definitions, locators, lookups, schema scripts, and data modules.

```csharp
string SqlText = @"
select
    Customer.Id,
    Customer.Code,
    Customer.Name
from Customer
order by Customer.Name";
```

A module can use that SQL as its list select.

```csharp
ModuleDef Module = DataRegistry.AddModule(
    "Customer",
    TitleKey: "Customers",
    ClassName: typeof(CustomerDataModule).FullName,
    ListSelectSql: SqlText);
```

The SQL is not hidden in an expression tree or generated at runtime from object navigation.
The developer can read it, test it, tune it, and reason about it.

## Global Parameter Prefix

Tripous SQL uses `:` as the global parameter prefix.
Application SQL can therefore use the same parameter syntax regardless of the target RDBMS.

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

At execution time, the active `SqlProvider` scans the SQL text, creates the parameter list, and replaces the global prefix with the native parameter syntax of the provider.

Examples:

- SQL Server uses `@IsActive`.
- PostgreSQL uses `@IsActive`.
- MySQL uses `@IsActive`.
- Firebird uses `@IsActive`.
- SQLite uses `:IsActive`.
- Oracle uses `:IsActive`.

This is separate from Tripous SQL variables.
Variables use `DbConfig.VariablesPrefix`, whose default value is `:@`, for values such as `:@CompanyId` or `:@AppDate`.

In practice:

- Use `:Name` for SQL parameters passed to `SqlStore` and `SqlProvider`.
- Use `:@Name` for Tripous variables that are replaced before execution.

## SqlStore Executes SQL

`SqlStore` is the main runtime object for executing SQL through a configured connection.

```csharp
SqlStore Store = Db.DefaultStore;

MemTable Table = Store.Select(@"
select
    Id,
    Code,
    Name
from Customer
where IsActive = :IsActive",
    true);
```

For commands that change data, use `ExecSql()`.

```csharp
Store.ExecSql(@"
update Customer
set Name = :Name
where Id = :Id",
    Name,
    Id);
```

The store delegates database-specific work to the active `SqlProvider`.

## SqlProvider Handles Dialects

Tripous supports 6 RDBMS:

- SQLite.
- SQL Server.
- Firebird.
- PostgreSQL.
- MySQL.
- Oracle.

The `SqlProvider` hierarchy is where provider-specific behavior lives.
This includes connection handling, SQL execution, schema operations, parameter behavior, data type translation, and other dialect differences.

That means most application code can stay SQL-centric while still having a provider abstraction below it.

## RDBMS-Neutral Schema Tokens

Schema scripts and generated schema versions may use Tripous tokens for types and default values.
Those tokens are translated later by the active provider.

```sql
Id @NVARCHAR(40) @NOT_NULL,
CreatedAt @DATE_TIME @NOT_NULL
```

The reason this exists is exactly the 6-RDBMS support.
Tripous keeps the declaration readable and neutral, then lets the provider produce the final database-specific SQL.

The application model remains one model.
The final SQL can still be different per database engine.

## MemTable Instead Of Entity Graphs

Tripous uses `MemTable` as an in-memory row buffer.
It is closer to a data table than to an ORM entity.

```csharp
MemTable Table = Store.Select("select * from Customer");

foreach (DataRow Row in Table.Rows)
{
    string Name = Row.AsString("Name");
}
```

This fits the SQL-first model:

- SQL selects rows.
- `MemTable` holds the result.
- Data modules apply behavior.
- Stores post changes with explicit SQL and transactions.

The framework does not require a class per table row.

## DataModule Adds Behavior

`DataModule` is where runtime behavior belongs.
The module descriptor describes the metadata.
The data module implements behavior around loading, editing, validation, saving, and business rules.

```csharp
/// <summary>
/// Provides data behavior for customers.
/// </summary>
public class CustomerDataModule : DataModule
{
    // ● public
    /// <summary>
    /// Executes custom logic before commit.
    /// </summary>
    public override void Commit()
    {
        base.Commit();
    }
}
```

This keeps SQL and data behavior close to the database workflow.
It also avoids putting database behavior into entity objects.

## Descriptors Provide Metadata

Descriptors explain what the framework should do with the SQL and tables.

```csharp
ModuleDef Module = DataRegistry.AddModule(
    "Customer",
    TitleKey: "Customers",
    ClassName: typeof(CustomerDataModule).FullName,
    ListSelectSql: SqlText);
```

A `ModuleDef` declares the module.
A `TableDef` declares fields and relations.
A `SelectDef` declares list SQL and filters.
A `LookupDef` declares lookup data.
A `LocatorDef` declares search behavior.

The SQL remains explicit, but the framework gains enough metadata to build UI, filters, lookups, locators, and data workflows.

## Why This Works Well For Tripous

Tripous applications often need:

- Clear database visibility.
- Multi-table forms.
- Generated registration from schema files.
- Manual extension after generation.
- Support for multiple relational database engines.
- Practical desktop CRUD workflows.
- Predictable SQL and transactions.

Those requirements fit SQL and metadata well.

An ORM can be very productive when the object model is the center of the application.
Tripous is different.
In Tripous, the database model, descriptors, SQL, and data modules are all first-class parts of the application model.

## What Tripous Does Not Do

Tripous.Data does not try to provide:

- Lazy-loaded entity graphs.
- Change tracking through entity classes.
- LINQ-to-database query translation.
- Navigation properties as the main data access model.
- Database independence by hiding SQL completely.

Instead, it provides:

- Explicit SQL.
- Provider abstraction.
- Metadata-driven registration.
- `MemTable` row buffers.
- Data modules.
- Transaction-aware stores.
- Schema and descriptor infrastructure.

## Practical Guideline

Write SQL when the database is the clearest expression of the operation.
Use descriptors to tell Tripous how that SQL participates in the application.
Use data modules for behavior.
Use providers and neutral tokens where the same declaration must work across different RDBMS engines.

That is the Tripous.Data approach:

- SQL stays visible.
- Metadata gives it structure.
- Providers handle database differences.
- Data modules provide behavior.

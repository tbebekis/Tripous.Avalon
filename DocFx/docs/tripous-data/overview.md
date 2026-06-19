# Overview

Tripous.Data is the database and metadata layer of Tripous.
It sits above Tripous Core and below Tripous.Desktop.

The layer provides database-neutral access to six RDBMS, metadata descriptors for modules and tables, SQL helpers, in-memory tables, configuration storage, schema creation, lookups, locators and data modules.

## Main Entry Points

- `Db`
- `DbConfig`
- `DbGlobalSettings`
- `SqlProvider`
- `SqlStore`
- `MemTable`
- `TableSet`
- `DataModule`
- `DataRegistry`
- `ModuleDef`
- `TableDef`
- `FieldDef`
- `SelectDef`

## Database Support

Tripous.Data is designed to work across six RDBMS:

- Firebird
- SQL Server
- MySQL
- PostgreSQL
- SQLite
- Oracle

SQL declaration code uses Tripous-neutral tokens and parameter names.
At execution time, providers translate the global Tripous parameter prefix, normally `:`, to the native prefix of the target database.

```sql
select *
from Customer
where Name like :Name
```

This lets framework and application code keep one database-neutral shape while still executing against different engines.

## Db And Providers

`Db`, `DbConfig` and `DbGlobalSettings` form the central data entry point.
They hold provider registrations, connection definitions and global data settings.

`SqlProvider` represents the RDBMS-specific behavior.
`SqlStore` represents a database connection context used by data modules, schema operations and SQL execution.

```csharp
SqlStore Store = SqlStores.CreateSqlStore("Main");
DataTable Table = Store.Select("select * from Customer");
```

## SQL Helpers

Tripous.Data contains helpers for parsing and producing SQL.

Important pieces include:

- `SelectSql`, for structured SELECT statements.
- `SelectSqlParser`, for parsing SELECT text.
- `SqlFilterDef` and `SqlFilterDefs`, for structured runtime filters.
- `SqlFilterExpressionDef`, for inline filter tags embedded in SQL text.
- RDBMS-specific metadata readers for schema information.

The goal is not to hide SQL.
The goal is to keep SQL explicit while providing enough structure for generated modules, filters and UI.

## Tables And Row Providers

`MemTable` wraps `DataTable` and `DataView` behavior used by Tripous modules and desktop forms.
`TableSet` groups related tables and supports master-detail data.

```csharp
MemTable Customers = new MemTable("Customer");
Customers.DataTable.Columns.Add("Id", typeof(int));
Customers.DataTable.Columns.Add("Name", typeof(string));
```

These types are used by `DataModule` and by the Desktop binding layer.

## Data Modules

`DataModule` is the data workflow object behind a form or service operation.
It loads list and item data, tracks changes, saves tables, and coordinates transactions.

A data module must stay UI-neutral.
It must not show dialogs, message boxes or any UI that waits for user interaction.

## Registry And Descriptors

`DataRegistry` stores metadata descriptors.
Descriptors declare modules, tables, fields, select statements, lookups, locators and code providers.

```csharp
ModuleDef Module = DataRegistry.AddModule("Customer");
TableDef Table = Module.AddTable("Customer");
Table.AddString("Name", 96);
```

The same metadata can drive:

- SQL execution.
- generated data modules.
- schema creation.
- generated desktop forms.
- lookup and locator behavior.

## Lookups And Locators

Lookups are used for small reference lists.
Locators are used for searchable reference tables.

Both are declared in the data layer and consumed by higher layers.
Desktop controls use the same descriptors to display lookup combo boxes and locator search boxes.

## Schema Creation

Tripous.Data can create and update database schema objects from descriptor metadata and schema scripts.
Schema execution uses transactions where the target RDBMS supports them.

The schema system is also the target of RegBuilder output.
Generated registry versions and manual descriptor code both feed the same runtime metadata model.

## Configuration Storage

The configuration system stores application and user settings in database tables.
`DbIni` provides database-backed key-value storage and supports transactional writes when needed.

## How To Read This Section

Start with these articles:

- `Db, DbConfig and DbGlobalSettings`.
- `Connections`.
- `SqlProvider and SqlProviders`.
- `SqlStore and Transactions`.
- `MemTable and Row Providers`.
- `DataModule`.
- `DataRegistry`.
- `Descriptors`.

Then continue with filters, lookups, locators, schema creation and supporting helpers.

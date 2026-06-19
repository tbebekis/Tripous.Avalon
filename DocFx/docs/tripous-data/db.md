# Db

`Db`, `DbConfig`, and `DbGlobalSettings` are the entry point of the Tripous.Data layer.
They play the same role for data services that `Sys`, `SysConfig`, and `SysGlobalSettings` play for Tripous Core.

- `Db` is the central facade.
- `DbConfig` contains static data-layer configuration.
- `DbGlobalSettings` contains JSON-backed runtime settings exposed as `Db.Settings`.

## Db

`Db` is the central static class of Tripous.Data.
It registers database provider factories, exposes the configured connections, returns the default `SqlStore`, and provides a few data conversion helpers.

The static constructor registers provider factories for the supported RDBMS providers:

- SQLite.
- SQL Server.
- Firebird.
- PostgreSQL.
- MySQL.
- Oracle.

This is part of how Tripous.Data supports the same declaration and data-access model across 6 different RDBMS.

## Startup Flow

A typical application startup configures Core first, then loads data connections, creates or updates the database, and finally registers descriptors.

```csharp
InitializeConfigs();

await LoadConnectionStrings();
await CreateDatabases();

Registry.RegisterSchemas();
Schemas.Execute();

Store = SqlStores.CreateDefaultSqlStore();

TypeStore.RegisterLoadedAssemblies();
Registry.RegisterDescriptors();
```

The exact startup code differs per application, but the order matters:

- Configuration and connection strings must exist before schema execution.
- Schemas must be registered before `Schemas.Execute()`.
- Descriptors are usually registered after the database layer is ready.

## Connections

`Db.Connections` stores the registered database connections.
Applications usually load them from the settings file.

```csharp
Db.LoadConnections();
```

The default connection is selected by `DbConfig.DefaultConnectionName`.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
```

Use `GetConnectionInfo()` when a specific named connection is needed.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetConnectionInfo("Reports");
```

Most descriptors default to `DbConfig.DefaultConnectionName` when no connection name is explicitly assigned.

## Default Store

`Db.DefaultStore` returns the default `SqlStore`.
It is created on first access through `SqlStores.CreateDefaultSqlStore()`.

```csharp
SqlStore Store = Db.DefaultStore;
MemTable Table = Store.Select("select * from Customer");
```

Sample application code often uses `Db.DefaultStore` in application-level services and resolvers.

```csharp
readonly SqlStore fStore = Db.DefaultStore;
```

For code that needs a different connection, create or inject the appropriate `SqlStore` instead of using the default one.

## DbIni

`Db.MainIni` returns the main `DbIni` instance, created from the default connection.
`DbIni` stores small database-backed values in the system initialization table.

```csharp
DbIni Ini = Db.MainIni;
```

The table and field names are controlled by `DbConfig`:

- `SysDbIniTableName`.
- `SysDbIniEntryField`.
- `SysDbIniValueField`.
- `SysDbIniBlobField`.

## DbConfig

`DbConfig` contains static configuration for the data layer.
It affects schema generation, SQL helper behavior, default connection selection, system table names, and company-aware values.

Object identifiers are controlled by `GuidOids`.

```csharp
DbConfig.GuidOids = true;

DataFieldType IdType = DbConfig.OidDataType;
int IdSize = DbConfig.OidSize;
```

When `GuidOids` is true, standard object identifiers are string GUID values.
When false, integer identifiers are used.

The default connection is controlled by `DefaultConnectionName`.

```csharp
DbConfig.DefaultConnectionName = Sys.DEFAULT;
```

System table names can also be customized before descriptors and schemas are built.

```csharp
DbConfig.SysAppUserTableName = "SYS_APP_USER";
DbConfig.SysConfigTableName = "SYS_CONFIG";
DbConfig.SysNumberSeriesTableName = "SYS_NUMBER_SERIES";
```

Change these values only during application initialization.
Changing them after descriptors, schemas, or stores are already in use can create inconsistent behavior.

## Company Context

Tripous.Data has built-in support for company-aware data.
The company field name and current company id are stored in `DbConfig`.

```csharp
DbConfig.CompanyFieldName = "CompanyId";
DbConfig.CompanyId = Sys.StandardCompanyGuid;
```

SQL value providers use these settings when replacing company tokens or filling default values.

```csharp
string CompanyIdSql = DbConfig.CompanyIdSql;
object CompanyId = DbConfig.CompanyIdValue;
```

This keeps company filtering and default value generation consistent across schema, registry, and data module code.

## Db.Settings

`Db.Settings` is an instance of `DbGlobalSettings`.
It derives from `SettingsBase`, so it follows the same JSON-backed settings pattern used by Core.

Important settings include:

- `DefaultRowLimit`.
- `DefaultCommandTimeoutSeconds`.
- `IdFieldsVisible`.
- `LocatorMinimumSearchTextLength`.
- `LocatorMaximumDropDownRows`.
- `LogSqlStatements`.

Example:

```csharp
Db.Settings.DefaultRowLimit = 500;
Db.Settings.DefaultCommandTimeoutSeconds = 300;
Db.Settings.LogSqlStatements = true;
```

Providers and stores use these settings when executing SQL.
For example, command timeout and default row limits flow from `Db.Settings`.

Locator code also uses `Db.Settings` to control minimum search length and maximum result rows.

```csharp
int MinLength = Db.Settings.LocatorMinimumSearchTextLength;
int MaxRows = Db.Settings.LocatorMaximumDropDownRows;
```

Applications often expose `LogSqlStatements` as a toolbar or menu toggle.

```csharp
Db.Settings.LogSqlStatements = !Db.Settings.LogSqlStatements;
```

## DataSet And DataTable Conversion

`Db` also contains helpers for serializing `DataTable` and `DataSet` instances to Base64 text and restoring them later.

```csharp
string Text = Db.TableToToBase64(Table);
DataTable Restored = Db.Base64ToTable(Text);
```

```csharp
string Text = Db.DataSetToToBase64(DataSet);
DataSet Restored = Db.Base64ToDataSet(Text);
```

These helpers use XML with schema information internally.
They are useful for small framework payloads, not for large data export workflows.

## When To Use Each One

Use `Db` when code needs the current data-layer services:

- Load or retrieve connections.
- Get the default connection info.
- Access the default store.
- Access the main `DbIni`.
- Convert small table or dataset payloads.

Use `DbConfig` during application initialization to define data-layer conventions:

- Default connection name.
- Identifier strategy.
- Company field and company id.
- System table names.
- SQL variable prefix.

Use `Db.Settings` for user or runtime settings:

- SQL logging.
- Command timeout.
- Browser row limit.
- Locator limits.
- Id field visibility.

The important distinction is timing.
`DbConfig` defines conventions before the system is built.
`Db.Settings` controls runtime behavior after the system is running.

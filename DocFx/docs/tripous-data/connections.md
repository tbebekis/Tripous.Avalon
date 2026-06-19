# Connections

Tripous.Data stores database connection definitions as `DbConnectionInfo` objects.
The global collection is available through `Db.Connections`.

Connection definitions are persisted in `DbConnections.json`, under the application folder managed by `SettingsBase`.

## DbConnectionInfo

`DbConnectionInfo` describes one database connection.

```csharp
DbConnectionInfo ConnectionInfo = new();

ConnectionInfo.Name = Sys.DEFAULT;
ConnectionInfo.DbServerType = DbServerType.Sqlite;
ConnectionInfo.ConnectionString = "Data Source=\"[Data]/todo.db3\"";
```

The most important properties are:

- `Name`, unique among all registered connections.
- `DbServerType`, the target RDBMS.
- `ConnectionString`, the provider connection string.
- `CommandTimeoutSeconds`, the command timeout for this connection.
- `AutoCreateGenerators`, used by providers that support generators or sequences.

Each connection also owns a `DbSchema` instance through its `Schema` property.
That schema object is used by metadata and schema inspection code.

## Supported Server Types

`DbServerType` identifies the database engine.

Tripous.Data supports 6 RDBMS:

- SQLite.
- SQL Server.
- Firebird.
- PostgreSQL.
- MySQL.
- Oracle.

The selected server type determines which `SqlProvider` is used.

```csharp
SqlProvider Provider = ConnectionInfo.GetSqlProvider();
```

## Connection Templates

`DbServerType` provides template connection strings.
Applications can use these templates when creating a default connection.

```csharp
DbConnectionInfo Result = new();

Result.Name = Sys.DEFAULT;
Result.DbServerType = DbServerType.Sqlite;
Result.ConnectionString = string.Format(
    DbServerType.Sqlite.GetTemplateConnectionString(),
    "[Data]/todo.db3");
```

For SQLite, the template is:

```text
Data Source="{0}"
```

For server databases, the template includes server, database, user, and password placeholders depending on the provider.

## DbConnections

`DbConnections` is the persisted collection of `DbConnectionInfo` objects.
It derives from `SettingsBase` and uses `DbConnections.json` as its file name.

```csharp
Db.Connections.Load();
```

If the file does not exist yet, sample applications create a default SQLite connection.

```csharp
if (Db.Connections.List.Count == 0)
{
    DbConnectionInfo ConnectionInfo = CreateDefaultConnectionInfo();
    Db.Connections.Add(ConnectionInfo);
    Db.Connections.Save();
}
```

`Add()` saves the collection after adding a new connection.
`Remove()` also saves after removing one.

## Default Connection

Most applications use a default connection.
The default connection name is stored in `DbConfig.DefaultConnectionName`.

```csharp
DbConfig.DefaultConnectionName = Sys.DEFAULT;
```

Use `Db.GetDefaultConnectionInfo()` to retrieve it.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
```

Many descriptors fall back to the default connection when their own connection name is empty.
This keeps small applications simple while still allowing multi-connection applications.

## Named Connections

Use `Db.GetConnectionInfo()` when a specific connection is required.

```csharp
DbConnectionInfo ReportsConnection = Db.GetConnectionInfo("Reports");
```

Use `Find()` when a missing connection is acceptable.

```csharp
DbConnectionInfo ConnectionInfo = Db.Connections.Find("Archive");

if (ConnectionInfo != null)
{
    SqlStore Store = SqlStores.CreateSqlStore(ConnectionInfo);
}
```

Use `Contains()` when only existence matters.

```csharp
bool Exists = Db.Connections.Contains("Reports");
```

## Path Placeholders

Connection strings may contain path placeholders.
`ConnectionStringBuilder.ReplacePathPlaceholders()` expands them to physical paths.

Supported placeholders are:

- `[AppPath]`, the application folder.
- `[Data]`, the application data folder.
- `[BackUp]`, the backup folder under the application data folder.

Example:

```csharp
string ConnectionString = "Data Source=\"[Data]/todo.db3\"";
string PhysicalConnectionString = ConnectionStringBuilder.ReplacePathPlaceholders(ConnectionString);
```

Providers normalize connection strings before opening connections, so placeholders are useful in persisted connection definitions.

## Creating The Database

A common startup pattern is to load connections, get the default connection, and create the database if it does not exist.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
SqlProvider Provider = ConnectionInfo.GetSqlProvider();
string ConnectionString = ConnectionInfo.ConnectionString;

if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
{
    Provider.CreateDatabase(ConnectionString);
}
```

This is used by the sample applications to create a first SQLite database automatically.

For server databases, creation support depends on the provider and permissions of the connection user.

## Command Timeout

`DbConnectionInfo.CommandTimeoutSeconds` controls command timeout for that connection.
If the value is lower than `Db.Settings.DefaultCommandTimeoutSeconds`, the global setting is used.

```csharp
ConnectionInfo.CommandTimeoutSeconds = 600;
```

This allows long-running connections to override the default while keeping normal connections on the global setting.

## Connection Editing

Desktop applications can expose the default connection through the connection edit dialog.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
await DbConnectionEditDialog.ShowModal(ConnectionInfo, Ui.MainWindow);
```

The dialog uses provider-specific connection string adapters to parse and rebuild connection strings.

## When To Use Multiple Connections

Use multiple named connections when different parts of the application target different databases.

- Main application data.
- Reporting database.
- Archive database.
- External integration database.

For small applications, one default connection is usually enough.

The important rule is that descriptors and schemas must reference a connection name that exists in `Db.Connections`.

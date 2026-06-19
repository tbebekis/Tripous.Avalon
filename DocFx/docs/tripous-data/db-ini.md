# DbIni

`DbIni` is a small database-backed key/value store.

It simulates an `.ini` file using a database table. It is useful for technical values that must live inside the same database as the application data.

## Table

The default table is `SYS_INI`.

The table and field names come from `DbConfig`.

- `SysDbIniTableName`
- `SysDbIniEntryField`
- `SysDbIniValueField`
- `SysDbIniBlobField`

When a `DbIni` instance is created, it ensures that the table exists.

```csharp
DbIni Ini = new DbIni(Db.GetDefaultConnectionInfo());
```

Most applications use the main instance.

```csharp
DbIni Ini = Db.MainIni;
```

## String And Scalar Values

`DbIni` stores values by entry key.

```csharp
DbIni Ini = Db.MainIni;

Ini.WriteString("Application.Title", "Tripous");
string Title = Ini.ReadString("Application.Title", "Default Title");

Ini.WriteInteger("Grid.RowLimit", 300);
int RowLimit = Ini.ReadInteger("Grid.RowLimit", 100);

Ini.WriteBool("Feature.Enabled", true);
bool Enabled = Ini.ReadBool("Feature.Enabled", false);

Ini.WriteDateTime("Maintenance.LastRun", DateTime.UtcNow);
DateTime LastRun = Ini.ReadDateTime("Maintenance.LastRun", DateTime.MinValue);
```

Supported scalar methods:

- `WriteString()` and `ReadString()`
- `WriteInteger()` and `ReadInteger()`
- `WriteFloat()` and `ReadFloat()`
- `WriteBool()` and `ReadBool()`
- `WriteDateTime()` and `ReadDateTime()`

## Blobs And Objects

`DbIni` can store binary data in the blob field.

```csharp
using MemoryStream Stream = new MemoryStream(Buffer);

Db.MainIni.WriteBlob("Report.Template", Stream);
```

It can also store an object as JSON in a blob.

```csharp
GridLayout Layout = new GridLayout();

Db.MainIni.WriteInstance("Customer.GridLayout", Layout);
Db.MainIni.ReadInstance("Customer.GridLayout", Layout);
```

This is useful for internal serialized state or simple per-database technical settings.

## Transactions

Several write methods have overloads that accept an existing `DbTransaction`.

```csharp
void SaveVersion(DbTransaction Transaction, DbIni Ini, int Version)
{
    Ini.WriteInteger(Transaction, "Database.Version.Default.Application", Version);
}
```

This is important when the value must be written atomically together with other database work.

## Schema Version Tracking

Tripous uses `DbIni` to track executed schema versions.

The entry name has this form:

```text
Database.Version.{ConnectionName}.{Domain}
```

During schema execution, Tripous reads this entry to find the current database version. After a schema version succeeds, it writes the new version to `DbIni` inside the final schema transaction.

## DbIni Versus Config

Use `DbIni` for low-level technical values.

Examples:

- database schema version;
- internal serialized state;
- per-database maintenance markers;
- small binary payloads.

Use `Config` for application configuration that has metadata, scopes, defaults, security level, editors and user/company/system resolution.

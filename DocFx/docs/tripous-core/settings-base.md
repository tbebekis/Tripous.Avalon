# SettingsBase

`SettingsBase` is the base class for JSON-backed settings objects.

It provides a small persistence pattern used by the framework-level settings classes:

- `Sys.Settings`
- `Db.Settings`
- `Ui.Settings`
- `Logger.Settings`
- `Db.Connections`

The class is intentionally simple. It loads and saves the public state of an object as JSON under `SysConfig.AppFolderPath`.

## File Location

By default, the file name is the class name plus `.json`.

```csharp
protected virtual string FileName => $"{this.GetType().Name}.json";
```

The full path is built from `SysConfig.AppFolderPath`.

```csharp
protected virtual string GetFilePath()
{
    return Path.Combine(SysConfig.AppFolderPath, FileName);
}
```

For example, `SysGlobalSettings` is stored as:

```text
{SysConfig.AppFolderPath}/SysGlobalSettings.json
```

A derived class may override `FileName` when the persisted file should have a stable explicit name.

```csharp
public class DbConnections: SettingsBase
{
    protected override string FileName => "DbConnections.json";
}
```

## Loading Settings

`Load()` calls `LoadBefore()`, reads the JSON file if it exists, populates the current instance, sets `IsLoaded`, and then calls `LoadAfter()`.

```csharp
public virtual void Load()
{
    LoadBefore();

    if (!File.Exists(SettingsFilePath))
    {
        IsLoaded = true;
        LoadAfter();
        return;
    }

    string JsonText = File.ReadAllText(SettingsFilePath);
    Json.PopulateObject(this, JsonText);

    IsLoaded = true;
    LoadAfter();
}
```

The object instance is preserved. Only its properties are loaded from JSON.

```csharp
Sys.Settings.Load();
Db.Settings.Load();
Ui.Settings.Load();
Logger.Settings.Load();
```

## Saving Settings

`Save()` creates the target folder when needed and writes the object through `Json.Serialize()`.

```csharp
Sys.Settings.NumericFormat = "N2";
Sys.Settings.DateFormat = "yyyy-MM-dd";
Sys.Settings.Save();
```

The same pattern is used by connection definitions.

```csharp
Db.Connections.Load();

DbConnectionInfo ConnectionInfo = new();
ConnectionInfo.Name = Sys.DEFAULT;
ConnectionInfo.DbServerType = DbServerType.Sqlite;
ConnectionInfo.ConnectionString = "Data Source=[Data]/app.db3";

Db.Connections.Add(ConnectionInfo);
Db.Connections.Save();
```

## Lifecycle Hooks

Derived classes can override four hooks.

- `LoadBefore()`
- `LoadAfter()`
- `SaveBefore()`
- `SaveAfter()`

`DbConnections` uses `LoadBefore()` to clear the current connection list before loading from disk.

```csharp
protected override void LoadBefore()
{
    List.Clear();
}
```

This prevents stale in-memory connection entries from surviving a reload.

## Global Settings Classes

Each framework layer has a global settings object that follows this pattern.

```csharp
static public SysGlobalSettings Settings { get; } = new();
static public DbGlobalSettings Settings { get; } = new();
static public UiGlobalSettings Settings { get; } = new();
static public LogGlobalSettings Settings { get; } = new();
```

Those objects expose layer-specific settings but rely on `SettingsBase` for persistence.

```csharp
Db.Settings.DefaultRowLimit = 500;
Db.Settings.LogSqlStatements = true;
Db.Settings.Save();
```

```csharp
Ui.Settings.FormColumnCount = 2;
Ui.Settings.ShowDataFormLog = true;
Ui.Settings.Save();
```

## When To Use It

Use `SettingsBase` for small JSON settings files owned by the application or framework.

Good fits:

- user-level application settings
- framework global settings
- tool settings
- database connection lists

Poor fits:

- business data
- large datasets
- multi-user configuration stored in the database
- values that need transaction control or audit history

For database-backed configuration use the Tripous.Data `Config` system instead.

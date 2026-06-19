# Sys, SysConfig And SysGlobalSettings

`Sys`, `SysConfig` and `SysGlobalSettings` form the small application-wide core of Tripous.

- `Sys` contains common utilities, constants, logging hooks, current user context and default formatting settings.
- `SysConfig` contains process/application configuration such as application mode and folder paths.
- `SysGlobalSettings` contains persisted global formatting settings exposed through `Sys.Settings`.

These classes are used by all other Tripous layers. `Tripous.Data` uses them for default values, connection paths and current user values. `Tripous.Desktop` uses them for display formats, logging callbacks and application folder commands.

## Application Bootstrap

Applications normally configure `SysConfig` during startup, before database connections, schemas or descriptors are registered.

```csharp
static void InitializeConfigs()
{
    SysConfig.ApplicationMode = ApplicationMode.Desktop;
    SysConfig.MainAssembly = typeof(AppHost).Assembly;
}
```

Tests and services can override the application folders. This keeps generated files, SQLite databases and settings isolated from the desktop user profile.

```csharp
void ConfigureApplication()
{
    SysConfig.ApplicationMode = ApplicationMode.Service;
    SysConfig.MainAssembly = typeof(TestDatabaseFixture).Assembly;
    SysConfig.AppFolderPath = fDatabaseFolder;
    SysConfig.AppDataFolderPath = fDatabaseFolder;
    SysConfig.AppTempFolderPath = fDatabaseFolder;
}
```

The folder properties are also used by other framework services.

- `SysConfig.AppFolderPath` is the base folder for JSON settings.
- `SysConfig.AppDataFolderPath` is used by data files, for example SQLite databases.
- `SysConfig.AppLogFolderPath` is used by logging.
- `SysConfig.AppTempFolderPath` is available for temporary files.

Connection strings may use path tokens that are resolved from `SysConfig`.

```csharp
DbConnectionInfo ConnectionInfo = new();
ConnectionInfo.Name = Sys.DEFAULT;
ConnectionInfo.DbServerType = DbServerType.Sqlite;
ConnectionInfo.ConnectionString = string.Format(
    DbServerType.Sqlite.GetTemplateConnectionString(),
    "[Data]/todo.db3");
```

## Sys Utilities

`Sys` is used throughout the codebase for small, repeated tasks that should behave consistently.

```csharp
if (Sys.IsNull(Row["CreatedAt"]))
    Row["CreatedAt"] = DateTime.Now;

if (Sys.IsNull(Row["IsDone"]))
    Row["IsDone"] = 0;
```

The conversion helpers provide default values instead of throwing on invalid or database-null input.

```csharp
decimal Amount = Sys.AsDecimal(Row["Amount"], 0);
int Quantity = Sys.AsInteger(Row["Quantity"], 0);
bool IsActive = Sys.AsBoolean(Row["IsActive"], false);
```

`Sys.GenId()` is the common helper for string identifiers.

```csharp
DataRow Row = Table.NewRow();
Row["Id"] = Sys.GenId();
```

`Sys.IsSameText()` is used for case-insensitive text comparisons.

```csharp
if (Sys.IsSameText(TableDef.Name, "TodoTask"))
    SetTodoDefaults(Row);
```

## Current User Context

`Sys.Context` stores process-level context. Data modules and document handlers use it to read the current application user.

```csharp
Sys.Context.CurrentUser = Module.LoadByUserName("test");

string UserId = Sys.GetCurrentAppUserId();
string UserName = Sys.GetCurrentAppUserName();
```

This is used by audit fields, posting logic and tests.

```csharp
Row.SetValue("ModifiedBy", Sys.GetCurrentAppUserId());
```

## Logging Hooks

`Sys` exposes logging delegates. The logging layer wires these delegates to `Logger`, while desktop applications can wire UI logging separately.

```csharp
Sys.DebugMode = true;
Sys.UiLogProc = Log;
```

Framework code can then call the common methods without depending directly on the UI or a concrete logging listener.

```csharp
try
{
    Store.ExecSql(SqlText);
}
catch (Exception Ex)
{
    Sys.LogError(Ex);
    throw;
}
```

## Global Formatting Settings

`Sys.Settings` is a `SysGlobalSettings` instance. It controls default numeric and date/time formats used by data and desktop code.

```csharp
Sys.Settings.NumericFormat = "N2";
Sys.Settings.DateFormat = "yyyy-MM-dd";
Sys.Settings.DateTimeFormat = "yyyy-MM-dd HH:mm";
Sys.Settings.Save();
```

The values are consumed by display helpers and grid column definitions.

```csharp
string Format = Sys.Settings.DateTimeFormat;
```

`SysGlobalSettings` derives from `SettingsBase`, so it can be loaded from and saved to a JSON file under `SysConfig.AppFolderPath`.

```csharp
Sys.Settings.Load();
Sys.Settings.NumericFormat = "N4";
Sys.Settings.Save();
```

## Typical Usage

Use these classes for application-wide concerns only.

- Use `SysConfig` during startup to define application mode, assembly and folders.
- Use `Sys` for common conversions, null checks, generated ids, current user access and logging hooks.
- Use `Sys.Settings` for global display formats.
- Do not place business rules in these classes.
- Do not use `Sys.Context` as a substitute for explicit method parameters when local state is clearer.

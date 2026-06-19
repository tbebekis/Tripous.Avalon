# Config System

Tripous.Data has two configuration layers.

- Runtime configuration for database access and data behavior.
- Persistent application configuration stored in the database.

## Runtime Data Configuration

The central entry point is `Db`.

```csharp
Db.LoadConnections();

DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
SqlStore Store = Db.DefaultStore;
DbIni Ini = Db.MainIni;
```

`Db.LoadConnections()` loads the registered database connections from `DbConnections.json`. The connection list is stored in `Db.Connections`, a `DbConnections` instance derived from `SettingsBase`.

```csharp
DbConnectionInfo ConnectionInfo = Db.Connections.Add(
    "Default",
    DbServerType.Sqlite,
    @"Data Source=/home/teo/.config/MyApp/Data/MyApp.db3",
    300);
```

Each `DbConnectionInfo` contains the connection name, `DbServerType`, connection string, command timeout, generator behavior and the associated `DbSchema` metadata object.

## DbConfig

`DbConfig` contains static database conventions used by the data layer.

- `DefaultConnectionName`
- `VariablesPrefix`
- `GuidOids`
- `CompanyFieldName`
- `CompanyId`
- `CodeProviderModuleName`
- `SysDbIniTableName`
- `SysLogTableName`
- `SysNumberSeriesTableName`
- `SysConfigTableName`

Example:

```csharp
DbConfig.DefaultConnectionName = "Default";
DbConfig.CompanyId = Sys.StandardCompanyGuid;
DbConfig.VariablesPrefix = ":@";
```

`VariablesPrefix` is used for logical variables such as `:@CompanyId`, `:@AppDate` and `:@SysDate`. This is different from the SQL parameter prefix used in normal SQL parameters.

## DbGlobalSettings

`Db.Settings` contains runtime behavior settings.

- `DefaultRowLimit`
- `DefaultCommandTimeoutSeconds`
- `IdFieldsVisible`
- `LocatorMinimumSearchTextLength`
- `LocatorMaximumDropDownRows`
- `LogSqlStatements`

Example:

```csharp
Db.Settings.DefaultRowLimit = 300;
Db.Settings.DefaultCommandTimeoutSeconds = 300;
Db.Settings.LocatorMinimumSearchTextLength = 3;
Db.Settings.LocatorMaximumDropDownRows = 75;
Db.Settings.LogSqlStatements = true;
```

These settings affect SQL execution, locator searches, descriptor defaults and SQL logging.

## Persistent Configuration

The `Config` class reads and writes application settings stored in the database.

Configuration definitions are registered in `DataRegistry.ConfigProperties`. Actual values are stored separately, normally in the `SYS_CONFIG` table.

```csharp
DataRegistry.AddConfigProperty(
    "MiniCrm.AutoOpenCustomerList",
    TitleKey: "Auto Open Customer List",
    GroupName: "Mini CRM",
    SecurityLevel: UserLevel.None,
    Kind: ConfigValueKind.Boolean,
    DefaultValue: "true");
```

Application code reads the effective value with `Config.GetValue()`.

```csharp
string Value = Config.GetValue("MiniCrm.AutoOpenCustomerList");
bool AutoOpen = Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
```

And it can store a user-level value with `Config.SetUserValue()`.

```csharp
Config.SetUserValue("MiniCrm.AutoOpenCustomerList", "false");
```

## Scopes

Configuration values may exist at three scopes.

- `System`
- `Company`
- `User`

Effective values are resolved in this order.

- User value
- Company value
- System value
- Definition default value

Company values use `DbConfig.CompanyId`. User values use the current application user from `Sys`.

## Value Kinds

`ConfigValueKind` tells Tripous how a value should be edited, validated and stored.

- `String`
- `Integer`
- `Boolean`
- `Date`
- `Time`
- `Double`
- `Decimal`
- `Lookup`
- `Enum`
- `Memo`
- `Object`

Simple values are stored in the normal value field. `Memo` and `Object` values use the text value field.

## Object Values

Object configuration values are stored as JSON.

```csharp
ConfigPropertyDef Def = DataRegistry.AddOrUpdateConfigProperty(
    "Application.Defaults",
    TitleKey: "Application Defaults",
    GroupName: "Application",
    SecurityLevel: UserLevel.Admin,
    Kind: ConfigValueKind.Object,
    DefaultValue: Json.Serialize(new AppDefaultProperties()),
    TypeName: typeof(AppDefaultProperties).FullName,
    EditorClassName: "MyApp.AppDefaultPropertiesEditor");

AppDefaultProperties Defaults = Config.GetObjectValue<AppDefaultProperties>("Application.Defaults");
```

`TypeName` identifies the object type. `EditorClassName` may point to a desktop editor for that configuration value.

## Apply Callback

A configuration definition may provide an `ApplyValueFunc`. It is called after a value is stored.

```csharp
ConfigPropertyDef Def = DataRegistry.AddOrUpdateConfigProperty(
    "Ui.ShowDataFormLog",
    TitleKey: "Show DataForm Log",
    GroupName: "Application",
    SecurityLevel: UserLevel.User,
    Kind: ConfigValueKind.Boolean,
    DefaultValue: "false");

Def.ApplyValueFunc = (ConfigDef, Text) =>
{
    bool Value = Convert.ToBoolean(Text, CultureInfo.InvariantCulture);
    Ui.Settings.ShowDataFormLog = Value;
};
```

This is useful when changing a stored configuration value must immediately update an in-memory setting.

# Overview

`Tripous.Logging` is the lightweight logging layer of Tripous.
It creates structured `LogEntry` objects and sends them to registered listeners.

The logger itself does not decide where log entries are stored.
That is the job of listeners, such as `FileLogListener` or `SyncedLogListener`.

## Main Types

- `Logger`, the static entry point.
- `LogGlobalSettings`, the settings facade over `Logger`.
- `LogLevel`, the accepted severity levels.
- `LogSource`, a named source that can use nested scopes.
- `LogEntry`, the actual log message.
- `LogRecord`, a tabular representation of a log entry.
- `LogListener`, the base class for listeners.
- `FileLogListener`, a listener that writes log lines to files.
- `SyncedLogListener`, a listener that marshals log entries to a synchronization context.
- `WriteLineFile`, the file writer used by `FileLogListener`.

## Basic Logging

Use `Logger` directly for simple messages.

```csharp
Logger.Info("Application started.");
Logger.Warn("Configuration value is missing.");
Logger.Error("Could not save file.");
```

Errors can carry exceptions.

```csharp
try
{
    Save();
}
catch (Exception Ex)
{
    Logger.Error(Ex);
}
```

The logger supports these levels:

- `Trace`
- `Debug`
- `Info`
- `Warning`
- `Error`
- `Fatal`

`Logger.MinLevel` controls which entries are accepted.
`Logger.Active` can disable logging completely.

## Sources And Scopes

For application code, a `LogSource` is usually better than calling `Logger` directly.
It names the component that creates log entries.

```csharp
LogSource Source = Logger.CreateSource("CustomerImporter");

Source.Info("Import started.");
Source.Warn("Customer code is empty.");
```

Scopes add a second label to entries.
They are useful around operations.

```csharp
Source.EnterScope("ImportCustomers");
Source.Info("Reading file.");
Source.ExitScope();
```

Each entry created inside that scope receives the source name and the current scope id.

## Listeners

Listeners receive `LogEntry` instances from `Logger`.
The logger calls listeners asynchronously, so listener code must be thread-safe.

```csharp
FileLogListener Listener = new FileLogListener();
Logger.Info("File logging is active.");
```

Listeners register themselves when constructed.
They can be removed with `Unregister()`.

## File Logging

`FileLogListener` writes log entries to files.
It uses `Logger.GetAsLine()` to format entries and `WriteLineFile` to rotate files by size.

```csharp
FileLogListener Listener = new FileLogListener(
    Folder: "",
    DefaultFileName: "Tripous",
    ColumnLine: Logger.GetLineCaptions(),
    MaxSizeKiloBytes: 512);
```

When no folder is passed, `Logger.LogFolderPath` is used.
Old files are deleted according to the retain policy.

## Global Settings

`LogGlobalSettings` exposes logger settings as a settings object.
The settings map directly to `Logger` properties.

Important settings are:

- `Active`
- `MinLevel`
- `LogFolderPath`
- `RetainPolicyCounter`
- `RetainDays`
- `MaxSizeKiloBytes`

## Output Formats

`LogEntry` can be formatted in several ways.

```csharp
string JsonText = Entry.AsJson();
string ListText = Entry.AsList();
string LineText = Entry.AsLine();
```

Use:

- `AsJson()` for structured storage or transport.
- `AsList()` for readable multiline diagnostics.
- `AsLine()` for log files.

## Practical Notes

- Use `Logger` for quick global logging.
- Use `LogSource` for component-level logging.
- Add at least one listener before expecting persisted output.
- Keep listener code thread-safe.
- Use `SyncedLogListener` before touching UI controls from log events.

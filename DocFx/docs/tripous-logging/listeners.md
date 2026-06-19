# Listeners

Listeners receive `LogEntry` objects from `Logger`.
They decide what to do with entries, such as writing to files or forwarding entries to UI code.

## LogListener

`LogListener` is the abstract base class for all listeners.
It registers itself with `Logger` when constructed, unless a derived class calls the protected constructor with auto-registration disabled.

```csharp
/// <summary>
/// Writes log entries to the console.
/// </summary>
public class ConsoleLogListener : LogListener
{
    /// <summary>
    /// Processes a log entry.
    /// </summary>
    /// <param name="Entry">The log entry.</param>
    public override void ProcessLog(LogEntry Entry)
    {
        Console.WriteLine(Entry.AsLine());
    }
}
```

Listeners can be manually registered or unregistered.

```csharp
LogListener Listener = new ConsoleLogListener();
Listener.Unregister();
Listener.Register();
```

`Logger` calls listeners asynchronously.
That means `ProcessLog()` must be thread-safe.
UI controls should not be updated directly from `ProcessLog()`.

## Retain Policy Properties

`LogListener` provides retain-policy properties for derived classes.

- `RetainPolicyCounter`, after how many writes to check retention.
- `RetainDays`, how many days to keep log files.
- `MaxSizeKiloBytes`, the maximum file size.

When a listener does not set its own values, the corresponding `Logger` values are used.

## FileLogListener

`FileLogListener` writes log entries to rotating files.

```csharp
FileLogListener Listener = new FileLogListener();

Logger.Info("Application started.");
```

The full constructor allows a folder, file name, first column line, and size limit.

```csharp
FileLogListener Listener = new FileLogListener(
    Folder: "",
    DefaultFileName: "Tripous",
    ColumnLine: Logger.GetLineCaptions(),
    MaxSizeKiloBytes: 512);
```

When the folder is empty, `Logger.LogFolderPath` is used.
When the file name is empty, `TripousLog.log` is used.
When the column line is empty, `Logger.GetLineCaptions()` is used.

Each entry is formatted with `Logger.GetAsLine()` before it is written.

## File Retention

`FileLogListener` uses `WriteLineFile`.
`WriteLineFile` creates a new file when the current file exceeds the configured size.

The generated file name starts with a UTC timestamp.

```text
yyyy-MM-dd_HH_mm_ss__fff_Tripous.log
```

After a number of writes, `FileLogListener` applies the retain policy and deletes old files.
The current log file is not deleted.

## SyncedLogListener

`SyncedLogListener` marshals log entries to a synchronization context.
It is intended for UI code.

```csharp
SyncedLogListener Listener = new SyncedLogListener();

Listener.EntryEvent += (Sender, Args) =>
{
    LogBox.AppendLine(Args.Entry.AsLine());
};
```

The logger still calls `ProcessLog()` asynchronously.
`SyncedLogListener` then posts the entry to the captured synchronization context and raises `EntryEvent`.

For derived classes, override `ProcessLogSynced()`.

```csharp
/// <summary>
/// Writes synchronized log entries to the desktop log box.
/// </summary>
public class UiLogListener : SyncedLogListener
{
    /// <summary>
    /// Processes a log entry on the synchronized context.
    /// </summary>
    /// <param name="Entry">The log entry.</param>
    public override void ProcessLogSynced(LogEntry Entry)
    {
        LogBox.AppendLine(Entry.AsLine());
    }
}
```

## LogEntryArgs

`LogEntryArgs` is the event argument type used by `SyncedLogListener.EntryEvent`.
It carries the `LogEntry`.

```csharp
Listener.EntryEvent += (Sender, Args) =>
{
    LogEntry Entry = Args.Entry;
    string Text = Entry.AsList();
};
```

## WriteLineFile

`WriteLineFile` is the low-level line writer used by `FileLogListener`.
It writes one line at a time, starts new files by size, and can delete files older than a configured number of days.

Application code usually uses `FileLogListener` instead of using `WriteLineFile` directly.

## Practical Notes

- Add at least one listener before expecting visible or persisted logs.
- Keep `ProcessLog()` thread-safe.
- Use `FileLogListener` for persisted application logs.
- Use `SyncedLogListener` for UI-bound logging.
- Call `Unregister()` when a listener should stop receiving entries.

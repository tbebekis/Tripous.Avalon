# Sources And Entries

`LogSource`, `LogEntry`, and `LogRecord` are the core data types of Tripous logging.
They define where a message comes from, what operation it belongs to, and how it can be represented.

## LogSource

`LogSource` is a named logging source.
The source name can be a class name, service name, form name, action name, or any other useful label.

```csharp
LogSource Source = LogSource.Create("OrderService");

Source.Info("Order service started.");
Source.Error("Order validation failed.");
```

A source can be disabled independently.

```csharp
Source.Active = false;
Source.Info("This entry is ignored.");
```

## Scopes

A source always has a current scope.
It starts with a default scope and can enter nested scopes.

```csharp
Source.EnterScope("PostInvoice");
Source.Info("Posting invoice.");
Source.ExitScope();
```

All entries created inside the scope receive the scope id.
Calling `ExitScope()` more times than needed is safe because the default scope is never removed.

Scopes may also carry parameters.
Those parameters are copied into each log entry created inside the scope.

```csharp
Dictionary<string, object> Params = new();
Params["InvoiceId"] = InvoiceId;

Source.EnterScope("PostInvoice", Params);
Source.Info("Posting invoice {InvoiceId}.");
Source.ExitScope();
```

## Source Logging Methods

`LogSource` provides level-specific methods.

```csharp
Source.Trace("Preparing data.");
Source.Debug("Mapped source row.");
Source.Info("Import completed.");
Source.Warn("Customer has no email.");
Source.Error("Import failed.");
```

It also supports explicit event ids.

```csharp
Source.Info("IMPORT-001", "Import started.");
```

For custom data, call `Log()` with a parameter dictionary.

```csharp
Dictionary<string, object> Params = new();
Params["CustomerId"] = CustomerId;
Params["OrderId"] = OrderId;

Source.Log(
    "ORDER-POSTED",
    LogLevel.Info,
    null,
    "Customer {CustomerId} order {OrderId} posted.",
    Params);
```

Parameter placeholders are replaced by values with matching dictionary keys.
The dictionary is also stored in the final `LogEntry`.

## LogEntry

`LogEntry` is the log message object passed to listeners.
It is created by `Logger` or by `LogSource`.

Important values include:

- `Id`, a generated entry id.
- `TimeStamp`, stored in UTC.
- `Date` and `Time`, derived text values.
- `User`, the current environment user.
- `Host`, the machine name.
- `Level` and `LevelText`.
- `Source`, the source name.
- `ScopeId`, the current scope.
- `EventId`, the event identifier.
- `Text`, the formatted message.
- `Exception`, when the entry represents an exception.
- `ExceptionData`, the full exception text.
- `Properties`, the structured parameter dictionary.

When no source is supplied and an exception exists, the source defaults to the exception type name.

## Formatting Entries

`LogEntry` has convenience methods for common output formats.

```csharp
string Line = Entry.AsLine();
string List = Entry.AsList();
string Json = Entry.AsJson();
```

`AsLine()` is used by file logging.
`AsList()` is useful for readable diagnostics.
`AsJson()` serializes a `LogRecord`.

## Saving One Entry

A single entry can be saved as a text file.

```csharp
Entry.SaveToFile();
```

When no folder is passed, `Logger.LogFolderPath` is used.
The file name is based on the entry timestamp.

## LogRecord

`LogRecord` is a tabular view of `LogEntry`.
It copies entry values into simple properties and can populate a `DataRow`.

```csharp
LogRecord Record = new LogRecord(Entry);
string Message = Record.MessageFull();
```

Use `LogRecord` when log data must be shown in a grid, stored in a table, or serialized in a simpler shape.

## Practical Notes

- Prefer one `LogSource` per component or workflow.
- Use scopes around operations that produce several log entries.
- Use event ids for messages that need stable identification.
- Use parameter dictionaries for values that should remain structured.
- Use `LogRecord` when displaying log entries in tabular UI.

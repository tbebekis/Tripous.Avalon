# DesktopExceptionHandler

`DesktopExceptionHandler` centralizes unhandled exception handling for Avalonia desktop applications.
It shows a user-facing error message, writes diagnostics to `LogBox` when available, and prevents repeated display of the same exception instance.

## What It Handles

`DesktopExceptionHandler.Initialize()` subscribes to three global sources.

- `AppDomain.CurrentDomain.UnhandledException`
- `TaskScheduler.UnobservedTaskException`
- `Dispatcher.UIThread.UnhandledException`

```csharp
DesktopExceptionHandler.Initialize();
```

Call it once during desktop application startup.

## UI Thread Exceptions

Avalonia UI thread exceptions are marked as handled.
The exception is then posted back to the UI thread for display and logging.

```csharp
Dispatcher.UIThread.UnhandledException += (Sender, Args) =>
{
    Args.Handled = true;
    LogAndShowException(Args.Exception);
};
```

This prevents the application from immediately crashing on an unhandled UI exception, while still making the error visible.

## Task Exceptions

Unobserved task exceptions are unwrapped, displayed, logged, and marked as observed.

```csharp
TaskScheduler.UnobservedTaskException += (Sender, Args) =>
{
    LogAndShowException(Args.Exception);
    Args.SetObserved();
};
```

Aggregate exceptions are reduced to their base exception before display.

## Business Exceptions

`TripousBusinessException` is treated as a user-facing error.
Only its message is shown.

```csharp
if (Ex is TripousBusinessException)
    await MessageBox.Error(Ex.Message);
```

This keeps expected business validation failures readable and avoids showing stack trace details to the user.

## Unexpected Exceptions

Unexpected exceptions use different messages in Debug and Release builds.

In Debug builds the message includes:

- exception type.
- error source.
- exception message.
- stack trace in the `LogBox`.

In Release builds the user sees a shorter message.

```csharp
string Message = $"An unexpected error occurred: {Ex.Message}";
await MessageBox.Error(Message);
```

The full exception is still written to debug output with `System.Diagnostics.Debug.WriteLine()`.

## Duplicate Protection

The handler stores the last exception instance.
If the same exception instance is reported again, it is ignored.

```csharp
if (Ex == LastException)
    return;

LastException = Ex;
```

This avoids repeated message boxes when the same exception is reported by more than one global handler.

## LogBox Integration

When `LogBox` has been initialized, the handler appends the error message there too.

```csharp
if (LogBox.IsInitialized)
    LogBox.AppendLine(Message);
```

`LogBox` is thread-safe and posts flush operations to the Avalonia UI thread.
It also keeps a maximum text length so the log control does not grow without limit.

## Practical Notes

- Initialize the handler once during application startup.
- Use `TripousBusinessException` for expected user-facing validation failures.
- Let unexpected exceptions reach the handler only when there is no local recovery path.
- Keep data modules free of UI dialogs; this handler belongs to the desktop layer.
- Use `LogBox` for diagnostics that should remain visible inside the application.

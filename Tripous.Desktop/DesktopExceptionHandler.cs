namespace Tripous.Desktop;

static public class DesktopExceptionHandler
{
    static void LogAndShowException(Exception ex)
    {
        if (ex == null) return;
 
        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                string Message = $"An unexpected error occurred: {ex.Message}";
                await MessageBox.Error(Message);
#if DEBUG
                Message += Environment.NewLine + ex.StackTrace;            
#endif
                if (LogBox.IsInitialized)
                    LogBox.AppendLine(Message);
            }
            catch 
            {
            }
        });

        // Optionally log errors
        System.Diagnostics.Debug.WriteLine($"GLOBAL ERROR: {ex}");
    }
    
    static public void Initialize()
    {
        // 1. Global exceptions in .NET (Non-UI threads)
        AppDomain.CurrentDomain.UnhandledException += (s, e) => 
            LogAndShowException(e.ExceptionObject as Exception);

        // 2. Exceptions inside Tasks (Async/Await)
        TaskScheduler.UnobservedTaskException += (s, e) => 
        {
            LogAndShowException(e.Exception);
            e.SetObserved(); // IMPORTANT: avoid application termination
        };

        // 3. Native Avalonia UI Exception Handling  
        Dispatcher.UIThread.UnhandledException += (s, e) =>
        {
            e.Handled = true; // IMPORTANT: avoid closing the application  
            LogAndShowException(e.Exception);
        };
    }
}
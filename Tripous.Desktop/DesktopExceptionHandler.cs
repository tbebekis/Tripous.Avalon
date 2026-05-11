namespace Tripous.Desktop;

static public class DesktopExceptionHandler
{
    static Exception LastException = null;
    // ● private
    static Exception Unwrap(Exception Ex)
    {
        if (Ex is AggregateException AggEx)
            return AggEx.GetBaseException();
        return Ex;
    }
#if DEBUG
    static void LogAndShowException(Exception Ex, string ErrorSource)
#else
    static void LogAndShowException(Exception Ex)
#endif
    {
        if (Ex == null)
            return;

        Ex = Unwrap(Ex);

        if (Ex == LastException)
            return;

        LastException = Ex;

        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
#if DEBUG
                string Message = $@"An unexpected error occurred.
Exception: {Ex.GetType().FullName}
Source: {ErrorSource}
Message: {Ex.Message}

";
                await MessageBox.Error(Message);
                Message += $@"Stack:
{Ex.StackTrace}
";
                 
#else
                string Message = $"An unexpected error occurred: {Ex.Message}";
                await MessageBox.Error(Message);
#endif                
                
                if (LogBox.IsInitialized)
                    LogBox.AppendLine(Message);
            }
            catch
            {
            }
        });

        System.Diagnostics.Debug.WriteLine($"GLOBAL ERROR: {Ex}");
    }

    // ● static public
    static public void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
#if DEBUG
            string ErrorSource = "AppDomain";
            LogAndShowException(e.ExceptionObject as Exception, ErrorSource);
#else
            LogAndShowException(e.ExceptionObject as Exception);
#endif            
            
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
#if DEBUG
            string ErrorSource = "TaskException";
            LogAndShowException(e.Exception, ErrorSource);
#else
            LogAndShowException(e.Exception);
#endif            
           
            e.SetObserved();
        };

        Dispatcher.UIThread.UnhandledException += (s, e) =>
        {
            e.Handled = true;
#if DEBUG
            string ErrorSource = "UiThread";
            LogAndShowException(e.Exception, ErrorSource);
#else
            LogAndShowException(e.Exception);
#endif


        };
    }
}
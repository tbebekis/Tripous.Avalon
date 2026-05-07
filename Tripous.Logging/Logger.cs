namespace Tripous.Logging;

/// <summary>
/// Represents an object that handles log information.
/// <para>
/// A code can instruct Logger to log a bit of information by calling
/// one of the various Log() methods. Then the Logger notifies
/// all ILogListener objects subscribed to it
/// by calling the ILogListener.ProcessLog() of each one
/// passing an LogInfo object that represents the log information.  
/// </para><para>
/// The Logger doesn't actually logs the information in any medium.
/// Instead it relies in that some one of its ILogListener subscribers
/// is capable of doing that.
/// </para><para>
/// Any thread can call any of the Log() methods of the Logger.
/// </para><para>
/// CAUTION: The Logger calls its Listeners asynchronously, that is from inside a thread.
/// Thus Listeners should synchronize the ProcessLog() call. Controls need to check if InvokeRequired.
/// </para>
/// </summary>
static public class Logger
{
    static Dictionary<string, object> EmptyParams = new Dictionary<string, object>();

    static readonly System.Threading.Lock syncLock = new();
    static int fActive = 0;
    static LogLevel fMinLevel = LogLevel.Info;
    static List<LogListener> listeners = new List<LogListener>();
    static Dictionary<string, int> fLineLengths = new Dictionary<string, int>();
    static string fLogFolderPath;
    static int fRetainDays;
    static int fMaxSizeKiloBytes;
    static int fRetainPolicyCounter;

    /// <summary>
    /// Static constructor
    /// </summary>
    static Logger()
    {
        Sys.LogExceptionProc = Error;
        Sys.LogErrorProc = Error;
        Sys.LogWarnProc = Warn;
        Sys.LogInfoProc = Info;
        
        fLineLengths.Add("Id", 40);
        fLineLengths.Add("TimeStamp", 24);
        fLineLengths.Add("Host", 24);
        fLineLengths.Add("User", 24);
        fLineLengths.Add("Level", 12);
        fLineLengths.Add("EventId", 14);
        fLineLengths.Add("Source", 64);
        fLineLengths.Add("Scope", 64);

        Active = true;
    }

    // ● public   
    /// <summary>
    /// Creates and returns a source
    /// </summary>
    static public LogSource CreateSource(string Name)
    {
        return new LogSource(Name);
    }

    /// <summary>
    /// Passes the log information to each listener.
    /// </summary>
    static public void Log(LogEntry Info)
    {
        lock (syncLock)
        {
            if (Active && fMinLevel != LogLevel.None && (int)Info.Level >= (int)fMinLevel)
            {
                foreach (LogListener Item in listeners)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            Item.ProcessLog(Info);
                        }
#if DEBUG
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
#else
                            catch
                            {
                            }
#endif

                    });


                }
            }

        }
    }
    /// <summary>
    /// Passes the log information to each listener.
    /// <para>NOTE: For how params are formatted into the specified text see <see cref="FormatParams" /> </para>
    /// </summary>
    static public void Log(string Source, string ScopeId, string EventId, LogLevel Level, Exception Exception, string Text, Dictionary<string, object> Params)
    {
        LogEntry Info = new LogEntry(Source, ScopeId, EventId, Level, Exception, Text, Params);
        Log(Info);
    }

    /// <summary>
    /// Logs a trace level message
    /// </summary>
    static public void Trace(string Source, string ScopeId, string EventId, string Text)
    {
        Exception Ex = null;
        Log(Source, ScopeId, EventId, LogLevel.Trace, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a trace level message
    /// </summary>
    static public void Trace(string Source, string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Trace, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a trace level message
    /// </summary>
    static public void Trace(string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Trace, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a trace level message
    /// </summary>
    static public void Trace(string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Trace, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs a debug level message
    /// </summary>
    static public void Debug(string Source, string ScopeId, string EventId, string Text)
    {
        Exception Ex = null;
        Log(Source, ScopeId, EventId, LogLevel.Debug, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a debug level message
    /// </summary>
    static public void Debug(string Source, string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Debug, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a debug level message
    /// </summary>
    static public void Debug(string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Debug, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a debug level message
    /// </summary>
    static public void Debug(string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Debug, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs an info level message
    /// </summary>
    static public void Info(string Source, string ScopeId, string EventId, string Text)
    {
        Exception Ex = null;
        Log(Source, ScopeId, EventId, LogLevel.Info, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an info level message
    /// </summary>
    static public void Info(string Source, string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Info, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an info level message
    /// </summary>
    static public void Info(string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Info, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an info level message
    /// </summary>
    static public void Info(string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Info, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs a warn level message
    /// </summary>
    static public void Warn(string Source, string ScopeId, string EventId, string Text)
    {
        Exception Ex = null;
        Log(Source, ScopeId, EventId, LogLevel.Warning, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a warn level message
    /// </summary>
    static public void Warn(string Source, string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Warning, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a warn level message
    /// </summary>
    static public void Warn(string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Warning, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a warn level message
    /// </summary>
    static public void Warn(string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Warning, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string Source, string ScopeId, string EventId, Exception Ex)
    {
        string Text = string.Empty;
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string Source, string EventId, Exception Ex)
    {
        string Text = string.Empty;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string EventId, Exception Ex)
    {
        string Text = string.Empty;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(Exception Ex)
    {
        string Text = string.Empty;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string Source, string ScopeId, string EventId, string Text)
    {
        Exception Ex = null;
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string Source, string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string EventId, string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    static public void Error(string Text)
    {
        Exception Ex = null;
        string ScopeId = "";
        string Source = "";
        string EventId = "0";
        Log(Source, ScopeId, EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Formats the specified params into a placeholder containing text. It also returns a dictionary of the specified params for use by structured log listeners.
    /// <example>The specified text may contain placeholders. Placeholders are replaced based in their order, NOT in their content. 
    /// The content of a placeholder becomes a key in the returned dictionary.
    /// <code>
    ///     string Text = "Customer {CustomerId} order {OrderId} is completed.";
    ///     var ParamsDictionary = FormatParams(ref Text, new { CustomerId = 123, OrderId = 456 });
    /// </code>
    /// </example>
    /// </summary>
    static public string FormatParams(string Text, Dictionary<string, object> Params)
    {
        // The Text parameter should be something like the following:
        // 'Customer {CustomerId} order {OrderId} is completed.';

        string Result = Text;
        string Param;
        string Value;
        foreach (var Pair in Params)
        {
            if (Pair.Value != null)
            {
                Param = "{" + Pair.Key + "}";
                Value = Pair.Value.ToString();

                Result = Result.Replace(Param, Value, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        return Result;
    }

    /// <summary>
    /// Adds a listener
    /// </summary>
    static internal void Add(LogListener Listener)
    {
        lock (syncLock)
        {
            try
            {
                if (!listeners.Contains(Listener))
                    listeners.Add(Listener);
            }
            catch
            {
            }
        }

    }
    /// <summary>
    /// Removes a listener
    /// </summary>
    static internal void Remove(LogListener Listener)
    {
        lock (syncLock)
        {
            try
            {
                if (listeners.Contains(Listener))
                    listeners.Remove(Listener);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Returns a string representation of a specified entry.
    /// </summary>
    static public string GetAsJson(LogEntry Entry)
    {
        LogRecord LR = new LogRecord(Entry);
        return Json.Serialize(LR);
    }
    /// <summary>
    /// Returns a string representation of a specified entry.
    /// </summary>
    static public string GetAsList(LogEntry Entry)
    {
        void AddLine(StringBuilder SB, string Name, string Value)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                Value = Name.PadRight(12) + ": " + Value;
                SB.Append(Value);
            }
        }

        StringBuilder SB = new StringBuilder();
        AddLine(SB, "Id", Entry.Id);
        AddLine(SB, "TimeStamp", Entry.TimeStampText);
        AddLine(SB, "Level", Entry.LevelText);
        AddLine(SB, "Source", Entry.Source);
        AddLine(SB, "Scope", Entry.ScopeId);
        AddLine(SB, "EventId", Entry.EventId);
        AddLine(SB, "Host", Entry.Host);
        AddLine(SB, "User", Entry.User);
        AddLine(SB, "Text", Entry.Text);

        if (!string.IsNullOrWhiteSpace(Entry.ExceptionData))
            AddLine(SB, "Stack", Environment.NewLine + Entry.ExceptionData);

        if ((Entry.Properties != null) && (Entry.Properties.Count > 0))
        {
            AddLine(SB, "Properties", " ");
            SB.AppendLine(Entry.GetPropertiesAsTextList());
        }

        return SB.ToString();

    }
    /// <summary>
    /// Returns a string representation of a specified entry.
    /// </summary>
    static public string GetAsLine(LogEntry Entry)
    {

        string RemoveLineEndings(string S)
        {
            return S.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        StringBuilder SB = new StringBuilder();
        SB.Append(Entry.Id.PadRight(fLineLengths["Id"]));
        SB.Append(Entry.TimeStampText.PadRight(fLineLengths["TimeStamp"]));
        SB.Append(Entry.Host.PadRight(fLineLengths["Host"]));
        SB.Append(Entry.User.PadRight(fLineLengths["User"]));
        SB.Append(Entry.LevelText.PadRight(fLineLengths["Level"]));
        SB.Append(Entry.EventId.PadRight(fLineLengths["EventId"]));
        SB.Append(Entry.Source.PadRight(fLineLengths["Source"]));
        SB.Append(Entry.ScopeId.PadRight(fLineLengths["Scope"]));

        if (!string.IsNullOrWhiteSpace(Entry.Text))
            SB.Append(RemoveLineEndings(Entry.Text));

        if (!string.IsNullOrWhiteSpace(Entry.ExceptionData))
            SB.Append(RemoveLineEndings(Entry.ExceptionData));

        string Result = SB.ToString();

        if ((Entry.Properties != null) && (Entry.Properties.Count > 0))
        {
            Result += " - Properties: ";
            Result += Entry.GetPropertiesAsSingleLine();
        }

        return Result;

    }

    /// <summary>
    /// Returns a string with the captions of the log information, property formatted, i.e. right padded with spaces.
    /// </summary>
    static public string GetLineCaptions()
    {
        StringBuilder SB = new StringBuilder();

        SB.Append("Id".PadRight(fLineLengths["Id"]));
        SB.Append("TimeStamp UTC".PadRight(fLineLengths["TimeStamp"]));
        SB.Append("Host".PadRight(fLineLengths["Host"]));
        SB.Append("User".PadRight(fLineLengths["User"]));
        SB.Append("Level".PadRight(fLineLengths["Level"]));
        SB.Append("EventId".PadRight(fLineLengths["EventId"]));
        SB.Append("Source".PadRight(fLineLengths["Source"]));
        SB.Append("Scope".PadRight(fLineLengths["Scope"]));
        SB.Append("Text");
        SB.AppendLine();

        return SB.ToString();
    }

    static public string GetLogTableSchema(string TableName = "SYS_LOG")
    {
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key
    ,Year int @NOT_NULL
    ,Month int @NOT_NULL
    ,DayOfMonth int @NOT_NULL
    ,LogTime @NVARCHAR(20) @NOT_NULL
    ,User @NVARCHAR(96) @NOT_NULL
    ,Host @NVARCHAR(96) @NOT_NULL
    ,Level @NVARCHAR(96) @NOT_NULL
    ,Source @NVARCHAR(512) @NOT_NULL
    ,Scope @NVARCHAR(512) @NOT_NULL
    ,EventId @NVARCHAR(96) @NOT_NULL
    ,Message @NBLOB_TEXT(96) @NOT_NULL 
)
";
        return SqlText;
    }
    
    // ● properties  
    /// <summary>
    /// When false no logs are recorded. Defaults to true.
    /// </summary>
    static public bool Active
    {
        get
        {
            lock (syncLock)
            {
                return fActive == 0;
            }
        }
        set
        {
            lock (syncLock)
            {
                if (!value)
                    fActive++;
                else
                {
                    fActive--;
                    if (fActive < 0)
                        fActive = 0;
                }
            }
        }
    }
    /// <summary>
    /// The level of the accepted log. For a log info to be recorded its log level must be greater or equal to this level. 
    /// See <see cref="LogLevel" /> enum for the numeric values of each level.
    /// <para>Defaults to Info.</para>
    /// </summary>
    static public LogLevel MinLevel
    {
        get
        {
            lock (syncLock)
            {
                return ((int)fMinLevel >= (int)LogLevel.Trace) ? fMinLevel : LogLevel.Trace;
            }
        }
        set
        {
            lock (syncLock) fMinLevel = value;
        }
    }
    /// <summary>
    /// Returns the path to the folder where file logs are saved
    /// </summary>
    static public string LogFolderPath
    {
        get
        {
            lock (syncLock)
            {
                string FolderPath = fLogFolderPath;

                if (string.IsNullOrWhiteSpace(fLogFolderPath))
                    FolderPath = SysConfig.AppLogFolderPath;

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                return FolderPath;
            }
        }
        set { fLogFolderPath = value; }

    }
    /// <summary>
    /// After how many writes to check whether it is time to apply the retain policy. Defaults to 100
    /// </summary>
    static public int RetainPolicyCounter
    {
        get { return fRetainPolicyCounter >= 100 ? fRetainPolicyCounter : 100; }
        set { fRetainPolicyCounter = value; }
    }
    /// <summary>
    /// Retain policy. How many days to retain in the storage medium. Defaults to 7
    /// </summary>
    static public int RetainDays
    {
        get { return fRetainDays >= 1 ? fRetainDays : 7; }
        set { fRetainDays = value; }
    }
    /// <summary>
    /// Retain policy. How many KB to allow a single log file to grow. Defaults to 512 KB
    /// </summary>
    static public int MaxSizeKiloBytes
    {
        get { return fMaxSizeKiloBytes >= 512 ? fMaxSizeKiloBytes : 512; }
        set { fMaxSizeKiloBytes = value; }
    }

    static public LogGlobalSettings Settings { get; } = new();
}

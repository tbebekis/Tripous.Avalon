namespace Tripous.Logging;

/// <summary>
/// Represents a named source in a log.
/// <para>The Source and the Scope of a LogEntry are just names devised by the developer.</para>
/// <para>A Source could be the full name of a class, such as a MainForm, an action url or any other suitable string and it marks all the log messages produced by this source.</para>
/// <para>A Scope could be the name of a method or the name of a group of methods, or whatever.</para>
/// <para>The Logger class provides a method to create a Source.</para>
/// <para>The Source creates a Default Scope when it is created. The developer never gets a reference to a Scope.</para>
/// <para>The developer may call EnterScope(...) passing it a Scope name.</para>
/// <para>The ExitScope() exits the last entered Scope. The Default Scope cannot exited.</para>
/// <para>It is even safe to call ExitScope() more times than the existing Scopes.</para>
/// <para>Any call to Source log methods, such as Log(), Debug(), Error(), etc. produces a LogEntry marked with the Source name and the name of the latest Scope.</para>
/// </summary>
public class LogSource
{

    /// <summary>
    /// Represents a named scope in a log
    /// </summary>
    class LogScope
    {
        LogSource fLogSource;

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public LogScope(LogSource LogSource, string Id = "", Dictionary<string, object> ScopeParams = null)
        {
            fLogSource = LogSource;

            if (string.IsNullOrWhiteSpace(Id))
                Id = Guid.NewGuid().ToString("D").ToUpper(); // Guid without brackets

            this.Id = Id;
             
            if (ScopeParams != null)
            {
                this.Properties = new Dictionary<string, object>();

                PropertyInfo[] Properties = ScopeParams.GetType().GetProperties();

                string Name;
                object Value;
                foreach (PropertyInfo Prop in Properties)
                {
                    Name = Prop.Name;
                    Value = Prop.GetValue(ScopeParams);

                    this.Properties[Name] = Value;  
                }
            }

        }

        // ● properties 
        /// <summary>
        /// A string that names the scope.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// A dictionary of the passed scope params, if any, else null.
        /// </summary>
        public Dictionary<string, object> Properties { get; private set; }
    }


    // ● private 
    readonly System.Threading.Lock syncLock = new();
    int fActive = 0;
    /// <summary>
    /// The list of scopes. Scopes can be nested. The last (default) scope can not be disposed. 
    /// That is there is always an available scope.
    /// </summary>
    List<LogScope> fScopes = new List<LogScope>();
    /// <summary>
    /// The current scope
    /// </summary>
    LogScope CurrentScope { get { return fScopes[fScopes.Count - 1]; } }
    Dictionary<string, object> EmptyParams = new ();

    // ● construction 
    /// <summary>
    /// Constructor
    /// </summary>
    public LogSource(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            Name = Sys.GenId(true);

        this.Name = Name;
        this.Active = true;
        EnterScope("Default Scope", new Dictionary<string, object>());
    }


    // ● public 
    /// <summary>
    /// Creates and returns a source
    /// </summary>
    static public LogSource Create(string Name)
    {
        return new LogSource(Name);
    }

    /// <summary>
    /// Begins a new log scope. A log scope is just a label under which next log messages are recorded. Log scopes can be nested.
    /// <para>The Name of a scope is the label of the scope.</para>
    /// <para>Scope params are attached to each log message in that scope. </para>
    /// </summary>
    public void EnterScope(string ScopeName, Dictionary<string, object> ScopeParams = null)
    {
        lock (syncLock)
        {
            if (string.IsNullOrWhiteSpace(ScopeName))   
                ScopeName = Sys.GenId(true);

            LogScope scope = new LogScope(this, ScopeName, ScopeParams);
            fScopes.Add(scope);                
        }
    }
    public void ExitScope()
    {
        lock (syncLock)
        {
            if (fScopes.Count > 1)
            {
                fScopes.RemoveAt(fScopes.Count - 1);
            }
        }
    }


    /// <summary>
    /// Logs a message
    /// </summary>
    public void Log(string EventId, LogLevel Level, Exception Ex, string Text, Dictionary<string, object> Params)
    {
        lock (syncLock)
        {
            if (Active)
            {
                LogEntry Info = new LogEntry(this.Name, CurrentScope.Id, EventId, Level, Ex, Text, Params);

                if (CurrentScope.Properties != null)
                {
                    foreach (var Entry in CurrentScope.Properties)
                    {
                        Info.Properties[Entry.Key] = Entry.Value;
                    }
                }

                Logger.Log(Info);
            }
        }

    }
    /// <summary>
    /// Logs a message
    /// </summary>
    public void Log(LogLevel Level, Exception Ex, string Text, Dictionary<string, object> Params)
    {  
        string EventId = "0";
        Log(EventId, Level, Ex, Text, Params);
    }
    /// <summary>
    /// Logs a message
    /// </summary>
    public void Log(string EventId, LogLevel Level, string Text, Dictionary<string, object> Params)
    {
        Exception Ex = null;
        Log(EventId, Level, Ex, Text, Params);
    }
    /// <summary>
    /// Logs a message
    /// </summary>
    public void Log(LogLevel Level, string Text, Dictionary<string, object> Params)
    {
        string EventId = "0";
        Exception Ex = null;
        Log(EventId, Level, Ex, Text, Params);
    }

    /// <summary>
    /// Logs a trace level message
    /// </summary>
    public void Trace(string EventId, string Text)
    {
        Log(EventId, LogLevel.Trace, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a trace level message
    /// </summary>
    public void Trace(string Text)
    {
        string EventId = "0";
        Trace(EventId, Text);
    }

    /// <summary>
    /// Logs a debug level message
    /// </summary>
    public void Debug(string EventId, string Text)
    {
        Log(EventId, LogLevel.Debug, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a debug level message
    /// </summary>
    public void Debug(string Text)
    {
        string EventId = "0";
        Debug(EventId, Text);
    }

    /// <summary>
    /// Logs an info level message
    /// </summary>
    public void Info(string EventId, string Text)
    {
        Log(EventId, LogLevel.Info, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an info level message
    /// </summary>
    public void Info(string Text)
    {
        string EventId = "0";
        Info(EventId, Text);
    }

    /// <summary>
    /// Logs a warn level message
    /// </summary>
    public void Warn(string EventId, string Text)
    {
        Log(EventId, LogLevel.Warning, Text, EmptyParams);
    }
    /// <summary>
    /// Logs a warn levelmessage
    /// </summary>
    public void Warn(string Text)
    {
        string EventId = "0";
        Warn(EventId, Text);
    }

    /// <summary>
    /// Logs an error message
    /// </summary>
    public void Error(string EventId, Exception Ex)
    {
        string Text = Ex.Message;
        Log(EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    public void Error(Exception Ex)
    {
        string EventId = "0";
        string Text = Ex.Message;
        Log(EventId, LogLevel.Error, Ex, Text, EmptyParams);
    }

    /// <summary>
    /// Logs an error message
    /// </summary>
    public void Error(string EventId, string Text)
    {
        Log(EventId, LogLevel.Error, Text, EmptyParams);
    }
    /// <summary>
    /// Logs an error message
    /// </summary>
    public void Error(string Text)
    {
        string EventId = "0";
        Log(EventId, LogLevel.Error, Text, EmptyParams);
    }


    // ● properties 
    /// <summary>
    /// The name of this log source.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// When false no logs are recorded. Defaults to true.
    /// </summary>
    public bool Active
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
}




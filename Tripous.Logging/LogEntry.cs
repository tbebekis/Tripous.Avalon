namespace Tripous.Logging;

/// <summary>
/// Represents a log message 
/// </summary>
public class LogEntry
{

    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
    public LogEntry(string Source, string ScopeId, string EventId, LogLevel Level, Exception Exception, string Text, Dictionary<string, object> Params = null)
    {
        Id = Sys.GenId(true);

        Host = Sys.HostName;
        User = Environment.UserName;

        TimeStamp = DateTime.UtcNow;
        TimeStampText = TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Date = TimeStamp.ToString("yyyy-MM-dd");
        Time = TimeStamp.ToString("HH:mm:ss.fff");

        this.Source = !string.IsNullOrWhiteSpace(Source) ? Source : (Exception != null ? Exception.GetType().FullName : string.Empty);
        this.ScopeId = !string.IsNullOrWhiteSpace(ScopeId) ? ScopeId : string.Empty;
        this.EventId = !string.IsNullOrWhiteSpace(EventId) ? EventId : "0";
        this.Level = Level;
        this.Exception = Exception;

        LevelText = this.Level.ToString();

        this.Text = !string.IsNullOrWhiteSpace(Text) ? Text : (Exception == null ? string.Empty : Exception.Message);
        ExceptionData = Exception == null ? string.Empty : Exception.GetErrorTextFull();

        Properties = Params;
        if (Properties != null && Properties.Count > 0) 
            this.Text = Logger.FormatParams(this.Text, Properties);
        else
            Properties = new Dictionary<string, object>();           
        
    }

    // ● public  
    /// <summary>
    /// Returns a string representation of this log info message.
    /// </summary>
    public override string ToString()
    {
        StringBuilder SB = new StringBuilder();

        SB.AppendLine(" ================================================================");
        SB.AppendLine("Date           : " + this.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        SB.AppendLine("NetUserName    : " + this.User);
        SB.AppendLine("Computer       : " + this.Host);
        SB.AppendLine(" ----------------------------------------------------------------");
        SB.AppendLine("Level          : " + Level.ToString());
        SB.AppendLine("Source         : " + Source);
        SB.AppendLine("Scope          : " + ScopeId);
        SB.AppendLine("Message text   : " + Text);
        if (!string.IsNullOrWhiteSpace(ExceptionData))
        {
            SB.AppendLine();
            SB.AppendLine(ExceptionData);
        }

        return SB.ToString();
    }
    /// <summary>
    /// Saves this log info to the folder. The file name is constructed using the date and time information.
    /// </summary>
    public void SaveToFile(string Folder = "")
    {
        Folder = !string.IsNullOrWhiteSpace(Folder) ? Folder : Logger.LogFolderPath;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        string FileName = Path.Combine(Folder, this.TimeStamp.ToFileName(true) + ".txt");
        File.WriteAllText(FileName, this.ToString());
    }

    /// <summary>
    /// Returns a string representation of the <see cref="Properties"/> property of this entry.
    /// </summary>
    public string GetPropertiesAsSingleLine()
    {
        string Result = string.Empty;

        if (Properties == null)
        {
            int Count = Properties.Count;
            int i = 0;

            foreach (var Pair in Properties)
            {
                i++;
                if (Pair.Value != null)
                {
                    Result += Pair.Key;
                    Result += " = ";
                    Result += Pair.Value.ToString();
                    if (i < Count)
                        Result += ", ";
                }
            }
        }

        return Result;
    }
    /// <summary>
    /// Returns a string representation of the <see cref="Properties"/> property of this entry.
    /// </summary>
    public string GetPropertiesAsTextList()
    {
        string Result = string.Empty;

        if (Properties == null)
        {
            int Count = Properties.Count;
            int i = 0;

            foreach (var Pair in Properties)
            {
                i++;
                if (Pair.Value != null)
                {
                    Result += Pair.Key;
                    Result += " = ";
                    Result += Pair.Value.ToString();
                    if (i < Count)
                        Result += Environment.NewLine;
                }
            }
        }

        return Result;
    }

    /// <summary>
    /// Returns a string representation of this entry.
    /// </summary>
    public string AsList()
    {
        return Logger.GetAsList(this);
    }
    /// <summary>
    /// Returns a string representation of this entry.
    /// </summary>
    public string AsLine()
    {
        return Logger.GetAsLine(this);
    }
    /// <summary>
    /// Returns a string representation of this entry.
    /// </summary>
    public string AsJson()
    {
        return Logger.GetAsJson(this);
    }

    // ● properties  
    public string Id { get; }
    /// <summary>
    /// Returns the UTC date-time this info created
    /// </summary>
    public DateTime TimeStamp { get; }
    /// <summary>
    /// The timestamp as string.
    /// </summary>
    public string TimeStampText { get; }
    /// <summary>
    /// Returns the UTC date this info created.
    /// </summary>
    public string Date { get; }
    /// <summary>
    /// Returns the UTC time this info created.
    /// </summary>
    public string Time { get; }
    /// <summary>
    /// The username of the current user of this application or the local computer
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// The name of the local computer
    /// </summary>
    public string Host { get; }

    /// <summary>
    /// The log level
    /// </summary>
    public LogLevel Level { get; }
    /// <summary>
    /// The level as string
    /// </summary>
    public string LevelText { get; }
    /// <summary>
    /// The event Id
    /// </summary>
    public string EventId { get; }

    /// <summary>
    /// The log message
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// The scope if any
    /// </summary>
    public string ScopeId { get; }
    /// <summary>
    /// The source of this log, if any
    /// </summary>
    public string Source { get; }
    /// <summary>
    /// A dictionary with params passed when the log message was formatted. For use by structured log listeners
    /// </summary>
    public Dictionary<string, object> Properties { get; }
    /// <summary>
    /// The exception if this is a log regarding an exception
    /// </summary>
    public Exception Exception { get; }
    /// <summary>
    /// The exception data, if this is a log regarding an exception
    /// </summary>
    public string ExceptionData { get; }


}


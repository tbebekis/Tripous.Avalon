namespace Tripous.Logging;

/// <summary>
/// A log listener that writes log info to file(s).
/// <para><strong>NOTE: </strong> Applies retain policy. By default keeps log files 7 days old.</para>
/// </summary>
public class FileLogListener : LogListener
{
    int Counter = 0;
    WriteLineFile LogFile;

    void ApplyRetainPolicy()
    {
        if (Counter > RetainPolicyCounter)
        {
            Counter = 0;
            LogFile.DeleteFilesOlderThan(RetainDays);
        }
    }

    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
    public FileLogListener(string Folder = "", string DefaultFileName = "", string ColumnLine = "", int MaxSizeKiloBytes = 512)
            : base()
    {
        LogFile = new WriteLineFile(Folder, DefaultFileName, ColumnLine, MaxSizeKiloBytes);
    }

    // ● public  
    /// <summary>
    /// Called by the Logger to pass LogInfo to a log listener.
    ///<para>
    /// CAUTION: The Logger calls its Listeners asynchronously, that is from inside a thread.
    /// Thus Listeners should synchronize the ProcessLogInfo() call. Controls need to check if InvokeRequired.
    /// </para>
    /// </summary>
    public override void ProcessLog(LogEntry Entry)
    {
        string Line = Logger.GetAsLine(Entry);
        LogFile.WriteLine(Line);

        Counter++;
        ApplyRetainPolicy();
    }

    // ● properties  
    /// <summary>
    /// The folder where log files are placed. Defaults to Sys.AppRootDataFolder/Logs
    /// </summary>
    public string Folder => LogFile.Folder;
    /// <summary>
    /// The max size of a log file in MB. When a file reaches that size, a new one is created. Defaults to 5MB.
    /// </summary>
    public override int MaxSizeKiloBytes
    {
        get => LogFile.MaxSizeKiloBytes;
        set { throw new TripousException($"Changing {nameof(MaxSizeKiloBytes)} is illegal."); }
    }
}
    


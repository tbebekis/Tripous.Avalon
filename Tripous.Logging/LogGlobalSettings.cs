namespace Tripous.Logging;

public class LogGlobalSettings: SettingsBase, INotifyPropertyChanged
{
 
    
    // ● private  
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    // ● construction  
    internal LogGlobalSettings()
    {
    }
    
    // ● properties
     /// <summary>
    /// When false no logs are recorded. Defaults to true.
    /// </summary>
    public bool Active
    {
        get => Logger.Active;
        set { if (Active != value) { Active = value; NotifyPropertyChanged(nameof(Active)); } }
    }
    /// <summary>
    /// The level of the accepted log. For a log info to be recorded its log level must be greater or equal to this level. 
    /// See <see cref="LogLevel" /> enum for the numeric values of each level.
    /// <para>Defaults to Info.</para>
    /// </summary>
    public LogLevel MinLevel
    {
        get => Logger.MinLevel;
        set { if (MinLevel != value) { MinLevel = value; NotifyPropertyChanged(nameof(MinLevel)); } }
    }
    /// <summary>
    /// Returns the path to the folder where file logs are saved
    /// </summary>
    public string LogFolderPath
    {
        get => Logger.LogFolderPath;
        set { if (LogFolderPath != value) { LogFolderPath = value; NotifyPropertyChanged(nameof(LogFolderPath)); } }

    }
    /// <summary>
    /// After how many writes to check whether it is time to apply the retain policy. Defaults to 100
    /// </summary>
    public int RetainPolicyCounter
    {
        get => Logger.RetainPolicyCounter;
        set { if (RetainPolicyCounter != value) { RetainPolicyCounter = value; NotifyPropertyChanged(nameof(RetainPolicyCounter)); } }
    }
    /// <summary>
    /// Retain policy. How many days to retain in the storage medium. Defaults to 7
    /// </summary>
    public int RetainDays
    {
        get => Logger.RetainDays;
        set { if (RetainDays != value) { RetainDays = value; NotifyPropertyChanged(nameof(RetainDays)); } }
    }
    /// <summary>
    /// Retain policy. How many KB to allow a single log file to grow. Defaults to 512 KB
    /// </summary>
    public int MaxSizeKiloBytes
    {
        get => Logger.MaxSizeKiloBytes;
        set { if (MaxSizeKiloBytes != value) { MaxSizeKiloBytes = value; NotifyPropertyChanged(nameof(MaxSizeKiloBytes)); } }
    }
    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}
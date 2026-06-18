/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Logging;

/// <summary>
/// Global settings for the <see cref="Logger"/> class.
/// </summary>
public class LogGlobalSettings: SettingsBase, INotifyPropertyChanged
{
    // ● private  
    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="PropertyName">The name of the property that changed.</param>
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
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
        set { if (Active != value) { Logger.Active = value; NotifyPropertyChanged(nameof(Active)); } }
    }
    /// <summary>
    /// The level of the accepted log. For a log info to be recorded its log level must be greater or equal to this level. 
    /// See <see cref="LogLevel" /> enum for the numeric values of each level.
    /// <para>Defaults to Info.</para>
    /// </summary>
    public LogLevel MinLevel
    {
        get => Logger.MinLevel;
        set { if (MinLevel != value) { Logger.MinLevel = value; NotifyPropertyChanged(nameof(MinLevel)); } }
    }
    /// <summary>
    /// Gets or sets the path to the folder where file logs are saved.
    /// </summary>
    public string LogFolderPath
    {
        get => Logger.LogFolderPath;
        set { if (LogFolderPath != value) { Logger.LogFolderPath = value; NotifyPropertyChanged(nameof(LogFolderPath)); } }

    }
    /// <summary>
    /// Gets or sets after how many writes to check whether it is time to apply the retain policy. Defaults to 100.
    /// </summary>
    public int RetainPolicyCounter
    {
        get => Logger.RetainPolicyCounter;
        set { if (RetainPolicyCounter != value) { Logger.RetainPolicyCounter = value; NotifyPropertyChanged(nameof(RetainPolicyCounter)); } }
    }
    /// <summary>
    /// Gets or sets how many days to retain in the storage medium. Defaults to 7.
    /// </summary>
    public int RetainDays
    {
        get => Logger.RetainDays;
        set { if (RetainDays != value) { Logger.RetainDays = value; NotifyPropertyChanged(nameof(RetainDays)); } }
    }
    /// <summary>
    /// Gets or sets how many KB to allow a single log file to grow. Defaults to 512 KB.
    /// </summary>
    public int MaxSizeKiloBytes
    {
        get => Logger.MaxSizeKiloBytes;
        set { if (MaxSizeKiloBytes != value) { Logger.MaxSizeKiloBytes = value; NotifyPropertyChanged(nameof(MaxSizeKiloBytes)); } }
    }
    
    // ● events
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}

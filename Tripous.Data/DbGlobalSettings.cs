namespace Tripous.Data;

/// <summary>
/// Db global settings
/// </summary>
public class DbGlobalSettings: SettingsBase, INotifyPropertyChanged
{
    bool fIdFieldsVisible;
    int fDefaultRowLimit;
    int fDefaultCommandTimeoutSeconds = 300;
    
    // ● private  
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    // ● construction  
    internal DbGlobalSettings()
    {
    }
    
    // ● properties
    /// <summary>
    /// The default RowLimit for browser SELECTs.
    /// </summary>
    public int DefaultRowLimit
    {
        get
        {
            if (fDefaultRowLimit >= 100 && fDefaultRowLimit <= 1500)
                return fDefaultRowLimit;
            return 300;
        }
        set { if (fDefaultRowLimit != value) { fDefaultRowLimit = value; NotifyPropertyChanged(nameof(DefaultRowLimit)); } }
    }
    /// <summary>
    /// Default command time in seconds
    /// </summary>
    public int DefaultCommandTimeoutSeconds
    {
        get => fDefaultCommandTimeoutSeconds >= 15 ? fDefaultCommandTimeoutSeconds : 15;
        set { if (fDefaultCommandTimeoutSeconds != value) { fDefaultCommandTimeoutSeconds = value; NotifyPropertyChanged(nameof(DefaultCommandTimeoutSeconds)); } }
    }
    /// <summary>
    /// When true then <see cref="FieldDef"/> fields with a name ending in "Id" are visible, else are hidden.
    /// </summary>
    public bool IdFieldsVisible 
    {
        get => fIdFieldsVisible;
        set { if (fIdFieldsVisible != value) { fIdFieldsVisible = value; NotifyPropertyChanged(nameof(IdFieldsVisible)); } }
    }
    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}
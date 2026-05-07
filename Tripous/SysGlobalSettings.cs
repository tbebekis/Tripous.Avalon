namespace Tripous;

public class SysGlobalSettings: SettingsBase, INotifyPropertyChanged
{

    string fNumericFormat;
    string fDateTimeFormat;
    string fDateFormat;
    
    // ● private  
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    // ● construction  
    internal SysGlobalSettings()
    {
    }
    
    // ● properties
    /// <summary>
    /// Default format
    /// </summary>
    public string NumericFormat
    {
        get => !string.IsNullOrWhiteSpace(fNumericFormat) ? fNumericFormat : "N2";
        set { if (fNumericFormat != value) { fNumericFormat = value; NotifyPropertyChanged(nameof(NumericFormat)); } }
    }
    /// <summary>
    /// Default format
    /// </summary>
    public string DateTimeFormat
    {
        get => !string.IsNullOrWhiteSpace(fDateTimeFormat) ? fDateTimeFormat : "yyyy-MM-dd HH:mm";
        set { if (fDateTimeFormat != value) { fDateTimeFormat = value; NotifyPropertyChanged(nameof(DateTimeFormat)); } }
    }
    /// <summary>
    /// Default format
    /// </summary>
    public string DateFormat
    {
        get => !string.IsNullOrWhiteSpace(fDateFormat) ? fDateFormat : "yyyy-MM-dd";
        set { if (fDateFormat != value) { fDateFormat = value; NotifyPropertyChanged(nameof(DateFormat)); } }
    }
    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}
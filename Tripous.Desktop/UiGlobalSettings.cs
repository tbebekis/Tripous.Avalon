namespace Tripous.Desktop;

public class UiGlobalSettings: INotifyPropertyChanged
{
    Stretch fFormImageStretch = Stretch.Uniform;
    int fFormMemoRowCount;
    double fFormColumnWidth;
    int fFormColumnCount;
    int fFormImageHeight;
    bool fShowIdColumns;
    
    // ● private  
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    
    static double GetCurrentScreenWidth()
    {
        if (Ui.MainWindow != null)
        {
            var screen = Ui.MainWindow.Screens.ScreenFromVisual(Ui.MainWindow);
            if (screen != null)
                return screen.WorkingArea.Width;
        }
            
        return 1024;  
    }
    static double GetAvailableScreenWidth()
    {
        double ScreenWidth = GetCurrentScreenWidth();
        double SideBarWidth = 200; 
        double Result = ScreenWidth - SideBarWidth;
        return Result;
    }
    static int GetFormDefaultColumnCount()
    {
        double AvailableWidth = GetAvailableScreenWidth();
        int Result = AvailableWidth > 1100 ? 3 : 2;
        return Result;
    }
    static double GetFormDefaultColumnWidth()
    {
        double AvailableWidth = GetAvailableScreenWidth();
        int ColumnCount = GetFormDefaultColumnCount();
        double Result = AvailableWidth / ColumnCount;
        return Result;
    }
    
    // ● construction  
    internal UiGlobalSettings()
    {
    }
    
    // ● properties
    /// <summary>
    /// How many columns a form layout may have in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public int FormColumnCount
    {
        get => fFormColumnCount >= 250 && fFormColumnCount <= 600 ? fFormColumnCount : GetFormDefaultColumnCount();
        set { if (fFormColumnCount != value) { fFormColumnCount = value; NotifyPropertyChanged(nameof(FormColumnCount)); } }
    }
    /// <summary>
    /// Column width of a form column in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public double FormColumnWidth
    {
        get => fFormColumnWidth >= 250 && fFormColumnWidth <= 600 ? fFormColumnWidth : GetFormDefaultColumnWidth();
        set { if (fFormColumnWidth != value) { fFormColumnWidth = value; NotifyPropertyChanged(nameof(FormColumnWidth)); } }
    }
    /// <summary>
    /// How many rows a control, for a <see cref="FieldDef.IsMemo"/> field, occupies in a column of an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public int FormMemoRowCount
    {
        get => fFormMemoRowCount >= 3 && fFormMemoRowCount <= 5 ? fFormMemoRowCount : 3;
        set { if (fFormMemoRowCount != value) { fFormMemoRowCount = value; NotifyPropertyChanged(nameof(FormMemoRowCount)); } }
    }
    /// <summary>
    /// The height of an image control, for a <see cref="FieldDef.IsImage"/> field,  of an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public int FormImageHeight
    {
        get => fFormImageHeight >= 80 && fFormImageHeight <= 300 ? fFormImageHeight : 160;
        set { if (fFormImageHeight != value) { fFormImageHeight = value; NotifyPropertyChanged(nameof(FormImageHeight)); } }
    }
    /// <summary>
    /// The stretch mode of an image control in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public Stretch FormImageStretch
    {
        get => fFormImageStretch;
        set { if (fFormImageStretch != value) { fFormImageStretch = value; NotifyPropertyChanged(nameof(FormImageStretch)); } }
    }
    /// <summary>
    /// When false then columns ending with Id are not visible.
    /// </summary>
    public bool ShowIdColumns
    {
        get => fShowIdColumns;
        set { if (fShowIdColumns != value) { fShowIdColumns = value; NotifyPropertyChanged(nameof(ShowIdColumns)); } }
    }
    

    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
    
}
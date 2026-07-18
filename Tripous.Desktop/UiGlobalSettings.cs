/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Global settings for the UI.
/// </summary>
public class UiGlobalSettings: SettingsBase, INotifyPropertyChanged
{
    Stretch fFormImageStretch = Stretch.Uniform;
    int fFormMemoRowCount;
    double fFormColumnWidth;
    int fFormColumnCount;
    int fFormImageHeight;
    int fFormMaxControlsPerColumn;
    bool fShowIdColumnsInGrid;
    double fDetailGridMinHeight;
    bool fShowLocatorGridFilterPanel;
    bool fShowDataFormLog;
    bool fShowDataFormFactBoxPane = true;
    int fNoteDurationSeconds;
    double fNoteWidth;
    double fNoteHeight;
    string fNoteForeground = "#FFFFFFFF";
    string fNoteInfoBackground = "#FF2563EB";
    string fNoteSuccessBackground = "#FF16A34A";
    string fNoteWarningBackground = "#FFF59E0B";
    string fNoteErrorBackground = "#FFDC2626";

    
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
        double SideBarWidth = 350; 
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
        get => fFormColumnCount >= 1 && fFormColumnCount <= 3 ? fFormColumnCount : GetFormDefaultColumnCount();
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
    /// How many rows a control, for a <see cref="FieldDef.IsMemo"/> field, occupies in a column of an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    public int FormMaxControlsPerColumn
    {
        get => fFormMaxControlsPerColumn >= 4 && fFormMaxControlsPerColumn <= 12 ? fFormMaxControlsPerColumn : 8;
        set { if (fFormMaxControlsPerColumn != value) { fFormMaxControlsPerColumn = value; NotifyPropertyChanged(nameof(FormMaxControlsPerColumn)); } }
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
    public bool ShowIdColumnsInGrids
    {
        get => fShowIdColumnsInGrid;
        set { if (fShowIdColumnsInGrid != value) { fShowIdColumnsInGrid = value; NotifyPropertyChanged(nameof(ShowIdColumnsInGrids)); } }
    }
    /// <summary>
    /// The default minimum height of a detail grid in an <see cref="ItemPage"/> hosted by a <see cref="DataForm"/>.
    /// </summary>
    public double DetailGridMinHeight
    {
        get => fDetailGridMinHeight >= 180 && fDetailGridMinHeight <= 350 ? fDetailGridMinHeight : 240;
        set { if (fDetailGridMinHeight != value) { fDetailGridMinHeight = value; NotifyPropertyChanged(nameof(DetailGridMinHeight)); } }
    }
    /// <summary>
    /// When true then locator grid dropdowns display a filter panel.
    /// </summary>
    public bool ShowLocatorGridFilterPanel
    {
        get => fShowLocatorGridFilterPanel;
        set { if (fShowLocatorGridFilterPanel != value) { fShowLocatorGridFilterPanel = value; NotifyPropertyChanged(nameof(ShowLocatorGridFilterPanel)); } }
    }
    /// <summary>
    /// When true then the <see cref="DataForm"/> log is shown.
    /// </summary>
    public bool ShowDataFormLog
    {
        get => fShowDataFormLog;
        set { if (fShowDataFormLog != value) { fShowDataFormLog = value; NotifyPropertyChanged(nameof(ShowDataFormLog)); } }
    }
    /// <summary>
    /// When true then the <see cref="DataForm"/> FactBox pane is shown initially.
    /// </summary>
    public bool ShowDataFormFactBoxPane
    {
        get => fShowDataFormFactBoxPane;
        set { if (fShowDataFormFactBoxPane != value) { fShowDataFormFactBoxPane = value; NotifyPropertyChanged(nameof(ShowDataFormFactBoxPane)); } }
    }
    /// <summary>
    /// Desktop notification duration in seconds.
    /// </summary>
    public int NoteDurationSeconds
    {
        get => fNoteDurationSeconds >= 1 && fNoteDurationSeconds <= 30 ? fNoteDurationSeconds : 4;
        set { if (fNoteDurationSeconds != value) { fNoteDurationSeconds = value; NotifyPropertyChanged(nameof(NoteDurationSeconds)); } }
    }
    /// <summary>
    /// Desktop notification width.
    /// </summary>
    public double NoteWidth
    {
        get => fNoteWidth >= 220 && fNoteWidth <= 600 ? fNoteWidth : 360;
        set { if (fNoteWidth != value) { fNoteWidth = value; NotifyPropertyChanged(nameof(NoteWidth)); } }
    }
    /// <summary>
    /// Desktop notification height.
    /// </summary>
    public double NoteHeight
    {
        get => fNoteHeight >= 52 && fNoteHeight <= 160 ? fNoteHeight : 76;
        set { if (fNoteHeight != value) { fNoteHeight = value; NotifyPropertyChanged(nameof(NoteHeight)); } }
    }
    /// <summary>
    /// Desktop notification foreground color.
    /// </summary>
    public string NoteForeground
    {
        get => !string.IsNullOrWhiteSpace(fNoteForeground) ? fNoteForeground : "#FFFFFFFF";
        set { if (fNoteForeground != value) { fNoteForeground = value; NotifyPropertyChanged(nameof(NoteForeground)); } }
    }
    /// <summary>
    /// Desktop information notification background color.
    /// </summary>
    public string NoteInfoBackground
    {
        get => !string.IsNullOrWhiteSpace(fNoteInfoBackground) ? fNoteInfoBackground : "#FF2563EB";
        set { if (fNoteInfoBackground != value) { fNoteInfoBackground = value; NotifyPropertyChanged(nameof(NoteInfoBackground)); } }
    }
    /// <summary>
    /// Desktop success notification background color.
    /// </summary>
    public string NoteSuccessBackground
    {
        get => !string.IsNullOrWhiteSpace(fNoteSuccessBackground) ? fNoteSuccessBackground : "#FF16A34A";
        set { if (fNoteSuccessBackground != value) { fNoteSuccessBackground = value; NotifyPropertyChanged(nameof(NoteSuccessBackground)); } }
    }
    /// <summary>
    /// Desktop warning notification background color.
    /// </summary>
    public string NoteWarningBackground
    {
        get => !string.IsNullOrWhiteSpace(fNoteWarningBackground) ? fNoteWarningBackground : "#FFF59E0B";
        set { if (fNoteWarningBackground != value) { fNoteWarningBackground = value; NotifyPropertyChanged(nameof(NoteWarningBackground)); } }
    }
    /// <summary>
    /// Desktop error notification background color.
    /// </summary>
    public string NoteErrorBackground
    {
        get => !string.IsNullOrWhiteSpace(fNoteErrorBackground) ? fNoteErrorBackground : "#FFDC2626";
        set { if (fNoteErrorBackground != value) { fNoteErrorBackground = value; NotifyPropertyChanged(nameof(NoteErrorBackground)); } }
    }

    // ● events
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}

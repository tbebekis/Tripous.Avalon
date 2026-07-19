/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Desktop notification type values.
/// </summary>
public enum DesktopNoteType
{
    /// <summary>
    /// Information notification.
    /// </summary>
    Information,
    /// <summary>
    /// Success notification.
    /// </summary>
    Success,
    /// <summary>
    /// Warning notification.
    /// </summary>
    Warning,
    /// <summary>
    /// Error notification.
    /// </summary>
    Error
}

/// <summary>
/// Displays a non-modal desktop notification.
/// </summary>
public class DesktopNote: Window
{
    // ● private fields
    static readonly List<DesktopNote> fNotes = [];
    static DesktopNote fSelectedNote;
    readonly DispatcherTimer fTimer = new();
    readonly DesktopNoteType fType;
    Border borderRoot;
    Border borderBox;
    TextBox edtMessage;
    bool fPinned;
    bool fIsResizing;
    Point fResizeStartPoint;
    double fResizeStartWidth;
    double fResizeStartHeight;

    // ● private
    static Color ParseColor(string Text, string DefaultText)
    {
        try
        {
            return Color.Parse(!string.IsNullOrWhiteSpace(Text) ? Text : DefaultText);
        }
        catch
        {
            return Color.Parse(DefaultText);
        }
    }
    static IBrush GetBackground(DesktopNoteType Type)
    {
        return Type switch
        {
            DesktopNoteType.Success => new SolidColorBrush(ParseColor(Ui.Settings.NoteSuccessBackground, "#FFE7FFFF")),
            DesktopNoteType.Warning => new SolidColorBrush(ParseColor(Ui.Settings.NoteWarningBackground, "#FFE7FFE7")),
            DesktopNoteType.Error => new SolidColorBrush(ParseColor(Ui.Settings.NoteErrorBackground, "#FFFFE7E7")),
            _ => new SolidColorBrush(ParseColor(Ui.Settings.NoteInfoBackground, "#FFFFFFD7")),
        };
    }
    static IBrush GetBorder(DesktopNoteType Type)
    {
        return Type switch
        {
            DesktopNoteType.Success => new SolidColorBrush(ParseColor("#FF2196F3", "#FF2196F3")),
            DesktopNoteType.Warning => new SolidColorBrush(ParseColor("#FF4CAF50", "#FF4CAF50")),
            DesktopNoteType.Error => new SolidColorBrush(ParseColor("#FFF44336", "#FFF44336")),
            _ => new SolidColorBrush(ParseColor("#FFFFEB3B", "#FFFFEB3B")),
        };
    }
    static string GetTitle(DesktopNoteType Type)
    {
        return Type switch
        {
            DesktopNoteType.Success => "Success",
            DesktopNoteType.Warning => "Warning",
            DesktopNoteType.Error => "Error",
            _ => "Information",
        };
    }
    static PixelRect GetOwnerArea()
    {
        Window Owner = Ui.MainWindow;
        if (Owner != null && Owner.ClientSize.Width > 0 && Owner.ClientSize.Height > 0)
            return new PixelRect(Owner.Position.X, Owner.Position.Y, (int)Owner.ClientSize.Width, (int)Owner.ClientSize.Height);
        return GetWorkingArea();
    }
    static PixelRect GetWorkingArea()
    {
        Window Owner = Ui.MainWindow;
        Screen Screen = Owner?.Screens.ScreenFromVisual(Owner);
        return Screen?.WorkingArea ?? new PixelRect(0, 0, 1024, 768);
    }
    static void Reposition()
    {
        PixelRect Area = GetOwnerArea();
        double Width = Ui.Settings.NoteWidth;
        double Height = Ui.Settings.NoteHeight;
        int Margin = 16;
        int Gap = 8;
        for (int Index = 0; Index < fNotes.Count; Index++)
        {
            DesktopNote Note = fNotes[Index];
            int X = Area.X + Area.Width - (int)Width - Margin;
            int Y = Area.Y + Area.Height - (int)Height - Margin - (Index * ((int)Height + Gap));
            Note.Position = new PixelPoint(X, Y);
        }
    }
    void CloseTimer_Tick(object sender, EventArgs e)
    {
        fTimer.Stop();
        if (!fPinned)
            Close();
    }
    void DesktopNote_Closed(object sender, EventArgs e)
    {
        fTimer.Stop();
        fTimer.Tick -= CloseTimer_Tick;
        Closed -= DesktopNote_Closed;
        fNotes.Remove(this);
        if (fSelectedNote == this)
            fSelectedNote = null;
        Reposition();
    }
    void Pin()
    {
        if (fPinned)
            return;
        fPinned = true;
        fTimer.Stop();
        fNotes.Remove(this);
        Reposition();
    }
    void Select()
    {
        if (fSelectedNote != null && fSelectedNote != this)
            fSelectedNote.SetSelected(false);
        fSelectedNote = this;
        SetSelected(true);
        Topmost = true;
        Activate();
    }
    void SetSelected(bool Value)
    {
        if (borderRoot != null)
            borderRoot.BorderBrush = Value
                ? new SolidColorBrush(Color.Parse("#591F2328"))
                : Brushes.Transparent;
    }
    void Note_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        Select();
        Pin();
    }
    void Caption_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.Source is Button)
            return;
        Select();
        Pin();
        BeginMoveDrag(e);
    }
    void ResizeGrip_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        Select();
        Pin();
        fIsResizing = true;
        fResizeStartPoint = e.GetPosition(this);
        fResizeStartWidth = Width;
        fResizeStartHeight = Height;
        e.Pointer.Capture(sender as IInputElement);
        e.Handled = true;
    }
    void ResizeGrip_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!fIsResizing)
            return;
        Point Point = e.GetPosition(this);
        Width = Math.Max(MinWidth, fResizeStartWidth + Point.X - fResizeStartPoint.X);
        Height = Math.Max(MinHeight, fResizeStartHeight + Point.Y - fResizeStartPoint.Y);
        e.Handled = true;
    }
    void ResizeGrip_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (!fIsResizing)
            return;
        fIsResizing = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }
    void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    void DesktopNote_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <param name="Type">The notification type.</param>
    public DesktopNote(string Message, DesktopNoteType Type)
    {
        fType = Type;
        Width = Ui.Settings.NoteWidth;
        Height = Ui.Settings.NoteHeight;
        MinWidth = 180;
        MinHeight = 70;
        CanResize = true;
        ShowInTaskbar = false;
        Topmost = true;
        WindowDecorations = Avalonia.Controls.WindowDecorations.None;
        Background = Brushes.Transparent;
        Content = CreateContent(Message);
        Closed += DesktopNote_Closed;
        KeyDown += DesktopNote_KeyDown;
        fTimer.Interval = TimeSpan.FromSeconds(Ui.Settings.NoteDurationSeconds);
        fTimer.Tick += CloseTimer_Tick;
    }

    // ● static public
    /// <summary>
    /// Shows a desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <param name="Type">The notification type.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote Show(string Message, DesktopNoteType Type)
    {
        DesktopNote Result = new(Message, Type);
        fNotes.Insert(0, Result);
        Reposition();
        Result.Show();
        Result.fTimer.Start();
        return Result;
    }

    // ● protected
    /// <summary>
    /// Creates the notification content.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <returns>The notification content control.</returns>
    protected virtual Control CreateContent(string Message)
    {
        Grid RootGrid = new()
        {
            RowDefinitions = new RowDefinitions("28,*")
        };
        Grid Caption = new()
        {
            ColumnDefinitions = new ColumnDefinitions("*,28")
        };
        TextBlock lblTitle = new()
        {
            Text = GetTitle(fType),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(6, 0, 0, 0),
            Foreground = new SolidColorBrush(ParseColor(Ui.Settings.NoteForeground, "#FF000000"))
        };
        Button btnClose = new()
        {
            Content = "x",
            Width = 22,
            Height = 22,
            Padding = new Thickness(0),
            Margin = new Thickness(0, 3, 3, 3),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Foreground = new SolidColorBrush(ParseColor(Ui.Settings.NoteForeground, "#FF000000")),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(btnClose, 1);
        Caption.Children.Add(lblTitle);
        Caption.Children.Add(btnClose);
        btnClose.Click += BtnClose_Click;

        edtMessage = new TextBox()
        {
            Text = Message ?? "",
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(4),
            Padding = new Thickness(4),
            Background = new SolidColorBrush(ParseColor("#59FFFFFF", "#59FFFFFF")),
            BorderBrush = Brushes.Transparent,
            Foreground = new SolidColorBrush(ParseColor(Ui.Settings.NoteForeground, "#FF000000")),
            FontFamily = FontFamily.Parse("monospace"),
            FontSize = 13
        };
        Grid.SetRow(edtMessage, 1);

        Border borderResizeGrip = new()
        {
            Width = 18,
            Height = 18,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Background = new SolidColorBrush(ParseColor("#01000000", "#01000000")),
            Cursor = new Cursor(StandardCursorType.SizeAll)
        };
        borderResizeGrip.PointerPressed += ResizeGrip_PointerPressed;
        borderResizeGrip.PointerMoved += ResizeGrip_PointerMoved;
        borderResizeGrip.PointerReleased += ResizeGrip_PointerReleased;
        Grid.SetRow(borderResizeGrip, 1);

        Border borderCaption = new()
        {
            Background = Brushes.Transparent,
            BorderBrush = GetBorder(fType),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Cursor = new Cursor(StandardCursorType.SizeAll),
            Child = Caption
        };
        borderCaption.PointerPressed += Caption_PointerPressed;
        RootGrid.Children.Add(borderCaption);
        RootGrid.Children.Add(edtMessage);
        RootGrid.Children.Add(borderResizeGrip);

        borderBox = new Border()
        {
            Background = GetBackground(fType),
            BorderBrush = GetBorder(fType),
            BorderThickness = new Thickness(6, 1, 1, 1),
            Child = RootGrid
        };
        borderRoot = new Border()
        {
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(2),
            Padding = new Thickness(1),
            Child = borderBox
        };
        borderRoot.PointerPressed += Note_PointerPressed;
        return borderRoot;
    }
}

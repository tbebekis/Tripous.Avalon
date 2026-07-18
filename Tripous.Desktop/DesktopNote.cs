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
    readonly DispatcherTimer fTimer = new();
    readonly DesktopNoteType fType;

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
            DesktopNoteType.Success => new SolidColorBrush(ParseColor(Ui.Settings.NoteSuccessBackground, "#FF16A34A")),
            DesktopNoteType.Warning => new SolidColorBrush(ParseColor(Ui.Settings.NoteWarningBackground, "#FFF59E0B")),
            DesktopNoteType.Error => new SolidColorBrush(ParseColor(Ui.Settings.NoteErrorBackground, "#FFDC2626")),
            _ => new SolidColorBrush(ParseColor(Ui.Settings.NoteInfoBackground, "#FF2563EB")),
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
        Close();
    }
    void DesktopNote_Closed(object sender, EventArgs e)
    {
        fTimer.Stop();
        fTimer.Tick -= CloseTimer_Tick;
        Closed -= DesktopNote_Closed;
        fNotes.Remove(this);
        Reposition();
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
        CanResize = false;
        ShowInTaskbar = false;
        Topmost = true;
        WindowDecorations = Avalonia.Controls.WindowDecorations.None;
        Background = Brushes.Transparent;
        Content = CreateContent(Message);
        Closed += DesktopNote_Closed;
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
        return new Border()
        {
            Background = GetBackground(fType),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14, 10),
            Child = new TextBlock()
            {
                Text = Message ?? "",
                Foreground = new SolidColorBrush(ParseColor(Ui.Settings.NoteForeground, "#FFFFFFFF")),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
            }
        };
    }
}

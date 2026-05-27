/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

public static class LogBox
{
    private const string SLine = "-------------------------------------------------------------------";
    static readonly object fLock = new();
    static readonly StringBuilder fBuffer = new();
    static TextBox fBox;
    static bool fFlushPosted;
    static int fMaxLength = 200000;

    // ● private
    /// <summary>
    /// Appends buffered text to the text box on the UI thread.
    /// </summary>
    static void Flush()
    {
        string Text;
        lock (fLock)
        {
            Text = fBuffer.ToString();
            fBuffer.Clear();
            fFlushPosted = false;
        }

        if (fBox == null || string.IsNullOrEmpty(Text))
            return;

        fBox.Text += Text;
        if (fBox.Text.Length > fMaxLength)
            fBox.Text = fBox.Text.Substring(fBox.Text.Length - fMaxLength);
        fBox.CaretIndex = fBox.Text?.Length ?? 0;
    }
    /// <summary>
    /// The core logging method. Thread-safe implementation for Avalonia.
    /// </summary>
    static void Log(string Text)
    {
        if (fBox == null || string.IsNullOrEmpty(Text))
            return;

        lock (fLock)
        {
            fBuffer.Append(Text);
            if (fFlushPosted)
                return;

            fFlushPosted = true;
        }

        Dispatcher.UIThread.Post(Flush, DispatcherPriority.Background);
    }

    // ● static public
    /// <summary>
    /// Initializes this class.
    /// </summary>
    static public void Initialize(TextBox Box)
    {
        fBox ??= Box;
    }
    /// <summary>
    /// Clears the box in a thread-safe manner.
    /// </summary>
    static public void Clear()
    {
        if (IsInitialized)
        {
            lock (fLock)
            {
                fBuffer.Clear();
            }

            Dispatcher.UIThread.Post(() => fBox.Text = string.Empty, DispatcherPriority.Background);
        }
    }
    /// <summary>
    /// Appends text in the box, in the last existing text line, if any.
    /// </summary>
    static public void Append(string Text)
    {
        if (IsInitialized && !string.IsNullOrWhiteSpace(Text))
            Log(Text);
    }
    /// <summary>
    /// Appends a new text line in the box.
    /// </summary>
    static public void AppendLine(string Text)
    {
        if (!IsInitialized) return;

        string FinalText;
        
        if (string.IsNullOrWhiteSpace(Text))
            FinalText = Environment.NewLine;
        else if (Text == SLine)
            FinalText = Environment.NewLine + Text;
        else
            FinalText = $"{Environment.NewLine}[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {Text} ";

        Log(FinalText);
    }
    static public void AppendLine(object Data)
    {
        if (Data != null)
        {
            string JsonText = Json.Serialize(Data);
            AppendLine(JsonText);
        }
    }
    static public void AppendLine(DataRow Row)
    {
        if (Row != null)
        {
            string JsonText = Json.Serialize(Row.ItemArray);
            AppendLine(JsonText);
        }
    }
    /// <summary>
    /// Appends a new empty text line in the box.
    /// </summary>
    static public void AppendLineEmpty() => AppendLine(string.Empty);
    /// <summary>
    /// Appends a new text line in the box based on an Exception.
    /// </summary>
    static public void AppendLine(Exception ex) => AppendLine(ex.ToString());
    /// <summary>
    /// Appends a separator line in the box.
    /// </summary>
    static public void AppendLine() => AppendLine(SLine);

    // ● properties
    /// <summary>
    /// Returns true if this class has been initialized via Initialize
    /// </summary>
    static public bool IsInitialized => fBox != null;
    /// <summary>
    /// Maximum number of characters kept in the text box.
    /// </summary>
    static public int MaxLength { get => fMaxLength; set => fMaxLength = value; }
}

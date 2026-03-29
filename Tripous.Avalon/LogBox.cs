namespace Tripous.Avalon;

public static class LogBox
{
    private const string SLine = "-------------------------------------------------------------------";
    private static TextBox _box;

    /// <summary>
    /// Initializes this class.
    /// </summary>
    public static void Initialize(TextBox box)
    {
        _box ??= box;
    }

    /// <summary>
    /// The core logging method. Thread-safe implementation for Avalonia.
    /// </summary>
    private static void Log(string text)
    {
        if (_box == null) return;

        // Χρησιμοποιούμε τον Dispatcher για να εκτελεστεί ο κώδικας στο UI Thread
        Dispatcher.UIThread.Post(() =>
        {
            _box.Text += text;
            
            // Αυτόματο scroll στο τέλος
            _box.CaretIndex = _box.Text?.Length ?? 0;
        });
    }

    /// <summary>
    /// Clears the box in a thread-safe manner.
    /// </summary>
    public static void Clear()
    {
        if (IsInitialized)
            Dispatcher.UIThread.Post(() => _box!.Text = string.Empty);
    }

    /// <summary>
    /// Appends text in the box, in the last existing text line, if any.
    /// </summary>
    public static void Append(string text)
    {
        if (IsInitialized && !string.IsNullOrWhiteSpace(text))
            Log(text);
    }

    /// <summary>
    /// Appends a new text line in the box.
    /// </summary>
    public static void AppendLine(string text)
    {
        if (!IsInitialized) return;

        string finalPath;
        
        if (string.IsNullOrWhiteSpace(text))
            finalPath = Environment.NewLine;
        else if (text == SLine)
            finalPath = Environment.NewLine + text;
        else
            finalPath = $"{Environment.NewLine}[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text} ";

        Log(finalPath);
    }

    /// <summary>
    /// Appends a new empty text line in the box.
    /// </summary>
    public static void AppendLineEmpty() => AppendLine(string.Empty);

    /// <summary>
    /// Appends a new text line in the box based on an Exception.
    /// </summary>
    public static void AppendLine(Exception ex) => AppendLine(ex.ToString());

    /// <summary>
    /// Appends a separator line in the box.
    /// </summary>
    public static void AppendLine() => AppendLine(SLine);

    /// <summary>
    /// Returns true if this class has been initialized via Initialize
    /// </summary>
    public static bool IsInitialized => _box != null;
}
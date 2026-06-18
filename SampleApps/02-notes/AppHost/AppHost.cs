namespace Notes;

/// <summary>
/// Provides application-wide startup state and helpers.
/// </summary>
static public partial class AppHost
{
    // ● constructor
    /// <summary>
    /// Initializes static application state.
    /// </summary>
    static AppHost()
    {
#if DEBUG
        Sys.DebugMode = true;
#endif
        HiddenMainWindow = new();
        Sys.UiLogProc = Log;
    }

    // ● static public
    /// <summary>
    /// Writes a message to the application log.
    /// </summary>
    /// <param name="Text">The log text.</param>
    static public void Log(string Text)
    {
        if (LogBox.IsInitialized)
            LogBox.AppendLine(Text);
    }

    // ● properties
    /// <summary>
    /// Gets the hidden startup window.
    /// </summary>
    static public HiddenMainWindow HiddenMainWindow { get; private set; }
    /// <summary>
    /// Gets the real main window.
    /// </summary>
    static public MainWindow MainWindow { get; private set; }
    /// <summary>
    /// Gets the Avalonia desktop lifetime.
    /// </summary>
    static public IClassicDesktopStyleApplicationLifetime AvaloniaDesktop { get; private set; }
    /// <summary>
    /// Gets the left pager handler.
    /// </summary>
    static public AppFormPagerHandler SideBarHandler { get; private set; }
    /// <summary>
    /// Gets the content pager handler.
    /// </summary>
    static public AppFormPagerHandler ContentHandler { get; private set; }
    /// <summary>
    /// Gets the default SQL store.
    /// </summary>
    static public SqlStore Store { get; private set; }
}

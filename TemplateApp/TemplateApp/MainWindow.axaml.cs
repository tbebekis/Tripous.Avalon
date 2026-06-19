namespace TemplateApp;

/// <summary>
/// Represents the main application window.
/// </summary>
public partial class MainWindow : Window
{
    // ● private fields
    bool fIsWindowInitialized;
    ToolBar fToolBar;
    AppFormPagerHandler fSideBarHandler;
    AppFormPagerHandler fContentHandler;

    // ● private methods
    /// <summary>
    /// Initializes the window after it is opened.
    /// </summary>
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);
        fSideBarHandler = new AppFormPagerHandler(pagerSideBar);
        fContentHandler = new AppFormPagerHandler(pagerContent);

        Ui.Post(() =>
        {
            CreateMenu();
            CreateToolBar();
            AppHost.InitializeUi(fSideBarHandler, fContentHandler);
            UpdateStatusBar("Ready");
        });
    }
    /// <summary>
    /// Creates the main menu.
    /// </summary>
    void CreateMenu()
    {
        MenuItem FileMenu = new() { Header = "_File" };
        FileMenu.AddMenuItem("_Application Folder", ShowAppFolder);
        FileMenu.AddMenuItem("_Toggle Log", ToggleLog);
        FileMenu.AddMenuItem("_Clear Log", LogBox.Clear);
        FileMenu.AddSeparator();
        FileMenu.AddMenuItem("E_xit", Close);
        MainMenu.Items.Add(FileMenu);
    }
    /// <summary>
    /// Creates the toolbar from registered commands.
    /// </summary>
    void CreateToolBar()
    {
        fToolBar = new();
        fToolBar.Panel = pnlToolBar;
        fToolBar.AddRange(AppRegistry.ToolBarCommands);
    }
    /// <summary>
    /// Updates the status bar.
    /// </summary>
    /// <param name="Message">The status message.</param>
    void UpdateStatusBar(string Message)
    {
        lblStatus.Text = Message;
        lblDetails.Text = "TemplateApp v1.0";
    }
    /// <summary>
    /// Opens the application folder.
    /// </summary>
    void ShowAppFolder()
    {
        Directory.CreateDirectory(SysConfig.AppFolderPath);
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
        UpdateStatusBar("Application folder opened.");
    }

    // ● protected methods
    /// <summary>
    /// Handles the window opened event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (fIsWindowInitialized)
            return;

        WindowInitialize();
        fIsWindowInitialized = true;
        LogBox.AppendLine("Application started.");
    }
    /// <summary>
    /// Handles the window closed event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Dispatcher.UIThread.Post(() => AppHost.HiddenMainWindow.Close(), DispatcherPriority.Background);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    // ● public methods
    /// <summary>
    /// Toggles the log area.
    /// </summary>
    public void ToggleLog()
    {
        if (edtLog.IsVisible)
        {
            edtLog.IsVisible = false;
            Splitter2.IsVisible = false;
        }
        else
        {
            Splitter2.IsVisible = true;
            edtLog.IsVisible = true;
        }
    }
    /// <summary>
    /// Toggles SQL statement logging.
    /// </summary>
    public void ToggleLogSqlStatements()
    {
        bool Flag = !Db.Settings.LogSqlStatements;
        string Text = Flag ? "ON" : "OFF";
        string Message = $"SQL statement logging is now: {Text}.";
        LogBox.AppendLine(Message);
        Db.Settings.LogSqlStatements = Flag;
    }
}

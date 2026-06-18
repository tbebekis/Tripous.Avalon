namespace MiniCrm;

/// <summary>
/// Represents the main application window.
/// </summary>
public partial class MainWindow : Window
{
    // ● private fields
    private bool fIsWindowInitialized;
    private ToolBar fToolBar;
    private AppFormPagerHandler fSideBarHandler;
    private AppFormPagerHandler fContentHandler;

    // ● private
    /// <summary>
    /// Initializes the window after it is opened.
    /// </summary>
    private void WindowInitialize()
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
    /// Creates menu items and assigns inline click handlers.
    /// </summary>
    private void CreateMenu()
    {
        MenuItem FileMenu = new() { Header = "_File" };
        MenuItem CustomersItem = new() { Header = "_Customers" };
        CustomersItem.Click += (Sender, Args) => ShowCustomers();
        MenuItem AppFolderItem = new() { Header = "_Application Folder" };
        AppFolderItem.Click += (Sender, Args) => ShowAppFolder();
        MenuItem ToggleLogItem = new() { Header = "_Toggle Log" };
        ToggleLogItem.Click += (Sender, Args) => ToggleLog();
        MenuItem ClearLogItem = new() { Header = "_Clear Log" };
        ClearLogItem.Click += (Sender, Args) => LogBox.Clear();
        MenuItem ExitItem = new() { Header = "E_xit" };
        ExitItem.Click += (Sender, Args) => Close();

        FileMenu.Items.Add(CustomersItem);
        FileMenu.Items.Add(AppFolderItem);
        FileMenu.Items.Add(ToggleLogItem);
        FileMenu.Items.Add(ClearLogItem);
        FileMenu.Items.Add(new Separator());
        FileMenu.Items.Add(ExitItem);
        MainMenu.Items.Add(FileMenu);
    }
    /// <summary>
    /// Creates the toolbar from registered commands.
    /// </summary>
    private void CreateToolBar()
    {
        fToolBar = new();
        fToolBar.Panel = pnlToolBar;
        fToolBar.AddRange(AppRegistry.ToolBarCommands);
    }
    /// <summary>
    /// Updates the status bar.
    /// </summary>
    /// <param name="Message">The status message.</param>
    private void UpdateStatusBar(string Message)
    {
        lblStatus.Text = Message;
        lblDetails.Text = "Mini CRM v1.0";
    }
    /// <summary>
    /// Shows the Customer data form.
    /// </summary>
    private void ShowCustomers()
    {
        AppHost.ContentHandler.ShowDataForm("Customer");
        UpdateStatusBar("Customers form opened.");
    }
    /// <summary>
    /// Opens the application folder that contains DbConnections.json and the Data folder.
    /// </summary>
    private void ShowAppFolder()
    {
        Directory.CreateDirectory(SysConfig.AppFolderPath);
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
        UpdateStatusBar("Application folder opened.");
    }

    // ● protected
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
        LogBox.AppendLine("Application Started.");
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

    // ● public
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
        string Message = $"SQL Statements Logging is now: {Text}.";
        LogBox.AppendLine(Message);
        Db.Settings.LogSqlStatements = Flag;
    }
}

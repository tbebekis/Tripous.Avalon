namespace TestApp;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private ToolBar ToolBar;

    private AppFormPagerHandler SideBarHandler; // pagerSideBar
    private AppFormPagerHandler ContentHandler; // pagerContent
    private MainCommandExecutor CommandExecutor;
 
    // ● private
    void WindowInitialize()
    {
        CommandExecutor = new MainCommandExecutor(this);
        CommandExecutor.RegisterCommands();
        
        SideBarHandler = new AppFormPagerHandler(pagerSideBar);
        ContentHandler = new AppFormPagerHandler(pagerContent);

        CreateMenu();
        CreateToolBar();

        AppHost.Initialize(SideBarHandler, ContentHandler);
    }
    
    void ToggleLog()
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
    void ShowApplicationFolder()
    {
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
    }

    void CreateMenu()
    {
    }
    void CreateToolBar()
    {
        ToolBar = new();
        ToolBar.Panel = pnlToolBar;

        ToolBar.AddButton("door_out.png", "Exit", (s, ea) => Close());
        ToolBar.AddButton("globe_model.png", "Countries", (s, ea) => ContentHandler.ShowDataForm("Country"));
        ToolBar.AddButton("user.png", "Customers", (s, ea) => ContentHandler.ShowDataForm("Customer"));
    }
 
    void Log(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        LogBox.AppendLine(Text);
    }

    void Test()
    {
        string Psw = "M!r0d@t0";
        //DataTests.TestSqlParamScanner_Edges();
        //DataTests.TestConnection(Psw);
        //DataTests.TestMsSqlStore(Psw);
        DataTests.TestConnectionStringBuilder();
    }
    
    // ● overrides
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (IsWindowInitialized)
            return;
 
        WindowInitialize();
        IsWindowInitialized = true;
    
        LogBox.AppendLine("Application Started.");
        
        Test();
    }
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        // TODO:
    }
    
    // ● construction
    public MainWindow()
    {
        InitializeComponent();
        Ui.MainWindow = this;
        LogBox.Initialize(edtLog);
    }
}
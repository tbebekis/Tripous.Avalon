namespace DesktopApp;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private ToolBar ToolBar;

    private AppFormPagerHandler SideBarHandler; // pagerSideBar
    private AppFormPagerHandler ContentHandler; // pagerContent
 
    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);
        
        SideBarHandler = new AppFormPagerHandler(pagerSideBar);
        ContentHandler = new AppFormPagerHandler(pagerContent);

        CreateMenu();
        CreateToolBar();

        AppHost.InitializeUi(SideBarHandler, ContentHandler);
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
        ToolBar.AddRange(AppRegistry.ToolBarCommands);
    }
 
    void Log(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        LogBox.AppendLine(Text);
    }

    void Test()
    {
        //DataTests.TestSqlParamScanner_Edges();
        //DataTests.TestConnection(Psw);
        //DataTests.TestMsSqlStore(Psw);
        //DataTests.TestConnectionStringBuilder();
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
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        
        Dispatcher.UIThread.Post(() => 
        {  
           AppHost.HiddenMainWindow.Close();  
        }, DispatcherPriority.Background);  
    }


    // ● construction
    public MainWindow()
    {
        InitializeComponent();
    }
}
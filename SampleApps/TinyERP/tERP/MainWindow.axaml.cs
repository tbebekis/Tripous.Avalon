namespace tERP;

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

        Ui.Post(() =>
        {
            CreateMenu();
            CreateToolBar();
            
            AppHost.InitializeUi(SideBarHandler, ContentHandler);  
            //Sys.LogInfo("Hi there");

            // a command for just calling the Test() method
            Command cmdTest = AppRegistry.ToolBarCommands.Find("Test");
            cmdTest.ExecuteCommand += (sender, args) => Test();
        });

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

    void HandleSchema()
    {
        string FolderPath = AppContext.BaseDirectory;
        string FilePath;
        
        Assembly A = typeof(DataLib).Assembly;

        string BaseNamespace = typeof(DataLib).Namespace;
        string SqlText = ResourceFiles.GetResourceFileText(A, BaseNamespace, "", "Schema01.sql");

        SchemaParserResult GroupDefs = SchemaRegistrationBuilder.Parse(SqlText, 1);
        
         
        FilePath = Path.Combine(FolderPath, "Schema.sql");
        File.WriteAllText(FilePath, GroupDefs.SchemaSql);
        
        // string Code = GroupDefs.GetCreateSqlTextByProvider(DbServerType.Sqlite);
        // FilePath = Path.Combine(FolderPath, "Schema_Sqlite.sql");
        // File.WriteAllText(FilePath, Code);
        
       
        FilePath = Path.Combine(FolderPath, "DEF_Schema.cs");
        File.WriteAllText(FilePath, GroupDefs.CreateTablesSourceCode);
        
         
        FilePath = Path.Combine(FolderPath, "DEF_Modules.cs");
        File.WriteAllText(FilePath, GroupDefs.ModuleDefsSourceCode);
        
       
        FilePath = Path.Combine(FolderPath, "DEF_Forms.cs");
        File.WriteAllText(FilePath, GroupDefs.FormDefsSourceCode);
        
        
        
        LogBox.AppendLine("HandleSchema: DONE");
    }

    void Test()
    {
        HandleSchema();
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
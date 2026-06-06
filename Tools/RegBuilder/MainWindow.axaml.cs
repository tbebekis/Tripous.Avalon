namespace RegBuilder;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    AppSettings Settings = new();
    string OutputFolderPath = Path.Combine(AppContext.BaseDirectory, "Output");
    
    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);

        //string FilePath = "../../../../../SampleApps/TinyERP/tERP.Data/Schema01.sql";
        //FilePath = Path.GetFullPath(FilePath);
        //LogBox.AppendLine(FilePath);
        
        if (!Directory.Exists(OutputFolderPath))
            Directory.CreateDirectory(OutputFolderPath);

        btnOpenOutputFolder.Click += (sender, args) => ShowOutputFolder();
        btnExecute.Click += (sender, args) => Execute();    
        btnAdd.Click += async (sender, args) => await Add();    
        btnEdit.Click += async (sender, args) => await Edit();
        btnDelete.Click += async (sender, args) => await Delete();
        
        btnExecute.IsEnabled = false;
 
        Ui.Post(() =>
        {
            SetHighlighters();
            LoadSettings();

            string RootFolderPath = FindRepoRootFolderPath();
            TypeStore.LoadAndRegisterAssemblies(Path.Combine(RootFolderPath, "SampleApps"));
            
            btnExecute.IsEnabled = true;
        });

    }
    void Execute()
    {
        RegBuilderProject Project = cboProjects.SelectedItem as RegBuilderProject;
        if (Project == null)
        {
            LogBox.AppendLine("No project selected.");
            return;
        }

        edtProjectLog.Text = $"Executing project: {Project.Name}. Please wait...";
        LogBox.AppendLine(edtProjectLog.Text);

        SchemaParserResult ParserResults = SchemaRegistrationBuilder.Parse(Project, OutputFolderPath);
        
        StringBuilder SB = new();
        
        if (ParserResults.HasErrors)
        {
            SB.AppendLine("ERRORS");
            SB.AppendLine(ParserResults.GetErrors());
        }
        if (ParserResults.HasWarnings)
        {
            SB.AppendLine("WARNINGS");
            SB.AppendLine(ParserResults.GetWarnings());
        }
        
        if (SB.Length > 0)
            SB.AppendLine();
        SB.AppendLine( $"Executing project: {Project.Name}. DONE");
        edtProjectLog.Text = SB.ToString();
            
        LogBox.AppendLine(SB.ToString());
 
        edtSchema.Text = ParserResults.CreateTablesSourceCode ?? string.Empty;
        edtModules.Text = ParserResults.ModuleDefsSourceCode ?? string.Empty;
        edtForms.Text = ParserResults.FormDefsSourceCode ?? string.Empty;
        edtLookups.Text = ParserResults.LookupDefsSourceCode ?? string.Empty;
        edtLocators.Text = ParserResults.LocatorDefsSourceCode ?? string.Empty;
        edtCodeProviders.Text = ParserResults.CodeProviderDefsSourceCode ?? string.Empty;
        edtSql.Text = ParserResults.SchemaSql ?? string.Empty;
 
    }
    void ShowOutputFolder()
    {        
        if (Directory.Exists(OutputFolderPath))
        Sys.OpenFileExplorer(OutputFolderPath);
    }
    async Task Add()
    {
        RegBuilderProject RegBuilderProject = new();
        RegBuilderProjectData BoxData = await RegBuilderProjectDialog.ShowModal(RegBuilderProject, this);
        if (BoxData.Result)
        {
            Settings.Projects.Add(BoxData.RegBuilderProject);
            Settings.Save();
            
            cboProjects.SelectedItem = RegBuilderProject;
        }
    }
    async Task Edit()
    {
        if (cboProjects.SelectedItem != null)
        {
            RegBuilderProject RegBuilderProject = cboProjects.SelectedItem as RegBuilderProject;
            RegBuilderProjectData BoxData = await RegBuilderProjectDialog.ShowModal(RegBuilderProject, this);
            if (BoxData.Result)
            {
                Settings.Save();
            }
        }
    }
    async Task Delete()
    {
        if (cboProjects.SelectedItem != null)
        {
            RegBuilderProject RegBuilderProject = cboProjects.SelectedItem as RegBuilderProject;
            bool Flag = await MessageBox.YesNo($"Delete the selected project: {RegBuilderProject.Name}?", this);
            if (Flag)
            {
                Settings.Projects.Remove(RegBuilderProject);
                Settings.Save();
            }
        }
    }
    void SetHighlighters()
    {
        var CSharp = Highlighters.Find(HighlightMode.CSharp);   
        var Sql =  Highlighters.Find(HighlightMode.SQL);  
        
        edtSchema.SyntaxHighlighting = CSharp;
        edtModules.SyntaxHighlighting = CSharp;
        edtForms.SyntaxHighlighting = CSharp;
        edtLookups.SyntaxHighlighting = CSharp;
        edtLocators.SyntaxHighlighting = CSharp;
        edtCodeProviders.SyntaxHighlighting = CSharp;
        edtSql.SyntaxHighlighting = Sql;
    }
    static string FindRepoRootFolderPath()
    {
        string Dir = AppContext.BaseDirectory;

        while (!string.IsNullOrWhiteSpace(Dir))
        {
            if (Directory.Exists(Path.Combine(Dir, "SampleApps")) &&
                Directory.Exists(Path.Combine(Dir, "Tools")))
                return Dir;

            Dir = Directory.GetParent(Dir)?.FullName;
        }

        return AppContext.BaseDirectory;
    }
    void LoadSettings()
    {
        Settings.Load();
        cboProjects.ItemsSource = Settings.Projects;

        if (Settings.Projects.Count > 0)
            cboProjects.SelectedIndex = 0;
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
 


    // ● construction
    public MainWindow()
    {
        InitializeComponent();
    }
}

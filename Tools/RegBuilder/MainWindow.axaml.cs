namespace RegBuilder;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    Settings Settings = new();
    string OutputFolderPath = Path.Combine(AppContext.BaseDirectory, "Output");
    string LastSchemaSql = null;
 
 
    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);
        
        // LoadAndRegisterAssemblies

        btnOpenOutputFolder.Click += (sender, args) => ShowOutputFolder();
        btnExecute.Click += (sender, args) => Execute();

        cboServer.ItemsSource = Enum.GetValues(typeof(DbServerType));
        cboServer.SelectedIndex = 0;
        cboServer.SelectionChanged += (sender, args) => ReplaceDataTypePlaceholders();

        Ui.Post(() =>
        {
            SetHighlighters();
            LoadSettings();

            string RootFolderPath = FindRepoRootFolderPath();
            TypeStore.LoadAndRegisterAssemblies(Path.Combine(RootFolderPath, "SampleApps"));
        });

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
    void SetHighlighters()
    {
        var CSharp = Highlighters.Find(HighlightMode.CSharp);   
        var Sql =  Highlighters.Find(HighlightMode.SQL);  
        
        edtTables.SyntaxHighlighting = CSharp;
        edtModules.SyntaxHighlighting = CSharp;
        edtForms.SyntaxHighlighting = CSharp;
        edtSchemaSql.SyntaxHighlighting = Sql;
        edtSchemaSqlServer.SyntaxHighlighting = Sql;
    }
    void LoadSettings()
    {
        Settings.Load();
        edtSourceFilePath.Text = Settings.SourceFilePath;
        edtSchemaVersion.Text = Settings.SchemaVersion.ToString();
        
        LogBox.AppendLine("Settings loaded.");
    }
    void SaveSettings()
    {
        Settings.SourceFilePath = edtSourceFilePath.Text;
        Settings.SchemaVersion = edtSchemaVersion.AsInt(-1);
        
        Settings.Save();  
        LogBox.AppendLine("Settings saved.");
    }
    void ShowOutputFolder()
    {
        if (Directory.Exists(OutputFolderPath))
            Sys.OpenFileExplorer(OutputFolderPath);
    }
    void Execute()
    {
        edtSchemaSql.Text = string.Empty;
        edtTables.Text = string.Empty;
        edtModules.Text = string.Empty;
        edtForms.Text = string.Empty;
            
        LastSchemaSql = string.Empty;
        
        SaveSettings();
        
        if (!Settings.Validate())
        {
            LogBox.AppendLine("Settings are not valid.");
            return;
        }
        
        if (!Directory.Exists(OutputFolderPath))
            Directory.CreateDirectory(OutputFolderPath);
        
        LogBox.AppendLine("Executing...");

        DuplicateCheck Checks = DuplicateCheck.None;
        if (chLookup.IsChecked == true)
            Checks |= DuplicateCheck.Lookup;
        if (chEnum.IsChecked == true)
            Checks |= DuplicateCheck.Enum;
        if (chModule.IsChecked == true)
            Checks |= DuplicateCheck.Module;
        if (chForm.IsChecked == true)
            Checks |= DuplicateCheck.Form;

        string SqlText = File.ReadAllText(Settings.SourceFilePath);
        SchemaParserResult ParserResults = SchemaRegistrationBuilder.Parse(SqlText, Settings.SchemaVersion, Checks);
 
        if (ParserResults.HasErrors || ParserResults.HasWarnings)
        {
            if (ParserResults.HasErrors)
            {
                LogBox.AppendLine("ERRORS");
                LogBox.AppendLine(ParserResults.GetErrors());
            }
            
            if (ParserResults.HasWarnings)
            {
                LogBox.AppendLine("WARNINGS");
                LogBox.AppendLine(ParserResults.GetWarnings());
            }
            
            return;
        }
        
        
        LogBox.Append("No errors...");

        string FilePath;
        /*
        string JsonText = Json.Serialize(ParserResults);
        FilePath = Path.Combine(OutputFolderPath, "RegBuilderResults.json");
        File.WriteAllText(FilePath, JsonText);
        */
       
        FilePath = Path.Combine(OutputFolderPath, "CodeProviderPatterns.cs");
        File.WriteAllText(FilePath, ParserResults.GenerateCodeProviderPatternsMethod());
         
        FilePath = Path.Combine(OutputFolderPath, "Schema.sql");
        File.WriteAllText(FilePath, ParserResults.SchemaSql);
       
        FilePath = Path.Combine(OutputFolderPath, "DEF_Schema.cs");
        File.WriteAllText(FilePath, ParserResults.CreateTablesSourceCode);
         
        FilePath = Path.Combine(OutputFolderPath, "DEF_Modules.cs");
        File.WriteAllText(FilePath, ParserResults.ModuleDefsSourceCode);
       
        FilePath = Path.Combine(OutputFolderPath, "DEF_Forms.cs");
        File.WriteAllText(FilePath, ParserResults.FormDefsSourceCode);
        
        Ui.Post(() =>
        {
            edtSchemaSql.Text = ParserResults.SchemaSql;
            edtTables.Text = ParserResults.CreateTablesSourceCode;
            edtModules.Text = ParserResults.ModuleDefsSourceCode;
            edtForms.Text = ParserResults.FormDefsSourceCode;
            
            LastSchemaSql = ParserResults.SchemaSql;

            ReplaceDataTypePlaceholders();
        });
        
        LogBox.Append("DONE");
    }

    void ReplaceDataTypePlaceholders()
    {
        if (string.IsNullOrWhiteSpace(LastSchemaSql))
            return;

        DbServerType ServerType = (DbServerType)cboServer.SelectedItem;
        
        SqlProvider Provider = SqlProviders.GetSqlProvider(ServerType);
        string SqlText = Provider.ReplaceDataTypePlaceholders(LastSchemaSql);
        edtSchemaSqlServer.Text = SqlText;
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
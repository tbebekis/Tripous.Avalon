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

        btnOpenOutputFolder.Click += (sender, args) => ShowOutputFolder();
        btnExecute.Click += (sender, args) => Execute();

        cboServer.ItemsSource = Enum.GetValues(typeof(DbServerType));
        cboServer.SelectedIndex = 0;
        cboServer.SelectionChanged += (sender, args) => ReplaceDataTypePlaceholders();

        Ui.Post(() =>
        {
            SetHighlighters();
            LoadSettings();
        });

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

        string SqlText = File.ReadAllText(Settings.SourceFilePath);
        SchemaParserResult GroupDefs = SchemaRegistrationBuilder.Parse(SqlText, Settings.SchemaVersion);

 
        if (GroupDefs.HasErrors || GroupDefs.HasWarnings)
        {
            if (GroupDefs.HasErrors)
            {
                LogBox.AppendLine("ERRORS");
                LogBox.AppendLine(GroupDefs.GetErrors());
            }
            
            if (GroupDefs.HasWarnings)
            {
                LogBox.AppendLine("WARNINGS");
                LogBox.AppendLine(GroupDefs.GetWarnings());
            }
            
            return;
        }
        
        
        LogBox.Append("No errors...");
        
         
        string FilePath = Path.Combine(OutputFolderPath, "Schema.sql");
        File.WriteAllText(FilePath, GroupDefs.SchemaSql);
       
        FilePath = Path.Combine(OutputFolderPath, "DEF_Schema.cs");
        File.WriteAllText(FilePath, GroupDefs.CreateTablesSourceCode);
         
        FilePath = Path.Combine(OutputFolderPath, "DEF_Modules.cs");
        File.WriteAllText(FilePath, GroupDefs.ModuleDefsSourceCode);
       
        FilePath = Path.Combine(OutputFolderPath, "DEF_Forms.cs");
        File.WriteAllText(FilePath, GroupDefs.FormDefsSourceCode);
        
        Ui.Post(() =>
        {
            edtSchemaSql.Text = GroupDefs.SchemaSql;
            edtTables.Text = GroupDefs.CreateTablesSourceCode;
            edtModules.Text = GroupDefs.ModuleDefsSourceCode;
            edtForms.Text = GroupDefs.FormDefsSourceCode;
            
            LastSchemaSql = GroupDefs.SchemaSql;

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
namespace SingleContextFile;

/*
AppRegistry.cs
   BaseDef.cs
   BoolOp.cs
   Command.cs
   ConditionOp.cs
   DataFieldType.cs
   DataFormAction.cs
   DataFormState.cs
   DataModule.cs
   DataRegistry.cs
   DataViewRowFilterFormatter.cs
   DefList.cs
   DesktopRegistry.cs
   FieldDef.cs
   FieldFlags.cs
   FormDef.cs
   FormDisplayMode.cs
   FormType.cs
   GridColumnDef.cs
   IDef.cs
   LocatorDef.cs
   LookupDisplayConverter.cs
   LookupItem.cs
   LookupSource.cs
   ModuleDef.cs
   ModuleItemDef.cs
   ReadOnlyDefList.cs
   SchemaVersionDef.cs
   SelectDef.cs
   SelectDefs.cs
   SqlFilterDef.cs
   SqlFilterDefs.cs
   SqlFilterExpressionDef.cs
   SqlFilterExpressionType.cs
   SqlWhereFilterFormatter.cs
   SqlWhereFilterMode.cs
   TableDef.cs
   TableSqls.cs
   TripousList.cs
 */

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    Settings Settings = new();
    readonly string OutputFolderPath = Path.Combine(AppContext.BaseDirectory, "Output");
    readonly string OutputFileName = "SingleFile.cs";
    string OutputFilePath;
 
    // ● private
    void WindowInitialize()
    {
        btnExecute.Click += (sender, args) => Execute();
        btnSave.Click += (sender, args) => SaveSettings();
        btnShowOutput.Click += (sender, args) => ShowOutputFolder();
        
        OutputFilePath = Path.Combine(OutputFolderPath, OutputFileName);
        edtOutputFilePath.Text = OutputFilePath;
        
        LogBox.Initialize(edtLog);
        LoadSettings();
        LogBox.AppendLine("Settings loaded");

        SetHighlighters();
    }
    void SetHighlighters()
    {
        var CSharp = Highlighters.Find(HighlightMode.CSharp);   
        //var Sql =  Highlighters.Find(HighlightMode.SQL);  
        
        edtOutput.SyntaxHighlighting = CSharp;
    }
    void LoadSettings()
    {
        Settings.Load();
 
        edtStartFolderPath.Text = Settings.StartFolderPath;
        edtMemo.Text = Settings.Text;
 
        if (string.IsNullOrWhiteSpace(edtStartFolderPath.Text))
            edtStartFolderPath.Text = "/home/teo/Dev/CSharp/Tripous.Avalon/Tripous.Desktop/Registry";
    }
    void SaveSettings()
    {
        Settings.StartFolderPath = edtStartFolderPath.Text;
        Settings.Text = edtMemo.Text;
        Settings.Save();  
        LogBox.AppendLine("Settings saved.");
    }
    void Execute()
    {
        LogBox.AppendLine("Executing");
        
        SaveSettings();
        
        if (!Directory.Exists(Settings.StartFolderPath))
        {
            LogBox.AppendLine($"Folder not found: {Settings.StartFolderPath}");
            return;
        }

        if (string.IsNullOrWhiteSpace(edtMemo.Text))
        {
            LogBox.AppendLine($"No files are defined");
            return;
        }
        
        string[] FilePaths = Directory.GetFiles(Settings.StartFolderPath, "*.cs", SearchOption.AllDirectories);
        Dictionary<string, string> SourceFilesDic = new();
        foreach (string FilePath in FilePaths)
        {
            string FileName = Path.GetFileName(FilePath);
            if (!SourceFilesDic.ContainsKey(FileName))
                SourceFilesDic.Add(FileName, FilePath);
        }

        StringBuilder SB = new();

        string FileText;
        string[] FileNames = Settings.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string FileName in FileNames)
        {
            if (SourceFilesDic.ContainsKey(FileName))
            {
                string FilePath = SourceFilesDic[FileName];
                FileText = File.ReadAllText(FilePath).Trim();
                SB.AppendLine(FileText);
                SB.AppendLine();
            }
        }

        FileText = SB.ToString();
        edtOutput.Text = FileText;
        
        string FolderPath = Path.GetDirectoryName(OutputFilePath);
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);
        
        File.WriteAllText(OutputFilePath, FileText);
        
        LogBox.AppendLine($"Output File saved: {OutputFilePath}");
        LogBox.AppendLine("DONE");
    }
    void ShowOutputFolder()
    {
        if (Directory.Exists(OutputFolderPath))
            Sys.OpenFileExplorer(OutputFolderPath);
    }
    
    // ● overrides
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (IsWindowInitialized)
            return;
 
        WindowInitialize();
        IsWindowInitialized = true;
    
 
    }
    
    public MainWindow()
    {
        InitializeComponent();
    }
}
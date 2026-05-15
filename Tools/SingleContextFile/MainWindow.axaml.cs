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
   LocatorFieldDef.cs
   Locator.cs
   LocatorEventArgs.cs
   LocatorEventType.cs
   LocatorSearchResult.cs
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
 
    SingleFileSet LastFileSet;
 
    // ● private
    void WindowInitialize()
    {
        btnExecute.Click += (sender, args) => Execute();
       
        btnShowOutput.Click += (sender, args) => ShowOutputFolder();
        
        btnAdd.Click += async (sender, args) => await AddFileSet();
        btnDelete.Click += async (sender, args) => await DeleteFileSet();
        btnSave.Click += (sender, args) => SaveSettings();

        lboFileSets.SelectionChanged += (sender, args) => SelectedFileSetChanged();
 
        LogBox.Initialize(edtLog);
        
        // load settings
        Settings.Load();
        edtStartFolderPath.Text = Settings.StartFolderPath;
 
        if (string.IsNullOrWhiteSpace(edtStartFolderPath.Text))
            edtStartFolderPath.Text = "/home/teo/Dev/CSharp/Tripous.Avalon/Tripous.Desktop/Registry";
        
        lboFileSets.ItemsSource = Settings.FileSets;
        if (lboFileSets.Items.Count > 0)
            lboFileSets.SelectedIndex = 0;
 
        LogBox.AppendLine("Settings loaded");

        SetHighlighters();
    }
    void SetHighlighters()
    {
        var CSharp = Highlighters.Find(HighlightMode.CSharp);   
        //var Sql =  Highlighters.Find(HighlightMode.SQL);  
        
        edtOutput.SyntaxHighlighting = CSharp;
    }
    void SaveSettings()
    {
        Settings.StartFolderPath = edtStartFolderPath.Text;
        
        SingleFileSet FileSet = lboFileSets.SelectedItem as SingleFileSet;
        if (FileSet != null)
            FileSet.Text = edtMemo.Text;
        
        Settings.Save();  
        LogBox.AppendLine("Settings saved.");
    }
    void ShowOutputFolder()
    {
        if (Directory.Exists(OutputFolderPath))
            Sys.OpenFileExplorer(OutputFolderPath);
    }

    void SelectedFileSetChanged()
    {
        if (LastFileSet != null)
        {
            LastFileSet.Text = edtMemo.Text;
            Settings.Save();  
        }

        LastFileSet = null;
        edtMemo.Text = string.Empty;
        edtOutput.Text = string.Empty;
        
        SingleFileSet FileSet = lboFileSets.SelectedItem as SingleFileSet;
        if (FileSet != null)
        {
            edtMemo.Text = FileSet.Text;
            LastFileSet = FileSet;
        }
    }

    async Task AddFileSet()
    {
        InputBoxData Data = await Ui.InputBox("Please provide a FileName for the new FileSet", "", this);
        if (Data.Result)
        {
            string FileName = Data.Value;
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                FileName = Path.ChangeExtension(FileName, "cs");
                if (Settings.FileSets.Any(x => x.FileName.IsSameText(FileName)))
                {
                    LogBox.AppendLine($"FileSet already exists: {FileName}");
                    return;
                }

                SingleFileSet FileSet = new() { FileName = FileName };
                Settings.FileSets.Add(FileSet);
                SaveSettings();
                lboFileSets.SelectedItem = FileSet;
            }
        }
    }
    async Task DeleteFileSet()
    {
        SingleFileSet FileSet = lboFileSets.SelectedItem as SingleFileSet;
        if (FileSet != null)
        {
            bool Flag = await MessageBox.YesNo($"Delete the FileSet: {FileSet.FileName}?", this);
            if (Flag)
            {
                Settings.FileSets.Remove(FileSet);
                Settings.Save();  
                if (lboFileSets.Items.Count > 0)
                    lboFileSets.SelectedIndex = 0;
            }
        }
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
        
        SingleFileSet FileSet = lboFileSets.SelectedItem as SingleFileSet;
        if (FileSet == null)
        {
            LogBox.AppendLine($"No FileSet to execute");
            return;
        }

        if (string.IsNullOrWhiteSpace(FileSet.Text))
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
        string[] FileNames = FileSet.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

        if (!Directory.Exists(OutputFolderPath))
            Directory.CreateDirectory(OutputFolderPath);
        
        string OutputFilePath = Path.Combine(OutputFolderPath, FileSet.FileName);
        File.WriteAllText(OutputFilePath, FileText);
        
        LogBox.AppendLine($"Output File saved: {OutputFilePath}");
        LogBox.AppendLine("DONE");
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
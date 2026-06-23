/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace RegBuilder;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    RegBuilderSettings Settings = new();
    RegBuilderProject LastGeneratedProject;
    readonly string SettingsFilePath = Path.Combine(AppContext.BaseDirectory, "AppSettings.json");
    readonly string WorkingFolderPath = Path.Combine(AppContext.BaseDirectory, "Output");
    
    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);

        //string FilePath = "../../../../../SampleApps/TinyERP/tERP.Data/Schema01.sql";
        //FilePath = Path.GetFullPath(FilePath);
        //LogBox.AppendLine(FilePath);
        
        if (!Directory.Exists(WorkingFolderPath))
            Directory.CreateDirectory(WorkingFolderPath);

        btnOpenWorkingFolder.Click += (sender, args) => ShowWorkingFolder();
        btnCopyToOutputFolder.Click += (sender, args) => CopyToOutputFolder();
        btnExecute.Click += (sender, args) => Execute();    
        btnAdd.Click += async (sender, args) => await Add();    
        btnEdit.Click += async (sender, args) => await Edit();
        btnDelete.Click += async (sender, args) => await Delete();
        
        btnExecute.IsEnabled = false;
        btnCopyToOutputFolder.IsEnabled = false;
 
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

        SchemaParserResult ParserResults = SchemaRegistrationBuilder.Parse(Project, WorkingFolderPath);
        
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
        LastGeneratedProject = Project;
        btnCopyToOutputFolder.IsEnabled = !ParserResults.HasErrors;
    }
    void ShowWorkingFolder()
    {        
        if (Directory.Exists(WorkingFolderPath))
            Sys.OpenFileExplorer(WorkingFolderPath);
    }
    void CopyToOutputFolder()
    {
        try
        {
            RegBuilderProject Project = cboProjects.SelectedItem as RegBuilderProject;
            if (Project == null)
            {
                LogBox.AppendLine("No project selected.");
                return;
            }
            if (!ReferenceEquals(Project, LastGeneratedProject))
            {
                LogBox.AppendLine("Execute the selected project before copying generated files.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Project.OutputFolderPath))
            {
                LogBox.AppendLine("No output folder configured for project: " + Project.Name);
                return;
            }

            string OutputFolderPath = ResolvePath(Project.OutputFolderPath);
            Directory.CreateDirectory(OutputFolderPath);

            foreach (string FileName in SchemaRegistrationBuilder.GetGeneratedSourceFileNames(Project.SchemaVersion))
            {
                string SourceFilePath = Path.Combine(WorkingFolderPath, FileName);
                string TargetFilePath = Path.Combine(OutputFolderPath, FileName);
                if (!File.Exists(SourceFilePath))
                    throw new FileNotFoundException("Generated source file was not found.", SourceFilePath);
                File.Copy(SourceFilePath, TargetFilePath, true);
            }

            LogBox.AppendLine($"Copied generated files: {Project.Name} -> {OutputFolderPath}");
        }
        catch (Exception Ex)
        {
            LogBox.AppendLine("Copy failed: " + Ex.Message);
        }
    }
    async Task Add()
    {
        RegBuilderProject RegBuilderProject = new();
        RegBuilderProjectData BoxData = await RegBuilderProjectDialog.ShowModal(RegBuilderProject, this);
        if (BoxData.Result)
        {
            List<RegBuilderProject> Projects = Settings.Projects.ToList();
            Projects.Add(BoxData.RegBuilderProject);
            Settings.Projects = Projects.ToArray();
            Settings.Save(SettingsFilePath);
            
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
                Settings.Save(SettingsFilePath);
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
                List<RegBuilderProject> Projects = Settings.Projects.ToList();
                Projects.Remove(RegBuilderProject);
                Settings.Projects = Projects.ToArray();
                Settings.Save(SettingsFilePath);
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
        Settings = RegBuilderSettings.Load(SettingsFilePath);
        cboProjects.ItemsSource = Settings.Projects;

        if (Settings.Projects.Length > 0)
            cboProjects.SelectedIndex = 0;
    }
    string ResolvePath(string FilePath)
        => Path.GetFullPath(FilePath, AppContext.BaseDirectory);

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

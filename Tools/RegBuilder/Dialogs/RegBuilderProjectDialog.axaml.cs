/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace RegBuilder;

/// <summary>
/// Dialog used to edit a RegBuilder project.
/// </summary>
public partial class RegBuilderProjectDialog : DialogWindow
{
    // ● private fields
    /// <summary>
    /// The dialog data.
    /// </summary>
    RegBuilderProjectData BoxData;

    // ● private
    /// <summary>
    /// Returns JSON serializer options for the outputs editor.
    /// </summary>
    JsonSerializerOptions GetJsonOptions()
    {
        JsonSerializerOptions Result = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        Result.Converters.Add(new JsonStringEnumConverter());
        return Result;
    }
    /// <summary>
    /// Returns the project outputs as formatted JSON.
    /// </summary>
    string GetOutputsText(RegBuilderProject Project)
    {
        RegBuilderOutput[] Outputs = Project.Outputs.Length > 0 ? Project.Outputs : [];
        return JsonSerializer.Serialize(Outputs, GetJsonOptions());
    }
    /// <summary>
    /// Parses and validates the outputs editor text.
    /// </summary>
    async Task<RegBuilderOutput[]> GetOutputs()
    {
        string Text = edtOutputs.GetText();
        if (string.IsNullOrWhiteSpace(Text))
        {
            await MessageBox.Error("Outputs JSON is required.", this);
            return null;
        }

        try
        {
            RegBuilderOutput[] Result = JsonSerializer.Deserialize<RegBuilderOutput[]>(Text, GetJsonOptions());
            if (Result == null || Result.Length == 0)
            {
                await MessageBox.Error("Outputs JSON must contain at least one output.", this);
                return null;
            }

            foreach (RegBuilderOutput Output in Result)
            {
                if (string.IsNullOrWhiteSpace(Output.TargetName))
                {
                    await MessageBox.Error("Each output must have a TargetName.", this);
                    return null;
                }
                if (string.IsNullOrWhiteSpace(Output.OutputFolderPath))
                {
                    await MessageBox.Error($"Output '{Output.TargetName}' must have an OutputFolderPath.", this);
                    return null;
                }
                if (Output.Artifacts == RegBuilderArtifactKind.None)
                {
                    await MessageBox.Error($"Output '{Output.TargetName}' must have at least one artifact.", this);
                    return null;
                }
            }

            return Result;
        }
        catch (Exception Ex)
        {
            await MessageBox.Error("Invalid Outputs JSON." + Environment.NewLine + Ex.Message, this);
            return null;
        }
    }
    
    // ● event handlers
    /// <summary>
    /// Handles OK and Cancel button clicks.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
    
    // ● overridables
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected override async Task WindowInitialize()
    {
        btnOK.Click += AnyClick;
        btnCancel.Click += AnyClick;
        
        BoxData = InputData as RegBuilderProjectData;
        ResultData = BoxData;

        edtName.Focus();
        
        await Task.CompletedTask;
    }
    /// <summary>
    /// Loads project values into the dialog controls.
    /// </summary>
    protected override async Task ItemToControls()
    {
        RegBuilderProject Project = BoxData.RegBuilderProject;
        
        edtName.Text = Project.Name;
        edtName.IsEnabled = string.IsNullOrWhiteSpace(edtName.Text);
        edtSchemaFilePath.Text = Project.SchemaFilePath;
        edtSchemaVersion.Value = Project.SchemaVersion;
        edtNamespaceName.Text = Project.NamespaceName;
        
        chLookup.IsChecked = DuplicateCheck.Lookup.In(Project.DuplicateChecks);
        chEnum.IsChecked = DuplicateCheck.Enum.In(Project.DuplicateChecks);
        chForm.IsChecked = DuplicateCheck.Form.In(Project.DuplicateChecks);
        chModule.IsChecked = DuplicateCheck.Module.In(Project.DuplicateChecks);
        chLocator.IsChecked = DuplicateCheck.Locator.In(Project.DuplicateChecks);
        chCodeProvider.IsChecked = DuplicateCheck.CodeProvider.In(Project.DuplicateChecks);

        string RefPathsText = string.Join(Environment.NewLine, Project.ReferenceFilePaths);
        edtReferenceFilePaths.Text = RefPathsText;
        edtOutputs.Text = GetOutputsText(Project);
        
        await Task.CompletedTask;
    }
    /// <summary>
    /// Saves dialog control values to the project.
    /// </summary>
    protected override async Task ControlsToItem()
    {
        await Task.CompletedTask;
        
        string ProjectName = edtName.GetText();
        string SchemaFilePath = edtSchemaFilePath.GetText();
        int SchemaVersion = edtSchemaVersion.Value.HasValue ? Convert.ToInt32(edtSchemaVersion.Value) : 0;
        string NamespaceName = edtNamespaceName.GetText();
        RegBuilderOutput[] Outputs = await GetOutputs();

        if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(SchemaFilePath) || SchemaVersion <= 0 || string.IsNullOrWhiteSpace(NamespaceName) || Outputs == null)
            return;
        
        RegBuilderProject Project = BoxData.RegBuilderProject;
        Project.Name = ProjectName;
        Project.SchemaFilePath = SchemaFilePath;
        Project.SchemaVersion = SchemaVersion;
        Project.NamespaceName = NamespaceName;
        Project.Outputs = Outputs;
        
        DuplicateCheck DuplicateChecks = DuplicateCheck.None;
        if (chLookup.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.Lookup;
        if (chEnum.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.Enum;
        if (chForm.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.Form;
        if (chModule.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.Module;
        if (chLocator.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.Locator;
        if (chCodeProvider.IsChecked == true)
            DuplicateChecks |= DuplicateCheck.CodeProvider;
        Project.DuplicateChecks = DuplicateChecks;

        Project.ReferenceFilePaths = edtReferenceFilePaths.GetTextAsLines();
        
        this.ModalResult = ModalResult.Ok;
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="RegBuilderProjectDialog"/> class.
    /// </summary>
    public RegBuilderProjectDialog()
    {
        InitializeComponent();
    }
    
    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="RegBuilderProject">The RegBuilder project to edit.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The dialog data.</returns>
    static public async Task<RegBuilderProjectData> ShowModal(RegBuilderProject RegBuilderProject, Control Caller = null)
    {
        RegBuilderProjectData BoxData = new() { RegBuilderProject = RegBuilderProject };
        DialogInfo Info = await  ShowModal<RegBuilderProjectDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

/// <summary>
/// Contains RegBuilder project dialog data.
/// </summary>
public class RegBuilderProjectData
{
    // ● properties
    /// <summary>
    /// Gets or sets the RegBuilder project.
    /// </summary>
    public RegBuilderProject RegBuilderProject { get; set; }  
    /// <summary>
    /// Gets the dialog information.
    /// </summary>
    public DialogInfo Info { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => Info.Result;
}

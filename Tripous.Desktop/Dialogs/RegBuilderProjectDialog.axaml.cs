/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

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

        if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(SchemaFilePath) || SchemaVersion <= 0 || string.IsNullOrWhiteSpace(NamespaceName))
            return;
        
        RegBuilderProject Project = BoxData.RegBuilderProject;
        Project.Name = ProjectName;
        Project.SchemaFilePath = SchemaFilePath;
        Project.SchemaVersion = SchemaVersion;
        Project.NamespaceName = NamespaceName;
        
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

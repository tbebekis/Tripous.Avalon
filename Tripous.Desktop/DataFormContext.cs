/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Holds the created objects and result data of a form opening operation.
/// </summary>
public class DataFormContext: FormContext
{
    // ● private
    static DataFormContext CreateCore(string FormId, string FormRegistryName, DataModule Module, Control Caller)
    {
        if (string.IsNullOrWhiteSpace(FormRegistryName))
            throw new TripousArgumentNullException(nameof(FormRegistryName));

        FormDef FormDef = DesktopRegistry.Forms.Get(FormRegistryName);
        if (string.IsNullOrWhiteSpace(FormDef.Module))
            throw new TripousDesktopException($"Form '{FormRegistryName}' has no Module.");

        ModuleDef ModuleDef = DataRegistry.Modules.Get(FormDef.Module);
        if (Module != null && !ModuleDef.Name.IsSameText(Module.ModuleDef.Name))
            throw new TripousDesktopException($"Form '{FormRegistryName}' cannot use module '{Module.ModuleDef.Name}'.");

        return new DataFormContext
        {
            FormId = FormId,
            ClassName = FormDef.ClassName,
            Caller = Caller ?? Ui.MainWindow,
            RegistryName = FormRegistryName,
            FormDef = FormDef,
            ModuleDef = ModuleDef,
            Module = Module ?? ModuleDef.Create(),
            Title = FormDef.Title,
        };
    }

    // ● static public
    static public DataFormContext Create(string FormRegistryName, Control Caller = null)
    {
        return CreateCore(FormRegistryName, FormRegistryName, null, Caller);
    }
    static public DataFormContext Create(string FormRegistryName, DataModule Module, Control Caller = null)
    {
        if (Module == null)
            throw new TripousArgumentNullException(nameof(Module));

        return CreateCore(FormRegistryName, FormRegistryName, Module, Caller);
    }
    static public DataFormContext Create(string FormId, string FormRegistryName, Control Caller = null)
    {
        return CreateCore(FormId, FormRegistryName, null, Caller);
    }
 
    public override AppForm CreateForm()
    {
        if (Form == null)
            Form = TypeStore.CreateInstance<DataForm>(ClassName);
        return Form;
    }
    static public async Task<DataFormContext> ShowFormModal(
        string FormRegistryName,
        DataFormAction StartAction = DataFormAction.List,
        object RowId = null,
        Control Caller = null)
    {
        DataFormContext Context = Create(FormRegistryName, Caller);
        Context.StartAction = StartAction;
        Context.RowId = RowId;
        return await AppFormDialog.ShowModalDataForm(Context);
    }
    
    // ● properties
    /// <summary>
    /// The form registration key.
    /// </summary>
    public string RegistryName { get; private set; }
    /// <summary>
    /// The form definition.
    /// </summary>
    public FormDef FormDef { get; private set; }
    /// <summary>
    /// The module definition
    /// </summary>
    public ModuleDef ModuleDef { get; private set; }
    /// <summary>
    /// The created module instance.
    /// </summary>
    public DataModule Module { get; private set; }
    /// <summary>
    /// The created form instance.
    /// </summary>
    public DataForm DataForm => Form as DataForm;

    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction { get; set; } = DataFormAction.List;
    /// <summary>
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions { get; set; } = DataFormAction.None;
    /// <summary>
    /// An optional row id, used mainly when the start action is Edit or Delete.
    /// </summary>
    public object RowId { get; set; }

 
}

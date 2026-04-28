namespace Tripous.Desktop;

/// <summary>
/// Holds the created objects and result data of a form opening operation.
/// </summary>
public class DataFormContext: FormContext
{
    static public DataFormContext Create(string FormRegistryName, Control Caller = null)
    {
        return DataFormContext.Create(FormRegistryName, FormRegistryName, Caller);
    }
    static public DataFormContext Create(string FormId, string FormRegistryName, Control Caller = null)
    {
        if (string.IsNullOrWhiteSpace(FormRegistryName))
            throw new ArgumentNullException(nameof(FormRegistryName));

        FormDef FormDef = UiRegistry.Forms.Get(FormRegistryName);

        if (string.IsNullOrWhiteSpace(FormDef.Module))
            throw new ApplicationException($"Form '{FormRegistryName}' has no Module.");

        ModuleDef ModuleDef = DataRegistry.Modules.Get(FormDef.Module);

        DataFormContext Result = new DataFormContext
        {
            FormId = FormId,
            ClassName = FormDef.ClassName,
            Caller = Caller?? Ui.MainWindow,
            
            RegistryName = FormRegistryName,
   
            FormDef = FormDef,
            ModuleDef = ModuleDef,
            Module = ModuleDef.Create(),
        };
 
        return Result;
    }
 
    public override AppForm CreateForm()
    {
        if (Form == null)
            Form = TypeResolver.CreateInstance<DataForm>(ClassName);
        return Form;
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

    /// <summary>
    /// Returns true if this is a fixed-list (no row insert-delete allowed) form.
    /// </summary>
    public bool IsFixedListForm { get; set; }
}
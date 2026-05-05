namespace Tripous.Desktop;

/// <summary>
/// Holds the created objects and result data of a form opening operation.
/// </summary>
public class FormContext
{
    private string fFormId;
    private string fTitle;

    static public FormContext Create(Type AppFormType, FormDisplayMode DisplayMode, Control Caller = null, object Tag = null)
    {
        if (!AppFormType.InheritsFrom(typeof(AppForm)))
            throw new TripousException($"Cannot create a {nameof(FormContext)} for a Form. The specified Form type is not a {nameof(AppForm)}.");
        return Create(AppFormType.FullName, DisplayMode, Caller, Tag);
    }
    static public FormContext Create(string ClassName, FormDisplayMode DisplayMode, Control Caller = null, object Tag = null)
    {
        return FormContext.Create(ClassName, ClassName, DisplayMode, Caller, Tag);
    }
    static public FormContext Create(string FormId, string ClassName, FormDisplayMode DisplayMode, Control Caller = null, object Tag = null)
    {
        FormContext Result = new()
        {
            FormId = FormId,
            ClassName = ClassName,
            DisplayMode = DisplayMode,
            Caller = Caller?? Ui.MainWindow,
            Tag = Tag,
        };
        return Result;
    }

    public virtual AppForm CreateForm()
    {
        if (Form == null)
            Form = TypeResolver.CreateInstance<AppForm>(ClassName);
        return Form;
    }
    
    // ● properties
    /// <summary>
    /// A string that uniquely identifies the form among all forms, e.g. Customer.12345
    /// </summary>
    public string FormId
    {
        get => !string.IsNullOrWhiteSpace(fFormId) ? fFormId : ClassName;
        set => fFormId = value;
    }
    /// <summary>
    /// The form class name, e.g. Tripous.Desktop.AppForm
    /// </summary>
    public string ClassName { get; protected set;  }
    /// <summary>
    /// Indicates how the form is displayed, i.e. TabItem  or Dialog.
    /// </summary>
    public FormDisplayMode DisplayMode { get; set;  }
    /// <summary>
    /// The caller control. Caller control is used in getting the <see cref="Window.Owner"/> when the form (i.e. the UserControl) is displayed in a modal dialog.
    /// </summary>
    public Control Caller { get; protected set; }
    /// <summary>
    /// The parent TabItem or Window.
    /// </summary>
    public ContentControl ParentControl { get; set; }
    /// <summary>
    /// The created form instance.
    /// </summary>
    public AppForm Form { get; protected set; }
    /// <summary>
    /// Optional title override.
    /// </summary>
    public string Title
    {
        get => !string.IsNullOrWhiteSpace(fTitle) ? fTitle : FormId;
        set => fTitle = value;
    }
    public ModalResult ModalResult { get; internal set; }
    public bool Result => ModalResult == ModalResult.Ok;
    /// <summary>
    /// Optional result data returned by the form.
    /// </summary>
    public object ResultData { get; set; }
    /// <summary>
    /// Optional.
    /// </summary>
    public object Options { get; set; }
    /// <summary>
    /// Optional parameter bag.
    /// </summary>
    public Dictionary<string, object> Params { get; } = new();
    /// <summary>
    /// Optional user data.
    /// </summary>
    public object Tag { get; set; }
}
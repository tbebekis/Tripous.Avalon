namespace Tripous.Desktop;

public class FormDef: BaseDef
{
    private string fClassName = typeof(DataForm).FullName;
    private string fItemPageClassName = typeof(ItemPage).FullName;
    private FormType fFormType = FormType.Master;
    private string fModule;

    public DataForm Create() => TypeResolver.CreateInstance<DataForm>(ClassName);

    // ● properties
    public string ClassName
    {
        get => fClassName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    public string ItemPageClassName
    {
        get => fItemPageClassName;
        set { if (fItemPageClassName != value) { fItemPageClassName = value; NotifyPropertyChanged(nameof(ItemPageClassName)); } }
    }
    public FormType FormType
    {
        get => fFormType;
        set { if (fFormType != value) { fFormType = value; NotifyPropertyChanged(nameof(FormType)); } }
    }
    public string Module
    {
        get => !string.IsNullOrWhiteSpace(fModule) ? fModule : Name;
        set { if (fModule != value) { fModule = value; NotifyPropertyChanged(nameof(Module)); } }
    }
}

 
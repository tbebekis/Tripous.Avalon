namespace Tripous.Desktop;
 
static public class DesktopRegistry
{
    // ● forms
    static public FormDef AddForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string ItemClassName = null, bool IsReadOnly = false)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousArgumentNullException(nameof(Name));
        if (DesktopRegistry.Forms.Contains(Name))
            throw new TripousException($"{nameof(FormDef)} '{Name}' is already registered.");
        
        FormDef Result = new();
        
        Result.Name = Name;
        Result.TitleKey = !string.IsNullOrWhiteSpace(TitleKey) ? TitleKey : Name.ToPlural();
        Result.Module = !string.IsNullOrWhiteSpace(Module) ? Module : Name;
        Result.ClassName = ClassName;
        Result.ItemClassName = ItemClassName;
        Result.IsReadOnly = IsReadOnly;
        
        DesktopRegistry.Forms.Add(Result);
        return Result;
    }
    static public FormDef AddForm(string Name, string Module) => AddForm(Name: Name, Module: Module);
    static public FormDef AddForm(string Name, string Module, string ClassName) => AddForm(Name: Name, Module: Module, ClassName: ClassName);
    static public FormDef AddForm(string Name, string Module, string ClassName, string TitleKey) => AddForm(Name: Name, Module: Module, ClassName: ClassName, TitleKey: TitleKey);
 
    // ● create form
    static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
    
    // ● properties
    static public DefList<FormDef> Forms { get; } = new();
    
}


 
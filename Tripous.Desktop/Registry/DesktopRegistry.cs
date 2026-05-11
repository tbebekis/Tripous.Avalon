namespace Tripous.Desktop;
 
static public class DesktopRegistry
{
    // ● forms
    static public FormDef AddForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousArgumentNullException(nameof(Name));
        if (DesktopRegistry.Forms.Contains(Name))
            throw new TripousException($"{nameof(FormDef)} '{Name}' is already registered.");
        
        FormDef Result = new();
        
        Result.Name = Name;
        Result.TitleKey = TitleKey;
        Result.Module = !string.IsNullOrWhiteSpace(Module) ? Module : Name;
        Result.ClassName = ClassName;
        Result.Group = Group;
        Result.ItemClassName = ItemClassName;
        Result.IsReadOnly = IsReadOnly;
        
        DesktopRegistry.Forms.Add(Result);
        return Result;
    }
    static public FormDef AddForm(string Name, string Module, string Group) => AddForm(Name: Name, Module: Module, Group: Group);
    static public FormDef AddForm(string Name, string Module, string ClassName, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, Group: Group);
    static public FormDef AddForm(string Name, string Module, string ClassName, string TitleKey, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, TitleKey: TitleKey, Group: Group);
 
    // ● create form
    static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
    
    // ● properties
    static public DefList<FormDef> Forms { get; } = new();
    
}


 
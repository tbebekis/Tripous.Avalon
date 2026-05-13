namespace Tripous.Desktop;
 
static public class DesktopRegistry
{
    // ● private
    static FormDef AddFormInternal(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false)
    {
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
    static void CheckForm(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(FormDef)}. No '{nameof(Name)}' is provided.");
        if (Forms.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(FormDef)}. '{Name}' is already registered.");
    }
    
    // ● forms
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false)
    {
        CheckForm(Name);
        FormDef Result = AddFormInternal(Name, TitleKey, Module, ClassName, Group, ItemClassName, IsReadOnly);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string Group) => AddForm(Name: Name, Module: Module, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string ClassName, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string ClassName, string TitleKey, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, TitleKey: TitleKey, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, that definition is returned.</para>
    /// </summary>
    static public FormDef AddOrGetForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false)
    {
        FormDef Result = Forms.Find(Name);
        if (Result == null)
            Result = AddFormInternal(Name, TitleKey, Module, ClassName, Group, ItemClassName, IsReadOnly);
        return Result;
    }
    
    // ● create form
    /// <summary>
    /// Creates a <see cref="DataForm"/> instance based on the name of a definition.
    /// </summary>
    static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
    
    // ● properties
    /// <summary>
    /// The list of registered forms.
    /// </summary>
    static public DefList<FormDef> Forms { get; } = new();
    
}


 
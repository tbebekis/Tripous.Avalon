namespace Tripous.Desktop;
 
static public class AvalonRegistry
{
    static public DefList<FormDef> Forms { get; } = new();
    static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
}

 
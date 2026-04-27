namespace Tripous.Models;

static public class Registry
{
    static public DefList<LookupDef> Lookups { get; } = new();
    static public DefList<LocatorDef> Locators { get; } = new();
    static public DefList<ModuleDef> Modules { get; } = new();
    static public DefList<ICommandExecutor> CommandExecutors { get; } = new();
    static public DefList<CommandDef> MenuCommands { get; } = new();
    static public DefList<CommandDef> ToolBarCommands { get; } = new();
    static public DefList<ILookupSource> LookupSources { get; } = new();

    static public DataModule CreateModule(string Name) => Modules.Get(Name).Create();
    
    //static public DefList<FormDef> Forms { get; } = new();
    //static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
}
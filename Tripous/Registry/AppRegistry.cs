namespace Tripous;

static public class AppRegistry
{
    // ●  properties 
    static public DefList<ICommandExecutor> CommandExecutors { get; } = new();
    static public DefList<CommandDef> MenuCommands { get; } = new();
    static public DefList<CommandDef> ToolBarCommands { get; } = new();
}
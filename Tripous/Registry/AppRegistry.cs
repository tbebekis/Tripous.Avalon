namespace Tripous;

static public class AppRegistry
{
    // ●  properties 
    static public DefList<Command> MenuCommands { get; } = new();
    static public DefList<Command> ToolBarCommands { get; } = new();
}
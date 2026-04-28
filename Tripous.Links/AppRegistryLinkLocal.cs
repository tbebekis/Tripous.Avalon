namespace Tripous;

/// <summary>
/// A local registry
/// </summary>
public class AppRegistryLinkLocal: AppRegistryLink
{
    // ●  construction 
    public AppRegistryLinkLocal()
    {
    }
    
    // ● public
    public override async Task Initialize()
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
            await Task.CompletedTask;
        }
    }
    
    // ● properties
    public override ReadOnlyDefList<ICommandExecutor> CommandExecutors { get; }  = new ReadOnlyDefList<ICommandExecutor>(AppRegistry.CommandExecutors);
    public override ReadOnlyDefList<CommandDef> MenuCommands { get; }  = new ReadOnlyDefList<CommandDef>(AppRegistry.MenuCommands);
    public override ReadOnlyDefList<CommandDef> ToolBarCommands { get; }  = new ReadOnlyDefList<CommandDef>(AppRegistry.ToolBarCommands);
}
namespace TestApp;

public class MainCommandExecutor: BaseDef, ICommandExecutor
{
    private MainWindow MainWindow;

    // ● construction
    public MainCommandExecutor(MainWindow MainWindow)
    {
        this.MainWindow = MainWindow;
        this.Name = "Main";
        DataRegistry.CommandExecutors.Add(this);
    }

    // ● public
    public void RegisterCommands()
    {
        CommandDef cmdFile = Commands.Add(new() { Name = "File",  Executor = "Main" });
        cmdFile.Commands.Add(new() { Name = "Exit",  Executor = "Main"});
    }
    
    public bool CanExecute(CommandDef Cmd) => true;
    public object Execute(CommandDef Cmd, object Param = null)
    {
        return null;
    }
    public async Task<object> ExecuteAsync(CommandDef Cmd, object Param = null)
    {
        await Task.CompletedTask;
        return null;
    }

    // ● properties
    public CommandList Commands { get; } = new();
}
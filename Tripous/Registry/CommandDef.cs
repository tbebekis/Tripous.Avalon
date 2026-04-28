namespace Tripous;

public class CommandDef: BaseDef
{
    private string fExecutor;
    private string fImageFileName;
    private CommandList fCommands = new();

    public bool CanExecute() => CommandExecutor != null && CommandExecutor.CanExecute(this);
    public object Execute(object Param = null) => CommandExecutor.Execute(this, Param);
    public async Task<object> ExecuteAsync(object Param = null) => await CommandExecutor.ExecuteAsync(this, Param);

    // ● properties
    [JsonIgnore]
    public ICommandExecutor CommandExecutor => AppRegistry.CommandExecutors.Get(Executor);
    public string Executor 
    {
        get => fExecutor;
        init { if (fExecutor != value) { fExecutor = value; NotifyPropertyChanged(nameof(Executor)); } }
    }
    public string ImageFileName
    {
        get => fImageFileName;
        init { if (fImageFileName != value) { fImageFileName = value; NotifyPropertyChanged(nameof(ImageFileName)); } }
    }
    public CommandList Commands
    {
        get => fCommands;
        init { if (fCommands != value) { fCommands = value; NotifyPropertyChanged(nameof(Commands)); } }
    }
}
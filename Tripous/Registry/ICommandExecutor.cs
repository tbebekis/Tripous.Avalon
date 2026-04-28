namespace Tripous;

public interface ICommandExecutor: IDef
{
    bool CanExecute(CommandDef Cmd);
    object Execute(CommandDef Cmd, object Param = null);
    Task<object> ExecuteAsync(CommandDef Cmd, object Param = null);
}
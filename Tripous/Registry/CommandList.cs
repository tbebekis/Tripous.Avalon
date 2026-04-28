namespace Tripous;

public class CommandList: DefList<CommandDef>
{
    protected override void CheckAdding(CommandDef Def)
    {
        base.CheckAdding(Def);
        if (Def.Executor == null)
            throw new ArgumentNullException(nameof(Def.Executor));
    }
}
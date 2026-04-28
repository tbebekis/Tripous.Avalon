namespace Tripous.Desktop;

public abstract class UiRegistryLink
{
    // ● construction
    public UiRegistryLink()
    {
    }

    // ● public
    public abstract Task Initialize();
    
    public abstract ReadOnlyDefList<FormDef> Forms { get; }
    public abstract DataForm CreateDataForm(string Name);
    
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
}
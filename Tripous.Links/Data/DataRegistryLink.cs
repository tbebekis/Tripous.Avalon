namespace Tripous.Data;

/// <summary>
/// A local or remote registry
/// </summary>
public abstract class DataRegistryLink
{
    // ● construction
    public DataRegistryLink()
    {
    }

    // ● public
    public abstract Task Initialize();
    public abstract DataModule CreateModule(string Name);
    
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
    public abstract ReadOnlyDefList<LookupDef> Lookups { get; }  
    public abstract ReadOnlyDefList<LocatorDef> Locators { get; }  
    public abstract ReadOnlyDefList<ModuleDef> Modules { get; } 
    public abstract ReadOnlyDefList<ILookupSource> LookupSources { get; }
}
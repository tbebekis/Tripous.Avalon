namespace Tripous.Data;

/// <summary>
/// A local registry
/// </summary>
public class DataRegistryLinkLocal: DataRegistryLink
{
    // ● construction
    public override async Task Initialize()
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
            await Task.CompletedTask;
        }
    }
    
    // ● public
    public override DataModule CreateModule(string Name) => DataRegistry.CreateModule(Name);

    // ● properties
    public override ReadOnlyDefList<LookupDef> Lookups { get; }  = new ReadOnlyDefList<LookupDef>(DataRegistry.Lookups);
    public override ReadOnlyDefList<LocatorDef> Locators { get; }  = new ReadOnlyDefList<LocatorDef>(DataRegistry.Locators);
    public override ReadOnlyDefList<ModuleDef> Modules { get; }  = new ReadOnlyDefList<ModuleDef>(DataRegistry.Modules);
    public override ReadOnlyDefList<ILookupSource> LookupSources { get; }  = new ReadOnlyDefList<ILookupSource>(DataRegistry.LookupSources);
}
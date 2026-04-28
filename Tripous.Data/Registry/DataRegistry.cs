namespace Tripous.Data;

static public class DataRegistry
{
    static public DefList<LookupDef> Lookups { get; } = new();
    static public DefList<LocatorDef> Locators { get; } = new();
    static public DefList<ModuleDef> Modules { get; } = new();

    static public DefList<LookupSource> LookupSources { get; } = new();

    static public DataModule CreateModule(string Name) => Modules.Get(Name).Create();
}


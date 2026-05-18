namespace Tripous.Data;

/// <summary>
/// Thread-safe cache of code provider entries.
/// </summary>
static public class CodeProviderEntries
{
    // ● private fields
    static readonly System.Threading.Lock syncLock = new();
    static readonly ConcurrentDictionary<string, CodeProviderEntry> Items = [];
    static CodeProviderModule fModule;
 
    // ● private
    static CodeProviderEntry Load(string CodeProviderName)
    {
        CodeProviderDef Def = DataRegistry.CodeProviders.Get(CodeProviderName);
        return Module.GetCodeProviderEntry(Def);
    }

    // ● public
    /// <summary>
    /// Returns a cached entry or null if the code provider name is empty.
    /// </summary>
    static public CodeProviderEntry Find(string CodeProviderName) => !string.IsNullOrWhiteSpace(CodeProviderName) ? Items.GetOrAdd(CodeProviderName, Load) : null;
    /// <summary>
    /// Returns a cached entry or throws an exception.
    /// </summary>
    static public CodeProviderEntry Get(string CodeProviderName)
    {
        CodeProviderEntry Result = Find(CodeProviderName);

        if (Result == null)
            throw new TripousDataException("Code provider name is empty.");

        return Result;
    }
    /// <summary>
    /// Removes a single cached entry.
    /// </summary>
    static public void Remove(string CodeProviderName)
    {
        if (string.IsNullOrWhiteSpace(CodeProviderName))
            return;

        Items.TryRemove(CodeProviderName, out _);
    }
    /// <summary>
    /// Clears all cached entries.
    /// </summary>
    static public void Clear() => Items.Clear();
    
    // ● properties
    static public CodeProviderModule Module
    {
        get
        {
            if (fModule == null)
            {
                ModuleDef ModuleDef = DataRegistry.Modules.Get(DbConfig.CodeProviderModuleName);
                fModule = ModuleDef.Create() as CodeProviderModule;

                if (fModule == null)
                    throw new TripousDataException($"{DbConfig.CodeProviderModuleName} module is not a {nameof(CodeProviderModule)}.");
            }

            return fModule;
        }
    }
 
}
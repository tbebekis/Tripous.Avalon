namespace Tripous.Data;

/// <summary>
/// Thread-safe cache of code provider entries.
/// </summary>
static public class CodeProviderEntries
{
    // ● private fields
    static readonly System.Threading.Lock syncLock = new();
    static readonly ConcurrentDictionary<string, CodeProviderEntry> Items = [];
  
    // ● private
    static CodeProviderEntry Load(string CodeProviderName)
    {
        CodeProviderDef Def = DataRegistry.CodeProviders.Get(CodeProviderName);
        string TableName = DbConfig.SysNumberSeriesTableName;
        string EntryCode = CodeProviderName;
        
        string SqlText = $"select * from {TableName} where Code = '{EntryCode}' ";
        DataRow Row = Db.DefaultStore.SelectResults(SqlText);
        if (Row == null)
            throw new TripousDataException($"{EntryCode} not found in {TableName}");
        
        CodeProviderEntry Result = new CodeProviderEntry(Row);
        return Result;
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

    /// <summary>
    /// Inserts missing code provider entries from schema patterns.
    /// Existing rows with a different pattern cause an error.
    /// </summary>
    static public void SeedPatterns(Dictionary<string, string> CodeProviderPatterns)
    {
        if (CodeProviderPatterns == null || CodeProviderPatterns.Count == 0)
            return;
        
        ModuleDef ModuleDef = DataRegistry.Modules.Get(DbConfig.CodeProviderModuleName);
        CodeProviderModule Module = ModuleDef.Create() as CodeProviderModule;

        if (Module == null)
            throw new TripousDataException($"{DbConfig.CodeProviderModuleName} module is not a {nameof(CodeProviderModule)}.");
        
        string SqlText = $"select * from {DbConfig.SysNumberSeriesTableName} where Code = :Code";

        foreach (var Pair in CodeProviderPatterns)
        {
            string Code = Pair.Key;
            string Pattern = Pair.Value;
            DataRow Row = Db.DefaultStore.SelectResults(SqlText, new Dictionary<string, object>() { ["Code"] = Code });

            if (Row != null)
            {
                string ExistingPattern = Row.AsString("Pattern");
                if (!ExistingPattern.IsSameText(Pattern))
                    throw new TripousDataException($"Code provider '{Code}' has a different stored pattern.");
                continue;
            }

            Row = Module.tblItem.NewRow();
            Module.tblItem.Rows.Add(Row); 
         
            Row["Code"] = Code;
            Row["Name"] = Code;
            Row["Pattern"] = Pattern;
            Row["ResetPeriodId"] = (int)ResetPeriod.None;
            Row["NextNumber"] = 1;
            Row["LastResetValue"] = DBNull.Value;
            Row["IsActive"] = 1;
        }

        if (Module.tblItem.Rows.Count > 0)
        {
            BatchCommitArgs BatchArgs = new(TransLimit: Module.tblItem.Rows.Count, AfterFunc: (object LastCommitedId) => false);
            Module.CommitBatch(BatchArgs);
        }
    }
 
 
}
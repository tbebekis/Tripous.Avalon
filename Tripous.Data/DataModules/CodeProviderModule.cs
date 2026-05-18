namespace Tripous.Data;

/// <summary>
/// The <see cref="CodeProviderDef"/> module
/// </summary>
public class CodeProviderModule: DataModule
{
    protected override void Commited(bool Reselect, object RowId)
    {
        base.Commited(Reselect, RowId);
        CodeProviderEntries.Clear();
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public CodeProviderModule()
    {
    }

    // ● public
    public virtual CodeProviderEntry GetCodeProviderEntry(CodeProviderDef CodeProviderDef) => GetCodeProviderEntry(CodeProviderDef.Name);
    public virtual CodeProviderEntry GetCodeProviderEntry(string CodeProviderName)
    {
        string TableName = ModuleDef.Table.Name;
        string EntryCode = CodeProviderName;
        
        string SqlText = $"select * from {TableName} where Code = '{EntryCode}' ";
        DataRow Row = Store.SelectResults(SqlText);
        if (Row == null)
            throw new TripousDataException($"{EntryCode} not found in {TableName}");
        
        CodeProviderEntry Result = new CodeProviderEntry(Row);
        return Result;
    }
    
    /// <summary>
    /// Inserts missing code provider entries from schema patterns.
    /// Existing rows with a different pattern cause an error.
    /// </summary>
    public virtual void SeedPatterns(Dictionary<string, string> CodeProviderPatterns)
    {
        if (CodeProviderPatterns == null || CodeProviderPatterns.Count == 0)
            return;
        
        string SqlText = $"select * from {DbConfig.SysNumberSeriesTableName} where Code = :Code";

        foreach (var Pair in CodeProviderPatterns)
        {
            string Code = Pair.Key;
            string Pattern = Pair.Value;
            DataRow Row = Store.SelectResults(SqlText, new Dictionary<string, object>() { ["Code"] = Code });

            if (Row != null)
            {
                string ExistingPattern = Row.AsString("Pattern");
                if (!ExistingPattern.IsSameText(Pattern))
                    throw new TripousDataException($"Code provider '{Code}' has a different stored pattern.");
                continue;
            }

            Insert();
            CurrentRow["Code"] = Code;
            CurrentRow["Name"] = Code;
            CurrentRow["Pattern"] = Pattern;
            CurrentRow["ResetPeriodId"] = (int)ResetPeriod.None;
            CurrentRow["NextNumber"] = 1;
            CurrentRow["LastResetValue"] = DBNull.Value;
            CurrentRow["IsActive"] = 1;
            Commit();
        }
    }
    
    /// <summary>
    /// Returns the next number using an atomic locked increment.
    /// Handles reset safely inside the same transaction.
    /// </summary>
    public virtual string GetNextCodeLocked(CodeProviderDef CodeProviderDef)
    {
        if (CodeProviderDef == null)
            throw new ArgumentNullException(nameof(CodeProviderDef));

        CodeProviderEntry CodeProviderEntry = CodeProviderEntries.Get(CodeProviderDef.Name);

        int Number = 1;

        string CodeProviderName = CodeProviderDef.Name;
        string ResetValue = CodeProviderEntry.GetResetValue(DateTime.Today);

        string SqlText = $"""
            update {DbConfig.SysNumberSeriesTableName}
            set NextNumber = :NextNumber,
                LastResetValue = :LastResetValue
            where Code = :Code
            """;

        using DbTransaction Transaction = Store.BeginTransaction();
        try
        {
            DataRow Row = Store.Provider.SelectForUpdate(
                Transaction,
                DbConfig.SysNumberSeriesTableName,
                "Code",
                Store.ConnectionInfo.CommandTimeoutSeconds,
                CodeProviderName);

            if (Row == null)
                throw new TripousDataException($"{CodeProviderName} not found in {DbConfig.SysNumberSeriesTableName}");

            string LastResetValue = Row.AsString("LastResetValue");
            int NextNumber = Row.AsInteger("NextNumber");

            bool RequiresReset = !string.IsNullOrWhiteSpace(ResetValue)
                                 && !LastResetValue.IsSameText(ResetValue);

            if (RequiresReset)
            {
                Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
                {
                    ["Code"] = CodeProviderName,
                    ["NextNumber"] = 2,
                    ["LastResetValue"] = ResetValue,
                });

                Transaction.Commit();
            }
            else
            {
                Number = NextNumber;
                
                Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
                {
                    ["Code"] = CodeProviderName,
                    ["NextNumber"] = NextNumber + 1,
                    ["LastResetValue"] = LastResetValue,
                });

                Transaction.Commit();
            }

            string Result = CodeProviderEntry.Format(DateTime.Today, Number);
            return Result;
        }
        catch
        {
            Transaction.Rollback();
            throw;
        }
    }
    /// <summary>
    /// Returns the next number using an atomic locked increment.
    /// </summary>
    public virtual string GetNextCodeLocked(string CodeProviderName) => GetNextCodeLocked(DataRegistry.CodeProviders.Get(CodeProviderName));
 }
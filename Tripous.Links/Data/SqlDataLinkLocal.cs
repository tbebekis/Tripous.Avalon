namespace Tripous.Data;


/// <summary>
/// A local Sql link.
/// </summary>
public class SqlDataLinkLocal: SqlDataLink
{
    private SqlStore SqlStore;
    
    // ● construction
    public SqlDataLinkLocal()
    {
    }
    
    // ● public
    public override async Task Initialize(string ConnectionName)
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
            this.ConnectionName = ConnectionName;
            SqlStore = SqlStores.CreateSqlStore(ConnectionName);
            await Task.CompletedTask;
        }
    }

    public override async Task<MemTable> Select(string SqlText, object[] Params)
    {
        await Task.CompletedTask;
        MemTable Table = SqlStore.Select(SqlText, Params);
        return Table;
    }
    public override async Task SelectTo(MemTable Table, string SqlText, object[] Params)
    {
        await Task.CompletedTask;
        SqlStore.SelectTo(Table, SqlText, Params);
    }
    
    public override async Task<DataRow> SelectResults(string SqlText, object[] Params)
    {
        await Task.CompletedTask;
        DataRow DataRow = SqlStore.SelectResults(SqlText, Params);
        return DataRow;
    }
    public override async Task<object> SelectResult(string SqlText, object Default, object[] Params)
    {
        await Task.CompletedTask;
        object Result = SqlStore.SelectResult(SqlText, Default, Params);
        return Result;
    }
    
    public override async Task<int> IntegerResult(string SqlText, int Default, object[] Params)
    {
        await Task.CompletedTask;
        int Result = SqlStore.IntegerResult(SqlText, Default, Params);
        return Result;
    }

    public override async Task<int> ExecSql(string SqlText, object[] Params)
    {
        await Task.CompletedTask;
        int Result = SqlStore.ExecSql(SqlText, Params);
        return Result;
    }
}
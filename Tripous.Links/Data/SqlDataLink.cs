namespace Tripous.Data;


/// <summary>
/// A local or remote Sql link.
/// </summary>
public abstract class SqlDataLink
{
    // ● construction
    public SqlDataLink()
    {
    }

    // ● public
    public abstract Task Initialize(string ConnectionName);

    public virtual async Task<MemTable> Select(string SqlText) => await Select(SqlText, null);
    public abstract Task<MemTable> Select(string SqlText, object[] Params);

    public virtual async Task SelectTo(MemTable Table, string SqlText) => await SelectTo(Table, SqlText, null);
    public abstract Task SelectTo(MemTable Table, string SqlText, object[] Params);   
    
    public virtual async Task<DataRow> SelectResults(string SqlText)  => await SelectResults(SqlText, null);
    public abstract Task<DataRow> SelectResults(string SqlText, object[] Params);
 
    public virtual async Task<object> SelectResult(string SqlText) => await SelectResult(SqlText, DBNull.Value);
    public virtual async Task<object> SelectResult(string SqlText, object Default) => await SelectResult(SqlText, Default, null);
    public abstract Task<object> SelectResult(string SqlText, object Default, object[] Params);
    
    public virtual async Task<int> IntegerResult(string SqlText, int Default) => await IntegerResult(SqlText, Default, null);
    public abstract Task<int> IntegerResult(string SqlText, int Default, object[] Params);
    
    public virtual async Task<int> ExecSql(string SqlText) => await ExecSql(SqlText, null);
    public abstract Task<int> ExecSql(string SqlText, object[] Params);
 
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
    public virtual string ConnectionName { get; protected set; }
}
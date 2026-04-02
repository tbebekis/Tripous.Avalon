namespace Tripous.Data;
 
public class SqlStore
{
    DbConnectionInfo ConnectionInfo;
    
    // ● construction
    public SqlStore(DbConnectionInfo ConnectionInfo)
    {
        this.ConnectionInfo = ConnectionInfo;
    }
    
    // ● public
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public DataTable Select(string SqlText, int? CommandTimeoutSeconds = null)
    {
        return Db.Select(this.ConnectionInfo, SqlText, CommandTimeoutSeconds);
    }
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string SqlText, int? CommandTimeoutSeconds = null)
    {
        return Db.ExecSql(this.ConnectionInfo, SqlText, CommandTimeoutSeconds);
    }
    
    // ● properties
    public string Name => ConnectionInfo.Name;
    public DbServerType DbType => ConnectionInfo.DbServerType;
}
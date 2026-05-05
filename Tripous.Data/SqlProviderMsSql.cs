namespace Tripous.Data;

public class SqlProviderMsSql : SqlProvider
{
    // ● constructor
    internal SqlProviderMsSql()
        : base(DbServerType.MsSql)
    {
    }

    
    // ● locking
    /// <summary>
    /// Returns a SELECT statement that locks a single row for update.
    /// </summary>
    public override string SelectForUpdateSql(string TableName, string FieldName)
    {
        return $"select * from {TableName} with (updlock, rowlock) where {FieldName} = :{FieldName}";
    }
    
    // ● miscs
    public override bool CreateDatabase(string ConnectionString)
    {
        if (CanConnect(ConnectionString))
            return false;

        string DbName = GetDatabaseName(ConnectionString);
        if (string.IsNullOrWhiteSpace(DbName))
            throw new Exception("Database name not found in connection string.");

        ConnectionStringBuilder B = CreateConnectionStringBuilder(ConnectionString);

        foreach (string Key in DatabaseKeys)
        {
            if (B.ContainsKey(Key))
            {
                B[Key] = "master";
                break;
            }
        }

        using DbConnection Con = CreateConnection(B.ConnectionString);
        Con.Open();

        using DbCommand Cmd = Con.CreateCommand();
        Cmd.CommandText = $"create database [{DbName}]";
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"Database '{DbName}' was created but is not ready.");

        return true;
    }
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;

        string Result = Regex.Replace(SqlText, @"^\s*select\s+distinct\s+", $"select distinct top {RowLimit} ", RegexOptions.IgnoreCase);
        if (!ReferenceEquals(Result, SqlText) && Result != SqlText)
            return Result;

        return Regex.Replace(SqlText, @"^\s*select\s+", $"select top {RowLimit} ", RegexOptions.IgnoreCase);
    }
    /// <summary>
    /// Concatenates two or more strings.
    /// <para>Example: SELECT FirstName || LastName As FullName FROM Customers </para>
    /// <para>Oracle, Firebird, SQLite: || </para>
    /// <para>MsSql, Access : + </para>
    /// </summary>
    public override string Concat(params string[] Parts) => string.Join(" + ", Parts);
    /// <summary>
    /// Returns the current date and time of the database server
    /// </summary>
    public override DateTime GetServerDateTime(string ConnectionString)
    {
        string SqlText = "SELECT CURRENT_TIMESTAM";
        DateTime Default = DateTime.Now.ToUniversalTime();
        object Value =  SelectResult(ConnectionString, SqlText, Default);
        DateTime Result = Convert.ToDateTime(Value);
        return Result;
    }
    
    // ● alter column
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // exec sp_rename N'TableName.ColumnName', 'NewColumnName', 'COLUMN'
        return $"exec sp_rename N'{TableName}.{ColumnName}', '{NewColumnName}', 'COLUMN'"; 
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        // alter table {TableName} alter column {ColumnName} {DataType} {Required}
        return $"alter table {TableName} alter column {ColumnName} {DataType} {Required}";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // update TableName set ColumnName = DefaultValue where ColumnName is null;
        //  alter table {TableName} alter column {ColumnName} {DataType} not null
        return $"alter table {TableName} alter column {ColumnName} {DataType} not null";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // alter table {TableName} alter column {ColumnName} {DataType} null
        return $"alter table {TableName} alter column {ColumnName} {DataType} null";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnDefaultSql(string TableName, string ColumnName, string DefaultExpression)
    {
        // alter table {TableName} add default {DefaultExpression} for {ColumnName}
        return $"alter table {TableName} add default {DefaultExpression} for {ColumnName}";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropColumnDefaultSql(string TableName, string ColumnName)
    { 
        return $@"
declare @ConstraintName nvarchar(100);

select @ConstraintName = OBJECT_NAME([default_object_id]) 
from SYS.COLUMNS
where [object_id] = OBJECT_ID('{TableName}') AND [name] = '{ColumnName}';

exec('ALTER TABLE {TableName} DROP CONSTRAINT ' +  @ConstraintName)
";
    }
    
    // ● generators  
    /// <summary>
    /// Attempts to set a generator/sequencer or identity column to Value.
    /// <para>VERY DANGEROOUS.</para>
    /// </summary>
    public override void SetTableGeneratorTo(string ConnectionString, string TableName, int Value)
    {
        string SqlText = string.Format("DBCC CHECKIDENT ({0}, RESEED, {1})", TableName, Value);
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public override int LastId(DbTransaction Transaction, string TableName)
    {
        string SqlText = $"SELECT IDENT_CURRENT('{TableName}') AS RESULT";
        
        int CommandTimeout = SysConfig.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }

    // ● properties
    public override string NativePrefix => "@";
    public override string ObjectStartDelimiter => "[";
    public override string ObjectEndDelimiter => "]";

    public override bool CanCreateDatabases => true;
    public override bool SupportsGenerators  => false;
    public override bool SupportsAutoIncFields  => true;
    public override string SuperUser => "sa";
    public override string SuperUserPassword => string.Empty;
    public override OidMode OidMode => OidMode.AutoInc;
    
    public override string[] ServerKeys => new[] { "Data Source", "Server" };
    public override string[] DatabaseKeys => new[] { "Initial Catalog", "Database" };
    public override string[] UserNameKeys => new[] { "User ID", "User Id", "Uid" };
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    public override string Description => "Microsoft SQL Server";
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    public override string LastIdSql => "select scope_identity()";
    public override string AutoIncSql => "int identity(1,1)";
    public override string VarcharSql => "varchar";
    public override string NVarcharSql => "nvarchar";
    public override string FloatSql => "float";
    public override string DecimalSql => "decimal(18, 4)";
    public override string DateSql => "date";
    public override string DateTimeSql => "datetime";
    public override string BoolSql => "int";
    public override string BlobSql => "varbinary(max)";
    public override string BlobTextSql => "varchar(max)";
    public override string NBlobTextSql => "nvarchar(max)";
}
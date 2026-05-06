namespace Tripous.Data;

public class SqlProviderMySql : SqlProvider
{
    // ● constructor
    internal SqlProviderMySql()
        : base(DbServerType.MySql)
    {
    }

    // ● miscs
    public override bool CreateDatabase(string ConnectionString)
    {
        if (CanConnect(ConnectionString))
            return false;

        ConnectionStringBuilder CSB = CreateConnectionStringBuilder(ConnectionString);
        string DatabaseName = CSB.Database;

        if (string.IsNullOrWhiteSpace(DatabaseName))
            throw new Exception("Database name not found in connection string.");

        CSB.RemoveKeys(DatabaseKeys);

        using DbConnection Con = CreateConnection(CSB.ConnectionString);
        Con.Open();

        using DbCommand Cmd = Con.CreateCommand();
        Cmd.CommandText = $"create database if not exists `{DatabaseName}`;";
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"MySql database '{DatabaseName}' was created but is not ready.");

        return true;
    }
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;
        return $"{SqlText.TrimEnd()} limit {RowLimit}";
    }
    public override string Concat(params string[] Parts) => $"concat({string.Join(", ", Parts)})";
 
    // ● alter column  
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // alter table {TableName} rename column {ColumnName} to {NewColumnName} 
        return $"alter table {TableName} rename column {ColumnName} to {NewColumnName} ";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        // alter table {TableName} modify column {ColumnName} {DataType} {Required}
        return $"alter table {TableName} modify column {ColumnName} {DataType} {Required}";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // update {TableName} set {ColumnName} = {DefaultExpression} where {ColumnName} is null; 
        // alter table {TableName} modify column {ColumnName} {DataType} not null
        return $"alter table {TableName} modify column {ColumnName} {DataType} not null";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // alter table {TableName} modify column {ColumnName} {DataType} null
        return $"alter table {TableName} modify column {ColumnName} {DataType} null";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnDefaultSql(string TableName, string ColumnName, string DefaultExpression)
    {
        // alter table {TableName} alter {ColumnName} set default {DefaultExpression}
        return $"alter table {TableName} alter {ColumnName} set default {DefaultExpression}";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropColumnDefaultSql(string TableName, string ColumnName)
    {
        // alter table {TableName} alter {ColumnName} drop default
        return $@"alter table {TableName} alter {ColumnName} drop default";
    }

    // ● constraints  
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a unique constraint
    /// </summary>
    public override string DropUniqueConstraintSql(string TableName, string ConstraintName)
    {
        return $"alter table {TableName} drop index {ConstraintName}";
    }
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a foreign key constraint
    /// </summary>
    public override string DropForeignKeySql(string TableName, string ConstraintName)
    {
        return $"alter table {TableName} drop foreign key {ConstraintName}";
    }
        
    // ● generators  
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public override int LastId(DbTransaction Transaction, string TableName)
    {
        string SqlText = $"SELECT LAST_INSERT_ID() AS RESULT;";
        
        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }
    
    
    // ● properties
    public override string NativePrefix => "@";
    public override string ObjectStartDelimiter => "`";
    public override string ObjectEndDelimiter => "`";

    public override bool CanCreateDatabases => true;
    public override bool SupportsGenerators => false;
    public override bool SupportsAutoIncFields => true;
    public override string SuperUser => "root";
    public override string SuperUserPassword => string.Empty;
    public override OidMode OidMode => OidMode.AutoInc;
    
    public override string[] ServerKeys => new[] { "Server", "Data Source" };
    public override string[] DatabaseKeys => new[] { "Database" };
    public override string[] UserNameKeys => new[] { "User Id", "User ID", "Uid" };
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    public override string LastIdSql => "select last_insert_id()";
    public override string PrimaryKeySql => "integer auto_increment not null primary key";
    public override string AutoIncSql => "integer auto_increment";
    public override string VarcharSql => "varchar";
    public override string NVarcharSql => "varchar";
    public override string FloatSql => "double";
    public override string DecimalSql => "decimal(18, 4)";
    public override string DateSql => "date";
    public override string DateTimeSql => "datetime";
    public override string BoolSql => "tinyint";
    public override string BlobSql => "longblob";
    public override string BlobTextSql => "longtext";
    public override string NBlobTextSql => "longtext";
    public override string NotNullSql => "not null";
    public override string NullSql => "null";
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// SqlProvider for Microsoft SQL Server databases.
/// </summary>
public class SqlProviderMsSql : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
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
    /// <summary>
    /// Creates a database.
    /// </summary>
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
        LogSql(Cmd);
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"Database '{DbName}' was created but is not ready.");

        return true;
    }
    /// <summary>
    /// Applies a row limit to the SqlText SELECT statement.
    /// </summary>
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
        string SqlText = "SELECT CURRENT_TIMESTAMP";
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
        
        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }

    // ● properties
    /// <summary>
    /// The prefix used for native parameters.
    /// </summary>
    public override string NativePrefix => "@";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectStartDelimiter => "[";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectEndDelimiter => "]";

    /// <summary>
    /// True if this provider can create databases.
    /// </summary>
    public override bool CanCreateDatabases => true;
    /// <summary>
    /// True if this provider supports generators.
    /// </summary>
    public override bool SupportsGenerators  => false;
    /// <summary>
    /// True if this provider supports auto-increment fields.
    /// </summary>
    public override bool SupportsAutoIncFields  => true;
    /// <summary>
    /// The super user name.
    /// </summary>
    public override string SuperUser => "sa";
    /// <summary>
    /// The super user password.
    /// </summary>
    public override string SuperUserPassword => string.Empty;
    /// <summary>
    /// The OidMode.
    /// </summary>
    public override OidMode OidMode => OidMode.AutoInc;
    
    /// <summary>
    /// A list of strings used as server name key in connection strings.
    /// </summary>
    public override string[] ServerKeys => new[] { "Data Source", "Server" };
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => new[] { "Initial Catalog", "Database" };
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => new[] { "User ID", "User Id", "Uid" };
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    /// <summary>
    /// A description of this provider.
    /// </summary>
    public override string Description => "Microsoft SQL Server";
    /// <summary>
    /// The keyword that can be used to return the current datetime in an SQL statement.
    /// </summary>
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    /// <summary>
    /// The keyword that can be used to return the last inserted id in an SQL statement.
    /// </summary>
    public override string LastIdSql => "select scope_identity()";
    
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => "int identity(1,1)";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string VarcharSql => "varchar";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NVarcharSql => "nvarchar";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string FloatSql => "float";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DecimalSql => "decimal(18, 4)";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DateSql => "date";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DateTimeSql => "datetime";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BoolSql => "int";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobSql => "varbinary(max)";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobTextSql => "varchar(max)";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NBlobTextSql => "nvarchar(max)";
}

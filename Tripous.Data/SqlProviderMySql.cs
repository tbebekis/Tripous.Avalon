/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// SqlProvider for MySql databases.
/// </summary>
public class SqlProviderMySql : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal SqlProviderMySql()
        : base(DbServerType.MySql)
    {
    }

    // ● miscs
    /// <summary>
    /// Creates a database.
    /// </summary>
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
        LogSql(Cmd);
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"MySql database '{DatabaseName}' was created but is not ready.");

        return true;
    }
    /// <summary>
    /// Applies a row limit to the SqlText.
    /// </summary>
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;
        return $"{SqlText.TrimEnd()} limit {RowLimit}";
    }
    /// <summary>
    /// Returns a concatenation of the specified parts.
    /// </summary>
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
    /// <summary>
    /// The prefix used for native parameters.
    /// </summary>
    public override string NativePrefix => "@";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectStartDelimiter => "`";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectEndDelimiter => "`";

    /// <summary>
    /// True if this provider can create databases.
    /// </summary>
    public override bool CanCreateDatabases => true;
    /// <summary>
    /// True if this provider supports generators.
    /// </summary>
    public override bool SupportsGenerators => false;
    /// <summary>
    /// True if this provider supports auto-increment fields.
    /// </summary>
    public override bool SupportsAutoIncFields => true;
    /// <summary>
    /// The super user name.
    /// </summary>
    public override string SuperUser => "root";
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
    public override string[] ServerKeys => new[] { "Server", "Data Source" };
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => new[] { "Database" };
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => new[] { "User Id", "User ID", "Uid" };
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    /// <summary>
    /// The SQL statement that returns the current server date and time.
    /// </summary>
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    /// <summary>
    /// The SQL statement that returns the last inserted id.
    /// </summary>
    public override string LastIdSql => "select last_insert_id()";
    
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string PrimaryKeySql => "integer auto_increment not null primary key";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => "integer auto_increment";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string VarcharSql => "varchar";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NVarcharSql => "varchar";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string FloatSql => "double";
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
    public override string BoolSql => "tinyint";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobSql => "longblob";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobTextSql => "longtext";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NBlobTextSql => "longtext";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NotNullSql => "not null";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NullSql => "null";
}

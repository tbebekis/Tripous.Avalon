/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// SqlProvider for PostgreSQL databases.
/// </summary>
public class SqlProviderPostgreSql : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal SqlProviderPostgreSql()
        : base(DbServerType.PostgreSql)
    {
    }

    // ● public
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

        CSB["Database"] = "postgres";

        using DbConnection Con = CreateConnection(CSB.ConnectionString);
        Con.Open();

        using DbCommand Cmd = Con.CreateCommand();
        Cmd.CommandText = $"create database \"{DatabaseName}\" encoding = 'UTF8'";
        LogSql(Cmd);
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"PostgreSql database '{DatabaseName}' was created but is not ready.");

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
    /// Returns the current date and time of the database server
    /// </summary>
    public override DateTime GetServerDateTime(string ConnectionString)
    {
        string SqlText = $"SELECT CURRENT_TIMESTAMP";
        DateTime Default = DateTime.Now.ToUniversalTime();
        object Value = SelectResult(ConnectionString, SqlText, Default);
        DateTime Result = Convert.ToDateTime(Value);
        return Result;
    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public override int LastId(DbTransaction Transaction, string TableName)
    {
        string SqlText = $"SELECT LASTVAL() AS RESULT;";
        
        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }

    // ● alter column 
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // alter table {TableName} rename column {ColumnName} to {NewColumnName} 
        return $"alter table {TableName} rename column {ColumnName} to {NewColumnName}  ";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        // alter table {TableName} alter column {ColumnName} type {DataType}  
        return $"alter table {TableName} alter column {ColumnName} type {DataType} ";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // update {TableName} set {ColumnName} = {DefaultExpression} where {ColumnName} is null; 
        // alter table {TableName} alter column {ColumnName} set not null  
        return $"alter table {TableName} alter column {ColumnName} set not null";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // alter table {TableName} alter column {ColumnName} drop not null 
        return $"alter table {TableName} alter column {ColumnName} drop not null";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnDefaultSql(string TableName, string ColumnName, string DefaultExpression)
    {
        // alter table {TableName} alter column {ColumnName} set default {DefaultExpression}
        return $"alter table {TableName} alter column {ColumnName} set default {DefaultExpression}";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropColumnDefaultSql(string TableName, string ColumnName)
    {
        // alter table {TableName} alter column {ColumnName} drop default
        return $@"alter table {TableName} alter column {ColumnName} drop default";
    }
    
    // ● generators  
    /// <summary>
    /// Returns true if the GeneratorName exists in a database.
    /// </summary>
    public override bool GeneratorExists(string ConnectionString, string GeneratorName)
    {
        string SqlText = $"SELECT count(sequence_name) FROM information_schema.sequences WHERE sequence_name = '{GeneratorName}' ;";  
        return this.IntegerResult(ConnectionString, SqlText, -1) > 0;
    }
    /// <summary>
    /// Creates the GeneratorName generator to the database.
    /// </summary>
    public override void CreateGenerator(string ConnectionString, string GeneratorName)
    {
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        string SqlText = $"CREATE SEQUENCE IF NOT EXISTS {GeneratorName} ;";
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Attempts to set a generator/sequencer to Value.
    /// <para>DANGEROOUS.</para>
    /// </summary>
    public override void SetGeneratorTo(string ConnectionString, string GeneratorName, int Value)
    {
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        string SqlText = $"SELECT setval('{GeneratorName}', {Value}) ;";
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Attempts to set a generator/sequencer or identity column to Value.
    /// <para>VERY DANGEROOUS.</para>
    /// </summary>
    public override void SetTableGeneratorTo(string ConnectionString, string TableName, int Value)
    {
        if (GeneratorExists(ConnectionString, "G_" + TableName))
            SetGeneratorTo(ConnectionString, "G_" + TableName, Value);
    }
    /// <summary>
    /// Returns the next value of the GeneratorName generator.
    /// </summary>
    public override int NextIdByGenerator(DbTransaction Transaction, string GeneratorName)
    { 
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        string SqlText = $"SELECT nextval('{GeneratorName}') ;";  
        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        return IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
    }
    
    // ● properties
    /// <summary>
    /// The prefix used for native parameters.
    /// </summary>
    public override string NativePrefix => "@";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectStartDelimiter => "\"";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectEndDelimiter => "\"";
    
    /// <summary>
    /// The connection string template.
    /// </summary>
    public override string ConnectionStringTemplate => @"Server={0}; Database={1}; User Id={2}; Password={3};";
    /// <summary>
    /// The super user name.
    /// </summary>
    public override string SuperUser => "postgres";
    /// <summary>
    /// The super user password.
    /// </summary>
    public override string SuperUserPassword => string.Empty;
    
    /// <summary>
    /// Returns true if the provider can create databases.
    /// </summary>
    public override bool CanCreateDatabases => true;
    /// <summary>
    /// True if the provider supports generators.
    /// </summary>
    public override bool SupportsGenerators => true;
    /// <summary>
    /// True if the provider supports auto-increment fields.
    /// </summary>
    public override bool SupportsAutoIncFields => true;
    /// <summary>
    /// The OidMode.
    /// </summary>
    public override OidMode OidMode => OidMode.AutoInc;
    
    /// <summary>
    /// A list of strings used as server name key in connection strings.
    /// </summary>
    public override string[] ServerKeys => new[] { "Server" };
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => new[] { "Database" };
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => new[] { "User Id" };
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => new[] { "Password" };
    
    /// <summary>
    /// The SQL used to get the current date and time of the database server.
    /// </summary>
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    /// <summary>
    /// The SQL used to get the last inserted id.
    /// </summary>
    public override string LastIdSql => "select lastval()";
    
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string PrimaryKeySql => "serial not null primary key";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => "serial";
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
    public override string FloatSql => "double precision";
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
    public override string DateTimeSql => "timestamp";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BoolSql => "integer";  
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobSql => "bytea";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobTextSql => "text";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NBlobTextSql => "text";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NotNullSql => "not null";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NullSql => " ";
}

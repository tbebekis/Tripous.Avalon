namespace Tripous.Data;

public class SqlProviderPostgreSql : SqlProvider
{
    // ● constructor
    internal SqlProviderPostgreSql()
        : base(DbServerType.PostgreSql)
    {
    }

    // ● public
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
        Cmd.ExecuteNonQuery();

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception($"PostgreSql database '{DatabaseName}' was created but is not ready.");

        return true;
    }
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
        string SqlText = $"elect CURRENT_TIMESTAMP";
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
        
        int CommandTimeout = SysConfig.DefaultCommandTimeoutSeconds;
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
        int CommandTimeout = SysConfig.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        return IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
    }
    
    // ● properties
    public override string NativePrefix => "@";
    public override string ObjectStartDelimiter => "\"";
    public override string ObjectEndDelimiter => "\"";
    
    public override string ConnectionStringTemplate => @"Server={0}; Database={1}; User Id={2}; Password={3};";
    public override string SuperUser => "postgres";
    public override string SuperUserPassword => string.Empty;
    public override bool CanCreateDatabases => true;
    public override bool SupportsGenerators => true;
    public override bool SupportsAutoIncFields => true;
    public override OidMode OidMode => OidMode.AutoInc;
    
    public override string[] ServerKeys => new[] { "Server" };
    public override string[] DatabaseKeys => new[] { "Database" };
    public override string[] UserNameKeys => new[] { "User Id" };
    public override string[] PasswordKeys => new[] { "Password" };
    
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    public override string LastIdSql => "select lastval()";
    public override string PrimaryKeySql => "serial not null primary key";
    public override string AutoIncSql => "serial";
    public override string VarcharSql => "varchar";
    public override string NVarcharSql => "varchar";
    public override string FloatSql => "double precision";
    public override string DecimalSql => "decimal(18, 4)";
    public override string DateSql => "date";
    public override string DateTimeSql => "timestamp";
    public override string BoolSql => "integer"; // "boolean";
    public override string BlobSql => "bytea";
    public override string BlobTextSql => "text";
    public override string NBlobTextSql => "text";
    public override string NotNullSql => "not null";
    public override string NullSql => " ";
}
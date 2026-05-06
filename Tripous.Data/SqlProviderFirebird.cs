namespace Tripous.Data;

public class SqlProviderFirebird : SqlProvider
{
    // ● constructor
    internal SqlProviderFirebird()
        : base(DbServerType.Firebird)
    {
    }

    // ● miscs
    public override bool CreateDatabase(string ConnectionString)
    {
        if (CanConnect(ConnectionString))
            return false;

        string CS = ConnectionStringBuilder.ReplacePathPlaceholders(ConnectionString);

        FirebirdSql.Data.FirebirdClient.FbConnection.CreateDatabase(
            CS,
            pageSize: 32768,
            forcedWrites: true,
            overwrite: false
        );

        if (!WaitUntilDatabaseReady(ConnectionString))
            throw new Exception("Firebird database was created but is not ready.");

        return true;
    }
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;

        string Result = Regex.Replace(SqlText, @"^\s*select\s+distinct\s+", $"select distinct first {RowLimit} ", RegexOptions.IgnoreCase);
        if (Result != SqlText)
            return Result;

        return Regex.Replace(SqlText, @"^\s*select\s+", $"select first {RowLimit} ", RegexOptions.IgnoreCase);
    }

    // ● alter column  
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // alter table {TableName} alter column {ColumnName} to {NewColumnName} 
        return $"alter table {TableName} alter column {ColumnName} to {NewColumnName} ";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// <para>NOTE: Firebird column size changes by using the "type" keyword, NOT a full column definition.</para>
    /// <para>Example: <code>alter table TableName alter ColumnName type varchar(100)</code> </para>
    /// </summary>
    public override string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        // ALTER TABLE t1 ALTER c1 TYPE char(90);
        // alter table {TableName} alter column {ColumnName} type {DataType} {Required}   

        return $"alter table {TableName} alter column {ColumnName} type {DataType}";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // update {TableName} set {ColumnName} = {DefaultExpression} where {ColumnName} is null; 
        // alter table {TableName} alter {ColumnName} set not null   
        return $"alter table {TableName} alter {ColumnName} set not null ";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // alter table {TableName} alter {ColumnName} drop not null 
        return $"alter table {TableName} alter {ColumnName} drop not null";
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
        string SqlText = string.Format("select count(RDB$GENERATOR_NAME) as CountResult from RDB$GENERATORS where RDB$GENERATOR_NAME = '{0}' ", GeneratorName);
        return IntegerResult(ConnectionString, SqlText, -1) > 0;
    }
    /// <summary>
    /// Creates the GeneratorName generator to the database.
    /// </summary>
    public override void CreateGenerator(string ConnectionString, string GeneratorName)
    {
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        string SqlText = "create generator " + GeneratorName;
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Attempts to set a generator/sequencer to Value.
    /// <para>DANGEROOUS.</para>
    /// </summary>
    public override void SetGeneratorTo(string ConnectionString, string GeneratorName, int Value)
    {
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

        string SqlText = string.Format("set generator {0} to {1}", GeneratorName, Value);

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
        string SqlText = $"SELECT GEN_ID({GeneratorName}, 1) as NEXT_ID FROM RDB$DATABASE";

        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        return IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
    }

    // ● properties
    public override string NativePrefix => "@";   // Firebird .NET gets @
    public override string ObjectStartDelimiter => "\"";
    public override string ObjectEndDelimiter => "\"";
 
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";

    // ● identity
    public override string LastIdSql => "select gen_id(GEN_IDENTITY, 0) from rdb$database";

    public override bool CanCreateDatabases => true;
    public override bool SupportsGenerators => true;
    public override bool SupportsAutoIncFields => true;
    public override string SuperUser => "SYSDBA";
    public override string SuperUserPassword => "masterkey";
    public override OidMode OidMode => OidMode.Generator;
    public override string[] ServerKeys => new[] { "DataSource", "Data Source", "Server" };
    public override string[] DatabaseKeys => new[] { "Database" };
    public override string[] UserNameKeys => new[] { "User", "User ID", "User Id", "Uid" };
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    // ● type mappings
    public override string PrimaryKeySql => "primary key";
    public override string AutoIncSql => "integer generated by default as identity";
    public override string VarcharSql => "varchar";
    public override string NVarcharSql => "varchar";
    public override string FloatSql => "double precision";
    public override string DecimalSql => "decimal(18, 4)";
    public override string DateSql => "date";
    public override string DateTimeSql => "timestamp";
    public override string BoolSql => "smallint";
    public override string BlobSql => "blob";
    public override string BlobTextSql => "blob sub_type text";
    public override string NBlobTextSql => "blob sub_type text";
    public override string NotNullSql => "not null";
    public override string NullSql => "null";
}
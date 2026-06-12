/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// SqlProvider for Firebird databases.
/// </summary>
public class SqlProviderFirebird : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal SqlProviderFirebird()
        : base(DbServerType.Firebird)
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
    /// <summary>
    /// Applies a row limit to the SqlText.
    /// </summary>
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
    /// <summary>
    /// The prefix used for native parameters.
    /// </summary>
    public override string NativePrefix => "@";   // Firebird .NET gets @
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectStartDelimiter => "\"";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectEndDelimiter => "\"";
 
    /// <summary>
    /// The SQL statement to get the server date and time.
    /// </summary>
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";

    // ● identity
    /// <summary>
    /// The SQL statement to get the last inserted id.
    /// </summary>
    public override string LastIdSql => "select gen_id(GEN_IDENTITY, 0) from rdb$database";

    /// <summary>
    /// True if the provider can create databases.
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
    /// The super user name.
    /// </summary>
    public override string SuperUser => "SYSDBA";
    /// <summary>
    /// The super user password.
    /// </summary>
    public override string SuperUserPassword => "masterkey";
    /// <summary>
    /// The OidMode.
    /// </summary>
    public override OidMode OidMode => OidMode.Generator;
    /// <summary>
    /// A list of strings used as server name key in connection strings.
    /// </summary>
    public override string[] ServerKeys => new[] { "DataSource", "Data Source", "Server" };
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => new[] { "Database" };
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => new[] { "User", "User ID", "User Id", "Uid" };
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => new[] { "Password", "Pwd" };

    // ● type mappings
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string PrimaryKeySql => "primary key";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => "integer generated by default as identity";
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
    public override string BoolSql => "smallint";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobSql => "blob";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobTextSql => "blob sub_type text";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NBlobTextSql => "blob sub_type text";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NotNullSql => "not null";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NullSql => "null";
}
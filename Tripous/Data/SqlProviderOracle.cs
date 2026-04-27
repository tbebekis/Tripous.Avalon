namespace Tripous.Data;

public class SqlProviderOracle : SqlProvider
{
    // ● constructor
    internal SqlProviderOracle()
        : base(DbServerType.Oracle)
    {
    }

    // ● miscs
    public override string CreateConnectionString(string Server, string Database, string UserName, string Password)
    {
        return string.Format(ConnectionStringTemplate, Server, UserName, Password);
    }
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;

        return $"select * from ({SqlText}) where rownum <= {RowLimit}";
    }
    /// <summary>
    /// Returns the current date and time of the database server
    /// </summary>
    public override DateTime GetServerDateTime(string ConnectionString)
    {
        string SqlText = $"SELECT TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') FROM Dual";
        DateTime Default = DateTime.Now.ToUniversalTime();
        object Value = SelectResult(ConnectionString, SqlText, Default);
        DateTime Result = Convert.ToDateTime(Value);
        return Result;
    }
    /// <summary>
    /// Quotes and formats a date value as a string, properly for use with an Sql statement
    /// </summary>
    public override string QSDate(DateTime Value)
    {
        // to_date('2010-12-14:09:56:53', 'YYYY-MM-DD:HH24:MI:SS')
        return string.Format("to_date('{0}', 'YYYY-MM-DD')", Value.ToString("yyyy-MM-dd"));
    }
    /// <summary>
    /// Quotes and formats a date-time value as a string, properly for use with an Sql statement
    /// </summary>
    public override string QSDateTime(DateTime Value)
    {
        // to_date('2010-12-14:09:56:53', 'YYYY-MM-DD:HH24:MI:SS')
        return string.Format("to_date('{0}', 'YYYY-MM-DD:HH24:MI:SS')", Value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
    
    // ● alter column 
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // alter table {TableName} rename column {ColumnName} to {NewColumnName}
        return $"alter table {TableName} rename column {ColumnName} to {NewColumnName}";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        // alter table {TableName} modify {ColumnName} {DataType} {Required}
        return $"alter table {TableName} modify {ColumnName} {DataType} {Required}";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // update {TableName} set {ColumnName} = {DefaultExpression} where {ColumnName} is null; 
        // alter table {TableName} modify {ColumnName} {DataType} not null
        return $"alter table {TableName} modify {ColumnName} {DataType} not null";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        // alter table {TableName} modify {ColumnName} {DataType} null
        return $"alter table {TableName} modify {ColumnName} {DataType} null";
    }

    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string SetColumnDefaultSql(string TableName, string ColumnName, string DefaultExpression)
    {
        // alter table {TableName} modify {ColumnName} default {DefaultExpression}
        return $"alter table {TableName} modify {ColumnName} default {DefaultExpression}";
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
        string SqlText = string.Format("select count(SEQUENCE_NAME) as CountResult from ALL_SEQUENCES where SEQUENCE_NAME = '{0}' ", GeneratorName);
        return this.IntegerResult(ConnectionString, SqlText, -1) > 0;
    }
    /// <summary>
    /// Creates the GeneratorName generator to the database.
    /// </summary>
    public override void CreateGenerator(string ConnectionString, string GeneratorName)
    {
        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        string SqlText = "CREATE SEQUENCE " + GeneratorName;
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Attempts to set a generator/sequencer to Value.
    /// <para>DANGEROOUS.</para>
    /// </summary>
    public override void SetGeneratorTo(string ConnectionString, string GeneratorName, int Value)
    {
        /* see: 
               http://asktom.oracle.com/pls/asktom/f?p=100:11:0::::P11_QUESTION_ID:1119633817597
               http://stackoverflow.com/questions/51470/how-do-i-reset-a-sequence-in-oracle
        */

        GeneratorName = GeneratorName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);


        /* always to zero */
        /* get the current value */
        string SqlText = string.Format("select {0}.NEXTVAL from DUAL", GeneratorName);
        int OldValue = this.IntegerResult(ConnectionString, SqlText, -1);

        /* subtract it  */
        if (OldValue > 0)
        {
            SqlText = string.Format("alter sequence {0} increment by -{1}  minvalue 0", GeneratorName, OldValue);
            this.ExecSql(ConnectionString, SqlText);


            /* select again */
            SqlText = string.Format("select {0}.NEXTVAL from DUAL", GeneratorName);
            Value = this.IntegerResult(ConnectionString, SqlText, -1);
        }


        /* reset it */
        SqlText = string.Format("alter sequence {0} increment by {1} minvalue 0", GeneratorName, Value);
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
        string SqlText = $"select {GeneratorName}.NEXTVAL from DUAL";

        int CommandTimeout = SysConfig.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        return IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
    }
    
 
    
    // ● properties
    public override string NativePrefix => ":";
    public override string ObjectStartDelimiter => "\"";
    public override string ObjectEndDelimiter => "\"";
    public override string Description => "Oracle";

    public override string SuperUser => "sysdba";
    public override string SuperUserPassword => "oracle";
    
    public override bool CanCreateDatabases => false;
    public override bool SupportsGenerators => true;
    public override bool SupportsAutoIncFields => false;
    
    public override OidMode OidMode => OidMode.Generator;
    
    public override string[] ServerKeys => new[] { "Data Source" };
    public override string[] DatabaseKeys => Array.Empty<string>();
    public override string[] UserNameKeys => new[] { "User Id" };
    public override string[] PasswordKeys => new[] { "Password" };
    
    public override string ServerDateTimeSql => "SYSDATE";
    public override string LastIdSql => string.Empty;
    public override string PrimaryKeySql => "integer not null primary key";
    public override string AutoIncSql => throw new NotSupportedException("Auto-increment fields are not supported by Oracle. Use sequence instead.");
    public override string VarcharSql => "varchar2";
    public override string NVarcharSql => "nvarchar2";
    public override string FloatSql => "float";
    public override string DecimalSql => "decimal(18, 4)";
    public override string DateSql => "date";
    public override string DateTimeSql => "timestamp";
    public override string BoolSql => "integer";
    public override string BlobSql => "blob";
    public override string BlobTextSql => "clob";
    public override string NBlobTextSql => "nclob";
    public override string NotNullSql => "not null";
    public override string NullSql => " ";
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */
namespace Tripous.Data;


/// <summary>
/// SqlProvider for Oracle databases.
/// </summary>
public class SqlProviderOracle : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal SqlProviderOracle()
        : base(DbServerType.Oracle)
    {
    }

    // ● miscs
    /// <summary>
    /// Creates a connection string
    /// </summary>
    public override string CreateConnectionString(string Server, string Database, string UserName, string Password)
    {
        return string.Format(ConnectionStringTemplate, Server, UserName, Password);
    }
    /// <summary>
    /// Applies a row limit to the SqlText
    /// </summary>
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

        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        return IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
    }
    
 
    
    // ● properties
    /// <summary>
    /// The Oracle prefix for native parameters.
    /// </summary>
    public override string NativePrefix => ":";
    /// <summary>
    /// The Oracle delimiter for object names.
    /// </summary>
    public override string ObjectStartDelimiter => "\"";
    /// <summary>
    /// The Oracle delimiter for object names.
    /// </summary>
    public override string ObjectEndDelimiter => "\"";
    /// <summary>
    /// The description of this sql provider
    /// </summary>
    public override string Description => "Oracle";

    /// <summary>
    /// The Oracle super user name.
    /// </summary>
    public override string SuperUser => "sysdba";
    /// <summary>
    /// The Oracle super user password.
    /// </summary>
    public override string SuperUserPassword => "oracle";
    
    /// <summary>
    /// True if this provider can create databases.
    /// </summary>
    public override bool CanCreateDatabases => false;
    /// <summary>
    /// True if this provider supports generators.
    /// </summary>
    public override bool SupportsGenerators => true;
    /// <summary>
    /// True if this provider supports auto-increment fields.
    /// </summary>
    public override bool SupportsAutoIncFields => false;
    
    /// <summary>
    /// The OID mode this provider supports.
    /// </summary>
    public override OidMode OidMode => OidMode.Generator;
    
    /// <summary>
    /// A list of strings used as server name key in connection strings.
    /// </summary>
    public override string[] ServerKeys => new[] { "Data Source" };
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => Array.Empty<string>();
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => new[] { "User Id" };
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => new[] { "Password" };
    
    /// <summary>
    /// The keyword that can be used to return the current datetime in an SQL statement.
    /// </summary>
    public override string ServerDateTimeSql => "SYSDATE";
    /// <summary>
    /// The keyword that can be used to return the last inserted id in an SQL statement.
    /// </summary>
    public override string LastIdSql => string.Empty;
    
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string PrimaryKeySql => "integer not null primary key";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => throw new NotSupportedException("Auto-increment fields are not supported by Oracle. Use sequence instead.");
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string VarcharSql => "varchar2";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NVarcharSql => "nvarchar2";
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
    public override string DateTimeSql => "timestamp";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BoolSql => "integer";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobSql => "blob";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string BlobTextSql => "clob";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NBlobTextSql => "nclob";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NotNullSql => "not null";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string NullSql => " ";
}
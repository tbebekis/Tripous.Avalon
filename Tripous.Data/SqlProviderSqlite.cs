/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */
namespace Tripous.Data;

/// <summary>
/// SqlProvider for SQLite3 databases.
/// </summary>
public class SqlProviderSqlite : SqlProvider
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal SqlProviderSqlite()
        : base(DbServerType.Sqlite)
    {
    }
    
    // ● locking
    /// <summary>
    /// Returns a SELECT statement used inside a transaction to select a row for update.
    /// </summary>
    public override string SelectForUpdateSql(string TableName, string FieldName)
    {
        return $"select * from {TableName} where {FieldName} = :{FieldName}";
    }

    // ● miscs
    /// <summary>
    /// Returns true if the database represented by the specified database exists, by checking the connection.
    /// </summary>
    public override bool DatabaseExists(string ConnectionString)
    {
        ConnectionStringBuilder CSB = new ConnectionStringBuilder(ConnectionString);
        string FilePath = CSB.Database;
        FilePath = ConnectionStringBuilder.ReplacePathPlaceholders(FilePath);
        return !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);
    }
    /// <summary>
    /// Creates the database represented by the specified connection string.
    /// </summary>
    public override bool CreateDatabase(string ConnectionString)
    {
        /*
        string NormalizeFilePath(string FilePath)
        {
            string FileName = Path.GetFileName(FilePath);
            if (FileName == FilePath) // in case where FilePath is only a FileName
                FilePath = Path.Combine(SysConfig.AppDataFolderPath, FileName);
            return FilePath;
        }
        */
        
        ConnectionStringBuilder CSB = CreateConnectionStringBuilder(ConnectionString);
        string FilePath = ConnectionStringBuilder.ReplacePathPlaceholders(CSB.Database);

        //FilePath = NormalizeFilePath(FilePath);

        if (string.IsNullOrWhiteSpace(FilePath))
            throw new Exception("SQLite database file path not found in connection string.");

        if (File.Exists(FilePath))
            return false;

        string Folder = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(Folder) && !Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        System.Data.SQLite.SQLiteConnection.CreateFile(FilePath);
        System.Data.SQLite.SQLiteConnection.ClearAllPools();

        return true;
    }
    /// <summary>
    /// Applies a row limit to the specified SQL text.
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
    public override string Concat(params string[] Parts) => string.Join(" || ", Parts);
    /// <summary>
    /// Creates a connection string based on the specified server, database, user name and password.
    /// </summary>
    public override string CreateConnectionString(string Server, string Database, string UserName, string Password)
    {
        return string.Format(ConnectionStringTemplate, Database);
    }
    /// <summary>
    /// Normalizes the specified connection string.
    /// </summary>
    public override string NormalizeConnectionString(string ConnectionString)
    {
        List<DbConProp> PropList = ConnectionStringAdapter.Parse(ConnectionString);
        DbConProp Prop = PropList.Get(DbConPropType.Database);
        
        string FilePath = Prop.Value;
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new TripousDataException($"{ServerType}: No Database Path in ConnectionString.");
        FilePath = ConnectionStringBuilder.ReplacePathPlaceholders(FilePath);
        FilePath = FilePath.QuotePath();
        Prop.Value = FilePath;

        ConnectionString = ConnectionStringAdapter.Construct(PropList);
        return ConnectionString;
    }

    // ● alter column 
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public override string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        // alter table {TableName} rename column {ColumnName} to {NewColumnName}   
        return $"alter table {TableName} rename column {ColumnName} to {NewColumnName}    ";
    }

    // ● constraints  
    /// <summary>
    /// Returns an "alter table" SQL statement for adding a unique constraint
    /// </summary>
    public override string AddUniqueConstraintSql(string TableName, string ColumnName, string ConstraintName)
    {
        return $"alter table {TableName} add constraint {ConstraintName} unique ({ColumnName})";
    }
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a unique constraint
    /// </summary>
    public override string DropUniqueConstraintSql(string TableName, string ConstraintName)
    {
        return $"drop index {ConstraintName}";
    }

    /// <summary>
    /// Returns an "alter table" SQL statement for adding a foreign key constraint
    /// </summary>
    public override string AddForeignKeySql(string TableName, string ColumnName, string ForeignTableName, string ForeignColumnName, string ConstraintName)
    {
        throw new NotSupportedException("adding a foreign key is not supported");
    }
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a foreign key constraint
    /// </summary>
    public override string DropForeignKeySql(string TableName, string ConstraintName)
    {
        throw new NotSupportedException("dropping a foreign key is not supported");
    }
    
    // ● generators  
    /// <summary>
    /// Attempts to set a generator/sequencer or identity column to Value.
    /// <para>VERY DANGEROOUS.</para>
    /// </summary>
    public override void SetTableGeneratorTo(string ConnectionString, string TableName, int Value)
    {
        string SqlText = string.Format("update sqlite_sequence set seq = {0} where name = '{1}'", Value, TableName);
        this.ExecSql(ConnectionString, SqlText);
    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public override int LastId(DbTransaction Transaction, string TableName)
    {
        string SqlText = $"select seq AS RESULT from sqlite_sequence where name = '{TableName}' ";
        
        int CommandTimeout = Db.Settings.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }
 
    // ● properties
    /// <summary>
    /// The prefix used in parameter names.
    /// </summary>
    public override string NativePrefix => ":";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectStartDelimiter => "\"";
    /// <summary>
    /// The delimiter used to quote object names.
    /// </summary>
    public override string ObjectEndDelimiter => "\"";
    /// <summary>
    /// The description of this sql provider
    /// </summary>
    public override string Description => "SQLite3";
 
    /// <summary>
    /// The keyword that can be used to return the current datetime in an SQL statement.
    /// </summary>
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    /// <summary>
    /// The keyword that can be used to return the last inserted id in an SQL statement.
    /// </summary>
    public override string LastIdSql => "select last_insert_rowid()";
    /// <summary>
    /// True if the provider can create databases.
    /// </summary>
    public override bool CanCreateDatabases => true;
    /// <summary>
    /// True if the provider supports generators.
    /// </summary>
    public override bool SupportsGenerators => false;
    /// <summary>
    /// True if the provider supports auto-increment fields.
    /// </summary>
    public override bool SupportsAutoIncFields => true;
    /// <summary>
    /// The OID mode this provider supports.
    /// </summary>
    public override OidMode OidMode => OidMode.AutoInc;
    
    /// <summary>
    /// A list of strings used as server name key in connection strings.
    /// </summary>
    public override string[] ServerKeys => Array.Empty<string>();
    /// <summary>
    /// A list of strings used as database name key in connection strings.
    /// </summary>
    public override string[] DatabaseKeys => new[] { "Data Source" };
    /// <summary>
    /// A list of strings used as user name key in connection strings.
    /// </summary>
    public override string[] UserNameKeys => Array.Empty<string>();
    /// <summary>
    /// A list of strings used as password key in connection strings.
    /// </summary>
    public override string[] PasswordKeys => Array.Empty<string>();
    
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string PrimaryKeySql => "integer not null primary key autoincrement";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string AutoIncSql => "integer autoincrement";
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
    public override string FloatSql => "real";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DecimalSql => "real";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DateSql => "datetime";
    /// <summary>
    /// Keyword, used in replacing a placeholder
    /// </summary>
    public override string DateTimeSql => "datetime";
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
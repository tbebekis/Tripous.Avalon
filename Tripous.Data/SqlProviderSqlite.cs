namespace Tripous.Data;

public class SqlProviderSqlite : SqlProvider
{
    // ● constructor
    internal SqlProviderSqlite()
        : base(DbServerType.Sqlite)
    {
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
    public override string ApplyRowLimit(string SqlText, int RowLimit)
    {
        RowLimit = NormalizeRowLimit(RowLimit);
        if (RowLimit <= 0)
            return SqlText;

        return $"{SqlText.TrimEnd()} limit {RowLimit}";
    }
    public override string Concat(params string[] Parts) => string.Join(" || ", Parts);
    public override string CreateConnectionString(string Server, string Database, string UserName, string Password)
    {
        return string.Format(ConnectionStringTemplate, Database);
    }

    public override string NormalizeConnectionString(string ConnectionString)
    {
        List<DbConProp> PropList = ConnectionStringAdapter.Parse(ConnectionString);
        DbConProp Prop = PropList.Get(DbConPropType.Database);
        
        string FilePath = Prop.Value;
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ApplicationException($"{ServerType}: No Database Path in ConnectionString.");
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
        
        int CommandTimeout = SysConfig.DefaultCommandTimeoutSeconds;
        int Default = -1;
        object[] Params = null;
        
        int Result = IntegerResult(Transaction, SqlText, CommandTimeout, Default, Params);
        return Result;
    }
 
    // ● properties
    public override string NativePrefix => ":";
    public override string ObjectStartDelimiter => "\"";
    public override string ObjectEndDelimiter => "\"";
    public override string Description => "SQLite3";
 
    public override string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    public override string LastIdSql => "select last_insert_rowid()";
    public override bool CanCreateDatabases => true;
    public override bool SupportsGenerators => false;
    public override bool SupportsAutoIncFields => true;
    public override OidMode OidMode => OidMode.AutoInc;
    
    public override string[] ServerKeys => Array.Empty<string>();
    public override string[] DatabaseKeys => new[] { "Data Source" };
    public override string[] UserNameKeys => Array.Empty<string>();
    public override string[] PasswordKeys => Array.Empty<string>();
    
    public override string PrimaryKeySql => "integer not null primary key autoincrement";
    public override string AutoIncSql => "integer autoincrement";
    public override string VarcharSql => "varchar";
    public override string NVarcharSql => "nvarchar";
    public override string FloatSql => "real";
    public override string DecimalSql => "real";
    public override string DateSql => "datetime";
    public override string DateTimeSql => "datetime";
    public override string BoolSql => "integer";
    public override string BlobSql => "blob";
    public override string BlobTextSql => "text";
    public override string NBlobTextSql => "text";
    public override string NotNullSql => "not null";
    public override string NullSql => " ";
}
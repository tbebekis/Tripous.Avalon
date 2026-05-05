using K4os.Compression.LZ4.Internal;

namespace Tripous.Data;

public abstract class SqlProvider
{
    // ● protected fields
    public const string GlobalPrefix = ":";
 
    // ● protected
    protected virtual string ReplaceDecimalPlaceholders(string SqlText)
    {
        string Pattern = Regex.Escape(SqlTypeTokens.CDECIMAL_) + @"\s*\(([^)]*)\)";
        return Regex.Replace(SqlText, Pattern, M => DecimalWithArgsSql(M.Groups[1].Value), RegexOptions.IgnoreCase);
    }
    protected virtual Dictionary<string, SqlParam> CreateParamMap(SqlParams Params)
    {
        Dictionary<string, SqlParam> Result = new Dictionary<string, SqlParam>(StringComparer.OrdinalIgnoreCase);
        foreach (SqlParam Param in Params.Items)
            Result[Param.Name] = Param;
        return Result;
    }
    protected virtual SqlParam FindParam(Dictionary<string, SqlParam> Map, string Name, string SqlText)
    {
        if (Map.TryGetValue(Name, out SqlParam Result))
            return Result;
        throw new Exception($"Sql parameter not found: {Name}");
    }     
    protected virtual string GetNativeParameterName(string Name)
    {
        return NativePrefix + Name;
    }
    protected virtual string GetSqlParameterToken(string Name)
    {
        return PositionalParameters ? "?" : GetNativeParameterName(Name);
    }
    protected virtual DbParameter CreateDbParameter(DbCommand Command, SqlParam Param)
    {
        DbParameter Result = Command.CreateParameter();
        Result.ParameterName = GetNativeParameterName(Param.Name);
        Result.Value = Param.Value == null ? DBNull.Value : Param.Value;
        return Result;
    }
    protected virtual void PrepareCommand(DbCommand Command, string SqlText, SqlParams Params)
    {
        SqlText = ReplaceDataTypePlaceholders(SqlText);
        List<SqlParamRef> Refs = SqlParamScanner.Scan(SqlText);
        if (Refs.Count == 0)
        {
            Command.CommandText = SqlText;
            return;
        }

        Dictionary<string, SqlParam> Map = CreateParamMap(Params);
        StringBuilder SB = new StringBuilder();
        HashSet<string> Added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int Pos = 0;

        foreach (SqlParamRef Ref in Refs)
        {
            SB.Append(SqlText, Pos, Ref.Index - Pos);
            SB.Append(GetSqlParameterToken(Ref.Name));
            Pos = Ref.Index + Ref.Name.Length + 1;

            SqlParam Param = FindParam(Map, Ref.Name, SqlText);
            if (PositionalParameters || !Added.Contains(Param.Name))
            {
                Command.Parameters.Add(CreateDbParameter(Command, Param));
                Added.Add(Param.Name);
            }
        }

        SB.Append(SqlText, Pos, SqlText.Length - Pos);
        Command.CommandText = SB.ToString();
    }
    protected virtual string DecimalWithArgsSql(string Args)
    {
        return $"decimal({Args})";
    }
 
    // ● constructor
    internal SqlProvider(DbServerType ServerType)
    {
        this.ServerType = ServerType;
        ConnectionStringAdapter = DbConAdapters.Get(this.ServerType);
    }

    // ● public
    public virtual SqlParams CreateSqlParams(string SqlText, params object[] Params)
    {
        SqlParams Result = new SqlParams();
        List<SqlParamRef> Refs = SqlParamScanner.Scan(SqlText);

        if (Refs.Count == 0 || Params == null || Params.Length == 0)
            return Result;

        if (Params.Length == 1)
        {
            object Source = Params[0];

            if (Source is SqlParams SqlParams)
                return SqlParams;

            if (Source is DataRow Row)
            {
                foreach (SqlParamRef Ref in Refs)
                    Result.Add(Ref.Name, Row.Table.Columns.Contains(Ref.Name) ? Row[Ref.Name] : DBNull.Value);
                return Result;
            }

            if (Source is IDictionary Dictionary)
            {
                foreach (SqlParamRef Ref in Refs)
                    Result.Add(Ref.Name, Dictionary.Contains(Ref.Name) ? Dictionary[Ref.Name] : DBNull.Value);
                return Result;
            }

            if (Source is IList List && Source is not string)
            {
                for (int i = 0; i < Refs.Count; i++)
                    Result.Add(Refs[i].Name, i < List.Count ? List[i] : DBNull.Value);
                return Result;
            }
        }

        for (int i = 0; i < Refs.Count; i++)
            Result.Add(Refs[i].Name, i < Params.Length ? Params[i] : DBNull.Value);

        return Result;
    }
    public virtual bool WaitUntilDatabaseReady(string ConnectionString, int RetryCount = 10, int DelayMilliseconds = 1000)
    {
        for (int i = 0; i < RetryCount; i++)
        {
            if (CanConnect(ConnectionString))
                return true;

            Thread.Sleep(DelayMilliseconds);
        }

        return false;
    }
 
    public virtual string ReplaceDataTypePlaceholders(string SqlText)
    {
        string Result = ReplaceDecimalPlaceholders(SqlText);
        Result = Result.Replace(SqlTypeTokens.CPRIMARY_KEY, PrimaryKeySql);
        Result = Result.Replace(SqlTypeTokens.CAUTO_INC, AutoIncSql);
        Result = Result.Replace(SqlTypeTokens.CVARCHAR, VarcharSql);
        Result = Result.Replace(SqlTypeTokens.CNVARCHAR, NVarcharSql);
        Result = Result.Replace(SqlTypeTokens.CFLOAT, FloatSql);
        Result = Result.Replace(SqlTypeTokens.CDECIMAL, DecimalSql);
        Result = Result.Replace(SqlTypeTokens.CDATE_TIME, DateTimeSql);
        Result = Result.Replace(SqlTypeTokens.CDATE, DateSql);
        Result = Result.Replace(SqlTypeTokens.CBOOL, BoolSql);
        Result = Result.Replace(SqlTypeTokens.CBLOB_TEXT, BlobTextSql);
        Result = Result.Replace(SqlTypeTokens.CNBLOB_TEXT, NBlobTextSql);
        Result = Result.Replace(SqlTypeTokens.CBLOB, BlobSql);
        Result = Result.Replace(SqlTypeTokens.CNOT_NULL, NotNullSql);
        Result = Result.Replace(SqlTypeTokens.CNULL, NullSql);
        return Result;
    }
 
    public virtual ConnectionStringBuilder CreateConnectionStringBuilder(string ConnectionString)
    {
        return new ConnectionStringBuilder(ConnectionString);
    }
    public virtual string GetDatabaseName(string ConnectionString)
    {
        ConnectionStringBuilder Builder = CreateConnectionStringBuilder(ConnectionString);
        return Builder.GetFirst(DatabaseKeys);
    }
    
    // ● connection
    public DbConnection CreateConnection(string ConnectionString)
    {
        DbConnection Result = Factory.CreateConnection();
        ConnectionString = NormalizeConnectionString(ConnectionString);
        Result.ConnectionString = ConnectionString;
        return Result;
    }
    public DbCommand CreateCommand(DbConnection Connection, string SqlText, params object[] Params)
    {
        SqlParams SqlParams = CreateSqlParams(SqlText, Params);
        return CreateCommand(Connection, SqlText, SqlParams);
    }
    public DbCommand CreateCommand(DbConnection Connection, string SqlText, SqlParams Params)
    {
        DbCommand Result = Connection.CreateCommand();
        PrepareCommand(Result, SqlText, Params);
        return Result;
    }
 
    /// <summary>
    /// Returns true if this connection info is valid and can connect to a database.
    /// </summary>
    public bool CanConnect(string ConnectionString, bool ThrowIfNot = false)
    {
        try
        {
            using (DbConnection Con = CreateConnection(ConnectionString))
                Con.Open();
            return true;
        }
        catch
        {
            if (ThrowIfNot)
                throw;
            return false;
        }
    }
    /// <summary>
    /// Returns true if the database exists.
    /// </summary>
    public virtual bool DatabaseExists(string ConnectionString) => CanConnect(ConnectionString, false);
    /// <summary>
    /// Ensures that a connection can be done by opening and closing the connection.
    /// </summary>
    public virtual void EnsureConnection(string ConnectionString) => CanConnect(ConnectionString, true);

    /// <summary>
    /// Creates and opens a DbConnection
    /// </summary>
    public virtual DbConnection OpenConnection(DbConnectionInfo ConnectionInfo) => OpenConnection(ConnectionInfo.ConnectionString);
    /// <summary>
    /// Creates and opens a DbConnection
    /// </summary>
    public virtual DbConnection OpenConnection(string ConnectionString)
    {
        DbConnection Result = CreateConnection(ConnectionString);
        Result.Open();
        return Result;
    }
    /// <summary>
    /// Creates a DbConnection, opens the connection and begins a transaction.
    /// Returns the transaction.
    /// </summary>
    public virtual DbTransaction BeginTransaction(DbConnectionInfo ConnectionInfo) => BeginTransaction(ConnectionInfo.ConnectionString);
    /// <summary>
    /// Creates a DbConnection, opens the connection and begins a transaction.
    /// Returns the transaction.
    /// </summary>
    public virtual DbTransaction BeginTransaction(string ConnectionString)
    {
        DbConnection Con = OpenConnection(ConnectionString);
        return Con.BeginTransaction();
    }

    public virtual string NormalizeConnectionString(string ConnectionString) => ConnectionString;
    public virtual string CreateConnectionString(string ServerName, string DatabaseName, string UserName, string Password)
    {
        return string.Format(ConnectionStringTemplate, ServerName, DatabaseName, UserName, Password);
    }
    public virtual bool CreateDatabase(string ConnectionString)
    {
        return false;
    }
    
    // ● Select
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(DbConnectionInfo ConnectionInfo, string SqlText) => Select(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.CommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(DbConnectionInfo ConnectionInfo, string SqlText, params object[] Params) => Select(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
    
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(string ConnectionString, string SqlText) => Select(ConnectionString, SqlText, SysConfig.DefaultCommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(string ConnectionString, string SqlText, params object[] Params) => Select(ConnectionString, SqlText, SysConfig.DefaultCommandTimeoutSeconds, Params);
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(string ConnectionString, string SqlText, int CommandTimeout, params object[] Params)
    {
        using (DbConnection Connection = CreateConnection(ConnectionString))
        {
            Connection.Open();

            using (DbTransaction Transaction = Connection.BeginTransaction())
            {
                try
                {
                    MemTable Table = new MemTable();
                    SelectTo(Transaction, Table, SqlText, CommandTimeout, Params);
                    Transaction.Commit();
                    return Table;
                }
                catch
                {
                    Transaction.Rollback();
                    throw;
                }
            }
        }
    }
    
    /// <summary>
    /// Executes a SELECT statement inside a transaction and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(DbTransaction Transaction, string SqlText, int CommandTimeout, params object[] Params)
    {
        MemTable Table = new MemTable();
        SelectTo(Transaction, Table, SqlText, CommandTimeout, Params);
        return Table;
    }
    
    // ● SelectTo
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(DbConnectionInfo ConnectionInfo, MemTable Table, string SqlText) => SelectTo(ConnectionInfo.ConnectionString, Table, SqlText, ConnectionInfo.CommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(DbConnectionInfo ConnectionInfo, MemTable Table, string SqlText, params object[] Params) => SelectTo(ConnectionInfo.ConnectionString, Table, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
 
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(string ConnectionString, MemTable Table, string SqlText, params object[] Params) => SelectTo(ConnectionString, Table, SqlText, SysConfig.DefaultCommandTimeoutSeconds, Params);
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(string ConnectionString, MemTable Table, string SqlText, int CommandTimeout, params object[] Params)
    {
        using (DbConnection Connection = CreateConnection(ConnectionString))
        {
            Connection.Open();

            using (DbCommand Command = CreateCommand(Connection, SqlText, Params))
            {
                Command.CommandTimeout = CommandTimeout;

                using (DbDataReader Reader = Command.ExecuteReader())
                {
                    Table.BeginLoadData();
                    try
                    {
                        Table.Clear(); 
                        Table.Load(Reader);
                    }
                    finally
                    {
                        Table.EndLoadData();
                    }
                    
                    Table.AcceptChanges();
                    //Table.CaseSensitive = false;
                    //Table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    return Table.Rows.Count;
                }
            }
        }
    }
    
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable using a transaction.
    /// </summary>
    public int SelectTo(DbTransaction Transaction, MemTable Table, string SqlText, int CommandTimeout, params object[] Params)
    {
        using (DbCommand Command = CreateCommand(Transaction.Connection, SqlText, Params))
        {
            Command.Transaction = Transaction;
            Command.CommandTimeout = CommandTimeout;

            using (DbDataReader Reader = Command.ExecuteReader())
            {
                Table.BeginLoadData();
                try
                {
                    Table.Clear(); 
                    Table.Load(Reader);
                }
                finally
                {
                    Table.EndLoadData();
                }
                
                Table.AcceptChanges();
                //Table.CaseSensitive = false;
                //Table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                return Table.Rows.Count;
            }
        }
    }
 
    // ● ExecSql
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(DbConnectionInfo ConnectionInfo, string SqlText) => ExecSql(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.CommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(DbConnectionInfo ConnectionInfo, string SqlText, params object[] Params) => ExecSql(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);

    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string ConnectionString, string SqlText) => ExecSql(ConnectionString, SqlText, SysConfig.DefaultCommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string ConnectionString, string SqlText, params object[] Params) => ExecSql(ConnectionString, SqlText, SysConfig.DefaultCommandTimeoutSeconds, Params);
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string ConnectionString, string SqlText, int CommandTimeout, params object[] Params)
    {
        using (DbConnection Connection = CreateConnection(ConnectionString))
        {
            Connection.Open();

            using (DbCommand Command = CreateCommand(Connection, SqlText, Params))
            {
                Command.CommandTimeout = CommandTimeout;

                int rowsAffected = Command.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }

    /// <summary>
    /// Executes a list of executable statements inside a single transaction
    /// </summary>
    public virtual void ExecSql(DbConnectionInfo ConnectionInfo, IEnumerable<string> SqlTextList) => ExecSql(ConnectionInfo.ConnectionString, SqlTextList);
    /// <summary>
    /// Executes a list of executable statements inside a single transaction
    /// </summary>
    public virtual void ExecSql(string ConnectionString, IEnumerable<string> SqlTextList)
    {
        using (DbTransaction transaction = this.BeginTransaction(ConnectionString))
        {
            try
            {
                foreach (string SqlText in SqlTextList)
                    ExecSql(transaction, SqlText);
 
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    
    /// <summary>
    /// Executes a single SQL operation inside a transaction.
    /// </summary>
    public int ExecSql(DbTransaction Transaction, string SqlText) => ExecSql(Transaction, SqlText, SysConfig.DefaultCommandTimeoutSeconds, null);
    /// <summary>
    /// Executes a single SQL operation inside a transaction.
    /// </summary>
    public int ExecSql(DbTransaction Transaction, string SqlText, params object[] Params) => ExecSql( Transaction, SqlText, SysConfig.DefaultCommandTimeoutSeconds, Params);
    /// <summary>
    /// Executes a single SQL operation inside a transaction.
    /// </summary>
    public int ExecSql(DbTransaction Transaction, string SqlText, int CommandTimeout, params object[] Params)
    {
        using (DbCommand Command = CreateCommand(Transaction.Connection, SqlText, Params))
        {
            Command.Transaction = Transaction;
            Command.CommandTimeout = CommandTimeout;
            return Command.ExecuteNonQuery();
        }
    }
    
    // ● ExecSql with callbacks
    /// <summary>
    /// It can be used in executin multiple SQL operations inside a single transaction using a callback.
    /// <para>Example</para>
    /// <code>
    /// Provider.ExecSql(ConnectionString,
    ///     tr => Provider.ExecSql(tr, Sql1, Params1),
    ///     tr => Provider.ExecSql(tr, Sql2, Params2),
    ///     tr => Provider.ExecSql(tr, Sql3, Params3)
    /// );
    /// </code>
    /// </summary>
    public void ExecSql(DbConnectionInfo ConnectionInfo, Action<DbTransaction> Action) => ExecSql(ConnectionInfo.ConnectionString, Action);
    /// <summary>
    /// It can be used in executin multiple SQL operations inside a single transaction using a callback.
    /// <para>Example</para>
    /// <code>
    /// Provider.ExecSql(ConnectionString,
    ///     tr => Provider.ExecSql(tr, Sql1, Params1),
    ///     tr => Provider.ExecSql(tr, Sql2, Params2),
    ///     tr => Provider.ExecSql(tr, Sql3, Params3)
    /// );
    /// </code>
    /// </summary>
    public void ExecSql(string ConnectionString, Action<DbTransaction> Action)
    {
        using (DbConnection Connection = CreateConnection(ConnectionString))
        {
            Connection.Open();

            using (DbTransaction Transaction = Connection.BeginTransaction())
            {
                try
                {
                    Action(Transaction);
                    Transaction.Commit();
                }
                catch
                {
                    Transaction.Rollback();
                    throw;
                }
            }
        }
    }

    // ● miscs Select
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// </summary>
    public DataRow SelectResults(string ConnectionString, string SqlText) => SelectResults(ConnectionString, SqlText, null);
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// <para></para>
    /// <para>Params can be: </para>
    /// <para>1. either a comma separated C# params list</para>
    /// <para>2. or the Params[0] element, that is the first element in Params, may be a DataRow, generic IDictionary, IList or Array
    /// and in that case no other Params elements are used.</para>
    /// </summary>
    public DataRow SelectResults(string ConnectionString, string SqlText, params object[] Params)
    {
        DataTable Table = Select(ConnectionString, SqlText, Params);
        if (Table.Rows.Count > 0)
            return Table.Rows[0];
        else
            return null;
    }
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// <para></para>
    /// <para>Params can be: </para>
    /// <para>1. either a comma separated C# params list</para>
    /// <para>2. or the Params[0] element, that is the first element in Params, may be a DataRow, generic IDictionary, IList or Array
    /// and in that case no other Params elements are used.</para>
    /// </summary>
    public DataRow SelectResults(DbTransaction Transaction, string SqlText, int CommandTimeout, params object[] Params)
    {
        DataTable Table = Select(Transaction, SqlText, CommandTimeout, Params);
        if (Table.Rows.Count > 0)
            return Table.Rows[0];
        else
            return null;
    }

    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string ConnectionString, string SqlText) => SelectResult(ConnectionString, SqlText, DBNull.Value);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string ConnectionString, string SqlText, object Default) => SelectResult(ConnectionString, SqlText, Default, null);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string ConnectionString, string SqlText, object Default, params object[] Params)
    {
        object Result = Default;

        DataRow Row = SelectResults(ConnectionString, SqlText, Params);
        if ((Row != null) && !Row.IsNull(0))
        {
            Result = Row[0];
        }

        return Result;
    }
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(DbTransaction Transaction, string SqlText, int CommandTimeout, object Default, params object[] Params)
    {
        object Result = Default;

        DataRow Row = SelectResults(Transaction, SqlText, CommandTimeout, Params);
        if ((Row != null) && !Row.IsNull(0))
        {
            Result = Row[0];
        }

        return Result;
    }

    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(string ConnectionString, string SqlText, int Default) => IntegerResult(ConnectionString, SqlText, Default, null);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(string ConnectionString, string SqlText, int Default, params object[] Params)
    {
        string S = SelectResult(ConnectionString, SqlText, Default, Params).ToString();
        return S.ToIntOrDefault(Default);
    }
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(DbTransaction Transaction, string SqlText, int CommandTimeout, int Default, params object[] Params)
    {
        string S = SelectResult(Transaction, SqlText, CommandTimeout, Default, Params).ToString();
        return S.ToIntOrDefault(Default);
    }

 
    // ● locking
    /// <summary>
    /// Returns a SELECT statement that locks a single row for update.
    /// </summary>
    public virtual string SelectForUpdateSql(string TableName, string FieldName)
    {
        return $"select * from {TableName} where {FieldName} = :{FieldName} for update";
    }
    /// <summary>
    /// Selects and locks a single row for update inside a transaction.
    /// </summary>
    public DataRow SelectForUpdate(DbTransaction Transaction, string TableName, string FieldName, object FieldValue)
    {
        return SelectForUpdate(Transaction, TableName, FieldName, SysConfig.DefaultCommandTimeoutSeconds, FieldValue);
    }
    /// <summary>
    /// Selects and locks a single row for update inside a transaction.
    /// </summary>
    public DataRow SelectForUpdate(DbTransaction Transaction, string TableName, string FieldName, int CommandTimeout, object FieldValue)
    {
        DataRow Result = null;
        string SqlText = SelectForUpdateSql(TableName, FieldName);

        DataTable Table = new();
        using (DbCommand Command = CreateCommand(Transaction.Connection, SqlText, FieldValue))
        {
            Command.Transaction = Transaction;
            Command.CommandTimeout = CommandTimeout;

            using (DbDataReader Reader = Command.ExecuteReader())
            {
                Table.BeginLoadData();
                try
                {
                    Table.Clear();
                    Table.Load(Reader);
                }
                finally
                {
                    Table.EndLoadData();
                }

                Table.AcceptChanges();
            }
        }

        if (Table.Rows.Count > 0)
            Result = Table.Rows[0];

        return Result;
    }
    /// <summary>
    /// Selects a row with update lock, increments an integer field, and returns the initial row.
    /// </summary>
    public virtual DataRow SelectIncrementLocked(DbConnectionInfo ConnectionInfo, string TableName, string KeyFieldName, object KeyValue, string ValueFieldName)
    {
        return SelectIncrementLocked(ConnectionInfo.ConnectionString, TableName, KeyFieldName, KeyValue, ValueFieldName, ConnectionInfo.CommandTimeoutSeconds);
    }
    /// <summary>
    /// Selects a row with update lock, increments an integer field, and returns the initial row.
    /// </summary>
    public virtual DataRow SelectIncrementLocked(string ConnectionString, string TableName, string KeyFieldName, object KeyValue, string ValueFieldName, int CommandTimeout)
    {
        using (DbTransaction Transaction = BeginTransaction(ConnectionString))
        {
            try
            {
                DataRow Result = SelectIncrementLocked(Transaction, TableName, KeyFieldName, KeyValue, ValueFieldName, CommandTimeout);
                Transaction.Commit();
                return Result;
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
        }
    }
    /// <summary>
    /// Selects a row with update lock, increments an integer field, and returns the initial row.
    /// </summary>
    public virtual DataRow SelectIncrementLocked(DbTransaction Transaction, string TableName, string KeyFieldName, object KeyValue, string ValueFieldName, int CommandTimeout)
    {
        DataRow Result = SelectForUpdate(Transaction, TableName, KeyFieldName, CommandTimeout, KeyValue);

        if (Result == null)
            throw new Exception($"Row not found. Table: {TableName}, Field: {KeyFieldName}, Value: {KeyValue}");

        int CurrentValue = Convert.ToInt32(Result[ValueFieldName]);

        string SqlText = $"update {TableName} set {ValueFieldName} = {ValueFieldName} + 1 where {KeyFieldName} = :{KeyFieldName}";

        using (DbCommand Command = CreateCommand(Transaction.Connection, SqlText, KeyValue))
        {
            Command.Transaction = Transaction;
            Command.CommandTimeout = CommandTimeout;
            Command.ExecuteNonQuery();
        }

        Result[ValueFieldName] = CurrentValue;
        Result.AcceptChanges();

        return Result;
    }
 
 
    
    // ● miscs
    public virtual string ApplyRowLimit(string SqlText, int RowLimit)
    {
        return SqlText;
    }
    /// <summary>
    /// Normalizes RowLimit. If it is less than zero it returns a default limit value.
    /// </summary>
    public int NormalizeRowLimit(int RowLimit)
    {
        if (RowLimit < 0)
            RowLimit = SysConfig.DefaultRowLimit;
        return RowLimit;
    }
    public virtual string QuoteName(string Name) => ObjectStartDelimiter + Name + ObjectEndDelimiter;
    /// <summary>
    /// Concatenates two or more strings.
    /// <para>Example: SELECT FirstName || LastName As FullName FROM Customers </para>
    /// <para>Oracle, Firebird, SQLite: || </para>
    /// <para>MsSql, Access : + </para>
    /// </summary>
    public virtual string Concat(params string[] Parts) => string.Join(" || ", Parts);
    /// <summary>
    /// Returns the current date and time of the database server
    /// </summary>
    public virtual DateTime GetServerDateTime(DbConnectionInfo ConnectionInfo) => GetServerDateTime(ConnectionInfo.ConnectionString);
    /// <summary>
    /// Returns the current date and time of the database server
    /// </summary>
    public virtual DateTime GetServerDateTime(string ConnectionString) => System.DateTime.Now.ToUniversalTime();
    /// <summary>
    /// Quotes and formats a date value as a string, properly for use with an Sql statement
    /// </summary>
    public virtual string QSDate(DateTime Value) => Value.ToString("yyyy-MM-dd").QS();
    /// <summary>
    /// Quotes and formats a date-time value as a string, properly for use with an Sql statement
    /// </summary>
    public virtual string QSDateTime(DateTime Value) => Value.ToString("yyyy-MM-dd HH:mm:ss").QS();
 
    // ● alter column  
    /// <summary>
    /// Returns true if this provider supports a specified <see cref="AlterTableType"/>
    /// </summary>
    public virtual bool SupportsAlterTableType(AlterTableType AlterType)
    {
        return AlterType.In(SupportedAlterTableTypes);
    }
    
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string AddColumnSql(string TableName, string ColumnDef)
    {
        // alter table TableName add ColumnName ColumnDef 
        return $"alter table {TableName} add {ColumnDef}";
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string DropColumnSql(string TableName, string ColumnName)
    {
        // alter table TableName drop ColumnName
        return $"alter table {TableName} drop {ColumnName}";
    }
    
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string RenameColumnSql(string TableName, string ColumnName, string NewColumnName)
    {
        throw new NotSupportedException("rename column not supported");
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string SetColumnLengthSql(string TableName, string ColumnName, string DataType, string Required, string DefaultExpression)
    {
        throw new NotSupportedException("altering column length not supported");
    }

    /// <summary>
    /// Returns an "UPDATE" statement for setting the default value to a column when it is null, i.e. where ColumnName is null.
    /// <para>To be used before setting a "not null" constraint to a column.</para>
    /// </summary>
    public virtual string SetDefaultBeforeNotNullUpdateSql(string TableName, string ColumnName, string DefaultExpression, bool IsString)
    {
        return $"update {TableName} set {ColumnName} = {DefaultExpression} where {ColumnName} is null";
    }
   
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string SetNotNullSql(string TableName, string ColumnName, string DataType)
    {
        throw new NotSupportedException("setting column to not null not supported");
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string DropNotNullSql(string TableName, string ColumnName, string DataType)
    {
        throw new NotSupportedException("dropping column not null not supported");
    }
    
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string SetColumnDefaultSql(string TableName, string ColumnName, string DefaultExpression)
    {
        throw new NotSupportedException("setting column default expression not supported");
    }
    /// <summary>
    /// Returns an "alter column" SQL statement.
    /// </summary>
    public virtual string DropColumnDefaultSql(string TableName, string ColumnName)
    {
        throw new NotSupportedException("dropping column default expression not supported");
    }

    // ● constraints  
    /// <summary>
    /// Returns an "alter table" SQL statement for adding a unique constraint
    /// </summary>
    public virtual string AddUniqueConstraintSql(string TableName, string ColumnName, string ConstraintName)
    {
        return $"alter table {TableName} add constraint {ConstraintName} unique ({ColumnName})";
    }
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a unique constraint
    /// </summary>
    public virtual string DropUniqueConstraintSql(string TableName, string ConstraintName)
    {
        return $"alter table {TableName} drop constraint {ConstraintName}";
    }

    /// <summary>
    /// Returns an "alter table" SQL statement for adding a foreign key constraint
    /// </summary>
    public virtual string AddForeignKeySql(string TableName, string ColumnName, string ForeignTableName, string ForeignColumnName, string ConstraintName)
    {
        return $"alter table {TableName} add constraint {ConstraintName} foreign key ({ColumnName}) references {ForeignTableName} ({ForeignColumnName})";
    }
    /// <summary>
    /// Returns an "alter table" SQL statement for dropping a foreign key constraint
    /// </summary>
    public virtual string DropForeignKeySql(string TableName, string ConstraintName)
    {
        return $"alter table {TableName} drop constraint {ConstraintName}";
    }

    // ● generators  
    /// <summary>
    /// Returns true if the GeneratorName exists in a database.
    /// </summary>
    public virtual bool GeneratorExists(string ConnectionString, string GeneratorName)
    {
        return false;
    }
    /// <summary>
    /// Creates the GeneratorName generator to the database.
    /// </summary>
    public virtual void CreateGenerator(string ConnectionString, string GeneratorName)
    {
    }
    /// <summary>
    /// Attempts to set a generator/sequencer to Value.
    /// <para>DANGEROOUS.</para>
    /// </summary>
    public virtual void SetGeneratorTo(string ConnectionString, string GeneratorName, int Value)
    {
    }
    /// <summary>
    /// Attempts to set a generator/sequencer or identity column to Value.
    /// <para>VERY DANGEROOUS.</para>
    /// </summary>
    public virtual void SetTableGeneratorTo(string ConnectionString, string TableName, int Value)
    {
    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public virtual int LastId(DbTransaction Transaction, string TableName)
    {
        return -1;
    }
    /// <summary>
    /// Returns the next value of the GeneratorName generator.
    /// </summary>
    public virtual int NextIdByGenerator(DbTransaction Transaction, string GeneratorName)
    {
        return -1;
    }

    // ● properties
    public string Name => ServerType.ToString();
    public DbProviderFactory Factory => ServerType.GetFactory();
    public DbServerType ServerType { get; }
    public DbConAdapter ConnectionStringAdapter { get; }
    public abstract string NativePrefix { get; }
    public virtual bool PositionalParameters => false;
    public virtual string ObjectStartDelimiter => "\"";
    public virtual string ObjectEndDelimiter => "\"";
    public virtual string Description => Name;
    public virtual string ConnectionStringTemplate => ServerType.GetTemplateConnectionString();
    
    // ● properties
    public virtual bool SupportsTransactions => true;
    public virtual bool CanCreateDatabases => false;
    public virtual bool SupportsGenerators => false;
    public virtual bool SupportsAutoIncFields => true;
    /// <summary>
    /// Returns a set (bit-field) of the supported <see cref="AlterTableType"/>s.
    /// </summary>
    public virtual AlterTableType SupportedAlterTableTypes =>  AlterTableType.All;
    
    
    public virtual string SuperUser => string.Empty;
    public virtual string SuperUserPassword => string.Empty;
    public virtual OidMode OidMode => OidMode.None;
    public virtual string[] ServerKeys => Array.Empty<string>();
    public virtual string[] DatabaseKeys => Array.Empty<string>();
    public virtual string[] UserNameKeys => Array.Empty<string>();
    public virtual string[] PasswordKeys => Array.Empty<string>();

    public virtual string ServerDateTimeSql => "CURRENT_TIMESTAMP";
    public virtual string LastIdSql => string.Empty;
    public virtual string PrimaryKeySql => "primary key";
    public virtual string AutoIncSql => "integer generated by default as identity";
    public virtual string VarcharSql => "varchar";
    public virtual string NVarcharSql => "nvarchar";
    public virtual string FloatSql => "float";
    public virtual string DecimalSql => "decimal(18, 4)";
    public virtual string DateSql => "date";
    public virtual string DateTimeSql => "timestamp";
    public virtual string BoolSql => "integer";
    public virtual string BlobSql => "blob";
    public virtual string BlobTextSql => "text";
    public virtual string NBlobTextSql => "text";
    public virtual string NotNullSql => "not null";
    public virtual string NullSql => "null";
}
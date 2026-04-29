namespace Tripous.Data;
/// <summary>
/// Sql store. Used in executing SELECT, INSERT, UPDATE, DELETE, etc commands, using a DbConnection.
/// <para>
/// Many methods use a Params parameter.
/// That Params can be: <br />
/// a. either a comma separated list of parameters or <br />
/// b. the Params[0] element, that is the first element in Params, may be 
/// <list type="number">
/// <item>a <see cref="DataRow"/></item>
/// <item>a generic <see cref="IDictionary" />&lt;string, object&gt;</item>
/// <item>or an <see cref="IList"/> or <see cref="Array"/></item>
/// </list>
/// and in this second case no other Params elements are used
/// </para>
/// </summary>
public class SqlStore
{
    // ● constructor
    public SqlStore(DbConnectionInfo ConnectionInfo)
    {
        this.Provider = ConnectionInfo.GetSqlProvider();
        this.ConnectionInfo = ConnectionInfo;
    }

    // ● connection
    /// <summary>
    /// Creates and opens a DbConnection
    /// </summary>
    public DbConnection OpenConnection() => Provider.OpenConnection(ConnectionInfo);
    /// <summary>
    /// Creates a DbConnection, opens the connection and begins a transaction.
    /// Returns the transaction.
    /// </summary>
    public virtual DbTransaction BeginTransaction() => Provider.BeginTransaction(ConnectionInfo);
    
    /// <summary>
    /// Returns true if this connection info is valid and can connect to a database.
    /// </summary>
    public bool CanConnect(bool ThrowIfNot = false) => Provider.CanConnect(ConnectionInfo.ConnectionString, ThrowIfNot);
    /// <summary>
    /// Ensures that a connection can be done by opening and closing the connection.
    /// </summary>
    public virtual void EnsureConnection(string ConnectionString) => Provider.EnsureConnection(ConnectionString);
 
    // ● Select
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(string SqlText) => Provider.Select(ConnectionInfo, SqlText);
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(string SqlText, params object[] Params) => Provider.Select(ConnectionInfo, SqlText,  Params);
    /// <summary>
    /// Executes a SELECT statement inside a transaction and returns the result as a DataTable.
    /// </summary>
    public MemTable Select(DbTransaction Transaction, string SqlText, params object[] Params) => Provider.Select(Transaction, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);

    // ● SelectTo
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(MemTable Table, string SqlText) => Provider.SelectTo(ConnectionInfo, Table, SqlText);
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable.
    /// </summary>
    public int SelectTo(MemTable Table, string SqlText, params object[] Params) => Provider.SelectTo(ConnectionInfo, Table, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
    /// <summary>
    /// Executes a SELECT statement and loads the result into the specified DataTable using a transaction.
    /// </summary>
    public int SelectTo(DbTransaction Transaction, MemTable Table, string SqlText, params object[] Params) => Provider.SelectTo(Transaction, Table, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
 
    // ● ExecSql
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string SqlText) => Provider.ExecSql(ConnectionInfo, SqlText);
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    public int ExecSql(string SqlText, params object[] Params) => Provider.ExecSql(ConnectionInfo, SqlText,  Params);

    /// <summary>
    /// Executes a list of executable statements inside a single transaction
    /// </summary>
    public virtual void ExecSql(IEnumerable<string> SqlTextList) => Provider.ExecSql(ConnectionInfo, SqlTextList);
    
    /// <summary>
    /// Executes a single SQL operation inside a transaction.
    /// </summary>
    public int ExecSql(DbTransaction Transaction, string SqlText) => Provider.ExecSql(Transaction, SqlText);
    /// <summary>
    /// Executes a single SQL operation inside a transaction.
    /// </summary>
    public int ExecSql(DbTransaction Transaction, string SqlText, params object[] Params) => Provider.ExecSql(Transaction, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
    
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
    public void ExecSql(Action<DbTransaction> Action) => Provider.ExecSql(ConnectionInfo, Action);

    // ● miscs Select
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// </summary>
    public DataRow SelectResults(string SqlText) => Provider.SelectResults(ConnectionInfo.ConnectionString, SqlText);
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// <para></para>
    /// <para>Params can be: </para>
    /// <para>1. either a comma separated C# params list</para>
    /// <para>2. or the Params[0] element, that is the first element in Params, may be a DataRow, generic IDictionary, IList or Array
    /// and in that case no other Params elements are used.</para>
    /// </summary>
    public DataRow SelectResults(string SqlText, params object[] Params) => Provider.SelectResults(ConnectionInfo.ConnectionString, SqlText, Params);
    /// <summary>
    /// Executes SqlText and returns the first DataRow of the result set.
    /// <para>WARNING: If SqlText returns no rows at all then this method returns null.</para>
    /// <para></para>
    /// <para>Params can be: </para>
    /// <para>1. either a comma separated C# params list</para>
    /// <para>2. or the Params[0] element, that is the first element in Params, may be a DataRow, generic IDictionary, IList or Array
    /// and in that case no other Params elements are used.</para>
    /// </summary>
    public DataRow SelectResults(DbTransaction Transaction, string SqlText, params object[] Params) => Provider.SelectResults(Transaction, SqlText, ConnectionInfo.CommandTimeoutSeconds, Params);
 
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string SqlText) => Provider.SelectResult(ConnectionInfo.ConnectionString, SqlText);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string SqlText, object Default) => Provider.SelectResult(ConnectionInfo.ConnectionString, SqlText, Default);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(string SqlText, object Default, params object[] Params) => Provider.SelectResult(ConnectionInfo.ConnectionString, SqlText, Default, Params);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select FIELD_NAME from TABLE_NAME"
    /// </summary>
    public object SelectResult(DbTransaction Transaction, string SqlText, object Default, params object[] Params) => Provider.SelectResult(Transaction, SqlText, ConnectionInfo.CommandTimeoutSeconds, Default, Params);
 
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(string SqlText, int Default) => Provider.IntegerResult(ConnectionInfo.ConnectionString, SqlText, Default);
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(string SqlText, int Default, params object[] Params) => Provider.IntegerResult(ConnectionInfo.ConnectionString, SqlText, Default, Params);
 
    /// <summary>
    /// Ideal for executing SELECT statements of the type "select count(ID) as COUNT_ID from TABLE_NAME where ID = 1234"
    /// </summary>
    public int IntegerResult(DbTransaction Transaction, string SqlText, int Default, params object[] Params)=> Provider.IntegerResult(Transaction, SqlText, ConnectionInfo.CommandTimeoutSeconds, Default, Params);
 
    public DateTime GetServerDateTime() => Provider.GetServerDateTime(ConnectionInfo);
    
    // ● id generation - Non Transactioned  
    /// <summary>
    /// Returns the next id value of a generator named after the TableName table.
    /// <para>It should be used only with databases that support generators or when a CustomOid object is used.</para>
    /// </summary>
    public virtual int NextId(DbTransaction Transaction, string TableName)
    {
       return NextIdByGenerator(Transaction, "G_" + TableName);
    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public virtual int LastId(DbTransaction Transaction, string TableName)
    {
        return Provider.LastId(Transaction, TableName);
    }
    /// <summary>
    /// Returns the next value of the GeneratorName generator.
    /// </summary>
    public virtual int NextIdByGenerator(DbTransaction Transaction, string GeneratorName)
    {
        return Provider.NextIdByGenerator(Transaction, GeneratorName);
    }

    // ● id generation - Transactioned  
    /// <summary>
    /// Returns the next id value of a generator named after the TableName table.
    /// <para>It should be used only with databases that support generators or when a CustomOid object is used.</para>
    /// </summary>
    public virtual  int NextId(string TableName)
    {

        int Result;

        using (DbTransaction Transaction = BeginTransaction())
        {
            try
            {
                Result = NextId(Transaction, TableName);
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
        }

        return Result;

    }
    /// <summary>
    /// Returns the last id produced by an INSERT Sqlt statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    public virtual int LastId(string TableName)
    {

        int Result;

        using (DbTransaction Transaction = BeginTransaction())
        {
            try
            {
                Result = LastId(Transaction, TableName);
                Transaction.Commit();
            }
            catch 
            {
                Transaction.Rollback();
                throw;
            }
        }

        return Result;

    }
    /// <summary>
    /// Returns the next value of the GeneratorName generator.
    /// </summary>
    public virtual int NextIdByGenerator(string GeneratorName)
    {

        int Result = -1;

        using (DbTransaction Transaction = BeginTransaction())
        {
            try
            {
                Result = NextIdByGenerator(Transaction, GeneratorName);
                Transaction.Commit();
            }
            catch 
            {
                Transaction.Rollback();
                throw;
            }
        }

        return Result;
    }
    
    // ● metadata related  
    /// <summary>
    /// Returns schema information for the data source of this System.Data.Common.DbConnection.
    /// </summary>
    public virtual DataTable GetSchema()
    {
        using (DbConnection Con = OpenConnection())
            return Con.GetSchema();
    }
    /// <summary>
    /// Returns schema information for the data source of this System.Data.Common.DbConnection
    /// using the specified string for the schema name.
    /// </summary>
    public virtual DataTable GetSchema(string collectionName)
    {
        using (DbConnection Con = OpenConnection())
            return Con.GetSchema(collectionName);
    }
    /// <summary>
    /// Returns schema information for the data source of this System.Data.Common.DbConnection
    /// using the specified string for the schema name and the specified string array
    /// for the restriction values.
    /// </summary>
    public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
    {
        using (DbConnection Con = OpenConnection())
            return Con.GetSchema(collectionName, restrictionValues);
    }

    public DataTable GetNativeSchemaFromTableName(string StatementName, string TableName)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            throw new ApplicationException($"Cannot get table schema. No table name defined");
        string SqlText = $"select * from {TableName}";
        return GetNativeSchemaFromSelect(StatementName, SqlText);
    }
    public DataTable GetNativeSchemaFromSelect(string StatementName, string SqlText)
    {
        if (SqlCache.Contains(this.ConnectionInfo.Name, StatementName))
        {
            return SqlCache.Find(this.ConnectionInfo.Name, StatementName);
        }
        else
        {
            SqlText = $"select * from ({SqlText}) X where 1=0";
            using (DbConnection Con = this.OpenConnection())
            {
                using (DbCommand Cmd = Con.CreateCommand())
                {
                    Cmd.CommandText = SqlText;
 
                    using (DbDataReader Reader = Cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        DataTable Table = Reader.GetSchemaTable();
                        SqlCache.Add(this.ConnectionInfo.Name, StatementName, Table);
                        return Table;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Returns a string list with the table names in the database.
    /// </summary>
    public List<string> GetTableNames()
    {
        DataTable Table = GetSchema("Tables");
        List<string> Result = new List<string>();

        foreach (DataRow Row in Table.Rows)
            Result.Add(Row["TABLE_NAME"].ToString());

        return Result;
    }
    /// <summary>
    /// Returns a string list with the field names of the TableName
    /// </summary>
    public List<string> GetFieldNames(string TableName)
    {
        DataTable Table = GetSchema("Columns");
        List<string> Result = new List<string>();

        foreach (DataRow Row in Table.Rows)
        {
            if (Sys.IsSameText(Row["TABLE_NAME"].ToString(), TableName))
                Result.Add(Row["COLUMN_NAME"].ToString());
        }

        return Result;
    }
    /// <summary>
    /// Returns a string list with the index names in the database
    /// </summary>
    public List<string> GetIndexNames()
    {
        DataTable Table = GetSchema("Indexes");
        List<string> Result = new List<string>();

        foreach (DataRow Row in Table.Rows)
        {
            string Name = Row["INDEX_NAME"].ToString();
            if (!string.IsNullOrWhiteSpace(Name) && !Result.Contains(Name))
                Result.Add(Name);
        }

        return Result;
    }

    /// <summary>
    /// Returns true if a table with TableName exists in the database.
    /// </summary>
    public bool TableExists(string TableName, bool UseSelect = false)
    {
        if (UseSelect)
        {
            string SqlText = string.Format("select count(*) as RESULT from {0}", TableName);
            try
            {
                IntegerResult(SqlText, 0);
                return true;
            }
            catch
            {                    
            }

            return false;

        }
        else
        {
            IList<string> List = GetTableNames();
            return List.ContainsText(TableName);
        }
    }
    /// <summary>
    /// Empties the TableName table in the database and initializes its generator/sequencer or identity column.
    /// <para>DANGEROUS</para>
    /// </summary>
    public void ResetTable(string TableName)
    {
        ExecSql("delete from " + TableName);
        Provider.SetTableGeneratorTo(this.ConnectionInfo.ConnectionString, TableName, 0);
    }
    /// <summary>
    /// Returns true if a table contains no rows in the database.
    /// </summary>
    public bool TableIsEmpty(string TableName)
    {
        string SqlText = string.Format("select count(*) as RESULT from {0}", TableName);

        return IntegerResult(SqlText, 0) <= 0;
    }
    /// <summary>
    /// Creates a new table in the database by executing CommandText. Returns true
    /// only if creates the table, false if the table already exists.
    /// <para>The method creates a table generator too, if the database supports generators/sequences.</para>
    /// <para>CommandText should be a CREATE TABLE statement and can contain datatype placeholders.
    /// See <see cref="SqlProvider.ReplaceDataTypePlaceholders"/> for details.</para>
    /// </summary>
    public virtual bool CreateTable(string SqlText)
    {
        string TableName = SqlHelper.ExtractTableName(SqlText);

        if (!string.IsNullOrWhiteSpace(TableName) && !TableExists(TableName))
        {
            SqlText = Provider.ReplaceDataTypePlaceholders(SqlText);

            ExecSql(SqlText);

            if (Provider.SupportsGenerators && ConnectionInfo.AutoCreateGenerators)
            {
                if (!Provider.GeneratorExists(this.ConnectionInfo.ConnectionString, "G_" + TableName))
                    Provider.CreateGenerator(this.ConnectionInfo.ConnectionString, "G_" + TableName);
                else
                    Provider.SetGeneratorTo(this.ConnectionInfo.ConnectionString, "G_" + TableName, 0);
            }

            return true;
        }

        return false;

    }
    /// <summary>
    /// Returns true if the FieldName exists in TableName table.
    /// </summary>
    public bool FieldExists(string TableName, string FieldName)
    {
        IList<string> List = GetFieldNames(TableName);
        return List.ContainsText(FieldName);
    }
    /// <summary>
    /// Returns true if an index with IndexName exists in the database.
    /// </summary>
    public bool IndexExists(string IndexName)
    {
        IList<string> List = GetIndexNames();
        return List.ContainsText(IndexName);


    }
    
    // ● properties
    public SqlProvider Provider { get; }
    public DbConnectionInfo ConnectionInfo { get; }
}
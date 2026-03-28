using System.Data.Common;
using System.Data;

namespace Tripous;

/// <summary>
/// The central point of this library
/// </summary>
static public class Db
{
    
    /// <summary>
    /// Registers DbProviderFactory classes
    /// </summary>
    static void RegisterDbProviderFactories()
    {
        //*
        DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
        DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
        DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
        //*/
    }
 
    // ● construction
    /// <summary>
    /// Static constructor
    /// </summary>
    static Db()
    {
        RegisterDbProviderFactories();
    }

    /// <summary>
    /// Initializes this class.
    /// </summary>
    static public void Initialize()
    {
        Connections.Load();
    }
    
    // ● public - data
    /// <summary>
    /// Returns the <see cref="DbProviderFactory"/>
    /// </summary>
    static public DbProviderFactory GetDbProviderFactory(DbServerType DbType)
    {
        string ProviderInvariantName =  DbType.GetProviderInvariantName();
        DbProviderFactory Factory = DbProviderFactories.GetFactory(ProviderInvariantName);
        return Factory;
    }
    /// <summary>
    /// Tests a database connection. Returns true if successful, otherwise throws an exception.
    /// </summary>
    static public bool CheckConnection(DbServerType dbType, string connectionString)
    {
        // Παίρνουμε το σωστό Factory βάσει του Enum (RDBMS-neutral)
        DbProviderFactory factory = GetDbProviderFactory(dbType);

        using (DbConnection connection = factory.CreateConnection())
        {
            if (connection == null)
                throw new Exception($"Could not create a connection for {dbType}.");

            connection.ConnectionString = connectionString;
        
            // Προσπάθεια ανοίγματος. Αν αποτύχει, θα πετάξει Exception
            // το οποίο θα πιάσεις στο UI (AnyClick) για να δείξεις το μήνυμα λάθους.
            connection.Open();
        
            return true; 
        }
    }
    /// <summary>
    /// Returns true if a connection can be made to a database.
    /// </summary>
    static public bool CanConnect(DbServerType dbType, string connectionString)
    {
        try
        {
            CheckConnection(dbType, connectionString);
            return true;
        }
        catch  
        {
            return false;
        }
    }
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    static public DataTable Select(DbConnectionInfo ConnectionInfo, string SqlText, int? CommandTimeoutSeconds = null)
    {
        var Timeout = CommandTimeoutSeconds ?? ConnectionInfo.CommandTimeoutSeconds;
        return Select(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.DbServerType, Timeout);
    }
    /// <summary>
    /// Executes a SELECT statement and returns the result as a DataTable.
    /// </summary>
    static public DataTable Select(string ConnectionString, string SqlText, DbServerType DbType, int? CommandTimeoutSeconds = null)
    {
        DbProviderFactory Factory = GetDbProviderFactory(DbType);
        var Timeout = CommandTimeoutSeconds ?? DbConnectionInfo.DefaultCommandTimeoutSeconds;

        using (DbConnection Connection = Factory.CreateConnection())
        {
            Connection.ConnectionString = ConnectionString;
            Connection.Open();

            using (DbCommand Command = Connection.CreateCommand())
            {
                Command.CommandText = SqlText;
                Command.CommandTimeout = Timeout;

                using (DbDataReader Reader = Command.ExecuteReader())
                {
                    DataTable Table = new DataTable();
                    Table.Load(Reader);
                    Table.CaseSensitive = false;
                    Table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    return Table;
                }
            }
        }
    }
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    static public int ExecSql(DbConnectionInfo ConnectionInfo, string SqlText, int? CommandTimeoutSeconds = null)
    {
        var Timeout = CommandTimeoutSeconds ?? ConnectionInfo.CommandTimeoutSeconds;
        return ExecSql(ConnectionInfo.ConnectionString, SqlText, ConnectionInfo.DbServerType, Timeout);
    }
    /// <summary>
    /// Executes a SQL statement (INSERT, UPDATE, DELETE, κλπ) and returns the number of rows affected.
    /// </summary>
    static public int ExecSql(string ConnectionString, string SqlText, DbServerType DbType, int? CommandTimeoutSeconds = null)
    {
        DbProviderFactory Factory = GetDbProviderFactory(DbType);
        var Timeout = CommandTimeoutSeconds ?? DbConnectionInfo.DefaultCommandTimeoutSeconds;

        using (DbConnection Connection = Factory.CreateConnection())
        {
            if (Connection == null)
                throw new Exception($"Could not create a connection for {DbType}.");

            Connection.ConnectionString = ConnectionString;
            Connection.Open();

            using (DbCommand Command = Connection.CreateCommand())
            {
                Command.CommandText = SqlText;
                Command.CommandTimeout = Timeout;

                int rowsAffected = Command.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }
    
    // ● properties
    static public DbConnections Connections = new DbConnections();
}
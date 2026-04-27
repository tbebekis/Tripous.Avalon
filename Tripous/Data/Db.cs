namespace Tripous.Data;

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

    static DbIni fMainIni;
    
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
    }
        
    // ● db connections
    /// <summary>
    /// Loads connections from a .json file, using the <see cref="SysConfig.SqlConnectionsFilePath"/> setting.
    /// </summary>
    static public void LoadConnections() =>  Connections.Load();
    static public DbConnectionInfo GetConnectionInfo(string Name) => Connections.Get(Name);
    /// <summary>
    /// Returns the default connection string, if any, else throws an exception.
    /// </summary>
    static public DbConnectionInfo GetDefaultConnectionInfo() => Connections.Get(SysConfig.DefaultConnectionName);
 
    // ● to/from base64  
    /// <summary>
    /// Converts Table to Base64 string
    /// </summary>
    static public string TableToToBase64(DataTable Table)
    {
        if (Table != null)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                Table.WriteXml(MS, XmlWriteMode.WriteSchema);
                return Convert.ToBase64String(MS.ToArray());
            }
        }

        return string.Empty;
    }
    /// <summary>
    /// Converts the Base64 Text to a DataTable
    /// </summary>
    static public DataTable Base64ToTable(string Text)
    {
        if (!string.IsNullOrWhiteSpace(Text))
        {
            using (MemoryStream MS = new MemoryStream(Convert.FromBase64String(Text)))
            {
                MS.Position = 0;
                DataTable Table = new MemTable("");
                Table.ReadXml(MS);
                Table.AcceptChanges();
                return Table;
            }
        }

        return null;
    }
    /// <summary>
    /// Converts DataSet to Base64 string
    /// </summary>
    static public string DataSetToToBase64(DataSet DS)
    {
        if (DS != null)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                DS.WriteXml(MS, XmlWriteMode.WriteSchema);
                return Convert.ToBase64String(MS.ToArray());
            }
        }

        return string.Empty;
    }
    /// <summary>
    /// Converts the Base64 Text to a DataSet
    /// </summary>
    static public DataSet Base64ToDataSet(string Text)
    {
        if (!string.IsNullOrWhiteSpace(Text))
        {
            using (MemoryStream MS = new MemoryStream(Convert.FromBase64String(Text)))
            {
                MS.Position = 0;
                DataSet ds = new DataSet("DataSet");
                ds.ReadXml(MS);
                ds.AcceptChanges();
                return ds;
            }
        }

        return null;
    }    
    
 
 
    // ● properties
    static public DbConnections Connections = new DbConnections();
    static public DbIni MainIni => fMainIni ?? (fMainIni = new DbIni(GetDefaultConnectionInfo()));
    static public readonly string StandardDefaultValues = "CompanyId;EmptyString;AppDate;SysDate;SysTime;DbServerTime;AppUserName;AppUserId;NetUserName;Guid";

}
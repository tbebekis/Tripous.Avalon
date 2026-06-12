/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

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

    /// <summary>
    /// Field backing the <see cref="MainIni"/> property.
    /// </summary>
    static DbIni fMainIni;
    /// <summary>
    /// Field backing the <see cref="DefaultStore"/> property.
    /// </summary>
    static SqlStore fDefaultStore;
    
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
    /// Loads connections from a .json file, using the <see cref="SettingsBase.SettingsFilePath"/> setting.
    /// </summary>
    static public void LoadConnections() => Connections.Load();
    /// <summary>
    /// Returns the <see cref="DbConnectionInfo"/> with the specified <paramref name="Name"/>, if any, else throws an exception.
    /// </summary>
    static public DbConnectionInfo GetConnectionInfo(string Name) => Connections.Get(Name);
    /// <summary>
    /// Returns the default connection string, if any, else throws an exception.
    /// </summary>
    static public DbConnectionInfo GetDefaultConnectionInfo() => Connections.Get(DbConfig.DefaultConnectionName);
 
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
    /// <summary>
    /// Returns the default <see cref="SqlStore"/>, creating it on first access via <see cref="SqlStores.CreateDefaultSqlStore"/>.
    /// </summary>
    static public SqlStore DefaultStore => fDefaultStore ??= SqlStores.CreateDefaultSqlStore();
    /// <summary>
    /// The registered database connections.
    /// </summary>
    static public DbConnections Connections = new DbConnections();
    /// <summary>
    /// Returns the main <see cref="DbIni"/> instance, creating it on first access using the default connection info.
    /// </summary>
    static public DbIni MainIni => fMainIni ??= new DbIni(GetDefaultConnectionInfo());
    /// <summary>
    /// A semi-colon delimited list of the standard default value names.
    /// </summary>
    static public readonly string StandardDefaultValues = "CompanyId;EmptyString;AppDate;SysDate;SysTime;DbServerTime;AppUserName;AppUserId;NetUserName;Guid";
 
    /// <summary>
    /// Db global settings
    /// </summary>
    static public DbGlobalSettings Settings { get; } = new();
}
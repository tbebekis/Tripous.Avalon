namespace Tripous.Data;

/// <summary>
/// Information about a connection to a database.
/// </summary>
public class DbConnectionInfo
{
    private int fCommandTimeoutSeconds;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public DbConnectionInfo()
    {
        Schema = new DbSchema(this);
    }

    // ● public
    public override string ToString()
    {
        return !string.IsNullOrWhiteSpace(Name)? Name: base.ToString();
    }

    /// <summary>
    /// Returns the provider invariant name.
    /// </summary>
    public string GetProviderInvariantName()
    {
        return DbServerType.GetProviderInvariantName();
    }
    /// <summary>
    /// Returns the <see cref="SqlProvider"/> of this connection string. If the provider is not registered with <see cref="SqlProviders"/> an exception is thrown.
    /// </summary>
    public SqlProvider GetSqlProvider() => SqlProviders.GetSqlProvider(DbServerType);
 
    
    // ● properties
    /// <summary>
    /// A unique name among all connections
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type of the RDBMs
    /// </summary>
    public DbServerType DbServerType { get; set; } = DbServerType.Sqlite;
    /// <summary>
    /// The connection string
    /// </summary>
    public string ConnectionString { get; set; } // encrypted?
    /// <summary>
    /// The time in seconds to wait for an SELECT/INSERT/UPDATE/DELETE/CREATE TABLE ect. command to execute. Zero means the default timeout.
    /// </summary>
    public int CommandTimeoutSeconds
    {
        get => fCommandTimeoutSeconds >= Db.Settings.DefaultCommandTimeoutSeconds
            ? fCommandTimeoutSeconds
            : Db.Settings.DefaultCommandTimeoutSeconds;
        set => fCommandTimeoutSeconds = value;
    }
    /// <summary>
    /// True to autocreate generators/sequencers
    /// </summary>
    public bool AutoCreateGenerators { get; set; }

    /// <summary>
    /// The <see cref="DbSchema"/> associated to this connection
    /// </summary>
    [JsonIgnore]
    public DbSchema Schema { get; private set; }
    /// <summary>
    /// A user defined information object.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}
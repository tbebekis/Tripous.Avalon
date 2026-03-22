using System.Text.Json.Serialization;

namespace Tripous;

/// <summary>
/// Information about a connection to a database.
/// </summary>
public class DbConnectionInfo
{
    public const int DefaultCommandTimeoutSeconds = 300;

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
    
    // ● properties
    /// <summary>
    /// A unique name among all connections
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type of the RDBMs
    /// </summary>
    public DbServerType DbServerType { get; set; } 
    /// <summary>
    /// The connection string
    /// </summary>
    public string ConnectionString { get; set; } // encrypted?
    /// <summary>
    /// The time in seconds to wait for an SELECT/INSERT/UPDATE/DELETE/CREATE TABLE ect. command to execute. Zero means the default timeout.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = DefaultCommandTimeoutSeconds;
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
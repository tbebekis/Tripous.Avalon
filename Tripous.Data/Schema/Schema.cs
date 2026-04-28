namespace Tripous.Data;

/// <summary>
/// The holder of all schema versions of a certain Schema.
/// <para>A <see cref="Schema"/> has a unique domain name and a connection name. </para>
/// <para>The <see cref="Schema.Domain"/> could be System, Application, or the unique identifier name of an external plugin. </para>
/// <para>The <see cref="Schema.ConnectionName"/> connection name, used in creating the relevant <see cref="SqlStore"/> instance that executes the schema.</para>
/// <para>A <see cref="Schema"/> has also a list of <see cref="SchemaVersion"/> items. </para>
/// <para>A <see cref="SchemaVersion"/> item has an integer version,  
/// and collections of <see cref="SchemaItem"/> items for tables and views,
/// along with collections of statements to be executed before and after the schema version is executed.</para>
/// </summary>
public class Schema  
{

    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
    public Schema()
    {
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Domain) && !string.IsNullOrWhiteSpace(ConnectionName))
        {
            return $"{Domain} - {ConnectionName}";
        }

        if (!string.IsNullOrWhiteSpace(Domain))
            return this.Domain;

        if (!string.IsNullOrWhiteSpace(ConnectionName))
            return this.ConnectionName;

        return base.ToString();
    }

    // ● public  
    /// <summary>
    /// Adds a new schema version, if the specified version is not found. Else returns the found version.
    /// </summary>
    public SchemaVersion FindOrAdd(int Version)
    {
        var Result = Find(Version);
        if (Result == null)
        {
            Result = new SchemaVersion() { Version = Version };
            Versions.Add(Result);
        }

        return Result;
    }
    /// <summary>
    /// Returns the a schema version item with a specified version, if any else null
    /// </summary>
    public SchemaVersion Find(int Version)
    {
        return Versions.Find(item => (Version == item.Version));
    }
    /// <summary>
    /// True if a schema version item with a specified version is already registered.
    /// </summary>
    public bool Exists(int Version)
    {
        return Find(Version) != null;
    }


    // ● execution methods  
    /// <summary>
    /// Executes the versions of a schema against a database.
    /// </summary>
    /// <param name="ConnectionInfo">The database connection to execute against.</param>
    /// <param name="VersionList">VersionList is a by Version sorted list of schemas for that database</param>
    void ExecuteSchema(DbConnectionInfo ConnectionInfo, List<SchemaVersion> VersionList)
    {
        /* get the current version of the database  from the dbIni table */
        DbIni Ini = Db.MainIni;
        string Entry = string.Format("Database.Version.{0}.{1}", ConnectionInfo.Name, Domain);
        int Version = Ini.ReadInteger(Entry, -1);

        foreach (SchemaVersion SV in VersionList)
        {
            if (SV.Version > Version)
            {
                SchemaExecutor.Execute(SV, ConnectionInfo);

                /* write the version to the dbIni */
                Ini.WriteInteger(Entry, SV.Version);  
            }
        }
    }
    /// <summary>
    /// Executes all schemas.
    /// </summary>
    internal void Execute()
    { 
        DbConnectionInfo Connection = Db.Connections.Find(ConnectionName);
        List<SchemaVersion> VersionList = Versions.OrderBy(item => item.Version).ToList();
        ExecuteSchema(Connection, VersionList);
    }


    /* property */
    /// <summary>
    /// Domain is the name of the register (client code) that has registered the schema.
    /// <para>It could be System, Application, or the unique identifier name of an external plugin.</para>
    /// </summary>
    public string Domain { get; set; }
    /// <summary>
    /// The name of the database connection this schema is applied against.
    /// </summary>
    public string ConnectionName { get; set; }
    /// <summary>
    /// The registered versions
    /// </summary>
    public List<SchemaVersion> Versions { get; set; } = new List<SchemaVersion>();
}
 

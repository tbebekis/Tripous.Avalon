namespace Tripous.Data;

public class DbSchema
{
       
    /// <summary>
    /// Constructor
    /// </summary>
    public DbSchema(DbConnectionInfo ConnectionInfo)
    {
        this.ConnectionInfo = ConnectionInfo;
    }

    /// <summary>
    /// Loads metadata information
    /// </summary>
    public void Load()
    {
        if (IsLoaded)
            return;

        DbSchemaLoader.Load(this);

        IsLoaded = true;
    }

    public void UnLoad()
    {
 
        DbSchemaLoader.UnLoad(this);
        IsLoaded = false;
    }
    public void ReLoad()
    {
        DbSchemaLoader.ReLoad(this);
    }

    public bool IsLoaded { get; private set; }
    public string Name => ConnectionInfo.Name;
    public DbServerType DbServerType => ConnectionInfo.DbServerType;

    public List<DbMetaTable> Tables { get; } = new();
    public List<DbMetaView> Views { get; } = new();
    public List<DbMetaProcedure> Procedures { get; } = new();
    public List<DbMetaSequence> Sequences { get; } = new();

    public DbConnectionInfo ConnectionInfo { get; }
}
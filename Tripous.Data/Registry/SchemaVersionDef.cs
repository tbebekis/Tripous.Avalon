namespace Tripous.Data;

public class SchemaVersionDef: BaseDef
{
    // ● overridable
    protected virtual void RegisterLookups()
    {
    }
    protected virtual void RegisterMasters()
    {
    }
    protected virtual void RegisterTransactions()
    {
    }

    protected virtual void RegisterInternal()
    {
        RegisterLookups();
        RegisterMasters();
        RegisterTransactions();
    }
    
    // ● construction
    public SchemaVersionDef()
    {
    }

    // ● public
    public void Register()
    {
        if (!IsRegistered)
        {
            if (string.IsNullOrWhiteSpace(Domain))
                throw new TripousDataException($"A {nameof(SchemaVersionDef)} must have a domain name such as {Sys.APPLICATION}");
            
            if (string.IsNullOrWhiteSpace(ConnectionName))
                throw new TripousDataException($"A {nameof(SchemaVersionDef)} must have a valid connection name");
            
            if (!Db.Connections.Contains(ConnectionName))
                throw new TripousDataException($"Connection Name {ConnectionName} not found for a {nameof(SchemaVersionDef)} schema");
            
            if (VersionNumber <= 0)
                throw new TripousDataException($"Version Number {VersionNumber} is not valid for a {nameof(SchemaVersionDef)} schema");
            
            Schema = Schemas.FindOrAdd(Domain, ConnectionName);
            Version = Schema.FindOrAdd(VersionNumber);

            RegisterInternal();
            
            IsRegistered = true;
        }
    }

    // ● properties
    public bool IsRegistered { get; private set; }
    
    public Schema Schema { get; private set; }
    public SchemaVersion Version { get; private set; }
    
    public virtual string Domain { get; } = Sys.APPLICATION;
    public virtual string ConnectionName { get; } = SysConfig.DefaultConnectionName;
    public virtual int VersionNumber { get;  } = -1;
    
    [JsonIgnore] public override bool IsSerializable => false;
}
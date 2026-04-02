namespace Tripous.Data;

/// <summary>
/// Base class
/// </summary>
public abstract class DbMetaObject
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string SchemaName { get; set; }               // dbo, public, κλπ (όπου υπάρχει)
    public virtual string SourceCode { get; set; }
    
    public virtual string DisplayText => Name;
}

 
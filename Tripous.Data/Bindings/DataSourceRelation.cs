namespace Tripous.Data;

/// <summary>
/// Describes a master-detail relation between two DataSource instances.
/// </summary>
public class DataSourceRelation
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceRelation(string Name, DataSource Parent, DataSource Child, string[] ParentFieldNames, string[] ChildFieldNames)
    {
        this.Name = Name;
        this.Parent = Parent;
        this.Child = Child;
        this.ParentFieldNames = ParentFieldNames;
        this.ChildFieldNames = ChildFieldNames;
    }

    // ● public
    /// <summary>
    /// Returns the relation name.
    /// </summary>
    public override string ToString()
    {
        return Name;
    }

    // ● properties
    /// <summary>
    /// Gets the relation name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the parent DataSource.
    /// </summary>
    public DataSource Parent { get; }
    /// <summary>
    /// Gets the child DataSource.
    /// </summary>
    public DataSource Child { get; }
    /// <summary>
    /// Gets the parent field names.
    /// </summary>
    public string[] ParentFieldNames { get; }
    /// <summary>
    /// Gets the child field names.
    /// </summary>
    public string[] ChildFieldNames { get; }
}

namespace Tripous;

/// <summary>
/// Base interface for all descriptors
/// </summary>
public interface IDef
{
    // ● methods
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    void CheckDescriptor();
    
    /// <summary>
    /// Assigns property values from a source instance.
    /// </summary>
    void Assign(IDef Source);
    /// <summary>
    /// Returns a clone of this instance.
    /// </summary>
    IDef Clone();
    /// <summary>
    /// Clears the property values of this instance.
    /// </summary>
    void Clear();

    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
    /// </summary>
    void UpdateReferences();
    
    // ● properties
    string Name { get; set; }
    string TitleKey { get; set; }
    [JsonIgnore] 
    public string Title { get; }
    [JsonIgnore] 
    public object Tag { get; }
}
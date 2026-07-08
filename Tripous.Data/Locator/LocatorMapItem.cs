namespace Tripous.Data;

/// <summary>
/// A locator mapping item.
/// </summary>
public class LocatorMapItem
{
    // ● private
    string fSourceField;
    string fTargetField;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapItem()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapItem(string SourceField, string TargetField)
    {
        this.SourceField = SourceField;
        this.TargetField = TargetField;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the source field name.
    /// </summary>
    public string SourceField
    {
        get => fSourceField;
        set => fSourceField = value;
    }
    /// <summary>
    /// Gets or sets the target field name.
    /// </summary>
    public string TargetField
    {
        get => fTargetField;
        set => fTargetField = value;
    }
}

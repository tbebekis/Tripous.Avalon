namespace Tripous.Data;

/// <summary>
/// A locator mapping plan.
/// </summary>
public class LocatorMapPlan
{
    // ● private
    string fLocatorName;
    string fReferenceField;
    List<LocatorMapItem> fItems;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapPlan()
    {
    }

    // ● public
    /// <summary>
    /// Adds a mapping item.
    /// </summary>
    public LocatorMapItem Add(string SourceField, string TargetField)
    {
        LocatorMapItem Result = new(SourceField, TargetField);
        Items.Add(Result);
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the locator name.
    /// </summary>
    public string LocatorName
    {
        get => fLocatorName;
        set => fLocatorName = value;
    }
    /// <summary>
    /// Gets or sets the reference field name.
    /// </summary>
    public string ReferenceField
    {
        get => fReferenceField;
        set => fReferenceField = value;
    }
    /// <summary>
    /// Gets the mapping items.
    /// </summary>
    public List<LocatorMapItem> Items => fItems ??= [];
}

namespace Tripous.Data;

/// <summary>
/// Context of a locator resolution operation.
/// </summary>
public class LocatorContext
{
    // ● private
    string fLocatorName;
    Dictionary<string, object> fParams;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorContext()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorContext(string LocatorName)
    {
        this.LocatorName = LocatorName;
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
    /// Gets the context parameters.
    /// <para>Use this dictionary for runtime execution hints that are not part of the locator descriptor identity, e.g. a <c>ConnectionName</c> override.</para>
    /// </summary>
    public Dictionary<string, object> Params => fParams ??= [];
}

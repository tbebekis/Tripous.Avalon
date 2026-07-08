namespace Tripous.Data;

/// <summary>
/// Request for a locator resolution operation.
/// </summary>
public class LocatorRequest
{
    // ● private
    object fKeyValue;
    string fSearchTerm;
    string fSearchField;
    LocatorContext fContext;
    bool fIsMultiRow;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorRequest()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the key value to resolve.
    /// <para>When specified, the locator resolves by exact key and ignores <see cref="SearchTerm"/>.</para>
    /// </summary>
    public object KeyValue
    {
        get => fKeyValue;
        set => fKeyValue = value;
    }
    /// <summary>
    /// Gets or sets the search term.
    /// </summary>
    public string SearchTerm
    {
        get => fSearchTerm;
        set => fSearchTerm = value;
    }
    /// <summary>
    /// Gets or sets the search field.
    /// </summary>
    public string SearchField
    {
        get => fSearchField;
        set => fSearchField = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the request is for a multi-row locator.
    /// </summary>
    public bool IsMultiRow
    {
        get => fIsMultiRow;
        set => fIsMultiRow = value;
    }
    /// <summary>
    /// Gets or sets the locator context.
    /// </summary>
    public LocatorContext Context
    {
        get => fContext ??= new();
        set => fContext = value;
    }
}

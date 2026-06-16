namespace tERP.Data;

/// <summary>
/// Contains application default settings grouped by trade direction.
/// </summary>
[TypeStore]
public class AppDefaultProperties
{
    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public AppDefaultProperties()
    {
    }

    // ● properties
    /// <summary>
    /// Default settings used by sales documents.
    /// </summary>
    public SalesDefaults Sales { get; set; } = new();
    /// <summary>
    /// Default settings used by purchase documents.
    /// </summary>
    public PurchaseDefaults Purchase { get; set; } = new();
}

namespace Tripous.Data;

/// <summary>
/// Provides DataSource access to a DataView.
/// </summary>
public class DataViewProvider: DataTableProvider
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataViewProvider(DataView View)
        : base(View)
    {
    }
}

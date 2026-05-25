namespace Tripous.Desktop;

/// <summary>
/// Binding information for simple controls (single-line controls, lookup combo-boxes and locator boxes).
/// </summary>
public class ControlBinding: TripousBinding
{
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public ControlBinding()
    {
    }

    // ● public
    /// <summary>
    /// Calls the base dispose handler and disposes the DataSource binding.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        DataSourceBinding?.Dispose();
    }

    // ● properties
    /// <summary>
    /// The control of this binding
    /// </summary>
    public Control Control { get; set; }
    /// <summary>
    /// The DataSource binding, when this binding uses the new binding system.
    /// </summary>
    public DataSourceBinding DataSourceBinding { get; set; }
    /// <summary>
    /// True when this binding uses the new DataSource binding system.
    /// </summary>
    public bool UsesDataSourceBinding => DataSourceBinding != null;
}

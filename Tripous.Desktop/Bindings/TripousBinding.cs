namespace Tripous.Desktop;

/// <summary>
/// Base class for Tripous UI bindings.
/// </summary>
public class TripousBinding
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TripousBinding()
    {
    }
    
    // ● public
    /// <summary>
    /// Calls the <see cref="DisposeAction"/> handler.
    /// </summary>
    public virtual void Dispose()
    {
        DisposeAction?.Invoke();
    }

    // ● properties
    /// <summary>
    /// The bound field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The <see cref="DataColumn"/> of this binding.
    /// </summary>
    public DataColumn DataColumn { get; set; }
    /// <summary>
    /// The <see cref="MemTable"/> of this binding.
    /// </summary>
    public MemTable Table => DataColumn?.Table as MemTable;

    /// <summary>
    /// Optional field definition associated to the binding.
    /// </summary>
    public FieldDef FieldDef { get; set; }
    /// <summary>
    /// Optional lookup source associated to the binding.
    /// </summary>
    public LookupSource LookupSource { get; set; }
    /// <summary>
    /// The locator, if any, else null.
    /// </summary>
    public Locator Locator { get; set; }

    /// <summary>
    /// Optional locator definition associated to the binding.
    /// </summary>
    public LocatorDef LocatorDef { get; set; }
    /// <summary>
    /// Optional reference context menu associated to the binding.
    /// </summary>
    public ReferenceContextMenu ReferenceContextMenu { get; set; }
    /// <summary>
    /// True while refreshing.
    /// </summary>
    public bool IsRefreshing { get; set; }
    
    public Action DisposeAction { get; set; }
}

namespace Tripous.Desktop;

/// <summary>
/// Defines the standard actions of a reference context menu.
/// </summary>
public enum ReferenceMenuActionType
{
    ShowList,
    Reload,
    Edit,
    Add,
    Clear,
}

/// <summary>
/// Provides the runtime context used by a reference context menu action.
/// </summary>
public class ReferenceMenuCommandContext
{
    // ● constructor
    public ReferenceMenuCommandContext()
    {
    }

    // ● properties
    public ReferenceMenuActionType ActionType { get; set; }
    public ReferenceContextMenu Menu { get; set; }
    public TripousBinding Binding { get; set; }
    public string FormName { get; set; }
    public object RowId { get; set; }
    public Control Caller { get; set; }
    public DataFormContext FormContext { get; set; }
    public object Result { get; set; }
}

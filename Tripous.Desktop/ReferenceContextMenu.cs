namespace Tripous.Desktop;

/// <summary>
/// Common context menu for controls that edit reference values, such as lookup and locator controls.
/// </summary>
[TypeStore]
public class ReferenceContextMenu
{
    // ● protected
    /// <summary>
    /// Shows the related list form.
    /// </summary>
    protected virtual void ShowList(TripousBinding Binding)
    {
    }
    /// <summary>
    /// Reloads the reference source.
    /// </summary>
    protected virtual void ReloadList(TripousBinding Binding)
    {
    }
    /// <summary>
    /// Edits the current reference item.
    /// </summary>
    protected virtual void Edit(TripousBinding Binding)
    {
    }
    /// <summary>
    /// Adds a new reference item.
    /// </summary>
    protected virtual void Add(TripousBinding Binding)
    {
    }
    /// <summary>
    /// Clears the current reference value.
    /// </summary>
    protected virtual void Clear(TripousBinding Binding)
    {
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ReferenceContextMenu()
    {
        Menu = new();

        mnuShowList = new MenuItem() { Header = "Show List" };
        mnuReload = new MenuItem() { Header = "Reload" };
        mnuEdit = new MenuItem() { Header = "Edit" };
        mnuAdd = new MenuItem() { Header = "Add" };
        mnuClear = new MenuItem() { Header = "Clear" };
        
        Menu.Items.Add(mnuShowList);
        Menu.Items.Add(mnuReload);
        Menu.Items.Add(mnuEdit);
        Menu.Items.Add(mnuAdd);
        Menu.Items.Add(mnuClear);
    }

    // ● public
    /// <summary>
    /// Dispatches a menu click to the corresponding operation.
    /// </summary>
    public virtual void MenuItemClicked(TripousBinding Binding, MenuItem MenuItem)
    {
        if (Binding == null)
            throw new TripousArgumentNullException(nameof(Binding));
        
        if (MenuItem == mnuShowList)
            ShowList(Binding);
        else if (MenuItem == mnuReload)
            ReloadList(Binding);
        else if (MenuItem == mnuEdit)
            Edit(Binding);
        else if (MenuItem == mnuAdd)
            Add(Binding);
        else if (MenuItem == mnuClear)
            Clear(Binding);
    }

    // ● properties

    /// <summary>
    /// The actual Avalonia context menu.
    /// </summary>
    public ContextMenu Menu { get; protected set; }

    /// <summary>
    /// Show List menu item.
    /// </summary>
    public MenuItem mnuShowList { get; protected set; }
    /// <summary>
    /// Reload menu item.
    /// </summary>
    public MenuItem mnuReload { get; protected set; }
    /// <summary>
    /// Edit menu item.
    /// </summary>
    public MenuItem mnuEdit { get; protected set; }
    /// <summary>
    /// Add menu item.
    /// </summary>
    public MenuItem mnuAdd { get; protected set; }
    /// <summary>
    /// Clear menu item.
    /// </summary>
    public MenuItem mnuClear{ get; protected set; }
}

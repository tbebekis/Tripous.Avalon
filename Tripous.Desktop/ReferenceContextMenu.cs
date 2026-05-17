namespace Tripous.Desktop;

public interface IReferenceContextMenuHost
{
    bool CanOpenRefContextMenu(ReferenceContextMenu RefContextMenu);
    void EnableRefContextMenuItems(ReferenceContextMenu RefContextMenu);
}

/// <summary>
/// Common context menu for controls that edit reference values, such as lookup and locator controls.
/// </summary>
[TypeStore]
public class ReferenceContextMenu
{
    // ● protected
    protected IReferenceContextMenuHost MenuHost;
    
    /// <summary>
    /// Shows the related list form.
    /// </summary>
    protected virtual void ShowList()
    {
    }
    /// <summary>
    /// Reloads the reference source.
    /// </summary>
    protected virtual void ReloadList()
    {
    }
    /// <summary>
    /// Edits the current reference item.
    /// </summary>
    protected virtual void Edit()
    {
    }
    /// <summary>
    /// Adds a new reference item.
    /// </summary>
    protected virtual void Add()
    {
    }
    /// <summary>
    /// Clears the current reference value.
    /// </summary>
    protected virtual void Clear()
    {
    }

    
    /// <summary>
    /// Dispatches a menu click to the corresponding operation.
    /// </summary>
    protected virtual void AnyMenuItem_Click(object Sender, RoutedEventArgs Args)
    {
        MenuItem MenuItem = Sender as MenuItem;
        
        if (MenuItem == mnuShowList)
            ShowList();
        else if (MenuItem == mnuReload)
            ReloadList();
        else if (MenuItem == mnuEdit)
            Edit();
        else if (MenuItem == mnuAdd)
            Add();
        else if (MenuItem == mnuClear)
            Clear();
    }
    protected virtual bool CanOpen() => MenuHost.CanOpenRefContextMenu(this);

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ReferenceContextMenu()
    {
        Menu = new();

        MenuItem CreateMenuItem(string Header)
        {
            MenuItem Result = new() { Header = Header };
            Result.Click += AnyMenuItem_Click;
            Menu.Items.Add(Result);
            return Result;
        }

        mnuShowList = CreateMenuItem("Show List");
        mnuReload = CreateMenuItem("Reload");
        mnuEdit = CreateMenuItem("Edit");
        mnuAdd = CreateMenuItem("Add");
        mnuClear = CreateMenuItem("Clear");
    }

    // ● public
    public virtual void Initialize(IReferenceContextMenuHost MenuHost, TripousBinding Binding)
    {
        if (this.Binding != null)
            throw new TripousDesktopException($"{this.GetType().FullName} is already initialized");
        
        if (MenuHost == null)
            throw new TripousArgumentNullException(nameof(MenuHost));
        if (Binding == null)
            throw new TripousArgumentNullException(nameof(Binding));
        
        this.MenuHost = MenuHost;
        this.Binding = Binding;

        Binding.ReferenceContextMenu = this;
            
        // -----------------------------------------------
        if (Binding is ControlBinding ControlBinding)
        {
            ControlBinding.Control.AddHandler(InputElement.PointerPressedEvent, (Sender, Args) =>
            {
                if (Sender is not Control Control)
                    return;
                
                if (!Args.GetCurrentPoint(Control).Properties.IsRightButtonPressed)
                    return;

                if (!CanOpen())
                    return;
                    
                if (Control is ComboBox ComboBox)
                    ComboBox.IsDropDownOpen = false;

                Menu.Open(Control);

                Args.Handled = true;
            }, RoutingStrategies.Tunnel);
        }
        // -----------------------------------------------
        Menu.Opening += (Sender, Args) => MenuHost.EnableRefContextMenuItems(this);
    }

    // ● properties
    /// <summary>
    /// The binding this instance serves.
    /// </summary>
    public TripousBinding Binding { get; protected set; }
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

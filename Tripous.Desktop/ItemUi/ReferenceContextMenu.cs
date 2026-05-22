namespace Tripous.Desktop;

public interface IReferenceContextMenuHost
{
    bool CanOpenRefContextMenu(ReferenceContextMenu RefContextMenu);
    bool CanExecute(ReferenceMenuCommandContext Context);
    object Execute(ReferenceMenuCommandContext Context);
}

/// <summary>
/// Common context menu for controls that edit reference values, such as lookup and locator controls.
/// </summary>
[TypeStore]
public class ReferenceContextMenu
{
    // ● protected
    protected IReferenceContextMenuHost MenuHost;
    
    // ● protected
    protected virtual string GetFormName()
    {
        return Binding.LookupSource?.LookupDef?.Form ?? Binding.LocatorDef?.Form;
    }
    protected virtual object GetRowId()
    {
        if (Binding is ControlBinding ControlBinding && ControlBinding.Control is ComboBox ComboBox)
        {
            if (ComboBox.SelectedItem is LookupItem Item && Item.IsNullItem)
                return null;
        }

        if (Binding?.Table?.CurrentRow == null || string.IsNullOrWhiteSpace(Binding.FieldName))
            return null;

        return Binding.Table.CurrentRow[Binding.FieldName];
    }
    protected virtual ReferenceMenuActionType GetActionType(MenuItem MenuItem)
    {
        if (MenuItem == mnuShowList)
            return ReferenceMenuActionType.ShowList;
        if (MenuItem == mnuReload)
            return ReferenceMenuActionType.Reload;
        if (MenuItem == mnuEdit)
            return ReferenceMenuActionType.Edit;
        if (MenuItem == mnuAdd)
            return ReferenceMenuActionType.Add;
        if (MenuItem == mnuClear)
            return ReferenceMenuActionType.Clear;

        return ReferenceMenuActionType.ShowList;
    }
    protected virtual ReferenceMenuCommandContext CreateContext(ReferenceMenuActionType ActionType)
    {
        return new ReferenceMenuCommandContext()
        {
            ActionType = ActionType,
            Menu = this,
            Binding = Binding,
            FormName = GetFormName(),
            RowId = ActionType == ReferenceMenuActionType.Edit ? GetRowId() : null,
            Caller = (Binding as ControlBinding)?.Control
        };
    }
    protected virtual void EnableMenuItems()
    {
        mnuReload.IsVisible = Binding.LocatorDef == null;
        mnuShowList.IsEnabled = MenuHost.CanExecute(CreateContext(ReferenceMenuActionType.ShowList));
        mnuReload.IsEnabled = MenuHost.CanExecute(CreateContext(ReferenceMenuActionType.Reload));
        mnuEdit.IsEnabled = MenuHost.CanExecute(CreateContext(ReferenceMenuActionType.Edit));
        mnuAdd.IsEnabled = MenuHost.CanExecute(CreateContext(ReferenceMenuActionType.Add));
        mnuClear.IsEnabled = MenuHost.CanExecute(CreateContext(ReferenceMenuActionType.Clear));

        ToolTip.SetTip(mnuEdit, mnuEdit.IsEnabled ? null : "No reference item selected.");
    }
    /// <summary>
    /// Dispatches a menu click to the corresponding operation.
    /// </summary>
    protected virtual async void AnyMenuItem_Click(object Sender, RoutedEventArgs Args)
    {
        MenuItem MenuItem = Sender as MenuItem;
        ReferenceMenuCommandContext Context = CreateContext(GetActionType(MenuItem));
        if (!MenuHost.CanExecute(Context))
            return;
        
        object Result = MenuHost.Execute(Context);
        if (Result is Task<DataFormContext> DataFormTask)
            Context.Result = await DataFormTask;
        else if (Result is Task<object> ObjectTask)
            Context.Result = await ObjectTask;
        else if (Result is Task Task)
            await Task;
        else
            Context.Result = Result;
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
        mnuReload.IsVisible = Binding.LocatorDef == null;
            
        // -----------------------------------------------
        if (Binding is ControlBinding ControlBinding)
        {
            if (ControlBinding.Control is LocatorBox LocatorBox)
            {
                if (LocatorBox.MenuButton != null)
                {
                    LocatorBox.MenuButton.Click += (Sender, Args) =>
                    {
                        if (!CanOpen())
                            return;

                        Menu.Open(LocatorBox.MenuButton);
                    };
                    LocatorBox.MenuButton.AddHandler(InputElement.PointerPressedEvent, (Sender, Args) =>
                    {
                        if (!Args.GetCurrentPoint(LocatorBox.MenuButton).Properties.IsRightButtonPressed)
                            return;

                        if (!CanOpen())
                            return;

                        Menu.Open(LocatorBox.MenuButton);
                        Args.Handled = true;
                    }, RoutingStrategies.Tunnel);
                }
            }
            else
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
        }
        // -----------------------------------------------
        Menu.Opening += (Sender, Args) => EnableMenuItems();
    }
    public virtual bool Open(Control Control)
    {
        if (Control == null || !CanOpen())
            return false;

        Menu.Open(Control);
        return true;
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

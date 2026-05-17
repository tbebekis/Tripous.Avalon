namespace Tripous.Desktop;
 
/// <summary>
/// The item part of a <see cref="DataForm"/>
/// </summary>
[TypeStore]
public class ItemPage : UserControl, IReferenceContextMenuHost
{
    // ● protected  
    protected UiItemContext Context;
    protected DataForm fDataForm;
 
    /// <summary>
    /// Creates a field editor.
    /// </summary>
    protected virtual Control CreateEditor(FieldDef Field, ItemBinder Binder)
    {
        Control Result;
        DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
        
        if (!string.IsNullOrWhiteSpace(Field.Locator))
        {
            LocatorBox Box = new();
            Binder.Bind(Box, Field);
            return Box;
        }
        else if (Field.IsLookup)
        {
            ComboBox Box = new();
            ControlBinding Binding = Binder.BindLookup(Box, Field.Name, DataColumn, Field);
            if (!Field.IsReadOnly && !Field.IsReadOnlyUI)
            {
                // context menu for lookup combo boxes and locator box controls.
                ReferenceContextMenu RefMenu = FormDef.CreateReferenceContextMenu();
                RefMenu.Initialize(this, Binding);
            }
   
            Result = Box;
        }
        else if (Field.IsDateTime)
        {
            DatePicker Box = new();
            Binder.Bind(Box, Field.Name, DataColumn, Field);
            Result = Box;
        }
        else
        {
            TextBox Box = new();
            if (Field.IsNumeric)
            {
                Box.TextAlignment = TextAlignment.Right;
            }
            else if (Field.IsMemo)
            {
                Binder.BindMemo(Box, Field.Name, DataColumn, Field);
                Box.AcceptsReturn = true;
                Box.TextWrapping = TextWrapping.Wrap;
                Box.MinHeight = Ui.Settings.FormMemoRowCount * 24;
            }
            else
            {
                Binder.Bind(Box, Field.Name, DataColumn, Field);
            }
            Result = Box;
        }
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 0, 0, 6);
        return Result;
    }
 
    // ● binding
    /// <summary>
    /// Refreshes all binders.
    /// </summary>
    protected virtual void Refresh()
    {
        foreach (ItemBinder Binder in Binders)
            Binder.Refresh();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemPage()
    {
        Context = new();
    }

    // ● public methods
    /// <summary>
    /// Binds this instance.
    /// </summary>
    public virtual void Bind() => Bind(Ui.Settings.FormColumnCount);
    /// <summary>
    /// Binds this instance.
    /// </summary>
    public virtual void Bind(int ColumnCount)
    {
        if (IsBindingDone)
            throw new TripousDesktopException($"{this.GetType().FullName} data binding is already done.");
        
        Context.CreateEditorFunc = CreateEditor;
 
        ItemBinder.CurrentRowChanging += (s, ea) => CurrentRowChanging?.Invoke(this, EventArgs.Empty);
        ItemBinder.CurrentRowChanged += (s, ea) => CurrentRowChanged?.Invoke(this, EventArgs.Empty);
 
        ScrollViewer ScrollViewer = UiFactory.CreateScrollViewer();
        StackPanel Root = UiFactory.CreateStackPanel();
        ScrollViewer.Content = Root;
        Content = ScrollViewer;

        Context.ColumnCount = ColumnCount;
        Context.ParentControl = Root;
        
        if (Context.TopTableUiInfo.DetailList.Count == 0)
            UiItemPage.CreateSinglePageLayout(Context);
        else
            UiItemPage.CreateTabbedTopLayout(Context);

        IsBindingDone = true;
    }

    // ● IReferenceContextMenuHost implementation
    public virtual bool CanOpenRefContextMenu(ReferenceContextMenu RefContextMenu)
    {
        bool Result = RefContextMenu.Binding.FieldDef.IsReadOnlyEdit? DataForm.FormState == DataFormState.Insert : true;
        return Result;
    }
    public virtual void EnableRefContextMenuItems(ReferenceContextMenu RefContextMenu)
    {
       // TODO: EnableRefContextMenuItems()
    }
    
    // ● properties
    /// <summary>
    /// The main item binder.
    /// </summary>
    public ItemBinder ItemBinder => Context.ItemBinder;
    /// <summary>
    /// The binders of this instance.
    /// </summary>
    public List<ItemBinder> Binders => Context.Binders;
    /// <summary>
    /// The current data row.
    /// </summary>
    public DataRow CurrentRow => ItemBinder.CurrentRow;
    /// <summary>
    /// The parent form.
    /// </summary>
    public DataForm DataForm
    {
        get => fDataForm;
        set
        {
            if (fDataForm != null)
                throw new TripousDesktopException($"{this.GetType().FullName} data form is already defined.");
            if (value == null)
                throw new TripousArgumentNullException(nameof(DataForm));
            
            fDataForm = value;
            Context.Module = fDataForm.Module;
        }
    }
    /// <summary>
    /// Form context.
    /// </summary>
    public DataFormContext DataFormContext => DataForm.DataFormContext;
    /// <summary>
    /// The form definition.
    /// </summary>
    public FormDef FormDef => DataFormContext.FormDef;
    /// <summary>
    /// The module definition.
    /// </summary>
    public ModuleDef ModuleDef => DataFormContext.ModuleDef;
    /// <summary>
    /// The data module.
    /// </summary>
    public DataModule Module => DataFormContext.Module;
    /// <summary>
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;
    /// <summary>
    /// True when the binding is completed
    /// </summary>
    public bool IsBindingDone { get; protected set;  }

    // ● events
    /// <summary>
    /// Occurs before the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanging;
    /// <summary>
    /// Occurs after the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanged;


}

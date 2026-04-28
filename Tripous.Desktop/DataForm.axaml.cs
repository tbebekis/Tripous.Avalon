namespace Tripous.Desktop;

public partial class DataForm : AppForm, IRowProvider
{
    protected DataFormState fFormState = DataFormState.None;
    protected DataFormAction LastAction = DataFormAction.None;
    protected bool Saving;
    
    protected ToolBar ToolBar;

    protected Button btnHome;
    protected Button btnFind;
    protected Button btnList;

    protected Button btnInsert;
    protected Button btnEdit;
    protected Button btnDelete;

    protected Button btnSave;

    protected Button btnCancel;
    protected Button btnOK;
    protected Button btnClose;
    
    protected ToolBar SelectListToolBar;
    private ComboBox cboSelectList;
    
    private void GridList_DoubleClick(object Sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var Row = gridList.SelectedItem;
            if (Row != null)
            {
                _ = Execute(DataFormAction.Edit);
            }
        }
    }
 
    /// <summary>
    /// Processes the form options
    /// </summary>
    protected virtual void ProcessFormOptions()
    {
    }
 
    /// <summary>
    /// Updates the user interface, title, enable-disable buttons etc.
    /// </summary>
    public virtual void UpdateUi()
    {
        EnableCommands();
    }
    /// <summary>
    /// Enables and disables buttons and menu items.
    /// </summary>
    protected virtual void EnableCommands()
    {
        // visible ===============================================================
        btnHome.IsVisible = true;
        btnFind.IsVisible = IsMasterForm;
        //edtFind.IsVisible = btnFind.IsVisible;
        btnList.IsVisible = IsMasterForm;

        btnInsert.IsVisible = IsMasterForm;
        btnEdit.IsVisible = IsMasterForm;
        btnDelete.IsVisible = true;
        btnInsert.IsVisible = IsMasterForm;

        btnCancel.IsVisible = true;                       // btnCancel - visible with all form modes
        btnOK.IsVisible = IsListForm || IsModal;
        btnClose.IsVisible = IsMasterMode && !IsModal;      // btnClose - visible with non-list master forms

        // enable ================================================================
        btnHome.IsEnabled = btnHome.ContextMenu != null && btnHome.ContextMenu.Items.Count > 0;
        btnFind.IsEnabled = !DataFormAction.Find.In(InvalidActions);
        btnList.IsEnabled = !DataFormAction.List.In(InvalidActions);
        
        btnInsert.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && !FormState.In(DataFormState.Insert | DataFormState.Edit) && FormType != FormType.List;
        btnEdit.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && !FormState.In(DataFormState.Insert | DataFormState.Edit) && FormType != FormType.List;
        btnDelete.IsEnabled = !DataFormAction.Delete.In(InvalidActions) && FormType == FormType.List && !IsListEmpty;
        btnSave.IsEnabled = (IsMasterForm && FormState.In(DataFormState.Insert | DataFormState.Edit)) || (IsListMode && FormState == DataFormState.List);
        
        // List state and Modal: cancels the form
        // List state and List forms: closes the form without saving
        // Edit states: cancels edits and returns to List state
        btnCancel.IsEnabled = IsListMode
            || (IsMasterForm && FormState.In(DataFormState.Insert | DataFormState.Edit))
            || (IsMasterForm && FormState.In(DataFormState.List ));

        // btnOK - accessible in List state only, with list forms and all modal forms
        // List state and Modal: saves any edits, closes the form with OK and returns the current row               
        btnOK.IsEnabled = IsListForm || (IsModal && FormState == DataFormState.List);

        // List state: closes the form                
        btnClose.IsEnabled = IsMasterMode && FormState.In(DataFormState.List );
    }
 
    // ● miscs
    /// <summary>
    /// Passes any result to the caller of the form, if any. Useful with modal forms.
    /// </summary>
    protected virtual void PassResultBack()
    {
    }
    /// <summary>
    /// Returns the control that is last added to the container
    /// </summary>
    protected virtual Control FindFirstFocusableControl(Control Container)
    {
        return null;
    }
    /// <summary>
    /// Handles a broadcaster event.
    /// </summary>
    protected virtual void HandleBroadcasterEvent(string EventName, IDictionary<string, object> Args)
    {
        switch (EventName)
        {
            case "NOT_EXISTED_EVENT_NAME": 
                break;
        }
    }
    /// <summary>
    /// It is called by the OnKeyDown() method. 
    /// <para>Returns true if processes the key</para>
    /// </summary>
    protected virtual bool ProcessKeyDown(KeyEventArgs e)
    {
        return false;
    }
    /// <summary>
    /// It is called when the escape key is pressed. 
    /// <para>Returning true indicates that the key press is handled.</para>
    /// <para>NOTE: By default, when is a modal dialog, it sets <see cref="ModalResult"/> to Cancel, and closes the form.</para>
    /// </summary>
    protected virtual bool ProcessEscapeKey()
    {
        if (!IsModal && this.FormState == DataFormState.List)
        {
            this.CloseForm();
            return true;
        }
        else if (this.IsModal)
        {
            this.ModalResult = ModalResult.Cancel;
            return true;
        }

        return false;
    }
    
    // ● overrides
    /// <summary>
    /// Called when the control is added to a rooted visual tree. 
    /// </summary>
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (!Design.IsDesignMode && !IsFormInitialized)
        {
            FormInitializing();
            FormInitialize();
            this.IsFormInitialized = true;
            FormInitialized();
            await Start();
        }
    }
    /// <summary>
    /// Raises KeyDown event
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!Design.IsDesignMode)
        {
            if (ProcessKeyDown(e))
            {
                e.Handled = true;
            }
        }

        base.OnKeyDown(e);
    }
 
    /// <summary>
    /// Called just before form initialization
    /// </summary>
    protected override void FormInitializing()
    {
        base.FormInitializing();
        
        ProcessFormOptions();

    }
    /// <summary>
    /// Called in order to initialize the form
    /// </summary>
    protected override void FormInitialize()
    {            
    }
    /// <summary>
    /// Called just after form initialization
    /// </summary>
    protected override void FormInitialized()
    {
    }
    
    /// <summary>
    /// Executes any first command on the form
    /// </summary>
    protected override async Task Start()
    {        
        CreateToolBar();
        CreateItemPanel();
        CreateFindPanel();
        
        if (StartAction == DataFormAction.List || StartAction == DataFormAction.Edit || StartAction == DataFormAction.Insert)
        {
            await Execute(StartAction);
        }
    }
 
    // ● form state
    protected virtual DataFormState FormState
    {
        get { return fFormState; }
        set
        {
            if (value != fFormState)
            {
                fFormState = value;
                FormStateChanged();
                UpdateUi();
            }
        }
    }
    protected virtual bool IsListEmpty => Module != null && Module.tblList != null && Module.tblList.Rows.Count > 0;
    protected virtual void FormStateChanged()
    { 
        if (gridList != null)
        {
            switch (FormState)
            {
                case DataFormState.List:
                    pnlItem.IsVisible = false;
                    pnlList.IsVisible = true;
                    break;
                case DataFormState.Insert:
                case DataFormState.Edit:
                    pnlList.IsVisible = false;
                    pnlItem.IsVisible = true;
                    break;
            }
        } 
    }
 
    // ● form actions
    protected virtual async Task Execute(DataFormAction Value)
    {
        if (!Executing(Value))
        {
            object oId;
            switch (Value)
            {
                case DataFormAction.Home:
                    ExecuteHome();
                    break;
                case DataFormAction.Find:
                    ExecuteFind();
                    break;

                case DataFormAction.List:
                    await ExecuteList();
                    break;
                case DataFormAction.Insert:
                    ExecuteInsert();
                    break;
                case DataFormAction.Edit:
                    oId = GetCurrentListId();
                    if (!Sys.IsNull(oId))
                        ExecuteEdit(oId);
                    break;
                case DataFormAction.Delete:
                    oId = GetCurrentListId();
                    if (!Sys.IsNull(oId))
                        await ExecuteDelete(oId);
                    break;
                case DataFormAction.Save:
                    ExecuteSave();
                    break;
                case DataFormAction.Cancel:
                    if (IsListMode)
                    {
                        if (IsModal)
                            this.ModalResult = ModalResult.Cancel;
                        else
                            CloseForm();
                    }
                    else if (IsMasterForm)
                    {
                        if (FormState == DataFormState.List)  
                        {
                            if (IsModal)
                                this.ModalResult = ModalResult.Cancel;
                            else
                                CloseForm();
                        }
                        else if (FormState == DataFormState.Insert || FormState == DataFormState.Edit)  
                        {
                            await ExecuteCancelEdit();
                        }                             
                    }
                    break;
                case DataFormAction.Ok:
                    if (IsListMode)
                        ListSave();

                    if (IsModal)
                        this.ModalResult = ModalResult.Ok;
                    else
                        CloseForm();

                    break;
                case DataFormAction.Close:
                    CloseForm();
                    break;
            }

            Executed(Value);
            UpdateUi();
        }

    }
    protected virtual bool Executing(DataFormAction Value)
    {
        return false;
    }
    protected virtual void Executed(DataFormAction Value)
    {
        LastAction = Value;
    }

    protected virtual void ExecuteHome()
    {
        this.FormState = DataFormState.List;
    }
    protected virtual void ExecuteFind()
    {
        this.FormState = DataFormState.List;
    }

    protected virtual async Task ExecuteList()
    {
        await Task.CompletedTask;
        
        if (!Saving && (FormState == DataFormState.Insert || FormState == DataFormState.Edit))  
        {
            if (!await ExecuteCancelEdit())
                return;
        }

        ListSelect();
        this.FormState = DataFormState.List;
    }
    protected virtual void ExecuteInsert()
    {
        ItemInsert();
        this.FormState = DataFormState.Insert;
    }
    protected virtual void ExecuteEdit(object oId = null)
    {
        if (oId == null)
            oId = GetCurrentListId();

        if (!Sys.IsNull(oId))
        {
            ItemLoad(oId);
            this.FormState = DataFormState.Edit;
        }
    }
    protected virtual async Task ExecuteDelete(object oId = null)
    {
        if (oId == null)
            oId = GetCurrentListId();

        if (!Sys.IsNull(oId))
        {
            if (await MessageBox.YesNo("Delete item?", this))
            {
                ItemDelete(oId);
            }
        }
    }
    protected virtual void ExecuteSave()
    {
        CheckSave();

        Saving = true;
        try
        {
            ItemSave();
        }
        finally
        {
            Saving = false;
        }

        this.FormState = DataFormState.Edit;
    }
    protected virtual async Task<bool> ExecuteCancelEdit()
    {
        if (ItemHasChanges())
        {
            if (!await MessageBox.YesNo("Cancel changes?"))
                return false;

            ItemCancelChanges();
        }

        return true;
    }
 
    // ● list
    protected virtual void ListSelect()
    {
        SelectDef SelectDef = cboSelectList.SelectedItem as SelectDef;
        if (SelectDef != null)
        {
            Module.ListSelect(SelectDef);
            
            BindListGrid();
        }
    }
    protected virtual void ListSave()
    {
    }
    protected virtual object GetCurrentListId()
    {
        if (gridList?.SelectedItem is DataRowView RowView)
        {
            DataRow Row = RowView.Row;
            if (Row.Table.Columns.Contains(ModuleDef.Table.KeyField))
                return Row[ModuleDef.Table.KeyField];
        }

        return null;
    }
  
    // ● item
    protected virtual void ItemInsert()
    {
    }
    protected virtual void ItemLoad(object oId)
    {
    }
    protected virtual void ItemDelete(object oId)
    {
    }
    protected virtual void ItemSave()
    {
    }
    protected virtual void CheckSave()
    {
    }
    protected virtual bool ItemHasChanges()
    {
        return false;
    }
    protected virtual void ItemCancelChanges()
    {
    }

    protected virtual void ItemBind()
    {
    }

    protected virtual void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;

            btnHome = ToolBar.AddButton("application_home.png", "Home", async () => await Execute(DataFormAction.Home));
            btnFind = ToolBar.AddButton("find.png", "Find", async () => await Execute(DataFormAction.Find));
            btnList = ToolBar.AddButton("table.png", "List", async () => await Execute(DataFormAction.List));
            ToolBar.AddSeparator();

            btnInsert = ToolBar.AddButton("table_add.png", "Insert", async () => await Execute(DataFormAction.Insert));
            btnEdit = ToolBar.AddButton("table_edit.png", "Edit", async () => await Execute(DataFormAction.Edit));
            btnDelete = ToolBar.AddButton("table_delete.png", "Delete", async () => await Execute(DataFormAction.Delete));
            ToolBar.AddSeparator();
            
            btnSave = ToolBar.AddButton("disk.png", "Save", async () => await Execute(DataFormAction.Save));
            ToolBar.AddSeparator();

            btnCancel = ToolBar.AddButton("cancel.png", "Cancel", async () => await Execute(DataFormAction.Cancel));
            btnOK = ToolBar.AddButton("accept.png", "OK", async () => await Execute(DataFormAction.Ok));
            ToolBar.AddSeparator();
            
            btnClose = ToolBar.AddButton("door_out.png", "Close", async () => await Execute(DataFormAction.Close));  
        }

        if (SelectListToolBar == null)
        {
            SelectListToolBar = new();
            SelectListToolBar.Panel = pnlSelectListToolBar;

            cboSelectList = SelectListToolBar.AddComboBox(ModuleDef.SelectList, 0, 150.0);
            SelectListToolBar.AddButton("lightning.png", "Execute", async () => await Execute(DataFormAction.List));
        }
   
    }
 
    protected virtual void BindListGrid()
    {
        gridList.AutoGenerateColumns = false;
        gridList.ItemsSource = null;
        gridList.Columns.Clear();

        foreach (DataColumn Column in Module.tblList.Columns)
        {
            DataGridColumn GridColumn;
            
            if (Column.ColumnName == "CountryId")
            {
                LookupSource CountryLookupSource = DataRegistry.LookupSources.Get("Country");  
                GridColumn = DataViewGridColumnFactory.CreateLookupColumn(Column.ColumnName, CountryLookupSource);
            }
            else if (Column.DataType == typeof(bool))
                GridColumn = DataViewGridColumnFactory.CreateBoolColumn(Column.ColumnName);
            else
                GridColumn = DataViewGridColumnFactory.CreateTextColumn(Column.ColumnName);
            
  

            gridList.Columns.Add(GridColumn);
        }

        gridList.ItemsSource = Module.tblList.DataView;
        gridList.PointerPressed += GridList_DoubleClick;
    }
    protected virtual void CreateItemPanel()
    {
        if (!IsListMode && !string.IsNullOrWhiteSpace(FormDef.ItemPageClassName))
        {
            ItemPage = TypeResolver.CreateInstance<ItemPage>(FormDef.ItemPageClassName);
            ItemPage.DataForm = this;

            pnlItem.Children.Clear();
            pnlItem.Children.Add(ItemPage);

            ItemPage.Bind();
        }
    }
    protected virtual void CreateFindPanel()
    {
    }
 
    // ● construction
    public DataForm()
    {
        InitializeComponent();
    }
 
    // ● properties
    /// <summary>
    /// Form context
    /// </summary>
    public DataFormContext DataFormContext => this.Context as DataFormContext;
    /// <summary>
    /// The form definition.
    /// </summary>
    public FormDef FormDef => DataFormContext.FormDef;
    /// <summary>
    /// The module definition
    /// </summary>
    public ModuleDef ModuleDef => DataFormContext.ModuleDef;
    /// <summary>
    /// The data module
    /// </summary>
    public DataModule Module => DataFormContext.Module;
    /// <summary>
    /// Returns the form mode
    /// </summary>
    public virtual FormType FormType => FormDef.FormType;
    /// <summary>
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;

    public ItemPage ItemPage { get; private set; }
    
    public DataRow ListCurrentRow => Module?.tblList?.CurrentRow;
    public DataRow CurrentRow => Module?.tblItem?.CurrentRow;

    /// <summary>
    /// Returns true when the form is a pure list form (grid only).
    /// </summary>
    public bool IsListMode => FormDef.FormType == FormType.List;
    /// <summary>
    /// Returns true when the form is a pure master form (list + item, different tables).
    /// </summary>
    public bool IsMasterMode => FormDef.FormType == FormType.Master;
    /// <summary>
    /// Returns true when the form is a master-list form (list + item, same table).
    /// </summary>
    public bool IsListMasterMode => FormDef.FormType == FormType.ListMaster;
    /// <summary>
    /// Returns true when the form has a list part (List or ListMaster).
    /// </summary>
    public bool IsListForm => FormDef.FormType == FormType.List || FormDef.FormType == FormType.ListMaster;
    /// <summary>
    /// Returns true when the form has an item part (Master or ListMaster).
    /// </summary>
    public bool IsMasterForm => FormDef.FormType == FormType.Master || FormDef.FormType == FormType.ListMaster;
    /// <summary>
    /// Returns true if this is a fixed-list (no row insert-delete allowed) form.
    /// </summary>
    public virtual bool IsFixedListForm => DataFormContext.IsFixedListForm;
}
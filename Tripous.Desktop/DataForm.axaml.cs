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
    protected ComboBox cboSelectList;
    
    void gridList_OnDoubleTapped(object sender, TappedEventArgs e)
    { 
        _ = Execute(DataFormAction.Edit);
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
        EnableControls();
    }
    /// <summary>
    /// Enables and disables buttons and menu items.
    /// </summary>
    protected virtual void EnableCommands()
    {
        // ● visible ===============================================================
        btnHome.IsVisible = !IsSingleSelect;
        btnFind.IsVisible = btnHome.IsVisible;

        btnList.IsVisible = !IsReadOnlyForm;

        btnInsert.IsVisible = !IsReadOnlyForm;
        btnEdit.IsVisible = !IsReadOnlyForm;
        btnDelete.IsVisible = !IsReadOnlyForm;
        btnInsert.IsVisible = !IsReadOnlyForm;

        btnCancel.IsVisible = true;                       // btnCancel - visible with all form modes
        btnOK.IsVisible = IsModal;
        btnClose.IsVisible = !IsModal;      // btnClose - visible with non-list master forms

        // ● enable ================================================================
        btnHome.IsEnabled = btnHome.ContextMenu != null && btnHome.ContextMenu.Items.Count > 0;
        btnFind.IsEnabled = !DataFormAction.Find.In(InvalidActions);
        btnList.IsEnabled = !DataFormAction.List.In(InvalidActions);
        
        btnInsert.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && !FormState.In(DataFormState.Insert | DataFormState.Edit);
        btnEdit.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && !FormState.In(DataFormState.Insert | DataFormState.Edit) ;
        btnDelete.IsEnabled = !DataFormAction.Delete.In(InvalidActions) && !IsListEmpty;
        btnSave.IsEnabled = FormState.In(DataFormState.Insert | DataFormState.Edit);
        
        // Edit states: cancels edits and returns to List state
        // List state and Modal: cancels the form
        btnCancel.IsEnabled = FormState.In(DataFormState.Insert | DataFormState.Edit) || (IsModal && FormState == DataFormState.List);
 
        // btnOK - accessible in List state only with modal forms
        // List state and Modal: closes the form with OK and returns the current row               
        btnOK.IsEnabled =IsModal && FormState == DataFormState.List;

        // List state: closes a non-modal form                
        btnClose.IsEnabled = FormState == DataFormState.List;
    }
    /// <summary>
    /// Enables and disables controls.
    /// </summary>
    protected virtual void EnableControls()
    {
        // ● visible ===============================================================
        pnlSideBar.IsVisible = !IsSingleSelect;
        Splitter.IsVisible = pnlSideBar.IsVisible;

        // ● enable ================================================================
        gridList.IsReadOnly = true;
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
        this.Module.CurrentRowChanged += (sender, args) =>
        {
            this.CurrentRowChanged?.Invoke(this, args);
        };
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
                    break;
                case DataFormAction.Ok:
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
            LogBox.AppendLine(oId);
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
    protected virtual object GetCurrentListId()
    {
        if (ListCurrentRow != null)
        {
            if (ListCurrentRow.Table.Columns.Contains(ModuleDef.Table.KeyField))
                return ListCurrentRow[ModuleDef.Table.KeyField];
        }
        
        return null;
    }
    void gridList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListCurrentRow = null;
        if (gridList.SelectedItem is DataRowView RowView)
        {
            ListCurrentRow = RowView.Row;
            LogBox.AppendLine("GRID");
            LogBox.AppendLine(ListCurrentRow);
        }
    }
  
    // ● item
    protected virtual void ItemInsert()
    {
    }
    protected virtual void ItemLoad(object oId)
    {
        Module.Edit(oId);
        LogBox.AppendLine(CurrentRow);
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
            /*
            if (Column.ColumnName == "CountryId")
            {
                LookupSource CountryLookupSource = DataRegistry.LookupSources.Get("Country");  
                GridColumn = DataViewGridColumnFactory.CreateLookupColumn(Column.ColumnName, CountryLookupSource);
            }
            else 
            */
            if (Column.DataType == typeof(bool))
                GridColumn = DataViewGridColumnFactory.CreateBoolColumn(Column.ColumnName);
            else
                GridColumn = DataViewGridColumnFactory.CreateTextColumn(Column.ColumnName);
            
  

            gridList.Columns.Add(GridColumn);
        }

        gridList.ItemsSource = Module.tblList.DataView;
 
        gridList.DoubleTapped += gridList_OnDoubleTapped;
        gridList.SelectionChanged += gridList_OnSelectionChanged;
    }



    protected virtual void CreateItemPanel()
    {
        if (!string.IsNullOrWhiteSpace(FormDef.ItemClassName))
        {
            ItemPage = TypeResolver.CreateInstance<ItemPage>(FormDef.ItemClassName);
            ItemPage.DataForm = this;

            pnlItem.Children.Clear();
            pnlItem.Children.Add(ItemPage);

            ItemPage.Bind();
        }
    }
    protected virtual void ItemBind()
    {
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
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;
    
    /// <summary>
    /// The item page
    /// </summary>
    public ItemPage ItemPage { get; private set; }

    /// <summary>
    /// The current row in the list part
    /// </summary>
    public DataRow ListCurrentRow { get; protected set; } //=> Module?.tblList?.CurrentRow;

    /// <summary>
    /// The current row in the item part
    /// </summary>
    public DataRow CurrentRow => Module?.tblItem?.CurrentRow;
    /// <summary>
    /// Returns true if this is a fixed (no row insert-delete allowed) form.
    /// </summary>
    public bool IsReadOnlyForm => !FormDef.IsReadOnly;
    /// <summary>
    /// When true then this is a form with a fixed single select.
    /// </summary>
    public bool IsSingleSelect => ModuleDef.IsSingleSelect;
    
    // ● events
    public event EventHandler CurrentRowChanged;


}
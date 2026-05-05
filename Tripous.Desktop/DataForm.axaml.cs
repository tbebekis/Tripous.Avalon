namespace Tripous.Desktop;

public partial class DataForm : AppForm
{
    protected DataFormState fFormState = DataFormState.None;
    protected DataFormAction LastAction = DataFormAction.None;
    
    protected bool Saving;
    protected bool fIdColumnsVisible = false;
    protected bool fFiltersSideBarVisible = true;
    
    protected ToolBar ToolBar;

    protected Button btnHome;
    protected Border sepHome;
    
    protected Button btnList;
    protected Button btnRefreshList;
    protected Button btnFind;
    protected ToggleButton btnToggleIds;
    protected Border sepList;
    
    protected Button btnInsert;
    protected Button btnEdit;
    protected Button btnDelete;
    protected Border sepEdit;

    protected Button btnSave;
    protected Border sepSave;

    protected Button btnCancel;
    protected Button btnOK;
    protected Border sepCancelOK;
    
    protected Button btnClose;
    
    protected ToolBar SelectListToolBar;
    protected ComboBox cboSelectList;

    protected SqlFilterPanelHandler FilterPanelHandler;
 
    // ● event handlers
    void gridList_OnDoubleTapped(object sender, TappedEventArgs e)
    { 
        _ = Execute(DataFormAction.Edit);
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
 
    // ● initialization
    /// <summary>
    /// Executes any first command on the form
    /// </summary>
    protected override async Task Start()
    {        
        gridList.DoubleTapped += gridList_OnDoubleTapped;
        
        CreateToolBar();
        CreateSelectListToolBar();
        CreateItemPanel();
        FilterPanelHandler = new(pnlFilters);
        
        ListIsDirty = true;
        FiltersSideBarVisible = false;
 
        if (StartAction.In(DataFormAction.List | DataFormAction.Edit | DataFormAction.Insert))
        {
            if (StartAction == DataFormAction.List)
                SelectedSelectChanged();
            await Execute(StartAction);
        }
    }
    
    // ● form state
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
                case DataFormAction.ToggleIds:
                    ExecuteToggleIds();
                    break;
                case DataFormAction.RefreshList:
                    await ExecuteRefreshList();
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
    protected virtual bool Executing(DataFormAction Value) => false;
    protected virtual void Executed(DataFormAction Value) =>  LastAction = Value;

    protected virtual void ExecuteHome() => FormState = DataFormState.List;
    protected virtual void ExecuteFind() => FiltersSideBarVisible = !FiltersSideBarVisible;

    protected virtual void ExecuteToggleIds() => IdColumnsVisible = !IdColumnsVisible;

    protected virtual async Task ExecuteList()
    {
        await Task.CompletedTask;
        
        if (!Saving && FormState.In(DataFormState.Insert |DataFormState.Edit))  
        {
            if (!await ExecuteCancelEdit())
                return;
        }
        
        if (ListIsDirty)  
            ListSelect();
        
        this.FormState = DataFormState.List;
    }
    protected virtual async Task ExecuteRefreshList()
    {
        ListIsDirty = true;
        await ExecuteList();
    }
    
    protected virtual void ExecuteInsert()
    {
        Insert();
        this.FormState = DataFormState.Insert;
    }
    protected virtual void ExecuteEdit(object oId = null)
    {
        if (oId == null)
            oId = GetCurrentListId();

        if (!Sys.IsNull(oId))
        {
            Load(oId);
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
                Delete(oId);
            }
        }
    }
    protected virtual void ExecuteSave()
    {
        Saving = true;
        try
        {
            Save();
        }
        finally
        {
            Saving = false;
        }

        this.FormState = DataFormState.Edit;
    }
    protected virtual async Task<bool> ExecuteCancelEdit()
    {
        if (HasChanges())
        {
            if (!await MessageBox.YesNo("Cancel changes?"))
                return false;

            CancelChanges();
        }

        return true;
    }
 
    // ● list
    protected virtual void ListSelect()
    {
        SelectDef SelectDef = cboSelectList.SelectedItem as SelectDef;
        if (SelectDef != null)
        {
            object LastOID = GetCurrentListId();
            
            UnBindListGrid();

            string SqlText = SelectDef.SqlText;
            string Where = FilterPanelHandler.GetWhere();
            if (!string.IsNullOrWhiteSpace(Where))
                SqlText = $"select * from ({SqlText}) X where {Where}";

            cboSelectList.Focus();
 
            Ui.Post(() => 
            {
                Module.ListSelect(SqlText);
                ListIsDirty = false;
                BindListGrid();
                ApplyIdColumnsVisible();
                
                LogBox.AppendLine(SqlText);
                
                GoToListOID(LastOID);
                
            });
        }
    }

    protected virtual bool GoToListOID(object oId)
    {
        DataView DataView = Module.tblList.DataView;
        
        if (!Sys.IsNull(oId))
        {
            object RowOID;
            if (Module.tblItem.DataView.Table.ContainsColumn(Module.tblItem.KeyField))
            {
                string FieldName = Module.tblItem.KeyField;
                
                foreach (DataRowView DRV in DataView)
                {
                    RowOID = DRV[FieldName];
                    if (object.Equals(RowOID, oId))
                    {
                        gridList.SelectedItem = DRV;
                        return true;
                    }
                }
            }
        }

        if (DataView.Count > 0)
            gridList.SelectedItem = DataView[0];

        return false;
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
  
    // ● item
    protected virtual void Insert() => Module.Insert();
    protected virtual void Load(object oId) => Module.Edit(oId);
    protected virtual void Delete(object oId) 
    {
        Module.Delete(oId);
        ListIsDirty = true; 
    }
    protected virtual void Save() 
    {
        Module.Commit(Reselect: false);
        ListIsDirty = true; 
    }

    protected virtual bool HasChanges() => Module.HasChanges();
    protected virtual void CancelChanges()
    {
        Module.Cancel();
        ListIsDirty = false;
    }

    // ● UI
    protected virtual void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;

            btnHome = ToolBar.AddButton("application_home.png", "Home", async () => await Execute(DataFormAction.Home));
            sepHome  = ToolBar.AddSeparator();
                
            btnList = ToolBar.AddButton("table.png", "List", async () => await Execute(DataFormAction.List));
            btnRefreshList = ToolBar.AddButton("table_refresh.png", "Refresh List", async () => await Execute(DataFormAction.RefreshList));
            btnFind = ToolBar.AddButton("find.png", "Find", async () => await Execute(DataFormAction.Find));
            btnToggleIds = ToolBar.AddToggleButton("table_select_row.png", "Toggle Ids", async () => await Execute(DataFormAction.ToggleIds));
            sepList  = ToolBar.AddSeparator(); // sepEdit
            
            btnInsert = ToolBar.AddButton("table_add.png", "Insert", async () => await Execute(DataFormAction.Insert));
            btnEdit = ToolBar.AddButton("table_edit.png", "Edit", async () => await Execute(DataFormAction.Edit));
            btnDelete = ToolBar.AddButton("table_delete.png", "Delete", async () => await Execute(DataFormAction.Delete));
            sepEdit = ToolBar.AddSeparator(); // sepEdit
            
            btnSave = ToolBar.AddButton("disk.png", "Save", async () => await Execute(DataFormAction.Save));
            sepSave = ToolBar.AddSeparator(); // sepSave

            btnCancel = ToolBar.AddButton("cancel.png", "Cancel", async () => await Execute(DataFormAction.Cancel));
            btnOK = ToolBar.AddButton("accept.png", "OK", async () => await Execute(DataFormAction.Ok));
            sepCancelOK = ToolBar.AddSeparator(); // sepCancelOK
            
            btnClose = ToolBar.AddButton("door_out.png", "Close", async () => await Execute(DataFormAction.Close));  
        }
    }
    protected virtual void CreateSelectListToolBar()
    {
        if (SelectListToolBar == null)
        {
            SelectListToolBar = new();
            SelectListToolBar.Panel = pnlSelectListToolBar;

            cboSelectList = SelectListToolBar.AddComboBox(ModuleDef.SelectList, 0, 150.0);
            cboSelectList.SelectionChanged += (s, ea) => SelectedSelectChanged();

            SelectListToolBar.AddButton("lightning.png", "Execute", async () => await Execute(DataFormAction.RefreshList));
            SelectListToolBar.AddButton("textfield_clear.png", "Clear Filter", () => FilterPanelHandler.Clear());
        }
    }

    protected virtual void BindListGrid() => DataGridBinder.BindGrid(gridList, Module.tblList.DataView, SupportsRecycling: false, GoToFirst: true);
    protected virtual void UnBindListGrid() => DataGridBinder.UnBindGrid(gridList);
 
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
        btnHome.IsVisible = true;
        sepHome.IsVisible = true;
        
        btnList.IsVisible = true;
        btnRefreshList.IsVisible = true;
        btnFind.IsVisible = !IsSingleSelect;
        btnToggleIds.IsVisible = true;
        sepList.IsVisible = btnHome.IsVisible || btnList.IsVisible || btnRefreshList.IsVisible || btnFind.IsVisible || btnToggleIds.IsVisible;
        
        btnInsert.IsVisible = IsEditableForm;
        btnEdit.IsVisible = IsEditableForm;
        btnDelete.IsVisible = IsEditableForm;
        sepEdit.IsVisible = IsEditableForm;
        
        btnSave.IsVisible = IsEditableForm;
        sepSave.IsVisible = btnSave.IsVisible;

        btnCancel.IsVisible = true;                       // btnCancel - visible with all form modes
        btnOK.IsVisible = IsModal;
        sepCancelOK.IsVisible = btnCancel.IsVisible || btnOK.IsVisible;
            
        btnClose.IsVisible = !IsModal;      // btnClose - visible with non-list master forms
        
        // ● enable ================================================================
        btnHome.IsEnabled = btnHome.ContextMenu != null && btnHome.ContextMenu.Items.Count > 0;
        
        btnList.IsEnabled = !DataFormAction.List.In(InvalidActions);
        btnRefreshList.IsEnabled = btnList.IsEnabled;
        btnFind.IsEnabled = !DataFormAction.Find.In(InvalidActions) && FormState == DataFormState.List;
        btnToggleIds.IsEnabled = FormState == DataFormState.List;
        
        btnInsert.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && FormState.In(DataFormState.List | DataFormState.Edit);
        btnEdit.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && FormState.In(DataFormState.List) && !IsListEmpty; ;
        btnDelete.IsEnabled = !DataFormAction.Delete.In(InvalidActions) && !IsListEmpty;
        btnSave.IsEnabled = FormState.In(DataFormState.Insert | DataFormState.Edit);
        
        // Edit states: cancels edits and returns to List state
        // List state and Modal: cancels the form
        btnCancel.IsEnabled = FormState.In(DataFormState.Insert | DataFormState.Edit) || (IsModal && FormState == DataFormState.List);
 
        // btnOK - accessible in List state only with modal forms
        // List state and Modal: closes the form with OK and returns the current row               
        btnOK.IsEnabled = IsModal && FormState == DataFormState.List;

        // List state: closes a non-modal form                
        btnClose.IsEnabled = FormState == DataFormState.List;
    }
    /// <summary>
    /// Enables and disables controls.
    /// </summary>
    protected virtual void EnableControls()
    {
        // ● visible ===============================================================
        pnlSideBar.IsVisible = !IsSingleSelect && FiltersSideBarVisible;
        Splitter.IsVisible = pnlSideBar.IsVisible;

        // ● enable ================================================================
        gridList.IsReadOnly = true;
    }
    /// <summary>
    /// Applies the visibility of list DataGrid columns ending with ID 
    /// </summary>
    protected virtual void ApplyIdColumnsVisible()
    {
        Ui.Post(() =>
        {
            bool Flag = IdColumnsVisible; // Checked = hide, else = show
            string S = Flag ? "Hide Ids" : "Show Ids";
            ToolTip.SetTip(btnToggleIds, S);
        
            List<GridColumnInfo> List = gridList.GetInfoList();
            foreach (var CI in List)
            {
                if (CI.FieldName.EndsWithText("Id"))
                    CI.GridColumn.IsVisible = Flag;
            }
        });
    }
    /// <summary>
    /// Called when another named SELECT is selected in the combobox with the select list.
    /// </summary>
    protected virtual void SelectedSelectChanged()
    {
        SelectDef SelectDef = cboSelectList.SelectedItem as SelectDef;
        if (SelectDef != null && SelectDef.UseFilters)
        {
            SqlFilterDefs FilterDefs = null;
            if (SelectDef.FilterDefs == null || SelectDef.FilterDefs.Count == 0)
                FilterDefs = SelectDef.DefineFilters(Module.Name, Module.Store);
            else
                FilterDefs = SelectDef.FilterDefs;

            FilterDefs = GetSavedFilterValues(SelectDef, FilterDefs);
            FilterPanelHandler.CreateFilterControls(FilterDefs);
        }
    }
    /// <summary>
    /// Creates and returna a new <see cref="SqlFilterDefs"/> instance with the saved filter values of the current user.
    /// </summary>
    protected virtual SqlFilterDefs GetSavedFilterValues(SelectDef SelectDef, SqlFilterDefs FilterDefs)
    {
        return FilterDefs;
    }
 
    
    /// <summary>
    /// It is called when the escape key is pressed. 
    /// <para>Returning true indicates that the key press is handled.</para>
    /// <para>NOTE: By default, when is a modal dialog, it sets <see cref="ModalResult"/> to Cancel, and closes the form.</para>
    /// </summary>
    protected override bool ProcessEscapeKey()
    {
        if (!IsModal && this.FormState == DataFormState.List)
        {
            this.CloseForm();
            return true;
        }

        return base.ProcessEscapeKey();
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
    /// <para>This setting comes from the <see cref="DataFormContext"/> which creates the form. </para>
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;
    /// <summary>
    /// The item page
    /// </summary>
    public ItemPage ItemPage { get; protected set; }

    /// <summary>
    /// The current row in the list part
    /// </summary>
    public DataRow ListCurrentRow => (gridList.SelectedItem is DataRowView RowView) ? RowView.Row : null;//{ get; protected set; }  
    /// <summary>
    /// The current row in the item part
    /// </summary>
    public DataRow CurrentRow => Module?.tblItem?.CurrentRow;
    /// <summary>
    /// The state of a data-form indicates the UI the form is currently displaying
    /// </summary>
    public virtual DataFormState FormState
    {
        get { return fFormState; }
        protected set
        {
            if (value != fFormState)
            {
                fFormState = value;
                FormStateChanged();
                UpdateUi();
            }
        }
    }
    
    /// <summary>
    /// Returns true if this is a form where insert-edit-delete is NOT allowed 
    /// </summary>
    public bool IsReadOnlyForm => FormDef.IsReadOnly;
    /// <summary>
    /// Returns true if this is a form where insert-edit-delete is allowed 
    /// </summary>
    public bool IsEditableForm => !IsReadOnlyForm;
    /// <summary>
    /// When true then this is a form with a fixed single select.
    /// </summary>
    public bool IsSingleSelect => ModuleDef.IsSingleSelect;
    /// <summary>
    /// True when the list grid/table is empty.
    /// </summary>
    public bool IsListEmpty => Module == null || Module.tblList == null || Module.tblList.Rows.Count == 0;

    public bool ListIsDirty { get; protected set; }

    /// <summary>
    /// Toggles visibility of list DataGrid columns ending with ID 
    /// </summary>
    public bool IdColumnsVisible
    {
        get => fIdColumnsVisible;
        set
        {
            if (fIdColumnsVisible != value)
            {
                fIdColumnsVisible = value;
                ApplyIdColumnsVisible();
            }
        }
    }
    /// <summary>
    /// When true the filters sidebar is visible.
    /// </summary>
    public bool FiltersSideBarVisible
    {
        get => fFiltersSideBarVisible;
        set
        {
            if (fFiltersSideBarVisible != value)
            {
                fFiltersSideBarVisible = value;
                Ui.Post(() =>
                {
                    pnlSideBar.IsVisible = !IsSingleSelect && value;
                    Splitter.IsVisible = !IsSingleSelect && value;
                    
                    pnlList.ColumnDefinitions[0].Width = value ? new GridLength(250) : new GridLength(0);
                    pnlList.ColumnDefinitions[0].MinWidth = pnlList.ColumnDefinitions[0].Width.Value;
                    pnlList.ColumnDefinitions[1].Width = value ? new GridLength(4) : new GridLength(0);
                });
            }
        }
    }
}
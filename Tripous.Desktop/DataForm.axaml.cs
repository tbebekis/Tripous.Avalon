/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// A data form with a List part and an Item part.
/// <para>
/// All user initiated form actions should enter through <see cref="Execute(DataFormAction)"/>.
/// The <c>ExecuteXXXX()</c> methods are the action handlers and they are the only methods that should
/// directly change, or eventually cause a change to, the <see cref="FormState"/>.
/// </para>
/// <para>
/// Low level data operation methods, such as <see cref="ListSelect"/>, <see cref="Insert"/>,
/// <see cref="Load"/>, <see cref="Delete"/> and <see cref="Save"/>, should be called only by the
/// corresponding <c>ExecuteXXXX()</c> method. This keeps UI commands, keyboard shortcuts and toolbar
/// buttons on the same execution path.
/// </para>
/// <para>
/// The cancel flow is intentionally split. <see cref="ExecuteCancel"/> handles the Cancel action.
/// <see cref="ExecuteCancelEdit"/> handles only rejecting item changes. When an item has changes,
/// Cancel asks for confirmation, rejects the changes, refreshes the item controls and stays in the
/// Item part. A second Cancel, with no changes left, returns to the List part.
/// </para>
/// <para>
/// Startup is the only intentional exception to the general rule. <see cref="ExecuteStartAction"/>
/// may call <c>ExecuteEdit(RowId)</c> or <c>ExecuteInsert()</c> directly and delayed through the UI
/// dispatcher. At startup the list part may not be loaded yet, so <c>Execute(Edit)</c> cannot always
/// resolve the desired row through the current list row. This exception is for bootstrapping only;
/// normal UI actions should still use <see cref="Execute(DataFormAction)"/>.
/// </para>
/// </summary>
public partial class DataForm : AppForm
{
    // ● protected fields
    /// <summary>
    /// The current form state.
    /// </summary>
    protected DataFormState fFormState = DataFormState.None;
    /// <summary>
    /// The last executed form action.
    /// </summary>
    protected DataFormAction LastAction = DataFormAction.None;
    /// <summary>
    /// True while a save operation is in progress.
    /// </summary>
    protected bool Saving;
    /// <summary>
    /// True when ID columns are visible.
    /// </summary>
    protected bool fIdColumnsVisible = false;
    /// <summary>
    /// True when the filters sidebar is visible.
    /// </summary>
    protected bool fFiltersSideBarVisible = true;
    /// <summary>
    /// The list row identifier to select after a list refresh.
    /// </summary>
    protected object fListTargetId;
    /// <summary>
    /// The main form toolbar.
    /// </summary>
    protected ToolBar ToolBar;
    /// <summary>
    /// The Home toolbar button.
    /// </summary>
    protected Button btnHome;
    /// <summary>
    /// The Home toolbar separator.
    /// </summary>
    protected Border sepHome;
    /// <summary>
    /// The List toolbar button.
    /// </summary>
    protected Button btnList;
    /// <summary>
    /// The Refresh List toolbar button.
    /// </summary>
    protected Button btnRefreshList;
    /// <summary>
    /// The Find toolbar button.
    /// </summary>
    protected Button btnFind;
    /// <summary>
    /// The Toggle Ids toolbar button.
    /// </summary>
    protected ToggleButton btnToggleIds;
    /// <summary>
    /// The List toolbar separator.
    /// </summary>
    protected Border sepList;
    /// <summary>
    /// The Insert toolbar button.
    /// </summary>
    protected Button btnInsert;
    /// <summary>
    /// The Edit toolbar button.
    /// </summary>
    protected Button btnEdit;
    /// <summary>
    /// The Delete toolbar button.
    /// </summary>
    protected Button btnDelete;
    /// <summary>
    /// Reloads the current item from the database.
    /// </summary>
    protected Button btnRefresh;
    /// <summary>
    /// The edit toolbar separator.
    /// </summary>
    protected Border sepEdit;
    /// <summary>
    /// The Save toolbar button.
    /// </summary>
    protected Button btnSave;
    /// <summary>
    /// The Save toolbar separator.
    /// </summary>
    protected Border sepSave;
    /// <summary>
    /// The Cancel toolbar button.
    /// </summary>
    protected Button btnCancel;
    /// <summary>
    /// The OK toolbar button.
    /// </summary>
    protected Button btnOK;
    /// <summary>
    /// The Cancel/OK toolbar separator.
    /// </summary>
    protected Border sepCancelOK;
    /// <summary>
    /// The Close toolbar button.
    /// </summary>
    protected Button btnClose;
    /// <summary>
    /// The select list toolbar.
    /// </summary>
    protected ToolBar SelectListToolBar;
    /// <summary>
    /// The select list combo box.
    /// </summary>
    protected ComboBox cboSelectList;
    /// <summary>
    /// The SQL filter panel handler.
    /// </summary>
    protected SqlFilterPanelHandler FilterPanelHandler;
 
    // ● event handlers
    /// <summary>
    /// Handles a list grid double-tap and executes the Edit action.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The tapped event arguments.</param>
    void gridList_OnDoubleTapped(object sender, TappedEventArgs e)
    { 
        _ = Execute(DataFormAction.Edit);
    }
 
    // ● shortcuts
    /// <summary>
    /// Executes a shortcut action when the associated button is available.
    /// </summary>
    /// <param name="Button">The toolbar button associated with the shortcut.</param>
    /// <param name="Action">The form action to execute.</param>
    /// <returns>True if the shortcut was handled; otherwise, false.</returns>
    protected virtual bool ExecuteShortcut(Button Button, DataFormAction Action)
    {
        if (Button == null || !Button.IsVisible || !Button.IsEnabled)
            return false;

        Ui.Post(async () => await Execute(Action));
        return true;
    }
    /// <summary>
    /// Processes a keyboard shortcut.
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    /// <returns>True if the shortcut was handled; otherwise, false.</returns>
    protected virtual bool ProcessShortcutKey(KeyEventArgs e)
    {
        if (e.KeyModifiers == KeyModifiers.None)
        {
            switch (e.Key)
            {
                case Key.F5:
                    return ExecuteShortcut(btnList, DataFormAction.List);
            }
        }
        else if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e.Key)
            {
                case Key.F5:
                    return ExecuteShortcut(btnRefreshList, DataFormAction.RefreshList);
                case Key.F:
                    return ExecuteShortcut(btnFind, DataFormAction.Find);
                case Key.Insert:
                    return ExecuteShortcut(btnInsert, DataFormAction.Insert);
                case Key.Enter:
                    if (IsModal && FormState == DataFormState.List)
                        return ExecuteShortcut(btnOK, DataFormAction.Ok);
                    return ExecuteShortcut(btnEdit, DataFormAction.Edit);
                case Key.Delete:
                    return ExecuteShortcut(btnDelete, DataFormAction.Delete);
                case Key.S:
                    return ExecuteShortcut(btnSave, DataFormAction.Save);
            }
        }

        return false;
    }

    // ● overrides
    /// <summary>
    /// Processes key down events.
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    /// <returns>True if the key was processed; otherwise, false.</returns>
    protected override bool ProcessKeyDown(KeyEventArgs e)
    {
        if (!Design.IsDesignMode && ProcessShortcutKey(e))
            return true;

        return base.ProcessKeyDown(e);
    }
    /// <summary>
    /// Called when the control is added to a rooted visual tree. 
    /// </summary>
    /// <param name="e">The visual tree attachment event arguments.</param>
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
            await ExecuteStartAction();
        }

        
    }
    
    // ● form state
    /// <summary>
    /// Called when the <see cref="FormState"/> property changes.
    /// </summary>
    protected virtual void FormStateChanged()
    { 
        if (gridList != null)
        {
            switch (FormState)
            {
                case DataFormState.List:
                    pnlItem.IsVisible = false;
                    pnlList.IsVisible = true;
                    Ui.Post(() => btnRefreshList.Focus());
                    break;
                case DataFormState.Insert:
                case DataFormState.Edit:
                    pnlList.IsVisible = false;
                    pnlItem.IsVisible = true;
                    KeyboardNavigation.SetTabNavigation(pnlItem, KeyboardNavigationMode.Cycle);
                    Ui.Post(() => FindFirstFocusableControl(pnlItem)?.Focus(NavigationMethod.Tab, KeyModifiers.None));
                    break;
            }
        } 
    }
 
    // ● form actions
    /// <summary>
    /// Executes the startup action.
    /// </summary>
    protected virtual async Task ExecuteStartAction()
    {
        if (StartAction == DataFormAction.Edit && !Sys.IsNull(DataFormContext.RowId))
        {
            fListTargetId = DataFormContext.RowId;
            Ui.Post(() =>
            {
                ExecuteEdit(DataFormContext.RowId);
                UpdateUi();
            });

            return;
        }
        else if (StartAction == DataFormAction.Insert)
        {
            Ui.Post(() =>
            {
                if (Module.State != DataMode.Insert)
                    ExecuteInsert();
                else
                    FormState = DataFormState.Insert;
                UpdateUi();
            });

            return;
        }

        await Execute(StartAction);
    }
    /// <summary>
    /// Executes a form action.
    /// </summary>
    /// <param name="Value">The action to execute.</param>
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
                case DataFormAction.Refresh:
                    await ExecuteRefresh();
                    break;
                case DataFormAction.Save:
                    await ExecuteSave();
                    break;
                
                case DataFormAction.Cancel:
                    await ExecuteCancel();
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
    /// <summary>
    /// Called before a form action is executed.
    /// </summary>
    /// <param name="Value">The action to execute.</param>
    /// <returns>True to cancel the default execution; otherwise, false.</returns>
    protected virtual bool Executing(DataFormAction Value) => false;
    /// <summary>
    /// Called after a form action is executed.
    /// </summary>
    /// <param name="Value">The executed action.</param>
    protected virtual void Executed(DataFormAction Value) =>  LastAction = Value;
    
    /// <summary>
    /// Executes a custom action.
    /// </summary>
    /// <param name="Value">The custom action value.</param>
    protected virtual async Task ExecuteCustom(object Value)
    {
        UpdateUi();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Executes the Home action.
    /// </summary>
    protected virtual void ExecuteHome() => FormState = DataFormState.List;
    /// <summary>
    /// Executes the Find action.
    /// </summary>
    protected virtual void ExecuteFind() => FiltersSideBarVisible = !FiltersSideBarVisible;
    /// <summary>
    /// Executes the Toggle Ids action.
    /// </summary>
    protected virtual void ExecuteToggleIds() => IdColumnsVisible = !IdColumnsVisible;
    /// <summary>
    /// Returns a text describing the current item for logging purposes.
    /// </summary>
    /// <param name="Id">The item identifier.</param>
    /// <returns>The item log text.</returns>
    protected virtual string GetItemLogText(object Id)
    {
        List<string> Parts = new();

        string FieldName = ModuleDef.ItemCaptionField;
        DataRow Row = IsInListState ? ListCurrentRow : CurrentRow;
        
        if (Row != null)
        {
            if (!FieldName.IsSameText("Code") && Row.Table.Columns.Contains("Code"))
            {
                object Code = Row["Code"];
                if (!Sys.IsNull(Code))
                    Parts.Add(Convert.ToString(Code, CultureInfo.CurrentCulture));
            }

            if (!string.IsNullOrWhiteSpace(FieldName) && Row.Table.Columns.Contains(FieldName))
            {
                object Caption = Row[FieldName];
                if (!Sys.IsNull(Caption))
                    Parts.Add(Convert.ToString(Caption, CultureInfo.CurrentCulture));
            }
        }

        string Result = "Current item";
        if (Parts.Count > 0)
            Result = string.Join(" - ", Parts);
        else if (!Sys.IsNull(Id))
            Result = Convert.ToString(Id, CultureInfo.CurrentCulture);
        return Result;
    }

    /// <summary>
    /// Executes the List action.
    /// </summary>
    protected virtual async Task ExecuteList()
    {
        if (!Saving && FormState.In(DataFormState.Insert |DataFormState.Edit))  
        {
            if (!await ExecuteCancelEdit())
                return;
        }
        
        if (ListIsDirty)  
            await ListSelect();
        
        this.FormState = DataFormState.List;
    }
    /// <summary>
    /// Executes the Refresh List action.
    /// </summary>
    protected virtual async Task ExecuteRefreshList()
    {
        ListIsDirty = true;
        await ExecuteList();
    }
    
    /// <summary>
    /// Executes the Insert action.
    /// </summary>
    protected virtual void ExecuteInsert()
    {
        Insert();
        this.FormState = DataFormState.Insert;
    }
    /// <summary>
    /// Executes the Edit action.
    /// </summary>
    /// <param name="oId">The row identifier to edit.</param>
    protected virtual void ExecuteEdit(object oId = null)
    {
        if (oId == null)
            oId = GetCurrentListId();

        if (!Sys.IsNull(oId))
        {
            Load(oId);
            UiLog($"Loaded {GetItemLogText(oId)}");
            ItemPage?.Binders.ForEach(Binder => Binder.Refresh());
            this.FormState = DataFormState.Edit;
        }
    }
    /// <summary>
    /// Executes the Delete action.
    /// </summary>
    /// <param name="oId">The row identifier to delete.</param>
    protected virtual async Task ExecuteDelete(object oId = null)
    {
        if (oId == null)
            oId = GetCurrentListId();

        if (!Sys.IsNull(oId))
        {
            string LogText = GetItemLogText(oId);
            
            if (await MessageBox.YesNo($"Delete item: {LogText}?", this))
            {
                Delete(oId);
                UiLog($"Deleted {LogText}");
                if (IsInListState)
                    await Execute(DataFormAction.RefreshList);
            }
        }
    }
    /// <summary>
    /// Executes the Save action.
    /// </summary>
    protected virtual async Task ExecuteSave()
    {
        Dictionary<GroupGrid, Tuple<int, GroupGridColumn>> DetailGridSelection = ItemPage?.CaptureDetailGridSelection();
        Saving = true;
        try
        {
            Save();
            fListTargetId = Module.LastCommitedId;
            UiLog($"Saved {GetItemLogText(Module.LastCommitedId)}");
            ItemPage?.RestoreDetailGridSelection(DetailGridSelection);
        }
        finally
        {
            Saving = false;
        }

        this.FormState = DataFormState.Edit;
        await Task.CompletedTask;
    }
    /// <summary>
    /// Confirms pending changes and reloads the current item from the database.
    /// </summary>
    protected virtual async Task ExecuteRefresh()
    {
        if (FormState != DataFormState.Edit || CurrentRow == null)
            return;

        object Id = CurrentRow[ModuleDef.Table.KeyField];
        if (HasChanges())
        {
            if (!await MessageBox.YesNo("Cancel changes and refresh the current item?", this))
                return;

            CancelChanges();
        }

        Refresh();
        UiLog($"Refreshed {GetItemLogText(Id)}");
    }
    /// <summary>
    /// Executes the Cancel action.
    /// </summary>
    protected virtual async Task ExecuteCancel()
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
            if (HasChanges())
                await ExecuteCancelEdit();
            else
                await ExecuteList();
        } 
    }
    /// <summary>
    /// Cancels item editing after optional confirmation.
    /// </summary>
    /// <returns>True if editing was cancelled or there were no changes; otherwise, false.</returns>
    protected virtual async Task<bool> ExecuteCancelEdit()
    {
        if (HasChanges())
        {
            if (IsEditableForm) // edit button is visible even when the form is read-only.
            {
                if (!await MessageBox.YesNo("Cancel changes?", this))
                    return false;
            }

            CancelChanges();
            ItemPage?.Refresh();
        }

        return true;
    }
 
    // ● list
    /// <summary>
    /// Selects the current list using the selected select definition and active filters.
    /// </summary>
    protected virtual async Task ListSelect()
    {
        SelectDef SelectDef = cboSelectList.SelectedItem as SelectDef;
        if (SelectDef != null)
        {
            object LastOID = !Sys.IsNull(fListTargetId) ? fListTargetId : GetCurrentListId();

            string SqlText = SelectDef.SqlText;
            string Where = FilterPanelHandler.GetWhere();
            if (!string.IsNullOrWhiteSpace(Where))
                SqlText = $"select * from ({SqlText}) X where {Where}";

            cboSelectList.Focus();
 
            await Dispatcher.UIThread.InvokeAsync(() => 
            {
                UnBindListGrid();
                Module.ListSelect(SqlText);
                ListIsDirty = false;
                BindListGrid(SelectDef);
                ApplyIdColumnsVisible();

                string Message = $@"List SELECT - Rows: {Module.tblList.Rows.Count}
{SqlText}
";
                UiLog(Message);
                
                GoToListOID(LastOID);
            });
        }
    }

    /// <summary>
    /// Selects a list row by identifier.
    /// </summary>
    /// <param name="oId">The row identifier.</param>
    /// <returns>True if the row was found and selected; otherwise, false.</returns>
    protected virtual bool GoToListOID(object oId)
    {
        DataView DataView = Module.tblList.DataView;
        
        if (!Sys.IsNull(oId))
        {
            object RowOID;
            if (Module.tblItem.DataView.Table.ContainsColumn(Module.tblItem.KeyField))
            {
                string FieldName = Module.tblItem.KeyField;
                
                for (int Index = 0; Index < DataView.Count; Index++)
                {
                    DataRowView DRV = DataView[Index];
                    RowOID = DRV[FieldName];
                    if (object.Equals(RowOID, oId))
                    {
                        GroupGridBinder.SelectRow(gridList, Index);
                        return true;
                    }
                }
            }
        }

        if (DataView.Count > 0)
            GroupGridBinder.SelectRow(gridList, 0);

        return false;
    }
    /// <summary>
    /// Returns the current list row identifier.
    /// </summary>
    /// <returns>The current list row identifier, if any; otherwise, null.</returns>
    protected virtual object GetCurrentListId()
    {
        if (ListCurrentRow != null)
        {
            if (ListCurrentRow.Table.Columns.Contains(ModuleDef.Table.KeyField))
                return ListCurrentRow[ModuleDef.Table.KeyField];
        }
        
        return null;
    }
    /// <summary>
    /// Passes modal result data back to the form context.
    /// </summary>
    protected override void PassResultBack()
    {
        if (!IsModal || DataFormContext == null || ModalResult != ModalResult.Ok)
            return;

        object ResultData = Module?.LastCommitedId;
        if (Sys.IsNull(ResultData))
            ResultData = GetCurrentListId();

        DataFormContext.ResultData = ResultData;
    }
  
    // ● item
    /// <summary>
    /// Inserts a new item.
    /// </summary>
    protected virtual void Insert() => Module.Insert();
    /// <summary>
    /// Loads an item for editing.
    /// </summary>
    /// <param name="oId">The row identifier.</param>
    protected virtual void Load(object oId) => Module.Edit(oId);
    /// <summary>
    /// Deletes an item.
    /// </summary>
    /// <param name="oId">The row identifier.</param>
    protected virtual void Delete(object oId) 
    {
        Module.Delete(oId);
        ListIsDirty = true; 
    }
    /// <summary>
    /// Saves the current item.
    /// </summary>
    protected virtual void Save() 
    {
        Module.Commit(Reselect: false);
        ListIsDirty = true; 
    }
    /// <summary>
    /// Reloads the current item and refreshes its controls.
    /// </summary>
    protected virtual void Refresh()
    {
        if (CurrentRow == null)
            return;

        object Id = CurrentRow[ModuleDef.Table.KeyField];
        Load(Id);
        ListIsDirty = true;
        ItemPage?.Refresh();
    }

    /// <summary>
    /// Returns true when the data module has changes.
    /// </summary>
    /// <returns>True if the data module has changes; otherwise, false.</returns>
    protected virtual bool HasChanges() => Module.HasChanges();
    /// <summary>
    /// Cancels changes in the data module.
    /// </summary>
    protected virtual void CancelChanges()
    {
        Module.Cancel();
    }
    /// <summary>
    /// Returns true when a detail grid command can execute.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    public virtual bool CanExecuteGridCommand(GridCommandContext Context) => true;

    // ● UI
    /// <summary>
    /// Creates the main toolbar.
    /// </summary>
    /// <returns>True if the toolbar was created; otherwise, false.</returns>
    protected virtual bool CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;

            btnHome = ToolBar.AddButton("application_home.png", "Home", async () => await Execute(DataFormAction.Home));
            sepHome  = ToolBar.AddSeparator();
                
            btnList = ToolBar.AddButton("table.png", "List (F5)", async () => await Execute(DataFormAction.List));
            btnRefreshList = ToolBar.AddButton("table_refresh.png", "Refresh List (Ctrl+F5)", async () => await Execute(DataFormAction.RefreshList));
            btnFind = ToolBar.AddButton("find.png", "Find (Ctrl+F)", async () => await Execute(DataFormAction.Find));
            btnToggleIds = ToolBar.AddToggleButton("table_select_row.png", "Toggle Ids", async () => await Execute(DataFormAction.ToggleIds));
            sepList  = ToolBar.AddSeparator(); // sepEdit
            
            btnInsert = ToolBar.AddButton("table_add.png", "Insert (Ctrl+Insert)", async () => await Execute(DataFormAction.Insert));
            btnEdit = ToolBar.AddButton("table_edit.png", "Edit (Ctrl+Enter)", async () => await Execute(DataFormAction.Edit));
            btnDelete = ToolBar.AddButton("table_delete.png", "Delete (Ctrl+Delete)", async () => await Execute(DataFormAction.Delete));
            btnRefresh = ToolBar.AddButton("table_refresh.png", "Refresh Item", async () => await Execute(DataFormAction.Refresh));
            sepEdit = ToolBar.AddSeparator(); // sepEdit
            
            btnSave = ToolBar.AddButton("disk.png", "Save (Ctrl+S)", async () => await Execute(DataFormAction.Save));
            sepSave = ToolBar.AddSeparator(); // sepSave

            btnCancel = ToolBar.AddButton("cancel.png", "Cancel (Escape)", async () => await Execute(DataFormAction.Cancel));
            btnOK = ToolBar.AddButton("accept.png", "OK (Ctrl+Enter)", async () => await Execute(DataFormAction.Ok));
            sepCancelOK = ToolBar.AddSeparator(); // sepCancelOK
            
            btnClose = ToolBar.AddButton("door_out.png", "Close", async () => await Execute(DataFormAction.Close));

            return true;
        }

        return false;
    }
    /// <summary>
    /// Creates the select list toolbar.
    /// </summary>
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

    /// <summary>
    /// Binds the list grid to the list table data view.
    /// </summary>
    /// <param name="SelectDef">The select definition.</param>
    protected virtual void BindListGrid(SelectDef SelectDef)
    {
        gridList.SettingsSuggestedFileName = $"{ModuleDef.Name}-ListGrid.json";
        GroupGridBinder.BindGrid(SelectDef, gridList, Module.tblList.DataView, GoToFirst: true);
    }
    /// <summary>
    /// Unbinds the list grid.
    /// </summary>
    protected virtual void UnBindListGrid() => GroupGridBinder.UnBindGrid(gridList);
    /// <summary>
    /// Returns the first editable focus control in a container.
    /// </summary>
    /// <param name="Container">The container to search.</param>
    /// <returns>The first editable focus control, if any; otherwise, null.</returns>
    protected override Control FindFirstFocusableControl(Control Container)
    {
        if (Container == null)
            return null;

        foreach (Control Control in Container.GetVisualDescendants().OfType<Control>())
        {
            if (IsEditableFocusControl(Control))
                return Control;
        }

        return null;
    }
    /// <summary>
    /// Returns true when a control can receive edit focus.
    /// </summary>
    /// <param name="Control">The control to check.</param>
    /// <returns>True if the control can receive edit focus; otherwise, false.</returns>
    protected virtual bool IsEditableFocusControl(Control Control)
    {
        if (Control == null || !Control.IsEffectivelyVisible || !Control.IsEnabled)
            return false;

        if (Control is TextBox TextBox)
            return !TextBox.IsReadOnly && TextBox.Focusable;
        if (Control is ComboBox ComboBox)
            return ComboBox.Focusable;
        if (Control is CalendarDatePicker CalendarDatePicker)
            return CalendarDatePicker.Focusable;
        if (Control is CheckBox CheckBox)
            return CheckBox.Focusable;
        if (Control is LocatorBox LocatorBox)
            return LocatorBox.Focusable;

        return false;
    }
    /// <summary>
    /// Focuses the previous editable control in the item panel.
    /// </summary>
    /// <param name="Current">The current control.</param>
    /// <returns>True if focus moved; otherwise, false.</returns>
    public virtual bool FocusPreviousEditableControl(Control Current)
    {
        // Used by CalendarDatePicker Shift+Tab workaround.
        if (Current == null || pnlItem == null)
            return false;

        List<Control> Controls = pnlItem.GetVisualDescendants()
            .OfType<Control>()
            .Where(IsEditableFocusControl)
            .ToList();

        if (Controls.Count == 0)
            return false;

        int Index = Controls.IndexOf(Current);
        if (Index < 0)
            return false;

        int PreviousIndex = Index > 0 ? Index - 1 : Controls.Count - 1;
        return Controls[PreviousIndex].Focus(NavigationMethod.Tab, KeyModifiers.Shift);
    }
 
    /// <summary>
    /// Creates the item panel.
    /// </summary>
    protected virtual void CreateItemPanel()
    {
        if (!string.IsNullOrWhiteSpace(FormDef.ItemClassName))
        {
            ItemPage = TypeStore.CreateInstance<ItemPage>(FormDef.ItemClassName);
            ItemPage.DataForm = this;

            pnlItem.Children.Clear();
            pnlItem.Children.Add(ItemPage);

            ItemPage.Bind();
            ItemPage.ApplyIdColumnsVisible(IdColumnsVisible);
        }
    }
    
    /// <summary>
    /// Updates the user interface, title, enable-disable buttons etc.
    /// </summary>
    public virtual void UpdateUi()
    {
        Ui.Post(() =>
        {
            EnableCommands();
            EnableControls();
        });
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
        btnFind.IsVisible = ModuleDef.UseFilters;
        btnToggleIds.IsVisible = true;
        sepList.IsVisible = btnHome.IsVisible || btnList.IsVisible || btnRefreshList.IsVisible || btnFind.IsVisible || btnToggleIds.IsVisible;
        
        btnInsert.IsVisible = IsEditableForm;
        btnEdit.IsVisible = true; // but it can be saved.
        btnDelete.IsVisible = IsEditableForm;
        btnRefresh.IsVisible = true;
        sepEdit.IsVisible = btnInsert.IsVisible || btnEdit.IsVisible || btnDelete.IsVisible || btnRefresh.IsVisible;
        
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
        btnToggleIds.IsEnabled = true;
        
        btnInsert.IsEnabled = IsEditableForm && !DataFormAction.Insert.In(InvalidActions) && FormState.In(DataFormState.List | DataFormState.Edit);
        btnEdit.IsEnabled = !DataFormAction.Insert.In(InvalidActions) && FormState.In(DataFormState.List) && !IsListEmpty; 
        btnDelete.IsEnabled = IsEditableForm && !DataFormAction.Delete.In(InvalidActions) && FormState.In(DataFormState.List) && !IsListEmpty;
        btnRefresh.IsEnabled = !DataFormAction.Refresh.In(InvalidActions) && FormState == DataFormState.Edit && CurrentRow != null;
        btnSave.IsEnabled = IsEditableForm && FormState.In(DataFormState.Insert | DataFormState.Edit);
        
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
        pnlSideBar.IsVisible = FiltersSideBarVisible;
        Splitter.IsVisible = pnlSideBar.IsVisible;

        // ● enable ================================================================
        gridList.IsReadOnly = true;
    }
    /// <summary>
    /// Applies the visibility of list GroupGrid columns ending with ID.
    /// </summary>
    protected virtual void ApplyIdColumnsVisible()
    {
        Ui.Post(() =>
        {
            bool Flag = IdColumnsVisible; // Checked = hide, else = show
            string S = Flag ? "Hide Ids" : "Show Ids";
            ToolTip.SetTip(btnToggleIds, S);
        
            List<GroupGridColumnBinding> List = gridList.GetInfoList();
            foreach (GroupGridColumnBinding CI in List)
            {
                if (CI.IsPlainId)
                    gridList.SetColumnVisible(CI.GridColumn, Flag);
            }

            ItemPage?.ApplyIdColumnsVisible(Flag);
        });
    }
    /// <summary>
    /// Called when another named SELECT is selected in the combobox with the select list.
    /// </summary>
    protected virtual void SelectedSelectChanged()
    {
        SelectDef SelectDef = cboSelectList.SelectedItem as SelectDef;
        if (SelectDef != null && (SelectDef.UseFilters || ModuleDef.UseFilters))
        {
            SqlFilterDefs FilterDefs = null;
            if (SelectDef.FilterDefs == null || SelectDef.FilterDefs.Count == 0)
                FilterDefs = SelectDef.DefineFilters(Module.Name, Module.Store);
            else
                FilterDefs = SelectDef.FilterDefs;

            SelectDef.ValidateBooleanFilterTypes(Module.Name, Module.Store, FilterDefs);
            FilterDefs = GetSavedFilterValues(SelectDef, FilterDefs);
            FilterPanelHandler.CreateFilterControls(FilterDefs);
        }
    }
    /// <summary>
    /// Creates and returns a new <see cref="SqlFilterDefs"/> instance with the saved filter values of the current user.
    /// </summary>
    /// <param name="SelectDef">The select definition.</param>
    /// <param name="FilterDefs">The filter definitions.</param>
    /// <returns>The filter definitions with saved values applied.</returns>
    protected virtual SqlFilterDefs GetSavedFilterValues(SelectDef SelectDef, SqlFilterDefs FilterDefs)
    {
        return FilterDefs;
    }
 
    /// <summary>
    /// It is called when the escape key is pressed. 
    /// <para>Returning true indicates that the key press is handled.</para>
    /// <para>NOTE: By default, when is a modal dialog, it sets <see cref="ModalResult"/> to Cancel, and closes the form.</para>
    /// </summary>
    /// <returns>True if the escape key was processed; otherwise, false.</returns>
    protected override bool ProcessEscapeKey()
    {
        if (btnCancel.IsVisible && btnCancel.IsEnabled)
        {
            Ui.Post(async () => await Execute(DataFormAction.Cancel));
            return true;
        }

        return base.ProcessEscapeKey();
    }

    /// <summary>
    /// Writes a message to the UI log when data form logging is enabled.
    /// </summary>
    /// <param name="Message">The message to write.</param>
    protected virtual void UiLog(string Message)  
    {
        if (Ui.Settings.ShowDataFormLog)
        {
            LogBox.AppendLine($"[{TitleText}] - {Message}");
        }
    }
 
        
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
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
    public DataRow ListCurrentRow => GroupGridBinder.GetCurrentRowView(gridList)?.Row;//{ get; protected set; }
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
    /// True when the form is in list state
    /// </summary>
    public virtual bool IsInListState => FormState == DataFormState.List;
    
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
    /// <summary>
    /// True when the list should be reloaded.
    /// </summary>
    public bool ListIsDirty { get; protected set; }

    /// <summary>
    /// Toggles visibility of list GroupGrid columns ending with ID.
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

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 
/// <summary>
/// The item part of a <see cref="DataForm"/>
/// </summary>
[TypeStore]
public class ItemPage : UserControl, IReferenceContextMenuHost, IGridHandler
{
    // ● protected fields
    protected UiItemContext Context;
    protected DataForm fDataForm;
    protected bool fIsReadOnly;
    protected Dictionary<DataGrid, bool> fGridReadOnlyStates = new();
    protected Dictionary<DataGridColumn, bool> fGridColumnReadOnlyStates = new();
 
    // ● protected methods
    protected virtual bool IsBindingReadOnly(TripousBinding Binding)
    {
        if (!DataForm.IsEditableForm)
            return true;
        if (Binding == null)
            return false;
        if (Binding.DataColumn != null && Binding.DataColumn.ReadOnly)
            return true;

        FieldDef Field = Binding.FieldDef;
        if (Field == null)
            return false;

        return Field.IsReadOnly
               || Field.IsReadOnlyUI
               || (Field.IsReadOnlyEdit && DataForm.FormState != DataFormState.Insert)
               || (Binding.LocatorDef != null && Binding.LocatorDef.IsReadOnly);
    }
    protected virtual void SetControlReadOnly(ControlBinding Binding, bool Value)
    {
        if (Binding == null || Binding.Control == null)
            return;

        bool IsReadOnly = Value || IsBindingReadOnly(Binding);
        switch (Binding.Control)
        {
            case LocatorBox Box:
                Box.IsReadOnly = IsReadOnly;
                break;
            case TextBox Box:
                Box.IsReadOnly = IsReadOnly;
                break;
            case Image:
                break;
            default:
                Binding.Control.IsEnabled = !IsReadOnly;
                break;
        }
    }
    protected virtual void SetGridReadOnly(UiDetailTableInfo DetailInfo, bool Value)
    {
        if (DetailInfo == null || DetailInfo.Grid == null)
            return;

        if (!fGridReadOnlyStates.ContainsKey(DetailInfo.Grid))
            fGridReadOnlyStates[DetailInfo.Grid] = DetailInfo.Grid.IsReadOnly;
        DetailInfo.Grid.IsReadOnly = Value || !DataForm.IsEditableForm || fGridReadOnlyStates[DetailInfo.Grid];
        foreach (GridColumnBinding Binding in DetailInfo.Grid.GetInfoList())
        {
            if (!fGridColumnReadOnlyStates.ContainsKey(Binding.GridColumn))
                fGridColumnReadOnlyStates[Binding.GridColumn] = Binding.GridColumn.IsReadOnly;

            bool IsReadOnly = Value
                              || !DataForm.IsEditableForm
                              || fGridColumnReadOnlyStates[Binding.GridColumn]
                              || (Binding.FieldDef != null
                                  && Binding.FieldDef.IsReadOnlyEdit
                                  && DataForm.FormState != DataFormState.Insert);
            Binding.GridColumn.IsReadOnly = IsReadOnly;
        }

        if (DetailInfo.ToolBarPanel == null)
            return;

        foreach (Button Button in DetailInfo.ToolBarPanel.Children.OfType<Button>())
        {
            if (Button.Tag is not GridCommand Command)
                continue;

            DetailGridCommandContext CommandContext = new()
            {
                Command = Command,
                Grid = DetailInfo.Grid,
                Table = DetailInfo.Table,
                DetailInfo = DetailInfo,
                ItemContext = Context
            };
            Button.IsEnabled = Command.IsEnabled && CanExecute(CommandContext);
        }
    }

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
            ControlBinding Binding = Binder.Bind(Box, Field);
            if (!Field.IsReadOnly && !Field.IsReadOnlyUI)
            {
                // context menu for lookup combo boxes and locator box controls.
                ReferenceContextMenu RefMenu = FormDef.CreateReferenceContextMenu();
                RefMenu.Initialize(this, Binding);
            }
            Result = Box;
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
            CalendarDatePicker Box = new()
            {
                SelectedDateFormat = CalendarDatePickerFormat.Custom,
                CustomDateFormatString = "yyyy-MM-dd"
            };
            Binder.Bind(Box, Field.Name, DataColumn, Field);
            Result = Box;
        }
        else
        {
            TextBox Box = new();
            if (Field.IsNumeric)
            {
                Box.TextAlignment = TextAlignment.Right;
                Binder.Bind(Box, Field.Name, DataColumn, Field);
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
        if (!string.IsNullOrWhiteSpace(Field.CodeProvider))
            Result.Classes.Add("CodeProvider");
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 0, 0, 6);
        return Result;
    }
    
    // ● IReferenceContextMenuHost related
    protected virtual bool IsSuccessfulReferenceResult(ReferenceMenuCommandContext Context) => Context?.FormContext != null && Context.FormContext.Result;
    protected virtual void ReloadReferenceLookup(ReferenceMenuCommandContext Context)
    {
        if (Context.Binding.LookupSource == null)
            return;

        LookupSource LookupSource = Context.Binding.LookupSource.LookupDef.Create();
        List<LookupItem> List = LookupSource.GetList();
        Context.Binding.LookupSource = LookupSource;
        if (Context.Binding is GridColumnBinding GridBinding)
        {
            if (GridBinding.ActiveLookupComboBox != null)
            {
                GridBinding.ActiveLookupComboBox.ItemsSource = List;
                object Value = Context.Binding.Table.CurrentRowView != null ? Context.Binding.Table.CurrentRowView[Context.Binding.FieldName] : null;
                GridBinding.ActiveLookupComboBox.SelectedItem = LookupSource.FindItem(Value);
            }
            RefreshReferenceBinding(Context);
            return;
        }

        if (Context.Binding is ControlBinding ControlBinding && ControlBinding.Control is ComboBox ComboBox)
        {
            ControlBinding.IsRefreshing = true;
            try
            {
                ComboBox.SelectedItem = null;
                ComboBox.SelectedIndex = -1;
                ComboBox.ItemsSource = List;
            }
            finally
            {
                ControlBinding.IsRefreshing = false;
            }
        }
    }
    protected virtual void RefreshReferenceBinding(ReferenceMenuCommandContext Context)
    {
        if (Context.Binding is ControlBinding ControlBinding)
        {
            ControlBindingHelper.Refresh(Context.Binding.Table, ControlBinding);
            return;
        }

        if (Context.Caller is not DataGrid Grid || Context.Binding is not GridColumnBinding || Context.Binding.Table == null)
            return;

        Grid.InvalidateVisual();
    }
    protected virtual void SetReferenceValue(ReferenceMenuCommandContext Context, object Value)
    {
        if (Context.Binding?.Table?.CurrentRow == null || string.IsNullOrWhiteSpace(Context.Binding.FieldName))
            return;

        if (Context.Binding is ControlBinding ControlBinding && ControlBinding.Control is LocatorBox && Context.Binding.Locator != null)
        {
            if (Sys.IsNull(Value))
            {
                Context.Binding.Locator.Assign(null, Context.Binding.Table.CurrentRow, Context.Binding.FieldName, Context.Binding.LocatorTargetFieldMap);
            }
            else if (Context.Binding.Locator.LocateByKey(Value))
            {
                Context.Binding.Locator.Assign(Context.Binding.Locator.SelectedRow, Context.Binding.Table.CurrentRow, Context.Binding.FieldName, Context.Binding.LocatorTargetFieldMap);
            }
            else
            {
                Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Value;
            }

            RefreshReferenceBinding(Context);
            return;
        }

        if (Context.Binding.LocatorDef != null && Context.Binding.Locator != null)
        {
            if (Sys.IsNull(Value))
            {
                Context.Binding.Locator.Assign(null, Context.Binding.Table.CurrentRow, Context.Binding.FieldName, Context.Binding.LocatorTargetFieldMap);
            }
            else if (Context.Binding.Locator.LocateByKey(Value))
            {
                Context.Binding.Locator.Assign(Context.Binding.Locator.SelectedRow, Context.Binding.Table.CurrentRow, Context.Binding.FieldName, Context.Binding.LocatorTargetFieldMap);
            }
            else
            {
                Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Value;
            }

            RefreshReferenceBinding(Context);
            return;
        }

        if (Context.Binding is GridColumnBinding && Context.Binding.Table.CurrentRowView != null)
        {
            Context.Binding.Table.CurrentRowView.BeginEdit();
            Context.Binding.Table.CurrentRowView[Context.Binding.FieldName] = Sys.IsNull(Value) ? DBNull.Value : Value;
        }
        else
        {
            Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Sys.IsNull(Value) ? DBNull.Value : Value;
        }
        RefreshReferenceBinding(Context);
    }
 
    // ● binding
    /// <summary>
    /// Refreshes all binders.
    /// </summary>
    public virtual void Refresh()
    {
        foreach (ItemBinder Binder in Binders)
            Binder.Refresh();
    }
    /// <summary>
    /// Captures the current selection of all detail grids.
    /// </summary>
    public virtual Dictionary<DataGrid, Tuple<int, DataGridColumn>> CaptureDetailGridSelection()
    {
        Dictionary<DataGrid, Tuple<int, DataGridColumn>> Result = new();

        foreach (UiDetailTableInfo DetailInfo in Context.TopTableUiInfo.DetailList)
        {
            if (DetailInfo.Grid == null || DetailInfo.Grid.SelectedIndex < 0)
                continue;

            Result[DetailInfo.Grid] = Tuple.Create(DetailInfo.Grid.SelectedIndex, DetailInfo.Grid.CurrentColumn);
        }

        return Result;
    }
    /// <summary>
    /// Restores the selection of all detail grids.
    /// </summary>
    public virtual void RestoreDetailGridSelection(Dictionary<DataGrid, Tuple<int, DataGridColumn>> Selections)
    {
        if (Selections == null || Selections.Count == 0)
            return;

        Ui.Post(() => Ui.Post(() =>
        {
            foreach (KeyValuePair<DataGrid, Tuple<int, DataGridColumn>> Pair in Selections)
            {
                DataGrid Grid = Pair.Key;
                int Index = Pair.Value.Item1;
                DataGridColumn Column = Pair.Value.Item2;
                if (Grid == null || Index < 0)
                    continue;
                if (Grid.ItemsSource is not IEnumerable Items)
                    continue;

                int Counter = 0;
                foreach (object Item in Items)
                {
                    if (Counter++ != Index)
                        continue;

                    Grid.SelectedIndex = Index;
                    Grid.SelectedItem = Item;
                    if (Column != null)
                        Grid.CurrentColumn = Column;
                    Grid.ScrollIntoView(Item, Grid.CurrentColumn);
                    break;
                }
            }
        }));
    }
    /// <summary>
    /// Applies the visibility of detail grid columns ending with ID.
    /// </summary>
    public virtual void ApplyIdColumnsVisible(bool Value)
    {
        foreach (UiDetailTableInfo DetailInfo in Context.TopTableUiInfo.DetailList)
        {
            if (DetailInfo.Grid == null)
                continue;

            List<GridColumnBinding> List = DetailInfo.Grid.GetInfoList();
            foreach (GridColumnBinding CI in List)
            {
                if (CI.IsPlainId)
                    CI.GridColumn.IsVisible = Value;
            }
        }
    }
    /// <summary>
    /// Sets the data-bound controls and detail grids to read-only or restores their field-defined state.
    /// </summary>
    public virtual void SetReadOnly(bool Value)
    {
        fIsReadOnly = Value;

        foreach (ItemBinder Binder in Binders)
        {
            foreach (ControlBinding Binding in Binder.Bindings)
                SetControlReadOnly(Binding, Value);
        }

        foreach (UiDetailTableInfo DetailInfo in Context.TopTableUiInfo.DetailList)
            SetGridReadOnly(DetailInfo, Value);
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemPage()
    {
        Context = new();
        Context.GridHandler = this;
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
        if (IsReadOnly)
            return false;

        bool Result = RefContextMenu.Binding.FieldDef.IsReadOnlyEdit? DataForm.FormState == DataFormState.Insert : true;
        return Result;
    }
    public virtual bool CanExecute(ReferenceMenuCommandContext Context)
    {
        if (IsReadOnly || Context == null || Context.Binding == null)
            return false;

        switch (Context.ActionType)
        {
            case ReferenceMenuActionType.ShowList:
            case ReferenceMenuActionType.Add:
                return !string.IsNullOrWhiteSpace(Context.FormName);
            case ReferenceMenuActionType.Reload:
                return Context.Binding.LookupSource != null;
            case ReferenceMenuActionType.Edit:
                return !string.IsNullOrWhiteSpace(Context.FormName) && !Sys.IsNull(Context.RowId);
            case ReferenceMenuActionType.Clear:
                return DataForm.FormState.In(DataFormState.Insert | DataFormState.Edit);
        }

        return false;
    }
    public virtual object Execute(ReferenceMenuCommandContext Context)
    {
        if (!CanExecute(Context))
            return null;

        switch (Context.ActionType)
        {
            case ReferenceMenuActionType.ShowList:
                return ExecuteReferenceShowList(Context);
            case ReferenceMenuActionType.Reload:
                return ExecuteReferenceReload(Context);
            case ReferenceMenuActionType.Edit:
                return ExecuteReferenceEdit(Context);
            case ReferenceMenuActionType.Add:
                return ExecuteReferenceAdd(Context);
            case ReferenceMenuActionType.Clear:
                return ExecuteReferenceClear(Context);
        }

        return null;
    }

    // ● IReferenceContextMenuHost related
    public virtual async Task<DataFormContext> ExecuteReferenceShowList(ReferenceMenuCommandContext Context)
    {
        Context.FormContext = await DataFormContext.ShowFormModal(Context.FormName, DataFormAction.List, null, Context.Caller);
        if (IsSuccessfulReferenceResult(Context))
            SetReferenceValue(Context, Context.FormContext.ResultData);
        return Context.FormContext;
    }
    public virtual object ExecuteReferenceReload(ReferenceMenuCommandContext Context)
    {
        ReloadReferenceLookup(Context);
        RefreshReferenceBinding(Context);
        return null;
    }
    public virtual async Task<DataFormContext> ExecuteReferenceEdit(ReferenceMenuCommandContext Context)
    {
        Context.FormContext = await DataFormContext.ShowFormModal(Context.FormName, DataFormAction.Edit, Context.RowId, Context.Caller);
        if (IsSuccessfulReferenceResult(Context))
        {
            ReloadReferenceLookup(Context);
            SetReferenceValue(Context, Context.RowId);
        }
        return Context.FormContext;
    }
    public virtual async Task<DataFormContext> ExecuteReferenceAdd(ReferenceMenuCommandContext Context)
    {
        Context.FormContext = await DataFormContext.ShowFormModal(Context.FormName, DataFormAction.Insert, null, Context.Caller);
        if (IsSuccessfulReferenceResult(Context))
        {
            ReloadReferenceLookup(Context);
            SetReferenceValue(Context, Context.FormContext.ResultData);
        }
        return Context.FormContext;
    }
    public virtual object ExecuteReferenceClear(ReferenceMenuCommandContext Context)
    {
        SetReferenceValue(Context, DBNull.Value);
        return null;
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
    /// <summary>
    /// True when data-bound controls and detail grids are read-only.
    /// </summary>
    public bool IsReadOnly => fIsReadOnly;

    // ● events
    /// <summary>
    /// Occurs before the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanging;
    /// <summary>
    /// Occurs after the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanged;


    public virtual GridCommand[] GetGridCommands()
    {
        List<GridCommand> Result = new();

        if (!DataForm.IsEditableForm)
            return Result.ToArray();

        if (!DataFormAction.Insert.In(InvalidActions))
        {
            Result.Add(new GridCommand()
            {
                ActionType = GridActionType.Add,
                Name = "Add",
                Title = "Add",
                ToolTip = "Add row (Shift+Insert)",
                ImageFileName = "table_add.png",
                KeyGesture = new KeyGesture(Key.Insert, KeyModifiers.Shift)
            });
        }

        if (!DataFormAction.Delete.In(InvalidActions))
        {
            Result.Add(new GridCommand()
            {
                ActionType = GridActionType.Delete,
                Name = "Delete",
                Title = "Delete",
                ToolTip = "Delete row (Shift+Delete)",
                ImageFileName = "table_delete.png",
                KeyGesture = new KeyGesture(Key.Delete, KeyModifiers.Shift)
            });
        }

        return Result.ToArray();
    }

    public virtual bool CanExecute(GridCommandContext Context)
    {
        if (Context == null || Context.Command == null || Context.Grid == null || Context.Table == null)
            return false;

        if (IsReadOnly || !DataForm.IsEditableForm)
            return false;

        if (!DataForm.FormState.In(DataFormState.Insert | DataFormState.Edit))
            return false;

        switch (Context.Command.ActionType)
        {
            case GridActionType.Add:
                return !DataFormAction.Insert.In(InvalidActions);
            case GridActionType.Delete:
                return !DataFormAction.Delete.In(InvalidActions) && Context.Grid.SelectedItem is DataRowView;
        }

        return Context.Command.IsEnabled;
    }

    public virtual object Execute(GridCommandContext Context)
    {
        if (!CanExecute(Context))
            return null;

        switch (Context.Command.ActionType)
        {
            case GridActionType.Add:
                return ExecuteGridAdd(Context);
            case GridActionType.Delete:
                return ExecuteGridDelete(Context);
        }

        return null;
    }
    /// <summary>
    /// Adds a new row to a detail grid table.
    /// </summary>
    public virtual object ExecuteGridAdd(GridCommandContext Context)
    {
        if (Context == null || Context.Table == null || Context.Grid == null)
            return null;

        DataRow Row = Context.Table.AddNewRow();
        DataRowView RowView = MemTable.GetDataRowView(Row, Context.Table.DataView);

        Dispatcher.UIThread.Post(() =>
        {
            if (RowView != null)
                Context.Grid.SelectedItem = RowView;
            Context.Grid.Focus();
        }, DispatcherPriority.Input);

        return Row;
    }
    /// <summary>
    /// Deletes the selected row from a detail grid table.
    /// </summary>
    public virtual object ExecuteGridDelete(GridCommandContext Context)
    {
        if (Context == null || Context.Table == null || Context.Grid == null || Context.Grid.SelectedItem is not DataRowView RowView)
            return null;

        Ui.Post(async () =>
        {
            bool Flag = await MessageBox.YesNo("Delete selected row?", Context.Grid);
            if (!Flag)
                return;

            DataRow Row = RowView.Row;
            DataView DataView = Context.Table.DataView;
            int RowIndex = DataView.Cast<DataRowView>().ToList().IndexOf(RowView);

            Row.Delete();

            Dispatcher.UIThread.Post(() =>
            {
                if (DataView.Count > 0)
                {
                    int NewIndex = RowIndex >= DataView.Count ? DataView.Count - 1 : RowIndex;
                    Context.Grid.SelectedItem = DataView[NewIndex];
                    Context.Table.CurrentRowView = DataView[NewIndex];
                }
                else
                {
                    Context.Grid.SelectedItem = null;
                    Context.Table.CurrentRow = null;
                }

                Context.Grid.Focus();
            }, DispatcherPriority.Input);
        });

        return null;
    }
}

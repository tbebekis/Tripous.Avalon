namespace Tripous.Desktop;
 
/// <summary>
/// The item part of a <see cref="DataForm"/>
/// </summary>
[TypeStore]
public class ItemPage : UserControl, IReferenceContextMenuHost, IGridHandler
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
            ControlBindingHelper.Refresh(Context.Binding.Table, ControlBinding);
    }
    protected virtual void SetReferenceValue(ReferenceMenuCommandContext Context, object Value)
    {
        if (Context.Binding?.Table?.CurrentRow == null || string.IsNullOrWhiteSpace(Context.Binding.FieldName))
            return;

        if (Context.Binding is ControlBinding ControlBinding && ControlBinding.Control is LocatorBox && Context.Binding.Locator != null)
        {
            if (Sys.IsNull(Value))
            {
                Context.Binding.Locator.Assign(null, Context.Binding.Table.CurrentRow);
                Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = DBNull.Value;
            }
            else if (Context.Binding.Locator.LocateByKey(Value))
            {
                Context.Binding.Locator.Assign(Context.Binding.Locator.SelectedRow, Context.Binding.Table.CurrentRow);
                Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Context.Binding.Locator.KeyValue;
            }
            else
            {
                Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Value;
            }

            RefreshReferenceBinding(Context);
            return;
        }

        Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Sys.IsNull(Value) ? DBNull.Value : Value;
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
        bool Result = RefContextMenu.Binding.FieldDef.IsReadOnlyEdit? DataForm.FormState == DataFormState.Insert : true;
        return Result;
    }
    public virtual bool CanExecute(ReferenceMenuCommandContext Context)
    {
        if (Context == null || Context.Binding == null)
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
                ToolTip = "Add row (Ctrl+Insert)",
                ImageFileName = "table_add.png",
                KeyGesture = new KeyGesture(Key.Insert, KeyModifiers.Control)
            });
        }

        if (!DataFormAction.Delete.In(InvalidActions))
        {
            Result.Add(new GridCommand()
            {
                ActionType = GridActionType.Delete,
                Name = "Delete",
                Title = "Delete",
                ToolTip = "Delete row (Ctrl+Delete)",
                ImageFileName = "table_delete.png",
                KeyGesture = new KeyGesture(Key.Delete, KeyModifiers.Control)
            });
        }

        return Result.ToArray();
    }

    public virtual bool CanExecute(GridCommandContext Context)
    {
        if (Context == null || Context.Command == null || Context.Grid == null || Context.Table == null)
            return false;

        if (!DataForm.IsEditableForm)
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
        if (Context == null || Context.Command == null)
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

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
    /// <summary>
    /// The item UI context.
    /// </summary>
    protected UiItemContext Context;
    /// <summary>
    /// The parent data form.
    /// </summary>
    protected DataForm fDataForm;
    /// <summary>
    /// True when this item page is read-only.
    /// </summary>
    protected bool fIsReadOnly;
    /// <summary>
    /// Stores the original read-only state of detail grids.
    /// </summary>
    protected Dictionary<GroupGrid, bool> fGridReadOnlyStates = new();
    /// <summary>
    /// Stores the original read-only state of detail grid columns.
    /// </summary>
    protected Dictionary<GroupGridColumn, bool> fGridColumnReadOnlyStates = new();
    /// <summary>
    /// The splitter between the main item page content and the FactBox pane.
    /// </summary>
    protected GridSplitter fFactBoxSplitter;
    /// <summary>
    /// The right-side FactBox pane.
    /// </summary>
    protected Border fFactBoxPane;
    /// <summary>
    /// The tab control hosting FactBoxes.
    /// </summary>
    protected TabControl fFactBoxTabs;
    /// <summary>
    /// The built-in item information FactBox control.
    /// </summary>
    protected ItemInfoFactBoxControl fStandardInfoFactBoxControl;
    /// <summary>
    /// The created FactBox controls by definition.
    /// </summary>
    protected List<Tuple<ItemFactBoxDef, ItemFactBoxControl>> fFactBoxControls = [];
    /// <summary>
    /// The splitter column of the FactBox pane.
    /// </summary>
    protected ColumnDefinition fFactBoxSplitterColumn;
    /// <summary>
    /// The column of the FactBox pane.
    /// </summary>
    protected ColumnDefinition fFactBoxColumn;
    /// <summary>
    /// The default width of the FactBox pane.
    /// </summary>
    protected double fFactBoxPaneWidth = 280;
    /// <summary>
    /// True when the FactBox pane is visible.
    /// </summary>
    protected bool fFactBoxPaneVisible;
 
    // ● protected methods
    /// <summary>
    /// Returns true when a binding should be read-only.
    /// </summary>
    /// <param name="Binding">The binding to check.</param>
    /// <returns>True if the binding should be read-only; otherwise, false.</returns>
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
               || (Field.IsReadOnlyEdit && DataForm.FormState != DataFormState.Insert);
    }
    /// <summary>
    /// Sets a bound control to read-only or restores its field-defined state.
    /// </summary>
    /// <param name="Binding">The control binding.</param>
    /// <param name="Value">True to force read-only.</param>
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
    /// <summary>
    /// Sets a detail grid to read-only or restores its field-defined state.
    /// </summary>
    /// <param name="DetailInfo">The detail table information.</param>
    /// <param name="Value">True to force read-only.</param>
    protected virtual void SetGridReadOnly(UiDetailTableInfo DetailInfo, bool Value)
    {
        if (DetailInfo == null || DetailInfo.Grid == null)
            return;

        if (!fGridReadOnlyStates.ContainsKey(DetailInfo.Grid))
            fGridReadOnlyStates[DetailInfo.Grid] = DetailInfo.Grid.IsReadOnly;
        DetailInfo.Grid.IsReadOnly = Value || !DataForm.IsEditableForm || fGridReadOnlyStates[DetailInfo.Grid];
        DetailInfo.Grid.IsToolBarVisible = !DetailInfo.Grid.IsReadOnly;
        foreach (GroupGridColumnBinding Binding in DetailInfo.Grid.GetInfoList())
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
    }
    /// <summary>
    /// Returns the visible FactBox definitions of the current form.
    /// </summary>
    /// <returns>The visible FactBox definitions.</returns>
    protected virtual List<ItemFactBoxDef> GetVisibleFactBoxes()
    {
        List<ItemFactBoxDef> Result = [];
        ModuleDef ModuleDef = DataForm?.DataFormContext?.ModuleDef;
        FormDef FormDef = DataForm?.DataFormContext?.FormDef;

        void AddRange(DefList<ItemFactBoxDef> List)
        {
            if (List == null)
                return;

            foreach (ItemFactBoxDef Def in List)
            {
                if (Def.IsVisible && !Result.Any(Item => Sys.IsSameText(Item.Name, Def.Name)))
                    Result.Add(Def);
            }
        }

        if (ModuleDef != null)
            AddRange(ModuleDef.FactBoxes);
        if (FormDef != null)
            AddRange(FormDef.FactBoxes);

        return Result;
    }
    /// <summary>
    /// Creates a placeholder control for a FactBox tab.
    /// </summary>
    /// <param name="Def">The FactBox definition.</param>
    /// <returns>The created placeholder control.</returns>
    protected virtual Control CreateFactBoxPlaceholder(ItemFactBoxDef Def)
    {
        return new TextBlock
        {
            Text = Def != null ? Def.Title : string.Empty,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(8)
        };
    }
    /// <summary>
    /// Creates the context passed to a FactBox provider and control.
    /// </summary>
    /// <param name="Def">The FactBox definition.</param>
    /// <returns>The created context.</returns>
    protected virtual ItemFactBoxContext CreateFactBoxContext(ItemFactBoxDef Def)
    {
        DataRow Row = CurrentRow;
        object KeyValue = null;
        TableDef ItemTableDef = ModuleDef?.Table;
        if (Row != null && ItemTableDef != null && !string.IsNullOrWhiteSpace(ItemTableDef.KeyField) && Row.Table.Columns.Contains(ItemTableDef.KeyField))
            KeyValue = Row[ItemTableDef.KeyField];
        return new()
        {
            FormName = FormDef?.Name,
            FormClassName = FormDef?.ClassName,
            ItemPageClassName = FormDef?.ItemClassName,
            FactBoxDef = Def,
            Module = Module,
            Row = Row,
            RowState = Row?.RowState.ToString(),
            KeyValue = KeyValue
        };
    }
    /// <summary>
    /// Creates the control that renders a FactBox.
    /// </summary>
    /// <param name="Def">The FactBox definition.</param>
    /// <returns>The created control.</returns>
    protected virtual Control CreateFactBoxControl(ItemFactBoxDef Def)
    {
        try
        {
            ItemFactBoxContext FactBoxContext = CreateFactBoxContext(Def);
            object Data = GetFactBoxData(Def, FactBoxContext);
            ItemFactBoxControl Control = string.IsNullOrWhiteSpace(Def.DesktopControlClassName)
                ? null
                : TypeStore.CreateInstance<ItemFactBoxControl>(Def.DesktopControlClassName);

            if (Control == null)
                return CreateFactBoxMessage("No FactBox control is defined.");

            Control.BindFactBox(FactBoxContext, Data);
            fFactBoxControls.Add(Tuple.Create(Def, Control));
            return Control;
        }
        catch (Exception Ex)
        {
            return CreateFactBoxMessage(Ex.Message);
        }
    }
    /// <summary>
    /// Creates a FactBox message control.
    /// </summary>
    /// <param name="Message">The message text.</param>
    /// <returns>The created message control.</returns>
    protected virtual Control CreateFactBoxMessage(string Message)
    {
        return new TextBlock
        {
            Text = Message ?? string.Empty,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(8)
        };
    }
    /// <summary>
    /// Returns the data produced by a FactBox provider.
    /// </summary>
    /// <param name="Def">The FactBox definition.</param>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The FactBox data.</returns>
    protected virtual object GetFactBoxData(ItemFactBoxDef Def, ItemFactBoxContext Context)
    {
        ItemFactBoxProvider Provider = Def.CreateProvider();
        return Provider != null ? Provider.GetData(Context) : null;
    }
    /// <summary>
    /// Creates the data for the built-in item information FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The created data.</returns>
    protected virtual ItemStandardInfoFactBoxData CreateStandardInfoFactBoxData(ItemFactBoxContext Context)
    {
        return new()
        {
            ItemInfo = new ItemInfoFactBoxProvider().GetData(Context) as Dictionary<string, object> ?? new(),
            Structure = new ItemStructureFactBoxProvider().GetData(Context) as ItemStructureFactBoxData
        };
    }
    /// <summary>
    /// Creates the built-in item information FactBox control.
    /// </summary>
    /// <returns>The created control.</returns>
    protected virtual Control CreateStandardInfoFactBoxControl()
    {
        try
        {
            ItemFactBoxContext FactBoxContext = CreateFactBoxContext(null);
            fStandardInfoFactBoxControl = new();
            fStandardInfoFactBoxControl.BindFactBox(FactBoxContext, CreateStandardInfoFactBoxData(FactBoxContext));
            return fStandardInfoFactBoxControl;
        }
        catch (Exception Ex)
        {
            return CreateFactBoxMessage(Ex.Message);
        }
    }
    /// <summary>
    /// Refreshes the built-in item information FactBox.
    /// </summary>
    protected virtual void RefreshStandardInfoFactBox()
    {
        if (fStandardInfoFactBoxControl == null)
            return;

        try
        {
            ItemFactBoxContext FactBoxContext = CreateFactBoxContext(null);
            fStandardInfoFactBoxControl.BindFactBox(FactBoxContext, CreateStandardInfoFactBoxData(FactBoxContext));
        }
        catch
        {
            // Ignore a FactBox refresh failure. The initial creation path shows errors in the tab content.
        }
    }
    /// <summary>
    /// Refreshes all created FactBoxes.
    /// </summary>
    protected virtual void RefreshFactBoxes()
    {
        RefreshStandardInfoFactBox();
        foreach (Tuple<ItemFactBoxDef, ItemFactBoxControl> Pair in fFactBoxControls)
        {
            try
            {
                ItemFactBoxContext FactBoxContext = CreateFactBoxContext(Pair.Item1);
                object Data = GetFactBoxData(Pair.Item1, FactBoxContext);
                Pair.Item2.BindFactBox(FactBoxContext, Data);
            }
            catch
            {
                // Ignore a FactBox refresh failure. The initial creation path shows errors in the tab content.
            }
        }
    }
    /// <summary>
    /// Creates a tab for a FactBox definition.
    /// </summary>
    /// <param name="Def">The FactBox definition.</param>
    /// <returns>The created tab item.</returns>
    protected virtual TabItem CreateFactBoxTab(ItemFactBoxDef Def)
    {
        return new TabItem
        {
            Header = Def.Title,
            Content = CreateFactBoxControl(Def)
        };
    }
    /// <summary>
    /// Creates the tab control hosting the FactBoxes.
    /// </summary>
    /// <param name="FactBoxes">The FactBox definitions.</param>
    /// <returns>The created tab control.</returns>
    protected virtual TabControl CreateFactBoxTabs(List<ItemFactBoxDef> FactBoxes)
    {
        fFactBoxTabs = new();
        fFactBoxTabs.Items.Add(new TabItem
        {
            Header = "Info",
            Content = CreateStandardInfoFactBoxControl()
        });
        if (FactBoxes != null)
        {
            foreach (ItemFactBoxDef Def in FactBoxes)
                fFactBoxTabs.Items.Add(CreateFactBoxTab(Def));
        }
        return fFactBoxTabs;
    }
    /// <summary>
    /// Creates the right-side FactBox pane.
    /// </summary>
    /// <param name="FactBoxes">The FactBox definitions.</param>
    /// <returns>The created pane.</returns>
    protected virtual Border CreateFactBoxPane(List<ItemFactBoxDef> FactBoxes)
    {
        fFactBoxPane = new()
        {
            MinWidth = 220,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1, 0, 0, 0),
            Child = CreateFactBoxTabs(FactBoxes)
        };
        return fFactBoxPane;
    }
    /// <summary>
    /// Creates the item page root content.
    /// </summary>
    /// <param name="MainContent">The main item page content.</param>
    /// <param name="FactBoxes">The visible FactBox definitions.</param>
    /// <returns>The created root content.</returns>
    protected virtual Control CreateItemPageRoot(Control MainContent, List<ItemFactBoxDef> FactBoxes)
    {
        Grid Result = new();
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        fFactBoxSplitterColumn = new ColumnDefinition(new GridLength(0));
        fFactBoxColumn = new ColumnDefinition(new GridLength(0));
        Result.ColumnDefinitions.Add(fFactBoxSplitterColumn);
        Result.ColumnDefinitions.Add(fFactBoxColumn);

        Grid.SetColumn(MainContent, 0);
        Result.Children.Add(MainContent);

        fFactBoxSplitter = new()
        {
            Width = 4,
            Background = Brushes.LightGray,
            ResizeDirection = GridResizeDirection.Columns
        };
        Grid.SetColumn(fFactBoxSplitter, 1);
        Result.Children.Add(fFactBoxSplitter);

        Border Pane = CreateFactBoxPane(FactBoxes);
        Grid.SetColumn(Pane, 2);
        Result.Children.Add(Pane);
        FactBoxPaneVisible = Ui.Settings.ShowDataFormFactBoxPane;
        return Result;
    }

    /// <summary>
    /// Creates a field editor.
    /// </summary>
    /// <param name="Field">The field definition.</param>
    /// <param name="Binder">The item binder.</param>
    /// <returns>The created editor control.</returns>
    protected virtual Control CreateEditor(FieldDef Field, ItemBinder Binder)
    {
        Control Result;
        DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
        
        if (!string.IsNullOrWhiteSpace(Field.Locator))
        {
            LocatorBox LocatorBox = new();
            ControlBinding Binding = Binder.Bind(LocatorBox, Field);
            if (!Field.IsReadOnly && !Field.IsReadOnlyUI)
            {
                // context menu for lookup combo boxes and locator box controls.
                ReferenceContextMenu RefMenu = FormDef.CreateReferenceContextMenu();
                RefMenu.Initialize(this, Binding);
            }
            Result = LocatorBox;
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
    /// <summary>
    /// Returns true when a reference command produced a successful form result.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>True if the reference command succeeded; otherwise, false.</returns>
    protected virtual bool IsSuccessfulReferenceResult(ReferenceMenuCommandContext Context) => Context?.FormContext != null && Context.FormContext.Result;
    /// <summary>
    /// Reloads a reference lookup source.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    protected virtual void ReloadReferenceLookup(ReferenceMenuCommandContext Context)
    {
        if (Context.Binding.LookupSource == null)
            return;

        LookupSource LookupSource = Context.Binding.LookupSource.LookupDef.Create();
        List<LookupItem> List = LookupSource.GetList();
        Context.Binding.LookupSource = LookupSource;
        if (Context.Binding is GroupGridColumnBinding)
        {
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
    /// <summary>
    /// Refreshes a reference binding.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    protected virtual void RefreshReferenceBinding(ReferenceMenuCommandContext Context)
    {
        if (Context.Binding is ControlBinding ControlBinding)
        {
            ControlBindingHelper.Refresh(Context.Binding.Table, ControlBinding);
            return;
        }

        if (Context.Caller is not GroupGrid Grid || Context.Binding is not GroupGridColumnBinding || Context.Binding.Table == null)
            return;

        Grid.InvalidateVisual();
    }
    /// <summary>
    /// Sets a reference value to the bound row.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <param name="Value">The reference value.</param>
    protected virtual void SetReferenceValue(ReferenceMenuCommandContext Context, object Value)
    {
        if (Context.Binding?.Table?.CurrentRow == null || string.IsNullOrWhiteSpace(Context.Binding.FieldName))
            return;

        if (Context.Binding.LocatorDef != null)
        {
            LocatorMapper Mapper = new();
            if (Sys.IsNull(Value))
            {
                Mapper.Apply(Context.Binding.LocatorMapPlan, null, Context.Binding.Table.CurrentRow);
            }
            else
            {
                LocatorRequest Request = new()
                {
                    Context = new LocatorContext(Context.Binding.LocatorDef.Name),
                    KeyValue = Value,
                    IsMultiRow = false,
                };
                Request.Context.Params["Row"] = Context.Binding.Table.CurrentRow;
                Request.Context.Params["DataRow"] = Context.Binding.Table.CurrentRow;
                LocatorResult Result = Locators.Execute(Request);
                if (Result.HasSingleResult)
                    Mapper.Apply(Context.Binding.LocatorMapPlan, Result.Table.Rows[0], Context.Binding.Table.CurrentRow);
                else
                    Context.Binding.Table.CurrentRow[Context.Binding.FieldName] = Value;
            }

            RefreshReferenceBinding(Context);
            return;
        }

        if (Context.Binding is GroupGridColumnBinding && Context.Binding.Table.CurrentRowView != null)
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
        RefreshFactBoxes();
    }
    /// <summary>
    /// Captures the current selection of all detail grids.
    /// </summary>
    /// <returns>The captured detail grid selections.</returns>
    public virtual Dictionary<GroupGrid, Tuple<int, GroupGridColumn>> CaptureDetailGridSelection()
    {
        Dictionary<GroupGrid, Tuple<int, GroupGridColumn>> Result = new();

        foreach (UiDetailTableInfo DetailInfo in Context.TopTableUiInfo.DetailList)
        {
            if (DetailInfo.Grid == null || DetailInfo.Grid.CurrentRowIndex < 0)
                continue;

            Result[DetailInfo.Grid] = Tuple.Create(DetailInfo.Grid.CurrentRowIndex, DetailInfo.Grid.CurrentCell.Column);
        }

        return Result;
    }
    /// <summary>
    /// Restores the selection of all detail grids.
    /// </summary>
    /// <param name="Selections">The captured detail grid selections.</param>
    public virtual void RestoreDetailGridSelection(Dictionary<GroupGrid, Tuple<int, GroupGridColumn>> Selections)
    {
        if (Selections == null || Selections.Count == 0)
            return;

        Ui.Post(() => Ui.Post(() =>
        {
            foreach (KeyValuePair<GroupGrid, Tuple<int, GroupGridColumn>> Pair in Selections)
            {
                GroupGrid Grid = Pair.Key;
                int Index = Pair.Value.Item1;
                GroupGridColumn Column = Pair.Value.Item2;
                if (Grid == null || Index < 0)
                    continue;

                GroupGridColumn TargetColumn = Column ?? Grid.GetVisibleValueColumns().FirstOrDefault();
                if (TargetColumn != null)
                    Grid.SetCurrentCell(Index, TargetColumn);
                Grid.SelectCurrentCell();
                Grid.ScrollToRow(Index);
            }
        }));
    }
    /// <summary>
    /// Applies the visibility of detail grid columns ending with ID.
    /// </summary>
    /// <param name="Value">True to show ID columns; otherwise, false.</param>
    public virtual void ApplyIdColumnsVisible(bool Value)
    {
        foreach (UiDetailTableInfo DetailInfo in Context.TopTableUiInfo.DetailList)
        {
            if (DetailInfo.Grid == null)
                continue;

            List<GroupGridColumnBinding> List = DetailInfo.Grid.GetInfoList();
            foreach (GroupGridColumnBinding CI in List)
            {
                if (CI.IsPlainId)
                    DetailInfo.Grid.SetColumnVisible(CI.GridColumn, Value);
            }
        }
    }
    /// <summary>
    /// Toggles the FactBox pane.
    /// </summary>
    public virtual void ToggleFactBoxPane() => FactBoxPaneVisible = !FactBoxPaneVisible;
    /// <summary>
    /// Sets the data-bound controls and detail grids to read-only or restores their field-defined state.
    /// </summary>
    /// <param name="Value">True to force read-only.</param>
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
    /// <param name="ColumnCount">The editor column count.</param>
    public virtual void Bind(int ColumnCount)
    {
        if (IsBindingDone)
            throw new TripousDesktopException($"{this.GetType().FullName} data binding is already done.");
        
        Context.CreateEditorFunc = CreateEditor;
 
        ItemBinder.CurrentRowChanging += (s, ea) => CurrentRowChanging?.Invoke(this, EventArgs.Empty);
        ItemBinder.CurrentRowChanged += (s, ea) =>
        {
            CurrentRowChanged?.Invoke(this, EventArgs.Empty);
            RefreshFactBoxes();
        };
 
        ScrollViewer ScrollViewer = UiFactory.CreateScrollViewer();
        StackPanel Root = UiFactory.CreateStackPanel();
        ScrollViewer.Content = Root;
        Content = CreateItemPageRoot(ScrollViewer, GetVisibleFactBoxes());

        Context.ColumnCount = ColumnCount;
        Context.ParentControl = Root;
        
        if (Context.TopTableUiInfo.DetailList.Count == 0)
            UiItemPage.CreateSinglePageLayout(Context);
        else
            UiItemPage.CreateTabbedTopLayout(Context);

        IsBindingDone = true;
    }

    // ● IReferenceContextMenuHost implementation
    /// <summary>
    /// Returns true when a reference context menu can open.
    /// </summary>
    /// <param name="RefContextMenu">The reference context menu.</param>
    /// <returns>True if the context menu can open; otherwise, false.</returns>
    public virtual bool CanOpenRefContextMenu(ReferenceContextMenu RefContextMenu)
    {
        if (IsReadOnly)
            return false;

        bool Result = RefContextMenu.Binding.FieldDef.IsReadOnlyEdit? DataForm.FormState == DataFormState.Insert : true;
        return Result;
    }
    /// <summary>
    /// Returns true when a reference menu command can execute.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
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
    /// <summary>
    /// Executes a reference menu command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The command result.</returns>
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
    /// <summary>
    /// Executes the Show List reference command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The data form context.</returns>
    public virtual async Task<DataFormContext> ExecuteReferenceShowList(ReferenceMenuCommandContext Context)
    {
        Context.FormContext = await DataFormContext.ShowFormModal(Context.FormName, DataFormAction.List, null, Context.Caller);
        if (IsSuccessfulReferenceResult(Context))
            SetReferenceValue(Context, Context.FormContext.ResultData);
        return Context.FormContext;
    }
    /// <summary>
    /// Executes the Reload reference command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The command result.</returns>
    public virtual object ExecuteReferenceReload(ReferenceMenuCommandContext Context)
    {
        ReloadReferenceLookup(Context);
        RefreshReferenceBinding(Context);
        return null;
    }
    /// <summary>
    /// Executes the Edit reference command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The data form context.</returns>
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
    /// <summary>
    /// Executes the Add reference command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The data form context.</returns>
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
    /// <summary>
    /// Executes the Clear reference command.
    /// </summary>
    /// <param name="Context">The reference menu command context.</param>
    /// <returns>The command result.</returns>
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
    /// True when this item page has a FactBox pane.
    /// </summary>
    public bool HasFactBoxPane => fFactBoxPane != null;
    /// <summary>
    /// Gets or sets whether the FactBox pane is visible.
    /// </summary>
    public bool FactBoxPaneVisible
    {
        get => fFactBoxPaneVisible;
        set
        {
            fFactBoxPaneVisible = value && HasFactBoxPane;
            if (fFactBoxPane != null)
                fFactBoxPane.IsVisible = fFactBoxPaneVisible;
            if (fFactBoxSplitter != null)
                fFactBoxSplitter.IsVisible = fFactBoxPaneVisible;
            if (fFactBoxSplitterColumn != null)
                fFactBoxSplitterColumn.Width = new GridLength(fFactBoxPaneVisible ? 4 : 0);
            if (fFactBoxColumn != null)
                fFactBoxColumn.Width = new GridLength(fFactBoxPaneVisible ? fFactBoxPaneWidth : 0);
        }
    }
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

    // ● IGridHandler implementation
    /// <summary>
    /// Returns the grid commands provided by this handler.
    /// </summary>
    /// <returns>The grid commands.</returns>
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

    /// <summary>
    /// Returns true when a grid command can execute.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    public virtual bool CanExecute(GridCommandContext Context)
    {
        if (Context == null || Context.Command == null || Context.Grid == null || Context.Table == null)
            return false;

        if (IsReadOnly || !DataForm.IsEditableForm)
            return false;

        if (!DataForm.FormState.In(DataFormState.Insert | DataFormState.Edit))
            return false;
        if (!DataForm.CanExecuteGridCommand(Context))
            return false;

        switch (Context.Command.ActionType)
        {
            case GridActionType.Add:
                return !DataFormAction.Insert.In(InvalidActions);
            case GridActionType.Delete:
                return !DataFormAction.Delete.In(InvalidActions) && Context.Grid.CurrentRow is DataRowView;
        }

        return Context.Command.IsEnabled;
    }

    /// <summary>
    /// Executes a grid command.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>The command result.</returns>
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
    /// <param name="Context">The grid command context.</param>
    /// <returns>The created row.</returns>
    public virtual object ExecuteGridAdd(GridCommandContext Context)
    {
        if (Context == null || Context.Table == null || Context.Grid == null)
            return null;

        DataRow Row = Context.Table.AddNewRow();
        DataRowView RowView = MemTable.GetDataRowView(Row, Context.Table.DataView);

        Dispatcher.UIThread.Post(() =>
        {
            if (RowView != null)
            {
                int RowIndex = Context.Table.DataView.Cast<DataRowView>().ToList().IndexOf(RowView);
                GroupGridBinder.SelectRow(Context.Grid, RowIndex);
            }
            Context.Grid.Focus();
        }, DispatcherPriority.Input);

        return Row;
    }
    /// <summary>
    /// Deletes the selected row from a detail grid table.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>The command result.</returns>
    public virtual object ExecuteGridDelete(GridCommandContext Context)
    {
        if (Context == null || Context.Table == null || Context.Grid == null || Context.Grid.CurrentRow is not DataRowView RowView)
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
                    GroupGridBinder.SelectRow(Context.Grid, NewIndex);
                    Context.Table.CurrentRowView = DataView[NewIndex];
                }
                else
                {
                    Context.Grid.ClearCurrentCell();
                    Context.Grid.ClearSelection();
                    Context.Table.CurrentRow = null;
                }

                Context.Grid.Focus();
            }, DispatcherPriority.Input);
        });

        return null;
    }
}

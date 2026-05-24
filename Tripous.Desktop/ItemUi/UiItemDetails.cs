namespace Tripous.Desktop;

/// <summary>
/// Creates the UI for item page details, including detail tabs, one-to-one detail sections and detail grids.
/// </summary>
static public class UiItemDetails
{
    // ● details
    /// <summary>
    /// Creates the first-level detail tabs under the top table tab.
    /// </summary>
    static public void CreateFirstLevelDetails(UiItemContext context, Control ParentControl)
    {
        List<UiDetailTableInfo> Details = context.TopTableUiInfo.DetailList.Where(Detail => Detail.ParentTableDef == context.TopTableUiInfo.TableDef).ToList();
        if (Details.Count == 0)
            return;
        TabControl TabControl = UiFactory.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            TabControl.Items.Add(CreateDetailTabItem(context, Detail, 1));
        UiFactory.AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Creates all detail tabs from level two and deeper.
    /// </summary>
    static public void CreateChildLevelDetails(UiItemContext context, TabControl ParentTabControl)
    {
        foreach (UiDetailTableInfo Detail in context.TopTableUiInfo.DetailList)
        {
            if (Detail.ParentTableDef == context.TopTableUiInfo.TableDef)
                continue;
            int Level = GetTableLevel(context, Detail.TableDef);
            ParentTabControl.Items.Add(CreateDetailTabItem(context, Detail, Level));
        }
    }
    /// <summary>
    /// Returns the table level in the detail tree.
    /// </summary>
    static public int GetTableLevel(UiItemContext context, TableDef TableDef)
    {
        int Result = 0;
        TableDef Table = TableDef;
        while (Table != null && Table != context.ModuleDef.Table)
        {
            Result++;
            Table = Table.Master;
        }
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    static public TabItem CreateDetailTabItem(UiItemContext context, UiDetailTableInfo DetailUiInfo, int Level)
    {
        TabItem Result = new()
        {
            Header = DetailUiInfo.TableDef.Title
        };
        StackPanel Panel = UiFactory.CreateStackPanel();
        Result.Content = Panel;
        CreateDetail(context, Panel, DetailUiInfo);
        return Result;
    }
    /// <summary>
    /// Creates the container UI for a multi-row detail table.
    /// </summary>
    static public void CreateDetail(UiItemContext context, Control ParentControl, UiDetailTableInfo DetailUiInfo)
    {
        DockPanel Panel = new();
        Border ToolBarBorder = CreateDetailToolBarBorder(DetailUiInfo);
        DataGrid Grid = CreateDetailDataGrid(context, DetailUiInfo.TableDef);
        DetailUiInfo.Grid = Grid;
        Panel.Children.Add(ToolBarBorder);
        Panel.Children.Add(Grid);
        CreateDetailGridToolBar(context, DetailUiInfo);
        CreateDetailGridReferenceMenus(context, DetailUiInfo);
        UiFactory.AddChild(ParentControl, Panel);
    }
    /// <summary>
    /// Creates the initially hidden toolbar area of a detail table.
    /// </summary>
    static public Border CreateDetailToolBarBorder(UiDetailTableInfo DetailUiInfo)
    {
        Border Result = UiFactory.CreateToolBarBorder();
        DockPanel.SetDock(Result, Dock.Top);
        StackPanel ToolBarPanel = UiFactory.CreateToolBarPanel();
        Result.Child = ToolBarPanel;
        DetailUiInfo.ToolBarPanel = ToolBarPanel;
        return Result;
    }
    /// <summary>
    /// Creates the field UI and binder used by a one-to-one detail table.
    /// </summary>
    static public void CreateOneToOneDetail(UiItemContext context, Control ParentControl, UiTableInfo TableUiInfo)
    {
        ItemBinder Binder = context.CreateOneToOneBinder(TableUiInfo.TableDef);
        Binder.TableInfo = TableUiInfo;
        context.Binders.Add(Binder);
        UiItemPage.CreateFieldGroups(context, ParentControl, TableUiInfo, Binder, context.ColumnCount);
    }
    /// <summary>
    /// Creates one-to-one detail controls under a specified parent table.
    /// </summary>
    static public void CreateOneToOneDetails(UiItemContext context, Control ParentControl, TableDef ParentTableDef)
    {
        foreach (UiTableInfo TableUiInfo in context.TopTableUiInfo.OneToOneList)
        {
            if (TableUiInfo.TableDef.Master != ParentTableDef)
                continue;
            CreateOneToOneDetail(context, ParentControl, TableUiInfo);
        }
    }

    // ● toolbar
    /// <summary>
    /// Creates the toolbar buttons of a detail data grid.
    /// </summary>
    static public void CreateDetailGridToolBar(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        if (context.GridHandler == null || DetailUiInfo.ToolBarPanel == null || DetailUiInfo.Grid == null)
            return;

        GridCommand[] Commands = context.GridHandler.GetGridCommands();
        Commands = Commands.Where(Command => Command.IsVisible).ToArray();
        if (Commands.Length == 0)
        {
            DetailUiInfo.ToolBarPanel.IsVisible = false;
            return;
        }

        ToolBar ToolBar = new() { Panel = DetailUiInfo.ToolBarPanel };
        Dictionary<GridCommand, Button> Buttons = new();

        DetailGridCommandContext CreateContext(GridCommand Command)
        {
            return new DetailGridCommandContext()
            {
                Command = Command,
                Grid = DetailUiInfo.Grid,
                Table = DetailUiInfo.Table,
                DetailInfo = DetailUiInfo,
                ItemContext = context
            };
        }

        void UpdateButtons()
        {
            foreach (KeyValuePair<GridCommand, Button> Pair in Buttons)
                Pair.Value.IsEnabled = Pair.Key.IsEnabled && context.GridHandler.CanExecute(CreateContext(Pair.Key));
        }

        void Execute(GridCommand Command)
        {
            DetailGridCommandContext CommandContext = CreateContext(Command);
            if (context.GridHandler.CanExecute(CommandContext))
                context.GridHandler.Execute(CommandContext);
            UpdateButtons();
        }

        foreach (GridCommand Command in Commands)
        {
            Button Button = ToolBar.AddButton(Command.ImageFileName, Command.ToolTip, () => Execute(Command));
            Button.IsTabStop = false;
            Button.Tag = Command;
            Buttons[Command] = Button;
        }

        DetailUiInfo.Grid.SelectionChanged += (Sender, Args) => UpdateButtons();
        DetailUiInfo.Grid.AddHandler(InputElement.KeyDownEvent, (Sender, Args) =>
        {
            foreach (GridCommand Command in Commands)
            {
                if (Command.KeyGesture != null && Command.KeyGesture.Matches(Args))
                {
                    Execute(Command);
                    Args.Handled = true;
                    break;
                }
            }
        }, RoutingStrategies.Tunnel, handledEventsToo: true);

        ToolBar.IsVisible = true;
        UpdateButtons();
    }
    static public void CreateDetailGridReferenceMenus(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        if (DetailUiInfo.Grid == null || DetailUiInfo.Table == null || context.GridHandler is not IReferenceContextMenuHost MenuHost)
            return;

        Dictionary<GridColumnBinding, ReferenceContextMenu> Menus = new();
        foreach (GridColumnBinding Binding in DetailUiInfo.Grid.GetInfoList())
        {
            if (!Binding.IsReference)
                continue;

            if (Binding.DataColumn == null)
                Binding.DataColumn = DetailUiInfo.Table.FindColumn(Binding.FieldName);

            ReferenceContextMenu Menu = new();
            Menu.Initialize(MenuHost, Binding);
            Menus[Binding] = Menu;
        }

        if (Menus.Count == 0)
            return;

        DetailUiInfo.Grid.CellPointerPressed += (Sender, Args) =>
        {
            if (!Args.PointerPressedEventArgs.GetCurrentPoint(DetailUiInfo.Grid).Properties.IsRightButtonPressed)
                return;

            GridColumnBinding Binding = Args.Column.GetInfo();
            if (Binding == null || !Menus.TryGetValue(Binding, out ReferenceContextMenu Menu))
                return;

            if (Args.Row.DataContext is DataRowView RowView)
            {
                DetailUiInfo.Grid.SelectedItem = RowView;
                DetailUiInfo.Table.CurrentRowView = RowView;
            }

            DetailUiInfo.Grid.CurrentColumn = Args.Column;
            if (Menu.Open(DetailUiInfo.Grid))
                Args.PointerPressedEventArgs.Handled = true;
        };
    }

    // ● detail grids
    /// <summary>
    /// Creates a detail data grid.
    /// </summary>
    static public DataGrid CreateDetailDataGrid(UiItemContext context, TableDef TableDef)
    {
        DataGrid Result = new()
        {
            AutoGenerateColumns = false,
            Focusable = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsTabStop = true,
            Margin = new Thickness(0, 8, 0, 8)
        };
        CreateDetailGridColumns(Result, TableDef);
        BindDetailGrid(context, Result, TableDef);
        GridEditController.Attach(Result);
        return Result;
    }
    /// <summary>
    /// Creates the columns of a detail data grid.
    /// </summary>
    static public void CreateDetailGridColumns(DataGrid Grid, TableDef TableDef)
    {
        foreach (FieldDef Field in TableDef.GetBindableFields())
        {
            if (!UiItemPage.IsDetailGridField(Field))
                continue;
            Grid.Columns.AddRange(CreateDetailGridColumns(Field));
        }
    }
    /// <summary>
    /// Creates columns for a detail grid field.
    /// </summary>
    static public List<DataGridColumn> CreateDetailGridColumns(FieldDef Field)
    {
        if (Field.IsLocator)
        {
            List<DataGridColumn> LocatorColumns = CreateLocatorDetailGridColumns(Field);
            if (LocatorColumns.Count > 0)
                return LocatorColumns;
        }

        return [CreateDetailGridColumn(Field)];
    }
    /// <summary>
    /// Creates display columns for a locator detail grid field.
    /// </summary>
    static public List<DataGridColumn> CreateLocatorDetailGridColumns(FieldDef Field)
    {
        List<DataGridColumn> Result = [];
        if (Field.TableDef == null)
            return Result;

        LocatorDef LocatorDef = DataRegistry.Locators.Find(Field.Locator);
        if (LocatorDef == null)
            return Result;

        ControlBindingHelper.EnsureLocatorFields(LocatorDef, Field);

        TableDef JoinTable = Field.TableDef.Joins.FirstOrDefault(item => item.MasterField.IsSameText(Field.Name));
        if (JoinTable == null)
            return Result;

        Dictionary<string, string> TargetFieldMap = CreateLocatorTargetFieldMap(LocatorDef, JoinTable);
        foreach (LocatorFieldDef LocatorField in LocatorDef.Fields.Where(item => item.IsVisible))
        {
            if (LocatorDef.KeyField.IsSameText(LocatorField.Name))
                continue;

            FieldDef JoinField = FindLocatorJoinField(JoinTable, LocatorField);
            if (JoinField == null)
                continue;

            bool IsReadOnly = Field.IsReadOnly || LocatorDef.IsReadOnly || !LocatorField.IsSearchable;
            Result.Add(DataGridBinder.CreateLocatorColumn(JoinField.Alias, JoinField.Title, Field, LocatorDef, LocatorField, TargetFieldMap, IsReadOnly: IsReadOnly));
        }

        return Result;
    }
    /// <summary>
    /// Creates a locator field to target field map.
    /// </summary>
    static public Dictionary<string, string> CreateLocatorTargetFieldMap(LocatorDef LocatorDef, TableDef JoinTable)
    {
        Dictionary<string, string> Result = new(StringComparer.OrdinalIgnoreCase);
        foreach (LocatorFieldDef LocatorField in LocatorDef.Fields)
        {
            FieldDef JoinField = FindLocatorJoinField(JoinTable, LocatorField);
            if (JoinField == null)
                continue;

            Result[LocatorField.Name] = JoinField.Alias;
            Result[LocatorField.Alias] = JoinField.Alias;
        }
        return Result;
    }
    /// <summary>
    /// Finds the join field that matches a locator field.
    /// </summary>
    static public FieldDef FindLocatorJoinField(TableDef JoinTable, LocatorFieldDef LocatorField)
    {
        return JoinTable.Fields.FirstOrDefault(item =>
        {
            if (item.Name.IsSameText(LocatorField.Name))
                return true;
            if (item.Alias.IsSameText(LocatorField.Alias))
                return true;
            if (!string.IsNullOrWhiteSpace(LocatorField.TargetField) && item.Alias.IsSameText(LocatorField.TargetField))
                return true;
            return false;
        });
    }
    /// <summary>
    /// Creates a column for a detail data grid.
    /// </summary>
    static public DataGridColumn CreateDetailGridColumn(FieldDef Field)
    {
        if (Field.IsLookup)
            return DataGridBinder.CreateLookupColumn(Field);
        return DataGridBinder.CreateGridColumn(Field);
    }
    /// <summary>
    /// Binds a detail data grid to the view of its table.
    /// </summary>
    static public void BindDetailGrid(UiItemContext context, DataGrid Grid, TableDef TableDef)
    {
        MemTable Table = context.Module.GetTable(TableDef.Name);
        DataView DataView = Table.DataView;
        DataViewItemsSource ItemsSource = new(DataView);
        Grid.ItemsSource = ItemsSource;

        void SelectFirstRow()
        {
            Ui.Post(() =>
            {
                if (ItemsSource.Count > 0 && Grid.SelectedItem == null)
                {
                    Grid.SelectedIndex = 0;
                    Grid.SelectedItem = ItemsSource[0];
                    Table.CurrentRowView = ItemsSource[0];
                    if (Grid.CurrentColumn == null)
                        Grid.CurrentColumn = Grid.Columns.FirstOrDefault(Column => Column.IsVisible);
                }
            });
        }

        ItemsSource.CollectionChanged += (Sender, Args) => SelectFirstRow();
        SelectFirstRow();
    }
}

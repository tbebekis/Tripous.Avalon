/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

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
        List<UiDetailTableInfo> Details = context.TopTableUiInfo.DetailList
            .Where(Detail => Detail.ParentTableDef == context.TopTableUiInfo.TableDef)
            .ToList();
        Details = OrderDetails(context, context.TopTableUiInfo.TableDef, Details);
        if (Details.Count == 0)
            return;
        TabControl TabControl = UiFactory.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            TabControl.Items.Add(CreateDetailTabItem(context, Detail));
        UiFactory.AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Returns the immediate child details of a detail table.
    /// </summary>
    static List<UiDetailTableInfo> GetChildDetails(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        List<UiDetailTableInfo> Result = context.TopTableUiInfo.DetailList
            .Where(Detail => Detail.ParentTableDef == DetailUiInfo.TableDef)
            .ToList();
        return OrderDetails(context, DetailUiInfo.TableDef, Result);
    }
    /// <summary>
    /// Orders sibling details according to the module detail order.
    /// </summary>
    static List<UiDetailTableInfo> OrderDetails(UiItemContext context, TableDef ParentTableDef, List<UiDetailTableInfo> Details)
    {
        if (Details.Count < 2 || ParentTableDef == null || !context.ModuleDef.DetailOrder.TryGetValue(ParentTableDef.Name, out List<string> DetailOrder))
            return Details;

        Dictionary<string, int> Order = new(StringComparer.OrdinalIgnoreCase);
        for (int Index = 0; Index < DetailOrder.Count; Index++)
        {
            string Name = DetailOrder[Index];
            if (!string.IsNullOrWhiteSpace(Name) && !Order.ContainsKey(Name))
                Order[Name] = Index;
        }

        return Details
            .Select((Detail, Index) => new
            {
                Detail,
                Index,
                Order = Order.TryGetValue(Detail.TableDef.Name, out int Value) ? Value : int.MaxValue
            })
            .OrderBy(Item => Item.Order)
            .ThenBy(Item => Item.Index)
            .Select(Item => Item.Detail)
            .ToList();
    }
    /// <summary>
    /// Creates a horizontal splitter between a parent detail and its child details.
    /// </summary>
    static GridSplitter CreateDetailSplitter()
    {
        return new GridSplitter
        {
            Height = 5,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            ResizeDirection = GridResizeDirection.Rows,
            ResizeBehavior = GridResizeBehavior.PreviousAndNext,
            Background = Brushes.LightGray
        };
    }
    /// <summary>
    /// Creates a panel for a single child detail.
    /// </summary>
    static Control CreateSingleChildDetail(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        Grid Result = new();
        Result.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        TextBlock Header = new()
        {
            Text = DetailUiInfo.TableDef.Title,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 8, 0, 4)
        };
        Control Content = CreateDetailBranch(context, DetailUiInfo, ApplyMinimumHeight: false);
        Avalonia.Controls.Grid.SetRow(Header, 0);
        Avalonia.Controls.Grid.SetRow(Content, 1);
        Result.Children.Add(Header);
        Result.Children.Add(Content);
        return Result;
    }
    /// <summary>
    /// Creates tabs for multiple child details.
    /// </summary>
    static Control CreateChildDetailTabs(UiItemContext context, List<UiDetailTableInfo> Details)
    {
        TabControl Result = UiFactory.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            Result.Items.Add(CreateDetailTabItem(context, Detail, ApplyMinimumHeight: false));
        return Result;
    }
    /// <summary>
    /// Creates a detail branch recursively.
    /// </summary>
    static Control CreateDetailBranch(UiItemContext context, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        Grid Result = new();
        List<UiDetailTableInfo> Children = GetChildDetails(context, DetailUiInfo);
        if (Children.Count == 0)
        {
            CreateDetail(context, Result, DetailUiInfo, ApplyMinimumHeight);
            return Result;
        }

        if (ApplyMinimumHeight)
            Result.MinHeight = (Ui.Settings.DetailGridMinHeight * 2) + 5;
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        Result.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        GridSplitter Splitter = CreateDetailSplitter();
        Control ChildControl = Children.Count == 1
            ? CreateSingleChildDetail(context, Children[0])
            : CreateChildDetailTabs(context, Children);
        Grid ParentControl = CreateDetail(context, Result, DetailUiInfo, ApplyMinimumHeight: false);
        Avalonia.Controls.Grid.SetRow(ParentControl, 0);
        Avalonia.Controls.Grid.SetRow(Splitter, 1);
        Avalonia.Controls.Grid.SetRow(ChildControl, 2);
        Result.Children.Add(Splitter);
        Result.Children.Add(ChildControl);
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    static public TabItem CreateDetailTabItem(UiItemContext context, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        TabItem Result = new()
        {
            Header = DetailUiInfo.TableDef.Title,
            Content = CreateDetailBranch(context, DetailUiInfo, ApplyMinimumHeight)
        };
        return Result;
    }
    /// <summary>
    /// Creates the container UI for a multi-row detail table.
    /// </summary>
    static public Grid CreateDetail(UiItemContext context, Control ParentControl, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        Grid Panel = new();
        Panel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Panel.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        Border ToolBarBorder = CreateDetailToolBarBorder(DetailUiInfo);
        DataGrid Grid = CreateDetailDataGrid(context, DetailUiInfo.TableDef, ApplyMinimumHeight);
        DetailUiInfo.Grid = Grid;
        Avalonia.Controls.Grid.SetRow(ToolBarBorder, 0);
        Avalonia.Controls.Grid.SetRow(Grid, 1);
        Panel.Children.Add(ToolBarBorder);
        Panel.Children.Add(Grid);
        CreateDetailGridToolBar(context, DetailUiInfo);
        CreateDetailGridReferenceMenus(context, DetailUiInfo);
        UiFactory.AddChild(ParentControl, Panel);
        return Panel;
    }
    /// <summary>
    /// Creates the initially hidden toolbar area of a detail table.
    /// </summary>
    static public Border CreateDetailToolBarBorder(UiDetailTableInfo DetailUiInfo)
    {
        Border Result = UiFactory.CreateToolBarBorder();
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
    /// <summary>
    /// Creates the reference menus for a detail data grid.
    /// </summary>
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
    static public DataGrid CreateDetailDataGrid(UiItemContext context, TableDef TableDef, bool ApplyMinimumHeight = true)
    {
        DataGrid Result = new()
        {
            AutoGenerateColumns = false,
            Focusable = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsTabStop = true,
            MinHeight = ApplyMinimumHeight ? Ui.Settings.DetailGridMinHeight : 0,
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
            if (TableDef.IsLocatorSnapshotField(Field))
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

        TableDef JoinTable = Field.TableDef.FindJoinTableByMasterKeyField(Field.Name);
        if (JoinTable == null)
            return Result;

        Dictionary<string, string> TargetFieldMap = Field.TableDef.CreateLocatorTargetFieldMap(Field, LocatorDef);
        foreach (LocatorFieldDef LocatorField in LocatorDef.Fields.Where(item => item.IsVisible))
        {
            if (LocatorDef.KeyField.IsSameText(LocatorField.Name))
                continue;

            FieldDef TargetField = Field.TableDef.FindLocatorTargetField(Field, LocatorField);
            if (TargetField == null)
                continue;

            bool IsReadOnly = Field.IsReadOnly || Field.IsReadOnlyUI || LocatorDef.IsReadOnly || !LocatorField.IsSearchable;
            Result.Add(DataGridBinder.CreateLocatorColumn(TargetField.Alias, TargetField.Title, Field, LocatorDef, LocatorField, TargetFieldMap, IsReadOnly: IsReadOnly));
        }

        return Result;
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

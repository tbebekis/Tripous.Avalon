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
        UiFactory.AddChild(ParentControl, Panel);
    }
    /// <summary>
    /// Creates the initially hidden toolbar area of a detail table.
    /// </summary>
    static public Border CreateDetailToolBarBorder(UiDetailTableInfo DetailUiInfo)
    {
        Border Result = new();
        Result.Classes.Add("ToolbarContainer");
        DockPanel.SetDock(Result, Dock.Top);
        StackPanel ToolBarPanel = UiFactory.CreateStackPanel();
        ToolBarPanel.Classes.Add("ToolBar");
        ToolBarPanel.Height = 32;
        ToolBarPanel.IsVisible = false;
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

    // ● detail grids
    /// <summary>
    /// Creates a detail data grid.
    /// </summary>
    static public DataGrid CreateDetailDataGrid(UiItemContext context, TableDef TableDef)
    {
        DataGrid Result = new()
        {
            AutoGenerateColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 8)
        };
        CreateDetailGridColumns(Result, TableDef);
        BindDetailGrid(context, Result, TableDef);
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
            Grid.Columns.Add(CreateDetailGridColumn(Field));
        }
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
        Grid.ItemsSource = context.Module.GetTable(TableDef.Name).DataView;
    }
}

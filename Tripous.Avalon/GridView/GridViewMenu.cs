using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Mysqlx.Crud;
using Avalonia.VisualTree;
using Tripous.Data;

namespace Tripous.Avalon;

public class GridViewMenu
{
    bool fIsEnabled;
    private ContextMenu mnuContextMenu;
    private MenuItem mnuToggleGrouping;
    private MenuItem mnuClearGroup;
    private MenuItem mnuToggleSorting;
    private MenuItem mnuClearSorting;
    private MenuItem mnuShowAllColumns;
    private MenuItem mnuVisibleColumns;
    private MenuItem mnuShowViewDefDialog;
    private MenuItem mnuShowFilterDialog;
    private MenuItem mnuClearColumnFilter;
    private MenuItem mnuClearAllFilters;
    private MenuItem mnuSummaries;
    private MenuItem mnuCount;      // all
    private MenuItem mnuSum;        // numeric
    private MenuItem mnuMin;        // numeric, date
    private MenuItem mnuMax;        // numeric, date
    private MenuItem mnuAvg;        // numeric
    private MenuItem mnuNone;
    private MenuItem mnuClearSummaries;
    
    private GridViewDef ViewDef => GridView.ViewDef;
    private DataGridColumn SelectedColumn;
    private GridViewColumnDef SelectedColumnDef => SelectedColumn != null? GridView.GetColumnDef(SelectedColumn) : null;
 
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        if (mnuShowAllColumns == sender)
            ClearHiddenColumns();
        else if (mnuClearGroup == sender)
            ClearGrouping();
        else if (mnuToggleGrouping == sender)
            ToggleGrouping();
        else if (mnuClearSummaries == sender)
            ClearSummaries();
        else if (mnuShowViewDefDialog == sender)
            await ShowViewDefDialog();
        else if (mnuShowFilterDialog == sender)
            await ShowFilterDialog();
        else if (mnuClearColumnFilter == sender)
            ClearColumnFilter();
        else if (mnuClearAllFilters == sender)
            ClearFilters();
        else if (mnuClearSorting == sender)
            ClearSorting();
        else if (mnuToggleSorting == sender)
            ToggleSorting();

        CloseMenu();
    }
    private void AnySummaryClick(object sender, RoutedEventArgs ea)
    {
        MenuItem mnuItem = sender as MenuItem;
        AggregateType AggregateType = (AggregateType)mnuItem.Tag;
        SetSummary(AggregateType);
        CloseMenu();
    }
    private void Grid_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var props = e.GetCurrentPoint(Grid).Properties;
        if (!props.IsRightButtonPressed) 
            return;
        
       Point P = e.GetPosition(Grid);
       DataGridColumn GridColumn = null;

       IInputElement Hit = Grid.InputHitTest(P);
       Control Ctrl = Hit as Control;

       while (Ctrl != null && GridColumn == null)
       {
           GridColumn = DataGridColumn.GetColumnContainingElement(Ctrl);
           Ctrl = Ctrl.Parent as Control;
       }

       SelectedColumn = GridColumn;
       if (SelectedColumn != null)
       {
           mnuContextMenu.Placement = PlacementMode.Pointer;
           UpdateContextMenu();
           mnuContextMenu.Open(Grid);
           e.Handled = true;
       }
    }
    
    // ● private
    string ObjectToString(object V) => V != null ? V.ToString() : string.Empty;
    bool HasFilter(DataGridColumn Column) => ViewDef.RowFilters.Contains(GridView.GetColumnDef(Column).FieldName);
    bool HasFilter(GridViewColumnDef ColumnDef) => ViewDef.RowFilters.Contains(ColumnDef.FieldName);
    
    void GridColumnListChanged()
    {
    }
    
    // ● menu functions
    void CreateContextMenu()
    {
        if (mnuContextMenu != null)
            return;
        
        List<object> list = new();
       
        mnuVisibleColumns   = list.AddMenuItem("Visible Columns");
        mnuShowAllColumns   = list.AddMenuItem("Show All Columns", AnyClick);
        list.AddSeparator();
        mnuToggleGrouping   = list.AddMenuItem("Toggle Grouping", AnyClick);
        mnuClearGroup       = list.AddMenuItem("Clear Group", AnyClick);
        list.AddSeparator();
        mnuSummaries        = list.AddMenuItem("Summaries");
        mnuClearSummaries   = list.AddMenuItem("Clear Summaries", AnyClick);
        List<object> SummaryList = new();
        mnuCount    = SummaryList.AddMenuItem("Count", AnySummaryClick, AggregateType.Count);
        mnuSum      = SummaryList.AddMenuItem("Sum", AnySummaryClick, AggregateType.Sum);
        mnuMin      = SummaryList.AddMenuItem("Min", AnySummaryClick, AggregateType.Min);
        mnuMax      = SummaryList.AddMenuItem("Max", AnySummaryClick, AggregateType.Max);
        mnuAvg      = SummaryList.AddMenuItem("Avg", AnySummaryClick, AggregateType.Avg);
        mnuNone     = SummaryList.AddMenuItem("None", AnySummaryClick, AggregateType.None);
        mnuSummaries.ItemsSource = SummaryList.ToArray();
        list.AddSeparator();
        mnuShowViewDefDialog  = list.AddMenuItem("Edit View", AnyClick);
        mnuShowFilterDialog       = list.AddMenuItem("Edit Filter", AnyClick);
        mnuClearColumnFilter      = list.AddMenuItem("Clear Filter", AnyClick);
        mnuClearAllFilters  = list.AddMenuItem("Clear All Filters", AnyClick);
        list.AddSeparator();
        mnuToggleSorting   = list.AddMenuItem("Toggle Sorting", AnyClick);
        mnuClearSorting    = list.AddMenuItem("Clear Sorting", AnyClick);
        
        mnuContextMenu = new ContextMenu();
        mnuContextMenu.ItemsSource = list.ToArray();
     
        //DataGridColumnHeader
    }
    void RecreateColumnMenus()
    {
        mnuVisibleColumns.Items.Clear();
        List<DataGridColumn> Columns = Grid.Columns
            .OrderBy(c => c.DisplayIndex)
            .ToList();
        
        foreach (var Column in Columns)
        {
            MenuItem mnuColumn;
            CheckBox chIsVisible;
            GridViewColumnDef ColumnDef = GridView.GetColumnDef(Column);
            
            mnuColumn = new MenuItem() { Header = Column.Header,  StaysOpenOnClick = true };
 
            chIsVisible = new CheckBox { IsChecked = Column.IsVisible, Margin = new Thickness(0, 0, 10, 0) };
            mnuColumn.Icon = chIsVisible;
            chIsVisible.IsChecked = Column.IsVisible;
 
            mnuColumn.Click += (s, e) => {
                chIsVisible.IsChecked = !chIsVisible.IsChecked;
            };
            chIsVisible.IsCheckedChanged += (s, e) =>
            {
                Column.IsVisible = chIsVisible.IsChecked == true;
                ColumnDef = GridView.GetColumnDef(Column);
                if (ColumnDef != null)
                    ColumnDef.VisibleIndex = Column.IsVisible? Grid.Columns.IndexOf(Column) : -1;
            };

            mnuVisibleColumns.Items.Add(mnuColumn);
        }
    }
    void UpdateContextMenu()
    {
        if (SelectedColumnDef != null)
        {
            // enabled
            bool IsNumeric = SelectedColumnDef.IsNumeric;
            bool IsDateTime = SelectedColumnDef.IsDateTime;
            bool IsSortable = SelectedColumnDef.IsString || IsNumeric || IsDateTime;
  
            mnuToggleGrouping.IsVisible = true;
            mnuSummaries.IsVisible = true;
            mnuCount.IsVisible = true;
            mnuSum.IsVisible = IsNumeric && SelectedColumnDef.Aggregate != AggregateType.Sum;
            mnuMin.IsVisible = (IsNumeric || IsDateTime) && SelectedColumnDef.Aggregate != AggregateType.Min;
            mnuMax.IsVisible = (IsNumeric || IsDateTime) && SelectedColumnDef.Aggregate != AggregateType.Max;
            mnuAvg.IsVisible = IsNumeric && SelectedColumnDef.Aggregate != AggregateType.Avg;
            mnuNone.IsVisible = SelectedColumnDef.Aggregate != AggregateType.None;

            mnuClearColumnFilter.IsVisible = HasFilter(SelectedColumnDef);
            mnuClearAllFilters.IsVisible = ViewDef.RowFilters.Count > 0;
            mnuShowFilterDialog.IsVisible = SelectedColumnDef.IsRowFilterSupportedColumn;
 
            mnuToggleSorting.IsVisible = IsSortable;
            
            // headers
            string Caption = SelectedColumnDef.Caption;
            
            if (SelectedColumnDef.GroupIndex >= 0)
                mnuToggleGrouping.Header = $"Remove from Group [{Caption}]";
            else
                mnuToggleGrouping.Header = $"Add to Group [{Caption}]";

            mnuSummaries.Header = $"Summaries [{Caption}]";
            
            mnuShowFilterDialog.Header = $"Edit Filter  [{Caption}]";
            mnuClearColumnFilter.Header = $"Clear Filter  [{Caption}]" ;
            
            if (SelectedColumnDef.SortIndex >= 0)
                mnuToggleSorting.Header = $"Remove from Sorted [{Caption}]";
            else
                mnuToggleSorting.Header = $"Add to Sorted [{Caption}]";
        }
        else
        {
            mnuToggleGrouping.IsVisible = false;
             
            //mnuShowAllColumns;
            //mnuColumns;
            mnuShowFilterDialog.IsVisible = false;
            mnuClearColumnFilter.IsVisible = false;
            //mnuClearAllFilters;
            mnuSummaries.IsVisible = false;
            mnuCount.IsVisible = false;
            mnuSum.IsVisible = false;
            mnuMin.IsVisible = false;
            mnuMax.IsVisible = false;
            mnuAvg.IsVisible = false;
            mnuNone.IsVisible = false;
            mnuClearSummaries.IsVisible = false;

            mnuToggleSorting.IsVisible = false;
        }

        RecreateColumnMenus(); 
    }
    void CloseMenu()
    {
        if (mnuContextMenu.IsOpen)
            mnuContextMenu.Close();
    }
    
    void ClearHiddenColumns()
    {
        GridViewColumnDef ColumnDef;
        foreach (var Column in Grid.Columns)
        {
            Column.IsVisible = true;
            ColumnDef = GridView.GetColumnDef(Column);
            if (ColumnDef != null)
                ColumnDef.VisibleIndex = Grid.Columns.IndexOf(Column);
        }
    }
    void ClearGrouping()
    {
        var List = ViewDef.GetGroupColumns();
        if (List.Count > 0)
        {
            foreach (var ColDef in List)
                ColDef.GroupIndex = -1;
            GridView.Refresh();
        }
    }
    void ClearSummaries()
    {
        var List = ViewDef.GetAggregateColumns();
        if (List.Count > 0)
        {
            foreach (var ColDef in List)
                ColDef.Aggregate = AggregateType.None;
            GridView.Refresh();
        }
    }
    void ClearFilters()
    {
        if (ViewDef.RowFilters.Count > 0)
        {
            ViewDef.RowFilters.Clear();
            GridView.Refresh();
        }
    }
    void ClearSorting()
    {
        var List = ViewDef.GetSortedColumns();
        if (List.Count > 0)
        {
            foreach (var ColDef in List)
                ColDef.SortIndex = -1;
            GridView.Refresh();
        }
    }
    
    void ToggleGrouping()
    {
        if (SelectedColumnDef != null)
        {
            SelectedColumnDef.GroupIndex = SelectedColumnDef.GroupIndex == -1 ? ViewDef.GetGroupColumns().Count : -1;
            GridView.Refresh();
        }
    }
    void SetSummary(AggregateType NewType)
    {
        if (SelectedColumnDef != null && SelectedColumnDef.Aggregate != NewType)
        {
            SelectedColumnDef.Aggregate = NewType;
            GridView.Refresh();
        }
    }

    async Task ShowViewDefDialog()
    {
        GridViewDef ViewDef2 = ViewDef.Clone();
        ViewDef2.IsNameReadOnly = true;
        
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef2);
        if (data.Result)
        {
            ViewDef.AssignFrom(ViewDef2);
            //string JsonText = Json.Serialize(ViewDef);
            GridView.Refresh();
        }
    }
    async Task ShowFilterDialog()
    {
        if (SelectedColumnDef != null)
        {
            
            RowFilterDef FilterItem = ViewDef.RowFilters.Find(SelectedColumnDef.FieldName);
            bool IsNew = FilterItem == null;
            if (IsNew)
            {
                FilterItem = new RowFilterDef();
                FilterItem.ColumnDef = SelectedColumnDef;
                FilterItem.BoolOp = BoolOp.And; // always AND
                FilterItem.FieldName = SelectedColumnDef.FieldName;
            }
 
            DialogData data = await DialogWindow.ShowModal<RowFilterItemDialog>(FilterItem, Grid);
            if (data.Result)
            {
                if (IsNew)
                    ViewDef.RowFilters.Add(FilterItem);
                GridView.Refresh();
            }
        }
    }
    void ClearColumnFilter()
    {
        if (SelectedColumnDef != null)
        {
            RowFilterDef FilterItem = ViewDef.RowFilters.Find(SelectedColumnDef.FieldName);
            if (FilterItem != null)
            {
                ViewDef.RowFilters.Remove(FilterItem);
                GridView.Refresh();
            }
        }
    }

    void ToggleSorting()
    {
        if (SelectedColumnDef != null)
        {
            SelectedColumnDef.SortIndex = SelectedColumnDef.SortIndex == -1 ? ViewDef.GetSortedColumns().Count : -1;
            GridView.Refresh();
        }
    }
    
    // ● construction
    public GridViewMenu()
    {
    }
    
    // ● properties
    public GridView GridView { get; set; }
    public DataGrid Grid => GridView != null ? GridView.Grid : null;
    public bool IsEnabled 
    {
        get => fIsEnabled;
        set
        {
            if (fIsEnabled != value)
            {
                fIsEnabled = value;
                if (value)
                {
                    Grid.IsReadOnly = true;
                    Grid.CanUserReorderColumns = true;
                    Grid.CanUserResizeColumns = true;

                    if (mnuContextMenu == null) 
                    {
                        CreateContextMenu();
                        Grid.AddHandler(InputElement.PointerPressedEvent, Grid_PointerPressed,  RoutingStrategies.Bubble, true);
                    }
                    
                    GridColumnListChanged();
                }
                else
                {
                    Grid.RemoveHandler(InputElement.PointerPressedEvent, Grid_PointerPressed);
                }
            }
        }
    }
    
}
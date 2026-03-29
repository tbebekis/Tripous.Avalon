namespace Tripous.Avalon;
 
 
/// <summary>
/// Handles a <see cref="Avalonia.Controls.DataGrid"/>
/// by displaying a context menu with items
/// for handling groups, summmaries, filters and column visibility.
/// </summary>
public class DataGridViewHandler
{
    private ContextMenu mnuContextMenu;
    private MenuItem mnuToggleGrouping;
    private MenuItem mnuClearGroup;
    private MenuItem mnuShowAllColumns;
    private MenuItem mnuColumns;
    private MenuItem mnuShowFilterDialog;
    private MenuItem mnuClearFilter;
    private MenuItem mnuClearAllFilters;
    private MenuItem mnuSummaries;
    private MenuItem mnuCount;      // all
    private MenuItem mnuSum;        // numeric
    private MenuItem mnuMin;        // numeric, date
    private MenuItem mnuMax;        // numeric, date
    private MenuItem mnuAvg;        // numeric
    private MenuItem mnuNone;
    private MenuItem mnuClearSummaries;
        
    private DataGridColumn colSelected;
    List<DataGridColumn> GroupColumns = new();  
    Dictionary<DataGridColumn, DataGridAggregateType> ColumnSummaries = new();
    Dictionary<DataGridColumn, RowFilterItem> ColumnFilters = new();
    
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        //
        if (mnuShowAllColumns == sender)
            ShowAllColumns();
        else if (mnuClearGroup == sender)
            ClearGrouping();
        else if (mnuToggleGrouping == sender)
            ToggleGrouping();
        else if (mnuCount == sender)
            SetSummary(DataGridAggregateType.Count);
        else if (mnuSum == sender)
            SetSummary(DataGridAggregateType.Sum);
        else if (mnuMin == sender)
            SetSummary(DataGridAggregateType.Min);
        else if (mnuMax == sender)
            SetSummary(DataGridAggregateType.Max);
        else if (mnuAvg == sender)
            SetSummary(DataGridAggregateType.Average);
        else if (mnuNone == sender)
            SetSummary(DataGridAggregateType.None);
        else if (mnuClearSummaries == sender)
            ClearSummaries();
        else if (mnuShowFilterDialog == sender)
            await ShowFilterDialog();
        else if (mnuClearFilter == sender)
            ClearFilter();
        else if (mnuClearAllFilters == sender)
            ClearAllFilters();
 
    }
    
    // ● private
    string ObjectToString(object V) => V != null ? V.ToString() : string.Empty;

    DataGridColumn GetSelectedColumn()
    {
        string columnId = ObjectToString(Grid.ColumnHeaderContextMenuColumnId);

        if (string.IsNullOrWhiteSpace(columnId))
            return null;

        foreach (var column in Grid.Columns)
        {
            // καλύτερα να δουλεύεις με ColumnKey
            if (string.Equals(ObjectToString(column.ColumnKey), columnId, StringComparison.Ordinal))
                return column;
        }

        return null;
    }
    void GroupColumnsChanged()
    {
        DataGridCollectionView View = Grid.ItemsSource as DataGridCollectionView;
        if (View == null)
            return;

        View.GroupDescriptions.Clear();

        foreach (DataGridColumn Column in GroupColumns)
        {
            string ColumnName = ObjectToString(Column.ColumnKey);

            if (!string.IsNullOrWhiteSpace(ColumnName))
                View.GroupDescriptions.Add(new DataRowViewGroupDescription(ColumnName));
        }

        View.Refresh();
    }
    void SummariesChanged()
    {
        foreach (var Column in Grid.Columns)
            Column.Summaries.Clear();

        foreach (var Pair in ColumnSummaries)
        {
            DataGridColumn Column = Pair.Key;
            DataGridAggregateType SumType = Pair.Value;
            GridColumnInfo ColInfo = Column.Tag as GridColumnInfo;

            if (SumType == DataGridAggregateType.None)
                continue;

            string SumTitle = SumType.ToString().ToLower();
            if (SumTitle == "average")
                SumTitle = "avg";
            
            var Summary = new DataRowViewSummaryDescription(SumType, ColInfo.FieldName)  
            {
                Scope = DataGridSummaryScope.Both,
                Title = $"{SumTitle}: "
            };

            Column.Summaries.Add(Summary);
        }

        bool HasSummaries = ColumnSummaries.Values.Any(v => v != DataGridAggregateType.None);

        Grid.ShowTotalSummary = HasSummaries;
        Grid.ShowGroupSummary = HasSummaries;
    }
    bool HasFilter(DataGridColumn Column)
    {
        return ColumnFilters.Keys.Contains(Column);
    }
    void ColumnFiltersChanged()
    {
        GridBinder Binder = GridBinder.GetGridBinder(Grid);
        if (Binder != null)
        {
            string Filter = string.Empty;
            if (ColumnFilters.Count > 0)
            {
                var List = ColumnFilters.Values.ToList();
                RowFilterItemList FilterItemList = new();
                FilterItemList.AddRange(List.ToArray());
                Filter = FilterItemList.Text;
            }
            
            if (Binder.DataTable is MemTable)
            {
                MemTable Table = Binder.DataTable as MemTable;
                Table.UserRowFilter = Filter;
            }
            else
            {
                Binder.DataTable.DefaultView.RowFilter = Filter;
            }
        }
 
   
    }
    
    // ● menu functions
    void CreateContextMenu()
    {
        List<object> list = new();
       
        mnuColumns          = list.AddMenuItem("Columns", null);
        mnuShowAllColumns   = list.AddMenuItem("Show All Columns", AnyClick);
        list.AddSeparator();
        mnuShowFilterDialog       = list.AddMenuItem("Edit Filter", AnyClick);
        mnuClearFilter      = list.AddMenuItem("Clear Filter", AnyClick);
        mnuClearAllFilters  = list.AddMenuItem("Clear All Filters", AnyClick);
        list.AddSeparator();
        mnuToggleGrouping   = list.AddMenuItem("Toggle Grouping", AnyClick);
        mnuClearGroup       = list.AddMenuItem("Clear Group", AnyClick);
        list.AddSeparator();
        mnuSummaries        = list.AddMenuItem("Summaries", null);
        mnuClearSummaries   = list.AddMenuItem("Clear Summaries", AnyClick);
        List<object> SummaryList = new();
        mnuCount    = SummaryList.AddMenuItem("Count", AnyClick);
        mnuSum      = SummaryList.AddMenuItem("Sum", AnyClick);
        mnuMin      = SummaryList.AddMenuItem("Min", AnyClick);
        mnuMax      = SummaryList.AddMenuItem("Max", AnyClick);
        mnuAvg      = SummaryList.AddMenuItem("Avg", AnyClick);
        mnuNone      = SummaryList.AddMenuItem("None", AnyClick);
        mnuSummaries.ItemsSource = SummaryList.ToArray();

        mnuContextMenu = new ContextMenu();
        mnuContextMenu.ItemsSource = list.ToArray();
 
        Grid.ColumnHeaderContextMenu = mnuContextMenu;
        
        //--------------------------------------------
        mnuContextMenu.Opening += (sender, args) =>
        {
            colSelected = GetSelectedColumn();
            Type DataType = colSelected != null? colSelected.GetColumnDataType(): null;

            bool IsNumeric = DataType != null? DataType.IsNumericType(): false;
            bool IsDate = DataType != null? DataType.IsDateType(): false;
            
            
            // enabled
            mnuToggleGrouping.IsEnabled = colSelected != null;

            mnuSummaries.IsEnabled = colSelected != null;
            mnuCount.IsEnabled = colSelected != null;
            mnuSum.IsEnabled = colSelected != null && IsNumeric && ColumnSummaries[colSelected] != DataGridAggregateType.Sum;
            mnuMin.IsEnabled = colSelected != null && (IsNumeric || IsDate) && ColumnSummaries[colSelected] != DataGridAggregateType.Min;
            mnuMax.IsEnabled = colSelected != null && (IsNumeric || IsDate) && ColumnSummaries[colSelected] != DataGridAggregateType.Max;
            mnuAvg.IsEnabled = colSelected != null && IsNumeric && ColumnSummaries[colSelected] != DataGridAggregateType.Average;
            mnuNone.IsEnabled = colSelected != null && ColumnSummaries[colSelected] != DataGridAggregateType.None;
 
            mnuClearFilter.IsEnabled = colSelected != null && HasFilter(colSelected);
            mnuClearAllFilters.IsEnabled = ColumnFilters.Count > 0;

            mnuShowFilterDialog.IsEnabled = (colSelected != null)
                                            && (colSelected.Tag is GridColumnInfo)
                                            && (colSelected.Tag as GridColumnInfo).IsRowFilterSupportedColumn;
 
            // headers
            if (colSelected != null)
            {
                if (GroupColumns.Contains(colSelected))
                    mnuToggleGrouping.Header = $"Remove from Group [{colSelected.Header}]";
                else
                    mnuToggleGrouping.Header = $"Add to Group [{colSelected.Header}]";
                
                mnuSummaries.Header = $"Summaries [{colSelected.Header}]";
            }
            else
            {
                mnuToggleGrouping.Header = "Group";
                mnuSummaries.Header = $"Summaries";
            }

            mnuShowFilterDialog.Header = mnuShowFilterDialog.IsEnabled ? $"Edit Filter  [{colSelected.Header}]" : "Edit Filter";
            mnuClearFilter.Header = mnuClearFilter.IsEnabled ? $"Clear Filter  [{colSelected.Header}]" : "Clear Filter";
            
            RecreateColumnMenus(); 
        };
        //--------------------------------------------
    }
    void RecreateColumnMenus()
    {
        mnuColumns.Items.Clear();
        List<DataGridColumn> Columns = Grid.Columns
            .OrderBy(c => c.DisplayIndex)
            .ToList();
        
        foreach (var Column in Columns)
        {
            MenuItem mnuColumn;
            CheckBox chIsVisible;
            
            mnuColumn = new MenuItem() { Header = Column.Header,  StaysOpenOnClick = true };
            mnuColumn.Click += AnyClick;
 
            chIsVisible = new CheckBox { IsChecked = Column.IsVisible, Margin = new Thickness(0, 0, 10, 0) };
            mnuColumn.Icon = chIsVisible;
            chIsVisible.IsChecked = Column.IsVisible;
            
            mnuColumn.Click += (s, e) => {
                chIsVisible.IsChecked = !chIsVisible.IsChecked;
            };
            chIsVisible.IsCheckedChanged += (s, e) => { 
                Column.IsVisible = chIsVisible.IsChecked.HasValue? chIsVisible.IsChecked.Value: false; 
            };

            mnuColumns.Items.Add(mnuColumn);
        }
    }
    
    void ShowAllColumns()
    {
        foreach (var Column in Grid.Columns)
            Column.IsVisible = true;
    }
    void ClearGrouping()
    {
        if (GroupColumns.Count > 0)
        {
            GroupColumns.Clear();
            GroupColumnsChanged();
        }
    }
    void ToggleGrouping()
    {
        if (colSelected != null)
        {
            if (GroupColumns.Contains(colSelected))
                GroupColumns.Remove(colSelected);
            else
                GroupColumns.Add(colSelected);

            GroupColumnsChanged();
        }
    }
    void SetSummary(DataGridAggregateType NewType)
    {
        if (colSelected != null && ColumnSummaries[colSelected] != NewType)
        {
            ColumnSummaries[colSelected] = NewType;
            SummariesChanged();
        }
    }
    void ClearSummaries()
    {
        foreach (var Pair in ColumnSummaries)
        {
            DataGridColumn Column = Pair.Key;
            ColumnSummaries[Column] = DataGridAggregateType.None;
        }
        
        SummariesChanged();
    }
    async Task ShowFilterDialog()
    {
        if (colSelected != null)
        {
            RowFilterItem RowFilterItem = HasFilter(colSelected)? ColumnFilters[colSelected]: null;
            bool IsNew = RowFilterItem == null;
            if (IsNew)
            {
                RowFilterItem = new RowFilterItem();
                RowFilterItem.BoolOp = BoolOp.And; // always AND
                GridColumnInfo ColumnInfo = colSelected.Tag as GridColumnInfo;
                RowFilterItem.FieldName = ColumnInfo.FieldName;
                RowFilterItem.Tag = ColumnInfo;
            }
            
            DialogData Data = await DialogWindow.ShowModal<RowFilterItemDialog>(RowFilterItem);
            if (Data.Result)
            {
                if (IsNew)
                    ColumnFilters[colSelected] = RowFilterItem;
                ColumnFiltersChanged();
            }
        }
 
    }
    void ClearFilter()
    {
        if (colSelected != null && HasFilter(colSelected))
        {
            ColumnFilters.Remove(colSelected);
            ColumnFiltersChanged();
        }
 
    }
    void ClearAllFilters()
    {
        ColumnFilters.Clear();
        ColumnFiltersChanged();
    }
 
    // ● construction
    public DataGridViewHandler(DataGrid Grid)
    {
        this.Grid = Grid;
        
        Grid.IsReadOnly = true;
        Grid.CanUserPaste = false;
        
        Grid.CanUserAddRows = false;
        Grid.CanUserDeleteRows = false;
        Grid.CanUserReorderRows = false;
        Grid.CanUserSelectRows = false;
        
        Grid.CanUserReorderColumns = true;
        Grid.CanUserHideColumns = true;
        Grid.CanUserResizeColumns = true;
        Grid.CanUserSelectColumns = true;
        Grid.CanUserResizeColumnsOnDoubleClick = true;
        
        Grid.ShowTotalSummary = true;
        Grid.ShowGroupSummary = true;
        Grid.TotalSummaryPosition = DataGridSummaryRowPosition.Bottom;
        Grid.GroupSummaryPosition = DataGridGroupSummaryPosition.Footer;
         
        CreateContextMenu();
        ColumnListChanged();
    }
    
    // ● public
    public void ClearColumnMenuItems()
    {
        mnuColumns.Items.Clear();
    }
    public void ColumnListChanged()
    {
        GroupColumns.Clear();
        GroupColumnsChanged();
            
        ColumnSummaries.Clear();
        foreach (var Column in Grid.Columns)
            ColumnSummaries[Column] = DataGridAggregateType.None;
        
        ColumnFilters.Clear();
        ColumnFiltersChanged();
    }

    // ● properties
    public DataGrid Grid { get;  }
}
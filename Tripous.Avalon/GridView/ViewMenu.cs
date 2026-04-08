namespace Tripous.Avalon;
 
 
/// <summary>
/// Handles a <see cref="Avalonia.Controls.DataGrid"/>
/// by displaying a context menu with items
/// for handling groups, summmaries, filters and column visibility.
/// </summary>
public class ViewMenu
{
    bool fIsEnabled;
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
    private MenuItem mnuExport;
    private MenuItem mnuExportToCSV;
    private MenuItem mnuExportToTabDelimited;
    private MenuItem mnuExportToXML;
    private MenuItem mnuExportToJSON;
    private MenuItem mnuExportToHTML;
    private MenuItem mnuExportToMarkdown;
    private MenuItem mnuExportToYAML;
       
    private DataGridColumn colSelected;
    List<DataGridColumn> GroupColumns = new();  
    Dictionary<DataGridColumn, AggregateType> ColumnSummaries = new();
    Dictionary<DataGridColumn, RowFilterDef> ColumnFilters = new();
    private GridExport GridExporter;
    
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        //
        if (mnuShowAllColumns == sender)
            ClearHiddenColumns();
        else if (mnuClearGroup == sender)
            ClearGrouping();
        else if (mnuToggleGrouping == sender)
            ToggleGrouping();
        else if (mnuClearSummaries == sender)
            ClearSummaries();
        else if (mnuShowFilterDialog == sender)
            await ShowFilterDialog();
        else if (mnuClearFilter == sender)
            ClearFilter();
        else if (mnuClearAllFilters == sender)
            ClearFilters();
    }
    private void AnySummaryClick(object sender, RoutedEventArgs ea)
    {
        MenuItem mnuItem = sender as MenuItem;
        AggregateType AggregateType = (AggregateType)mnuItem.Tag;
        SetSummary(AggregateType);
    }
    private async void AnyExportClick(object sender, RoutedEventArgs ea)
    {
        MenuItem mnuItem = sender as MenuItem;
        DataGridClipboardExportFormat ExportFormat = (DataGridClipboardExportFormat)mnuItem.Tag;
        
        if (GridExporter == null)
            GridExporter = new GridExport();
        
        await GridExporter.ExportToAsync(Grid, ExportFormat);
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
        Grid.UpdateLayout();
    }
    void ColumnSummariesChanged()
    {
        foreach (var Column in Grid.Columns)
            Column.Summaries.Clear();

        foreach (var Pair in ColumnSummaries)
        {
            DataGridColumn Column = Pair.Key;
            AggregateType SumType = Pair.Value;
            GridColumnInfo ColInfo = Column.Tag as GridColumnInfo;

            if (SumType == AggregateType.None)
                continue;

            string SumTitle = SumType.ToString().ToLower();
            if (SumTitle == "average")
                SumTitle = "avg";
            
            var Summary = new DataRowViewSummaryDescription(SumType.ToAvalonia(), ColInfo.FieldName)  
            {
                Scope = DataGridSummaryScope.Both,
                Title = $"{SumTitle}: "
            };

            Column.Summaries.Add(Summary);
        }

        bool HasSummaries = ColumnSummaries.Values.Any(v => v != AggregateType.None);

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
                RowFilterDefs filterDefs = new();
                filterDefs.AddRange(List.ToArray());
                Filter = filterDefs.Text;
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
        mnuCount    = SummaryList.AddMenuItem("Count", AnySummaryClick, AggregateType.Count);
        mnuSum      = SummaryList.AddMenuItem("Sum", AnySummaryClick, AggregateType.Sum);
        mnuMin      = SummaryList.AddMenuItem("Min", AnySummaryClick, AggregateType.Min);
        mnuMax      = SummaryList.AddMenuItem("Max", AnySummaryClick, AggregateType.Max);
        mnuAvg      = SummaryList.AddMenuItem("Avg", AnySummaryClick, AggregateType.Avg);
        mnuNone      = SummaryList.AddMenuItem("None", AnySummaryClick, AggregateType.None);
        mnuSummaries.ItemsSource = SummaryList.ToArray();
        list.AddSeparator();
        mnuExport        = list.AddMenuItem("Export", null);
        List<object> ExportList = new();
        mnuExportToCSV = ExportList.AddMenuItem("CSV", AnyExportClick, DataGridClipboardExportFormat.Csv);
        mnuExportToTabDelimited = ExportList.AddMenuItem("TAB", AnyExportClick, DataGridClipboardExportFormat.Text);
        mnuExportToXML = ExportList.AddMenuItem("XML", AnyExportClick, DataGridClipboardExportFormat.Xml);
        mnuExportToJSON = ExportList.AddMenuItem("JSON", AnyExportClick, DataGridClipboardExportFormat.Json);
        mnuExportToHTML = ExportList.AddMenuItem("HTML", AnyExportClick, DataGridClipboardExportFormat.Html);
        mnuExportToYAML = ExportList.AddMenuItem("YAML", AnyExportClick, DataGridClipboardExportFormat.Yaml);
        mnuExportToMarkdown = ExportList.AddMenuItem("Markdown", AnyExportClick, DataGridClipboardExportFormat.Markdown);
        
        mnuExport.ItemsSource = ExportList.ToArray();

        mnuContextMenu = new ContextMenu();
        mnuContextMenu.ItemsSource = list.ToArray();
 
       
        
        //--------------------------------------------
        mnuContextMenu.Opening += (sender, args) =>
        {
            colSelected = GetSelectedColumn();
            Type DataType = colSelected != null? colSelected.GetColumnDataType(): null;

            bool IsNumeric = DataType != null? DataType.IsNumeric(): false;
            bool IsDate = DataType != null? DataType.IsDateTime(): false;
            
            
            // enabled
            mnuToggleGrouping.IsEnabled = colSelected != null;

            mnuSummaries.IsEnabled = colSelected != null;
            mnuCount.IsEnabled = colSelected != null;
            mnuSum.IsEnabled = colSelected != null && IsNumeric && ColumnSummaries[colSelected] != AggregateType.Sum;
            mnuMin.IsEnabled = colSelected != null && (IsNumeric || IsDate) && ColumnSummaries[colSelected] != AggregateType.Min;
            mnuMax.IsEnabled = colSelected != null && (IsNumeric || IsDate) && ColumnSummaries[colSelected] != AggregateType.Max;
            mnuAvg.IsEnabled = colSelected != null && IsNumeric && ColumnSummaries[colSelected] != AggregateType.Avg;
            mnuNone.IsEnabled = colSelected != null && ColumnSummaries[colSelected] != AggregateType.None;
 
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
    
    void ClearHiddenColumns()
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
    void ClearSummaries()
    {
        foreach (var Pair in ColumnSummaries)
        {
            DataGridColumn Column = Pair.Key;
            ColumnSummaries[Column] = AggregateType.None;
        }
        
        ColumnSummariesChanged();
    }
    void ClearFilters()
    {
        ColumnFilters.Clear();
        ColumnFiltersChanged();
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
    void SetSummary(AggregateType NewType)
    {
        if (colSelected != null && ColumnSummaries[colSelected] != NewType)
        {
            ColumnSummaries[colSelected] = NewType;
            ColumnSummariesChanged();
        }
    }
    async Task ShowFilterDialog()
    {
        if (colSelected != null)
        {
            RowFilterDef rowFilterDef = HasFilter(colSelected)? ColumnFilters[colSelected]: null;
            bool IsNew = rowFilterDef == null;
            if (IsNew)
            {
                rowFilterDef = new RowFilterDef();
                rowFilterDef.BoolOp = BoolOp.And; // always AND
                GridColumnInfo ColumnInfo = colSelected.Tag as GridColumnInfo;
                rowFilterDef.FieldName = ColumnInfo.FieldName;
                rowFilterDef.Tag = ColumnInfo;
            }
            
            /*
            DialogData data = await DialogWindow.ShowModal<RowFilterItemDialog>(rowFilterDef);
            if (data.Result)
            {
                if (IsNew)
                    ColumnFilters[colSelected] = rowFilterDef;
                ColumnFiltersChanged();
            }
            */
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
 
    // ● construction
    public ViewMenu(DataGrid Grid)
    {
        this.Grid = Grid;
        

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
            ColumnSummaries[Column] = AggregateType.None;
        
        ColumnFilters.Clear();
        ColumnFiltersChanged();
    }
    
    public void SaveTo(GridViewDef Def)
    {
        Def.ClearLists();
        
        /*
        List<DataGridColumn> ColumnList = Grid.Columns
            .OrderBy(c => c.DisplayIndex)
            .ToList();

        GridColumnInfo CI;
        foreach (DataGridColumn Col in ColumnList)
        {
            CI = Col.GetColumnInfo();
            
            // columns in visible order
            Def.OrderList.Add(CI.FieldName);
            
            // hidden columns
            if (!Col.IsVisible)
                Def.HiddenList.Add(CI.FieldName);
            
            // column summaries
            if (ColumnSummaries.Keys.Contains(Col))
                Def.Summaries[CI.FieldName] = ColumnSummaries[Col];
        }
        
        // columns in group
        foreach (DataGridColumn Col in GroupColumns)
        {
            CI = Col.GetColumnInfo();
            Def.GroupList.Add(CI.FieldName);
        }
        */

        // DataView.RowFilter
        foreach (var Entry in ColumnFilters)
            Def.RowFilters.Add(Entry.Value);
 
    }
    public void Apply(GridViewDef Def)
    {
        GroupColumns.Clear();
        ColumnSummaries.Clear();
        ColumnFilters.Clear();
        
        ColumnFiltersChanged();
        GroupColumnsChanged();
        ColumnSummariesChanged();
        
        //DataGridColumn Column;
        //RowFilterDef RowFilterDef;

        /*
        string FieldName;
        for (int i = 0; i < Def.OrderList.Count; i++)
        {
            FieldName = Def.OrderList[i];
            Column = Grid.FindColumn(FieldName);
            if (Column != null)
            {
                Column.DisplayIndex = i;
                Column.IsVisible = !Def.HiddenList.Contains(FieldName);
                ColumnSummaries[Column] = Def.Summaries.ContainsKey(FieldName) ? Def.Summaries[FieldName]: AggregateType.None;

                RowFilterDef = Def.RowFilters.FirstOrDefault(x => FieldName.IsSameText(x.FieldName));
                if (RowFilterDef != null)
                    ColumnFilters[Column] = RowFilterDef;
            }
        }

        for (int i = 0; i < Def.GroupList.Count; i++)
        {
            FieldName = Def.GroupList[i];
            Column = Grid.FindColumn(FieldName);
            if (Column != null)
                GroupColumns.Add(Column);
        }
        */
        
        Dispatcher.UIThread.Post(() => 
        {  
            ColumnFiltersChanged();
            GroupColumnsChanged();
            ColumnSummariesChanged();

        }, DispatcherPriority.Background);

    }
    
    // ● properties
    public DataGrid Grid { get;  }
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

                    if (mnuContextMenu == null)
                    {
                        CreateContextMenu();
                        Grid.ColumnHeaderContextMenu = mnuContextMenu;
                    }
                    
                    ColumnListChanged();
                }
                else
                {
                    Grid.ColumnHeaderContextMenu = null;
                }
            }
        }
    }
 
}
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Tripous;
using Tripous.Data;

namespace Tripous.Avalon;

public partial class GridViewDefDialog : DialogWindow
{
    
    private GridViewDef ViewDef;
    
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (btnCancel == sender)
            this.ModalResult = ModalResult.Cancel;
        else if (btnOK == sender)
            await ControlsToItem();
        
        else if (btnToHidden == sender)
            ToHidden(); 
        else if (btnToHiddenAll == sender)
            ToHiddenAll(); 
        else if (btnToVisible == sender)
            ToVisible(); 
        else if (btnToVisibleAll == sender)
            ToVisibleAll(); 
        else if (btnVisibleUp == sender)
            MoveVisible(true); 
        else if (btnVisibleDown == sender)
            MoveVisible(false); 
        
        else if (btnToGrouped == sender)
            ToGrouped(); 
        else if (btnToGroupedAll == sender)
            ToGroupedAll(); 
        else if (btnToNotGrouped == sender)
            ToNotGrouped(); 
        else if (btnToNotGroupedAll == sender)
            ToNotGroupedAll(); 
        else if (btnGroupedUp == sender)
            MoveGrouped(true); 
        else if (btnGroupedDown == sender)
            MoveGrouped(false); 
        
        else if (btnAddRowFilter == sender)
            await AddRowFilter(); 
        else if (btnEditRowFilter == sender)
            await EditRowFilter(); 
        else if (btnDeleteRowFilter == sender)
            await DeleteRowFilter(); 
        
        else if (btnToSorted == sender)
            ToSorted(); 
        else if (btnToSortedAll == sender)
            ToSortedAll(); 
        else if (btnToNotSorted == sender)
            ToNotSorted(); 
        else if (btnToNotSortedAll == sender)
            ToNotSortedAll(); 
        else if (btnSortedUp == sender)
            MoveSorted(true); 
        else if (btnSortedDown == sender)
            MoveSorted(false); 
    }
    void lboColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 1. save previous
        if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is GridViewColumnDef OldItem)
            ControlsToColumn(OldItem);

        // 2. load new
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is GridViewColumnDef NewItem)
            ColumnToControls(NewItem);
    }
    async void lboColumns_DoubleClick(object sender, RoutedEventArgs e) => await AddRowFilter();

    void AnyColumnCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lboColumns.SelectedItem is GridViewColumnDef ColumnDef)
            ControlsToColumn(ColumnDef);
    }
    void VisibleList_DoubleClick(object sender, RoutedEventArgs e)
    {
        if (sender == lboVisibleColumns)
            ToHidden();
        else if (sender == lboHiddenColumns)
            ToVisible();
    }
    void GroupedList_DoubleClick(object sender, RoutedEventArgs e)
    {
        if (sender == lboGroupedColumns)
            ToNotGrouped();
        else if (sender == lboNotGroupedColumns)
            ToGrouped();
    }
    async void lboRowFilters_DoubleClick(object sender, RoutedEventArgs e)
    {
        await EditRowFilter();
    }
    void SortedList_DoubleClick(object sender, RoutedEventArgs e)
    {
        if (sender == lboSortedColumns)
            ToNotSorted();
        else if (sender == lboNotSortedColumns)
            ToSorted();
    }
    
    // ● private
    void ToHidden()
    {
        if (lboVisibleColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        Col.VisibleIndex = -1;
        RefreshVisibleLists();
    }
    void ToHiddenAll()
    {
        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.VisibleIndex = -1;

        RefreshVisibleLists();
    }
    void ToVisible()
    {
        if (lboHiddenColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        int Max = ViewDef.Columns.Where(x => x.VisibleIndex >= 0).Select(x => x.VisibleIndex).DefaultIfEmpty(-1).Max();
        Col.VisibleIndex = Max + 1;

        RefreshVisibleLists();
    }
    void ToVisibleAll()
    {
        int Index = 0;

        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.VisibleIndex = Index++;

        RefreshVisibleLists();
    }
    void MoveVisible(bool Up)
    {
        if (lboVisibleColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        var List = ViewDef.Columns
            .Where(x => x.VisibleIndex >= 0)
            .OrderBy(x => x.VisibleIndex)
            .ToList();

        int i = List.IndexOf(Col);
        if (i < 0)
            return;

        int j = Up ? i - 1 : i + 1;
        if (j < 0 || j >= List.Count)
            return;

        int Temp = List[i].VisibleIndex;
        List[i].VisibleIndex = List[j].VisibleIndex;
        List[j].VisibleIndex = Temp;

        RefreshVisibleLists();
    }
    void RefreshVisibleLists()
    {
        var Visible = ViewDef.Columns
            .Where(x => x.VisibleIndex >= 0)
            .OrderBy(x => x.VisibleIndex)
            .ToList();

        var Hidden = ViewDef.Columns
            .Where(x => x.VisibleIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboVisibleColumns.ItemsSource = Visible;
        lboHiddenColumns.ItemsSource = Hidden;
        
        edtSummary.Text = ViewDef.GetDescription();
    }

    void ToNotGrouped()
    {
        if (lboGroupedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        Col.GroupIndex = -1;
        RefreshGroupedLists();
    }
    void ToNotGroupedAll()
    {
        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.GroupIndex = -1;

        RefreshGroupedLists();
    }
    void ToGrouped()
    {
        if (lboNotGroupedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        int Max = ViewDef.Columns.Where(x => x.GroupIndex >= 0).Select(x => x.GroupIndex).DefaultIfEmpty(-1).Max();
        Col.GroupIndex = Max + 1;

        RefreshGroupedLists();
    }
    void ToGroupedAll()
    {
        int Index = 0;

        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.GroupIndex = Index++;

        RefreshGroupedLists();
    }
    void MoveGrouped(bool Up)
    {
        if (lboGroupedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        var List = ViewDef.Columns
            .Where(x => x.GroupIndex >= 0)
            .OrderBy(x => x.GroupIndex)
            .ToList();

        int i = List.IndexOf(Col);
        if (i < 0)
            return;

        int j = Up ? i - 1 : i + 1;
        if (j < 0 || j >= List.Count)
            return;

        int Temp = List[i].GroupIndex;
        List[i].GroupIndex = List[j].GroupIndex;
        List[j].GroupIndex = Temp;

        RefreshGroupedLists();
    }
    void RefreshGroupedLists()
    {
        var Grouped = ViewDef.Columns
            .Where(x => x.GroupIndex >= 0)
            .OrderBy(x => x.GroupIndex)
            .ToList();

        var NotGrouped = ViewDef.Columns
            .Where(x => x.GroupIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboGroupedColumns.ItemsSource = Grouped;
        lboNotGroupedColumns.ItemsSource = NotGrouped;
        edtSummary.Text = ViewDef.GetDescription();
    }
    
    async Task AddRowFilter()
    {
        if (lboColumns.SelectedItem is not GridViewColumnDef ColumnDef)
            return;

        if (ViewDef.RowFilters.Contains(ColumnDef.FieldName))
            return;
            
        RowFilterDef FilterItem = new RowFilterDef();
        FilterItem.ColumnDef = ColumnDef;
        FilterItem.BoolOp = BoolOp.And; // always AND
        FilterItem.FieldName = ColumnDef.FieldName;
 
        DialogData data = await DialogWindow.ShowModal<RowFilterItemDialog>(FilterItem, this);
        if (data.Result)
        {
            ViewDef.RowFilters.Add(FilterItem);
            RefreshRowFilterList();
            lboRowFilters.SelectedItem = FilterItem;
            tabRowFilters.IsSelected = true;
        }
    }
    async Task EditRowFilter()
    { 
        if (lboRowFilters.SelectedItem is not RowFilterDef FilterItem)
        return;
        
        GridViewColumnDef ColumnDef = ViewDef.Find(FilterItem.FieldName);
        if (ColumnDef == null)
        {
            string Message =
                $"Filter Field does not exist: {FilterItem.FieldName}.{Environment.NewLine}Please delete the Filter.";
            await MessageBox.Error(Message, this);
            return;
        }
        
        FilterItem.ColumnDef = ColumnDef;
        DialogData data = await DialogWindow.ShowModal<RowFilterItemDialog>(FilterItem,this);
        if (data.Result)
        {
            RefreshRowFilterList();
            lboRowFilters.SelectedItem = FilterItem;
        }
        
    }
    async Task DeleteRowFilter()
    {
        if (lboRowFilters.SelectedItem is not RowFilterDef FilterItem)
            return;

        string Message = "Delete selected Filter?";
        bool Flag = await MessageBox.YesNo(Message, this);
        if (!Flag)
            return;
        
        ViewDef.RowFilters.Remove(FilterItem);
        RefreshRowFilterList();
    }
    void RefreshRowFilterList()
    {
        var RowFilters = ViewDef.RowFilters.ToList();
        lboRowFilters.ItemsSource = RowFilters;
        edtSummary.Text = ViewDef.GetDescription();
    }

    void ToNotSorted()
    {
        if (lboSortedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        Col.SortIndex = -1;
        RefreshSortedLists();
    }
    void ToNotSortedAll()
    {
        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.SortIndex = -1;

        RefreshSortedLists();
    }
    void ToSorted()
    {
        if (lboNotSortedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        int Max = ViewDef.Columns
            .Where(x => x.SortIndex >= 0)
            .Select(x => x.SortIndex)
            .DefaultIfEmpty(-1).Max();
        Col.SortIndex = Max + 1;

        RefreshSortedLists();
    }
    void ToSortedAll()
    {
        int Index = 0;

        foreach (GridViewColumnDef Col in ViewDef.Columns)
            Col.SortIndex = Index++;

        RefreshSortedLists();
    }
    void MoveSorted(bool Up)
    {
        if (lboSortedColumns.SelectedItem is not GridViewColumnDef Col)
            return;

        var List = ViewDef.Columns
            .Where(x => x.SortIndex >= 0)
            .OrderBy(x => x.SortIndex)
            .ToList();

        int i = List.IndexOf(Col);
        if (i < 0)
            return;

        int j = Up ? i - 1 : i + 1;
        if (j < 0 || j >= List.Count)
            return;

        int Temp = List[i].SortIndex;
        List[i].SortIndex = List[j].SortIndex;
        List[j].SortIndex = Temp;

        RefreshSortedLists();
    }
    void RefreshSortedLists()
    {
        var Sorted = ViewDef.Columns
            .Where(x => x.SortIndex >= 0)
            .OrderBy(x => x.SortIndex)
            .ToList();

        var NotSorted = ViewDef.Columns
            .Where(x => x.SortIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboSortedColumns.ItemsSource = Sorted;
        lboNotSortedColumns.ItemsSource = NotSorted;
        edtSummary.Text = ViewDef.GetDescription();
    }
    
    void ColumnToControls(GridViewColumnDef Column)
    {
        if (Column == null)
            return;

        edtFieldName.Text = Column.FieldName;
        edtTitle.Text = Column.Title;
        edtDataType.Text = Column.DataType != null ? Column.DataType.Name : string.Empty;
        edtDisplayFormat.Text = Column.DisplayFormat;
        edtEditFormat.Text = Column.EditFormat;

        cboAggregate.SelectedItem = Column.Aggregate;
        cboSortDirection.SelectedItem = Column.SortDirection;
        cboBlobType.SelectedItem = Column.BlobType;

        chIsReadOnly.IsChecked = Column.IsReadOnly;
        chSourceAllowsNull.IsChecked = Column.SourceAllowsNull;
        chIsIntAsBool.IsChecked = Column.IsIntAsBool;

        edtDisplayMember.Text = Column.DisplayMember;
        edtValueMember.Text = Column.ValueMember;
        edtLookupSourceName.Text = Column.LookupSourceName;
        edtLookupSql.Text = Column.LookupSql;
    }
    void ControlsToColumn(GridViewColumnDef Column)
    {
        if (Column == null)
            return;

        Column.Title = edtTitle.Text;
        Column.DisplayFormat = edtDisplayFormat.Text;
        Column.EditFormat = edtEditFormat.Text;
        
        if (cboAggregate.SelectedItem is AggregateType Aggregate)
            Column.Aggregate = Aggregate;

        if (cboSortDirection.SelectedItem is ListSortDirection SortDirection)
            Column.SortDirection = SortDirection;

        if (cboBlobType.SelectedItem is BlobType BlobType)
            Column.BlobType = BlobType;

        Column.IsReadOnly  = chIsReadOnly.IsChecked == true;
        Column.IsIntAsBool = chIsIntAsBool.IsChecked == true;

        Column.DisplayMember = edtDisplayMember.Text;
        Column.ValueMember = edtValueMember.Text;
        Column.LookupSourceName = edtLookupSourceName.Text;
        Column.LookupSql = edtLookupSql.Text;
        
        edtSummary.Text = ViewDef.GetDescription();
    }
    
    // ● overrides
    protected override async Task WindowInitialize()
    {
        if (Design.IsDesignMode)
            return;
        
        ViewDef = InputData as GridViewDef;
        ResultData = ViewDef;
 
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        if (Design.IsDesignMode)
            return;

        if (ViewDef == null)
            return;

        // ● General
        edtName.Text = ViewDef.Name;
        edtName.IsReadOnly = ViewDef.IsNameReadOnly;
        chShowGroupColumnsAsDataColumns.IsChecked = ViewDef.ShowGroupColumnsAsDataColumns;
        edtSummary.Text = ViewDef.GetDescription();

        // ● Columns
        lboColumns.ItemsSource = ViewDef.Columns;
        cboAggregate.ItemsSource = Enum.GetValues(typeof(AggregateType));
        cboBlobType.ItemsSource = Enum.GetValues(typeof(BlobType));

        // ● Visible / Hidden
        var Visible = ViewDef.Columns
            .Where(x => x.VisibleIndex >= 0)
            .OrderBy(x => x.VisibleIndex)
            .ToList();

        var Hidden = ViewDef.Columns
            .Where(x => x.VisibleIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboVisibleColumns.ItemsSource = Visible;
        lboHiddenColumns.ItemsSource = Hidden;

        // ● Grouped / NotGrouped
        var Grouped = ViewDef.Columns
            .Where(x => x.GroupIndex >= 0)
            .OrderBy(x => x.GroupIndex)
            .ToList();

        var NotGrouped = ViewDef.Columns
            .Where(x => x.GroupIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboGroupedColumns.ItemsSource = Grouped;
        lboNotGroupedColumns.ItemsSource = NotGrouped;

        // ● Row Filters
        lboRowFilters.ItemsSource = ViewDef.RowFilters;
        
        // ● Sorting
        cboSortDirection.ItemsSource = Enum.GetValues(typeof(ListSortDirection));
        
        var Sorted = ViewDef.Columns
            .Where(x => x.SortIndex >= 0)
            .OrderBy(x => x.SortIndex)
            .ToList();

        var NotSorted = ViewDef.Columns
            .Where(x => x.SortIndex < 0)
            .OrderBy(x => x.Title)
            .ToList();

        lboSortedColumns.ItemsSource = Sorted;
        lboNotSortedColumns.ItemsSource = NotSorted;

        // miscs
        lboColumns.SelectionChanged += lboColumns_SelectionChanged;
        lboColumns.DoubleTapped += lboColumns_DoubleClick;

        cboAggregate.SelectionChanged += AnyColumnCombo_SelectionChanged;
        cboSortDirection.SelectionChanged += AnyColumnCombo_SelectionChanged;
        cboBlobType.SelectionChanged += AnyColumnCombo_SelectionChanged;

        if (ViewDef.Columns.Count > 0)
            lboColumns.SelectedIndex = 0;
        
        lboVisibleColumns.DoubleTapped += VisibleList_DoubleClick;
        lboHiddenColumns.DoubleTapped += VisibleList_DoubleClick;
        
        lboGroupedColumns.DoubleTapped += GroupedList_DoubleClick;
        lboNotGroupedColumns.DoubleTapped += GroupedList_DoubleClick;

        lboRowFilters.DoubleTapped += lboRowFilters_DoubleClick;

        lboSortedColumns.DoubleTapped += SortedList_DoubleClick;
        lboNotSortedColumns.DoubleTapped += SortedList_DoubleClick;

        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        if (Design.IsDesignMode)
            return;

        if (ViewDef == null)
            return;

        if (lboColumns.SelectedItem is GridViewColumnDef Column)
            ControlsToColumn(Column);

        ViewDef.Name = edtName.Text;
        ViewDef.ShowGroupColumnsAsDataColumns = chShowGroupColumnsAsDataColumns.IsChecked == true;

        this.ModalResult = ModalResult.Ok;
        await Task.CompletedTask;
    }
    
    // ● construction
    public GridViewDefDialog()
    {
        InitializeComponent();
    }
}
namespace Tripous.Avalon;

public partial class PivotDefDialog : DialogWindow
{
    private PivotDef PivotDef;
    private DataView DataView;
 
    private ObservableCollection<PivotColumnDefRow> AllColumns = new();
    private ObservableCollection<PivotColumnDefRow> RowColumnList = new();
    private ObservableCollection<PivotColumnDefRow> ColumnColumnList = new();
    private ObservableCollection<PivotColumnDefRow> ValueColumnList = new();
 
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (btnCancel == sender)
            this.ModalResult = ModalResult.Cancel;
        else if (btnOK == sender)
            await ControlsToItem();
 
        else if (btnUpRow == sender)
            MoveRow(true);
        else if (btnDownRow == sender)
            MoveRow(false);
 
        else if (btnUpColumn == sender)
            MoveColumn(true);
        else if (btnDownColumn == sender)
            MoveColumn(false);
 
        else if (btnUpValue == sender)
            MoveValue(true);
        else if (btnDownValue == sender)
            MoveValue(false);
    }
    
    // ● private
    void MoveRow(bool Up)
    {
        PivotColumnDefRow CI = lboRows.SelectedItem as PivotColumnDefRow;
        if (CI != null)
        {
            int Index = RowColumnList.IndexOf(CI);
            if (Index >= 0)
            {
                int NewIndex = Up ? Index - 1 : Index + 1;
                if (NewIndex >= 0 && NewIndex < RowColumnList.Count)
                {
                    RowColumnList.Move(Index, NewIndex);
                    lboRows.SelectedItem = CI;
                }
            }
        }
    }
    void MoveColumn(bool Up)
    {
        PivotColumnDefRow CI = lboColumns.SelectedItem as PivotColumnDefRow;
        if (CI != null)
        {
            int Index = ColumnColumnList.IndexOf(CI);
            if (Index >= 0)
            {
                int NewIndex = Up ? Index - 1 : Index + 1;
                if (NewIndex >= 0 && NewIndex < ColumnColumnList.Count)
                {
                    ColumnColumnList.Move(Index, NewIndex);
                    lboColumns.SelectedItem = CI;
                }
            }
        }
    }
    void MoveValue(bool Up)
    {
        PivotColumnDefRow ValueDef = lboValues.SelectedItem as PivotColumnDefRow;
        if (ValueDef != null)
        {
            int Index = ValueColumnList.IndexOf(ValueDef);
            if (Index >= 0)
            {
                int NewIndex = Up ? Index - 1 : Index + 1;
                if (NewIndex >= 0 && NewIndex < ValueColumnList.Count)
                {
                    ValueColumnList.Move(Index, NewIndex);
                    lboValues.SelectedItem = ValueDef;
                }
            }
        }
    }
    
    // ● overrides
    protected override async Task WindowInitialize()
    {
        if (Design.IsDesignMode)
            return;

        PivotDef = InputData as PivotDef;
        DataView = PivotDef.DataView;
        ResultData = PivotDef;
        btnCancel.Focus();
 
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        if (Design.IsDesignMode)
            return;
 
        PivotColumnDefRow ColumnDefRow;
        DataColumn Column;
        foreach (PivotColumnDef PivotColumnDef in PivotDef.Columns)
        {
            Column = DataView.Table.FindColumn(PivotColumnDef.FieldName);
            if (Column != null)
            {
                ColumnDefRow = new PivotColumnDefRow(PivotColumnDef, Column);
                AllColumns.Add(ColumnDefRow);
                
                if (PivotColumnDef.Axis == PivotAxis.Row)
                    RowColumnList.Add(ColumnDefRow);
                else if (PivotColumnDef.Axis == PivotAxis.Column)
                    ColumnColumnList.Add(ColumnDefRow);
                
                if (PivotColumnDef.IsValue)
                    ValueColumnList.Add(ColumnDefRow);
            }
        }

        // grid columns
        Grid.AutoGenerateColumns = false;
        Grid.CanUserAddRows = false;
        Grid.CanUserDeleteRows = false;
        
        DataGridColumn GridColumn;
        GridColumn = Grid.CreateColumn(typeof(string), "FieldName", "Field", true);
        GridColumn = Grid.CreateColumn(typeof(string), "Caption", "Caption", false);
        GridColumn = Grid.CreateColumn(typeof(PivotAxis), "Axis", "Axis", false);
        GridColumn = Grid.CreateColumn(typeof(bool), "IsValue", "Is Value", false);
        GridColumn = Grid.CreateColumn(typeof(PivotValueAggregateType), "ValueAggregateType", "Aggregate", false);
        GridColumn = Grid.CreateColumn(typeof(string), "Format", "Format", false);
        GridColumn = Grid.CreateColumn(typeof(bool), "SortByValue", "Sort By Value", false);
        GridColumn = Grid.CreateColumn(typeof(bool), "SortDescending", "Sort Descending", false);
        Grid.ItemsSource = AllColumns;
        
        // list boxes
        lboRows.ItemsSource = RowColumnList;
        lboColumns.ItemsSource = ColumnColumnList;
        lboValues.ItemsSource = ValueColumnList;
       
        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        if (Design.IsDesignMode)
            return;
        
        await Task.CompletedTask;
        
        this.ModalResult = ModalResult.Ok;
    }
 
    
    // ● construction
    public PivotDefDialog()
    {
        InitializeComponent();
    }
}
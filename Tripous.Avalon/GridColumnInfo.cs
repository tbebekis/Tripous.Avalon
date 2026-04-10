namespace Tripous.Avalon;

public class GridColumnInfo
{
    // ● constructor
    public GridColumnInfo(DataColumn TableColumn, DataGridColumn GridColumn)
    {
        this.TableColumn = TableColumn;
        this.GridColumn = GridColumn;

        FieldName = TableColumn.ColumnName;
        BindingPath = TripousAvalonExtensions.GetBindingPath(FieldName);
        DataType = TableColumn.DataType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        
      
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName)? FieldName:  base.ToString();
 

    // ● DataGrid
    public DataColumn TableColumn { get; }
    public DataGridColumn GridColumn { get; }
    public string FieldName { get; }
    public string BindingPath { get; }
    public Type DataType  { get; }
    public Type UnderlyingType { get; }
    public bool IsString => UnderlyingType.IsString(); //== typeof(string);
    public bool IsDate => UnderlyingType.IsDateTime(); // == typeof(DateTime);
    public bool IsNumeric => UnderlyingType.IsNumeric();
    public bool IsRowFilterSupportedColumn => IsString || IsNumeric || IsDate;

    public AggregateType[] ValidAggregates => UnderlyingType.GetValidAggregates();

}
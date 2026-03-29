namespace Tripous.Avalon;

public class GridColumnInfo
{
    public GridColumnInfo(DataColumn TableColumn, DataGridColumn GridColumn)
    {
        this.TableColumn = TableColumn;
        this.GridColumn = GridColumn;

        FieldName = TableColumn.ColumnName;
        BindingPath = TripousAvalonExtensions.GetBindingPath(FieldName);
        DataType = TableColumn.DataType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        
        GridColumn.ColumnKey = TableColumn.ColumnName;
    }

    
    public DataColumn TableColumn { get; }
    public DataGridColumn GridColumn { get; }
    public string FieldName { get; }
    public string BindingPath { get; }
    public Type DataType  { get; }
    public Type UnderlyingType { get; }
    
    public bool IsStringColumn => UnderlyingType == typeof(string);

    public bool IsDateColumn => UnderlyingType == typeof(DateTime);

    public bool IsNumericColumn =>
        UnderlyingType == typeof(byte) ||
        UnderlyingType == typeof(short) ||
        UnderlyingType == typeof(int) ||
        UnderlyingType == typeof(long) ||
        UnderlyingType == typeof(float) ||
        UnderlyingType == typeof(double) ||
        UnderlyingType == typeof(decimal);

    public bool IsRowFilterSupportedColumn => IsStringColumn || IsNumericColumn || IsDateColumn;
}
using System.Data;
using Avalonia.Controls;

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
}
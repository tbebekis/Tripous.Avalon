namespace Tripous.Desktop;

public class GridColumnInfo
{
    public GridColumnInfo(DataGridColumn GridColumn, DataColumn DataColumn)
    {
        this.GridColumn = GridColumn;
        this.DataColumn = DataColumn;
        this.FieldName = DataColumn.ColumnName;
        this.DataType = DataColumn.DataType;
    }
    public GridColumnInfo(DataGridColumn GridColumn, FieldDef FieldDef)
    {
        this.GridColumn = GridColumn;
        this.FieldDef = FieldDef;
        this.FieldName = FieldDef.Name;
        this.DataType = FieldDef.DataType.GetNetType();
    }
    public GridColumnInfo(DataGridColumn GridColumn, string FieldName, Type DataType)
    {
        this.GridColumn = GridColumn;
        this.FieldName = FieldName;
        this.DataType = DataType;
    }

    override public string ToString() => FieldName;
    
    public DataGridColumn GridColumn { get; }
    public DataColumn DataColumn { get; }
    public FieldDef FieldDef { get; }
    public string FieldName { get; }
    public Type DataType { get; }

}
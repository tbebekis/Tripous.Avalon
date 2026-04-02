namespace Tripous.Avalon;

public class PivotColumnInfo
{
    // ● constructor
    public PivotColumnInfo(DataColumn TableColumn)
    {
        this.TableColumn = TableColumn;
        FieldName = TableColumn.ColumnName;
        DataType = TableColumn.DataType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;
    }
    
    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName)? FieldName:  base.ToString();
 
    // ● properties
    public DataColumn TableColumn { get; }
    public string FieldName { get; }
    public Type DataType  { get; }
    public Type UnderlyingType { get; }
    public bool IsString => UnderlyingType.IsString(); //== typeof(string);
    public bool IsDate => UnderlyingType.IsDateTime(); // == typeof(DateTime);
    public bool IsNumeric => UnderlyingType.IsNumeric();
    public bool IsPivotSupportedColumn => IsString || IsNumeric || IsDate;
    public PivotValueAggregateType[] ValidAggregates => UnderlyingType.GetValidPivotAggregates();

    public PivotColumnDef ColumnDef { get; set; }
}
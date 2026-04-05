namespace Tripous.Data;

public class GridViewColumnDef
{
    // ● private fields
    private string fDisplayFormat;
    private string fEditFormat;
    private string fTitle;
    private bool fAllowsNullFromSource;

    // ● construction
    public GridViewColumnDef()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName) ? FieldName : base.ToString();
    public void SetAllowsNullFromSource(bool Value)
    {
        fAllowsNullFromSource = Value;
    }
    
    // ● properties
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The caption, header, of the column.
    /// </summary>
    public string Title
    {
        get => !string.IsNullOrWhiteSpace(fTitle) ? fTitle : FieldName;
        set => fTitle = value;
    }
    /// <summary>
    /// When below zero the column is not visible. Else this value is the order of the column in the visible columns.
    /// </summary>
    public int VisibleIndex { get; set; } = 0;
    /// <summary>
    /// When below zero the column is not part of the group. Else this value is the column index in the group.
    /// </summary>
    public int GroupIndex { get; set; } = -1;
    /// <summary>
    /// The aggregate type of the column. <see cref="AggregateType.None"/> means no aggregate is applied.
    /// </summary>
    public AggregateType Aggregate { get; set; } = AggregateType.None;
    /// <summary>
    /// When true the column is not editable.
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// The display format used by the grid cell when no explicit value is assigned.
    /// </summary>
    public string DisplayFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fDisplayFormat))
                return fDisplayFormat;

            if (UnderlyingType.IsNumeric() && !UnderlyingType.IsInteger())
                return "N2";

            if (UnderlyingType.IsDateTime())
                return "yyyy-MM-dd HH:mm";

            return string.Empty;
        }
        set => fDisplayFormat = value;
    }
    /// <summary>
    /// The edit format used by inplace editors when no explicit value is assigned.
    /// </summary>
    public string EditFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fEditFormat))
                return fEditFormat;

            if (UnderlyingType.IsDateTime())
                return "yyyy-MM-dd HH:mm";

            return string.Empty;
        }
        set => fEditFormat = value;
    }
    
    public string LookupSql { get; set; }
    public string DisplayMember { get; set; }
    public string ValueMember { get; set; }
    
    [JsonIgnore]
    public IEnumerable LookupItemsSource { get; set; }
    [JsonIgnore]
    public Func<IEnumerable> LookupItemsProvider { get; set; }
    [JsonIgnore]
    public bool IsLookup => LookupItemsSource != null || LookupItemsProvider != null;
    
    [JsonIgnore]
    public Type DataType { get; set; }
    [JsonIgnore]
    public Type UnderlyingType => DataType != null ? Nullable.GetUnderlyingType(DataType) ?? DataType : null;
    [JsonIgnore]
    public bool IsString => UnderlyingType != null && UnderlyingType.IsString();
    [JsonIgnore]
    public bool IsDateTime => UnderlyingType != null && UnderlyingType.IsDateTime();
    [JsonIgnore]
    public bool IsNumeric => UnderlyingType != null && UnderlyingType.IsNumeric();
    [JsonIgnore]
    public bool IsRowFilterSupportedColumn => IsString || IsNumeric || IsDateTime;
    [JsonIgnore]
    public bool IsNullable => 
        (DataType != null && Nullable.GetUnderlyingType(DataType) != null) 
        || fAllowsNullFromSource;
    [JsonIgnore]
    public AggregateType[] ValidAggregates => UnderlyingType != null ? UnderlyingType.GetValidAggregates() : Array.Empty<AggregateType>();
}
 
public class GridViewDef
{
    // ● construction
    public GridViewDef()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();
    public string GetDescription()
    {
        StringBuilder SB = new();
        
        // group
        string S = string.Join(", ", GetGroupColumns() );
        SB.AppendLine($"Group: [{S}]");
            
        // hidden columns
        S = string.Join(", ", GetHiddenColumns());
        SB.AppendLine($"Hidden: {S}");
            
        // summaries
        S = string.Join(", ", GetAggregateColumns()
            .Select(x => $"{x.FieldName} = {x.Aggregate}"));
        SB.AppendLine($"Summaries: {S}");
            
        // row filters
        S = RowFilters.Text;
        SB.AppendLine($"RowFilter: {S}");
            
        S = SB.ToString();
        return S;
    }
 
    /// <summary>
    /// Returns columns that participate in the group
    /// </summary>
    public List<GridViewColumnDef> GetGroupColumns() 
        => Columns.Where(x => x.GroupIndex >= 0).OrderBy(x => x.GroupIndex).ToList();
    /// <summary>
    /// Executes get columns.
    /// </summary>
    public List<GridViewColumnDef> GetAggregateColumns() => Columns.Where(x => x.Aggregate != AggregateType.None).ToList();
    /// <summary>
    /// Executes get values.
    /// </summary>
    public List<GridViewColumnDef> GetVisibleColumns() 
        => Columns.Where(x => x.VisibleIndex >= 0).OrderBy(x => x.VisibleIndex).ToList();
    /// <summary>
    /// Executes get values.
    /// </summary>
    public List<GridViewColumnDef> GetHiddenColumns() => Columns.Where(x => x.VisibleIndex < 0).ToList();

    public GridViewColumnDef Find(string FieldName) => Columns.FirstOrDefault(x => FieldName.IsSameText(x.FieldName));
    public bool Contains(string FieldName) => Columns.Any(x => x.FieldName.IsSameText(FieldName));
    public GridViewColumnDef Get(string FieldName)
    {
        GridViewColumnDef Result = Find(FieldName);
        if (Result == null)
            throw new ApplicationException($"Column not found: {FieldName}");
        return Result;
    }

    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Returns all columns
    /// </summary>
    public List<GridViewColumnDef> Columns { get; set; } = new();
    /// <summary>
    /// RowFilter definitions for columns participating in the DataView filtering.
    /// </summary>
    public RowFilterDefs RowFilters { get; set; } = new();
    /// <summary>
    /// When true then grouped columns are displayed as data columns too.
    /// </summary>
    public bool ShowGroupColumnsAsDataColumns { get; set; } = false;

    [JsonIgnore] 
    public GridViewColumnDef this[string FieldName] => Get(FieldName);
    
    [JsonIgnore]
    public object Tag { get; set; }
}
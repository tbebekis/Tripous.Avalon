namespace Tripous.Data;

/// <summary>
/// Pivot definition.
/// </summary>
public class PivotDef
{
    // ● public
    /// <summary>
    /// Executes get rows.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetRows() => Columns.Where(x => x.Axis == PivotAxis.Row);
    /// <summary>
    /// Executes get columns.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetColumns() => Columns.Where(x => x.Axis == PivotAxis.Column);
    /// <summary>
    /// Executes get values.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetValues() => Columns.Where(x => x.IsValue);
    
    // ● properties
    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public ObservableCollection<PivotColumnDef> Columns { get; set; } = new();
    /// <summary>
    /// Gets or sets the show subtotals.
    /// </summary>
    public bool ShowSubtotals { get; set; } = true;
    /// <summary>
    /// Gets or sets the show grand totals.
    /// </summary>
    public bool ShowGrandTotals { get; set; } = true;
    /// <summary>
    /// Still ignored by the current engine. Values are rendered on columns.
    /// </summary>
    public bool ShowValuesOnRows { get; set; }
    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
    [JsonIgnore]
    public DataView DataView { get; set; }
}
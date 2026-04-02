namespace Tripous.Data;

/// <summary>
/// Pivot output data.
/// </summary>
public class PivotData
{
    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public List<PivotDataColumn> Columns { get; } = new();
    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    public List<PivotDataRow> Rows { get; } = new();
}
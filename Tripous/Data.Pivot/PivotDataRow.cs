namespace Tripous.Data;

/// <summary>
/// A pivot output row.
/// </summary>
public class PivotDataRow
{
    /// <summary>
    /// Gets or sets the values.
    /// </summary>
    public object[] Values { get; set; }
    /// <summary>
    /// Gets or sets the row type.
    /// </summary>
    public PivotDataRowType RowType { get; set; } = PivotDataRowType.Normal;
    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public int Level { get; set; } = -1;

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}
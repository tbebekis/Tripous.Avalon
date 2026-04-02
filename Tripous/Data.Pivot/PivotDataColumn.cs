namespace Tripous.Data;

/// <summary>
/// A pivot output column.
/// </summary>
public class PivotDataColumn
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    public string Caption { get; set; }
    /// <summary>
    /// Gets or sets the data type.
    /// </summary>
    public Type DataType { get; set; }
    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public PivotDataColumnKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the row level.
    /// </summary>
    public int RowLevel { get; set; } = -1;
    /// <summary>
    /// Gets or sets the source column.
    /// </summary>
    public PivotColumnDef SourceColumn { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}
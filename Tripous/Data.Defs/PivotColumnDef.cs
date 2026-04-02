namespace Tripous.Data;

/// <summary>
/// Pivot column definition.
/// </summary>
public class PivotColumnDef
{
    private string fCaption;

    // ● public
    public override string ToString()
    {
        return $"{FieldName}, Axis: {Axis}, IsValue: {IsValue}";
    }

    // ● properties
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// Gets or sets the axis.
    /// </summary>
    public PivotAxis Axis { get; set; }
    /// <summary>
    /// Gets or sets the is value.
    /// </summary>
    public bool IsValue { get; set; }
    /// <summary>
    /// Gets or sets the value aggregate type.
    /// </summary>
    public PivotValueAggregateType ValueAggregateType { get; set; }
    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    public string Caption
    {
        get => !string.IsNullOrWhiteSpace(fCaption) ? fCaption : FieldName;
        set => fCaption = value;
    }
    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Gets or sets the sort descending.
    /// </summary>
    public bool SortDescending { get; set; }
    /// <summary>
    /// Gets or sets the sort by value.
    /// </summary>
    public bool SortByValue { get; set; } = true;
    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}
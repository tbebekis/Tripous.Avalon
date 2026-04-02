using System.Text.Json.Serialization;

namespace PivotTestApp;

public class PivotValueDef
{
    private string fCaption;
    
    // ● construction
    public PivotValueDef()
    {
    }
    
    // ● public
    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Caption))
            return Caption;

        return !string.IsNullOrWhiteSpace(FieldName)
            ? $"{AggregateType}({FieldName})"
            : base.ToString();
    }

    public PivotValueDef Clone()
    {
        PivotValueDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }

    public void AssignFrom(PivotValueDef Source)
    {
        FieldName = Source.FieldName;
        AggregateType = Source.AggregateType;
        Caption = Source.Caption;
        Format = Source.Format;
    }
 
    // ● properties
    /// <summary>
    /// The source field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The aggregate function applied to the field.
    /// </summary>
    public PivotValueAggregateType AggregateType { get; set; }
    /// <summary>
    /// Optional display caption.
    /// </summary>
    public string Caption
    {
        get => !string.IsNullOrWhiteSpace(fCaption) ? fCaption : FieldName;
        set => fCaption = value;
    }
    /// <summary>
    /// How to format numbers
    /// </summary>
    public string Format { get; set; } = "C0";  // N0

    [JsonIgnore]
    public object Tag { get; set; }
}



namespace Tripous.Data;

/// <summary>
/// Specifies pivot value aggregate type.
/// </summary>
public enum PivotValueAggregateType
{
    None,
    Sum,
    Avg,
    Count,
    Min,
    Max,
    StdDev,
    StdDevP,
    Variance,
    VarianceP,
    CountDistinct,
    Product
}
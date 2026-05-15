namespace Tripous.Data;

public enum AggregateType
{
    None,
    Sum,
    Avg,
    Count,
    Min,
    Max, 
}

static public class AggregateTypes
{
    static public AggregateType[] GetValidAggregates(this Type DataType)
    {
        if (DataType == null)
            return Array.Empty<AggregateType>();

        if (DataType.IsNumeric())
        {
            return new[]
            {
                AggregateType.Count,
                AggregateType.Sum,
                AggregateType.Avg,
                AggregateType.Min,
                AggregateType.Max
            };
        }

        if (DataType.IsDateTime())
        {
            return new[]
            {
                AggregateType.Count,
                AggregateType.Min,
                AggregateType.Max
            };
        }

        return new[]
        {
            AggregateType.Count
        };
    }
}
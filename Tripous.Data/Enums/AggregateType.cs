/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Specifies the aggregate operation to be applied to a data column.
/// </summary>
public enum AggregateType
{
    /// <summary>
    /// No aggregate operation.
    /// </summary>
    None,
    /// <summary>
    /// Sum of values.
    /// </summary>
    Sum,
    /// <summary>
    /// Average of values.
    /// </summary>
    Avg,
    /// <summary>
    /// Number of values.
    /// </summary>
    Count,
    /// <summary>
    /// Minimum value.
    /// </summary>
    Min,
    /// <summary>
    /// Maximum value.
    /// </summary>
    Max, 
}

/// <summary>
/// Provides helper methods for aggregate types.
/// </summary>
static public class AggregateTypes
{
    /// <summary>
    /// Returns the aggregate operations supported by a specified data type.
    /// </summary>
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
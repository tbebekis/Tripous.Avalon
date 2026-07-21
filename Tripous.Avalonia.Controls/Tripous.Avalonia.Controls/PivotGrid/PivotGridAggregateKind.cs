// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines aggregate operations supported by a pivot grid measure.
/// </summary>
public enum PivotGridAggregateKind
{
    /// <summary>
    /// Counts source rows.
    /// </summary>
    Count,
    /// <summary>
    /// Sums numeric values.
    /// </summary>
    Sum,
    /// <summary>
    /// Returns the minimum comparable value.
    /// </summary>
    Min,
    /// <summary>
    /// Returns the maximum comparable value.
    /// </summary>
    Max,
    /// <summary>
    /// Returns the average numeric value.
    /// </summary>
    Average,
    /// <summary>
    /// Returns the sample standard deviation of numeric values.
    /// </summary>
    StdDev,
    /// <summary>
    /// Returns the population standard deviation of numeric values.
    /// </summary>
    StdDevP,
    /// <summary>
    /// Returns the sample variance of numeric values.
    /// </summary>
    Variance,
    /// <summary>
    /// Returns the population variance of numeric values.
    /// </summary>
    VarianceP,
    /// <summary>
    /// Counts distinct non-empty values.
    /// </summary>
    CountDistinct,
    /// <summary>
    /// Returns the product of numeric values.
    /// </summary>
    Product,
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies an aggregate calculation kind for group grid summaries.
/// </summary>
public enum GroupGridAggregateKind
{
    /// <summary>
    /// No aggregate.
    /// </summary>
    None,
    /// <summary>
    /// Counts rows.
    /// </summary>
    Count,
    /// <summary>
    /// Sums values.
    /// </summary>
    Sum,
    /// <summary>
    /// Returns the minimum value.
    /// </summary>
    Min,
    /// <summary>
    /// Returns the maximum value.
    /// </summary>
    Max,
    /// <summary>
    /// Returns the average value.
    /// </summary>
    Average,
}

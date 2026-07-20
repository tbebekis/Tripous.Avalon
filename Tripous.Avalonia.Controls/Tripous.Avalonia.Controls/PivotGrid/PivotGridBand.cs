// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines the visual bands of a pivot grid.
/// </summary>
public enum PivotGridBand
{
    /// <summary>
    /// No band.
    /// </summary>
    None,
    /// <summary>
    /// The available field panel.
    /// </summary>
    FieldPanel,
    /// <summary>
    /// The row, column, and measure axis panel.
    /// </summary>
    AxisPanel,
    /// <summary>
    /// The corner area where row and column headers meet.
    /// </summary>
    Corner,
    /// <summary>
    /// The row-axis header area.
    /// </summary>
    RowHeader,
    /// <summary>
    /// The column-axis header area.
    /// </summary>
    ColumnHeader,
    /// <summary>
    /// The value cell body area.
    /// </summary>
    Body,
}

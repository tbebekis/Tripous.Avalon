// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines layout metrics used by the pivot grid visual surface.
/// </summary>
public class PivotGridLayoutMetrics
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridLayoutMetrics"/> class.
    /// </summary>
    public PivotGridLayoutMetrics()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the available fields panel height.
    /// </summary>
    public double FieldPanelHeight { get; set; } = 42;
    /// <summary>
    /// Gets or sets the axis drop-zone panel height.
    /// </summary>
    public double AxisPanelHeight { get; set; } = 38;
    /// <summary>
    /// Gets or sets the row header width.
    /// </summary>
    public double RowHeaderWidth { get; set; } = 180;
    /// <summary>
    /// Gets or sets the column header height.
    /// </summary>
    public double ColumnHeaderHeight { get; set; } = 56;
    /// <summary>
    /// Gets or sets the body row height.
    /// </summary>
    public double RowHeight { get; set; } = 28;
    /// <summary>
    /// Gets or sets the row tree indent width.
    /// </summary>
    public double RowIndentWidth { get; set; } = 18;
    /// <summary>
    /// Gets or sets the row expander width.
    /// </summary>
    public double RowExpanderWidth { get; set; } = 18;
    /// <summary>
    /// Gets or sets the default value cell width.
    /// </summary>
    public double ValueCellWidth { get; set; } = 110;
    /// <summary>
    /// Gets or sets the vertical scroll bar width.
    /// </summary>
    public double VerticalScrollBarWidth { get; set; } = 14;
    /// <summary>
    /// Gets or sets the vertical scroll thumb minimum height.
    /// </summary>
    public double VerticalScrollThumbMinHeight { get; set; } = 24;
    /// <summary>
    /// Gets or sets the horizontal scroll bar height.
    /// </summary>
    public double HorizontalScrollBarHeight { get; set; } = 14;
    /// <summary>
    /// Gets or sets the horizontal scroll thumb minimum width.
    /// </summary>
    public double HorizontalScrollThumbMinWidth { get; set; } = 24;
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines the simple layout metrics used by the non-visual group grid model.
/// </summary>
public class GroupGridLayoutMetrics
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridLayoutMetrics"/> class.
    /// </summary>
    public GroupGridLayoutMetrics()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the toolbar band height.
    /// </summary>
    public double ToolBarHeight { get; set; } = 32;
    /// <summary>
    /// Gets or sets the group panel band height.
    /// </summary>
    public double GroupPanelHeight { get; set; } = 32;
    /// <summary>
    /// Gets or sets the column header band height.
    /// </summary>
    public double ColumnHeaderHeight { get; set; } = 28;
    /// <summary>
    /// Gets or sets the filter row band height.
    /// </summary>
    public double FilterRowHeight { get; set; } = 28;
    /// <summary>
    /// Gets or sets the body row height.
    /// </summary>
    public double RowHeight { get; set; } = 26;
    /// <summary>
    /// Gets or sets the footer summary band height.
    /// </summary>
    public double FooterSummaryHeight { get; set; } = 28;
    /// <summary>
    /// Gets or sets the group level indent width.
    /// </summary>
    public double GroupIndentWidth { get; set; } = 18;
    /// <summary>
    /// Gets or sets the group expander hit-test width.
    /// </summary>
    public double GroupExpanderWidth { get; set; } = 18;
    /// <summary>
    /// Gets or sets the column resize handle hit-test width.
    /// </summary>
    public double ColumnResizeHandleWidth { get; set; } = 5;
    /// <summary>
    /// Gets or sets the vertical scroll bar width.
    /// </summary>
    public double VerticalScrollBarWidth { get; set; } = 14;
    /// <summary>
    /// Gets or sets the minimum vertical scroll thumb height.
    /// </summary>
    public double VerticalScrollThumbMinHeight { get; set; } = 24;
    /// <summary>
    /// Gets or sets the horizontal scroll bar height.
    /// </summary>
    public double HorizontalScrollBarHeight { get; set; } = 14;
    /// <summary>
    /// Gets or sets the minimum horizontal scroll thumb width.
    /// </summary>
    public double HorizontalScrollThumbMinWidth { get; set; } = 24;
}

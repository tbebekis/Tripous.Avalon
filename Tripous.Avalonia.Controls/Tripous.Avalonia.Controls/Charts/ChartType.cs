// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines chart types supported by <see cref="ChartControl"/>.
/// </summary>
public enum ChartType
{
    /// <summary>
    /// Vertical column chart.
    /// </summary>
    Column,
    /// <summary>
    /// Horizontal bar chart.
    /// </summary>
    Bar,
    /// <summary>
    /// Line chart.
    /// </summary>
    Line,
    /// <summary>
    /// Filled line chart.
    /// </summary>
    Area,
    /// <summary>
    /// Pie chart.
    /// </summary>
    Pie,
    /// <summary>
    /// Donut chart.
    /// </summary>
    Donut,
    /// <summary>
    /// Stacked vertical column chart.
    /// </summary>
    StackedColumn,
    /// <summary>
    /// Stacked horizontal bar chart.
    /// </summary>
    StackedBar,
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents an aggregated chart data point.
/// </summary>
public class ChartDataPoint
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartDataPoint"/> class.
    /// </summary>
    public ChartDataPoint()
    {
    }

    // ● public methods
    /// <summary>
    /// Returns the display text.
    /// </summary>
    /// <returns>The display text.</returns>
    public override string ToString() => Text;

    // ● properties
    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public string CategoryKey { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the category text.
    /// </summary>
    public string CategoryText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the series key.
    /// </summary>
    public string SeriesKey { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the series text.
    /// </summary>
    public string SeriesText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the aggregated value.
    /// </summary>
    public object Value { get; set; }
    /// <summary>
    /// Gets or sets the numeric value used by the renderer.
    /// </summary>
    public decimal NumericValue { get; set; }
    /// <summary>
    /// Gets or sets the formatted value text.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}

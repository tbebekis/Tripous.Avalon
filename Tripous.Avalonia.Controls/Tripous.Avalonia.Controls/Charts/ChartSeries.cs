// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a chart series.
/// </summary>
public class ChartSeries
{
    // ● private fields
    readonly List<ChartDataPoint> fPoints = new();

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSeries"/> class.
    /// </summary>
    public ChartSeries()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the series key.
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display text.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Gets the series points.
    /// </summary>
    public IList<ChartDataPoint> Points => fPoints;
}

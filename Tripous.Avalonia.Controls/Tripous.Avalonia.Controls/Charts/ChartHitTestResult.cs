// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents chart hit-test information.
/// </summary>
public class ChartHitTestResult
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartHitTestResult"/> class.
    /// </summary>
    public ChartHitTestResult()
    {
    }

    // ● static public methods
    /// <summary>
    /// Creates an empty hit-test result.
    /// </summary>
    /// <returns>The result.</returns>
    static public ChartHitTestResult Empty() => new();

    // ● properties
    /// <summary>
    /// Gets or sets the hit kind.
    /// </summary>
    public ChartHitTestKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the series index.
    /// </summary>
    public int SeriesIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the point index.
    /// </summary>
    public int PointIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    public ChartSeries Series { get; set; }
    /// <summary>
    /// Gets or sets the data point.
    /// </summary>
    public ChartDataPoint DataPoint { get; set; }
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents persisted pivot grid measure settings.
/// </summary>
public class PivotGridMeasureSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridMeasureSettings"/> class.
    /// </summary>
    public PivotGridMeasureSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the measure name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the source field name.
    /// </summary>
    public string SourceFieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the aggregate operation.
    /// </summary>
    public PivotGridAggregateKind AggregateKind { get; set; } = PivotGridAggregateKind.Sum;
    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the value cell width.
    /// </summary>
    public double Width { get; set; }
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents persisted chart settings.
/// </summary>
public class ChartSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSettings"/> class.
    /// </summary>
    public ChartSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the settings name.
    /// </summary>
    public string Name { get; set; } = "Default";
    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the chart type.
    /// </summary>
    public ChartType ChartType { get; set; } = ChartType.Column;
    /// <summary>
    /// Gets or sets the category field name.
    /// </summary>
    public string CategoryFieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the optional series field name.
    /// </summary>
    public string SeriesFieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the value field name.
    /// </summary>
    public string ValueFieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the aggregate kind.
    /// </summary>
    public ChartAggregateKind AggregateKind { get; set; } = ChartAggregateKind.Sum;
    /// <summary>
    /// Gets or sets the category sort direction.
    /// </summary>
    public ChartSortDirection SortDirection { get; set; }
    /// <summary>
    /// Gets or sets the optional category limit. Zero means unlimited.
    /// </summary>
    public int TopN { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the legend is displayed.
    /// </summary>
    public bool ShowLegend { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether value labels are displayed.
    /// </summary>
    public bool ShowValueLabels { get; set; }
    /// <summary>
    /// Gets or sets the value display format.
    /// </summary>
    public string ValueFormat { get; set; } = "N2";
    /// <summary>
    /// Gets or sets the palette name.
    /// </summary>
    public string PaletteName { get; set; } = "Business";
}

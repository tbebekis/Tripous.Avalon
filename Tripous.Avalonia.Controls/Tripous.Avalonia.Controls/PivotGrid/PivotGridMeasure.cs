// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a value measure aggregated by a pivot grid.
/// </summary>
public class PivotGridMeasure
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridMeasure"/> class.
    /// </summary>
    public PivotGridMeasure()
    {
    }

    // ● public methods
    /// <summary>
    /// Formats a measure value for display.
    /// </summary>
    /// <param name="Value">The measure value.</param>
    /// <returns>The display text.</returns>
    public virtual string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(DisplayFormat))
            return string.Format(CultureInfo.CurrentCulture, $"{{0:{DisplayFormat}}}", Value);

        return string.Format(CultureInfo.CurrentCulture, "{0}", Value);
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
    /// Gets or sets the source field name used by the measure.
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
    /// Gets or sets the measure cell width.
    /// </summary>
    public double Width { get; set; } = 110;
}

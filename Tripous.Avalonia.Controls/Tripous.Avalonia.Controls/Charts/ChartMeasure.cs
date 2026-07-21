// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a chart measure.
/// </summary>
public class ChartMeasure
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartMeasure"/> class.
    /// </summary>
    public ChartMeasure()
    {
    }

    // ● public methods
    /// <summary>
    /// Formats a measure value.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns>The formatted text.</returns>
    public string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(ValueFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(ValueFormat, CultureInfo.CurrentCulture);

        return Convert.ToString(Value, CultureInfo.CurrentCulture) ?? string.Empty;
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
    /// Gets or sets the aggregate kind.
    /// </summary>
    public ChartAggregateKind AggregateKind { get; set; } = ChartAggregateKind.Sum;
    /// <summary>
    /// Gets or sets the value display format.
    /// </summary>
    public string ValueFormat { get; set; } = "N2";
}

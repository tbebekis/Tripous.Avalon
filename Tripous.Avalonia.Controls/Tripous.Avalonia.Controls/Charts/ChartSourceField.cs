// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a field discovered from a chart data source.
/// </summary>
public class ChartSourceField
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSourceField"/> class.
    /// </summary>
    public ChartSourceField()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the field data type.
    /// </summary>
    public Type ValueType { get; set; } = typeof(object);
    /// <summary>
    /// Gets or sets a value indicating whether the field can be used as a category or series field.
    /// </summary>
    public bool CanUseAsDimension { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field can be used as a measure source.
    /// </summary>
    public bool CanUseAsMeasure { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field is numeric.
    /// </summary>
    public bool IsNumeric { get; set; }
}

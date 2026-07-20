// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a field discovered from a pivot grid data source.
/// </summary>
public class PivotGridSourceField
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridSourceField"/> class.
    /// </summary>
    public PivotGridSourceField()
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
    /// Gets or sets a value indicating whether the field can be used in a row or column axis.
    /// </summary>
    public bool CanUseAsAxis { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field can be used as a measure source.
    /// </summary>
    public bool CanUseAsMeasure { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field is numeric.
    /// </summary>
    public bool IsNumeric { get; set; }
}

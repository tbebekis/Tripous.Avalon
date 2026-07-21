// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a source field while editing pivot grid layout settings.
/// </summary>
public class PivotGridFieldSettingsItem
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridFieldSettingsItem"/> class.
    /// </summary>
    public PivotGridFieldSettingsItem()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the source field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the source value type.
    /// </summary>
    public Type ValueType { get; set; } = typeof(object);
    /// <summary>
    /// Gets or sets a value indicating whether the field can be used in an axis.
    /// </summary>
    public bool CanUseAsAxis { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field can be used as a value measure.
    /// </summary>
    public bool CanUseAsMeasure { get; set; }
    /// <summary>
    /// Gets or sets the current layout role.
    /// </summary>
    public PivotGridFieldRole Role { get; set; }
    /// <summary>
    /// Gets or sets the aggregate operation used when the field is a value measure.
    /// </summary>
    public PivotGridAggregateKind AggregateKind { get; set; } = PivotGridAggregateKind.Sum;
    /// <summary>
    /// Gets or sets the display format used when the field is a value measure.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the cell width used when the field is a value measure.
    /// </summary>
    public double Width { get; set; }
}

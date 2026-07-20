// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a field used in a pivot grid row or column axis.
/// </summary>
public class PivotGridField
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridField"/> class.
    /// </summary>
    public PivotGridField()
    {
    }

    // ● public methods
    /// <summary>
    /// Formats a field value for display.
    /// </summary>
    /// <param name="Value">The field value.</param>
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
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the field width when displayed as an axis member.
    /// </summary>
    public double Width { get; set; } = 120;
}

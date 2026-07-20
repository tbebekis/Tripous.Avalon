// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a selectable value in the pivot grid filter dialog.
/// </summary>
public class PivotGridFilterValueItem
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridFilterValueItem"/> class.
    /// </summary>
    public PivotGridFilterValueItem()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the source value.
    /// </summary>
    public object Value { get; set; }
    /// <summary>
    /// Gets or sets the display text.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the value is accepted by the filter.
    /// </summary>
    public bool IsChecked { get; set; }
}

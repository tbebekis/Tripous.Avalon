// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes persisted settings for a single group grid column.
/// </summary>
public class GroupGridColumnSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridColumnSettings"/> class.
    /// </summary>
    public GroupGridColumnSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the column name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header used by configuration dialogs.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the column is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets the visible column order index.
    /// </summary>
    public int VisibleIndex { get; set; }
    /// <summary>
    /// Gets or sets the column width, or 0 when width is not persisted.
    /// </summary>
    public double Width { get; set; }
    /// <summary>
    /// Gets or sets the group order index, or -1 when the column is not grouped.
    /// </summary>
    public int GroupIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the column filter text.
    /// </summary>
    public string FilterText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the group summary aggregate kind.
    /// </summary>
    public GroupGridAggregateKind GroupSummary { get; set; }
    /// <summary>
    /// Gets or sets the total summary aggregate kind.
    /// </summary>
    public GroupGridAggregateKind TotalSummary { get; set; }
}

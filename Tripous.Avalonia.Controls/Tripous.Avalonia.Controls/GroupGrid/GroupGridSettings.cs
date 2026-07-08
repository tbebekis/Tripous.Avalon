// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes serializable user settings for a group grid layout.
/// </summary>
public class GroupGridSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridSettings"/> class.
    /// </summary>
    public GroupGridSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the settings name.
    /// </summary>
    public string Name { get; set; } = "Default";
    /// <summary>
    /// Gets or sets the sorted column name.
    /// </summary>
    public string SortColumnName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public GroupGridSortDirection SortDirection { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the toolbar band is visible.
    /// </summary>
    public bool IsToolBarVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the group panel band is visible.
    /// </summary>
    public bool IsGroupPanelVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the column header band is visible.
    /// </summary>
    public bool IsColumnHeadersVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the filter panel band is visible.
    /// </summary>
    public bool IsFilterPanelVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the totals summary band is visible.
    /// </summary>
    public bool IsTotalsSummaryVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the default insert toolbar button is visible.
    /// </summary>
    public bool IsInsertButtonVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the default delete toolbar button is visible.
    /// </summary>
    public bool IsDeleteButtonVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the default edit toolbar button is visible.
    /// </summary>
    public bool IsEditButtonVisible { get; set; }
    /// <summary>
    /// Gets or sets the column settings.
    /// </summary>
    public List<GroupGridColumnSettings> Columns { get; set; } = new();
}

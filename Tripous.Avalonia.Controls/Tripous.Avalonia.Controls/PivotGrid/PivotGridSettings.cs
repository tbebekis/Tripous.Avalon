// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents persisted pivot grid layout settings.
/// </summary>
public class PivotGridSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridSettings"/> class.
    /// </summary>
    public PivotGridSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the settings name.
    /// </summary>
    public string Name { get; set; } = "Default";
    /// <summary>
    /// Gets or sets row axis field settings.
    /// </summary>
    public List<PivotGridFieldSettings> RowFields { get; set; } = new();
    /// <summary>
    /// Gets or sets column axis field settings.
    /// </summary>
    public List<PivotGridFieldSettings> ColumnFields { get; set; } = new();
    /// <summary>
    /// Gets or sets measure settings.
    /// </summary>
    public List<PivotGridMeasureSettings> Measures { get; set; } = new();
    /// <summary>
    /// Gets or sets a value indicating whether the top field panel is displayed.
    /// </summary>
    public bool ShowFieldPanel { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether row grand totals are displayed as a total column.
    /// </summary>
    public bool ShowRowGrandTotals { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether column grand totals are displayed as a total row.
    /// </summary>
    public bool ShowColumnGrandTotals { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether hover tooltips are displayed.
    /// </summary>
    public bool ShowToolTips { get; set; } = true;
    /// <summary>
    /// Gets or sets the row header width.
    /// </summary>
    public double RowHeaderWidth { get; set; }
    /// <summary>
    /// Gets or sets visible value column width overrides.
    /// </summary>
    public Dictionary<string, double> ValueColumnWidths { get; set; } = new();
    /// <summary>
    /// Gets or sets collapsed row-axis node keys.
    /// </summary>
    public List<string> CollapsedRowKeys { get; set; } = new();
    /// <summary>
    /// Gets or sets value-list filter settings.
    /// </summary>
    public List<PivotGridFilterSettings> Filters { get; set; } = new();
    /// <summary>
    /// Gets or sets the active sort role.
    /// </summary>
    public PivotGridFieldRole SortRole { get; set; }
    /// <summary>
    /// Gets or sets the active sort field name.
    /// </summary>
    public string SortFieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the active sort direction.
    /// </summary>
    public PivotGridSortDirection SortDirection { get; set; }
}

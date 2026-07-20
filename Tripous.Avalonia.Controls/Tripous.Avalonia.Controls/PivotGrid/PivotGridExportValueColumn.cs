// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a value column included in a pivot grid export snapshot.
/// </summary>
public class PivotGridExportValueColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridExportValueColumn"/> class.
    /// </summary>
    /// <param name="ColumnItem">The source column axis item.</param>
    /// <param name="ColumnIndex">The visible column index.</param>
    /// <param name="Measure">The export measure.</param>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <param name="IsTotal">True when this is a row grand-total column.</param>
    public PivotGridExportValueColumn(PivotGridAxisItem ColumnItem, int ColumnIndex, PivotGridExportMeasure Measure, int MeasureIndex, bool IsTotal)
    {
        this.ColumnItem = ColumnItem;
        this.ColumnIndex = ColumnIndex;
        this.Measure = Measure;
        this.MeasureIndex = MeasureIndex;
        this.IsTotal = IsTotal;
        ColumnText = IsTotal ? "Total" : ColumnItem?.Text ?? string.Empty;
        Header = string.IsNullOrWhiteSpace(ColumnText) ? Measure?.Header ?? string.Empty : ColumnText + " / " + (Measure?.Header ?? string.Empty);
    }

    // ● properties
    /// <summary>
    /// Gets the source column axis item.
    /// </summary>
    public PivotGridAxisItem ColumnItem { get; }
    /// <summary>
    /// Gets the visible column index.
    /// </summary>
    public int ColumnIndex { get; }
    /// <summary>
    /// Gets the export measure.
    /// </summary>
    public PivotGridExportMeasure Measure { get; }
    /// <summary>
    /// Gets the measure index.
    /// </summary>
    public int MeasureIndex { get; }
    /// <summary>
    /// Gets a value indicating whether this is a row grand-total column.
    /// </summary>
    public bool IsTotal { get; }
    /// <summary>
    /// Gets the column-axis display text.
    /// </summary>
    public string ColumnText { get; }
    /// <summary>
    /// Gets the flattened export header text.
    /// </summary>
    public string Header { get; }
}

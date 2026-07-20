// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents an aggregated pivot grid value cell.
/// </summary>
public class PivotGridValueCell
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridValueCell"/> class.
    /// </summary>
    /// <param name="RowItem">The row axis item.</param>
    /// <param name="ColumnItem">The column axis item.</param>
    /// <param name="Measure">The measure.</param>
    /// <param name="Value">The aggregated value.</param>
    public PivotGridValueCell(PivotGridAxisItem RowItem, PivotGridAxisItem ColumnItem, PivotGridMeasure Measure, object Value)
    {
        this.RowItem = RowItem;
        this.ColumnItem = ColumnItem;
        this.Measure = Measure;
        this.Value = Value;
    }

    // ● properties
    /// <summary>
    /// Gets the row axis item.
    /// </summary>
    public PivotGridAxisItem RowItem { get; }
    /// <summary>
    /// Gets the column axis item.
    /// </summary>
    public PivotGridAxisItem ColumnItem { get; }
    /// <summary>
    /// Gets the measure.
    /// </summary>
    public PivotGridMeasure Measure { get; }
    /// <summary>
    /// Gets the aggregated value.
    /// </summary>
    public object Value { get; }
    /// <summary>
    /// Gets the formatted display text.
    /// </summary>
    public string Text => Measure == null ? string.Empty : Measure.FormatValue(Value);
}

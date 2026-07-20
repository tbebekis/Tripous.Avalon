// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes the logical pivot grid element found during hit testing.
/// </summary>
public class PivotGridHitTestResult
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridHitTestResult"/> class.
    /// </summary>
    public PivotGridHitTestResult()
    {
    }

    // ● static public
    /// <summary>
    /// Gets an empty hit-test result.
    /// </summary>
    static public PivotGridHitTestResult Empty => new();

    // ● properties
    /// <summary>
    /// Gets or sets the local x-coordinate used for hit testing.
    /// </summary>
    public double X { get; set; }
    /// <summary>
    /// Gets or sets the local y-coordinate used for hit testing.
    /// </summary>
    public double Y { get; set; }
    /// <summary>
    /// Gets or sets the visual band.
    /// </summary>
    public PivotGridBand Band { get; set; }
    /// <summary>
    /// Gets or sets the logical hit-test kind.
    /// </summary>
    public PivotGridHitTestKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the row axis index, or -1 when not applicable.
    /// </summary>
    public int RowIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the column axis index, or -1 when not applicable.
    /// </summary>
    public int ColumnIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the measure index, or -1 when not applicable.
    /// </summary>
    public int MeasureIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the row axis item.
    /// </summary>
    public PivotGridAxisItem RowItem { get; set; }
    /// <summary>
    /// Gets or sets the row axis node.
    /// </summary>
    public PivotGridAxisNode RowNode { get; set; }
    /// <summary>
    /// Gets or sets the column axis item.
    /// </summary>
    public PivotGridAxisItem ColumnItem { get; set; }
    /// <summary>
    /// Gets or sets the measure.
    /// </summary>
    public PivotGridMeasure Measure { get; set; }
    /// <summary>
    /// Gets or sets the source field.
    /// </summary>
    public PivotGridSourceField SourceField { get; set; }
    /// <summary>
    /// Gets or sets the value cell.
    /// </summary>
    public PivotGridValueCell Cell { get; set; }
}

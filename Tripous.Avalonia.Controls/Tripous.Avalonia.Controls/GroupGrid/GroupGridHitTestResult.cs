// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes the logical group grid element found during hit testing.
/// </summary>
public class GroupGridHitTestResult
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridHitTestResult"/> class.
    /// </summary>
    public GroupGridHitTestResult()
    {
    }

    // ● static public
    /// <summary>
    /// Gets an empty hit-test result.
    /// </summary>
    static public GroupGridHitTestResult Empty => new();

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
    /// Gets or sets the visual band that was hit.
    /// </summary>
    public GroupGridBand Band { get; set; }
    /// <summary>
    /// Gets or sets the kind of element that was hit.
    /// </summary>
    public GroupGridHitTestKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the visible node index, or -1 when not applicable.
    /// </summary>
    public int VisibleNodeIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the visible row kind.
    /// </summary>
    public GroupGridRowKind RowKind { get; set; }
    /// <summary>
    /// Gets or sets the adapter row index, or -1 when not applicable.
    /// </summary>
    public int RowIndex { get; set; } = -1;
    /// <summary>
    /// Gets or sets the column that was hit.
    /// </summary>
    public GroupGridColumn Column { get; set; }
    /// <summary>
    /// Gets or sets the visible column index, or -1 when not applicable.
    /// </summary>
    public int ColumnIndex { get; set; } = -1;
    /// <summary>
    /// Gets a value indicating whether a column was hit.
    /// </summary>
    public bool HasColumn => Column != null;
    /// <summary>
    /// Gets a value indicating whether a logical cell was hit.
    /// </summary>
    public bool HasCell => RowIndex >= 0 && Column != null;
    /// <summary>
    /// Gets the logical cell that was hit.
    /// </summary>
    public GroupGridCell Cell => new(RowIndex, Column);
}

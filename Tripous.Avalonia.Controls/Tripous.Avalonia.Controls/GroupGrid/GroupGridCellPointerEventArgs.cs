// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for a <see cref="GroupGrid"/> cell pointer event.
/// </summary>
public class GroupGridCellPointerEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridCellPointerEventArgs"/> class.
    /// </summary>
    /// <param name="HitTest">The hit-test result.</param>
    /// <param name="Row">The adapter row object.</param>
    /// <param name="Position">The pointer position in grid coordinates.</param>
    /// <param name="PointerArgs">The original pointer pressed event args.</param>
    /// <param name="IsRightButton">True when the right button was pressed; otherwise, false.</param>
    public GroupGridCellPointerEventArgs(GroupGridHitTestResult HitTest, object Row, Point Position, PointerPressedEventArgs PointerArgs, bool IsRightButton)
    {
        this.HitTest = HitTest;
        this.Row = Row;
        this.Position = Position;
        this.PointerArgs = PointerArgs;
        this.IsRightButton = IsRightButton;
    }

    // ● properties
    /// <summary>
    /// Gets the hit-test result.
    /// </summary>
    public GroupGridHitTestResult HitTest { get; }
    /// <summary>
    /// Gets the logical cell.
    /// </summary>
    public GroupGridCell Cell => HitTest?.Cell ?? GroupGridCell.Empty;
    /// <summary>
    /// Gets the hit column.
    /// </summary>
    public GroupGridColumn Column => HitTest?.Column;
    /// <summary>
    /// Gets the adapter row index, or -1 when no data row was hit.
    /// </summary>
    public int RowIndex => HitTest?.RowIndex ?? -1;
    /// <summary>
    /// Gets the adapter row object, or null when no data row was hit.
    /// </summary>
    public object Row { get; }
    /// <summary>
    /// Gets the pointer position in grid coordinates.
    /// </summary>
    public Point Position { get; }
    /// <summary>
    /// Gets the original Avalonia pointer pressed event args.
    /// </summary>
    public PointerPressedEventArgs PointerArgs { get; }
    /// <summary>
    /// Gets a value indicating whether the right pointer button was pressed.
    /// </summary>
    public bool IsRightButton { get; }
    /// <summary>
    /// Gets the pointer update kind.
    /// </summary>
    public PointerUpdateKind PointerUpdateKind => PointerArgs?.GetCurrentPoint(null).Properties.PointerUpdateKind ?? default;
    /// <summary>
    /// Gets or sets a value indicating whether the event was handled.
    /// </summary>
    public bool Handled { get; set; }
}

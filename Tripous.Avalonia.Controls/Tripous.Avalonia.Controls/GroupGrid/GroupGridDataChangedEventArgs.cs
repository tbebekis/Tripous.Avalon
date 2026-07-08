// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for group grid adapter change notifications.
/// </summary>
public class GroupGridDataChangedEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDataChangedEventArgs"/> class.
    /// </summary>
    /// <param name="Kind">The kind of change.</param>
    /// <param name="RowIndex">The affected row index, or -1 when not applicable.</param>
    /// <param name="OldRowIndex">The old row index for move operations, or -1 when not applicable.</param>
    /// <param name="ColumnName">The affected column name, or an empty string when not applicable.</param>
    public GroupGridDataChangedEventArgs(GroupGridDataChangeKind Kind, int RowIndex = -1, int OldRowIndex = -1, string ColumnName = "")
    {
        this.Kind = Kind;
        this.RowIndex = RowIndex;
        this.OldRowIndex = OldRowIndex;
        this.ColumnName = ColumnName ?? string.Empty;
    }

    // ● static public
    /// <summary>
    /// Creates a reset notification.
    /// </summary>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs Reset() => new(GroupGridDataChangeKind.Reset);
    /// <summary>
    /// Creates a row-added notification.
    /// </summary>
    /// <param name="RowIndex">The added row index.</param>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs RowAdded(int RowIndex) => new(GroupGridDataChangeKind.RowAdded, RowIndex);
    /// <summary>
    /// Creates a row-removed notification.
    /// </summary>
    /// <param name="RowIndex">The removed row index.</param>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs RowRemoved(int RowIndex) => new(GroupGridDataChangeKind.RowRemoved, RowIndex);
    /// <summary>
    /// Creates a row-moved notification.
    /// </summary>
    /// <param name="OldRowIndex">The old row index.</param>
    /// <param name="RowIndex">The new row index.</param>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs RowMoved(int OldRowIndex, int RowIndex) => new(GroupGridDataChangeKind.RowMoved, RowIndex, OldRowIndex);
    /// <summary>
    /// Creates a row-changed notification.
    /// </summary>
    /// <param name="RowIndex">The changed row index.</param>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs RowChanged(int RowIndex) => new(GroupGridDataChangeKind.RowChanged, RowIndex);
    /// <summary>
    /// Creates a cell-changed notification.
    /// </summary>
    /// <param name="RowIndex">The changed row index.</param>
    /// <param name="ColumnName">The changed column name.</param>
    /// <returns>The created event arguments.</returns>
    static public GroupGridDataChangedEventArgs CellChanged(int RowIndex, string ColumnName) => new(GroupGridDataChangeKind.CellChanged, RowIndex, -1, ColumnName);

    // ● properties
    /// <summary>
    /// Gets the kind of change.
    /// </summary>
    public GroupGridDataChangeKind Kind { get; }
    /// <summary>
    /// Gets the affected row index, or -1 when not applicable.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the old row index for move operations, or -1 when not applicable.
    /// </summary>
    public int OldRowIndex { get; }
    /// <summary>
    /// Gets the affected column name, or an empty string when not applicable.
    /// </summary>
    public string ColumnName { get; }
}

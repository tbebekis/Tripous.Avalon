// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a logical row in the flat visible-node list.
/// </summary>
public readonly struct GroupGridRowInfo
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridRowInfo"/> struct.
    /// </summary>
    /// <param name="Kind">The row kind.</param>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <param name="RowIndex">The adapter row index, or -1 when not applicable.</param>
    /// <param name="Level">The group tree level.</param>
    /// <param name="Column">The group column, when applicable.</param>
    /// <param name="Key">The group key, when applicable.</param>
    /// <param name="Row">The source row, when applicable.</param>
    /// <param name="IsExpanded">True when a group row is expanded.</param>
    public GroupGridRowInfo(GroupGridRowKind Kind, int VisibleNodeIndex, int RowIndex, int Level, GroupGridColumn Column, object Key, object Row, bool IsExpanded)
    {
        this.Kind = Kind;
        this.VisibleNodeIndex = VisibleNodeIndex;
        this.RowIndex = RowIndex;
        this.Level = Level;
        this.Column = Column;
        this.Key = Key;
        this.Row = Row;
        this.IsExpanded = IsExpanded;
    }

    // ● static public
    /// <summary>
    /// Gets an empty row info value.
    /// </summary>
    static public GroupGridRowInfo Empty => new(GroupGridRowKind.None, -1, -1, -1, null, null, null, false);

    // ● properties
    /// <summary>
    /// Gets the row kind.
    /// </summary>
    public GroupGridRowKind Kind { get; }
    /// <summary>
    /// Gets the visible-node index.
    /// </summary>
    public int VisibleNodeIndex { get; }
    /// <summary>
    /// Gets the adapter row index, or -1 when not applicable.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the group tree level.
    /// </summary>
    public int Level { get; }
    /// <summary>
    /// Gets the group column, when applicable.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets the group key, when applicable.
    /// </summary>
    public object Key { get; }
    /// <summary>
    /// Gets the source row, when applicable.
    /// </summary>
    public object Row { get; }
    /// <summary>
    /// Gets a value indicating whether a group row is expanded.
    /// </summary>
    public bool IsExpanded { get; }
    /// <summary>
    /// Gets a value indicating whether this row info is empty.
    /// </summary>
    public bool IsEmpty => Kind == GroupGridRowKind.None;
    /// <summary>
    /// Gets a value indicating whether this row info describes a data row.
    /// </summary>
    public bool IsDataRow => Kind == GroupGridRowKind.DataRow;
    /// <summary>
    /// Gets a value indicating whether this row info describes a group header row.
    /// </summary>
    public bool IsGroup => Kind == GroupGridRowKind.Group;
    /// <summary>
    /// Gets a value indicating whether this row info describes a group summary row.
    /// </summary>
    public bool IsGroupSummary => Kind == GroupGridRowKind.GroupSummary;
    /// <summary>
    /// Gets a value indicating whether this row info describes the total summary row.
    /// </summary>
    public bool IsTotalSummary => Kind == GroupGridRowKind.TotalSummary;
}

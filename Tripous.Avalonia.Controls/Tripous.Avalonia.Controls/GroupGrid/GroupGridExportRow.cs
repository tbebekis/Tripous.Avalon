// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a row included in a group grid export snapshot.
/// </summary>
public class GroupGridExportRow
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridExportRow"/> class.
    /// </summary>
    /// <param name="RowInfo">The source visible row information.</param>
    /// <param name="GroupText">The group header text, when applicable.</param>
    /// <param name="Cells">The exported row cells.</param>
    public GroupGridExportRow(GroupGridRowInfo RowInfo, string GroupText, IEnumerable<GroupGridExportCell> Cells)
    {
        Kind = RowInfo.Kind;
        VisibleNodeIndex = RowInfo.VisibleNodeIndex;
        RowIndex = RowInfo.RowIndex;
        Level = RowInfo.Level;
        GroupColumn = RowInfo.Column;
        GroupKey = RowInfo.Key;
        SourceRow = RowInfo.Row;
        IsExpanded = RowInfo.IsExpanded;
        this.GroupText = GroupText ?? string.Empty;
        this.Cells = new ReadOnlyCollection<GroupGridExportCell>((Cells ?? Array.Empty<GroupGridExportCell>()).ToList());
    }

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
    /// Gets the group level.
    /// </summary>
    public int Level { get; }
    /// <summary>
    /// Gets the group column, when applicable.
    /// </summary>
    public GroupGridColumn GroupColumn { get; }
    /// <summary>
    /// Gets the group key, when applicable.
    /// </summary>
    public object GroupKey { get; }
    /// <summary>
    /// Gets the source row, when applicable.
    /// </summary>
    public object SourceRow { get; }
    /// <summary>
    /// Gets a value indicating whether the group row is expanded.
    /// </summary>
    public bool IsExpanded { get; }
    /// <summary>
    /// Gets the group header text, when applicable.
    /// </summary>
    public string GroupText { get; }
    /// <summary>
    /// Gets the exported row cells.
    /// </summary>
    public IReadOnlyList<GroupGridExportCell> Cells { get; }
    /// <summary>
    /// Gets a value indicating whether this row is a data row.
    /// </summary>
    public bool IsDataRow => Kind == GroupGridRowKind.DataRow;
    /// <summary>
    /// Gets a value indicating whether this row is a group row.
    /// </summary>
    public bool IsGroup => Kind == GroupGridRowKind.Group;
    /// <summary>
    /// Gets a value indicating whether this row is a group summary row.
    /// </summary>
    public bool IsGroupSummary => Kind == GroupGridRowKind.GroupSummary;
}

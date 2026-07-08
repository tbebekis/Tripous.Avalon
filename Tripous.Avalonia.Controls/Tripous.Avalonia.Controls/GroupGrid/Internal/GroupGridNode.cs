// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents an internal projected group grid row node.
/// </summary>
internal class GroupGridNode
{
    // ● private fields
    readonly List<GroupGridNode> fChildren = new();
    readonly Dictionary<GroupGridColumn, GroupGridSummaryValue> fSummaries = new();

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridNode"/> class.
    /// </summary>
    /// <param name="Kind">The node kind.</param>
    /// <param name="Parent">The parent node.</param>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Row">The source row.</param>
    /// <param name="Column">The group column.</param>
    /// <param name="Key">The group key.</param>
    public GroupGridNode(GroupGridNodeKind Kind, GroupGridNode Parent, int RowIndex, object Row, GroupGridColumn Column = null, object Key = null)
    {
        this.Kind = Kind;
        this.Parent = Parent;
        this.RowIndex = RowIndex;
        this.Row = Row;
        this.Column = Column;
        this.Key = Key;
        this.Level = Parent == null ? -1 : Parent.Level + 1;
    }

    // ● public methods
    /// <summary>
    /// Adds a child node.
    /// </summary>
    /// <param name="Node">The child node.</param>
    public void Add(GroupGridNode Node)
    {
        fChildren.Add(Node);
    }
    /// <summary>
    /// Removes all child nodes.
    /// </summary>
    public void Clear()
    {
        fChildren.Clear();
        fSummaries.Clear();
    }
    /// <summary>
    /// Removes all summary values from this node and its descendants.
    /// </summary>
    public void ClearSummaries()
    {
        fSummaries.Clear();
        foreach (GroupGridNode Node in fChildren)
            Node.ClearSummaries();
    }
    /// <summary>
    /// Sets a summary value.
    /// </summary>
    /// <param name="Summary">The summary value.</param>
    public void SetSummary(GroupGridSummaryValue Summary)
    {
        if (Summary.Column != null)
            fSummaries[Summary.Column] = Summary;
    }
    /// <summary>
    /// Returns a summary value for a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The summary value, or an empty value when not found.</returns>
    public GroupGridSummaryValue GetSummary(GroupGridColumn Column)
    {
        return Column != null && fSummaries.TryGetValue(Column, out GroupGridSummaryValue Result)
            ? Result
            : GroupGridSummaryValue.Empty;
    }
    /// <summary>
    /// Adds visible descendant nodes to a flat list.
    /// </summary>
    /// <param name="List">The target visible node list.</param>
    public void AddVisibleNodesTo(List<GroupGridNode> List)
    {
        if (Kind != GroupGridNodeKind.Root)
            List.Add(this);

        if (!IsExpanded)
            return;

        foreach (GroupGridNode Node in fChildren)
            Node.AddVisibleNodesTo(List);
    }

    // ● properties
    /// <summary>
    /// Gets the node kind.
    /// </summary>
    public GroupGridNodeKind Kind { get; }
    /// <summary>
    /// Gets the parent node.
    /// </summary>
    public GroupGridNode Parent { get; }
    /// <summary>
    /// Gets the adapter row index, or -1 for non-data nodes.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the source row.
    /// </summary>
    public object Row { get; }
    /// <summary>
    /// Gets the group column.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets the group key.
    /// </summary>
    public object Key { get; }
    /// <summary>
    /// Gets the node tree level.
    /// </summary>
    public int Level { get; }
    /// <summary>
    /// Gets or sets a value indicating whether child nodes are visible.
    /// </summary>
    public bool IsExpanded { get; set; } = true;
    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public IReadOnlyList<GroupGridNode> Children => fChildren;
    /// <summary>
    /// Gets the summary values.
    /// </summary>
    public IReadOnlyDictionary<GroupGridColumn, GroupGridSummaryValue> Summaries => fSummaries;
    /// <summary>
    /// Gets a value indicating whether this node is the root.
    /// </summary>
    public bool IsRoot => Kind == GroupGridNodeKind.Root;
    /// <summary>
    /// Gets a value indicating whether this node represents an adapter data row.
    /// </summary>
    public bool IsDataRow => Kind == GroupGridNodeKind.DataRow;
    /// <summary>
    /// Gets a value indicating whether this node represents a group header row.
    /// </summary>
    public bool IsGroup => Kind == GroupGridNodeKind.Group;
    /// <summary>
    /// Gets a value indicating whether this node represents a group summary row.
    /// </summary>
    public bool IsGroupSummary => Kind == GroupGridNodeKind.GroupSummary;
}

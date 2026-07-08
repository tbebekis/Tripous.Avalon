// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Builds the internal root-node and flat visible-node projection for a group grid.
/// </summary>
internal class GroupGridNodeProjection
{
    // ● private fields
    readonly List<GroupGridNode> fVisibleNodes = new();

    // ● private methods
    GroupGridNode FindOrAddGroupNode(GroupGridNode Parent, GroupGridColumn Column, object Key)
    {
        foreach (GroupGridNode Child in Parent.Children)
            if (Child.IsGroup && ReferenceEquals(Child.Column, Column) && Equals(Child.Key, Key))
                return Child;

        GroupGridNode Result = new(GroupGridNodeKind.Group, Parent, -1, null, Column, Key);
        Parent.Add(Result);
        return Result;
    }
    List<int> CreateRowIndexes(IGroupGridDataAdapter Adapter, IReadOnlyList<int> RowIndexes)
    {
        return RowIndexes == null
            ? Enumerable.Range(0, Adapter.RowCount).ToList()
            : RowIndexes.ToList();
    }
    void BuildUngrouped(IGroupGridDataAdapter Adapter, IReadOnlyList<int> RowIndexes)
    {
        foreach (int Index in CreateRowIndexes(Adapter, RowIndexes))
        {
            GroupGridNode Node = new(GroupGridNodeKind.DataRow, Root, Index, Adapter.GetRow(Index));
            Root.Add(Node);
        }
    }
    void BuildGrouped(IGroupGridDataAdapter Adapter, IReadOnlyList<GroupGridColumn> GroupColumns, IReadOnlyList<int> RowIndexes)
    {
        foreach (int RowIndex in CreateRowIndexes(Adapter, RowIndexes))
        {
            GroupGridNode Parent = Root;

            foreach (GroupGridColumn Column in GroupColumns)
            {
                object Key = Adapter.GetValue(RowIndex, Column);
                Parent = FindOrAddGroupNode(Parent, Column, Key);
            }

            GroupGridNode RowNode = new(GroupGridNodeKind.DataRow, Parent, RowIndex, Adapter.GetRow(RowIndex));
            Parent.Add(RowNode);
        }

        AddGroupSummaryNodes(Root);
    }
    void AddGroupSummaryNodes(GroupGridNode Node)
    {
        foreach (GroupGridNode Child in Node.Children.ToArray())
            if (Child.IsGroup)
            {
                AddGroupSummaryNodes(Child);
                Child.Add(new GroupGridNode(GroupGridNodeKind.GroupSummary, Child, -1, null, Child.Column, Child.Key));
            }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridNodeProjection"/> class.
    /// </summary>
    public GroupGridNodeProjection()
    {
        Root = new GroupGridNode(GroupGridNodeKind.Root, null, -1, null);
    }

    // ● public methods
    /// <summary>
    /// Rebuilds the projection from a data adapter.
    /// </summary>
    /// <param name="Adapter">The data adapter.</param>
    public void Rebuild(IGroupGridDataAdapter Adapter, IReadOnlyList<GroupGridColumn> GroupColumns, IReadOnlyList<int> RowIndexes = null)
    {
        Root.Clear();
        fVisibleNodes.Clear();

        if (Adapter == null)
            return;

        if (GroupColumns == null || GroupColumns.Count == 0)
            BuildUngrouped(Adapter, RowIndexes);
        else
            BuildGrouped(Adapter, GroupColumns, RowIndexes);

        Root.AddVisibleNodesTo(fVisibleNodes);
    }
    /// <summary>
    /// Rebuilds the flat visible-node list from the existing root node.
    /// </summary>
    public void UpdateVisibleNodes()
    {
        fVisibleNodes.Clear();
        Root.AddVisibleNodesTo(fVisibleNodes);
    }
    /// <summary>
    /// Returns the visible-node index of a specified adapter row index.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <returns>The visible-node index, or -1 if not found.</returns>
    public int IndexOfRow(int RowIndex)
    {
        for (int Index = 0; Index < fVisibleNodes.Count; Index++)
            if (fVisibleNodes[Index].IsDataRow && fVisibleNodes[Index].RowIndex == RowIndex)
                return Index;

        return -1;
    }
    /// <summary>
    /// Returns the data row node at a specified visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>The data row node, or null when the visible node is not a data row.</returns>
    public GroupGridNode DataRowNodeByVisibleIndex(int VisibleNodeIndex)
    {
        if (VisibleNodeIndex < 0 || VisibleNodeIndex >= fVisibleNodes.Count)
            return null;

        GroupGridNode Node = fVisibleNodes[VisibleNodeIndex];
        return Node.IsDataRow ? Node : null;
    }
    /// <summary>
    /// Returns the node at a specified visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>The node, or null if the index is out of range.</returns>
    public GroupGridNode NodeByVisibleIndex(int VisibleNodeIndex)
    {
        if (VisibleNodeIndex < 0 || VisibleNodeIndex >= fVisibleNodes.Count)
            return null;

        return fVisibleNodes[VisibleNodeIndex];
    }
    /// <summary>
    /// Returns the next data row node from a visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The starting visible-node index.</param>
    /// <param name="Delta">The data-row delta.</param>
    /// <returns>The matching data row node, or null if not found.</returns>
    public GroupGridNode FindDataRowNode(int VisibleNodeIndex, int Delta)
    {
        if (Delta == 0)
            return DataRowNodeByVisibleIndex(VisibleNodeIndex);

        int Step = Delta < 0 ? -1 : 1;
        int Remaining = Math.Abs(Delta);
        int Index = VisibleNodeIndex;

        while (Remaining > 0)
        {
            Index += Step;
            if (Index < 0 || Index >= fVisibleNodes.Count)
                return null;

            if (fVisibleNodes[Index].IsDataRow)
                Remaining--;
        }

        return DataRowNodeByVisibleIndex(Index);
    }

    // ● properties
    /// <summary>
    /// Gets the root node.
    /// </summary>
    public GroupGridNode Root { get; }
    /// <summary>
    /// Gets the flat visible-node list.
    /// </summary>
    public IReadOnlyList<GroupGridNode> VisibleNodes => fVisibleNodes;
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a projected pivot grid row-axis node.
/// </summary>
public class PivotGridAxisNode
{
    // ● private fields
    readonly List<PivotGridAxisNode> fChildren = new();

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridAxisNode"/> class.
    /// </summary>
    /// <param name="Parent">The parent node.</param>
    /// <param name="Item">The row axis item.</param>
    /// <param name="Level">The row axis level.</param>
    public PivotGridAxisNode(PivotGridAxisNode Parent, PivotGridAxisItem Item, int Level)
    {
        this.Parent = Parent;
        this.Item = Item;
        this.Level = Level;
    }

    // ● public methods
    /// <summary>
    /// Adds a child node.
    /// </summary>
    /// <param name="Node">The child node.</param>
    public void Add(PivotGridAxisNode Node)
    {
        fChildren.Add(Node);
    }
    /// <summary>
    /// Adds this node and its visible descendants to a list.
    /// </summary>
    /// <param name="List">The target list.</param>
    public void AddVisibleNodesTo(List<PivotGridAxisNode> List)
    {
        if (!IsRoot)
            List.Add(this);
        if (!IsExpanded)
            return;

        foreach (PivotGridAxisNode Child in fChildren)
            Child.AddVisibleNodesTo(List);
    }

    // ● properties
    /// <summary>
    /// Gets the parent node.
    /// </summary>
    public PivotGridAxisNode Parent { get; }
    /// <summary>
    /// Gets the row axis item.
    /// </summary>
    public PivotGridAxisItem Item { get; }
    /// <summary>
    /// Gets the row axis level.
    /// </summary>
    public int Level { get; }
    /// <summary>
    /// Gets or sets a value indicating whether child nodes are visible.
    /// </summary>
    public bool IsExpanded { get; set; } = true;
    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public IReadOnlyList<PivotGridAxisNode> Children => fChildren;
    /// <summary>
    /// Gets a value indicating whether this is the root node.
    /// </summary>
    public bool IsRoot => Parent == null;
    /// <summary>
    /// Gets a value indicating whether this node has child nodes.
    /// </summary>
    public bool HasChildren => fChildren.Count > 0;
}

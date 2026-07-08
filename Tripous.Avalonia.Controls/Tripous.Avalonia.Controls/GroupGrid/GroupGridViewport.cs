// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes the rendered window into the logical visible-node list.
/// </summary>
public readonly struct GroupGridViewport: IEquatable<GroupGridViewport>
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridViewport"/> struct.
    /// </summary>
    /// <param name="FirstVisibleNodeIndex">The first visible-node index included in the viewport.</param>
    /// <param name="LastVisibleNodeIndex">The last visible-node index included in the viewport.</param>
    public GroupGridViewport(int FirstVisibleNodeIndex, int LastVisibleNodeIndex)
    {
        this.FirstVisibleNodeIndex = FirstVisibleNodeIndex;
        this.LastVisibleNodeIndex = LastVisibleNodeIndex;
    }

    // ● static public
    /// <summary>
    /// Gets an empty viewport.
    /// </summary>
    static public GroupGridViewport Empty => new(-1, -1);

    // ● public methods
    /// <summary>
    /// Returns true when this instance is equal to another viewport.
    /// </summary>
    /// <param name="Other">The other viewport.</param>
    /// <returns>True if the viewports are equal; otherwise, false.</returns>
    public bool Equals(GroupGridViewport Other)
    {
        return FirstVisibleNodeIndex == Other.FirstVisibleNodeIndex
               && LastVisibleNodeIndex == Other.LastVisibleNodeIndex;
    }
    /// <inheritdoc />
    public override bool Equals(object Obj) => Obj is GroupGridViewport Other && Equals(Other);
    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(FirstVisibleNodeIndex, LastVisibleNodeIndex);
    /// <summary>
    /// Returns true when two viewports are equal.
    /// </summary>
    /// <param name="Left">The left viewport.</param>
    /// <param name="Right">The right viewport.</param>
    /// <returns>True if the viewports are equal; otherwise, false.</returns>
    static public bool operator ==(GroupGridViewport Left, GroupGridViewport Right) => Left.Equals(Right);
    /// <summary>
    /// Returns true when two viewports are not equal.
    /// </summary>
    /// <param name="Left">The left viewport.</param>
    /// <param name="Right">The right viewport.</param>
    /// <returns>True if the viewports are not equal; otherwise, false.</returns>
    static public bool operator !=(GroupGridViewport Left, GroupGridViewport Right) => !Left.Equals(Right);
    /// <summary>
    /// Returns true when a visible-node index is inside this viewport.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>True if the index is inside this viewport; otherwise, false.</returns>
    public bool Contains(int VisibleNodeIndex)
    {
        return !IsEmpty
               && VisibleNodeIndex >= FirstVisibleNodeIndex
               && VisibleNodeIndex <= LastVisibleNodeIndex;
    }

    // ● properties
    /// <summary>
    /// Gets the first visible-node index included in the viewport.
    /// </summary>
    public int FirstVisibleNodeIndex { get; }
    /// <summary>
    /// Gets the last visible-node index included in the viewport.
    /// </summary>
    public int LastVisibleNodeIndex { get; }
    /// <summary>
    /// Gets the number of visible nodes included in the viewport.
    /// </summary>
    public int Count => IsEmpty ? 0 : LastVisibleNodeIndex - FirstVisibleNodeIndex + 1;
    /// <summary>
    /// Gets a value indicating whether this viewport is empty.
    /// </summary>
    public bool IsEmpty => FirstVisibleNodeIndex < 0 || LastVisibleNodeIndex < FirstVisibleNodeIndex;
}

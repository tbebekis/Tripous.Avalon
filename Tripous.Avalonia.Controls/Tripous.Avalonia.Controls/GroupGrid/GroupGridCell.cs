// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Identifies a logical cell by adapter row index and grid column.
/// </summary>
public readonly struct GroupGridCell: IEquatable<GroupGridCell>
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridCell"/> struct.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    public GroupGridCell(int RowIndex, GroupGridColumn Column)
    {
        this.RowIndex = RowIndex;
        this.Column = Column;
    }

    // ● static public
    /// <summary>
    /// Gets an empty cell.
    /// </summary>
    static public GroupGridCell Empty => new(-1, null);

    // ● public methods
    /// <summary>
    /// Returns true when this instance is equal to another cell.
    /// </summary>
    /// <param name="Other">The other cell.</param>
    /// <returns>True if the cells are equal; otherwise, false.</returns>
    public bool Equals(GroupGridCell Other) => RowIndex == Other.RowIndex && ReferenceEquals(Column, Other.Column);
    /// <inheritdoc />
    public override bool Equals(object Obj) => Obj is GroupGridCell Other && Equals(Other);
    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(RowIndex, Column);
    /// <summary>
    /// Returns true when two cells are equal.
    /// </summary>
    /// <param name="Left">The left cell.</param>
    /// <param name="Right">The right cell.</param>
    /// <returns>True if the cells are equal; otherwise, false.</returns>
    static public bool operator ==(GroupGridCell Left, GroupGridCell Right) => Left.Equals(Right);
    /// <summary>
    /// Returns true when two cells are not equal.
    /// </summary>
    /// <param name="Left">The left cell.</param>
    /// <param name="Right">The right cell.</param>
    /// <returns>True if the cells are not equal; otherwise, false.</returns>
    static public bool operator !=(GroupGridCell Left, GroupGridCell Right) => !Left.Equals(Right);

    // ● properties
    /// <summary>
    /// Gets the adapter row index.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the grid column.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets a value indicating whether this cell is empty.
    /// </summary>
    public bool IsEmpty => RowIndex < 0 || Column == null;
}

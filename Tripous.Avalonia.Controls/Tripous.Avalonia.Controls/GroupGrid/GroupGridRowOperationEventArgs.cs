// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for a group grid row operation.
/// </summary>
public class GroupGridRowOperationEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridRowOperationEventArgs"/> class.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Row">The source row.</param>
    public GroupGridRowOperationEventArgs(int RowIndex, object Row)
    {
        this.RowIndex = RowIndex;
        this.Row = Row;
    }

    // ● properties
    /// <summary>
    /// Gets the adapter row index.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the source row.
    /// </summary>
    public object Row { get; }
    /// <summary>
    /// Gets or sets a value indicating whether the operation is canceled.
    /// </summary>
    public bool Cancel { get; set; }
}

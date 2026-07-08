// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for group grid cell editing events.
/// </summary>
public class GroupGridCellEditEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridCellEditEventArgs"/> class.
    /// </summary>
    /// <param name="Cell">The edited cell.</param>
    /// <param name="Value">The edit value.</param>
    public GroupGridCellEditEventArgs(GroupGridCell Cell, object Value)
    {
        this.Cell = Cell;
        this.Value = Value;
    }

    // ● properties
    /// <summary>
    /// Gets the edited cell.
    /// </summary>
    public GroupGridCell Cell { get; }
    /// <summary>
    /// Gets or sets the edit value.
    /// </summary>
    public object Value { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the operation should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets or sets the validation error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

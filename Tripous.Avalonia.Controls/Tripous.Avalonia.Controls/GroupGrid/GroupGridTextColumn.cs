// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a text column in a group grid.
/// </summary>
public class GroupGridTextColumn: GroupGridColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridTextColumn"/> class.
    /// </summary>
    public GroupGridTextColumn()
    {
        ValueType = typeof(string);
    }
}

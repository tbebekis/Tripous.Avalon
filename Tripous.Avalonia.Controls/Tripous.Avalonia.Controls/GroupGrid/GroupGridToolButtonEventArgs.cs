// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for a group grid toolbar button click.
/// </summary>
public class GroupGridToolButtonEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridToolButtonEventArgs"/> class.
    /// </summary>
    /// <param name="Button">The clicked toolbar button.</param>
    public GroupGridToolButtonEventArgs(GroupGridToolButton Button)
    {
        this.Button = Button;
    }

    // ● properties
    /// <summary>
    /// Gets the clicked toolbar button.
    /// </summary>
    public GroupGridToolButton Button { get; }
}

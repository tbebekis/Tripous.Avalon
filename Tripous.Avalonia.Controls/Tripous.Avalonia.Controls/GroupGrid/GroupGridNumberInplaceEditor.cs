// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a right-aligned text editor for numeric group grid cells.
/// </summary>
public class GroupGridNumberInplaceEditor: GroupGridTextInplaceEditor
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridNumberInplaceEditor"/> class.
    /// </summary>
    public GroupGridNumberInplaceEditor()
    {
        TextAlignment = TextAlignment.Right;
    }
}

// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for creating a custom group grid in-place editor.
/// </summary>
public class GroupGridCreateInplaceEditorEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridCreateInplaceEditorEventArgs"/> class.
    /// </summary>
    /// <param name="Column">The column being edited.</param>
    public GroupGridCreateInplaceEditorEventArgs(GroupGridColumn Column)
    {
        this.Column = Column;
    }

    // ● properties
    /// <summary>
    /// Gets the column being edited.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string FieldName => Column?.Name ?? string.Empty;
    /// <summary>
    /// Gets the value type.
    /// </summary>
    public Type ValueType => Column?.ValueType ?? typeof(object);
    /// <summary>
    /// Gets or sets the custom editor.
    /// </summary>
    public GroupGridInplaceEditorBase Editor { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the event supplied an editor.
    /// </summary>
    public bool Handled { get; set; }
}

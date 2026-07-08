// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data for normalizing date text before committing a group grid cell edit.
/// </summary>
public class GroupGridDateNormalizeEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDateNormalizeEventArgs"/> class.
    /// </summary>
    /// <param name="Column">The date column.</param>
    /// <param name="Text">The editor text.</param>
    /// <param name="CultureInfo">The culture used for parsing.</param>
    public GroupGridDateNormalizeEventArgs(GroupGridColumn Column, string Text, CultureInfo CultureInfo)
    {
        this.Column = Column;
        this.Text = Text ?? string.Empty;
        this.CultureInfo = CultureInfo ?? CultureInfo.CurrentCulture;
    }

    // ● properties
    /// <summary>
    /// Gets the date column.
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
    /// Gets the editor text.
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// Gets the culture used for parsing.
    /// </summary>
    public CultureInfo CultureInfo { get; }
    /// <summary>
    /// Gets or sets the normalized value.
    /// </summary>
    public DateTime Value { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the event supplied a normalized value.
    /// </summary>
    public bool Handled { get; set; }
}

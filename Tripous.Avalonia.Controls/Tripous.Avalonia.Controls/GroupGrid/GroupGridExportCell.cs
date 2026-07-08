// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a cell included in a group grid export snapshot.
/// </summary>
public class GroupGridExportCell
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridExportCell"/> class.
    /// </summary>
    /// <param name="Column">The export column.</param>
    /// <param name="Value">The raw cell value.</param>
    /// <param name="Text">The formatted display text.</param>
    public GroupGridExportCell(GroupGridExportColumn Column, object Value, string Text)
    {
        this.Column = Column;
        this.Value = Value;
        this.Text = Text ?? string.Empty;
    }

    // ● properties
    /// <summary>
    /// Gets the export column.
    /// </summary>
    public GroupGridExportColumn Column { get; }
    /// <summary>
    /// Gets the raw cell value.
    /// </summary>
    public object Value { get; }
    /// <summary>
    /// Gets the formatted display text.
    /// </summary>
    public string Text { get; }
}

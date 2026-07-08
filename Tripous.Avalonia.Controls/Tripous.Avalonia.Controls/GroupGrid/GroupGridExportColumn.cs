// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a column included in a group grid export snapshot.
/// </summary>
public class GroupGridExportColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridExportColumn"/> class.
    /// </summary>
    /// <param name="Column">The source grid column.</param>
    public GroupGridExportColumn(GroupGridColumn Column)
    {
        this.Column = Column;
        Name = Column == null ? string.Empty : Column.Name;
        Header = Column == null || string.IsNullOrWhiteSpace(Column.Header) ? Name : Column.Header;
        ValueType = Column == null ? typeof(object) : Column.ValueType;
    }

    // ● properties
    /// <summary>
    /// Gets the source grid column.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets the column name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the column header text.
    /// </summary>
    public string Header { get; }
    /// <summary>
    /// Gets the column value type.
    /// </summary>
    public Type ValueType { get; }
}

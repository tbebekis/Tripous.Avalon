/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Information about a detail grid UI.
/// </summary>
public class UiDetailTableInfo 
{
    // ● public
    /// <summary>
    /// The detail grid.
    /// </summary>
    public GroupGrid Grid { get; set; }
    /// <summary>
    /// The parent table definition.
    /// </summary>
    public TableDef ParentTableDef { get; set; }
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}

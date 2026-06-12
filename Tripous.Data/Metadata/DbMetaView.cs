/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Represents metadata for a database relational view object.
/// </summary>
public class DbMetaView : DbMetaObject
{
    // ● public methods
    /// <summary>
    /// Generates a comma-separated string list of all column names in the view, separated by newlines.
    /// </summary>
    public string GetFieldNameList() => string.Join($", {Environment.NewLine}", Columns.Select(x => x.Name));

    // ● properties
    /// <summary>
    /// Gets the collection of schema column metadata objects defined for the view projection.
    /// </summary>
    public List<DbMetaColumn> Columns { get; } = new();
}
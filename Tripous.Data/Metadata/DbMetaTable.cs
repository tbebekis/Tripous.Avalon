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
/// Represents metadata for a database table, including its structural components and schema relationships.
/// </summary>
public class DbMetaTable : DbMetaObject
{
    // ● public methods
    /// <summary>
    /// Generates a comma-separated string list of all column names in the table, separated by newlines.
    /// </summary>
    public string GetFieldNameList() => string.Join($", {Environment.NewLine}", Columns.Select(x => x.Name));
    /// <summary>
    /// Generates a basic SQL DDL script statement for creating the table schema.
    /// </summary>
    public string GetCreateTable()
    {
        StringBuilder SB = new();

        SB.AppendLine($"create table {Name} ( ");
        string FieldList = string.Join($", {Environment.NewLine}", Columns.Select(x => "  " + x.DisplayText));
        SB.AppendLine(FieldList);
        SB.AppendLine(")");

        return SB.ToString();
    }

    // ● properties
    /// <summary>
    /// Gets the collection of schema column metadata objects defined for the table.
    /// </summary>
    public List<DbMetaColumn> Columns { get; } = new();
    /// <summary>
    /// Gets the collection of foreign key constraint metadata objects defined for the table.
    /// </summary>
    public List<DbMetaForeignKey> ForeignKeys { get; } = new();
    /// <summary>
    /// Gets the collection of general schema constraint metadata objects defined for the table.
    /// </summary>
    public List<DbMetaConstraint> Constraints { get; } = new();
    /// <summary>
    /// Gets the collection of index metadata objects defined for the table optimization.
    /// </summary>
    public List<DbMetaIndex> Indexes { get; } = new();
    /// <summary>
    /// Gets the collection of schema trigger metadata objects associated with the table events.
    /// </summary>
    public List<DbMetaTrigger> Triggers { get; } = new();
}
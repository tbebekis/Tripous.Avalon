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
/// Represents metadata for a database index.
/// </summary>
public class DbMetaIndex : DbMetaObject
{
    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether the index enforces uniqueness.
    /// </summary>
    public bool IsUnique { get; set; }
    /// <summary>
    /// Gets or sets the storage structure type of the index (e.g., BTREE, HASH).
    /// </summary>
    public string IndexType { get; set; }
    /// <summary>
    /// Gets or sets the list of column names included in this index.
    /// </summary>
    public string Columns { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the index metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name + $" ({Columns})";
            if (!string.IsNullOrWhiteSpace(IndexType))
                Result += $" ({IndexType})";
            else if (IsUnique)
                Result += $" Unique";

            return Result;
        }
    }
}
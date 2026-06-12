/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents metadata for a database foreign key constraint.
/// </summary>
public class DbMetaForeignKey : DbMetaConstraint
{
    // ● properties
    /// <summary>
    /// Gets or sets the name of the referenced parent table.
    /// </summary>
    public string ForeignTable { get; set; }
    /// <summary>
    /// Gets or sets the list of referenced column names in the parent table.
    /// </summary>
    public string ForeignFields { get; set; }
    /// <summary>
    /// Gets or sets the referential trigger action rule for updates (e.g., CASCADE, SET NULL).
    /// </summary>
    public string UpdateRule { get; set; }
    /// <summary>
    /// Gets or sets the referential trigger action rule for deletes (e.g., CASCADE, RESTRICT).
    /// </summary>
    public string DeleteRule { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the foreign key metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name;

            if (!string.IsNullOrWhiteSpace(Columns))
                Result += $" ({Columns})";

            if (!string.IsNullOrWhiteSpace(ForeignTable))
            {
                Result += $" references ({ForeignTable})";
                if (!string.IsNullOrWhiteSpace(ForeignFields))
                    Result += $" ({ForeignFields})";
            }

            return Result;
        }
    }
}
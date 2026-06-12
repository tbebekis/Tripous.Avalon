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
/// Represents metadata for a database stored procedure or function.
/// </summary>
public class DbMetaProcedure : DbMetaObject
{
    // ● properties
    /// <summary>
    /// Gets or sets the execution module type (e.g., procedure vs function).
    /// </summary>
    public string ProcedureType { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the procedure metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name;

            if (!string.IsNullOrWhiteSpace(ProcedureType))
                Result += $" ({ProcedureType})";

            return Result;
        }
    }
}
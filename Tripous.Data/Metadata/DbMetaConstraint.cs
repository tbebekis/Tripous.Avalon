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
/// Specifies the type of a database constraint.
/// </summary>
public enum ConstraintType
{
    /// <summary>
    /// The constraint type is unknown or undefined.
    /// </summary>
    Unknown,
    /// <summary>
    /// Represents a primary key constraint.
    /// </summary>
    PrimaryKey,
    /// <summary>
    /// Represents a foreign key constraint.
    /// </summary>
    ForeignKey,
    /// <summary>
    /// Represents a unique constraint.
    /// </summary>
    Unique,
    /// <summary>
    /// Represents a check constraint.
    /// </summary>
    Check,
    /// <summary>
    /// Represents a non-nullable column constraint.
    /// </summary>
    NotNull,
}

/// <summary>
/// Represents metadata for a database constraint.
/// </summary>
public class DbMetaConstraint : DbMetaObject
{
    // ● private fields
    string fConstraintTypeText;
    ConstraintType fConstraintType;
    string fColumns;

    // ● properties
    /// <summary>
    /// Gets or sets the database-specific text representation of the constraint type.
    /// </summary>
    public string ConstraintTypeText { get => fConstraintTypeText; set => fConstraintTypeText = value; }
    /// <summary>
    /// Gets or sets the strongly-typed schema constraint category.
    /// </summary>
    public ConstraintType ConstraintType { get => fConstraintType; set => fConstraintType = value; }
    /// <summary>
    /// Gets or sets the list of column names involved in this constraint.
    /// </summary>
    public string Columns { get => fColumns; set => fColumns = value; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the constraint metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name;

            if (!string.IsNullOrWhiteSpace(Columns))
                Result += $" ({Columns})";

            if (!string.IsNullOrWhiteSpace(ConstraintTypeText))
                Result += $" {ConstraintTypeText}";

            return Result;
        }
    }
}
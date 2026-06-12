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
/// Represents metadata for a database table column.
/// </summary>
public class DbMetaColumn : DbMetaObject
{
    // ● private fields
    string fDataType = string.Empty;

    // ● properties
    /// <summary>
    /// Gets or sets the data type name (e.g., varchar, integer, timestamp).
    /// </summary>
    public string DataType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the database-specific data sub-type (e.g., VARCHAR to CHARACTER VARYING in Firebird).
    /// </summary>
    public string DataSubType { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the column accepts null values.
    /// </summary>
    public bool IsNullable { get; set; }
    /// <summary>
    /// Gets or sets the maximum character length for string columns.
    /// </summary>
    public int SizeInChars { get; set; }
    /// <summary>
    /// Gets or sets the maximum byte length for string or binary columns.
    /// </summary>
    public int SizeInBytes { get; set; }
    /// <summary>
    /// Gets or sets the numeric precision.
    /// </summary>
    public int Precision { get; set; }
    /// <summary>
    /// Gets or sets the numeric scale.
    /// </summary>
    public int Scale { get; set; }
    /// <summary>
    /// Gets or sets the default value expression.
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// Gets or sets the computed column expression.
    /// </summary>
    public string Expression { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the column is an identity or auto-increment column.
    /// </summary>
    public bool IsIdentity { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the column value is calculated or computed.
    /// </summary>
    public bool IsComputed { get; set; }
    /// <summary>
    /// Gets or sets the 1-based index position of the column within the table schema.
    /// </summary>
    public int OrdinalPosition { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the column metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name + " " + DataType;

            if (!string.IsNullOrWhiteSpace(DataSubType))
                Result += " - " + DataSubType;

            if (IsIdentity)
                Result += " IDENTITY";

            if (SizeInChars > 0)
                Result += $"({SizeInChars})";
            if ((Precision > 0) && (Scale > 0))
                Result += $"({Precision},{Scale})";

            Result += IsNullable ? " null" : " not null";

            return Result;
        }
    }
}
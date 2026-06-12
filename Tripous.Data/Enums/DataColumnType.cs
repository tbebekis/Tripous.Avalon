/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Indicates the data type of a column.
/// </summary>
[Flags]
public enum DataColumnType
{
    /// <summary>
    /// No type specified.
    /// </summary>
    None        = 0x0000,
    /// <summary>
    /// A text (string) column.
    /// </summary>
    Text        = 0x0001,
    /// <summary>
    /// A boolean column.
    /// </summary>
    Boolean     = 0x0002,
    /// <summary>
    /// A date-only column.
    /// </summary>
    Date        = 0x0004,
    /// <summary>
    /// A date and time column.
    /// </summary>
    DateTime    = 0x0008,
    /// <summary>
    /// An integer column.
    /// </summary>
    Integer     = 0x0010,
    /// <summary>
    /// A decimal (floating-point) column.
    /// </summary>
    Decimal     = 0x0020,
    /// <summary>
    /// A currency column.
    /// </summary>
    Currency    = 0x0040,
    /// <summary>
    /// An image column.
    /// </summary>
    Image       = 0x0080,
    /// <summary>
    /// A memo (long text) column.
    /// </summary>
    Memo        = 0x0100,
    /// <summary>
    /// A lookup column, referring to a value in another table.
    /// </summary>
    Lookup      = 0x0200,
}
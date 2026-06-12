/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;
 

/// <summary>
/// Indicates how a WHERE filter is incorporated into an SQL statement.
/// </summary>
public enum SqlWhereFilterMode
{
    /// <summary>
    /// No WHERE filter is applied.
    /// </summary>
    None = 0,
    /// <summary>
    /// The filter is incorporated directly (inline) into the SQL text.
    /// </summary>
    Inline = 1,
    /// <summary>
    /// The filter is incorporated using parameters.
    /// </summary>
    Parameterized = 2
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a condition operator used in building filter expressions.
/// </summary>
public enum ConditionOp
{
    /// <summary>
    /// No operator specified.
    /// </summary>
    None = 0,
    /// <summary>
    /// Equal to (=).
    /// </summary>
    Equal = 1,
    /// <summary>
    /// Not equal to (&lt;&gt;).
    /// </summary>
    NotEqual = 2,
    /// <summary>
    /// Greater than (&gt;).
    /// </summary>
    Greater = 3,
    /// <summary>
    /// Greater than or equal to (&gt;=).
    /// </summary>
    GreaterOrEqual = 4,
    /// <summary>
    /// Less than (&lt;).
    /// </summary>
    Less = 5,
    /// <summary>
    /// Less than or equal to (&lt;=).
    /// </summary>
    LessOrEqual = 6,
    /// <summary>
    /// Matches a pattern using the SQL LIKE operator.
    /// </summary>
    Like = 7,
    /// <summary>
    /// The value contains the specified substring.
    /// </summary>
    Contains = 8,
    /// <summary>
    /// The value starts with the specified substring.
    /// </summary>
    StartsWith = 9,
    /// <summary>
    /// The value ends with the specified substring.
    /// </summary>
    EndsWith = 10,
    /// <summary>
    /// The value is between two specified values.
    /// </summary>
    Between = 11,
    /// <summary>
    /// The value is contained in a specified list of values.
    /// </summary>
    In = 12,
    /// <summary>
    /// The value is null.
    /// </summary>
    Null = 13
}
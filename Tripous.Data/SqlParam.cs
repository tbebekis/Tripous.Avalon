/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a parameter for a SQL statement.
/// </summary>
public class SqlParam
{
 

    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public SqlParam(string Name, object Value)
    {
        this.Name = Name;
        this.Value = Value;
    }

    // ● public
    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The parameter value
    /// </summary>
    public object Value { get; private set; }
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a reference to a SqlParam
/// </summary>
public class SqlParamRef
{
 

    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public SqlParamRef(string Name, int Index)
    {
        this.Name = Name;
        this.Index = Index;
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => Name;
 

    // ● properties
    /// <summary>
    /// The name of the parameter
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The index of the parameter
    /// </summary>
    public int Index { get; private set; }
}
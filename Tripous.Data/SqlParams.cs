/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// A collection of SqlParam objects
/// </summary>
public class SqlParams
{
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public SqlParams()
    {
    }

    // ● public
    /// <summary>
    /// Adds a new SqlParam object to this collection.
    /// </summary>
    public SqlParams Add(string Name, object Value)
    {
        Items.Add(new SqlParam(Name, Value));
        return this;
    }
    
    // ● properties
    /// <summary>
    /// The number of items in this collection.
    /// </summary>
    public int Count => Items.Count;
    /// <summary>
    /// Returns the SqlParam at the specified index.
    /// </summary>
    public SqlParam this[int Index] => Items[Index];
    /// <summary>
    /// The collection of SqlParam objects.
    /// </summary>
    public List<SqlParam> Items { get; private set; } = [];
}
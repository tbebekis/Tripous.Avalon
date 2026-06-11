/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Provides a read-only wrapper around a descriptor list.
/// </summary>
public class ReadOnlyDefList<T> where T: IDef
{
    private readonly DefList<T> InternalDefList;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ReadOnlyDefList(DefList<T> DefList)
    {
        this.InternalDefList = DefList;
    }
 
    // ● public
    /// <summary>
    /// Returns true when a descriptor with the specified name exists.
    /// </summary>
    public bool Contains(string Name) => InternalDefList.Contains(Name);
    /// <summary>
    /// Finds and returns a descriptor by name, if any; otherwise returns null.
    /// </summary>
    public T Find(string Name) => InternalDefList.Find(Name);
    /// <summary>
    /// Returns a descriptor by name.
    /// Throws an exception when the descriptor is not found.
    /// </summary>
    public T Get(string Name) => InternalDefList.Get(Name);
    /// <summary>
    /// Returns an enumerator for the wrapped descriptor list.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => InternalDefList.GetEnumerator();
    
    // ● properties
    /// <summary>
    /// Gets a descriptor by name.
    /// </summary>
    public T this[string Name] => Get(Name);
    /// <summary>
    /// Gets the number of descriptors in the wrapped list.
    /// </summary>
    public int Count => InternalDefList.Count;
}
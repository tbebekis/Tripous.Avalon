/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Base class for item FactBox data providers.
/// </summary>
[TypeStore]
public abstract class ItemFactBoxProvider
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    protected ItemFactBoxProvider()
    {
    }

    // ● public
    /// <summary>
    /// Creates serializable data for a FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The serializable FactBox data.</returns>
    public abstract object GetData(ItemFactBoxContext Context);
}

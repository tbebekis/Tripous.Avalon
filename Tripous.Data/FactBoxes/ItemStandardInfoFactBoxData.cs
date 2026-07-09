/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Serializable data for the built-in item information FactBox.
/// </summary>
public class ItemStandardInfoFactBoxData
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStandardInfoFactBoxData()
    {
    }

    // ● properties
    /// <summary>
    /// The current item row information.
    /// </summary>
    public Dictionary<string, object> ItemInfo { get; set; } = new();
    /// <summary>
    /// The item module structure information.
    /// </summary>
    public ItemStructureFactBoxData Structure { get; set; }
}

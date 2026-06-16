/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the operational purpose of a warehouse.
/// </summary>
[TypeStore]
public enum WarehouseType
{
    /// <summary>No warehouse type is specified.</summary>
    None = 0,
    /// <summary>The primary company warehouse.</summary>
    Main = 1,
    /// <summary>A retail or branch store warehouse.</summary>
    Store = 2,
    /// <summary>A temporary warehouse for goods in transit.</summary>
    Transit = 3,
    /// <summary>A warehouse used for production materials and output.</summary>
    Production = 4,
    /// <summary>A warehouse for damaged, rejected, or scrap items.</summary>
    Scrap = 5,
    /// <summary>A logical warehouse without a physical storage location.</summary>
    Virtual = 6,
}

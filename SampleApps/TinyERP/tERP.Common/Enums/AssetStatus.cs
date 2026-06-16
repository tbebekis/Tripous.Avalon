/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the lifecycle status of a fixed asset.
/// </summary>
[TypeStore]
public enum AssetStatus
{
    /// <summary>No asset status is specified.</summary>
    None = 0,
    /// <summary>The asset record is being prepared.</summary>
    Draft = 1,
    /// <summary>The asset is owned and currently in use.</summary>
    Active = 2,
    /// <summary>The asset has been removed from active use.</summary>
    Disposed = 3,
    /// <summary>The asset was disposed of through a sale.</summary>
    Sold = 4,
    /// <summary>The asset was discarded or destroyed as scrap.</summary>
    Scrapped = 5,
}

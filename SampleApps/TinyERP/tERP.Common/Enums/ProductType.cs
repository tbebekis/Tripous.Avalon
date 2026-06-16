/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the commercial and inventory nature of a product.
/// </summary>
[TypeStore]
public enum ProductType
{
    /// <summary>No product type is specified.</summary>
    None = 0,
    /// <summary>A tangible item that can be sold, purchased, or stocked.</summary>
    Goods = 1,
    /// <summary>An intangible service provided or purchased.</summary>
    Service = 2,
    /// <summary>A material consumed in production or assembly.</summary>
    RawMaterial = 3
}

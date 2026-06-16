/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines a price resolver that determines the unit price of a commercial document line.
/// </summary>
[TypeStore]
public interface IPriceResolver
{
    // ● public
    /// <summary>
    /// Resolves the applicable product price.
    /// </summary>
    PriceResult Resolve(PriceResolveArgs Args);
}

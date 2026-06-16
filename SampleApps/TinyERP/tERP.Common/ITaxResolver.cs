/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines a tax resolver that determines the tax components of a commercial document line.
/// </summary>
[TypeStore]
public interface ITaxResolver
{
    // ● public
    /// <summary>
    /// Resolves the applicable tax rules and calculates the tax result.
    /// </summary>
    TaxResult Resolve(TaxResolveArgs Args);
}

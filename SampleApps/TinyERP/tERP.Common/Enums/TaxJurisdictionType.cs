/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the geographic level represented by a tax jurisdiction.
/// </summary>
[TypeStore]
public enum TaxJurisdictionType
{
    /// <summary>No jurisdiction type is specified.</summary>
    None = 0,
    /// <summary>A sovereign country.</summary>
    Country = 1,
    /// <summary>A state, province, or equivalent administrative region.</summary>
    State = 2,
    /// <summary>A county or equivalent subdivision of a state.</summary>
    County = 3,
    /// <summary>A city or municipality.</summary>
    City = 4,
    /// <summary>A special local tax authority or district.</summary>
    Special = 5,
    /// <summary>A tax territory containing multiple countries, such as the European Union.</summary>
    TaxZone = 6,
}

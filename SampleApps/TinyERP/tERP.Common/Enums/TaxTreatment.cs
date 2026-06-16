/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Legacy document tax treatment retained until the generated modules
/// and TradeDataModule are migrated to the tax rule model.
/// </summary>
[TypeStore]
public enum TaxTreatment
{
    /// <summary>No legacy tax treatment is specified.</summary>
    None = 0,
    /// <summary>The document is subject to normal domestic taxation.</summary>
    Normal = 1,
    /// <summary>The document is exempt from tax.</summary>
    Exempt = 2,
    /// <summary>The document concerns a transaction with a non-EU country.</summary>
    ThirdCountry = 3,
    /// <summary>The document concerns an intra-community EU transaction.</summary>
    IntraCommunity = 4,
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the business role of a contact entry.
/// </summary>
[TypeStore]
public enum ContactType
{
    /// <summary>No contact role is specified.</summary>
    None = 0,
    /// <summary>A general personal contact.</summary>
    Person = 1,
    /// <summary>A contact for accounting and payment matters.</summary>
    Accounting = 2,
    /// <summary>A contact for sales and commercial matters.</summary>
    Sales = 3,
    /// <summary>A contact for technical or customer support.</summary>
    Support = 4,
    /// <summary>A contact with another business role.</summary>
    Other = 5
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines custom commands available for document forms.
/// </summary>
[TypeStore]
public enum DocumentAction
{
    /// <summary>No document action is specified.</summary>
    None = 0,
    /// <summary>Finalizes, posts, and locks the current document.</summary>
    Post = 1,
    /// <summary>Creates a Delivery Note from the current Order.</summary>
    CreateDeliveryNote = 2,
    /// <summary>Creates a Sales Return from the current Sales Delivery Note.</summary>
    CreateReturn = 3,
    /// <summary>Creates an Invoice from the current Delivery Note.</summary>
    CreateInvoice = 4,
    /// <summary>Creates a Credit Note from the current Invoice.</summary>
    CreateCreditNote = 5,
    /// <summary>Creates a Cancellation document from the current Invoice.</summary>
    CreateCancellation = 6,
    /// <summary>Creates a payment document from the current Invoice.</summary>
    CreatePayment = 7,
}

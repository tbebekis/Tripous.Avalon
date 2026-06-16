/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines whether increases to an account are normally recorded as debits or credits.
/// </summary>
[TypeStore]
public enum NormalBalance
{
    /// <summary>No normal balance is specified.</summary>
    None = 0,

    /// <summary>
    /// Debit-nature account.
    /// Typical for Assets and Expenses.
    /// </summary>
    Debit = 1,

    /// <summary>
    /// Credit-nature account.
    /// Typical for Liabilities, Equity and Revenue.
    /// </summary>
    Credit = 2,
}

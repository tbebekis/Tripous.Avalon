/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides Purchase Invoice cancellation posting behavior.
/// </summary>
public class PurchaseCancellationDataModule: PurchaseDataModule
{
    // ● protected
    /// <summary>
    /// Cancels the source Purchase Invoice during Cancellation posting.
    /// </summary>
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            CancelSourceInvoice(e.Transaction, "Purchase Invoice");
    }

    // ● construction
    public PurchaseCancellationDataModule()
    {
    }
}

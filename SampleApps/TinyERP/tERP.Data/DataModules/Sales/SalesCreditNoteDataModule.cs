/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides Sales Credit Note posting behavior.
/// </summary>
public class SalesCreditNoteDataModule: SalesDataModule
{
    // ● protected
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceTransformationQuantities(e.Transaction, "Sales Invoice", "CreditedQuantity", "Credit", "The Sales Credit Note has no lines.");
    }

    // ● construction
    public SalesCreditNoteDataModule()
    {
    }
}

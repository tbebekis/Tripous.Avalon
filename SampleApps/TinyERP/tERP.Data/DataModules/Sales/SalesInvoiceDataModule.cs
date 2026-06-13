/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class SalesInvoiceDataModule: SalesDataModule
{
    // ● protected
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceTransformationQuantities(e.Transaction, "Sales Delivery Note", "InvoicedQuantity", "Invoice", "The Sales Invoice has no lines.");
    }

    // ● construction
    public SalesInvoiceDataModule()
    {
    }
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides Sales Invoice behavior and Credit Note transformation.
/// </summary>
public class SalesInvoiceDataModule: SalesDataModule
{
    // ● protected
    /// <summary>
    /// Validates that the current Sales Invoice can create a Credit Note.
    /// </summary>
    protected virtual void CheckCanCreateCreditNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Sales Invoice is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Sales Invoice changes before creating a Sales Credit Note.");
    }
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

    // ● public
    /// <summary>
    /// Creates an unsaved Sales Credit Note from the remaining invoice quantities.
    /// </summary>
    public virtual SalesCreditNoteDataModule CreateCreditNote()
    {
        CheckCanCreateCreditNote();
        SalesCreditNoteDataModule Result = CreateTransformedDocument("SalesCreditNote", "Sales Invoice", "CreditedQuantity") as SalesCreditNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Sales Credit Note module.");
        return Result;
    }
}

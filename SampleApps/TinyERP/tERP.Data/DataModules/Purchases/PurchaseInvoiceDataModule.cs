/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides Purchase Invoice behavior and Credit Note transformation.
/// </summary>
public class PurchaseInvoiceDataModule: PurchaseDataModule
{
    // ● protected
    /// <summary>
    /// Validates that the current Purchase Invoice can create a Credit Note.
    /// </summary>
    protected virtual void CheckCanCreateCreditNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Invoice is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Invoice changes before creating a Purchase Credit Note.");
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceTransformationQuantities(e.Transaction, "Purchase Delivery Note", "InvoicedQuantity", "Invoice", "The Purchase Invoice has no lines.");
    }

    // ● construction
    public PurchaseInvoiceDataModule()
    {
    }

    // ● public
    /// <summary>
    /// Creates an unsaved Purchase Credit Note from the remaining invoice quantities.
    /// </summary>
    public virtual PurchaseCreditNoteDataModule CreateCreditNote()
    {
        CheckCanCreateCreditNote();
        PurchaseCreditNoteDataModule Result = CreateTransformedDocument("PurchaseCreditNote", "Purchase Invoice", "CreditedQuantity") as PurchaseCreditNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Credit Note module.");
        return Result;
    }
}

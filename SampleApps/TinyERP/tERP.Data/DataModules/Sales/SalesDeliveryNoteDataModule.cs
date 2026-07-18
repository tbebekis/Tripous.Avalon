/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class SalesDeliveryNoteDataModule: SalesStockDataModule
{
    // ● protected
    protected virtual void CheckCanCreateInvoice()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Sales Delivery Note is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Sales Delivery Note changes before creating a Sales Invoice.");
    }
    protected virtual void CheckCanCreateReturn()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Sales Delivery Note is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Sales Delivery Note changes before creating a Sales Return.");
    }
    protected virtual void ApplyJsonSource(JsonDataModule Source)
    {
        if (Source == null)
            throw new TripousArgumentNullException(nameof(Source));

        State = (DataMode)Source.State;

        tblItem.EventsDisabled = true;
        try
        {
            JsonApplyTableRows(tblItem, Source);
        }
        finally
        {
            tblItem.EventsDisabled = false;
        }
    }
    protected override bool UpdatesSourceOrder() => true;

    // ● construction
    public SalesDeliveryNoteDataModule()
    {
    }

    // ● public
    public virtual SalesReturnDataModule CreateReturn()
    {
        CheckCanCreateReturn();
        SalesReturnDataModule Result = CreateTransformedDocument("SalesReturn", "Sales Delivery Note") as SalesReturnDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Sales Return module.");
        return Result;
    }
    public virtual SalesInvoiceDataModule CreateInvoice()
    {
        CheckCanCreateInvoice();
        SalesInvoiceDataModule Result = CreateTransformedDocument("SalesInvoice", "Sales Delivery Note", "InvoicedQuantity") as SalesInvoiceDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Sales Invoice module.");
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Sales Return data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateReturn(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        SalesReturnDataModule ReturnModule = CreateReturn();
        JsonDataModule Result = new(ReturnModule);
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Sales Invoice data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateInvoice(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        SalesInvoiceDataModule InvoiceModule = CreateInvoice();
        JsonDataModule Result = new(InvoiceModule);
        return Result;
    }
}

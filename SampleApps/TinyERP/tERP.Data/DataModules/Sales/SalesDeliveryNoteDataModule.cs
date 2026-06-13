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
}
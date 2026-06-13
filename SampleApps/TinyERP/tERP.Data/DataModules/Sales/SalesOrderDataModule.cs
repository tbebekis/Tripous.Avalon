/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;


public class SalesOrderDataModule: SalesDataModule
{
    // ● protected
    protected virtual void CheckCanCreateDeliveryNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Sales Order is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Sales Order changes before creating a Sales Delivery Note.");
    }

    // ● construction
    public SalesOrderDataModule()
    {
    }

    // ● public
    public virtual SalesDeliveryNoteDataModule CreateDeliveryNote()
    {
        CheckCanCreateDeliveryNote();
        SalesDeliveryNoteDataModule Result = CreateTransformedDocument("SalesDeliveryNote", "Sales Order") as SalesDeliveryNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Sales Delivery Note module.");
        return Result;
    }
}
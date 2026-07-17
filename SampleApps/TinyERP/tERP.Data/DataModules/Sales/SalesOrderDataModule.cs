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
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Sales Delivery Note data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateDeliveryNote(JsonDataModule Source)
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

        SalesDeliveryNoteDataModule DeliveryNoteModule = CreateDeliveryNote();
        JsonDataModule Result = new(DeliveryNoteModule);
        return Result;
    }
}

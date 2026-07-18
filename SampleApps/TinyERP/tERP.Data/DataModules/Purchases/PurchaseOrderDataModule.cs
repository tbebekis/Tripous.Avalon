/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class PurchaseOrderDataModule: PurchaseDataModule
{
    // ● protected
    protected virtual void CheckCanCreateDeliveryNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Order is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Order changes before creating a Purchase Delivery Note.");
    }

    // ● construction
    public PurchaseOrderDataModule()
    {
    }

    // ● public
    public virtual PurchaseDeliveryNoteDataModule CreateDeliveryNote()
    {
        CheckCanCreateDeliveryNote();
        PurchaseDeliveryNoteDataModule Result = CreateTransformedDocument("PurchaseDeliveryNote", "Purchase Order") as PurchaseDeliveryNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Delivery Note module.");
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Purchase Delivery Note data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateDeliveryNote(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        PurchaseDeliveryNoteDataModule DeliveryNoteModule = CreateDeliveryNote();
        JsonDataModule Result = new(DeliveryNoteModule);
        return Result;
    }
}

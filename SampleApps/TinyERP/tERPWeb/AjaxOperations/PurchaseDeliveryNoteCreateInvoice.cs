/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Purchase Invoice data module from a Purchase Delivery Note data module.
/// </summary>
[AjaxOperation("App.PurchaseDeliveryNote.CreateInvoice")]
public class PurchaseDeliveryNoteCreateInvoice: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not PurchaseDeliveryNoteDataModule)
            Sys.Throw($"DataModule is not a Purchase Delivery Note module: {Module.Name}");

        PurchaseDeliveryNoteDataModule DeliveryNoteModule = Module as PurchaseDeliveryNoteDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "PurchaseInvoice";
        Result["DataModule"] = DeliveryNoteModule.JsonCreateInvoice(Packet);
        return Result;
    }
}

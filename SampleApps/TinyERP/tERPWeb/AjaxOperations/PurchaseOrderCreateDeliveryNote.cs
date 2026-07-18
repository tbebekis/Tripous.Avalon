/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Purchase Delivery Note data module from a Purchase Order data module.
/// </summary>
[AjaxOperation("App.PurchaseOrder.CreateDeliveryNote")]
public class PurchaseOrderCreateDeliveryNote: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not PurchaseOrderDataModule)
            Sys.Throw($"DataModule is not a Purchase Order module: {Module.Name}");

        PurchaseOrderDataModule PurchaseOrderModule = Module as PurchaseOrderDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "PurchaseDeliveryNote";
        Result["DataModule"] = PurchaseOrderModule.JsonCreateDeliveryNote(Packet);
        return Result;
    }
}

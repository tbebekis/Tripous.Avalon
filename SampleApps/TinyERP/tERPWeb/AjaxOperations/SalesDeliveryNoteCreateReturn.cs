/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Sales Return data module from a Sales Delivery Note data module.
/// </summary>
[AjaxOperation("App.SalesDeliveryNote.CreateReturn")]
public class SalesDeliveryNoteCreateReturn: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not SalesDeliveryNoteDataModule)
            Sys.Throw($"DataModule is not a Sales Delivery Note module: {Module.Name}");

        SalesDeliveryNoteDataModule DeliveryNoteModule = Module as SalesDeliveryNoteDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "SalesReturn";
        Result["DataModule"] = DeliveryNoteModule.JsonCreateReturn(Packet);
        return Result;
    }
}

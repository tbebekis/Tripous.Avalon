/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Sales Delivery Note data module from a Sales Order data module.
/// </summary>
[AjaxOperation("App.SalesOrder.CreateDeliveryNote")]
public class SalesOrderCreateDeliveryNote: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not SalesOrderDataModule)
            Sys.Throw($"DataModule is not a Sales Order module: {Module.Name}");

        SalesOrderDataModule SalesOrderModule = Module as SalesOrderDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "SalesDeliveryNote";
        Result["DataModule"] = SalesOrderModule.JsonCreateDeliveryNote(Packet);
        return Result;
    }
}

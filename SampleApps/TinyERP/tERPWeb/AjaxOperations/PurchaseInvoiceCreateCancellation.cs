/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Purchase Cancellation data module from a Purchase Invoice data module.
/// </summary>
[AjaxOperation("App.PurchaseInvoice.CreateCancellation")]
public class PurchaseInvoiceCreateCancellation: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not PurchaseInvoiceDataModule)
            Sys.Throw($"DataModule is not a Purchase Invoice module: {Module.Name}");

        PurchaseInvoiceDataModule InvoiceModule = Module as PurchaseInvoiceDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "PurchaseCancellation";
        Result["DataModule"] = InvoiceModule.JsonCreateCancellation(Packet);
        return Result;
    }
}

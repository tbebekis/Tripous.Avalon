/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Sales Cancellation data module from a Sales Invoice data module.
/// </summary>
[AjaxOperation("App.SalesInvoice.CreateCancellation")]
public class SalesInvoiceCreateCancellation: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not SalesInvoiceDataModule)
            Sys.Throw($"DataModule is not a Sales Invoice module: {Module.Name}");

        SalesInvoiceDataModule InvoiceModule = Module as SalesInvoiceDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "SalesCancellation";
        Result["DataModule"] = InvoiceModule.JsonCreateCancellation(Packet);
        return Result;
    }
}

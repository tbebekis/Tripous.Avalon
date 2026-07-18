/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a payment cancellation data module from a payment data module.
/// </summary>
[AjaxOperation("App.Payment.CreateCancellation")]
public class PaymentCreateCancellation: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not PaymentDataModule)
            Sys.Throw($"DataModule is not a payment module: {Module.Name}");

        PaymentDataModule PaymentModule = Module as PaymentDataModule;
        JsonDataModule CancellationPacket = PaymentModule.JsonCreateCancellation(Packet);
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = CancellationPacket.Name;
        Result["DataModule"] = CancellationPacket;
        return Result;
    }
}

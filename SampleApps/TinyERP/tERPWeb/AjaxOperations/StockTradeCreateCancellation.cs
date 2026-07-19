/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates a Stock Transaction cancellation data module from a Stock Transaction data module.
/// </summary>
[AjaxOperation("App.StockTrade.CreateCancellation")]
public class StockTradeCreateCancellation: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not StockTradeDataModule)
            Sys.Throw($"DataModule is not a Stock Transaction module: {Module.Name}");

        StockTradeDataModule StockTradeModule = Module as StockTradeDataModule;
        AjaxResponse Result = new(Request.OperationName);
        Result["WebFormName"] = "StockTrade";
        Result["DataModule"] = StockTradeModule.JsonCreateCancellation(Packet);
        return Result;
    }
}

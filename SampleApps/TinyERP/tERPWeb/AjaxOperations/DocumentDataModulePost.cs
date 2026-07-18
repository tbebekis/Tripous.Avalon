/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Posts a document data module.
/// </summary>
[AjaxOperation("App.DocumentDataModule.Post")]
public class DocumentDataModulePost: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        if (Module is not DocumentDataModule)
            Sys.Throw($"DataModule is not a document module: {Module.Name}");

        DocumentDataModule DocumentModule = Module as DocumentDataModule;
        JsonDataModule ResultPacket = DocumentModule.JsonPost(Packet);
        AjaxResponse Result = CreateDataModuleResponse(Request, ResultPacket);
        Result["PostedInfo"] = DocumentPostedInfo.FromModule(Module.Name, DocumentModule).ToDictionary();
        return Result;
    }
}

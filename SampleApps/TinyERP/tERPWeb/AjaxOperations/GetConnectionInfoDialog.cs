/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the database connection dialog markup.
/// </summary>
[AjaxOperation("App.GetConnectionInfoDialog")]
public class GetConnectionInfoDialog: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["Html"] = Context.ViewToStringConverter.ViewToString("/Views/Home/_ConnectionInfoDialog.cshtml");
        Result["ConnectionInfo"] = GetDefaultConnectionInfoPacket();
        Result["Providers"] = GetConnectionProviderPackets();
        return Result;
    }
}

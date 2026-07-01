/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the HTML and metadata for a registered WebDesk form.
/// </summary>
[AjaxOperation("App.GetWebForm")]
public class GetWebForm: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        string WebFormName = GetStringParam(Request, "WebFormName");
        if (string.IsNullOrWhiteSpace(WebFormName))
            WebFormName = GetStringParam(Request, "Form");
        if (string.IsNullOrWhiteSpace(WebFormName))
            Sys.Throw("No WebFormName specified.");

        WebFormDef Form = WebDeskRegistry.FindForm(WebFormName);
        if (Form == null)
            Sys.Throw($"WebForm not found: {WebFormName}");

        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        if (!Form.CanAccess(User))
            Sys.Throw($"Access denied to WebForm: {WebFormName}");

        WebFormProviderPacket Packet = Tripous.WebDesk.WebFormProviders.Execute(Request, Form, Context);

        AjaxResponse Result = new(Request.OperationName);
        Result["Form"] = Packet;
        return Result;
    }
}

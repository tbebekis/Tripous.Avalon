/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns registered WebDesk forms available to the current user.
/// </summary>
[AjaxOperation("App.GetWebForms")]
public class GetWebForms: AjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        var Forms = WebDeskRegistry.Forms
            .Where(Form => Form.CanAccess(User))
            .OrderBy(Form => Form.Group)
            .ThenBy(Form => Form.Title)
            .Select(Form => new
            {
                Form.Name,
                Form.TitleKey,
                Form.Title,
                Form.Module,
                Form.ViewName,
                Form.ItemViewName,
                Form.Group,
                Form.IsReadOnly,
                Form.IsCustom,
                Form.JsFormClassType,
                Form.CssFiles,
                Form.JavaScriptFiles
            })
            .ToArray();

        AjaxResponse Result = new(Request.OperationName);
        Result["WebForms"] = Forms;
        return Result;
    }
}

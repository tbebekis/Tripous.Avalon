/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the change password dialog markup.
/// </summary>
[AjaxOperation("App.GetChangePasswordDialog")]
public class GetChangePasswordDialog: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        AddUserInfo(Result);
        Result["Html"] = Context.ViewToStringConverter.ViewToString("/Views/Home/_ChangePasswordDialog.cshtml");
        return Result;
    }
}

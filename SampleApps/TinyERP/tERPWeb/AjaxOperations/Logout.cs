/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Logs out the current application user.
/// </summary>
[AjaxOperation("App.Logout")]
public class Logout: AjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (Sys.Context != null)
            Sys.Context.CurrentUser = null;

        AjaxResponse Result = new(Request.OperationName);
        Result["Success"] = true;
        Result["Message"] = "Logged out.";
        return Result;
    }
}

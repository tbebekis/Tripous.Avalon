/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns string resources for the current user language.
/// </summary>
[AjaxOperation("App.GetStringResources")]
public class GetStringResources: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        if (Sys.Context == null || Sys.Context.CurrentUser == null)
            AutoLoginUser();
        AddStringResourceInfo(Result);
        return Result;
    }
}

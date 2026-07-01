/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns client startup information.
/// </summary>
[AjaxOperation("App.GetStartupInfo")]
public class GetStartupInfo: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        bool FirstRunRequired = RequiresFirstRun();
        bool UsersEnabled = UseUsers();

        if (!FirstRunRequired && !UsersEnabled)
            AutoLoginUser();

        AjaxResponse Result = new(Request.OperationName);
        Result["ApplicationName"] = SysConfig.AppName;
        Result["UseUsers"] = UsersEnabled;
        Result["RequiresFirstRun"] = FirstRunRequired;
        Result["SupportedCultures"] = DataLib.SupportedCultures;
        if (FirstRunRequired)
            Result["FirstRunHtml"] = Context.ViewToStringConverter.ViewToString("/Views/Home/_FirstRunDialog.cshtml");
        else if (UsersEnabled && (Sys.Context == null || Sys.Context.CurrentUser == null))
            Result["LoginHtml"] = Context.ViewToStringConverter.ViewToString("/Views/Home/_LoginDialog.cshtml");
        AddUserInfo(Result);
        return Result;
    }
}

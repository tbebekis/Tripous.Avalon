/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Changes the password of the current application user.
/// </summary>
[AjaxOperation("App.ChangePassword")]
public class ChangePassword: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        string CurrentPassword = GetStringParam(Request, "CurrentPassword");
        string NewPassword = GetStringParam(Request, "NewPassword");
        string ConfirmPassword = GetStringParam(Request, "ConfirmPassword");

        if (User == null)
        {
            Result["Success"] = false;
            Result["Message"] = "No current user.";
            return Result;
        }
        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            Result["Success"] = false;
            Result["Message"] = "All password fields are required.";
            return Result;
        }
        if (NewPassword != ConfirmPassword)
        {
            Result["Success"] = false;
            Result["Message"] = "Passwords differ.";
            return Result;
        }

        try
        {
            AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
            Module.ChangePassword(User.UserName, CurrentPassword, NewPassword);
            Result["Success"] = true;
            Result["Message"] = "Password changed.";
        }
        catch (Exception e)
        {
            Result["Success"] = false;
            Result["Message"] = e.Message;
        }

        return Result;
    }
}

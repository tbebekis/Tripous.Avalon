/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Authenticates an application user.
/// </summary>
[AjaxOperation("App.Login")]
public class Login: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        string UserName = GetStringParam(Request, "UserName");
        string PasswordText = GetStringParam(Request, "Password");
        string CultureCode = GetStringParam(Request, "CultureCode");
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(PasswordText))
        {
            Result["Success"] = false;
            Result["Message"] = "Incomplete input";
            return Result;
        }

        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
        AppUser User = Module.LoadByUserName(UserName);
        if (User == null)
        {
            Result["Success"] = false;
            Result["Message"] = "Invalid user name or password.";
            return Result;
        }
        if (!User.IsActive)
        {
            Result["Success"] = false;
            Result["Message"] = "User account is disabled.";
            return Result;
        }

        string Password = User.Properties["Password"] as string;
        string Salt = User.Properties["Salt"] as string;
        bool Flag = Sec.VerifyPassword(PasswordText, Password, Salt, 100_000);
        if (!Flag)
        {
            Result["Success"] = false;
            Result["Message"] = "Invalid user name or password.";
            return Result;
        }

        User.CultureCode = !string.IsNullOrWhiteSpace(CultureCode) ? CultureCode : User.CultureCode;
        User.LastLoginAt = Module.RecordLogin(User.Id, User.CultureCode);
        Sys.Context.CurrentUser = User;

        Result["Success"] = true;
        Result["Message"] = "Login succeeded.";
        AddUserInfo(Result);
        AddStringResourceInfo(Result);
        return Result;
    }
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Creates the first administrator user.
/// </summary>
[AjaxOperation("App.CreateFirstRunAdmin")]
public class CreateFirstRunAdmin: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        if (!RequiresFirstRun())
        {
            Result["Success"] = false;
            Result["Message"] = "First run setup is not required.";
            return Result;
        }

        string FullName = GetStringParam(Request, "FullName");
        string UserName = GetStringParam(Request, "UserName");
        string Password = GetStringParam(Request, "Password");
        string ConfirmPassword = GetStringParam(Request, "ConfirmPassword");
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            Result["Success"] = false;
            Result["Message"] = "Incomplete input";
            return Result;
        }
        if (Password != ConfirmPassword)
        {
            Result["Success"] = false;
            Result["Message"] = "Passwords differ";
            return Result;
        }

        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
        Module.AddUser(FullName: FullName, UserName: UserName, PlainTextPassword: Password, UserLevel: UserLevel.Admin);
        if (!UseUsers())
            Sys.Context.CurrentUser = Module.LoadByUserName(UserName);

        Result["Success"] = true;
        Result["Message"] = "Administrator account created.";
        AddUserInfo(Result);
        return Result;
    }
}

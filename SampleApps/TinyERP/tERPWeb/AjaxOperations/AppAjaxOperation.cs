/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Base class for tERPWeb Ajax operations.
/// </summary>
public abstract class AppAjaxOperation: AjaxOperation
{
    // ● protected
    /// <summary>
    /// Returns true when application users are enabled.
    /// </summary>
    protected bool UseUsers()
    {
        string Value = Config.GetValue(DataLib.SUseUsers);
        return !string.IsNullOrWhiteSpace(Value) && Convert.ToBoolean(Value);
    }
    /// <summary>
    /// Returns true when the first application user must be created.
    /// </summary>
    protected bool RequiresFirstRun()
    {
        SqlStore Store = Db.DefaultStore;
        return !Store.TableExists(DbConfig.SysAppUserTableName) || Store.TableIsEmpty(DbConfig.SysAppUserTableName);
    }
    /// <summary>
    /// Returns the user name used for development auto-login.
    /// </summary>
    protected string GetAutoLoginUserName()
    {
        if (!string.IsNullOrWhiteSpace(DataLib.DebugUserName))
            return DataLib.DebugUserName;
        object Result = Db.DefaultStore.SelectResult("""
            select UserName
            from SYS_APP_USER
            where IsActive = 1
            order by UserLevelId desc, UserName
            """, string.Empty);
        return Result == null || Result == DBNull.Value ? string.Empty : Result.ToString();
    }
    /// <summary>
    /// Auto-logins the current request context when users are disabled.
    /// </summary>
    protected void AutoLoginUser()
    {
        if (Sys.Context == null || Sys.Context.CurrentUser != null)
            return;
        string UserName = GetAutoLoginUserName();
        if (string.IsNullOrWhiteSpace(UserName))
            return;
        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
        AppUser User = Module.LoadByUserName(UserName);
        if (User != null && User.IsActive)
            Sys.Context.CurrentUser = User;
    }
    /// <summary>
    /// Writes current user properties to a response.
    /// </summary>
    protected void AddUserInfo(AjaxResponse Response)
    {
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        Response["IsAuthenticated"] = User != null;
        Response["UserName"] = User != null ? User.UserName : string.Empty;
        Response["FullName"] = User != null ? User.FullName : string.Empty;
        Response["UserLevel"] = User != null ? User.UserLevel.ToString() : UserLevel.None.ToString();
        Response["UserLevelId"] = User != null ? (int)User.UserLevel : (int)UserLevel.None;
        Response["CultureCode"] = User != null && !string.IsNullOrWhiteSpace(User.CultureCode) ? User.CultureCode : CultureInfo.CurrentCulture.Name;
    }
}

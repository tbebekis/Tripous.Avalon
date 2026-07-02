/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the application settings dialog markup.
/// </summary>
[AjaxOperation("App.GetApplicationSettingsDialog")]
public class GetApplicationSettingsDialog: AppAjaxOperation
{
    // ● private
    bool CanAccess(ConfigPropertyDef Def)
    {
        UserLevel Current = Sys.Context != null && Sys.Context.CurrentUser != null ? Sys.Context.CurrentUser.UserLevel : UserLevel.God;
        if ((Current & UserLevel.God) == UserLevel.God)
            return true;
        if (Def.SecurityLevel == UserLevel.None)
            return true;
        if ((Current & UserLevel.Admin) == UserLevel.Admin)
            return Def.SecurityLevel == UserLevel.Admin || Def.SecurityLevel == UserLevel.User || Def.SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.User) == UserLevel.User)
            return Def.SecurityLevel == UserLevel.User || Def.SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.Guest) == UserLevel.Guest)
            return Def.SecurityLevel == UserLevel.Guest;
        return (Current & Def.SecurityLevel) == Def.SecurityLevel;
    }
    ConfigScope GetScope(AjaxRequest Request)
    {
        string Text = GetStringParam(Request, "Scope");
        if (Enum.TryParse(Text, true, out ConfigScope Result))
            return Result;
        return ConfigScope.User;
    }
    string GetOwnerKey(ConfigScope Scope)
    {
        if (Scope == ConfigScope.System)
            return string.Empty;
        if (Scope == ConfigScope.Company)
            return DbConfig.CompanyId.ToString();
        return Sys.GetCurrentAppUserName();
    }
    string GetEffectiveValue(ConfigPropertyDef Def, ConfigScope Scope, string OwnerKey)
    {
        string Value = Config.GetValue(Def.Name, Scope, OwnerKey);
        if (Value == null)
            Value = Config.GetValue(Def.Name);
        return Value;
    }
    string GetTitle(ConfigPropertyDef Def)
    {
        if (!string.IsNullOrWhiteSpace(Def.TitleKey))
            return Def.TitleKey;
        return Def.Name;
    }
    bool IsVisibleAtScope(ConfigPropertyDef Def, ConfigScope Scope)
    {
        return Def.SupportsScope(Scope);
    }
    List<Dictionary<string, object>> GetScalarSettings(ConfigScope Scope, string OwnerKey)
    {
        return DataRegistry.ConfigProperties
            .Where(CanAccess)
            .Where(Def => IsVisibleAtScope(Def, Scope))
            .Where(Def => Def.Kind != ConfigValueKind.Object)
            .OrderBy(Def => Def.GroupName)
            .ThenBy(GetTitle)
            .Select(Def => new Dictionary<string, object>()
            {
                ["Name"] = Def.Name,
                ["Title"] = GetTitle(Def),
                ["GroupName"] = Def.GroupName ?? string.Empty,
                ["Kind"] = Def.Kind.ToString(),
                ["Value"] = GetEffectiveValue(Def, Scope, OwnerKey) ?? string.Empty
            })
            .ToList();
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        ConfigScope Scope = GetScope(Request);
        string OwnerKey = GetOwnerKey(Scope);
        Dictionary<string, object> ViewData = new()
        {
            ["Scope"] = Scope.ToString(),
            ["OwnerKey"] = OwnerKey,
            ["Settings"] = GetScalarSettings(Scope, OwnerKey)
        };

        AjaxResponse Result = new(Request.OperationName);
        Result["Html"] = Context.ViewToStringConverter.ViewToString("/Views/Home/_ApplicationSettingsDialog.cshtml", ViewData);
        return Result;
    }
}

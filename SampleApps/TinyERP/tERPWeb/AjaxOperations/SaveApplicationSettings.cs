/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Saves application settings.
/// </summary>
[AjaxOperation("App.SaveApplicationSettings")]
public class SaveApplicationSettings: AppAjaxOperation
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
    Dictionary<string, string> GetValues(AjaxRequest Request)
    {
        Dictionary<string, string> Result = new();
        object Value = Request.GetParam("Values");
        if (Value is JsonElement Element && Element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty Property in Element.EnumerateObject())
                Result[Property.Name] = Property.Value.ValueKind == JsonValueKind.String ? Property.Value.GetString() : Property.Value.ToString();
        }
        else if (Value is Dictionary<string, object> Dictionary)
        {
            foreach (var Pair in Dictionary)
                Result[Pair.Key] = Convert.ToString(Pair.Value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
        return Result;
    }
    string NormalizeValue(ConfigPropertyDef Def, string Value)
    {
        if (Def.Kind == ConfigValueKind.Boolean)
            return !string.IsNullOrWhiteSpace(Value) && Convert.ToBoolean(Value, CultureInfo.InvariantCulture) ? "true" : "false";
        return Value ?? string.Empty;
    }
    bool CanSaveAtScope(ConfigPropertyDef Def, ConfigScope Scope)
    {
        return Def.SupportsScope(Scope);
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        ConfigScope Scope = GetScope(Request);
        string OwnerKey = GetOwnerKey(Scope);
        Dictionary<string, string> Values = GetValues(Request);
        int Count = 0;

        foreach (var Pair in Values)
        {
            ConfigPropertyDef Def = DataRegistry.ConfigProperties.Find(Pair.Key);
            if (Def == null || Def.Kind == ConfigValueKind.Object)
                continue;
            if (!CanSaveAtScope(Def, Scope))
                continue;
            if (!CanAccess(Def))
                throw new TripousException($"Access denied to setting '{Def.Name}'.");
            Config.SetValue(Def.Name, NormalizeValue(Def, Pair.Value), Scope, OwnerKey);
            Count++;
        }

        AjaxResponse Result = new(Request.OperationName);
        Result["Success"] = true;
        Result["Message"] = "Settings saved.";
        Result["Count"] = Count;
        return Result;
    }
}

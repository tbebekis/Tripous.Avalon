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
    // ● private
    /// <summary>
    /// Returns the logical connection property types in display order.
    /// </summary>
    static DbConPropType[] GetConnectionPropTypes()
    {
        return [
            DbConPropType.Server,
            DbConPropType.Port,
            DbConPropType.Database,
            DbConPropType.UserId,
            DbConPropType.Password,
            DbConPropType.IntegratedSecurity,
            DbConPropType.TrustServerCertificate,
            DbConPropType.SslMode,
            DbConPropType.Charset
        ];
    }
    /// <summary>
    /// Returns a string property value from a JSON object.
    /// </summary>
    static string GetJsonString(JsonElement Element, string Name)
    {
        if (Element.ValueKind == JsonValueKind.Object && Element.TryGetProperty(Name, out JsonElement Value))
            return Value.ValueKind == JsonValueKind.String ? Value.GetString() ?? string.Empty : Value.ToString();
        return string.Empty;
    }

    // ● protected
    /// <summary>
    /// Returns true when application users are enabled.
    /// </summary>
    protected bool UseUsers()
    {
        string Value = Config.GetValue(DataLib.SUseUsers, ConfigScope.System, string.Empty);
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
        Response["IsAdmin"] = User != null && (User.IsAdmin || User.IsGod);
        Response["CultureCode"] = User != null && !string.IsNullOrWhiteSpace(User.CultureCode) ? User.CultureCode : CultureInfo.CurrentCulture.Name;
    }
    /// <summary>
    /// Returns the physical file path of the default SQLite database.
    /// </summary>
    protected string GetDefaultDatabaseFilePath()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        if (ConnectionInfo.DbServerType != DbServerType.Sqlite)
            throw new TripousException("Database regeneration is supported only for SQLite connections.");

        ConnectionStringBuilder Builder = new(ConnectionInfo.ConnectionString);
        string Result = Builder.Database;
        Result = ConnectionStringBuilder.ReplacePathPlaceholders(Result);
        return Result;
    }
    /// <summary>
    /// Returns true when the current user may access a resource with a required security level.
    /// </summary>
    protected bool CanCurrentUserAccess(UserLevel SecurityLevel)
    {
        if (SecurityLevel == UserLevel.None)
            return true;
        if ((Sys.Context == null || Sys.Context.CurrentUser == null) && !UseUsers())
            AutoLoginUser();
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        UserLevel Current = User != null ? User.UserLevel : UserLevel.None;
        if ((Current & UserLevel.God) == UserLevel.God)
            return true;
        if ((Current & UserLevel.Admin) == UserLevel.Admin)
            return SecurityLevel == UserLevel.Admin || SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.User) == UserLevel.User)
            return SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.Guest) == UserLevel.Guest)
            return SecurityLevel == UserLevel.Guest;
        return (Current & SecurityLevel) == SecurityLevel;
    }
    /// <summary>
    /// Returns the requested database server type.
    /// </summary>
    protected DbServerType GetDbServerTypeParam(AjaxRequest Request, DbServerType Default)
    {
        string Text = GetStringParam(Request, "DbServerType");
        if (Enum.TryParse(Text, true, out DbServerType Result))
            return Result;
        return Default;
    }
    /// <summary>
    /// Returns the requested command timeout.
    /// </summary>
    protected int GetCommandTimeoutParam(AjaxRequest Request, int Default)
    {
        string Text = GetStringParam(Request, "CommandTimeoutSeconds");
        if (int.TryParse(Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Result))
            return Result;
        return Default;
    }
    /// <summary>
    /// Returns connection properties from request values.
    /// </summary>
    protected List<DbConProp> GetConnectionProps(AjaxRequest Request, DbConAdapter Adapter)
    {
        List<DbConProp> Result = new();
        object Value = Request.GetParam("Values");
        if (Value is JsonElement Element && Element.ValueKind == JsonValueKind.Object)
        {
            foreach (DbConPropType PropType in GetConnectionPropTypes())
            {
                if (!Adapter.IsValid(PropType))
                    continue;
                string Text = GetJsonString(Element, PropType.ToString()).Trim();
                if (!string.IsNullOrWhiteSpace(Text))
                    Result.Add(new DbConProp { PropType = PropType, Value = Text });
            }
        }
        return Result;
    }
    /// <summary>
    /// Validates connection information before save or test.
    /// </summary>
    protected string ValidateConnectionInfo(string Name, DbConAdapter Adapter, List<DbConProp> Props)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "Name is required.";
        foreach (DbConPropDef Def in Adapter.PropDefs.Where(item => item.IsRequired))
        {
            DbConProp Prop = Props.FirstOrDefault(item => item.PropType == Def.PropType);
            if (Prop == null || string.IsNullOrWhiteSpace(Prop.Value))
                return Def.Label + " is required.";
        }
        return string.Empty;
    }
    /// <summary>
    /// Returns metadata for supported database providers.
    /// </summary>
    protected List<Dictionary<string, object>> GetConnectionProviderPackets()
    {
        List<Dictionary<string, object>> Result = new();
        foreach (DbServerType ServerType in Enum.GetValues(typeof(DbServerType)).Cast<DbServerType>())
        {
            DbConAdapter Adapter = DbConAdapters.Get(ServerType);
            Result.Add(new Dictionary<string, object>()
            {
                ["Name"] = ServerType.ToString(),
                ["Props"] = Adapter.PropDefs.Select(Def => new Dictionary<string, object>()
                {
                    ["PropType"] = Def.PropType.ToString(),
                    ["Label"] = Def.Label,
                    ["IsRequired"] = Def.IsRequired,
                    ["DefaultValue"] = Def.DefaultValue,
                    ["ValidValues"] = Def.ValidValues
                }).ToList()
            });
        }
        return Result;
    }
    /// <summary>
    /// Returns a packet for the default connection information.
    /// </summary>
    protected Dictionary<string, object> GetDefaultConnectionInfoPacket()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        DbConAdapter Adapter = DbConAdapters.Get(ConnectionInfo.DbServerType);
        return new Dictionary<string, object>()
        {
            ["Name"] = ConnectionInfo.Name ?? string.Empty,
            ["DbServerType"] = ConnectionInfo.DbServerType.ToString(),
            ["CommandTimeoutSeconds"] = ConnectionInfo.CommandTimeoutSeconds,
            ["ConnectionString"] = ConnectionInfo.ConnectionString ?? string.Empty,
            ["Props"] = Adapter.Parse(ConnectionInfo.ConnectionString ?? string.Empty).Select(Prop => new Dictionary<string, object>()
            {
                ["PropType"] = Prop.PropType.ToString(),
                ["Value"] = Prop.Value
            }).ToList()
        };
    }
}

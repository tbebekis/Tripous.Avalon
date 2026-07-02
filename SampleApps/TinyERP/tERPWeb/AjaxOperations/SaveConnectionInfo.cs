/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Saves the default database connection information.
/// </summary>
[AjaxOperation("App.SaveConnectionInfo")]
public class SaveConnectionInfo: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        string Name = GetStringParam(Request, "Name");
        DbServerType ServerType = GetDbServerTypeParam(Request, ConnectionInfo.DbServerType);
        DbConAdapter Adapter = DbConAdapters.Get(ServerType);
        List<DbConProp> Props = GetConnectionProps(Request, Adapter);
        string Message = ValidateConnectionInfo(Name, Adapter, Props);
        AjaxResponse Result = new(Request.OperationName);

        if (!string.IsNullOrWhiteSpace(Message))
        {
            Result["Success"] = false;
            Result["Message"] = Message;
            return Result;
        }

        try
        {
            ConnectionInfo.Name = Name;
            ConnectionInfo.DbServerType = ServerType;
            ConnectionInfo.CommandTimeoutSeconds = GetCommandTimeoutParam(Request, ConnectionInfo.CommandTimeoutSeconds);
            ConnectionInfo.ConnectionString = Adapter.Construct(Props);
            Db.Connections.Save();
            Result["Success"] = true;
            Result["Message"] = "Connection information saved.";
            Result["ConnectionString"] = ConnectionInfo.ConnectionString;
        }
        catch (Exception e)
        {
            Result["Success"] = false;
            Result["Message"] = e.Message;
        }

        return Result;
    }
}

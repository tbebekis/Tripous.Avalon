/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Tests the default database connection information.
/// </summary>
[AjaxOperation("App.TestConnectionInfo")]
public class TestConnectionInfo: AppAjaxOperation
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
            string ConnectionString = Adapter.Construct(Props);
            SqlProvider Provider = SqlProviders.GetSqlProvider(ServerType);
            Provider.CanConnect(ConnectionString, true);
            Result["Success"] = true;
            Result["Message"] = "Connection succeeded.";
            Result["ConnectionString"] = ConnectionString;
        }
        catch (Exception e)
        {
            Result["Success"] = false;
            Result["Message"] = e.Message;
        }

        return Result;
    }
}

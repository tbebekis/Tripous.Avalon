/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Connection string adapter for Oracle databases.
/// </summary>
public class OracleConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Does nothing because Oracle writes the server as part of the Data Source value.
    /// </summary>
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
    }
    /// <summary>
    /// Does nothing because Oracle writes the port as part of the Data Source value.
    /// </summary>
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    /// <summary>
    /// Writes the Oracle Data Source value using server, port and service name when available.
    /// </summary>
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        var server = Find(Props, DbConPropType.Server);
        var port = Find(Props, DbConPropType.Port);
        var service = Find(Props, DbConPropType.Database);
        if (!string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(service))
            Add(Parts, "Data Source", string.IsNullOrWhiteSpace(port) ? "//" + server + "/" + service : "//" + server + ":" + port + "/" + service);
        else
            Add(Parts, "Data Source", service);
    }
    
    /// <summary>
    /// Reads the Oracle server from the Data Source value.
    /// </summary>
    protected override void ReadServer(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        var hostPart = slashIndex >= 0 ? dataSource.Substring(0, slashIndex) : dataSource;
        var colonIndex = hostPart.LastIndexOf(':');
        if (colonIndex > 0)
            Add(Props, DbConPropType.Server, hostPart.Substring(0, colonIndex));
        else
            Add(Props, DbConPropType.Server, hostPart);
    }
    /// <summary>
    /// Reads the Oracle port from the Data Source value.
    /// </summary>
    protected override void ReadPort(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        var hostPart = slashIndex >= 0 ? dataSource.Substring(0, slashIndex) : dataSource;
        var colonIndex = hostPart.LastIndexOf(':');
        if (colonIndex > 0 && colonIndex < hostPart.Length - 1)
            Add(Props, DbConPropType.Port, hostPart.Substring(colonIndex + 1));
    }
    /// <summary>
    /// Reads the Oracle service name or data source value.
    /// </summary>
    protected override void ReadDatabase(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        if (slashIndex >= 0 && slashIndex < dataSource.Length - 1)
            Add(Props, DbConPropType.Database, dataSource.Substring(slashIndex + 1));
        else
            Add(Props, DbConPropType.Database, dataSource);
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.Oracle;
    /// <summary>
    /// Gets the connection property definitions supported by Oracle.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server" },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "1521" },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Service Name / Data Source", IsRequired = true, Aliases = ["Data Source"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User Id", IsRequired = true, Aliases = ["User Id"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password"] },
        new DbConPropDef { PropType = DbConPropType.IntegratedSecurity, Label = "Integrated Security", Aliases = ["Integrated Security"], ValidValues = ["True", "False"] }
    ];
}
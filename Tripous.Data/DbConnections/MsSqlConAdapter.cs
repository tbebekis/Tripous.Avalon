/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Connection string adapter for Microsoft SQL Server databases.
/// </summary>
public class MsSqlConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Writes the server and port using the SQL Server Data Source syntax.
    /// </summary>
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        var server = Find(Props, DbConPropType.Server);
        var port = Find(Props, DbConPropType.Port);
        if (string.IsNullOrWhiteSpace(server))
            return;
        Add(Parts, "Data Source", string.IsNullOrWhiteSpace(port) ? server : server + "," + port);
    }
    /// <summary>
    /// Does nothing because SQL Server stores the port as part of the Data Source value.
    /// </summary>
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    /// <summary>
    /// Writes the database name using the Initial Catalog key.
    /// </summary>
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Initial Catalog", Find(Props, DbConPropType.Database));
    }
    /// <summary>
    /// Writes the TrustServerCertificate setting.
    /// </summary>
    protected override void WriteTrustServerCertificate(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "TrustServerCertificate", Find(Props, DbConPropType.TrustServerCertificate));
    }

    /// <summary>
    /// Reads the server name from the SQL Server Data Source value.
    /// </summary>
    protected override void ReadServer(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var value = Read(Dict, "Data Source", "Server");
        if (string.IsNullOrWhiteSpace(value))
            return;
        var index = value.LastIndexOf(',');
        Add(Props, DbConPropType.Server, index > 0 ? value.Substring(0, index) : value);
    }
    /// <summary>
    /// Reads the port from the SQL Server Data Source value.
    /// </summary>
    protected override void ReadPort(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var value = Read(Dict, "Data Source", "Server");
        if (string.IsNullOrWhiteSpace(value))
            return;
        var index = value.LastIndexOf(',');
        if (index > 0 && index < value.Length - 1)
            Add(Props, DbConPropType.Port, value.Substring(index + 1));
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.MsSql;
    /// <summary>
    /// Gets the connection property definitions supported by SQL Server.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server", IsRequired = true, Aliases = ["Server", "Data Source"] },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "1433", Aliases = ["Port"] },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Database", IsRequired = true, Aliases = ["Database", "Initial Catalog"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User Id", Aliases = ["User Id", "UID"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password", "Pwd"] },
        new DbConPropDef { PropType = DbConPropType.IntegratedSecurity, Label = "Integrated Security", Aliases = ["Integrated Security", "Trusted_Connection"], ValidValues = ["True", "False"] },
        new DbConPropDef { PropType = DbConPropType.TrustServerCertificate, Label = "Trust Server Certificate", Aliases = ["TrustServerCertificate"], ValidValues = ["True", "False"] }
    ];
}
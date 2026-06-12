/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Connection string adapter for PostgreSQL databases.
/// </summary>
public class PostgreSqlConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Writes the server value using the PostgreSQL Host key.
    /// </summary>
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Host", Find(Props, DbConPropType.Server));
    }
    /// <summary>
    /// Writes the user id value using the PostgreSQL Username key.
    /// </summary>
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Username", Find(Props, DbConPropType.UserId));
    }
    /// <summary>
    /// Writes the SSL mode setting.
    /// </summary>
    protected override void WriteSslMode(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "SSL Mode", Find(Props, DbConPropType.SslMode));
    }
    /// <summary>
    /// Writes the trust server certificate setting.
    /// </summary>
    protected override void WriteTrustServerCertificate(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Trust Server Certificate", Find(Props, DbConPropType.TrustServerCertificate));
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.PostgreSql;
    /// <summary>
    /// Gets the connection property definitions supported by PostgreSQL.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Host", IsRequired = true, Aliases = ["Host", "Server"] },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "5432", Aliases = ["Port"] },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Database", IsRequired = true, Aliases = ["Database"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "Username", IsRequired = true, Aliases = ["Username", "User Id", "User"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password", "Pwd"] },
        new DbConPropDef { PropType = DbConPropType.SslMode, Label = "SSL Mode", Aliases = ["SSL Mode", "SslMode"], ValidValues = ["Disable", "Prefer", "Require", "VerifyCA", "VerifyFull"] },
        new DbConPropDef { PropType = DbConPropType.TrustServerCertificate, Label = "Trust Server Certificate", Aliases = ["Trust Server Certificate"], ValidValues = ["True", "False"] }
    ];
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Connection string adapter for MySQL databases.
/// </summary>
public class MySqlConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Writes the user id using the MySQL Uid key.
    /// </summary>
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Uid", Find(Props, DbConPropType.UserId));
    }
    /// <summary>
    /// Writes the SSL mode setting.
    /// </summary>
    protected override void WriteSslMode(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "SslMode", Find(Props, DbConPropType.SslMode));
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.MySql;
    /// <summary>
    /// Gets the connection property definitions supported by MySQL.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server", IsRequired = true, Aliases = ["Server", "Host"] },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "3306", Aliases = ["Port"] },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Database", IsRequired = true, Aliases = ["Database"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User Id", IsRequired = true, Aliases = ["User Id", "UID", "User"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password", "Pwd"] },
        new DbConPropDef { PropType = DbConPropType.SslMode, Label = "SSL Mode", Aliases = ["SslMode"], ValidValues = ["None", "Preferred", "Required", "VerifyCA", "VerifyFull"] },
        new DbConPropDef { PropType = DbConPropType.Charset, Label = "Charset", Aliases = ["Charset"] }
    ];
}
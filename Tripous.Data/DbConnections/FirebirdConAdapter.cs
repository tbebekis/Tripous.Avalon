/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Connection string adapter for Firebird databases.
/// </summary>
public class FirebirdConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Writes the server value using the Firebird DataSource key.
    /// </summary>
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "DataSource", Find(Props, DbConPropType.Server));
    }
    /// <summary>
    /// Writes the user id value using the Firebird User key.
    /// </summary>
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "User", Find(Props, DbConPropType.UserId));
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.Firebird;
    /// <summary>
    /// Gets the connection property definitions supported by Firebird.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server", IsRequired = true, Aliases = ["DataSource", "Server"] },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "3050", Aliases = ["Port"] },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Database", IsRequired = true, Aliases = ["Database"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User", IsRequired = true, Aliases = ["User", "User Id", "UID"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password", "Pwd"] },
        new DbConPropDef { PropType = DbConPropType.Charset, Label = "Charset", DefaultValue = "UTF8", Aliases = ["Charset"] }
    ];
}
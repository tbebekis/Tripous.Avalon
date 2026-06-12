/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// Connection string adapter for SQLite databases.
/// </summary>
public class SqliteConAdapter : DbConAdapter
{
    // ● protected methods
    /// <summary>
    /// Does nothing because SQLite does not use a server value.
    /// </summary>
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
    }
    /// <summary>
    /// Does nothing because SQLite does not use a port value.
    /// </summary>
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    /// <summary>
    /// Writes the database file path using the Data Source key.
    /// </summary>
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Data Source", Find(Props, DbConPropType.Database));
    }

    /// <summary>
    /// Reads the database file path from the Data Source or Database key.
    /// </summary>
    protected override void ReadDatabase(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        string FilePath = Read(Dict, "Data Source", "Database");
        Add(Props, DbConPropType.Database, FilePath);
    }

    // ● properties
    /// <summary>
    /// Gets the database server type handled by this adapter.
    /// </summary>
    public override DbServerType ServerType => DbServerType.Sqlite;
    /// <summary>
    /// Gets the connection property definitions supported by SQLite.
    /// </summary>
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Database, Label = "File Path", IsRequired = true, Aliases = ["Data Source", "Database"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password"] }
    ];
}
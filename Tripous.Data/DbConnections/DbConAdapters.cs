/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;
 
 
/// <summary>
/// Registry of connection string adapters used for building and parsing connection strings.
/// </summary>
static public class DbConAdapters
{
    // ● private fields
    static private readonly Dictionary<DbServerType, DbConAdapter> fMap = new Dictionary<DbServerType, DbConAdapter>
    {
        { DbServerType.MsSql, new MsSqlConAdapter() },
        { DbServerType.MySql, new MySqlConAdapter() },
        { DbServerType.PostgreSql, new PostgreSqlConAdapter() },
        { DbServerType.Firebird, new FirebirdConAdapter() },
        { DbServerType.Oracle, new OracleConAdapter() },
        { DbServerType.Sqlite, new SqliteConAdapter() },
    };

    // ● static public methods
    /// <summary>
    /// Returns the connection string adapter for the specified server type.
    /// </summary>
    static public DbConAdapter Get(DbServerType ServerType)
    {
        return fMap[ServerType];
    }
    /// <summary>
    /// Returns all registered connection string adapters.
    /// </summary>
    static public DbConAdapter[] GetAll()
    {
        return fMap.Values.ToArray();
    }
}
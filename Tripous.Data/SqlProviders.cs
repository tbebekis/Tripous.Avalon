/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// A collection of SqlProvider instances.
/// </summary>
static public class SqlProviders
{
    static readonly Lazy<SqlProviderMsSql> fMsSql = new(() => new SqlProviderMsSql());
    static readonly Lazy<SqlProviderMySql> fMySql = new(() => new SqlProviderMySql());
    static readonly Lazy<SqlProviderFirebird> fFirebird = new(() => new SqlProviderFirebird());
    static readonly Lazy<SqlProviderSqlite> fSqlite = new(() => new SqlProviderSqlite());
    static readonly Lazy<SqlProviderPostgreSql> fPostgreSql = new(() => new SqlProviderPostgreSql());
    static readonly Lazy<SqlProviderOracle> fOracle = new(() => new SqlProviderOracle());

    /// <summary>
    /// Gets the SqlProvider instance for the specified DbServerType.
    /// </summary>
    static public SqlProvider GetSqlProvider(DbServerType DbServerType)
    {
        switch (DbServerType)
        {
            case DbServerType.MsSql: return MsSql;
            case DbServerType.MySql: return MySql;
            case DbServerType.Firebird: return Firebird;
            case DbServerType.Sqlite: return Sqlite;
            case DbServerType.PostgreSql: return PostgreSql;
            case DbServerType.Oracle: return Oracle;
        }

        throw new Exception($"Unsupported DbServerType: {DbServerType}");
    }

    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderMsSql MsSql => fMsSql.Value;
    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderMySql MySql => fMySql.Value;
    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderFirebird Firebird => fFirebird.Value;
    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderSqlite Sqlite => fSqlite.Value;
    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderPostgreSql PostgreSql => fPostgreSql.Value;
    /// <summary>
    /// Gets a SqlProvider instance  
    /// </summary>
    static public SqlProviderOracle Oracle => fOracle.Value;
}
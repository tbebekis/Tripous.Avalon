/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Indicates the type of the database server.
/// </summary>
public enum DbServerType
{
    /// <summary>
    /// Microsoft SQL Server.
    /// </summary>
    MsSql,
    /// <summary>
    /// MySQL.
    /// </summary>
    MySql,
    /// <summary>
    /// PostgreSQL.
    /// </summary>
    PostgreSql,
    /// <summary>
    /// Firebird.
    /// </summary>
    Firebird,
    /// <summary>
    /// Oracle.
    /// </summary>
    Oracle,
    /// <summary>
    /// SQLite.
    /// </summary>
    Sqlite, 
}

/// <summary>
/// Extension methods.
/// </summary>
static public class DbServerTypeHelper
{
    /// <summary>
    /// Returns a template connection string for the specified <paramref name="DbServerType"/>, with placeholders to be filled in by the caller.
    /// </summary>
    static public string GetTemplateConnectionString(this DbServerType DbServerType)
    {
        string Template = "";
        switch (DbServerType)
        {
            case DbServerType.MsSql: Template = @"Data Source={0}; Initial Catalog={1}; User ID=sa; Password={2}; TrustServerCertificate=true;"; break;
            case DbServerType.MySql: Template = @"Server={0}; Database={1}; User Id=root; Password={2};";break;
            case DbServerType.PostgreSql: Template = @"Server={0}; Database={1}; User Id={2}; Password={3};"; break;
            case DbServerType.Firebird: Template = @"DataSource={0}; Database={1}; User=SYSDBA; Password=masterkey Charset=UTF8;"; break;
            case DbServerType.Oracle: Template = @"Data Source={0}; User Id={1}; Password={2};"; break;
            case DbServerType.Sqlite: Template = @"Data Source=""{0}"""; break;
            //case DbServerType.Odbc: Template = @"Driver={0}; Server={1}; DataBase={2}; Uid={3}; Pwd={4}; Trusted_Connection=Yes;"; break;
        }
        return Template;
    }
    /// <summary>
    /// Returns the provider invariant name.
    /// </summary>
    static public string GetProviderInvariantName(this DbServerType DbServerType) => DbServerType.GetFactory().GetType().Namespace;
    /// <summary>
    /// Returns the <see cref="DbProviderFactory"/> associated with the specified <paramref name="DbServerType"/>.
    /// </summary>
    static public DbProviderFactory GetFactory(this DbServerType DbServerType)
    {
        switch (DbServerType)
        {
            case DbServerType.MsSql: return Microsoft.Data.SqlClient.SqlClientFactory.Instance;  
            case DbServerType.MySql: return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
            case DbServerType.PostgreSql: return Npgsql.NpgsqlFactory.Instance;
            case DbServerType.Firebird: return FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance;
            case DbServerType.Oracle: return Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance;  
            case DbServerType.Sqlite: return System.Data.SQLite.SQLiteFactory.Instance;
            default: throw new Exception($"Unsupported DbType: {DbServerType}");
        }
    }
}
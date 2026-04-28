namespace Tripous.Data;

static public class SqlProviders
{
    static readonly Lazy<SqlProviderMsSql> fMsSql = new(() => new SqlProviderMsSql());
    static readonly Lazy<SqlProviderMySql> fMySql = new(() => new SqlProviderMySql());
    static readonly Lazy<SqlProviderFirebird> fFirebird = new(() => new SqlProviderFirebird());
    static readonly Lazy<SqlProviderSqlite> fSqlite = new(() => new SqlProviderSqlite());
    static readonly Lazy<SqlProviderPostgreSql> fPostgreSql = new(() => new SqlProviderPostgreSql());
    static readonly Lazy<SqlProviderOracle> fOracle = new(() => new SqlProviderOracle());

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

    static public SqlProviderMsSql MsSql => fMsSql.Value;
    static public SqlProviderMySql MySql => fMySql.Value;
    static public SqlProviderFirebird Firebird => fFirebird.Value;
    static public SqlProviderSqlite Sqlite => fSqlite.Value;
    static public SqlProviderPostgreSql PostgreSql => fPostgreSql.Value;
    static public SqlProviderOracle Oracle => fOracle.Value;
}
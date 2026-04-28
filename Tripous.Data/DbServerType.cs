namespace Tripous.Data;

public enum DbServerType
{
    MsSql,
    MySql,
    PostgreSql,
    Firebird,
    Oracle,
    Sqlite, 
}

static public class DbServerTypeHelper
{
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
    /*
    {
        return DbServerType.GetFactory().GetType().Namespace;
        switch (DbServerType)
        {
            case DbServerType.MsSql: return typeof(Microsoft.Data.SqlClient.SqlClientFactory).Namespace;  
            case DbServerType.MySql: return typeof(MySql.Data.MySqlClient.MySqlClientFactory).Namespace;
            case DbServerType.PostgreSql: return typeof(Npgsql.NpgsqlFactory).Namespace;
            case DbServerType.Firebird: return typeof(FirebirdSql.Data.FirebirdClient.FirebirdClientFactory).Namespace;
            case DbServerType.Oracle: return typeof(Oracle.ManagedDataAccess.Client.OracleClientFactory).Namespace;  
            case DbServerType.Sqlite: return typeof(System.Data.SQLite.SQLiteFactory).Namespace;
            default: throw new Exception($"Unsupported DbType: {DbServerType}");
        }
    }
    */

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
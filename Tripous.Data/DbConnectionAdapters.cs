namespace Tripous.Data;

public enum DbConPropType
{
    Server,
    Port,
    Database,
    UserId,
    Password,
    IntegratedSecurity,
    TrustServerCertificate,
    SslMode,
    Charset
}

// ● models
public class DbConProp
{
    // ● public
    public override string ToString() => $"{PropType}: {Value}";

    // ● properties
    public DbConPropType PropType { get; set; }
    public string Value { get; set; } = "";
}
public class DbConPropDef
{
    // ● properties
    public DbConPropType PropType { get; set; }
    public string Label { get; set; } = "";
    public bool IsRequired { get; set; }
    public string DefaultValue { get; set; } = "";
    public string[] Aliases { get; set; } = [];
    public string[] ValidValues { get; set; } = [];
}

static public class DbConPropExtensions
{
    static public bool HasProp(this List<DbConProp> List, DbConPropType PropType) => Find(List, PropType) != null && !string.IsNullOrEmpty(Find(List, PropType).Value);
    static public DbConProp Find(this List<DbConProp> List, DbConPropType PropType) => List.FirstOrDefault(x => x.PropType == PropType);
    static public DbConProp Get(this List<DbConProp> List, DbConPropType PropType)
    {
        DbConProp Prop = List.FirstOrDefault(x => x.PropType == PropType);
        if (Prop == null)
            throw new ApplicationException($"Connection string property not found: {PropType}");
        return Prop;
    }

    static public string GetValue(this List<DbConProp> List, DbConPropType PropType) => Get(List, PropType).Value;
    static public void SetValue(this List<DbConProp> List, DbConPropType PropType, string Value) => Get(List, PropType).Value = Value;
}

// ● base
public abstract class DbConAdapter
{
    // ● private fields
    static private readonly char[] fSemiColonSeparator = [';'];

    // ● protected methods
    protected string Find(List<DbConProp> Props, DbConPropType Type)
    {
        var prop = Props.FirstOrDefault(item => item.PropType == Type);
        return prop == null ? "" : prop.Value;
    }
    protected void Add(List<string> Parts, string Name, string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value))
            Parts.Add(Name + "=" + Value);
    }
    protected void Add(List<DbConProp> Props, DbConPropType Type, string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value))
            Props.Add(new DbConProp { PropType = Type, Value = Value });
    }
    protected bool Contains(Dictionary<string, string> Dict, params string[] Names)
    {
        foreach (var name in Names)
        {
            if (Dict.ContainsKey(name))
                return true;
        }
        return false;
    }
    protected string Read(Dictionary<string, string> Dict, params string[] Names)
    {
        foreach (var name in Names)
        {
            if (Dict.TryGetValue(name, out var value))
                return value;
        }
        return "";
    }
    protected Dictionary<string, string> ParseToDictionary(string ConnectionString)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parts = ConnectionString.Split(fSemiColonSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var index = part.IndexOf('=');
            if (index <= 0)
                continue;
            var name = part.Substring(0, index).Trim();
            var value = part.Substring(index + 1).Trim();
            if (!string.IsNullOrWhiteSpace(name))
                result[name] = value;
        }
        return result;
    }
    protected virtual void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Server", Find(Props, DbConPropType.Server));
    }
    protected virtual void WritePort(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Port", Find(Props, DbConPropType.Port));
    }
    protected virtual void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Database", Find(Props, DbConPropType.Database));
    }
    protected virtual void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "User Id", Find(Props, DbConPropType.UserId));
    }
    protected virtual void WritePassword(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Password", Find(Props, DbConPropType.Password));
    }
    protected virtual void WriteIntegratedSecurity(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Integrated Security", Find(Props, DbConPropType.IntegratedSecurity));
    }
    protected virtual void WriteTrustServerCertificate(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Trust Server Certificate", Find(Props, DbConPropType.TrustServerCertificate));
    }
    protected virtual void WriteSslMode(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "SslMode", Find(Props, DbConPropType.SslMode));
    }
    protected virtual void WriteCharset(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Charset", Find(Props, DbConPropType.Charset));
    }
    protected virtual void ReadServer(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.Server, Read(Dict, "Server", "Data Source", "DataSource", "Host"));
    }
    protected virtual void ReadPort(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.Port, Read(Dict, "Port"));
    }
    protected virtual void ReadDatabase(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.Database, Read(Dict, "Database", "Initial Catalog", "Data Source"));
    }
    protected virtual void ReadUserId(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.UserId, Read(Dict, "User Id", "UID", "User", "Username"));
    }
    protected virtual void ReadPassword(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.Password, Read(Dict, "Password", "Pwd"));
    }
    protected virtual void ReadIntegratedSecurity(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.IntegratedSecurity, Read(Dict, "Integrated Security", "Trusted_Connection"));
    }
    protected virtual void ReadTrustServerCertificate(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.TrustServerCertificate, Read(Dict, "Trust Server Certificate", "TrustServerCertificate"));
    }
    protected virtual void ReadSslMode(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.SslMode, Read(Dict, "SslMode", "SSL Mode"));
    }
    protected virtual void ReadCharset(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        Add(Props, DbConPropType.Charset, Read(Dict, "Charset", "Character Set"));
    }

    // ● public methods
    public string Construct(List<DbConProp> Props)
    {
        var parts = new List<string>();
        if (IsValid(DbConPropType.Server))
            WriteServer(parts, Props);
        if (IsValid(DbConPropType.Port))
            WritePort(parts, Props);
        if (IsValid(DbConPropType.Database))
            WriteDatabase(parts, Props);
        if (IsValid(DbConPropType.UserId))
            WriteUserId(parts, Props);
        if (IsValid(DbConPropType.Password))
            WritePassword(parts, Props);
        if (IsValid(DbConPropType.IntegratedSecurity))
            WriteIntegratedSecurity(parts, Props);
        if (IsValid(DbConPropType.TrustServerCertificate))
            WriteTrustServerCertificate(parts, Props);
        if (IsValid(DbConPropType.SslMode))
            WriteSslMode(parts, Props);
        if (IsValid(DbConPropType.Charset))
            WriteCharset(parts, Props);
        return string.Join(";", parts);
    }
    public List<DbConProp> Parse(string ConnectionString)
    {
        var result = new List<DbConProp>();
        var dict = ParseToDictionary(ConnectionString);
        if (IsValid(DbConPropType.Server))
            ReadServer(dict, result);
        if (IsValid(DbConPropType.Port))
            ReadPort(dict, result);
        if (IsValid(DbConPropType.Database))
            ReadDatabase(dict, result);
        if (IsValid(DbConPropType.UserId))
            ReadUserId(dict, result);
        if (IsValid(DbConPropType.Password))
            ReadPassword(dict, result);
        if (IsValid(DbConPropType.IntegratedSecurity))
            ReadIntegratedSecurity(dict, result);
        if (IsValid(DbConPropType.TrustServerCertificate))
            ReadTrustServerCertificate(dict, result);
        if (IsValid(DbConPropType.SslMode))
            ReadSslMode(dict, result);
        if (IsValid(DbConPropType.Charset))
            ReadCharset(dict, result);
        return result;
    }
    public bool IsValid(DbConPropType Type)
    {
        return PropDefs.Any(item => item.PropType == Type);
    }


    // ● properties
    public abstract DbServerType ServerType { get; }
    public abstract DbConPropDef[] PropDefs { get; }
}

// ● adapters
public class MsSqlAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        var server = Find(Props, DbConPropType.Server);
        var port = Find(Props, DbConPropType.Port);
        if (string.IsNullOrWhiteSpace(server))
            return;
        Add(Parts, "Data Source", string.IsNullOrWhiteSpace(port) ? server : server + "," + port);
    }
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Initial Catalog", Find(Props, DbConPropType.Database));
    }
    protected override void WriteTrustServerCertificate(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "TrustServerCertificate", Find(Props, DbConPropType.TrustServerCertificate));
    }
    protected override void ReadServer(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var value = Read(Dict, "Data Source", "Server");
        if (string.IsNullOrWhiteSpace(value))
            return;
        var index = value.LastIndexOf(',');
        Add(Props, DbConPropType.Server, index > 0 ? value.Substring(0, index) : value);
    }
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
    public override DbServerType ServerType => DbServerType.MsSql;
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
public class MySqlAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Uid", Find(Props, DbConPropType.UserId));
    }
    protected override void WriteSslMode(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "SslMode", Find(Props, DbConPropType.SslMode));
    }

    // ● properties
    public override DbServerType ServerType => DbServerType.MySql;
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
public class PostgreSqlAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Host", Find(Props, DbConPropType.Server));
    }
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Username", Find(Props, DbConPropType.UserId));
    }
    protected override void WriteSslMode(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "SSL Mode", Find(Props, DbConPropType.SslMode));
    }
    protected override void WriteTrustServerCertificate(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Trust Server Certificate", Find(Props, DbConPropType.TrustServerCertificate));
    }

    // ● properties
    public override DbServerType ServerType => DbServerType.PostgreSql;
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
public class FirebirdAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "DataSource", Find(Props, DbConPropType.Server));
    }
    protected override void WriteUserId(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "User", Find(Props, DbConPropType.UserId));
    }

    // ● properties
    public override DbServerType ServerType => DbServerType.Firebird;
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server", IsRequired = true, Aliases = ["DataSource", "Server"] },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "3050", Aliases = ["Port"] },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Database", IsRequired = true, Aliases = ["Database"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User", IsRequired = true, Aliases = ["User", "User Id", "UID"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password", "Pwd"] },
        new DbConPropDef { PropType = DbConPropType.Charset, Label = "Charset", DefaultValue = "UTF8", Aliases = ["Charset"] }
    ];
}
public class OracleAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
    }
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        var server = Find(Props, DbConPropType.Server);
        var port = Find(Props, DbConPropType.Port);
        var service = Find(Props, DbConPropType.Database);
        if (!string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(service))
            Add(Parts, "Data Source", string.IsNullOrWhiteSpace(port) ? "//" + server + "/" + service : "//" + server + ":" + port + "/" + service);
        else
            Add(Parts, "Data Source", service);
    }
    protected override void ReadServer(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        var hostPart = slashIndex >= 0 ? dataSource.Substring(0, slashIndex) : dataSource;
        var colonIndex = hostPart.LastIndexOf(':');
        if (colonIndex > 0)
            Add(Props, DbConPropType.Server, hostPart.Substring(0, colonIndex));
        else
            Add(Props, DbConPropType.Server, hostPart);
    }
    protected override void ReadPort(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        var hostPart = slashIndex >= 0 ? dataSource.Substring(0, slashIndex) : dataSource;
        var colonIndex = hostPart.LastIndexOf(':');
        if (colonIndex > 0 && colonIndex < hostPart.Length - 1)
            Add(Props, DbConPropType.Port, hostPart.Substring(colonIndex + 1));
    }
    protected override void ReadDatabase(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        var dataSource = Read(Dict, "Data Source");
        var slashIndex = dataSource.IndexOf('/');
        if (slashIndex >= 0 && slashIndex < dataSource.Length - 1)
            Add(Props, DbConPropType.Database, dataSource.Substring(slashIndex + 1));
        else
            Add(Props, DbConPropType.Database, dataSource);
    }

    // ● properties
    public override DbServerType ServerType => DbServerType.Oracle;
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Server, Label = "Server" },
        new DbConPropDef { PropType = DbConPropType.Port, Label = "Port", DefaultValue = "1521" },
        new DbConPropDef { PropType = DbConPropType.Database, Label = "Service Name / Data Source", IsRequired = true, Aliases = ["Data Source"] },
        new DbConPropDef { PropType = DbConPropType.UserId, Label = "User Id", IsRequired = true, Aliases = ["User Id"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password"] },
        new DbConPropDef { PropType = DbConPropType.IntegratedSecurity, Label = "Integrated Security", Aliases = ["Integrated Security"], ValidValues = ["True", "False"] }
    ];
}
public class SqliteAdapter : DbConAdapter
{
    // ● protected methods
    protected override void WriteServer(List<string> Parts, List<DbConProp> Props)
    {
    }
    protected override void WritePort(List<string> Parts, List<DbConProp> Props)
    {
    }
    protected override void WriteDatabase(List<string> Parts, List<DbConProp> Props)
    {
        Add(Parts, "Data Source", Find(Props, DbConPropType.Database));
    }
    protected override void ReadDatabase(Dictionary<string, string> Dict, List<DbConProp> Props)
    {
        string FilePath = Read(Dict, "Data Source", "Database");
        Add(Props, DbConPropType.Database, FilePath);
    }

    // ● properties
    public override DbServerType ServerType => DbServerType.Sqlite;
    public override DbConPropDef[] PropDefs => [
        new DbConPropDef { PropType = DbConPropType.Database, Label = "File Path", IsRequired = true, Aliases = ["Data Source", "Database"] },
        new DbConPropDef { PropType = DbConPropType.Password, Label = "Password", Aliases = ["Password"] }
    ];
}

// ● registry
static public class DbConAdapters
{
    // ● private fields
    static private readonly Dictionary<DbServerType, DbConAdapter> fMap = new Dictionary<DbServerType, DbConAdapter>
    {
        { DbServerType.MsSql, new MsSqlAdapter() },
        { DbServerType.MySql, new MySqlAdapter() },
        { DbServerType.PostgreSql, new PostgreSqlAdapter() },
        { DbServerType.Firebird, new FirebirdAdapter() },
        { DbServerType.Oracle, new OracleAdapter() },
        { DbServerType.Sqlite, new SqliteAdapter() },
    };

    // ● static public methods
    static public DbConAdapter Get(DbServerType ServerType)
    {
        return fMap[ServerType];
    }
    static public DbConAdapter[] GetAll()
    {
        return fMap.Values.ToArray();
    }
}
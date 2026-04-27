namespace Tripous.Data;

/// <summary>
/// A connection string builder.
/// </summary>
public class ConnectionStringBuilder : DbConnectionStringBuilder
{
    // ● private
    static string RemoveTrailingSlash(string Path)
    {
        return Path.TrimEnd('\\', '/');
    }

    // ● constructor
    public ConnectionStringBuilder()
    {
    }
    public ConnectionStringBuilder(string ConnectionString)
    {
        this.ConnectionString = ConnectionString;
    }
    public ConnectionStringBuilder(bool UseOdbcRules)
        : base(UseOdbcRules)
    {
    }
    public ConnectionStringBuilder(bool UseOdbcRules, string ConnectionString)
        : base(UseOdbcRules)
    {
        this.ConnectionString = ConnectionString;
    }

    // ● static public
    public const string AliasKey = "Alias";

    static public string NormalizeConnectionString(string ConnectionString)
    {
        return ReplacePathPlaceholders(RemoveAliasEntry(ConnectionString));
    }
    static public string ReplacePathPlaceholders(string ConnectionString)
    {
        string Result = ConnectionString;
        Result = Result.Replace("[AppPath]", RemoveTrailingSlash(SysConfig.AppFolderPath));
        Result = Result.Replace("[Data]", RemoveTrailingSlash(SysConfig.AppDataFolderPath));
        Result = Result.Replace("[BackUp]", RemoveTrailingSlash(SysConfig.AppDataFolderPath) + Path.DirectorySeparatorChar + "BackUp");
        
        if (Path.DirectorySeparatorChar == '/')
            Result = Result.Replace('\\', '/');
        else
            Result = Result.Replace('/', '\\');
        
        return Result;
    }
    static public void ExtractAlias(string Input, ref string Alias, ref string ConnectionString)
    {
        Alias = string.Empty;
        ConnectionString = string.Empty;

        if (string.IsNullOrWhiteSpace(Input))
            return;

        ConnectionStringBuilder Builder = new ConnectionStringBuilder(Input);

        if (Builder.ContainsKey(AliasKey))
        {
            Alias = Builder[AliasKey].ToString();
            Builder.Remove(AliasKey);
        }

        ConnectionString = Builder.ConnectionString;
    }
    static public string GetAlias(string ConnectionString)
    {
        string Alias = string.Empty;
        string Cs = string.Empty;
        ExtractAlias(ConnectionString, ref Alias, ref Cs);
        return Alias;
    }
    static public string RemoveAliasEntry(string ConnectionString)
    {
        string Alias = string.Empty;
        string Cs = string.Empty;
        ExtractAlias(ConnectionString, ref Alias, ref Cs);
        return Cs;
    }

    // ● public
    public virtual ConnectionStringBuilder CreateConnectionStringBuilder(string ConnectionString)
    {
        ConnectionStringBuilder Result = new ConnectionStringBuilder(ConnectionString);
        if (string.IsNullOrEmpty(ConnectionString) || !Result.ContainsKey(AliasKey))
            Result.Alias = Alias;
        return Result;
    }
    public void SetConnectionString(string ConnectionString)
    {
        this.ConnectionString = ConnectionString;
    }
    public bool TryGetValue(string Key, out string Value)
    {
        Value = string.Empty;

        if (!string.IsNullOrWhiteSpace(Key) && ContainsKey(Key))
        {
            Value = this[Key].ToString();
            return true;
        }

        return false;
    }
    public string GetFirst(string[] Keys)
    {
        foreach (string Key in Keys)
        {
            if (TryGetValue(Key, out string Value) && !string.IsNullOrWhiteSpace(Value))
                return Value;
        }

        return string.Empty;
    }
    public void RemoveKeys(string[] Keys)
    {
        foreach (string Key in Keys)
        {
            if (ContainsKey(Key))
                Remove(Key);
        }
    }
    public DataTable ToDataTable()
    {
        DataTable Result = new DataTable();
        Result.Columns.Add("Key");
        Result.Columns.Add("Value");
        Result.DefaultView.Sort = "Key";
        ToDataTable(Result);
        return Result;
    }
    public void ToDataTable(DataTable Table)
    {
        if (Table.Columns.Count == 0)
        {
            Table.Columns.Add("Key");
            Table.Columns.Add("Value");
            Table.DefaultView.Sort = "Key";
        }

        Table.Rows.Clear();

        foreach (string Key in Keys)
            Table.Rows.Add(Key, this[Key].ToString());
    }
    public void FromDataTable(DataTable Table)
    {
        Clear();

        foreach (DataRow Row in Table.Rows)
            this[Row.AsString("Key")] = Row.AsString("Value");
    }

    // ● properties
    public override object this[string Key]
    {
        get => ContainsKey(Key) ? base[Key] : string.Empty;
        set => base[Key] = value;
    }
    public string Alias
    {
        get { TryGetValue(AliasKey, out string S); return S; }
        set => this[AliasKey] = value;
    }
    public string User
    {
        get
        {
            if (TryGetValue("User", out string S)) return S;
            if (TryGetValue("UserId", out S)) return S;
            if (TryGetValue("User ID", out S)) return S;
            if (TryGetValue("UID", out S)) return S;
            return string.Empty;
        }
    }
    public string Password
    {
        get
        {
            if (TryGetValue("Password", out string S)) return S;
            if (TryGetValue("Psw", out S)) return S;
            return string.Empty;
        }
    }
    public string Database
    {
        get
        {
            if (TryGetValue("Initial Catalog", out string S) || TryGetValue("Database", out S))
                return S;

            if (TryGetValue("Data Source", out S))
                return S;

            return string.Empty;
        }
    }
    public string Server
    {
        get
        {
            if (TryGetValue("Initial Catalog", out string S))
            {
                if (TryGetValue("Data Source", out S))
                    return S;
                return "localhost";
            }
            else if (TryGetValue("Database", out S))
            {
                if (TryGetValue("DataSource", out S) || TryGetValue("Data Source", out S))
                    return S;
                return "localhost";
            }
            else if (TryGetValue("Data Source", out S) || TryGetValue("Server ", out S))
            {
                return S;
            }

            return string.Empty;
        }
    }
    public string OleDbProvider
    {
        get { TryGetValue("Provider", out string S); return S; }
        set => this["Provider"] = value;
    }
    public string ExtendedProperties
    {
        get { TryGetValue("Extended Properties", out string S); return S; }
        set => this["Extended Properties"] = value;
    }
    
   
 
}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides helper methods and strongly typed accessors for manipulating database connection strings.
/// </summary>
public class ConnectionStringBuilder : DbConnectionStringBuilder
{
    // ● private
    static string RemoveTrailingSlash(string Path)
    {
        return Path.TrimEnd('\\', '/');
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ConnectionStringBuilder()
    {
    }
    /// <summary>
    /// Initializes a new instance using the specified connection string.
    /// </summary>
    public ConnectionStringBuilder(string ConnectionString)
    {
        this.ConnectionString = ConnectionString;
    }
    /// <summary>
    /// Initializes a new instance using the specified ODBC parsing rules.
    /// </summary>
    public ConnectionStringBuilder(bool UseOdbcRules)
        : base(UseOdbcRules)
    {
    }
    /// <summary>
    /// Initializes a new instance using the specified ODBC parsing rules and connection string.
    /// </summary>
    public ConnectionStringBuilder(bool UseOdbcRules, string ConnectionString)
        : base(UseOdbcRules)
    {
        this.ConnectionString = ConnectionString;
    }

    // ● static public
    /// <summary>
    /// The connection string key used to store an alias.
    /// </summary>
    public const string AliasKey = "Alias";

    /// <summary>
    /// Removes the alias entry and expands any path placeholders.
    /// </summary>
    static public string NormalizeConnectionString(string ConnectionString)
    {
        return ReplacePathPlaceholders(RemoveAliasEntry(ConnectionString));
    }
    /// <summary>
    /// Replaces all supported path placeholders with physical paths.
    /// </summary>
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
    /// <summary>
    /// Extracts the alias and the remaining connection string.
    /// </summary>
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
    /// <summary>
    /// Returns the alias stored in a connection string.
    /// </summary>
    static public string GetAlias(string ConnectionString)
    {
        string Alias = string.Empty;
        string Cs = string.Empty;
        ExtractAlias(ConnectionString, ref Alias, ref Cs);
        return Alias;
    }
    /// <summary>
    /// Removes the alias entry from a connection string.
    /// </summary>
    static public string RemoveAliasEntry(string ConnectionString)
    {
        string Alias = string.Empty;
        string Cs = string.Empty;
        ExtractAlias(ConnectionString, ref Alias, ref Cs);
        return Cs;
    }

    // ● public
    /// <summary>
    /// Creates a new connection string builder initialized from the specified connection string.
    /// If no alias is present, the current alias is copied to the new builder.
    /// </summary>
    public virtual ConnectionStringBuilder CreateConnectionStringBuilder(string ConnectionString)
    {
        ConnectionStringBuilder Result = new ConnectionStringBuilder(ConnectionString);
        if (string.IsNullOrEmpty(ConnectionString) || !Result.ContainsKey(AliasKey))
            Result.Alias = Alias;
        return Result;
    }
    /// <summary>
    /// Sets the connection string.
    /// </summary>
    public void SetConnectionString(string ConnectionString)
    {
        this.ConnectionString = ConnectionString;
    }
    /// <summary>
    /// Tries to retrieve a value associated with the specified key.
    /// </summary>
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
    /// <summary>
    /// Returns the first non-empty value found among the specified keys.
    /// </summary>
    public string GetFirst(string[] Keys)
    {
        foreach (string Key in Keys)
        {
            if (TryGetValue(Key, out string Value) && !string.IsNullOrWhiteSpace(Value))
                return Value;
        }

        return string.Empty;
    }
    /// <summary>
    /// Removes all specified keys from the connection string.
    /// </summary>
    public void RemoveKeys(string[] Keys)
    {
        foreach (string Key in Keys)
        {
            if (ContainsKey(Key))
                Remove(Key);
        }
    }
    /// <summary>
    /// Converts the connection string entries to a new data table.
    /// </summary>
    public DataTable ToDataTable()
    {
        DataTable Result = new DataTable();
        Result.Columns.Add("Key");
        Result.Columns.Add("Value");
        Result.DefaultView.Sort = "Key";
        ToDataTable(Result);
        return Result;
    }
    /// <summary>
    /// Populates the specified data table with the connection string entries.
    /// </summary>
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
    /// <summary>
    /// Loads connection string entries from a data table.
    /// </summary>
    public void FromDataTable(DataTable Table)
    {
        Clear();

        foreach (DataRow Row in Table.Rows)
            this[Row.AsString("Key")] = Row.AsString("Value");
    }

    // ● properties
    /// <summary>
    /// Gets or sets a value associated with the specified key.
    /// Returns an empty string when the key does not exist.
    /// </summary>
    public override object this[string Key]
    {
        get => ContainsKey(Key) ? base[Key] : string.Empty;
        set => base[Key] = value;
    }
    /// <summary>
    /// Gets or sets the connection alias.
    /// </summary>
    public string Alias
    {
        get { TryGetValue(AliasKey, out string S); return S; }
        set => this[AliasKey] = value;
    }
    /// <summary>
    /// Gets the user name from the connection string.
    /// Supports common key variations such as User, UserId, User ID and UID.
    /// </summary>
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
    /// <summary>
    /// Gets the password from the connection string.
    /// </summary>
    public string Password
    {
        get
        {
            if (TryGetValue("Password", out string S)) return S;
            if (TryGetValue("Psw", out S)) return S;
            return string.Empty;
        }
    }
    /// <summary>
    /// Gets the database name or database file path, depending on the provider.
    /// </summary>
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
    /// <summary>
    /// Gets the database server name or address.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the OLE DB provider name.
    /// </summary>
    public string OleDbProvider
    {
        get { TryGetValue("Provider", out string S); return S; }
        set => this["Provider"] = value;
    }
    /// <summary>
    /// Gets or sets the OLE DB extended properties value.
    /// </summary>
    public string ExtendedProperties
    {
        get { TryGetValue("Extended Properties", out string S); return S; }
        set => this["Extended Properties"] = value;
    }
 
}
namespace PasswordManager.Services;

/// <summary>
/// Imports and exports encrypted credential rows.
/// </summary>
static public class CredentialTransferService
{
    // ● private
    /// <summary>
    /// Escapes a value for simple sample SQL statements.
    /// </summary>
    static string Q(string Value)
    {
        return Value == null ? "null" : $"'{Value.Replace("'", "''")}'";
    }
    /// <summary>
    /// Formats a date-time value for SQL.
    /// </summary>
    static string Q(DateTime Value)
    {
        return Q(Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
    }

    // ● static public
    /// <summary>
    /// Returns the fixed export file path used by this sample.
    /// </summary>
    static public string GetExportFilePath()
    {
        return System.IO.Path.Combine(SysConfig.AppFolderPath, "credential-export.json");
    }
    /// <summary>
    /// Exports encrypted credential rows to the fixed sample export file.
    /// </summary>
    static public string Export(SqlStore Store)
    {
        MemTable Table = Store.Select("""
                                      select
                                          Id,
                                          CategoryId,
                                          Title,
                                          UserName,
                                          Url,
                                          Password,
                                          Notes,
                                          CreatedAt,
                                          UpdatedAt
                                          from Credential
                                          order by Title
                                          """);
        if (Table.Rows.Count == 0)
            throw new TripousException("There are no credentials to export.");
        List<CredentialTransferRow> List = [];
        foreach (DataRow Row in Table.Rows)
        {
            CredentialTransferRow Item = new();
            Item.Id = Row["Id"].ToString();
            Item.CategoryId = Convert.ToInt32(Row["CategoryId"], CultureInfo.InvariantCulture);
            Item.Title = Row["Title"].ToString();
            Item.UserName = Row["UserName"] == DBNull.Value ? string.Empty : Row["UserName"].ToString();
            Item.Url = Row["Url"] == DBNull.Value ? string.Empty : Row["Url"].ToString();
            Item.Password = Row["Password"] == DBNull.Value ? string.Empty : Row["Password"].ToString();
            Item.Notes = Row["Notes"] == DBNull.Value ? string.Empty : Row["Notes"].ToString();
            Item.CreatedAt = Convert.ToDateTime(Row["CreatedAt"], CultureInfo.InvariantCulture);
            Item.UpdatedAt = Convert.ToDateTime(Row["UpdatedAt"], CultureInfo.InvariantCulture);
            List.Add(Item);
        }

        string FilePath = GetExportFilePath();
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath));
        JsonSerializerOptions Options = new() { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(List, Options), Encoding.UTF8);
        return FilePath;
    }
    /// <summary>
    /// Imports encrypted credential rows from the fixed sample export file.
    /// </summary>
    static public int Import(SqlStore Store)
    {
        string FilePath = GetExportFilePath();
        if (!File.Exists(FilePath))
            throw new TripousException($"Export file not found: {FilePath}");
        string JsonText = File.ReadAllText(FilePath, Encoding.UTF8);
        List<CredentialTransferRow> List = JsonSerializer.Deserialize<List<CredentialTransferRow>>(JsonText) ?? [];
        Store.ExecSql("delete from Credential");
        foreach (CredentialTransferRow Item in List)
        {
            string SqlText = $"""
                             insert into Credential
                                (Id, CategoryId, Title, UserName, Url, Password, Notes, CreatedAt, UpdatedAt)
                             values
                                ({Q(Item.Id)}, {Item.CategoryId}, {Q(Item.Title)}, {Q(Item.UserName)}, {Q(Item.Url)}, {Q(Item.Password)}, {Q(Item.Notes)}, {Q(Item.CreatedAt)}, {Q(Item.UpdatedAt)})
                             """;
            Store.ExecSql(SqlText);
        }

        return List.Count;
    }
}

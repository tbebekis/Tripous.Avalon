namespace PasswordManager.Services;

/// <summary>
/// Imports and exports encrypted credential rows.
/// </summary>
static public class CredentialTransferService
{
    // ● private
    /// <summary>
    /// Creates a transfer category row from a data row.
    /// </summary>
    static CredentialTransferCategoryRow CreateCategoryRow(DataRow Row)
    {
        CredentialTransferCategoryRow Result = new();
        Result.Id = Convert.ToInt32(Row["Id"], CultureInfo.InvariantCulture);
        Result.Name = Row["Name"].ToString();
        Result.DisplayOrder = Convert.ToInt32(Row["DisplayOrder"], CultureInfo.InvariantCulture);
        return Result;
    }
    /// <summary>
    /// Creates a transfer credential row from a data row.
    /// </summary>
    static CredentialTransferRow CreateCredentialRow(DataRow Row)
    {
        CredentialTransferRow Result = new();
        Result.Id = Row["Id"].ToString();
        Result.CategoryId = Convert.ToInt32(Row["CategoryId"], CultureInfo.InvariantCulture);
        Result.Title = Row["Title"].ToString();
        Result.UserName = Row["UserName"] == DBNull.Value ? string.Empty : Row["UserName"].ToString();
        Result.Url = Row["Url"] == DBNull.Value ? string.Empty : Row["Url"].ToString();
        Result.Password = Row["Password"] == DBNull.Value ? string.Empty : Row["Password"].ToString();
        Result.Notes = Row["Notes"] == DBNull.Value ? string.Empty : Row["Notes"].ToString();
        Result.CreatedAt = Convert.ToDateTime(Row["CreatedAt"], CultureInfo.InvariantCulture);
        Result.UpdatedAt = Convert.ToDateTime(Row["UpdatedAt"], CultureInfo.InvariantCulture);
        return Result;
    }
    /// <summary>
    /// Inserts a category row inside the current import transaction.
    /// </summary>
    static void InsertCategory(SqlStore Store, System.Data.Common.DbTransaction Transaction, CredentialTransferCategoryRow Item)
    {
        string SqlText = @"
                         insert into Category
                            (Id, Name, DisplayOrder)
                         values
                            (:Id, :Name, :DisplayOrder)
                         ";
        Store.ExecSql(Transaction, SqlText, Item.Id, Item.Name, Item.DisplayOrder);
    }
    /// <summary>
    /// Inserts a credential row inside the current import transaction.
    /// </summary>
    static void InsertCredential(SqlStore Store, System.Data.Common.DbTransaction Transaction, CredentialTransferRow Item)
    {
        string SqlText = @"
                         insert into Credential
                            (Id, CategoryId, Title, UserName, Url, Password, Notes, CreatedAt, UpdatedAt)
                         values
                            (:Id, :CategoryId, :Title, :UserName, :Url, :Password, :Notes, :CreatedAt, :UpdatedAt)
                         ";
        Store.ExecSql(Transaction, SqlText, Item.Id, Item.CategoryId, Item.Title, Item.UserName, Item.Url, Item.Password, Item.Notes, Item.CreatedAt, Item.UpdatedAt);
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
        MemTable CategoryTable = Store.Select(@"
                                             select
                                                 Id,
                                                 Name,
                                                 DisplayOrder
                                             from Category
                                             order by DisplayOrder
                                             ");
        MemTable CredentialTable = Store.Select(@"
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
                                               ");
        CredentialTransferFile TransferFile = new();
        foreach (DataRow Row in CategoryTable.Rows)
            TransferFile.Categories.Add(CreateCategoryRow(Row));
        foreach (DataRow Row in CredentialTable.Rows)
            TransferFile.Credentials.Add(CreateCredentialRow(Row));

        string FilePath = GetExportFilePath();
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath));
        JsonSerializerOptions Options = new() { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(TransferFile, Options), Encoding.UTF8);
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
        CredentialTransferFile TransferFile = JsonSerializer.Deserialize<CredentialTransferFile>(JsonText) ?? new();
        using SqlTransactionContext Context = Store.BeginTransactionContext();
        Store.ExecSql(Context.Transaction, "delete from Credential");
        Store.ExecSql(Context.Transaction, "delete from Category");
        foreach (CredentialTransferCategoryRow Item in TransferFile.Categories)
            InsertCategory(Store, Context.Transaction, Item);
        foreach (CredentialTransferRow Item in TransferFile.Credentials)
            InsertCredential(Store, Context.Transaction, Item);
        Context.Commit();
        return TransferFile.Credentials.Count;
    }
}

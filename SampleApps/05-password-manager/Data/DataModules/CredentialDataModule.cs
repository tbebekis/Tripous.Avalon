namespace PasswordManager.Data;

/// <summary>
/// Data module that encrypts and decrypts credential secret fields.
/// </summary>
public class CredentialDataModule : DataModule
{
    // ● private
    /// <summary>
    /// Decrypts secret fields in a data row.
    /// </summary>
    private void DecryptRow(DataRow Row, bool AcceptChanges)
    {
        if (Row == null || Row.RowState == DataRowState.Deleted)
            return;
        string Password = Row["Password"] == DBNull.Value ? string.Empty : Row["Password"].ToString();
        string Notes = Row["Notes"] == DBNull.Value ? string.Empty : Row["Notes"].ToString();
        Row["Password"] = VaultService.Decrypt(Password);
        Row["Notes"] = VaultService.Decrypt(Notes);
        if (AcceptChanges)
            Row.AcceptChanges();
    }
    /// <summary>
    /// Encrypts secret fields in a data row.
    /// </summary>
    private void EncryptRow(DataRow Row)
    {
        if (Row == null || Row.RowState == DataRowState.Deleted)
            return;
        string Password = Row["Password"] == DBNull.Value ? string.Empty : Row["Password"].ToString();
        string Notes = Row["Notes"] == DBNull.Value ? string.Empty : Row["Notes"].ToString();
        Row["Password"] = VaultService.Encrypt(Password);
        Row["Notes"] = VaultService.Encrypt(Notes);
    }

    // ● protected
    /// <summary>
    /// Sets default values for credential rows.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (TableDef.Name == "Credential")
        {
            if (Sys.IsNull(Row["CategoryId"]))
                Row["CategoryId"] = 1;
            if (Sys.IsNull(Row["CreatedAt"]))
                Row["CreatedAt"] = DateTime.Now;
            Row["UpdatedAt"] = DateTime.Now;
        }
    }
    /// <summary>
    /// Decrypts the item row after it is loaded for editing.
    /// </summary>
    protected override void Edited(object RowId)
    {
        base.Edited(RowId);
        DecryptRow(tblItem.Rows.Count > 0 ? tblItem.Rows[0] : null, AcceptChanges: true);
    }

    // ● public
    /// <summary>
    /// Encrypts secret fields before commit and restores plaintext in memory afterwards.
    /// </summary>
    public override object Commit(bool Reselect = false)
    {
        DataRow Row = tblItem.Rows.Count > 0 ? tblItem.Rows[0] : null;
        object Result;
        bool Succeeded = false;
        EncryptRow(Row);
        try
        {
            Result = base.Commit(Reselect);
            Succeeded = true;
        }
        finally
        {
            DecryptRow(tblItem.Rows.Count > 0 ? tblItem.Rows[0] : null, AcceptChanges: Succeeded);
        }
        return Result;
    }
}

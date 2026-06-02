/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class AppUserDataModule: DataModule
{
    public AppUserDataModule()
    {
    }

    public void AddUser(string FullName, string UserName, string PlainTextPassword, UserLevel UserLevel, string CultureCode = "en-US", string Email = "", string Phone = "")
    {
        string Salt = Sec.CreateSalt();
        string Password = Sec.HashPassword(PlainTextPassword, Salt, 100_000);

        Insert();

        CurrentRow["Id"] = Sys.GenId();
        CurrentRow["FullName"] = FullName;
        CurrentRow["UserName"] = UserName;
        CurrentRow["Password"] = Password;
        CurrentRow["Salt"] = Salt;
        CurrentRow["UserLevelId"] = (int)UserLevel;
        CurrentRow["CultureCode"] = CultureCode;
        CurrentRow["Email"] = Email;
        CurrentRow["Phone"] = Phone;
        CurrentRow["LastLoginAt"] = DateTime.MinValue;
        CurrentRow["PasswordChangedAt"] = DateTime.MinValue;
        CurrentRow["IsActive"] = true;

        Commit();
    }

    public AppUser LoadByUserName(string UserName)
    {
        AppUser Result = null;
        
        string SafeUserName = UserName.Replace("'", "''");
        string TableName = ModuleDef.Table.Name;
        string SqlText = $"select * from {TableName} where UserName = '{SafeUserName}'";
        
        DataRow Row = Store.SelectResults(SqlText);

        if (Row != null)
        {
            Result = new();

            Result.Id = Row.AsString("Id");
            Result.UserName = Row.AsString("UserName");
            Result.FullName = Row.AsString("FullName");
            Result.UserLevel = (UserLevel)Row.AsInteger("UserLevelId");
            Result.CultureCode = Row.AsString("CultureCode");
            Result.Email = Row.AsString("Email");
            Result.Phone = Row.AsString("Phone");
            Result.LastLoginAt = Row.AsDateTime("LastLoginAt");
            
            Result.IsActive = Row.AsBoolean("IsActive");
            Result.Remarks = Row.AsString("Remarks");
            
            Result.Properties["Password"] = Row.AsString("Password");
            Result.Properties["Salt"] = Row.AsString("Salt");
        }

        return Result;
    }
}
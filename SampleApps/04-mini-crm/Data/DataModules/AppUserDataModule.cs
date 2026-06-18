namespace MiniCrm.Data;

/// <summary>
/// Data module for application users.
/// </summary>
public class AppUserDataModule : DataModule
{
    // ● protected
    /// <summary>
    /// Sets default values for new user rows.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <param name="Row">The data row.</param>
    /// <param name="TableDef">The table definition.</param>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);
        string Salt = Sec.CreateSalt();
        Row["Password"] = Sec.HashPassword("changeme", Salt, 100_000);
        Row["Salt"] = Salt;
        Row["UserLevelId"] = (int)UserLevel.User;
        Row["CultureCode"] = "en-US";
        Row["LastLoginAt"] = DateTime.MinValue;
        Row["PasswordChangedAt"] = DateTime.MinValue;
        Row["IsActive"] = 1;
        Row["Remarks"] = "Default password: changeme";
    }

    // ● public
    /// <summary>
    /// Adds a new application user.
    /// </summary>
    /// <param name="FullName">The full name.</param>
    /// <param name="UserName">The user name.</param>
    /// <param name="PlainTextPassword">The plain text password.</param>
    /// <param name="UserLevel">The user level.</param>
    /// <param name="CultureCode">The culture code.</param>
    /// <param name="Email">The email address.</param>
    /// <param name="Phone">The phone number.</param>
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
        CurrentRow["IsActive"] = 1;
        Commit();
    }
    /// <summary>
    /// Sets the password of an existing application user.
    /// </summary>
    /// <param name="UserId">The user id.</param>
    /// <param name="PlainTextPassword">The plain text password.</param>
    public void SetPassword(string UserId, string PlainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(UserId))
            throw new TripousException("User id is required.");
        if (string.IsNullOrWhiteSpace(PlainTextPassword))
            throw new TripousException("Password is required.");
        string Salt = Sec.CreateSalt();
        string Password = Sec.HashPassword(PlainTextPassword, Salt, 100_000);
        Edit(UserId);
        CurrentRow["Password"] = Password;
        CurrentRow["Salt"] = Salt;
        CurrentRow["PasswordChangedAt"] = Store.GetServerDateTime();
        Commit();
    }
    /// <summary>
    /// Changes the password of an existing application user after validating the current password.
    /// </summary>
    /// <param name="UserName">The user name.</param>
    /// <param name="CurrentPlainTextPassword">The current plain text password.</param>
    /// <param name="NewPlainTextPassword">The new plain text password.</param>
    public void ChangePassword(string UserName, string CurrentPlainTextPassword, string NewPlainTextPassword)
    {
        AppUser User = LoadByUserName(UserName);
        if (User == null)
            throw new TripousException("User not found.");
        string Password = User.Properties["Password"] as string;
        string Salt = User.Properties["Salt"] as string;
        bool IsValid = Sec.VerifyPassword(CurrentPlainTextPassword, Password, Salt, 100_000);
        if (!IsValid)
            throw new TripousException("Current password is invalid.");
        SetPassword(User.Id, NewPlainTextPassword);
    }
    /// <summary>
    /// Loads an application user by user name.
    /// </summary>
    /// <param name="UserName">The user name.</param>
    /// <returns>The application user, or null.</returns>
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

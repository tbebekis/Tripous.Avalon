namespace MiniCrm.Data;

/// <summary>
/// Defines database schema version 1.
/// </summary>
public partial class SchemaVersion1 : SchemaVersionDef
{
    // ● protected
    /// <summary>
    /// Registers schema version 1 tables and seed statements.
    /// </summary>
    protected override void RegisterInternal()
    {
        string SqlText = @"
CREATE TABLE SYS_LOG (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Year integer @NOT_NULL,
   Month integer @NOT_NULL,
   DayOfMonth integer @NOT_NULL,
   LogTime @NVARCHAR(20) @NOT_NULL,
   User @NVARCHAR(96) @NOT_NULL,
   Host @NVARCHAR(96) @NOT_NULL,
   Level @NVARCHAR(96) @NOT_NULL,
   Source @NVARCHAR(512) @NOT_NULL,
   Scope @NVARCHAR(512) @NOT_NULL,
   EventId @NVARCHAR(96) @NOT_NULL,
   Message @NBLOB_TEXT @NOT_NULL
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE SYS_NUMBER_SERIES (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Code @NVARCHAR(40) @NOT_NULL,
   Name @NVARCHAR(96) @NOT_NULL,
   Pattern @NVARCHAR(64) @NOT_NULL,
   ResetPeriodId integer default 0 @NOT_NULL,
   NextNumber integer default 1 @NOT_NULL,
   LastResetValue @NVARCHAR(16) @NULL,
   IsActive @BOOL default 1 @NOT_NULL,
   CONSTRAINT UQ_SYS_NUMBER_SERIES_Code UNIQUE (Code),
   CONSTRAINT UQ_SYS_NUMBER_SERIES_Name UNIQUE (Name)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE SYS_STR_RES (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Lang @NVARCHAR(12) @NOT_NULL,
   ResKey @NVARCHAR(96) @NOT_NULL,
   ResValue @NBLOB_TEXT @NOT_NULL,
   CONSTRAINT UQ_SYS_STR_RES_Lang_ResKey UNIQUE (Lang, ResKey)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE SYS_APP_USER (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   UserName @NVARCHAR(64) @NOT_NULL,
   Password @NVARCHAR(512) @NOT_NULL,
   Salt @NVARCHAR(256) @NOT_NULL,
   FullName @NVARCHAR(96) @NOT_NULL,
   UserLevelId integer @NOT_NULL,
   CultureCode @NVARCHAR(16) @NULL,
   Email @NVARCHAR(96) @NULL,
   Phone @NVARCHAR(40) @NULL,
   LastLoginAt @DATE_TIME @NULL,
   PasswordChangedAt @DATE_TIME @NULL,
   IsActive @BOOL default 1 @NOT_NULL,
   Remarks @NBLOB_TEXT @NULL,
   CONSTRAINT UQ_SYS_APP_USER_UserName UNIQUE (UserName)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE SYS_CONFIG (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   ScopeId integer @NOT_NULL,
   OwnerKey @NVARCHAR(96) @NULL,
   Name @NVARCHAR(128) @NOT_NULL,
   Value @NVARCHAR(512) @NULL,
   TextValue @NBLOB_TEXT @NULL,
   ModifiedAt @DATE_TIME @NULL,
   ModifiedBy @NVARCHAR(40) @NULL,
   CONSTRAINT UQ_SYS_CONFIG_Scope_Owner_Name UNIQUE (ScopeId, OwnerKey, Name),
   FOREIGN KEY (ModifiedBy) REFERENCES SYS_APP_USER(Id)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE Customer (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Code @NVARCHAR(40) @NOT_NULL,
   Name @NVARCHAR(128) @NOT_NULL,
   Email @NVARCHAR(96) @NULL,
   Phone @NVARCHAR(40) @NULL,
   Website @NVARCHAR(128) @NULL,
   City @NVARCHAR(96) @NULL,
   IsActive @BOOL default 1 @NOT_NULL,
   CreatedAt @DATE_TIME @NOT_NULL,
   UpdatedAt @DATE_TIME @NOT_NULL,
   Notes @NBLOB_TEXT @NULL,
   CONSTRAINT UQ_Customer_Code UNIQUE (Code),
   CONSTRAINT UQ_Customer_Name UNIQUE (Name)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE Contact (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   CustomerId @NVARCHAR(40) @NOT_NULL,
   FirstName @NVARCHAR(64) @NOT_NULL,
   LastName @NVARCHAR(64) @NOT_NULL,
   JobTitle @NVARCHAR(96) @NULL,
   Email @NVARCHAR(96) @NULL,
   Phone @NVARCHAR(40) @NULL,
   IsPrimaryContact @BOOL default 0 @NOT_NULL,
   Notes @NBLOB_TEXT @NULL,
   FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE ActivityType (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(64) @NOT_NULL,
   DisplayOrder integer @NOT_NULL
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE Activity (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   CustomerId @NVARCHAR(40) @NOT_NULL,
   ContactId @NVARCHAR(40) @NULL,
   ActivityDate @DATE_TIME @NOT_NULL,
   ActivityTypeId integer @NOT_NULL,
   Subject @NVARCHAR(128) @NOT_NULL,
   Description @NVARCHAR(4000) @NULL,
   IsClosed @BOOL default 0 @NOT_NULL,
   FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
   FOREIGN KEY (ContactId) REFERENCES Contact(Id),
   FOREIGN KEY (ActivityTypeId) REFERENCES ActivityType(Id)
)
";
        Version.AddTable(SqlText);

        Version.AddStatementAfter("insert into ActivityType (Id, Name, DisplayOrder) values (1, 'Call', 10)");
        Version.AddStatementAfter("insert into ActivityType (Id, Name, DisplayOrder) values (2, 'Email', 20)");
        Version.AddStatementAfter("insert into ActivityType (Id, Name, DisplayOrder) values (3, 'Meeting', 30)");
        Version.AddStatementAfter("insert into ActivityType (Id, Name, DisplayOrder) values (4, 'Task', 40)");
        Version.AddStatementAfter("insert into SYS_NUMBER_SERIES (Id, Code, Name, Pattern, ResetPeriodId, NextNumber, LastResetValue, IsActive) values ('CUSTOMER', 'CUSTOMER', 'Customer Code', 'C-{0000}', 0, 3, null, 1)");
        Version.AddStatementAfter("insert into Customer (Id, Code, Name, Email, Phone, Website, City, IsActive, CreatedAt, UpdatedAt, Notes) values ('CUST-001', 'C-0001', 'Acme Services', 'info@acme.example', '+30 210 1000001', 'https://acme.example', 'Athens', 1, '2026-01-10 09:00:00', '2026-01-10 09:00:00', 'Sample customer used by the Mini CRM sample.')");
        Version.AddStatementAfter("insert into Customer (Id, Code, Name, Email, Phone, Website, City, IsActive, CreatedAt, UpdatedAt, Notes) values ('CUST-002', 'C-0002', 'Northwind Trading', 'contact@northwind.example', '+30 2310 100002', 'https://northwind.example', 'Thessaloniki', 1, '2026-01-11 10:00:00', '2026-01-11 10:00:00', 'Second sample customer.')");
        Version.AddStatementAfter("insert into Contact (Id, CustomerId, FirstName, LastName, JobTitle, Email, Phone, IsPrimaryContact, Notes) values ('CONT-001', 'CUST-001', 'Maria', 'Papadopoulou', 'Operations Manager', 'maria@acme.example', '+30 694 1000001', 1, 'Primary Acme contact.')");
        Version.AddStatementAfter("insert into Contact (Id, CustomerId, FirstName, LastName, JobTitle, Email, Phone, IsPrimaryContact, Notes) values ('CONT-002', 'CUST-002', 'Nikos', 'Ioannou', 'Sales Director', 'nikos@northwind.example', '+30 697 1000002', 1, 'Primary Northwind contact.')");
        Version.AddStatementAfter("insert into Activity (Id, CustomerId, ContactId, ActivityDate, ActivityTypeId, Subject, Description, IsClosed) values ('ACT-001', 'CUST-001', 'CONT-001', '2026-01-12 11:30:00', 1, 'Introductory call', 'Initial sample phone call with Acme.', 1)");
        Version.AddStatementAfter("insert into Activity (Id, CustomerId, ContactId, ActivityDate, ActivityTypeId, Subject, Description, IsClosed) values ('ACT-002', 'CUST-002', 'CONT-002', '2026-01-13 14:00:00', 3, 'Product demo', 'Sample meeting activity for Northwind.', 0)");
    }

    // ● properties
    /// <summary>
    /// Gets the schema version number.
    /// </summary>
    public override int VersionNumber => 1;
}

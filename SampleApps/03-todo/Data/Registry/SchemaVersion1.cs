namespace ToDo.Data;

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
        // ● Standard system log table used by the Tripous logging infrastructure.
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

        // ● Standard system user table.
        // ● The sample does not enable login yet, but later samples use the same shape.
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

        // ● TodoStatus is a normal table and also the source table of a LookupDef.
        // ● A table-backed lookup is more flexible than an enum because users may edit rows through a normal DataForm.
        SqlText = @"
CREATE TABLE TodoStatus (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(64) @NOT_NULL,
   DisplayOrder integer @NOT_NULL
)
";
        Version.AddTable(SqlText);

        // ● SYS_CONFIG stores values edited by ConfigDialog.
        // ● This sample keeps the table minimal but compatible with SysConfigModule.
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

        // ● TodoTask is the master table of this sample.
        // ● TodoStatusId is a foreign key and is described in RegistryVersion1 as a lookup field.
        // ● @BOOL is provider-specific integer boolean SQL. Tripous boolean database fields are 0/1 values.
        SqlText = @"
CREATE TABLE TodoTask (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Title @NVARCHAR(128) @NOT_NULL,
   Description @NVARCHAR(4000),
   TodoStatusId integer @NOT_NULL,
   DueDate @DATE @NULL,
   CompletedAt @DATE_TIME @NULL,
   Priority integer @NOT_NULL,
   IsDone @BOOL @NOT_NULL,
   CreatedAt @DATE_TIME @NOT_NULL,
   UpdatedAt @DATE_TIME @NOT_NULL,
   FOREIGN KEY (TodoStatusId) REFERENCES TodoStatus(Id)
)
";
        Version.AddTable(SqlText);

        // ● AddStatementAfter() runs after the tables are created.
        // ● This is a good place for seed rows, such as the fixed status rows used by the lookup.
        Version.AddStatementAfter("insert into TodoStatus (Id, Name, DisplayOrder) values (1, 'Open', 10)");
        Version.AddStatementAfter("insert into TodoStatus (Id, Name, DisplayOrder) values (2, 'In Progress', 20)");
        Version.AddStatementAfter("insert into TodoStatus (Id, Name, DisplayOrder) values (3, 'Waiting', 30)");
        Version.AddStatementAfter("insert into TodoStatus (Id, Name, DisplayOrder) values (4, 'Done', 40)");
    }

    // ● properties
    /// <summary>
    /// Gets the schema version number.
    /// </summary>
    public override int VersionNumber => 1;
}

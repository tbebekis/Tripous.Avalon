namespace PasswordManager.Data;

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
CREATE TABLE SYS_CONFIG (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   ScopeId integer @NOT_NULL,
   OwnerKey @NVARCHAR(96) @NULL,
   Name @NVARCHAR(128) @NOT_NULL,
   Value @NVARCHAR(512) @NULL,
   TextValue @NBLOB_TEXT @NULL,
   ModifiedAt @DATE_TIME @NULL,
   ModifiedBy @NVARCHAR(40) @NULL,
   CONSTRAINT UQ_SYS_CONFIG_Scope_Owner_Name UNIQUE (ScopeId, OwnerKey, Name)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE Category (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(64) @NOT_NULL,
   DisplayOrder integer @NOT_NULL,
   CONSTRAINT UQ_Category_Name UNIQUE (Name)
)
";
        Version.AddTable(SqlText);

        SqlText = @"
CREATE TABLE Credential (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   CategoryId integer @NOT_NULL,
   Title @NVARCHAR(128) @NOT_NULL,
   UserName @NVARCHAR(128) @NULL,
   Url @NVARCHAR(512) @NULL,
   Password @NBLOB_TEXT @NULL,
   Notes @NBLOB_TEXT @NULL,
   CreatedAt @DATE_TIME @NOT_NULL,
   UpdatedAt @DATE_TIME @NOT_NULL,
   FOREIGN KEY (CategoryId) REFERENCES Category(Id)
)
";
        Version.AddTable(SqlText);

        Version.AddStatementAfter("insert into Category (Id, Name, DisplayOrder) values (1, 'Personal', 10)");
        Version.AddStatementAfter("insert into Category (Id, Name, DisplayOrder) values (2, 'Work', 20)");
        Version.AddStatementAfter("insert into Category (Id, Name, DisplayOrder) values (3, 'Finance', 30)");
    }

    // ● properties
    /// <summary>
    /// Gets the schema version number.
    /// </summary>
    public override int VersionNumber => 1;
}

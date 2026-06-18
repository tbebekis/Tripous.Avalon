namespace Notes.Data;

/// <summary>
/// Defines database schema version 1.
/// </summary>
public partial class SchemaVersion1 : SchemaVersionDef
{
    // ● protected
    /// <summary>
    /// Registers schema version 1 tables.
    /// </summary>
    protected override void RegisterInternal()
    {
        // ● SchemaVersionDef.Register() gives us the Version object.
        // ● Version.AddTable() registers a RDBMS-neutral CREATE TABLE statement.
        // ● Tripous replaces tokens such as @NVARCHAR, @DATE_TIME, @BOOL and @NOT_NULL with provider-specific SQL.
        // ● @BOOL is provider-specific integer boolean SQL. Tripous boolean database fields are 0/1 values.
        // ● New schema versions may add new tables, views and statements. They may also execute ALTER TABLE statements.
        // ● Version.AddStatementBefore() is for SQL that must run before table/view creation.
        // ● Version.AddStatementAfter() is for SQL that must run after table/view creation, such as seed data.
        string SqlText = @"
CREATE TABLE Note (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Title @NVARCHAR(128) @NOT_NULL,
   Body @NVARCHAR(4000),
   CreatedAt @DATE_TIME @NOT_NULL,
   UpdatedAt @DATE_TIME @NOT_NULL,
   IsPinned @BOOL @NOT_NULL
)
";
        Version.AddTable(SqlText);
    }

    // ● properties
    /// <summary>
    /// Gets the schema version number.
    /// </summary>
    public override int VersionNumber => 1;
}

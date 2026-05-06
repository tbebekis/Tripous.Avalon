namespace Tripous.Data;

static public class SchemaVersionExtensions
{
    // ● lookups
    static public void AddLookup(this SchemaVersion SV, string TableName)
    {
        string SqlText = @$"
CREATE TABLE {TableName} (
     Id  @NVARCHAR(40)  @NOT_NULL primary key,
     Name @NVARCHAR(96) @NOT_NULL,
     CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
)
";        
        SV.AddTable(SqlText);
    }
    static public void AddLookupWithCode(this SchemaVersion SV, string TableName)
    {
        string SqlText = @$"
CREATE TABLE {TableName} (
     Id  @NVARCHAR(40)  @NOT_NULL primary key,
     Code @NVARCHAR(40) @NOT_NULL,
     Name @NVARCHAR(96) @NOT_NULL,
     CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
     CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)
";        
        SV.AddTable(SqlText);
    }
    static public void AddLookupWithCodeAndIsActive(this SchemaVersion SV, string TableName)
    {
        string SqlText = @$"
CREATE TABLE {TableName} (
     Id  @NVARCHAR(40)  @NOT_NULL primary key,
     Code @NVARCHAR(40) @NOT_NULL,
     Name @NVARCHAR(96) @NOT_NULL,
     IsActive @BOOL default 1 @NOT_NULL,
     CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
     CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)
";        
        SV.AddTable(SqlText);
    }
}
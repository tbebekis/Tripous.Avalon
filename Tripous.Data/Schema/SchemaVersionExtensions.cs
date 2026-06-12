/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Extension methods for <see cref="SchemaVersion"/>
/// </summary>
static public class SchemaVersionExtensions
{
    // ● lookups
    /// <summary>
    /// Adds a CREATE TABLE for a lookup table, with Id and Name columns.
    /// </summary>
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
    /// <summary>
    /// Adds a CREATE TABLE for a lookup table, with Id, Name and Code columns.
    /// </summary>
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
    /// <summary>
    /// Adds a CREATE TABLE for a lookup table, with Id, Name, Code and IsActive columns.
    /// </summary>
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
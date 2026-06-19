# Schema Creation

Tripous.Data creates and updates a database through registered schema versions.

A schema belongs to a domain and a connection name. Each schema has one or more versions, and each version may register tables, views, SQL statements that run before object creation, and SQL statements that run after object creation.

## Main Types

- `Schema` is the registered schema for a domain and connection.
- `SchemaVersion` contains the tables, views and SQL statements of one version.
- `SchemaVersionDef` is the usual base class for manual or generated schema version classes.
- `Schemas` is the global schema registry.
- `SchemaExecutor` is the internal execution engine.

## Schema Version Definition

A schema version class usually derives from `SchemaVersionDef` and fills the `Version` object inside `RegisterInternal()`.

```csharp
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
```

`SchemaVersionDef.Register()` validates the domain, connection name and version number. It then finds or creates the matching `Schema`, finds or creates the matching `SchemaVersion`, and calls `RegisterInternal()`.

## RDBMS Neutral SQL

Schema SQL uses Tripous tokens instead of native database types.

- `@NVARCHAR`
- `@DATE_TIME`
- `@BOOL`
- `@NOT_NULL`

During execution, `SchemaExecutor` calls the provider and replaces these tokens with native SQL. This is one of the reasons the same schema declaration can target the six supported RDBMS engines.

## Registering Schemas

Applications usually keep schema version definitions in a registry class.

```csharp
static public partial class Registry
{
    // ● private fields
    static readonly List<SchemaVersionDef> fSchemaVersionList = [];

    // ● constructor
    static Registry()
    {
        fSchemaVersionList.AddRange([
            new SchemaVersion1()
        ]);
    }

    // ● static public
    /// <summary>
    /// Registers database schema versions.
    /// </summary>
    static public void RegisterSchemas()
    {
        foreach (SchemaVersionDef Version in fSchemaVersionList)
            Version.Register();
    }
}
```

After registration, `Schemas.Execute()` executes all registered schemas.

```csharp
Registry.RegisterSchemas();
Schemas.Execute();
```

## Version Tracking

Tripous stores the last executed database version in `DbIni`.

The entry name has this form.

```text
Database.Version.{ConnectionName}.{Domain}
```

When `Schemas.Execute()` runs, only versions greater than the stored version are executed. The new version number is written to `DbIni` inside the final schema transaction.

`SchemaVersion.Execute()` exists for direct execution, but it does not write the version number to `DbIni`.

## Tables Views And Statements

`SchemaVersion` exposes these methods.

- `AddTable()` registers a `CREATE TABLE` statement.
- `AddView()` registers a named view.
- `AddStatementBefore()` registers SQL that runs before table and view creation.
- `AddStatementAfter()` registers SQL that runs after table and view creation.

Tables and views are created only when missing. Index statements are also checked, and an already existing index is not created again.

## Execution Phases

Schema execution uses two transaction phases.

- Phase 1 runs `StatementsBefore`, creates tables, and creates views.
- Phase 2 runs `StatementsAfter` and writes the new version number to `DbIni`.

This split is intentional. DDL transaction behavior differs between RDBMS engines, so the schema engine keeps object creation and later data or alteration statements separated.

## Generated And Manual Schemas

RegBuilder can generate `SchemaVersionDef` classes from schema files. Manual applications can write the same kind of class directly.

The runtime model is the same in both cases: register `SchemaVersionDef` instances, then execute the global `Schemas` registry.

# Database Schema Information

Tripous.Data can read structural information from an existing database.

This is separate from schema creation. Schema creation writes tables, views and statements. Database schema information reads what already exists in the database and exposes it as metadata objects.

## Two Metadata Levels

Tripous.Data provides two practical levels.

- `SqlStore` has lightweight methods for names and existence checks.
- `DbSchema` loads a richer object model with tables, columns, indexes, constraints, views, procedures and sequences.

## Lightweight Checks

Use `SqlStore` when code only needs a quick answer.

```csharp
SqlStore Store = SqlStores.CreateSqlStore("Default");

List<string> TableNames = Store.GetTableNames();
List<string> ViewNames = Store.GetViewNames();
List<string> FieldNames = Store.GetFieldNames("Customer");
List<string> IndexNames = Store.GetIndexNames();

bool CustomerExists = Store.TableExists("Customer");
bool NameExists = Store.FieldExists("Customer", "Name");
bool IndexExists = Store.IndexExists("IX_Customer_Name");
```

These methods are used by schema execution too. For example, `SchemaExecutor` checks existing tables, views and indexes before creating anything.

## DbSchema

Each `DbConnectionInfo` owns a `DbSchema` instance.

```csharp
DbConnectionInfo ConnectionInfo = Db.GetConnectionInfo("Default");

ConnectionInfo.Schema.Load();

foreach (DbMetaTable Table in ConnectionInfo.Schema.Tables)
{
    string TableName = Table.Name;
    string FieldList = Table.GetFieldNameList();
}
```

`DbSchema.Load()` loads metadata once and caches it. `UnLoad()` clears the collections, and `ReLoad()` refreshes them from the database.

```csharp
DbSchema Schema = Db.GetConnectionInfo("Default").Schema;

Schema.Load();
Schema.ReLoad();
Schema.UnLoad();
```

## Metadata Objects

`DbSchema` exposes these collections.

- `Tables`
- `Views`
- `Procedures`
- `Sequences`

Tables contain related metadata.

- `Columns`
- `ForeignKeys`
- `Constraints`
- `Indexes`
- `Triggers`

The basic metadata classes are:

- `DbMetaObject`
- `DbMetaTable`
- `DbMetaColumn`
- `DbMetaIndex`
- `DbMetaConstraint`
- `DbMetaForeignKey`
- `DbMetaTrigger`
- `DbMetaView`
- `DbMetaProcedure`
- `DbMetaSequence`

## Columns And Display Text

Column metadata includes the database type, size, precision, scale, nullability, identity flag, computed flag and default value.

```csharp
DbSchema Schema = Db.GetConnectionInfo("Default").Schema;

Schema.Load();

DbMetaTable Table = Schema.Tables.First(x => x.Name.IsSameText("Customer"));

foreach (DbMetaColumn Column in Table.Columns)
{
    string Name = Column.Name;
    string Type = Column.DataType;
    bool IsNullable = Column.IsNullable;
    string Text = Column.DisplayText;
}
```

`DisplayText` is useful for diagnostics or developer tools because it formats the metadata in a compact readable form.

## RDBMS Specific Loading

`DbSchemaLoader` is the internal loader. It uses provider-specific SQL resources in order to normalize database metadata into the common Tripous metadata classes.

The background study for this work is available at [Db-Schema-Info](https://github.com/tbebekis/Db-Schema-Info). It contains the RDBMS-specific `SELECT` statements for reading tables, columns, views, triggers, procedures, constraints, indexes and sequences from Firebird, SQL Server, MySQL, PostgreSQL, SQLite and Oracle.

The result is not meant to hide every RDBMS difference. It gives Tripous.Data a common metadata model for inspection, tooling, schema checks and diagnostics.

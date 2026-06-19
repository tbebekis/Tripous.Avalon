# Overview

Manual application declaration is the direct way to describe a Tripous application in C# code.

The framework is fully declarative. An application can register its database schema, modules, tables, fields, lookups, locators, select definitions, code providers, configuration properties and optional presentation descriptors by creating descriptor objects and adding them to the Tripous registries.

This is the core idea:

- The developer writes C# declarations.
- The declarations are stored in central registries.
- The framework uses those declarations to create runtime behavior.
- The application keeps full control over SQL, metadata and presentation structure.

Manual declaration is not a fallback mechanism and it is not a simplified mode. It is the declaration model itself.

The Registration Builder is the Tripous tool that performs automatic application declaration. It reads schema files and metadata comments, and then generates C# registration code.

The important point is that the Registration Builder uses the same model. It generates the same kind of C# declarations that a developer can write by hand.

Manual declaration:

```text
Manual C# Declaration
        |
        v
Tripous Registries
        |
        v
Runtime Application
```

Automatic declaration:

```text
Schema.sql
        |
        v
Registration Builder
        |
        v
Generated C# Declaration
        |
        v
Tripous Registries
        |
        v
Runtime Application
```

Both paths end in the same descriptors and registries.

## What Gets Declared

A typical Tripous application declares several kinds of objects.

- `SchemaVersionDef` declares database schema versions.
- `ModuleDef` declares a data module exposed by the application.
- `TableDef` declares the editable table structure used by a module.
- `FieldDef` declares fields, data types, flags, lookups, locators and presentation behavior.
- `SelectDef` declares list queries and filters.
- `FormDef` declares forms when the application uses a presentation layer.
- `LookupDef` declares lookup sources.
- `LocatorDef` declares search-and-select behavior for foreign keys.
- `CodeProvider` definitions declare generated code values such as customer codes or document numbers.
- Configuration property definitions declare editable application settings.

These declarations do not replace the database.

Tripous still uses SQL directly. The declarations describe how the framework should understand and present the database structure.

## Where Declarations Live

The sample applications use a small application-level registry layer.

The `Registry` class owns the list of schema versions and registry versions.

```csharp
static public void RegisterSchemas()
{
    foreach (SchemaVersionDef Version in fSchemaVersionList)
        Version.Register();
}

static public void RegisterDescriptors()
{
    foreach (RegistryVersion Version in fRegistryVersionList)
    {
        Version.RegisterLookupSources();
        Version.RegisterLocators();
        Version.RegisterModules();
        Version.RegisterForms();
        Version.RegisterConfigProperties();
    }

    DataRegistry.Modules.UpdateReferences();
    DesktopRegistry.Forms.UpdateReferences();
}
```

Each `RegistryVersion` then registers the descriptors for one application version.

This is an application architecture convention used by the samples. It is not a required base class from the Tripous core, but it keeps registration explicit, ordered and easy to inspect.

## From Schema To Module

The registration process starts with the database schema.

Tripous schema declarations use SQL, but the SQL may contain RDBMS-neutral tokens such as `@NVARCHAR`, `@DATE`, `@DATE_TIME`, `@BOOL`, `@NOT_NULL` and `@NULL`.

This token model exists because Tripous supports six different RDBMS engines: SQL Server, SQLite, PostgreSQL, MySQL, Firebird and Oracle.

The active `SqlProvider` replaces those tokens with the proper database-specific SQL when the schema is executed.

The `03-todo` sample registers the `TodoTask` table like this:

```csharp
string SqlText = @"
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
```

This declaration creates the physical database table.

After the schema is registered, the application registers the descriptors that explain how Tripous should use that table.

The same sample declares the `TodoTask` module by creating a `ModuleDef`, defining its list SQL, adding filters through `SelectDef`, and describing editable fields through `TableDef`.

```csharp
ModuleDef Module = DataRegistry.AddModule("TodoTask", TitleKey: "ToDo", ClassName: typeof(ToDoDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
SelectDef SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Title", FieldName: "t.Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("Status", FieldName: "s.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);

TableDef Table = Module.Table;
Table.Name = "TodoTask";
Table.AddId();
Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
Table.AddDate("DueDate");
Table.AddBoolean("IsDone");
```

This small block gives the framework enough metadata to understand:

- Which table is edited.
- Which fields exist.
- Which fields are required.
- Which lookup is used for the status field.
- Which SQL query is used for the list view.
- Which filters the list view exposes.

The schema declaration creates the database structure. The module declaration describes how the application works with that structure.

The same pattern scales to more complex applications with detail tables, joins, locators, custom data modules, code providers and presentation descriptors.

## Why Manual Declaration Matters

Manual declaration is important because it makes the Tripous model visible.

It shows exactly what the framework needs in order to build a data-centric application:

- Database structure.
- Data module definitions.
- Field metadata.
- List SQL and filters.
- Lookup and locator metadata.
- Optional form registration.
- Application configuration.

Once this model is understood, automatic declaration becomes easier to understand too.

The Registration Builder does not introduce a different architecture. It writes the declarations that the developer would otherwise write by hand.

## When To Use Manual Declaration

Manual declaration is useful when:

- The application is small or medium sized.
- The developer wants full control over every descriptor.
- The database structure is hand-written.
- The application needs custom descriptors that are easier to express directly in C#.
- The developer is learning how Tripous registration works.

Automatic declaration is useful when:

- The schema is large.
- Many modules follow common conventions.
- Repetitive registration code should be generated.
- The application already uses metadata comments in `Schema.sql` files.

Both approaches can coexist. Generated declarations may be extended manually where needed.

## Related Articles

- [Declaration Order](declaration-order.md)
- [Application Host](application-host.md)
- [Modules](modules.md)
- [Tables and Fields](tables-and-fields.md)
- [Automatic Application Declaration](../automatic-declaration/overview.md)

# Overview

Automatic application declaration is the Tripous workflow where descriptor code is generated from schema files.

Instead of writing all `ModuleDef`, `TableDef`, `FieldDef`, `FormDef`, `LookupDef`, `LocatorDef`, `SelectDef` and `CodeProviderDef` declarations by hand, the developer writes a schema file with Tripous metadata comments.

The Registration Builder reads that schema file and writes C# registration code.

The generated code uses the same descriptor classes and registries as manual registration.

## Main Idea

Automatic declaration follows this pipeline:

```text
Schema.sql
        |
        v
Metadata Comments
        |
        v
Registration Builder
        |
        v
Generated C# Registry Files
        |
        v
Tripous Descriptors
        |
        v
Runtime Application
```

The generated registry files are not a separate architecture.

They are generated C# code that registers the same descriptors a developer could write manually.

## Metadata Syntax

Tripous uses a special metadata syntax inside SQL comments.

This metadata is read by the Registration Builder.

It describes how a database table should become application declarations.

Example:

```sql
/*---------------------------------------------------
Table: Product
Module: Product ProductDataModule
Group: Inventory
Form: Product ProductForm
Code: PRD-YYYY-XXXXXX
FilterFields: Code, Name, ProductGroup__Name
IsLookup
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,                    -- Code
    Name @NVARCHAR(96) @NOT_NULL,
    ProductGroupId @NVARCHAR(40) @NULL,              -- Lookup
    ProductGroupName @NVARCHAR(96) @NULL,            -- Snapshot ProductGroup.Name
    IsActive @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    FOREIGN KEY (ProductGroupId) REFERENCES ProductGroup(Id)
    )
```

The SQL part describes the database table.

The metadata comment describes application intent.

From this block the Registration Builder can infer:

- A schema table.
- A module.
- A module group.
- A form.
- A code provider.
- A lookup source.
- Filters.
- A lookup field.
- A snapshot field.
- Field data types and flags.

This is why automatic declaration is centered on `Schema.sql` files.

The schema is both database definition and application metadata source.

## Generated Output

The generated output is C# code.

Typical generated files include:

- Schema version files.
- Registry version files.
- Module registration files.
- Form registration files.
- Lookup registration files.
- Locator registration files.
- Code provider registration files.

In TinyERP, generated files live under:

```text
SampleApps/TinyERP/tERP.Data/Registry
```

Generated files contain descriptor registration code such as:

```csharp
Module = DataRegistry.AddOrUpdateModule("Product", ClassName: "ProductDataModule", ListSelectSql: SqlText);
tblTop = Module.Table;
tblTop.Name = "Product";
tblTop.AddId("Id").SetNullable(false);
tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Code", FieldName: "Code", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("Name", FieldName: "Name", FilterDataType: DataFieldType.String);
```

This is ordinary Tripous registration code.

The difference is that the Registration Builder wrote it.

## TinyERP

TinyERP is the main sample for automatic declaration.

It uses:

- Schema files.
- Metadata comments.
- The Registration Builder console tool.
- Generated registry files.
- Manual data modules.
- Manual document handlers.
- Manual desktop forms.
- Tests.

The generated declarations provide the application structure.

The handwritten code provides business behavior and UI-specific extensions.

This is the intended pattern for larger Tripous applications.

## What Automatic Declaration Is Good For

Automatic declaration is useful when:

- The database schema is large.
- Many modules follow consistent conventions.
- Forms are mostly descriptor-driven.
- Lookups and locators follow naming rules.
- Select definitions and filters can be inferred from metadata.
- Repetitive registration code would be hard to maintain by hand.

It is less useful when:

- The application is very small.
- The descriptors are highly custom.
- The developer is still learning the descriptor model.

The smaller samples use manual registration because they make the model visible.

TinyERP uses automatic registration because it has many declarations.

## What Comes Next

The rest of this section explains:

- How `Schema.sql` files are organized.
- How metadata comments work.
- How the Registration Builder is run.
- What files are generated.
- How generated schema versions and registry versions are structured.
- How generated modules, forms, lookups, locators, select definitions and code providers work.
- How to extend generated declarations manually.
- Which best practices keep generated registration predictable.

## Related Articles

- [Generated vs Manual Registration](../generated-vs-manual-registration.md)
- [Schema.sql Files](schema-sql-files.md)
- [Metadata Comments](metadata-comments.md)
- [Registration Builder](reg-builder.md)
- [Generated Artifacts](generated-artifacts.md)

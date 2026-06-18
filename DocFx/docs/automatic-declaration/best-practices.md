# Best Practices

Automatic application declaration works best when the project treats generated code as a reproducible artifact and handwritten code as the place for application decisions.

The Registration Builder should generate the common registration structure.

The developer should keep business behavior, special cases and final corrections in handwritten extension code.

## Keep Generated Files Disposable

Generated files should be replaceable at any time.

Do not edit them manually.

When a generated file needs to change, change the source that created it:

- change `Schema.sql` for database structure
- change metadata comments for generated descriptors
- change RegBuilder project configuration for generation settings
- regenerate the output

If a required change cannot be expressed by schema metadata, add it as a manual extension after generation.

## Keep Schema Files As The Source Of Truth

The `Schema.sql` files should describe the application database as clearly as possible.

Use them for:

- tables
- fields
- primary keys
- foreign keys
- indexes
- constraints
- RDBMS-neutral type tokens
- RDBMS-neutral SQL tokens
- metadata comments

Avoid hiding structural database decisions in handwritten registration code.

If a table, field or relation is part of the schema, it belongs in the schema file.

## Keep Metadata Focused

Metadata comments should describe registration facts that can be generated safely.

Good metadata candidates:

- module name
- module group
- form class name
- item page class name
- lookup fields
- locator fields
- field groups
- field flags
- list filters
- code providers
- enum fields
- snapshot fields

Bad metadata candidates:

- complex business behavior
- runtime decisions
- user-specific behavior
- special SQL that belongs to a custom locator
- behavior that requires a custom class

When metadata becomes hard to read, move the special case to handwritten extension code.

## Use One Top Table Per Module

Each generated module should have one top table.

Only that top table should contain the module-level metadata block.

Detail tables should describe their own fields and relations, but they should not declare the parent module metadata.

This keeps module generation predictable.

## Prefer Stable Names

Generated registration depends heavily on names.

Keep these names stable after data exists:

- table names
- field names
- module names
- form names
- lookup names
- locator names
- code provider names
- enum names

Renaming a descriptor may be harmless in code, but it can break saved settings, user permissions, configuration rows, number series rows or existing data references.

## Keep Module Blocks Explicit

When a table produces multiple modules, declare a separate module block for each module.

Example:

```sql
Module: SalesOrder SalesOrderDataModule
Group: Sales
Form: SalesOrder SalesOrderForm
ItemPage: TradeItemPage
Code: Draft SO-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesOrder'
FilterFields: Code, TradeDate, Person__Name, TradeStatus

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
Form: SalesInvoice SalesInvoiceForm
ItemPage: TradeItemPage
Code: Draft SI-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesInvoice'
FilterFields: Code, TradeDate, Person__Name, TradeStatus
```

This makes the generated result easier to review and easier to extend manually.

## Use ListWhere For Module Filtering

Use `ListWhere` when multiple modules share the same top table and each module needs a separate list.

Write only the condition.

Do not include the `WHERE` keyword.

Good:

```sql
ListWhere: DocumentType.ModuleName = 'SalesInvoice'
```

Avoid putting this condition only in handwritten UI code.

The module list definition should already know what rows belong to the module.

## Use FilterFields Deliberately

Use `FilterFields` when the generated default filters are too many or not in the desired order.

Example:

```sql
FilterFields: Code, TradeDate, Person__Name, TradeStatus
```

Only use fields that exist in the generated list SELECT.

This includes top table fields, generated join aliases and generated enum display columns.

## Use Header Code For Document Modules

For document modules, prefer header-level `Code:` metadata.

This is especially useful when many modules share the same physical table.

Example:

```sql
Module: SalesInvoice SalesInvoiceDataModule
Code: Draft SI-YYYY-XXXXXX
```

Use field-level `-- Code` metadata mostly for simple tables with one module.

## Keep Joins Useful But Not Excessive

Lookup fields may generate joins and display columns.

Use those generated join aliases when they improve list readability or filters.

Do not add joins only because the data might be useful someday.

Every generated join affects the list SELECT and may affect performance.

## Prefer Custom Locators For Business Logic

Schema metadata can create ordinary locators.

Use handwritten extensions or custom locator classes when the locator needs:

- role filtering
- active-row filtering
- permission-sensitive filtering
- complex joins
- calculated values
- custom returned values
- custom mapping behavior

The tERP `Customer`, `Supplier` and `Product` locators are examples of this approach.

## Keep Snapshot Fields Intentional

Snapshot fields store copied values from a selected lookup or locator row.

Use them when the value must remain historically stable.

Typical examples:

- document line product code
- document line product name
- partner name at posting time
- tax or unit information captured at transaction time

Do not use snapshot fields for values that should always reflect the current master record.

## Put Manual Extensions In Partial Registry Files

Keep handwritten extension code close to registration, but outside generated files.

A practical layout is:

- generated `RegistryVersionN.*.cs` files
- generated `SchemaVersionN.cs` files
- handwritten `Registry.cs` coordinator
- handwritten `Registry.Miscs.cs` extensions
- handwritten domain-specific registry files

This keeps regeneration safe and keeps application-specific changes easy to find.

## Register Generated Versions Explicitly

When a new schema or registry version is generated, the application must add it to the coordinator.

Example:

```csharp
SchemaVersionList.AddRange([
    new SchemaVersion1(),
    new SchemaVersion2()
]);

RegistryVersionList.AddRange([
    new RegistryVersion1(),
    new RegistryVersion2()
]);
```

This step is intentionally explicit.

The application controls which generated versions participate in startup registration.

## Review Generated Output

After running the Registration Builder, review the generated files before committing them.

Check especially:

- generated table registration
- generated joins
- generated list SELECT statements
- generated filters
- generated locators
- generated forms
- generated code providers
- generated schema statements

The generated output is code and should be reviewed like code.

## Recreate Development Databases When Needed

Schema changes may require database recreation during development.

Examples:

- added fields
- removed fields
- changed field types
- changed required flags
- changed constraints
- changed indexes

For providers that support database creation, Tripous can recreate the database when the application startup and connection settings allow it.

For providers that do not support database creation, or when permissions do not allow it, the developer must drop and recreate the required database objects manually.

## Keep Sample Data Separate

Sample data is not generated by the Registration Builder.

It is application code or application data maintained by the developer.

Keep sample data versioning separate from schema and registry versioning.

This makes it clear which part creates the database structure, which part registers descriptors and which part inserts demo data.

## Do Not Overuse Metadata

The goal is not to express the whole application in comments.

The goal is to generate the repetitive registration layer and leave the important application behavior in normal code.

Use metadata for declarative facts.

Use handwritten code for behavior.

## Practical Checklist

- Keep `Schema.sql` readable.
- Keep table and field names stable.
- Use one top table per generated module.
- Put module metadata only on the top table.
- Use separate module blocks for shared tables.
- Use `ListWhere` for shared-table module filtering.
- Use `FilterFields` to control generated filter order.
- Use header `Code:` for document modules.
- Use custom locators for business-specific lookup behavior.
- Use snapshot fields only for historically captured values.
- Never edit generated registry files manually.
- Put final corrections in handwritten extension files.
- Add new generated versions to the registry coordinator.
- Review generated output before committing.

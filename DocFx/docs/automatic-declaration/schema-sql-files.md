# Schema.sql Files

`Schema.sql` files are the main input for automatic application declaration.

They are not only database scripts.

They are also metadata sources for the Registration Builder.

## Dual Role

A schema file has two roles:

- It declares database structure.
- It declares application metadata through comments.

The SQL part creates tables, columns, constraints and foreign keys.

The metadata comments tell Tripous how those tables should become modules, forms, lookups, locators, filters, field groups, code providers and generated descriptors.

## Basic Shape

A table block usually has this shape:

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

The `CREATE TABLE` statement is still valid schema input.

The header and field comments add Tripous metadata.

## Provider-Neutral Tokens

Schema files may use provider-neutral tokens.

Common tokens include:

- `@NVARCHAR(size)`
- `@NBLOB_TEXT`
- `@DATE`
- `@DATE_TIME`
- `@BOOL`
- `@NOT_NULL`
- `@NULL`

The active `SqlProvider` translates those tokens to the target database dialect for one of the six relational database engines supported by Tripous.

This lets the same schema definition work across supported relational database engines.

## TableName Placeholder

Generated schema code often uses `{TableName}` inside `CREATE TABLE` statements and constraints.

Example:

```sql
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
```

The generated `SchemaVersionN.cs` file assigns the actual table name before registering the SQL.

```csharp
string TableName = "Product";
string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
Version.AddTable(SqlText);
```

This keeps table blocks reusable inside the generator and keeps constraint names aligned with the table name.

## Metadata Comments

Tripous metadata lives in SQL comments.

There are two main places for metadata:

- Table header comments.
- Field comments.

Header comments describe table-level and module-level intent.

Field comments describe field-level behavior.

A module has one and only one top table.

Only that top table should declare module-level metadata such as:

- `Module`
- `Group`
- `Form`
- `ItemPage`
- `Code`
- `ListWhere`
- `FilterFields`
- `DetailOrder`

Detail and subdetail tables are declared as tables, but they do not declare their own module metadata unless they are also exposed as separate top-level modules.

This convention keeps module ownership clear.

Examples:

```sql
Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
Form: SalesInvoice SalesInvoiceForm
ItemPage: TradeItemPage
Code: Draft SINV-YYYY-XXXXXX
ListWhere: DocumentType.ModuleName = 'SalesInvoice'
FilterFields: Code, TradeDate, Person__Code, Person__Name, TradeStatus, TotalAmount
```

```sql
PersonId @NVARCHAR(40) @NOT_NULL,          -- Locator Person
PersonCode @NVARCHAR(40) @NULL,           -- Snapshot Person.Code
PersonName @NVARCHAR(128) @NULL,          -- Snapshot Person.Name
Remarks @NBLOB_TEXT @NULL,                -- LargeMemo; Group Notes
```

The Registration Builder parses this metadata syntax and converts it to descriptor registrations.

## Schema Versions

Applications may have multiple schema versions.

Each schema version can add or change database structure.

The Registration Builder can generate versioned schema classes such as:

- `SchemaVersion1`
- `SchemaVersion2`

Each generated schema version registers tables and schema statements for that version.

During parsing, the Registration Builder resolves table dependencies and calculates creation order.

It can rebuild the schema text in dependency order and inject `CreationOrder` in generated table headers.

`CreationOrder` is generated output.

It is not required in the input `Schema.sql` file.

The rebuilt schema text is available through `SchemaParserResult.SchemaSql`.

This makes future application extension easier because new database work can be grouped by version.

## Registry Versions

Descriptor registration can also be versioned.

The Registration Builder can generate registry version classes such as:

- `RegistryVersion1`
- `RegistryVersion2`

A registry version groups generated descriptor declarations for the same application version.

This keeps generated modules, forms, lookups, locators and code providers organized over time.

## Sample Data Versions

Applications may also keep sample data versioned.

TinyERP uses this pattern.

```text
SampleData.Version1.cs
SampleData.Version2.cs
```

This is useful when schema and registration versions need matching setup data.

Sample data files are not generated by the Registration Builder.

They are the developer's responsibility.

Sample data versions can seed:

- Master data.
- Document types.
- Number series.
- Lookup rows.
- Demo records.
- Required system defaults.

Versioned schema, registration and sample data make it easier to extend the application later.

## TinyERP Files

TinyERP uses schema files under:

```text
SampleApps/TinyERP/tERP.Data
```

Current files include:

- `Schema01.sql`
- `Schema02.sql`

Generated schema and registry code is under:

```text
SampleApps/TinyERP/tERP.Data/Registry
```

Sample data is under:

```text
SampleApps/TinyERP/tERP.Data/SampleData
```

## What Belongs In Schema Files

Put structural application metadata in schema files.

Good candidates:

- Tables.
- Fields.
- Primary keys.
- Foreign keys.
- Unique constraints.
- Lookup declarations.
- Locator declarations.
- Snapshot declarations.
- Module declarations.
- Form declarations.
- Field groups.
- Filter fields.
- Code provider metadata.

Keep behavior out of schema files.

Behavior belongs in:

- Data modules.
- Document handlers.
- Services.
- Desktop forms.
- Tests.

## Practical Rule

When writing schema files for automatic declaration:

- Keep table blocks readable.
- Keep metadata comments close to the table or field they describe.
- Use provider-neutral tokens where possible.
- Use `{TableName}` for table-specific constraint names.
- Use clear module and form names.
- Use field comments for field-level behavior.
- Use header metadata for module-level behavior.
- Keep business logic out of the schema.
- Treat schema, registry and sample data versions as a way to organize future growth.

## Related Articles

- [Overview](overview.md)
- [Metadata Comments](metadata-comments.md)
- [Registration Builder](reg-builder.md)
- [Generated Schema Versions](generated-schema-versions.md)
- [Generated Registry Versions](generated-registry-versions.md)

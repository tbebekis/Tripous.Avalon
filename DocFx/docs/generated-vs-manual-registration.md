# Generated vs Manual Registration

Tripous has one registration model.

Manual registration and generated registration are two ways to produce the same descriptor objects.

This is the central idea:

```text
Manual Registration
        ==
Generated Registration
```

The Registration Builder is the Tripous code generation tool that reads schema files and metadata comments and writes C# registration code.

It does not create a second framework mode.

It writes C# registration code that uses the same descriptor classes a developer can use by hand.

## One Runtime Model

A Tripous application is described by descriptors.

Important descriptor types include:

- `SchemaVersionDef`
- `ModuleDef`
- `TableDef`
- `FieldDef`
- `FormDef`
- `LookupDef`
- `LocatorDef`
- `SelectDef`
- `CodeProviderDef`

Manual registration creates those descriptors directly.

Generated registration creates those descriptors through generated C# code.

In both cases, the descriptors end up in the same registries.

```text
Manual C# Code
        |
        v
Tripous Descriptors
        |
        v
Tripous Registries
        |
        v
Runtime Application
```

```text
Schema.sql + Metadata Comments
        |
        v
Registration Builder
        |
        v
Generated C# Code
        |
        v
Tripous Descriptors
        |
        v
Tripous Registries
        |
        v
Runtime Application
```

The runtime does not care whether the descriptors were handwritten or generated.

## Manual Registration

Manual registration means writing the descriptor code directly.

Example:

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

This code explicitly declares:

- The module.
- The list SQL.
- The list filters.
- The editable table.
- The fields.
- The lookup relation.
- The field behavior.

Manual registration is the most direct way to see the Tripous model.

It is not a fallback and it is not a simplified path.

It is the model itself.

## Generated Registration

Generated registration starts from schema files and metadata comments.

The Registration Builder reads those inputs and writes C# registration code.

Example schema input:

```sql
/*---------------------------------------------------
Table: Product
Module: Product ProductDataModule
Group: Inventory
Form: Product ProductForm
Code: PRD-YYYY-XXXXXX
FilterFields: Code, Name, ProductGroup__Name
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

This one block gives the Registration Builder both database structure and application metadata.

It can infer the table, module, form, group, code provider, filters, lookup relation and snapshot field.

Generated code still calls the same registry APIs.

Example from a generated module:

```csharp
Module = DataRegistry.AddOrUpdateModule("Account", ClassName: "AccountDataModule", ListSelectSql: SqlText);
tblTop = Module.Table;
tblTop.Name = "Account";
tblTop.KeyField = "Id";
tblTop.AddId("Id").SetNullable(false);
tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
tblTop.AddEnumLookupId("AccountTypeId", "AccountType", TypeStore.Get("AccountType"), Flags: FieldFlags.Required).SetNullable(false);
SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Name", FieldName: "Name", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("Code", FieldName: "Code", FilterDataType: DataFieldType.String);
```

This is normal descriptor code.

The difference is who wrote it.

- Manual registration: the developer wrote the C# code.
- Generated registration: the Registration Builder wrote the C# code.

The output is still a `ModuleDef`, a `TableDef`, `FieldDef` objects and a `SelectDef`.

## What The Registration Builder Automates

The Registration Builder automates repetitive declaration work.

It can generate:

- Schema version classes.
- Module registrations.
- Table and field registrations.
- Detail and subdetail registrations.
- Join registrations.
- Form registrations.
- Lookup registrations.
- Locator registrations.
- Select SQL.
- Filter registrations.
- Column type registrations.
- Code provider registrations.
- Number series seed statements.

It is especially useful when:

- The schema has many tables.
- Modules follow consistent naming conventions.
- Many forms are straightforward data forms.
- Joins and filters follow metadata conventions.
- Code provider and lookup patterns are repeated.

## What The Registration Builder Does Not Replace

Generated registration does not replace application code.

The developer still writes:

- Business logic.
- Data modules.
- Document handlers.
- Services.
- Custom forms.
- Custom item pages.
- Application commands.
- Validation logic.
- Posting logic.
- Tests.

The generated descriptors describe the application structure.

Handwritten code implements application behavior.

This separation is important.

## Why This Matters

Some frameworks have two separate modes:

- A manual mode.
- An automatic mode.

Those modes often have different assumptions and different runtime behavior.

Tripous does not work that way.

Tripous has one descriptor model.

The Registration Builder only automates descriptor creation.

That means:

- A developer can inspect generated code.
- A developer can write equivalent code by hand.
- Generated modules can be extended manually.
- Manual modules can coexist with generated modules.
- The same runtime features apply to both approaches.
- Learning manual registration explains generated registration.
- Reading generated registration teaches the manual model too.

## Typical Use

Small samples often use manual registration.

Examples:

- `02-notes`
- `03-todo`
- `04-mini-crm`
- `05-password-manager`

These samples keep the registration code visible and easy to study.

Larger samples can use generated registration.

Example:

- `TinyERP`

TinyERP uses schema metadata and the Registration Builder because it has many modules, joins, lookups, locators, forms and code providers.

## Extension Pattern

Generated registration is usually treated as a baseline.

The normal pattern is:

- Put structural declarations in schema metadata.
- Generate registry files.
- Do not edit generated files manually.
- Add business logic in handwritten data modules.
- Add UI behavior in handwritten forms.
- Add application behavior in services and handlers.
- Regenerate descriptors when schema metadata changes.

Generated code should remain disposable.

Handwritten extensions should remain stable.

## Practical Rule

Use this rule when deciding between manual and generated registration:

- Use manual registration to learn the descriptor model.
- Use manual registration for small applications and special cases.
- Use generated registration when the schema is large and repetitive.
- Keep generated code close to the code you would write manually.
- Do not hide important behavior in generation conventions.
- Do not edit generated files directly.
- Put behavior in data modules, handlers, services and forms.
- Treat both approaches as two paths to the same descriptors.

## Related Articles

- [Manual Application Declaration](manual-declaration/overview.md)
- [Automatic Application Declaration](automatic-declaration/overview.md)
- [Registration Builder](automatic-declaration/reg-builder.md)
- [Generated Artifacts](automatic-declaration/generated-artifacts.md)
- [Manual Extensions After Generation](automatic-declaration/manual-extensions-after-generation.md)

# Tables and Fields

`TableDef` and `FieldDef` describe the editable data structure of a module.

They are not the database schema.

The database schema creates physical tables and columns. `TableDef` and `FieldDef` describe how Tripous should work with those tables and columns at runtime.

## TableDef

A `TableDef` describes one editable table.

The top table of a module is available through `ModuleDef.Table`.

```csharp
TableDef Table = Module.Table;
Table.Name = "TodoTask";
Table.AddId();
Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
Table.AddDate("DueDate");
Table.AddBoolean("IsDone");
```

The table name should match the database table name.

The fields should match real database columns unless they are explicitly marked as extra fields.

## Schema Alignment

The schema and the table definition describe the same table from different perspectives.

Schema declaration:

```csharp
string SqlText = @"
CREATE TABLE TodoTask (
   Id @NVARCHAR(40) @NOT_NULL primary key,
   Title @NVARCHAR(128) @NOT_NULL,
   Description @NVARCHAR(4000),
   TodoStatusId integer @NOT_NULL,
   DueDate @DATE @NULL,
   IsDone @BOOL @NOT_NULL
)
";
Version.AddTable(SqlText);
```

Runtime declaration:

```csharp
TableDef Table = Module.Table;
Table.Name = "TodoTask";
Table.AddId();
Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
Table.AddDate("DueDate");
Table.AddBoolean("IsDone");
```

The schema creates the database table.

The `TableDef` tells Tripous how to edit that table.

The two declarations should agree on:

- Table name.
- Primary key.
- Field names.
- Data types.
- String sizes.
- Required fields.
- Foreign key fields.
- Boolean fields.

## Primary Keys

Most modules start by declaring an id field.

```csharp
Table.AddId();
```

`AddId()` uses the default Tripous object id configuration.

Other helper methods are available when needed:

```csharp
Table.AddStringId();
Table.AddIntegerId();
```

Lookup tables often use integer ids.

```csharp
Table.Name = "TodoStatus";
Table.AddIntegerId();
Table.AddString("Name", 64, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddInteger("DisplayOrder", Flags: FieldFlags.Required);
```

## FieldDef

A `FieldDef` describes one field.

Important properties include:

- `Name` is the field name.
- `Alias` is the field alias used in joined tables or select results.
- `TitleKey` is the display title key.
- `DataType` is the logical Tripous data type.
- `MaxLength` is the maximum string length.
- `Decimals` is used by numeric fields.
- `Flags` describe behavior.
- `LookupSource` links the field to a lookup.
- `Locator` links the field to a locator.
- `CodeProvider` links the field to a code provider.
- `DefaultValue` stores a default value expression.
- `Expression` stores a calculated expression.
- `DisplayFormat` controls display formatting.
- `EditFormat` controls edit formatting.
- `DisplayWidth` provides a preferred display width.
- `Group` groups fields for presentation.
- `ToolTip` stores field help text.
- `SnapshotOf` identifies a source field whose value is stored as a snapshot.

Most fields are created through helper methods on `TableDef`.

## Common Field Helpers

String fields:

```csharp
Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
```

Integer fields:

```csharp
Table.AddInteger("Priority", Flags: FieldFlags.Required);
```

Decimal fields:

```csharp
Table.AddDecimal("Amount", Decimals: 2, Flags: FieldFlags.Required);
```

Date and date-time fields:

```csharp
Table.AddDate("DueDate");
Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
```

Boolean fields:

```csharp
Table.AddBoolean("IsDone");
```

Text blob fields:

```csharp
Table.AddTextBlob("Notes").SetMemo();
```

Large memo fields:

```csharp
Table.AddTextBlob("Message").SetLargeMemo();
```

Blob fields:

```csharp
Table.AddBlob("Data");
```

## Field Flags

`FieldFlags` control field behavior.

Common flags:

- `Required` means the field is required.
- `Hidden` hides the field from normal presentation.
- `ReadOnly` makes the field read-only.
- `ReadOnlyUI` makes the field read-only in presentation controls.
- `ReadOnlyEdit` prevents editing after insert.
- `Searchable` marks the field as searchable.
- `Memo` marks a multiline text field.
- `LargeMemo` marks a large multiline text field.
- `Boolean` marks an integer-backed boolean field.
- `NoInsertUpdate` excludes the field from insert and update statements.
- `ForeignKey` marks a foreign key field.
- `Extra` marks a field that does not exist in the database.

Flags can be combined.

```csharp
Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
```

This field is required, displayed as read-only and not editable after insert.

## Required And Nullable

The database schema controls nullability at the database level.

`FieldFlags.Required` describes application-level required behavior.

The two should normally match.

```csharp
Title @NVARCHAR(128) @NOT_NULL
```

```csharp
Table.AddString("Title", 128, Flags: FieldFlags.Required);
```

If the database field is `@NOT_NULL`, the `FieldDef` should normally be `Required`.

If the field is optional in the database, the `Required` flag should normally be omitted.

## Boolean Fields

Tripous boolean database fields are integer-backed values.

The schema uses the RDBMS-neutral `@BOOL` token.

```csharp
IsDone @BOOL @NOT_NULL
```

The table definition uses `AddBoolean()`.

```csharp
Table.AddBoolean("IsDone");
```

`AddBoolean()` sets the logical data type and the `Boolean` flag.

## Lookup Fields

A lookup field stores a key and displays a value from a lookup source.

```csharp
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
```

This declaration means:

- The field name is `TodoStatusId`.
- The stored value is an integer.
- The lookup source name is `TodoStatus`.
- The display title is `Status`.
- The field is required.

Lookup sources must be registered separately before module references are resolved.

## Enum Lookup Fields

Enum lookup fields can create the required lookup source automatically.

```csharp
Table.AddEnumLookupId("UserLevelId", "UserLevel", TypeStore.Get("UserLevel"), Flags: FieldFlags.Required);
```

The field stores an integer value.

The lookup items come from the enum type.

## Locator Fields

A locator field stores a key selected from another table.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";
```

The locator name points to a registered `LocatorDef`.

The locator controls how the related row is searched and selected.

## Code Provider Fields

A field may receive its value from a code provider.

```csharp
Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetCodeProviderName("CUSTOMER");
```

This pattern is used when the application generates values such as customer codes or document numbers.

The field is required and searchable, but normal editing is restricted because the value comes from the code provider.

## Detail Tables

Detail tables are declared under a parent `TableDef`.

```csharp
TableDef Contact = Table.AddDetail("Contact", "Id", "CustomerId");
Contact.AddId();
Contact.AddString("CustomerId", 40, Flags: FieldFlags.Required | FieldFlags.Hidden);
Contact.AddString("FirstName", 64, Flags: FieldFlags.Required);
Contact.AddString("LastName", 64, Flags: FieldFlags.Required);
Contact.AddBoolean("IsPrimaryContact", Flags: FieldFlags.Required);
```

The arguments to `AddDetail()` are:

- Detail table name.
- Master field name.
- Detail foreign key field name.

A detail table has its own fields and may have its own joins.

## Joins

Joins describe related table metadata.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";

TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
CustomerJoin.AddString("Code", 40);
CustomerJoin.AddString("Name", 128);
```

The join is defined by:

- The local key field.
- The locator name.
- The foreign table name.
- The foreign table alias.
- The foreign primary key field.

Joined fields are related display data.

They are not normal editable fields of the main table.

## Extra Fields

Sometimes a field is needed in a data table but does not exist in the database table.

Such fields should be marked with `FieldFlags.Extra`.

Extra fields are useful for calculated values, presentation-only values or temporary values.

Fields that exist only in list SQL do not necessarily belong in the editable `TableDef`.

## Practical Rule

When declaring a table manually:

- Start from the database schema.
- Declare the top table name.
- Declare the primary key.
- Add fields in a readable order.
- Match required flags with database nullability.
- Match string sizes with schema sizes.
- Use lookup helpers for lookup foreign keys.
- Use locators for search-and-select foreign keys.
- Use `ReadOnlyUI` and `ReadOnlyEdit` for generated or audit fields.
- Use `NoInsertUpdate` or `Extra` only when the field should not be persisted normally.
- Add details and joins after the main fields are clear.

## Related Articles

- [Modules](modules.md)
- [Select Definitions](select-definitions.md)
- [Lookups](lookups.md)
- [Locators](locators.md)
- [Code Providers](code-providers.md)

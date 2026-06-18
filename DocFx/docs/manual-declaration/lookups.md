# Lookups

Lookups translate stored values into display values.

A lookup field usually stores an id, code or enum value. The lookup supplies the text shown to the user or to the presentation layer.

Example:

- `TodoTask.TodoStatusId` stores `1`.
- The `TodoStatus` lookup displays `Open`.

## LookupDef And LookupSource

`LookupDef` is the descriptor.

It declares where lookup items come from and which fields provide values and display text.

`LookupSource` is the runtime object.

It loads rows or enum values and creates `LookupItem` values.

A lookup may load items from:

- A table name.
- A SQL select statement.
- An enum type.
- A custom `LookupSource` class.
- A `DataTable` loaded by application code.

## Table-Backed Lookups

The most common lookup is backed by a database table.

The `03-todo` sample declares a lookup for task status rows.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName("TodoStatus", "TodoStatus", FormName: "TodoStatus");
Lookup.ValueField = "Id";
Lookup.DisplayField = "Name";
```

This means:

- The lookup name is `TodoStatus`.
- The source table is `TodoStatus`.
- The value comes from the `Id` field.
- The display text comes from the `Name` field.
- The form named `TodoStatus` may be used to edit lookup rows.

The matching field declaration uses the lookup name.

```csharp
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
```

The field stores the integer id. The lookup supplies the display text.

## ValueField And DisplayField

`ValueField` is the stored value.

`DisplayField` is the display text.

For most lookup tables the defaults are:

- `ValueField = "Id"`
- `DisplayField = "Name"`

When the table uses another display column, set it explicitly.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName("SYS_APP_USER", "SYS_APP_USER");
Lookup.ValueField = "Id";
Lookup.DisplayField = "UserName";
```

The lookup source requires both fields to exist in the loaded data.

## SQL Lookups

A lookup may be based on a SQL select statement.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithSql("ActiveCustomer", @"
select
    Id,
    Name
from Customer
where IsActive = 1
order by Name
");
Lookup.ValueField = "Id";
Lookup.DisplayField = "Name";
```

SQL lookups are useful when the display list needs filtering, ordering or calculated columns that are not expressed by a simple table name.

## Enum Lookups

Enum lookups use an enum type as the source.

```csharp
Table.AddEnumLookupId("UserLevelId", "UserLevel", TypeStore.Get("UserLevel"), Flags: FieldFlags.Required);
```

`AddEnumLookupId()` creates an integer lookup field and registers the enum lookup source when needed.

The stored value is the enum integer value.

The display text is the enum member name.

## Custom LookupSource

Applications may provide a custom lookup source class.

```csharp
DataRegistry.AddLookupWithClassName("MyLookup", typeof(MyLookupSource).FullName);
```

A custom lookup source is useful when lookup items must come from custom logic instead of a table, SQL statement or enum.

The custom class should derive from `LookupSource`.

## Null Item

A lookup may include an empty item.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName("Category", "Category", UseNullItem: true);
```

`UseNullItem` is useful for optional lookup fields.

Required lookup fields normally do not need a null item.

## Lookup Fields

A table field references a lookup by name.

String lookup field:

```csharp
Table.AddStringLookupId("CustomerId", "Customer", TitleKey: "Customer", Flags: FieldFlags.Required);
```

Integer lookup field:

```csharp
Table.AddIntegerLookupId("ActivityTypeId", "ActivityType", TitleKey: "Activity Type", Flags: FieldFlags.Required);
```

Generic lookup field:

```csharp
Table.AddLookupId("StatusId", DataFieldType.Integer, "Status", Flags: FieldFlags.Required);
```

The field data type must match the stored value type.

## Registration Order

Lookups should be registered before modules.

The sample registry uses this order:

```csharp
Version.RegisterLookupSources();
Version.RegisterLocators();
Version.RegisterModules();
```

Modules may declare fields that reference lookups by name.

Those names are resolved later by:

```csharp
DataRegistry.Modules.UpdateReferences();
```

Registering lookups first makes module declarations easier to validate and understand.

## Lookup Versus Locator

Lookups and locators are related, but they solve different problems.

A lookup is for choosing from a known list of values.

Examples:

- Status.
- Category.
- Activity type.
- User level enum.

A locator is for searching and selecting a row from another table.

Examples:

- Customer.
- Contact.
- Product.
- Document.

Use a lookup when the list is small, stable or naturally presented as a dropdown-like value.

Use a locator when the user must search a larger table, inspect multiple columns or select a related row through a search dialog.

## Practical Rule

When declaring lookups manually:

- Register lookup sources before modules.
- Use table-backed lookups for editable lookup tables.
- Use enum lookups for fixed enum values.
- Use SQL lookups when the list needs filtering or calculated display fields.
- Use custom `LookupSource` classes only when lookup loading needs custom logic.
- Set `ValueField` and `DisplayField` explicitly when defaults are not correct.
- Match lookup field data type with the lookup value type.
- Use `UseNullItem` only for optional values.

## Related Articles

- [Modules](modules.md)
- [Tables and Fields](tables-and-fields.md)
- [Locators](locators.md)
- [Select Definitions](select-definitions.md)

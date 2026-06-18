# Modules

`ModuleDef` is the central descriptor of a data module.

A module tells Tripous how an application works with a logical data area:

- Which `DataModule` class is used.
- Which SQL query is used for the list view.
- Which filters are available.
- Which table is edited.
- Which fields belong to that table.
- Which lookup fields, locator fields, details and joins exist.
- Which security level is required.
- Which options control module behavior.

The module does not replace SQL and it does not replace the database schema.

The schema creates the database objects. The module describes how the application uses those objects.

## Creating A Module

Modules are registered in `DataRegistry`.

```csharp
ModuleDef Module = DataRegistry.AddModule(
    "TodoTask",
    TitleKey: "ToDo",
    ClassName: typeof(ToDoDataModule).FullName,
    ListSelectSql: SqlText,
    IsSingleSelect: true);
```

The important arguments are:

- `Name` is the module name used by registries and forms.
- `TitleKey` is the display title key.
- `ClassName` is the `DataModule` class used at runtime.
- `ListSelectSql` is the SQL used by the module list.
- `IsSingleSelect` indicates that the module uses one fixed list select.
- `SecurityLevel` may restrict access to the module.

If `ClassName` is omitted, Tripous uses the default `DataModule`.

Custom modules use a custom subclass:

```csharp
ModuleDef Module = DataRegistry.AddModule(
    "Customer",
    TitleKey: "Customers",
    ClassName: typeof(MiniCrmDataModule).FullName,
    ListSelectSql: SqlText,
    IsSingleSelect: true);
```

## List SQL

The list SQL defines what appears in the list part of the module.

It can be simple SQL or it can include joins and calculated columns.

```csharp
string SqlText = @"
select
    t.Id
   ,t.Title
   ,s.Name as Status
   ,t.DueDate
   ,t.Priority
   ,t.IsDone
   ,t.CreatedAt
   ,t.UpdatedAt
from
    TodoTask t
        left join TodoStatus s on s.Id = t.TodoStatusId
order by
    t.IsDone,
    t.Priority desc,
    t.DueDate,
    t.Title
";
```

This SQL is independent from the editable table definition.

The list can join lookup tables, use aliases, calculate display columns and order rows in whatever way the application needs.

## Select Definitions And Filters

Every module has a `SelectList`.

The first select is created from the `ListSelectSql` argument.

Filters are added to the corresponding `SelectDef`.

```csharp
SelectDef SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Title", FieldName: "t.Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("Status", FieldName: "s.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("DueDate", FieldName: "t.DueDate", FilterDataType: DataFieldType.Date, ConditionOp: ConditionOp.Between);
SelectDef.AddFilter("IsDone", FieldName: "t.IsDone", FilterDataType: DataFieldType.Boolean);
```

A filter describes:

- The filter name.
- The actual SQL field expression.
- The filter data type.
- The condition operation.

The `FieldName` argument is important when the list SQL uses aliases or joins.

For example, the list column may be named `Status`, but the actual SQL filter must use `s.Name`.

Boolean fields in Tripous database tables are integer-backed `0` and `1` values. The filter metadata lets the presentation layer display them as boolean values while the generated SQL uses the database field.

## Editable Table

The module table describes the editable table, not the list SQL result.

```csharp
TableDef Table = Module.Table;
Table.Name = "TodoTask";
Table.AddId();
Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddString("Description", 4000, Flags: FieldFlags.Memo);
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
Table.AddDate("DueDate");
Table.AddDateTime("CompletedAt", Flags: FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
Table.AddInteger("Priority", Flags: FieldFlags.Required);
Table.AddBoolean("IsDone");
Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
```

The `TableDef` tells Tripous:

- The database table name.
- The primary key field.
- The fields that participate in insert and update operations.
- Field data types and sizes.
- Field flags.
- Lookup fields.
- Locator fields.
- Detail tables.
- Join metadata.

The table definition should match the database schema, but it is not the schema itself.

## Field Flags

`FieldFlags` describe important field behavior.

Common flags include:

- `Required` marks a field as required.
- `Searchable` marks a field as searchable.
- `Hidden` hides a field from normal presentation.
- `ReadOnly` makes a field read-only.
- `ReadOnlyUI` makes a field read-only in presentation controls.
- `ReadOnlyEdit` allows editing only when inserting.
- `Memo` marks a multiline text field.
- `LargeMemo` marks a large multiline text field.
- `Boolean` marks an integer-backed boolean field.
- `NoInsertUpdate` excludes a field from insert and update SQL.
- `ForeignKey` marks a foreign key field.
- `Extra` marks a field that does not exist in the database.

Flags are combined with bitwise OR.

```csharp
Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI);
```

This is often used for generated codes.

```csharp
Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetCodeProviderName("CUSTOMER");
```

The field is required and searchable, but normal editing is blocked because the value comes from a code provider.

## Lookup Fields

Lookup fields connect stored values with display values.

```csharp
Table.AddIntegerLookupId("TodoStatusId", "TodoStatus", TitleKey: "Status", Flags: FieldFlags.Required);
```

This field stores an integer id in `TodoTask.TodoStatusId`.

The lookup named `TodoStatus` supplies the display value.

Lookup definitions are registered separately, but modules reference them by name.

## Locator Fields

Locator fields are used when the user must search and select a related row.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";
```

The field stores the selected key.

The locator named `Customer` defines how the user searches for and selects that key.

## Detail Tables

A module may include detail tables.

The `Customer` module in `04-mini-crm` declares child tables for contacts and activities.

```csharp
TableDef Contact = Table.AddDetail("Contact", "Id", "CustomerId");
Contact.AddId();
Contact.AddString("CustomerId", 40, Flags: FieldFlags.Required | FieldFlags.Hidden);
Contact.AddString("FirstName", 64, Flags: FieldFlags.Required);
Contact.AddString("LastName", 64, Flags: FieldFlags.Required);
Contact.AddString("Email", 96);
Contact.AddBoolean("IsPrimaryContact", Flags: FieldFlags.Required);
Contact.AddTextBlob("Notes").SetMemo();
```

`AddDetail()` declares the master-detail relationship.

- `Contact` is the detail table.
- `Id` is the master field on the parent table.
- `CustomerId` is the foreign key field on the detail table.

The detail table then declares its own fields.

## Joins

Joins let a table definition include fields from a related table.

They are useful when a field stores a foreign key but the module also needs related display columns.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";

TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
CustomerJoin.AddString("Code", 40);
CustomerJoin.AddString("Name", 128);
```

This describes a join from the current table to `Customer`.

The current table field is `CustomerId`. The foreign table is `Customer`. The foreign primary key is `Id`.

Joined fields are not the same as editable fields on the main table. They describe related data that can be selected or displayed with the module.

## Module Options

Important `ModuleDef` properties include:

- `Name` identifies the module.
- `TitleKey` is the display title key.
- `ClassName` points to the runtime `DataModule` class.
- `ConnectionName` selects the database connection.
- `IsSingleSelect` controls whether the module uses a fixed single select.
- `UseFilters` controls whether filters are used by presentation forms.
- `SecurityLevel` controls the minimum user level required for access.
- `GuidOids` controls whether ids are GUID strings.
- `CascadeDeletes` controls whether deletes happen bottom-to-top through detail tables.
- `ItemCaptionField` controls which field is used as the item caption.
- `DetailOrder` controls preferred detail table ordering.
- `Table` is the top editable table.
- `SelectList` contains list selects and filters.
- `Stocks` contains additional named selects loaded for module use.

Most small modules only need `Name`, `TitleKey`, `ClassName`, `ListSelectSql`, `IsSingleSelect`, `Table` and filters.

Larger modules use the rest when they need custom access rules, detail ordering, code generation or additional select data.

## Reference Resolution

Modules often refer to other descriptors by name.

Examples:

- Lookup fields refer to `LookupDef` names.
- Locator fields refer to `LocatorDef` names.
- Joined fields refer to related tables.
- Detail tables refer to parent and child key fields.

Those references are resolved after all descriptors are registered.

The sample registry calls:

```csharp
DataRegistry.Modules.UpdateReferences();
```

This is why lookup sources and locators are registered before modules, and why reference resolution happens after all modules are registered.

## Related Articles

- [Declaration Order](declaration-order.md)
- [Tables and Fields](tables-and-fields.md)
- [Select Definitions](select-definitions.md)
- [Lookups](lookups.md)
- [Locators](locators.md)
- [Code Providers](code-providers.md)

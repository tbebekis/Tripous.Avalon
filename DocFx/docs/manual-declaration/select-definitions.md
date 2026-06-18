# Select Definitions

`SelectDef` describes a SELECT statement used by a module.

It is normally used for the list view of a form, for browser grids and for any module workflow that needs a read projection over table data.

A select definition is not the editable table.

The editable table is described by `TableDef`.

The list projection is described by `SelectDef`.

## Role

A `SelectDef` declares:

- The SQL text.
- The list columns returned by that SQL.
- The filters that can be applied to that SQL.
- Optional display labels for returned columns.
- Optional column type hints for returned columns.

This allows the list view to show data that is more useful than a plain `select * from TableName`.

The list SQL may:

- Join lookup tables.
- Join reference tables.
- Return aliases.
- Return calculated columns.
- Return formatted display columns.
- Apply default ordering.

The module table still remains the source of truth for insert, edit, delete and save operations.

## Default Select

When a module is registered, Tripous creates the first select definition automatically.

```csharp
ModuleDef Module = DataRegistry.AddModule("Category", TitleKey: "Categories", ListSelectSql: SqlText, IsSingleSelect: true);
SelectDef SelectDef = Module.SelectList[0];
```

The first select is the default list select.

If `ListSelectSql` is not provided, Tripous uses a simple default SQL statement.

```csharp
select * from Category
```

For real modules, it is usually better to provide explicit list SQL.

## List SQL

The `05-password-manager` sample uses a list select that joins `Credential` with `Category`.

```csharp
string SqlText = @"
select
    c.Id
   ,c.Title
   ,c.UserName
   ,c.Url
   ,cat.Name as Category
   ,c.CreatedAt
   ,c.UpdatedAt
from
    Credential c
        left join Category cat on cat.Id = c.CategoryId
order by
    c.Title
";

ModuleDef Module = DataRegistry.AddModule("Credential", TitleKey: "Credentials", ClassName: typeof(CredentialDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
```

The list returns `Category` as a display column.

The editable table still stores `CategoryId`.

```csharp
TableDef Table = Module.Table;
Table.Name = "Credential";
Table.AddIntegerLookupId("CategoryId", "Category", TitleKey: "Category", Flags: FieldFlags.Required);
```

This is the normal pattern:

- The table stores ids and editable values.
- The select shows user-friendly list columns.
- The SQL aliases become list column names.

## Filters

Filters are added to the `SelectDef`.

```csharp
SelectDef SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Title", FieldName: "c.Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("UserName", FieldName: "c.UserName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("Url", FieldName: "c.Url", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("Category", FieldName: "cat.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
SelectDef.AddFilter("UpdatedAt", FieldName: "c.UpdatedAt", FilterDataType: DataFieldType.DateTime, ConditionOp: ConditionOp.Between);
```

A filter declares:

- `Name`, the filter name shown to the application.
- `FieldName`, the SQL field or expression used in the generated WHERE clause.
- `FilterDataType`, the value type of the filter.
- `ConditionOp`, the comparison operation.
- `BoolOp`, the boolean operator used with other filters.
- `TitleKey`, the optional display title key.

`FieldName` is important when the visible list column is an alias.

For example, the list column may be `Category`, but the filter must use `cat.Name`.

```csharp
SelectDef.AddFilter("Category", FieldName: "cat.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
```

## Filter Types

Common filter types are:

- `DataFieldType.String` with `ConditionOp.Contains`.
- `DataFieldType.Date` with `ConditionOp.Between`.
- `DataFieldType.DateTime` with `ConditionOp.Between`.
- `DataFieldType.Integer` with `ConditionOp.Equal`.
- `DataFieldType.Decimal` with `ConditionOp.Equal` or `ConditionOp.Between`.
- `DataFieldType.Boolean` with boolean UI handling.

Boolean fields in Tripous tables are integer-backed values.

The filter metadata lets the UI present them as boolean values while the generated SQL still targets the stored field.

## Display Labels

`DisplayLabels` can override list column titles.

```csharp
SelectDef.DisplayLabels["UserName"] = "User name";
SelectDef.DisplayLabels["UpdatedAt"] = "Updated";
```

Use this when the returned column name is correct for SQL but not ideal as a user-visible title.

Do not use labels to hide unclear SQL aliases.

Prefer clear aliases first.

## Column Types

`ColumnTypes` can provide explicit list column type hints.

```csharp
SelectDef.ColumnTypes["Title"] = DataColumnType.Text;
SelectDef.ColumnTypes["UpdatedAt"] = DataColumnType.DateTime;
SelectDef.ColumnTypes["Category"] = DataColumnType.Text;
```

Column type hints help presentation code choose the right display behavior.

They are especially useful for generated declarations where SQL result columns may come from joins, lookups or calculated expressions.

## Multiple Selects

A module owns a `SelectList`.

The first item is the default list select.

Additional select definitions may be added when the same module needs alternate list projections.

```csharp
Module.SelectList.Add("RecentlyUpdated", @"
select
    Id,
    Title,
    UpdatedAt
from Credential
order by UpdatedAt desc
");
```

Keep alternate selects aligned with the same module key field when they are used to open the same item form.

## Generated Select Definitions

The Registration Builder generates select definitions from schema metadata.

The generated code follows the same model as manual registration.

```csharp
Module = DataRegistry.AddOrUpdateModule("Customer", ListSelectSql: SqlText);
SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Code", FieldName: "Code", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("Name", FieldName: "Name", FilterDataType: DataFieldType.String);
SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
```

Manual declaration and generated declaration use the same `SelectDef` object.

## Practical Rule

When declaring select definitions manually:

- Treat `SelectDef` as the module list projection.
- Treat `TableDef` as the editable table structure.
- Always include the key field needed to open the item.
- Use clear SQL aliases for visible columns.
- Use `FieldName` when a filter targets a joined table or SQL expression.
- Add filters only for columns that make sense in search.
- Prefer `Contains` for text filters.
- Prefer `Between` for date and date-time filters.
- Add `ColumnTypes` when SQL aliases or expressions make the type unclear.
- Keep list SQL readable and provider-neutral where possible.

## Related Articles

- [Modules](modules.md)
- [Tables and Fields](tables-and-fields.md)
- [Forms](forms.md)
- [Lookups](lookups.md)
- [Locators](locators.md)

# Select And Filter Descriptors

Tripous.Data uses `SelectDef` and filter descriptors to describe list SELECTs and their runtime filters.
This is the metadata behind browser/list views, filter panels, and generated list SQL.

This article covers structured filter descriptors, based on `SqlFilterDef` and `SqlFilterDefs`.
Inline SQL filter expression tags are a separate feature with a different purpose.

## SelectDef

`SelectDef` describes a named SELECT statement.
It stores SQL text, display labels, column type hints, and filter definitions.

```csharp
SelectDef SelectDef = Module.SelectList[0];

SelectDef.SqlText = @"
select
    Id,
    Code,
    Name,
    CreatedAt
from Customer
order by Name";
```

Display labels are used when Tripous creates columns for list grids.

```csharp
SelectDef.DisplayLabels["Code"] = "Code";
SelectDef.DisplayLabels["Name"] = "Name";
```

## Adding Filters

Filters are added to `SelectDef.FilterDefs`.
The usual shortcut is `SelectDef.AddFilter()`.

```csharp
SelectDef.AddFilter(
    "Name",
    FilterDataType: DataFieldType.String,
    ConditionOp: ConditionOp.Contains);
```

For a date range:

```csharp
SelectDef.AddFilter(
    "CreatedAt",
    FilterDataType: DataFieldType.DateTime,
    ConditionOp: ConditionOp.Between);
```

For a joined field or aliased column, use `FieldName`.

```csharp
SelectDef.AddFilter(
    "Customer",
    FieldName: "c.Name",
    FilterDataType: DataFieldType.String,
    ConditionOp: ConditionOp.Contains);
```

`Name` is the filter name shown in metadata.
`FieldName` is the field used in SQL.
When `FieldName` is empty, it falls back to `Name`.

## SqlFilterDef

`SqlFilterDef` describes one condition.

The important parts are:

- `FieldName`, the field or SQL expression to filter.
- `FilterDataType`, such as string, integer, decimal, date, or boolean.
- `ConditionOp`, such as equal, contains, between, or in.
- `BoolOp`, such as and/or.
- `Value` and `Value2`, the runtime values.

```csharp
SqlFilterDef Filter = new();

Filter.Name = "Name";
Filter.FieldName = "Customer.Name";
Filter.FilterDataType = DataFieldType.String;
Filter.ConditionOp = ConditionOp.Contains;
Filter.BoolOp = BoolOp.And;
Filter.Value = "acme";
```

`Between` uses both values.

```csharp
SqlFilterDef Filter = new();

Filter.Name = "CreatedAt";
Filter.FilterDataType = DataFieldType.DateTime;
Filter.ConditionOp = ConditionOp.Between;
Filter.Value = new DateTime(2026, 1, 1);
Filter.Value2 = new DateTime(2026, 1, 31);
```

`In` uses a collection value.

```csharp
SqlFilterDef Filter = new();

Filter.Name = "StatusId";
Filter.FilterDataType = DataFieldType.Integer;
Filter.ConditionOp = ConditionOp.In;
Filter.Value = new[] { 1, 2, 3 };
```

## Formatting SQL WHERE

`SqlFilterDefs` can produce SQL WHERE text.
Prefer the parameterized formatter for database execution.

```csharp
SqlFilterDefs Filters = new();

SqlFilterDef NameFilter = Filters.Add(
    "Name",
    FilterDataType: DataFieldType.String,
    ConditionOp: ConditionOp.Contains);

NameFilter.Value = "acme";

Dictionary<string, object> Params = new();
string WhereText = Filters.GetSqlWhereFilterTextParameterized(Params);
```

The generated WHERE fragment uses Tripous parameters.

```sql
Name like :Name
```

The `Params` dictionary receives the parameter values.
At execution time, `SqlProvider` translates the global `:` prefix to the native RDBMS prefix.

Inline SQL is also available.

```csharp
string WhereText = Filters.GetSqlWhereFilterTextInline();
```

Use inline SQL only for trusted/generated values.
For user input, use parameterized SQL.

## Formatting DataView RowFilter

The same filter definitions can produce a `DataView.RowFilter`.

```csharp
string RowFilter = Filters.GetDataViewRowFilterText();

Table.UserRowFilter = RowFilter;
```

This is useful when filtering an in-memory `MemTable.DataView`.

## Boolean Filters

Tripous database booleans are often integer-backed `0` / `1` values.
`SelectDef.ValidateBooleanFilterTypes()` checks that boolean filters are backed by integer-compatible SELECT columns.

```csharp
SelectDef.ValidateBooleanFilterTypes(
    Module.Name,
    Store,
    SelectDef.FilterDefs);
```

This avoids a mismatch between UI boolean filters and the actual SELECT schema.

## Auto-Defined Filters

`SelectDef.DefineFilters()` can inspect a SELECT schema and suggest filters for common column names such as `Code`, `Name`, `Date`, `Amount`, and `Price`.

```csharp
SqlFilterDefs Filters = SelectDef.DefineFilters(Module.Name, Store);
```

This is useful for generated modules and list views, where common filters can be inferred from the SELECT result.

## Common Operators

Common `ConditionOp` values are:

- `Equal`.
- `NotEqual`.
- `Greater`.
- `GreaterOrEqual`.
- `Less`.
- `LessOrEqual`.
- `Like`.
- `Contains`.
- `StartsWith`.
- `EndsWith`.
- `Between`.
- `In`.
- `Null`.

Common `BoolOp` values are:

- `And`.
- `Or`.
- `AndNot`.
- `OrNot`.

## Where It Is Used

Select and filter descriptors are used by:

- Module list SELECTs.
- Browser/list grids.
- SQL filter panels.
- Generated RegBuilder module code.
- `SelectSql` WHERE extension.
- `SqlWhereFilterFormatter`.
- `DataViewRowFilterFormatter`.

The important idea is that the filter definition is metadata.
The same definition can drive UI input, SQL WHERE generation, and in-memory `DataView` filtering.

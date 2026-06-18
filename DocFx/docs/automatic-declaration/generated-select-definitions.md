# Generated Select Definitions

Generated select definitions are created inside `RegistryVersionN.Modules.cs`.

They are not written to a separate generated file.

Each generated module receives a list SELECT SQL statement and a `SelectDef` descriptor.

The list SELECT is used by the module list view, filtering and column metadata.

## Module List SQL

The Registration Builder creates list SQL for each generated module.

Example shape:

```csharp
SqlText = @"
select
   Account.Id,
   Account.Code,
   Account.Name,
   Account.AccountTypeId,
   case
      when Account.AccountTypeId = 1 then 'Asset'
      when Account.AccountTypeId = 2 then 'Liability'
      else ''
   end as AccountType
from
  Account
";
Module = DataRegistry.AddOrUpdateModule("Account", ClassName: "AccountDataModule", ListSelectSql: SqlText);
```

The generated SQL becomes the module's list select.

## SelectDef Access

After the module is registered, the generated code uses the first select definition.

```csharp
SelectDef = Module.SelectList[0];
```

The builder then adds filters and column type metadata to that `SelectDef`.

## Generated Columns

Generated list SQL may include:

- top table fields
- enum display columns
- lookup display columns
- locator display columns
- generated join aliases
- calculated display columns

There is no metadata syntax for selecting an arbitrary subset of list SELECT fields.

The builder includes every top table field except:

- blob fields
- fields marked with `Hidden`

`FilterFields` controls generated filters only.

It does not remove columns from the generated list SELECT.

Enum fields usually produce both the id field and a display column.

Example:

```sql
Account.AccountTypeId,
case
   when Account.AccountTypeId = 1 then 'Asset'
   when Account.AccountTypeId = 2 then 'Liability'
   else ''
end as AccountType
```

Here `AccountTypeId` is the stored value and `AccountType` is the display value.

## Join Aliases

Generated join aliases use this convention:

```text
JOIN_ALIAS__FIELD_NAME
```

Example:

```sql
COALESCE(NumberSeries.Code, '') as NumberSeries__Code,
COALESCE(NumberSeries.Name, '') as NumberSeries__Name
```

These aliases are final list select column names.

They can be used by filters and `FilterFields`.

For foreign-key joins, the generated list SELECT adds only common display string fields from the joined table:

- `Code`
- `Name`
- `Title`

`Id` is not added as a display alias.

Non-string joined fields are not added automatically to the list SELECT.

## ListWhere

`ListWhere` metadata adds a module-specific condition to the generated list SQL.

Example:

```sql
ListWhere: DocumentType.ModuleName = 'SalesInvoice'
```

The metadata value is only the condition.

Do not include the `WHERE` keyword.

This is useful when multiple modules share the same physical table.

For example, sales orders, invoices and credit notes may all use the `Trade` table but each module receives a different list condition.

## FilterFields

`FilterFields` controls which list columns become filters and in what order.

Example:

```sql
FilterFields: Code, TradeDate, Person__Code, Person__Name, TradeStatus, ExternalRef, TotalAmount
```

Generated filters:

```csharp
SelectDef.AddFilter("Code", FieldName: "Code", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("Person__Code", FieldName: "Person__Code", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("TradeStatus", FieldName: "TradeStatus", FilterDataType: DataFieldType.String);
```

Names are resolved against the final list SELECT columns.

This means `FilterFields` may reference:

- normal fields such as `Code`
- enum display columns such as `TradeStatus`
- join aliases such as `Person__Name`

Unknown or duplicate names are validation errors.

## Automatic Filters

When `FilterFields` is omitted, the builder uses automatic filter generation.

Automatic filters are based on the generated list columns and their data types.

Use `FilterFields` when the list should expose a smaller or more deliberate filter set.

## Column Types

Generated select definitions store column type metadata.

Example:

```csharp
SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
SelectDef.ColumnTypes["Amount"] = DataColumnType.Currency;
SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
```

Column types help the UI and filtering system understand how each column should be handled.

## Source Of Column Types

Column types are inferred from:

- schema field types
- enum display columns
- lookup and locator join columns
- generated calculated columns

For example:

- `@NVARCHAR(40)` becomes text
- `@BOOL` becomes boolean
- `@DATE` becomes date
- `@DATE_TIME` becomes date-time
- decimal amount fields may become decimal or currency columns

## Relationship With Modules

Select definitions belong to modules.

The generated module registers its list SQL first.

Then it configures the `SelectDef`.

This keeps the list view, filters and table descriptor aligned.

## Manual Edits

Do not edit generated select definitions manually inside `RegistryVersionN.Modules.cs`.

When generated list SQL or filters need to change:

- change field metadata
- change `ListWhere`
- change `FilterFields`
- change lookup or locator metadata
- run the Registration Builder again
- review the generated diff

Use handwritten registry update methods only for deliberate application-specific adjustments.

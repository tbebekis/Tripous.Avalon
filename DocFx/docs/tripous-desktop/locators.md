# Locators

Locators are searchable reference selectors.
They are used when a field stores one key value, but the user needs to search and see more information from the referenced table.

For example, a document row may store `CustomerId`, while the UI shows customer code and customer name.

## Main Types

- `LocatorDef` describes the source table, key field, visible fields, and optional locator form.
- `LocatorFieldDef` describes one visible or searchable field of the locator source.
- `Locator` performs the search and assigns selected values to the target row.
- `LocatorBox` is the item form control for locator fields.
- `GridLocatorBox` is the locator editor used inside detail grids.
- `LocatorTargetFieldMap` maps locator source fields or aliases to target row fields.

## Locator Definition

A locator definition belongs to the data registry.
It describes where values come from and which fields the UI can show or search.

```csharp
LocatorDef LocatorDef = DataRegistry.AddLocator(
    "Customer",
    "Customer",
    "Id");

LocatorDef.Add("Code", DataFieldType.String, "CustomerCode");
LocatorDef.Add("Name", DataFieldType.String, "CustomerName");
```

The locator returns one key value, but it can display multiple fields.
The key field is assigned to the bound field, while mapped visible fields can be assigned to snapshot fields.

## Locator Fields

`LocatorFieldDef.Name` is a source field name.
`LocatorFieldDef.TargetField` is the target row field that receives the selected value.

Common field settings are:

- `Alias`, used when the source select exposes a different column name.
- `DataType`, used for display and search behavior.
- `TargetField`, used when the selected value should be copied to the target row.
- `IsVisible`, used by locator controls and grid locator columns.
- `IsSearchable`, used by the locator search UI.
- `DisplayWidth`, used by `LocatorBox` text boxes.

## LocatorBox

`LocatorBox` is used in item forms.
It displays the visible locator fields, opens the locator search UI, and raises `RowSelected` when the user chooses a source row.

Binding is handled by `ControlBindingHelper`.

```csharp
LocatorBox Box = new();
ControlBinding Binding = Binder.Bind(Box, CustomerIdField);
```

During binding Tripous:

- finds the `LocatorDef` from `FieldDef.Locator`.
- creates a `Locator` instance.
- initializes the locator.
- assigns the locator to the `LocatorBox`.
- creates the target-field map from the target `TableDef`.
- refreshes the visible text boxes from the current row.

When the user selects a row, Tripous calls `Locator.Assign()`.

```csharp
Locator.Assign(
    Args.Row,
    Row,
    Binding.FieldName,
    Binding.LocatorTargetFieldMap);
```

This writes the selected key to the bound field and writes any mapped locator fields to the target row.

## Target Field Mapping

Target-field mapping is the most important part of locator assignment.
The same locator definition may be reused in different modules, grids, and SQL projections.
Only the binding layer knows which target row fields exist in the current context.

Tripous therefore builds a map at binding time.

```csharp
Dictionary<string, string> Map =
    FieldDef.TableDef.CreateLocatorTargetFieldMap(FieldDef, LocatorDef);
```

The map accepts both locator field names and locator field aliases.
It can target two different kinds of fields:

- virtual fields that come from joins and appear only in the select result.
- actual database fields that store snapshots, such as `CustomerName` or `ProductName`.

This lets a locator work both with joined read models and editable tables that store snapshot values.

## Grid Locators

Detail grids use `GridLocatorBox`.
`DataGridBinder.CreateLocatorColumn()` creates a display column and an edit template for the locator field.

When editing starts, the grid locator is initialized with:

- the `LocatorDef`.
- the visible `LocatorFieldDef`.
- the current `DataRowView`.
- the key field name.
- the target-field map.

```csharp
GridLocatorBox Box = new();
Box.Initialize(
    LocatorDef,
    LocatorFieldDef,
    RowView,
    "ProductId",
    TargetFieldMap);
```

When the user selects a source row, `GridLocatorBox` assigns the locator values to the current detail row.
It also refreshes other locator columns in the same grid so snapshot values become visible immediately.

## Detail Grid Columns

`UiItemDetails` creates locator columns from locator visible fields.
It skips the locator key field and creates one visible column for each useful locator field.

It also skips raw locator snapshot fields when creating ordinary detail grid columns.
Those fields are still stored in the row when needed, but the user edits them through the locator UI.

This gives a natural grid experience:

- the user searches by code, name, or another visible locator field.
- the row stores the key field.
- mapped snapshot fields are assigned automatically.
- visible locator columns refresh after selection.

## Clearing Values

A locator can also clear values.
When `Locator.Assign()` receives no source row, it clears the key field and mapped target fields.

This behavior is used when a locator control is cleared by the user.

## Read-Only Rules

A locator is read-only when either the field or the locator definition is read-only.

```csharp
Box.IsReadOnly =
    FieldDef.IsReadOnly
    || FieldDef.IsReadOnlyUI
    || LocatorDef.IsReadOnly;
```

In grids, individual locator fields may also be read-only when the locator field is not searchable.

## Practical Notes

- Use locators for large reference tables or references that need searchable display fields.
- Use simple lookups for small fixed lists.
- Define visible and searchable locator fields deliberately.
- Keep locator snapshot fields mapped through metadata, not manually assigned in UI code.
- Remember that the locator key and the visible locator fields are separate concerns.

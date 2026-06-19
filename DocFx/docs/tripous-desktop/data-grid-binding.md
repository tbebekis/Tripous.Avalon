# DataGrid Binding

Tripous.Desktop binds Avalonia `DataGrid` controls to `DataView` and `DataRowView` objects through `DataGridBinder`.
The binder creates the grid columns, stores column metadata in `GridColumnBinding`, and keeps the grid aligned with Tripous `TableDef`, `FieldDef`, `LookupDef`, and `LocatorDef` metadata.

## Main Types

- `DataGridBinder` is the static helper that creates columns and binds grids.
- `GridColumnBinding` is attached to each grid column through `DataGridColumn.Tag`.
- `DataViewItemsSource` exposes a `DataView` as an Avalonia-friendly item source.
- `GridEditController` handles editing behavior in detail grids.
- `LookupSource` supplies display values for lookup columns.
- `LocatorDef` and `GridLocatorBox` support locator columns in editable grids.

## Binding A Grid

Use `DataGridBinder.BindGrid()` instead of assigning `ItemsSource` directly.
The binder clears previous columns, creates the right column types, wraps the `DataView` in a `DataViewItemsSource`, and optionally selects the first row.

```csharp
DataView DataView = Module.tblList.DefaultView;

DataGridBinder.BindGrid(
    Grid,
    DataView,
    SupportsRecycling: false,
    GoToFirst: true);
```

When the same grid is rebound, unbind it first or let `BindGrid()` do it.

```csharp
DataGridBinder.UnBindGrid(Grid);
DataGridBinder.BindGrid(Grid, DataView);
```

`DataViewItemsSource` is important because it follows `DataView.ListChanged` notifications and keeps the visual rows synchronized with the underlying table view.

## Binding With SelectDef

List grids can be bound with a `SelectDef`.
In that mode `DataGridBinder` creates columns from the `DataView`, hides columns that are not part of the select definition, and makes the grid read-only.

```csharp
SelectDef SelectDef = ModuleDef.Selects[0];
DataView DataView = Module.tblList.DefaultView;

DataGridBinder.BindGrid(
    SelectDef,
    Grid,
    DataView,
    SupportsRecycling: false,
    GoToFirst: true);
```

This is the usual case for browse/list pages where the SQL select defines the visible projection.

## Column Metadata

Every column created by `DataGridBinder` gets a `GridColumnBinding` in `DataGridColumn.Tag`.
That object connects the visual column with the Tripous metadata behind it.

```csharp
foreach (GridColumnBinding Binding in Grid.GetInfoList())
{
    if (Binding.IsPlainId)
        Binding.GridColumn.IsVisible = false;
}
```

Useful metadata includes:

- `FieldName`, the actual data field.
- `DisplayFieldName`, the field shown to the user when it differs from the stored field.
- `FieldDef`, `LookupDef`, and `LocatorDef`, when schema metadata exists.
- `LookupSource`, for lookup columns.
- `LocatorTargetFieldMap`, for locator snapshot assignment.
- `IsReference`, which identifies lookup and locator-backed columns.
- `IsPlainId`, which identifies simple `Id` columns that are not references.

## Generated Columns

`DataGridBinder` can create columns from:

- `DataColumn` objects.
- `DataTable` objects.
- `FieldDef` objects.
- `TableDef` objects.
- explicit column name, header, and `DataFieldType`.

Boolean fields become checkbox columns.
Other fields usually become text columns.
Headers are derived from captions or field names, and the trailing `Id` is stripped from plain identifiers.

Date and date-time fields also get date-aware formatting.
Names ending in `Date` use `Sys.Settings.DateFormat`.
Names ending in `DateTime` or `DT` use `Sys.Settings.DateTimeFormat`.

## Lookup Columns

Lookup fields are stored as keys but displayed as user text.
`DataGridBinder` creates a display template that shows the lookup item text and an edit template that uses a `ComboBox`.

```csharp
DataGridColumn Column = DataGridBinder.CreateLookupColumn(FieldDef);
Grid.Columns.Add(Column);
```

When the user selects a lookup item, the grid writes `LookupItem.Value` to the data row.
If the table defines lookup snapshots, Tripous assigns those snapshot fields at the same time.

The active lookup combo box is also tracked in `GridColumnBinding.ActiveLookupComboBox`.
That allows higher-level code to refresh or inspect the currently open lookup editor.

## Locator Columns

Locator columns are used when a detail grid needs a search dialog instead of a simple combo box.
The edit template hosts a `GridLocatorBox`, initialized with the `LocatorDef`, the locator field, the current row, and the target-field map.

The target-field map is important because locator selection can fill more than one field.
Tripous supports both cases:

- virtual fields that come from joins and only appear in the select result.
- actual database fields that store snapshots, such as `ProductName`.

Detail grids do not show locator snapshot fields as raw columns.
Instead, `UiItemDetails` uses the locator visible fields and the target-field mapping to create the user-facing locator columns.

## Editing And Focus

Grid edit templates work directly with `DataRowView`.
When editing starts, the row enters edit mode.
When the value changes, Tripous converts the value according to the target `DataColumn` type and writes it to the row.

Checkbox and lookup editors commit immediately.
After a commit, focus is restored to the grid cell so keyboard navigation remains natural.
Escape cancels lookup editing, and `Alt+Down` opens the lookup combo box.

## Detail Grids

`UiItemDetails` uses `DataGridBinder` for detail grid columns.
It skips fields that do not belong in a compact editable grid, such as memo, large memo, image, and raw locator snapshot fields.

```csharp
DataGrid Grid = new DataGrid();

DataGridBinder.CreateColumns(Grid, TableDef);
GridEditController.Attach(Grid);
DataGridBinder.BindGrid(Grid, DetailTable.DefaultView);
```

In generated item pages this is done by the framework.
Application code usually defines the table metadata and lets `UiItemDetails` build the grid.

## Recycling

Most binder methods accept `SupportsRecycling`.
This value is passed to Avalonia `FuncDataTemplate`.

Display templates listen for row column changes because recycled cells may not refresh after programmatic updates.
Edit templates avoid that refresh logic so they do not overwrite typing or selection state while the user is editing.

## Practical Notes

- Prefer `DataGridBinder` for Tripous grids instead of manual Avalonia binding.
- Call `UnBindGrid()` when a grid is reused with a different data source.
- Use `Grid.GetInfoList()` when code needs to inspect generated column metadata.
- Keep locator snapshot fields out of the visible detail grid unless there is a deliberate reason to show them.

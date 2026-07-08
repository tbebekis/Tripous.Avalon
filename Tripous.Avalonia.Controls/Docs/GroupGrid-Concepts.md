# GroupGrid Concepts

`GroupGrid` is a custom-rendered Avalonia grid for dense business screens. It is not a wrapper around Avalonia `DataGrid`. It is built around a small runtime model that separates the visual control, the non-visual engine, column descriptors, data adapters, projection rows, editing, settings, and export.

This document is a short conceptual map for developers who want to understand how the control is meant to be used and extended.

## Main Idea

`GroupGrid` treats the grid as a runtime, not only as a visual table.

The public control is responsible for Avalonia integration, input handling, drawing, in-place editors, menus, dialogs, and toolbars. The engine owns the non-visual state: columns, data access, grouping, sorting, filtering, summaries, current cell, selected cell, edit state, and the visible projection.

The important split is:

- `GroupGrid` is the Avalonia control.
- `GroupGridEngine` is the non-visual runtime.
- `IGroupGridDataAdapter` is the data access contract.
- `GroupGridColumn` and its derived classes describe fields and behavior.
- Exporters and settings work from snapshots of the current runtime state.

## Quick Examples

### Add a grid in XAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="clr-namespace:Avalonia.Controls;assembly=Tripous.Avalonia.Controls">
    <controls:GroupGrid x:Name="Grid" />
</Window>
```

### Bind a POCO list

```csharp
List<OrderRow> Rows = new()
{
    new() { Customer = "Alpha", Quantity = 2, Amount = 35.50m },
    new() { Customer = "Beta", Quantity = 5, Amount = 120.00m },
};

Grid.Columns.Add(new GroupGridTextColumn { Name = "Customer", Header = "Customer", Width = 160 });
Grid.Columns.Add(new GroupGridNumberColumn { Name = "Quantity", Header = "Qty", Width = 80, ValueType = typeof(int) });
Grid.Columns.Add(new GroupGridNumberColumn { Name = "Amount", Header = "Amount", Width = 110, ValueType = typeof(decimal), DisplayFormat = "N2" });
Grid.ItemsSource = Rows;
```

### Auto-generate columns

```csharp
Grid.AutoGenerateColumns = true;
Grid.ItemsSource = Rows;
```

### Bind a DataTable

```csharp
DataTable Table = new("Orders");
Table.Columns.Add("Customer", typeof(string));
Table.Columns.Add("Quantity", typeof(int));
Table.Columns.Add("Amount", typeof(decimal));
Table.Rows.Add("Alpha", 2, 35.50m);
Table.Rows.Add("Beta", 5, 120.00m);

Grid.AutoGenerateColumns = true;
Grid.ItemsSource = Table;
```

### Add summaries

```csharp
GroupGridNumberColumn AmountColumn = new()
{
    Name = "Amount",
    Header = "Amount",
    Width = 110,
    ValueType = typeof(decimal),
    DisplayFormat = "N2",
    GroupSummary = GroupGridAggregateKind.Sum,
    TotalSummary = GroupGridAggregateKind.Sum,
};

Grid.Columns.Add(AmountColumn);
```

### Group, sort, and filter

```csharp
GroupGridColumn CustomerColumn = Grid.GetAllColumns().First(Column => Column.Name == "Customer");
GroupGridColumn AmountColumn = Grid.GetAllColumns().First(Column => Column.Name == "Amount");

Grid.GroupColumn(CustomerColumn);
Grid.ToggleSort(AmountColumn);
Grid.SetColumnFilter(AmountColumn, ">=100");
```

When no columns are grouped, the group panel displays a small prompt. Customize or clear it with `EmptyGroupPanelText`.

### Hide Id columns

```csharp
Grid.AreIdColumnsVisible = false;
```

Columns named `Id` or ending with `Id` are hidden case-insensitively.

### Show, hide, or make columns read-only by name

```csharp
Grid.SetColumnVisible("InternalNotes", false);
Grid.SetColumnsVisible(new[] { "Source", "Status" }, true);

Grid.SetColumnReadOnly("Amount", true);
Grid.SetColumnsReadOnly(new[] { "Quantity", "Amount" }, false);
```

### Best-fit columns

```csharp
Grid.BestFitColumn("Customer");
Grid.BestFitColumn(AmountColumn);
Grid.BestFitColumns();
```

Best-fit sizing always includes the column header. It measures display text from the current projected visible data rows, respects `MinWidth`, and uses a default row sample limit of 2500 rows per column. Pass `0` or a negative value to measure all visible data rows.

### Scroll and select

```csharp
Grid.ScrollToRow(25);
Grid.SetCurrentCell(25, Grid.GetAllColumns().First(Column => Column.Name == "Amount"));
Grid.SelectCurrentCell();
```

`ScrollToRow()` uses adapter row indexes. It returns `false` when the row is not present in the current projection, for example because it is filtered out.

### Read the current row

```csharp
Grid.CurrentRowChanged += (Sender, Args) =>
{
    object Row = Grid.CurrentRow;
    int RowIndex = Grid.CurrentRowIndex;
};
```

Use the control-level current-row API when application code needs the active data row. Consumers should not reach into `Grid.Engine` for ordinary current-row state.

### Save and load layout

```csharp
string FilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "MyApp",
    "orders-grid.json");

Grid.SaveSettings(FilePath, "Orders");
Grid.LoadSettings(FilePath);
```

Settings JSON is formatted and stores layout state such as widths, visibility, grouping, filters, sorting, summaries, and band visibility.

The column header context menu also exposes `Save Settings...` and `Load Settings...` items that use Avalonia file pickers. Set `IsSettingsMenuItemsVisible` to `false` when an application wants to provide its own settings commands. Set `SettingsSuggestedFileName` to customize the suggested file name used by the built-in save settings picker.

### Export

```csharp
string Folder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

Grid.SaveExport(new GroupGridCsvExporter(), Path.Combine(Folder, "orders.csv"));
Grid.SaveExport(new GroupGridJsonExporter(), Path.Combine(Folder, "orders.json"));
Grid.SaveExport(new GroupGridHtmlExporter(), Path.Combine(Folder, "orders.html"));
```

### Register a custom exporter

```csharp
GroupGridExporters.Register(() => new MyCustomExporter());
```

Registered exporters appear in the grid export menu.

### Add a toolbar button

```csharp
Grid.AddButton(GroupGridToolButtonAlignment.Right, "Refresh", "Refresh", "Reloads the grid data.");
Grid.ToolButtonClicked += (Sender, Args) =>
{
    if (Args.Button.Name == "Refresh")
        ReloadRows();
};
```

### Validate edits

```csharp
Grid.CellValidating += (Sender, Args) =>
{
    if (Args.Cell.Column.Name == "Amount" && Args.Value is decimal Value && Value < 0)
        Args.Cancel = true;
};
```

### React after a cell value is committed

```csharp
Grid.CellValueCommitted += (Sender, Args) =>
{
    if (Args.Cell.Column.Name == "CustomerId")
        ApplyLookupSnapshots(Grid.CurrentRow, Args.Value);
};
```

The control exposes the edit lifecycle events directly: `BeginningEdit`, `CellValidating`, `CellValueCommitting`, `CellValueCommitted`, and `EditCanceled`. Handlers receive the same mutable event arguments used by the engine, so validation cancellation and value changes affect the actual commit.

### Provide a custom editor

```csharp
Grid.CreateInplaceEditor += (Sender, Args) =>
{
    if (Args.Column.Name == "Code")
        Args.Editor = new MyCodeEditor();
};
```

### Provide a custom drop-down editor

```csharp
public class MyLookupEditor : GroupGridDropDownInplaceEditorBase
{
    public override Control CreateDropDownControl()
    {
        return ResultGrid;
    }

    void ResultGrid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        DropDownHost.CommitDropDownValue(CurrentResult);
    }

    protected override object GetDropDownSelectedValue()
    {
        return CurrentResult;
    }
}
```

`IGroupGridDropDownEditorHost` lets a custom drop-down editor ask the owning grid to close the drop-down, cancel it, restore focus, or commit a selected value without depending on `GroupGrid` internals.

## Data Access

The grid does not own application data. It reads and writes values through `IGroupGridDataAdapter`.

The built-in convenience path is `ItemsSource`. It supports common list sources and `DataTable` / `DataView`, creating the appropriate adapter internally. Developers can also assign `DataAdapter` directly when they need full control.

Conceptually, the adapter answers:

```text
How many rows exist?
What is the value at row X and column Y?
Can that value be edited?
How is a new value written back?
Can a row be inserted or deleted?
What changed in the data source?
```

This keeps `GroupGrid` independent from any specific application framework, ORM, table model, or business object model.

## Columns

Columns are descriptors, not visual controls.

A column describes a field:

- Name and header.
- Width and visibility.
- Read-only state.
- Value type.
- Formatting.
- Alignment.
- User permissions for resize, reorder, grouping, and hiding.
- Summary settings.
- Type-specific editing and display behavior.

Built-in column types cover common field shapes:

- `GroupGridTextColumn`
- `GroupGridNumberColumn`
- `GroupGridDateColumn`
- `GroupGridCheckBoxColumn`
- `GroupGridLookupColumn`

Custom column types can be added when a field needs special formatting, parsing, aggregation, or editor behavior.

## Projection

The rows displayed by the grid are not always the raw data rows.

The engine builds a projection:

```text
data rows
    filtering
    sorting
    grouping
    summaries
visible projection rows
```

When grouping is enabled, the visible projection contains multiple row kinds:

- Data rows.
- Group header rows.
- Group summary rows.

The footer total summary is displayed in its own band, outside the body projection.

This is why APIs that work with the viewport use projection concepts, while APIs that work with data values use adapter row indexes.

## Viewport And Scrolling

The grid uses a virtual viewport. It renders only the visible range of the current projection.

The key concepts are:

- `VisibleNodeCount`: total rows in the current projection.
- `FirstVisibleNodeIndex`: first projected row shown by the viewport.
- `Viewport`: the current visible projected-row range.
- `ScrollToRow(rowIndex)`: scrolls an adapter row into view when that row exists in the current projection.
- `ScrollCurrentCellIntoView()`: scrolls the current cell into view.

An adapter row may not be scrollable if it is filtered out of the current projection.

## Current Cell And Selection

`GroupGrid` v1 intentionally keeps selection simple.

There is:

- One current cell.
- One selected cell.
- Single-row visual selection based on the selected cell.

The current cell is the active work position for keyboard navigation and editing. The selected cell is the visual selection target. This keeps the control predictable for v1 data-entry workflows.

## Editing

Editing is a session, not just a text box.

The flow is:

```text
begin edit
    create in-place editor
    load current value
    user changes value
    normalize and parse
    validate
    commit or cancel
```

Built-in editors cover text, numbers, dates, lookups, and boolean values. Applications can provide custom editors through `CreateInplaceEditor`.

Validation and commit events are raised by the engine so applications can cancel or react to changes without replacing the whole editing pipeline.

## Settings

`GroupGridSettings` is the serializable layout state of a grid.

It stores:

- Band visibility.
- Default toolbar button visibility.
- Sorting.
- Column order.
- Column widths.
- Column visibility.
- Grouping.
- Filters.
- Summary settings.

Use `CreateSettings()` and `ApplySettings()` for in-memory snapshots. Use `SaveSettings(filePath)` and `LoadSettings(filePath)` when the application wants persistent layout files. File paths are supplied by the application and must be full paths.

## Export

Export is based on a snapshot, not direct drawing.

`CreateExportSnapshot()` captures:

- Visible columns.
- Projected rows.
- Display text.
- Raw values.
- Group rows.
- Group summary rows.
- Total summary cells.

The exporter registry contains the built-in CSV, JSON, and HTML exporters. Applications can register additional exporters as instances or factories.

CSV and JSON focus on data rows. HTML represents the current projected grid shape, including groups and summaries.

## Theming

The grid exposes theme-facing brush properties for the visual parts of the control:

- Main grid surface.
- Toolbar.
- Group panel.
- Headers.
- Filter row.
- Group rows and summaries.
- Selected, current, and editing cells.
- Footer summaries.
- Text.
- Scrollbars.
- Grid lines.
- Drag and resize guides.

These properties are Avalonia styled properties, so they can be set directly or through resources/styles.

## Extensibility

The intended extension points are:

- Custom data adapters.
- Custom column types.
- Custom in-place editors.
- Custom toolbar buttons.
- Row operation event handlers.
- Editing validation and commit event handlers.
- Custom exporters.
- Theme resources and brush properties.

The preferred model is to extend through these contracts instead of reaching into internal projection or rendering state.

## V1 Boundary

`GroupGrid` v1 deliberately stays conservative in a few areas:

- Sorting is single-column.
- Selection is single-cell/single-row.
- Filters are text-based.
- Range selection and multi-selection are not part of v1.
- Typed filter descriptors are left for a future v2.

This keeps the first public surface understandable and reduces risk in the core runtime.

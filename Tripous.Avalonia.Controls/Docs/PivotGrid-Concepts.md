# PivotGrid Concepts

`PivotGrid` is a custom-rendered Avalonia pivot grid for interactive cross-tab analysis. It is not a wrapper around another grid. It is built around a small runtime model that separates the visual control, the non-visual engine, source adapters, field descriptors, measures, settings, hit testing, and export.

The control belongs to `Tripous.Avalonia.Controls` and remains framework-neutral. It has no dependency on Tripous data modules, descriptors, registries, or runtime services.

## Main Idea

`PivotGrid` turns a flat data source into a visible pivot projection.

Source members are called fields. A field may be assigned to one of four roles:

- Available: the source field is known but not currently used in the pivot.
- Row: the field participates in the row axis.
- Column: the field participates in the column axis.
- Measure: the field is aggregated into value cells.

The important split is:

- `PivotGrid` is the Avalonia control.
- `PivotGridEngine` is the non-visual pivot runtime.
- `IPivotGridDataAdapter` is the source access contract.
- `PivotGridField` describes row and column fields.
- `PivotGridMeasure` describes value fields and aggregate behavior.
- Exporters and settings work from snapshots of the current visible pivot state.

## Quick Example

### Add a PivotGrid in XAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="clr-namespace:Avalonia.Controls;assembly=Tripous.Avalonia.Controls">
    <controls:PivotGrid x:Name="Grid" />
</Window>
```

### Bind a POCO list

```csharp
List<SalesRow> Rows = new()
{
    new() { Region = "North", Quarter = "Q1", Person = "Alex", Amount = 1200m, Units = 4 },
    new() { Region = "North", Quarter = "Q2", Person = "Alex", Amount = 1800m, Units = 6 },
    new() { Region = "South", Quarter = "Q1", Person = "Bianca", Amount = 900m, Units = 3 },
};

Grid.RowFields.Add(new PivotGridField { Name = "Region", Header = "Region" });
Grid.ColumnFields.Add(new PivotGridField { Name = "Quarter", Header = "Quarter" });
Grid.Measures.Add(new PivotGridMeasure
{
    Name = "Amount",
    Header = "Amount",
    SourceFieldName = "Amount",
    AggregateKind = PivotGridAggregateKind.Sum,
    DisplayFormat = "N2",
});

Grid.ItemsSource = Rows;
```

### Bind a DataTable

```csharp
DataTable Table = new("Sales");
Table.Columns.Add("Region", typeof(string));
Table.Columns.Add("Quarter", typeof(string));
Table.Columns.Add("Amount", typeof(decimal));
Table.Rows.Add("North", "Q1", 1200m);
Table.Rows.Add("South", "Q1", 900m);

Grid.ItemsSource = Table;
```

Assigning a `DataTable` uses its `DefaultView`. A `DataView` may also be assigned directly.

## Source Field Rules

`PivotGrid` discovers valid source fields and ignores unsupported members.

Valid axis fields include:

- string
- char
- bool
- enum
- numeric values
- `DateTime`
- `DateTimeOffset`

Valid measure fields include:

- numeric values for `Sum` and `Average`
- comparable values for `Min` and `Max`
- any valid source field for `Count`

Unsupported fields include complex objects, collections, byte arrays, streams, images, and other non-scalar values.

## Layout

The visual layout follows the common pivot grid shape:

- Available fields are shown in an optional top field panel.
- Value measure fields are shown in the top-left axis panel.
- Column fields are shown above the pivot column header area.
- Row fields are shown in the left row-header area.
- Value cells are rendered in the body.

Users may drag fields between available fields, rows, columns, and values. They may also reorder fields inside row, column, and value roles.

The top field panel can be hidden with `ShowFieldPanel` when screen space is more important than direct available-field access.

## Row Axis

When multiple row fields are used, the row axis becomes a tree.

Row nodes support expand and collapse. Parent nodes aggregate values from descendant leaf rows. Expanded parent row values are drawn with emphasis, while total cells remain bold.

Useful APIs:

```csharp
Grid.ExpandAllRows();
Grid.CollapseAllRows();
```

## Aggregates

Measures use `PivotGridAggregateKind`.

Supported aggregate kinds:

- Count
- Sum
- Min
- Max
- Average

Example:

```csharp
Grid.SetMeasureAggregate(0, PivotGridAggregateKind.Average);
```

## Sorting And Filtering

Sorting is engine-owned and uses one active row or column field sort at a time.

Sorting follows the same cycle used by `GroupGrid`:

- None
- Ascending
- Descending
- None

```csharp
Grid.ToggleSort(PivotGridFieldRole.Row, "Region");
Grid.SetSort(PivotGridFieldRole.Column, "Quarter", PivotGridSortDirection.Descending);
Grid.ClearSort();
```

Filtering is value-list based. The built-in filter dialog presents distinct field values with search, select all, deselect all, and selected/total counts.

```csharp
Grid.SetFieldFilter("Region", new object[] { "North", "South" });
Grid.ClearFieldFilter("Region");
Grid.ClearFilters();
```

Filters are persisted using invariant value keys, so settings can be saved and restored reliably.

## Sizing

Value columns and the row header can be resized by mouse.

Useful APIs:

```csharp
Grid.SetValueColumnWidth(0, 0, 160);
Grid.AutoFitValueColumnWidth(0, 0);
Grid.AutoFitValueColumnWidths();
Grid.ClearValueColumnWidths();

Grid.SetRowHeaderWidth(220);
Grid.AutoFitRowHeaderWidth();
Grid.ResetRowHeaderWidth();
```

Double-clicking a value column divider auto-fits that visible value column. Double-clicking the row-header divider auto-fits the row header.

Column auto-fit considers:

- column group headers
- measure headers
- value cells
- row totals
- column totals
- grand totals

## Current Cell And Navigation

`PivotGrid` tracks a single current value cell.

Keyboard navigation includes:

- arrows
- `Home`
- `End`
- `Ctrl+Home`
- `Ctrl+End`
- `PageUp`
- `PageDown`

Copy shortcuts:

- `Ctrl+C`: copies current cell text.
- `Ctrl+Shift+C`: copies the visible pivot matrix as tab-separated text.

Useful APIs:

```csharp
Grid.SetCurrentCell(0, 0, 0);
Grid.ScrollCurrentCellIntoView();
Grid.ClearCurrentCell();

string Text = Grid.CurrentCellText;
```

Programmatic current-cell selection scrolls the selected cell into view, including row total, column total, and grand-total cells.

## Context Menu

The context menu is intentionally flat and compact, avoiding theme submenu arrows.

The general menu includes commands for:

- copy current cell or whole pivot
- expand/collapse all rows
- auto-fit and reset widths
- row/column totals
- field panel visibility
- tooltips
- settings
- export

The field menu adds field-specific commands for:

- sorting
- aggregate selection for measures
- filtering
- moving a field to rows, columns, values, or available

## Settings

`PivotGridSettings` is a serializable layout snapshot.

```csharp
PivotGridSettings Settings = Grid.CreateSettings("Sales");
Grid.ApplySettings(Settings);

Grid.SaveSettings("/path/sales-pivot.json", "Sales");
Grid.LoadSettings("/path/sales-pivot.json");
```

Settings include:

- row fields
- column fields
- measures
- aggregate kinds
- display formats
- row and column total visibility
- top field panel visibility
- tooltip visibility
- row header width
- visible value column width overrides
- sorting
- filters
- collapsed row keys

The built-in settings dialog edits field roles, measure aggregate and format, totals, field panel visibility, and tooltips.

## Export

The export API uses a snapshot of the visible pivot matrix.

```csharp
string Folder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

Grid.SaveExport(new PivotGridCsvExporter(), Path.Combine(Folder, "sales-pivot.csv"));
Grid.SaveExport(new PivotGridJsonExporter(), Path.Combine(Folder, "sales-pivot.json"));
Grid.SaveExport(new PivotGridHtmlExporter(), Path.Combine(Folder, "sales-pivot.html"));
```

Built-in exporters:

- CSV
- JSON
- HTML

HTML export marks totals in bold.

Custom exporters can be registered through `PivotGridExporters`.

## Hit Testing And Tooltips

`HitTest(Point)` returns the logical pivot element under a point.

Hit-test results may identify:

- available fields
- row fields
- column fields
- measure fields
- row headers
- row expanders
- column headers
- value cells
- row-header resize handle
- value-column resize handle

`GetToolTipText(Point)` exposes the tooltip text used by the control. Tooltips can be enabled or disabled through `ShowToolTips`.

## Demo And Tests

The repository includes:

- `Demos/PivotGrid-Demo-00`
- `Tests/PivotGrid-Tests`

The demo includes small and large datasets, long row-header data, drag/drop field editing, filters, settings, scrolling, resizing, copy, and export scenarios.

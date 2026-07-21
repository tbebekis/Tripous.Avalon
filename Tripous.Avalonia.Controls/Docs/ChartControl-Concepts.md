# ChartControl Concepts

`ChartControl` is a custom-rendered Avalonia control for small BI charts over flat result sets. It is intended for business dashboards, saved report views, SQL result visualizations, and application screens that need a compact chart next to grids or pivot layouts.

The control belongs to `Tripous.Avalonia.Controls` and remains framework-neutral. It has no dependency on Tripous data modules, descriptors, registries, or runtime services.

## Main Idea

`ChartControl` turns a flat source into an aggregated chart projection.

The user chooses:

- a category field
- an optional series field
- a value field
- an aggregate kind
- a chart type
- sorting and optional TopN
- legend, labels, value format, and palette settings

The important split is:

- `ChartControl` is the Avalonia visual control.
- `ChartEngine` is the non-visual chart projection runtime.
- `IChartDataAdapter` is the neutral source access contract.
- `ChartSettings` is the serializable settings model.
- `ChartSeries` and `ChartDataPoint` represent the aggregated projection.

## Quick Example

### Add a ChartControl in XAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="clr-namespace:Avalonia.Controls;assembly=Tripous.Avalonia.Controls">
    <controls:ChartControl x:Name="Chart" />
</Window>
```

### Bind a POCO list

```csharp
List<SalesRow> Rows = new()
{
    new() { Region = "North", Quarter = "Q1", Amount = 1200m },
    new() { Region = "North", Quarter = "Q2", Amount = 1800m },
    new() { Region = "South", Quarter = "Q1", Amount = 900m },
};

Chart.ApplySettings(new ChartSettings
{
    Title = "Sales by Region and Quarter",
    ChartType = ChartType.Column,
    CategoryFieldName = "Region",
    SeriesFieldName = "Quarter",
    ValueFieldName = "Amount",
    AggregateKind = ChartAggregateKind.Sum,
    SortDirection = ChartSortDirection.Ascending,
    ValueFormat = "N2",
});

Chart.ItemsSource = Rows;
```

### Bind a DataTable

```csharp
DataTable Table = new("Sales");
Table.Columns.Add("Region", typeof(string));
Table.Columns.Add("Quarter", typeof(string));
Table.Columns.Add("Amount", typeof(decimal));
Table.Rows.Add("North", "Q1", 1200m);
Table.Rows.Add("South", "Q1", 900m);

Chart.ItemsSource = Table;
```

Assigning a `DataTable` uses its `DefaultView`. A `DataView` may also be assigned directly.

## Source Field Rules

`ChartControl` discovers valid source fields and ignores unsupported members.

Valid category and series fields include:

- string
- char
- bool
- enum
- numeric values
- `DateTime`
- `DateTimeOffset`
- `Guid`

Valid value fields include numeric values and scalar fields that can be used for `Count` or `CountDistinct`.

Unsupported fields include complex objects, collections, byte arrays, streams, images, and other non-scalar values.

## Chart Types

Supported chart types:

- `Column`
- `Bar`
- `Line`
- `Area`
- `Pie`
- `Donut`
- `StackedColumn`
- `StackedBar`

Cartesian charts render axes and ticks. Pie and donut charts render category slices. Pie and donut value labels include both category and formatted value when labels are enabled.

## Aggregates

Measures use `ChartAggregateKind`.

Supported aggregate kinds:

- Count
- Sum
- Min
- Max
- Average
- StdDev
- StdDevP
- Variance
- VarianceP
- CountDistinct
- Product

Example:

```csharp
ChartSettings Settings = Chart.CreateSettings();
Settings.AggregateKind = ChartAggregateKind.Average;
Chart.ApplySettings(Settings);
```

## Sorting And TopN

Sorting applies to category text.

Supported sort directions:

- `None`
- `Ascending`
- `Descending`

`TopN` limits the number of categories. A value of `0` means no limit. Categories are selected by total numeric value before category text sorting is applied.

```csharp
Chart.ApplySettings(new ChartSettings
{
    CategoryFieldName = "Region",
    ValueFieldName = "Amount",
    AggregateKind = ChartAggregateKind.Sum,
    TopN = 5,
    SortDirection = ChartSortDirection.Ascending,
});
```

## Settings

`ChartSettings` is a serializable POCO.

Settings include:

- `Name`
- `Title`
- `ChartType`
- `CategoryFieldName`
- `SeriesFieldName`
- `ValueFieldName`
- `AggregateKind`
- `SortDirection`
- `TopN`
- `ShowLegend`
- `ShowValueLabels`
- `ValueFormat`
- `PaletteName`

Useful APIs:

```csharp
ChartSettings Settings = Chart.CreateSettings();
Chart.ApplySettings(Settings);
Chart.SaveSettings("/path/chart-settings.json");
Chart.LoadSettings("/path/chart-settings.json");
```

The context menu includes `Save Settings...` and `Load Settings...` by default. A host can hide those file commands:

```csharp
Chart.IsSettingsMenuItemsVisible = false;
```

The suggested file name used by the save picker is controlled by:

```csharp
Chart.SettingsSuggestedFileName = "sales-chart.json";
```

## Context Menu And Dialog

Right-click opens the built-in context menu.

The context menu provides:

- settings dialog
- chart type selection
- sorting
- TopN presets
- palette selection
- legend toggle
- value labels toggle
- optional Save Settings and Load Settings commands

The settings dialog can also be shown directly:

```csharp
bool Applied = await Chart.ShowSettingsDialogAsync();
```

The dialog edits fields from `DataAdapter.SourceFields`, so assign `ItemsSource` or `DataAdapter` before opening it when field selection is needed.

## Hit Testing And Tooltips

The control exposes hit testing and tooltip text:

```csharp
ChartHitTestResult Hit = Chart.HitTest(Point);
string ToolTip = Chart.GetToolTipText(Point);
```

Hit testing identifies data points and legend items. Tooltip text includes category, optional series, and formatted value.

## Demo And Tests

The demo project is:

- `Tripous.Avalonia.Controls/Demos/Charts-Demo-00`

The test project is:

- `Tripous.Avalonia.Controls/Tests/Charts-Tests`

The demo includes sample sales data, source switching between POCO list, `DataTable`, and `DataView`, chart type buttons, labels, legend, TopN, and the settings dialog.

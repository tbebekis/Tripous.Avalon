# ChartControl Design

## Status

`ChartControl` v1 is implemented for the current milestone.

This document records the design boundaries and runtime shape. The user-facing feature list and examples are tracked in:

- [ReadMe.md](../ReadMe.md)
- [ChartControl Concepts](ChartControl-Concepts.md)

## Purpose

`ChartControl` is a reusable Avalonia chart control for business-style visualization of flat result sets.

It is intended for scenarios where a source is projected into:

- categories
- optional series
- aggregated values
- saved chart settings
- simple dashboard visuals

The control is a visual surface and a neutral runtime. It is not a Tripous data-aware component.

## Hard Boundaries

- No reference to `Tripous`.
- No reference to `Tripous.Data`.
- No `TableDef`, `FieldDef`, `Locator`, `LookupDef`, registry, data module, or Tripous naming in the public API.
- No Tripous adapters inside this library.
- Future Tripous or DbPark integration must be implemented outside this library.
- Source members are called fields, not columns, because the source may be a POCO list, `DataTable`, or `DataView`.

## Runtime Shape

The main runtime pieces are:

- `ChartControl`: the Avalonia control.
- `ChartEngine`: the non-visual chart runtime.
- `IChartDataAdapter`: the neutral source access contract.
- `ChartDataViewDataAdapter`: adapter for `DataView` and `DataTable.DefaultView`.
- `ChartListDataAdapter`: adapter for POCO `IList` sources.
- `ChartSettings`: serializable chart settings.
- `ChartSourceField`: discovered source field metadata.
- `ChartMeasure`: measure descriptor and value formatter.
- `ChartSeries`: projected series.
- `ChartDataPoint`: aggregated category/series point.
- `ChartPalette`: built-in palette provider.
- `ChartHitTestResult`: hit-test result for data points and legend items.
- `ChartSettingsDialog`: built-in settings editor.

## Source Strategy

The control supports common Avalonia application data sources directly:

- POCO `IList`
- `DataTable`
- `DataView`

All sources are adapted into `IChartDataAdapter`.

The control discovers scalar source fields and ignores unsupported members. This keeps field selection predictable and prevents complex object graphs from leaking into chart settings.

## Projection Strategy

The engine groups rows by:

- category field
- optional series field

For each category/series bucket it aggregates the selected value field using `ChartAggregateKind`.

The engine produces:

- ordered category keys and texts
- one or more `ChartSeries`
- one `ChartDataPoint` per visible category in each series

`TopN` is applied by total numeric category value. Category sorting is then applied to the selected categories.

## Rendering Strategy

Rendering is pure Avalonia drawing.

The first milestone intentionally keeps rendering simple:

- cartesian charts render basic axes and ticks
- column and bar charts render grouped or stacked rectangles
- line and area charts render connected points
- pie and donut charts render category slices
- legend is rendered when multiple series exist and legend display is enabled
- labels are rendered only when enabled
- pie and donut labels include category and formatted value

The renderer does not use an external charting library.

## Interaction Strategy

The control exposes:

- hover tooltip text
- hit testing for data points and legend items
- right-click context menu
- settings dialog

The context menu provides direct commands for common chart changes:

- chart type
- sort direction
- TopN
- palette
- legend visibility
- value label visibility
- settings dialog
- optional Save Settings and Load Settings

The Save Settings and Load Settings menu items are controlled by `IsSettingsMenuItemsVisible`, matching the pattern used by the grid controls.

## Settings Strategy

`ChartSettings` persists the user-visible chart definition:

- name
- title
- chart type
- category field
- optional series field
- value field
- aggregate kind
- sort direction
- TopN
- legend visibility
- value label visibility
- value format
- palette name

Settings are JSON snapshots produced by `CreateSettings()` and restored by `ApplySettings()`. File persistence is handled by `SaveSettings()` and `LoadSettings()`.

## Dialog Strategy

`ChartSettingsDialog` is deliberately small.

It edits the stable chart settings and gets available fields from `DataAdapter.SourceFields`. Hosts that need a richer chart designer can build one outside the control and still use the same `ChartSettings` model.

The dialog is resizable, owner-centered, and disables minimize/maximize while allowing resize.

## Current Non-Goals

- Scientific plotting.
- Multiple measures in one chart.
- Date bucketing.
- Automatic "Other" bucket for TopN.
- Negative-value stack rendering.
- Per-slice pie hit geometry.
- Smart label collision avoidance.
- Export image support.
- Print-specific integration.
- Tripous-specific source adapters or registry integration.

These can be revisited after real usage shows the need.

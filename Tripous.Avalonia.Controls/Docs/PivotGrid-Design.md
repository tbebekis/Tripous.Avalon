# PivotGrid Design

## Status

`PivotGrid` v1 is implemented for the current milestone.

This document records the design boundaries and runtime shape. The user-facing feature list and examples are tracked in:

- [ReadMe.md](../ReadMe.md)
- [PivotGrid Concepts](PivotGrid-Concepts.md)

## Purpose

`PivotGrid` is a reusable Avalonia pivot grid control for business-style cross-tab analysis.

It is intended for scenarios where a flat source is projected into:

- row groups
- column groups
- value measures
- totals
- filters
- saved layouts
- exportable results

The control is a visual surface and a neutral runtime. It is not a Tripous data-aware component.

## Hard Boundaries

- No reference to `Tripous`.
- No reference to `Tripous.Data`.
- No `TableDef`, `FieldDef`, `Locator`, `LookupDef`, registry, data module, or Tripous naming in the public API.
- No Tripous adapters inside this library.
- Future Tripous integration must be implemented outside this library.
- Source members are called fields, not columns, because a source member may become a row field, column field, measure, or available field.

## Runtime Shape

The main runtime pieces are:

- `PivotGrid`: the Avalonia control.
- `PivotGridEngine`: the non-visual pivot runtime.
- `IPivotGridDataAdapter`: the neutral source access contract.
- `PivotGridField`: row and column field descriptor.
- `PivotGridMeasure`: value descriptor and aggregate configuration.
- `PivotGridSourceField`: discovered source field metadata.
- `PivotGridAxisItem`: row and column item identity.
- `PivotGridAxisNode`: visible row tree node.
- `PivotGridValueCell`: aggregated value cell.
- `PivotGridSettings`: serializable layout snapshot.
- `PivotGridExportSnapshot`: visible export projection.

## Source Strategy

The control supports common Avalonia application data sources directly:

- POCO `IList<T>`
- `DataTable`
- `DataView`

All sources are adapted into `IPivotGridDataAdapter`.

The control discovers scalar source fields and ignores unsupported members. This keeps the initial layout predictable and prevents complex object graphs from leaking into pivot axes.

## Layout Strategy

The visual layout follows common pivot grid products:

- Available fields panel at the top.
- Values in the top-left axis panel.
- Column fields above the column header area.
- Row fields in the left row-header area.
- Value cells in the body.

The available fields panel can be hidden for dense layouts. Field editing remains available through drag/drop and the settings dialog.

## Interaction Strategy

The main interaction model is direct manipulation:

- drag fields between available, rows, columns, and values
- drag fields to reorder inside their role
- double-click role list items in settings to remove them
- drag dividers to resize value columns or the row header
- double-click dividers to auto-fit
- expand/collapse row tree nodes
- use context menu commands for filtering, sorting, layout, settings, and export

The context menu is intentionally flat. The default Avalonia submenu indicator was visually too heavy for this control, so commands are grouped with separators and compact labels instead of nested submenus.

## Settings Strategy

Settings persist what users expect after arranging a pivot:

- row fields
- column fields
- measures
- aggregate kinds
- display formats
- row and column totals
- field panel visibility
- tooltip visibility
- row header width
- visible value column width overrides
- sort state
- filters
- collapsed row keys

Settings are JSON snapshots produced by `CreateSettings()` and restored by `ApplySettings()`. File persistence is handled by `SaveSettings()` and `LoadSettings()`.

## Export Strategy

Export works from `PivotGridExportSnapshot`, not from the visual tree.

The snapshot contains:

- row fields
- column fields
- measures
- visible value columns
- visible row texts
- value cells
- row total cells
- column total row
- grand total cells

Built-in exporters write CSV, JSON, and standalone HTML.

## Current Non-Goals

- In-place editing of source rows.
- Frozen panes.
- Multi-cell range selection.
- Virtualized measure/column realization beyond current scroll clipping.
- Printer-specific integration.
- Tripous-specific source adapters or registry integration.

These can be revisited only after real usage shows the need.

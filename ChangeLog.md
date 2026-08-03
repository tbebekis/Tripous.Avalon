# Changelog

All notable changes to this project will be documented in this file.

## 2026-08-03

### Tripous.Avalonia.Controls
- Added explicit Light/Dark theme style dictionaries for `GroupGrid`, `PivotGrid`, and `ChartControl`
- Added theme-facing styled brush properties for custom-rendered `GroupGrid`, `PivotGrid`, and `ChartControl` surfaces
- Updated the control demos with explicit `StyleInclude` loading, runtime Light/Dark switching, and resource/local override checks
- Updated `Tripous.Avalonia.Controls` documentation with the explicit theme dictionary loading pattern

### Tripous.Desktop
- Improved TinyERP desktop Light/Dark theme behavior for selected rows, selected tabs, drag/drop tab markers, and validation/error labels
- Enabled content tab reordering by default for the TinyERP desktop content page handler

## 2026-07-20

### Tripous.Avalonia.Controls
- Added `ChartControl`, a framework-neutral custom-rendered Avalonia BI chart control
- Added neutral chart data adapters for POCO `IList`, `DataTable`, and `DataView` sources
- Added `ChartSettings`, chart model types, `ChartEngine`, palettes, hit testing, tooltip text, and JSON settings support
- Added chart types: Column, Bar, Line, Area, Pie, Donut, StackedColumn, and StackedBar
- Added chart aggregates: Count, Sum, Min, Max, Average, StdDev, StdDevP, Variance, VarianceP, CountDistinct, and Product
- Added chart context menu and settings dialog
- Added `Charts-Demo-00` sample application
- Added `Charts-Tests` unit test project
- Added `PivotGrid`, a framework-neutral custom-rendered Avalonia pivot grid
- Added neutral pivot data adapters for POCO `IList<T>`, `DataTable`, and `DataView` sources
- Added pivot row fields, column fields, measures, available fields, drag/drop field assignment, and field reordering
- Added expandable row-axis tree nodes with parent aggregate rows
- Added row grand totals, column grand totals, and grand-total cells
- Added value aggregates: Count, Sum, Min, Max, and Average
- Added row/column sorting, field value-list filtering, filter dialog, and settings dialog
- Added current-cell selection, keyboard navigation, scrollbars, copy current cell, and copy visible pivot as tab-separated text
- Added value column resize, row header resize, auto-fit, reset width commands, and double-click auto-fit
- Added hover tooltips with enable/disable setting
- Added `PivotGridSettings` JSON save/load support including fields, measures, aggregates, filters, sort state, totals, field panel visibility, tooltip visibility, row header width, value column widths, and collapsed row keys
- Added CSV, JSON, and HTML pivot exporters
- Added `PivotGrid-Demo-00` sample application
- Added `PivotGrid-Tests` unit test project
- Added `PivotGrid` documentation under `Tripous.Avalonia.Controls/Docs`

## 2026-07-19

### TinyERP
- Renamed the `Language` table to `SYS_LANG` and linked `SYS_STR_RES` to it through `LanguageId`
- Added `SysStrRes` as the shared system string-resource cache and localizer bridge
- Added automatic insertion of missing English string-resource keys, controlled by system configuration
- Added desktop and web Resource Translations admin forms for editing `SYS_STR_RES`
- Added startup string-resource loading for tERPWeb and language-aware client localization
- Added English and Greek sample string resources and removed the third sample language
- Made application user culture selection use supported languages
- Updated login handling to persist `LastLoginAt`
- Updated desktop and web FactBox buttons so the pane starts hidden and is opened explicitly
- Reworked the desktop startup window so startup dialogs have a visible full-screen owner

### Tripous.Web
- Added WebDesk support used by TinyERP for runtime string-resource packets
- Added a web Resource Translations editor with per-language columns, inline save, delete confirmation, filtering and sorting
- Added main toolbar and menu command integration for the web Resource Translations form

### Tripous.Desktop
- Added a desktop Resource Translations editor backed by the shared resource translation service
- Improved `GroupGrid` header sorting behavior
- Updated data form FactBox command visibility and initial state

## 2026-06-22

### Framework
- Upgraded from Avalonia 11.3.12 to Avalonia 12.0.4
- Upgraded AvaloniaEdit from 11.4.1 to 12.0.0
- Updated clipboard API usage for Avalonia 12
- Replaced obsolete Watermark properties with PlaceholderText
- Replaced obsolete SystemDecorations with WindowDecorations

### Validation
- Full solution builds without warnings or errors
- All sample applications tested successfully
- tERP tested successfully

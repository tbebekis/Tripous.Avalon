# Web Locators

Tripous.Web uses the same `LocatorDef` model as desktop code, but the Web runtime executes locator operations through Ajax.

The current Web locator flow is:

- The client asks the server for locator metadata with `Locator.GetInfo`.
- The server returns a global `JsonLocatorDef` and, when target table context is supplied, a table-specific `JsonLocatorMapPlan`.
- The client executes a search with `Locator.Execute`.
- The server returns a `JsonDataTable` with result rows and the same table-specific map plan for the current operation.
- The client applies the map plan to the target row.

## Locator Metadata

`JsonLocatorDef` is global metadata. It describes the locator itself:

- locator name
- key field
- search fields
- result fields
- visible list fields
- web form name
- runtime limits

`JsonLocatorMapPlan` is table-specific metadata. It describes how fields from a selected locator result row map to fields of a target table row.

For example, a `Product` locator used by a `StockTradeLine` row may map:

- `Id` to `ProductId`
- `Code` to `ProductCode`
- `Name` to `ProductName`
- `UnitOfMeasureName` to `UnitOfMeasureName`
- `UnitRatio` to `UnitRatio`

The same locator may be used by another table with a different map plan.

## Table Locator List

`JsonDataTable` may include a `Locators` property.

`Locators` contains table-specific locator definitions. Each item is a `JsonLocatorDef` with its `MapPlan` populated for that table instance.

This is used when the server sends a table schema to the client. The client can then configure data-aware controls without hardcoded locator setup.

## Grid Locator Columns

`tp.Grid` reads `tp.DataTable.Locators` when binding to a table.

For each locator map plan, the grid:

- finds mapped target columns that correspond to locator search fields
- marks those columns as locator columns
- configures `tp.GridInplaceEditorLocator`
- keeps the map plan reference field as the real target key field

This lets a visible column such as `ProductCode` or `ProductName` start a locator search while the selected key is stored in `ProductId`.

The grid keeps manual locator setup authoritative. If a grid column already has explicit locator parameters, table metadata does not overwrite it.

## LocatorBox

`tp.LocatorBox` is the Web input control for locator fields.

It supports:

- multiple visible text boxes
- Ajax metadata loading
- Ajax search execution
- result dropdown
- keyboard navigation
- map plan application
- cancellation when the surrounding operation is no longer valid

The button at the right side is reserved for the locator menu. Dropdown search opens from locator search execution, not from that button.

## tERPWeb Build Note

tERPWeb JavaScript source fragments live under:

```text
SampleApps/TinyERP/tERPWeb/wwwroot/js-src
```

The generated JavaScript bundles are written under:

```text
SampleApps/TinyERP/tERPWeb/wwwroot/tp/js
```

Examples include:

- `tp.js`
- `tp-Data.js`
- `tp-UI.js`
- `tp-Grid.js`
- `tp-WebDesk.js`

When testing tERPWeb, run a full rebuild of the `tERPWeb` project before starting the application. The rebuild runs the Tripous Web bundler and creates the generated `tp*.js` bundles required by the MVC layout and demos.

Do not edit generated bundles directly. Edit the source fragments under `wwwroot/js-src` and rebuild.

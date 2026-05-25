# Avalonia Binding

## The Problem

- Tripous works heavily with runtime data structures such as `DataTable`, `DataView`, and `DataRowView`.
- Avalonia native binding does not handle `DataRowView` column access the way WPF did.
- Direct binding paths such as `[Code]`, `Code`, `Row[Code]`, and similar variants are not reliable on raw `DataRowView`.
- `DataGrid` auto-generated columns on `DataView` expose `DataRowView` CLR properties, not the actual table columns.
- Snapshot templates can display values, but refresh behavior is not reliable enough for production.
- Locator-style updates can change multiple fields directly on the underlying row, so the UI must refresh even when changes bypass the bound control.

## The Findings

- Raw `DataView` can be used as an item source, but it is not a suitable binding surface.
- `DataGridTemplateColumn` can display values by manually reading `DataRowView[FieldName]`, but this is a snapshot-style solution.
- `DataRowView.PropertyChanged`, `DataTable.ColumnChanged`, and `DataView.ListChanged` fire for row changes.
- Direct writes through both `DataRowView` and `DataRow` can be detected through provider-level notifications.
- Avalonia binding works reliably when the UI binds to a stable wrapper object instead of raw ADO.NET row objects.
- Indexer binding can read initial values, but CLR-style property notification is not enough by itself for all runtime field scenarios.
- A Tripous-owned binding layer is needed to provide current row, navigation, add/delete, master-detail, external refresh, and metadata expansion.

## The Solution

- Introduce a `DataSource` controller as the binding orchestrator.
- Introduce a `DataSourceRow` wrapper as the UI-facing row object.
- Introduce `IDataProvider` implementations for source-specific access.
- Keep provider responsibility focused on schema, value read/write, item creation/deletion, and external change notification.
- Keep `DataSourceRow` as the single row wrapper, avoiding an extra per-row adapter layer.
- Keep Avalonia-specific binding code in `Tripous.Desktop`.
- Keep data abstractions in `Tripous.Data`.

## Architecture

### Tripous.Data/Bindings

- `IDataProvider`
- `DataTableProvider`
- `DataViewProvider`
- `ListProvider<T>`
- `DataSource`
- `DataSourceList`
- `DataSourceRow`
- `DataSourceRelation`

### Tripous.Desktop/Bindings

- `DataSourceBinding`
- `DataSourceBindingExtensions`

## DataSource

- Owns the provider.
- Has a stable `Name` used as the association key with a `DataModule` table, usually the `MemTable.Name` or an alias supplied by `DataModule`.
- Can be retrieved by name through the owning `DataModule` or a similar owner-level lookup.
- May keep an owner/context reference, preferably through a neutral interface, so it remains a binding facade and not the data lifecycle owner.
- Exposes `Rows`, `AllRows`, `Current`, `Position`, `Count`, `IsBof`, `IsEof`, `IsEmpty`, and `HasRows`.
- Supports navigation through `MoveFirst()`, `MovePrevious()`, `MoveNext()`, and `MoveLast()`.
- Supports row creation through `NewRow()`, `AddRow()`, `AppendRow()`, and `AddNew()`.
- Supports deletion through `DeleteCurrent()` and `DeleteRow()`.
- Supports master-detail through `AddDetail()` and `RemoveDetail()`.
- Exposes its detail `DataSource` list and optional master `DataSource`.
- Builds relation names from the master and detail names as `{Master.Name}_TO_{Detail.Name}`.
- Removes details by relation, child `DataSource`, or child `DataSource.Name`.
- Supports `DetailsActive` to temporarily stop current-row propagation from a master to its details.
- Supports field/value filtering through `SetFilter()` and `CancelFilter()`.
- Applies visible-row filtering as master-detail condition plus field/value filter.
- String filters support case-insensitive contains and `prefix*` matching for all providers, including POCO lists.
- Supports cascade behavior through `CascadeDeleteRule`.
- Full expression filtering similar to `DataView.RowFilter` remains a later design step.
- Raises events for loading, clearing, creating, adding, deleting, changing, and position changes.

## DataSourceList

- Represents an owner-level collection of named `DataSource` instances.
- Uses case-insensitive name lookup.
- `Find(Name)` returns null when not found.
- `Get(Name)` throws when not found.
- Rejects null items, unnamed items, and duplicate names.
- Has an `Owner` context and assigns it to added/replaced `DataSource` items.
- Clears item `Owner` when removing, replacing, or clearing items.
- Changing `Owner` updates existing items that still point to the previous owner.

## DataSourceRow

- Wraps a single underlying item.
- Reads and writes values through the owning `DataSource.Provider`.
- Exposes indexer access by field name.
- Implements `INotifyPropertyChanged`.
- Provides typed accessors such as `AsString()`, `AsInteger()`, `AsInt32()`, `AsDecimal()`, `AsBoolean()`, and `AsDateTime()`.
- Raises field notifications for both internal changes and provider-detected external changes.

## Providers

- `DataTableProvider` supports `DataTable`, `DataView`, `DataRowView`, and `DataRow`.
- `DataViewProvider` supports explicit `DataView` sources.
- `ListProvider<T>` supports POCO lists where `T` implements `INotifyPropertyChanged`.
- Providers raise `ItemChanged` when the underlying data changes outside `DataSourceRow.SetValue()`.

## Desktop Binding

- `DataSourceBindingExtensions` exposes extension methods on `DataSource`.
- Simple controls bind to `DataSource.Current[FieldName]`.
- Grids bind to `DataSource.Rows` and `DataSource.Current`.
- Generated grid columns bind to `DataSourceRow[FieldName]`.
- Binding methods return `DataSourceBinding` objects.
- `DataSourceBinding` stores source, control, grid column, field name, target property, binding kind, and dispose lifecycle.
- Applications call `BindingComplete()` after all bindings are assigned.
- `BindingComplete()` performs the initial current-row synchronization without replacing an existing current row.

## Lifecycle Notes

- Avoid per-cell manual event subscriptions in `DataGrid` templates when native binding can be used.
- If a cell template subscribes to row or provider events, it must unsubscribe when the cell visual is detached.
- `DataGrid` cell virtualization and visual recycling can otherwise keep old rows alive.
- The current design keeps row notifications centralized through `DataSource` and `DataSourceRow`.
- `DataSourceBinding` owns disposable binding subscriptions for controls and grids.

## Tested Behavior

- Loading and navigation from `DataTable`.
- Loading from filtered and sorted `DataView`.
- Automatic `DataSource.Name` from `DataTable.TableName` and `DataView.Table.TableName`.
- Changing values through `DataSourceRow`.
- Detecting external changes through `DataRowView`, `DataRow`, and POCO notifications.
- Add and delete rows.
- Master-detail filtering.
- Generated `DataSourceRelation.Name` from master and detail `DataSource` names.
- `DetailsActive` activation and deactivation.
- Field/value filtering through `SetFilter()` and `CancelFilter()`.
- POCO list filtering, including refresh after POCO property changes.
- `DataSourceList` name lookup, duplicate protection, and owner propagation.
- Restrict and cascade delete behavior.
- `ListProvider<T>` with POCO objects.
- Position and change events, including cancellation.
- Desktop binding metadata for controls and grids.
- `BindingComplete()` selects the first row when current is empty and preserves an existing current row.

## Next Steps

- Move stabilized code into the real `Tripous.Data` and `Tripous.Desktop` projects.
- Add overloads that accept Tripous metadata such as `FieldDef`.
- Add lookup and locator metadata to `DataSourceBinding`.
- Add specialized grid columns for dates, numbers, lookups, locators, and images.
- Add broader unit tests after the production move.

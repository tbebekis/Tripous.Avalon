# GroupGrid Design

## Status

This is a historical design note from the early `GroupGrid` planning phase. It is kept for background context.

The current v1 status, implemented feature list, and next hardening items are tracked in:

- [ReadMe.md](../ReadMe.md)
- [CurrentWork.md](../CurrentWork.md)
- [GroupGrid Concepts.md](GroupGrid-Concepts.md)

## Purpose

`GroupGrid` is a reusable Avalonia grid control for dense data-entry and business application screens.

The control belongs to `Tripous.Avalonia.Controls` and must remain independent from Tripous.

The old JavaScript `tp.Grid` in `Material/WebDesk/wwwroot/tp/js/tp-Grid.js` is reference material for behavior and API ideas, not code to port directly.

## Hard Boundaries

- No reference to `Tripous`.
- No reference to `Tripous.Data`.
- No `MemTable`, `TableDef`, `FieldDef`, `Locator`, `LookupDef`, registry, or Tripous naming in the public API.
- No Tripous adapters in this solution.
- Future Tripous integration must be implemented outside this library.

## V1 Goal

Build the smallest useful native Avalonia grid foundation that proves:

- Rows can be supplied by a neutral data adapter.
- Columns can be declared explicitly.
- Cells can display values.
- A current cell and current row can be tracked.
- Keyboard navigation is predictable.
- A cell can enter edit mode and commit or cancel.
- Basic validation hooks exist.

V1 should prefer clear contracts over many features.

## V1 Shape

V1 should be a real grid foundation, not a wrapper around Avalonia `DataGrid`.

Initial pieces:

- `GroupGrid`: the Avalonia control.
- `GroupGridEngine`: the non-visual grid engine used by the control.
- `GroupGridColumn`: column descriptor owned by the grid.
- `IGroupGridDataAdapter`: neutral data access contract.
- `GroupGridCell`: immutable row/column coordinate value.
- `GroupGridSelection`: selection/current state holder.
- `GroupGridEditor`: editor abstraction or editor factory contract.
- `GroupGridNode`: internal projected row node.
- `GroupGridRootNode`: internal root node.
- `GroupGridNodeList`: internal flat visible node list.

Names are provisional.

The first code pass should include the root/data-row node model, but not grouping behavior.

## V1 Non-Goals

- Grouping UI.
- Filtering UI.
- Sorting UI.
- Column drag reorder.
- Column chooser.
- Frozen columns.
- Virtualization beyond what is necessary for a first working control.
- Tripous lookup or locator editors.
- Data-aware toolbar commands.
- Automatic schema discovery from Tripous metadata.

## Reference From tp.Grid

Useful concepts to study:

- `tp.GridColumn`: column metadata, formatting, editor association, read-only, visible, width.
- `tp.GridInplaceEditor`: common editing lifecycle.
- `tp.GridNode` and `GridRootNode`: grouping tree model for later phases.
- `FocusedRow`: explicit current row tracking.
- `ShowEditor()` / `HideEditor()`: edit begin/end separation.
- `BuildGroups()` / `Render()`: separate data shaping from visual rendering.
- Toolbar row commands: useful later, not V1 core.

Do not copy browser DOM assumptions into Avalonia.

## Data Adapter Contract

The grid should not require a specific collection or framework model.

Initial shape to explore:

- `int RowCount`.
- `object GetRow(int rowIndex)`.
- `object GetValue(int rowIndex, GroupGridColumn column)`.
- `void SetValue(int rowIndex, GroupGridColumn column, object value)`.
- `bool CanSetValue(int rowIndex, GroupGridColumn column)`.
- Change notification for reset, row added, row removed, and cell changed.
- Optional add/delete row support later.

Open question:

- Should V1 expose a strongly typed generic adapter, a non-generic adapter, or both?
- Should notifications be .NET events on the adapter, `INotifyCollectionChanged`, or a small custom event with grid-specific change kinds?

Current leaning:

- Start non-generic internally.
- Add generic convenience adapters later.
- Use row index for V1 operations but keep `GetRow()` so external code can identify the backing object.
- Do not make `IEnumerable` the core contract. It hides indexed access and change shape.

## Data Source Strategy

Avalonia developers usually work with POCO item lists, not SQL-like tables.

The grid must support that style as a first-class scenario:

- `ObservableCollection<T>`.
- `IList<T>`.
- `IReadOnlyList<T>`.
- POCO rows with properties.
- Rows implementing `INotifyPropertyChanged`.

But POCO lists should still be adapted into the same core adapter contract.

The core grid should not know whether rows come from POCO objects, `DataView`, `DataTable`, Tripous `MemTable`, or any other source.

Public convenience APIs can exist:

- `ItemsSource`.
- explicit `DataAdapter`.
- optional auto-generated columns from POCO properties.

Adapter types can be layered:

- Built-in POCO/list adapter inside this library.
- Built-in observable collection support inside this library.
- External Tripous adapter later, outside this library.
- External `DataView`/`DataRowView` adapter later, probably from Tripous side unless we decide to support `System.Data` directly.

Current leaning:

- Core control uses `IGroupGridDataAdapter`.
- `ItemsSource` is a convenience property that creates a default adapter for common POCO list sources.
- POCO adapter listens to `INotifyCollectionChanged` when available.
- POCO adapter listens to `INotifyPropertyChanged` on row objects when available.
- Non-observable lists can still render but require explicit refresh or adapter notifications.
- DataView-style sources must be supported through adapter notifications, not by assuming `ObservableCollection<T>`.

Tripous.Avalon reference:

- Current `DataGridBinder` binds Avalonia `DataGrid` to `DataView` by wrapping it in `DataViewItemsSource`.
- `DataViewItemsSource` derives from `ObservableCollection<DataRowView>` and listens to `DataView.ListChanged`.
- Display templates listen to `DataTable.ColumnChanged` to refresh changed cells.
- Edit templates write values explicitly through `DataRowView[columnName]`.

Design consequence:

- `IGroupGridDataAdapter` must expose row collection changes.
- `IGroupGridDataAdapter` must expose cell value changes.
- The grid should not rely only on `INotifyCollectionChanged` or `INotifyPropertyChanged`.
- Cell refresh should be possible when an adapter reports row index plus column key/name.
- A DataView adapter can later translate `ListChanged` and `ColumnChanged` into grid adapter notifications.

## Column Contract

A column descriptor should contain:

- Key or name.
- Header text.
- Width.
- Minimum width.
- Visible flag.
- Read-only flag.
- Value type.
- Text alignment.
- Display formatter.
- Edit parser or converter.
- Editor kind or editor factory.
- Tag or user data.
- Whether users may resize it.
- Whether users may reorder it.
- Whether users may group by it.
- Whether users may hide/show it.

Open question:

- Should columns be plain model objects, Avalonia styled elements, or a hybrid similar to Avalonia `DataGridColumn`?

Current leaning:

- Columns should be descriptor/controller objects, not visual controls.
- The grid owns visual realization.
- Avoid making columns depend on a specific data adapter.
- Use a base `GroupGridColumn` type and derive specialized column types from it.
- Built-in columns should cover common data types.
- Custom column types must be possible without changing the grid core.

Provisional column properties:

- `Name`.
- `Header`.
- `Width`.
- `MinWidth`.
- `IsVisible`.
- `IsReadOnly`.
- `ValueType`.
- `TextAlignment`.
- `DisplayFormat`.
- `Formatter`.
- `Parser`.
- `EditorFactory`.
- `CanUserResize`.
- `CanUserReorder`.
- `CanUserGroup`.
- `CanUserHide`.

Provisional built-in column types:

- `GroupGridTextColumn`.
- `GroupGridNumberColumn`.
- `GroupGridDateColumn`.
- `GroupGridCheckBoxColumn`.
- `GroupGridComboBoxColumn` or lookup-style column later.
- `GroupGridTemplateColumn` later if needed.

Extension point:

- A column type decides how to format a value for display.
- A column type decides what editor it creates.
- A column type decides how editor values are parsed or converted.
- The grid should ask the column for display/editor behavior instead of switching on data type everywhere.
- The adapter remains responsible only for reading and writing values.

Reference note:

- The old `tp.Grid` has one `GridColumn` type with editor behavior attached to it.
- `GroupGridEngine` should improve on that by using specialized column classes while preserving a common base contract.

## Grouping And Summaries

The grid is always node-based, even when it is not grouped.

- All data rows hang under an internal root node.
- Grouping creates group nodes under the root or under parent group nodes.
- Group expand/collapse state lives on the current group node.
- A full projection rebuild creates new group nodes expanded by default.
- We do not preserve collapsed paths across full rebuilds.

This matches the practical behavior of the old `tp.Grid` and keeps grouping state local and simple.

Every group creates two non-data rows:

- A group header row.
- A group summary row.

The footer summary is a separate total-summary row in the footer band, not a body visible node.

Summary behavior:

- `Count` counts data rows.
- `Sum`, `Min`, `Max`, and `Average` operate on non-null cell values.
- Group summaries are stored on the group node and displayed by the group summary row.
- Total summaries are stored on the root node and displayed by the footer summary band.

Adapter changes:

- Reset, row add/remove/move, and row change rebuild the projection.
- Cell changes rebuild the projection when the changed column is currently grouped.
- Other cell changes recalculate summaries without rebuilding groups.

## Hit Testing

Hit testing is part of the grid core model, not only the future Avalonia visual layer.

The visual control should call the core hit-test logic and receive:

- The band that was hit.
- The hit-test kind.
- The visible node index when applicable.
- The row kind when applicable.
- The adapter row index when applicable.
- The exact `GroupGridColumn` when a column or cell is hit.
- The visible column index when applicable.

This preserves a direct route from a visual cell to its column and avoids the column-reference problem we had with Avalonia `DataGrid`.

Current non-visual hit testing uses `GroupGridLayoutMetrics`:

- Toolbar height.
- Group panel height.
- Column header height.
- Filter row height.
- Body row height.
- Footer summary height.
- Group indent width.
- Group expander hit width.
- Column resize handle width.

## Grid State

The grid must keep these concepts separate:

- Current row.
- Current column.
- Current cell.
- Focused visual cell.
- Selection.
- Editing cell.

V1 should start with single current cell and single selected row/cell behavior.

Multi-selection is not part of `GroupGrid`.

If an application needs to select multiple rows, it should add a normal boolean/checkbox column and store that selection in its data.

Current leaning:

- Current cell is logical state: row index plus column.
- Focused visual cell is rendering/focus state and may be absent while virtualized or not realized.
- Selection should not be implemented as focus.
- Editing cell should be nullable and distinct from current cell.
- V1 selection is a single selected value cell.
- `SelectedRowIndex` is derived from `SelectedCell`, not a separate row collection.
- At most one row is selected at any time.
- At most one cell is selected at any time.
- The grid has no selected row collection, selected cell collection, range selection, or selection mode.

Provisional state properties:

- `CurrentRowIndex`.
- `CurrentColumn`.
- `CurrentCell`.
- `SelectedCell`.
- `SelectedRowIndex`.
- `EditingCell`.

## Editing Lifecycle

Initial lifecycle:

- Begin edit.
- Prepare editor with current value.
- Commit edit.
- Validate value.
- Set value through adapter.
- Cancel edit.
- End edit and restore display mode.

Needed hooks:

- Can begin edit.
- Cell validating.
- Cell value committing.
- Cell value committed.
- Edit canceled.

Open question:

- Should validation use events, command-like callbacks, or adapter methods?

Current leaning:

- Use events for grid-level lifecycle hooks.
- Use adapter methods only for data access and write permission.
- Use column parser/converter for text-to-value conversion.
- A failed validation keeps the editor open.
- Escape cancels without setting the value.
- Commit should go through one path regardless of mouse, keyboard, or focus loss.
- The core model owns only editing state and lifecycle events.
- Avalonia editor controls/templates are a later visual-layer concern.

Provisional events:

- `BeginningEdit`.
- `CellValidating`.
- `CellValueCommitting`.
- `CellValueCommitted`.
- `EditCanceled`.
- `EditingCellChanged`.
- `CurrentCellChanged`.
- `CurrentRowChanged`.

## Keyboard Navigation

V1 should define predictable movement before implementation.

Initial keys:

- Arrow keys move current cell.
- Tab moves to next editable cell.
- Shift+Tab moves to previous editable cell.
- Enter begins edit or commits edit.
- Escape cancels edit.
- F2 begins edit.

Open question:

- Should Enter move vertically after commit, or stay on the same cell by default?

Current leaning:

- Arrow keys move current cell when not editing.
- Arrow keys are left to the editor while editing.
- Enter begins edit when not editing.
- Enter commits edit when editing and stays on the same cell for V1.
- Tab commits edit and moves horizontally.
- Escape cancels edit.
- F2 begins edit.
- Home/End move to first/last visible cell in the row.
- PageUp/PageDown can wait until scrolling and virtualization are clearer.
- Keyboard handlers should call core navigation/editing methods instead of implementing movement directly in the visual layer.

## Rendering Direction

Important implementation decision:

- Option A: custom `Control` with custom measure/arrange/rendering.
- Option B: composition from Avalonia panels and controls.
- Option C: derive from or wrap Avalonia `DataGrid`.

Current leaning:

- Avoid deriving from Avalonia `DataGrid`.
- Start with native Avalonia composition or custom layout.
- Decide after a small prototype evaluates focus, keyboard, and editor placement.

Current prototype:

- `GroupGridEngine` is the non-visual core engine.
- `GroupGrid` is the Avalonia `Control` surface over a `GroupGridEngine`.
- Rendering is custom and direct for now.
- Mouse click handling sets current/selected cells and toggles group expanders.
- Keyboard handling calls core navigation and edit-state methods.
- Mouse wheel changes the first visible node index for a first-pass vertical viewport.
- Current-cell keyboard navigation scrolls the current row into the viewport.
- Real scrollbar integration and editor controls are not implemented yet.
- Commit from a real editor value is not implemented yet.
- `GroupGrid` exposes convenience pass-through properties such as `Columns`, `DataAdapter`, and `LayoutMetrics`.
- `GroupGrid.ItemsSource` can create a built-in `GroupGridListDataAdapter<T>` for sources implementing `IList<T>`.
- `GroupGrid.AutoGenerateColumns` can create initial columns from public item properties when the column collection is empty.
- `GroupGrid` exposes `FirstVisibleNodeIndex` and `SetFirstVisibleNodeIndex()` for virtual viewport control.
- `GroupGrid` exposes pass-through methods for basic grouping, column visibility, and group expand/collapse operations.
- `GroupGrid` exposes pass-through methods/properties for current cell, selection, editing state, and basic navigation.
- This keeps the model stable while the visual strategy is tested.

Prototype question:

- Can a composed control using panels and lightweight cell controls handle focus/current-cell/edit placement without fighting Avalonia?
- If not, move earlier toward custom layout/rendering.

V1 layout bands:

- Toolbar.
- Group panel.
- Header row.
- Filter row.
- Viewport rows.
- Footer summary row.

The old `tp.Grid` has separate panels for toolbar, groups, columns, filters, viewport, summaries, and bottom scrollbar. That shape is useful later, but V1 should not create all bands before they have behavior.

The visual layout should follow the old `tp.Grid` structure:

- Toolbar band.
- Group panel band.
- Column captions/header band.
- Filter row band.
- Body band.
- Footer summary band.

Some bands may be hidden in V1, but their layout slots should be explicit in the model.

Display text belongs to the core model:

- Data cells use `GroupGridColumn.FormatValue()`.
- Group header rows use the group column header/name and formatted group key.
- Group summary cells use `GroupGridColumn.FormatSummaryValue()`.
- Footer summary cells use `GroupGridColumn.FormatSummaryValue()`.
- The visual layer should request display text from `GroupGridEngine` instead of duplicating formatting logic.

## Cell Identity And Hit Testing

Avoid the Avalonia `DataGrid` problem where getting from a visual cell to its owning column is hard.

Every realized cell should carry or be able to return structured group grid identity:

- Band.
- Hit kind.
- Visible node index.
- Adapter row index.
- Column reference.
- Logical cell.

The result of hit testing should be a dedicated object, not a raw visual element.

Hit testing must distinguish:

- Toolbar.
- Group panel.
- Column header.
- Column resizer.
- Filter cell.
- Body row.
- Body cell.
- Group expander.
- Footer summary cell.
- Column drag source.
- Column drop target.
- Group panel drop target.

The most important rule:

- If the pointer is over a cell, the hit-test result must directly contain the `GroupGridColumn` reference.

This should be planned before rendering, because selection, editing, resizing, sorting, filtering, context menus, and drag/drop will all use hit testing.

## Column Interaction

The grid must support desktop-style column operations:

- Drag a column header to the group panel to group by that column.
- Drag grouped columns inside the group panel to reorder grouping levels.
- Drag a grouped column back to the column header band to ungroup.
- Drag column headers to reorder visible columns.
- Drag a column header outside the grid to hide/remove that column from the visible grid.
- Drag a grouped column outside the grid to remove it from grouping.
- Drag a grouped column from the group panel to the column header band to ungroup it and insert it among visible columns.
- Hide columns.
- Show hidden columns.

These features do not have to be implemented in V1, but the model must not block them.

Design implications:

- Column order is owned by the grid.
- Group columns should probably be a separate ordered list of references to existing columns.
- Hidden columns should remain in the column collection.
- Visible value columns are derived from column collection order, visibility, and grouping state.
- Hit testing must identify column header drag sources and valid drop targets.
- Drag/drop must distinguish at least four drop zones: column header band, group panel, outside grid, and invalid/no-op.
- Dropping a header on the group panel groups the column.
- Dropping a header among headers reorders visible columns.
- Dropping a grouped column among headers ungroups it and places it in column order.
- Dropping a grouped column inside the group panel reorders grouping levels.
- Dropping a column/grouped-column outside the grid removes it from that visible role.
- A future column chooser can operate over the same column collection.

## Future Grouping Shape

The old `tp.Grid` uses `GridNode` as a tree projection over data rows. That is a useful design clue.

For Avalonia `GroupGrid`, future grouping should likely be a projection layer:

- Input rows come from the adapter.
- A row projection turns source rows into visual rows.
- Visual rows can later be data rows, group headers, group footers, and summaries.
- Current cell applies only to data rows.
- Selection rules decide whether group rows are selectable.

V1 should name internal concepts so this projection can be added later without renaming public API.

Core idea:

- The grid is always node-based, even when it is not grouped.
- All visual rows hang under a single root node.
- In the ungrouped case, the root node contains one child node per data row.
- In the grouped case, the root node contains group nodes, and group nodes contain child group nodes or data row nodes.
- This keeps rendering, navigation, expansion, summaries, and future grouping behavior on one model.

This mirrors a successful `tp.Grid` design decision and should be treated as a foundation concept, not a later feature.

## Node Model

The node model should exist from V1.

V1 node kinds:

- Root.
- Data row.

Future node kinds:

- Group header.
- Group summary/footer.
- Summary/footer row.
- Filter row if it ever participates in navigation.

The root node is never rendered as a visual row. It owns the projected tree.

The grid also keeps a flat visible-node list:

- Rebuilt from the root node.
- Used by rendering.
- Used by keyboard navigation.
- Used by scrolling and virtualization.
- Contains data-row nodes in V1.
- Can contain group/header/footer nodes later.

Important distinction:

- Source row index belongs to the adapter.
- Visual node index belongs to the current projection.
- Current cell should store the data-row node or adapter row index plus column.
- Navigation moves through visible nodes but edits only data-row nodes.

V1 build process:

- Clear root children.
- Create one data-row node per adapter row.
- Add each data-row node under root.
- Rebuild the flat visible-node list from root.
- Render from the flat visible-node list.

Future grouping build process:

- Clear root children.
- Group adapter rows into group nodes.
- For each group, create a header row node with no data row.
- Add data-row nodes under the correct parent group.
- Respect expanded/collapsed state.
- For each group, create a summary/footer row node with no data row.
- Rebuild the flat visible-node list from root.

This is not just for grouping. It also keeps the first rendering implementation close to the future model and avoids replacing row identity later.

Group rows:

- Each group contributes two extra visual rows that do not map to adapter data rows.
- The group header row displays the group title, expander, key, and count/summary hints.
- The group summary row displays aggregate values for the group.
- The grid footer summary row displays aggregate values for all visible data rows.
- Current cell and editing apply only to data-row nodes, not to group header or group summary nodes.
- Keyboard row navigation should skip group header/summary nodes for cell editing, unless a later explicit row-selection mode decides otherwise.

Summary model:

- Columns may define group summary aggregate kind.
- Columns may define total summary aggregate kind.
- Summary rows render summary values through the column.
- Initial aggregate kinds: count, sum, min, max, average.

## First Demo

The first demo should prove:

- A grid with 5-10 columns.
- 50-100 rows.
- Text, number, date, and bool values.
- Current cell highlight.
- Keyboard navigation.
- TextBox edit.
- CheckBox edit.
- Commit/cancel behavior.
- A validation error path.

No Tripous data should appear in the demo.

## Resolved Decisions

These early open decisions are resolved in the v1 implementation.

- Public grid types live under the `Avalonia.Controls` namespace.
- Core contracts, including `IGroupGridDataAdapter`, also live under `Avalonia.Controls`.
- Row operations are adapter-row-index based. The adapter also exposes `GetRow(int RowIndex)` for access to the backing source object.
- Cell values use `object` accessors in the adapter contract. Columns provide `ValueType`, formatting, parsing, and type-specific behavior.
- V1 uses a virtual body viewport with projected visible rows and custom vertical/horizontal scrolling.
- V1 exposes Avalonia styled brush properties for theming, but the control is not a `TemplatedControl`.
- Column descriptors are plain classes and do not derive from `AvaloniaObject`.
- The grid custom-renders cells. Real Avalonia controls are created only for in-place editing and editor drop-downs.

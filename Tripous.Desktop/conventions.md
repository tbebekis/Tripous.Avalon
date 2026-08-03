# Tripous Desktop Conventions

This document collects the conventions used by `Tripous.Desktop`.

It is a living document. Update it whenever a rule becomes part of the framework design.

## Application Styles

- Applications using `Tripous.Desktop` should include the shared style dictionary explicitly.
- The include path is `avares://Tripous.Desktop/Themes/TripousDesktop.axaml`.
- The dictionary contains common Tripous desktop styles for modal dialogs, toolbar panels, toolbar buttons, toolbar separators, status bars, locator buttons, error text, app forms, grid splitters, and AvaloniaEdit editor colors and text view margin.
- Application-specific styles should remain in the application `App.axaml` or its own dictionaries.
- If an application needs to override a shared Tripous desktop style, declare its override after the `TripousDesktop.axaml` include.

## AvaloniaEdit Highlighting

- Use `Highlighters.Find(HighlightMode.SQL)` for simple one-time SQL highlighting assignment.
- Use `Highlighters.Find(HighlightMode.Markdown)` for simple one-time Markdown highlighting assignment.
- Use `Highlighters.Apply(TextEditor, HighlightMode.SQL)` or `Highlighters.Apply(TextEditor, HighlightMode.Markdown)` when the editor should pick the current Light/Dark palette from its actual theme.
- Controls that support runtime theme switching should re-apply highlighting when their `ActualThemeVariant` changes.

## DataForm

- `DataForm` has a List part and an Item part.
- User initiated form actions should enter through `Execute(DataFormAction Value)`.
- `ExecuteXXXX()` methods are the action handlers.
- `ExecuteXXXX()` methods are the only methods that should directly change, or eventually cause a change to, `FormState`.
- Low level methods such as `ListSelect()`, `Insert()`, `Load()`, `Delete()` and `Save()` should be called only by the corresponding `ExecuteXXXX()` method.
- `ExecuteStartAction()` is the intentional exception to the action rule.
- `ExecuteStartAction()` may call `ExecuteEdit(RowId)` or `ExecuteInsert()` directly and delayed through the UI dispatcher.
- The startup exception exists because the List part may not be loaded yet, so `Execute(Edit)` cannot always resolve the desired row through the current list row.
- Normal UI actions should still use `Execute(DataFormAction Value)`.

## DataForm State

- `DataFormState.List` shows the List part.
- `DataFormState.Insert` and `DataFormState.Edit` show the Item part.
- `FormStateChanged()` handles the visible part switch and focus handling.
- The List part may be dirty even when the Item part is active.
- Item cancel must not mark the List part as clean.
- When returning to List, `ExecuteList()` must load/reload the list when `ListIsDirty` is true.
- `ListSelect()` should bind the list grid before the List part is shown.

## Cancel

- `ExecuteCancel()` handles the Cancel action.
- `ExecuteCancelEdit()` handles only rejecting item changes.
- When an item has changes, Cancel asks for confirmation.
- If the user confirms cancel changes, item changes are rejected, item controls are refreshed and the form stays in the Item part.
- If the user refuses cancel changes, the form stays in the Item part.
- A second Cancel, with no item changes left, returns to the List part.
- `Escape` maps to `DataFormAction.Cancel` when the cancel button is visible and enabled.

## List Target Row

- `DataForm` keeps a list target id for selecting a row after returning to List.
- Startup Edit sets the list target id from `DataFormContext.RowId`.
- Save sets the list target id from `Module.LastCommitedId`.
- `ListSelect()` uses the list target id before falling back to the current list id.
- After Insert and Save, returning to List should select the newly inserted row.
- After startup Edit, returning to List should select the edited row.

## Reference Menus

- Reference menu applies to controls or cells that edit reference values.
- Reference menu actions are:
- `Show List`
- `Reload`
- `Edit`
- `Add`
- `Clear`
- `ReferenceContextMenu` creates a `ReferenceMenuCommandContext` for each action.
- `IReferenceContextMenuHost` decides whether the menu can open and whether each action can execute.
- `ItemPage` is the default `IReferenceContextMenuHost`.
- Default `ItemPage` handling:
- `Show List` opens a modal `DataForm` with `DataFormAction.List`.
- `Edit` opens a modal `DataForm` with `DataFormAction.Edit` and the selected row id.
- `Add` opens a modal `DataForm` with `DataFormAction.Insert`.
- `Reload` reloads the lookup source and refreshes the binding.
- `Clear` clears the bound reference field.
- Modal reference forms return `DataFormContext`.
- `DataFormContext.Result` indicates modal OK.
- `DataFormContext.ResultData` contains the selected or committed row id.

## Lookup Fields

- Lookup fields are used for small reference sets.
- Lookup fields are normally displayed with a `ComboBox`.
- Lookup source lists are loaded as complete lists.
- Lookup reference menu supports `Show List`, `Reload`, `Edit`, `Add` and `Clear`.
- `Edit` is disabled when there is no selected lookup value.
- `Edit` is disabled when the selected item is the lookup null item.
- `Add` reloads the lookup source after successful modal OK and selects the returned id.
- `Show List` selects the returned id after successful modal OK.
- `Edit` reloads and refreshes the binding after successful modal OK.
- `Reload` must not clear the bound list and leave the combo box with an empty dropdown.
- Reload should create a new `LookupSource`, load it, then assign it to the binding and control.

## Locator Fields

- Locator fields are used for large reference sets.
- Locator should be used when a lookup list would be too large to load completely.
- `Locator` is the data/search engine.
- `LocatorBox` is the item form UI for locator fields.
- A locator returns one key value, usually an `Id`.
- A locator may display multiple visible text boxes, such as `Code` and `Name`.
- The user searches with a trigger term, currently ending in `?`.
- The locator search trigger is fixed and it is the `?` character.
- `Locator.Execute(Term)` runs the search.
- A single result should assign immediately.
- Multiple results should show a popup grid.
- Selecting a result should assign the locator key and any target fields.
- Locator grid columns are a separate UI projection and are not the same as lookup grid columns.
- The registration builder creates locator definitions but does not populate `LocatorDef.Fields`.
- Defining `LocatorDef.Fields` is the programmer's responsibility.
- When possible, the desktop binding layer may derive missing locator fields from the join table fields as a fallback.
- This fallback is a convenience, not a replacement for explicit locator field definitions.
- In a `LocatorBox`, the last visible locator text box should take the remaining available width.
- For non-last locator text boxes, `LocatorFieldDef.DisplayWidth` is used when it is greater than zero.
- When `DisplayWidth` is not defined, locator text box width follows conventions: `Code` fields are narrow, `Name` fields are wider, and the last field stretches.
- For common `Code` and `Name` locators, `Code` should be significantly narrower than `Name`.

## Detail Grids

- Detail grids are created by `UiItemDetails`.
- Detail grid commands are provided by `IGridHandler`.
- `ItemPage` is the default `IGridHandler`.
- Detail grid toolbar buttons are created from `GridCommand`.
- Detail grid commands should use `DetailGridCommandContext`.
- Detail grid toolbar buttons and shortcuts must call `CanExecute()` before executing.
- Detail grid Add uses `Ctrl+Insert`.
- Detail grid Delete uses `Ctrl+Delete`.
- Deleting a detail row should select the next row, previous row, or clear selection when empty.
- Detail grids support reference menus on reference columns.
- Right click on a detail grid reference cell should open the reference menu for that cell.

## Id Columns

- `btnToggleIds` controls id-column visibility.
- The setting is form-wide.
- The setting applies to the List grid and all detail grids.
- Plain columns ending in `Id` are controlled by the toggle.
- Reference columns ending in `Id` are not treated as plain id columns.
- Lookup or locator id columns may remain visible because they display meaningful reference text.
- `btnToggleIds` should be enabled regardless of whether the form is in List or Item state.

## Date Fields

- Date fields should display with date formatting in grids.
- DateTime fields should display with datetime formatting in grids.
- Field names ending in `Date` use `Sys.Settings.DateFormat`.
- Field names ending in `DateTime` or `DT` use `Sys.Settings.DateTimeFormat`.
- Date formatting should apply to both display and edit templates where practical.

## Shortcuts

- DataForm shortcuts should execute only when the corresponding toolbar button is visible and enabled.
- `List` uses `F5`.
- `Refresh List` uses `Ctrl+F5`.
- `Find` uses `Ctrl+F`.
- `Insert` uses `Ctrl+Insert`.
- `Edit` uses `Ctrl+Enter`.
- `Delete` uses `Ctrl+Delete`.
- `Save` uses `Ctrl+S`.
- `OK` uses `Ctrl+Enter`.
- `Cancel` uses `Escape`.
- In modal `DataForm` List state, `Ctrl+Enter` must execute `OK`, not `Edit`.
- In non-modal List state, `Ctrl+Enter` may execute `Edit`.
- `OK` is enabled only for modal forms in List state.
- Modal `OK` returns the selected list row id through `DataFormContext.ResultData`.
- Shortcut text should appear in toolbar button tooltips.

## Logging

- Save and Delete actions should log to `LogBox`.
- Logging belongs in the action layer, not in low-level data methods.
- Save logs after successful `Save()`.
- Delete logs after successful `Delete()`.
- Delete should capture log text before deleting the row.
- Log text should include row id.
- If `ModuleDef.ItemCaptionField` exists, log text should include its value.
- If the caption field is not `Code` and the row has a `Code` column, log text should include `Code`.

## Modal Result

- `AppForm.ModalResult` updates `Context.ModalResult`.
- `AppForm.ModalResult` calls `PassResultBack()` before closing.
- `DataForm.PassResultBack()` returns the current list id or the last committed id through `DataFormContext.ResultData`.
- Modal lookup/reference forms use `ResultData` to update the caller reference field.

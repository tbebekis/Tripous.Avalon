# Grid Navigation Spec

## Edit Mode
- Edit mode means the cell shows its inplace editor.
- When a text cell enters edit mode, the editor selects all text.
- Printable text, `Enter`, and `F2` put the current cell in edit mode.
- When a printable key opens edit mode, the typed character is not posted automatically. The editor opens with the existing value selected.

## Tab Navigation
- `Tab` moves from cell to cell.
- `Shift+Tab` moves to the previous cell.
- `Tab` works the same whether the cell is in edit mode or not.
- If the cell is in edit mode, `Tab` commits/posts the current editor value first.
- After commit, focus moves to the next or previous cell.
- When focus reaches the last cell of the row, `Tab` does not move to the next row.
- When focus reaches the first cell of the row, `Shift+Tab` does not move to the previous row.

## Arrow Navigation
- In edit mode, `Left` and `Right` are handled by the inplace editor.
- Outside edit mode, `Left` and `Right` move between cells in the current row.
- Outside edit mode, `Up` and `Down` move between rows and keep the current column.
- If the grid has focus but no current cell, arrow navigation first establishes a current cell.

## Enter
- Outside edit mode, `Enter` puts the current cell in edit mode.
- In edit mode, `Enter` commits/posts the current editor value.
- After `Enter` commits, edit mode closes and the same cell remains selected.
- `Enter` does not move to the next cell or row.

## Escape
- In edit mode, `Escape` cancels the current cell edit.
- Cancel restores the old cell value.
- After cancel, edit mode closes and the same cell remains selected.

## Boundaries
- Cell navigation stays within the current row.
- Moving to another row is done only with `Up` or `Down`.
- Keyboard navigation should not move focus outside the grid while the grid is active.

## Responsibilities
- `DataGridBinder` creates columns and inplace editors.
- `GridEditController` controls keyboard editing and navigation behavior.
- The controller tracks edit state through grid edit events, not by guessing from focused controls.

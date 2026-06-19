# Detail Grid Commands

Tripous.Desktop detail grids use a small command system for row operations.
The command system is centered on `GridCommand`, `GridCommandContext`, and `IGridHandler`.

## Main Types

- `GridActionType` defines standard actions: `Add`, `Delete`, `Edit`, and `Custom`.
- `GridCommand` describes a toolbar or shortcut command.
- `GridCommandContext` carries the command, grid, and table.
- `DetailGridCommandContext` adds the detail table information and item context.
- `IGridHandler` provides commands, checks whether they can run, and executes them.

## Default Handler

`ItemPage` implements `IGridHandler`.
Its constructor assigns itself to the item context.

```csharp
/// <summary>
/// Constructor.
/// </summary>
public ItemPage()
{
    Context = new();
    Context.GridHandler = this;
}
```

That means generated item pages get detail grid commands automatically.
`UiItemDetails` asks `context.GridHandler.GetGridCommands()` while building each detail grid toolbar.

## Default Commands

The default `ItemPage.GetGridCommands()` returns Add and Delete commands when the form is editable.
Each command contains the action type, title, tooltip, icon file, and key gesture.

```csharp
/// <summary>
/// Returns the grid commands provided by this handler.
/// </summary>
/// <returns>The grid commands.</returns>
public virtual GridCommand[] GetGridCommands()
{
    List<GridCommand> Result = new();

    Result.Add(new GridCommand()
    {
        ActionType = GridActionType.Add,
        Name = "Add",
        Title = "Add",
        ToolTip = "Add row (Shift+Insert)",
        ImageFileName = "table_add.png",
        KeyGesture = new KeyGesture(Key.Insert, KeyModifiers.Shift)
    });

    return Result.ToArray();
}
```

The current defaults are:

- Add row: `Shift+Insert`.
- Delete row: `Shift+Delete`.

Commands are not shown when `IsVisible` is false.
Buttons are disabled when `IsEnabled` is false or `CanExecute()` returns false.

## Toolbar Creation

`UiItemDetails.CreateDetailGridToolBar()` creates the toolbar buttons for a detail grid.
For each visible command it creates a button and stores the `GridCommand` in `Button.Tag`.

When a button is clicked, `UiItemDetails` creates a `DetailGridCommandContext` and calls the grid handler.

```csharp
DetailGridCommandContext Context = new()
{
    Command = Command,
    Grid = DetailInfo.Grid,
    Table = DetailInfo.Table,
    DetailInfo = DetailInfo,
    ItemContext = ItemContext
};

if (ItemContext.GridHandler.CanExecute(Context))
    ItemContext.GridHandler.Execute(Context);
```

The same command path is used by keyboard shortcuts.
The toolbar also updates button state when the grid selection changes.

## CanExecute Rules

`ItemPage.CanExecute()` protects the detail table from invalid operations.
The command can run only when:

- the context has a command, grid, and table.
- the item page is not read-only.
- the parent `DataForm` is editable.
- the form state is `Insert` or `Edit`.
- `DataForm.CanExecuteGridCommand()` allows it.
- Delete has a selected `DataRowView`.

This keeps detail rows tied to the edit lifecycle of the master form.
Users can add or delete detail rows while inserting or editing the main row, but not while browsing a read-only form.

## Add Row

The default Add command calls `Context.Table.AddNewRow()`.
Then it finds the new `DataRowView`, selects it in the grid, and restores focus to the grid.

```csharp
DataRow Row = Context.Table.AddNewRow();
DataRowView RowView = MemTable.GetDataRowView(Row, Context.Table.DataView);

if (RowView != null)
    Context.Grid.SelectedItem = RowView;

Context.Grid.Focus();
```

The row is added to the detail `MemTable`.
Saving or cancelling is still controlled by the parent `DataForm`.

## Delete Row

The default Delete command asks for confirmation before deleting the selected detail row.
After delete, it selects the next available row, the previous row, or clears the selection when the grid is empty.

```csharp
if (Context.Grid.SelectedItem is DataRowView RowView)
{
    DataRow Row = RowView.Row;
    Row.Delete();
}
```

The actual implementation also keeps `Context.Table.CurrentRowView` and `Context.Table.CurrentRow` aligned with the grid selection.

## Custom Commands

An item page may override `GetGridCommands()`, `CanExecute()`, or `Execute()` to add detail-grid actions.
Use `GridActionType.Custom` for commands outside the standard Add/Delete/Edit set.

```csharp
/// <summary>
/// Returns the grid commands provided by this handler.
/// </summary>
/// <returns>The grid commands.</returns>
public override GridCommand[] GetGridCommands()
{
    List<GridCommand> Result = base.GetGridCommands().ToList();

    Result.Add(new GridCommand()
    {
        ActionType = GridActionType.Custom,
        Name = "Recalculate",
        Title = "Recalculate",
        ToolTip = "Recalculate detail row",
        ImageFileName = "calculator.png"
    });

    return Result.ToArray();
}
```

Custom command execution should still respect the edit state and table context.

```csharp
/// <summary>
/// Executes a grid command.
/// </summary>
/// <param name="Context">The grid command context.</param>
/// <returns>The command result.</returns>
public override object Execute(GridCommandContext Context)
{
    if (Context.Command.Name == "Recalculate")
    {
        RecalculateDetailRow(Context);
        return null;
    }

    return base.Execute(Context);
}
```

## DataForm Hook

`DataForm.CanExecuteGridCommand()` is a final hook for form-level rules.
The default implementation returns true.

```csharp
/// <summary>
/// Returns true when a detail grid command can execute.
/// </summary>
/// <param name="Context">The grid command context.</param>
/// <returns>True if the command can execute; otherwise, false.</returns>
public virtual bool CanExecuteGridCommand(GridCommandContext Context) => true;
```

Override it when a form must disable detail commands because of business state, document status, user permission, or another condition outside the grid itself.

## Practical Notes

- Keep detail grid commands in the UI layer.
- Do not put modal UI or message boxes in a data module.
- Prefer overriding `ItemPage` command methods for page-specific behavior.
- Use `DataForm.CanExecuteGridCommand()` for broad form-level restrictions.
- Keep Add/Delete behavior consistent with the parent form save/cancel lifecycle.

# Reference Context Menu

`ReferenceContextMenu` is the common context menu for controls and grid columns that edit reference values.
It is used with lookup combo boxes, locator boxes, and reference columns in detail grids.

## Main Types

- `ReferenceContextMenu` creates and opens the Avalonia context menu.
- `ReferenceMenuActionType` defines the supported actions.
- `ReferenceMenuCommandContext` carries the binding, form name, row id, caller control, and command result.
- `IReferenceContextMenuHost` decides whether the menu can open, whether a command can run, and how it executes.
- `ItemPage` is the default host.

## Menu Actions

The standard actions are:

- `ShowList`, opens the reference form in list mode.
- `Reload`, reloads a lookup source.
- `Edit`, opens the selected reference item for editing.
- `Add`, opens the reference form for inserting a new item.
- `Clear`, clears the bound reference value.

Lookup references support all actions.
Locator references do not show Reload, because locators perform searches instead of holding a fixed lookup list.

## Creating The Menu

Generated item pages create reference menus while binding editors.
The menu is created from the current `FormDef`, so applications may replace the menu class if needed.

```csharp
ReferenceContextMenu Menu = FormDef.CreateReferenceContextMenu();
Menu.Initialize(this, Binding);
```

`Initialize()` stores the host and binding, assigns the menu back to `Binding.ReferenceContextMenu`, and wires the right-click behavior.

For locator boxes, the menu also opens from the locator menu button.
For combo boxes and other bound controls, the menu opens on right-click.

## Command Context

For each menu click, `ReferenceContextMenu` creates a `ReferenceMenuCommandContext`.

```csharp
ReferenceMenuCommandContext Context = new()
{
    ActionType = ReferenceMenuActionType.Edit,
    Menu = Menu,
    Binding = Binding,
    FormName = FormName,
    RowId = RowId,
    Caller = Caller
};
```

The form name comes from the lookup or locator definition.
The row id comes from the current bound value and is used only by Edit.

The menu then asks the host whether the command can execute and dispatches it.

```csharp
if (MenuHost.CanExecute(Context))
    MenuHost.Execute(Context);
```

Async results are supported.
When a command returns a `Task<DataFormContext>`, the menu awaits it and stores the form context in the command context.

## ItemPage Host

`ItemPage` implements `IReferenceContextMenuHost`.
It blocks the menu when the item page is read-only.
It also honors fields that are read-only during edit mode.

```csharp
public virtual bool CanOpenRefContextMenu(ReferenceContextMenu RefContextMenu)
{
    if (IsReadOnly)
        return false;

    if (RefContextMenu.Binding.FieldDef.IsReadOnlyEdit)
        return DataForm.FormState == DataFormState.Insert;

    return true;
}
```

`ItemPage.CanExecute()` applies the per-action rules.

- Show List and Add require a reference form name.
- Reload requires a lookup source.
- Edit requires a reference form name and a selected row id.
- Clear requires the parent form to be in `Insert` or `Edit`.

## Show List

Show List opens the reference form in list mode.
When the user selects a row and closes successfully, Tripous writes the selected reference value back to the binding.

```csharp
Context.FormContext = await DataFormContext.ShowFormModal(
    Context.FormName,
    DataFormAction.List,
    null,
    Context.Caller);
```

## Reload

Reload is for lookup sources.
It creates a new `LookupSource`, loads the list, assigns it back to the binding, and refreshes the bound control or grid column.

This avoids leaving an open combo box with an empty or stale list.

```csharp
LookupSource LookupSource = Context.Binding.LookupSource.LookupDef.Create();
List<LookupItem> List = LookupSource.GetList();
Context.Binding.LookupSource = LookupSource;
```

For a grid lookup column, the active combo box receives the new list.
For a normal combo box, the items source is replaced while the binding is in refresh mode.

## Edit

Edit opens the selected reference item in edit mode.
After a successful edit, Tripous reloads the lookup source and sets the same reference value again, so display fields refresh.

```csharp
Context.FormContext = await DataFormContext.ShowFormModal(
    Context.FormName,
    DataFormAction.Edit,
    Context.RowId,
    Context.Caller);
```

For locator references, setting the value again can locate the row by key and reassign mapped locator fields.

## Add

Add opens the reference form in insert mode.
After a successful insert, Tripous reloads lookup data and assigns the inserted id to the bound row.

```csharp
Context.FormContext = await DataFormContext.ShowFormModal(
    Context.FormName,
    DataFormAction.Insert,
    null,
    Context.Caller);
```

The inserted id is expected in `Context.FormContext.ResultData`.

## Clear

Clear sets the reference value to `DBNull.Value`.
For locator bindings, Tripous calls `Locator.Assign()` with no source row, so the key field and mapped target fields are cleared together.

```csharp
SetReferenceValue(Context, DBNull.Value);
```

## Detail Grid Reference Menus

`UiItemDetails.CreateDetailGridReferenceMenus()` creates reference menus for detail grid columns.
It scans the grid column metadata and attaches menus only to reference columns.

```csharp
foreach (GridColumnBinding Binding in DetailInfo.Grid.GetInfoList())
{
    if (!Binding.IsReference)
        continue;

    ReferenceContextMenu Menu = new();
    Menu.Initialize(MenuHost, Binding);
}
```

On right-click, the grid selects the clicked row, updates the current column, and opens the matching menu.
This makes reference actions work against the row the user clicked, not a previously selected row.

## Practical Notes

- Use reference context menus for lookup and locator fields that have related forms.
- Keep command execution in the `ItemPage` or another UI-layer host.
- Do not put reference form opening or message boxes in data modules.
- Reload only applies to lookup sources.
- Clear must clear locator target fields as well as the key field.

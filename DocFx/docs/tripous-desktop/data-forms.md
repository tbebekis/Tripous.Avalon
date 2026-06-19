# DataForm

`DataForm` is the standard Tripous.Desktop CRUD form.

It derives from `AppForm` and connects a registered `FormDef` with a `ModuleDef` and a `DataModule`.

## Purpose

A `DataForm` provides:

- a list part with a `DataGrid`;
- an item part with an `ItemPage`;
- a toolbar with list and edit actions;
- select and filter support;
- insert, edit, delete, save, cancel and refresh workflows;
- modal selection support;
- detail grid command support;
- UI logging through `LogBox`.

Most application data screens are `DataForm` instances.

## DataFormContext

`DataFormContext` resolves the registered form and module.

```csharp
DataFormContext Context = DataFormContext.Create("Customer", Ui.MainWindow);
```

The context contains:

- `RegistryName`, the form registration key.
- `FormDef`, the desktop form definition.
- `ModuleDef`, the data module definition.
- `Module`, the created or supplied `DataModule`.
- `StartAction`, the first action the form should execute.
- `InvalidActions`, actions the form must not execute.
- `RowId`, an optional row id for startup edit/delete scenarios.

To show a registered form in a pager:

```csharp
DataForm Form = PagerHandler.ShowDataForm("Customer", Ui.MainWindow);
```

To show it modally:

```csharp
DataFormContext Context = await DataFormContext.ShowFormModal(
    "Customer",
    DataFormAction.List,
    Caller: Ui.MainWindow);
```

## Form States

`DataFormState` describes what the form is currently showing.

- `List`
- `Insert`
- `Edit`

The list state shows the browser grid. Insert and edit states show the item page.

```csharp
if (FormState == DataFormState.Edit && CurrentRow != null)
{
    string Code = CurrentRow.AsString("Code");
}
```

## Form Actions

All user actions should enter through `Execute(DataFormAction)`.

The toolbar, keyboard shortcuts and mouse actions use the same action path.

Important actions:

- `List`
- `RefreshList`
- `Insert`
- `Edit`
- `Delete`
- `Refresh`
- `Save`
- `Cancel`
- `Ok`
- `Close`
- `Find`
- `ToggleIds`

The corresponding `Execute...()` methods are the action handlers and are the normal customization points.

## List Part

The list part uses the selected `SelectDef` from the module definition.

When the list is refreshed, `DataForm`:

- gets the selected `SelectDef`;
- asks the SQL filter panel for a `WHERE` clause;
- calls `Module.ListSelect()`;
- binds `Module.tblList.DataView` to the list grid;
- restores selection when possible.

The list grid is bound through `DataGridBinder`.

## Item Part

The item part is an `ItemPage`.

`DataForm` creates it from `FormDef.ItemClassName`.

```csharp
ItemPage = TypeStore.CreateInstance<ItemPage>(FormDef.ItemClassName);
ItemPage.DataForm = this;
ItemPage.Bind();
```

The default `ItemPage` can generate editors and detail grids from `ModuleDef`, `TableDef` and `FieldDef`. A custom item page can be registered when a screen needs special item UI.

## Save And Cancel

The save workflow calls `Module.Commit()`.

After saving, `DataForm`:

- stores the last committed id as the list target id;
- marks the list as dirty;
- stays in edit state;
- restores detail grid selection when possible.

Cancel is intentionally split.

- If the item has changes, cancel asks for confirmation and rejects changes.
- If there are no item changes, cancel returns to the list.
- In modal list state, cancel closes the dialog with `ModalResult.Cancel`.

## Modal Result

When a modal `DataForm` closes with OK, it returns a row id.

```csharp
DataFormContext Context = await DataFormContext.ShowFormModal("Customer");

if (Context.Result)
{
    object CustomerId = Context.ResultData;
}
```

`ResultData` is normally the last committed id, or the current selected list row id.

## Toolbar And Shortcuts

The default toolbar includes:

- Home
- List
- Refresh List
- Find
- Toggle Ids
- Insert
- Edit
- Delete
- Refresh Item
- Save
- Cancel
- OK
- Close

Common shortcuts include:

- `F5`, list.
- `Ctrl+F5`, refresh list.
- `Ctrl+F`, filters.
- `Ctrl+Insert`, insert.
- `Ctrl+Enter`, edit or modal OK.
- `Ctrl+Delete`, delete.
- `Ctrl+S`, save.
- `Escape`, cancel.

Command visibility and enablement are updated by `EnableCommands()`.

## Read-Only Forms

`FormDef.IsReadOnly` makes a form non-editable.

In read-only forms:

- insert is hidden;
- delete is hidden;
- save is hidden;
- edit may still open the item page for viewing.

This is useful for logs, balances, movements and other derived views.

## Custom Data Forms

Custom forms derive from `DataForm` and override the smallest required part.

Example: add a custom toolbar button and action.

```csharp
/// <summary>
/// Data form for sales orders.
/// </summary>
public class SalesOrderForm : DataForm
{
    // ● protected fields
    /// <summary>
    /// Button that creates a delivery note.
    /// </summary>
    protected Button btnCreateDeliveryNote;

    // ● protected
    /// <summary>
    /// Returns true when the custom action can execute.
    /// </summary>
    protected virtual bool CanCreateDeliveryNote()
    {
        return FormState == DataFormState.Edit && CurrentRow != null && !HasChanges();
    }
    /// <summary>
    /// Executes the custom action.
    /// </summary>
    protected virtual async Task ExecuteCreateDeliveryNote()
    {
        if (!CanCreateDeliveryNote())
            return;

        await Task.CompletedTask;
    }
    /// <summary>
    /// Updates command state.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();
        btnCreateDeliveryNote.IsEnabled = CanCreateDeliveryNote();
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        btnCreateDeliveryNote = ToolBar.AddButton(
            "document_export.png",
            "Create Delivery Note",
            async () => await ExecuteCreateDeliveryNote());

        return true;
    }
}
```

Register the custom form through `FormDef.ClassName`.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "SalesOrder",
    TitleKey: "SalesOrder",
    Module: "SalesOrder",
    ClassName: "SalesOrderForm",
    Group: "Sales");
```

## Useful Customization Points

- `Executing()` can intercept or cancel an action.
- `Executed()` runs after an action.
- `ExecuteCustom()` can dispatch custom actions.
- `CreateToolBar()` can add toolbar controls.
- `EnableCommands()` can update command state.
- `GetItemLogText()` can customize log text.
- `ListSelect()` can customize list loading.
- `Insert()`, `Load()`, `Delete()`, `Save()` and `Refresh()` can customize module operations when needed.

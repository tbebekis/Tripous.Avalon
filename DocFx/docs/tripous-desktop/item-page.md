# ItemPage

`ItemPage` is the item editor part of a `DataForm`.

It creates and binds the controls for the current item, including top-table fields and detail grids.

## Role

`DataForm` owns the list and the overall form lifecycle. `ItemPage` owns the item UI.

`ItemPage` is responsible for:

- creating field editors;
- binding controls to `DataRow` values;
- creating lookup and locator controls;
- creating detail grids;
- applying read-only rules;
- handling reference context menu actions;
- handling detail grid add/delete commands;
- refreshing item bindings after data changes.

## Creation

`DataForm` creates the item page from `FormDef.ItemClassName`.

```csharp
ItemPage = TypeStore.CreateInstance<ItemPage>(FormDef.ItemClassName);
ItemPage.DataForm = this;
ItemPage.Bind();
```

If no custom item page is registered, `FormDef.ItemClassName` defaults to `ItemPage`.

## Binding

`Bind()` builds the item UI once.

```csharp
public virtual void Bind() => Bind(Ui.Settings.FormColumnCount);
```

During binding, `ItemPage`:

- creates a `UiItemContext`;
- sets `Context.CreateEditorFunc`;
- creates the scroll viewer and root panel;
- decides between a single-page layout and a tabbed layout;
- creates binders for top and detail tables;
- marks binding as complete.

The editor column count normally comes from `Ui.Settings.FormColumnCount`.

## Editor Creation

`CreateEditor()` chooses the control for each `FieldDef`.

Common mappings:

- locator field -> `LocatorBox`;
- lookup field -> `ComboBox`;
- date/time field -> `CalendarDatePicker`;
- numeric field -> right-aligned `TextBox`;
- memo field -> multiline `TextBox`;
- normal field -> `TextBox`.

Example customization:

```csharp
/// <summary>
/// Creates a field editor.
/// </summary>
protected override Control CreateEditor(FieldDef Field, ItemBinder Binder)
{
    if (Field.Name == "Notes")
    {
        TextBox Box = new();
        Box.AcceptsReturn = true;
        Box.TextWrapping = TextWrapping.Wrap;
        Binder.BindMemo(Box, Field.Name, Binder.TableInfo.Table.FindColumn(Field.Name), Field);
        return Box;
    }

    return base.CreateEditor(Field, Binder);
}
```

## Read-Only Rules

`ItemPage` combines form state, field metadata and locator metadata to decide whether a control is editable.

A binding is read-only when:

- the parent `DataForm` is not editable;
- the data column is read-only;
- the field is read-only;
- the field is read-only in UI;
- the field is read-only during edit and the form is not in insert state;
- the locator is read-only.

```csharp
ItemPage.SetReadOnly(true);
ItemPage.SetReadOnly(false);
```

The same logic is applied to detail grids and detail grid columns.

## Refresh

`Refresh()` refreshes all item binders.

```csharp
if (ItemPage != null)
    ItemPage.Refresh();
```

`DataForm` calls this after cancel, refresh and other data changes that require controls to re-read the current row.

## Detail Grids

When the module has detail tables, `ItemPage` creates detail grids and their bindings.

It also supports:

- showing or hiding plain `Id` columns;
- forcing read-only mode;
- preserving detail grid selection during save;
- add/delete commands through `IGridHandler`.

```csharp
Dictionary<DataGrid, Tuple<int, DataGridColumn>> Selection =
    ItemPage.CaptureDetailGridSelection();

ItemPage.RestoreDetailGridSelection(Selection);
```

The default detail grid commands are:

- Add row, `Shift+Insert`;
- Delete row, `Shift+Delete`.

## Reference Context Menu

Lookup and locator controls can get a reference context menu.

The menu can:

- show the related list;
- reload a lookup source;
- edit the current referenced row;
- add a new referenced row;
- clear the reference value.

`ItemPage` implements `IReferenceContextMenuHost` and executes those commands.

Example flow:

- user opens a lookup context menu;
- user selects Add;
- `ItemPage` opens the referenced form modally with `DataFormAction.Insert`;
- if the modal form returns OK, `ItemPage` reloads the lookup and assigns the new value.

## Custom Item Pages

Use a custom item page when generated layout is close, but one form needs extra item UI behavior.

Register it through `ItemClassName`.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "SalesOrder",
    TitleKey: "SalesOrder",
    Module: "SalesOrder",
    ClassName: "SalesOrderForm",
    Group: "Sales",
    ItemClassName: "TradeItemPage");
```

Good customization points:

- `CreateEditor()`
- `Bind(int ColumnCount)`
- `SetReadOnly()`
- `Refresh()`
- `CanExecute(ReferenceMenuCommandContext Context)`
- `Execute(ReferenceMenuCommandContext Context)`
- `GetGridCommands()`
- `CanExecute(GridCommandContext Context)`
- `Execute(GridCommandContext Context)`

For most forms, overriding `CreateEditor()` or adding custom controls after `base.Bind()` is enough.

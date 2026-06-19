# Bindings

Tripous.Desktop bindings connect Avalonia controls to `DataRow` values.

They are used by generated item pages, lookup controls, locator controls and detail grids.

## Main Types

- `TripousBinding`
- `ControlBinding`
- `ItemBinder`
- `ControlBindingHelper`
- `DataViewItemsSource`
- `GridColumnBinding`

This article focuses on item control bindings. Grid column bindings are covered separately in the DataGrid binding article.

## TripousBinding

`TripousBinding` is the base binding object.

It stores common binding metadata:

- `FieldName`
- `DataColumn`
- `FieldDef`
- `DataType`
- `LookupSource`
- `Locator`
- `LocatorDef`
- `LocatorTargetFieldMap`
- `ReferenceContextMenu`
- `IsRefreshing`
- `DisposeAction`

`IsRefreshing` is important. It prevents event handlers from writing back to the row while a control is being refreshed from the row.

## ControlBinding

`ControlBinding` adds the Avalonia control.

```csharp
ControlBinding Binding = new()
{
    Control = TextBox,
    FieldName = "Name",
    DataColumn = Table.Columns["Name"],
    FieldDef = Field
};
```

Generated item pages use `ControlBinding` for text boxes, combo boxes, check boxes, date controls, image controls and locator boxes.

## ItemBinder

`ItemBinder` owns the control bindings for one current row provider.

It watches:

- current row changes;
- column changes in the current row.

When the current row changes, all controls refresh. When a bound column changes, only the related controls refresh.

```csharp
ItemBinder Binder = new();
Binder.RowProvider = Module.tblItem;
Binder.TableInfo = TableInfo;
```

`ItemPage` normally creates and owns item binders automatically.

## Binding Controls

`ItemBinder` exposes helper methods for common controls.

```csharp
DataColumn Column = Table.FindColumn("Name");

ControlBinding Binding = Binder.Bind(
    TextBox,
    "Name",
    Column,
    FieldDef);
```

Supported control bindings include:

- `TextBox`
- memo `TextBox`
- `CheckBox`
- `DatePicker`
- `CalendarDatePicker`
- `ComboBox`
- `ListBox`
- `NumericUpDown`
- lookup `ComboBox`
- `Image`
- `LocatorBox`

## Refresh Flow

`ControlBindingHelper.Refresh()` reads the current row and updates the control.

Examples:

- `TextBox.Text` is refreshed from the row value.
- `ComboBox.SelectedItem` is refreshed from a `LookupSource`.
- `CheckBox.IsChecked` is refreshed from boolean or integer-backed boolean fields.
- `Image.Source` is refreshed from a file path or `byte[]`.
- `LocatorBox` refreshes key and target display boxes.

The refresh code sets `Binding.IsRefreshing = true` while updating the control.

## Write Back Flow

Each binding subscribes to the relevant control event.

Examples:

- `TextBox.TextChanged`
- `CheckBox.IsCheckedChanged`
- `ComboBox.SelectionChanged`
- `CalendarDatePicker.SelectedDateChanged`
- `NumericUpDown.ValueChanged`
- `LocatorBox.RowSelected`

When the event fires, the binding writes the converted value back to the current row.

```csharp
if (!Binding.IsRefreshing)
{
    Row["Name"] = TextBox.Text;
}
```

The actual helper converts values according to the target `DataColumn.DataType`.

## Lookup Bindings

Lookup combo boxes use a `LookupSource`.

```csharp
ControlBinding Binding = Binder.BindLookup(
    ComboBox,
    "CountryId",
    Table.FindColumn("CountryId"),
    FieldDef);
```

The binding:

- creates the lookup source from `LookupDef`;
- sets the combo box item source;
- selects the item matching the current row value;
- writes the selected `LookupItem.Value` back to the row;
- assigns lookup snapshot fields when defined.

The selected item display relies on `LookupItem.ToString()`.

## Locator Bindings

Locator bindings connect a `LocatorBox` to a locator field.

```csharp
ControlBinding Binding = Binder.Bind(LocatorBox, FieldDef);
```

The binding:

- finds the `LocatorDef`;
- creates and initializes the `Locator`;
- ensures visible locator fields when possible;
- creates the locator target-field map;
- refreshes locator display boxes from the current row;
- assigns selected locator values back to the row.

Locator assignment uses the same target-field mapping rules described in the Tripous.Data locator documentation.

## Disposal

Bindings that subscribe to control events set `DisposeAction`.

```csharp
Binding.Dispose();
```

This removes event handlers where needed. It is mainly useful for long-lived custom UI or controls that are created and destroyed manually.

## DataViewItemsSource

`DataViewItemsSource` adapts a `DataView` to an observable collection of `DataRowView`.

It is useful for Avalonia controls that need collection change notifications.

```csharp
DataViewItemsSource ItemsSource = new(Module.tblList.DataView);
```

It listens to `DataView.ListChanged` and mirrors additions, deletions, moves and reloads.

## Custom Binding Use

Most code should use the generated item UI. Custom item pages can use `ItemBinder` directly when adding special controls.

```csharp
TextBox NotesBox = new();
DataColumn Column = Module.tblItem.FindColumn("Notes");
FieldDef Field = ModuleDef.Table.Fields.Find("Notes");

ItemBinder.BindMemo(NotesBox, "Notes", Column, Field);
```

Use the existing binder instead of creating an independent binding system. That keeps row changes, refreshes and read-only handling consistent with the rest of the form.

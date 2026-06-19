# Generated Item UI

Tripous.Desktop can generate an item editor from data descriptors.

The generated item UI is built from `ModuleDef`, `TableDef`, `FieldDef`, lookup definitions and locator definitions. This is what allows a normal `DataForm` to display useful item editors without hand-written UI for every table.

## Main Types

- `ItemPage`
- `UiItemContext`
- `UiItemPage`
- `UiItemDetails`
- `UiItemInfo`
- `UiTableInfo`
- `UiDetailTableInfo`
- `UiFactory`
- `ItemBinder`

## Descriptor Driven UI

The source of the generated UI is the data registry.

- `ModuleDef` identifies the top table and select list.
- `TableDef` describes the top table, one-to-one details and one-to-many details.
- `FieldDef` describes each field and its UI behavior.
- `LookupDef` drives combo box lists.
- `LocatorDef` drives locator boxes and locator grid columns.

The same descriptor metadata that drives data access also drives the desktop item UI.

## UiItemContext

`UiItemContext` holds the shared state while the item page is being built.

It contains:

- the `DataModule`;
- the top table UI information;
- the top `ItemBinder`;
- all item binders;
- the visual column count;
- the detail grid handler;
- the editor creation function.

When the module is assigned, the context creates the top `UiTableInfo` and connects the main binder to the top table row provider.

## Top Fields

Top-table fields appear first.

Only bindable fields are displayed. Fields are grouped by `FieldDef.Group`. Empty group names become `General`.

Each group is displayed inside an `Expander`.

```text
General
    Code
    Name
    IsActive

Address
    Street
    City
    CountryId
```

Inside each group, fields are split into one to three visual columns.

The column count comes from:

```csharp
Ui.Settings.FormColumnCount
```

The maximum number of controls per column comes from:

```csharp
Ui.Settings.FormMaxControlsPerColumn
```

## Field Editors

The generated editor depends on `FieldDef`.

- Boolean fields become `CheckBox`.
- Locator fields become `LocatorBox`.
- Lookup fields become `ComboBox`.
- Date/time fields become `CalendarDatePicker`.
- Numeric fields become right-aligned `TextBox`.
- Memo fields become multiline `TextBox`.
- Large memo fields become standalone expander groups.
- Image fields become image preview controls.
- Normal fields become `TextBox`.

`UiFactory.CreateFieldLabel()` creates labels. Required non-boolean fields get an asterisk.

## Single Page Layout

When the top table has no multi-row details, the item page uses a simple layout.

```text
Top field groups
One-to-one detail groups
```

This is created by:

```csharp
UiItemPage.CreateSinglePageLayout(Context);
```

## Tabbed Layout

When the top table has multi-row details, the item page uses a root `TabControl`.

```text
Top table tab
    Top field groups
    One-to-one detail groups
    First-level detail tabs

Subdetail tabs
```

This is created by:

```csharp
UiItemPage.CreateTabbedTopLayout(Context);
```

The first tab is always the top table page.

## One-To-One Details

One-to-one detail tables are displayed as controls, not grids.

They use the same generated field group layout as the top table. Their binders are created with `UiItemContext.CreateOneToOneBinder()`.

This is useful for table splits where several tables behave as one logical item.

## One-To-Many Details

One-to-many details are displayed as `DataGrid` controls.

Detail grids:

- are never placed inside expanders;
- get their own tab or detail area;
- may have a toolbar above the grid;
- are bound to the detail `MemTable.DataView`;
- skip memo, large memo and image fields as automatic columns;
- skip locator snapshot fields as automatic columns.

The default minimum detail grid height comes from:

```csharp
Ui.Settings.DetailGridMinHeight
```

## Subdetails

Subdetails are supported recursively.

If a detail has child details, Tripous creates a split layout:

```text
Parent detail grid
Splitter
Child detail area
```

When multiple child details exist, the child area uses tabs.

## Detail Grid Columns

Detail grid columns are created from bindable fields.

- Lookup fields use lookup columns.
- Locator fields may create visible locator display columns.
- Normal fields use normal grid columns.
- Memo and image fields are not automatic detail grid columns.

Locator detail columns use the same target-field mapping rules described in the Tripous.Data locator documentation.

## Detail Toolbars

Each detail may have a toolbar area above the grid.

The toolbar area is initially created by `UiFactory.CreateToolBarBorder()` and `UiFactory.CreateToolBarPanel()`.

`UiItemDetails.CreateDetailGridToolBar()` asks the current grid handler for commands. The default `ItemPage` grid handler provides Add and Delete commands when the form is editable.

## Reference Menus

Generated lookup, locator and reference grid columns may receive a reference context menu.

The menu allows the user to:

- show the referenced list;
- reload lookup data;
- edit the referenced row;
- add a referenced row;
- clear the reference.

This is wired through `IReferenceContextMenuHost`, implemented by `ItemPage`.

## Customization

The usual customization point is a custom `ItemPage`.

Register it with `FormDef.ItemClassName`.

```csharp
DesktopRegistry.AddOrUpdateForm(
    "PurchaseInvoice",
    TitleKey: "PurchaseInvoice",
    Module: "PurchaseInvoice",
    ClassName: "PurchaseInvoiceForm",
    Group: "Purchases",
    ItemClassName: "TradeItemPage");
```

Common customizations:

- override `CreateEditor()` for a special field editor;
- override `Bind()` for extra UI around generated controls;
- override grid command methods for detail behavior;
- override reference menu methods for custom reference workflows.

For most screens, the descriptor metadata should do the work. Custom item pages should be used when a screen has behavior that cannot be expressed cleanly with descriptors.

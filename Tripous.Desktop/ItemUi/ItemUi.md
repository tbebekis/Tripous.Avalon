# Item UI

The Item UI helpers let you build item-entry UIs with the same layout rules and visual style used by the default `ItemPage`, without copying its implementation.

Use them when:

- a derived `ItemPage` needs a custom layout but should keep the standard Tripous item-page look
- a custom item-entry window is built directly around a `DataModule`
- you need the default field grouping, one-to-one detail handling, or detail grids in only part of a custom screen

The default implementation remains the best reference. Start with `ItemPage.Bind()` when you want to see the normal initialization order end to end.

## Main Types

| Type                | Purpose                                                                    |
| ------------------- | -------------------------------------------------------------------------- |
| `UiItemContext`     | Shared state used while building and binding an item UI.                   |
| `UiFactory`         | Reusable control factories used by the default item UI.                    |
| `UiItemInfo`        | Builds the runtime UI metadata tree for the top table and visible details. |
| `UiItemPage`        | Builds field groups, field rows, and the top-table layout.                 |
| `UiItemDetails`     | Builds one-to-one detail sections, detail tabs, and detail grids.          |
| `UiTableInfo`       | Runtime UI information for one single-row table.                           |
| `UiFieldInfo`       | Runtime association between a field and its generated control.             |
| `UiDetailTableInfo` | Runtime UI information for one multi-row detail table.                     |

## Choose the Extension Style

### Derive from `ItemPage`

This is the normal choice when you still want to use `DataForm`.

`DataForm` creates an `ItemPage` from `FormDef.ItemClassName`, assigns its `DataForm`, adds it to the form, and calls `Bind()`. A derived page can override the parts it needs while keeping the normal form lifecycle.

Typical reasons:

- custom field editor creation
- custom layout around otherwise standard field groups
- extra controls before or after the standard layout

### Build a Fully Custom Item Window

This is the choice when the standard `DataForm` shell is not suitable and you are building your own window around a `DataModule`.

The same Item UI helpers are still useful. You create a `UiItemContext`, give it the `DataModule`, provide a field-editor factory, choose a root control, and call the layout helpers you need.

Note that this is a custom window, not a drop-in replacement inside the current `DataForm` implementation. Inside `DataForm`, the supported replacement point is still a derived `ItemPage`.

## Initialization Order

For a custom item UI, the normal order is:

1. Create a `UiItemContext`.
2. Assign `context.Module`.
3. When binding starts, assign `context.CreateEditorFunc`.
4. Create the visual root controls.
5. Set `context.ParentControl`.
6. Set `context.ColumnCount`.
7. Choose `UiItemPage.CreateSinglePageLayout(context)` or `UiItemPage.CreateTabbedTopLayout(context)`.

Setting `context.Module` is important. It creates `context.TopTableUiInfo`, initializes the main `ItemBinder.RowProvider`, and assigns the main `ItemBinder.TableInfo`.

## Minimal Custom Item Page

This example builds a custom item-entry control with the same default layout rules as `ItemPage`.

```csharp
public class CustomerEntryPage : UserControl
{
    readonly UiItemContext Context = new();

    public CustomerEntryPage(DataModule Module)
    {
        Context.Module = Module;
    }

    public void Bind()
    {
        Context.CreateEditorFunc = CreateEditor;

        // current row events, if needed
        // Context.ItemBinder.CurrentRowChanging += (s, ea) => CurrentRowChanging?.Invoke(this, EventArgs.Empty);
        // Context.ItemBinder.CurrentRowChanged += (s, ea) => CurrentRowChanged?.Invoke(this, EventArgs.Empty);

        ScrollViewer ScrollViewer = UiFactory.CreateScrollViewer();
        StackPanel Root = UiFactory.CreateStackPanel();
        ScrollViewer.Content = Root;
        Content = ScrollViewer;

        Context.ParentControl = Root;
        Context.ColumnCount = Ui.Settings.FormColumnCount;

        if (Context.TopTableUiInfo.DetailList.Count == 0)
            UiItemPage.CreateSinglePageLayout(Context);
        else
            UiItemPage.CreateTabbedTopLayout(Context);
    }

    Control CreateEditor(FieldDef Field, ItemBinder Binder)
    {
        Control Result;
        DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);

        if (Field.IsDateTime)
        {
            DatePicker Box = new();
            Binder.Bind(Box, Field.Name, DataColumn, Field);
            Result = Box;
        }
        else
        {
            TextBox Box = new();
            Binder.Bind(Box, Field.Name, DataColumn, Field);
            Result = Box;
        }

        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 0, 0, 6);
        return Result;
    }
}
```

This example is intentionally small. The split between construction and `Bind()` mirrors the default `ItemPage`: construction stores the long-lived state, while binding wires the field editor factory and builds the visual tree. If you want the default editor behavior for locators, lookups, memos, numeric fields, and reference context menus, copy the policy from `ItemPage.CreateEditor()` or derive from `ItemPage` instead.

## Derived `ItemPage` Example

If only one or two field types need custom treatment, deriving from `ItemPage` is usually simpler.

```csharp
[TypeStore]
public class CustomerItemPage : ItemPage
{
    protected override Control CreateEditor(FieldDef Field, ItemBinder Binder)
    {
        if (Field.Name == "Notes")
        {
            TextBox Box = new()
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                MinHeight = 120
            };

            DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
            Binder.BindMemo(Box, Field.Name, DataColumn, Field);
            return Box;
        }

        return base.CreateEditor(Field, Binder);
    }
}
```

Register the class through `FormDef.ItemClassName` so `DataForm` creates it instead of the default `ItemPage`.

## Reusing Only Part of the Default UI

You do not have to use the full default layout. The helpers are composable.

### Field Groups Only

```csharp
UiItemPage.CreateFieldGroups(
    context,
    parentControl,
    context.TopTableUiInfo,
    context.ItemBinder,
    context.ColumnCount
);
```

### One-to-One Details Only

```csharp
UiItemDetails.CreateOneToOneDetails(
    context,
    parentControl,
    context.TopTableUiInfo.TableDef
);
```

### First-Level Multi-Row Detail Tabs Only

```csharp
UiItemDetails.CreateFirstLevelDetails(context, parentControl);
```

### One Specific Detail Grid

```csharp
DataGrid Grid = UiItemDetails.CreateDetailDataGrid(context, detailTableDef);
```

## Keeping the Standard Look

Prefer the shared factories when you want the screen to look like the default `ItemPage`:

- `UiFactory.CreateScrollViewer()`
- `UiFactory.CreateStackPanel()`
- `UiFactory.CreateExpander(...)`
- `UiFactory.CreateTabControl()`
- `UiFactory.CreateFieldLabel(...)`
- `UiFactory.CreateLargeMemoEditor(...)`
- `UiFactory.CreateImageControl(...)`

Prefer the shared layout helpers when you want the same grouping and spacing rules:

- `UiItemPage.CreateFieldGroups(...)`
- `UiItemPage.CreateLargeMemoGroups(...)`
- `UiItemPage.CreateSinglePageLayout(...)`
- `UiItemPage.CreateTabbedTopLayout(...)`
- `UiItemDetails.CreateOneToOneDetails(...)`
- `UiItemDetails.CreateFirstLevelDetails(...)`

## Reference Context Menus

The default `ItemPage.CreateEditor()` creates lookup combo boxes and attaches `ReferenceContextMenu` instances where appropriate.

If you build a fully custom item-entry UI and you also want the same reference menu behavior, you must provide the same host behavior yourself:

- implement `IReferenceContextMenuHost`
- create the lookup binding
- create the menu through `FormDef.CreateReferenceContextMenu()`
- call `Initialize(host, binding)`

If your custom UI lives inside a derived `ItemPage`, this behavior is already available through the base implementation.

## Checklist

Before treating a custom item UI as complete, verify that:

- `context.Module` is assigned before using `context.TopTableUiInfo`
- `context.CreateEditorFunc` is assigned before creating field groups
- `context.ParentControl` points to the control that should receive generated content
- `context.ColumnCount` is set before layout creation
- one-to-one detail binders are added through `UiItemDetails.CreateOneToOneDetails(...)`
- each custom editor binds itself through the relevant `ItemBinder`
- lookup controls that need the normal reference menu behavior are initialized explicitly
- `ItemPage.Bind()` has been reviewed when matching the default startup sequence matters

## Practical Rule

Use the smallest amount of custom code that solves the actual problem:

- derive from `ItemPage` when the standard form shell still fits
- compose `UiItem*` helpers when the page layout must be custom
- build a fully custom window only when the `DataForm` shell itself is the wrong abstraction

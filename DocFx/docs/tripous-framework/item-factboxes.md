# Item FactBoxes

Item FactBoxes are readonly contextual information panels shown next to the item part of a data form.

They are intended for information that helps the user understand the current row without turning internal calculation or audit tables into editable detail tabs.

Typical examples include:

- Document totals
- Tax analysis
- Customer balance
- Credit limit
- Stock availability
- Last transactions
- Audit information
- Module structure information

FactBoxes are part of the desktop-like data entry model used by both `Tripous.Desktop` and `Tripous.WebDesk`.

## Built-In Info Sheet

The framework provides one built-in standard sheet named `Info`.

The built-in `Info` sheet is always created by the item page framework. It is not registered in `ModuleDef.FactBoxes`.

It combines:

- Current item information, produced by `ItemInfoFactBoxProvider`.
- Module, table and field structure information, produced by `ItemStructureFactBoxProvider`.

The structure section shows the module tree, table visibility, master-detail relationships, and field metadata.

This makes every standard data form self-describing, even when the application does not register any custom FactBoxes.

## Custom FactBoxes

Custom FactBoxes are declared with `ItemFactBoxDef`.

`ItemFactBoxDef` is the single declaration used by both desktop and web.

Important properties are:

- `Name`: The FactBox registration name.
- `TitleKey`: The displayed title.
- `ProviderClassName`: The provider class that gathers the FactBox data.
- `DesktopControlClassName`: Optional desktop control renderer.
- `WebViewName`: Optional WebDesk Razor partial view name or path. When empty, the generic server renderer is used.
- `IsVisible`: Whether the FactBox is visible by default.

Custom FactBoxes are registered on `ModuleDef.FactBoxes`.

Desktop also supports `FormDef.FactBoxes` as a form-specific extension point.

WebDesk supports `WebFormDef.FactBoxes` as the equivalent web form-specific extension point.

When both module-level and form-level FactBoxes are present, module-level FactBoxes are added first. Duplicate names are ignored after the first match.

The TinyERP sample keeps a small demonstration of all extension points:

- `Company Summary` is registered on `ModuleDef.FactBoxes`.
- `Company Desktop Form Info` is registered on desktop `FormDef.FactBoxes`.
- `Company WebForm Info` is registered on `WebFormDef.FactBoxes`.

The two form-level examples intentionally show different form-scope information, not the same company summary data.

## Providers

A FactBox provider derives from `ItemFactBoxProvider`.

The provider receives an `ItemFactBoxContext` and returns serializable data.

Providers belong to the non-UI data side of the application. They must not show windows, dialogs, message boxes, notifications, or wait for user interaction.

`ItemFactBoxProvider` is marked with `[TypeStore]`. Provider subclasses do not need their own `[TypeStore]` attribute.

The provider may use:

- `Context.Module`
- `Context.ModuleDef`
- `Context.TableDef`
- `Context.Row`
- `Context.KeyValue`
- `Context.RowState`
- `Context.FactBoxDef`

The same provider is used by desktop and web.

Simple key/value data is a good default shape:

```csharp
/// <summary>
/// Provides summary information for the Company item FactBox.
/// </summary>
public class CompanySummaryFactBoxProvider: ItemFactBoxProvider
{
    // ● public
    /// <summary>
    /// Creates serializable data for a FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The serializable FactBox data.</returns>
    public override object GetData(ItemFactBoxContext Context)
    {
        DataModule Module = Context?.Module;
        DataRow CompanyRow = Context?.Row ?? Module?.tblItem?.CurrentRow;

        return new Dictionary<string, object>
        {
            ["Company"] = CompanyRow?.AsString("Name"),
            ["Tax Number"] = CompanyRow?.AsString("TaxNumber")
        };
    }
}
```

## Registration

A module can register a custom FactBox after modules are registered.

```csharp
ModuleDef Module = DataRegistry.Modules.Find("Company");
if (Module != null && !Module.FactBoxes.Contains("CompanySummary"))
{
    Module.FactBoxes.Add(new ItemFactBoxDef
    {
        Name = "CompanySummary",
        TitleKey = "Company Summary",
        ProviderClassName = typeof(CompanySummaryFactBoxProvider).FullName,
        DesktopControlClassName = "Tripous.Desktop.ItemInfoFactBoxControl",
        WebViewName = "FactBoxes/CompanySummary"
    });
}
```

This produces:

- The built-in `Info` tab.
- A custom `Company Summary` tab after it.

The sample also registers form-level FactBoxes for the same `Company` module to demonstrate that form-specific sheets are appended after module-level sheets.

## Desktop Rendering

In desktop applications, the item page creates the FactBox pane and tab control.

The built-in `Info` sheet is created automatically.

For custom FactBoxes:

- If `DesktopControlClassName` is set, the framework creates that `ItemFactBoxControl`.
- If a simple key/value display is enough, `Tripous.Desktop.ItemInfoFactBoxControl` can be used.
- For richer desktop UI, create a custom control deriving from `ItemFactBoxControl`.

The desktop control receives the same `ItemFactBoxContext` and provider data returned by the provider.

## Web Rendering

WebDesk renders FactBoxes server-side.

`DataModule.GetFactBoxes` returns server-rendered HTML, the FactBox count, and the initial pane visibility flag.

The browser only injects the returned HTML and initializes Tripous controls such as `tp.TabControl` and `tp.Accordion`.

For custom web FactBoxes:

- If `WebViewName` is empty, the generic server renderer is used.
- If `WebViewName` is set, WebDesk renders the specified Razor partial.

The Razor partial receives the provider data as its model.

It also receives these `ViewData` entries:

- `FactBoxContext`
- `FactBoxDef`

Example partial:

```cshtml
@model IReadOnlyDictionary<string, object>
@{
    string ValueOf(string Key)
    {
        if (Model != null && Model.TryGetValue(Key, out object Value) && Value != null)
            return Convert.ToString(Value, CultureInfo.CurrentCulture) ?? string.Empty;
        return string.Empty;
    }
}

<div class="tp-WebDataForm-FactBoxSummary">
    <div class="tp-WebDataForm-FactBoxSummaryTitle">@ValueOf("Company")</div>
    <div class="tp-WebDataForm-FactBoxSummaryGrid">
        <div>
            <span>Tax Number</span>
            <strong>@ValueOf("Tax Number")</strong>
        </div>
    </div>
</div>
```

## Design Rules

FactBoxes are readonly contextual information.

Use a FactBox when the user needs information near the current item, but should not edit that information as part of the normal detail grid workflow.

Do not use a FactBox as a replacement for required editable fields or editable detail tables.

Do not put UI behavior in the provider. The provider gathers data and returns serializable results. Rendering belongs to the desktop control or web partial.

Keep the built-in `Info` sheet unregistered. `ModuleDef.FactBoxes`, `FormDef.FactBoxes`, and `WebFormDef.FactBoxes` are for custom application sheets only.

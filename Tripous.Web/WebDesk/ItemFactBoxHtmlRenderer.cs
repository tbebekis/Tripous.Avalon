/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Renders item FactBoxes as WebDesk HTML.
/// </summary>
public class ItemFactBoxHtmlRenderer
{
    // ● private
    IViewToStringConverter fViewToStringConverter;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemFactBoxHtmlRenderer()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ViewToStringConverter">The view-to-string converter.</param>
    public ItemFactBoxHtmlRenderer(IViewToStringConverter ViewToStringConverter)
    {
        fViewToStringConverter = ViewToStringConverter;
    }

    // ● protected
    /// <summary>
    /// HTML encodes a value.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns>The encoded text.</returns>
    protected virtual string Encode(object Value) => System.Text.Encodings.Web.HtmlEncoder.Default.Encode(Convert.ToString(Value, CultureInfo.CurrentCulture) ?? string.Empty);
    /// <summary>
    /// Formats JavaScript and server class information.
    /// </summary>
    /// <param name="JsName">The JavaScript class name.</param>
    /// <param name="ServerName">The server class name or Razor path.</param>
    /// <returns>The formatted text.</returns>
    protected virtual string FormatClassInfo(string JsName, string ServerName)
    {
        JsName = JsName ?? string.Empty;
        ServerName = ServerName ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(JsName) && !string.IsNullOrWhiteSpace(ServerName))
            return $"{JsName} ({ServerName})";
        return JsName.Length > 0 ? JsName : ServerName;
    }
    /// <summary>
    /// Returns the effective Razor view path.
    /// </summary>
    /// <param name="ViewName">The view name.</param>
    /// <returns>The effective Razor view path.</returns>
    protected virtual string GetViewPath(string ViewName)
    {
        if (string.IsNullOrWhiteSpace(ViewName))
            return string.Empty;
        if (ViewName.StartsWith("/", StringComparison.Ordinal))
            return ViewName;

        string ViewFileName = ViewName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? ViewName : $"{ViewName}.cshtml";
        return $"/Views/WebForms/{ViewFileName}";
    }
    /// <summary>
    /// Renders a key/value row.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Key">The row key.</param>
    /// <param name="Value">The row value.</param>
    protected virtual void RenderKeyValue(StringBuilder Builder, string Key, object Value)
    {
        Builder.Append("<div class=\"tp-WebDataForm-FactBoxKeyValue\"><span class=\"tp-WebDataForm-FactBoxKey\">");
        Builder.Append(Encode(Key));
        Builder.Append("</span><span class=\"tp-WebDataForm-FactBoxValue\">");
        Builder.Append(Encode(Value));
        Builder.Append("</span></div>");
    }
    /// <summary>
    /// Renders item row information.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Dictionary">The item information.</param>
    protected virtual void RenderItemInfo(StringBuilder Builder, IReadOnlyDictionary<string, object> Dictionary)
    {
        if (Dictionary == null)
            return;

        foreach (KeyValuePair<string, object> Pair in Dictionary)
            RenderKeyValue(Builder, Pair.Key, Pair.Value);
    }
    /// <summary>
    /// Renders item row information.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Dictionary">The item information.</param>
    protected virtual void RenderDictionary(StringBuilder Builder, IDictionary Dictionary)
    {
        if (Dictionary == null)
            return;

        foreach (DictionaryEntry Entry in Dictionary)
            RenderKeyValue(Builder, Convert.ToString(Entry.Key, CultureInfo.CurrentCulture), Entry.Value);
    }
    /// <summary>
    /// Renders module structure information.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Structure">The structure information.</param>
    protected virtual void RenderStructureInfo(StringBuilder Builder, ItemStructureFactBoxData Structure)
    {
        if (Structure == null)
            return;

        RenderKeyValue(Builder, "Group", Structure.ModuleGroup);
        RenderKeyValue(Builder, "Module Class", FormatClassInfo(Structure.ModuleJsClassName, Structure.ModuleClassName));
        RenderKeyValue(Builder, "Form Class", FormatClassInfo(Structure.FormJsClassName, Structure.FormClassName));
        RenderKeyValue(Builder, "ItemPage Class", FormatClassInfo(Structure.ItemPageJsClassName, Structure.ItemPageClassName));
        RenderKeyValue(Builder, "Tables", $"{Structure.VisibleTableCount}/{Structure.TableCount} visible");
        if (Structure.Table != null)
            RenderTableAccordion(Builder, Structure.Table);
    }
    /// <summary>
    /// Renders a table accordion.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Table">The top table.</param>
    protected virtual void RenderTableAccordion(StringBuilder Builder, ItemStructureTableInfo Table)
    {
        Builder.Append("<div class=\"tp-WebDataForm-FactBoxAccordion\" data-allow-multi-expand=\"true\">");
        RenderTableAccordionItem(Builder, Table, 0);
        Builder.Append("</div>");
    }
    /// <summary>
    /// Renders a table accordion item.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Table">The table information.</param>
    /// <param name="Level">The table level.</param>
    protected virtual void RenderTableAccordionItem(StringBuilder Builder, ItemStructureTableInfo Table, int Level)
    {
        if (Table == null)
            return;

        string VisibleText = Table.IsUiVisible ? "visible" : "hidden";
        string DetailText = Table.IsDetail ? "detail" : "top";
        string Title = $"{Table.Title} ({Table.Name}) - {VisibleText}, {DetailText}, fields {Table.VisibleFieldCount}/{Table.FieldCount}";
        Builder.Append(Level == 0 ? "<div class=\"tp-Expanded\">" : "<div>");
        Builder.Append("<div style=\"padding-left: ");
        Builder.Append(14 + Level * 12);
        Builder.Append("px\">");
        Builder.Append(Encode(Title));
        Builder.Append("</div><div class=\"tp-WebDataForm-FactBoxTableBody\">");
        RenderTableInfo(Builder, Table);
        Builder.Append("</div></div>");

        foreach (ItemStructureTableInfo Detail in Table.Details)
            RenderTableAccordionItem(Builder, Detail, Level + 1);
    }
    /// <summary>
    /// Renders table information.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Table">The table information.</param>
    protected virtual void RenderTableInfo(StringBuilder Builder, ItemStructureTableInfo Table)
    {
        RenderKeyValue(Builder, "Alias", Table.Alias);
        RenderKeyValue(Builder, "Master", Table.MasterName);
        RenderKeyValue(Builder, "Details", string.Join(", ", Table.DetailNames));
        RenderKeyValue(Builder, "KeyField", Table.KeyField);
        if (Table.IsDetail)
        {
            RenderKeyValue(Builder, "MasterField", Table.MasterField);
            RenderKeyValue(Builder, "DetailField", Table.DetailField);
        }
        RenderKeyValue(Builder, "OneToOne", Table.IsOneToOne);
        RenderKeyValue(Builder, "Joins", Table.JoinCount);
        RenderKeyValue(Builder, "Stocks", Table.StockCount);
        RenderKeyValue(Builder, "Fields", $"{Table.VisibleFieldCount}/{Table.FieldCount} visible");
        RenderFieldsTable(Builder, Table.Fields);
    }
    /// <summary>
    /// Renders a fields table.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Fields">The field information list.</param>
    protected virtual void RenderFieldsTable(StringBuilder Builder, List<ItemStructureFieldInfo> Fields)
    {
        string[] Headers = ["Title", "Name", "Visible", "Hidden", "DataType", "Required", "ReadOnly", "Lookup", "Locator", "Group", "Size", "Decimals", "Default", "Nullable", "Width", "Expression", "CodeProvider", "SnapshotOf", "Flags"];
        Builder.Append("<div class=\"tp-WebDataForm-FactBoxFieldWrap\"><table class=\"tp-WebDataForm-FactBoxFieldTable\"><tr>");
        foreach (string Header in Headers)
        {
            Builder.Append("<th>");
            Builder.Append(Encode(Header));
            Builder.Append("</th>");
        }
        Builder.Append("</tr>");

        if (Fields != null)
        {
            foreach (ItemStructureFieldInfo Field in Fields)
                RenderFieldRow(Builder, Field);
        }

        Builder.Append("</table></div>");
    }
    /// <summary>
    /// Renders a field row.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Field">The field information.</param>
    protected virtual void RenderFieldRow(StringBuilder Builder, ItemStructureFieldInfo Field)
    {
        object[] Values = [
            Field.Title,
            Field.Name,
            Field.IsVisible ? "x" : string.Empty,
            Field.IsVisible ? string.Empty : "x",
            Field.DataType,
            Field.IsRequired ? "x" : string.Empty,
            Field.IsReadOnly ? "x" : string.Empty,
            Field.LookupSource,
            Field.Locator,
            Field.Group,
            Field.MaxLength > 0 ? Field.MaxLength.ToString(CultureInfo.CurrentCulture) : string.Empty,
            Field.Decimals >= 0 ? Field.Decimals.ToString(CultureInfo.CurrentCulture) : string.Empty,
            Field.DefaultValue,
            Field.IsNullable ? "x" : string.Empty,
            Field.DisplayWidth > 0 ? Field.DisplayWidth.ToString(CultureInfo.CurrentCulture) : string.Empty,
            Field.Expression,
            Field.CodeProvider,
            Field.SnapshotOf,
            Field.Flags
        ];
        int[] CenterIndexes = [2, 3, 5, 6, 13];
        Builder.Append("<tr>");
        for (int Index = 0; Index < Values.Length; Index++)
        {
            Builder.Append(Array.IndexOf(CenterIndexes, Index) >= 0 ? "<td class=\"tp-Center\">" : "<td>");
            Builder.Append(Encode(Values[Index]));
            Builder.Append("</td>");
        }
        Builder.Append("</tr>");
    }
    /// <summary>
    /// Renders a generic FactBox data object.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Data">The FactBox data.</param>
    protected virtual void RenderGenericData(StringBuilder Builder, object Data)
    {
        if (Data is IDictionary Dictionary)
        {
            RenderDictionary(Builder, Dictionary);
            return;
        }

        if (Data is IReadOnlyDictionary<string, object> ReadOnlyDictionary)
        {
            RenderItemInfo(Builder, ReadOnlyDictionary);
            return;
        }

        Builder.Append("<pre class=\"tp-WebDataForm-FactBoxJson\">");
        Builder.Append(Encode(Json.Serialize(Data)));
        Builder.Append("</pre>");
    }
    /// <summary>
    /// Creates the built-in standard information FactBox data.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The created data.</returns>
    protected virtual ItemStandardInfoFactBoxData CreateStandardInfoFactBoxData(ItemFactBoxContext Context)
    {
        return new()
        {
            ItemInfo = new ItemInfoFactBoxProvider().GetData(Context) as Dictionary<string, object> ?? new(),
            Structure = new ItemStructureFactBoxProvider().GetData(Context) as ItemStructureFactBoxData
        };
    }
    /// <summary>
    /// Renders a FactBox page.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Data">The FactBox data.</param>
    protected virtual void RenderFactBoxPage(StringBuilder Builder, object Data)
    {
        Builder.Append("<div class=\"tp-WebDataForm-FactBoxPage\">");
        if (Data is ItemStandardInfoFactBoxData StandardData)
        {
            RenderItemInfo(Builder, StandardData.ItemInfo);
            RenderStructureInfo(Builder, StandardData.Structure);
        }
        else if (Data is ItemStructureFactBoxData StructureData)
        {
            RenderStructureInfo(Builder, StructureData);
        }
        else
        {
            RenderGenericData(Builder, Data);
        }
        Builder.Append("</div>");
    }
    /// <summary>
    /// Creates the context for a custom FactBox.
    /// </summary>
    /// <param name="Context">The base FactBox context.</param>
    /// <param name="Def">The custom FactBox definition.</param>
    /// <returns>The custom FactBox context.</returns>
    protected virtual ItemFactBoxContext CreateCustomFactBoxContext(ItemFactBoxContext Context, ItemFactBoxDef Def)
    {
        return new()
        {
            FormName = Context.FormName,
            FormClassName = Context.FormClassName,
            FormJsClassName = Context.FormJsClassName,
            ItemPageClassName = Context.ItemPageClassName,
            ItemPageJsClassName = Context.ItemPageJsClassName,
            FactBoxDef = Def,
            Module = Context.Module,
            Row = Context.Row,
            RowState = Context.RowState,
            KeyValue = Context.KeyValue
        };
    }
    /// <summary>
    /// Renders a FactBox tab header.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Title">The tab title.</param>
    protected virtual void RenderFactBoxTabHeader(StringBuilder Builder, string Title)
    {
        Builder.Append("<div>");
        Builder.Append(Encode(Title));
        Builder.Append("</div>");
    }
    /// <summary>
    /// Renders a custom FactBox page.
    /// </summary>
    /// <param name="Builder">The target builder.</param>
    /// <param name="Context">The FactBox context.</param>
    /// <param name="Data">The FactBox data.</param>
    protected virtual void RenderCustomFactBoxPage(StringBuilder Builder, ItemFactBoxContext Context, object Data)
    {
        string ViewName = Context?.FactBoxDef?.WebViewName;
        if (fViewToStringConverter == null || string.IsNullOrWhiteSpace(ViewName))
        {
            RenderFactBoxPage(Builder, Data);
            return;
        }

        Builder.Append("<div class=\"tp-WebDataForm-FactBoxPage\">");
        Builder.Append(fViewToStringConverter.ViewToString(GetViewPath(ViewName), Data, new Dictionary<string, object>
        {
            ["FactBoxContext"] = Context,
            ["FactBoxDef"] = Context.FactBoxDef
        }));
        Builder.Append("</div>");
    }

    // ● public
    /// <summary>
    /// Renders FactBoxes.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <param name="CustomFactBoxes">The custom FactBox definitions.</param>
    /// <returns>The rendered HTML.</returns>
    public virtual string Render(ItemFactBoxContext Context, IEnumerable<ItemFactBoxDef> CustomFactBoxes)
    {
        StringBuilder Headers = new();
        StringBuilder Pages = new();
        int Count = 0;

        Headers.Append("<div>");
        Pages.Append("<div>");

        RenderFactBoxTabHeader(Headers, "Info");
        RenderFactBoxPage(Pages, CreateStandardInfoFactBoxData(Context));
        Count++;

        if (CustomFactBoxes != null)
        {
            foreach (ItemFactBoxDef Def in CustomFactBoxes)
            {
                if (Def == null || !Def.IsVisible)
                    continue;

                ItemFactBoxContext CustomContext = CreateCustomFactBoxContext(Context, Def);
                ItemFactBoxProvider Provider = Def.CreateProvider();
                object Data = Provider != null ? Provider.GetData(CustomContext) : null;
                RenderFactBoxTabHeader(Headers, Def.Title);
                RenderCustomFactBoxPage(Pages, CustomContext, Data);
                Count++;
            }
        }

        Headers.Append("</div>");
        Pages.Append("</div>");

        return Count > 0 ? Headers.ToString() + Pages : string.Empty;
    }
}

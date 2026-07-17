/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Base class for objects providing HTML and metadata for a WebDesk form.
/// </summary>
public abstract class WebFormProvider
{
    // ● private fields
    string fName;

    // ● private
    /// <summary>
    /// Creates a reduced filter packet.
    /// </summary>
    static WebFormSelectFilterPacket CreateFilterPacket(SqlFilterDef FilterDef)
    {
        return new WebFormSelectFilterPacket
        {
            Name = FilterDef.Name,
            TitleKey = FilterDef.TitleKey,
            Title = FilterDef.Title,
            FieldName = FilterDef.FieldName,
            FilterDataType = FilterDef.FilterDataType.ToString(),
            BoolOp = FilterDef.BoolOp.ToString(),
            ConditionOp = FilterDef.ConditionOp.ToString()
        };
    }
    /// <summary>
    /// Creates a reduced select packet.
    /// </summary>
    static WebFormSelectPacket CreateSelectPacket(SelectDef SelectDef)
    {
        WebFormSelectPacket Result = new()
        {
            Name = SelectDef.Name,
            TitleKey = SelectDef.TitleKey,
            Title = SelectDef.Title,
            UseFilters = SelectDef.UseFilters
        };

        foreach (SqlFilterDef FilterDef in SelectDef.FilterDefs)
            Result.Filters.Add(CreateFilterPacket(FilterDef));

        return Result;
    }
    /// <summary>
    /// Adds reduced select descriptors to a provider packet.
    /// </summary>
    static void AddSelectList(WebFormProviderPacket Packet, WebFormDef Form)
    {
        if (Packet == null || Form == null || string.IsNullOrWhiteSpace(Form.Module))
            return;

        ModuleDef ModuleDef = DataRegistry.Modules.Find(Form.Module);
        if (ModuleDef == null)
            return;

        foreach (SelectDef SelectDef in ModuleDef.SelectList)
            Packet.SelectList.Add(CreateSelectPacket(SelectDef));
    }

    // ● protected
    /// <summary>
    /// Returns the web form name declared by the provider attribute.
    /// </summary>
    protected virtual string GetName()
    {
        WebFormProviderAttribute Attribute = GetType().GetCustomAttribute<WebFormProviderAttribute>();
        return Attribute != null ? Attribute.WebFormName : string.Empty;
    }
    /// <summary>
    /// Returns the HTML markup for the form.
    /// </summary>
    protected abstract string GetHtml(WebFormProviderContext Context);
    /// <summary>
    /// Creates a provider packet for the specified context.
    /// </summary>
    protected virtual WebFormProviderPacket CreatePacket(WebFormProviderContext Context)
    {
        WebFormDef Form = Context.Form;
        WebFormProviderPacket Result = new();

        Result.Name = Form.Name;
        Result.TitleKey = Form.TitleKey;
        Result.Title = Form.Title;
        Result.Module = Form.Module;
        Result.ViewName = Form.ViewName;
        Result.ItemViewName = Form.ItemViewName;
        Result.Group = Form.Group;
        Result.IsReadOnly = Form.IsReadOnly;
        Result.IsCustom = Form.IsCustom;
        Result.JsFormClassType = Form.JsFormClassType;
        Result.JsDataModuleClassType = Form.JsDataModuleClassType;
        Result.CssFiles.AddRange(Form.CssFiles);
        Result.JavaScriptFiles.AddRange(Form.JavaScriptFiles);
        AddSelectList(Result, Form);
        Result.Html = GetHtml(Context);

        return Result;
    }

    // ● public
    /// <summary>
    /// Executes this provider and returns the form packet.
    /// </summary>
    public virtual WebFormProviderPacket Execute(WebFormProviderContext Context)
    {
        if (Context == null)
            throw new TripousArgumentNullException(nameof(Context));
        return CreatePacket(Context);
    }

    // ● properties
    /// <summary>
    /// Gets the web form name handled by this provider.
    /// </summary>
    public string Name => fName ??= GetName();
}

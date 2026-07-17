/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Represents a reduced SELECT descriptor returned with a WebDesk form packet.
/// </summary>
public class WebFormSelectPacket
{
    // ● properties
    /// <summary>
    /// Gets or sets the select name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the select title key.
    /// </summary>
    public string TitleKey { get; set; }
    /// <summary>
    /// Gets or sets the select title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether filters should be used.
    /// </summary>
    public bool UseFilters { get; set; }
    /// <summary>
    /// Gets the filter descriptors.
    /// </summary>
    public List<WebFormSelectFilterPacket> Filters { get; } = new();
}

/// <summary>
/// Represents a reduced SELECT filter descriptor returned with a WebDesk form packet.
/// </summary>
public class WebFormSelectFilterPacket
{
    // ● properties
    /// <summary>
    /// Gets or sets the filter name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the filter title key.
    /// </summary>
    public string TitleKey { get; set; }
    /// <summary>
    /// Gets or sets the filter title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets the field name used by the filter.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// Gets or sets the filter data type name.
    /// </summary>
    public string FilterDataType { get; set; }
    /// <summary>
    /// Gets or sets the default boolean operator name.
    /// </summary>
    public string BoolOp { get; set; }
    /// <summary>
    /// Gets or sets the default condition operator name.
    /// </summary>
    public string ConditionOp { get; set; }
}

/// <summary>
/// Represents the packet returned by a WebDesk form provider.
/// </summary>
public class WebFormProviderPacket
{
    // ● properties
    /// <summary>
    /// Gets or sets the registered web form name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the title key.
    /// </summary>
    public string TitleKey { get; set; }
    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets the registered module name.
    /// </summary>
    public string Module { get; set; }
    /// <summary>
    /// Gets or sets the Razor view name.
    /// </summary>
    public string ViewName { get; set; }
    /// <summary>
    /// Gets or sets the optional item view name.
    /// </summary>
    public string ItemViewName { get; set; }
    /// <summary>
    /// Gets or sets the form group.
    /// </summary>
    public string Group { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the form is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the form is custom.
    /// </summary>
    public bool IsCustom { get; set; }
    /// <summary>
    /// Gets or sets the JavaScript form class type.
    /// </summary>
    public string JsFormClassType { get; set; }
    /// <summary>
    /// Gets or sets the JavaScript data module class type.
    /// </summary>
    public string JsDataModuleClassType { get; set; }
    /// <summary>
    /// Gets or sets the form HTML.
    /// </summary>
    public string Html { get; set; }
    /// <summary>
    /// Gets the reduced select descriptors used by the form.
    /// </summary>
    public List<WebFormSelectPacket> SelectList { get; } = new();
    /// <summary>
    /// Gets the CSS files required by the form.
    /// </summary>
    public List<string> CssFiles { get; } = new();
    /// <summary>
    /// Gets the JavaScript files required by the form.
    /// </summary>
    public List<string> JavaScriptFiles { get; } = new();
}

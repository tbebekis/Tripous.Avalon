/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for an active select filter sent by a Tripous Web client.
/// </summary>
public class JsonSelectFilter
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonSelectFilter()
    {
    }

    // ● properties
    /// <summary>
    /// The select name.
    /// </summary>
    public string SelectName { get; set; }
    /// <summary>
    /// The registered filter name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The boolean operator name.
    /// </summary>
    public string BoolOp { get; set; }
    /// <summary>
    /// The condition operator name.
    /// </summary>
    public string ConditionOp { get; set; }
    /// <summary>
    /// The first filter value.
    /// </summary>
    public object Value { get; set; }
    /// <summary>
    /// The second filter value, used by Between.
    /// </summary>
    public object Value2 { get; set; }
}

/// <summary>
/// JSON contract for a list of active select filters sent by a Tripous Web client.
/// </summary>
public class JsonSelectFilters: List<JsonSelectFilter>
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonSelectFilters()
    {
    }

    // ● static public
    /// <summary>
    /// Creates a filter list from an arbitrary JSON value.
    /// </summary>
    static public JsonSelectFilters From(object Value)
    {
        if (Value == null)
            return new JsonSelectFilters();
        if (Value is JsonSelectFilters Filters)
            return Filters;
        if (Value is JsonElement Element)
            return Json.Deserialize<JsonSelectFilters>(Element.GetRawText()) ?? new JsonSelectFilters();
        return Json.Deserialize<JsonSelectFilters>(Json.Serialize(Value)) ?? new JsonSelectFilters();
    }
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a locator definition.
/// </summary>
public class JsonLocatorDef
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorDef()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorDef(LocatorDef Source)
    {
        if (Source != null)
        {
            Name = Source.Name;
            KeyField = Source.KeyField;
            Form = Source.Form;
            WebForm = Source.WebForm;
            MinimumSearchLength = Source.MinimumSearchLength;
            MaximumResultCount = Source.MaximumResultCount;
            SingleRowSearchFields.AddRange(Source.GetSearchFields(IsMultiRow: false));
            MultiRowSearchFields.AddRange(Source.GetSearchFields(IsMultiRow: true));
            ResultFields.AddRange(Source.GetResultFields());
            ListVisibleFields.AddRange(Source.GetListVisibleFields());

            foreach (LocatorFieldDef FieldDef in Source.Fields)
                Fields.Add(new JsonLocatorField(FieldDef));
        }
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorDef(LocatorDef Source, LocatorMapPlan MapPlan)
        : this(Source)
    {
        this.MapPlan = new JsonLocatorMapPlan(MapPlan);
    }

    // ● properties
    /// <summary>
    /// The locator name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The key field name.
    /// </summary>
    public string KeyField { get; set; } = "Id";
    /// <summary>
    /// The desktop form name.
    /// </summary>
    public string Form { get; set; } = string.Empty;
    /// <summary>
    /// The web form name.
    /// </summary>
    public string WebForm { get; set; } = string.Empty;
    /// <summary>
    /// The minimum search text length.
    /// </summary>
    public int MinimumSearchLength { get; set; }
    /// <summary>
    /// The maximum result row count.
    /// </summary>
    public int MaximumResultCount { get; set; }
    /// <summary>
    /// Optional table-specific mapping plan.
    /// </summary>
    public JsonLocatorMapPlan MapPlan { get; set; }
    /// <summary>
    /// The locator fields.
    /// </summary>
    public List<JsonLocatorField> Fields { get; set; } = [];
    /// <summary>
    /// The fields used by a single-row locator UI.
    /// </summary>
    public List<string> SingleRowSearchFields { get; set; } = [];
    /// <summary>
    /// The fields used by a multi-row locator UI.
    /// </summary>
    public List<string> MultiRowSearchFields { get; set; } = [];
    /// <summary>
    /// The fields returned by locator execution.
    /// </summary>
    public List<string> ResultFields { get; set; } = [];
    /// <summary>
    /// The fields displayed by locator list UIs.
    /// </summary>
    public List<string> ListVisibleFields { get; set; } = [];
}

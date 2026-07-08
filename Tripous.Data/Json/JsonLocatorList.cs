/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a table-specific locator definition list.
/// </summary>
public class JsonLocatorList
{
    // ● private
    void Add(TableDef TableDef, FieldDef FieldDef)
    {
        if (TableDef == null || FieldDef == null || string.IsNullOrWhiteSpace(FieldDef.Locator))
            return;

        LocatorDef LocatorDef = DataRegistry.FindLocator(FieldDef.Locator);
        if (LocatorDef == null)
            return;

        LocatorMapPlan MapPlan = new LocatorMapper().CreatePlan(LocatorDef, TableDef, FieldDef);
        if (Items.Any(item => item.Name.IsSameText(LocatorDef.Name) && GetReferenceField(item).IsSameText(MapPlan.ReferenceField)))
            return;

        Items.Add(new JsonLocatorDef(LocatorDef, MapPlan));
    }
    static string GetReferenceField(JsonLocatorDef LocatorDef)
    {
        return LocatorDef != null && LocatorDef.MapPlan != null ? LocatorDef.MapPlan.ReferenceField : string.Empty;
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorList()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorList(TableDef Source)
    {
        if (Source != null)
        {
            foreach (FieldDef FieldDef in Source.Fields)
                Add(Source, FieldDef);
        }
    }

    // ● properties
    /// <summary>
    /// The locator definitions.
    /// </summary>
    public List<JsonLocatorDef> Items { get; set; } = [];
}

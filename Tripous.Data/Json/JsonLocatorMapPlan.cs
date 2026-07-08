/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a locator mapping plan.
/// </summary>
public class JsonLocatorMapPlan
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorMapPlan()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorMapPlan(LocatorMapPlan Source)
    {
        if (Source != null)
        {
            LocatorName = Source.LocatorName;
            ReferenceField = Source.ReferenceField;

            foreach (LocatorMapItem Item in Source.Items)
                Items.Add(new JsonLocatorMapItem(Item));
        }
    }

    // ● properties
    /// <summary>
    /// The locator name.
    /// </summary>
    public string LocatorName { get; set; } = string.Empty;
    /// <summary>
    /// The reference field name.
    /// </summary>
    public string ReferenceField { get; set; } = string.Empty;
    /// <summary>
    /// The mapping items.
    /// </summary>
    public List<JsonLocatorMapItem> Items { get; set; } = [];
}

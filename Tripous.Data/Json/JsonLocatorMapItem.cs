/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a locator mapping item.
/// </summary>
public class JsonLocatorMapItem
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorMapItem()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorMapItem(LocatorMapItem Source)
    {
        if (Source != null)
        {
            SourceField = Source.SourceField;
            TargetField = Source.TargetField;
        }
    }

    // ● properties
    /// <summary>
    /// The locator result field name.
    /// </summary>
    public string SourceField { get; set; } = string.Empty;
    /// <summary>
    /// The target row field name.
    /// </summary>
    public string TargetField { get; set; } = string.Empty;
}

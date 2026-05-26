/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class Registry
{
    static public void RegisterLocators()
    {
        // TODO: Register extra Locators
    }
    /// <summary>
    /// Locators added by registration builder are incomplete.
    /// <para>We have to add the proper fields here</para>
    /// </summary>
    static public void RegisterLocatorFields()
    {
       LocatorDef LocatorDef = DataRegistry.AddOrGetLocator("Country", "Country", "Id", FormName: "Country");
       LocatorDef.Add("Id");
       LocatorDef.Add("Code");
       LocatorDef.Add("Name");
    }
}
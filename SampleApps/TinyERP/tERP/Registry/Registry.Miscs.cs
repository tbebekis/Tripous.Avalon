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
    static public void RegisterLookups()
    {
        // TODO: Register extra definitions
    }
    
    static public void RegisterLocators()
    {
        // TODO: Register extra definitions
    }
    
    static public void RegisterLookupSources()
    {
        // TODO: Register extra definitions
    }
    
    /// <summary>
    /// Definitions added by registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateLookups()
    {
        LookupDef LookupDef = DataRegistry.Lookups.Find("AppUser");
        if (LookupDef != null)
            LookupDef.DisplayField = "FullName";
    }
    /// <summary>
    /// Definitions added by registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateLocators()
    {
        LocatorDef LocatorDef = DataRegistry.AddOrGetLocator("Country", "Country", "Id", FormName: "Country");
        LocatorDef.Add("Id");
        LocatorDef.Add("Code");
        LocatorDef.Add("Name");
    }
    /// <summary>
    /// Definitions added by registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateForms()
    {
    }
    /// <summary>
    /// Definitions added by registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateModules()
    {
    }


}
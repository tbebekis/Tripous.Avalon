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
        /*
        DataRegistry.Modules.Get("SalesOrder").IsDocument = true;
        DataRegistry.Modules.Get("SalesDeliveryNote").IsDocument = true;
        DataRegistry.Modules.Get("SalesInvoice").IsDocument = true;
        DataRegistry.Modules.Get("SalesCreditNote").IsDocument = true;
        DataRegistry.Modules.Get("SalesReturn").IsDocument = true;
        DataRegistry.Modules.Get("SalesCancellation").IsDocument = true;
        
        DataRegistry.Modules.Get("PurchaseOrder").IsDocument = true;
        DataRegistry.Modules.Get("PurchaseDeliveryNote").IsDocument = true;
        DataRegistry.Modules.Get("PurchaseInvoice").IsDocument = true;
        DataRegistry.Modules.Get("PurchaseCreditNote").IsDocument = true;
        DataRegistry.Modules.Get("PurchaseReturn").IsDocument = true;
        DataRegistry.Modules.Get("PurchaseCancellation").IsDocument = true;
        
        DataRegistry.Modules.Get("StockTrade").IsDocument = true;
        DataRegistry.Modules.Get("StockCount").IsDocument = true;
        
        DataRegistry.Modules.Get("JournalEntry").IsDocument = true;
        
        DataRegistry.Modules.Get("StockMovement").IsDocumentSnapshot = true;
        DataRegistry.Modules.Get("FinanceMovement").IsDocumentSnapshot = true;
        */
    }

 
 





}
/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

static public partial class Registry
{
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateLookups()
    {
        LookupDef LookupDef = DataRegistry.Lookups.Find("AppUser");
        if (LookupDef != null)
            LookupDef.DisplayField = "FullName";
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateLocators()
    {
        void AddFields(LocatorDef LocatorDef, string[] FieldNames)
        {
            foreach (string FieldName in FieldNames)
                LocatorDef.Add(FieldName);
        }
        
        // Country
        LocatorDef LocatorDef = DataRegistry.AddOrUpdateLocator("Country", "Country", "Id", FormName: "Country");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);

        string SqlText = @"
select 
     P.Id
    ,P.Code
    ,P.Name
from Person P
where
        P.IsActive = 1
";
        // Person
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Person", SqlText,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        SqlText = @"
select 
     P.Id
    ,P.Code
    ,P.Name
from Person P
inner join PersonRole PR on PR.PersonId = P.Id
inner join PersonRoleType PRT on PRT.Id = PR.RoleTypeId
where
        P.IsActive = 1
";        
        
        // Customer
        string WhereSql = @"  and PRT.Code = 'CUS' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Customer", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // Supplier
        WhereSql = @"  and PRT.Code = 'SUP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Supplier", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // Employee
        WhereSql = @"  and PRT.Code = 'EMP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Employee", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // Manager
        WhereSql = @"  and PRT.Code = 'MGR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Manager", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // Carrier
        WhereSql = @"  and PRT.Code = 'CAR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Carrier", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        
        // PersonId
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateForms()
    {
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
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
        
        string[] SalesDocumentModules = ["SalesOrder", "SalesDeliveryNote", "SalesInvoice", "SalesCreditNote", "SalesReturn", "SalesCancellation"];
        string[] PurchaseDocumentModules = ["PurchaseOrder", "PurchaseDeliveryNote", "PurchaseInvoice", "PurchaseCreditNote", "PurchaseReturn", "PurchaseCancellation"];
        string[] StockDocumentModules = ["StockTrade", "StockCount"];
        string[] JournalDocumentModules = ["JournalEntry"];
        
        string[] MovementModules = ["StockMovement", "FinanceMovement"];
        
        string[] DocumentModules = SalesDocumentModules.Concat(PurchaseDocumentModules).Concat(StockDocumentModules).Concat(JournalDocumentModules).ToArray();
        string[] DocumentSnapshotModules = MovementModules.ToArray();
        string[] AllModules = SalesDocumentModules.Concat(PurchaseDocumentModules).Concat(StockDocumentModules).Concat(JournalDocumentModules).Concat(MovementModules).ToArray();

        void SetTradeModulePersonLocator(string Locator, string[] ModuleNames)
        {
            foreach (string ModuleName in ModuleNames)
            {
                ModuleDef ModuleDef = DataRegistry.Modules.Find(ModuleName);
                if (ModuleDef != null && ModuleDef.Table.Fields.Contains("PersonId"))
                    ModuleDef.Table.Fields["PersonId"].Locator = Locator;
            }
        }

        SetTradeModulePersonLocator("Customer", SalesDocumentModules);
        SetTradeModulePersonLocator("Supplier", PurchaseDocumentModules);
    }
}
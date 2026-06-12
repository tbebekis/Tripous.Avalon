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
        LookupDef LookupDef = DataRegistry.Lookups.Find(DbConfig.SysAppUserTableName);
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
            LocatorDef.Fields.Clear();

            foreach (string FieldName in FieldNames)
            {
                LocatorFieldDef FieldDef = LocatorDef.Add(FieldName);
                if (FieldName.IsSameText("Id"))
                {
                    FieldDef.IsVisible = false;
                    FieldDef.IsSearchable = false;
                }
            }
                
        }
        
        // ● Country
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
        // ● Person
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
        
        // ● Customer
        string WhereSql = @"  and PRT.Code = 'CUS' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Customer", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // ● Supplier
        WhereSql = @"  and PRT.Code = 'SUP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Supplier", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // ● Employee
        WhereSql = @"  and PRT.Code = 'EMP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Employee", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // ● Manager
        WhereSql = @"  and PRT.Code = 'MGR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Manager", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        // ● Carrier
        WhereSql = @"  and PRT.Code = 'CAR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Carrier", SqlText + WhereSql,  "Id", FormName: "Person");
        AddFields(LocatorDef, ["Id", "Code", "Name"]);
        
        //*
        // ● Product
        SqlText = @"
select
     P.Id as Id
    ,P.Code as Code
    ,P.Name as Name
    ,coalesce(PUM.UnitId, P.PrimaryUnitOfMeasureId) as UnitOfMeasureId
    ,UOM.Name as UnitOfMeasureName
    ,coalesce(PUM.Ratio, 1) as UnitRatio
    ,P.TaxProductGroupId
from Product P
left join ProductUnitOfMeasure PUM
    on PUM.ProductId = P.Id
    and PUM.IsActive = 1
    and PUM.IsSalesDefault = 1
    and not exists
    (
        select 1
        from ProductUnitOfMeasure PUM2
        where PUM2.ProductId = PUM.ProductId
          and PUM2.IsActive = 1
          and PUM2.IsSalesDefault = 1
          and PUM2.Id < PUM.Id
    )
left join UnitOfMeasure UOM on UOM.Id = coalesce(PUM.UnitId, P.PrimaryUnitOfMeasureId)
where P.IsActive = 1
";       
        
        LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Product", SqlText,  "Id", FormName: "Product");
        AddFields(LocatorDef, [
            "Id", 
            "Code", 
            "Name", 
            "UnitOfMeasureId", 
            "UnitOfMeasureName",
            "UnitRatio",
            "TaxProductGroupId"
        ]);
        
        LocatorDef.Fields.Find("UnitOfMeasureId").IsVisible = false;
        
        //*/
        
 
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateForms()
    {
        DesktopRegistry.Forms.Get("SalesDeliveryNote").ClassName = "SalesDeliveryNoteForm";
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

        //---------------------------------------------------------------
        void SetTradeModulePersonLocator(string Locator, string[] ModuleNames)
        {
            foreach (string ModuleName in ModuleNames)
            {
                ModuleDef ModuleDef = DataRegistry.Modules.Find(ModuleName);
                if (ModuleDef != null && ModuleDef.Table.Fields.Contains("PersonId"))
                {
                    ModuleDef.Table.Fields["PersonId"].Locator = Locator;
                    ModuleDef.Table.Fields["PersonId"].TitleKey = Locator;
                }
            }
        }
        //---------------------------------------------------------------

        SetTradeModulePersonLocator("Customer", SalesDocumentModules);
        SetTradeModulePersonLocator("Supplier", PurchaseDocumentModules);
    }

    static public void RegisterSycConfigProperties()
    {
        // ●  Application Defaults
        string Name = DataLib.SAppDefaultProperties;
        string TitleKey = "Application Defaults";
        string GroupName = "Application";
        UserLevel SecurityLevel = UserLevel.Admin;
        ConfigValueKind Kind = ConfigValueKind.Object;
        string DefaultValue = Json.Serialize(new AppDefaultProperties());
        string TypeName = typeof(AppDefaultProperties).FullName;
        
        ConfigPropertyDef ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, TypeName);
        
        // ●  Show DataForm Log
        Name = Ui.SShowDataFormLog;
        TitleKey = "Show DataForm Log";
        SecurityLevel = UserLevel.User;
        Kind = ConfigValueKind.Boolean;
        DefaultValue = "false";
        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue);
        ConfigPropertyDef.ApplyValueFunc = (Def, S) =>
        {
            bool Value = Convert.ToBoolean(S);
            Ui.Settings.ShowDataFormLog = Value;
        };
    }
}

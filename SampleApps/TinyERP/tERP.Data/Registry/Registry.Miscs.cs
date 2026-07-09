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
    /// <para>This method provides a chance to complete locator definitions.</para>
    /// </summary>
    static public void UpdateLocators()
    {
        void AddFields(LocatorDef Def, params (string Name, DataFieldType DataType)[] Fields)
        {
            Def.Fields.Clear();

            foreach ((string Name, DataFieldType DataType) Field in Fields)
                Def.Add(Field.Name, Field.DataType);
        }
        void AddStringFields(LocatorDef Def, params string[] FieldNames)
        {
            Def.Fields.Clear();

            foreach (string FieldName in FieldNames)
                Def.Add(FieldName);
        }
        void SetLists(LocatorDef Def, string[] ResultFields, string[] SearchFields, string[] ListVisibleFields = null)
        {
            Def.ResultFields.Clear();
            Def.ListVisibleFields.Clear();
            Def.SingleRowSearchFields.Clear();
            Def.MultiRowSearchFields.Clear();

            Def.AddResultFields(ResultFields);
            if (ListVisibleFields != null)
                Def.AddListVisibleFields(ListVisibleFields);
            Def.AddSearchFields(SearchFields);
        }
        TableDef FindSourceTable(LocatorDef Def)
        {
            return DataRegistry.Modules
                .Select(item => item.Table)
                .FirstOrDefault(item => item != null && item.Name.IsSameText(Def.Source));
        }
        void CompleteGeneratedLocator(LocatorDef Def)
        {
            if (Def == null || Def.Fields.Count > 0)
                return;

            TableDef Table = FindSourceTable(Def);
            if (Table == null)
            {
                Def.Add(Def.KeyField);
                Def.AddResultFields(Def.KeyField);
                return;
            }

            Def.Add(Def.KeyField, Table.Fields.Find(Def.KeyField)?.DataType ?? DataFieldType.String);

            foreach (string FieldName in new[] { "Code", "Name" })
            {
                FieldDef FieldDef = Table.Fields.Find(FieldName);
                if (FieldDef != null)
                    Def.Add(FieldDef.Name, FieldDef.DataType);
            }

            Def.AddResultFields(Def.Fields.Select(item => item.Name).ToArray());
            Def.AddSearchFields(Def.Fields.Where(item => !item.Name.IsSameText(Def.KeyField) && item.DataType == DataFieldType.String).Select(item => item.Name).ToArray());
        }

        foreach (LocatorDef Def in DataRegistry.Locators)
            CompleteGeneratedLocator(Def);

        // ● Country
        LocatorDef LocatorDef = DataRegistry.AddOrUpdateLocator("Country", Source: "Country", KeyField: "Id", FormName: "Country", WebFormName: "Country");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

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
        LocatorDef = DataRegistry.AddOrUpdateLocator("Person", Source: SqlText, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

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
        LocatorDef = DataRegistry.AddOrUpdateLocator("Customer", Source: SqlText + WhereSql, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

        // ● Supplier
        WhereSql = @"  and PRT.Code = 'SUP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocator("Supplier", Source: SqlText + WhereSql, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

        // ● Employee
        WhereSql = @"  and PRT.Code = 'EMP' ";
        LocatorDef = DataRegistry.AddOrUpdateLocator("Employee", Source: SqlText + WhereSql, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

        // ● Manager
        WhereSql = @"  and PRT.Code = 'MGR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocator("Manager", Source: SqlText + WhereSql, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

        // ● Carrier
        WhereSql = @"  and PRT.Code = 'CAR' ";
        LocatorDef = DataRegistry.AddOrUpdateLocator("Carrier", Source: SqlText + WhereSql, KeyField: "Id", FormName: "Person", WebFormName: "Person");
        AddStringFields(LocatorDef, "Id", "Code", "Name");
        SetLists(LocatorDef, ["Id", "Code", "Name"], ["Code", "Name"]);

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

        LocatorDef = DataRegistry.AddOrUpdateLocator("Product", Source: SqlText, KeyField: "Id", FormName: "Product", WebFormName: "Product");
        AddFields(LocatorDef,
            ("Id", DataFieldType.String),
            ("Code", DataFieldType.String),
            ("Name", DataFieldType.String),
            ("UnitOfMeasureId", DataFieldType.String),
            ("UnitOfMeasureName", DataFieldType.String),
            ("UnitRatio", DataFieldType.Decimal),
            ("TaxProductGroupId", DataFieldType.String));
        SetLists(LocatorDef, ["Id", "Code", "Name", "UnitOfMeasureId", "UnitOfMeasureName", "UnitRatio", "TaxProductGroupId"], ["Code", "Name"], ["Code", "Name", "UnitOfMeasureName", "UnitRatio"]);

        // ● Payment Settlement Finance Movement
        LocatorDef = DataRegistry.AddOrUpdateLocator(
            "PaymentSettlementFinanceMovement",
            Source: "FinanceMovement",
            ClassName: typeof(PaymentSettlementFinanceMovementLocator).FullName,
            KeyField: "Id",
            FormName: "FinanceMovement",
            WebFormName: "FinanceMovement");
        AddFields(LocatorDef,
            ("Id", DataFieldType.String),
            ("DocumentCode", DataFieldType.String),
            ("DocumentDate", DataFieldType.Date),
            ("PersonCode", DataFieldType.String),
            ("PersonName", DataFieldType.String),
            ("TradeType", DataFieldType.String),
            ("Direction", DataFieldType.Integer),
            ("Amount", DataFieldType.Decimal),
            ("OpenAmount", DataFieldType.Decimal));
        SetLists(LocatorDef, ["Id", "DocumentCode", "DocumentDate", "PersonCode", "PersonName", "TradeType", "Direction", "Amount", "OpenAmount"], ["DocumentCode", "PersonCode", "PersonName", "TradeType"], ["DocumentCode", "DocumentDate", "PersonCode", "PersonName", "Amount", "OpenAmount"]);
    }
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
        string[] CustomerPaymentModules = ["CustomerReceipt", "CustomerReceiptCancellation"];
        string[] SupplierPaymentModules = ["SupplierPayment", "SupplierPaymentCancellation"];
        
        string[] PaymentDocumentModules = CustomerPaymentModules.Concat(SupplierPaymentModules).ToArray();
        string[] DocumentModules = SalesDocumentModules.Concat(PurchaseDocumentModules).Concat(StockDocumentModules).Concat(JournalDocumentModules).Concat(PaymentDocumentModules).ToArray();
        string[] DocumentSnapshotModules = MovementModules.ToArray();
        string[] AllModules = SalesDocumentModules.Concat(PurchaseDocumentModules).Concat(StockDocumentModules).Concat(JournalDocumentModules).Concat(PaymentDocumentModules).Concat(MovementModules).ToArray();

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
        
        SetTradeModulePersonLocator("Customer", CustomerPaymentModules);
        SetTradeModulePersonLocator("Supplier", SupplierPaymentModules);
    }

    static public void RegisterSycConfigProperties()
    {
        ModuleDef AppUserModule = DataRegistry.Modules.Find("AppUser");
        if (AppUserModule != null)
            AppUserModule.SecurityLevel = UserLevel.Admin;

        // ●  Application Defaults
        string Name = DataLib.SAppDefaultProperties;
        string TitleKey = "Application Defaults";
        string GroupName = "Application";
        UserLevel SecurityLevel = UserLevel.Admin;
        ConfigValueKind Kind = ConfigValueKind.Object;
        string DefaultValue = Json.Serialize(new AppDefaultProperties());
        string TypeName = typeof(AppDefaultProperties).FullName;
        string EditorClassName = "tERP.AppDefaultPropertiesEditor";
        
        ConfigPropertyDef ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, TypeName, EditorClassName);
        // ●  Use Users
        Name = DataLib.SUseUsers;
        TitleKey = "Use Users";
        SecurityLevel = UserLevel.Admin;
        Kind = ConfigValueKind.Boolean;
        DefaultValue = "false";
        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, Scopes: ConfigScopeFlags.System);

        // ● Select List Row Limit
        Name = Config.SSelectListRowLimit;
        TitleKey = "Select List Row Limit";
        SecurityLevel = UserLevel.User;
        Kind = ConfigValueKind.Integer;
        DefaultValue = Db.Settings.DefaultRowLimit.ToString(CultureInfo.InvariantCulture);
        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, Scopes: ConfigScopeFlags.System | ConfigScopeFlags.User);

        // ● Show DataForm FactBox Pane
        Name = Config.SShowDataFormFactBoxPane;
        TitleKey = "Show DataForm FactBox Pane";
        SecurityLevel = UserLevel.User;
        Kind = ConfigValueKind.Boolean;
        DefaultValue = "true";
        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, Scopes: ConfigScopeFlags.System | ConfigScopeFlags.User);
    }
}

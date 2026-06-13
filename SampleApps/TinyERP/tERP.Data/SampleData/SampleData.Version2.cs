/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public partial class SampleData2: SampleData
{
    // ● private
    static void Add_DocumentType()
    {
        string ModuleName = "DocumentType";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        tblSource.CopyColumnsFrom(Module.tblItem);
        Dictionary<string, string> Ids = [];
        Ids["SalesOrder"] = AddDocumentType(tblSource, "SAL-ORD", "Sales Order", 1, "SalesOrder", "SalesOrder", false, false, false, 0, 0, 0, false, null, 10, "#2563EB", "ShoppingCart");
        Ids["SalesDeliveryNote"] = AddDocumentType(tblSource, "SAL-DN", "Sales Delivery Note", 1, "SalesDeliveryNote", "SalesDeliveryNote", true, false, false, -1, 0, 0, false, null, 20, "#0891B2", "Truck");
        Ids["SalesInvoice"] = AddDocumentType(tblSource, "SAL-INV", "Sales Invoice", 1, "SalesInvoice", "SalesInvoice", false, true, true, 0, 1, 1, false, null, 30, "#16A34A", "FileText");
        Ids["SalesCreditNote"] = AddDocumentType(tblSource, "SAL-CN", "Sales Credit Note", 1, "SalesCreditNote", "SalesCreditNote", false, true, true, 0, -1, -1, false, null, 40, "#F59E0B", "Undo2");
        Ids["SalesReturn"] = AddDocumentType(tblSource, "SAL-RET", "Sales Return", 1, "SalesReturn", "SalesReturn", true, false, false, 1, 0, 0, false, null, 50, "#CA8A04", "RotateCcw");
        Ids["PurchaseOrder"] = AddDocumentType(tblSource, "PUR-ORD", "Purchase Order", 2, "PurchaseOrder", "PurchaseOrder", false, false, false, 0, 0, 0, false, null, 110, "#7C3AED", "ShoppingBag");
        Ids["PurchaseDeliveryNote"] = AddDocumentType(tblSource, "PUR-DN", "Purchase Delivery Note", 2, "PurchaseDeliveryNote", "PurchaseDeliveryNote", true, false, false, 1, 0, 0, false, null, 120, "#0D9488", "PackageCheck");
        Ids["PurchaseInvoice"] = AddDocumentType(tblSource, "PUR-INV", "Purchase Invoice", 2, "PurchaseInvoice", "PurchaseInvoice", false, true, true, 0, -1, -1, false, null, 130, "#059669", "ReceiptText");
        Ids["PurchaseCreditNote"] = AddDocumentType(tblSource, "PUR-CN", "Purchase Credit Note", 2, "PurchaseCreditNote", "PurchaseCreditNote", false, true, true, 0, 1, 1, false, null, 140, "#D97706", "Undo2");
        Ids["PurchaseReturn"] = AddDocumentType(tblSource, "PUR-RET", "Purchase Return", 2, "PurchaseReturn", "PurchaseReturn", true, false, false, -1, 0, 0, false, null, 150, "#B45309", "RotateCcw");
        Ids["StockTrade"] = AddDocumentType(tblSource, "STK-TRD", "Stock Trade", 3, "StockTrade", "StockTrade", true, false, false, 0, 0, 0, false, null, 210, "#4F46E5", "Boxes");
        Ids["StockCount"] = AddDocumentType(tblSource, "STK-CNT", "Stock Count", 3, "StockCount", "StockCount", true, false, false, 0, 0, 0, false, null, 220, "#0284C7", "ClipboardCheck");
        Ids["JournalEntry"] = AddDocumentType(tblSource, "JRN", "Journal Entry", 5, "JournalEntry", "JournalEntry", false, false, true, 0, 0, 1, false, null, 310, "#475569", "BookOpen");
        AddDocumentType(tblSource, "SAL-CANCEL", "Sales Cancellation", 1, "SalesCancellation", "SalesCancellation", true, true, true, 0, -1, -1, true, Ids["SalesInvoice"], 60, "#DC2626", "Ban");
        AddDocumentType(tblSource, "PUR-CANCEL", "Purchase Cancellation", 2, "PurchaseCancellation", "PurchaseCancellation", true, true, true, 0, 1, 1, true, Ids["PurchaseInvoice"], 160, "#DC2626", "Ban");
        Module.BatchInsert(tblSource);
    }
    static string AddDocumentType(MemTable Table, string Code, string Name, int TradeTypeId, string NumberSeriesCode, string ModuleName, bool AffectsStock, bool AffectsFinancial, bool AffectsAccounting, int StockDirection, int FinancialDirection, int AccountingDirection, bool IsCancellation, string CancellationTargetId, int DisplayOrder, string Color, string IconName)
    {
        string Id = Sys.GenId();
        AddRow(Table,
            ("Id", Id),
            ("Code", Code),
            ("Name", Name),
            ("TradeTypeId", TradeTypeId),
            ("NumberSeriesId", FindNumberSeriesId(NumberSeriesCode)),
            ("ModuleName", ModuleName),
            ("IsActive", true),
            ("IsSystem", true),
            ("AllowManualNumber", false),
            ("AutoComplete", false),
            ("AffectsStock", AffectsStock),
            ("AffectsFinancial", AffectsFinancial),
            ("AffectsAccounting", AffectsAccounting),
            ("StockDirection", StockDirection),
            ("FinancialDirection", FinancialDirection),
            ("AccountingDirection", AccountingDirection),
            ("IsCancellation", IsCancellation),
            ("CancellationTargetId", string.IsNullOrWhiteSpace(CancellationTargetId) ? DBNull.Value : (object)CancellationTargetId),
            ("PrintTemplate", DBNull.Value),
            ("ReportName", DBNull.Value),
            ("DisplayOrder", DisplayOrder),
            ("Color", Color),
            ("IconName", IconName),
            ("Remarks", DBNull.Value)
        );
        return Id;
    }
    static string FindNumberSeriesId(string Code)
    {
        DataRow Row = FindNumberSeriesRow(Code);
        if (Row == null)
            throw new TripousDataException($"Number series not found: {Code}");
        return Row.AsString("Id");
    }
    static DataRow FindNumberSeriesRow(string Code)
    {
        string SqlText = $"select * from {DbConfig.SysNumberSeriesTableName} where Code = :Code";
        DataRow Row = Store.SelectResults(SqlText, new Dictionary<string, object>() { ["Code"] = Code });
        return Row;
    }
    /// <summary>
    /// Adds a compact chart of accounts for accounting UI tests.
    /// </summary>
    static void Add_Account()
    {
        string ModuleName = "Account";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        tblSource.CopyColumnsFrom(Module.tblItem);
        string AddAccount(string Code, string Name, object ParentAccountId, AccountType AccountType, NormalBalance NormalBalance, bool IsPosting)
        {
            string Id = Sys.GenId();
            AddRow(tblSource, ("Id", Id), ("Code", Code), ("Name", Name), ("ParentAccountId", ParentAccountId), ("AccountTypeId", (int)AccountType), ("NormalBalanceId", (int)NormalBalance), ("IsPosting", IsPosting), ("IsActive", true), ("Remarks", DBNull.Value));
            return Id;
        }
        string AssetsId = AddAccount("10", "Assets", DBNull.Value, AccountType.Asset, NormalBalance.Debit, false);
        AddAccount("10-1000", "Cash", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        AddAccount("10-2000", "Bank", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        AddAccount("10-3000", "Customers", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        AddAccount("10-4000", "Inventory", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        AddAccount("10-5000", "VAT Receivable", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        AddAccount("10-6000", "Fixed Assets", AssetsId, AccountType.Asset, NormalBalance.Debit, true);
        string LiabilitiesId = AddAccount("20", "Liabilities", DBNull.Value, AccountType.Liability, NormalBalance.Credit, false);
        AddAccount("20-1000", "Suppliers", LiabilitiesId, AccountType.Liability, NormalBalance.Credit, true);
        AddAccount("20-2000", "VAT Payable", LiabilitiesId, AccountType.Liability, NormalBalance.Credit, true);
        string EquityId = AddAccount("30", "Equity", DBNull.Value, AccountType.Equity, NormalBalance.Credit, false);
        AddAccount("30-1000", "Share Capital", EquityId, AccountType.Equity, NormalBalance.Credit, true);
        AddAccount("30-2000", "Retained Earnings", EquityId, AccountType.Equity, NormalBalance.Credit, true);
        string RevenueId = AddAccount("70", "Revenue", DBNull.Value, AccountType.Revenue, NormalBalance.Credit, false);
        AddAccount("70-1000", "Sales Revenue", RevenueId, AccountType.Revenue, NormalBalance.Credit, true);
        AddAccount("70-2000", "Service Revenue", RevenueId, AccountType.Revenue, NormalBalance.Credit, true);
        string ExpensesId = AddAccount("60", "Expenses", DBNull.Value, AccountType.Expense, NormalBalance.Debit, false);
        AddAccount("60-1000", "Purchases", ExpensesId, AccountType.Expense, NormalBalance.Debit, true);
        AddAccount("60-2000", "Cost of Goods Sold", ExpensesId, AccountType.Expense, NormalBalance.Debit, true);
        AddAccount("60-3000", "Rent Expense", ExpensesId, AccountType.Expense, NormalBalance.Debit, true);
        AddAccount("60-4000", "Utilities Expense", ExpensesId, AccountType.Expense, NormalBalance.Debit, true);
        Module.BatchInsert(tblSource);
    }
    /// <summary>
    /// Adds active assets for asset lifecycle tests.
    /// </summary>
    static void Add_Asset()
    {
        string ModuleName = "Asset";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        tblSource.CopyColumnsFrom(Module.tblItem);
        object ComputersId = SampleTables["AssetCategory"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Computers"))["Id"];
        object VehiclesId = SampleTables["AssetCategory"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Vehicles"))["Id"];
        object HeadOfficeId = SampleTables["AssetLocation"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Head Office"))["Id"];
        object WarehouseId = SampleTables["AssetLocation"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Main Warehouse"))["Id"];
        object StraightLineId = SampleTables["AssetDepreciationMethod"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Straight Line"))["Id"];
        object LaptopSupplierId = SampleTables["ProductSupplier"].Rows.Cast<DataRow>().First(x => x.AsString("SupplierCode").IsSameText("LAP-14"))["Id"];
        string UserId = Sys.GetCurrentAppUserId();
        DateTime CreatedAt = DateTime.Now;
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "QA Office Laptop"), ("AssetCategoryId", ComputersId), ("AssetLocationId", HeadOfficeId), ("StatusId", (int)AssetStatus.Active), ("AcquisitionDate", DateTime.Today.AddMonths(-6)), ("InServiceDate", DateTime.Today.AddMonths(-6)), ("AcquisitionCost", 1250.0000m), ("DepreciationMethodId", StraightLineId), ("UsefulLifeMonths", 36), ("SalvageValue", 50.0000m), ("AccumulatedDepreciation", 200.0000m), ("BookValue", 1050.0000m), ("SerialNumber", "QA-LAPTOP-001"), ("SupplierId", LaptopSupplierId), ("Remarks", "Sample asset for lifecycle tests."), ("CreatedAt", CreatedAt), ("CreatedBy", UserId), ("ModifiedAt", DBNull.Value), ("ModifiedBy", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "QA Delivery Van"), ("AssetCategoryId", VehiclesId), ("AssetLocationId", WarehouseId), ("StatusId", (int)AssetStatus.Active), ("AcquisitionDate", DateTime.Today.AddYears(-1)), ("InServiceDate", DateTime.Today.AddYears(-1)), ("AcquisitionCost", 24500.0000m), ("DepreciationMethodId", StraightLineId), ("UsefulLifeMonths", 60), ("SalvageValue", 2500.0000m), ("AccumulatedDepreciation", 4400.0000m), ("BookValue", 20100.0000m), ("SerialNumber", "QA-VAN-001"), ("SupplierId", DBNull.Value), ("Remarks", "Sample asset for depreciation and maintenance tests."), ("CreatedAt", CreatedAt), ("CreatedBy", UserId), ("ModifiedAt", DBNull.Value), ("ModifiedBy", DBNull.Value));
        Module.BatchInsert(tblSource);
    }
    /// <summary>
    /// Adds opening stock movements and matching balances for every product and warehouse.
    /// </summary>
    static void Add_Stock()
    {
        if (!Store.TableExists("StockMovement") || !Store.TableIsEmpty("StockMovement") || !Store.TableExists("StockBalance") || !Store.TableIsEmpty("StockBalance"))
            return;
        DataModule MovementModule = DataRegistry.Modules.Get("StockMovement").Create();
        DataModule BalanceModule = DataRegistry.Modules.Get("StockBalance").Create();
        MemTable tblMovement = new() { TableName = MovementModule.tblItem.TableName };
        MemTable tblBalance = new() { TableName = BalanceModule.tblItem.TableName };
        tblMovement.CopyColumnsFrom(MovementModule.tblItem);
        tblBalance.CopyColumnsFrom(BalanceModule.tblItem);
        SampleTables[tblMovement.TableName] = tblMovement;
        SampleTables[tblBalance.TableName] = tblBalance;
        DataRow MainWarehouseRow = SampleTables["Warehouse"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Main Warehouse"));
        DataRow RetailStoreRow = SampleTables["Warehouse"].Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Retail Store"));
        object StockCountDocumentTypeId = SampleTables["DocumentType"].Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("STK-CNT"))["Id"];
        string UserId = Sys.GetCurrentAppUserId();
        DateTime MovementDate = DateTime.Today.AddDays(-30);
        Dictionary<string, (decimal MainQuantity, decimal RetailQuantity, decimal UnitCost)> Values = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Coffee Machine"] = (100.0000m, 10.0000m, 175.0000m),
            ["Espresso Beans"] = (300.0000m, 40.0000m, 11.2000m),
            ["Laptop Computer 14"] = (50.0000m, 5.0000m, 900.0000m),
            ["Monitor 27 Inch"] = (80.0000m, 12.0000m, 210.0000m),
            ["Wireless Keyboard"] = (150.0000m, 25.0000m, 25.0000m),
            ["Coffee Capsules"] = (400.0000m, 80.0000m, 4.5000m),
            ["Mineral Water"] = (1000.0000m, 200.0000m, 0.3500m),
            ["Orange Juice"] = (500.0000m, 100.0000m, 1.1000m),
        };
        void AddStock(DataRow ProductRow, DataRow WarehouseRow, decimal Quantity, decimal UnitCost)
        {
            string MovementId = Sys.GenId();
            object UnitId = ProductRow["PrimaryUnitOfMeasureId"];
            DataRow UnitRow = SampleTables["UnitOfMeasure"].Rows.Cast<DataRow>().First(x => x["Id"].Equals(UnitId));
            decimal CostAmount = Quantity * UnitCost;
            string DocumentCode = WarehouseRow.AsString("Name").IsSameText("Main Warehouse") ? "OPENING-STOCK-MAIN" : "OPENING-STOCK-RETAIL";
            AddRow(tblMovement, ("Id", MovementId), ("TradeTypeId", (int)TradeType.Warehouse), ("ProductId", ProductRow["Id"]), ("WarehouseId", WarehouseRow["Id"]), ("MovementDate", MovementDate), ("Direction", 1), ("Quantity", Quantity), ("PrimaryQuantity", Quantity), ("UnitOfMeasureId", UnitId), ("UnitOfMeasureName", UnitRow.AsString("Name")), ("UnitRatio", 1.0000m), ("UnitCost", UnitCost), ("CostAmount", CostAmount), ("SourceModule", "SampleData"), ("SourceTable", "Product"), ("SourceId", ProductRow["Id"]), ("DocumentTypeId", StockCountDocumentTypeId), ("DocumentCode", DocumentCode), ("DocumentDate", MovementDate), ("CreatedAt", DateTime.Now), ("CreatedBy", UserId));
            AddRow(tblBalance, ("Id", Sys.GenId()), ("ProductId", ProductRow["Id"]), ("WarehouseId", WarehouseRow["Id"]), ("PrimaryQuantity", Quantity), ("TotalCostAmount", CostAmount), ("AverageUnitCost", UnitCost), ("LastMovementDate", MovementDate), ("LastMovementId", MovementId));
        }
        foreach (DataRow ProductRow in SampleTables["Product"].Rows)
        {
            var Value = Values[ProductRow.AsString("Name")];
            AddStock(ProductRow, MainWarehouseRow, Value.MainQuantity, Value.UnitCost);
            AddStock(ProductRow, RetailStoreRow, Value.RetailQuantity, Value.UnitCost);
        }
        MovementModule.BatchInsert(tblMovement);
        BalanceModule.BatchInsert(tblBalance);
    }

    // ● protected
    protected override void AddSampleDataInternal()
    {
        Add_DocumentType();
        Add_Account();
        Add_Asset();
        Add_Stock();
        SetIsAdded();
    }

    // ● construction
    public SampleData2()
    {
    }

    // ● properties
    public override int VersionNumber => 2;
}

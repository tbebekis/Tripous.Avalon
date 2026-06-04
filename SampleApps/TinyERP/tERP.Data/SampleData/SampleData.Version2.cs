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
        Ids["SalesInvoice"] = AddDocumentType(tblSource, "SAL-INV", "Sales Invoice", 1, "SalesInvoice", "SalesInvoice", true, true, true, -1, 1, 1, false, null, 30, "#16A34A", "FileText");
        Ids["SalesCreditNote"] = AddDocumentType(tblSource, "SAL-CN", "Sales Credit Note", 1, "SalesCreditNote", "SalesCreditNote", true, true, true, 1, -1, -1, false, null, 40, "#F59E0B", "Undo2");
        Ids["SalesReturn"] = AddDocumentType(tblSource, "SAL-RET", "Sales Return", 1, "SalesReturn", "SalesReturn", true, false, false, 1, 0, 0, false, null, 50, "#CA8A04", "RotateCcw");
        Ids["PurchaseOrder"] = AddDocumentType(tblSource, "PUR-ORD", "Purchase Order", 2, "PurchaseOrder", "PurchaseOrder", false, false, false, 0, 0, 0, false, null, 110, "#7C3AED", "ShoppingBag");
        Ids["PurchaseDeliveryNote"] = AddDocumentType(tblSource, "PUR-DN", "Purchase Delivery Note", 2, "PurchaseDeliveryNote", "PurchaseDeliveryNote", true, false, false, 1, 0, 0, false, null, 120, "#0D9488", "PackageCheck");
        Ids["PurchaseInvoice"] = AddDocumentType(tblSource, "PUR-INV", "Purchase Invoice", 2, "PurchaseInvoice", "PurchaseInvoice", true, true, true, 1, -1, -1, false, null, 130, "#059669", "ReceiptText");
        Ids["PurchaseCreditNote"] = AddDocumentType(tblSource, "PUR-CN", "Purchase Credit Note", 2, "PurchaseCreditNote", "PurchaseCreditNote", true, true, true, -1, 1, 1, false, null, 140, "#D97706", "Undo2");
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

    // ● protected
    protected override void AddSampleDataInternal()
    {
        Add_DocumentType();
        SetIsAdded();
    }

    // ● construction
    public SampleData2()
    {
    }

    // ● properties
    public override int VersionNumber => 2;
}

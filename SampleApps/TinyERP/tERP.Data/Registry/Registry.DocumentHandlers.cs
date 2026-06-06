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
    // ● static public
    static public void RegisterDocumentHandlers()
    {
        DataRegistry.AddOrUpdateDocumentHandler("SalesOrder", typeof(SalesOrderDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("SalesDeliveryNote", typeof(SalesDeliveryNoteDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("SalesInvoice", typeof(SalesInvoiceDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("SalesCreditNote", typeof(SalesCreditNoteDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("SalesReturn", typeof(SalesReturnDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("SalesCancellation", typeof(SalesCancellationDocumentHandler).FullName);
        
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseOrder", typeof(PurchaseOrderDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseDeliveryNote", typeof(PurchaseDeliveryNoteDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseInvoice", typeof(PurchaseInvoiceDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseCreditNote", typeof(PurchaseCreditNoteDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseReturn", typeof(PurchaseReturnDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("PurchaseCancellation", typeof(PurchaseCancellationDocumentHandler).FullName);
        
        DataRegistry.AddOrUpdateDocumentHandler("StockTrade", typeof(StockTradeDocumentHandler).FullName);
        DataRegistry.AddOrUpdateDocumentHandler("StockCount", typeof(StockCountDocumentHandler).FullName);
        
        DataRegistry.AddOrUpdateDocumentHandler("JournalEntry", typeof(JournalEntryDocumentHandler).FullName);
    }
}
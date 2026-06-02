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
    // ● static public
    static public void RegisterDocumentHandlers()
    {
        DataRegistry.AddOrGetDocumentHandler("SalesOrder", typeof(SalesOrderDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("SalesDeliveryNote", typeof(SalesDeliveryNoteDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("SalesInvoice", typeof(SalesInvoiceDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("SalesCreditNote", typeof(SalesCreditNoteDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("SalesReturn", typeof(SalesReturnDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("SalesCancellation", typeof(SalesCancellationDocumentHandler).FullName);
        
        DataRegistry.AddOrGetDocumentHandler("PurchaseOrder", typeof(PurchaseOrderDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("PurchaseDeliveryNote", typeof(PurchaseDeliveryNoteDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("PurchaseInvoice", typeof(PurchaseInvoiceDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("PurchaseCreditNote", typeof(PurchaseCreditNoteDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("PurchaseReturn", typeof(PurchaseReturnDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("PurchaseCancellation", typeof(PurchaseCancellationDocumentHandler).FullName);
        
        DataRegistry.AddOrGetDocumentHandler("StockTrade", typeof(StockTradeDocumentHandler).FullName);
        DataRegistry.AddOrGetDocumentHandler("StockCount", typeof(StockCountDocumentHandler).FullName);
        
        DataRegistry.AddOrGetDocumentHandler("JournalEntry", typeof(JournalEntryDocumentHandler).FullName);
    }
}
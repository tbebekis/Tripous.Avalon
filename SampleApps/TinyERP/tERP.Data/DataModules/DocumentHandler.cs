/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

[TypeStore]
public abstract class DocumentHandler
{
    // ● construction
    public DocumentHandler()
    {
    }

    // ● public
    public virtual void Validate(DocumentContext Context)
    {
    }
    public virtual void Post(DocumentContext Context)
    {
    }
    public virtual void Cancel(DocumentContext Context)
    {
    }

    // ● properties
    public DocumentHandlerDef HandlerDef { get; set; }
}

public class TradeDocumentHandler : DocumentHandler
{
    public TradeDocumentHandler()
    {
    }
}

public class SalesDocumentHandler : TradeDocumentHandler
{
    public SalesDocumentHandler()
    {
    }
}

public class SalesOrderDocumentHandler : SalesDocumentHandler
{
    public SalesOrderDocumentHandler()
    {
    }
}

public class SalesDeliveryNoteDocumentHandler : SalesDocumentHandler
{
    public SalesDeliveryNoteDocumentHandler()
    {
    }
}

public class SalesInvoiceDocumentHandler : SalesDocumentHandler
{
    public SalesInvoiceDocumentHandler()
    {
    }
}

public class SalesCreditNoteDocumentHandler : SalesDocumentHandler
{
    public SalesCreditNoteDocumentHandler()
    {
    }
}

public class SalesReturnDocumentHandler : SalesDocumentHandler
{
    public SalesReturnDocumentHandler()
    {
    }
}

public class SalesCancellationDocumentHandler : SalesDocumentHandler
{
    public SalesCancellationDocumentHandler()
    {
    }
}

public class PurchaseDocumentHandler : TradeDocumentHandler
{
    public PurchaseDocumentHandler()
    {
    }
}

public class PurchaseOrderDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseOrderDocumentHandler()
    {
    }
}

public class PurchaseDeliveryNoteDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseDeliveryNoteDocumentHandler()
    {
    }
}

public class PurchaseInvoiceDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseInvoiceDocumentHandler()
    {
    }
}

public class PurchaseCreditNoteDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseCreditNoteDocumentHandler()
    {
    }
}

public class PurchaseReturnDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseReturnDocumentHandler()
    {
    }
}

public class PurchaseCancellationDocumentHandler : PurchaseDocumentHandler
{
    public PurchaseCancellationDocumentHandler()
    {
    }
}

public class StockTradeDocumentHandler : DocumentHandler
{
    public StockTradeDocumentHandler()
    {
    }
}

public class StockCountDocumentHandler : DocumentHandler
{
    public StockCountDocumentHandler()
    {
    }
}

public class JournalEntryDocumentHandler : DocumentHandler
{
    public JournalEntryDocumentHandler()
    {
    }
}
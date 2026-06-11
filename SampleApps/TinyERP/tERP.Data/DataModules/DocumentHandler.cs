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
        if (Context == null)
            throw new TripousDataException($"{nameof(Context)} is null.");
        if (Context.DataModule == null)
            throw new TripousDataException($"{nameof(Context.DataModule)} is null.");
        if (Context.Row == null)
            throw new TripousDataException($"{nameof(Context.Row)} is null.");
        if (string.IsNullOrWhiteSpace(Context.DocumentTypeId))
            throw new TripousDataException($"{nameof(Context.DocumentTypeId)} is empty.");
        if (string.IsNullOrWhiteSpace(Context.DocumentId))
            throw new TripousDataException($"{nameof(Context.DocumentId)} is empty.");
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
    // ● construction
    public TradeDocumentHandler()
    {
    }

    // ● public
    public override void Validate(DocumentContext Context)
    {
        base.Validate(Context);

        if ((TradeStatus)Context.Row.AsInteger("TradeStatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("Only draft documents can be posted.");
        if (Context.Row.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled document cannot be posted.");
        if (Context.Row.AsBoolean("IsLocked"))
            throw new TripousBusinessException("A locked document cannot be posted.");
    }
    public override void Post(DocumentContext Context)
    {
        base.Post(Context);

        string UserId = Sys.GetCurrentAppUserId();
        if (string.IsNullOrWhiteSpace(UserId))
            throw new TripousBusinessException("The current application user is not available.");

        DateTime Now = DateTime.UtcNow;
        Context.Row.SetValue("TradeStatusId", (int)TradeStatus.Posted);
        Context.Row.SetValue("PostingDate", DateTime.Today);
        Context.Row.SetValue("PostedAt", Now);
        Context.Row.SetValue("PostedBy", UserId);
        Context.Row.SetValue("IsLocked", true);
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

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class StockTradeDocumentHandler : DocumentHandler
{
    public StockTradeDocumentHandler()
    {
    }
}

public class StockCountDocumentHandler : DocumentHandler
{
    // ● construction
    public StockCountDocumentHandler()
    {
    }

    // ● public
    public override void Validate(DocumentContext Context)
    {
        base.Validate(Context);

        if ((TradeStatus)Context.Row.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("Only draft stock counts can be posted.");
    }
    public override void Post(DocumentContext Context)
    {
        base.Post(Context);
        Context.Row.SetValue("StatusId", (int)TradeStatus.Posted);
    }
}



/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Validates and posts stock count documents.
/// </summary>
public class StockCountDocumentHandler : DocumentHandler
{
    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public StockCountDocumentHandler()
    {
    }

    // ● public
    /// <summary>
    /// Validates that the stock count can be posted.
    /// </summary>
    public override void Validate(DocumentContext Context)
    {
        base.Validate(Context);

        if ((TradeStatus)Context.Row.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("Only draft stock counts can be posted.");
    }
    /// <summary>
    /// Applies posted status to the stock count.
    /// </summary>
    public override void Post(DocumentContext Context)
    {
        base.Post(Context);
        Context.Row.SetValue("StatusId", (int)TradeStatus.Posted);
    }
}

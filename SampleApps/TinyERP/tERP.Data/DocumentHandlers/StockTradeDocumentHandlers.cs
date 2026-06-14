/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Validates and posts stock transaction documents.
/// </summary>
public class StockTradeDocumentHandler : DocumentHandler
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public StockTradeDocumentHandler()
    {
    }

    // ● public
    /// <summary>
    /// Validates that the stock transaction can be posted.
    /// </summary>
    public override void Validate(DocumentContext Context)
    {
        base.Validate(Context);

        if ((TradeStatus)Context.Row.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("Only draft stock transactions can be posted.");
        if (Context.Row.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled stock transaction cannot be posted.");
        if (Context.Row.AsBoolean("IsLocked"))
            throw new TripousBusinessException("A locked stock transaction cannot be posted.");
    }
    /// <summary>
    /// Applies posted status and audit values to the stock transaction.
    /// </summary>
    public override void Post(DocumentContext Context)
    {
        base.Post(Context);

        string UserId = Sys.GetCurrentAppUserId();
        if (string.IsNullOrWhiteSpace(UserId))
            throw new TripousBusinessException("The current application user is not available.");

        Context.Row.SetValue("StatusId", (int)TradeStatus.Posted);
        Context.Row.SetValue("PostingDate", DateTime.Today);
        Context.Row.SetValue("PostedAt", DateTime.UtcNow);
        Context.Row.SetValue("PostedBy", UserId);
        Context.Row.SetValue("IsLocked", true);
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

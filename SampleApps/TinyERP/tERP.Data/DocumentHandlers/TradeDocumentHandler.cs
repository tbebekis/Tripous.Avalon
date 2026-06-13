/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

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
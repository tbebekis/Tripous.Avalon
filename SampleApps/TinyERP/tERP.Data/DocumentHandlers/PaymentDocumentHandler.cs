/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Validates and posts payment documents.
/// </summary>
public class PaymentDocumentHandler : DocumentHandler
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public PaymentDocumentHandler()
    {
    }

    // ● public
    /// <summary>
    /// Validates that the payment can be posted.
    /// </summary>
    public override void Validate(DocumentContext Context)
    {
        base.Validate(Context);
        if ((TradeStatus)Context.Row.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("Only draft payments can be posted.");
        if (Context.Row.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled payment cannot be posted.");
        if (Context.Row.AsBoolean("IsLocked"))
            throw new TripousBusinessException("A locked payment cannot be posted.");
    }
    /// <summary>
    /// Applies posted status to the payment.
    /// </summary>
    public override void Post(DocumentContext Context)
    {
        base.Post(Context);
        string UserId = Sys.GetCurrentAppUserId();
        if (string.IsNullOrWhiteSpace(UserId))
            throw new TripousBusinessException("The current application user is not available.");
        DateTime Now = DateTime.UtcNow;
        Context.Row.SetValue("StatusId", (int)TradeStatus.Posted);
        Context.Row.SetValue("PostingDate", DateTime.Today);
        Context.Row.SetValue("PostedAt", Now);
        Context.Row.SetValue("PostedBy", UserId);
        Context.Row.SetValue("IsLocked", true);
    }
}

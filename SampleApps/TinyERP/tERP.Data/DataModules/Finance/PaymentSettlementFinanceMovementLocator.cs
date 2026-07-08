/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Locator for payment settlement finance movements.
/// </summary>
[TypeStore]
public class PaymentSettlementFinanceMovementLocator: Locator
{
    // ● private
    /// <summary>
    /// Returns a named request parameter.
    /// </summary>
    object GetParam(LocatorRequest Request, string Name)
    {
        if (Request?.Context?.Params == null || string.IsNullOrWhiteSpace(Name))
            return null;

        return Request.Context.Params.TryGetValue(Name, out object Value) ? Value : null;
    }
    /// <summary>
    /// Returns the payment header row of a settlement row.
    /// </summary>
    DataRow GetPaymentRow(LocatorRequest Request)
    {
        DataRow PaymentRow = GetParam(Request, "PaymentRow") as DataRow;
        if (PaymentRow != null)
            return PaymentRow;

        DataRow SettlementRow = GetParam(Request, "SettlementRow") as DataRow
            ?? GetParam(Request, "DataRow") as DataRow
            ?? GetParam(Request, "Row") as DataRow
            ?? GetParam(Request, "Context") as DataRow;

        string PaymentId = SettlementRow != null ? SettlementRow.AsString("PaymentId") : Convert.ToString(GetParam(Request, "PaymentId"));
        DataTable Table = SettlementRow?.Table.DataSet?.Tables["Payment"];
        if (Table != null)
        {
            foreach (DataRow Row in Table.Rows)
            {
                if (Row.RowState != DataRowState.Deleted && Row.AsString("Id").IsSameText(PaymentId))
                    return Row;
            }
            if (string.IsNullOrWhiteSpace(PaymentId) && Table.Rows.Count == 1 && Table.Rows[0].RowState != DataRowState.Deleted)
                return Table.Rows[0];
        }

        if (string.IsNullOrWhiteSpace(PaymentId))
            return null;

        return Store.SelectResults(@"
select *
from Payment
where Id = :Id
", new Dictionary<string, object>()
        {
            ["Id"] = PaymentId,
        });
    }
    /// <summary>
    /// Returns the financial direction of the payment document type.
    /// </summary>
    int GetFinancialDirection(DataRow PaymentRow)
    {
        if (PaymentRow == null)
            return 0;

        object Result = Store.SelectResult(@"
select FinancialDirection
from DocumentType
where Id = :Id
", 0, new Dictionary<string, object>()
        {
            ["Id"] = PaymentRow.AsString("DocumentTypeId"),
        });

        return Sys.IsNull(Result) ? 0 : Convert.ToInt32(Result);
    }
    /// <summary>
    /// Escapes a string value for inline SQL text.
    /// </summary>
    string ToSql(string Value) => (Value ?? string.Empty).Replace("'", "''");
    /// <summary>
    /// Returns the posted settlement amount SQL expression.
    /// </summary>
    string GetPostedSettlementAmountSql(string PaymentId)
    {
        string CurrentPaymentId = ToSql(PaymentId);
        return $@"
coalesce((
    select sum(PS.Amount)
    from PaymentSettlement PS
    inner join Payment P on P.Id = PS.PaymentId
    where PS.FinanceMovementId = FM.Id
      and P.Id <> '{CurrentPaymentId}'
      and P.StatusId = {(int)TradeStatus.Posted}
      and P.IsCancelled = 0
), 0)
";
    }
    /// <summary>
    /// Returns a fallback SELECT statement used when no payment context is available.
    /// </summary>
    SelectSql GetFallbackSelectSql()
    {
        SelectSql Result = new();
        Result.Select = $@"
 FM.Id as Id
,FM.DocumentCode as DocumentCode
,FM.DocumentDate as DocumentDate
,P.Code as PersonCode
,P.Name as PersonName
,case FM.TradeTypeId
     when {(int)TradeType.Sales} then 'Sales'
     when {(int)TradeType.Purchases} then 'Purchases'
     else ''
 end as TradeType
,FM.Direction as Direction
,FM.Amount as Amount
,FM.Amount as OpenAmount
";
        Result.From = @"
FinanceMovement FM
left join Person P on P.Id = FM.PersonId
";
        Result.OrderBy = "FM.DocumentDate, FM.DocumentCode";
        return Result;
    }
    /// <summary>
    /// Returns the payment-aware SELECT statement for open settlement movements.
    /// </summary>
    SelectSql GetPaymentSelectSql(DataRow PaymentRow)
    {
        if (PaymentRow == null)
            return GetFallbackSelectSql();
        if (!string.IsNullOrWhiteSpace(PaymentRow.AsString("CancelledPaymentId")))
            throw new TripousBusinessException("Payment cancellation documents cannot have settlement lines.");
        if (string.IsNullOrWhiteSpace(PaymentRow.AsString("PersonId")))
            throw new TripousBusinessException("Select a customer or supplier before selecting a finance movement.");
        if (string.IsNullOrWhiteSpace(PaymentRow.AsString("CurrencyId")))
            throw new TripousBusinessException("Select a currency before selecting a finance movement.");
        if (PaymentRow.AsInteger("PartnerTradeTypeId") == (int)TradeType.None)
            throw new TripousBusinessException("Payment trade type is required before selecting a finance movement.");

        int Direction = GetFinancialDirection(PaymentRow);
        if (Direction == 0)
            throw new TripousBusinessException("Payment financial direction is required before selecting a finance movement.");

        string PaymentId = PaymentRow.AsString("Id");
        string PostedSettlementAmountSql = GetPostedSettlementAmountSql(PaymentId);
        string OpenAmountSql = $"FM.Amount - {PostedSettlementAmountSql}";

        SelectSql Result = new();
        Result.Select = $@"
 FM.Id as Id
,FM.DocumentCode as DocumentCode
,FM.DocumentDate as DocumentDate
,P.Code as PersonCode
,P.Name as PersonName
,case FM.TradeTypeId
     when {(int)TradeType.Sales} then 'Sales'
     when {(int)TradeType.Purchases} then 'Purchases'
     else ''
 end as TradeType
,FM.Direction as Direction
,FM.Amount as Amount
,{OpenAmountSql} as OpenAmount
";
        Result.From = @"
FinanceMovement FM
left join Person P on P.Id = FM.PersonId
";
        Result.Where = $@"
FM.SourceTable = 'Trade'
and FM.PersonId = '{ToSql(PaymentRow.AsString("PersonId"))}'
and FM.CurrencyId = '{ToSql(PaymentRow.AsString("CurrencyId"))}'
and FM.TradeTypeId = {PaymentRow.AsInteger("PartnerTradeTypeId")}
and FM.Direction = {Direction}
and FM.CancellationMovementId is null
and {OpenAmountSql} > 0
";
        Result.OrderBy = "FM.DocumentDate, FM.DocumentCode";
        return Result;
    }

    // ● protected
    /// <summary>
    /// Returns the SELECT statement to execute.
    /// </summary>
    protected override string GetSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        SelectSql SelectSql = GetPaymentSelectSql(GetPaymentRow(Request));
        string Result = $"select * from ({SelectSql.Text}) X";
        string Where = GetWhereSql(LocatorDef, Request);

        if (!string.IsNullOrWhiteSpace(Where))
            Result += $" where {Where}";

        string OrderBy = !string.IsNullOrWhiteSpace(LocatorDef.OrderBy) ? LocatorDef.OrderBy : SelectSql.OrderBy;
        if (!string.IsNullOrWhiteSpace(OrderBy))
            Result += $" order by {OrderBy}";

        return Store.Provider.ApplyRowLimit(Result, LocatorDef.MaximumResultCount + 1);
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public PaymentSettlementFinanceMovementLocator()
    {
    }
}

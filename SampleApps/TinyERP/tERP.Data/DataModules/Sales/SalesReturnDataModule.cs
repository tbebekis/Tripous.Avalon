/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class SalesReturnDataModule: SalesStockDataModule
{
    // ● protected
    protected virtual Dictionary<string, decimal> GetReturnedQuantities()
    {
        MemTable LineTable = FindItemTable("TradeLine");
        if (LineTable == null)
            throw new TripousDataException("TradeLine table is not available.");

        Dictionary<string, decimal> Result = new(StringComparer.OrdinalIgnoreCase);
        foreach (DataRow Row in LineTable.Rows)
        {
            if (Row.RowState == DataRowState.Deleted || Row.RowState == DataRowState.Detached)
                continue;

            string SourceTradeLineId = Row.AsString("SourceTradeLineId");
            if (string.IsNullOrWhiteSpace(SourceTradeLineId))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Source Sales Delivery Note line is required.");

            decimal Quantity = Row.AsDecimal("Quantity");
            if (Quantity <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");

            Result.TryGetValue(SourceTradeLineId, out decimal TotalQuantity);
            Result[SourceTradeLineId] = TotalQuantity + Quantity;
        }

        if (Result.Count == 0)
            throw new TripousBusinessException("The Sales Return has no lines.");

        return Result;
    }
    protected virtual void UpdateSourceReturnedQuantities(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("SourceId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow SourceDelivery = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceDelivery == null)
            throw new TripousBusinessException("The source Sales Delivery Note does not exist.");
        if ((TradeStatus)SourceDelivery.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Sales Delivery Notes can be returned.");
        if (SourceDelivery.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Sales Delivery Note cannot be returned.");

        Dictionary<string, decimal> Quantities = GetReturnedQuantities();
        foreach (KeyValuePair<string, decimal> Entry in Quantities.OrderBy(Item => Item.Key))
        {
            DataRow SourceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (SourceLine == null || !SourceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException("A source Sales Delivery Note line does not exist.");

            decimal DeliveredQuantity = SourceLine.AsDecimal("Quantity");
            decimal ReturnedQuantity = SourceLine.AsDecimal("ReturnedQuantity");
            decimal RemainingQuantity = DeliveredQuantity - ReturnedQuantity;
            if (Entry.Value > RemainingQuantity)
                throw new TripousBusinessException($"Return quantity {Entry.Value} exceeds remaining quantity {RemainingQuantity}.");

            string SqlText = """
                             update TradeLine
                             set ReturnedQuantity = :ReturnedQuantity
                             where Id = :Id
                             """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Entry.Key,
                ["ReturnedQuantity"] = ReturnedQuantity + Entry.Value,
            });
        }
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceReturnedQuantities(e.Transaction);
    }

    // ● construction
    public SalesReturnDataModule()
    {
    }
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class PurchaseDeliveryNoteDataModule: PurchaseStockDataModule
{
    // ● protected
    protected virtual void CheckCanCreateInvoice()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Delivery Note is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Delivery Note changes before creating a Purchase Invoice.");
    }
    protected virtual void CheckCanCreateReturn()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Delivery Note is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Delivery Note changes before creating a Purchase Return.");
    }
    protected virtual Dictionary<string, decimal> GetSourceLineQuantities()
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
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Source Purchase Order line is required.");

            decimal Quantity = Row.AsDecimal("Quantity");
            if (Quantity <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");

            Result.TryGetValue(SourceTradeLineId, out decimal TotalQuantity);
            Result[SourceTradeLineId] = TotalQuantity + Quantity;
        }

        if (Result.Count == 0)
            throw new TripousBusinessException("The Purchase Delivery Note has no lines.");

        return Result;
    }
    protected virtual void UpdateSourceExecutedQuantities(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("SourceId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow SourceOrder = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceOrder == null)
            throw new TripousBusinessException("The source Purchase Order does not exist.");
        if ((TradeStatus)SourceOrder.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Purchase Orders can be received.");
        if (SourceOrder.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Purchase Order cannot be received.");

        Dictionary<string, decimal> Quantities = GetSourceLineQuantities();
        foreach (KeyValuePair<string, decimal> Entry in Quantities.OrderBy(Item => Item.Key))
        {
            DataRow SourceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (SourceLine == null || !SourceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException("A source Purchase Order line does not exist.");

            decimal OrderedQuantity = SourceLine.AsDecimal("Quantity");
            decimal ExecutedQuantity = SourceLine.AsDecimal("ExecutedQuantity");
            decimal RemainingQuantity = OrderedQuantity - ExecutedQuantity;
            if (Entry.Value > RemainingQuantity)
                throw new TripousBusinessException($"Receipt quantity {Entry.Value} exceeds remaining quantity {RemainingQuantity}.");

            string SqlText = """
                             update TradeLine
                             set ExecutedQuantity = :ExecutedQuantity
                             where Id = :Id
                             """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Entry.Key,
                ["ExecutedQuantity"] = ExecutedQuantity + Entry.Value,
            });
        }

        string CompletionSql = """
                               select count(*)
                               from TradeLine
                               where TradeId = :TradeId
                                 and Quantity > ExecutedQuantity
                               """;
        int RemainingLineCount = Store.IntegerResult(Transaction, CompletionSql, 0, new Dictionary<string, object>()
        {
            ["TradeId"] = SourceId,
        });
        if (RemainingLineCount == 0)
        {
            string SqlText = """
                             update Trade
                             set TradeStatusId = :TradeStatusId,
                                 ModifiedAt = :ModifiedAt,
                                 ModifiedBy = :ModifiedBy
                             where Id = :Id
                             """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = SourceId,
                ["TradeStatusId"] = (int)TradeStatus.Completed,
                ["ModifiedAt"] = DateTime.UtcNow,
                ["ModifiedBy"] = Sys.GetCurrentAppUserId(),
            });
        }
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceExecutedQuantities(e.Transaction);
    }

    // ● construction
    public PurchaseDeliveryNoteDataModule()
    {
    }

    // ● public
    public virtual PurchaseReturnDataModule CreateReturn()
    {
        CheckCanCreateReturn();
        PurchaseReturnDataModule Result = CreateTransformedDocument("PurchaseReturn", "Purchase Delivery Note", "ReturnedQuantity") as PurchaseReturnDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Return module.");
        return Result;
    }
    public virtual PurchaseInvoiceDataModule CreateInvoice()
    {
        CheckCanCreateInvoice();
        PurchaseInvoiceDataModule Result = CreateTransformedDocument("PurchaseInvoice", "Purchase Delivery Note", "InvoicedQuantity") as PurchaseInvoiceDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Invoice module.");
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Purchase Return data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateReturn(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        PurchaseReturnDataModule ReturnModule = CreateReturn();
        JsonDataModule Result = new(ReturnModule);
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a transformed Purchase Invoice data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateInvoice(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        PurchaseInvoiceDataModule InvoiceModule = CreateInvoice();
        JsonDataModule Result = new(InvoiceModule);
        return Result;
    }
    /// <summary>
    /// Returns true when the persisted delivery note has quantity that can still be invoiced.
    /// </summary>
    public override bool HasRemainingInvoiceQuantity()
    {
        if (CurrentRow == null)
            return false;

        int Count = Store.IntegerResult("""
                                        select count(*)
                                        from TradeLine
                                        where TradeId = :TradeId
                                          and Quantity > InvoicedQuantity
                                        """, 0, new Dictionary<string, object>()
        {
            ["TradeId"] = CurrentRow.AsString("Id"),
        });
        return Count > 0;
    }
    /// <summary>
    /// Returns true when the persisted delivery note has quantity that can still be returned.
    /// </summary>
    public override bool HasRemainingTransformQuantity()
    {
        return HasRemainingQuantity("ReturnedQuantity");
    }
}

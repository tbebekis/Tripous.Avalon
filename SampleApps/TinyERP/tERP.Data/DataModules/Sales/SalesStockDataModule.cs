/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;


/// <summary>
/// Base data module for sales documents that affect stock.
/// </summary>
public class SalesStockDataModule: SalesDataModule
{
    // ● protected
    /// <summary>
    /// Creates a stock movement and updates the corresponding stock balance.
    /// </summary>
    protected virtual void CreateStockMovement(DbTransaction Transaction, DataRow Row, string UserId, DateTime CreatedAt, DateTime MovementDate)
    {
        string ProductId = Row.AsString("ProductId");
        string WarehouseId = Row.AsString("WarehouseId");
        if (string.IsNullOrWhiteSpace(WarehouseId))
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Warehouse is required for stock movement.");

        DataRow Warehouse = Store.Provider.SelectForUpdate(Transaction, "Warehouse", "Id", WarehouseId);
        if (Warehouse == null)
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Warehouse does not exist.");

        string UnitOfMeasureId = Row.AsString("UnitOfMeasureId");
        if (string.IsNullOrWhiteSpace(UnitOfMeasureId))
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Unit of measure is required for stock movement.");

        decimal Quantity = Row.AsDecimal("Quantity");
        decimal PrimaryQuantity = Row.AsDecimal("PrimaryUnitQuantity");
        if (Quantity <= 0 || PrimaryQuantity <= 0)
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero for stock movement.");

        string BalanceSql = """
                            select Id
                            from StockBalance
                            where ProductId = :ProductId
                              and WarehouseId = :WarehouseId
                            """;
        DataRow BalanceKey = Store.SelectResults(Transaction, BalanceSql, new Dictionary<string, object>()
        {
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
        });
        DataRow Balance = BalanceKey == null
            ? null
            : Store.Provider.SelectForUpdate(Transaction, "StockBalance", "Id", BalanceKey["Id"]);

        decimal CurrentQuantity = Balance == null ? 0 : Balance.AsDecimal("PrimaryQuantity");
        decimal CurrentTotalCostAmount = Balance == null ? 0 : Balance.AsDecimal("TotalCostAmount");
        decimal UnitCost = Balance == null ? 0 : Balance.AsDecimal("AverageUnitCost");
        decimal CostAmount = RoundAmount(PrimaryQuantity * UnitCost);
        decimal NewQuantity = RoundAmount(CurrentQuantity + DocumentType.StockDirection * PrimaryQuantity);
        decimal NewTotalCostAmount = RoundAmount(CurrentTotalCostAmount + DocumentType.StockDirection * CostAmount);
        if (NewQuantity == 0)
            NewTotalCostAmount = 0;
        decimal NewAverageUnitCost = NewQuantity == 0 ? 0 : RoundAmount(NewTotalCostAmount / NewQuantity);

        if (NewQuantity < 0 && !Warehouse.AsBoolean("AllowNegativeStock"))
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Stock quantity cannot become negative.");

        string MovementId = Sys.GenId();
        string SqlText = """
                         insert into StockMovement
                         (
                           Id, TradeTypeId, ProductId, WarehouseId,
                           MovementDate, Direction, Quantity, PrimaryQuantity,
                           UnitOfMeasureId, UnitOfMeasureName, UnitRatio,
                           UnitCost, CostAmount,
                           SourceModule, SourceTable, SourceId,
                           DocumentTypeId, DocumentCode, DocumentDate,
                           CreatedAt, CreatedBy
                         )
                         values
                         (
                           :Id, :TradeTypeId, :ProductId, :WarehouseId,
                           :MovementDate, :Direction, :Quantity, :PrimaryQuantity,
                           :UnitOfMeasureId, :UnitOfMeasureName, :UnitRatio,
                           :UnitCost, :CostAmount,
                           :SourceModule, :SourceTable, :SourceId,
                           :DocumentTypeId, :DocumentCode, :DocumentDate,
                           :CreatedAt, :CreatedBy
                         )
                         """;
        Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
        {
            ["Id"] = MovementId,
            ["TradeTypeId"] = DocumentType.TradeTypeId,
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["MovementDate"] = MovementDate,
            ["Direction"] = DocumentType.StockDirection,
            ["Quantity"] = Quantity,
            ["PrimaryQuantity"] = PrimaryQuantity,
            ["UnitOfMeasureId"] = UnitOfMeasureId,
            ["UnitOfMeasureName"] = Row.AsString("UnitOfMeasureName"),
            ["UnitRatio"] = Row.AsDecimal("UnitRatio", 1),
            ["UnitCost"] = UnitCost,
            ["CostAmount"] = CostAmount,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "TradeLine",
            ["SourceId"] = Row.AsString("Id"),
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            ["CreatedAt"] = CreatedAt,
            ["CreatedBy"] = UserId,
        });

        if (Balance == null)
        {
            SqlText = """
                      insert into StockBalance
                      (
                        Id, ProductId, WarehouseId,
                        PrimaryQuantity, TotalCostAmount, AverageUnitCost,
                        LastMovementDate, LastMovementId
                      )
                      values
                      (
                        :Id, :ProductId, :WarehouseId,
                        :PrimaryQuantity, :TotalCostAmount, :AverageUnitCost,
                        :LastMovementDate, :LastMovementId
                      )
                      """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Sys.GenId(),
                ["ProductId"] = ProductId,
                ["WarehouseId"] = WarehouseId,
                ["PrimaryQuantity"] = NewQuantity,
                ["TotalCostAmount"] = NewTotalCostAmount,
                ["AverageUnitCost"] = NewAverageUnitCost,
                ["LastMovementDate"] = MovementDate,
                ["LastMovementId"] = MovementId,
            });
        }
        else
        {
            SqlText = """
                      update StockBalance
                      set PrimaryQuantity = :PrimaryQuantity,
                          TotalCostAmount = :TotalCostAmount,
                          AverageUnitCost = :AverageUnitCost,
                          LastMovementDate = :LastMovementDate,
                          LastMovementId = :LastMovementId
                      where Id = :Id
                      """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Balance["Id"],
                ["PrimaryQuantity"] = NewQuantity,
                ["TotalCostAmount"] = NewTotalCostAmount,
                ["AverageUnitCost"] = NewAverageUnitCost,
                ["LastMovementDate"] = MovementDate,
                ["LastMovementId"] = MovementId,
            });
        }
    }
    /// <summary>
    /// Creates stock movements for all active document lines.
    /// </summary>
    protected virtual void CreateStockMovements(DbTransaction Transaction)
    {
        if (!DocumentType.AffectsStock || DocumentType.StockDirection == 0)
            return;

        MemTable LineTable = FindItemTable("TradeLine");
        if (LineTable == null)
            throw new TripousDataException("TradeLine table is not available.");

        string UserId = Sys.GetCurrentAppUserId();
        DateTime CreatedAt = DateTime.UtcNow;
        DateTime MovementDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);

        IEnumerable<DataRow> Rows = LineTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .OrderBy(Row => Row.AsString("WarehouseId"))
            .ThenBy(Row => Row.AsString("ProductId"));
        foreach (DataRow Row in Rows)
        {
            string ProductId = Row.AsString("ProductId");
            if (string.IsNullOrWhiteSpace(ProductId))
                continue;

            CreateStockMovement(Transaction, Row, UserId, CreatedAt, MovementDate);
        }
    }
    protected virtual bool UpdatesSourceOrder() => false;
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
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Source Sales Order line is required.");

            decimal Quantity = Row.AsDecimal("Quantity");
            if (Quantity <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");

            Result.TryGetValue(SourceTradeLineId, out decimal TotalQuantity);
            Result[SourceTradeLineId] = TotalQuantity + Quantity;
        }

        if (Result.Count == 0)
            throw new TripousBusinessException("The Sales Delivery Note has no lines.");

        return Result;
    }
    protected virtual void UpdateSourceExecutedQuantities(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("SourceId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow SourceOrder = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceOrder == null)
            throw new TripousBusinessException("The source Sales Order does not exist.");
        if ((TradeStatus)SourceOrder.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Sales Orders can be delivered.");
        if (SourceOrder.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Sales Order cannot be delivered.");

        Dictionary<string, decimal> Quantities = GetSourceLineQuantities();
        foreach (KeyValuePair<string, decimal> Entry in Quantities.OrderBy(Item => Item.Key))
        {
            DataRow SourceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (SourceLine == null || !SourceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException("A source Sales Order line does not exist.");

            decimal OrderedQuantity = SourceLine.AsDecimal("Quantity");
            decimal ExecutedQuantity = SourceLine.AsDecimal("ExecutedQuantity");
            decimal RemainingQuantity = OrderedQuantity - ExecutedQuantity;
            if (Entry.Value > RemainingQuantity)
                throw new TripousBusinessException($"Delivery quantity {Entry.Value} exceeds remaining quantity {RemainingQuantity}.");

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
        {
            CreateStockMovements(e.Transaction);
            if (UpdatesSourceOrder())
                UpdateSourceExecutedQuantities(e.Transaction);
        }
    }

    // ● construction
    public SalesStockDataModule()
    {
    }
}

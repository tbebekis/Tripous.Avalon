/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class PurchaseStockDataModule: PurchaseDataModule
{
    // ● protected
    /// <summary>
    /// Creates a stock movement and updates the moving-average stock balance.
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
        decimal UnitCost;
        decimal CostAmount;
        if (DocumentType.StockDirection > 0)
        {
            CostAmount = RoundAmount(Row.AsDecimal("NetAmount") - Row.AsDecimal("DocumentDiscountAmount"));
            if (CostAmount < 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Net stock cost cannot be negative.");
            UnitCost = RoundAmount(CostAmount / PrimaryQuantity);
        }
        else
        {
            UnitCost = Balance == null ? 0 : Balance.AsDecimal("AverageUnitCost");
            CostAmount = RoundAmount(PrimaryQuantity * UnitCost);
        }

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

        MemTable LineTable = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("TradeLine"));
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
            if (!string.IsNullOrWhiteSpace(Row.AsString("ProductId")))
                CreateStockMovement(Transaction, Row, UserId, CreatedAt, MovementDate);
        }
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            CreateStockMovements(e.Transaction);
    }

    // ● construction
    public PurchaseStockDataModule()
    {
    }
}
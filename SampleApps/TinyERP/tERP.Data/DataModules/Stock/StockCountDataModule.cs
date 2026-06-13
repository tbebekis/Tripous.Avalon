/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class StockCountDataModule: DocumentDataModule
{
    // ● private fields
    int fCalculationLevel;

    // ● private
    decimal Round(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    void CalculateLine(DataRow Row)
    {
        if (Row == null || Row.RowState == DataRowState.Deleted || Row.RowState == DataRowState.Detached)
            return;

        decimal DifferenceQuantity = Round(Row.AsDecimal("CountedQuantity") - Row.AsDecimal("SystemQuantity"));
        Row.SetValue("DifferenceQuantity", DifferenceQuantity);
        Row.SetValue("DifferenceCostAmount", Round(DifferenceQuantity * Row.AsDecimal("UnitCost")));
    }
    void LoadLineStock(DataRow Row)
    {
        string ProductId = Row.AsString("ProductId");
        string WarehouseId = CurrentRow.AsString("WarehouseId");
        if (string.IsNullOrWhiteSpace(ProductId))
            return;

        string SqlText = """
                         select
                           Product.Code,
                           Product.Name,
                           Product.PrimaryUnitOfMeasureId,
                           coalesce(StockBalance.PrimaryQuantity, 0) as PrimaryQuantity,
                           coalesce(StockBalance.AverageUnitCost, 0) as AverageUnitCost
                         from
                           Product
                             left join StockBalance on StockBalance.ProductId = Product.Id
                               and StockBalance.WarehouseId = :WarehouseId
                         where
                           Product.Id = :ProductId
                         """;
        DataRow Product = Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
        });
        if (Product == null)
            throw new TripousBusinessException("The selected product does not exist.");

        Row.SetValue("ProductCode", Product["Code"]);
        Row.SetValue("ProductName", Product["Name"]);
        Row.SetValue("UnitOfMeasureId", Product["PrimaryUnitOfMeasureId"]);
        Row.SetValue("SystemQuantity", Product["PrimaryQuantity"]);
        Row.SetValue("UnitCost", Product["AverageUnitCost"]);
        CalculateLine(Row);
    }
    void ValidateStockCount()
    {
        MemTable LineTable = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("StockCountLine"));
        if (LineTable == null)
            throw new TripousDataException("StockCountLine table is not available.");

        List<DataRow> Rows = LineTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();
        if (Rows.Count == 0)
            throw new TripousBusinessException("The Stock Count has no lines.");

        HashSet<string> ProductIds = new(StringComparer.OrdinalIgnoreCase);
        foreach (DataRow Row in Rows)
        {
            string ProductId = Row.AsString("ProductId");
            if (string.IsNullOrWhiteSpace(ProductId))
                throw new TripousBusinessException("Product is required on every Stock Count line.");
            if (!ProductIds.Add(ProductId))
                throw new TripousBusinessException($"{Row.AsString("ProductName")}: Product exists more than once in the Stock Count.");
            if (string.IsNullOrWhiteSpace(Row.AsString("UnitOfMeasureId")))
                throw new TripousBusinessException($"{Row.AsString("ProductName")}: Unit of measure is required.");
            if (Row.AsDecimal("CountedQuantity") < 0)
                throw new TripousBusinessException($"{Row.AsString("ProductName")}: Counted quantity cannot be negative.");
            if (Row.AsDecimal("UnitCost") < 0)
                throw new TripousBusinessException($"{Row.AsString("ProductName")}: Unit cost cannot be negative.");
        }
    }

    // ● protected
    protected virtual void CreateStockAdjustment(DbTransaction Transaction, DataRow Row, string WarehouseId, string UserId, DateTime CreatedAt, DateTime MovementDate)
    {
        string ProductId = Row.AsString("ProductId");
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
        decimal SystemQuantity = Row.AsDecimal("SystemQuantity");
        if (CurrentQuantity != SystemQuantity)
            throw new TripousBusinessException($"{Row.AsString("ProductName")}: Stock changed after the count was entered. Expected {SystemQuantity}, current {CurrentQuantity}.");

        decimal CountedQuantity = Row.AsDecimal("CountedQuantity");
        decimal DifferenceQuantity = Round(CountedQuantity - CurrentQuantity);
        if (DifferenceQuantity == 0)
            return;

        decimal CurrentTotalCostAmount = Balance == null ? 0 : Balance.AsDecimal("TotalCostAmount");
        decimal CurrentAverageUnitCost = Balance == null ? 0 : Balance.AsDecimal("AverageUnitCost");
        int Direction = DifferenceQuantity > 0 ? 1 : -1;
        decimal MovementQuantity = Math.Abs(DifferenceQuantity);
        decimal UnitCost = Direction > 0 ? Row.AsDecimal("UnitCost") : CurrentAverageUnitCost;
        decimal CostAmount = Round(MovementQuantity * UnitCost);
        decimal NewTotalCostAmount = CountedQuantity == 0
            ? 0
            : Round(CurrentTotalCostAmount + Direction * CostAmount);
        decimal NewAverageUnitCost = CountedQuantity == 0 ? 0 : Round(NewTotalCostAmount / CountedQuantity);
        object UnitOfMeasureName = Store.SelectResult(Transaction, "select Name from UnitOfMeasure where Id = :Id", "", new Dictionary<string, object>()
        {
            ["Id"] = Row.AsString("UnitOfMeasureId"),
        });
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
                           :UnitOfMeasureId, :UnitOfMeasureName, 1,
                           :UnitCost, :CostAmount,
                           :SourceModule, :SourceTable, :SourceId,
                           :DocumentTypeId, :DocumentCode, :DocumentDate,
                           :CreatedAt, :CreatedBy
                         )
                         """;
        Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
        {
            ["Id"] = MovementId,
            ["TradeTypeId"] = CurrentRow.AsInteger("TradeTypeId"),
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["MovementDate"] = MovementDate,
            ["Direction"] = Direction,
            ["Quantity"] = MovementQuantity,
            ["PrimaryQuantity"] = MovementQuantity,
            ["UnitOfMeasureId"] = Row.AsString("UnitOfMeasureId"),
            ["UnitOfMeasureName"] = UnitOfMeasureName,
            ["UnitCost"] = UnitCost,
            ["CostAmount"] = CostAmount,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "StockCountLine",
            ["SourceId"] = Row.AsString("Id"),
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("CountDate", DateTime.Today),
            ["CreatedAt"] = CreatedAt,
            ["CreatedBy"] = UserId,
        });

        SqlText = """
                  update StockCountLine
                  set DifferenceQuantity = :DifferenceQuantity,
                      UnitCost = :UnitCost,
                      DifferenceCostAmount = :DifferenceCostAmount
                  where Id = :Id
                  """;
        Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
        {
            ["Id"] = Row.AsString("Id"),
            ["DifferenceQuantity"] = DifferenceQuantity,
            ["UnitCost"] = UnitCost,
            ["DifferenceCostAmount"] = Direction * CostAmount,
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
                ["PrimaryQuantity"] = CountedQuantity,
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
                ["PrimaryQuantity"] = CountedQuantity,
                ["TotalCostAmount"] = NewTotalCostAmount,
                ["AverageUnitCost"] = NewAverageUnitCost,
                ["LastMovementDate"] = MovementDate,
                ["LastMovementId"] = MovementId,
            });
        }
    }
    protected virtual void CreateStockAdjustments(DbTransaction Transaction)
    {
        string WarehouseId = CurrentRow.AsString("WarehouseId");
        DataRow Warehouse = Store.Provider.SelectForUpdate(Transaction, "Warehouse", "Id", WarehouseId);
        if (Warehouse == null)
            throw new TripousBusinessException("The Stock Count warehouse does not exist.");

        MemTable LineTable = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("StockCountLine"));
        if (LineTable == null)
            throw new TripousDataException("StockCountLine table is not available.");

        string UserId = Sys.GetCurrentAppUserId();
        DateTime CreatedAt = DateTime.UtcNow;
        DateTime MovementDate = CurrentRow.AsDateTime("CountDate", DateTime.Today);
        IEnumerable<DataRow> Rows = LineTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .OrderBy(Row => Row.AsString("ProductId"));
        foreach (DataRow Row in Rows)
            CreateStockAdjustment(Transaction, Row, WarehouseId, UserId, CreatedAt, MovementDate);
    }
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("DocumentTypeId", DocumentType.Id);
            Row.SetValue("WarehouseId", DataLib.GetDefaultWarehouseId());
            Row.SetValue("CountDate", DateTime.UtcNow.Date);
            Row.SetValue("StatusId", (int)TradeStatus.Draft);
        }
    }
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);

        if (fCalculationLevel > 0 || !State.In(DataMode.Insert | DataMode.Edit))
            return;

        fCalculationLevel++;
        try
        {
            if (Table == tblItem && ea.Column.ColumnName.IsSameText("WarehouseId"))
            {
                MemTable LineTable = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("StockCountLine"));
                if (LineTable != null)
                {
                    foreach (DataRow Row in LineTable.Rows)
                        LoadLineStock(Row);
                }
            }
            else if (Table.TableName.IsSameText("StockCountLine") && ea.Column.ColumnName.IsSameText("ProductId"))
            {
                LoadLineStock(ea.Row);
            }
            else if (Table.TableName.IsSameText("StockCountLine")
                     && (ea.Column.ColumnName.IsSameText("CountedQuantity") || ea.Column.ColumnName.IsSameText("UnitCost")))
            {
                CalculateLine(ea.Row);
            }
        }
        finally
        {
            fCalculationLevel--;
        }
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            CreateStockAdjustments(e.Transaction);
    }

    // ● construction
    public StockCountDataModule()
    {
    }

    // ● public
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);
        if (!IsPosting && CurrentRow != null && (TradeStatus)CurrentRow.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("A posted Stock Count cannot be saved.");

        MemTable LineTable = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("StockCountLine"));
        if (LineTable != null)
        {
            foreach (DataRow Row in LineTable.Rows)
                CalculateLine(Row);
        }
        ValidateStockCount();
    }
}
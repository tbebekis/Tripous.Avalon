/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Handles warehouse transfers, stock receipts, stock issues, and their reversals.
/// </summary>
public class StockTradeDataModule: DocumentDataModule
{
    // ● private fields
    int fCalculationLevel;

    // ● private
    /// <summary>Rounds stock quantities and amounts to four decimal places.</summary>
    decimal Round(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    /// <summary>Returns the stock transaction line table.</summary>
    MemTable GetLineTable()
    {
        MemTable Result = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("StockTradeLine"));
        if (Result == null)
            throw new TripousDataException("StockTradeLine table is not available.");
        return Result;
    }
    /// <summary>Returns a display label for a stock transaction line.</summary>
    string GetLineLabel(DataRow Row)
    {
        string Result = Row.AsString("ProductName");
        return string.IsNullOrWhiteSpace(Result) ? "Stock transaction line" : Result;
    }
    /// <summary>Returns the effective source warehouse of a line.</summary>
    string GetSourceWarehouseId(DataRow Row)
    {
        string Result = Row.AsString("WarehouseId");
        return string.IsNullOrWhiteSpace(Result) ? CurrentRow.AsString("WarehouseId") : Result;
    }
    /// <summary>Calculates primary quantity and cost amount for a line.</summary>
    void CalculateLine(DataRow Row)
    {
        if (Row == null || Row.RowState == DataRowState.Deleted || Row.RowState == DataRowState.Detached)
            return;

        decimal Quantity = Row.AsDecimal("Quantity");
        decimal UnitRatio = Row.AsDecimal("UnitRatio", 1);
        decimal PrimaryQuantity = Round(Quantity * UnitRatio);
        Row.SetValue("PrimaryQuantity", PrimaryQuantity);
        Row.SetValue("CostAmount", Round(PrimaryQuantity * Row.AsDecimal("UnitCost")));
    }
    /// <summary>Loads product snapshot and primary unit values into a line.</summary>
    void LoadProduct(DataRow Row)
    {
        string ProductId = Row.AsString("ProductId");
        if (string.IsNullOrWhiteSpace(ProductId))
            return;

        DataRow Product = Store.SelectResults("""
                                              select
                                                Product.Code,
                                                Product.Name,
                                                Product.PrimaryUnitOfMeasureId,
                                                UnitOfMeasure.Name as UnitOfMeasureName
                                              from Product
                                                left join UnitOfMeasure on UnitOfMeasure.Id = Product.PrimaryUnitOfMeasureId
                                              where Product.Id = :Id
                                              """, new Dictionary<string, object>()
        {
            ["Id"] = ProductId,
        });
        if (Product == null)
            throw new TripousBusinessException("The selected product does not exist.");

        Row.SetValue("ProductCode", Product["Code"]);
        Row.SetValue("ProductName", Product["Name"]);
        Row.SetValue("UnitOfMeasureId", Product["PrimaryUnitOfMeasureId"]);
        Row.SetValue("UnitOfMeasureName", Product["UnitOfMeasureName"]);
        Row.SetValue("UnitRatio", 1m);
        CalculateLine(Row);
    }
    /// <summary>Loads and validates the selected product unit ratio.</summary>
    void LoadUnitOfMeasure(DataRow Row)
    {
        string ProductId = Row.AsString("ProductId");
        string UnitOfMeasureId = Row.AsString("UnitOfMeasureId");
        if (string.IsNullOrWhiteSpace(ProductId) || string.IsNullOrWhiteSpace(UnitOfMeasureId))
            return;

        DataRow Product = Store.SelectResults("""
                                              select PrimaryUnitOfMeasureId
                                              from Product
                                              where Id = :Id
                                              """, new Dictionary<string, object>()
        {
            ["Id"] = ProductId,
        });
        if (Product == null)
            throw new TripousBusinessException("The selected product does not exist.");

        decimal UnitRatio = 1;
        if (!UnitOfMeasureId.IsSameText(Product.AsString("PrimaryUnitOfMeasureId")))
        {
            object Ratio = Store.SelectResult("""
                                              select Ratio
                                              from ProductUnitOfMeasure
                                              where ProductId = :ProductId
                                                and UnitId = :UnitOfMeasureId
                                                and IsActive = 1
                                              """, null, new Dictionary<string, object>()
            {
                ["ProductId"] = ProductId,
                ["UnitOfMeasureId"] = UnitOfMeasureId,
            });
            if (Sys.IsNull(Ratio))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: The selected unit of measure is not valid for the product.");
            UnitRatio = Convert.ToDecimal(Ratio);
        }

        object UnitName = Store.SelectResult("select Name from UnitOfMeasure where Id = :Id", "", new Dictionary<string, object>()
        {
            ["Id"] = UnitOfMeasureId,
        });
        Row.SetValue("UnitOfMeasureName", UnitName);
        Row.SetValue("UnitRatio", UnitRatio);
        CalculateLine(Row);
    }
    /// <summary>Validates stock transaction header and line values.</summary>
    void ValidateStockTrade()
    {
        StockTradeOperation Operation = (StockTradeOperation)CurrentRow.AsInteger("OperationTypeId");
        if (Operation != StockTradeOperation.Transfer
            && Operation != StockTradeOperation.Receipt
            && Operation != StockTradeOperation.Issue)
            throw new TripousBusinessException("A stock transaction operation is required.");

        string WarehouseId = CurrentRow.AsString("WarehouseId");
        if (string.IsNullOrWhiteSpace(WarehouseId))
            throw new TripousBusinessException("Warehouse is required.");

        string ToWarehouseId = CurrentRow.AsString("ToWarehouseId");
        if (Operation == StockTradeOperation.Transfer)
        {
            if (string.IsNullOrWhiteSpace(ToWarehouseId))
                throw new TripousBusinessException("Destination warehouse is required for a transfer.");
            if (WarehouseId.IsSameText(ToWarehouseId))
                throw new TripousBusinessException("Source and destination warehouses must be different.");
        }
        else if (!string.IsNullOrWhiteSpace(ToWarehouseId))
        {
            throw new TripousBusinessException("Destination warehouse is valid only for a transfer.");
        }

        List<DataRow> Rows = GetLineTable().Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();
        if (Rows.Count == 0)
            throw new TripousBusinessException("The Stock Transaction has no lines.");

        foreach (DataRow Row in Rows)
        {
            CalculateLine(Row);
            if (string.IsNullOrWhiteSpace(Row.AsString("ProductId")))
                throw new TripousBusinessException("Product is required on every Stock Transaction line.");
            if (string.IsNullOrWhiteSpace(Row.AsString("UnitOfMeasureId")))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Unit of measure is required.");
            if (Row.AsDecimal("UnitRatio") <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Unit ratio must be greater than zero.");
            if (Row.AsDecimal("Quantity") <= 0 || Row.AsDecimal("PrimaryQuantity") <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");
            if (Operation == StockTradeOperation.Receipt && Row.AsDecimal("UnitCost") < 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Unit cost cannot be negative.");
            if (Operation == StockTradeOperation.Transfer
                && !GetSourceWarehouseId(Row).IsSameText(WarehouseId))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Transfer lines must use the header source warehouse.");
        }
    }
    /// <summary>Locks and returns a product warehouse balance.</summary>
    DataRow GetLockedBalance(DbTransaction Transaction, string ProductId, string WarehouseId)
    {
        DataRow Key = Store.SelectResults(Transaction, """
                                                       select Id
                                                       from StockBalance
                                                       where ProductId = :ProductId
                                                         and WarehouseId = :WarehouseId
                                                       """, new Dictionary<string, object>()
        {
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
        });
        return Key == null ? null : Store.Provider.SelectForUpdate(Transaction, "StockBalance", "Id", Key["Id"]);
    }
    /// <summary>Creates one immutable stock movement and updates its balance.</summary>
    string ApplyStockMovement(DbTransaction Transaction, DataRow Row, string WarehouseId, int Direction, decimal UnitCost, string UserId, DateTime CreatedAt, DateTime MovementDate)
    {
        string ProductId = Row.AsString("ProductId");
        DataRow Balance = GetLockedBalance(Transaction, ProductId, WarehouseId);
        decimal PrimaryQuantity = Row.AsDecimal("PrimaryQuantity");
        decimal CostAmount = Round(PrimaryQuantity * UnitCost);
        decimal CurrentQuantity = Balance == null ? 0 : Balance.AsDecimal("PrimaryQuantity");
        decimal CurrentTotalCostAmount = Balance == null ? 0 : Balance.AsDecimal("TotalCostAmount");
        decimal NewQuantity = Round(CurrentQuantity + Direction * PrimaryQuantity);
        decimal NewTotalCostAmount = Round(CurrentTotalCostAmount + Direction * CostAmount);
        if (NewQuantity < 0)
        {
            DataRow Warehouse = Store.Provider.SelectForUpdate(Transaction, "Warehouse", "Id", WarehouseId);
            if (Warehouse == null)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Warehouse does not exist.");
            if (!Warehouse.AsBoolean("AllowNegativeStock"))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Stock quantity cannot become negative.");
        }
        if (NewQuantity >= 0 && NewTotalCostAmount < 0)
            throw new TripousBusinessException($"{GetLineLabel(Row)}: Stock value cannot become negative.");
        if (NewQuantity == 0)
            NewTotalCostAmount = 0;
        decimal NewAverageUnitCost = NewQuantity == 0 ? 0 : Round(NewTotalCostAmount / NewQuantity);
        string MovementId = Sys.GenId();

        Store.ExecSql(Transaction, """
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
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = MovementId,
            ["TradeTypeId"] = (int)TradeType.Warehouse,
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["MovementDate"] = MovementDate,
            ["Direction"] = Direction,
            ["Quantity"] = Row.AsDecimal("Quantity"),
            ["PrimaryQuantity"] = PrimaryQuantity,
            ["UnitOfMeasureId"] = Row.AsString("UnitOfMeasureId"),
            ["UnitOfMeasureName"] = Row.AsString("UnitOfMeasureName"),
            ["UnitRatio"] = Row.AsDecimal("UnitRatio", 1),
            ["UnitCost"] = UnitCost,
            ["CostAmount"] = CostAmount,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "StockTradeLine",
            ["SourceId"] = Row.AsString("Id"),
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("DocumentDate", DateTime.Today),
            ["CreatedAt"] = CreatedAt,
            ["CreatedBy"] = UserId,
        });

        if (Balance == null)
        {
            Store.ExecSql(Transaction, """
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
                                       """, new Dictionary<string, object>()
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
            Store.ExecSql(Transaction, """
                                       update StockBalance
                                       set PrimaryQuantity = :PrimaryQuantity,
                                           TotalCostAmount = :TotalCostAmount,
                                           AverageUnitCost = :AverageUnitCost,
                                           LastMovementDate = :LastMovementDate,
                                           LastMovementId = :LastMovementId
                                       where Id = :Id
                                       """, new Dictionary<string, object>()
            {
                ["Id"] = Balance["Id"],
                ["PrimaryQuantity"] = NewQuantity,
                ["TotalCostAmount"] = NewTotalCostAmount,
                ["AverageUnitCost"] = NewAverageUnitCost,
                ["LastMovementDate"] = MovementDate,
                ["LastMovementId"] = MovementId,
            });
        }

        return MovementId;
    }
    /// <summary>Creates all stock movements for the posting transaction.</summary>
    void CreateStockMovements(DbTransaction Transaction)
    {
        StockTradeOperation Operation = (StockTradeOperation)CurrentRow.AsInteger("OperationTypeId");
        ValidateCancellation(Transaction, Operation);
        List<DataRow> Rows = GetLineTable().Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .OrderBy(Row => Row.AsString("ProductId"))
            .ThenBy(GetSourceWarehouseId)
            .ToList();
        List<string> WarehouseIds = Rows.Select(GetSourceWarehouseId)
            .Concat(Operation == StockTradeOperation.Transfer ? [CurrentRow.AsString("ToWarehouseId")] : [])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(Id => Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
        foreach (string WarehouseId in WarehouseIds)
        {
            if (Store.Provider.SelectForUpdate(Transaction, "Warehouse", "Id", WarehouseId) == null)
                throw new TripousBusinessException("A Stock Transaction warehouse does not exist.");
        }

        string UserId = Sys.GetCurrentAppUserId();
        DateTime CreatedAt = DateTime.UtcNow;
        DateTime MovementDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);
        decimal TotalCostAmount = 0;
        bool IsReversal = !string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelsStockTradeId"));
        List<(DataRow Row, decimal UnitCost, decimal CostAmount)> PostedLines = [];
        foreach (DataRow Row in Rows)
        {
            string SourceWarehouseId = GetSourceWarehouseId(Row);
            DataRow SourceBalance = GetLockedBalance(Transaction, Row.AsString("ProductId"), SourceWarehouseId);
            decimal AverageUnitCost = SourceBalance == null ? 0 : SourceBalance.AsDecimal("AverageUnitCost");
            decimal UnitCost = IsReversal ? Row.AsDecimal("UnitCost") : AverageUnitCost;

            if (Operation == StockTradeOperation.Receipt)
            {
                UnitCost = Row.AsDecimal("UnitCost");
                ApplyStockMovement(Transaction, Row, SourceWarehouseId, 1, UnitCost, UserId, CreatedAt, MovementDate);
            }
            else if (Operation == StockTradeOperation.Issue)
            {
                ApplyStockMovement(Transaction, Row, SourceWarehouseId, -1, UnitCost, UserId, CreatedAt, MovementDate);
            }
            else
            {
                string ToWarehouseId = CurrentRow.AsString("ToWarehouseId");
                ApplyStockMovement(Transaction, Row, SourceWarehouseId, -1, UnitCost, UserId, CreatedAt, MovementDate);
                ApplyStockMovement(Transaction, Row, ToWarehouseId, 1, UnitCost, UserId, CreatedAt, MovementDate);
            }

            decimal CostAmount = Round(Row.AsDecimal("PrimaryQuantity") * UnitCost);
            PostedLines.Add((Row, UnitCost, CostAmount));
            TotalCostAmount = Round(TotalCostAmount + CostAmount);
        }

        foreach ((DataRow Row, decimal UnitCost, decimal CostAmount) PostedLine in PostedLines)
        {
            Store.ExecSql(Transaction, """
                                       update StockTradeLine
                                       set PrimaryQuantity = :PrimaryQuantity,
                                           UnitCost = :UnitCost,
                                           CostAmount = :CostAmount
                                       where Id = :Id
                                       """, new Dictionary<string, object>()
            {
                ["Id"] = PostedLine.Row.AsString("Id"),
                ["PrimaryQuantity"] = PostedLine.Row.AsDecimal("PrimaryQuantity"),
                ["UnitCost"] = PostedLine.UnitCost,
                ["CostAmount"] = PostedLine.CostAmount,
            });
        }

        Store.ExecSql(Transaction, """
                                   update StockTrade
                                   set TotalCostAmount = :TotalCostAmount
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = CurrentRow.AsString("Id"),
            ["TotalCostAmount"] = TotalCostAmount,
        });
        if (IsReversal)
            CancelSourceStockTrade(Transaction);

        foreach ((DataRow Row, decimal UnitCost, decimal CostAmount) PostedLine in PostedLines)
        {
            PostedLine.Row.SetValue("UnitCost", PostedLine.UnitCost);
            PostedLine.Row.SetValue("CostAmount", PostedLine.CostAmount);
        }
        CurrentRow.SetValue("TotalCostAmount", TotalCostAmount);
    }
    /// <summary>Validates that a cancellation fully reverses its source document.</summary>
    void ValidateCancellation(DbTransaction Transaction, StockTradeOperation Operation)
    {
        string SourceId = CurrentRow.AsString("CancelsStockTradeId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow Source = Store.Provider.SelectForUpdate(Transaction, "StockTrade", "Id", SourceId);
        if (Source == null)
            throw new TripousBusinessException("The source Stock Transaction does not exist.");
        if ((TradeStatus)Source.AsInteger("StatusId") != TradeStatus.Posted || Source.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("Only a posted, active Stock Transaction can be cancelled.");
        if (!string.IsNullOrWhiteSpace(Source.AsString("CancelsStockTradeId")))
            throw new TripousBusinessException("A Stock Cancellation cannot be cancelled.");
        if (!string.IsNullOrWhiteSpace(Source.AsString("CancelledByStockTradeId")))
            throw new TripousBusinessException("The Stock Transaction has already been cancelled.");

        StockTradeOperation SourceOperation = (StockTradeOperation)Source.AsInteger("OperationTypeId");
        StockTradeOperation ExpectedOperation = SourceOperation == StockTradeOperation.Receipt
            ? StockTradeOperation.Issue
            : SourceOperation == StockTradeOperation.Issue
                ? StockTradeOperation.Receipt
                : StockTradeOperation.Transfer;
        if (Operation != ExpectedOperation)
            throw new TripousBusinessException("The cancellation operation does not reverse the source Stock Transaction.");

        string ExpectedWarehouseId = SourceOperation == StockTradeOperation.Transfer
            ? Source.AsString("ToWarehouseId")
            : Source.AsString("WarehouseId");
        string ExpectedToWarehouseId = SourceOperation == StockTradeOperation.Transfer
            ? Source.AsString("WarehouseId")
            : "";
        if (!CurrentRow.AsString("WarehouseId").IsSameText(ExpectedWarehouseId)
            || !CurrentRow.AsString("ToWarehouseId").IsSameText(ExpectedToWarehouseId))
            throw new TripousBusinessException("The cancellation warehouses do not reverse the source Stock Transaction.");

        DataTable SourceLines = Store.Select(Transaction, """
                                                          select *
                                                          from StockTradeLine
                                                          where StockTradeId = :StockTradeId
                                                          """, new Dictionary<string, object>()
        {
            ["StockTradeId"] = SourceId,
        });
        List<DataRow> CancellationLines = GetLineTable().Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();
        if (CancellationLines.Count != SourceLines.Rows.Count)
            throw new TripousBusinessException("The cancellation must contain all source Stock Transaction lines.");

        string[] TextFields = ["ProductId", "UnitOfMeasureId"];
        string[] DecimalFields = ["UnitRatio", "Quantity", "PrimaryQuantity", "UnitCost", "CostAmount"];
        foreach (DataRow CancellationLine in CancellationLines)
        {
            string SourceLineId = CancellationLine.AsString("SourceStockTradeLineId");
            DataRow SourceLine = SourceLines.Rows.Cast<DataRow>()
                .FirstOrDefault(Row => Row.AsString("Id").IsSameText(SourceLineId));
            if (SourceLine == null)
                throw new TripousBusinessException("A cancellation source line does not exist.");
            string ExpectedLineWarehouseId = SourceOperation == StockTradeOperation.Transfer
                ? Source.AsString("ToWarehouseId")
                : SourceLine.AsString("WarehouseId");
            if (!CancellationLine.AsString("WarehouseId").IsSameText(ExpectedLineWarehouseId))
                throw new TripousBusinessException("The cancellation line must preserve the reversing warehouse.");
            foreach (string FieldName in TextFields)
            {
                if (!CancellationLine.AsString(FieldName).IsSameText(SourceLine.AsString(FieldName)))
                    throw new TripousBusinessException($"The cancellation line must preserve the source {FieldName} value.");
            }
            foreach (string FieldName in DecimalFields)
            {
                if (CancellationLine.AsDecimal(FieldName) != SourceLine.AsDecimal(FieldName))
                    throw new TripousBusinessException($"The cancellation line must preserve the source {FieldName} value.");
            }
        }
    }
    /// <summary>Marks the source stock transaction as cancelled.</summary>
    void CancelSourceStockTrade(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("CancelsStockTradeId");
        DataRow Source = Store.Provider.SelectForUpdate(Transaction, "StockTrade", "Id", SourceId);
        if (Source == null)
            throw new TripousBusinessException("The source Stock Transaction does not exist.");
        if ((TradeStatus)Source.AsInteger("StatusId") != TradeStatus.Posted || Source.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("Only a posted, active Stock Transaction can be cancelled.");
        if (!string.IsNullOrWhiteSpace(Source.AsString("CancelsStockTradeId")))
            throw new TripousBusinessException("A Stock Cancellation cannot be cancelled.");
        if (!string.IsNullOrWhiteSpace(Source.AsString("CancelledByStockTradeId")))
            throw new TripousBusinessException("The Stock Transaction has already been cancelled.");

        Store.ExecSql(Transaction, """
                                   update StockTrade
                                   set StatusId = :StatusId,
                                       IsCancelled = :IsCancelled,
                                       CancelledByStockTradeId = :CancelledByStockTradeId,
                                       CancelledAt = :CancelledAt,
                                       CancelledBy = :CancelledBy
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourceId,
            ["StatusId"] = (int)TradeStatus.Cancelled,
            ["IsCancelled"] = true,
            ["CancelledByStockTradeId"] = CurrentRow.AsString("Id"),
            ["CancelledAt"] = DateTime.UtcNow,
            ["CancelledBy"] = Sys.GetCurrentAppUserId(),
        });
    }

    // ● protected
    /// <summary>
    /// Sets defaults for stock transaction headers.
    /// </summary>
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;
        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("DocumentTypeId", DocumentType.Id);
            Row.SetValue("TradeTypeId", (int)TradeType.Warehouse);
            Row.SetValue("OperationTypeId", (int)StockTradeOperation.Transfer);
            Row.SetValue("WarehouseId", DataLib.GetDefaultWarehouseId());
            Row.SetValue("DocumentDate", DateTime.UtcNow.Date);
            Row.SetValue("StatusId", (int)TradeStatus.Draft);
        }
    }
    /// <summary>
    /// Sets default warehouse values on new stock transaction lines.
    /// </summary>
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs Args)
    {
        base.NewRowAdded(Table, Args);

        if (Table.TableName.IsSameText("StockTradeLine") && CurrentRow != null)
            Args.Row.SetValue("WarehouseId", CurrentRow["WarehouseId"]);
    }
    /// <summary>
    /// Recalculates line values when product, quantity, ratio, or cost changes.
    /// </summary>
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs Args)
    {
        base.ColumnChanged(Table, Args);

        if (fCalculationLevel > 0 || !State.In(DataMode.Insert | DataMode.Edit))
            return;

        fCalculationLevel++;
        try
        {
            if (Table.TableName.IsSameText("StockTradeLine") && Args.Column.ColumnName.IsSameText("ProductId"))
                LoadProduct(Args.Row);
            else if (Table.TableName.IsSameText("StockTradeLine") && Args.Column.ColumnName.IsSameText("UnitOfMeasureId"))
                LoadUnitOfMeasure(Args.Row);
            else if (Table.TableName.IsSameText("StockTradeLine")
                     && (Args.Column.ColumnName.IsSameText("Quantity")
                         || Args.Column.ColumnName.IsSameText("UnitRatio")
                         || Args.Column.ColumnName.IsSameText("UnitCost")))
                CalculateLine(Args.Row);
        }
        finally
        {
            fCalculationLevel--;
        }
    }
    /// <summary>
    /// Creates stock movements after the document rows are stored.
    /// </summary>
    protected override void TableSet_TransactionStageCommit(object Sender, TransactionEventArgs Args)
    {
        base.TableSet_TransactionStageCommit(Sender, Args);

        if (IsPosting && Args.Stage == TransactionStage.Post && Args.ExecTime == ExecTime.After)
            CreateStockMovements(Args.Transaction);
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public StockTradeDataModule()
    {
    }

    // ● public
    /// <summary>
    /// Validates and recalculates the stock transaction before saving or posting.
    /// </summary>
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);
        if (!IsPosting && CurrentRow != null && (TradeStatus)CurrentRow.AsInteger("StatusId") != TradeStatus.Draft)
            throw new TripousBusinessException("A posted Stock Transaction cannot be saved.");
        ValidateStockTrade();
    }
    /// <summary>
    /// Creates an unsaved reversing stock transaction.
    /// </summary>
    public virtual StockTradeDataModule CreateCancellation()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Stock Transaction is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Stock Transaction changes before creating a cancellation.");
        if ((TradeStatus)CurrentRow.AsInteger("StatusId") != TradeStatus.Posted || CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("Only a posted, active Stock Transaction can be cancelled.");
        if (!string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelsStockTradeId")))
            throw new TripousBusinessException("A Stock Cancellation cannot be cancelled.");
        if (!string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelledByStockTradeId")))
            throw new TripousBusinessException("The Stock Transaction has already been cancelled.");

        StockTradeDataModule Result = DataRegistry.CreateModule("StockTrade") as StockTradeDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Stock Transaction module.");

        StockTradeOperation SourceOperation = (StockTradeOperation)CurrentRow.AsInteger("OperationTypeId");
        Result.Insert();
        Result.CurrentRow.SetValue("OperationTypeId", SourceOperation == StockTradeOperation.Receipt
            ? (int)StockTradeOperation.Issue
            : SourceOperation == StockTradeOperation.Issue
                ? (int)StockTradeOperation.Receipt
                : (int)StockTradeOperation.Transfer);
        Result.CurrentRow.SetValue("WarehouseId", SourceOperation == StockTradeOperation.Transfer
            ? CurrentRow["ToWarehouseId"]
            : CurrentRow["WarehouseId"]);
        Result.CurrentRow.SetValue("ToWarehouseId", SourceOperation == StockTradeOperation.Transfer
            ? CurrentRow["WarehouseId"]
            : DBNull.Value);
        Result.CurrentRow.SetValue("DocumentDate", DateTime.Today);
        Result.CurrentRow.SetValue("CancelsStockTradeId", CurrentRow["Id"]);
        Result.CurrentRow.SetValue("Remarks", $"Cancellation of {CurrentRow.AsString("Code")}");

        foreach (DataRow SourceLine in GetLineTable().Rows)
        {
            if (SourceLine.RowState == DataRowState.Deleted || SourceLine.RowState == DataRowState.Detached)
                continue;

            DataRow Line = Result.GetLineTable().AddNewRow();
            string[] FieldNames =
            [
                "ProductId", "ProductCode", "ProductName", "UnitOfMeasureId", "UnitOfMeasureName",
                "UnitRatio", "Quantity", "PrimaryQuantity", "UnitCost", "CostAmount", "Remarks"
            ];
            foreach (string FieldName in FieldNames)
                Line.SetValue(FieldName, SourceLine[FieldName]);
            Line.SetValue("WarehouseId", SourceOperation == StockTradeOperation.Transfer
                ? Result.CurrentRow["WarehouseId"]
                : SourceLine["WarehouseId"]);
            Line.SetValue("SourceStockTradeLineId", SourceLine["Id"]);
        }

        return Result;
    }
}

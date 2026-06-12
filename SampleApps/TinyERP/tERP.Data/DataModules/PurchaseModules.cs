/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class PurchaseDataModule: TradeDataModule
{
    // ● protected
    protected virtual string GetDefaultId(string ConfigValue, Func<string> DefaultProvider) => !string.IsNullOrWhiteSpace(ConfigValue) ? ConfigValue : DefaultProvider();
    protected override void ValidateLine(DataRow Row, List<string> Errors)
    {
        base.ValidateLine(Row, Errors);

        string LineLabel = GetLineLabel(Row);
        if (!AppDefaultProperties.Purchase.AllowZeroUnitPrice && Row.AsDecimal("UnitPrice") == 0)
            Errors.Add($"{LineLabel}: Unit price must be greater than zero.");
    }
    protected virtual void CopyCommonValues(DataRow Source, DataRow Dest, IEnumerable<string> ExcludedFields)
    {
        HashSet<string> Excluded = new(ExcludedFields, StringComparer.OrdinalIgnoreCase);

        foreach (DataColumn Column in Dest.Table.Columns)
        {
            if (!Excluded.Contains(Column.ColumnName) && Source.Table.Columns.Contains(Column.ColumnName))
                Dest.SetValue(Column.ColumnName, Source[Column.ColumnName]);
        }
    }
    protected virtual PurchaseDataModule CreateTransformedDocument(string TargetModuleName)
    {
        PurchaseDataModule Result = DataRegistry.CreateModule(TargetModuleName) as PurchaseDataModule;
        if (Result == null)
            throw new TripousDataException($"Module '{TargetModuleName}' is not a purchase document module.");

        Result.IsTransforming = true;
        try
        {
            Result.Insert();

            string[] HeaderExcludedFields =
            [
                "Id", "DocumentTypeId", "Code", "TradeStatusId", "TradeDate", "PostingDate",
                "SourceId", "CancelsTradeId", "CancelledByTradeId",
                "LinesAmount", "NetAmount", "TaxAmount", "TotalAmount",
                "IsLocked", "IsCancelled",
                "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy",
                "PostedAt", "PostedBy", "CancelledAt", "CancelledBy"
            ];
            CopyCommonValues(CurrentRow, Result.CurrentRow, HeaderExcludedFields);
            Result.CurrentRow.SetValue("SourceId", CurrentRow["Id"]);

            MemTable SourceLineTable = FindItemTable("TradeLine");
            MemTable TargetLineTable = Result.FindItemTable("TradeLine");
            if (SourceLineTable == null || TargetLineTable == null)
                throw new TripousDataException("TradeLine table is not available.");

            string[] LineExcludedFields =
            [
                "Id", "TradeId", "Quantity", "PrimaryUnitQuantity",
                "ReservedQuantity", "ExecutedQuantity",
                "GrossAmount", "DiscountAmount", "NetUnitPrice", "NetAmount",
                "DocumentDiscountAmount", "TaxAmount", "TotalAmount",
                "TaxPercent", "IsTaxExempt", "IsReverseCharge",
                "SourceTradeLineId"
            ];

            foreach (DataRow SourceLine in SourceLineTable.Rows)
            {
                if (SourceLine.RowState == DataRowState.Deleted || SourceLine.RowState == DataRowState.Detached)
                    continue;

                decimal Quantity = SourceLine.AsDecimal("Quantity") - SourceLine.AsDecimal("ExecutedQuantity");
                if (Quantity <= 0)
                    continue;

                DataRow TargetLine = TargetLineTable.AddNewRow();
                CopyCommonValues(SourceLine, TargetLine, LineExcludedFields);
                TargetLine.SetValue("Quantity", Quantity);
                TargetLine.SetValue("DiscountPercent", SourceLine.AsDecimal("DiscountPercent"));
                TargetLine.SetValue("SourceTradeLineId", SourceLine["Id"]);
            }

            if (TargetLineTable.Rows.Count == 0)
                throw new TripousBusinessException("The source document has no remaining quantity to transform.");

            Result.Calculate(null, "DiscountPercent", "DiscountPercent");
            return Result;
        }
        catch
        {
            Result.Cancel();
            throw;
        }
        finally
        {
            Result.IsTransforming = false;
        }
    }
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("WarehouseId", GetDefaultId(AppDefaultProperties.Purchase.WarehouseId, DataLib.GetDefaultWarehouseId));
            Row.SetValue("CostCenterId", GetDefaultId(AppDefaultProperties.Purchase.CostCenterId, DataLib.GetDefaultPurchaseCostCenterId));
            Row.SetValue("BranchId", GetDefaultId(AppDefaultProperties.Purchase.BranchId, DataLib.GetDefaultBranchId));
            Row.SetValue("PriceListTypeId", GetDefaultId(AppDefaultProperties.Purchase.PriceListTypeId, DataLib.GetDefaultPriceListTypeId));
            Row.SetValue("CurrencyId", GetDefaultId(AppDefaultProperties.Purchase.CurrencyId, DataLib.GetDefaultCurrencyId));
            Row.SetValue("PaymentMethodId", GetDefaultId(AppDefaultProperties.Purchase.PaymentMethodId, DataLib.GetDefaultPaymentMethodId));
            Row.SetValue("PaymentTermId", GetDefaultId(AppDefaultProperties.Purchase.PaymentTermId, DataLib.GetDefaultPaymentTermId));
            Row.SetValue("TaxBusinessGroupId", GetDefaultId(AppDefaultProperties.Purchase.TaxBusinessGroupId, DataLib.GetDefaultTaxBusinessGroupId));
            Row.SetValue("OriginTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Purchase.OriginTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
            Row.SetValue("DestinationTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Purchase.DestinationTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
        }
    }
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (!IsTransforming && IsTradeLineTable(Table))
            ea.Row.SetValue("Quantity", AppDefaultProperties.Purchase.DefaultQuantity);
    }

    // ● construction
    public PurchaseDataModule()
    {
    }
}

public class PurchaseOrderDataModule: PurchaseDataModule
{
    // ● protected
    protected virtual void CheckCanCreateDeliveryNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Order is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Order changes before creating a Purchase Delivery Note.");
        if ((TradeStatus)CurrentRow.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Purchase Orders can create a Purchase Delivery Note.");
        if (CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Purchase Order cannot create a Purchase Delivery Note.");
    }

    // ● construction
    public PurchaseOrderDataModule()
    {
    }

    // ● public
    public virtual PurchaseDeliveryNoteDataModule CreateDeliveryNote()
    {
        CheckCanCreateDeliveryNote();
        PurchaseDeliveryNoteDataModule Result = CreateTransformedDocument("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Delivery Note module.");
        return Result;
    }
}

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

public class PurchaseDeliveryNoteDataModule: PurchaseStockDataModule
{
    // ● protected
    protected virtual void CheckCanCreateReturn()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Purchase Delivery Note is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Purchase Delivery Note changes before creating a Purchase Return.");
        if ((TradeStatus)CurrentRow.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Purchase Delivery Notes can create a Purchase Return.");
        if (CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Purchase Delivery Note cannot create a Purchase Return.");
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
        PurchaseReturnDataModule Result = CreateTransformedDocument("PurchaseReturn") as PurchaseReturnDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Purchase Return module.");
        return Result;
    }
}

public class PurchaseInvoiceDataModule: PurchaseDataModule
{
    public PurchaseInvoiceDataModule()
    {
    }
}

public class PurchaseCreditNoteDataModule: PurchaseDataModule
{
    public PurchaseCreditNoteDataModule()
    {
    }
}

public class PurchaseReturnDataModule: PurchaseStockDataModule
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
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Source Purchase Delivery Note line is required.");

            decimal Quantity = Row.AsDecimal("Quantity");
            if (Quantity <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");

            Result.TryGetValue(SourceTradeLineId, out decimal TotalQuantity);
            Result[SourceTradeLineId] = TotalQuantity + Quantity;
        }

        if (Result.Count == 0)
            throw new TripousBusinessException("The Purchase Return has no lines.");

        return Result;
    }
    protected virtual void UpdateSourceReturnedQuantities(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("SourceId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow SourceDelivery = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceDelivery == null)
            throw new TripousBusinessException("The source Purchase Delivery Note does not exist.");
        if ((TradeStatus)SourceDelivery.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Purchase Delivery Notes can be returned.");
        if (SourceDelivery.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Purchase Delivery Note cannot be returned.");

        Dictionary<string, decimal> Quantities = GetReturnedQuantities();
        foreach (KeyValuePair<string, decimal> Entry in Quantities.OrderBy(Item => Item.Key))
        {
            DataRow SourceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (SourceLine == null || !SourceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException("A source Purchase Delivery Note line does not exist.");

            decimal ReceivedQuantity = SourceLine.AsDecimal("Quantity");
            decimal ReturnedQuantity = SourceLine.AsDecimal("ExecutedQuantity");
            decimal RemainingQuantity = ReceivedQuantity - ReturnedQuantity;
            if (Entry.Value > RemainingQuantity)
                throw new TripousBusinessException($"Return quantity {Entry.Value} exceeds remaining quantity {RemainingQuantity}.");

            string SqlText = """
                             update TradeLine
                             set ExecutedQuantity = :ExecutedQuantity
                             where Id = :Id
                             """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Entry.Key,
                ["ExecutedQuantity"] = ReturnedQuantity + Entry.Value,
            });
        }
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
            UpdateSourceReturnedQuantities(e.Transaction);

        base.TableSet_TransactionStageCommit(sender, e);
    }

    // ● construction
    public PurchaseReturnDataModule()
    {
    }
}

public class PurchaseCancellationDataModule: PurchaseDataModule
{
    public PurchaseCancellationDataModule()
    {
    }
}

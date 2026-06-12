/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;
 
public class SalesDataModule: TradeDataModule
{
    // ● protected
    /// <summary>
    /// Copies common column values except for the specified fields.
    /// </summary>
    protected virtual void CopyCommonValues(DataRow Source, DataRow Dest, IEnumerable<string> ExcludedFields)
    {
        HashSet<string> Excluded = new(ExcludedFields, StringComparer.OrdinalIgnoreCase);

        foreach (DataColumn Column in Dest.Table.Columns)
        {
            if (!Excluded.Contains(Column.ColumnName) && Source.Table.Columns.Contains(Column.ColumnName))
                Dest.SetValue(Column.ColumnName, Source[Column.ColumnName]);
        }
    }
    /// <summary>
    /// Creates an unsaved sales document from the current document.
    /// </summary>
    protected virtual SalesDataModule CreateTransformedDocument(string TargetModuleName)
    {
        SalesDataModule Result = DataRegistry.CreateModule(TargetModuleName) as SalesDataModule;
        if (Result == null)
            throw new TripousDataException($"Module '{TargetModuleName}' is not a sales document module.");

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
    /// <summary>
    /// Returns the configured identifier or resolves the current application default.
    /// </summary>
    protected virtual string GetDefaultId(string ConfigValue, Func<string> DefaultProvider) => !string.IsNullOrWhiteSpace(ConfigValue) ? ConfigValue : DefaultProvider();
    /// <summary>
    /// Validates sales pricing before commit.
    /// </summary>
    protected override void ValidateLine(DataRow Row, List<string> Errors)
    {
        base.ValidateLine(Row, Errors);

        string LineLabel = GetLineLabel(Row);
        if (!AppDefaultProperties.Sales.AllowZeroUnitPrice && Row.AsDecimal("UnitPrice") == 0)
            Errors.Add($"{LineLabel}: Unit price must be greater than zero.");
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("WarehouseId", GetDefaultId(AppDefaultProperties.Sales.WarehouseId, DataLib.GetDefaultWarehouseId));
            Row.SetValue("CostCenterId", GetDefaultId(AppDefaultProperties.Sales.CostCenterId, DataLib.GetDefaultSalesCostCenterId));
            Row.SetValue("BranchId", GetDefaultId(AppDefaultProperties.Sales.BranchId, DataLib.GetDefaultBranchId));
            Row.SetValue("PriceListTypeId", GetPriceListTypeId());
            Row.SetValue("CurrencyId", GetDefaultId(AppDefaultProperties.Sales.CurrencyId, DataLib.GetDefaultCurrencyId));
            Row.SetValue("PaymentMethodId", GetDefaultId(AppDefaultProperties.Sales.PaymentMethodId, DataLib.GetDefaultPaymentMethodId));
            Row.SetValue("PaymentTermId", GetDefaultId(AppDefaultProperties.Sales.PaymentTermId, DataLib.GetDefaultPaymentTermId));

            Row.SetValue("TaxBusinessGroupId", GetDefaultId(AppDefaultProperties.Sales.TaxBusinessGroupId, DataLib.GetDefaultTaxBusinessGroupId));
            Row.SetValue("OriginTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Sales.OriginTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
            Row.SetValue("DestinationTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Sales.DestinationTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
        }
    }
    /// <summary>
    /// Sets sales defaults on a newly added commercial document line.
    /// </summary>
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (!IsTransforming && IsTradeLineTable(Table))
            ea.Row.SetValue("Quantity", AppDefaultProperties.Sales.DefaultQuantity);
    }

    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);
        if (!IsCopyingPersonAddresses 
            &&Table == tblItem 
            && !IsTransforming 
            && State.In(DataMode.Insert | DataMode.Edit) 
            && "PersonId".IsSameText(ea.Column.ColumnName))
        {
            CopyPersonAddresses(ea.Row);
        }
    }
    protected virtual void CopyPersonAddresses(DataRow Row)
    {
        List<PersonAddress> AddressList = DataLib.LoadPersonAddressList(Row.AsString("PersonId"));

        PersonAddress BillingAddress = AddressList.FirstOrDefault(x => x.AddressType == AddressType.Billing && x.IsDefault)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Billing)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main && x.IsDefault)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main)
                                       ?? AddressList.FirstOrDefault(x => x.IsDefault)
                                       ?? AddressList.FirstOrDefault();

        PersonAddress ShippingAddress = AddressList.FirstOrDefault(x => x.AddressType == AddressType.Shipping && x.IsDefault)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Shipping)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main && x.IsDefault)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main)
                                        ?? AddressList.FirstOrDefault(x => x.IsDefault)
                                        ?? AddressList.FirstOrDefault();

        IsCopyingPersonAddresses = true;
        try
        {
            Row.SetValue("BillingName", BillingAddress != null ? BillingAddress.Name : DBNull.Value);
            Row.SetValue("BillingAddressLine1", BillingAddress != null ? BillingAddress.AddressLine1 : DBNull.Value);
            Row.SetValue("BillingAddressLine2", BillingAddress != null ? BillingAddress.AddressLine2 : DBNull.Value);
            Row.SetValue("BillingCity", BillingAddress != null ? BillingAddress.City : DBNull.Value);
            Row.SetValue("BillingRegion", BillingAddress != null ? BillingAddress.Region : DBNull.Value);
            Row.SetValue("BillingPostalCode", BillingAddress != null ? BillingAddress.PostalCode : DBNull.Value);
            Row.SetValue("BillingCountryId", BillingAddress != null ? BillingAddress.CountryId : DBNull.Value);

            Row.SetValue("ShippingName", ShippingAddress != null ? ShippingAddress.Name : DBNull.Value);
            Row.SetValue("ShippingAddressLine1", ShippingAddress != null ? ShippingAddress.AddressLine1 : DBNull.Value);
            Row.SetValue("ShippingAddressLine2", ShippingAddress != null ? ShippingAddress.AddressLine2 : DBNull.Value);
            Row.SetValue("ShippingCity", ShippingAddress != null ? ShippingAddress.City : DBNull.Value);
            Row.SetValue("ShippingRegion", ShippingAddress != null ? ShippingAddress.Region : DBNull.Value);
            Row.SetValue("ShippingPostalCode", ShippingAddress != null ? ShippingAddress.PostalCode : DBNull.Value);
            Row.SetValue("ShippingCountryId", ShippingAddress != null ? ShippingAddress.CountryId : DBNull.Value);
        }
        finally
        {
            IsCopyingPersonAddresses = false;
        }

        ResolvePrices();
        Calculate();
    }

    // ● construction
    public SalesDataModule()
    {
    }
}

public class SalesOrderDataModule: SalesDataModule
{
    // ● protected
    protected virtual void CheckCanCreateDeliveryNote()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No Sales Order is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the Sales Order changes before creating a Sales Delivery Note.");
        if ((TradeStatus)CurrentRow.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted Sales Orders can create a Sales Delivery Note.");
        if (CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled Sales Order cannot create a Sales Delivery Note.");
    }

    // ● construction
    public SalesOrderDataModule()
    {
    }

    // ● public
    public virtual SalesDeliveryNoteDataModule CreateDeliveryNote()
    {
        CheckCanCreateDeliveryNote();
        SalesDeliveryNoteDataModule Result = CreateTransformedDocument("SalesDeliveryNote") as SalesDeliveryNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create a Sales Delivery Note module.");
        return Result;
    }
}

public class SalesDeliveryNoteDataModule: SalesDataModule
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
            UpdateSourceExecutedQuantities(e.Transaction);
        }
    }

    // ● construction
    public SalesDeliveryNoteDataModule()
    {
    }
}

public class SalesInvoiceDataModule: SalesDataModule
{
    // ● construction
    public SalesInvoiceDataModule()
    {
    }
}

public class SalesCreditNoteDataModule: SalesDataModule
{
    // ● construction
    public SalesCreditNoteDataModule()
    {
    }
}

public class SalesReturnDataModule: SalesDataModule
{
    // ● construction
    public SalesReturnDataModule()
    {
    }
}

public class SalesCancellationDataModule: SalesDataModule
{
    // ● construction
    public SalesCancellationDataModule()
    {
    }
}

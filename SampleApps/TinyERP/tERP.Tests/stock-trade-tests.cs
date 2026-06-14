namespace tERP.Tests;

/// <summary>
/// Tests stock transaction posting, costing, rollback, and cancellation.
/// </summary>
[Collection(TestCollection.Name)]
public class StockTradeTests
{
    // ● private fields
    readonly TestDatabaseFixture fFixture;

    // ● private
    DataRow GetProduct(string Name)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                     select Id, Name
                                                     from Product
                                                     where Name = :Name
                                                     """, new Dictionary<string, object>()
        {
            ["Name"] = Name,
        });
        if (Result == null)
            throw new TripousDataException($"Product not found: {Name}");
        return Result;
    }
    string GetWarehouseId(string Name)
    {
        object Result = fFixture.Store.SelectResult("select Id from Warehouse where Name = :Name", null, new Dictionary<string, object>()
        {
            ["Name"] = Name,
        });
        if (Sys.IsNull(Result))
            throw new TripousDataException($"Warehouse not found: {Name}");
        return Result.ToString();
    }
    string GetUnitOfMeasureId(string Code)
    {
        object Result = fFixture.Store.SelectResult("select Id from UnitOfMeasure where Code = :Code", null, new Dictionary<string, object>()
        {
            ["Code"] = Code,
        });
        if (Sys.IsNull(Result))
            throw new TripousDataException($"Unit of measure not found: {Code}");
        return Result.ToString();
    }
    DataRow GetStockBalance(string ProductId, string WarehouseId)
    {
        return fFixture.Store.SelectResults("""
                                            select *
                                            from StockBalance
                                            where ProductId = :ProductId
                                              and WarehouseId = :WarehouseId
                                            """, new Dictionary<string, object>()
        {
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
        });
    }
    DataTable GetStockMovements(string SourceId)
    {
        return fFixture.Store.Select("""
                                     select *
                                     from StockMovement
                                     where SourceId = :SourceId
                                     order by Direction, WarehouseId
                                     """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataRow GetStockTrade(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from StockTrade where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Stock Transaction not found: {Id}");
        return Result;
    }
    void SetStockBalance(string ProductId, string WarehouseId, decimal PrimaryQuantity, decimal AverageUnitCost)
    {
        decimal TotalCostAmount = PrimaryQuantity * AverageUnitCost;
        int AffectedRows = fFixture.Store.ExecSql("""
                                                  update StockBalance
                                                  set PrimaryQuantity = :PrimaryQuantity,
                                                      TotalCostAmount = :TotalCostAmount,
                                                      AverageUnitCost = :AverageUnitCost,
                                                      LastMovementDate = null,
                                                      LastMovementId = null
                                                  where ProductId = :ProductId
                                                    and WarehouseId = :WarehouseId
                                                  """, new Dictionary<string, object>()
        {
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["PrimaryQuantity"] = PrimaryQuantity,
            ["TotalCostAmount"] = TotalCostAmount,
            ["AverageUnitCost"] = AverageUnitCost,
        });
        if (AffectedRows > 0)
            return;

        fFixture.Store.ExecSql("""
                               insert into StockBalance
                               (
                                 Id, ProductId, WarehouseId,
                                 PrimaryQuantity, TotalCostAmount, AverageUnitCost
                               )
                               values
                               (
                                 :Id, :ProductId, :WarehouseId,
                                 :PrimaryQuantity, :TotalCostAmount, :AverageUnitCost
                               )
                               """, new Dictionary<string, object>()
        {
            ["Id"] = Sys.GenId(),
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["PrimaryQuantity"] = PrimaryQuantity,
            ["TotalCostAmount"] = TotalCostAmount,
            ["AverageUnitCost"] = AverageUnitCost,
        });
    }
    StockTradeDataModule CreateStockTrade(StockTradeOperation Operation, string WarehouseName, string ToWarehouseName, string ProductName, decimal Quantity, decimal UnitCost)
    {
        StockTradeDataModule Module = DataRegistry.CreateModule("StockTrade") as StockTradeDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Stock Transaction module.");

        Module.Insert();
        Module.CurrentRow.SetValue("OperationTypeId", (int)Operation);
        Module.CurrentRow.SetValue("WarehouseId", GetWarehouseId(WarehouseName));
        Module.CurrentRow.SetValue("ToWarehouseId", string.IsNullOrWhiteSpace(ToWarehouseName) ? DBNull.Value : GetWarehouseId(ToWarehouseName));
        Module.CurrentRow.SetValue("DocumentDate", DateTime.Today);
        DataRow Line = Module.GetTable("StockTradeLine").AddNewRow();
        Line.SetValue("ProductId", GetProduct(ProductName)["Id"]);
        Line.SetValue("Quantity", Quantity);
        Line.SetValue("UnitCost", UnitCost);
        Module.Commit();
        return Module;
    }

    // ● construction
    public StockTradeTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    /// <summary>Verifies that receipts use the entered unit cost.</summary>
    [Fact]
    public void PostingReceiptUsesEnteredCost()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 100m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Receipt, "Main Warehouse", null, "Orange Juice", 5m, 200m);
        string LineId = Module.GetTable("StockTradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        DataRow Movement = GetStockMovements(LineId).Rows[0];
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(200m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(15m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(2000m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(133.3333m, Balance.AsDecimal("AverageUnitCost"));
    }
    /// <summary>Verifies conversion from an alternative unit to primary units.</summary>
    [Fact]
    public void PostingReceiptUsesProductUnitRatio()
    {
        DataRow Product = GetProduct("Coffee Machine");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        StockTradeDataModule Module = DataRegistry.CreateModule("StockTrade") as StockTradeDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Stock Transaction module.");
        Module.Insert();
        Module.CurrentRow.SetValue("OperationTypeId", (int)StockTradeOperation.Receipt);
        Module.CurrentRow.SetValue("WarehouseId", WarehouseId);
        Module.CurrentRow.SetValue("ToWarehouseId", DBNull.Value);
        DataRow Line = Module.GetTable("StockTradeLine").AddNewRow();
        Line.SetValue("ProductId", Product["Id"]);
        Line.SetValue("UnitOfMeasureId", GetUnitOfMeasureId("BX"));
        Line.SetValue("Quantity", 2m);
        Line.SetValue("UnitCost", 10m);
        Module.Commit();
        string LineId = Module.GetTable("StockTradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovements(LineId).Rows[0];
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(12m, Movement.AsDecimal("UnitRatio"));
        Assert.Equal(2m, Movement.AsDecimal("Quantity"));
        Assert.Equal(24m, Movement.AsDecimal("PrimaryQuantity"));
        Assert.Equal(240m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(24m, Balance.AsDecimal("PrimaryQuantity"));
    }
    /// <summary>Verifies that issues use the current moving-average cost.</summary>
    [Fact]
    public void PostingIssueUsesCurrentAverageCost()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 100m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Issue, "Main Warehouse", null, "Orange Juice", 4m, 999m);
        string LineId = Module.GetTable("StockTradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        DataRow Movement = GetStockMovements(LineId).Rows[0];
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(100m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(6m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(600m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(100m, Balance.AsDecimal("AverageUnitCost"));
    }
    /// <summary>Verifies that transfers preserve the source stock cost.</summary>
    [Fact]
    public void PostingTransferPreservesTransferredCost()
    {
        DataRow Product = GetProduct("Orange Juice");
        string MainWarehouseId = GetWarehouseId("Main Warehouse");
        string RetailWarehouseId = GetWarehouseId("Retail Store");
        SetStockBalance(Product.AsString("Id"), MainWarehouseId, 10m, 100m);
        SetStockBalance(Product.AsString("Id"), RetailWarehouseId, 2m, 50m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Transfer, "Main Warehouse", "Retail Store", "Orange Juice", 4m, 0m);
        string LineId = Module.GetTable("StockTradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataTable Movements = GetStockMovements(LineId);
        DataRow MainBalance = GetStockBalance(Product.AsString("Id"), MainWarehouseId);
        DataRow RetailBalance = GetStockBalance(Product.AsString("Id"), RetailWarehouseId);
        Assert.Equal(2, Movements.Rows.Count);
        Assert.All(Movements.Rows.Cast<DataRow>(), Row => Assert.Equal(100m, Row.AsDecimal("UnitCost")));
        Assert.Equal(6m, MainBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(600m, MainBalance.AsDecimal("TotalCostAmount"));
        Assert.Equal(6m, RetailBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(500m, RetailBalance.AsDecimal("TotalCostAmount"));
        Assert.Equal(83.3333m, RetailBalance.AsDecimal("AverageUnitCost"));
    }
    /// <summary>Verifies that invalid negative stock rolls back posting.</summary>
    [Fact]
    public void PostingIssueRejectsNegativeStockAndRollsBack()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 3m, 100m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Issue, "Main Warehouse", null, "Orange Juice", 4m, 0m);
        string StockTradeId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("StockTradeLine").Rows[0].AsString("Id");

        Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Equal((int)TradeStatus.Draft, GetStockTrade(StockTradeId).AsInteger("StatusId"));
        Assert.Empty(GetStockMovements(LineId).Rows.Cast<DataRow>());
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(3m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(300m, Balance.AsDecimal("TotalCostAmount"));
    }
    /// <summary>Verifies that transfer cancellation restores both warehouse balances.</summary>
    [Fact]
    public void PostingTransferCancellationReversesMovements()
    {
        DataRow Product = GetProduct("Orange Juice");
        string MainWarehouseId = GetWarehouseId("Main Warehouse");
        string RetailWarehouseId = GetWarehouseId("Retail Store");
        SetStockBalance(Product.AsString("Id"), MainWarehouseId, 10m, 100m);
        SetStockBalance(Product.AsString("Id"), RetailWarehouseId, 2m, 50m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Transfer, "Main Warehouse", "Retail Store", "Orange Juice", 4m, 0m);
        string StockTradeId = Module.CurrentRow.AsString("Id");
        Module.Post();
        StockTradeDataModule Cancellation = Module.CreateCancellation();
        string CancellationId = Cancellation.CurrentRow.AsString("Id");

        Cancellation.Post();

        DataRow Source = GetStockTrade(StockTradeId);
        DataRow MainBalance = GetStockBalance(Product.AsString("Id"), MainWarehouseId);
        DataRow RetailBalance = GetStockBalance(Product.AsString("Id"), RetailWarehouseId);
        Assert.Equal((int)TradeStatus.Cancelled, Source.AsInteger("StatusId"));
        Assert.True(Source.AsBoolean("IsCancelled"));
        Assert.Equal(CancellationId, Source.AsString("CancelledByStockTradeId"));
        Assert.Equal(10m, MainBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1000m, MainBalance.AsDecimal("TotalCostAmount"));
        Assert.Equal(2m, RetailBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(100m, RetailBalance.AsDecimal("TotalCostAmount"));
    }
    /// <summary>Verifies that a modified cancellation is rejected atomically.</summary>
    [Fact]
    public void PostingModifiedCancellationIsRejected()
    {
        DataRow Product = GetProduct("Orange Juice");
        string MainWarehouseId = GetWarehouseId("Main Warehouse");
        string RetailWarehouseId = GetWarehouseId("Retail Store");
        SetStockBalance(Product.AsString("Id"), MainWarehouseId, 10m, 100m);
        SetStockBalance(Product.AsString("Id"), RetailWarehouseId, 2m, 50m);
        StockTradeDataModule Module = CreateStockTrade(StockTradeOperation.Transfer, "Main Warehouse", "Retail Store", "Orange Juice", 4m, 0m);
        string StockTradeId = Module.CurrentRow.AsString("Id");
        Module.Post();
        StockTradeDataModule Cancellation = Module.CreateCancellation();
        Cancellation.GetTable("StockTradeLine").Rows[0].SetValue("Quantity", 3m);

        Assert.Throws<TripousBusinessException>(() => Cancellation.Post());

        DataRow Source = GetStockTrade(StockTradeId);
        DataRow MainBalance = GetStockBalance(Product.AsString("Id"), MainWarehouseId);
        DataRow RetailBalance = GetStockBalance(Product.AsString("Id"), RetailWarehouseId);
        Assert.Equal((int)TradeStatus.Posted, Source.AsInteger("StatusId"));
        Assert.False(Source.AsBoolean("IsCancelled"));
        Assert.Equal(6m, MainBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(6m, RetailBalance.AsDecimal("PrimaryQuantity"));
    }
}

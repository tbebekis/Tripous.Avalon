namespace tERP.Tests;

[Collection(TestCollection.Name)]
public class StockCountTests
{
    // ● private fields
    readonly TestDatabaseFixture fFixture;

    // ● private
    DataRow GetProduct(string Name)
    {
        string SqlText = """
                         select
                           Product.Id,
                           Product.Code,
                           Product.Name,
                           Product.PrimaryUnitOfMeasureId
                         from Product
                         where Product.Name = :Name
                         """;
        DataRow Result = fFixture.Store.SelectResults(SqlText, new Dictionary<string, object>()
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
    DataRow GetStockMovement(string SourceId)
    {
        return fFixture.Store.SelectResults("select * from StockMovement where SourceId = :SourceId", new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataRow GetStockCount(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from StockCount where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Stock Count not found: {Id}");
        return Result;
    }
    DataRow GetStockCountLine(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from StockCountLine where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Stock Count line not found: {Id}");
        return Result;
    }
    void SetStockBalance(string ProductId, string WarehouseId, decimal PrimaryQuantity, decimal AverageUnitCost)
    {
        decimal TotalCostAmount = PrimaryQuantity * AverageUnitCost;
        string SqlText = """
                         update StockBalance
                         set PrimaryQuantity = :PrimaryQuantity,
                             TotalCostAmount = :TotalCostAmount,
                             AverageUnitCost = :AverageUnitCost,
                             LastMovementDate = null,
                             LastMovementId = null
                         where ProductId = :ProductId
                           and WarehouseId = :WarehouseId
                         """;
        int AffectedRows = fFixture.Store.ExecSql(SqlText, new Dictionary<string, object>()
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
    StockCountDataModule CreateStockCount(string ProductName, decimal CountedQuantity, decimal UnitCost)
    {
        return CreateStockCount((ProductName, CountedQuantity, UnitCost));
    }
    StockCountDataModule CreateStockCount(params (string ProductName, decimal CountedQuantity, decimal UnitCost)[] Lines)
    {
        string WarehouseId = GetWarehouseId("Main Warehouse");
        StockCountDataModule Module = DataRegistry.CreateModule("StockCount") as StockCountDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Stock Count module.");

        Module.Insert();
        Module.CurrentRow.SetValue("WarehouseId", WarehouseId);
        Module.CurrentRow.SetValue("CountDate", DateTime.Today);
        foreach ((string ProductName, decimal CountedQuantity, decimal UnitCost) Entry in Lines)
        {
            DataRow Product = GetProduct(Entry.ProductName);
            DataRow Line = Module.GetTable("StockCountLine").AddNewRow();
            Line.SetValue("ProductId", Product["Id"]);
            Line.SetValue("CountedQuantity", Entry.CountedQuantity);
            Line.SetValue("UnitCost", Entry.UnitCost);
        }
        Module.Commit();
        return Module;
    }

    // ● construction
    public StockCountTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    [Fact]
    public void PostingInitialStockCountCreatesStock()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        StockCountDataModule Module = CreateStockCount("Orange Juice", 10m, 25m);
        string StockCountId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("StockCountLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow StockCount = GetStockCount(StockCountId);
        DataRow Line = GetStockCountLine(LineId);
        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal((int)TradeStatus.Posted, StockCount.AsInteger("StatusId"));
        Assert.False(StockCount.AsString("Code").StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(10m, Line.AsDecimal("DifferenceQuantity"));
        Assert.Equal(250m, Line.AsDecimal("DifferenceCostAmount"));
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(10m, Movement.AsDecimal("Quantity"));
        Assert.Equal(25m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(10m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(250m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(25m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void JsonCalculateUpdatesStockCountLineDifference()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 25m);
        StockCountDataModule Source = DataRegistry.CreateModule("StockCount") as StockCountDataModule;
        if (Source == null)
            throw new TripousDataException("Cannot create the Stock Count module.");
        Source.Insert();
        Source.CurrentRow.SetValue("WarehouseId", WarehouseId);
        DataRow Line = Source.GetTable("StockCountLine").AddNewRow();
        Line.SetValue("ProductId", Product["Id"]);
        Line.SetValue("CountedQuantity", 12m);

        StockCountDataModule Target = DataRegistry.CreateModule("StockCount") as StockCountDataModule;
        if (Target == null)
            throw new TripousDataException("Cannot create the Stock Count module.");
        Target.JsonCalculate(new JsonDataModule(Source), "StockCountLine", "CountedQuantity", Line.AsString("Id"));
        DataRow CalculatedLine = Target.GetTable("StockCountLine").Rows[0];

        Assert.Equal(10m, CalculatedLine.AsDecimal("SystemQuantity"));
        Assert.Equal(2m, CalculatedLine.AsDecimal("DifferenceQuantity"));
        Assert.Equal(50m, CalculatedLine.AsDecimal("DifferenceCostAmount"));
    }
    [Fact]
    public void PostingStockCountDecreaseUsesCurrentAverageCost()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 100m);
        StockCountDataModule Module = CreateStockCount("Orange Juice", 6m, 999m);
        string LineId = Module.GetTable("StockCountLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Line = GetStockCountLine(LineId);
        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(-4m, Line.AsDecimal("DifferenceQuantity"));
        Assert.Equal(-400m, Line.AsDecimal("DifferenceCostAmount"));
        Assert.Equal(100m, Line.AsDecimal("UnitCost"));
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(4m, Movement.AsDecimal("Quantity"));
        Assert.Equal(400m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(6m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(600m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(100m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PostingStockCountRejectsChangedStock()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 100m);
        StockCountDataModule Module = CreateStockCount("Orange Juice", 8m, 100m);
        string StockCountId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("StockCountLine").Rows[0].AsString("Id");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 12m, 100m);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Contains("stock changed after the count was entered", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Draft, GetStockCount(StockCountId).AsInteger("StatusId"));
        Assert.Null(GetStockMovement(LineId));
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(12m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1200m, Balance.AsDecimal("TotalCostAmount"));
    }
    [Fact]
    public void PostingStockCountWithZeroDifferenceCreatesNoMovement()
    {
        DataRow Product = GetProduct("Orange Juice");
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 100m);
        StockCountDataModule Module = CreateStockCount("Orange Juice", 10m, 100m);
        string StockCountId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("StockCountLine").Rows[0].AsString("Id");

        Module.Post();

        Assert.Equal((int)TradeStatus.Posted, GetStockCount(StockCountId).AsInteger("StatusId"));
        Assert.Null(GetStockMovement(LineId));
        DataRow Line = GetStockCountLine(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(0m, Line.AsDecimal("DifferenceQuantity"));
        Assert.Equal(0m, Line.AsDecimal("DifferenceCostAmount"));
        Assert.Equal(10m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1000m, Balance.AsDecimal("TotalCostAmount"));
    }
    [Fact]
    public void FailedMultiLineStockCountRollsBackAllAdjustments()
    {
        DataRow FirstProduct = GetProduct("Coffee Machine");
        DataRow SecondProduct = GetProduct("Orange Juice");
        DataRow[] OrderedProducts = [FirstProduct, SecondProduct];
        OrderedProducts = OrderedProducts.OrderBy(Product => Product.AsString("Id")).ToArray();
        string WarehouseId = GetWarehouseId("Main Warehouse");
        SetStockBalance(OrderedProducts[0].AsString("Id"), WarehouseId, 10m, 100m);
        SetStockBalance(OrderedProducts[1].AsString("Id"), WarehouseId, 20m, 50m);
        StockCountDataModule Module = CreateStockCount(
            (OrderedProducts[0].AsString("Name"), 8m, 100m),
            (OrderedProducts[1].AsString("Name"), 18m, 50m));
        string StockCountId = Module.CurrentRow.AsString("Id");
        DataRow FirstLine = Module.GetTable("StockCountLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(OrderedProducts[0].AsString("Id")));
        DataRow SecondLine = Module.GetTable("StockCountLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(OrderedProducts[1].AsString("Id")));
        SetStockBalance(OrderedProducts[1].AsString("Id"), WarehouseId, 21m, 50m);

        Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Equal((int)TradeStatus.Draft, GetStockCount(StockCountId).AsInteger("StatusId"));
        Assert.Null(GetStockMovement(FirstLine.AsString("Id")));
        Assert.Null(GetStockMovement(SecondLine.AsString("Id")));
        DataRow FirstBalance = GetStockBalance(OrderedProducts[0].AsString("Id"), WarehouseId);
        DataRow SecondBalance = GetStockBalance(OrderedProducts[1].AsString("Id"), WarehouseId);
        Assert.Equal(10m, FirstBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1000m, FirstBalance.AsDecimal("TotalCostAmount"));
        Assert.Equal(21m, SecondBalance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1050m, SecondBalance.AsDecimal("TotalCostAmount"));
    }
}

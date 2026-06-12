namespace tERP.Tests;

[Collection(TestCollection.Name)]
public class SalesDocumentTests
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
                           Product.TaxProductGroupId,
                           Product.PrimaryUnitOfMeasureId as UnitOfMeasureId,
                           UnitOfMeasure.Name as UnitOfMeasureName
                         from
                           Product
                             inner join UnitOfMeasure on UnitOfMeasure.Id = Product.PrimaryUnitOfMeasureId
                         where
                           Product.Name = :Name
                         """;
        DataRow Result = fFixture.Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["Name"] = Name,
        });

        if (Result == null)
            throw new TripousDataException($"Product not found: {Name}");

        return Result;
    }
    string GetCustomerId()
    {
        object Result = fFixture.Store.SelectResult("select Id from Person where Code = 'CUST-ACME'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test customer not found.");
        return Result.ToString();
    }
    string GetTaxJurisdictionId()
    {
        object Result = fFixture.Store.SelectResult("select Id from TaxJurisdiction where Code = 'GR'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test tax jurisdiction not found.");
        return Result.ToString();
    }
    DataRow GetTrade(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from Trade where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Trade not found: {Id}");
        return Result;
    }
    DataRow GetTradeLine(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from TradeLine where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Trade line not found: {Id}");
        return Result;
    }
    DataRow GetStockMovement(string SourceId)
    {
        return fFixture.Store.SelectResults("select * from StockMovement where SourceId = :SourceId", new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataRow GetStockBalance(string ProductName, string WarehouseName)
    {
        string SqlText = """
                         select StockBalance.*
                         from StockBalance
                           inner join Product on Product.Id = StockBalance.ProductId
                           inner join Warehouse on Warehouse.Id = StockBalance.WarehouseId
                         where Product.Name = :ProductName
                           and Warehouse.Name = :WarehouseName
                         """;
        return fFixture.Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["ProductName"] = ProductName,
            ["WarehouseName"] = WarehouseName,
        });
    }
    string GetWarehouseId(string Name)
    {
        object Result = fFixture.Store.SelectResult("select Id from Warehouse where Name = :Name", null, new Dictionary<string, object>()
        {
            ["Name"] = Name,
        });
        if (Sys.IsNull(Result))
            throw new TripousDataException($"Test warehouse not found: {Name}");
        return Result.ToString();
    }
    void SetStockBalance(string ProductName, decimal PrimaryQuantity, decimal AverageUnitCost, string WarehouseName)
    {
        string ProductId = GetProduct(ProductName).AsString("Id");
        string WarehouseId = GetWarehouseId(WarehouseName);
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

        SqlText = """
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
                  """;
        fFixture.Store.ExecSql(SqlText, new Dictionary<string, object>()
        {
            ["Id"] = Sys.GenId(),
            ["ProductId"] = ProductId,
            ["WarehouseId"] = WarehouseId,
            ["PrimaryQuantity"] = PrimaryQuantity,
            ["TotalCostAmount"] = TotalCostAmount,
            ["AverageUnitCost"] = AverageUnitCost,
        });
    }
    void ConfigureDocument(SalesDataModule Module, string WarehouseName)
    {
        Module.CurrentRow.SetValue("PersonId", GetCustomerId());
        Module.CurrentRow.SetValue("OriginTaxJurisdictionId", GetTaxJurisdictionId());
        Module.CurrentRow.SetValue("DestinationTaxJurisdictionId", GetTaxJurisdictionId());
        Module.CurrentRow.SetValue("WarehouseId", GetWarehouseId(WarehouseName));
    }
    DataRow AddLine(SalesDataModule Module, string ProductName, decimal Quantity, string WarehouseName)
    {
        DataRow Product = GetProduct(ProductName);
        MemTable LineTable = Module.GetTable("TradeLine");
        DataRow Result = LineTable.AddNewRow();
        Result.SetValue("ProductId", Product["Id"]);
        Result.SetValue("ProductCode", Product["Code"]);
        Result.SetValue("ProductName", Product["Name"]);
        Result.SetValue("TaxProductGroupId", Product["TaxProductGroupId"]);
        Result.SetValue("UnitOfMeasureId", Product["UnitOfMeasureId"]);
        Result.SetValue("UnitOfMeasureName", Product["UnitOfMeasureName"]);
        Result.SetValue("UnitRatio", 1m);
        Result.SetValue("Quantity", Quantity);
        Result.SetValue("WarehouseId", GetWarehouseId(WarehouseName));
        return Result;
    }
    SalesOrderDataModule CreateSalesOrder(decimal Quantity)
    {
        return CreateSalesOrder(("Laptop Computer 14", Quantity));
    }
    SalesOrderDataModule CreateSalesOrder(params (string ProductName, decimal Quantity)[] Lines)
    {
        SalesOrderDataModule Module = DataRegistry.CreateModule("SalesOrder") as SalesOrderDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Sales Order module.");

        Module.Insert();
        ConfigureDocument(Module, "Main Warehouse");
        foreach ((string ProductName, decimal Quantity) Line in Lines)
        {
            SetStockBalance(Line.ProductName, 1000m, 0m, "Main Warehouse");
            AddLine(Module, Line.ProductName, Line.Quantity, "Main Warehouse");
        }
        Module.Commit();
        return Module;
    }
    SalesDeliveryNoteDataModule CreateDeliveryNote(SalesOrderDataModule OrderModule, decimal Quantity)
    {
        return CreateDeliveryNote(OrderModule, ("Laptop Computer 14", Quantity));
    }
    SalesDeliveryNoteDataModule CreateDeliveryNote(SalesOrderDataModule OrderModule, params (string ProductName, decimal Quantity)[] Lines)
    {
        SalesDeliveryNoteDataModule Result = OrderModule.CreateDeliveryNote();
        MemTable LineTable = Result.GetTable("TradeLine");
        Dictionary<string, decimal> Quantities = Lines.ToDictionary(Line => Line.ProductName, Line => Line.Quantity, StringComparer.OrdinalIgnoreCase);

        foreach (DataRow Row in LineTable.Rows.Cast<DataRow>().ToArray())
        {
            if (Quantities.TryGetValue(Row.AsString("ProductName"), out decimal Quantity))
                Row.SetValue("Quantity", Quantity);
            else
                Row.Delete();
        }

        Result.Commit();
        return Result;
    }
    string GetModuleLineId(SalesDataModule Module, string ProductName)
    {
        DataRow Line = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.RowState != DataRowState.Deleted && Row.AsString("ProductName").IsSameText(ProductName));
        return Line.AsString("Id");
    }
    SalesDeliveryNoteDataModule CreateStandaloneDeliveryNote(decimal Quantity)
    {
        return CreateStandaloneDeliveryNote("Mineral Water", Quantity, "Main Warehouse");
    }
    SalesDeliveryNoteDataModule CreateStandaloneDeliveryNote(string ProductName, decimal Quantity, string WarehouseName)
    {
        SalesDeliveryNoteDataModule Result = DataRegistry.CreateModule("SalesDeliveryNote") as SalesDeliveryNoteDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Sales Delivery Note module.");

        SetStockBalance(ProductName, 1000m, 0m, WarehouseName);
        Result.Insert();
        ConfigureDocument(Result, WarehouseName);
        AddLine(Result, ProductName, Quantity, WarehouseName);
        Result.Commit();
        return Result;
    }

    // ● construction
    public SalesDocumentTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    [Fact]
    public void PostingSalesOrderLocksDocument()
    {
        SalesOrderDataModule Module = CreateSalesOrder(10m);
        string OrderId = Module.CurrentRow.AsString("Id");

        Module.Post();

        DataRow Order = GetTrade(OrderId);
        Assert.Equal((int)TradeStatus.Posted, Order.AsInteger("TradeStatusId"));
        Assert.True(Order.AsBoolean("IsLocked"));
        Assert.False(Order.AsString("Code").StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase));
        Assert.False(Sys.IsNull(Order["PostedAt"]));
        Assert.Equal(Sys.Context.CurrentUser.Id, Order.AsString("PostedBy"));
    }
    [Fact]
    public void PartialDeliveriesUpdateExecutedQuantity()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(10m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(OrderModule, 4m);
        string FirstDeliveryLineId = FirstDelivery.GetTable("TradeLine").Rows[0].AsString("Id");
        FirstDelivery.Post();
        Assert.Equal(4m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
        DataRow FirstMovement = GetStockMovement(FirstDeliveryLineId);
        Assert.NotNull(FirstMovement);
        Assert.Equal(-1, FirstMovement.AsInteger("Direction"));
        Assert.Equal(4m, FirstMovement.AsDecimal("Quantity"));

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule SecondDelivery = CreateDeliveryNote(OrderModule, 6m);
        SecondDelivery.Post();
        Assert.Equal(10m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));

        OrderModule.Edit(OrderId);
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => OrderModule.CreateDeliveryNote());
        Assert.Contains("only posted sales orders", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void MultiLineOrderRemainsPostedUntilAllLinesAreDelivered()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(
            ("Laptop Computer 14", 10m),
            ("Monitor 27 Inch", 5m));
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string LaptopLineId = GetModuleLineId(OrderModule, "Laptop Computer 14");
        string MonitorLineId = GetModuleLineId(OrderModule, "Monitor 27 Inch");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(OrderModule, ("Laptop Computer 14", 10m));
        FirstDelivery.Post();

        Assert.Equal(10m, GetTradeLine(LaptopLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(0m, GetTradeLine(MonitorLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule SecondDelivery = CreateDeliveryNote(OrderModule, ("Monitor 27 Inch", 5m));
        SecondDelivery.Post();

        Assert.Equal(5m, GetTradeLine(MonitorLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));
    }
    [Fact]
    public void MultiLineOrderCompletesAfterDifferentPartialQuantities()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(
            ("Laptop Computer 14", 10m),
            ("Monitor 27 Inch", 5m));
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string LaptopLineId = GetModuleLineId(OrderModule, "Laptop Computer 14");
        string MonitorLineId = GetModuleLineId(OrderModule, "Monitor 27 Inch");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(
            OrderModule,
            ("Laptop Computer 14", 4m),
            ("Monitor 27 Inch", 2m));
        FirstDelivery.Post();

        Assert.Equal(4m, GetTradeLine(LaptopLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(2m, GetTradeLine(MonitorLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule SecondDelivery = CreateDeliveryNote(
            OrderModule,
            ("Laptop Computer 14", 6m),
            ("Monitor 27 Inch", 3m));
        SecondDelivery.Post();

        Assert.Equal(10m, GetTradeLine(LaptopLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(5m, GetTradeLine(MonitorLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));
    }
    [Fact]
    public void FailedMultiLineDeliveryDoesNotCompleteOrder()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(
            ("Laptop Computer 14", 10m),
            ("Monitor 27 Inch", 5m),
            ("Wireless Keyboard", 10m));
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string LaptopLineId = GetModuleLineId(OrderModule, "Laptop Computer 14");
        string MonitorLineId = GetModuleLineId(OrderModule, "Monitor 27 Inch");
        string KeyboardLineId = GetModuleLineId(OrderModule, "Wireless Keyboard");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(
            OrderModule,
            ("Laptop Computer 14", 10m),
            ("Monitor 27 Inch", 2m));
        FirstDelivery.Post();

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule FailedDelivery = CreateDeliveryNote(
            OrderModule,
            ("Monitor 27 Inch", 4m),
            ("Wireless Keyboard", 10m));

        Assert.Throws<TripousBusinessException>(() => FailedDelivery.Post());

        Assert.Equal(10m, GetTradeLine(LaptopLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(2m, GetTradeLine(MonitorLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(0m, GetTradeLine(KeyboardLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
    }
    [Fact]
    public void DeliveryQuantityCannotExceedRemainingQuantity()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(10m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(OrderModule, 6m);
        FirstDelivery.Post();

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule ExcessDelivery = CreateDeliveryNote(OrderModule, 5m);
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ExcessDelivery.Post());

        Assert.Contains("exceeds remaining quantity 4", Error.Message.ToLowerInvariant());
        Assert.Equal(6m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
    }
    [Fact]
    public void FailedDeliveryPostingRollsBackDocumentAndSource()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(10m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(OrderModule, 6m);
        FirstDelivery.Post();

        OrderModule.Edit(OrderId);
        SalesDeliveryNoteDataModule ExcessDelivery = CreateDeliveryNote(OrderModule, 5m);
        string DeliveryId = ExcessDelivery.CurrentRow.AsString("Id");
        string DraftCode = ExcessDelivery.CurrentRow.AsString("Code");

        Assert.Throws<TripousBusinessException>(() => ExcessDelivery.Post());

        DataRow Delivery = GetTrade(DeliveryId);
        Assert.Equal((int)TradeStatus.Draft, Delivery.AsInteger("TradeStatusId"));
        Assert.False(Delivery.AsBoolean("IsLocked"));
        Assert.Equal(DraftCode, Delivery.AsString("Code"));
        Assert.True(Sys.IsNull(Delivery["PostedAt"]));
        Assert.Equal(6m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(ExcessDelivery.GetTable("TradeLine").Rows[0].AsString("Id")));
    }
    [Fact]
    public void StandaloneDeliveryNoteCanBePosted()
    {
        SalesDeliveryNoteDataModule Module = CreateStandaloneDeliveryNote(48m);
        string DeliveryId = Module.CurrentRow.AsString("Id");
        string DeliveryLineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Delivery = GetTrade(DeliveryId);
        Assert.Equal((int)TradeStatus.Posted, Delivery.AsInteger("TradeStatusId"));
        Assert.True(Delivery.AsBoolean("IsLocked"));
        Assert.True(Sys.IsNull(Delivery["SourceId"]));
        DataRow Movement = GetStockMovement(DeliveryLineId);
        Assert.NotNull(Movement);
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(48m, Movement.AsDecimal("Quantity"));
        Assert.Equal("SalesDeliveryNote", Movement.AsString("SourceModule"));
    }
    [Fact]
    public void PostingDeliveryNoteUpdatesStockBalance()
    {
        SalesDeliveryNoteDataModule Module = CreateStandaloneDeliveryNote("Laptop Computer 14", 4m, "Main Warehouse");
        string DeliveryLineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");
        SetStockBalance("Laptop Computer 14", 20m, 100m, "Main Warehouse");

        Module.Post();

        DataRow Movement = GetStockMovement(DeliveryLineId);
        DataRow Balance = GetStockBalance("Laptop Computer 14", "Main Warehouse");
        Assert.Equal(100m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(400m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(16m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(1600m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(100m, Balance.AsDecimal("AverageUnitCost"));
        Assert.Equal(Movement.AsString("Id"), Balance.AsString("LastMovementId"));
    }
    [Fact]
    public void PostingDeliveryNoteRejectsNegativeStock()
    {
        SalesDeliveryNoteDataModule Module = CreateStandaloneDeliveryNote("Laptop Computer 14", 6m, "Main Warehouse");
        string DeliveryId = Module.CurrentRow.AsString("Id");
        string DeliveryLineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");
        SetStockBalance("Laptop Computer 14", 5m, 100m, "Main Warehouse");

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Contains("cannot become negative", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Draft, GetTrade(DeliveryId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(DeliveryLineId));
        DataRow Balance = GetStockBalance("Laptop Computer 14", "Main Warehouse");
        Assert.Equal(5m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(500m, Balance.AsDecimal("TotalCostAmount"));
    }
    [Fact]
    public void PostingDeliveryNoteAllowsNegativeStockWhenConfigured()
    {
        string WarehouseName = "Scrap / Damaged Stock";
        SalesDeliveryNoteDataModule Module = CreateStandaloneDeliveryNote("Laptop Computer 14", 6m, WarehouseName);
        string DeliveryLineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");
        SetStockBalance("Laptop Computer 14", 0m, 0m, WarehouseName);

        Module.Post();

        Assert.NotNull(GetStockMovement(DeliveryLineId));
        DataRow Balance = GetStockBalance("Laptop Computer 14", WarehouseName);
        Assert.Equal(-6m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(0m, Balance.AsDecimal("AverageUnitCost"));
    }
}

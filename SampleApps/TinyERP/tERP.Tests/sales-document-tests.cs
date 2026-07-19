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
        SetStockBalance(ProductName, PrimaryQuantity, PrimaryQuantity * AverageUnitCost, AverageUnitCost, WarehouseName);
    }
    void SetStockBalance(string ProductName, decimal PrimaryQuantity, decimal TotalCostAmount, decimal AverageUnitCost, string WarehouseName)
    {
        string ProductId = GetProduct(ProductName).AsString("Id");
        string WarehouseId = GetWarehouseId(WarehouseName);
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
    SalesReturnDataModule CreateSalesReturn(params (string ProductName, decimal Quantity, decimal UnitPrice)[] Lines)
    {
        SalesReturnDataModule Result = DataRegistry.CreateModule("SalesReturn") as SalesReturnDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Sales Return module.");

        Result.Insert();
        ConfigureDocument(Result, "Main Warehouse");
        foreach ((string ProductName, decimal Quantity, decimal UnitPrice) Line in Lines)
        {
            DataRow Row = AddLine(Result, Line.ProductName, Line.Quantity, "Main Warehouse");
            Row.SetValue("UnitPrice", Line.UnitPrice);
        }
        Result.Commit();
        return Result;
    }
    SalesReturnDataModule CreateSalesReturn(SalesDeliveryNoteDataModule DeliveryModule, decimal Quantity)
    {
        SalesReturnDataModule Result = DeliveryModule.CreateReturn();
        decimal UnitPrice = Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice");
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Assert.Equal(UnitPrice, Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        Result.Commit();
        return Result;
    }
    SalesInvoiceDataModule CreateSalesInvoice(SalesDeliveryNoteDataModule DeliveryModule, decimal Quantity)
    {
        SalesInvoiceDataModule Result = DeliveryModule.CreateInvoice();
        decimal UnitPrice = Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice");
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Assert.Equal(UnitPrice, Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        Result.Commit();
        return Result;
    }
    SalesCreditNoteDataModule CreateSalesCreditNote(SalesInvoiceDataModule InvoiceModule, decimal Quantity)
    {
        SalesCreditNoteDataModule Result = InvoiceModule.CreateCreditNote();
        decimal UnitPrice = Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice");
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Assert.Equal(UnitPrice, Result.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        Result.Commit();
        return Result;
    }
    SalesCancellationDataModule CreateSalesCancellation(SalesInvoiceDataModule InvoiceModule)
    {
        SalesCancellationDataModule Result = InvoiceModule.CreateCancellation();
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
    public void SalesOrderDeliveryReturnInvoiceCreditFlowKeepsQuantityCountersIndependent()
    {
        SalesOrderDataModule OrderModule = CreateSalesOrder(10m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        SalesDeliveryNoteDataModule FirstDelivery = CreateDeliveryNote(OrderModule, 8m);
        string FirstDeliveryLineId = FirstDelivery.GetTable("TradeLine").Rows[0].AsString("Id");
        FirstDelivery.Post();
        SalesDeliveryNoteDataModule SecondDelivery = CreateDeliveryNote(OrderModule, 2m);
        string SecondDeliveryLineId = SecondDelivery.GetTable("TradeLine").Rows[0].AsString("Id");
        SecondDelivery.Post();

        Assert.Equal(10m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));
        Assert.Equal(-1, GetStockMovement(FirstDeliveryLineId).AsInteger("Direction"));
        Assert.Equal(-1, GetStockMovement(SecondDeliveryLineId).AsInteger("Direction"));

        SalesReturnDataModule FirstReturn = CreateSalesReturn(FirstDelivery, 3m);
        string FirstReturnLineId = FirstReturn.GetTable("TradeLine").Rows[0].AsString("Id");
        FirstReturn.Post();
        SalesReturnDataModule SecondReturn = CreateSalesReturn(FirstDelivery, 5m);
        string SecondReturnLineId = SecondReturn.GetTable("TradeLine").Rows[0].AsString("Id");
        SecondReturn.Post();

        Assert.Equal(8m, GetTradeLine(FirstDeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Equal(0m, GetTradeLine(FirstDeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(1, GetStockMovement(FirstReturnLineId).AsInteger("Direction"));
        Assert.Equal(1, GetStockMovement(SecondReturnLineId).AsInteger("Direction"));
        Assert.False(FirstDelivery.HasRemainingTransformQuantity());
        Assert.Throws<TripousBusinessException>(() => FirstDelivery.CreateReturn());

        SalesInvoiceDataModule SmallInvoice = CreateSalesInvoice(SecondDelivery, 2m);
        string SmallInvoiceLineId = SmallInvoice.GetTable("TradeLine").Rows[0].AsString("Id");
        SmallInvoice.Post();
        Assert.Equal(2m, GetTradeLine(SecondDeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(0m, GetTradeLine(SecondDeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Null(GetStockMovement(SmallInvoiceLineId));
        Assert.False(SecondDelivery.HasRemainingInvoiceQuantity());
        Assert.Throws<TripousBusinessException>(() => SecondDelivery.CreateInvoice());

        SalesInvoiceDataModule FullInvoice = CreateSalesInvoice(FirstDelivery, 8m);
        string FullInvoiceLineId = FullInvoice.GetTable("TradeLine").Rows[0].AsString("Id");
        decimal InvoiceUnitPrice = FullInvoice.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice");
        FullInvoice.Post();
        Assert.Equal(8m, GetTradeLine(FirstDeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(8m, GetTradeLine(FirstDeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Null(GetStockMovement(FullInvoiceLineId));

        SalesCreditNoteDataModule FirstCredit = CreateSalesCreditNote(FullInvoice, 2m);
        string FirstCreditLineId = FirstCredit.GetTable("TradeLine").Rows[0].AsString("Id");
        Assert.Equal(InvoiceUnitPrice, FirstCredit.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        FirstCredit.Post();
        SalesCreditNoteDataModule SecondCredit = CreateSalesCreditNote(FullInvoice, 3m);
        string SecondCreditLineId = SecondCredit.GetTable("TradeLine").Rows[0].AsString("Id");
        Assert.Equal(InvoiceUnitPrice, SecondCredit.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        SecondCredit.Post();
        SalesCreditNoteDataModule ThirdCredit = CreateSalesCreditNote(FullInvoice, 3m);
        string ThirdCreditLineId = ThirdCredit.GetTable("TradeLine").Rows[0].AsString("Id");
        Assert.Equal(InvoiceUnitPrice, ThirdCredit.GetTable("TradeLine").Rows[0].AsDecimal("UnitPrice"));
        ThirdCredit.Post();

        Assert.Equal(8m, GetTradeLine(FullInvoiceLineId).AsDecimal("CreditedQuantity"));
        Assert.Null(GetStockMovement(FirstCreditLineId));
        Assert.Null(GetStockMovement(SecondCreditLineId));
        Assert.Null(GetStockMovement(ThirdCreditLineId));
        Assert.False(FullInvoice.HasRemainingCreditQuantity());
        Assert.Throws<TripousBusinessException>(() => FullInvoice.CreateCreditNote());
        Assert.Throws<TripousBusinessException>(() => FullInvoice.CreateCancellation());
    }
    [Fact]
    public void PartialSalesInvoicesUpdateInvoicedQuantityIndependentlyFromReturns()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();

        SalesInvoiceDataModule FirstInvoice = CreateSalesInvoice(DeliveryModule, 4m);
        string FirstInvoiceLineId = FirstInvoice.GetTable("TradeLine").Rows[0].AsString("Id");
        FirstInvoice.Post();

        SalesReturnDataModule ReturnModule = CreateSalesReturn(DeliveryModule, 3m);
        ReturnModule.Post();

        Assert.Equal(4m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(3m, GetTradeLine(DeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Null(GetStockMovement(FirstInvoiceLineId));

        SalesInvoiceDataModule SecondInvoice = DeliveryModule.CreateInvoice();
        Assert.Equal(6m, SecondInvoice.GetTable("TradeLine").Rows[0].AsDecimal("Quantity"));
        string SecondInvoiceLineId = SecondInvoice.GetTable("TradeLine").Rows[0].AsString("Id");
        SecondInvoice.Commit();
        SecondInvoice.Post();

        Assert.Equal(10m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(3m, GetTradeLine(DeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Null(GetStockMovement(SecondInvoiceLineId));
        Assert.False(DeliveryModule.HasRemainingInvoiceQuantity());
    }
    [Fact]
    public void PostingSalesInvoiceRejectsQuantityExceedingCurrentRemainingQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();

        SalesInvoiceDataModule FirstInvoice = CreateSalesInvoice(DeliveryModule, 6m);
        SalesInvoiceDataModule ExcessInvoice = CreateSalesInvoice(DeliveryModule, 5m);
        FirstInvoice.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ExcessInvoice.Post());

        Assert.Contains("invoice quantity 5 exceeds remaining quantity 4", Error.Message.ToLowerInvariant());
        Assert.Equal(6m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
    }
    [Fact]
    public void PartialSalesCreditNotesUpdateCreditedQuantityWithoutStockMovement()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 10m);
        string InvoiceLineId = InvoiceModule.GetTable("TradeLine").Rows[0].AsString("Id");
        InvoiceModule.Post();

        SalesCreditNoteDataModule FirstCreditNote = CreateSalesCreditNote(InvoiceModule, 4m);
        string FirstCreditLineId = FirstCreditNote.GetTable("TradeLine").Rows[0].AsString("Id");
        Assert.Equal(InvoiceModule.CurrentRow.AsString("PersonId"), FirstCreditNote.CurrentRow.AsString("PersonId"));
        Assert.Equal(InvoiceModule.CurrentRow.AsString("BillingCountryId"), FirstCreditNote.CurrentRow.AsString("BillingCountryId"));
        Assert.Equal(InvoiceModule.CurrentRow.AsString("DestinationTaxJurisdictionId"), FirstCreditNote.CurrentRow.AsString("DestinationTaxJurisdictionId"));
        FirstCreditNote.Post();

        SalesCreditNoteDataModule SecondCreditNote = InvoiceModule.CreateCreditNote();
        Assert.Equal(6m, SecondCreditNote.GetTable("TradeLine").Rows[0].AsDecimal("Quantity"));
        string SecondCreditLineId = SecondCreditNote.GetTable("TradeLine").Rows[0].AsString("Id");
        SecondCreditNote.Commit();
        SecondCreditNote.Post();

        Assert.Equal(10m, GetTradeLine(InvoiceLineId).AsDecimal("CreditedQuantity"));
        Assert.Null(GetStockMovement(FirstCreditLineId));
        Assert.Null(GetStockMovement(SecondCreditLineId));
        Assert.False(InvoiceModule.HasRemainingCreditQuantity());
    }
    [Fact]
    public void PostingSalesCreditNoteRejectsQuantityExceedingCurrentRemainingQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 10m);
        string InvoiceLineId = InvoiceModule.GetTable("TradeLine").Rows[0].AsString("Id");
        InvoiceModule.Post();

        SalesCreditNoteDataModule FirstCreditNote = CreateSalesCreditNote(InvoiceModule, 6m);
        SalesCreditNoteDataModule ExcessCreditNote = CreateSalesCreditNote(InvoiceModule, 5m);
        FirstCreditNote.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ExcessCreditNote.Post());

        Assert.Contains("credit quantity 5 exceeds remaining quantity 4", Error.Message.ToLowerInvariant());
        Assert.Equal(6m, GetTradeLine(InvoiceLineId).AsDecimal("CreditedQuantity"));
    }
    [Fact]
    public void PostingSalesCancellationCancelsInvoiceAndReleasesDeliveryQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 4m);
        string InvoiceId = InvoiceModule.CurrentRow.AsString("Id");
        InvoiceModule.Post();

        SalesCancellationDataModule CancellationModule = CreateSalesCancellation(InvoiceModule);
        string CancellationId = CancellationModule.CurrentRow.AsString("Id");
        string CancellationLineId = CancellationModule.GetTable("TradeLine").Rows[0].AsString("Id");
        Assert.Equal(InvoiceId, CancellationModule.CurrentRow.AsString("CancelsTradeId"));
        Assert.Equal(4m, CancellationModule.GetTable("TradeLine").Rows[0].AsDecimal("Quantity"));
        CancellationModule.Post();

        DataRow Invoice = GetTrade(InvoiceId);
        Assert.Equal((int)TradeStatus.Cancelled, Invoice.AsInteger("TradeStatusId"));
        Assert.True(Invoice.AsBoolean("IsCancelled"));
        Assert.Equal(CancellationId, Invoice.AsString("CancelledByTradeId"));
        Assert.False(Sys.IsNull(Invoice["CancelledAt"]));
        Assert.Equal(Sys.Context.CurrentUser.Id, Invoice.AsString("CancelledBy"));
        Assert.Equal(0m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Null(GetStockMovement(CancellationLineId));

        SalesInvoiceDataModule ReplacementInvoice = DeliveryModule.CreateInvoice();
        Assert.Equal(10m, ReplacementInvoice.GetTable("TradeLine").Rows[0].AsDecimal("Quantity"));
    }
    [Fact]
    public void PostingSecondSalesCancellationRejectsAlreadyCancelledInvoice()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 10m);
        InvoiceModule.Post();
        SalesCancellationDataModule FirstCancellation = CreateSalesCancellation(InvoiceModule);
        SalesCancellationDataModule SecondCancellation = CreateSalesCancellation(InvoiceModule);
        FirstCancellation.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => SecondCancellation.Post());

        Assert.Contains("already cancelled", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void SalesInvoiceWithPostedCreditNoteCannotBeCancelled()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 10m);
        InvoiceModule.Post();
        SalesCreditNoteDataModule CreditNoteModule = CreateSalesCreditNote(InvoiceModule, 4m);
        CreditNoteModule.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => InvoiceModule.CreateCancellation());

        Assert.Contains("with posted credit notes cannot be cancelled", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void SalesCancellationMustPreserveInvoiceQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice(DeliveryModule, 4m);
        string InvoiceId = InvoiceModule.CurrentRow.AsString("Id");
        InvoiceModule.Post();
        SalesCancellationDataModule CancellationModule = InvoiceModule.CreateCancellation();
        CancellationModule.GetTable("TradeLine").Rows[0].SetValue("Quantity", 3m);
        CancellationModule.Commit();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => CancellationModule.Post());

        Assert.Contains("cancellation quantity must equal", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Posted, GetTrade(InvoiceId).AsInteger("TradeStatusId"));
        Assert.Equal(4m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
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

        SalesDeliveryNoteDataModule SecondDelivery = CreateDeliveryNote(OrderModule, 6m);
        SecondDelivery.Post();
        Assert.Equal(10m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => OrderModule.CreateDeliveryNote());
        Assert.Contains("only posted sales orders", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PostingStaleSalesOrderModuleRejectsAlreadyPostedDocument()
    {
        SalesOrderDataModule FirstModule = CreateSalesOrder(10m);
        string OrderId = FirstModule.CurrentRow.AsString("Id");
        SalesOrderDataModule StaleModule = DataRegistry.CreateModule("SalesOrder") as SalesOrderDataModule;
        if (StaleModule == null)
            throw new TripousDataException("Cannot create the Sales Order module.");
        StaleModule.Edit(OrderId);

        FirstModule.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => StaleModule.Post());
        Assert.Contains("changed after it was loaded", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
    }
    [Fact]
    public void SavingStaleSalesOrderModuleRejectsPostedDocument()
    {
        SalesOrderDataModule FirstModule = CreateSalesOrder(10m);
        string OrderId = FirstModule.CurrentRow.AsString("Id");
        SalesOrderDataModule StaleModule = DataRegistry.CreateModule("SalesOrder") as SalesOrderDataModule;
        if (StaleModule == null)
            throw new TripousDataException("Cannot create the Sales Order module.");
        StaleModule.Edit(OrderId);
        StaleModule.CurrentRow.SetValue("Remarks", "Stale edit");

        FirstModule.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => StaleModule.Commit());
        Assert.Contains("changed after it was loaded", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
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
    public void PostingDeliveryNoteClearsCostWhenStockBecomesZero()
    {
        SalesDeliveryNoteDataModule Module = CreateStandaloneDeliveryNote("Laptop Computer 14", 3m, "Main Warehouse");
        SetStockBalance("Laptop Computer 14", 3m, 100m, 33.3333m, "Main Warehouse");

        Module.Post();

        DataRow Balance = GetStockBalance("Laptop Computer 14", "Main Warehouse");
        Assert.Equal(0m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(0m, Balance.AsDecimal("AverageUnitCost"));
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
    [Fact]
    public void PostingSalesReturnCreatesIncomingStockAtAverageCost()
    {
        SetStockBalance("Laptop Computer 14", 10m, 12m, "Main Warehouse");
        SalesReturnDataModule Module = CreateSalesReturn(("Laptop Computer 14", 4m, 20m));
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance("Laptop Computer 14", "Main Warehouse");
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(4m, Movement.AsDecimal("PrimaryQuantity"));
        Assert.Equal(12m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(48m, Movement.AsDecimal("CostAmount"));
        Assert.Equal("SalesReturn", Movement.AsString("SourceModule"));
        Assert.Equal(14m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(168m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(12m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PartialSalesReturnsUpdateSourceDeliveryQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryId = DeliveryModule.CurrentRow.AsString("Id");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        SetStockBalance("Laptop Computer 14", 20m, 12m, "Main Warehouse");
        DeliveryModule.Post();

        SalesReturnDataModule FirstReturn = CreateSalesReturn(DeliveryModule, 4m);
        FirstReturn.Post();

        Assert.Equal(4m, GetTradeLine(DeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(DeliveryId).AsInteger("TradeStatusId"));
        Assert.Equal(14m, GetStockBalance("Laptop Computer 14", "Main Warehouse").AsDecimal("PrimaryQuantity"));

        SalesReturnDataModule SecondReturn = CreateSalesReturn(DeliveryModule, 6m);
        SecondReturn.Post();

        Assert.Equal(10m, GetTradeLine(DeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(DeliveryId).AsInteger("TradeStatusId"));
        Assert.Equal(20m, GetStockBalance("Laptop Computer 14", "Main Warehouse").AsDecimal("PrimaryQuantity"));
        Assert.False(DeliveryModule.HasRemainingTransformQuantity());

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => DeliveryModule.CreateReturn());
        Assert.Contains("no remaining quantity", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void SalesReturnQuantityCannotExceedRemainingDeliveryQuantity()
    {
        SalesDeliveryNoteDataModule DeliveryModule = CreateStandaloneDeliveryNote("Laptop Computer 14", 10m, "Main Warehouse");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        SetStockBalance("Laptop Computer 14", 20m, 12m, "Main Warehouse");
        DeliveryModule.Post();

        SalesReturnDataModule ReturnModule = CreateSalesReturn(DeliveryModule, 11m);
        string ReturnId = ReturnModule.CurrentRow.AsString("Id");
        string ReturnLineId = ReturnModule.GetTable("TradeLine").Rows[0].AsString("Id");
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ReturnModule.Post());

        Assert.Contains("exceeds remaining quantity 10", Error.Message.ToLowerInvariant());
        Assert.Equal(0m, GetTradeLine(DeliveryLineId).AsDecimal("ReturnedQuantity"));
        Assert.Equal((int)TradeStatus.Draft, GetTrade(ReturnId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(ReturnLineId));
        Assert.Equal(10m, GetStockBalance("Laptop Computer 14", "Main Warehouse").AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void FailedMultiLineSalesReturnRollsBackAllStockChanges()
    {
        DataRow[] Products =
        [
            GetProduct("Laptop Computer 14"),
            GetProduct("Orange Juice"),
        ];
        Products = Products.OrderBy(Product => Product.AsString("Id")).ToArray();
        SetStockBalance(Products[0].AsString("Name"), 10m, 12m, "Main Warehouse");
        SetStockBalance(Products[1].AsString("Name"), 20m, 8m, "Main Warehouse");
        SalesReturnDataModule Module = CreateSalesReturn(
            (Products[0].AsString("Name"), 4m, 20m),
            (Products[1].AsString("Name"), 0m, 10m));
        string ReturnId = Module.CurrentRow.AsString("Id");
        DataRow FirstLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[0].AsString("Id")));
        DataRow SecondLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[1].AsString("Id")));

        Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Equal((int)TradeStatus.Draft, GetTrade(ReturnId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(FirstLine.AsString("Id")));
        Assert.Null(GetStockMovement(SecondLine.AsString("Id")));
        Assert.Equal(10m, GetStockBalance(Products[0].AsString("Name"), "Main Warehouse").AsDecimal("PrimaryQuantity"));
        Assert.Equal(120m, GetStockBalance(Products[0].AsString("Name"), "Main Warehouse").AsDecimal("TotalCostAmount"));
        Assert.Equal(20m, GetStockBalance(Products[1].AsString("Name"), "Main Warehouse").AsDecimal("PrimaryQuantity"));
        Assert.Equal(160m, GetStockBalance(Products[1].AsString("Name"), "Main Warehouse").AsDecimal("TotalCostAmount"));
    }
}

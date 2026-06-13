namespace tERP.Tests;

[Collection(TestCollection.Name)]
public class PurchaseDocumentTests
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
    string GetSupplierId()
    {
        object Result = fFixture.Store.SelectResult("select Id from Person where Code = 'SUP-HELIOS'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test supplier not found.");
        return Result.ToString();
    }
    string GetTaxJurisdictionId()
    {
        object Result = fFixture.Store.SelectResult("select Id from TaxJurisdiction where Code = 'GR'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test tax jurisdiction not found.");
        return Result.ToString();
    }
    string GetCountryId()
    {
        object Result = fFixture.Store.SelectResult("select Id from Country where Code = 'GR'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test country not found.");
        return Result.ToString();
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
    DataRow GetTrade(string Id)
    {
        return fFixture.Store.SelectResults("select * from Trade where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
    }
    DataRow GetTradeLine(string Id)
    {
        return fFixture.Store.SelectResults("select * from TradeLine where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
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
    int GetStockMovementCount(string SourceId)
    {
        object Result = fFixture.Store.SelectResult("select count(*) from StockMovement where SourceId = :SourceId", 0, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
        return Convert.ToInt32(Result);
    }
    void SetStockBalance(string ProductId, string WarehouseId, decimal PrimaryQuantity, decimal AverageUnitCost)
    {
        SetStockBalance(ProductId, WarehouseId, PrimaryQuantity, PrimaryQuantity * AverageUnitCost, AverageUnitCost);
    }
    void SetStockBalance(string ProductId, string WarehouseId, decimal PrimaryQuantity, decimal TotalCostAmount, decimal AverageUnitCost)
    {
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
    void ConfigurePurchaseDocument(PurchaseDataModule Module, string WarehouseId = "")
    {
        WarehouseId = string.IsNullOrWhiteSpace(WarehouseId) ? DataLib.GetDefaultWarehouseId() : WarehouseId;
        Module.CurrentRow.SetValue("PersonId", GetSupplierId());
        Module.CurrentRow.SetValue("WarehouseId", WarehouseId);
        Module.CurrentRow.SetValue("CostCenterId", DataLib.GetDefaultSalesCostCenterId());
        Module.CurrentRow.SetValue("BranchId", DataLib.GetDefaultBranchId());
        Module.CurrentRow.SetValue("PriceListTypeId", DataLib.GetDefaultPriceListTypeId());
        Module.CurrentRow.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
        Module.CurrentRow.SetValue("ExchangeRate", 1m);
        Module.CurrentRow.SetValue("PaymentMethodId", DataLib.GetDefaultPaymentMethodId());
        Module.CurrentRow.SetValue("PaymentTermId", DataLib.GetDefaultPaymentTermId());
        Module.CurrentRow.SetValue("TaxBusinessGroupId", DataLib.GetDefaultTaxBusinessGroupId());
        Module.CurrentRow.SetValue("OriginTaxJurisdictionId", GetTaxJurisdictionId());
        Module.CurrentRow.SetValue("DestinationTaxJurisdictionId", GetTaxJurisdictionId());
    }
    DataRow AddLine(PurchaseDataModule Module, string ProductName, decimal Quantity, decimal UnitPrice, decimal UnitRatio = 1, string WarehouseId = "")
    {
        WarehouseId = string.IsNullOrWhiteSpace(WarehouseId) ? DataLib.GetDefaultWarehouseId() : WarehouseId;
        DataRow Product = GetProduct(ProductName);
        DataRow Line = Module.GetTable("TradeLine").AddNewRow();
        Line.SetValue("ProductId", Product["Id"]);
        Line.SetValue("ProductCode", Product["Code"]);
        Line.SetValue("ProductName", Product["Name"]);
        Line.SetValue("TaxProductGroupId", Product["TaxProductGroupId"]);
        Line.SetValue("WarehouseId", WarehouseId);
        Line.SetValue("UnitOfMeasureId", Product["UnitOfMeasureId"]);
        Line.SetValue("UnitOfMeasureName", Product["UnitOfMeasureName"]);
        Line.SetValue("UnitRatio", UnitRatio);
        Line.SetValue("Quantity", Quantity);
        Line.SetValue("UnitPrice", UnitPrice);
        return Line;
    }
    PurchaseDeliveryNoteDataModule CreatePurchaseDeliveryNote(string ProductName, decimal Quantity, decimal UnitPrice, decimal DocumentDiscountPercent = 0, decimal UnitRatio = 1)
    {
        PurchaseDeliveryNoteDataModule Module = DataRegistry.CreateModule("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Delivery Note module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        AddLine(Module, ProductName, Quantity, UnitPrice, UnitRatio);
        Module.CurrentRow.SetValue("DiscountPercent", DocumentDiscountPercent);
        Module.Commit();
        return Module;
    }
    PurchaseDeliveryNoteDataModule CreatePurchaseDeliveryNote(params (string ProductName, decimal Quantity, decimal UnitPrice)[] Lines)
    {
        PurchaseDeliveryNoteDataModule Module = DataRegistry.CreateModule("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Delivery Note module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        foreach ((string ProductName, decimal Quantity, decimal UnitPrice) Line in Lines)
            AddLine(Module, Line.ProductName, Line.Quantity, Line.UnitPrice);
        Module.Commit();
        return Module;
    }
    PurchaseOrderDataModule CreatePurchaseOrder(string ProductName, decimal Quantity, decimal UnitPrice)
    {
        return CreatePurchaseOrder((ProductName, Quantity, UnitPrice));
    }
    PurchaseOrderDataModule CreatePurchaseOrder(params (string ProductName, decimal Quantity, decimal UnitPrice)[] Lines)
    {
        PurchaseOrderDataModule Module = DataRegistry.CreateModule("PurchaseOrder") as PurchaseOrderDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Order module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        foreach ((string ProductName, decimal Quantity, decimal UnitPrice) Line in Lines)
            AddLine(Module, Line.ProductName, Line.Quantity, Line.UnitPrice);
        Module.Commit();
        return Module;
    }
    PurchaseDeliveryNoteDataModule CreatePurchaseDeliveryNote(PurchaseOrderDataModule OrderModule, decimal Quantity)
    {
        PurchaseDeliveryNoteDataModule Result = OrderModule.CreateDeliveryNote();
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Result.Commit();
        return Result;
    }
    PurchaseDeliveryNoteDataModule CreatePurchaseDeliveryNote(PurchaseOrderDataModule OrderModule, params (string ProductName, decimal Quantity)[] Lines)
    {
        PurchaseDeliveryNoteDataModule Result = OrderModule.CreateDeliveryNote();
        Dictionary<string, decimal> Quantities = Lines.ToDictionary(Line => Line.ProductName, Line => Line.Quantity, StringComparer.OrdinalIgnoreCase);
        foreach (DataRow Row in Result.GetTable("TradeLine").Rows.Cast<DataRow>().ToArray())
        {
            if (Quantities.TryGetValue(Row.AsString("ProductName"), out decimal Quantity))
                Row.SetValue("Quantity", Quantity);
            else
                Row.Delete();
        }
        Result.Commit();
        return Result;
    }
    string GetModuleLineId(PurchaseDataModule Module, string ProductName)
    {
        DataRow Line = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.RowState != DataRowState.Deleted && Row.AsString("ProductName").IsSameText(ProductName));
        return Line.AsString("Id");
    }
    PurchaseReturnDataModule CreatePurchaseReturn(string ProductName, decimal Quantity, decimal UnitPrice, string WarehouseId = "")
    {
        PurchaseReturnDataModule Module = DataRegistry.CreateModule("PurchaseReturn") as PurchaseReturnDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Return module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module, WarehouseId);
        AddLine(Module, ProductName, Quantity, UnitPrice, 1, WarehouseId);
        Module.Commit();
        return Module;
    }
    PurchaseReturnDataModule CreatePurchaseReturn(params (string ProductName, decimal Quantity, decimal UnitPrice)[] Lines)
    {
        PurchaseReturnDataModule Module = DataRegistry.CreateModule("PurchaseReturn") as PurchaseReturnDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Return module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        foreach ((string ProductName, decimal Quantity, decimal UnitPrice) Line in Lines)
            AddLine(Module, Line.ProductName, Line.Quantity, Line.UnitPrice);
        Module.Commit();
        return Module;
    }
    PurchaseReturnDataModule CreatePurchaseReturn(PurchaseDeliveryNoteDataModule DeliveryModule, decimal Quantity)
    {
        PurchaseReturnDataModule Result = DeliveryModule.CreateReturn();
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Result.Commit();
        return Result;
    }
    PurchaseInvoiceDataModule CreatePurchaseInvoice(PurchaseDeliveryNoteDataModule DeliveryModule, decimal Quantity)
    {
        PurchaseInvoiceDataModule Result = DeliveryModule.CreateInvoice();
        Result.GetTable("TradeLine").Rows[0].SetValue("Quantity", Quantity);
        Result.Commit();
        return Result;
    }

    // ● construction
    public PurchaseDocumentTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    [Fact]
    public void NewPurchaseDocumentUsesConfiguredDefaults()
    {
        PurchaseOrderDataModule Module = DataRegistry.CreateModule("PurchaseOrder") as PurchaseOrderDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Order module.");

        Module.Insert();
        DataRow Line = Module.GetTable("TradeLine").AddNewRow();

        Assert.Equal(DataLib.GetDefaultWarehouseId(), Module.CurrentRow.AsString("WarehouseId"));
        Assert.Equal(DataLib.GetDefaultPurchaseCostCenterId(), Module.CurrentRow.AsString("CostCenterId"));
        Assert.Equal(DataLib.GetDefaultBranchId(), Module.CurrentRow.AsString("BranchId"));
        Assert.Equal(DataLib.GetDefaultPriceListTypeId(), Module.CurrentRow.AsString("PriceListTypeId"));
        Assert.Equal(DataLib.GetDefaultCurrencyId(), Module.CurrentRow.AsString("CurrencyId"));
        Assert.Equal(DataLib.GetDefaultPaymentMethodId(), Module.CurrentRow.AsString("PaymentMethodId"));
        Assert.Equal(DataLib.GetDefaultPaymentTermId(), Module.CurrentRow.AsString("PaymentTermId"));
        Assert.Equal(DataLib.GetDefaultTaxBusinessGroupId(), Module.CurrentRow.AsString("TaxBusinessGroupId"));
        Assert.Equal(DataLib.GetDefaultTaxJurisdictionId(), Module.CurrentRow.AsString("OriginTaxJurisdictionId"));
        Assert.Equal(DataLib.GetDefaultTaxJurisdictionId(), Module.CurrentRow.AsString("DestinationTaxJurisdictionId"));
        Assert.Equal(1m, Line.AsDecimal("Quantity"));
    }
    [Fact]
    public void PartialPurchaseInvoicesPreserveContextAndRemainIndependentFromReturns()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule DeliveryModule = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();

        PurchaseInvoiceDataModule FirstInvoice = CreatePurchaseInvoice(DeliveryModule, 4m);
        string FirstInvoiceLineId = FirstInvoice.GetTable("TradeLine").Rows[0].AsString("Id");

        Assert.Equal(DeliveryModule.CurrentRow.AsString("PersonId"), FirstInvoice.CurrentRow.AsString("PersonId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("BillingCountryId"), FirstInvoice.CurrentRow.AsString("BillingCountryId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("ShippingCountryId"), FirstInvoice.CurrentRow.AsString("ShippingCountryId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("DestinationTaxJurisdictionId"), FirstInvoice.CurrentRow.AsString("DestinationTaxJurisdictionId"));

        FirstInvoice.Post();
        PurchaseReturnDataModule ReturnModule = CreatePurchaseReturn(DeliveryModule, 3m);
        ReturnModule.Post();

        Assert.Equal(4m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(3m, GetTradeLine(DeliveryLineId).AsDecimal("ExecutedQuantity"));
        Assert.Null(GetStockMovement(FirstInvoiceLineId));

        PurchaseInvoiceDataModule SecondInvoice = DeliveryModule.CreateInvoice();
        Assert.Equal(6m, SecondInvoice.GetTable("TradeLine").Rows[0].AsDecimal("Quantity"));
        string SecondInvoiceLineId = SecondInvoice.GetTable("TradeLine").Rows[0].AsString("Id");
        SecondInvoice.Commit();
        SecondInvoice.Post();

        Assert.Equal(10m, GetTradeLine(DeliveryLineId).AsDecimal("InvoicedQuantity"));
        Assert.Equal(3m, GetTradeLine(DeliveryLineId).AsDecimal("ExecutedQuantity"));
        Assert.Null(GetStockMovement(SecondInvoiceLineId));
        Assert.Equal(7m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.False(DeliveryModule.HasRemainingInvoiceQuantity());
    }
    [Fact]
    public void PurchaseSupplierSelectionCopiesDocumentAddresses()
    {
        PurchaseDeliveryNoteDataModule Module = DataRegistry.CreateModule("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Delivery Note module.");

        Module.Insert();
        Module.CurrentRow.SetValue("PersonId", GetSupplierId());

        Assert.Equal("Billing Address", Module.CurrentRow.AsString("BillingName"));
        Assert.Equal("8 Piraeus Street", Module.CurrentRow.AsString("BillingAddressLine1"));
        Assert.Equal("Piraeus", Module.CurrentRow.AsString("BillingCity"));
        Assert.Equal("18531", Module.CurrentRow.AsString("BillingPostalCode"));
        Assert.Equal(GetCountryId(), Module.CurrentRow.AsString("BillingCountryId"));
        Assert.Equal("Shipping Address", Module.CurrentRow.AsString("ShippingName"));
        Assert.Equal("12 Industrial Road", Module.CurrentRow.AsString("ShippingAddressLine1"));
        Assert.Equal("Aspropyrgos", Module.CurrentRow.AsString("ShippingCity"));
        Assert.Equal("19300", Module.CurrentRow.AsString("ShippingPostalCode"));
        Assert.Equal(GetCountryId(), Module.CurrentRow.AsString("ShippingCountryId"));
    }
    [Fact]
    public void PurchaseDocumentRejectsMissingBillingAddress()
    {
        PurchaseDeliveryNoteDataModule Module = DataRegistry.CreateModule("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Delivery Note module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        AddLine(Module, "Espresso Beans", 1m, 20m);
        Module.CurrentRow.SetValue("BillingAddressLine1", DBNull.Value);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Commit());

        Assert.Contains("billing address line 1 is required", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PurchaseDeliveryNoteRejectsMissingShippingAddress()
    {
        PurchaseDeliveryNoteDataModule Module = DataRegistry.CreateModule("PurchaseDeliveryNote") as PurchaseDeliveryNoteDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the Purchase Delivery Note module.");

        Module.Insert();
        ConfigurePurchaseDocument(Module);
        AddLine(Module, "Espresso Beans", 1m, 20m);
        Module.CurrentRow.SetValue("ShippingAddressLine1", DBNull.Value);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Commit());

        Assert.Contains("shipping address line 1 is required", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PurchaseReturnTransformationCopiesSupplierAndTaxContext()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule DeliveryModule = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        DeliveryModule.Post();

        PurchaseReturnDataModule ReturnModule = DeliveryModule.CreateReturn();

        Assert.Equal(DeliveryModule.CurrentRow.AsString("PersonId"), ReturnModule.CurrentRow.AsString("PersonId"));
        Assert.Equal("SUP-HELIOS", ReturnModule.CurrentRow.AsString("Person__Code"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("TaxBusinessGroupId"), ReturnModule.CurrentRow.AsString("TaxBusinessGroupId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("OriginTaxJurisdictionId"), ReturnModule.CurrentRow.AsString("OriginTaxJurisdictionId"));
        Assert.Equal(GetTaxJurisdictionId(), ReturnModule.CurrentRow.AsString("DestinationTaxJurisdictionId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("BillingCountryId"), ReturnModule.CurrentRow.AsString("BillingCountryId"));
        Assert.Equal(DeliveryModule.CurrentRow.AsString("ShippingCountryId"), ReturnModule.CurrentRow.AsString("ShippingCountryId"));
    }
    [Fact]
    public void PurchaseDocumentRejectsZeroUnitPriceByDefault()
    {
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => CreatePurchaseDeliveryNote("Espresso Beans", 1m, 0m));

        Assert.Contains("unit price must be greater than zero", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PostingPurchaseDeliveryNoteCreatesIncomingStock()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        string TradeId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Trade = GetTrade(TradeId);
        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal((int)TradeStatus.Posted, Trade.AsInteger("TradeStatusId"));
        Assert.True(Trade.AsBoolean("IsLocked"));
        Assert.False(Trade.AsString("Code").StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase));
        Assert.False(Sys.IsNull(Trade["PostingDate"]));
        Assert.False(Sys.IsNull(Trade["PostedAt"]));
        Assert.Equal(Sys.Context.CurrentUser.Id, Trade.AsString("PostedBy"));
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(10m, Movement.AsDecimal("PrimaryQuantity"));
        Assert.Equal(20m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(200m, Movement.AsDecimal("CostAmount"));
        Assert.Equal("PurchaseDeliveryNote", Movement.AsString("SourceModule"));
        Assert.Equal(10m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(200m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(20m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PartialPurchaseReceiptsUpdateExecutedQuantity()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseOrderDataModule OrderModule = CreatePurchaseOrder("Espresso Beans", 10m, 20m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        PurchaseDeliveryNoteDataModule FirstReceipt = CreatePurchaseDeliveryNote(OrderModule, 4m);
        FirstReceipt.Post();

        Assert.Equal(4m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));

        PurchaseDeliveryNoteDataModule SecondReceipt = CreatePurchaseDeliveryNote(OrderModule, 6m);
        SecondReceipt.Post();

        Assert.Equal(10m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));
        Assert.Equal(10m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void MultiLinePurchaseOrderCompletesAfterAllLinesAreReceived()
    {
        PurchaseOrderDataModule OrderModule = CreatePurchaseOrder(
            ("Espresso Beans", 10m, 20m),
            ("Orange Juice", 5m, 10m));
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string EspressoLineId = GetModuleLineId(OrderModule, "Espresso Beans");
        string OrangeJuiceLineId = GetModuleLineId(OrderModule, "Orange Juice");
        OrderModule.Post();

        PurchaseDeliveryNoteDataModule FirstReceipt = CreatePurchaseDeliveryNote(OrderModule, ("Espresso Beans", 10m));
        FirstReceipt.Post();

        Assert.Equal(10m, GetTradeLine(EspressoLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(0m, GetTradeLine(OrangeJuiceLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));

        PurchaseDeliveryNoteDataModule SecondReceipt = CreatePurchaseDeliveryNote(OrderModule, ("Orange Juice", 5m));
        SecondReceipt.Post();

        Assert.Equal(5m, GetTradeLine(OrangeJuiceLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Completed, GetTrade(OrderId).AsInteger("TradeStatusId"));
    }
    [Fact]
    public void PurchaseReceiptQuantityCannotExceedRemainingQuantity()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseOrderDataModule OrderModule = CreatePurchaseOrder("Espresso Beans", 10m, 20m);
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string OrderLineId = OrderModule.GetTable("TradeLine").Rows[0].AsString("Id");
        OrderModule.Post();

        PurchaseDeliveryNoteDataModule FirstReceipt = CreatePurchaseDeliveryNote(OrderModule, 6m);
        FirstReceipt.Post();

        PurchaseDeliveryNoteDataModule ExcessReceipt = CreatePurchaseDeliveryNote(OrderModule, 5m);
        string ReceiptId = ExcessReceipt.CurrentRow.AsString("Id");
        string ReceiptLineId = ExcessReceipt.GetTable("TradeLine").Rows[0].AsString("Id");

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ExcessReceipt.Post());

        Assert.Contains("exceeds remaining quantity 4", Error.Message.ToLowerInvariant());
        Assert.Equal(6m, GetTradeLine(OrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
        Assert.Equal((int)TradeStatus.Draft, GetTrade(ReceiptId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(ReceiptLineId));
        Assert.Equal(6m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void FailedMultiLinePurchaseReceiptRollsBackSourceAndStock()
    {
        DataRow Espresso = GetProduct("Espresso Beans");
        DataRow OrangeJuice = GetProduct("Orange Juice");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Espresso.AsString("Id"), WarehouseId, 0m, 0m);
        SetStockBalance(OrangeJuice.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseOrderDataModule OrderModule = CreatePurchaseOrder(
            ("Espresso Beans", 10m, 20m),
            ("Orange Juice", 5m, 10m));
        string OrderId = OrderModule.CurrentRow.AsString("Id");
        string EspressoOrderLineId = GetModuleLineId(OrderModule, "Espresso Beans");
        string OrangeJuiceOrderLineId = GetModuleLineId(OrderModule, "Orange Juice");
        OrderModule.Post();

        PurchaseDeliveryNoteDataModule Receipt = CreatePurchaseDeliveryNote(
            OrderModule,
            ("Espresso Beans", 4m),
            ("Orange Juice", 6m));
        string ReceiptId = Receipt.CurrentRow.AsString("Id");
        string EspressoReceiptLineId = GetModuleLineId(Receipt, "Espresso Beans");
        string OrangeJuiceReceiptLineId = GetModuleLineId(Receipt, "Orange Juice");

        Assert.Throws<TripousBusinessException>(() => Receipt.Post());

        Assert.Equal((int)TradeStatus.Posted, GetTrade(OrderId).AsInteger("TradeStatusId"));
        Assert.Equal((int)TradeStatus.Draft, GetTrade(ReceiptId).AsInteger("TradeStatusId"));
        Assert.Equal(0m, GetTradeLine(EspressoOrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal(0m, GetTradeLine(OrangeJuiceOrderLineId).AsDecimal("ExecutedQuantity"));
        Assert.Null(GetStockMovement(EspressoReceiptLineId));
        Assert.Null(GetStockMovement(OrangeJuiceReceiptLineId));
        Assert.Equal(0m, GetStockBalance(Espresso.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, GetStockBalance(OrangeJuice.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void PostingPurchaseDeliveryNoteUpdatesMovingAverageCost()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 10m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m, 10m);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(18m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(180m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(20m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(280m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(14m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PostingPurchaseDeliveryNoteUsesPrimaryUnitQuantity()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote("Espresso Beans", 5m, 20m, 0m, 2m);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(5m, Movement.AsDecimal("Quantity"));
        Assert.Equal(10m, Movement.AsDecimal("PrimaryQuantity"));
        Assert.Equal(10m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(100m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(10m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(100m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(10m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PostingPurchaseDeliveryNotePreservesNetCostAfterUnitCostRounding()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote("Espresso Beans", 1m, 100m, 0m, 3m);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(33.3333m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(100m, Movement.AsDecimal("CostAmount"));
        Assert.Equal(3m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(100m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(33.3333m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PostingPurchaseDeliveryNoteTwiceDoesNotDuplicateStock()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Post());
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal("Only draft documents can be posted.", Error.Message);
        Assert.Equal(1, GetStockMovementCount(LineId));
        Assert.Equal(10m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(200m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(20m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void FailedMultiLinePurchaseDeliveryNoteRollsBackAllStockChanges()
    {
        DataRow[] Products =
        [
            GetProduct("Espresso Beans"),
            GetProduct("Orange Juice"),
        ];
        Products = Products.OrderBy(Product => Product.AsString("Id")).ToArray();
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Products[0].AsString("Id"), WarehouseId, 0m, 0m);
        SetStockBalance(Products[1].AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule Module = CreatePurchaseDeliveryNote(
            (Products[0].AsString("Name"), 10m, 20m),
            (Products[1].AsString("Name"), 0m, 10m));
        string TradeId = Module.CurrentRow.AsString("Id");
        DataRow FirstLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[0].AsString("Id")));
        DataRow SecondLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[1].AsString("Id")));

        Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Equal((int)TradeStatus.Draft, GetTrade(TradeId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(FirstLine.AsString("Id")));
        Assert.Null(GetStockMovement(SecondLine.AsString("Id")));
        Assert.Equal(0m, GetStockBalance(Products[0].AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, GetStockBalance(Products[1].AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void PostingPurchaseReturnCreatesOutgoingStockAtAverageCost()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 10m, 12m);
        PurchaseReturnDataModule Module = CreatePurchaseReturn("Espresso Beans", 4m, 20m);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(4m, Movement.AsDecimal("PrimaryQuantity"));
        Assert.Equal(12m, Movement.AsDecimal("UnitCost"));
        Assert.Equal(48m, Movement.AsDecimal("CostAmount"));
        Assert.Equal("PurchaseReturn", Movement.AsString("SourceModule"));
        Assert.Equal(6m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(72m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(12m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PartialPurchaseReturnsUpdateSourceDeliveryQuantity()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule DeliveryModule = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        string DeliveryId = DeliveryModule.CurrentRow.AsString("Id");
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();

        PurchaseReturnDataModule FirstReturn = CreatePurchaseReturn(DeliveryModule, 4m);
        FirstReturn.Post();

        Assert.Equal(4m, GetTradeLine(DeliveryLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(DeliveryId).AsInteger("TradeStatusId"));
        Assert.Equal(6m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));

        PurchaseReturnDataModule SecondReturn = CreatePurchaseReturn(DeliveryModule, 6m);
        SecondReturn.Post();

        Assert.Equal(10m, GetTradeLine(DeliveryLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Posted, GetTrade(DeliveryId).AsInteger("TradeStatusId"));
        Assert.Equal(0m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.False(DeliveryModule.HasRemainingTransformQuantity());

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => DeliveryModule.CreateReturn());
        Assert.Contains("no remaining quantity", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PurchaseReturnQuantityCannotExceedRemainingDeliveryQuantity()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseDeliveryNoteDataModule DeliveryModule = CreatePurchaseDeliveryNote("Espresso Beans", 10m, 20m);
        string DeliveryLineId = DeliveryModule.GetTable("TradeLine").Rows[0].AsString("Id");
        DeliveryModule.Post();

        PurchaseReturnDataModule ReturnModule = CreatePurchaseReturn(DeliveryModule, 11m);
        string ReturnId = ReturnModule.CurrentRow.AsString("Id");
        string ReturnLineId = ReturnModule.GetTable("TradeLine").Rows[0].AsString("Id");
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => ReturnModule.Post());

        Assert.Contains("exceeds remaining quantity 10", Error.Message.ToLowerInvariant());
        Assert.Equal(0m, GetTradeLine(DeliveryLineId).AsDecimal("ExecutedQuantity"));
        Assert.Equal((int)TradeStatus.Draft, GetTrade(ReturnId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(ReturnLineId));
        Assert.Equal(10m, GetStockBalance(Product.AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
    }
    [Fact]
    public void PostingPurchaseReturnClearsCostWhenStockBecomesZero()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 3m, 100m, 33.3333m);
        PurchaseReturnDataModule Module = CreatePurchaseReturn("Espresso Beans", 3m, 20m);

        Module.Post();

        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(0m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(0m, Balance.AsDecimal("AverageUnitCost"));
    }
    [Fact]
    public void PostingPurchaseReturnRejectsNegativeStock()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Product.AsString("Id"), WarehouseId, 5m, 12m);
        PurchaseReturnDataModule Module = CreatePurchaseReturn("Espresso Beans", 6m, 20m);
        string TradeId = Module.CurrentRow.AsString("Id");
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Post());

        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Contains("cannot become negative", Error.Message.ToLowerInvariant());
        Assert.Equal((int)TradeStatus.Draft, GetTrade(TradeId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(LineId));
        Assert.Equal(5m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(60m, Balance.AsDecimal("TotalCostAmount"));
    }
    [Fact]
    public void FailedMultiLinePurchaseReturnRollsBackAllStockChanges()
    {
        DataRow[] Products =
        [
            GetProduct("Espresso Beans"),
            GetProduct("Orange Juice"),
        ];
        Products = Products.OrderBy(Product => Product.AsString("Id")).ToArray();
        string WarehouseId = DataLib.GetDefaultWarehouseId();
        SetStockBalance(Products[0].AsString("Id"), WarehouseId, 10m, 12m);
        SetStockBalance(Products[1].AsString("Id"), WarehouseId, 1m, 8m);
        PurchaseReturnDataModule Module = CreatePurchaseReturn(
            (Products[0].AsString("Name"), 4m, 20m),
            (Products[1].AsString("Name"), 2m, 10m));
        string TradeId = Module.CurrentRow.AsString("Id");
        DataRow FirstLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[0].AsString("Id")));
        DataRow SecondLine = Module.GetTable("TradeLine").Rows.Cast<DataRow>()
            .Single(Row => Row.AsString("ProductId").IsSameText(Products[1].AsString("Id")));

        Assert.Throws<TripousBusinessException>(() => Module.Post());

        Assert.Equal((int)TradeStatus.Draft, GetTrade(TradeId).AsInteger("TradeStatusId"));
        Assert.Null(GetStockMovement(FirstLine.AsString("Id")));
        Assert.Null(GetStockMovement(SecondLine.AsString("Id")));
        Assert.Equal(10m, GetStockBalance(Products[0].AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.Equal(120m, GetStockBalance(Products[0].AsString("Id"), WarehouseId).AsDecimal("TotalCostAmount"));
        Assert.Equal(1m, GetStockBalance(Products[1].AsString("Id"), WarehouseId).AsDecimal("PrimaryQuantity"));
        Assert.Equal(8m, GetStockBalance(Products[1].AsString("Id"), WarehouseId).AsDecimal("TotalCostAmount"));
    }
    [Fact]
    public void PostingPurchaseReturnAllowsNegativeStockWhenConfigured()
    {
        DataRow Product = GetProduct("Espresso Beans");
        string WarehouseId = GetWarehouseId("Scrap / Damaged Stock");
        SetStockBalance(Product.AsString("Id"), WarehouseId, 0m, 0m);
        PurchaseReturnDataModule Module = CreatePurchaseReturn("Espresso Beans", 6m, 20m, WarehouseId);
        string LineId = Module.GetTable("TradeLine").Rows[0].AsString("Id");

        Module.Post();

        DataRow Movement = GetStockMovement(LineId);
        DataRow Balance = GetStockBalance(Product.AsString("Id"), WarehouseId);
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(-6m, Balance.AsDecimal("PrimaryQuantity"));
        Assert.Equal(0m, Balance.AsDecimal("TotalCostAmount"));
        Assert.Equal(0m, Balance.AsDecimal("AverageUnitCost"));
    }
}

namespace tERP.Data;

[TypeStore]
public class AppDefaultProperties
{
    public AppDefaultProperties()
    {
    }

    public SalesDefaults Sales { get; set; } = new();
}

[TypeStore]
public class SalesDefaults
{
    public SalesDefaults()
    {
    }
 
    public string WarehouseId { get; set; } = DataLib.GetDefaultWarehouseId();
    public string CostCenterId { get; set; } = DataLib.GetDefaultSalesCostCenterId();
    public string BranchId { get; set; } = DataLib.GetDefaultBranchId();
    public string CurrencyId { get; set; } = DataLib.GetDefaultCurrencyId();
    public string PaymentMethodId { get; set; } = DataLib.GetDefaultPaymentMethodId();
    public string PaymentTermId { get; set; } = DataLib.GetDefaultPaymentTermId();
    public string PriceListTypeId { get; set; } = DataLib.GetDefaultPriceListTypeId();
    public decimal DefaultQuantity { get; set; } = 1;
    public List<string> TradeLineGridFields { get; set; } = [
        "DisplayOrder",
        "LineTypeId",
        "ProductCode",
        "ProductName",
        "UnitOfMeasureId",
        "Quantity",
        "UnitPrice",
        "GrossAmount",
        "DiscountPercent",
        "DiscountAmount",
        "NetAmount",
        "VatRatePercent",
        "VatAmount",
        "TotalAmount",    
    ];
}


 
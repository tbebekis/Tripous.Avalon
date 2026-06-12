namespace tERP.Data;

[TypeStore]
public class AppDefaultProperties
{
    public AppDefaultProperties()
    {
    }

    public SalesDefaults Sales { get; set; } = new();
    public PurchaseDefaults Purchase { get; set; } = new();
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
    public string TaxBusinessGroupId { get; set; } = DataLib.GetDefaultTaxBusinessGroupId();
    public string OriginTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    public string DestinationTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    public decimal DefaultQuantity { get; set; } = 1;
    public bool AllowZeroUnitPrice { get; set; } = false;
    public string PriceResolverClassName { get; set; } = typeof(PriceResolver).FullName;
    public string TaxResolverClassName { get; set; } = typeof(TaxResolver).FullName;
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
        "DocumentDiscountAmount",
        "NetAmount",
        "TaxPercent",
        "TaxAmount",
        "TotalAmount",
    ];
}

[TypeStore]
public class PurchaseDefaults
{
    // ● construction
    public PurchaseDefaults()
    {
    }

    // ● properties
    public string WarehouseId { get; set; } = DataLib.GetDefaultWarehouseId();
    public string CostCenterId { get; set; } = DataLib.GetDefaultPurchaseCostCenterId();
    public string BranchId { get; set; } = DataLib.GetDefaultBranchId();
    public string CurrencyId { get; set; } = DataLib.GetDefaultCurrencyId();
    public string PaymentMethodId { get; set; } = DataLib.GetDefaultPaymentMethodId();
    public string PaymentTermId { get; set; } = DataLib.GetDefaultPaymentTermId();
    public string PriceListTypeId { get; set; } = DataLib.GetDefaultPriceListTypeId();
    public string TaxBusinessGroupId { get; set; } = DataLib.GetDefaultTaxBusinessGroupId();
    public string OriginTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    public string DestinationTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    public decimal DefaultQuantity { get; set; } = 1;
    public bool AllowZeroUnitPrice { get; set; } = false;
    public string PriceResolverClassName { get; set; } = typeof(PriceResolver).FullName;
    public string TaxResolverClassName { get; set; } = typeof(TaxResolver).FullName;
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
        "DocumentDiscountAmount",
        "NetAmount",
        "TaxPercent",
        "TaxAmount",
        "TotalAmount",
    ];
}

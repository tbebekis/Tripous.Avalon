namespace tERP.Data;

/// <summary>
/// Contains default values used by purchase documents.
/// </summary>
[TypeStore]
public class PurchaseDefaults
{
    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public PurchaseDefaults()
    {
    }

    // ● properties
    /// <summary>
    /// Default warehouse identifier.
    /// </summary>
    public string WarehouseId { get; set; } = DataLib.GetDefaultWarehouseId();
    /// <summary>
    /// Default purchase cost center identifier.
    /// </summary>
    public string CostCenterId { get; set; } = DataLib.GetDefaultPurchaseCostCenterId();
    /// <summary>
    /// Default branch identifier.
    /// </summary>
    public string BranchId { get; set; } = DataLib.GetDefaultBranchId();
    /// <summary>
    /// Default currency identifier.
    /// </summary>
    public string CurrencyId { get; set; } = DataLib.GetDefaultCurrencyId();
    /// <summary>
    /// Default payment method identifier.
    /// </summary>
    public string PaymentMethodId { get; set; } = DataLib.GetDefaultPaymentMethodId();
    /// <summary>
    /// Default payment term identifier.
    /// </summary>
    public string PaymentTermId { get; set; } = DataLib.GetDefaultPaymentTermId();
    /// <summary>
    /// Default price list type identifier.
    /// </summary>
    public string PriceListTypeId { get; set; } = DataLib.GetDefaultPriceListTypeId();
    /// <summary>
    /// Default tax business group identifier.
    /// </summary>
    public string TaxBusinessGroupId { get; set; } = DataLib.GetDefaultTaxBusinessGroupId();
    /// <summary>
    /// Default origin tax jurisdiction identifier.
    /// </summary>
    public string OriginTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    /// <summary>
    /// Default destination tax jurisdiction identifier.
    /// </summary>
    public string DestinationTaxJurisdictionId { get; set; } = DataLib.GetDefaultTaxJurisdictionId();
    /// <summary>
    /// Default line quantity.
    /// </summary>
    public decimal DefaultQuantity { get; set; } = 1;
    /// <summary>
    /// Indicates whether zero unit prices are allowed.
    /// </summary>
    public bool AllowZeroUnitPrice { get; set; } = false;
    /// <summary>
    /// Price resolver class name.
    /// </summary>
    public string PriceResolverClassName { get; set; } = typeof(PriceResolver).FullName;
    /// <summary>
    /// Tax resolver class name.
    /// </summary>
    public string TaxResolverClassName { get; set; } = typeof(TaxResolver).FullName;
    /// <summary>
    /// Trade line grid field names.
    /// </summary>
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

namespace tERP.Data;

public partial class RegistryVersion1: RegistryVersion
{
    // ● public
    public override void RegisterLookups()
    {
        DataRegistry.AddOrGetLookupWithTableName("AssetCategory", "AssetCategory", FormName: "AssetCategory");
        DataRegistry.AddOrGetLookupWithTableName("AssetDepreciationMethod", "AssetDepreciationMethod", FormName: "AssetDepreciationMethod");
        DataRegistry.AddOrGetLookupWithTableName("AssetLocation", "AssetLocation", FormName: "AssetLocation");
        DataRegistry.AddOrGetLookupWithTableName("Bank", "Bank", FormName: "Bank");
        DataRegistry.AddOrGetLookupWithTableName("Carrier", "Carrier", FormName: "Carrier");
        DataRegistry.AddOrGetLookupWithTableName("Category", "Category", FormName: "Category");
        DataRegistry.AddOrGetLookupWithTableName("Company", "Company", FormName: "Company");
        DataRegistry.AddOrGetLookupWithTableName("CompanyBranch", "CompanyBranch");
        DataRegistry.AddOrGetLookupWithTableName("ContactType", "ContactType", FormName: "ContactType");
        DataRegistry.AddOrGetLookupWithTableName("CostCenter", "CostCenter", FormName: "CostCenter");
        DataRegistry.AddOrGetLookupWithTableName("Country", "Country", FormName: "Country");
        DataRegistry.AddOrGetLookupWithTableName("Currency", "Currency", FormName: "Currency");
        DataRegistry.AddOrGetLookupWithTableName("CustomerCategory", "CustomerCategory", FormName: "CustomerCategory");
        DataRegistry.AddOrGetLookupWithTableName("DiscountCategory", "DiscountCategory", FormName: "DiscountCategory");
        DataRegistry.AddOrGetLookupWithTableName("ExpenseCategory", "ExpenseCategory", FormName: "ExpenseCategory");
        DataRegistry.AddOrGetLookupWithTableName("Language", "Language", FormName: "Language");
        DataRegistry.AddOrGetLookupWithTableName("PaymentMethod", "PaymentMethod", FormName: "PaymentMethod");
        DataRegistry.AddOrGetLookupWithTableName("PaymentTerm", "PaymentTerm", FormName: "PaymentTerm");
        DataRegistry.AddOrGetLookupWithTableName("PersonRoleType", "PersonRoleType", FormName: "PersonRoleType");
        DataRegistry.AddOrGetLookupWithTableName("PriceListType", "PriceListType", FormName: "PriceListType");
        DataRegistry.AddOrGetLookupWithTableName("ProductAttributeGroup", "ProductAttributeGroup", FormName: "ProductAttributeGroup");
        DataRegistry.AddOrGetLookupWithTableName("ProductBrand", "ProductBrand", FormName: "ProductBrand");
        DataRegistry.AddOrGetLookupWithTableName("ProductDimension", "ProductDimension", FormName: "ProductDimension");
        DataRegistry.AddOrGetLookupWithTableName("ProductGroup", "ProductGroup", FormName: "ProductGroup");
        DataRegistry.AddOrGetLookupWithTableName("SalesPerson", "SalesPerson", FormName: "SalesPerson");
        DataRegistry.AddOrGetLookupWithTableName("SupplierCategory", "SupplierCategory", FormName: "SupplierCategory");
        DataRegistry.AddOrGetLookupWithTableName("SYS_NUMBER_SERIES", "SYS_NUMBER_SERIES", FormName: "NumberSeries");
        DataRegistry.AddOrGetLookupWithTableName("TaxCategory", "TaxCategory", FormName: "TaxCategory");
        DataRegistry.AddOrGetLookupWithTableName("TaxOffice", "TaxOffice", FormName: "TaxOffice");
        DataRegistry.AddOrGetLookupWithTableName("UnitOfMeasure", "UnitOfMeasure", FormName: "UnitOfMeasure");
        DataRegistry.AddOrGetLookupWithTableName("VatRate", "VatRate", FormName: "VatRate");
        DataRegistry.AddOrGetLookupWithTableName("Warehouse", "Warehouse", FormName: "Warehouse");
    }
}
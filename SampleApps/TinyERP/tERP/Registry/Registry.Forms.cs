/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class Registry
{
    // ● static public
    static public void RegisterForms()
    {
        if (!DesktopRegistry.Forms.Contains("AssetCategory"))
            DesktopRegistry.AddForm("AssetCategory", TitleKey: "AssetCategory", Module: "AssetCategory", Group: "Assets");
        if (!DesktopRegistry.Forms.Contains("AssetDepreciationMethod"))
            DesktopRegistry.AddForm("AssetDepreciationMethod", TitleKey: "AssetDepreciationMethod", Module: "AssetDepreciationMethod", Group: "Assets");
        if (!DesktopRegistry.Forms.Contains("AssetLocation"))
            DesktopRegistry.AddForm("AssetLocation", TitleKey: "AssetLocation", Module: "AssetLocation", Group: "Assets");
        if (!DesktopRegistry.Forms.Contains("Bank"))
            DesktopRegistry.AddForm("Bank", TitleKey: "Bank", Module: "Bank", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("Carrier"))
            DesktopRegistry.AddForm("Carrier", TitleKey: "Carrier", Module: "Carrier", Group: "Purchases");
        if (!DesktopRegistry.Forms.Contains("CashAccount"))
            DesktopRegistry.AddForm("CashAccount", TitleKey: "CashAccount", Module: "CashAccount", Group: "Finance");
        if (!DesktopRegistry.Forms.Contains("Category"))
            DesktopRegistry.AddForm("Category", TitleKey: "Category", Module: "Category", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("Company"))
            DesktopRegistry.AddForm("Company", TitleKey: "Company", Module: "Company", Group: "Company");
        if (!DesktopRegistry.Forms.Contains("ContactType"))
            DesktopRegistry.AddForm("ContactType", TitleKey: "ContactType", Module: "ContactType", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("CostCenter"))
            DesktopRegistry.AddForm("CostCenter", TitleKey: "CostCenter", Module: "CostCenter", Group: "Company");
        if (!DesktopRegistry.Forms.Contains("Country"))
            DesktopRegistry.AddForm("Country", TitleKey: "Country", Module: "Country", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("Currency"))
            DesktopRegistry.AddForm("Currency", TitleKey: "Currency", Module: "Currency", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("CustomerCategory"))
            DesktopRegistry.AddForm("CustomerCategory", TitleKey: "CustomerCategory", Module: "CustomerCategory", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("DiscountCategory"))
            DesktopRegistry.AddForm("DiscountCategory", TitleKey: "DiscountCategory", Module: "DiscountCategory", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("DocumentType"))
            DesktopRegistry.AddForm("DocumentType", TitleKey: "DocumentType", Module: "DocumentType", Group: "Documents");
        if (!DesktopRegistry.Forms.Contains("ExpenseCategory"))
            DesktopRegistry.AddForm("ExpenseCategory", TitleKey: "ExpenseCategory", Module: "ExpenseCategory", Group: "Accounting");
        if (!DesktopRegistry.Forms.Contains("FiscalYear"))
            DesktopRegistry.AddForm("FiscalYear", TitleKey: "FiscalYear", Module: "FiscalYear", Group: "Company");
        if (!DesktopRegistry.Forms.Contains("FixedAsset"))
            DesktopRegistry.AddForm("FixedAsset", TitleKey: "FixedAsset", Module: "FixedAsset", Group: "Assets");
        if (!DesktopRegistry.Forms.Contains("Language"))
            DesktopRegistry.AddForm("Language", TitleKey: "Language", Module: "Language", Group: "System");
        if (!DesktopRegistry.Forms.Contains("Log"))
            DesktopRegistry.AddForm("Log", TitleKey: "Log", Module: "Log", Group: "System", IsReadOnly: true);
        if (!DesktopRegistry.Forms.Contains("NumberSeries"))
            DesktopRegistry.AddForm("NumberSeries", TitleKey: "NumberSeries", Module: "NumberSeries", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("PaymentMethod"))
            DesktopRegistry.AddForm("PaymentMethod", TitleKey: "PaymentMethod", Module: "PaymentMethod", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("PaymentTerm"))
            DesktopRegistry.AddForm("PaymentTerm", TitleKey: "PaymentTerm", Module: "PaymentTerm", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("Person"))
            DesktopRegistry.AddForm("Person", TitleKey: "Person", Module: "Person", Group: "People");
        if (!DesktopRegistry.Forms.Contains("PersonRoleType"))
            DesktopRegistry.AddForm("PersonRoleType", TitleKey: "PersonRoleType", Module: "PersonRoleType", Group: "People");
        if (!DesktopRegistry.Forms.Contains("PriceList"))
            DesktopRegistry.AddForm("PriceList", TitleKey: "PriceList", Module: "PriceList", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("PriceListType"))
            DesktopRegistry.AddForm("PriceListType", TitleKey: "PriceListType", Module: "PriceListType", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("Product"))
            DesktopRegistry.AddForm("Product", TitleKey: "Product", Module: "Product", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("ProductAttributeGroup"))
            DesktopRegistry.AddForm("ProductAttributeGroup", TitleKey: "ProductAttributeGroup", Module: "ProductAttributeGroup", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("ProductBrand"))
            DesktopRegistry.AddForm("ProductBrand", TitleKey: "ProductBrand", Module: "ProductBrand", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("ProductDimension"))
            DesktopRegistry.AddForm("ProductDimension", TitleKey: "ProductDimension", Module: "ProductDimension", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("ProductGroup"))
            DesktopRegistry.AddForm("ProductGroup", TitleKey: "ProductGroup", Module: "ProductGroup", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("Project"))
            DesktopRegistry.AddForm("Project", TitleKey: "Project", Module: "Project", Group: "Projects");
        if (!DesktopRegistry.Forms.Contains("SalesPerson"))
            DesktopRegistry.AddForm("SalesPerson", TitleKey: "SalesPerson", Module: "SalesPerson", Group: "Sales");
        if (!DesktopRegistry.Forms.Contains("StockReason"))
            DesktopRegistry.AddForm("StockReason", TitleKey: "StockReason", Module: "StockReason", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("SupplierCategory"))
            DesktopRegistry.AddForm("SupplierCategory", TitleKey: "SupplierCategory", Module: "SupplierCategory", Group: "Purchases");
        if (!DesktopRegistry.Forms.Contains("TaxCategory"))
            DesktopRegistry.AddForm("TaxCategory", TitleKey: "TaxCategory", Module: "TaxCategory", Group: "Accounting");
        if (!DesktopRegistry.Forms.Contains("TaxOffice"))
            DesktopRegistry.AddForm("TaxOffice", TitleKey: "TaxOffice", Module: "TaxOffice", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("UnitOfMeasure"))
            DesktopRegistry.AddForm("UnitOfMeasure", TitleKey: "UnitOfMeasure", Module: "UnitOfMeasure", Group: "Inventory");
        if (!DesktopRegistry.Forms.Contains("VatRate"))
            DesktopRegistry.AddForm("VatRate", TitleKey: "VatRate", Module: "VatRate", Group: "Setup");
        if (!DesktopRegistry.Forms.Contains("Warehouse"))
            DesktopRegistry.AddForm("Warehouse", TitleKey: "Warehouse", Module: "Warehouse", Group: "Inventory");
    }
}
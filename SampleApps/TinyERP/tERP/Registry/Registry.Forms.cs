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
        DesktopRegistry.AddOrGetForm("Account", TitleKey: "Account", Module: "Account", Group: "Accounting");
        DesktopRegistry.AddOrGetForm("AppUser", TitleKey: "AppUser", Module: "AppUser", Group: "Setup");
        DesktopRegistry.AddOrGetForm("Asset", TitleKey: "Asset", Module: "Asset", Group: "Assets");
        DesktopRegistry.AddOrGetForm("AssetCategory", TitleKey: "AssetCategory", Module: "AssetCategory", Group: "Assets");
        DesktopRegistry.AddOrGetForm("AssetDepreciationMethod", TitleKey: "AssetDepreciationMethod", Module: "AssetDepreciationMethod", Group: "Assets");
        DesktopRegistry.AddOrGetForm("AssetLocation", TitleKey: "AssetLocation", Module: "AssetLocation", Group: "Assets");
        DesktopRegistry.AddOrGetForm("Bank", TitleKey: "Bank", Module: "Bank", Group: "Setup");
        DesktopRegistry.AddOrGetForm("Carrier", TitleKey: "Carrier", Module: "Carrier", Group: "Purchases");
        DesktopRegistry.AddOrGetForm("CashAccount", TitleKey: "CashAccount", Module: "CashAccount", Group: "Finance");
        DesktopRegistry.AddOrGetForm("Category", TitleKey: "Category", Module: "Category", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("Company", TitleKey: "Company", Module: "Company", Group: "Company");
        DesktopRegistry.AddOrGetForm("ContactType", TitleKey: "ContactType", Module: "ContactType", Group: "Setup");
        DesktopRegistry.AddOrGetForm("CostCenter", TitleKey: "CostCenter", Module: "CostCenter", Group: "Company");
        DesktopRegistry.AddOrGetForm("Country", TitleKey: "Country", Module: "Country", Group: "Setup");
        DesktopRegistry.AddOrGetForm("Currency", TitleKey: "Currency", Module: "Currency", Group: "Setup");
        DesktopRegistry.AddOrGetForm("CustomerCategory", TitleKey: "CustomerCategory", Module: "CustomerCategory", Group: "Sales");
        DesktopRegistry.AddOrGetForm("DiscountCategory", TitleKey: "DiscountCategory", Module: "DiscountCategory", Group: "Sales");
        DesktopRegistry.AddOrGetForm("DocumentType", TitleKey: "DocumentType", Module: "DocumentType", Group: "Documents");
        DesktopRegistry.AddOrGetForm("ExpenseCategory", TitleKey: "ExpenseCategory", Module: "ExpenseCategory", Group: "Accounting");
        DesktopRegistry.AddOrGetForm("FinanceBalance", TitleKey: "FinanceBalance", Module: "FinanceBalance", Group: "Finance", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("FinanceMovement", TitleKey: "FinanceMovement", Module: "FinanceMovement", Group: "Finance", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("FiscalYear", TitleKey: "FiscalYear", Module: "FiscalYear", Group: "Company");
        DesktopRegistry.AddOrGetForm("FixedAsset", TitleKey: "FixedAsset", Module: "FixedAsset", Group: "Assets");
        DesktopRegistry.AddOrGetForm("JournalEntry", TitleKey: "JournalEntry", Module: "JournalEntry", Group: "Accounting");
        DesktopRegistry.AddOrGetForm("Language", TitleKey: "Language", Module: "Language", Group: "System");
        DesktopRegistry.AddOrGetForm("Log", TitleKey: "Log", Module: "Log", Group: "System", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("NumberSeries", TitleKey: "NumberSeries", Module: "NumberSeries", Group: "Setup");
        DesktopRegistry.AddOrGetForm("PaymentMethod", TitleKey: "PaymentMethod", Module: "PaymentMethod", Group: "Sales");
        DesktopRegistry.AddOrGetForm("PaymentTerm", TitleKey: "PaymentTerm", Module: "PaymentTerm", Group: "Sales");
        DesktopRegistry.AddOrGetForm("Person", TitleKey: "Person", Module: "Person", Group: "People");
        DesktopRegistry.AddOrGetForm("PersonRoleType", TitleKey: "PersonRoleType", Module: "PersonRoleType", Group: "People");
        DesktopRegistry.AddOrGetForm("PriceList", TitleKey: "PriceList", Module: "PriceList", Group: "Sales");
        DesktopRegistry.AddOrGetForm("PriceListType", TitleKey: "PriceListType", Module: "PriceListType", Group: "Sales");
        DesktopRegistry.AddOrGetForm("Product", TitleKey: "Product", Module: "Product", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("ProductAttributeGroup", TitleKey: "ProductAttributeGroup", Module: "ProductAttributeGroup", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("ProductBrand", TitleKey: "ProductBrand", Module: "ProductBrand", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("ProductDimension", TitleKey: "ProductDimension", Module: "ProductDimension", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("ProductGroup", TitleKey: "ProductGroup", Module: "ProductGroup", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("Project", TitleKey: "Project", Module: "Project", Group: "Projects");
        DesktopRegistry.AddOrGetForm("PurchaseCancellation", TitleKey: "PurchaseCancellation", Module: "PurchaseCancellation", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("PurchaseCreditNote", TitleKey: "PurchaseCreditNote", Module: "PurchaseCreditNote", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("PurchaseDeliveryNote", TitleKey: "PurchaseDeliveryNote", Module: "PurchaseDeliveryNote", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("PurchaseInvoice", TitleKey: "PurchaseInvoice", Module: "PurchaseInvoice", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("PurchaseOrder", TitleKey: "PurchaseOrder", Module: "PurchaseOrder", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("PurchaseReturn", TitleKey: "PurchaseReturn", Module: "PurchaseReturn", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("ResourceStrings", TitleKey: "ResourceStrings", Module: "ResourceStrings", Group: "Setup");
        DesktopRegistry.AddOrGetForm("SalesCancellation", TitleKey: "SalesCancellation", Module: "SalesCancellation", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("SalesCreditNote", TitleKey: "SalesCreditNote", Module: "SalesCreditNote", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("SalesDeliveryNote", TitleKey: "SalesDeliveryNote", Module: "SalesDeliveryNote", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("SalesInvoice", TitleKey: "SalesInvoice", Module: "SalesInvoice", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("SalesOrder", TitleKey: "SalesOrder", Module: "SalesOrder", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("SalesPerson", TitleKey: "SalesPerson", Module: "SalesPerson", Group: "Sales");
        DesktopRegistry.AddOrGetForm("SalesReturn", TitleKey: "SalesReturn", Module: "SalesReturn", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddOrGetForm("StockBalance", TitleKey: "StockBalance", Module: "StockBalance", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("StockCount", TitleKey: "StockCount", Module: "StockCount", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("StockMovement", TitleKey: "StockMovement", Module: "StockMovement", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("StockReason", TitleKey: "StockReason", Module: "StockReason", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("StockReservation", TitleKey: "StockReservation", Module: "StockReservation", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddOrGetForm("StockTrade", TitleKey: "StockTrade", Module: "StockTrade", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("SupplierCategory", TitleKey: "SupplierCategory", Module: "SupplierCategory", Group: "Purchases");
        DesktopRegistry.AddOrGetForm("TaxCategory", TitleKey: "TaxCategory", Module: "TaxCategory", Group: "Accounting");
        DesktopRegistry.AddOrGetForm("TaxOffice", TitleKey: "TaxOffice", Module: "TaxOffice", Group: "Setup");
        DesktopRegistry.AddOrGetForm("UnitOfMeasure", TitleKey: "UnitOfMeasure", Module: "UnitOfMeasure", Group: "Inventory");
        DesktopRegistry.AddOrGetForm("VatRate", TitleKey: "VatRate", Module: "VatRate", Group: "Setup");
        DesktopRegistry.AddOrGetForm("Warehouse", TitleKey: "Warehouse", Module: "Warehouse", Group: "Inventory");
    }
}
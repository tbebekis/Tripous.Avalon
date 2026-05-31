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
        DesktopRegistry.AddForm("Account", TitleKey: "Account", Module: "Account", Group: "Accounting");
        DesktopRegistry.AddForm("AppUser", TitleKey: "AppUser", Module: "AppUser", Group: "Setup");
        DesktopRegistry.AddForm("Asset", TitleKey: "Asset", Module: "Asset", Group: "Assets");
        DesktopRegistry.AddForm("AssetCategory", TitleKey: "AssetCategory", Module: "AssetCategory", Group: "Assets");
        DesktopRegistry.AddForm("AssetDepreciationMethod", TitleKey: "AssetDepreciationMethod", Module: "AssetDepreciationMethod", Group: "Assets");
        DesktopRegistry.AddForm("AssetLocation", TitleKey: "AssetLocation", Module: "AssetLocation", Group: "Assets");
        DesktopRegistry.AddForm("Bank", TitleKey: "Bank", Module: "Bank", Group: "Setup");
        DesktopRegistry.AddForm("Carrier", TitleKey: "Carrier", Module: "Carrier", Group: "Purchases");
        DesktopRegistry.AddForm("CashAccount", TitleKey: "CashAccount", Module: "CashAccount", Group: "Finance");
        DesktopRegistry.AddForm("Category", TitleKey: "Category", Module: "Category", Group: "Inventory");
        DesktopRegistry.AddForm("Company", TitleKey: "Company", Module: "Company", Group: "Company");
        DesktopRegistry.AddForm("ContactType", TitleKey: "ContactType", Module: "ContactType", Group: "Setup");
        DesktopRegistry.AddForm("CostCenter", TitleKey: "CostCenter", Module: "CostCenter", Group: "Company");
        DesktopRegistry.AddForm("Country", TitleKey: "Country", Module: "Country", Group: "Setup");
        DesktopRegistry.AddForm("Currency", TitleKey: "Currency", Module: "Currency", Group: "Setup");
        DesktopRegistry.AddForm("CustomerCategory", TitleKey: "CustomerCategory", Module: "CustomerCategory", Group: "Sales");
        DesktopRegistry.AddForm("DiscountCategory", TitleKey: "DiscountCategory", Module: "DiscountCategory", Group: "Sales");
        DesktopRegistry.AddForm("DocumentType", TitleKey: "DocumentType", Module: "DocumentType", Group: "Documents");
        DesktopRegistry.AddForm("ExpenseCategory", TitleKey: "ExpenseCategory", Module: "ExpenseCategory", Group: "Accounting");
        DesktopRegistry.AddForm("FinanceBalance", TitleKey: "FinanceBalance", Module: "FinanceBalance", Group: "Finance", IsReadOnly: true);
        DesktopRegistry.AddForm("FinanceMovement", TitleKey: "FinanceMovement", Module: "FinanceMovement", Group: "Finance", IsReadOnly: true);
        DesktopRegistry.AddForm("FiscalYear", TitleKey: "FiscalYear", Module: "FiscalYear", Group: "Company");
        DesktopRegistry.AddForm("FixedAsset", TitleKey: "FixedAsset", Module: "FixedAsset", Group: "Assets");
        DesktopRegistry.AddForm("JournalEntry", TitleKey: "JournalEntry", Module: "JournalEntry", Group: "Accounting");
        DesktopRegistry.AddForm("Language", TitleKey: "Language", Module: "Language", Group: "System");
        DesktopRegistry.AddForm("Log", TitleKey: "Log", Module: "Log", Group: "System", IsReadOnly: true);
        DesktopRegistry.AddForm("NumberSeries", TitleKey: "NumberSeries", Module: "NumberSeries", Group: "Setup");
        DesktopRegistry.AddForm("PaymentMethod", TitleKey: "PaymentMethod", Module: "PaymentMethod", Group: "Sales");
        DesktopRegistry.AddForm("PaymentTerm", TitleKey: "PaymentTerm", Module: "PaymentTerm", Group: "Sales");
        DesktopRegistry.AddForm("Person", TitleKey: "Person", Module: "Person", Group: "People");
        DesktopRegistry.AddForm("PersonRoleType", TitleKey: "PersonRoleType", Module: "PersonRoleType", Group: "People");
        DesktopRegistry.AddForm("PriceList", TitleKey: "PriceList", Module: "PriceList", Group: "Sales");
        DesktopRegistry.AddForm("PriceListType", TitleKey: "PriceListType", Module: "PriceListType", Group: "Sales");
        DesktopRegistry.AddForm("Product", TitleKey: "Product", Module: "Product", Group: "Inventory");
        DesktopRegistry.AddForm("ProductAttributeGroup", TitleKey: "ProductAttributeGroup", Module: "ProductAttributeGroup", Group: "Inventory");
        DesktopRegistry.AddForm("ProductBrand", TitleKey: "ProductBrand", Module: "ProductBrand", Group: "Inventory");
        DesktopRegistry.AddForm("ProductDimension", TitleKey: "ProductDimension", Module: "ProductDimension", Group: "Inventory");
        DesktopRegistry.AddForm("ProductGroup", TitleKey: "ProductGroup", Module: "ProductGroup", Group: "Inventory");
        DesktopRegistry.AddForm("Project", TitleKey: "Project", Module: "Project", Group: "Projects");
        
        DesktopRegistry.AddForm("PurchaseCancellation", TitleKey: "PurchaseCancellation", Module: "PurchaseCancellation", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("PurchaseCreditNote", TitleKey: "PurchaseCreditNote", Module: "PurchaseCreditNote", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("PurchaseDeliveryNote", TitleKey: "PurchaseDeliveryNote", Module: "PurchaseDeliveryNote", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("PurchaseInvoice", TitleKey: "PurchaseInvoice", Module: "PurchaseInvoice", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("PurchaseOrder", TitleKey: "PurchaseOrder", Module: "PurchaseOrder", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("PurchaseReturn", TitleKey: "PurchaseReturn", Module: "PurchaseReturn", ClassName: "DataForm", Group: "Purchases", ItemClassName: "TradeItemPage");
        
        DesktopRegistry.AddForm("SalesCancellation", TitleKey: "SalesCancellation", Module: "SalesCancellation", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("SalesCreditNote", TitleKey: "SalesCreditNote", Module: "SalesCreditNote", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("SalesDeliveryNote", TitleKey: "SalesDeliveryNote", Module: "SalesDeliveryNote", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("SalesInvoice", TitleKey: "SalesInvoice", Module: "SalesInvoice", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("SalesOrder", TitleKey: "SalesOrder", Module: "SalesOrder", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        DesktopRegistry.AddForm("SalesPerson", TitleKey: "SalesPerson", Module: "SalesPerson", Group: "Sales");
        DesktopRegistry.AddForm("SalesReturn", TitleKey: "SalesReturn", Module: "SalesReturn", ClassName: "DataForm", Group: "Sales", ItemClassName: "TradeItemPage");
        
        DesktopRegistry.AddForm("StockBalance", TitleKey: "StockBalance", Module: "StockBalance", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddForm("StockCount", TitleKey: "StockCount", Module: "StockCount", Group: "Inventory");
        DesktopRegistry.AddForm("StockMovement", TitleKey: "StockMovement", Module: "StockMovement", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddForm("StockReason", TitleKey: "StockReason", Module: "StockReason", Group: "Inventory");
        DesktopRegistry.AddForm("StockReservation", TitleKey: "StockReservation", Module: "StockReservation", Group: "Inventory", IsReadOnly: true);
        DesktopRegistry.AddForm("StockTrade", TitleKey: "StockTrade", Module: "StockTrade", Group: "Inventory");
        DesktopRegistry.AddForm("SupplierCategory", TitleKey: "SupplierCategory", Module: "SupplierCategory", Group: "Purchases");
        DesktopRegistry.AddForm("TaxCategory", TitleKey: "TaxCategory", Module: "TaxCategory", Group: "Accounting");
        DesktopRegistry.AddForm("TaxOffice", TitleKey: "TaxOffice", Module: "TaxOffice", Group: "Setup");
        DesktopRegistry.AddForm("UnitOfMeasure", TitleKey: "UnitOfMeasure", Module: "UnitOfMeasure", Group: "Inventory");
        DesktopRegistry.AddForm("VatRate", TitleKey: "VatRate", Module: "VatRate", Group: "Setup");
        DesktopRegistry.AddForm("Warehouse", TitleKey: "Warehouse", Module: "Warehouse", Group: "Inventory");
    }
}
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
    // ● private
    static void RegisterCodeProviders_FromModules()
    {
        DataRegistry.AddOrGetCodeProvider("ASSET");
        DataRegistry.AddOrGetCodeProvider("BillOfMaterial");
        DataRegistry.AddOrGetCodeProvider("CashAccount");
        DataRegistry.AddOrGetCodeProvider("Company");
        DataRegistry.AddOrGetCodeProvider("FixedAsset");
        DataRegistry.AddOrGetCodeProvider("JOURNAL_ENTRY");
        DataRegistry.AddOrGetCodeProvider("PersonAddress");
        DataRegistry.AddOrGetCodeProvider("Product");
        DataRegistry.AddOrGetCodeProvider("Project");
        DataRegistry.AddOrGetCodeProvider("SalesPerson");
        DataRegistry.AddOrGetCodeProvider("STOCK_COUNT");
        DataRegistry.AddOrGetCodeProvider("STOCK_TRADE_DRAFT");
        DataRegistry.AddOrGetCodeProvider("TRADE-DRAFT");
        DataRegistry.AddOrGetCodeProvider("Warehouse");
        DataRegistry.AddOrGetCodeProvider("WarehouseLocation");
    }
    static void RegisterLookups_FromModules()
    {
        DataRegistry.AddOrGetLookupWithTableName("Account", "Account", FormName: "Account");
        DataRegistry.AddOrGetLookupWithTableName("AppUser", "AppUser", FormName: "AppUser");
        DataRegistry.AddOrGetLookupWithTableName("AssetCategory", "AssetCategory", FormName: "AssetCategory");
        DataRegistry.AddOrGetLookupWithTableName("AssetDepreciationMethod", "AssetDepreciationMethod", FormName: "AssetDepreciationMethod");
        DataRegistry.AddOrGetLookupWithTableName("AssetLocation", "AssetLocation", FormName: "AssetLocation");
        DataRegistry.AddOrGetLookupWithTableName("Bank", "Bank", FormName: "Bank");
        DataRegistry.AddOrGetLookupWithTableName("Carrier", "Carrier", FormName: "Carrier");
        DataRegistry.AddOrGetLookupWithTableName("CashAccount", "CashAccount", FormName: "CashAccount");
        DataRegistry.AddOrGetLookupWithTableName("Category", "Category", FormName: "Category");
        DataRegistry.AddOrGetLookupWithTableName("Company", "Company", FormName: "Company");
        DataRegistry.AddOrGetLookupWithTableName("CompanyBankAccount", "CompanyBankAccount");
        DataRegistry.AddOrGetLookupWithTableName("CompanyBranch", "CompanyBranch");
        DataRegistry.AddOrGetLookupWithTableName("ContactType", "ContactType", FormName: "ContactType");
        DataRegistry.AddOrGetLookupWithTableName("CostCenter", "CostCenter", FormName: "CostCenter");
        DataRegistry.AddOrGetLookupWithTableName("Country", "Country", FormName: "Country");
        DataRegistry.AddOrGetLookupWithTableName("Currency", "Currency", FormName: "Currency");
        DataRegistry.AddOrGetLookupWithTableName("CustomerCategory", "CustomerCategory", FormName: "CustomerCategory");
        DataRegistry.AddOrGetLookupWithTableName("DiscountCategory", "DiscountCategory", FormName: "DiscountCategory");
        DataRegistry.AddOrGetLookupWithTableName("DocumentType", "DocumentType", FormName: "DocumentType");
        DataRegistry.AddOrGetLookupWithTableName("ExpenseCategory", "ExpenseCategory", FormName: "ExpenseCategory");
        DataRegistry.AddOrGetLookupWithTableName("JournalEntry", "JournalEntry", FormName: "JournalEntry");
        DataRegistry.AddOrGetLookupWithTableName("Language", "Language", FormName: "Language");
        DataRegistry.AddOrGetLookupWithTableName("PaymentMethod", "PaymentMethod", FormName: "PaymentMethod");
        DataRegistry.AddOrGetLookupWithTableName("PaymentTerm", "PaymentTerm", FormName: "PaymentTerm");
        DataRegistry.AddOrGetLookupWithTableName("Person", "Person", FormName: "Person");
        DataRegistry.AddOrGetLookupWithTableName("PersonRoleType", "PersonRoleType", FormName: "PersonRoleType");
        DataRegistry.AddOrGetLookupWithTableName("PriceListType", "PriceListType", FormName: "PriceListType");
        DataRegistry.AddOrGetLookupWithTableName("ProductAttributeGroup", "ProductAttributeGroup", FormName: "ProductAttributeGroup");
        DataRegistry.AddOrGetLookupWithTableName("ProductBrand", "ProductBrand", FormName: "ProductBrand");
        DataRegistry.AddOrGetLookupWithTableName("ProductDimension", "ProductDimension", FormName: "ProductDimension");
        DataRegistry.AddOrGetLookupWithTableName("ProductGroup", "ProductGroup", FormName: "ProductGroup");
        DataRegistry.AddOrGetLookupWithTableName("Project", "Project", FormName: "Project");
        DataRegistry.AddOrGetLookupWithTableName("SalesPerson", "SalesPerson", FormName: "SalesPerson");
        DataRegistry.AddOrGetLookupWithTableName("SupplierCategory", "SupplierCategory", FormName: "SupplierCategory");
        DataRegistry.AddOrGetLookupWithTableName("SYS_NUMBER_SERIES", "SYS_NUMBER_SERIES", FormName: "NumberSeries");
        DataRegistry.AddOrGetLookupWithTableName("TaxCategory", "TaxCategory", FormName: "TaxCategory");
        DataRegistry.AddOrGetLookupWithTableName("TaxOffice", "TaxOffice", FormName: "TaxOffice");
        DataRegistry.AddOrGetLookupWithTableName("UnitOfMeasure", "UnitOfMeasure", FormName: "UnitOfMeasure");
        DataRegistry.AddOrGetLookupWithTableName("VatRate", "VatRate", FormName: "VatRate");
        DataRegistry.AddOrGetLookupWithTableName("Warehouse", "Warehouse", FormName: "Warehouse");
    }
    static void RegisterLocators_FromModules()
    {
        DataRegistry.AddOrGetLocator("Country", "Country", "Id", FormName: "Country");
        DataRegistry.AddOrGetLocator("Customer", "Person", "Id", FormName: "Person");
        DataRegistry.AddOrGetLocator("JournalEntry", "JournalEntry", "Id", FormName: "JournalEntry");
        DataRegistry.AddOrGetLocator("Person", "Person", "Id", FormName: "Person");
        DataRegistry.AddOrGetLocator("Product", "Product", "Id", FormName: "Product");
        DataRegistry.AddOrGetLocator("StockCount", "StockCount", "Id", FormName: "StockCount");
        DataRegistry.AddOrGetLocator("StockTrade", "StockTrade", "Id", FormName: "StockTrade");
        DataRegistry.AddOrGetLocator("Supplier", "ProductSupplier", "Id");
        DataRegistry.AddOrGetLocator("Trade", "Trade", "Id", FormName: "SalesOrder");
        DataRegistry.AddOrGetLocator("TradeLine", "TradeLine", "Id");
    }
    static void RegisterModule_Account()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Account.Id,
   Account.Code,
   Account.Name,
   Account.ParentAccountId,
   Account.AccountTypeId,
   case
      when Account.AccountTypeId = 0 then 'None'
      when Account.AccountTypeId = 1 then 'Asset'
      when Account.AccountTypeId = 2 then 'Liability'
      when Account.AccountTypeId = 3 then 'Equity'
      when Account.AccountTypeId = 4 then 'Revenue'
      when Account.AccountTypeId = 5 then 'Expense'
      else ''
   end as AccountType,
   Account.NormalBalanceId,
   case
      when Account.NormalBalanceId = 0 then 'None'
      when Account.NormalBalanceId = 1 then 'Debit'
      when Account.NormalBalanceId = 2 then 'Credit'
      else ''
   end as NormalBalance,
   Account.IsPosting,
   Account.IsActive
from
  Account
";
        Module = DataRegistry.AddOrGetModule("Account", ClassName: "AccountDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Account";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("ParentAccountId", "Account", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddEnumLookupId("AccountTypeId", "AccountType", TypeStore.Get("AccountType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("NormalBalanceId", "NormalBalance", TypeStore.Get("NormalBalance"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsPosting", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        string[] FilterFields = ["Name", "AccountType", "Code", "IsActive", "IsPosting", "NormalBalance"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ParentAccountId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AccountTypeId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["AccountType"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NormalBalanceId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["NormalBalance"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsPosting"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_AppUser()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   AppUser.Id,
   AppUser.UserName,
   AppUser.Password,
   AppUser.Salt,
   AppUser.FullName,
   AppUser.UserLevelId,
   case
      when AppUser.UserLevelId = 0 then 'None'
      when AppUser.UserLevelId = 1 then 'Guest'
      when AppUser.UserLevelId = 2 then 'User'
      when AppUser.UserLevelId = 4 then 'Admin'
      when AppUser.UserLevelId = 8 then 'ClientApp'
      when AppUser.UserLevelId = 256 then 'Service'
      when AppUser.UserLevelId = 4096 then 'God'
      else ''
   end as UserLevel,
   AppUser.CultureCode,
   AppUser.Email,
   AppUser.Phone,
   AppUser.LastLoginAt,
   AppUser.PasswordChangedAt,
   AppUser.IsActive
from
  AppUser
";
        Module = DataRegistry.AddOrGetModule("AppUser", ClassName:"AppUserDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "AppUser";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("UserName", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Password", MaxLength: 512, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Salt", MaxLength: 256, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("FullName", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("UserLevelId", "UserLevel", TypeStore.Get("UserLevel"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("CultureCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Phone", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDateTime("LastLoginAt", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDateTime("PasswordChangedAt", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["CultureCode", "Email", "FullName", "IsActive", "LastLoginAt", "Password", "PasswordChangedAt", "Phone", "Salt", "UserLevel", "UserName"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UserName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Password"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Salt"] = DataColumnType.Text;
        SelectDef.ColumnTypes["FullName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UserLevelId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["UserLevel"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CultureCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Email"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Phone"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LastLoginAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PasswordChangedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_Asset()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Asset.Id,
   Asset.Code,
   Asset.Name,
   Asset.AssetCategoryId,
   Asset.AssetLocationId,
   Asset.StatusId,
   case
      when Asset.StatusId = 0 then 'None'
      when Asset.StatusId = 1 then 'Draft'
      when Asset.StatusId = 2 then 'Active'
      when Asset.StatusId = 3 then 'Disposed'
      when Asset.StatusId = 4 then 'Sold'
      when Asset.StatusId = 5 then 'Scrapped'
      else ''
   end as AssetStatus,
   Asset.AcquisitionDate,
   Asset.InServiceDate,
   Asset.AcquisitionCost,
   Asset.DepreciationMethodId,
   Asset.UsefulLifeMonths,
   Asset.SalvageValue,
   Asset.AccumulatedDepreciation,
   Asset.BookValue,
   Asset.SerialNumber,
   Asset.SupplierId,
   Asset.CreatedAt,
   Asset.CreatedBy,
   Asset.ModifiedAt,
   Asset.ModifiedBy,
   COALESCE(AssetCategory.Name, '') as AssetCategory__Name,
   COALESCE(AssetLocation.Name, '') as AssetLocation__Name,
   COALESCE(DepreciationMethod.Name, '') as DepreciationMethod__Name
from
  Asset
    left join AssetCategory AssetCategory on AssetCategory.Id = Asset.AssetCategoryId
    left join AssetLocation AssetLocation on AssetLocation.Id = Asset.AssetLocationId
    left join AssetDepreciationMethod DepreciationMethod on DepreciationMethod.Id = Asset.DepreciationMethodId
    left join ProductSupplier Supplier on Supplier.Id = Asset.SupplierId
    left join AppUser CreatedBy on CreatedBy.Id = Asset.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Asset.ModifiedBy
";
        Module = DataRegistry.AddOrGetModule("Asset", ClassName: "AssetDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Asset";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Classification", "Acquisition", "Depreciation", "Supplier", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("ASSET");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("AssetCategoryId", "AssetCategory", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Classification");
        tblTop.AddStringLookupId("AssetLocationId", "AssetLocation", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Classification");
        tblTop.AddEnumLookupId("StatusId", "AssetStatus", TypeStore.Get("AssetStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("AcquisitionDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Acquisition");
        tblTop.AddDate("InServiceDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Acquisition");
        tblTop.AddDecimal("AcquisitionCost", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetGroup("Acquisition");
        tblTop.AddStringLookupId("DepreciationMethodId", "AssetDepreciationMethod", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Depreciation");
        tblTop.AddInteger("UsefulLifeMonths", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Depreciation");
        tblTop.AddDecimal("SalvageValue", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Depreciation");
        tblTop.AddDecimal("AccumulatedDepreciation", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Depreciation");
        tblTop.AddDecimal("BookValue", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Depreciation");
        tblTop.AddString("SerialNumber", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Classification");
        tblTop.AddString("SupplierId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Supplier");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        TableDef tblSupplier = tblTop.AddJoin("SupplierId", "ProductSupplier", "Supplier", "Id");
        tblTop.Fields.Get("SupplierId").Locator = "Supplier";
        tblSupplier.AddId("Id").SetNullable(false);
        tblSupplier.AddString("SupplierCode", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "AccumulatedDepreciation", "AcquisitionCost", "AcquisitionDate", "AssetCategory__Name", "AssetLocation__Name", "AssetStatus", "BookValue", "Code", "CreatedAt", "CreatedBy", "DepreciationMethod__Name", "InServiceDate", "ModifiedAt", "ModifiedBy", "SalvageValue", "SerialNumber", "UsefulLifeMonths"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetCategoryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetLocationId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["StatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["AssetStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AcquisitionDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["InServiceDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["AcquisitionCost"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DepreciationMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UsefulLifeMonths"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["SalvageValue"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["AccumulatedDepreciation"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["BookValue"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["SerialNumber"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SupplierId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetCategory__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetLocation__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DepreciationMethod__Name"] = DataColumnType.Text;
        TableDef tblAssetDepreciationLine = tblTop.AddDetail("AssetDepreciationLine", "Id", "AssetId");
        tblAssetDepreciationLine.KeyField = "Id";
        tblAssetDepreciationLine.AddId("Id").SetNullable(false);
        tblAssetDepreciationLine.AddString("AssetId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblAssetDepreciationLine.AddDate("DepreciationDate", Flags: FieldFlags.Required).SetNullable(false);
        tblAssetDepreciationLine.AddDecimal("DepreciationAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblAssetDepreciationLine.AddDecimal("AccumulatedDepreciation", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblAssetDepreciationLine.AddDecimal("BookValueAfter", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblAssetDepreciationLine.AddStringLookupId("JournalEntryId", "JournalEntry", Flags: FieldFlags.Hidden).SetNullable(true);
        tblAssetDepreciationLine.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true);
        tblAssetDepreciationLine.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false);
        tblAssetDepreciationLine.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false);
    }
    static void RegisterModule_AssetCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   AssetCategory.Id,
   AssetCategory.Name,
   AssetCategory.IsActive
from
  AssetCategory
";
        Module = DataRegistry.AddOrGetModule("AssetCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "AssetCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_AssetDepreciationMethod()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   AssetDepreciationMethod.Id,
   AssetDepreciationMethod.Name,
   AssetDepreciationMethod.IsActive
from
  AssetDepreciationMethod
";
        Module = DataRegistry.AddOrGetModule("AssetDepreciationMethod", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "AssetDepreciationMethod";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_AssetLocation()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   AssetLocation.Id,
   AssetLocation.Name,
   AssetLocation.IsActive
from
  AssetLocation
";
        Module = DataRegistry.AddOrGetModule("AssetLocation", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "AssetLocation";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_Bank()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Bank.Id,
   Bank.Code,
   Bank.Name
from
  Bank
";
        Module = DataRegistry.AddOrGetModule("Bank", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Bank";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_Carrier()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Carrier.Id,
   Carrier.Code,
   Carrier.Name,
   Carrier.IsActive
from
  Carrier
";
        Module = DataRegistry.AddOrGetModule("Carrier", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Carrier";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_CashAccount()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   CashAccount.Id,
   CashAccount.Code,
   CashAccount.Name,
   CashAccount.CurrencyId,
   CashAccount.CompanyBranchId,
   CashAccount.Balance,
   CashAccount.IsActive,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(CompanyBranch.Code, '') as CompanyBranch__Code,
   COALESCE(CompanyBranch.Name, '') as CompanyBranch__Name
from
  CashAccount
    left join Currency Currency on Currency.Id = CashAccount.CurrencyId
    left join CompanyBranch CompanyBranch on CompanyBranch.Id = CashAccount.CompanyBranchId
";
        Module = DataRegistry.AddOrGetModule("CashAccount", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "CashAccount";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("CashAccount");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CompanyBranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddDecimal("Balance", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        string[] FilterFields = ["Name", "Balance", "Code", "CompanyBranch__Code", "CompanyBranch__Name", "Currency__Code", "Currency__Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Balance"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBranch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBranch__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_Category()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Category.Id,
   Category.ParentId,
   Category.Code,
   Category.Name,
   Category.LevelNo,
   Category.SortNo,
   Category.VatRateId,
   Category.RevenueAccount,
   Category.ExpenseAccount,
   Category.IsSystem,
   Category.IsActive,
   Category.Color,
   Category.IconName,
   COALESCE(VatRate.Code, '') as VatRate__Code,
   COALESCE(VatRate.Name, '') as VatRate__Name
from
  Category
    left join VatRate VatRate on VatRate.Id = Category.VatRateId
";
        Module = DataRegistry.AddOrGetModule("Category", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Category";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("ParentId", "Category", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("LevelNo", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("SortNo", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("RevenueAccount", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("ExpenseAccount", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "ExpenseAccount", "IconName", "IsActive", "IsSystem", "LevelNo", "RevenueAccount", "SortNo", "VatRate__Code", "VatRate__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ParentId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LevelNo"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["SortNo"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["VatRateId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["RevenueAccount"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExpenseAccount"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsSystem"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_Company()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Company.Id,
   Company.Code,
   Company.Name,
   Company.Title,
   Company.TaxNumber,
   Company.TaxOfficeId,
   Company.CountryId,
   Company.CurrencyId,
   Company.AddressLine1,
   Company.AddressLine2,
   Company.City,
   Company.PostalCode,
   Company.Phone,
   Company.Email,
   Company.Website,
   COALESCE(TaxOffice.Code, '') as TaxOffice__Code,
   COALESCE(TaxOffice.Name, '') as TaxOffice__Name,
   COALESCE(Country.Code, '') as Country__Code,
   COALESCE(Country.Name, '') as Country__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name
from
  Company
    left join TaxOffice TaxOffice on TaxOffice.Id = Company.TaxOfficeId
    left join Country Country on Country.Id = Company.CountryId
    left join Currency Currency on Currency.Id = Company.CurrencyId
";
        Module = DataRegistry.AddOrGetModule("Company", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Company";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Company");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Title", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("TaxNumber", MaxLength: 32, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("TaxOfficeId", "TaxOffice", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Website", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "AddressLine1", "AddressLine2", "City", "Code", "Country__Code", "Country__Name", "Currency__Code", "Currency__Name", "Email", "Phone", "PostalCode", "TaxNumber", "TaxOffice__Code", "TaxOffice__Name", "Title", "Website"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxNumber"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOfficeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["City"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Phone"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Email"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Website"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOffice__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOffice__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        TableDef tblCompanyBranch = tblTop.AddDetail("CompanyBranch", "Id", "CompanyId");
        tblCompanyBranch.KeyField = "Id";
        tblCompanyBranch.AddId("Id").SetNullable(false);
        tblCompanyBranch.AddString("CompanyId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCompanyBranch.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddString("City", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddString("CountryId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("Phone", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBranch.AddBoolean("IsPrimary", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblCompanyBranch.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        TableDef tblCountry = tblCompanyBranch.AddJoin("CountryId", "Country", "Country", "Id");
        tblCompanyBranch.Fields.Get("CountryId").Locator = "Country";
        tblCountry.AddId("Id").SetNullable(false);
        tblCountry.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCountry.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        TableDef tblCompanyBankAccount = tblTop.AddDetail("CompanyBankAccount", "Id", "CompanyId");
        tblCompanyBankAccount.KeyField = "Id";
        tblCompanyBankAccount.AddId("Id").SetNullable(false);
        tblCompanyBankAccount.AddString("CompanyId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCompanyBankAccount.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("BankName", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("Iban", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("SwiftBic", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true);
        tblCompanyBankAccount.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblCompanyBankAccount.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
    }
    static void RegisterModule_ContactType()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ContactType.Id,
   ContactType.Name,
   ContactType.IsActive
from
  ContactType
";
        Module = DataRegistry.AddOrGetModule("ContactType", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ContactType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_CostCenter()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   CostCenter.Id,
   CostCenter.Code,
   CostCenter.Name,
   CostCenter.ParentCostCenterId,
   CostCenter.ManagerPersonId,
   CostCenter.StartDate,
   CostCenter.EndDate,
   CostCenter.IsActive,
   CostCenter.Color,
   CostCenter.IconName,
   COALESCE(ManagerPerson.Code, '') as ManagerPerson__Code,
   COALESCE(ManagerPerson.Name, '') as ManagerPerson__Name,
   COALESCE(ManagerPerson.Title, '') as ManagerPerson__Title
from
  CostCenter
    left join Person ManagerPerson on ManagerPerson.Id = CostCenter.ManagerPersonId
";
        Module = DataRegistry.AddOrGetModule("CostCenter", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "CostCenter";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("ParentCostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("ManagerPersonId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddDate("StartDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDate("EndDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblManagerPerson = tblTop.AddJoin("ManagerPersonId", "Person", "ManagerPerson", "Id");
        tblTop.Fields.Get("ManagerPersonId").Locator = "Person";
        tblManagerPerson.AddId("Id").SetNullable(false);
        tblManagerPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblManagerPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblManagerPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblManagerPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        string[] FilterFields = ["Name", "Code", "Color", "EndDate", "IconName", "IsActive", "ManagerPerson__Code", "ManagerPerson__Name", "ManagerPerson__Title", "StartDate"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ParentCostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["StartDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["EndDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Title"] = DataColumnType.Text;
    }
    static void RegisterModule_Country()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Country.Id,
   Country.Code,
   Country.Iso2,
   Country.Iso3,
   Country.Name
from
  Country
";
        Module = DataRegistry.AddOrGetModule("Country", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Country";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Iso2", MaxLength: 2, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Iso3", MaxLength: 3, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code", "Iso2", "Iso3"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Iso2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Iso3"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_Currency()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Currency.Id,
   Currency.Code,
   Currency.Name,
   Currency.Symbol,
   Currency.Decimals
from
  Currency
";
        Module = DataRegistry.AddOrGetModule("Currency", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Currency";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Symbol", MaxLength: 8, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Decimals", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("2");
        string[] FilterFields = ["Name", "Code", "Decimals", "Symbol"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Symbol"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Decimals"] = DataColumnType.Integer;
    }
    static void RegisterModule_CustomerCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   CustomerCategory.Id,
   CustomerCategory.Name
from
  CustomerCategory
";
        Module = DataRegistry.AddOrGetModule("CustomerCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "CustomerCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_DiscountCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   DiscountCategory.Id,
   DiscountCategory.Name
from
  DiscountCategory
";
        Module = DataRegistry.AddOrGetModule("DiscountCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "DiscountCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_DocumentType()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   DocumentType.Id,
   DocumentType.Code,
   DocumentType.Name,
   DocumentType.TradeTypeId,
   case
      when DocumentType.TradeTypeId = 0 then 'None'
      when DocumentType.TradeTypeId = 1 then 'Sales'
      when DocumentType.TradeTypeId = 2 then 'Purchases'
      when DocumentType.TradeTypeId = 3 then 'Warehouse'
      when DocumentType.TradeTypeId = 4 then 'Financial'
      when DocumentType.TradeTypeId = 5 then 'Accounting'
      else ''
   end as TradeType,
   DocumentType.NumberSeriesId,
   DocumentType.HandlerClass,
   DocumentType.IsActive,
   DocumentType.IsSystem,
   DocumentType.AllowManualNumber,
   DocumentType.AutoComplete,
   DocumentType.AffectsStock,
   DocumentType.AffectsFinancial,
   DocumentType.AffectsAccounting,
   DocumentType.StockDirection,
   DocumentType.FinancialDirection,
   DocumentType.AccountingDirection,
   DocumentType.IsCancellation,
   DocumentType.CancellationTargetId,
   DocumentType.PrintTemplate,
   DocumentType.ReportName,
   DocumentType.DisplayOrder,
   DocumentType.Color,
   DocumentType.IconName,
   COALESCE(NumberSeries.Code, '') as NumberSeries__Code,
   COALESCE(NumberSeries.Name, '') as NumberSeries__Name
from
  DocumentType
    left join SYS_NUMBER_SERIES NumberSeries on NumberSeries.Id = DocumentType.NumberSeriesId
";
        Module = DataRegistry.AddOrGetModule("DocumentType", ClassName: "DocumentTypeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "DocumentType";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Posting", "Cancellation", "Output", "Appearance", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("TradeTypeId", "TradeType", TypeStore.Get("TradeType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("NumberSeriesId", "SYS_NUMBER_SERIES", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("HandlerClass", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AllowManualNumber", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AutoComplete", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsStock", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddBoolean("AffectsFinancial", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddBoolean("AffectsAccounting", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddInteger("StockDirection", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddInteger("FinancialDirection", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddInteger("AccountingDirection", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
        tblTop.AddBoolean("IsCancellation", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Cancellation");
        tblTop.AddStringLookupId("CancellationTargetId", "DocumentType", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Cancellation");
        tblTop.AddString("PrintTemplate", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Output");
        tblTop.AddString("ReportName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Output");
        tblTop.AddInteger("DisplayOrder", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        string[] FilterFields = ["Name", "AccountingDirection", "AffectsAccounting", "AffectsFinancial", "AffectsStock", "AllowManualNumber", "AutoComplete", "Code", "Color", "DisplayOrder", "FinancialDirection", "HandlerClass", "IconName", "IsActive", "IsCancellation", "IsSystem", "NumberSeries__Code", "NumberSeries__Name", "PrintTemplate", "ReportName", "StockDirection", "TradeType"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeTypeId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeType"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NumberSeriesId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["HandlerClass"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsSystem"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AllowManualNumber"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AutoComplete"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsStock"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsFinancial"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsAccounting"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["StockDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["FinancialDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["AccountingDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["IsCancellation"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CancellationTargetId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrintTemplate"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ReportName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DisplayOrder"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NumberSeries__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NumberSeries__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_ExpenseCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ExpenseCategory.Id,
   ExpenseCategory.Code,
   ExpenseCategory.Name
from
  ExpenseCategory
";
        Module = DataRegistry.AddOrGetModule("ExpenseCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ExpenseCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_FinanceBalance()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   FinanceBalance.Id,
   FinanceBalance.CashAccountId,
   FinanceBalance.CompanyBankAccountId,
   FinanceBalance.Balance,
   FinanceBalance.LastMovementDate,
   FinanceBalance.LastMovementId,
   COALESCE(CashAccount.Code, '') as CashAccount__Code,
   COALESCE(CashAccount.Name, '') as CashAccount__Name,
   COALESCE(CompanyBankAccount.Code, '') as CompanyBankAccount__Code,
   COALESCE(CompanyBankAccount.Name, '') as CompanyBankAccount__Name
from
  FinanceBalance
    left join CashAccount CashAccount on CashAccount.Id = FinanceBalance.CashAccountId
    left join CompanyBankAccount CompanyBankAccount on CompanyBankAccount.Id = FinanceBalance.CompanyBankAccountId
    left join FinanceMovement LastMovement on LastMovement.Id = FinanceBalance.LastMovementId
";
        Module = DataRegistry.AddOrGetModule("FinanceBalance", ClassName: "FinanceBalanceDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "FinanceBalance";
        tblTop.KeyField = "Id";
        tblTop.IsUiVisible = false;
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("CashAccountId", "CashAccount", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("CompanyBankAccountId", "CompanyBankAccount", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddDecimal("Balance", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDate("LastMovementDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("LastMovementId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        string[] FilterFields = ["Balance", "CashAccount__Code", "CashAccount__Name", "CompanyBankAccount__Code", "CompanyBankAccount__Name", "LastMovementDate"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CashAccountId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccountId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Balance"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["LastMovementDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["LastMovementId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CashAccount__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CashAccount__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccount__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccount__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_FinanceMovement()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   FinanceMovement.Id,
   FinanceMovement.MovementDate,
   FinanceMovement.CashAccountId,
   FinanceMovement.CompanyBankAccountId,
   FinanceMovement.Direction,
   FinanceMovement.Amount,
   FinanceMovement.CurrencyId,
   FinanceMovement.ExchangeRate,
   FinanceMovement.SourceModule,
   FinanceMovement.SourceTable,
   FinanceMovement.SourceId,
   FinanceMovement.DocumentTypeId,
   FinanceMovement.DocumentCode,
   FinanceMovement.DocumentDate,
   FinanceMovement.Remarks,
   FinanceMovement.CreatedAt,
   FinanceMovement.CreatedBy,
   COALESCE(CashAccount.Code, '') as CashAccount__Code,
   COALESCE(CashAccount.Name, '') as CashAccount__Name,
   COALESCE(CompanyBankAccount.Code, '') as CompanyBankAccount__Code,
   COALESCE(CompanyBankAccount.Name, '') as CompanyBankAccount__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name
from
  FinanceMovement
    left join CashAccount CashAccount on CashAccount.Id = FinanceMovement.CashAccountId
    left join CompanyBankAccount CompanyBankAccount on CompanyBankAccount.Id = FinanceMovement.CompanyBankAccountId
    left join Currency Currency on Currency.Id = FinanceMovement.CurrencyId
    left join DocumentType DocumentType on DocumentType.Id = FinanceMovement.DocumentTypeId
    left join AppUser CreatedBy on CreatedBy.Id = FinanceMovement.CreatedBy
";
        Module = DataRegistry.AddOrGetModule("FinanceMovement", ClassName: "FinanceMovementDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "FinanceMovement";
        tblTop.KeyField = "Id";
        tblTop.IsUiVisible = false;
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddDate("MovementDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CashAccountId", "CashAccount", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("CompanyBankAccountId", "CompanyBankAccount", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddInteger("Direction", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("Amount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("SourceModule", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceTable", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("DocumentCode", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("DocumentDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Amount", "CashAccount__Code", "CashAccount__Name", "CompanyBankAccount__Code", "CompanyBankAccount__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "Direction", "DocumentCode", "DocumentDate", "DocumentType__Code", "DocumentType__Name", "ExchangeRate", "MovementDate", "Remarks", "SourceModule", "SourceTable"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["MovementDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["CashAccountId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccountId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Direction"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["Amount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["SourceModule"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceTable"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CashAccount__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CashAccount__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccount__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyBankAccount__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_FiscalYear()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   FiscalYear.Id,
   FiscalYear.Code,
   FiscalYear.Name,
   FiscalYear.StartDate,
   FiscalYear.EndDate,
   FiscalYear.IsActive,
   FiscalYear.IsClosed
from
  FiscalYear
";
        Module = DataRegistry.AddOrGetModule("FiscalYear", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "FiscalYear";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("StartDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("EndDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsClosed", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "EndDate", "IsActive", "IsClosed", "StartDate"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["StartDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["EndDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsClosed"] = DataColumnType.Boolean;
        TableDef tblFiscalPeriod = tblTop.AddDetail("FiscalPeriod", "Id", "YearId");
        tblFiscalPeriod.KeyField = "Id";
        tblFiscalPeriod.AddId("Id").SetNullable(false);
        tblFiscalPeriod.AddString("YearId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblFiscalPeriod.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddInteger("PeriodNo", Flags: FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddDate("StartDate", Flags: FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddDate("EndDate", Flags: FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddBoolean("IsClosed", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblFiscalPeriod.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_FixedAsset()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   FixedAsset.Id,
   FixedAsset.Code,
   FixedAsset.Name,
   FixedAsset.AssetCategoryId,
   FixedAsset.AssetLocationId,
   FixedAsset.AssetDepreciationMethodId,
   FixedAsset.PurchaseDate,
   FixedAsset.PurchaseValue,
   FixedAsset.UsefulLifeMonths,
   FixedAsset.DepreciationRate,
   FixedAsset.SerialNumber,
   FixedAsset.Manufacturer,
   FixedAsset.Model,
   FixedAsset.IsActive,
   COALESCE(AssetCategory.Name, '') as AssetCategory__Name,
   COALESCE(AssetLocation.Name, '') as AssetLocation__Name,
   COALESCE(AssetDepreciationMethod.Name, '') as AssetDepreciationMethod__Name
from
  FixedAsset
    left join AssetCategory AssetCategory on AssetCategory.Id = FixedAsset.AssetCategoryId
    left join AssetLocation AssetLocation on AssetLocation.Id = FixedAsset.AssetLocationId
    left join AssetDepreciationMethod AssetDepreciationMethod on AssetDepreciationMethod.Id = FixedAsset.AssetDepreciationMethodId
";
        Module = DataRegistry.AddOrGetModule("FixedAsset", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "FixedAsset";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("FixedAsset");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("AssetCategoryId", "AssetCategory", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("AssetLocationId", "AssetLocation", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("AssetDepreciationMethodId", "AssetDepreciationMethod", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddDate("PurchaseDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDecimal("PurchaseValue", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddInteger("UsefulLifeMonths", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDecimal("DepreciationRate", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("SerialNumber", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Manufacturer", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("Model", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        string[] FilterFields = ["Name", "AssetCategory__Name", "AssetDepreciationMethod__Name", "AssetLocation__Name", "Code", "DepreciationRate", "IsActive", "Manufacturer", "Model", "PurchaseDate", "PurchaseValue", "SerialNumber", "UsefulLifeMonths"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetCategoryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetLocationId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetDepreciationMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PurchaseDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PurchaseValue"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["UsefulLifeMonths"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["DepreciationRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["SerialNumber"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Manufacturer"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Model"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AssetCategory__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetLocation__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AssetDepreciationMethod__Name"] = DataColumnType.Text;
        TableDef tblAssetAssignment = tblTop.AddDetail("AssetAssignment", "Id", "FixedAssetId");
        tblAssetAssignment.KeyField = "Id";
        tblAssetAssignment.AddId("Id").SetNullable(false);
        tblAssetAssignment.AddString("FixedAssetId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblAssetAssignment.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblAssetAssignment.AddDate("AssignmentDate", Flags: FieldFlags.None).SetNullable(true);
        tblAssetAssignment.AddDate("ReturnDate", Flags: FieldFlags.None).SetNullable(true);
        tblAssetAssignment.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblPerson = tblAssetAssignment.AddJoin("PersonId", "Person", "Person", "Id");
        tblAssetAssignment.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblAssetMaintenance = tblTop.AddDetail("AssetMaintenance", "Id", "FixedAssetId");
        tblAssetMaintenance.KeyField = "Id";
        tblAssetMaintenance.AddId("Id").SetNullable(false);
        tblAssetMaintenance.AddString("FixedAssetId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblAssetMaintenance.AddDate("Date", Flags: FieldFlags.Required).SetNullable(false);
        tblAssetMaintenance.AddString("Description", MaxLength: 255, Flags: FieldFlags.Required).SetNullable(false);
        tblAssetMaintenance.AddDecimal("Cost", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblAssetMaintenance.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblAssetDocument = tblTop.AddDetail("AssetDocument", "Id", "FixedAssetId");
        tblAssetDocument.KeyField = "Id";
        tblAssetDocument.AddId("Id").SetNullable(false);
        tblAssetDocument.AddString("FixedAssetId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblAssetDocument.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblAssetDocument.AddString("FileName", MaxLength: 255, Flags: FieldFlags.None).SetNullable(true);
        tblAssetDocument.AddString("Description", MaxLength: 255, Flags: FieldFlags.None).SetNullable(true);
        tblAssetDocument.AddTextBlob("BlobText", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblAssetInsurance = tblTop.AddDetail("AssetInsurance", "Id", "FixedAssetId");
        tblAssetInsurance.KeyField = "Id";
        tblAssetInsurance.AddId("Id").SetNullable(false);
        tblAssetInsurance.AddString("FixedAssetId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblAssetInsurance.AddString("PolicyNumber", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblAssetInsurance.AddDate("StartDate", Flags: FieldFlags.None).SetNullable(true);
        tblAssetInsurance.AddDate("EndDate", Flags: FieldFlags.None).SetNullable(true);
        tblAssetInsurance.AddDecimal("Amount", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblAssetInsurance.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblAssetInsurance.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_JournalEntry()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   JournalEntry.Id,
   JournalEntry.Code,
   JournalEntry.EntryDate,
   JournalEntry.StatusId,
   case
      when JournalEntry.StatusId = 0 then 'Draft'
      when JournalEntry.StatusId = 1 then 'Posted'
      when JournalEntry.StatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   JournalEntry.TotalDebit,
   JournalEntry.TotalCredit,
   JournalEntry.SourceModule,
   JournalEntry.SourceTable,
   JournalEntry.SourceId,
   JournalEntry.DocumentTypeId,
   JournalEntry.DocumentCode,
   JournalEntry.DocumentDate,
   JournalEntry.CancelledDocumentId,
   JournalEntry.CancellationDocumentId,
   JournalEntry.CreatedAt,
   JournalEntry.CreatedBy,
   JournalEntry.ModifiedAt,
   JournalEntry.ModifiedBy,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name
from
  JournalEntry
    left join DocumentType DocumentType on DocumentType.Id = JournalEntry.DocumentTypeId
    left join AppUser CreatedBy on CreatedBy.Id = JournalEntry.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = JournalEntry.ModifiedBy
";
        Module = DataRegistry.AddOrGetModule("JournalEntry", ClassName: "JournalEntryDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "JournalEntry";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Source", "Document", "Relations", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("JOURNAL_ENTRY");
        tblTop.AddDate("EntryDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("StatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("TotalDebit", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("TotalCredit", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("SourceModule", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Source");
        tblTop.AddString("SourceTable", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Source");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Source");
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Document");
        tblTop.AddString("DocumentCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true).SetGroup("Document");
        tblTop.AddDate("DocumentDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Document");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        tblTop.AddString("CancelledDocumentId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancellationDocumentId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        TableDef tblCancelledDocument = tblTop.AddJoin("CancelledDocumentId", "JournalEntry", "CancelledDocument", "Id");
        tblTop.Fields.Get("CancelledDocumentId").Locator = "JournalEntry";
        tblCancelledDocument.AddId("Id").SetNullable(false);
        tblCancelledDocument.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("JOURNAL_ENTRY");
        tblCancelledDocument.AddString("DocumentCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true).SetGroup("Document");
        TableDef tblCancellationDocument = tblTop.AddJoin("CancellationDocumentId", "JournalEntry", "CancellationDocument", "Id");
        tblTop.Fields.Get("CancellationDocumentId").Locator = "JournalEntry";
        tblCancellationDocument.AddId("Id").SetNullable(false);
        tblCancellationDocument.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("JOURNAL_ENTRY");
        tblCancellationDocument.AddString("DocumentCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true).SetGroup("Document");
        string[] FilterFields = ["Code", "CreatedAt", "CreatedBy", "DocumentCode", "DocumentDate", "DocumentType__Code", "DocumentType__Name", "EntryDate", "ModifiedAt", "ModifiedBy", "SourceModule", "SourceTable", "TotalCredit", "TotalDebit", "TradeStatus"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["EntryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["StatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TotalDebit"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalCredit"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["SourceModule"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceTable"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["CancelledDocumentId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancellationDocumentId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        TableDef tblJournalEntryLine = tblTop.AddDetail("JournalEntryLine", "Id", "JournalEntryId");
        tblJournalEntryLine.KeyField = "Id";
        tblJournalEntryLine.AddId("Id").SetNullable(false);
        tblJournalEntryLine.AddString("JournalEntryId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblJournalEntryLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblJournalEntryLine.AddStringLookupId("AccountId", "Account", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblJournalEntryLine.AddDecimal("DebitAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblJournalEntryLine.AddDecimal("CreditAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblJournalEntryLine.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden).SetNullable(true);
        tblJournalEntryLine.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblJournalEntryLine.AddString("ReferenceNo", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblJournalEntryLine.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true);
        tblJournalEntryLine.AddString("SourceModule", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblJournalEntryLine.AddString("SourceTable", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblJournalEntryLine.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
    }
    static void RegisterModule_Language()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Language.Id,
   Language.Code,
   Language.Name,
   Language.CultureName,
   Language.IsDefault,
   Language.IsActive,
   Language.IsRightToLeft,
   Language.Color,
   Language.IconName
from
  Language
";
        Module = DataRegistry.AddOrGetModule("Language", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Language";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 16, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("CultureName", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsRightToLeft", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "CultureName", "IconName", "IsActive", "IsDefault", "IsRightToLeft"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CultureName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsDefault"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsRightToLeft"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
    }
    static void RegisterModule_Log()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   SYS_LOG.Id,
   SYS_LOG.Year,
   SYS_LOG.Month,
   SYS_LOG.DayOfMonth,
   SYS_LOG.LogTime,
   SYS_LOG.User,
   SYS_LOG.Host,
   SYS_LOG.Level,
   SYS_LOG.Source,
   SYS_LOG.Scope,
   SYS_LOG.EventId
from
  SYS_LOG
";
        Module = DataRegistry.AddOrGetModule("Log", ClassName: "LogDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "SYS_LOG";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddInteger("Year", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Month", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("DayOfMonth", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("LogTime", MaxLength: 20, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("User", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Host", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Level", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Source", MaxLength: 512, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Scope", MaxLength: 512, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("EventId", MaxLength: 96, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddTextBlob("Message", Flags: FieldFlags.Required).SetNullable(false).SetLargeMemo();
        string[] FilterFields = ["DayOfMonth", "Host", "Level", "LogTime", "Month", "Scope", "Source", "User", "Year"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Year"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["Month"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["DayOfMonth"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["LogTime"] = DataColumnType.Text;
        SelectDef.ColumnTypes["User"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Host"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Level"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Source"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Scope"] = DataColumnType.Text;
        SelectDef.ColumnTypes["EventId"] = DataColumnType.Text;
    }
    static void RegisterModule_NumberSeries()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   SYS_NUMBER_SERIES.Id,
   SYS_NUMBER_SERIES.Code,
   SYS_NUMBER_SERIES.Name,
   SYS_NUMBER_SERIES.Pattern,
   SYS_NUMBER_SERIES.ResetPeriodId,
   case
      when SYS_NUMBER_SERIES.ResetPeriodId = 0 then 'None'
      when SYS_NUMBER_SERIES.ResetPeriodId = 1 then 'Year'
      when SYS_NUMBER_SERIES.ResetPeriodId = 2 then 'Semester'
      when SYS_NUMBER_SERIES.ResetPeriodId = 3 then 'Quarter'
      when SYS_NUMBER_SERIES.ResetPeriodId = 4 then 'Month'
      when SYS_NUMBER_SERIES.ResetPeriodId = 5 then 'Week'
      when SYS_NUMBER_SERIES.ResetPeriodId = 6 then 'Day'
      else ''
   end as ResetPeriod,
   SYS_NUMBER_SERIES.NextNumber,
   SYS_NUMBER_SERIES.LastResetValue,
   SYS_NUMBER_SERIES.IsActive
from
  SYS_NUMBER_SERIES
";
        Module = DataRegistry.AddOrGetModule("NumberSeries", ClassName: "CodeProviderModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "SYS_NUMBER_SERIES";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Pattern", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("ResetPeriodId", "ResetPeriod", TypeStore.Get("ResetPeriod"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("NextNumber", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("LastResetValue", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive", "LastResetValue", "NextNumber", "Pattern", "ResetPeriod"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Pattern"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResetPeriodId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["ResetPeriod"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NextNumber"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["LastResetValue"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_PaymentMethod()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   PaymentMethod.Id,
   PaymentMethod.Code,
   PaymentMethod.Name,
   PaymentMethod.IsActive
from
  PaymentMethod
";
        Module = DataRegistry.AddOrGetModule("PaymentMethod", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "PaymentMethod";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_PaymentTerm()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   PaymentTerm.Id,
   PaymentTerm.Code,
   PaymentTerm.Name,
   PaymentTerm.Days,
   PaymentTerm.IsActive
from
  PaymentTerm
";
        Module = DataRegistry.AddOrGetModule("PaymentTerm", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "PaymentTerm";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Days", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Days", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Days"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_Person()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Person.Id,
   Person.Code,
   Person.Name,
   Person.Title,
   Person.TaxNumber,
   Person.TaxOfficeId,
   Person.CountryId,
   Person.CurrencyId,
   Person.LanguageId,
   Person.AddressLine1,
   Person.AddressLine2,
   Person.City,
   Person.PostalCode,
   Person.Phone,
   Person.Mobile,
   Person.Email,
   Person.Website,
   Person.ContactPerson,
   Person.IsCompany,
   Person.IsActive,
   Person.Color,
   Person.IconName,
   COALESCE(TaxOffice.Code, '') as TaxOffice__Code,
   COALESCE(TaxOffice.Name, '') as TaxOffice__Name,
   COALESCE(Country.Code, '') as Country__Code,
   COALESCE(Country.Name, '') as Country__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(Language.Code, '') as Language__Code,
   COALESCE(Language.Name, '') as Language__Name
from
  Person
    left join TaxOffice TaxOffice on TaxOffice.Id = Person.TaxOfficeId
    left join Country Country on Country.Id = Person.CountryId
    left join Currency Currency on Currency.Id = Person.CurrencyId
    left join Language Language on Language.Id = Person.LanguageId
";
        Module = DataRegistry.AddOrGetModule("Person", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Person";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Tax", "Preferences", "Address", "Appearance", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Title", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("TaxNumber", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Tax");
        tblTop.AddStringLookupId("TaxOfficeId", "TaxOffice", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Tax");
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Preferences");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Preferences");
        tblTop.AddStringLookupId("LanguageId", "Language", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Preferences");
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Mobile", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Website", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("ContactPerson", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        tblTop.AddBoolean("IsCompany", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        string[] FilterFields = ["Name", "AddressLine1", "AddressLine2", "City", "Code", "Color", "ContactPerson", "Country__Code", "Country__Name", "Currency__Code", "Currency__Name", "Email", "IconName", "IsActive", "IsCompany", "Language__Code", "Language__Name", "Mobile", "Phone", "PostalCode", "TaxNumber", "TaxOffice__Code", "TaxOffice__Name", "Title", "Website"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxNumber"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOfficeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LanguageId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["City"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Phone"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Mobile"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Email"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Website"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ContactPerson"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsCompany"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOffice__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxOffice__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Language__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Language__Name"] = DataColumnType.Text;
        TableDef tblPersonRole = tblTop.AddDetail("PersonRole", "Id", "PersonId");
        tblPersonRole.KeyField = "Id";
        tblPersonRole.AddId("Id").SetNullable(false);
        tblPersonRole.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonRole.AddStringLookupId("RoleTypeId", "PersonRoleType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonRole.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblPersonRole.AddDate("StartDate", Flags: FieldFlags.None).SetNullable(true);
        tblPersonRole.AddDate("EndDate", Flags: FieldFlags.None).SetNullable(true);
        tblPersonRole.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblPersonAddress = tblTop.AddDetail("PersonAddress", "Id", "PersonId");
        tblPersonAddress.KeyField = "Id";
        tblPersonAddress.AddId("Id").SetNullable(false);
        tblPersonAddress.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonAddress.AddEnumLookupId("AddressTypeId", "AddressType", TypeStore.Get("AddressType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonAddress.AddString("Code", MaxLength: 40, Flags: FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(true).SetCodeProviderName("PersonAddress");
        tblPersonAddress.AddString("Name", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true);
        tblPersonAddress.AddString("Region", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddString("City", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddString("PostalCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddString("AddressLine1", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddString("AddressLine2", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonAddress.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblPersonAddress.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblPersonContact = tblTop.AddDetail("PersonContact", "Id", "PersonId");
        tblPersonContact.KeyField = "Id";
        tblPersonContact.AddId("Id").SetNullable(false);
        tblPersonContact.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonContact.AddStringLookupId("ContactTypeId", "ContactType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonContact.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPersonContact.AddString("JobTitle", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonContact.AddString("Phone", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblPersonContact.AddString("Mobile", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblPersonContact.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblPersonContact.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblPersonContact.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblPersonBankAccount = tblTop.AddDetail("PersonBankAccount", "Id", "PersonId");
        tblPersonBankAccount.KeyField = "Id";
        tblPersonBankAccount.AddId("Id").SetNullable(false);
        tblPersonBankAccount.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonBankAccount.AddStringLookupId("BankId", "Bank", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblPersonBankAccount.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPersonBankAccount.AddString("Iban", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblPersonBankAccount.AddString("SwiftCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblPersonBankAccount.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblPersonBankAccount.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblPersonBankAccount.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
    }
    static void RegisterModule_PersonRoleType()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   PersonRoleType.Id,
   PersonRoleType.Code,
   PersonRoleType.Name,
   PersonRoleType.IsActive,
   PersonRoleType.Color,
   PersonRoleType.IconName
from
  PersonRoleType
";
        Module = DataRegistry.AddOrGetModule("PersonRoleType", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "PersonRoleType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "IconName", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
    }
    static void RegisterModule_PriceList()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   PriceList.Id,
   PriceList.PriceListTypeId,
   PriceList.DiscountCategoryId,
   PriceList.CustomerId,
   PriceList.ProductId,
   PriceList.UnitOfMeasureId,
   PriceList.MinQuantity,
   PriceList.UnitPrice,
   PriceList.ValidFrom,
   PriceList.ValidTo,
   PriceList.IsActive,
   COALESCE(PriceListType.Code, '') as PriceListType__Code,
   COALESCE(PriceListType.Name, '') as PriceListType__Name,
   COALESCE(DiscountCategory.Name, '') as DiscountCategory__Name,
   COALESCE(Customer.Code, '') as Customer__Code,
   COALESCE(Customer.Name, '') as Customer__Name,
   COALESCE(Customer.Title, '') as Customer__Title,
   COALESCE(Product.Code, '') as Product__Code,
   COALESCE(Product.Name, '') as Product__Name,
   COALESCE(UnitOfMeasure.Code, '') as UnitOfMeasure__Code,
   COALESCE(UnitOfMeasure.Name, '') as UnitOfMeasure__Name
from
  PriceList
    left join PriceListType PriceListType on PriceListType.Id = PriceList.PriceListTypeId
    left join DiscountCategory DiscountCategory on DiscountCategory.Id = PriceList.DiscountCategoryId
    left join Person Customer on Customer.Id = PriceList.CustomerId
    left join Product Product on Product.Id = PriceList.ProductId
    left join UnitOfMeasure UnitOfMeasure on UnitOfMeasure.Id = PriceList.UnitOfMeasureId
";
        Module = DataRegistry.AddOrGetModule("PriceList", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "PriceList";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("PriceListTypeId", "PriceListType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("DiscountCategoryId", "DiscountCategory", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("CustomerId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("MinQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("ValidFrom", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDate("ValidTo", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblCustomer = tblTop.AddJoin("CustomerId", "Person", "Customer", "Id");
        tblTop.Fields.Get("CustomerId").Locator = "Customer";
        tblCustomer.AddId("Id").SetNullable(false);
        tblCustomer.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCustomer.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblCustomer.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblCustomer.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblProduct = tblTop.AddJoin("ProductId", "Product", "Product", "Id");
        tblTop.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Customer__Code", "Customer__Name", "Customer__Title", "DiscountCategory__Name", "IsActive", "MinQuantity", "PriceListType__Code", "PriceListType__Name", "Product__Code", "Product__Name", "UnitOfMeasure__Code", "UnitOfMeasure__Name", "UnitPrice", "ValidFrom", "ValidTo"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PriceListTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DiscountCategoryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CustomerId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProductId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasureId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["MinQuantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["UnitPrice"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["ValidFrom"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ValidTo"] = DataColumnType.Date;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["PriceListType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PriceListType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DiscountCategory__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasure__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasure__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_PriceListType()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   PriceListType.Id,
   PriceListType.Code,
   PriceListType.Name,
   PriceListType.CurrencyId,
   PriceListType.IsTaxIncluded,
   PriceListType.IsDefault,
   PriceListType.IsActive,
   PriceListType.Color,
   PriceListType.IconName,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name
from
  PriceListType
    left join Currency Currency on Currency.Id = PriceListType.CurrencyId
";
        Module = DataRegistry.AddOrGetModule("PriceListType", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "PriceListType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsTaxIncluded", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "Currency__Code", "Currency__Name", "IconName", "IsActive", "IsDefault", "IsTaxIncluded"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsTaxIncluded"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsDefault"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_Product()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Product.Id,
   Product.Code,
   Product.Name,
   Product.ProductTypeId,
   case
      when Product.ProductTypeId = 0 then 'None'
      when Product.ProductTypeId = 1 then 'Goods'
      when Product.ProductTypeId = 2 then 'Service'
      when Product.ProductTypeId = 3 then 'RawMaterial'
      else ''
   end as ProductType,
   Product.CategoryId,
   Product.VatRateId,
   Product.PrimaryUnitOfMeasureId,
   Product.Barcode,
   Product.Weight,
   Product.Volume,
   Product.IsActive,
   Product.Color,
   Product.IconName,
   COALESCE(Category.Code, '') as Category__Code,
   COALESCE(Category.Name, '') as Category__Name,
   COALESCE(VatRate.Code, '') as VatRate__Code,
   COALESCE(VatRate.Name, '') as VatRate__Name,
   COALESCE(PrimaryUnitOfMeasure.Code, '') as PrimaryUnitOfMeasure__Code,
   COALESCE(PrimaryUnitOfMeasure.Name, '') as PrimaryUnitOfMeasure__Name
from
  Product
    left join Category Category on Category.Id = Product.CategoryId
    left join VatRate VatRate on VatRate.Id = Product.VatRateId
    left join UnitOfMeasure PrimaryUnitOfMeasure on PrimaryUnitOfMeasure.Id = Product.PrimaryUnitOfMeasureId
";
        Module = DataRegistry.AddOrGetModule("Product", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Product";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("ProductTypeId", "ProductType", TypeStore.Get("ProductType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CategoryId", "Category", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddStringLookupId("PrimaryUnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDecimal("Weight", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDecimal("Volume", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Barcode", "Category__Code", "Category__Name", "Code", "Color", "IconName", "IsActive", "PrimaryUnitOfMeasure__Code", "PrimaryUnitOfMeasure__Name", "ProductType", "VatRate__Code", "VatRate__Name", "Volume", "Weight"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProductTypeId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["ProductType"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CategoryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRateId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrimaryUnitOfMeasureId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Barcode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Weight"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["Volume"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Category__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Category__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrimaryUnitOfMeasure__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrimaryUnitOfMeasure__Name"] = DataColumnType.Text;
        TableDef tblProductGroups = tblTop.AddDetail("ProductGroups", "Id", "ProductId");
        tblProductGroups.KeyField = "Id";
        tblProductGroups.AddId("Id").SetNullable(false);
        tblProductGroups.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductGroups.AddStringLookupId("GroupId", "ProductGroup", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductGroups.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProductCategory = tblTop.AddDetail("ProductCategory", "Id", "ProductId");
        tblProductCategory.KeyField = "Id";
        tblProductCategory.AddId("Id").SetNullable(false);
        tblProductCategory.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductCategory.AddStringLookupId("CategoryId", "Category", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductCategory.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        TableDef tblProductUnitOfMeasure = tblTop.AddDetail("ProductUnitOfMeasure", "Id", "ProductId");
        tblProductUnitOfMeasure.KeyField = "Id";
        tblProductUnitOfMeasure.AddId("Id").SetNullable(false);
        tblProductUnitOfMeasure.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddStringLookupId("UnitId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddDecimal("Ratio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProductUnitOfMeasure.AddBoolean("IsSalesDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductUnitOfMeasure.AddBoolean("IsPurchaseDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductUnitOfMeasure.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductUnitOfMeasure.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProductBarcode = tblTop.AddDetail("ProductBarcode", "Id", "ProductId");
        tblProductBarcode.KeyField = "Id";
        tblProductBarcode.AddId("Id").SetNullable(false);
        tblProductBarcode.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductBarcode.AddString("Barcode", MaxLength: 512, Flags: FieldFlags.Required).SetNullable(false);
        tblProductBarcode.AddString("Name", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblProductBarcode.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductBarcode.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductBarcode.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblProductSupplier = tblTop.AddDetail("ProductSupplier", "Id", "ProductId");
        tblProductSupplier.KeyField = "Id";
        tblProductSupplier.AddId("Id").SetNullable(false);
        tblProductSupplier.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductSupplier.AddString("SupplierId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductSupplier.AddString("SupplierCode", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblProductSupplier.AddInteger("LeadDays", Flags: FieldFlags.None).SetNullable(true);
        tblProductSupplier.AddDecimal("LastCost", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblProductSupplier.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductSupplier.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductSupplier.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblSupplier = tblProductSupplier.AddJoin("SupplierId", "Person", "Supplier", "Id");
        tblProductSupplier.Fields.Get("SupplierId").Locator = "Person";
        tblSupplier.AddId("Id").SetNullable(false);
        tblSupplier.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblSupplier.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblSupplier.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblSupplier.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblBillOfMaterial = tblTop.AddDetail("BillOfMaterial", "Id", "ProductId");
        tblBillOfMaterial.KeyField = "Id";
        tblBillOfMaterial.AddId("Id").SetNullable(false);
        tblBillOfMaterial.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblBillOfMaterial.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("BillOfMaterial");
        tblBillOfMaterial.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblBillOfMaterial.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false);
        tblBillOfMaterial.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblBillOfMaterial.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblBillOfMaterial.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblBillOfMaterialLine = tblBillOfMaterial.AddDetail("BillOfMaterialLine", "Id", "BillOfMaterialId");
        tblBillOfMaterialLine.KeyField = "Id";
        tblBillOfMaterialLine.AddId("Id").SetNullable(false);
        tblBillOfMaterialLine.AddString("BillOfMaterialId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblBillOfMaterialLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblBillOfMaterialLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false);
        tblBillOfMaterialLine.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
        TableDef tblProduct = tblBillOfMaterialLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblBillOfMaterialLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProductImage = tblTop.AddDetail("ProductImage", "Id", "ProductId");
        tblProductImage.KeyField = "Id";
        tblProductImage.AddId("Id").SetNullable(false);
        tblProductImage.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductImage.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProductImage.AddBlob("ImageBlob", Flags: FieldFlags.None).SetNullable(true);
        tblProductImage.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductImage.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductImage.AddInteger("DisplayOrder", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductImage.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProductAttribute = tblTop.AddDetail("ProductAttribute", "Id", "ProductId");
        tblProductAttribute.KeyField = "Id";
        tblProductAttribute.AddId("Id").SetNullable(false);
        tblProductAttribute.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductAttribute.AddStringLookupId("ProductAttributeGroupId", "ProductAttributeGroup", Flags: FieldFlags.Hidden).SetNullable(true);
        tblProductAttribute.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProductAttribute.AddEnumLookupId("TypeId", "ProductAttributeType", TypeStore.Get("ProductAttributeType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductAttribute.AddString("TextValue", MaxLength: 512, Flags: FieldFlags.Required).SetNullable(false);
        tblProductAttribute.AddString("UnitOfMeasure", MaxLength: 30, Flags: FieldFlags.None).SetNullable(true);
        tblProductAttribute.AddInteger("DisplayOrder", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductAttribute.AddBoolean("IsSpec", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductAttribute.AddBoolean("IsFilter", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductAttribute.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        TableDef tblProductWarehouse = tblTop.AddDetail("ProductWarehouse", "Id", "ProductId");
        tblProductWarehouse.KeyField = "Id";
        tblProductWarehouse.AddId("Id").SetNullable(false);
        tblProductWarehouse.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductWarehouse.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductWarehouse.AddDecimal("MinStock", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblProductWarehouse.AddDecimal("MaxStock", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblProductWarehouse.AddDecimal("ReorderPoint", Decimals: 4, Flags: FieldFlags.None).SetNullable(true);
        tblProductWarehouse.AddBoolean("IsDefault", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductWarehouse.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductWarehouse.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
    }
    static void RegisterModule_ProductAttributeGroup()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ProductAttributeGroup.Id,
   ProductAttributeGroup.Name,
   ProductAttributeGroup.DisplayOrder,
   ProductAttributeGroup.IsActive
from
  ProductAttributeGroup
";
        Module = DataRegistry.AddOrGetModule("ProductAttributeGroup", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ProductAttributeGroup";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("DisplayOrder", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "DisplayOrder", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DisplayOrder"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_ProductBrand()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ProductBrand.Id,
   ProductBrand.Name
from
  ProductBrand
";
        Module = DataRegistry.AddOrGetModule("ProductBrand", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ProductBrand";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_ProductDimension()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ProductDimension.Id,
   ProductDimension.Name,
   ProductDimension.IsActive
from
  ProductDimension
";
        Module = DataRegistry.AddOrGetModule("ProductDimension", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ProductDimension";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        TableDef tblProductDimensionValue = tblTop.AddDetail("ProductDimensionValue", "Id", "ProductDimensionId");
        tblProductDimensionValue.KeyField = "Id";
        tblProductDimensionValue.AddId("Id").SetNullable(false);
        tblProductDimensionValue.AddString("ProductDimensionId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblProductDimensionValue.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProductDimensionValue.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
    }
    static void RegisterModule_ProductGroup()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   ProductGroup.Id,
   ProductGroup.Code,
   ProductGroup.Name,
   ProductGroup.IsSystem,
   ProductGroup.IsActive,
   ProductGroup.Color,
   ProductGroup.IconName
from
  ProductGroup
";
        Module = DataRegistry.AddOrGetModule("ProductGroup", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "ProductGroup";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "IconName", "IsActive", "IsSystem"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsSystem"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
    }
    static void RegisterModule_Project()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Project.Id,
   Project.Code,
   Project.Name,
   Project.CustomerId,
   Project.ProjectStatusId,
   case
      when Project.ProjectStatusId = 0 then 'None'
      when Project.ProjectStatusId = 1 then 'Draft'
      when Project.ProjectStatusId = 2 then 'Active'
      when Project.ProjectStatusId = 3 then 'Suspended'
      when Project.ProjectStatusId = 4 then 'Completed'
      when Project.ProjectStatusId = 5 then 'Cancelled'
      else ''
   end as ProjectStatus,
   Project.StartDate,
   Project.EndDate,
   Project.CostCenterId,
   Project.ManagerPersonId,
   Project.IsActive,
   Project.Color,
   Project.IconName,
   COALESCE(Customer.Code, '') as Customer__Code,
   COALESCE(Customer.Name, '') as Customer__Name,
   COALESCE(Customer.Title, '') as Customer__Title,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(ManagerPerson.Code, '') as ManagerPerson__Code,
   COALESCE(ManagerPerson.Name, '') as ManagerPerson__Name,
   COALESCE(ManagerPerson.Title, '') as ManagerPerson__Title
from
  Project
    left join Person Customer on Customer.Id = Project.CustomerId
    left join CostCenter CostCenter on CostCenter.Id = Project.CostCenterId
    left join Person ManagerPerson on ManagerPerson.Id = Project.ManagerPersonId
";
        Module = DataRegistry.AddOrGetModule("Project", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Project";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Project");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("CustomerId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddEnumLookupId("ProjectStatusId", "ProjectStatus", TypeStore.Get("ProjectStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDate("StartDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddDate("EndDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("CostCenterId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddString("ManagerPersonId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        TableDef tblCustomer = tblTop.AddJoin("CustomerId", "Person", "Customer", "Id");
        tblTop.Fields.Get("CustomerId").Locator = "Customer";
        tblCustomer.AddId("Id").SetNullable(false);
        tblCustomer.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCustomer.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblCustomer.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblCustomer.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblManagerPerson = tblTop.AddJoin("ManagerPersonId", "Person", "ManagerPerson", "Id");
        tblTop.Fields.Get("ManagerPersonId").Locator = "Person";
        tblManagerPerson.AddId("Id").SetNullable(false);
        tblManagerPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblManagerPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblManagerPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblManagerPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        string[] FilterFields = ["Name", "Code", "Color", "CostCenter__Code", "CostCenter__Name", "Customer__Code", "Customer__Name", "Customer__Title", "EndDate", "IconName", "IsActive", "ManagerPerson__Code", "ManagerPerson__Name", "ManagerPerson__Title", "ProjectStatus", "StartDate"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CustomerId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["ProjectStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["StartDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["EndDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Customer__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ManagerPerson__Title"] = DataColumnType.Text;
    }
    static void RegisterModule_PurchaseCancellation()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseCancellation", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_PurchaseCreditNote()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseCreditNote", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_PurchaseDeliveryNote()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseDeliveryNote", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_PurchaseInvoice()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseInvoice", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_PurchaseOrder()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseOrder", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_PurchaseReturn()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("PurchaseReturn", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_ResourceStrings()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   SYS_STR_RES.Id,
   SYS_STR_RES.Lang,
   SYS_STR_RES.ResKey
from
  SYS_STR_RES
";
        Module = DataRegistry.AddOrGetModule("ResourceStrings", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "SYS_STR_RES";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Lang", MaxLength: 12, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("ResKey", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddTextBlob("ResValue", Flags: FieldFlags.Required).SetNullable(false).SetMemo();
        string[] FilterFields = ["Lang", "ResKey"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Lang"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResKey"] = DataColumnType.Text;
    }
    static void RegisterModule_SalesCancellation()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesCancellation", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SalesCreditNote()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesCreditNote", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SalesDeliveryNote()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesDeliveryNote", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SalesInvoice()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesInvoice", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SalesOrder()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesOrder", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SalesPerson()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   SalesPerson.Id,
   SalesPerson.Code,
   SalesPerson.Name,
   SalesPerson.IsActive
from
  SalesPerson
";
        Module = DataRegistry.AddOrGetModule("SalesPerson", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "SalesPerson";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("SalesPerson");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_SalesReturn()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Trade.Id,
   Trade.DocumentTypeId,
   Trade.Code,
   Trade.TradeStatusId,
   case
      when Trade.TradeStatusId = 0 then 'Draft'
      when Trade.TradeStatusId = 1 then 'Posted'
      when Trade.TradeStatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   Trade.TaxTreatmentId,
   case
      when Trade.TaxTreatmentId = 0 then 'None'
      when Trade.TaxTreatmentId = 1 then 'Normal'
      when Trade.TaxTreatmentId = 2 then 'Exempt'
      when Trade.TaxTreatmentId = 3 then 'ThirdCountry'
      when Trade.TaxTreatmentId = 4 then 'IntraCommunity'
      else ''
   end as TaxTreatment,
   Trade.TradeDate,
   Trade.PostingDate,
   Trade.DeliveryDate,
   Trade.DueDate,
   Trade.ExternalRef,
   Trade.PersonId,
   Trade.WarehouseId,
   Trade.SalesPersonId,
   Trade.ProjectId,
   Trade.CostCenterId,
   Trade.BranchId,
   Trade.CurrencyId,
   Trade.ExchangeRate,
   Trade.PaymentMethodId,
   Trade.PaymentTermId,
   Trade.BillingName,
   Trade.BillingAddressLine1,
   Trade.BillingAddressLine2,
   Trade.BillingCity,
   Trade.BillingPostalCode,
   Trade.BillingCountryId,
   Trade.ShippingName,
   Trade.ShippingAddressLine1,
   Trade.ShippingAddressLine2,
   Trade.ShippingCity,
   Trade.ShippingPostalCode,
   Trade.ShippingCountryId,
   Trade.SourceId,
   Trade.CancelsTradeId,
   Trade.CancelledByTradeId,
   Trade.LinesAmount,
   Trade.DiscountPercent,
   Trade.DiscountAmount,
   Trade.DiscountReason,
   Trade.ChargesAmount,
   Trade.NetAmount,
   Trade.VatAmount,
   Trade.TotalAmount,
   Trade.IsLocked,
   Trade.IsCancelled,
   Trade.CreatedAt,
   Trade.CreatedBy,
   Trade.ModifiedAt,
   Trade.ModifiedBy,
   Trade.PostedAt,
   Trade.PostedBy,
   Trade.CancelledAt,
   Trade.CancelledBy,
   Trade.Remarks,
   Trade.Comments,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Person.Code, '') as Person__Code,
   COALESCE(Person.Name, '') as Person__Name,
   COALESCE(Person.Title, '') as Person__Title,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(SalesPerson.Code, '') as SalesPerson__Code,
   COALESCE(SalesPerson.Name, '') as SalesPerson__Name,
   COALESCE(SalesPerson.Title, '') as SalesPerson__Title,
   COALESCE(Project.Code, '') as Project__Code,
   COALESCE(Project.Name, '') as Project__Name,
   COALESCE(CostCenter.Code, '') as CostCenter__Code,
   COALESCE(CostCenter.Name, '') as CostCenter__Name,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Currency.Code, '') as Currency__Code,
   COALESCE(Currency.Name, '') as Currency__Name,
   COALESCE(PaymentMethod.Code, '') as PaymentMethod__Code,
   COALESCE(PaymentMethod.Name, '') as PaymentMethod__Name,
   COALESCE(PaymentTerm.Code, '') as PaymentTerm__Code,
   COALESCE(PaymentTerm.Name, '') as PaymentTerm__Name,
   COALESCE(BillingCountry.Code, '') as BillingCountry__Code,
   COALESCE(BillingCountry.Name, '') as BillingCountry__Name,
   COALESCE(ShippingCountry.Code, '') as ShippingCountry__Code,
   COALESCE(ShippingCountry.Name, '') as ShippingCountry__Name
from
  Trade
    left join DocumentType DocumentType on DocumentType.Id = Trade.DocumentTypeId
    left join Person Person on Person.Id = Trade.PersonId
    left join Warehouse Warehouse on Warehouse.Id = Trade.WarehouseId
    left join Person SalesPerson on SalesPerson.Id = Trade.SalesPersonId
    left join Project Project on Project.Id = Trade.ProjectId
    left join CostCenter CostCenter on CostCenter.Id = Trade.CostCenterId
    left join CompanyBranch Branch on Branch.Id = Trade.BranchId
    left join Currency Currency on Currency.Id = Trade.CurrencyId
    left join PaymentMethod PaymentMethod on PaymentMethod.Id = Trade.PaymentMethodId
    left join PaymentTerm PaymentTerm on PaymentTerm.Id = Trade.PaymentTermId
    left join Country BillingCountry on BillingCountry.Id = Trade.BillingCountryId
    left join Country ShippingCountry on ShippingCountry.Id = Trade.ShippingCountryId
    left join AppUser CreatedBy on CreatedBy.Id = Trade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = Trade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = Trade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = Trade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("SalesReturn", ClassName: "TradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Trade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Dates", "Party", "Organization", "Payment", "Billing", "Shipping", "Relations", "Amounts", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblTop.AddEnumLookupId("TradeStatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddEnumLookupId("TaxTreatmentId", "TaxTreatment", TypeStore.Get("TaxTreatment"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDate("TradeDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DeliveryDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddDate("DueDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddString("ExternalRef", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Party");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Party");
        tblTop.AddStringLookupId("SalesPersonId", "Person", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("ProjectId", "Project", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CostCenterId", "CostCenter", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Organization");
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Payment");
        tblTop.AddDecimal("ExchangeRate", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentMethodId", "PaymentMethod", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddStringLookupId("PaymentTermId", "PaymentTerm", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Payment");
        tblTop.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblTop.AddStringLookupId("BillingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Billing");
        tblTop.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine1", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingAddressLine2", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingCity", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblTop.AddStringLookupId("ShippingCountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Shipping");
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelsTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDecimal("LinesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddString("DiscountReason", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true).SetGroup("Amounts");
        tblTop.AddDecimal("ChargesAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Amounts");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddString("Comments", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        TableDef tblPerson = tblTop.AddJoin("PersonId", "Person", "Person", "Id");
        tblTop.Fields.Get("PersonId").Locator = "Person";
        tblPerson.AddId("Id").SetNullable(false);
        tblPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        TableDef tblSource = tblTop.AddJoin("SourceId", "Trade", "Source", "Id");
        tblTop.Fields.Get("SourceId").Locator = "Trade";
        tblSource.AddId("Id").SetNullable(false);
        tblSource.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblSource.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblSource.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblSource.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelsTrade = tblTop.AddJoin("CancelsTradeId", "Trade", "CancelsTrade", "Id");
        tblTop.Fields.Get("CancelsTradeId").Locator = "Trade";
        tblCancelsTrade.AddId("Id").SetNullable(false);
        tblCancelsTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelsTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelsTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelsTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        TableDef tblCancelledByTrade = tblTop.AddJoin("CancelledByTradeId", "Trade", "CancelledByTrade", "Id");
        tblTop.Fields.Get("CancelledByTradeId").Locator = "Trade";
        tblCancelledByTrade.AddId("Id").SetNullable(false);
        tblCancelledByTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("TRADE-DRAFT");
        tblCancelledByTrade.AddString("BillingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("BillingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Billing");
        tblCancelledByTrade.AddString("ShippingName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        tblCancelledByTrade.AddString("ShippingPostalCode", MaxLength: 20, Flags: FieldFlags.None).SetNullable(true).SetGroup("Shipping");
        string[] FilterFields = ["BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingCountry__Code", "BillingCountry__Name", "BillingName", "BillingPostalCode", "Branch__Code", "Branch__Name", "CancelledAt", "CancelledBy", "ChargesAmount", "Code", "Comments", "CostCenter__Code", "CostCenter__Name", "CreatedAt", "CreatedBy", "Currency__Code", "Currency__Name", "DeliveryDate", "DiscountAmount", "DiscountPercent", "DiscountReason", "DocumentType__Code", "DocumentType__Name", "DueDate", "ExchangeRate", "ExternalRef", "IsCancelled", "IsLocked", "LinesAmount", "ModifiedAt", "ModifiedBy", "NetAmount", "PaymentMethod__Code", "PaymentMethod__Name", "PaymentTerm__Code", "PaymentTerm__Name", "Person__Code", "Person__Name", "Person__Title", "PostedAt", "PostedBy", "PostingDate", "Project__Code", "Project__Name", "Remarks", "SalesPerson__Code", "SalesPerson__Name", "SalesPerson__Title", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingCountry__Code", "ShippingCountry__Name", "ShippingName", "ShippingPostalCode", "TaxTreatment", "TotalAmount", "TradeDate", "TradeStatus", "VatAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeStatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TaxTreatmentId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TaxTreatment"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DeliveryDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["DueDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["ExternalRef"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProjectId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenterId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CurrencyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ExchangeRate"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PaymentMethodId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTermId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingAddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCity"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingPostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelsTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["LinesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountPercent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["DiscountAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["DiscountReason"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ChargesAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["NetAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["VatAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["TotalAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Comments"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Person__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SalesPerson__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Project__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CostCenter__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Currency__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentMethod__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PaymentTerm__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BillingCountry__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ShippingCountry__Name"] = DataColumnType.Text;
        TableDef tblTradeTax = tblTop.AddDetail("TradeTax", "Id", "TradeId");
        tblTradeTax.KeyField = "Id";
        tblTradeTax.AddId("Id").SetNullable(false);
        tblTradeTax.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeTax.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeTax.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        TableDef tblTradeLine = tblTop.AddDetail("TradeLine", "Id", "TradeId");
        tblTradeLine.KeyField = "Id";
        tblTradeLine.AddId("Id").SetNullable(false);
        tblTradeLine.AddString("TradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblTradeLine.AddEnumLookupId("LineTypeId", "TradeLineType", TypeStore.Get("TradeLineType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddString("Description", MaxLength: 256, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("PrimaryUnitQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTradeLine.AddDecimal("VatRatePercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("GrossAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountPercent", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("DiscountAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetUnitPrice", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("NetAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("VatAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddDecimal("TotalAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblSourceTradeLine = tblTradeLine.AddJoin("SourceTradeLineId", "TradeLine", "SourceTradeLine", "Id");
        tblTradeLine.Fields.Get("SourceTradeLineId").Locator = "TradeLine";
        tblSourceTradeLine.AddId("Id").SetNullable(false);
        tblSourceTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.None).SetNullable(true);
        tblSourceTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_StockBalance()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockBalance.Id,
   StockBalance.ProductId,
   StockBalance.WarehouseId,
   StockBalance.PrimaryQuantity,
   StockBalance.TotalCostAmount,
   StockBalance.AverageUnitCost,
   StockBalance.LastMovementDate,
   StockBalance.LastMovementId,
   COALESCE(Product.Code, '') as Product__Code,
   COALESCE(Product.Name, '') as Product__Name,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name
from
  StockBalance
    left join Product Product on Product.Id = StockBalance.ProductId
    left join Warehouse Warehouse on Warehouse.Id = StockBalance.WarehouseId
    left join StockMovement LastMovement on LastMovement.Id = StockBalance.LastMovementId
";
        Module = DataRegistry.AddOrGetModule("StockBalance", ClassName: "StockBalanceDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockBalance";
        tblTop.KeyField = "Id";
        tblTop.IsUiVisible = false;
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("PrimaryQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("TotalCostAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("AverageUnitCost", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDate("LastMovementDate", Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("LastMovementId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        TableDef tblProduct = tblTop.AddJoin("ProductId", "Product", "Product", "Id");
        tblTop.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["AverageUnitCost", "LastMovementDate", "PrimaryQuantity", "Product__Code", "Product__Name", "TotalCostAmount", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProductId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrimaryQuantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["TotalCostAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["AverageUnitCost"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["LastMovementDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["LastMovementId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_StockCount()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockCount.Id,
   StockCount.Code,
   StockCount.WarehouseId,
   StockCount.CountDate,
   StockCount.StatusId,
   case
      when StockCount.StatusId = 0 then 'Draft'
      when StockCount.StatusId = 1 then 'Posted'
      when StockCount.StatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   StockCount.CancelledDocumentId,
   StockCount.CancellationDocumentId,
   StockCount.CreatedAt,
   StockCount.CreatedBy,
   StockCount.ModifiedAt,
   StockCount.ModifiedBy,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name
from
  StockCount
    left join Warehouse Warehouse on Warehouse.Id = StockCount.WarehouseId
    left join AppUser CreatedBy on CreatedBy.Id = StockCount.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = StockCount.ModifiedBy
";
        Module = DataRegistry.AddOrGetModule("StockCount", ClassName: "StockCountDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockCount";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Relations", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_COUNT");
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("CountDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("StatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        tblTop.AddString("CancelledDocumentId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancellationDocumentId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        TableDef tblCancelledDocument = tblTop.AddJoin("CancelledDocumentId", "StockCount", "CancelledDocument", "Id");
        tblTop.Fields.Get("CancelledDocumentId").Locator = "StockCount";
        tblCancelledDocument.AddId("Id").SetNullable(false);
        tblCancelledDocument.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_COUNT");
        TableDef tblCancellationDocument = tblTop.AddJoin("CancellationDocumentId", "StockCount", "CancellationDocument", "Id");
        tblTop.Fields.Get("CancellationDocumentId").Locator = "StockCount";
        tblCancellationDocument.AddId("Id").SetNullable(false);
        tblCancellationDocument.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_COUNT");
        string[] FilterFields = ["Code", "CountDate", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "TradeStatus", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CountDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["StatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledDocumentId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancellationDocumentId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        TableDef tblStockCountLine = tblTop.AddDetail("StockCountLine", "Id", "StockCountId");
        tblStockCountLine.KeyField = "Id";
        tblStockCountLine.AddId("Id").SetNullable(false);
        tblStockCountLine.AddString("StockCountId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddString("ProductName", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockCountLine.AddDecimal("SystemQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockCountLine.AddDecimal("CountedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockCountLine.AddDecimal("DifferenceQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockCountLine.AddDecimal("UnitCost", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockCountLine.AddDecimal("DifferenceCostAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockCountLine.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProduct = tblStockCountLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblStockCountLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_StockMovement()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockMovement.Id,
   StockMovement.ProductId,
   StockMovement.WarehouseId,
   StockMovement.MovementDate,
   StockMovement.Direction,
   StockMovement.Quantity,
   StockMovement.PrimaryQuantity,
   StockMovement.UnitOfMeasureId,
   StockMovement.UnitOfMeasureName,
   StockMovement.UnitRatio,
   StockMovement.UnitCost,
   StockMovement.CostAmount,
   StockMovement.SourceModule,
   StockMovement.SourceTable,
   StockMovement.SourceId,
   StockMovement.DocumentTypeId,
   StockMovement.DocumentCode,
   StockMovement.DocumentDate,
   StockMovement.CreatedAt,
   StockMovement.CreatedBy,
   COALESCE(Product.Code, '') as Product__Code,
   COALESCE(Product.Name, '') as Product__Name,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(UnitOfMeasure.Code, '') as UnitOfMeasure__Code,
   COALESCE(UnitOfMeasure.Name, '') as UnitOfMeasure__Name,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name
from
  StockMovement
    left join Product Product on Product.Id = StockMovement.ProductId
    left join Warehouse Warehouse on Warehouse.Id = StockMovement.WarehouseId
    left join UnitOfMeasure UnitOfMeasure on UnitOfMeasure.Id = StockMovement.UnitOfMeasureId
    left join DocumentType DocumentType on DocumentType.Id = StockMovement.DocumentTypeId
    left join AppUser CreatedBy on CreatedBy.Id = StockMovement.CreatedBy
";
        Module = DataRegistry.AddOrGetModule("StockMovement", ClassName: "StockMovementDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockMovement";
        tblTop.KeyField = "Id";
        tblTop.IsUiVisible = false;
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("MovementDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Direction", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("PrimaryQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddDecimal("UnitCost", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("CostAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("SourceModule", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceTable", MaxLength: 64, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("DocumentCode", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("DocumentDate", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false);
        TableDef tblProduct = tblTop.AddJoin("ProductId", "Product", "Product", "Id");
        tblTop.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["CostAmount", "CreatedAt", "CreatedBy", "Direction", "DocumentCode", "DocumentDate", "DocumentType__Code", "DocumentType__Name", "MovementDate", "PrimaryQuantity", "Product__Code", "Product__Name", "Quantity", "SourceModule", "SourceTable", "UnitCost", "UnitOfMeasure__Code", "UnitOfMeasure__Name", "UnitOfMeasureName", "UnitRatio", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProductId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["MovementDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["Direction"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["Quantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["PrimaryQuantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["UnitOfMeasureId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasureName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitRatio"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["UnitCost"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["CostAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["SourceModule"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceTable"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasure__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["UnitOfMeasure__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_StockReason()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockReason.Id,
   StockReason.Code,
   StockReason.Name,
   StockReason.StockDirection,
   StockReason.AffectsCost,
   StockReason.RequiresRemarks,
   StockReason.IsSystem,
   StockReason.IsActive,
   StockReason.Color,
   StockReason.IconName
from
  StockReason
";
        Module = DataRegistry.AddOrGetModule("StockReason", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockReason";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("StockDirection", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsCost", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("RequiresRemarks", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "AffectsCost", "Code", "Color", "IconName", "IsActive", "IsSystem", "RequiresRemarks", "StockDirection"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["StockDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["AffectsCost"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["RequiresRemarks"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsSystem"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
    }
    static void RegisterModule_StockReservation()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockReservation.Id,
   StockReservation.ProductId,
   StockReservation.WarehouseId,
   StockReservation.ReservedQuantity,
   StockReservation.ExecutedQuantity,
   StockReservation.SourceModule,
   StockReservation.SourceTable,
   StockReservation.SourceId,
   StockReservation.SourceLineId,
   StockReservation.CreatedAt,
   COALESCE(Product.Code, '') as Product__Code,
   COALESCE(Product.Name, '') as Product__Name,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name
from
  StockReservation
    left join Product Product on Product.Id = StockReservation.ProductId
    left join Warehouse Warehouse on Warehouse.Id = StockReservation.WarehouseId
";
        Module = DataRegistry.AddOrGetModule("StockReservation", ClassName: "StockReservationDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockReservation";
        tblTop.KeyField = "Id";
        tblTop.IsUiVisible = false;
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("ReservedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("ExecutedQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("SourceModule", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceTable", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("SourceLineId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false);
        TableDef tblProduct = tblTop.AddJoin("ProductId", "Product", "Product", "Id");
        tblTop.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["CreatedAt", "ExecutedQuantity", "Product__Code", "Product__Name", "ReservedQuantity", "SourceModule", "SourceTable", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ProductId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ReservedQuantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["ExecutedQuantity"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["SourceModule"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceTable"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["SourceLineId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["Product__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Product__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_StockTrade()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   StockTrade.Id,
   StockTrade.DocumentTypeId,
   StockTrade.WarehouseId,
   StockTrade.ToWarehouseId,
   StockTrade.Code,
   StockTrade.DocumentDate,
   StockTrade.PostingDate,
   StockTrade.StatusId,
   case
      when StockTrade.StatusId = 0 then 'Draft'
      when StockTrade.StatusId = 1 then 'Posted'
      when StockTrade.StatusId = 2 then 'Cancelled'
      else ''
   end as TradeStatus,
   StockTrade.TotalCostAmount,
   StockTrade.Remarks,
   StockTrade.IsLocked,
   StockTrade.IsCancelled,
   StockTrade.CancelsStockTradeId,
   StockTrade.CancelledByStockTradeId,
   StockTrade.CreatedAt,
   StockTrade.CreatedBy,
   StockTrade.ModifiedAt,
   StockTrade.ModifiedBy,
   StockTrade.PostedAt,
   StockTrade.PostedBy,
   StockTrade.CancelledAt,
   StockTrade.CancelledBy,
   COALESCE(DocumentType.Code, '') as DocumentType__Code,
   COALESCE(DocumentType.Name, '') as DocumentType__Name,
   COALESCE(Warehouse.Code, '') as Warehouse__Code,
   COALESCE(Warehouse.Name, '') as Warehouse__Name,
   COALESCE(ToWarehouse.Code, '') as ToWarehouse__Code,
   COALESCE(ToWarehouse.Name, '') as ToWarehouse__Name
from
  StockTrade
    left join DocumentType DocumentType on DocumentType.Id = StockTrade.DocumentTypeId
    left join Warehouse Warehouse on Warehouse.Id = StockTrade.WarehouseId
    left join Warehouse ToWarehouse on ToWarehouse.Id = StockTrade.ToWarehouseId
    left join AppUser CreatedBy on CreatedBy.Id = StockTrade.CreatedBy
    left join AppUser ModifiedBy on ModifiedBy.Id = StockTrade.ModifiedBy
    left join AppUser PostedBy on PostedBy.Id = StockTrade.PostedBy
    left join AppUser CancelledBy on CancelledBy.Id = StockTrade.CancelledBy
";
        Module = DataRegistry.AddOrGetModule("StockTrade", ClassName: "StockTradeDataModule", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "StockTrade";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Warehouses", "Dates", "Relations", "Status", "Audit", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("DocumentTypeId", "DocumentType", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetGroup("Warehouses");
        tblTop.AddStringLookupId("ToWarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Warehouses");
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_TRADE_DRAFT");
        tblTop.AddDate("DocumentDate", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Dates");
        tblTop.AddDate("PostingDate", Flags: FieldFlags.None).SetNullable(true).SetGroup("Dates");
        tblTop.AddEnumLookupId("StatusId", "TradeStatus", TypeStore.Get("TradeStatus"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("TotalCostAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true).SetMemo().SetGroup("Notes");
        tblTop.AddBoolean("IsLocked", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddBoolean("IsCancelled", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Status");
        tblTop.AddString("CancelsStockTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddString("CancelledByStockTradeId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Relations");
        tblTop.AddDateTime("CreatedAt", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddStringLookupId("CreatedBy", "AppUser", Flags: FieldFlags.Required).SetNullable(false).SetGroup("Audit");
        tblTop.AddDateTime("ModifiedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("ModifiedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("PostedAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("PostedBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddDateTime("CancelledAt", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        tblTop.AddStringLookupId("CancelledBy", "AppUser", Flags: FieldFlags.None).SetNullable(true).SetGroup("Audit");
        TableDef tblCancelsStockTrade = tblTop.AddJoin("CancelsStockTradeId", "StockTrade", "CancelsStockTrade", "Id");
        tblTop.Fields.Get("CancelsStockTradeId").Locator = "StockTrade";
        tblCancelsStockTrade.AddId("Id").SetNullable(false);
        tblCancelsStockTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_TRADE_DRAFT");
        TableDef tblCancelledByStockTrade = tblTop.AddJoin("CancelledByStockTradeId", "StockTrade", "CancelledByStockTrade", "Id");
        tblTop.Fields.Get("CancelledByStockTradeId").Locator = "StockTrade";
        tblCancelledByStockTrade.AddId("Id").SetNullable(false);
        tblCancelledByStockTrade.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("STOCK_TRADE_DRAFT");
        string[] FilterFields = ["CancelledAt", "CancelledBy", "Code", "CreatedAt", "CreatedBy", "DocumentDate", "DocumentType__Code", "DocumentType__Name", "IsCancelled", "IsLocked", "ModifiedAt", "ModifiedBy", "PostedAt", "PostedBy", "PostingDate", "Remarks", "TotalCostAmount", "ToWarehouse__Code", "ToWarehouse__Name", "TradeStatus", "Warehouse__Code", "Warehouse__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ToWarehouseId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["PostingDate"] = DataColumnType.Date;
        SelectDef.ColumnTypes["StatusId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeStatus"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TotalCostAmount"] = DataColumnType.Currency;
        SelectDef.ColumnTypes["Remarks"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsLocked"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsCancelled"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["CancelsStockTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledByStockTradeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CreatedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CreatedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ModifiedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["ModifiedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostedAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["PostedBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CancelledAt"] = DataColumnType.DateTime;
        SelectDef.ColumnTypes["CancelledBy"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["DocumentType__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Warehouse__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ToWarehouse__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ToWarehouse__Name"] = DataColumnType.Text;
        TableDef tblStockTradeLine = tblTop.AddDetail("StockTradeLine", "Id", "StockTradeId");
        tblStockTradeLine.KeyField = "Id";
        tblStockTradeLine.AddId("Id").SetNullable(false);
        tblStockTradeLine.AddString("StockTradeId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddInteger("LineNo", Flags: FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddString("ProductCode", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddString("ProductName", MaxLength: 128, Flags: FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddStringLookupId("WarehouseId", "Warehouse", Flags: FieldFlags.Hidden).SetNullable(true);
        tblStockTradeLine.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddString("UnitOfMeasureName", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
        tblStockTradeLine.AddDecimal("UnitRatio", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblStockTradeLine.AddDecimal("Quantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockTradeLine.AddDecimal("PrimaryQuantity", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockTradeLine.AddDecimal("UnitCost", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockTradeLine.AddDecimal("CostAmount", Decimals: 4, Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblStockTradeLine.AddString("SourceTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblStockTradeLine.AddString("SourceStockTradeLineId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true);
        tblStockTradeLine.AddString("Remarks", MaxLength: 512, Flags: FieldFlags.None).SetNullable(true);
        TableDef tblProduct = tblStockTradeLine.AddJoin("ProductId", "Product", "Product", "Id");
        tblStockTradeLine.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.None).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
    }
    static void RegisterModule_SupplierCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   SupplierCategory.Id,
   SupplierCategory.Name
from
  SupplierCategory
";
        Module = DataRegistry.AddOrGetModule("SupplierCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "SupplierCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_TaxCategory()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   TaxCategory.Id,
   TaxCategory.Code,
   TaxCategory.Name,
   TaxCategory.VatRateId,
   TaxCategory.IsDomestic,
   TaxCategory.IsEuropeanUnion,
   TaxCategory.IsThirdCountry,
   TaxCategory.IsTaxExempt,
   TaxCategory.IsReverseCharge,
   TaxCategory.IsIntrastat,
   TaxCategory.IsVies,
   TaxCategory.IsActive,
   TaxCategory.Color,
   TaxCategory.IconName,
   COALESCE(VatRate.Code, '') as VatRate__Code,
   COALESCE(VatRate.Name, '') as VatRate__Name
from
  TaxCategory
    left join VatRate VatRate on VatRate.Id = TaxCategory.VatRateId
";
        Module = DataRegistry.AddOrGetModule("TaxCategory", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "TaxCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddBoolean("IsDomestic", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsEuropeanUnion", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsThirdCountry", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsTaxExempt", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsReverseCharge", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsIntrastat", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsVies", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true);
        string[] FilterFields = ["Name", "Code", "Color", "IconName", "IsActive", "IsDomestic", "IsEuropeanUnion", "IsIntrastat", "IsReverseCharge", "IsTaxExempt", "IsThirdCountry", "IsVies", "VatRate__Code", "VatRate__Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRateId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsDomestic"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsEuropeanUnion"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsThirdCountry"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsTaxExempt"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsReverseCharge"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsIntrastat"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsVies"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["VatRate__Name"] = DataColumnType.Text;
    }
    static void RegisterModule_TaxOffice()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   TaxOffice.Id,
   TaxOffice.Code,
   TaxOffice.Name
from
  TaxOffice
";
        Module = DataRegistry.AddOrGetModule("TaxOffice", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "TaxOffice";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_UnitOfMeasure()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   UnitOfMeasure.Id,
   UnitOfMeasure.Code,
   UnitOfMeasure.Name
from
  UnitOfMeasure
";
        Module = DataRegistry.AddOrGetModule("UnitOfMeasure", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "UnitOfMeasure";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
    }
    static void RegisterModule_VatRate()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   VatRate.Id,
   VatRate.Code,
   VatRate.Name,
   VatRate.Percent,
   VatRate.IsActive
from
  VatRate
";
        Module = DataRegistry.AddOrGetModule("VatRate", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "VatRate";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("Percent", Decimals: 2, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive", "Percent"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Percent"] = DataColumnType.Decimal;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
    }
    static void RegisterModule_Warehouse()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        SqlText = @"
select
   Warehouse.Id,
   Warehouse.Code,
   Warehouse.Name,
   Warehouse.CompanyId,
   Warehouse.BranchId,
   Warehouse.WarehouseTypeId,
   case
      when Warehouse.WarehouseTypeId = 0 then 'None'
      when Warehouse.WarehouseTypeId = 1 then 'Main'
      when Warehouse.WarehouseTypeId = 2 then 'Store'
      when Warehouse.WarehouseTypeId = 3 then 'Transit'
      when Warehouse.WarehouseTypeId = 4 then 'Production'
      when Warehouse.WarehouseTypeId = 5 then 'Scrap'
      when Warehouse.WarehouseTypeId = 6 then 'Virtual'
      else ''
   end as WarehouseType,
   Warehouse.AddressLine1,
   Warehouse.AddressLine2,
   Warehouse.City,
   Warehouse.PostalCode,
   Warehouse.CountryId,
   Warehouse.Phone,
   Warehouse.Email,
   Warehouse.ResponsiblePersonId,
   Warehouse.IsActive,
   Warehouse.IsVirtual,
   Warehouse.AllowNegativeStock,
   Warehouse.AffectsAvailability,
   Warehouse.Color,
   Warehouse.IconName,
   COALESCE(Company.Code, '') as Company__Code,
   COALESCE(Company.Name, '') as Company__Name,
   COALESCE(Company.Title, '') as Company__Title,
   COALESCE(Branch.Code, '') as Branch__Code,
   COALESCE(Branch.Name, '') as Branch__Name,
   COALESCE(Country.Code, '') as Country__Code,
   COALESCE(Country.Name, '') as Country__Name,
   COALESCE(ResponsiblePerson.Code, '') as ResponsiblePerson__Code,
   COALESCE(ResponsiblePerson.Name, '') as ResponsiblePerson__Name,
   COALESCE(ResponsiblePerson.Title, '') as ResponsiblePerson__Title
from
  Warehouse
    left join Company Company on Company.Id = Warehouse.CompanyId
    left join CompanyBranch Branch on Branch.Id = Warehouse.BranchId
    left join Country Country on Country.Id = Warehouse.CountryId
    left join Person ResponsiblePerson on ResponsiblePerson.Id = Warehouse.ResponsiblePersonId
";
        Module = DataRegistry.AddOrGetModule("Warehouse", ListSelectSql: SqlText);
        if (Module.Table.Fields.Count > 0)
            return;
        tblTop = Module.Table;
        tblTop.Name = "Warehouse";
        tblTop.KeyField = "Id";
        tblTop.FieldGroups.AddRange(["Address", "Settings", "Appearance", "Notes"]);
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Warehouse");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CompanyId", "Company", Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Hidden).SetNullable(true);
        tblTop.AddEnumLookupId("WarehouseTypeId", "WarehouseType", TypeStore.Get("WarehouseType"), Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblTop.AddString("ResponsiblePersonId", MaxLength: 40, Flags: FieldFlags.Hidden).SetNullable(true).SetGroup("Settings");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Settings");
        tblTop.AddBoolean("IsVirtual", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Settings");
        tblTop.AddBoolean("AllowNegativeStock", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Settings");
        tblTop.AddBoolean("AffectsAvailability", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1").SetGroup("Settings");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
        TableDef tblResponsiblePerson = tblTop.AddJoin("ResponsiblePersonId", "Person", "ResponsiblePerson", "Id");
        tblTop.Fields.Get("ResponsiblePersonId").Locator = "Person";
        tblResponsiblePerson.AddId("Id").SetNullable(false);
        tblResponsiblePerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblResponsiblePerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblResponsiblePerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.None).SetNullable(true).SetGroup("Address");
        tblResponsiblePerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetGroup("Appearance");
        string[] FilterFields = ["Name", "AddressLine1", "AddressLine2", "AffectsAvailability", "AllowNegativeStock", "Branch__Code", "Branch__Name", "City", "Code", "Color", "Company__Code", "Company__Name", "Company__Title", "Country__Code", "Country__Name", "Email", "IconName", "IsActive", "IsVirtual", "Phone", "PostalCode", "ResponsiblePerson__Code", "ResponsiblePerson__Name", "ResponsiblePerson__Title", "WarehouseType"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CompanyId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["BranchId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["WarehouseTypeId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["WarehouseType"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine1"] = DataColumnType.Text;
        SelectDef.ColumnTypes["AddressLine2"] = DataColumnType.Text;
        SelectDef.ColumnTypes["City"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PostalCode"] = DataColumnType.Text;
        SelectDef.ColumnTypes["CountryId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Phone"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Email"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResponsiblePersonId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["IsVirtual"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AllowNegativeStock"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsAvailability"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Company__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Company__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Company__Title"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Branch__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Country__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResponsiblePerson__Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResponsiblePerson__Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ResponsiblePerson__Title"] = DataColumnType.Text;
        TableDef tblWarehouseLocation = tblTop.AddDetail("WarehouseLocation", "Id", "WarehouseId");
        tblWarehouseLocation.KeyField = "Id";
        tblWarehouseLocation.AddId("Id").SetNullable(false);
        tblWarehouseLocation.AddString("WarehouseId", MaxLength: 40, Flags: FieldFlags.Hidden | FieldFlags.Required).SetNullable(false);
        tblWarehouseLocation.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("WarehouseLocation");
        tblWarehouseLocation.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
        tblWarehouseLocation.AddString("Zone", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblWarehouseLocation.AddString("Aisle", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblWarehouseLocation.AddString("Rack", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblWarehouseLocation.AddString("Shelf", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblWarehouseLocation.AddString("Bin", MaxLength: 40, Flags: FieldFlags.None).SetNullable(true);
        tblWarehouseLocation.AddBoolean("IsActive", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblWarehouseLocation.AddTextBlob("Notes", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo();
    }

    // ● static public
    static public void RegisterModules()
    {
        RegisterCodeProviders_FromModules();
        RegisterLookups_FromModules();
        RegisterLocators_FromModules();
        RegisterModule_Account();
        RegisterModule_AppUser();
        RegisterModule_Asset();
        RegisterModule_AssetCategory();
        RegisterModule_AssetDepreciationMethod();
        RegisterModule_AssetLocation();
        RegisterModule_Bank();
        RegisterModule_Carrier();
        RegisterModule_CashAccount();
        RegisterModule_Category();
        RegisterModule_Company();
        RegisterModule_ContactType();
        RegisterModule_CostCenter();
        RegisterModule_Country();
        RegisterModule_Currency();
        RegisterModule_CustomerCategory();
        RegisterModule_DiscountCategory();
        RegisterModule_DocumentType();
        RegisterModule_ExpenseCategory();
        RegisterModule_FinanceBalance();
        RegisterModule_FinanceMovement();
        RegisterModule_FiscalYear();
        RegisterModule_FixedAsset();
        RegisterModule_JournalEntry();
        RegisterModule_Language();
        RegisterModule_Log();
        RegisterModule_NumberSeries();
        RegisterModule_PaymentMethod();
        RegisterModule_PaymentTerm();
        RegisterModule_Person();
        RegisterModule_PersonRoleType();
        RegisterModule_PriceList();
        RegisterModule_PriceListType();
        RegisterModule_Product();
        RegisterModule_ProductAttributeGroup();
        RegisterModule_ProductBrand();
        RegisterModule_ProductDimension();
        RegisterModule_ProductGroup();
        RegisterModule_Project();
        RegisterModule_PurchaseCancellation();
        RegisterModule_PurchaseCreditNote();
        RegisterModule_PurchaseDeliveryNote();
        RegisterModule_PurchaseInvoice();
        RegisterModule_PurchaseOrder();
        RegisterModule_PurchaseReturn();
        RegisterModule_ResourceStrings();
        RegisterModule_SalesCancellation();
        RegisterModule_SalesCreditNote();
        RegisterModule_SalesDeliveryNote();
        RegisterModule_SalesInvoice();
        RegisterModule_SalesOrder();
        RegisterModule_SalesPerson();
        RegisterModule_SalesReturn();
        RegisterModule_StockBalance();
        RegisterModule_StockCount();
        RegisterModule_StockMovement();
        RegisterModule_StockReason();
        RegisterModule_StockReservation();
        RegisterModule_StockTrade();
        RegisterModule_SupplierCategory();
        RegisterModule_TaxCategory();
        RegisterModule_TaxOffice();
        RegisterModule_UnitOfMeasure();
        RegisterModule_VatRate();
        RegisterModule_Warehouse();
    }
}
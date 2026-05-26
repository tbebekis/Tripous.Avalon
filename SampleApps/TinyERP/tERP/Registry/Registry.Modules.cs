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
        DataRegistry.AddCodeProvider("Company");
        DataRegistry.AddCodeProvider("Product");
        DataRegistry.AddCodeProvider("Project");
        DataRegistry.AddCodeProvider("SalesPerson");
        DataRegistry.AddCodeProvider("Warehouse");
    }
    static void RegisterLookupSources_FromModules()
    {
        DataRegistry.AddLookupWithTableName("Bank", "Bank", FormName: "Bank");
        DataRegistry.AddLookupWithTableName("Carrier", "Carrier", FormName: "Carrier");
        DataRegistry.AddLookupWithTableName("Category", "Category", FormName: "Category");
        DataRegistry.AddLookupWithTableName("Company", "Company", FormName: "Company");
        DataRegistry.AddLookupWithTableName("CompanyBranch", "CompanyBranch");
        DataRegistry.AddLookupWithTableName("CostCenter", "CostCenter", FormName: "CostCenter");
        DataRegistry.AddLookupWithTableName("Country", "Country", FormName: "Country");
        DataRegistry.AddLookupWithTableName("Currency", "Currency", FormName: "Currency");
        DataRegistry.AddLookupWithTableName("CustomerCategory", "CustomerCategory", FormName: "CustomerCategory");
        DataRegistry.AddLookupWithTableName("DiscountCategory", "DiscountCategory", FormName: "DiscountCategory");
        DataRegistry.AddLookupWithTableName("DocumentType", "DocumentType", FormName: "DocumentType");
        DataRegistry.AddLookupWithTableName("ExpenseCategory", "ExpenseCategory", FormName: "ExpenseCategory");
        DataRegistry.AddLookupWithTableName("Language", "Language", FormName: "Language");
        DataRegistry.AddLookupWithTableName("PaymentMethod", "PaymentMethod", FormName: "PaymentMethod");
        DataRegistry.AddLookupWithTableName("PaymentTerm", "PaymentTerm", FormName: "PaymentTerm");
        DataRegistry.AddLookupWithTableName("PersonRoleType", "PersonRoleType", FormName: "PersonRoleType");
        DataRegistry.AddLookupWithTableName("PriceListType", "PriceListType", FormName: "PriceListType");
        DataRegistry.AddLookupWithTableName("ProductBrand", "ProductBrand", FormName: "ProductBrand");
        DataRegistry.AddLookupWithTableName("ProductGroup", "ProductGroup", FormName: "ProductGroup");
        DataRegistry.AddLookupWithTableName("SalesPerson", "SalesPerson", FormName: "SalesPerson");
        DataRegistry.AddLookupWithTableName("SupplierCategory", "SupplierCategory", FormName: "SupplierCategory");
        DataRegistry.AddLookupWithTableName("SYS_NUMBER_SERIES", "SYS_NUMBER_SERIES", FormName: "NumberSeries");
        DataRegistry.AddLookupWithTableName("TaxCategory", "TaxCategory", FormName: "TaxCategory");
        DataRegistry.AddLookupWithTableName("TaxOffice", "TaxOffice", FormName: "TaxOffice");
        DataRegistry.AddLookupWithTableName("UnitOfMeasure", "UnitOfMeasure", FormName: "UnitOfMeasure");
        DataRegistry.AddLookupWithTableName("VatRate", "VatRate", FormName: "VatRate");
        DataRegistry.AddLookupWithTableName("Warehouse", "Warehouse", FormName: "Warehouse");
    }
    static void RegisterLocators_FromModules()
    {
        DataRegistry.AddLocator("Country", "Country", "Id", FormName: "Country");
        DataRegistry.AddLocator("Customer", "Person", "Id", FormName: "Person");
        DataRegistry.AddLocator("Person", "Person", "Id", FormName: "Person");
        DataRegistry.AddLocator("Product", "Product", "Id", FormName: "Product");
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
        Module = DataRegistry.AddModule("Bank", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Bank";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("Carrier", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Carrier";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
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
        Module = DataRegistry.AddModule("Category", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Category";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("ParentId", "Category", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("LevelNo", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("SortNo", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("RevenueAccount", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ExpenseAccount", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Company", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Company";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Company");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Title", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("TaxNumber", MaxLength: 32, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("TaxOfficeId", "TaxOffice", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Website", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
        tblCompanyBranch.AddString("CompanyId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCompanyBranch.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddString("City", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddString("CountryId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBranch.AddString("Phone", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddString("Email", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBranch.AddBoolean("IsPrimary", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblCompanyBranch.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        TableDef tblCountry = tblCompanyBranch.AddJoin("CountryId", "Country", "Country", "Id");
        tblCompanyBranch.Fields.Get("CountryId").Locator = "Country";
        tblCountry.AddId("Id").SetNullable(false);
        tblCountry.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCountry.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        TableDef tblCompanyBankAccount = tblTop.AddDetail("CompanyBankAccount", "Id", "CompanyId");
        tblCompanyBankAccount.KeyField = "Id";
        tblCompanyBankAccount.AddId("Id").SetNullable(false);
        tblCompanyBankAccount.AddString("CompanyId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCompanyBankAccount.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("BankName", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("Iban", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddString("SwiftBic", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblCompanyBankAccount.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCompanyBankAccount.AddBoolean("IsDefault", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblCompanyBankAccount.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
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
        Module = DataRegistry.AddModule("CostCenter", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "CostCenter";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("ParentCostCenterId", "CostCenter", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ManagerPersonId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDate("StartDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDate("EndDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblManagerPerson = tblTop.AddJoin("ManagerPersonId", "Person", "ManagerPerson", "Id");
        tblTop.Fields.Get("ManagerPersonId").Locator = "Person";
        tblManagerPerson.AddId("Id").SetNullable(false);
        tblManagerPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblManagerPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblManagerPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblManagerPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Country", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Country";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Iso2", MaxLength: 2, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Iso3", MaxLength: 3, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("Currency", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Currency";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Symbol", MaxLength: 8, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Decimals", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("2");
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
        Module = DataRegistry.AddModule("CustomerCategory", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "CustomerCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("DiscountCategory", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "DiscountCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
   DocumentType.IsActive,
   DocumentType.AffectsStock,
   DocumentType.AffectsFinancial,
   DocumentType.AffectsAccounting,
   DocumentType.StockDirection,
   DocumentType.FinancialDirection,
   DocumentType.AccountingDirection,
   DocumentType.IsCancellation,
   DocumentType.TargetDocumentTypeId,
   DocumentType.RequiresApproval,
   DocumentType.AutoComplete,
   DocumentType.Color,
   DocumentType.IconName,
   DocumentType.PrintTemplate,
   DocumentType.ReportName,
   COALESCE(NumberSeries.Code, '') as NumberSeries__Code,
   COALESCE(NumberSeries.Name, '') as NumberSeries__Name
from
  DocumentType
    left join SYS_NUMBER_SERIES NumberSeries on NumberSeries.Id = DocumentType.NumberSeriesId
";
        Module = DataRegistry.AddModule("DocumentType", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "DocumentType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("TradeTypeId", "TradeType", TypeStore.Get("TradeType"), Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("NumberSeriesId", "SYS_NUMBER_SERIES", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("AffectsStock", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsFinancial", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsAccounting", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("StockDirection", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("FinancialDirection", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("AccountingDirection", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsCancellation", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("TargetDocumentTypeId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("RequiresApproval", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AutoComplete", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("PrintTemplate", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ReportName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        string[] FilterFields = ["Name", "AccountingDirection", "AffectsAccounting", "AffectsFinancial", "AffectsStock", "AutoComplete", "Code", "Color", "FinancialDirection", "IconName", "IsActive", "IsCancellation", "NumberSeries__Code", "NumberSeries__Name", "PrintTemplate", "ReportName", "RequiresApproval", "StockDirection", "TradeType"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["TradeTypeId"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["TradeType"] = DataColumnType.Text;
        SelectDef.ColumnTypes["NumberSeriesId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsStock"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsFinancial"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AffectsAccounting"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["StockDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["FinancialDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["AccountingDirection"] = DataColumnType.Integer;
        SelectDef.ColumnTypes["IsCancellation"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["TargetDocumentTypeId"] = DataColumnType.Text;
        SelectDef.ColumnTypes["RequiresApproval"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["AutoComplete"] = DataColumnType.Boolean;
        SelectDef.ColumnTypes["Color"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IconName"] = DataColumnType.Text;
        SelectDef.ColumnTypes["PrintTemplate"] = DataColumnType.Text;
        SelectDef.ColumnTypes["ReportName"] = DataColumnType.Text;
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
        Module = DataRegistry.AddModule("ExpenseCategory", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "ExpenseCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name", "Code"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
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
        Module = DataRegistry.AddModule("FiscalYear", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "FiscalYear";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("StartDate", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("EndDate", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsClosed", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        tblFiscalPeriod.AddString("YearId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblFiscalPeriod.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddInteger("PeriodNo", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddDate("StartDate", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddDate("EndDate", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblFiscalPeriod.AddBoolean("IsClosed", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblFiscalPeriod.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Language", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Language";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 16, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("CultureName", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsDefault", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsRightToLeft", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Log", ClassName: "LogDataModule", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "SYS_LOG";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddInteger("Year", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Month", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("DayOfMonth", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("LogTime", MaxLength: 20, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("User", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Host", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Level", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Source", MaxLength: 512, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Scope", MaxLength: 512, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("EventId", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddTextBlob("Message", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("NumberSeries", ClassName: "CodeProviderModule", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "SYS_NUMBER_SERIES";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Pattern", MaxLength: 64, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("ResetPeriodId", "ResetPeriod", TypeStore.Get("ResetPeriod"), Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddInteger("NextNumber", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("LastResetValue", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
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
        Module = DataRegistry.AddModule("PaymentMethod", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "PaymentMethod";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
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
        Module = DataRegistry.AddModule("PaymentTerm", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "PaymentTerm";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("Days", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Person", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Person";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Title", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("TaxNumber", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("TaxOfficeId", "TaxOffice", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("LanguageId", "Language", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Mobile", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Website", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ContactPerson", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Notes", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsCompany", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
        tblPersonRole.AddString("PersonId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblPersonRole.AddStringLookupId("RoleTypeId", "PersonRoleType", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblPersonRole.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblPersonRole.AddDate("StartDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblPersonRole.AddDate("EndDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblPersonRole.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("PersonRoleType", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "PersonRoleType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("PriceList", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "PriceList";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddStringLookupId("PriceListTypeId", "PriceListType", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("DiscountCategoryId", "DiscountCategory", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("CustomerId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("UnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("MinQuantity", Decimals: 4, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDecimal("UnitPrice", Decimals: 4, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddDate("ValidFrom", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDate("ValidTo", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblCustomer = tblTop.AddJoin("CustomerId", "Person", "Customer", "Id");
        tblTop.Fields.Get("CustomerId").Locator = "Customer";
        tblCustomer.AddId("Id").SetNullable(false);
        tblCustomer.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCustomer.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCustomer.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblCustomer.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblProduct = tblTop.AddJoin("ProductId", "Product", "Product", "Id");
        tblTop.Fields.Get("ProductId").Locator = "Product";
        tblProduct.AddId("Id").SetNullable(false);
        tblProduct.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblProduct.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProduct.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.Visible).SetNullable(true);
        tblProduct.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("PriceListType", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "PriceListType";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsTaxIncluded", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsDefault", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Product", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Product";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Product");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddEnumLookupId("ProductTypeId", "ProductType", TypeStore.Get("ProductType"), Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CategoryId", "Category", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("PrimaryUnitOfMeasureId", "UnitOfMeasure", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDecimal("Weight", Decimals: 4, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDecimal("Volume", Decimals: 4, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        tblProductGroups.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductGroups.AddStringLookupId("GroupId", "ProductGroup", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductGroups.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblProductCategory = tblTop.AddDetail("ProductCategory", "Id", "ProductId");
        tblProductCategory.KeyField = "Id";
        tblProductCategory.AddId("Id").SetNullable(false);
        tblProductCategory.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductCategory.AddStringLookupId("CategoryId", "Category", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductCategory.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        TableDef tblProductUnitOfMeasure = tblTop.AddDetail("ProductUnitOfMeasure", "Id", "ProductId");
        tblProductUnitOfMeasure.KeyField = "Id";
        tblProductUnitOfMeasure.AddId("Id").SetNullable(false);
        tblProductUnitOfMeasure.AddString("ProductId", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddStringLookupId("UnitId", "UnitOfMeasure", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddDecimal("Ratio", Decimals: 4, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblProductUnitOfMeasure.AddString("Barcode", MaxLength: 64, Flags: FieldFlags.Visible).SetNullable(true);
        tblProductUnitOfMeasure.AddBoolean("IsSalesDefault", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductUnitOfMeasure.AddBoolean("IsPurchaseDefault", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblProductUnitOfMeasure.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblProductUnitOfMeasure.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("ProductBrand", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "ProductBrand";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        string[] FilterFields = ["Name"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
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
        Module = DataRegistry.AddModule("ProductGroup", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "ProductGroup";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("Project", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Project";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Project");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddString("CustomerId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddEnumLookupId("ProjectStatusId", "ProjectStatus", TypeStore.Get("ProjectStatus"), Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddDate("StartDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddDate("EndDate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("CostCenterId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ManagerPersonId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblCustomer = tblTop.AddJoin("CustomerId", "Person", "Customer", "Id");
        tblTop.Fields.Get("CustomerId").Locator = "Customer";
        tblCustomer.AddId("Id").SetNullable(false);
        tblCustomer.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblCustomer.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblCustomer.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblCustomer.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblManagerPerson = tblTop.AddJoin("ManagerPersonId", "Person", "ManagerPerson", "Id");
        tblTop.Fields.Get("ManagerPersonId").Locator = "Person";
        tblManagerPerson.AddId("Id").SetNullable(false);
        tblManagerPerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblManagerPerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblManagerPerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblManagerPerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("SalesPerson", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "SalesPerson";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("SalesPerson");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        string[] FilterFields = ["Name", "Code", "IsActive"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(FieldName, FieldName: FieldName);
        SelectDef.ColumnTypes["Id"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
        SelectDef.ColumnTypes["Name"] = DataColumnType.Text;
        SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
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
        Module = DataRegistry.AddModule("StockReason", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "StockReason";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddInteger("StockDirection", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsCost", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("RequiresRemarks", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsSystem", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("SupplierCategory", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "SupplierCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("TaxCategory", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "TaxCategory";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("VatRateId", "VatRate", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsDomestic", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsEuropeanUnion", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsThirdCountry", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsTaxExempt", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsReverseCharge", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsIntrastat", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsVies", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
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
        Module = DataRegistry.AddModule("TaxOffice", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "TaxOffice";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("UnitOfMeasure", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "UnitOfMeasure";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
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
        Module = DataRegistry.AddModule("VatRate", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "VatRate";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddDecimal("Percent", Decimals: 2, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
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
        Module = DataRegistry.AddModule("Warehouse", ListSelectSql: SqlText);
        tblTop = Module.Table;
        tblTop.Name = "Warehouse";
        tblTop.KeyField = "Id";
        tblTop.AddId("Id").SetNullable(false);
        tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetNullable(false).SetCodeProviderName("Warehouse");
        tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("CompanyId", "Company", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblTop.AddStringLookupId("BranchId", "CompanyBranch", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddEnumLookupId("WarehouseTypeId", "WarehouseType", TypeStore.Get("WarehouseType"), Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddString("AddressLine1", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("AddressLine2", MaxLength: 160, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("City", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddStringLookupId("CountryId", "Country", Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Phone", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("Email", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("ResponsiblePersonId", MaxLength: 40, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddBoolean("IsActive", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddBoolean("IsVirtual", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AllowNegativeStock", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("0");
        tblTop.AddBoolean("AffectsAvailability", Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false).SetDefaultValue("1");
        tblTop.AddString("Color", MaxLength: 32, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
        tblTop.AddTextBlob("Remarks", Flags: FieldFlags.Visible).SetNullable(true);
        TableDef tblResponsiblePerson = tblTop.AddJoin("ResponsiblePersonId", "Person", "ResponsiblePerson", "Id");
        tblTop.Fields.Get("ResponsiblePersonId").Locator = "Person";
        tblResponsiblePerson.AddId("Id").SetNullable(false);
        tblResponsiblePerson.AddString("Code", MaxLength: 40, Flags: FieldFlags.Visible | FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
        tblResponsiblePerson.AddString("Name", MaxLength: 96, Flags: FieldFlags.Visible | FieldFlags.Required).SetNullable(false);
        tblResponsiblePerson.AddString("PostalCode", MaxLength: 16, Flags: FieldFlags.Visible).SetNullable(true);
        tblResponsiblePerson.AddString("IconName", MaxLength: 96, Flags: FieldFlags.Visible).SetNullable(true);
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
    }

    // ● static public
    static public void RegisterModules()
    {
        RegisterCodeProviders_FromModules();
        RegisterLookupSources_FromModules();
        RegisterLocators_FromModules();
        RegisterModule_Bank();
        RegisterModule_Carrier();
        RegisterModule_Category();
        RegisterModule_Company();
        RegisterModule_CostCenter();
        RegisterModule_Country();
        RegisterModule_Currency();
        RegisterModule_CustomerCategory();
        RegisterModule_DiscountCategory();
        RegisterModule_DocumentType();
        RegisterModule_ExpenseCategory();
        RegisterModule_FiscalYear();
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
        RegisterModule_ProductBrand();
        RegisterModule_ProductGroup();
        RegisterModule_Project();
        RegisterModule_SalesPerson();
        RegisterModule_StockReason();
        RegisterModule_SupplierCategory();
        RegisterModule_TaxCategory();
        RegisterModule_TaxOffice();
        RegisterModule_UnitOfMeasure();
        RegisterModule_VatRate();
        RegisterModule_Warehouse();
    }
}
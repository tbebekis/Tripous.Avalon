namespace tERP.Data;

public partial class RegistryVersion1: RegistryVersion
{
    // ● private
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
        Module = DataRegistry.AddOrGetModule("AppUser", ClassName: "AppUserDataModule", ListSelectSql: SqlText);
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

    // ● public
    public override void RegisterModules()
    {
        RegisterModule_AppUser();
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
        RegisterModule_ExpenseCategory();
        RegisterModule_FiscalYear();
        RegisterModule_FixedAsset();
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
        RegisterModule_ResourceStrings();
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
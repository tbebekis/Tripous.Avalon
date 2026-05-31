/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

public partial class SchemaVersion1: SchemaVersionDef
{
    // ● private
    void RegisterTable_SYS_LOG()
    {
        string TableName = "SYS_LOG";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key
    ,Year int @NOT_NULL
    ,Month int @NOT_NULL
    ,DayOfMonth int @NOT_NULL
    ,LogTime @NVARCHAR(20) @NOT_NULL
    ,User @NVARCHAR(96) @NOT_NULL
    ,Host @NVARCHAR(96) @NOT_NULL
    ,Level @NVARCHAR(96) @NOT_NULL
    ,Source @NVARCHAR(512) @NOT_NULL
    ,Scope @NVARCHAR(512) @NOT_NULL
    ,EventId @NVARCHAR(96) @NOT_NULL
    ,Message @NBLOB_TEXT @NOT_NULL          -- LargeMemo 
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_SYS_NUMBER_SERIES()
    {
        string TableName = "SYS_NUMBER_SERIES";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id              @NVARCHAR(40) @NOT_NULL primary key,
    Code            @NVARCHAR(40) @NOT_NULL,
    Name            @NVARCHAR(96) @NOT_NULL,
    Pattern         @NVARCHAR(64) @NOT_NULL,
    ResetPeriodId   integer default 0 @NOT_NULL, -- Enum
    NextNumber      integer default 1 @NOT_NULL,
    LastResetValue  @NVARCHAR(16) @NULL,
    IsActive        @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_NumberSeries_Code UNIQUE (Code),
    CONSTRAINT UQ_NumberSeries_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AppUser()
    {
        string TableName = "AppUser";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    UserName @NVARCHAR(64) @NOT_NULL,
    Password @NVARCHAR(512) @NOT_NULL,
    Salt @NVARCHAR(256) @NOT_NULL,

    FullName @NVARCHAR(96) @NOT_NULL,

    UserLevelId int @NOT_NULL,                     -- Enum UserLevel

    Email @NVARCHAR(96) @NULL,
    Phone @NVARCHAR(40) @NULL,

    LastLoginAt @DATE_TIME @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_UserName UNIQUE (UserName)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_CustomerCategory()
    {
        string TableName = "CustomerCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_SupplierCategory()
    {
        string TableName = "SupplierCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductBrand()
    {
        string TableName = "ProductBrand";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_DiscountCategory()
    {
        string TableName = "DiscountCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_UnitOfMeasure()
    {
        string TableName = "UnitOfMeasure";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_TaxOffice()
    {
        string TableName = "TaxOffice";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Bank()
    {
        string TableName = "Bank";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ExpenseCategory()
    {
        string TableName = "ExpenseCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PaymentMethod()
    {
        string TableName = "PaymentMethod";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_SalesPerson()
    {
        string TableName = "SalesPerson";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,                -- Code XXXX
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Carrier()
    {
        string TableName = "Carrier";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Country()
    {
        string TableName = "Country";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Iso2 @NVARCHAR(2) @NOT_NULL,
    Iso3 @NVARCHAR(3) @NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Currency()
    {
        string TableName = "Currency";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Symbol @NVARCHAR(8) @NOT_NULL,
    Decimals int default 2 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_VatRate()
    {
        string TableName = "VatRate";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Percent @DECIMAL_(5,2) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PaymentTerm()
    {
        string TableName = "PaymentTerm";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    Days integer @NOT_NULL,          -- payment due days

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductGroup()
    {
        string TableName = "ProductGroup";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL, -- business code
    Name @NVARCHAR(96) @NOT_NULL, -- display title

    IsSystem @BOOL default 0 @NOT_NULL, -- protected/system group
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_FiscalYear()
    {
        string TableName = "FiscalYear";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    StartDate @DATE @NOT_NULL,       -- fiscal year start
    EndDate @DATE @NOT_NULL,         -- fiscal year end

    IsActive @BOOL default 1 @NOT_NULL,
    IsClosed @BOOL default 0 @NOT_NULL, -- no more postings allowed

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Language()
    {
        string TableName = "Language";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(16) @NOT_NULL,                   -- ISO code, e.g. EN, EL, DE
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    CultureName @NVARCHAR(32) @NULL,                -- en-US, el-GR, de-DE

    IsDefault @BOOL default 0 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,

    IsRightToLeft @BOOL default 0 @NOT_NULL,        -- Arabic, Hebrew, etc.

    Color @NVARCHAR(32) @NULL,                      -- ui display color
    IconName @NVARCHAR(96) @NULL,                   -- ui icon / flag icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PersonRoleType()
    {
        string TableName = "PersonRoleType";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,
    IconName @NVARCHAR(96) @NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockReason()
    {
        string TableName = "StockReason";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                   -- business code
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    StockDirection integer default 0 @NOT_NULL,     -- 1=in, -1=out, 0=no stock effect

    AffectsCost @BOOL default 0 @NOT_NULL,          -- affects inventory valuation
    RequiresRemarks @BOOL default 0 @NOT_NULL,      -- user must enter explanation

    IsSystem @BOOL default 0 @NOT_NULL,             -- protected/system-defined reason
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                      -- ui display color
    IconName @NVARCHAR(96) @NULL,                   -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ContactType()
    {
        string TableName = "ContactType";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetCategory()
    {
        string TableName = "AssetCategory";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetLocation()
    {
        string TableName = "AssetLocation";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL 
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetDepreciationMethod()
    {
        string TableName = "AssetDepreciationMethod";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL 
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductDimension()
    {
        string TableName = "ProductDimension";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Name @NVARCHAR(96) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductAttributeGroup()
    {
        string TableName = "ProductAttributeGroup";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Name @NVARCHAR(96) @NOT_NULL,

    DisplayOrder int default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Account()
    {
        string TableName = "Account";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    ParentAccountId @NVARCHAR(40) @NULL,              -- Lookup Account

    AccountTypeId int @NOT_NULL,                      -- Enum AccountType
    NormalBalanceId int @NOT_NULL,                    -- Enum NormalBalance

    IsPosting @BOOL DEFAULT 1 @NOT_NULL,
    IsActive @BOOL DEFAULT 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentAccountId) REFERENCES Account(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PriceListType()
    {
        string TableName = "PriceListType";
        string SqlText = $@"
CREATE TABLE {TableName} (
     Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    CurrencyId @NVARCHAR(40) @NOT_NULL,             -- Lookup

    IsTaxIncluded @BOOL default 1 @NOT_NULL,
    IsDefault @BOOL default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,
    IconName @NVARCHAR(96) @NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),

    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Company()
    {
        string TableName = "Company";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,           -- Code XXXXXX
    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,
    TaxNumber @NVARCHAR(32) @NOT_NULL,
    TaxOfficeId @NVARCHAR(40) @NULL,        -- Lookup
    CountryId @NVARCHAR(40) @NULL,      -- Lookup
    CurrencyId @NVARCHAR(40) @NULL,     -- Lookup
    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,
    Phone @NVARCHAR(32) @NULL,
    Email @NVARCHAR(96) @NULL,
    Website @NVARCHAR(96) @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (TaxOfficeId) REFERENCES TaxOffice(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_TaxCategory()
    {
        string TableName = "TaxCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,               -- business code
    Name @NVARCHAR(96) @NOT_NULL,               -- display title

    VatRateId @NVARCHAR(40) @NULL,              -- Lookup   -- default vat rate

    IsDomestic @BOOL default 0 @NOT_NULL,
    IsEuropeanUnion @BOOL default 0 @NOT_NULL,
    IsThirdCountry @BOOL default 0 @NOT_NULL,

    IsTaxExempt @BOOL default 0 @NOT_NULL,
    IsReverseCharge @BOOL default 0 @NOT_NULL,
    IsIntrastat @BOOL default 0 @NOT_NULL,
    IsVies @BOOL default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,       -- ui display color
    IconName @NVARCHAR(96) @NULL,    -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_FiscalPeriod()
    {
        string TableName = "FiscalPeriod";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    YearId  @NVARCHAR(40) @NOT_NULL,            -- Master

    Code @NVARCHAR(40) @NOT_NULL,               -- business code
    Name @NVARCHAR(96) @NOT_NULL,               -- display title

    PeriodNo integer @NOT_NULL,                 -- 1..12 or custom sequence

    StartDate @DATE @NOT_NULL,
    EndDate @DATE @NOT_NULL,

    IsClosed @BOOL default 0 @NOT_NULL,         -- no postings allowed

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_FiscalYear_PeriodNo UNIQUE (YearId, PeriodNo),

    FOREIGN KEY (YearId) REFERENCES FiscalYear(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Person()
    {
        string TableName = "Person";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,

    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,

    TaxNumber @NVARCHAR(32) @NULL,                  -- Group Tax
    TaxOfficeId @NVARCHAR(40) @NULL,                -- Lookup; Group Tax

    CountryId @NVARCHAR(40) @NULL,                  -- Lookup; Group Preferences
    CurrencyId @NVARCHAR(40) @NULL,                 -- Lookup; Group Preferences
    LanguageId @NVARCHAR(40) @NULL,                 -- Lookup; Group Preferences -- preferred language

    AddressLine1 @NVARCHAR(160) @NULL,              -- Group Address
    AddressLine2 @NVARCHAR(160) @NULL,              -- Group Address
    City @NVARCHAR(96) @NULL,                       -- Group Address
    PostalCode @NVARCHAR(16) @NULL,                 -- Group Address

    Phone @NVARCHAR(32) @NULL,                      -- Group Address
    Mobile @NVARCHAR(32) @NULL,                     -- Group Address
    Email @NVARCHAR(96) @NULL,                      -- Group Address
    Website @NVARCHAR(96) @NULL,                    -- Group Address

    ContactPerson @NVARCHAR(96) @NULL,              -- Group Address

    Notes @NBLOB_TEXT @NULL,                        -- LargeMemo; Group Notes

    IsCompany @BOOL default 1 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                      -- Group Appearance
    IconName @NVARCHAR(96) @NULL,                   -- Group Appearance

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (TaxOfficeId) REFERENCES TaxOffice(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    FOREIGN KEY (LanguageId) REFERENCES Language(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Category()
    {
        string TableName = "Category";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ParentId @NVARCHAR(40) @NULL,                   -- Lookup       -- parent category

    Code @NVARCHAR(40) @NOT_NULL,                   -- business code
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    LevelNo integer default 0 @NOT_NULL,            -- optional hierarchy level

    SortNo integer default 0 @NOT_NULL,             -- display order

    VatRateId @NVARCHAR(40) @NULL,                  -- Lookup       -- default vat rate
    RevenueAccount @NVARCHAR(40) @NULL,             -- optional accounting account
    ExpenseAccount @NVARCHAR(40) @NULL,             -- optional accounting account

    IsSystem @BOOL default 0 @NOT_NULL,             -- protected/system category
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                      -- ui display color
    IconName @NVARCHAR(96) @NULL,                   -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentId) REFERENCES Category(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_FixedAsset()
    {
        string TableName = "FixedAsset";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,

    Code                @NVARCHAR(40) @NOT_NULL, -- Code AST-XXXXXX
    Name                @NVARCHAR(96) @NOT_NULL,

    AssetCategoryId     @NVARCHAR(40) @NOT_NULL, -- Lookup
    AssetLocationId     @NVARCHAR(40) @NOT_NULL, -- Lookup
    AssetDepreciationMethodId   @NVARCHAR(40) @NULL,         -- Lookup

    PurchaseDate           @DATE @NULL,
    PurchaseValue          @DECIMAL_(18, 4) @NULL,

    UsefulLifeMonths       int @NULL,
    DepreciationRate       @DECIMAL_(18, 4) @NULL,    

    SerialNumber        @NVARCHAR(96) @NULL,
    Manufacturer        @NVARCHAR(96) @NULL,
    Model               @NVARCHAR(96) @NULL,

    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (AssetCategoryId) REFERENCES AssetCategory(Id),
    FOREIGN KEY (AssetLocationId) REFERENCES AssetLocation(Id),
    FOREIGN KEY (AssetDepreciationMethodId) REFERENCES AssetDepreciationMethod(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductDimensionValue()
    {
        string TableName = "ProductDimensionValue";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductDimensionId @NVARCHAR(40) @NOT_NULL,   -- Master

    Name @NVARCHAR(96) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    FOREIGN KEY (ProductDimensionId) REFERENCES ProductDimension(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_DocumentType()
    {
        string TableName = "DocumentType";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    TradeTypeId int @NOT_NULL,                         -- Enum TradeType

    NumberSeriesId @NVARCHAR(40) @NULL,                -- Lookup
    HandlerClass @NVARCHAR(256) @NULL,                 -- IDocumentHandler full class name

    IsActive @BOOL default 1 @NOT_NULL,
    IsSystem @BOOL default 0 @NOT_NULL,                -- system defined and protected type
    AllowManualNumber @BOOL default 0 @NOT_NULL,
    AutoComplete @BOOL default 0 @NOT_NULL,

    AffectsStock @BOOL default 0 @NOT_NULL,            -- Group Posting
    AffectsFinancial @BOOL default 0 @NOT_NULL,        -- Group Posting
    AffectsAccounting @BOOL default 0 @NOT_NULL,       -- Group Posting

    StockDirection int default 0 @NOT_NULL,            -- Group Posting
    FinancialDirection int default 0 @NOT_NULL,        -- Group Posting
    AccountingDirection int default 0 @NOT_NULL,       -- Group Posting

    IsCancellation @BOOL default 0 @NOT_NULL,          -- Group Cancellation
    CancellationTargetId @NVARCHAR(40) @NULL,          -- Lookup; Group Cancellation -- what document type may cancel

    PrintTemplate @NVARCHAR(96) @NULL,                 -- Group Output
    ReportName @NVARCHAR(96) @NULL,                    -- Group Output

    DisplayOrder int default 0 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                         -- Group Appearance -- ui display color
    IconName @NVARCHAR(96) @NULL,                      -- Group Appearance -- ui icon

    Remarks @NBLOB_TEXT @NULL,                         -- LargeMemo; Group Notes

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (NumberSeriesId) REFERENCES SYS_NUMBER_SERIES(Id),
    FOREIGN KEY (CancellationTargetId) REFERENCES DocumentType(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_CompanyBranch()
    {
        string TableName = "CompanyBranch";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    CompanyId @NVARCHAR(40) @NOT_NULL,          -- Master
    Code @NVARCHAR(40) @NOT_NULL,                
    Name @NVARCHAR(96) @NOT_NULL,
    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,
    CountryId @NVARCHAR(40) @NOT_NULL,          -- Locator
    Phone @NVARCHAR(32) @NULL,
    Email @NVARCHAR(96) @NULL,
    IsPrimary int default 0 @NOT_NULL,
    IsActive int default 1 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    CONSTRAINT UQ_{TableName}_CompanyId_Code UNIQUE (CompanyId, Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_CompanyBankAccount()
    {
        string TableName = "CompanyBankAccount";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    CompanyId @NVARCHAR(40) @NOT_NULL,              -- Master
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    BankName @NVARCHAR(96) @NOT_NULL,
    Iban @NVARCHAR(40) @NOT_NULL,
    SwiftBic @NVARCHAR(16) @NULL,
    CurrencyId @NVARCHAR(40) @NOT_NULL,             -- Lookup
    IsDefault int default 0 @NOT_NULL,
    IsActive int default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    CONSTRAINT UQ_{TableName}_CompanyId_Code UNIQUE (CompanyId, Code)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PersonRole()
    {
        string TableName = "PersonRole";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    PersonId @NVARCHAR(40) @NOT_NULL,           -- Master
    RoleTypeId @NVARCHAR(40) @NOT_NULL,         -- Lookup

    IsActive @BOOL default 1 @NOT_NULL,

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    Remarks @NBLOB_TEXT @NULL,

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (RoleTypeId) REFERENCES PersonRoleType(Id),

    CONSTRAINT UQ_{TableName}_Person_Role UNIQUE (PersonId, RoleTypeId)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_CostCenter()
    {
        string TableName = "CostCenter";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                   -- business code
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    ParentCostCenterId @NVARCHAR(40) @NULL,         -- Lookup   -- optional hierarchy parent
    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator Person  -- responsible person

    StartDate @DATE @NULL,                          -- activation date
    EndDate @DATE @NULL,                            -- deactivation date

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                      -- ui display color
    IconName @NVARCHAR(96) @NULL,                   -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentCostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (ManagerPersonId) REFERENCES Person(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Product()
    {
        string TableName = "Product";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                           -- Code XXXXXX -- business code
    Name @NVARCHAR(96) @NOT_NULL,                           -- display title

    ProductTypeId integer @NOT_NULL,                        -- Enum         -- Goods, Service, RawMaterial

    CategoryId @NVARCHAR(40) @NULL,                         -- Lookup
    VatRateId @NVARCHAR(40) @NULL,                          -- Lookup

    PrimaryUnitOfMeasureId @NVARCHAR(40) @NOT_NULL,         -- Lookup       -- inventory/base unit

    Barcode @NVARCHAR(64) @NULL,

    Weight @DECIMAL @NULL,
    Volume @DECIMAL @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,
    IconName @NVARCHAR(96) @NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CategoryId) REFERENCES Category(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id),
    FOREIGN KEY (PrimaryUnitOfMeasureId) REFERENCES UnitOfMeasure(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PersonAddress()
    {
        string TableName = "PersonAddress";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    PersonId            @NVARCHAR(40) @NOT_NULL, -- Master
    AddressTypeId       int @NOT_NULL,      -- Enum AddressType
    Code                @NVARCHAR(40) @NULL,     -- Code ADR-XXXXXX

    Name                @NVARCHAR(96) @NULL,
    CountryId           @NVARCHAR(40) @NULL,     -- Lookup
    Region              @NVARCHAR(96) @NULL,
    City                @NVARCHAR(96) @NULL,
    PostalCode          @NVARCHAR(40) @NULL,

    AddressLine1        @NVARCHAR(96) @NULL,
    AddressLine2        @NVARCHAR(96) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PersonContact()
    {
        string TableName = "PersonContact";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    PersonId            @NVARCHAR(40) @NOT_NULL, -- Master
    ContactTypeId       @NVARCHAR(40) @NOT_NULL, -- Lookup

    Name                @NVARCHAR(96) @NOT_NULL,
    JobTitle            @NVARCHAR(96) @NULL,

    Phone               @NVARCHAR(40) @NULL,
    Mobile              @NVARCHAR(40) @NULL,
    Email               @NVARCHAR(96) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (ContactTypeId) REFERENCES ContactType(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PersonBankAccount()
    {
        string TableName = "PersonBankAccount";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    PersonId            @NVARCHAR(40) @NOT_NULL, -- Master

    BankId              @NVARCHAR(40) @NOT_NULL, -- Lookup
    Name                @NVARCHAR(96) @NOT_NULL,

    Iban                @NVARCHAR(40) @NULL,
    SwiftCode           @NVARCHAR(40) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (BankId) REFERENCES Bank(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetAssignment()
    {
        string TableName = "AssetAssignment";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    FixedAssetId        @NVARCHAR(40) @NOT_NULL, -- Master

    PersonId            @NVARCHAR(40) @NULL,     -- Locator Person

    AssignmentDate      @DATE @NULL,
    ReturnDate          @DATE @NULL,

    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (FixedAssetId) REFERENCES FixedAsset(Id),
    FOREIGN KEY (PersonId) REFERENCES Person(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetMaintenance()
    {
        string TableName = "AssetMaintenance";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    FixedAssetId        @NVARCHAR(40) @NOT_NULL, -- Master

    Date                @DATE @NOT_NULL,
    Description         @NVARCHAR(255) @NOT_NULL,

    Cost                @DECIMAL_(18, 4) @NULL,

    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (FixedAssetId) REFERENCES FixedAsset(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetDocument()
    {
        string TableName = "AssetDocument";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    FixedAssetId        @NVARCHAR(40) @NOT_NULL, -- Master

    Name                @NVARCHAR(96) @NOT_NULL,
    FileName            @NVARCHAR(255) @NULL,
    Description         @NVARCHAR(255) @NULL,

    BlobText            @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (FixedAssetId) REFERENCES FixedAsset(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetInsurance()
    {
        string TableName = "AssetInsurance";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    FixedAssetId @NVARCHAR(40) @NOT_NULL,         -- Master

    PolicyNumber @NVARCHAR(96) @NULL,

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    Amount @DECIMAL_(18, 4) @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Notes @NBLOB_TEXT @NULL,

    FOREIGN KEY (FixedAssetId) REFERENCES FixedAsset(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_JournalEntry()
    {
        string TableName = "JournalEntry";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code JE-YYYY-XXXXXX JOURNAL_ENTRY

    EntryDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    TotalDebit @DECIMAL DEFAULT 0 @NOT_NULL,
    TotalCredit @DECIMAL DEFAULT 0 @NOT_NULL,

    SourceModule @NVARCHAR(64) @NULL,                 -- Group Source
    SourceTable @NVARCHAR(64) @NULL,                  -- Group Source
    SourceId @NVARCHAR(40) @NULL,                     -- Group Source

    DocumentTypeId @NVARCHAR(40) @NULL,               -- Lookup; Group Document
    DocumentCode @NVARCHAR(40) @NULL,                 -- Group Document
    DocumentDate @DATE @NULL,                         -- Group Document

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo; Group Notes

    CancelledDocumentId @NVARCHAR(40) @NULL,          -- Locator JournalEntry; Group Relations
    CancellationDocumentId @NVARCHAR(40) @NULL,       -- Locator JournalEntry; Group Relations

    CreatedAt @DATE_TIME @NOT_NULL,                   -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser; Group Audit
    ModifiedAt @DATE_TIME @NULL,                      -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                   -- Lookup AppUser; Group Audit

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT CHK_{TableName}_Totals CHECK (TotalDebit = TotalCredit),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_PriceList()
    {
        string TableName = "PriceList";
        string SqlText = $@"
CREATE TABLE {TableName} (
   Id @NVARCHAR(40) @NOT_NULL primary key,

    PriceListTypeId @NVARCHAR(40) @NOT_NULL,        -- Lookup

    DiscountCategoryId @NVARCHAR(40) @NULL,        -- Lookup
    CustomerId @NVARCHAR(40) @NULL,             -- Locator Customer

    ProductId @NVARCHAR(40) @NOT_NULL,          -- Locator Product
    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,    -- Lookup

    MinQuantity @DECIMAL_(18, 4) default 0 @NOT_NULL,

    UnitPrice @DECIMAL_(18, 4) @NOT_NULL,

    ValidFrom @DATE @NULL,
    ValidTo @DATE @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    FOREIGN KEY (PriceListTypeId) REFERENCES PriceListType(Id),
    FOREIGN KEY (DiscountCategoryId) REFERENCES DiscountCategory(Id),
    FOREIGN KEY (CustomerId) REFERENCES Person(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductGroups()
    {
        string TableName = "ProductGroups";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,  -- Master
    GroupId @NVARCHAR(40) @NOT_NULL,    -- Lookup

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Product_Group UNIQUE (ProductId, GroupId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (GroupId) REFERENCES ProductGroup(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Warehouse()
    {
        string TableName = "Warehouse";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                       -- Code WH-XXXXXX -- business code
    Name @NVARCHAR(96) @NOT_NULL,                       -- display title

    CompanyId @NVARCHAR(40) @NOT_NULL,                  -- Lookup -- owner company
    BranchId @NVARCHAR(40) @NULL,                       -- Lookup -- optional company branch

    WarehouseTypeId integer default 0 @NOT_NULL,        -- Enum -- Main, Store, Transit, Production, Scrap, Virtual

    AddressLine1 @NVARCHAR(160) @NULL,                  -- Group Address
    AddressLine2 @NVARCHAR(160) @NULL,                  -- Group Address
    City @NVARCHAR(96) @NULL,                           -- Group Address
    PostalCode @NVARCHAR(16) @NULL,                     -- Group Address
    CountryId @NVARCHAR(40) @NULL,                      -- Lookup; Group Address

    Phone @NVARCHAR(32) @NULL,                          -- Group Address
    Email @NVARCHAR(96) @NULL,                          -- Group Address

    ResponsiblePersonId @NVARCHAR(40) @NULL,            -- Locator Person; Group Settings -- person responsible for warehouse

    IsActive @BOOL default 1 @NOT_NULL,                 -- Group Settings
    IsVirtual @BOOL default 0 @NOT_NULL,                -- Group Settings -- logical/non-physical warehouse
    AllowNegativeStock @BOOL default 0 @NOT_NULL,       -- Group Settings -- allow stock below zero
    AffectsAvailability @BOOL default 1 @NOT_NULL,      -- Group Settings -- participates in available stock

    Color @NVARCHAR(32) @NULL,                          -- Group Appearance -- ui display color
    IconName @NVARCHAR(96) @NULL,                       -- Group Appearance -- ui icon

    Remarks @NBLOB_TEXT @NULL,                          -- LargeMemo; Group Notes

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (BranchId) REFERENCES CompanyBranch(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    FOREIGN KEY (ResponsiblePersonId) REFERENCES Person(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Project()
    {
        string TableName = "Project";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                   -- Code YYYY-XXXX -- business code
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    CustomerId @NVARCHAR(40) @NULL,                 -- Locator Customer    -- customer/person owner

    ProjectStatusId integer default 0 @NOT_NULL,    -- Enum         -- Draft, Active, Completed, Cancelled

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    CostCenterId @NVARCHAR(40) @NULL,

    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator Person     -- responsible person

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                      -- ui display color
    IconName @NVARCHAR(96) @NULL,                   -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CustomerId) REFERENCES Person(Id),
    FOREIGN KEY (CostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (ManagerPersonId) REFERENCES Person(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductCategory()
    {
        string TableName = "ProductCategory";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,                      -- Master
    CategoryId @NVARCHAR(40) @NOT_NULL,                     -- Lookup

    IsActive @BOOL default 1 @NOT_NULL, 

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (CategoryId) REFERENCES Category(Id),

    CONSTRAINT UQ_{TableName}_Product_Category UNIQUE (ProductId, CategoryId)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductUnitOfMeasure()
    {
        string TableName = "ProductUnitOfMeasure";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Master
    UnitId @NVARCHAR(40) @NOT_NULL,                     -- Lookup

    Ratio @DECIMAL @NOT_NULL,                           -- ratio to primary unit

    Barcode @NVARCHAR(64) @NULL,

    IsSalesDefault @BOOL default 0 @NOT_NULL,
    IsPurchaseDefault @BOOL default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Product_Unit UNIQUE (ProductId, UnitId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitId) REFERENCES UnitOfMeasure(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductBarcode()
    {
        string TableName = "ProductBarcode";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    ProductId           @NVARCHAR(40) @NOT_NULL, -- Master

    Barcode             @NVARCHAR(512) @NOT_NULL,
    Name                @NVARCHAR(96) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (ProductId) REFERENCES Product(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductSupplier()
    {
        string TableName = "ProductSupplier";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    ProductId           @NVARCHAR(40) @NOT_NULL, -- Master

    SupplierId          @NVARCHAR(40) @NOT_NULL, -- Locator Person
    SupplierCode        @NVARCHAR(96) @NULL,

    LeadDays            int @NULL,
    LastCost            @DECIMAL_(18, 4) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (SupplierId) REFERENCES Person(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_BillOfMaterial()
    {
        string TableName = "BillOfMaterial";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    ProductId           @NVARCHAR(40) @NOT_NULL, -- Master

    Code                @NVARCHAR(40) @NOT_NULL, -- Code BOM-XXXXXX
    Name                @NVARCHAR(96) @NOT_NULL,

    Quantity            @DECIMAL_(18, 4) @NOT_NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (ProductId) REFERENCES Product(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_CashAccount()
    {
        string TableName = "CashAccount";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,

    Code                @NVARCHAR(40) @NOT_NULL, -- Code CASH-XXXXXX
    Name                @NVARCHAR(96) @NOT_NULL,

    CurrencyId          @NVARCHAR(40) @NOT_NULL, -- Lookup
    CompanyBranchId     @NVARCHAR(40) @NULL,     -- Lookup

    Balance             @DECIMAL_(18, 4) @NULL,

    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    FOREIGN KEY (CompanyBranchId) REFERENCES CompanyBranch(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductImage()
    {
        string TableName = "ProductImage";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,            -- Master

    Name @NVARCHAR(96) @NOT_NULL,

    ImageBlob @BLOB @NULL,

    IsDefault @BOOL default 0 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    DisplayOrder int default 0 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    FOREIGN KEY (ProductId) REFERENCES Product(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductAttribute()
    {
        string TableName = "ProductAttribute";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,              -- Master
    ProductAttributeGroupId @NVARCHAR(40) @NULL,    -- Lookup

    Name @NVARCHAR(96) @NOT_NULL,
    TypeId int @NOT_NULL,                           -- Enum ProductAttributeType -- Text, Integer, Decimal, Option
    TextValue @NVARCHAR(512) @NOT_NULL,

    UnitOfMeasure @NVARCHAR(30) @NULL,

    DisplayOrder int default 0 @NOT_NULL,
    IsSpec @BOOL default 1 @NOT_NULL,
    IsFilter @BOOL default 0 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Product_Name UNIQUE (ProductId, Name),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (ProductAttributeGroupId) REFERENCES ProductAttributeGroup(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_JournalEntryLine()
    {
        string TableName = "JournalEntryLine";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    JournalEntryId @NVARCHAR(40) @NOT_NULL,          -- Master

    LineNo int @NOT_NULL,

    AccountId @NVARCHAR(40) @NOT_NULL,               -- Lookup

    DebitAmount @DECIMAL DEFAULT 0 @NOT_NULL,
    CreditAmount @DECIMAL DEFAULT 0 @NOT_NULL,

    CurrencyId @NVARCHAR(40) @NULL,                  -- Lookup
    ExchangeRate @DECIMAL DEFAULT 1 @NOT_NULL,

    ReferenceNo @NVARCHAR(40) @NULL,
    Remarks @NVARCHAR(512) @NULL,

    SourceModule @NVARCHAR(64) @NULL,
    SourceTable @NVARCHAR(64) @NULL,
    SourceId @NVARCHAR(40) @NULL,

    CONSTRAINT UQ_{TableName}_LineNo UNIQUE (JournalEntryId, LineNo),

    CONSTRAINT CHK_{TableName}_DebitAmount CHECK (DebitAmount >= 0),
    CONSTRAINT CHK_{TableName}_CreditAmount CHECK (CreditAmount >= 0),

    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (AccountId) REFERENCES Account(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_ProductWarehouse()
    {
        string TableName = "ProductWarehouse";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    ProductId           @NVARCHAR(40) @NOT_NULL, -- Master

    WarehouseId         @NVARCHAR(40) @NOT_NULL, -- Lookup

    MinStock            @DECIMAL_(18, 4) @NULL,
    MaxStock            @DECIMAL_(18, 4) @NULL,
    ReorderPoint        @DECIMAL_(18, 4) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_WarehouseLocation()
    {
        string TableName = "WarehouseLocation";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    WarehouseId         @NVARCHAR(40) @NOT_NULL, -- Master

    Code                @NVARCHAR(40) @NOT_NULL, -- Code LOC-XXXXXX
    Name                @NVARCHAR(96) @NOT_NULL,

    Zone                @NVARCHAR(40) @NULL,
    Aisle               @NVARCHAR(40) @NULL,
    Rack                @NVARCHAR(40) @NULL,
    Shelf               @NVARCHAR(40) @NULL,
    Bin                 @NVARCHAR(40) @NULL,

    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo
 

    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_BillOfMaterialLine()
    {
        string TableName = "BillOfMaterialLine";
        string SqlText = $@"
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    BillOfMaterialId    @NVARCHAR(40) @NOT_NULL, -- Master

    ProductId           @NVARCHAR(40) @NOT_NULL, -- Locator Product

    Quantity            @DECIMAL_(18, 4) @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,       -- LargeMemo 

    FOREIGN KEY (BillOfMaterialId) REFERENCES BillOfMaterial(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
)
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Trade()
    {
        string TableName = "Trade";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup
    Code @NVARCHAR(40) @NOT_NULL,                       -- Code TR-DRAFT-YYYY-XXXXXX TRADE-DRAFT

    TradeStatusId int default 0 @NOT_NULL,              -- Enum TradeStatus
    TaxTreatmentId int default 1 @NOT_NULL,             -- Enum TaxTreatment

    TradeDate @DATE @NOT_NULL,                          -- Group Dates
    PostingDate @DATE @NULL,                            -- Group Dates
    DeliveryDate @DATE @NULL,                           -- Group Dates
    DueDate @DATE @NULL,                                -- Group Dates

    ExternalRef @NVARCHAR(96) @NULL,                    -- e.g. ""Related to Order 123"", ""Your ref: PO-456""

    PersonId @NVARCHAR(40) @NOT_NULL,                   -- Locator Person; Group Party -- Customer, Supplier, etc
    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup; Group Party

    SalesPersonId @NVARCHAR(40) @NULL,                  -- Lookup Person; Group Organization
    ProjectId @NVARCHAR(40) @NULL,                      -- Lookup; Group Organization
    CostCenterId @NVARCHAR(40) @NULL,                   -- Lookup; Group Organization
    BranchId @NVARCHAR(40) @NULL,                       -- Lookup; Group Organization

    CurrencyId @NVARCHAR(40) @NOT_NULL,                 -- Lookup; Group Payment
    ExchangeRate @DECIMAL default 1 @NOT_NULL,          -- Group Payment -- Exchange Rate for base currency

    PaymentMethodId @NVARCHAR(40) @NULL,                -- Lookup; Group Payment
    PaymentTermId @NVARCHAR(40) @NULL,                  -- Lookup; Group Payment

    BillingName @NVARCHAR(96) @NULL,                    -- Group Billing
    BillingAddressLine1 @NVARCHAR(128) @NULL,           -- Group Billing
    BillingAddressLine2 @NVARCHAR(128) @NULL,           -- Group Billing
    BillingCity @NVARCHAR(64) @NULL,                    -- Group Billing
    BillingPostalCode @NVARCHAR(20) @NULL,              -- Group Billing
    BillingCountryId @NVARCHAR(40) @NULL,               -- Lookup; Group Billing

    ShippingName @NVARCHAR(96) @NULL,                   -- Group Shipping
    ShippingAddressLine1 @NVARCHAR(128) @NULL,          -- Group Shipping
    ShippingAddressLine2 @NVARCHAR(128) @NULL,          -- Group Shipping
    ShippingCity @NVARCHAR(64) @NULL,                   -- Group Shipping
    ShippingPostalCode @NVARCHAR(20) @NULL,             -- Group Shipping
    ShippingCountryId @NVARCHAR(40) @NULL,              -- Lookup; Group Shipping

    SourceId @NVARCHAR(40) @NULL,                       -- Locator Trade; Group Relations
    CancelsTradeId @NVARCHAR(40) @NULL,                 -- Locator Trade; Group Relations
    CancelledByTradeId @NVARCHAR(40) @NULL,             -- Locator Trade; Group Relations

    LinesAmount @DECIMAL default 0 @NOT_NULL,           -- Group Amounts -- sum of lines before header discounts/charges/taxes
    DiscountPercent @DECIMAL default 0 @NOT_NULL,       -- Group Amounts -- Header Discount %
    DiscountAmount @DECIMAL default 0 @NOT_NULL,        -- Group Amounts
    DiscountReason @NVARCHAR(256) @NULL,                -- Group Amounts

    ChargesAmount @DECIMAL default 0 @NOT_NULL,         -- Group Amounts

    NetAmount @DECIMAL default 0 @NOT_NULL,             -- Group Amounts -- = LinesAmount - DiscountAmount + ChargesAmount
    VatAmount @DECIMAL default 0 @NOT_NULL,             -- Group Amounts
    TotalAmount @DECIMAL default 0 @NOT_NULL,           -- Group Amounts

    IsLocked @BOOL default 0 @NOT_NULL,                 -- Group Status -- Lock document from editing
    IsCancelled @BOOL default 0 @NOT_NULL,              -- Group Status

    CreatedAt @DATE_TIME @NOT_NULL,                     -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser; Group Audit
    ModifiedAt @DATE_TIME @NULL,                        -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                     -- Lookup AppUser; Group Audit
    PostedAt @DATE_TIME @NULL,                          -- Group Audit
    PostedBy @NVARCHAR(40) @NULL,                       -- Lookup AppUser; Group Audit
    CancelledAt @DATE_TIME @NULL,                       -- Group Audit
    CancelledBy @NVARCHAR(40) @NULL,                    -- Lookup AppUser; Group Audit

    Remarks @NVARCHAR(512) @NULL,                       -- Memo; Group Notes -- internal
    Comments @NVARCHAR(512) @NULL,                      -- Memo; Group Notes -- customer visible

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (SalesPersonId) REFERENCES Person(Id),
    FOREIGN KEY (ProjectId) REFERENCES Project(Id),
    FOREIGN KEY (CostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (BranchId) REFERENCES CompanyBranch(Id),

    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),

    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(Id),
    FOREIGN KEY (PaymentTermId) REFERENCES PaymentTerm(Id),

    FOREIGN KEY (BillingCountryId) REFERENCES Country(Id),
    FOREIGN KEY (ShippingCountryId) REFERENCES Country(Id),

    FOREIGN KEY (SourceId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelsTradeId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelledByTradeId) REFERENCES Trade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (PostedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (CancelledBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockTrade()
    {
        string TableName = "StockTrade";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- controls numbering, posting behavior and movement direction

    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup; Group Warehouses -- main/source warehouse
    ToWarehouseId @NVARCHAR(40) @NULL,                  -- Lookup; Group Warehouses -- destination warehouse, used only for transfers

    Code @NVARCHAR(40) @NOT_NULL,                       -- Code STK-DRAFT-YYYY-XXXXXX STOCK_TRADE_DRAFT

    DocumentDate @DATE @NOT_NULL,                       -- Group Dates
    PostingDate @DATE @NULL,                            -- Group Dates -- date used for generated stock movements

    StatusId int @NOT_NULL,                             -- Enum TradeStatus

    TotalCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,       -- total internal stock cost value posted by this document

    Remarks @NVARCHAR(512) @NULL,                       -- Memo; Group Notes -- internal notes

    IsLocked @BOOL DEFAULT 0 @NOT_NULL,                 -- Group Status
    IsCancelled @BOOL DEFAULT 0 @NOT_NULL,              -- Group Status

    CancelsStockTradeId @NVARCHAR(40) @NULL,            -- Locator StockTrade; Group Relations -- original document cancelled by this one
    CancelledByStockTradeId @NVARCHAR(40) @NULL,        -- Locator StockTrade; Group Relations -- reverse/cancellation document

    CreatedAt @DATE_TIME @NOT_NULL,                     -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser; Group Audit
    ModifiedAt @DATE_TIME @NULL,                        -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                     -- Lookup AppUser; Group Audit
    PostedAt @DATE_TIME @NULL,                          -- Group Audit
    PostedBy @NVARCHAR(40) @NULL,                       -- Lookup AppUser; Group Audit
    CancelledAt @DATE_TIME @NULL,                       -- Group Audit
    CancelledBy @NVARCHAR(40) @NULL,                    -- Lookup AppUser; Group Audit

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (ToWarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (CancelsStockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (CancelledByStockTradeId) REFERENCES StockTrade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (PostedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (CancelledBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockMovement()
    {
        string TableName = "StockMovement";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    MovementDate @DATE @NOT_NULL,                       -- stock ledger date
    Direction int @NOT_NULL,                            -- 1=in, -1=out

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive, in movement unit
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- quantity in product primary unit

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- unit of measure snapshot
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- converts Quantity to PrimaryQuantity

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit at movement time
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceModule @NVARCHAR(64) @NOT_NULL,               -- source module name, e.g. Trade or StockTrade
    SourceTable @NVARCHAR(64) @NOT_NULL,                -- source line table, e.g. TradeLine or StockTradeLine
    SourceId @NVARCHAR(40) @NOT_NULL,                   -- source line Id

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- source document type
    DocumentCode @NVARCHAR(40) @NOT_NULL,               -- source document code snapshot
    DocumentDate @DATE @NOT_NULL,                       -- source document date snapshot

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CHK_{TableName}_PrimaryQuantity CHECK (PrimaryQuantity >= 0),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockCount()
    {
        string TableName = "StockCount";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code SC-YYYY-XXXXXX STOCK_COUNT

    WarehouseId @NVARCHAR(40) @NOT_NULL,              -- Lookup

    CountDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo; Group Notes

    CancelledDocumentId @NVARCHAR(40) @NULL,          -- Locator StockCount; Group Relations
    CancellationDocumentId @NVARCHAR(40) @NULL,       -- Locator StockCount; Group Relations

    CreatedAt @DATE_TIME @NOT_NULL,                   -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser; Group Audit
    ModifiedAt @DATE_TIME @NULL,                      -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                   -- Lookup AppUser; Group Audit

    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockReservation()
    {
        string TableName = "StockReservation";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    ProductId @NVARCHAR(40) @NOT_NULL,                 -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,              -- Lookup

    ReservedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    ExecutedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,

    SourceModule @NVARCHAR(96) @NOT_NULL,
    SourceTable @NVARCHAR(96) @NOT_NULL,
    SourceId @NVARCHAR(40) @NOT_NULL,
    SourceLineId @NVARCHAR(40) @NOT_NULL,

    CreatedAt @DATE_TIME @NOT_NULL,

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    CONSTRAINT UQ_{TableName}_SourceLine UNIQUE (SourceLineId)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_FinanceMovement()
    {
        string TableName = "FinanceMovement";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    MovementDate @DATE @NOT_NULL,

    CashAccountId @NVARCHAR(40) @NULL,                -- Lookup
    CompanyBankAccountId @NVARCHAR(40) @NULL,         -- Lookup

    Direction int @NOT_NULL,                          -- 1=in, -1=out

    Amount @DECIMAL DEFAULT 0 @NOT_NULL,

    CurrencyId @NVARCHAR(40) @NOT_NULL,              -- Lookup
    ExchangeRate @DECIMAL DEFAULT 1 @NOT_NULL,

    SourceModule @NVARCHAR(64) @NOT_NULL,
    SourceTable @NVARCHAR(64) @NOT_NULL,
    SourceId @NVARCHAR(40) @NOT_NULL,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,          -- Lookup
    DocumentCode @NVARCHAR(40) @NOT_NULL,
    DocumentDate @DATE @NOT_NULL,

    Remarks @NVARCHAR(512) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,               -- Lookup AppUser

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Amount CHECK (Amount >= 0),

    FOREIGN KEY (CashAccountId) REFERENCES CashAccount(Id),
    FOREIGN KEY (CompanyBankAccountId) REFERENCES CompanyBankAccount(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_Asset()
    {
        string TableName = "Asset";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                  -- Code AST-XXXXXX ASSET
    Name @NVARCHAR(96) @NOT_NULL,

    AssetCategoryId @NVARCHAR(40) @NOT_NULL,       -- Lookup; Group Classification
    AssetLocationId @NVARCHAR(40) @NULL,           -- Lookup; Group Classification

    StatusId int DEFAULT 1 @NOT_NULL,              -- Enum AssetStatus

    AcquisitionDate @DATE @NOT_NULL,               -- Group Acquisition
    InServiceDate @DATE @NULL,                     -- Group Acquisition

    AcquisitionCost @DECIMAL @NOT_NULL,            -- Group Acquisition

    DepreciationMethodId @NVARCHAR(40) @NOT_NULL,  -- Lookup; Group Depreciation
    UsefulLifeMonths int @NOT_NULL,                -- Group Depreciation
    SalvageValue @DECIMAL DEFAULT 0 @NOT_NULL,     -- Group Depreciation

    AccumulatedDepreciation @DECIMAL DEFAULT 0 @NOT_NULL, -- Group Depreciation
    BookValue @DECIMAL DEFAULT 0 @NOT_NULL,        -- Group Depreciation

    SerialNumber @NVARCHAR(96) @NULL,              -- Group Classification

    SupplierId @NVARCHAR(40) @NULL,                -- Locator Supplier; Group Supplier

    Remarks @NBLOB_TEXT @NULL,                     -- LargeMemo; Group Notes

    CreatedAt @DATE_TIME @NOT_NULL,                -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,             -- Lookup AppUser; Group Audit
    ModifiedAt @DATE_TIME @NULL,                   -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                -- Lookup AppUser; Group Audit

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),

    FOREIGN KEY (AssetCategoryId) REFERENCES AssetCategory(Id),
    FOREIGN KEY (AssetLocationId) REFERENCES AssetLocation(Id),
    FOREIGN KEY (DepreciationMethodId) REFERENCES AssetDepreciationMethod(Id),
    FOREIGN KEY (SupplierId) REFERENCES ProductSupplier(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_TradeTax()
    {
        string TableName = "TradeTax";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    TradeId @NVARCHAR(40) @NOT_NULL,                    -- Master
    VatRateId @NVARCHAR(40) @NOT_NULL,                  -- Lookup

    VatRatePercent @DECIMAL default 0 @NOT_NULL,        -- Snapshot of the percent at production time

    NetAmount @DECIMAL default 0 @NOT_NULL,
    VatAmount @DECIMAL default 0 @NOT_NULL,
    TotalAmount @DECIMAL default 0 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Trade_VatRate UNIQUE (TradeId, VatRateId),

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_TradeLine()
    {
        string TableName = "TradeLine";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    TradeId @NVARCHAR(40) @NOT_NULL,                    -- Master

    LineNo int @NOT_NULL,

    LineTypeId int default 1 @NOT_NULL,                 -- Enum TradeLineType

    ProductId @NVARCHAR(40) @NULL,                      -- Locator Product

    ProductCode @NVARCHAR(40) @NULL,                    -- Snapshot
    ProductName @NVARCHAR(128) @NULL,                   -- Snapshot

    Description @NVARCHAR(256) @NULL,                   -- Snapshot

    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup (line override)

    UnitOfMeasureId @NVARCHAR(40) @NULL,                -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NULL,              -- Snapshot

    UnitRatio @DECIMAL default 1 @NOT_NULL,             -- Snapshot ratio to primary unit

    Quantity @DECIMAL default 0 @NOT_NULL,
    PrimaryUnitQuantity @DECIMAL default 0 @NOT_NULL,

    ReservedQuantity @DECIMAL default 0 @NOT_NULL,
    ExecutedQuantity @DECIMAL default 0 @NOT_NULL,

    VatRateId @NVARCHAR(40) @NULL,                      -- Lookup
    VatRatePercent @DECIMAL default 0 @NOT_NULL,        -- Snapshot

    UnitPrice @DECIMAL default 0 @NOT_NULL,

    GrossAmount @DECIMAL default 0 @NOT_NULL,           -- Quantity * UnitPrice

    DiscountPercent @DECIMAL default 0 @NOT_NULL,
    DiscountAmount @DECIMAL default 0 @NOT_NULL,

    NetUnitPrice @DECIMAL default 0 @NOT_NULL,          -- Display/convenience value

    NetAmount @DECIMAL default 0 @NOT_NULL,             -- GrossAmount - DiscountAmount
    VatAmount @DECIMAL default 0 @NOT_NULL,
    TotalAmount @DECIMAL default 0 @NOT_NULL,

    SourceTradeLineId @NVARCHAR(40) @NULL,              -- Locator TradeLine

    CONSTRAINT UQ_{TableName}_Trade_LineNo UNIQUE (TradeId, LineNo),

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),

    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id),

    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockBalance()
    {
        string TableName = "StockBalance";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- current stock in product primary unit
    TotalCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,       -- current total stock value
    AverageUnitCost @DECIMAL DEFAULT 0 @NOT_NULL,       -- TotalCostAmount / PrimaryQuantity

    LastMovementDate @DATE @NULL,
    LastMovementId @NVARCHAR(40) @NULL,

    CONSTRAINT UQ_{TableName}_Product_Warehouse UNIQUE (ProductId, WarehouseId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (LastMovementId) REFERENCES StockMovement(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockCountLine()
    {
        string TableName = "StockCountLine";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    StockCountId @NVARCHAR(40) @NOT_NULL,             -- Master

    LineNo int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                -- Locator Product

    ProductCode @NVARCHAR(40) @NOT_NULL,
    ProductName @NVARCHAR(96) @NOT_NULL,

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,          -- Lookup

    SystemQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    CountedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceQuantity @DECIMAL DEFAULT 0 @NOT_NULL,

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,

    Remarks @NVARCHAR(512) @NULL,

    FOREIGN KEY (StockCountId) REFERENCES StockCount(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),

    CONSTRAINT UQ_{TableName}_LineNo UNIQUE (StockCountId, LineNo)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_FinanceBalance()
    {
        string TableName = "FinanceBalance";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    CashAccountId @NVARCHAR(40) @NULL,                -- Lookup
    CompanyBankAccountId @NVARCHAR(40) @NULL,         -- Lookup

    Balance @DECIMAL DEFAULT 0 @NOT_NULL,

    LastMovementDate @DATE @NULL,
    LastMovementId @NVARCHAR(40) @NULL,

    FOREIGN KEY (CashAccountId) REFERENCES CashAccount(Id),
    FOREIGN KEY (CompanyBankAccountId) REFERENCES CompanyBankAccount(Id),
    FOREIGN KEY (LastMovementId) REFERENCES FinanceMovement(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_AssetDepreciationLine()
    {
        string TableName = "AssetDepreciationLine";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    AssetId @NVARCHAR(40) @NOT_NULL,                  -- Master

    DepreciationDate @DATE @NOT_NULL,

    DepreciationAmount @DECIMAL DEFAULT 0 @NOT_NULL,
    AccumulatedDepreciation @DECIMAL DEFAULT 0 @NOT_NULL,
    BookValueAfter @DECIMAL DEFAULT 0 @NOT_NULL,

    JournalEntryId @NVARCHAR(40) @NULL,               -- Lookup

    Remarks @NVARCHAR(512) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser

    FOREIGN KEY (AssetId) REFERENCES Asset(Id),
    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )
";
        Version.AddTable(SqlText);
    }
    void RegisterTable_StockTradeLine()
    {
        string TableName = "StockTradeLine";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    StockTradeId @NVARCHAR(40) @NOT_NULL,               -- Master
    LineNo int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    ProductCode @NVARCHAR(40) @NOT_NULL,                -- product code snapshot
    ProductName @NVARCHAR(128) @NOT_NULL,               -- product name snapshot

    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup -- optional source warehouse override

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- unit of measure snapshot
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- converts line quantity to primary/base quantity

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive, direction is determined by DocumentType
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- Quantity * UnitRatio

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceTradeLineId @NVARCHAR(40) @NULL,              -- optional source commercial line
    SourceStockTradeLineId @NVARCHAR(40) @NULL,         -- optional source stock line, e.g. reversal/copy/adjustment flow

    Remarks @NVARCHAR(512) @NULL,                       -- internal line notes

    CONSTRAINT UQ_{TableName}_StockTrade_LineNo UNIQUE (StockTradeId, LineNo),

    FOREIGN KEY (StockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id),
    FOREIGN KEY (SourceStockTradeLineId) REFERENCES StockTradeLine(Id)
    )
";
        Version.AddTable(SqlText);
    }

    // ● protected
    protected override void RegisterInternal()
    {
        RegisterTable_SYS_LOG();
        RegisterTable_SYS_NUMBER_SERIES();
        RegisterTable_AppUser();
        RegisterTable_CustomerCategory();
        RegisterTable_SupplierCategory();
        RegisterTable_ProductBrand();
        RegisterTable_DiscountCategory();
        RegisterTable_UnitOfMeasure();
        RegisterTable_TaxOffice();
        RegisterTable_Bank();
        RegisterTable_ExpenseCategory();
        RegisterTable_PaymentMethod();
        RegisterTable_SalesPerson();
        RegisterTable_Carrier();
        RegisterTable_Country();
        RegisterTable_Currency();
        RegisterTable_VatRate();
        RegisterTable_PaymentTerm();
        RegisterTable_ProductGroup();
        RegisterTable_FiscalYear();
        RegisterTable_Language();
        RegisterTable_PersonRoleType();
        RegisterTable_StockReason();
        RegisterTable_ContactType();
        RegisterTable_AssetCategory();
        RegisterTable_AssetLocation();
        RegisterTable_AssetDepreciationMethod();
        RegisterTable_ProductDimension();
        RegisterTable_ProductAttributeGroup();
        RegisterTable_Account();
        RegisterTable_PriceListType();
        RegisterTable_Company();
        RegisterTable_TaxCategory();
        RegisterTable_FiscalPeriod();
        RegisterTable_Person();
        RegisterTable_Category();
        RegisterTable_FixedAsset();
        RegisterTable_ProductDimensionValue();
        RegisterTable_DocumentType();
        RegisterTable_CompanyBranch();
        RegisterTable_CompanyBankAccount();
        RegisterTable_PersonRole();
        RegisterTable_CostCenter();
        RegisterTable_Product();
        RegisterTable_PersonAddress();
        RegisterTable_PersonContact();
        RegisterTable_PersonBankAccount();
        RegisterTable_AssetAssignment();
        RegisterTable_AssetMaintenance();
        RegisterTable_AssetDocument();
        RegisterTable_AssetInsurance();
        RegisterTable_JournalEntry();
        RegisterTable_PriceList();
        RegisterTable_ProductGroups();
        RegisterTable_Warehouse();
        RegisterTable_Project();
        RegisterTable_ProductCategory();
        RegisterTable_ProductUnitOfMeasure();
        RegisterTable_ProductBarcode();
        RegisterTable_ProductSupplier();
        RegisterTable_BillOfMaterial();
        RegisterTable_CashAccount();
        RegisterTable_ProductImage();
        RegisterTable_ProductAttribute();
        RegisterTable_JournalEntryLine();
        RegisterTable_ProductWarehouse();
        RegisterTable_WarehouseLocation();
        RegisterTable_BillOfMaterialLine();
        RegisterTable_Trade();
        RegisterTable_StockTrade();
        RegisterTable_StockMovement();
        RegisterTable_StockCount();
        RegisterTable_StockReservation();
        RegisterTable_FinanceMovement();
        RegisterTable_Asset();
        RegisterTable_TradeTax();
        RegisterTable_TradeLine();
        RegisterTable_StockBalance();
        RegisterTable_StockCountLine();
        RegisterTable_FinanceBalance();
        RegisterTable_AssetDepreciationLine();
        RegisterTable_StockTradeLine();
    }

    // ● construction
    public SchemaVersion1()
    {
    }

    // ● properties
    public override int VersionNumber { get; } = 1;
}


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
    ,Message @NBLOB_TEXT(96) @NOT_NULL
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
    Code @NVARCHAR(40) @NOT_NULL,
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
    void RegisterTable_PriceList()
    {
        string TableName = "PriceList";
        string SqlText = $@"
CREATE TABLE {TableName} (
   Id @NVARCHAR(40) @NOT_NULL primary key,

    PriceTypeId @NVARCHAR(40) @NOT_NULL,        -- Lookup

    DiscountGroupId @NVARCHAR(40) @NULL,        -- Lookup
    CustomerId @NVARCHAR(40) @NULL,             -- Locator

    ProductId @NVARCHAR(40) @NOT_NULL,          -- Locator
    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,    -- Lookup

    MinQuantity @DECIMAL_(18, 4) default 0 @NOT_NULL,

    UnitPrice @DECIMAL_(18, 4) @NOT_NULL,

    ValidFrom @DATE @NULL,
    ValidTo @DATE @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    FOREIGN KEY (PriceTypeId) REFERENCES PriceType(Id),
    FOREIGN KEY (DiscountGroupId) REFERENCES DiscountGroup(Id),
    FOREIGN KEY (CustomerId) REFERENCES Person(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id)
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
    void RegisterTable_NumberSeries()
    {
        string TableName = "NumberSeries";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Prefix @NVARCHAR(16) @NULL,
    Padding int default 6 @NOT_NULL,
    NextNumber int @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
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
    void RegisterTable_Company()
    {
        string TableName = "Company";
        string SqlText = $@"
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,
    TaxNumber @NVARCHAR(32) @NOT_NULL,
    TaxOfficeId @NVARCHAR(40) @NULL,        -- Lookup
    CountryId @NVARCHAR(40) @NOT_NULL,      -- Lookup
    CurrencyId @NVARCHAR(40) @NOT_NULL,     -- Lookup
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
    CountryId @NVARCHAR(40) @NOT_NULL,          -- Lookup
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
    void RegisterTable_Warehouse()
    {
        string TableName = "Warehouse";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                       -- business code
    Name @NVARCHAR(96) @NOT_NULL,                       -- display title

    CompanyId @NVARCHAR(40) @NOT_NULL,                  -- Lookup      -- owner company
    BranchId @NVARCHAR(40) @NULL,                       -- Lookup      -- optional company branch

    WarehouseTypeId integer default 0 @NOT_NULL,        -- Enum -- Main, Store, Transit, Production, Scrap, Virtual

    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,
    CountryId @NVARCHAR(40) @NULL,                      -- Lookup

    Phone @NVARCHAR(32) @NULL,
    Email @NVARCHAR(96) @NULL,

    ResponsiblePersonId @NVARCHAR(40) @NULL,            -- Locator  -- Person responsible for warehouse

    IsActive @BOOL default 1 @NOT_NULL,
    IsVirtual @BOOL default 0 @NOT_NULL,                -- logical/non-physical warehouse
    AllowNegativeStock @BOOL default 0 @NOT_NULL,       -- allow stock below zero
    AffectsAvailability @BOOL default 1 @NOT_NULL,      -- participates in available stock

    Color @NVARCHAR(32) @NULL,                          -- ui display color
    IconName @NVARCHAR(96) @NULL,                       -- ui icon

    Remarks @NBLOB_TEXT @NULL,

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
    void RegisterTable_DocumentType()
    {
        string TableName = "DocumentType";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                       -- business code
    Name @NVARCHAR(96) @NOT_NULL,                       -- display title

    TradeTypeId integer @NOT_NULL,                      -- Enum   -- Sales, Purchases, Warehouse, etc.

    NumberSeriesId @NVARCHAR(40) @NULL,                 -- Lookup -- numbering series

    IsActive @BOOL default 1 @NOT_NULL,

    AffectsStock @BOOL default 0 @NOT_NULL,             -- creates stock movements
    AffectsFinancial @BOOL default 0 @NOT_NULL,         -- affects customer/supplier balances
    AffectsAccounting @BOOL default 0 @NOT_NULL,        -- creates accounting entries

    StockDirection integer default 0 @NOT_NULL,         -- 1=in, -1=out, 0=no stock effect
    FinancialDirection integer default 0 @NOT_NULL,     -- 1=debit, -1=credit, 0=no effect
    AccountingDirection integer default 0 @NOT_NULL,    -- reserved for accounting logic

    IsCancellation @BOOL default 0 @NOT_NULL,           -- reverses/cancels another document type
    TargetDocumentTypeId @NVARCHAR(40) @NULL,           -- target/reversed document type

    RequiresApproval @BOOL default 0 @NOT_NULL,         -- requires approval before completion
    AutoComplete @BOOL default 0 @NOT_NULL,             -- auto-post on save

    Color @NVARCHAR(32) @NULL,                          -- ui display color
    IconName @NVARCHAR(96) @NULL,                       -- ui icon

    PrintTemplate @NVARCHAR(96) @NULL,                  -- print layout/template
    ReportName @NVARCHAR(96) @NULL,                     -- internal report identifier

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (NumberSeriesId) REFERENCES NumberSeries(Id),
    FOREIGN KEY (TargetDocumentTypeId) REFERENCES DocumentType(Id)
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
    void RegisterTable_Person()
    {
        string TableName = "Person";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,

    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,

    TaxNumber @NVARCHAR(32) @NULL,
    TaxOfficeId @NVARCHAR(40) @NULL,        -- Lookup

    CountryId @NVARCHAR(40) @NULL,          -- Lookup
    CurrencyId @NVARCHAR(40) @NULL,         -- Lookup
    LanguageId @NVARCHAR(40) @NULL,         -- Lookup   -- preferred language

    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,

    Phone @NVARCHAR(32) @NULL,
    Mobile @NVARCHAR(32) @NULL,
    Email @NVARCHAR(96) @NULL,
    Website @NVARCHAR(96) @NULL,

    ContactPerson @NVARCHAR(96) @NULL,

    Notes @NBLOB_TEXT @NULL,

    IsCompany @BOOL default 1 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,
    IconName @NVARCHAR(96) @NULL,

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
    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator   -- responsible person

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
    void RegisterTable_Project()
    {
        string TableName = "Project";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                   -- business code
    Name @NVARCHAR(96) @NOT_NULL,                   -- display title

    CustomerId @NVARCHAR(40) @NULL,                 -- Locator     -- customer/person owner

    ProjectStatusId integer default 0 @NOT_NULL,    -- Enum         -- Draft, Active, Completed, Cancelled

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    CostCenterId @NVARCHAR(40) @NULL,

    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator      -- responsible person

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
    void RegisterTable_Product()
    {
        string TableName = "Product";
        string SqlText = $@"
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,                           -- business code
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

    CONSTRAINT UQ_{TableName}_Product_Category UNIQUE (CategoryId, CategoryId)
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

    // ● protected
    protected override void RegisterInternal()
    {
        RegisterTable_SYS_LOG();
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
        RegisterTable_PriceListType();
        RegisterTable_PriceList();
        RegisterTable_PaymentTerm();
        RegisterTable_NumberSeries();
        RegisterTable_ProductGroup();
        RegisterTable_ProductGroups();
        RegisterTable_Company();
        RegisterTable_CompanyBranch();
        RegisterTable_CompanyBankAccount();
        RegisterTable_TaxCategory();
        RegisterTable_FiscalYear();
        RegisterTable_FiscalPeriod();
        RegisterTable_Warehouse();
        RegisterTable_DocumentType();
        RegisterTable_Language();
        RegisterTable_Person();
        RegisterTable_PersonRoleType();
        RegisterTable_PersonRole();
        RegisterTable_CostCenter();
        RegisterTable_Project();
        RegisterTable_StockReason();
        RegisterTable_Category();
        RegisterTable_Product();
        RegisterTable_ProductCategory();
        RegisterTable_ProductUnitOfMeasure();
    }

    // ● construction
    public SchemaVersion1()
    {
    }

    // ● properties
    public override int VersionNumber { get; } = 1;
}
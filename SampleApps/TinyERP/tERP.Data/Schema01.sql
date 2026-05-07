/*---------------------------------------------------
Table: CustomerCategory
Group: Sales
-----------------------------------------------------

----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: SupplierCategory
Group: Purchases
-----------------------------------------------------

----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: ProductBrand
Group: Inventory
-----------------------------------------------------

----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: DiscountCategory
Group: Sales
-----------------------------------------------------

----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: UnitOfMeasure
Group: Inventory
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: TaxOffice
Group: Setup
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: Bank
Group: Setup
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: ExpenseCategory
Group: Accounting
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: PaymentMethod
Group: Sales
-----------------------------------------------------

----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
/*---------------------------------------------------
Table: DocumentType
Group: Documents
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
/*---------------------------------------------------
Table: SalesPerson
Group: Sales
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
/*---------------------------------------------------
Table: Carrier
Group: Purchases
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: Country
Group: Setup
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Iso2 @NVARCHAR(2) @NOT_NULL,
    Iso3 @NVARCHAR(3) @NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: Currency
Group: Setup
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Symbol @NVARCHAR(8) @NOT_NULL,
    Decimals int default 2 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: VatRate
Group: Setup
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Percent @DECIMAL_(5,2) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )

/*---------------------------------------------------
Table: PriceList
Group: Sales
-----------------------------------------------------  
    RETAIL      Retail Prices
    WHOLESALE   Wholesale Prices
    EXPORT      Export Prices
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    CurrencyId @NVARCHAR(40) @NOT_NULL, -- default currency

    IsTaxIncluded @BOOL default 1 @NOT_NULL, -- prices include vat
    IsDefault @BOOL default 0 @NOT_NULL,     -- default price list

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,       -- ui display color
    IconName @NVARCHAR(96) @NULL,    -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),

    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )


/*---------------------------------------------------
Table: PaymentTerm
Group: Sales
-----------------------------------------------------  
    CASH      Cash Payment
    NET30     30 Days
    NET60     60 Days
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: NumberSeries
Group: Setup
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductGroup
Group: Inventory
-----------------------------------------------------  
    CONSUMER   Consumer Products
    EXPORT     Export Products
    SEASONAL   Seasonal Products
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductGroups
Group: Inventory
-----------------------------------------------------  
    (Coffee Machine, Consumer)
    (Coffee Machine, Seasonal)
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,
    ProductGroupId @NVARCHAR(40) @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Product_Group UNIQUE (ProductId, ProductGroupId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (ProductGroupId) REFERENCES ProductGroup(Id)
    )

/*---------------------------------------------------
Table: Company
Group: Company
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,
    TaxNumber @NVARCHAR(32) @NOT_NULL,
    TaxOfficeId @NVARCHAR(40) @NULL,
    CountryId @NVARCHAR(40) @NOT_NULL,
    CurrencyId @NVARCHAR(40) @NOT_NULL,
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

/*---------------------------------------------------
Table: CompanyBranch
Group: Company
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    CompanyId @NVARCHAR(40) @NOT_NULL,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,
    CountryId @NVARCHAR(40) @NOT_NULL,
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

/*---------------------------------------------------
Table: CompanyBankAccount
Group: Company
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id  @NVARCHAR(40)  @NOT_NULL primary key,
    CompanyId @NVARCHAR(40) @NOT_NULL,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    BankName @NVARCHAR(96) @NOT_NULL,
    Iban @NVARCHAR(40) @NOT_NULL,
    SwiftBic @NVARCHAR(16) @NULL,
    CurrencyId @NVARCHAR(40) @NOT_NULL,
    IsDefault int default 0 @NOT_NULL,
    IsActive int default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    CONSTRAINT UQ_{TableName}_CompanyId_Code UNIQUE (CompanyId, Code)
    )

/*---------------------------------------------------
Table: TaxCategory
Group: Accounting
-----------------------------------------------------  
    DOMESTIC     Domestic Transactions
    EU           European Union
    THIRD        Third Countries
    EXEMPT       Tax Exempt
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    VatRateId @NVARCHAR(40) @NULL,   -- default vat rate

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

/*---------------------------------------------------
Table: FiscalYear
Group: Company
-----------------------------------------------------  
    FY2025   Fiscal Year 2025
    FY2026   Fiscal Year 2026
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: FiscalPeriod
Group: Company
-----------------------------------------------------  
    FY2025-01   January 2025
    FY2025-02   February 2025
    FY2025-12   December 2025
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    FiscalYearId @NVARCHAR(40) @NOT_NULL,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    PeriodNo integer @NOT_NULL,      -- 1..12 or custom sequence

    StartDate @DATE @NOT_NULL,
    EndDate @DATE @NOT_NULL,

    IsClosed @BOOL default 0 @NOT_NULL, -- no postings allowed

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_FiscalYear_PeriodNo UNIQUE (FiscalYearId, PeriodNo),

    FOREIGN KEY (FiscalYearId) REFERENCES FiscalYear(Id)
    )

/*---------------------------------------------------
Table: Warehouse
Group: Inventory
-----------------------------------------------------  
    MAIN      Main Warehouse
    STORE-01  Retail Store
    TRANSIT   Goods In Transit
    SCRAP     Scrap / Damaged Stock
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,        -- business code
    Name @NVARCHAR(96) @NOT_NULL,        -- display title

    CompanyId @NVARCHAR(40) @NOT_NULL,   -- owner company
    BranchId @NVARCHAR(40) @NULL,        -- optional company branch

    WarehouseType integer default 0 @NOT_NULL, -- Main, Store, Transit, Production, Scrap, Virtual

    AddressLine1 @NVARCHAR(160) @NULL,
    AddressLine2 @NVARCHAR(160) @NULL,
    City @NVARCHAR(96) @NULL,
    PostalCode @NVARCHAR(16) @NULL,
    CountryId @NVARCHAR(40) @NULL,

    Phone @NVARCHAR(32) @NULL,
    Email @NVARCHAR(96) @NULL,

    ResponsiblePersonId @NVARCHAR(40) @NULL, -- Person responsible for warehouse

    IsActive @BOOL default 1 @NOT_NULL,
    IsVirtual @BOOL default 0 @NOT_NULL,             -- logical/non-physical warehouse
    AllowNegativeStock @BOOL default 0 @NOT_NULL,    -- allow stock below zero
    AffectsAvailability @BOOL default 1 @NOT_NULL,   -- participates in available stock

    Color @NVARCHAR(32) @NULL,       -- ui display color
    IconName @NVARCHAR(96) @NULL,    -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (BranchId) REFERENCES CompanyBranch(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    FOREIGN KEY (ResponsiblePersonId) REFERENCES Person(Id)
    )

/*---------------------------------------------------
Table: DocumentType
Group: Documents
-----------------------------------------------------  
    SAL-INV     Sales Invoice
    PUR-INV     Purchase Invoice
    RETAIL      Retail Receipt
    SAL-CREDIT  Sales Credit Note
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,     -- business code
    Name @NVARCHAR(96) @NOT_NULL,     -- display title

    TradeType integer @NOT_NULL,      -- Sales, Purchases, Warehouse, etc.

    NumberSeriesId @NVARCHAR(40) @NULL, -- numbering series

    IsActive @BOOL default 1 @NOT_NULL,

    AffectsStock @BOOL default 0 @NOT_NULL,       -- creates stock movements
    AffectsFinancial @BOOL default 0 @NOT_NULL,   -- affects customer/supplier balances
    AffectsAccounting @BOOL default 0 @NOT_NULL,  -- creates accounting entries

    StockDirection integer default 0 @NOT_NULL,       -- 1=in, -1=out, 0=no stock effect
    FinancialDirection integer default 0 @NOT_NULL,   -- 1=debit, -1=credit, 0=no effect
    AccountingDirection integer default 0 @NOT_NULL,  -- reserved for accounting logic

    IsCancellation @BOOL default 0 @NOT_NULL, -- reverses/cancels another document type
    TargetDocumentTypeId @NVARCHAR(40) @NULL, -- target/reversed document type

    RequiresApproval @BOOL default 0 @NOT_NULL, -- requires approval before completion
    AutoComplete @BOOL default 0 @NOT_NULL,     -- auto-post on save

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    PrintTemplate @NVARCHAR(96) @NULL, -- print layout/template
    ReportName @NVARCHAR(96) @NULL,    -- internal report identifier

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (NumberSeriesId) REFERENCES NumberSeries(Id),
    FOREIGN KEY (TargetDocumentTypeId) REFERENCES DocumentType(Id)
    )

/*---------------------------------------------------
Table: Language
Group: System
-----------------------------------------------------  
    EN   English
    EL   Greek
    DE   German
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(16) @NOT_NULL, -- ISO code, e.g. EN, EL, DE
    Name @NVARCHAR(96) @NOT_NULL, -- display title

    CultureName @NVARCHAR(32) @NULL, -- en-US, el-GR, de-DE

    IsDefault @BOOL default 0 @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,

    IsRightToLeft @BOOL default 0 @NOT_NULL, -- Arabic, Hebrew, etc.

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon / flag icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: Person
Group: People
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,

    Name @NVARCHAR(96) @NOT_NULL,
    Title @NVARCHAR(160) @NULL,

    TaxNumber @NVARCHAR(32) @NULL,
    TaxOfficeId @NVARCHAR(40) @NULL,

    CountryId @NVARCHAR(40) @NULL,
    CurrencyId @NVARCHAR(40) @NULL,
    LanguageId @NVARCHAR(40) @NULL, -- preferred language

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


/*---------------------------------------------------
Table: PersonRoleType
Group: People
-----------------------------------------------------  
    CUS = Customer
    SUP = Supplier
    CAR = Carrier   
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: PersonRole
Group: People
-----------------------------------------------------  
    (Alpha Transport, Supplier)
    (Alpha Transport, Carrier)
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    PersonId @NVARCHAR(40) @NOT_NULL,
    RoleTypeId @NVARCHAR(40) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    Remarks @NBLOB_TEXT @NULL,

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (RoleTypeId) REFERENCES PersonRoleType(Id),

    CONSTRAINT UQ_{TableName}_Person_Role UNIQUE (PersonId, RoleTypeId)
    )


/*---------------------------------------------------
Table: CostCenter
Group: Company
-----------------------------------------------------  
    ADM       Administration
    SALES     Sales Department
    PROD      Production
    SUPPORT   Technical Support
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    ParentCostCenterId @NVARCHAR(40) @NULL, -- optional hierarchy parent

    ManagerPersonId @NVARCHAR(40) @NULL, -- responsible person

    StartDate @DATE @NULL, -- activation date
    EndDate @DATE @NULL,   -- deactivation date

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentCostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (ManagerPersonId) REFERENCES Person(Id)
    )


/*---------------------------------------------------
Table: Project
Group: Projects
-----------------------------------------------------  
    PRJ-0001   ERP Installation
    PRJ-0002   CRM Migration
    PRJ-0003   Warehouse Automation
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    CustomerId @NVARCHAR(40) @NULL,  -- customer/person owner

    ProjectStatus integer default 0 @NOT_NULL, -- Draft, Active, Completed, Cancelled

    StartDate @DATE @NULL,
    EndDate @DATE @NULL,

    CostCenterId @NVARCHAR(40) @NULL,

    ManagerPersonId @NVARCHAR(40) @NULL, -- responsible person

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (CustomerId) REFERENCES Person(Id),
    FOREIGN KEY (CostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (ManagerPersonId) REFERENCES Person(Id)
    )

/*---------------------------------------------------
Table: StockReason
Group: Inventory
-----------------------------------------------------  
    ADJUST     Inventory Adjustment
    DAMAGE     Damaged Goods
    LOSS       Stock Loss
    RETURN     Customer Return
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,    -- business code
    Name @NVARCHAR(96) @NOT_NULL,    -- display title

    StockDirection integer default 0 @NOT_NULL, -- 1=in, -1=out, 0=no stock effect

    AffectsCost @BOOL default 0 @NOT_NULL, -- affects inventory valuation
    RequiresRemarks @BOOL default 0 @NOT_NULL, -- user must enter explanation

    IsSystem @BOOL default 0 @NOT_NULL, -- protected/system-defined reason
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )


/*---------------------------------------------------
Table: ProductCategory
Group: Inventory
-----------------------------------------------------  
    Electronics
        Laptops
        Monitors

    Food
        Coffee
        Drinks
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    ParentId @NVARCHAR(40) @NULL, -- parent category

    Code @NVARCHAR(40) @NOT_NULL, -- business code
    Name @NVARCHAR(96) @NOT_NULL, -- display title

    LevelNo integer default 0 @NOT_NULL, -- optional hierarchy level

    SortNo integer default 0 @NOT_NULL, -- display order

    VatRateId @NVARCHAR(40) @NULL, -- default vat rate
    RevenueAccount @NVARCHAR(40) @NULL, -- optional accounting account
    ExpenseAccount @NVARCHAR(40) @NULL, -- optional accounting account

    IsSystem @BOOL default 0 @NOT_NULL, -- protected/system category
    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,     -- ui display color
    IconName @NVARCHAR(96) @NULL,  -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentId) REFERENCES ProductCategory(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id)
    )


/*---------------------------------------------------
Table: Product
Group: Inventory
-----------------------------------------------------  
    PRD-0001   Coffee Machine
    PRD-0002   Espresso Beans
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL, -- business code
    Name @NVARCHAR(96) @NOT_NULL, -- display title

    ProductType integer @NOT_NULL, -- Goods, Service, RawMaterial

    ProductCategoryId @NVARCHAR(40) @NULL,
    VatRateId @NVARCHAR(40) @NULL,

    PrimaryUnitOfMeasureId @NVARCHAR(40) @NOT_NULL, -- inventory/base unit

    Barcode @NVARCHAR(64) @NULL,

    Weight @DECIMAL @NULL,
    Volume @DECIMAL @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,
    IconName @NVARCHAR(96) @NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ProductCategoryId) REFERENCES ProductCategory(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id),
    FOREIGN KEY (PrimaryUnitOfMeasureId) REFERENCES UnitOfMeasure(Id)
    )

/*---------------------------------------------------
Table: ProductUnitOfMeasure
Group: Inventory
-----------------------------------------------------  
    (Coffee Machine, Piece, Ratio=1)
    (Coffee Machine, Box, Ratio=12)
    (Coffee Machine, Pallet, Ratio=576)
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,
    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,

    Ratio @DECIMAL @NOT_NULL, -- ratio to primary unit

    Barcode @NVARCHAR(64) @NULL,

    IsSalesDefault @BOOL default 0 @NOT_NULL,
    IsPurchaseDefault @BOOL default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Product_Unit UNIQUE (ProductId, UnitOfMeasureId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id)
    )
/*---------------------------------------------------
Table: SYS_LOG
Module: Log  LogDataModule
Group: System 
IsReadOnly
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: SYS_NUMBER_SERIES
Module: NumberSeries CodeProviderModule
Group: Setup
IsLookup: true   
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AppUser
Module: AppUser
Group: Setup
-----------------------------------------------------
Application users
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: CustomerCategory
Module: CustomerCategory  
Group: Sales 
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: SupplierCategory
Module: SupplierCategory  
Group: Purchases
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: ProductBrand
Module: ProductBrand  
Group: Inventory
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: DiscountCategory
Module: DiscountCategory    
Group: Sales
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: UnitOfMeasure
Module: UnitOfMeasure  
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
Module: TaxOffice    
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
Module: Bank    
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
Module: ExpenseCategory  
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
Module: PaymentMethod
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
Table: SalesPerson
Module: SalesPerson  
Group: Sales 
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,                -- Code XXXX
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
    )
/*---------------------------------------------------
Table: Carrier
Module: Carrier   
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
Module: Country   
Group: Setup
IsLookup: true
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
Module: Currency   
Group: Setup
IsLookup: true  
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
Module: VatRate  
Group: Setup
IsLookup: true   
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
Table: PriceListType
Module: PriceListType   
Group: Sales
IsLookup: true  
-----------------------------------------------------
    RETAIL      Retail Prices
    WHOLESALE   Wholesale Prices
    EXPORT      Export Prices
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: PriceList
Module: PriceList   
Group: Sales 
-----------------------------------------------------
    Product pricing rules
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: PaymentTerm
Module: PaymentTerm  
Group: Sales
IsLookup: true   
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
Table: ProductGroup
Module: ProductGroup  
Group: Inventory
IsLookup: true  
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
-----------------------------------------------------  
    (Coffee Machine, Consumer)
    (Coffee Machine, Seasonal)
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,  -- Master
    GroupId @NVARCHAR(40) @NOT_NULL,    -- Lookup

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Product_Group UNIQUE (ProductId, GroupId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (GroupId) REFERENCES ProductGroup(Id)
    )

/*---------------------------------------------------
Table: Company
Module: Company  
Group: Company
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: CompanyBranch
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: CompanyBankAccount
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: TaxCategory
Module: TaxCategory  
Group: Accounting
IsLookup: true  
-----------------------------------------------------  
    DOMESTIC     Domestic Transactions
    EU           European Union
    THIRD        Third Countries
    EXEMPT       Tax Exempt
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: FiscalYear
Module: FiscalYear  
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
-----------------------------------------------------  
    FY2025-01   January 2025
    FY2025-02   February 2025
    FY2025-12   December 2025
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: Warehouse
Module: Warehouse
Group: Inventory
IsLookup: true
FieldGroups: Address, Settings, Appearance, Notes
-----------------------------------------------------
Represents a physical or logical warehouse where inventory is stored,
received, produced, transferred, or consumed.

Warehouses participate in inventory movements and stock calculations.
They may represent real locations, transit locations, production areas,
scrap areas, or virtual warehouses used for operational purposes.

Examples:
    MAIN      Main Warehouse
    STORE-01  Retail Store
    TRANSIT   Goods In Transit
    SCRAP     Scrap / Damaged Stock
----------------------------------------------------*/
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





/*---------------------------------------------------
Table: Language
Module: Language  
Group: System
IsLookup: true  
-----------------------------------------------------  
    EN   English
    EL   Greek
    DE   German
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: Person
Module: Person
Group: People
FieldGroups: Tax, Preferences, Address, Appearance, Notes
-----------------------------------------------------
Represents a person or organization participating in business
transactions.

Used as the common master record for customers, suppliers,
employees, contacts, and other business parties.

Both individuals and companies are stored in the same table and
distinguished by the IsCompany flag.
----------------------------------------------------*/
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




/*---------------------------------------------------
Table: PersonRoleType
Module: PersonRoleType   
Group: People
IsLookup: true  
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
-----------------------------------------------------  
    (Alpha Transport, Supplier)
    (Alpha Transport, Carrier)
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: CostCenter
Module: CostCenter  
Group: Company
IsLookup: true   
-----------------------------------------------------  
    ADM       Administration
    SALES     Sales Department
    PROD      Production
    SUPPORT   Technical Support
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: Project
Module: Project  
Group: Projects
-----------------------------------------------------  
    PRJ-0001   ERP Installation
    PRJ-0002   CRM Migration
    PRJ-0003   Warehouse Automation
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: StockReason
Module: StockReason  
Group: Inventory
-----------------------------------------------------  
    ADJUST     Inventory Adjustment
    DAMAGE     Damaged Goods
    LOSS       Stock Loss
    RETURN     Customer Return
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: Category
Module: Category  
Group: Inventory
IsLookup: true
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


/*---------------------------------------------------
Table: Product
Module: Product  
Group: Inventory
-----------------------------------------------------  
    PRD-0001   Coffee Machine
    PRD-0002   Espresso Beans
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductCategory
-----------------------------------------------------  
 
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductId @NVARCHAR(40) @NOT_NULL,                      -- Master
    CategoryId @NVARCHAR(40) @NOT_NULL,                     -- Lookup

    IsActive @BOOL default 1 @NOT_NULL, 

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (CategoryId) REFERENCES Category(Id),

    CONSTRAINT UQ_{TableName}_Product_Category UNIQUE (ProductId, CategoryId)
    )


/*---------------------------------------------------
Table: ProductUnitOfMeasure
-----------------------------------------------------  
    (Coffee Machine, Piece, Ratio=1)
    (Coffee Machine, Box, Ratio=12)
    (Coffee Machine, Pallet, Ratio=576)
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: PersonAddress
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ContactType
Module: ContactType
Group: Setup

IsLookup
-----------------------------------------------------
Defines contact role/type values used by PersonContact.

Examples:
- Sales
- Accounting
- Technical
- Logistics
----------------------------------------------------*/
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL
)

/*---------------------------------------------------
Table: PersonContact
-----------------------------------------------------
Stores contact persons or contact points for a Person.

Examples:
- sales contact for a customer
- accounting contact for a supplier
- technical contact for a partner
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: PersonBankAccount
-----------------------------------------------------
Stores bank accounts belonging to a Person.

Examples:
- customer bank account
- supplier IBAN
- partner settlement account
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductBarcode
-----------------------------------------------------
Stores multiple barcodes for a product.

Examples:
- retail barcode (EAN13)
- box barcode
- pallet barcode
- internal barcode
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductSupplier
-----------------------------------------------------
Stores supplier relations for a product.

Examples:
- default supplier
- alternative supplier
- supplier product code
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductWarehouse
-----------------------------------------------------
Stores product settings per warehouse.

Examples:
- min/max stock
- reorder level
- preferred warehouse
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: WarehouseLocation
-----------------------------------------------------
Stores internal locations inside a warehouse.

Examples:
- Zone A
- Rack B
- Shelf C
- Bin A-01-03
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: BillOfMaterial
-----------------------------------------------------
Defines product composition.

Examples:
- bicycle consists of frame, wheels and seat
- recipe consists of raw materials
- assembly product
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: BillOfMaterialLine
-----------------------------------------------------
Stores product components of a Bill Of Material.

Examples:
- wheel x 2
- seat x 1
- screw x 12
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: CashAccount
Module: CashAccount
Group: Finance
-----------------------------------------------------
Defines cash accounts used for financial transactions.

Examples:
- Main Cash
- Store Cash
- Petty Cash
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AssetCategory
Module: AssetCategory
Group: Assets

IsLookup
-----------------------------------------------------
Defines fixed asset categories.

Examples:
- Vehicles
- Computers
- Machinery
- Furniture
----------------------------------------------------*/
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL
)

/*---------------------------------------------------
Table: AssetLocation
Module: AssetLocation
Group: Assets

IsLookup
-----------------------------------------------------
Defines asset locations.

Examples:
- Head Office
- Warehouse A
- Production Line 1
- Branch Office
----------------------------------------------------*/
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL 
)

/*---------------------------------------------------
Table: AssetDepreciationMethod
Module: AssetDepreciationMethod
Group: Assets

IsLookup
-----------------------------------------------------
Defines depreciation methods.

Examples:
- Straight Line
- Declining Balance
----------------------------------------------------*/
CREATE TABLE {TableName}
(
    Id                  @NVARCHAR(40) @NOT_NULL primary key,
    Name                @NVARCHAR(96) @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL 
)

/*---------------------------------------------------
Table: FixedAsset
Module: FixedAsset
Group: Assets

-----------------------------------------------------
Defines company fixed assets.

Examples:
- company vehicle
- office computer
- production machine
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AssetAssignment
-----------------------------------------------------
Stores asset assignments.

Examples:
- laptop assigned to employee
- vehicle assigned to manager
- machine assigned to department
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AssetMaintenance
-----------------------------------------------------
Stores maintenance history of an asset.

Examples:
- vehicle service
- machine repair
- computer upgrade
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AssetDocument
-----------------------------------------------------
Stores documents related to an asset.

Examples:
- invoice
- warranty
- manual
- certificate
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: AssetInsurance
-----------------------------------------------------
Stores insurance information of an asset.

Examples:
- vehicle insurance
- equipment insurance
- machinery insurance
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: ProductDimension
Module: ProductDimension
Group: Inventory

IsLookup
-----------------------------------------------------
Defines product dimensions.

Examples:
- Color
- Size
- Material
- Package
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Name @NVARCHAR(96) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL
    )

/*---------------------------------------------------
Table: ProductDimensionValue
-----------------------------------------------------
Defines values of a product dimension.

Examples:
- Black
- XL
- Cotton
- 250g
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ProductDimensionId @NVARCHAR(40) @NOT_NULL,   -- Master

    Name @NVARCHAR(96) @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    FOREIGN KEY (ProductDimensionId) REFERENCES ProductDimension(Id)
    )

 

/*---------------------------------------------------
Table: ProductImage
-----------------------------------------------------
Stores product images.

Examples:
- catalog image
- package image
- technical image
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: ProductAttributeGroup
Module: ProductAttributeGroup
Group: Inventory

IsLookup
-----------------------------------------------------
Defines groups for product attributes.

Examples:
- Technical
- Dimensions
- Performance
- Packaging
- eShop
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Name @NVARCHAR(96) @NOT_NULL,

    DisplayOrder int default 0 @NOT_NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
    )

/*---------------------------------------------------
Table: ProductAttribute
-----------------------------------------------------
Stores product-specific attributes.

Rules:
- each row belongs to one Product
- TypeId defines how TextValue is interpreted
- TextValue always stores the actual value as text
- Integer and Decimal values are validated by TypeId
- Option means TextValue contains one or more option values
- when Option contains multiple values, values are separated by ;
- UnitOfMeasure is optional and mainly useful for numeric values

Examples:
- Color, Option, TextValue = Red
- Available Colors, Option, TextValue = Red;Green;Blue
- Weight, Decimal, TextValue = 12.5, UnitOfMeasure = Kg
- Pieces, Integer, TextValue = 24
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: DocumentType 
Module: DocumentType DocumentTypeDataModule
Group: Documents
IsLookup
FieldGroups: Posting, Cancellation, Output, Appearance, Notes
-----------------------------------------------------
Defines document types and their posting behavior.

A document type controls numbering, posting handlers, stock effects,
financial effects, accounting effects, cancellation behavior, and
optional output templates.

Examples:
    SAL-INV     Sales Invoice
    PUR-INV     Purchase Invoice
    RETAIL      Retail Receipt
    SAL-CREDIT  Sales Credit Note
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: Trade

Module: SalesOrder TradeDataModule
Group: Sales
Form: SalesOrder DataForm TradeItemPage

Module: SalesDeliveryNote TradeDataModule
Group: Sales
Form: SalesDeliveryNote DataForm TradeItemPage

Module: SalesInvoice TradeDataModule
Group: Sales
Form: SalesInvoice DataForm TradeItemPage

Module: SalesCreditNote TradeDataModule
Group: Sales
Form: SalesCreditNote DataForm TradeItemPage

Module: SalesReturn TradeDataModule
Group: Sales
Form: SalesReturn DataForm TradeItemPage

Module: SalesCancellation TradeDataModule
Group: Sales
Form: SalesCancellation DataForm TradeItemPage

Module: PurchaseOrder TradeDataModule
Group: Purchases
Form: PurchaseOrder DataForm TradeItemPage

Module: PurchaseDeliveryNote TradeDataModule
Group: Purchases
Form: PurchaseDeliveryNote DataForm TradeItemPage

Module: PurchaseInvoice TradeDataModule
Group: Purchases
Form: PurchaseInvoice DataForm TradeItemPage

Module: PurchaseCreditNote TradeDataModule
Group: Purchases
Form: PurchaseCreditNote DataForm TradeItemPage

Module: PurchaseReturn TradeDataModule
Group: Purchases
Form: PurchaseReturn DataForm TradeItemPage

Module: PurchaseCancellation TradeDataModule
Group: Purchases
Form: PurchaseCancellation DataForm TradeItemPage

FieldGroups: Dates, Party, Organization, Payment, Billing, Shipping, Relations, Amounts, Status, Audit, Notes
-----------------------------------------------------
Commercial document header.

Used as the shared storage table for sales and purchase documents.

Supported business document types include:
- orders
- delivery notes
- invoices
- credit notes
- returns
- cancellation documents

Each declared module represents a specific business view over this table,
with its own menu command, form, module registration, document types,
validation rules, and posting behavior.

Document-specific behavior is determined by the selected DocumentType and
its associated handler implementation.

Line details are stored in TradeLine.
----------------------------------------------------*/
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

    ExternalRef @NVARCHAR(96) @NULL,                    -- e.g. "Related to Order 123", "Your ref: PO-456"

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



/*---------------------------------------------------
Table: TradeTax
-----------------------------------------------------
Hidden detail table.

Stores VAT summary lines per VAT rate for a Trade document.
Generated and maintained by TradeDataModule.
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: TradeLine
-----------------------------------------------------
Commercial document line.
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: StockTrade 
Module: StockTrade StockTradeDataModule
Group: Inventory
Form: Default
FieldGroups: Warehouses, Dates, Relations, Status, Audit, Notes
-----------------------------------------------------
Warehouse transaction document.

Used for inventory-only operations that do not involve
customers, suppliers, receivables, or payables.

Examples:
- warehouse transfer
- stock count adjustment
- stock write-off
- internal stock correction

Posting this document generates StockMovement rows and updates
inventory balances. It does not represent a commercial transaction.
----------------------------------------------------*/
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




/*---------------------------------------------------
Table: StockTradeLine
-----------------------------------------------------
Stock transaction document line.

Represents the intended stock operation for one product.
Posting StockTradeLine rows produces immutable StockMovement rows.

Examples:
- one transfer line produces one OUT movement and one IN movement
- one write-off line produces one OUT movement
- one positive adjustment line produces one IN movement
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: StockMovement 
Module: StockMovement StockMovementDataModule
Group: Inventory
  
IsReadOnly
NotUiVisible
-----------------------------------------------------
Immutable stock ledger movement.

Produced only by posting business documents:
- Trade / TradeLine for sales and purchases
- StockTrade / StockTradeLine for warehouse operations

Each row represents one physical stock effect in one warehouse.
Quantities are always positive.
Direction is stored separately.
-----------------------------------------------------
The central stock ledger of the ERP system.

All inventory changes are recorded here as immutable movement rows.

StockMovement is the single source of truth for:
- current stock balances
- stock history
- inventory valuation
- stock audits and traceability

Rows are generated only by posting business documents and are never edited or deleted.

Typical sources include:
- sales and purchase documents
- warehouse transfers
- stock adjustments
- write-offs and destructions

Current stock quantities are calculated from StockMovement, not from document tables.  
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: StockBalance 
Module: StockBalance StockBalanceDataModule 
Group: Inventory

IsReadOnly
NotUiVisible
-----------------------------------------------------
Cached current stock balance per Product / Warehouse.

StockMovement remains the single source of truth.
StockBalance is maintained during posting/reversal and can always be rebuilt from StockMovement.
  
Maintains the current on-hand inventory balance per Product and Warehouse.

This table is a performance cache and not the authoritative inventory ledger.

The authoritative source of inventory data is StockMovement. All quantities, costs, and stock valuations originate from StockMovement records.

StockBalance is maintained automatically during document posting and cancellation and may be fully rebuilt at any time from StockMovement.

Users never insert, edit, or delete rows in this table directly.

Used for:
- current stock availability
- inventory valuation
- fast stock queries and reporting

One row exists per Product/Warehouse combination.  
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: StockCount 
Module: StockCount StockCountDataModule
Group: Inventory
FieldGroups: Relations, Audit, Notes
-----------------------------------------------------
Physical inventory count document.

Used to record the results of a physical warehouse inventory count
and reconcile actual quantities against system quantities.

After posting, the document generates StockMovement records for
quantity differences and becomes immutable.

Used for:
- periodic inventory counts
- cycle counts
- stock corrections
- inventory reconciliation
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: StockCountLine
-----------------------------------------------------
Inventory count line.

Stores both the system quantity and the physically counted quantity for a product.

At posting time, the difference between the counted quantity and the system quantity is converted into inventory adjustment StockMovement records.

Positive differences generate inbound movements.
Negative differences generate outbound movements.
  
Inventory count lines represent a snapshot of warehouse stock at the time the count begins.

During the counting process no inventory movements should be posted for the warehouse being counted.

The SystemQuantity value is captured at count time and is not recalculated during posting.

At posting time, the difference between the counted quantity and the recorded system quantity is converted into inventory adjustment StockMovement records.  
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: StockReservation 
Module: StockReservation StockReservationDataModule
Group: Inventory

NotUiVisible
IsReadOnly
-----------------------------------------------------
Represents reserved inventory created by commercial documents, usually sales orders.

A reservation does not move stock and does not create StockMovement records.

It reduces available quantity but leaves on-hand quantity unchanged.

StockReservation is created, updated, released, or cancelled automatically by Trade posting and document conversion flows.

The authoritative source document is traced through SourceModule, SourceTable, SourceId and SourceLineId.

Used for:
- sales order reservations
- available stock calculation
- preventing over-allocation
- linking ordered quantities to later deliveries or invoices
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: FinanceMovement 
Module: FinanceMovement FinanceMovementDataModule
Group: Finance

NotUiVisible
IsReadOnly
-----------------------------------------------------
The central financial ledger of the ERP system.

All cash and bank account changes are recorded here as immutable movement rows.

FinanceMovement is the single source of truth for:
- cash balances
- bank balances
- financial transaction history
- cash flow reporting
- financial audits and traceability

Rows are generated only by posting business documents and are never edited or deleted.

Typical sources include:
- customer receipts
- supplier payments
- cash deposits
- cash withdrawals
- bank transfers
- financial adjustments

A FinanceMovement represents one financial effect on one cash account or one bank account.

Current balances are calculated from FinanceMovement, not from document tables.

Cancellation is performed through reversal movements that preserve the complete audit trail.
  
A financial movement affects exactly one financial account.

A movement may belong either to a CashAccount or to a CompanyBankAccount, but never to both simultaneously.

This rule ensures that every movement has a single financial destination and prevents double counting of financial balances.

Transfers between cash and bank accounts are represented by separate movement rows, one for each affected account.  
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: FinanceBalance
Module: FinanceBalance FinanceBalanceDataModule
Group: Finance

NotUiVisible
IsReadOnly
-----------------------------------------------------
Maintains the current balance of cash and bank accounts.

This table is a performance cache and not the authoritative financial ledger.

The authoritative source of financial data is FinanceMovement. All balances, cash flow information, and financial history originate from FinanceMovement records.

FinanceBalance is maintained automatically during document posting and cancellation and may be fully rebuilt at any time from FinanceMovement.

Users never insert, edit, or delete rows in this table directly.

Used for:
- current cash balances
- current bank balances
- cash availability
- fast financial queries and reporting

One row exists per financial account.
  
A balance row belongs to exactly one financial account.

A row may reference either a CashAccount or a CompanyBankAccount, but never both simultaneously.  
----------------------------------------------------*/
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


/*---------------------------------------------------
Table: Account 
Module: Account AccountDataModule
Group: Accounting
-----------------------------------------------------
Defines general ledger accounts used by the accounting subsystem.

Accounts form the chart of accounts of the company.

Each accounting line references one Account.

Accounts may be organized hierarchically through ParentAccountId.

Only posting accounts should be used in JournalEntryLine records.
Parent/group accounts are used only for structure and reporting.

AccountType defines the accounting nature of the account:
- Asset
- Liability
- Equity
- Revenue
- Expense

NormalBalance defines the natural side of the account:
- Asset and Expense accounts normally have Debit balance
- Liability, Equity and Revenue accounts normally have Credit balance

The accounting subsystem uses accounts to record balanced double-entry transactions.

Examples:
- Cash
- Bank
- Customers
- Suppliers
- Sales Revenue
- Purchase Expenses
- VAT Payable
- VAT Receivable
----------------------------------------------------*/
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




/*---------------------------------------------------
Table: JournalEntry  
Module: JournalEntry JournalEntryDataModule
Group: Accounting
FieldGroups: Source, Document, Relations, Audit, Notes
-----------------------------------------------------
Represents one accounting journal entry.

A journal entry records one balanced double-entry accounting transaction.

Each JournalEntry contains two or more JournalEntryLine records.
The total debit amount of all lines must always equal the total credit amount.

Journal entries may be entered manually or generated automatically by
posting business documents.

Typical sources include:
- sales invoices
- purchase invoices
- customer receipts
- supplier payments
- inventory adjustments
- asset depreciation

After posting, a journal entry becomes immutable.

Corrections are performed through reversal journal entries rather than
editing or deleting posted records.
----------------------------------------------------*/
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

/*---------------------------------------------------
Table: JournalEntryLine
-----------------------------------------------------
Represents one accounting line within a journal entry.

Each line affects exactly one accounting account.

A line may contain either a debit amount or a credit amount, but never both simultaneously.

The sum of all debit lines must equal the sum of all credit lines of the parent JournalEntry.

JournalEntryLine records form the accounting ledger of the ERP system.

Every financial event is ultimately represented by one or more journal entry lines.

Examples:

Sales Invoice
    Customer Account      Debit
    Sales Revenue         Credit
    VAT Payable           Credit

Customer Receipt
    Cash Account          Debit
    Customer Account      Credit

Purchase Invoice
    Expense Account       Debit
    VAT Receivable        Debit
    Supplier Account      Credit

Posted lines are immutable.

Corrections are performed through reversal journal entries rather than direct modification of existing lines.
  
Exactly one of DebitAmount or CreditAmount should be greater than zero.

A journal line should never contain both a debit amount and a credit amount simultaneously.

A line with both amounts equal to zero is considered invalid.  
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: Asset 
Module: Asset AssetDataModule
Group: Assets
FieldGroups: Classification, Acquisition, Depreciation, Supplier, Audit, Notes
-----------------------------------------------------
Represents a fixed asset owned by the company.

Assets are long-term resources used by the business and are subject to
depreciation over their useful life.

Examples:
- vehicles
- computers
- machinery
- furniture
- office equipment

An asset may generate depreciation records during its lifetime and may
eventually be sold, disposed, or scrapped.

The asset module is responsible for tracking:
- acquisition cost
- depreciation
- accumulated depreciation
- book value
- asset lifecycle events

Current BookValue is calculated as:

AcquisitionCost - AccumulatedDepreciation
----------------------------------------------------*/
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



/*---------------------------------------------------
Table: AssetDepreciationLine 
-----------------------------------------------------
Represents one depreciation event of a fixed asset.

Each row records the depreciation amount calculated for a specific accounting period.

Depreciation records are immutable and form the depreciation history of the asset.

Depreciation lines are used to:
- calculate accumulated depreciation
- calculate current book value
- provide auditability of asset valuation
- generate accounting journal entries

One Asset may have many depreciation lines during its useful life.

A depreciation line may be linked to the JournalEntry generated for the same depreciation event.

Corrections are performed through reversal records and reversal journal entries rather than direct modification of existing depreciation records.

The parent Asset stores AccumulatedDepreciation and BookValue as cached current values, but the authoritative depreciation history is this table.
----------------------------------------------------*/
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

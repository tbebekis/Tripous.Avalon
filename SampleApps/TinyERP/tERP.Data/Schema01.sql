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
Table: SYS_STR_RES
Module: ResourceStrings
Group: Setup
-----------------------------------------------------
Application resource strings
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Lang @NVARCHAR(12) @NOT_NULL,           -- e.g. en, el
    ResKey @NVARCHAR(96) @NOT_NULL,
    ResValue @NBLOB_TEXT @NOT_NULL,         -- Memo

    CONSTRAINT UQ_{TableName}_Lang_ResKey UNIQUE (Lang, ResKey)
    )

/*---------------------------------------------------
Table: SYS_APP_USER
Module: AppUser AppUserDataModule
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
    CultureCode @NVARCHAR(16) @NULL,

    Email @NVARCHAR(96) @NULL,
    Phone @NVARCHAR(40) @NULL,

    LastLoginAt @DATE_TIME @NULL,
    PasswordChangedAt @DATE_TIME @NULL,

    IsActive @BOOL default 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_UserName UNIQUE (UserName)
    )

/*---------------------------------------------------
Table: SYS_CONFIG
Module: SysConfig   SysConfigModule
Group: Setup
-----------------------------------------------------
Stores configuration values.

Configuration definitions are registered in code.
This table stores only configuration values.

Values may exist at different scopes:
- System
- Company
- User

The effective value is resolved by the application
using the following order:

User -> Company -> System -> DefaultValue
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    ScopeId int @NOT_NULL,                 -- Enum ConfigScope -- System, Company, User
    OwnerKey @NVARCHAR(96) @NULL,          -- CompanyId, UserName, or empty string for System 

    Name @NVARCHAR(128) @NOT_NULL,         -- ConfigPropertyDef.Name

    Value @NVARCHAR(512) @NULL,            -- scalar values
    TextValue @NBLOB_TEXT @NULL,           -- Memo/Object values

    ModifiedAt @DATE_TIME @NULL,           -- [ReadOnlyUI]
    ModifiedBy @NVARCHAR(40) @NULL,        -- Lookup SYS_APP_USER; [ReadOnlyUI]

    CONSTRAINT UQ_{TableName}_Scope_Owner_Name UNIQUE (ScopeId, OwnerKey, Name),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id)
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

    DiscountCategoryId @NVARCHAR(40) @NULL,         -- Lookup
    CustomerId @NVARCHAR(40) @NULL,                 -- Locator Customer

    ProductId @NVARCHAR(40) @NOT_NULL,              -- Locator Product
    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,        -- Lookup

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

    ResponsiblePersonId @NVARCHAR(40) @NULL,            -- Locator Employee; Group Settings -- person responsible for warehouse

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
    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator Manager  -- responsible person

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

    ManagerPersonId @NVARCHAR(40) @NULL,            -- Locator Manager     -- responsible person

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
    PersonId            @NVARCHAR(40) @NOT_NULL,                -- Master
    AddressTypeId       int @NOT_NULL,                          -- Enum AddressType
    Code                @NVARCHAR(40) @NULL,                    -- Code ADR-XXXXXX

    Name                @NVARCHAR(96) @NULL,
    CountryId           @NVARCHAR(40) @NULL,                    -- Lookup
    Region              @NVARCHAR(96) @NULL,
    City                @NVARCHAR(96) @NULL,
    PostalCode          @NVARCHAR(40) @NULL,

    AddressLine1        @NVARCHAR(96) @NULL,
    AddressLine2        @NVARCHAR(96) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,                      -- LargeMemo 

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
    PersonId            @NVARCHAR(40) @NOT_NULL,                -- Master
    ContactTypeId       @NVARCHAR(40) @NOT_NULL,                -- Lookup

    Name                @NVARCHAR(96) @NOT_NULL,
    JobTitle            @NVARCHAR(96) @NULL,

    Phone               @NVARCHAR(40) @NULL,
    Mobile              @NVARCHAR(40) @NULL,
    Email               @NVARCHAR(96) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,                      -- LargeMemo
 

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
    PersonId            @NVARCHAR(40) @NOT_NULL,                -- Master

    BankId              @NVARCHAR(40) @NOT_NULL,                -- Lookup
    Name                @NVARCHAR(96) @NOT_NULL,

    Iban                @NVARCHAR(40) @NULL,
    SwiftCode           @NVARCHAR(40) @NULL,

    IsDefault           @BOOL default 0 @NOT_NULL,
    IsActive            @BOOL default 1 @NOT_NULL,
    Notes               @NBLOB_TEXT @NULL,                      -- LargeMemo 

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

    SupplierId          @NVARCHAR(40) @NOT_NULL, -- Locator Supplier
    SupplierCode        @NVARCHAR(96) @NULL,     -- TitleKey Supplier Product Code

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
    FixedAssetId        @NVARCHAR(40) @NOT_NULL,                -- Master

    PersonId            @NVARCHAR(40) @NULL,                    -- Locator Employee

    AssignmentDate      @DATE @NULL,
    ReturnDate          @DATE @NULL,

    Notes               @NBLOB_TEXT @NULL,                      -- LargeMemo
 

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

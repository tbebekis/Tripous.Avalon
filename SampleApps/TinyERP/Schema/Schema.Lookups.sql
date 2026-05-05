/* 
CustomerCategory
SupplierCategory
ProductBrand
DiscountCategory
*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name)
)
 
/*
UnitOfMeasure
TaxOffice
Currency
Bank
ExpenseCategory
*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)  

/*
PriceList
PaymentMethod
DocumentType
SalesPerson
Carrier
*/
CREATE TABLE {TableName} (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)

CREATE TABLE Country (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Iso2 @NVARCHAR(2) @NOT_NULL,
    Iso3 @NVARCHAR(3) @NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)

CREATE TABLE Currency (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Symbol @NVARCHAR(8) @NOT_NULL,
    Decimals int default 2 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
)

CREATE TABLE VatRate (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Percent @DECIMAL_(5,2) @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code) 
)

CREATE TABLE PaymentTerm (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,
    Days int @NOT_NULL,
    IsActive @BOOL default 1 @NOT_NULL,
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
) 

CREATE TABLE NumberSeries (
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



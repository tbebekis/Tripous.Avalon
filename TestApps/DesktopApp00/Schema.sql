CREATE TABLE Country (
    Id  @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL
)

CREATE TABLE Category (
    Id @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL
)

CREATE TABLE Product (
    Id @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CategoryId @NVARCHAR(40) @NOT_NULL,
    UnitPrice @DECIMAL @NOT_NULL,
    UnitCost @DECIMAL @NOT_NULL,
    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
)

CREATE TABLE Customer (
    Id @NVARCHAR(40)  @NOT_NULL primary key,
    Name @NVARCHAR(96) @NOT_NULL,
    CountryId @NVARCHAR(40) @NOT_NULL,
    FOREIGN KEY (CountryId) REFERENCES Country(Id)
)

CREATE TABLE Trade (
    Id @NVARCHAR(40)  @NOT_NULL primary key,
    Date @DATE @NOT_NULL,
    CustomerId @NVARCHAR(40) @NOT_NULL,
    TradeType integer @NOT_NULL,
    Status integer @NOT_NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
)

CREATE TABLE TradeLines (
    Id @NVARCHAR(40)  @NOT_NULL primary key,
    TradeId @NVARCHAR(40) @NOT_NULL,
    ProductId @NVARCHAR(40) @NOT_NULL,
    Qty @DECIMAL @NOT_NULL,
    UnitPrice @DECIMAL @NOT_NULL,
    NetAmount @DECIMAL @NOT_NULL,
    TaxPercent @DECIMAL @NOT_NULL,
    TaxAmount @DECIMAL @NOT_NULL,
    LineAmount @DECIMAL @NOT_NULL,
    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
)
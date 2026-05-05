CREATE TABLE Company (
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
    FOREIGN KEY (TaxOfficeId) REFERENCES TaxOffice(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
)

CREATE TABLE CompanyBranch (
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
    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (CountryId) REFERENCES Country(Id),
    CONSTRAINT UQ_CompanyBranch_CompanyId_Code UNIQUE (CompanyId, Code)
)

CREATE TABLE CompanyBankAccount (
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
    FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    CONSTRAINT UQ_CompanyBankAccount_CompanyId_Code UNIQUE (CompanyId, Code)
)
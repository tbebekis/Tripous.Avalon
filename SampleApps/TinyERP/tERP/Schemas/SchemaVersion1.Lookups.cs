namespace tERP;

public partial class SchemaVersion1: SchemaVersionDef
{
    void RegisterStandarLookups()
    {
        string[] Lookups = ["CustomerCategory", "SupplierCategory", "ProductBrand", "DiscountCategory"];
        string[] LookupsWithCode = ["UnitOfMeasure", "TaxOffice", "Bank", "ExpenseCategory" ];
        string[] LookupsWithCodeAndActive = ["PriceList", "PaymentMethod", "DocumentType", "SalesPerson", "Carrier"];
        
        foreach (var LU in Lookups)
            Version.AddLookup(LU);
        
        foreach (var LU in LookupsWithCode)
            Version.AddLookupWithCode(LU);
        
        foreach (var LU in LookupsWithCodeAndActive)
            Version.AddLookupWithCodeAndIsActive(LU);

        Db.LookupTableItemDefs.FindOrAddWithTableRange("Lookups", Lookups);
        Db.LookupTableItemDefs.FindOrAddWithTableRange("LookupsWithCode", LookupsWithCode);
        Db.LookupTableItemDefs.FindOrAddWithTableRange("LookupsWithCodeAndActive", LookupsWithCodeAndActive);
    }

    void RegisterCountry()
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
    void RegisterCurrency()
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
    void RegisterVatRate()
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
    void RegisterPaymentTerm()
    {
        string TableName = "PaymentTerm";
        
        string SqlText = $@"
CREATE TABLE {TableName} (
       Id  @NVARCHAR(40)  @NOT_NULL primary key,
       Code @NVARCHAR(40) @NOT_NULL,
       Name @NVARCHAR(96) @NOT_NULL,
       Days int @NOT_NULL,
       IsActive @BOOL default 1 @NOT_NULL,
       CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),
       CONSTRAINT UQ_{TableName}_Code UNIQUE (Code)
   ) 
";
        Version.AddTable(SqlText);     
    }
    void RegisterNumberSeries()
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
 
    protected override void RegisterLookups()
    {
        // ● standard lookups
        RegisterStandarLookups();
 
        // ● other lookups
        RegisterCountry();
        RegisterCurrency();
        RegisterVatRate();
        RegisterPaymentTerm();
        RegisterNumberSeries();
    }
}
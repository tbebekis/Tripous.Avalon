namespace tERP;

static public partial class AppHost
{
    static void RegisterSchema_01()
    {
        Schema Schema = Schemas.FindOrAdd(Sys.APPLICATION, SysConfig.DefaultConnectionName);
        SchemaVersion SV = Schema.FindOrAdd(1);
        string SqlText;
        string TableName;
 
        // ● standard lookups
        string[] Lookups = ["CustomerCategory", "SupplierCategory", "ProductBrand", "DiscountCategory"];
        string[] LookupsWithCode = ["UnitOfMeasure", "TaxOffice",  "Bank", "ExpenseCategory" ];
        string[] LookupsWithCodeAndActive = ["PriceList", "PaymentMethod", "DocumentType", "SalesPerson", "Carrier"];
        
        foreach (var LU in Lookups)
            AddLookup(SV, LU);
        
        foreach (var LU in LookupsWithCode)
            AddLookupWithCode(SV, LU);
        
        foreach (var LU in LookupsWithCodeAndActive)
            AddLookupWithCodeAndIsActive(SV, LU);
        
        // ● other lookups
        TableName = "Country";
        SqlText = $@"
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
        SV.AddTable(SqlText);
        
   
        TableName = "Currency";
        SqlText = $@"
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
        SV.AddTable(SqlText);        
        
        
        TableName = "VatRate";
        SqlText = $@"
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
        SV.AddTable(SqlText);    
        
        TableName = "PaymentTerm";
        SqlText = $@"
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
        SV.AddTable(SqlText);       
        
        TableName = "NumberSeries";
        SqlText = $@"
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
        SV.AddTable(SqlText);            
        
        
    }
}
namespace DesktopApp;

static public partial class AppHost
{
    static void RegisterSchema_01()
    {
        Schema Schema = Schemas.FindOrAdd(Sys.SYSTEM, SysConfig.DefaultConnectionName);
        SchemaVersion SV = Schema.FindOrAdd(1);
 
        // ● Country
        string SqlText = @"
CREATE TABLE Country (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(96) @NOT_NULL
)
";
        SV.AddTable(SqlText);

        // ● Category
        SqlText = @"
CREATE TABLE Category (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(96) @NOT_NULL
)
";
        SV.AddTable(SqlText);      
        
        // ● Product
        SqlText = @"
CREATE TABLE Product (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(96) @NOT_NULL,
   CategoryId integer @NOT_NULL,
   UnitPrice @DECIMAL @NOT_NULL,
   UnitCost @DECIMAL @NOT_NULL,
   FOREIGN KEY (CategoryId) REFERENCES Category(Id)
)
";
        SV.AddTable(SqlText);   
        
        // ● Customer
        SqlText = @"
CREATE TABLE Customer (
   Id integer @NOT_NULL primary key,
   Name @NVARCHAR(96) @NOT_NULL,
   CountryId integer @NOT_NULL,
   FOREIGN KEY (CountryId) REFERENCES Country(Id)
)
";
        SV.AddTable(SqlText);   
        
        // ● Trade
        SqlText = @"
CREATE TABLE Trade (
   Id integer @NOT_NULL primary key,
   Date @DATE @NOT_NULL,
   CustomerId integer @NOT_NULL,
   TradeType integer @NOT_NULL,
   Status integer @NOT_NULL,
   FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
)
";
        SV.AddTable(SqlText);   
        
        // ● TradeLines
        SqlText = @"
CREATE TABLE TradeLines (
   Id integer @NOT_NULL primary key,
   TradeId integer @NOT_NULL,
   ProductId integer @NOT_NULL,
   Qty @DECIMAL @NOT_NULL,
   UnitPrice @DECIMAL @NOT_NULL,
   NetAmount @DECIMAL @NOT_NULL,
   TaxPercent @DECIMAL @NOT_NULL,
   TaxAmount @DECIMAL @NOT_NULL,
   LineAmount @DECIMAL @NOT_NULL,
   FOREIGN KEY (TradeId) REFERENCES Trade(Id),
   FOREIGN KEY (ProductId) REFERENCES Product(Id)
)
";
        SV.AddTable(SqlText);   
    }
}
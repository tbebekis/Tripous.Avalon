namespace tERP;

static internal partial class AppHost
{
    static readonly Dictionary<string, MemTable> SampleTables = new(StringComparer.OrdinalIgnoreCase);

    static bool CanAdd(string ModuleName, out DataModule Module)
    {
        bool Result = false;
        Module = null;
        ModuleDef ModuleDef = DataRegistry.Modules.Get(ModuleName);
        string TableName = ModuleDef.Table.Name;
        if (Store.TableExists(TableName) && Store.TableIsEmpty(TableName))
        {
            Module = ModuleDef.Create();
            Result = true;
        }

        return Result;
    }
    
    // ● initial data - added after a user decision
    static void Add_CustomerCategory()
    {
        string ModuleName = "CustomerCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
 
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        
        tblSource.CopyColumnsFrom(Module.tblItem); 
        
        tblSource.Rows.Add(Sys.GenId(), "Retail");
        tblSource.Rows.Add(Sys.GenId(), "Wholesale");
        tblSource.Rows.Add(Sys.GenId(), "Corporate");
        tblSource.Rows.Add(Sys.GenId(), "Public Sector");
        tblSource.Rows.Add(Sys.GenId(), "Partner");

        Module.BatchInsert(tblSource);
    }
    static void Add_SupplierCategory()
    {
        string ModuleName = "SupplierCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
 
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        
        tblSource.CopyColumnsFrom(Module.tblItem); 

        tblSource.Rows.Add(Sys.GenId(), "Local Supplier");
        tblSource.Rows.Add(Sys.GenId(), "International Supplier");
        tblSource.Rows.Add(Sys.GenId(), "Manufacturer");
        tblSource.Rows.Add(Sys.GenId(), "Distributor");
        tblSource.Rows.Add(Sys.GenId(), "Service Provider");

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductBrand()
    {
        string ModuleName = "ProductBrand";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "Apex");
        tblSource.Rows.Add(Sys.GenId(), "Nova");
        tblSource.Rows.Add(Sys.GenId(), "Orion");
        tblSource.Rows.Add(Sys.GenId(), "Atlas");
        tblSource.Rows.Add(Sys.GenId(), "Vertex");

        Module.BatchInsert(tblSource);
    }
    static void Add_DiscountCategory()
    {
        string ModuleName = "DiscountCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "Standard");
        tblSource.Rows.Add(Sys.GenId(), "Preferred");
        tblSource.Rows.Add(Sys.GenId(), "Volume");
        tblSource.Rows.Add(Sys.GenId(), "Seasonal");
        tblSource.Rows.Add(Sys.GenId(), "Promotional");

        Module.BatchInsert(tblSource);
    }
    static void Add_UnitOfMeasure()
    {
        string ModuleName = "UnitOfMeasure";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        // UN/CEFACT Recommendation 20
        tblSource.Rows.Add(Sys.GenId(), "H87", "Piece");
        tblSource.Rows.Add(Sys.GenId(), "KGM", "Kilogram");
        tblSource.Rows.Add(Sys.GenId(), "GRM", "Gram");
        tblSource.Rows.Add(Sys.GenId(), "MTR", "Meter");
        tblSource.Rows.Add(Sys.GenId(), "CMT", "Centimeter");
        tblSource.Rows.Add(Sys.GenId(), "LTR", "Liter");
        tblSource.Rows.Add(Sys.GenId(), "MTK", "Square Meter");
        tblSource.Rows.Add(Sys.GenId(), "MTQ", "Cubic Meter");
        tblSource.Rows.Add(Sys.GenId(), "BX", "Box");
        tblSource.Rows.Add(Sys.GenId(), "DZN", "Dozen");

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxOffice()
    {
        string ModuleName = "TaxOffice";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "TAX-001", "Central Tax Office");
        tblSource.Rows.Add(Sys.GenId(), "TAX-002", "North Tax Office");
        tblSource.Rows.Add(Sys.GenId(), "TAX-003", "South Tax Office");
        tblSource.Rows.Add(Sys.GenId(), "TAX-004", "East Tax Office");
        tblSource.Rows.Add(Sys.GenId(), "TAX-005", "West Tax Office");

        Module.BatchInsert(tblSource);
    }
    static void Add_Bank()
    {
        string ModuleName = "Bank";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "BNK-001", "First National Bank");
        tblSource.Rows.Add(Sys.GenId(), "BNK-002", "Central Commercial Bank");
        tblSource.Rows.Add(Sys.GenId(), "BNK-003", "Union Bank");
        tblSource.Rows.Add(Sys.GenId(), "BNK-004", "Metropolitan Bank");
        tblSource.Rows.Add(Sys.GenId(), "BNK-005", "Trust Bank");

        Module.BatchInsert(tblSource);
    }
    static void Add_ExpenseCategory()
    {
        string ModuleName = "ExpenseCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "EXP-001", "Rent");
        tblSource.Rows.Add(Sys.GenId(), "EXP-002", "Utilities");
        tblSource.Rows.Add(Sys.GenId(), "EXP-003", "Office Supplies");
        tblSource.Rows.Add(Sys.GenId(), "EXP-004", "Travel");
        tblSource.Rows.Add(Sys.GenId(), "EXP-005", "Marketing");

        Module.BatchInsert(tblSource);
    }
    static void Add_PaymentMethod()
    {
        string ModuleName = "PaymentMethod";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        // UNTDID 4461 / EN 16931 payment codes.
        tblSource.Rows.Add(Sys.GenId(), "10", "Cash", true);
        tblSource.Rows.Add(Sys.GenId(), "20", "Cheque", true);
        tblSource.Rows.Add(Sys.GenId(), "30", "Credit Transfer", true);
        tblSource.Rows.Add(Sys.GenId(), "42", "Payment To Bank Account", true);
        tblSource.Rows.Add(Sys.GenId(), "48", "Bank Card", true);
        tblSource.Rows.Add(Sys.GenId(), "49", "Direct Debit", true);

        Module.BatchInsert(tblSource);
    }
    static void Add_SalesPerson()
    {
        string ModuleName = "SalesPerson";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "Alex Morgan", true);
        tblSource.Rows.Add(Sys.GenId(), "Maria Taylor", true);
        tblSource.Rows.Add(Sys.GenId(), "Nikos Papadopoulos", true);
        tblSource.Rows.Add(Sys.GenId(), "Sofia Adams", true);
        tblSource.Rows.Add(Sys.GenId(), "George Miller", true);

        Module.BatchInsert(tblSource);
    }
    static void Add_Carrier()
    {
        string ModuleName = "Carrier";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "ROEX", "Road Express", true);
        tblSource.Rows.Add(Sys.GenId(), "CILO", "City Logistics", true);
        tblSource.Rows.Add(Sys.GenId(), "GLFR", "Global Freight", true);
        tblSource.Rows.Add(Sys.GenId(), "AICS", "Air Cargo Services", true);
        tblSource.Rows.Add(Sys.GenId(), "SETR", "Sea Transport", true);

        Module.BatchInsert(tblSource);
    }
    static void Add_Country()
    {
        string ModuleName = "Country";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        // ISO 3166-1
        tblSource.Rows.Add(Sys.GenId(), "GR", "GR", "GRC", "Greece");
        tblSource.Rows.Add(Sys.GenId(), "CY", "CY", "CYP", "Cyprus");
        tblSource.Rows.Add(Sys.GenId(), "DE", "DE", "DEU", "Germany");
        tblSource.Rows.Add(Sys.GenId(), "FR", "FR", "FRA", "France");
        tblSource.Rows.Add(Sys.GenId(), "IT", "IT", "ITA", "Italy");
        tblSource.Rows.Add(Sys.GenId(), "ES", "ES", "ESP", "Spain");
        tblSource.Rows.Add(Sys.GenId(), "PT", "PT", "PRT", "Portugal");
        tblSource.Rows.Add(Sys.GenId(), "NL", "NL", "NLD", "Netherlands");
        tblSource.Rows.Add(Sys.GenId(), "BE", "BE", "BEL", "Belgium");
        tblSource.Rows.Add(Sys.GenId(), "AT", "AT", "AUT", "Austria");
        tblSource.Rows.Add(Sys.GenId(), "SE", "SE", "SWE", "Sweden");
        tblSource.Rows.Add(Sys.GenId(), "DK", "DK", "DNK", "Denmark");
        tblSource.Rows.Add(Sys.GenId(), "FI", "FI", "FIN", "Finland");
        tblSource.Rows.Add(Sys.GenId(), "NO", "NO", "NOR", "Norway");
        tblSource.Rows.Add(Sys.GenId(), "IE", "IE", "IRL", "Ireland");
        tblSource.Rows.Add(Sys.GenId(), "GB", "GB", "GBR", "United Kingdom");
        tblSource.Rows.Add(Sys.GenId(), "CH", "CH", "CHE", "Switzerland");
        tblSource.Rows.Add(Sys.GenId(), "PL", "PL", "POL", "Poland");
        tblSource.Rows.Add(Sys.GenId(), "CZ", "CZ", "CZE", "Czechia");
        tblSource.Rows.Add(Sys.GenId(), "RO", "RO", "ROU", "Romania");
        tblSource.Rows.Add(Sys.GenId(), "BG", "BG", "BGR", "Bulgaria");
        tblSource.Rows.Add(Sys.GenId(), "TR", "TR", "TUR", "Turkey");
        tblSource.Rows.Add(Sys.GenId(), "US", "US", "USA", "United States");
        tblSource.Rows.Add(Sys.GenId(), "CA", "CA", "CAN", "Canada");
        tblSource.Rows.Add(Sys.GenId(), "MX", "MX", "MEX", "Mexico");
        tblSource.Rows.Add(Sys.GenId(), "BR", "BR", "BRA", "Brazil");
        tblSource.Rows.Add(Sys.GenId(), "AR", "AR", "ARG", "Argentina");
        tblSource.Rows.Add(Sys.GenId(), "CL", "CL", "CHL", "Chile");
        tblSource.Rows.Add(Sys.GenId(), "CO", "CO", "COL", "Colombia");
        tblSource.Rows.Add(Sys.GenId(), "PE", "PE", "PER", "Peru");
        tblSource.Rows.Add(Sys.GenId(), "AU", "AU", "AUS", "Australia");
        tblSource.Rows.Add(Sys.GenId(), "NZ", "NZ", "NZL", "New Zealand");
        tblSource.Rows.Add(Sys.GenId(), "CN", "CN", "CHN", "China");
        tblSource.Rows.Add(Sys.GenId(), "JP", "JP", "JPN", "Japan");
        tblSource.Rows.Add(Sys.GenId(), "KR", "KR", "KOR", "South Korea");
        tblSource.Rows.Add(Sys.GenId(), "IN", "IN", "IND", "India");
        tblSource.Rows.Add(Sys.GenId(), "SG", "SG", "SGP", "Singapore");
        tblSource.Rows.Add(Sys.GenId(), "AE", "AE", "ARE", "United Arab Emirates");
        tblSource.Rows.Add(Sys.GenId(), "IL", "IL", "ISR", "Israel");
        tblSource.Rows.Add(Sys.GenId(), "ZA", "ZA", "ZAF", "South Africa");

        Module.BatchInsert(tblSource);
    }
    static void Add_Currency()
    {
        string ModuleName = "Currency";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        // ISO 4217
        tblSource.Rows.Add(Sys.GenId(), "EUR", "Euro", "EUR", 2);
        tblSource.Rows.Add(Sys.GenId(), "USD", "US Dollar", "USD", 2);
        tblSource.Rows.Add(Sys.GenId(), "GBP", "Pound Sterling", "GBP", 2);
        tblSource.Rows.Add(Sys.GenId(), "CHF", "Swiss Franc", "CHF", 2);
        tblSource.Rows.Add(Sys.GenId(), "SEK", "Swedish Krona", "SEK", 2);
        tblSource.Rows.Add(Sys.GenId(), "NOK", "Norwegian Krone", "NOK", 2);
        tblSource.Rows.Add(Sys.GenId(), "DKK", "Danish Krone", "DKK", 2);
        tblSource.Rows.Add(Sys.GenId(), "PLN", "Polish Zloty", "PLN", 2);
        tblSource.Rows.Add(Sys.GenId(), "CZK", "Czech Koruna", "CZK", 2);
        tblSource.Rows.Add(Sys.GenId(), "RON", "Romanian Leu", "RON", 2);
        tblSource.Rows.Add(Sys.GenId(), "BGN", "Bulgarian Lev", "BGN", 2);
        tblSource.Rows.Add(Sys.GenId(), "TRY", "Turkish Lira", "TRY", 2);
        tblSource.Rows.Add(Sys.GenId(), "CAD", "Canadian Dollar", "CAD", 2);
        tblSource.Rows.Add(Sys.GenId(), "MXN", "Mexican Peso", "MXN", 2);
        tblSource.Rows.Add(Sys.GenId(), "BRL", "Brazilian Real", "BRL", 2);
        tblSource.Rows.Add(Sys.GenId(), "ARS", "Argentine Peso", "ARS", 2);
        tblSource.Rows.Add(Sys.GenId(), "CLP", "Chilean Peso", "CLP", 0);
        tblSource.Rows.Add(Sys.GenId(), "COP", "Colombian Peso", "COP", 2);
        tblSource.Rows.Add(Sys.GenId(), "PEN", "Peruvian Sol", "PEN", 2);
        tblSource.Rows.Add(Sys.GenId(), "AUD", "Australian Dollar", "AUD", 2);
        tblSource.Rows.Add(Sys.GenId(), "NZD", "New Zealand Dollar", "NZD", 2);
        tblSource.Rows.Add(Sys.GenId(), "CNY", "Yuan Renminbi", "CNY", 2);
        tblSource.Rows.Add(Sys.GenId(), "JPY", "Yen", "JPY", 0);
        tblSource.Rows.Add(Sys.GenId(), "KRW", "Won", "KRW", 0);
        tblSource.Rows.Add(Sys.GenId(), "INR", "Indian Rupee", "INR", 2);
        tblSource.Rows.Add(Sys.GenId(), "SGD", "Singapore Dollar", "SGD", 2);
        tblSource.Rows.Add(Sys.GenId(), "AED", "UAE Dirham", "AED", 2);
        tblSource.Rows.Add(Sys.GenId(), "ILS", "New Israeli Sheqel", "ILS", 2);
        tblSource.Rows.Add(Sys.GenId(), "ZAR", "Rand", "ZAR", 2);

        Module.BatchInsert(tblSource);
    }
    static void Add_VatRate()
    {
        string ModuleName = "VatRate";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(Sys.GenId(), "VAT00", "Zero VAT", 0.00m, true);
        tblSource.Rows.Add(Sys.GenId(), "VAT06", "Reduced VAT 6%", 6.00m, true);
        tblSource.Rows.Add(Sys.GenId(), "VAT13", "Reduced VAT 13%", 13.00m, true);
        tblSource.Rows.Add(Sys.GenId(), "VAT17", "Reduced VAT 17%", 17.00m, true);
        tblSource.Rows.Add(Sys.GenId(), "VAT24", "Standard VAT 24%", 24.00m, true);

        Module.BatchInsert(tblSource);
    }
    static void Add_PriceListType()
    {
        string ModuleName = "PriceListType";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblCurrency = SampleTables["Currency"];
        object EurId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];
        object UsdId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("USD"))["Id"];

        tblSource.Rows.Add(Sys.GenId(), "RETAIL", "Retail Prices", EurId, true, true, true, "#2563EB", "ShoppingCart", DBNull.Value);
        tblSource.Rows.Add(Sys.GenId(), "WHOLESALE", "Wholesale Prices", EurId, false, false, true, "#16A34A", "Package", DBNull.Value);
        tblSource.Rows.Add(Sys.GenId(), "EXPORT", "Export Prices", UsdId, false, false, true, "#9333EA", "Globe", DBNull.Value);

        Module.BatchInsert(tblSource);
    }
    
    static public void AddInitialData()
    {
        Add_CustomerCategory();
        Add_SupplierCategory();
        Add_ProductBrand();
        Add_DiscountCategory();
        Add_UnitOfMeasure();
        Add_TaxOffice();
        Add_Bank();
        Add_ExpenseCategory();
        Add_PaymentMethod();
        Add_SalesPerson();
        Add_Carrier();
        Add_Country();
        Add_Currency();
        Add_VatRate();
        Add_PriceListType();
    }
    
    // ● default initial data - added always
    static public Dictionary<string, string> GetCodeProviderPatterns()
    {
        Dictionary<string, string> Result = [];

        Result["Company"] = "XXXXXX";
        Result["CompanyBranch"] = "XXXXXX";
        Result["Product"] = "XXXXXX";
        Result["Project"] = "XXXXXX";
        Result["SalesPerson"] = "XXXXXX";
        Result["Warehouse"] = "XXXXXX";

        return Result;
    }
    static void AddCodeProviderPatterns()
    {
        string TableName = DbConfig.SysNumberSeriesTableName;
        if (Store.TableExists(TableName) && Store.TableIsEmpty(TableName))
        {
            Dictionary<string, string> CodeProviderPatters = GetCodeProviderPatterns();
            CodeProviderEntries.SeedPatterns(CodeProviderPatters);
        }
    }
    static void Add_Company()
    {
        string ModuleName = "Company";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        tblSource.Rows.Add(
            Sys.StandardCompanyGuid,
            "Default",
            "Default",
            "0123456789",
            "",
            "",
            ""
        );

        Module.BatchInsert(tblSource);
    }
    static void AddDefaultInitialData()
    {
        AddCodeProviderPatterns();
        Add_Company();
    }


}
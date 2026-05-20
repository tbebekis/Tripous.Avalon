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
    static DataRow AddRow(MemTable Table, params (string ColumnName, object Value)[] Values)
    {
        DataRow Row = Table.NewRow();
        foreach (var Value in Values)
            Row[Value.ColumnName] = Value.Value;
        Table.Rows.Add(Row);
        return Row;
    }
    
    // ● sample data - added after a user decision
    static void Add_CustomerCategory()
    {
        string ModuleName = "CustomerCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;
 
        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;
        
        tblSource.CopyColumnsFrom(Module.tblItem); 
        
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Retail"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Wholesale"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Corporate"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Public Sector"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Partner"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Local Supplier"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "International Supplier"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Manufacturer"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Distributor"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Service Provider"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Apex"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Nova"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Orion"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Atlas"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Vertex"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Standard"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Preferred"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Volume"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Seasonal"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Promotional"));

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
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "H87"), ("Name", "Piece"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "KGM"), ("Name", "Kilogram"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GRM"), ("Name", "Gram"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MTR"), ("Name", "Meter"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CMT"), ("Name", "Centimeter"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "LTR"), ("Name", "Liter"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MTK"), ("Name", "Square Meter"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MTQ"), ("Name", "Cubic Meter"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BX"), ("Name", "Box"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DZN"), ("Name", "Dozen"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TAX-001"), ("Name", "Central Tax Office"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TAX-002"), ("Name", "North Tax Office"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TAX-003"), ("Name", "South Tax Office"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TAX-004"), ("Name", "East Tax Office"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TAX-005"), ("Name", "West Tax Office"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BNK-001"), ("Name", "First National Bank"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BNK-002"), ("Name", "Central Commercial Bank"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BNK-003"), ("Name", "Union Bank"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BNK-004"), ("Name", "Metropolitan Bank"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BNK-005"), ("Name", "Trust Bank"));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXP-001"), ("Name", "Rent"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXP-002"), ("Name", "Utilities"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXP-003"), ("Name", "Office Supplies"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXP-004"), ("Name", "Travel"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXP-005"), ("Name", "Marketing"));

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
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "10"), ("Name", "Cash"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "20"), ("Name", "Cheque"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "30"), ("Name", "Credit Transfer"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "42"), ("Name", "Payment To Bank Account"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "48"), ("Name", "Bank Card"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "49"), ("Name", "Direct Debit"), ("IsActive", true));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Alex Morgan"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Maria Taylor"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Nikos Papadopoulos"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Sofia Adams"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "George Miller"), ("IsActive", true));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ROEX"), ("Name", "Road Express"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CILO"), ("Name", "City Logistics"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GLFR"), ("Name", "Global Freight"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AICS"), ("Name", "Air Cargo Services"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SETR"), ("Name", "Sea Transport"), ("IsActive", true));

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
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR"), ("Iso2", "GR"), ("Iso3", "GRC"), ("Name", "Greece"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CY"), ("Iso2", "CY"), ("Iso3", "CYP"), ("Name", "Cyprus"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DE"), ("Iso2", "DE"), ("Iso3", "DEU"), ("Name", "Germany"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "FR"), ("Iso2", "FR"), ("Iso3", "FRA"), ("Name", "France"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "IT"), ("Iso2", "IT"), ("Iso3", "ITA"), ("Name", "Italy"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ES"), ("Iso2", "ES"), ("Iso3", "ESP"), ("Name", "Spain"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PT"), ("Iso2", "PT"), ("Iso3", "PRT"), ("Name", "Portugal"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NL"), ("Iso2", "NL"), ("Iso3", "NLD"), ("Name", "Netherlands"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BE"), ("Iso2", "BE"), ("Iso3", "BEL"), ("Name", "Belgium"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AT"), ("Iso2", "AT"), ("Iso3", "AUT"), ("Name", "Austria"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SE"), ("Iso2", "SE"), ("Iso3", "SWE"), ("Name", "Sweden"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DK"), ("Iso2", "DK"), ("Iso3", "DNK"), ("Name", "Denmark"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "FI"), ("Iso2", "FI"), ("Iso3", "FIN"), ("Name", "Finland"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NO"), ("Iso2", "NO"), ("Iso3", "NOR"), ("Name", "Norway"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "IE"), ("Iso2", "IE"), ("Iso3", "IRL"), ("Name", "Ireland"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GB"), ("Iso2", "GB"), ("Iso3", "GBR"), ("Name", "United Kingdom"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CH"), ("Iso2", "CH"), ("Iso3", "CHE"), ("Name", "Switzerland"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PL"), ("Iso2", "PL"), ("Iso3", "POL"), ("Name", "Poland"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CZ"), ("Iso2", "CZ"), ("Iso3", "CZE"), ("Name", "Czechia"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RO"), ("Iso2", "RO"), ("Iso3", "ROU"), ("Name", "Romania"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BG"), ("Iso2", "BG"), ("Iso3", "BGR"), ("Name", "Bulgaria"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TR"), ("Iso2", "TR"), ("Iso3", "TUR"), ("Name", "Turkey"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "US"), ("Iso2", "US"), ("Iso3", "USA"), ("Name", "United States"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CA"), ("Iso2", "CA"), ("Iso3", "CAN"), ("Name", "Canada"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MX"), ("Iso2", "MX"), ("Iso3", "MEX"), ("Name", "Mexico"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BR"), ("Iso2", "BR"), ("Iso3", "BRA"), ("Name", "Brazil"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AR"), ("Iso2", "AR"), ("Iso3", "ARG"), ("Name", "Argentina"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CL"), ("Iso2", "CL"), ("Iso3", "CHL"), ("Name", "Chile"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CO"), ("Iso2", "CO"), ("Iso3", "COL"), ("Name", "Colombia"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PE"), ("Iso2", "PE"), ("Iso3", "PER"), ("Name", "Peru"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AU"), ("Iso2", "AU"), ("Iso3", "AUS"), ("Name", "Australia"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NZ"), ("Iso2", "NZ"), ("Iso3", "NZL"), ("Name", "New Zealand"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CN"), ("Iso2", "CN"), ("Iso3", "CHN"), ("Name", "China"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "JP"), ("Iso2", "JP"), ("Iso3", "JPN"), ("Name", "Japan"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "KR"), ("Iso2", "KR"), ("Iso3", "KOR"), ("Name", "South Korea"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "IN"), ("Iso2", "IN"), ("Iso3", "IND"), ("Name", "India"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SG"), ("Iso2", "SG"), ("Iso3", "SGP"), ("Name", "Singapore"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AE"), ("Iso2", "AE"), ("Iso3", "ARE"), ("Name", "United Arab Emirates"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "IL"), ("Iso2", "IL"), ("Iso3", "ISR"), ("Name", "Israel"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ZA"), ("Iso2", "ZA"), ("Iso3", "ZAF"), ("Name", "South Africa"));

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
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EUR"), ("Name", "Euro"), ("Symbol", "EUR"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "USD"), ("Name", "US Dollar"), ("Symbol", "USD"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GBP"), ("Name", "Pound Sterling"), ("Symbol", "GBP"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CHF"), ("Name", "Swiss Franc"), ("Symbol", "CHF"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SEK"), ("Name", "Swedish Krona"), ("Symbol", "SEK"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NOK"), ("Name", "Norwegian Krone"), ("Symbol", "NOK"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DKK"), ("Name", "Danish Krone"), ("Symbol", "DKK"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PLN"), ("Name", "Polish Zloty"), ("Symbol", "PLN"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CZK"), ("Name", "Czech Koruna"), ("Symbol", "CZK"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RON"), ("Name", "Romanian Leu"), ("Symbol", "RON"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BGN"), ("Name", "Bulgarian Lev"), ("Symbol", "BGN"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "TRY"), ("Name", "Turkish Lira"), ("Symbol", "TRY"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CAD"), ("Name", "Canadian Dollar"), ("Symbol", "CAD"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MXN"), ("Name", "Mexican Peso"), ("Symbol", "MXN"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "BRL"), ("Name", "Brazilian Real"), ("Symbol", "BRL"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ARS"), ("Name", "Argentine Peso"), ("Symbol", "ARS"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CLP"), ("Name", "Chilean Peso"), ("Symbol", "CLP"), ("Decimals", 0));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "COP"), ("Name", "Colombian Peso"), ("Symbol", "COP"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PEN"), ("Name", "Peruvian Sol"), ("Symbol", "PEN"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AUD"), ("Name", "Australian Dollar"), ("Symbol", "AUD"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NZD"), ("Name", "New Zealand Dollar"), ("Symbol", "NZD"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CNY"), ("Name", "Yuan Renminbi"), ("Symbol", "CNY"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "JPY"), ("Name", "Yen"), ("Symbol", "JPY"), ("Decimals", 0));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "KRW"), ("Name", "Won"), ("Symbol", "KRW"), ("Decimals", 0));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "INR"), ("Name", "Indian Rupee"), ("Symbol", "INR"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SGD"), ("Name", "Singapore Dollar"), ("Symbol", "SGD"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AED"), ("Name", "UAE Dirham"), ("Symbol", "AED"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ILS"), ("Name", "New Israeli Sheqel"), ("Symbol", "ILS"), ("Decimals", 2));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ZAR"), ("Name", "Rand"), ("Symbol", "ZAR"), ("Decimals", 2));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT00"), ("Name", "Zero VAT"), ("Percent", 0.00m), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT06"), ("Name", "Reduced VAT 6%"), ("Percent", 6.00m), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT13"), ("Name", "Reduced VAT 13%"), ("Percent", 13.00m), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT17"), ("Name", "Reduced VAT 17%"), ("Percent", 17.00m), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT24"), ("Name", "Standard VAT 24%"), ("Percent", 24.00m), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_PaymentTerm()
    {
        string ModuleName = "PaymentTerm";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CASH"), ("Name", "Cash Payment"), ("Days", 0), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NET30"), ("Name", "30 Days"), ("Days", 30), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "NET60"), ("Name", "60 Days"), ("Days", 60), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductGroup()
    {
        string ModuleName = "ProductGroup";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CONSUMER"), ("Name", "Consumer Products"), ("IsSystem", false), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Package"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXPORT"), ("Name", "Export Products"), ("IsSystem", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Globe"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SEASONAL"), ("Name", "Seasonal Products"), ("IsSystem", false), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "CalendarDays"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_FiscalYear()
    {
        string ModuleName = "FiscalYear";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        int Year = DateTime.Today.Year;
        int PreviousYear = Year - 1;

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", $"FY{PreviousYear}"), ("Name", $"Fiscal Year {PreviousYear}"), ("StartDate", new DateTime(PreviousYear, 1, 1)), ("EndDate", new DateTime(PreviousYear, 12, 31)), ("IsActive", true), ("IsClosed", false), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", $"FY{Year}"), ("Name", $"Fiscal Year {Year}"), ("StartDate", new DateTime(Year, 1, 1)), ("EndDate", new DateTime(Year, 12, 31)), ("IsActive", true), ("IsClosed", false), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_Language()
    {
        string ModuleName = "Language";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EN"), ("Name", "English"), ("CultureName", "en-US"), ("IsDefault", true), ("IsActive", true), ("IsRightToLeft", false), ("Color", "#2563EB"), ("IconName", "Languages"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EL"), ("Name", "Greek"), ("CultureName", "el-GR"), ("IsDefault", false), ("IsActive", true), ("IsRightToLeft", false), ("Color", "#16A34A"), ("IconName", "Languages"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DE"), ("Name", "German"), ("CultureName", "de-DE"), ("IsDefault", false), ("IsActive", true), ("IsRightToLeft", false), ("Color", "#F59E0B"), ("IconName", "Languages"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_PersonRoleType()
    {
        string ModuleName = "PersonRoleType";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUS"), ("Name", "Customer"), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "UserRound"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP"), ("Name", "Supplier"), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Truck"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CAR"), ("Name", "Carrier"), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "PackageCheck"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_StockReason()
    {
        string ModuleName = "StockReason";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ADJUST"), ("Name", "Inventory Adjustment"), ("StockDirection", 0), ("AffectsCost", true), ("RequiresRemarks", true), ("IsSystem", false), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "SlidersHorizontal"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DAMAGE"), ("Name", "Damaged Goods"), ("StockDirection", -1), ("AffectsCost", true), ("RequiresRemarks", true), ("IsSystem", false), ("IsActive", true), ("Color", "#DC2626"), ("IconName", "PackageX"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "LOSS"), ("Name", "Stock Loss"), ("StockDirection", -1), ("AffectsCost", true), ("RequiresRemarks", true), ("IsSystem", false), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "PackageMinus"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RETURN"), ("Name", "Customer Return"), ("StockDirection", 1), ("AffectsCost", false), ("RequiresRemarks", false), ("IsSystem", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Undo2"), ("Remarks", DBNull.Value));

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RETAIL"), ("Name", "Retail Prices"), ("CurrencyId", EurId), ("IsTaxIncluded", true), ("IsDefault", true), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "ShoppingCart"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "WHOLESALE"), ("Name", "Wholesale Prices"), ("CurrencyId", EurId), ("IsTaxIncluded", false), ("IsDefault", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Package"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXPORT"), ("Name", "Export Prices"), ("CurrencyId", UsdId), ("IsTaxIncluded", false), ("IsDefault", false), ("IsActive", true), ("Color", "#9333EA"), ("IconName", "Globe"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxCategory()
    {
        string ModuleName = "TaxCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblVatRate = SampleTables["VatRate"];
        object Vat24Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT24"))["Id"];
        object Vat00Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT00"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DOMESTIC"), ("Name", "Domestic Transactions"), ("VatRateId", Vat24Id), ("IsDomestic", true), ("IsEuropeanUnion", false), ("IsThirdCountry", false), ("IsTaxExempt", false), ("IsReverseCharge", false), ("IsIntrastat", false), ("IsVies", false), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Landmark"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EU"), ("Name", "European Union"), ("VatRateId", Vat24Id), ("IsDomestic", false), ("IsEuropeanUnion", true), ("IsThirdCountry", false), ("IsTaxExempt", false), ("IsReverseCharge", true), ("IsIntrastat", true), ("IsVies", true), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "CircleFlag"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "THIRD"), ("Name", "Third Countries"), ("VatRateId", Vat00Id), ("IsDomestic", false), ("IsEuropeanUnion", false), ("IsThirdCountry", true), ("IsTaxExempt", false), ("IsReverseCharge", false), ("IsIntrastat", false), ("IsVies", false), ("IsActive", true), ("Color", "#9333EA"), ("IconName", "Globe"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXEMPT"), ("Name", "Tax Exempt"), ("VatRateId", Vat00Id), ("IsDomestic", false), ("IsEuropeanUnion", false), ("IsThirdCountry", false), ("IsTaxExempt", true), ("IsReverseCharge", false), ("IsIntrastat", false), ("IsVies", false), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "BadgePercent"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_FiscalPeriod()
    {
        string TableName = "FiscalPeriod";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("FiscalYear").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblFiscalPeriod = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblFiscalPeriod);

        string[] MonthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        MemTable tblFiscalYear = SampleTables["FiscalYear"];

        foreach (DataRow YearRow in tblFiscalYear.Rows)
        {
            object YearId = YearRow["Id"];
            int Year = ((DateTime)YearRow["StartDate"]).Year;

            Module.Edit(YearId);
            tblFiscalPeriod = Module.GetTable(TableName);

            for (int Month = 1; Month <= 12; Month++)
            {
                DateTime StartDate = new(Year, Month, 1);
                DateTime EndDate = StartDate.AddMonths(1).AddDays(-1);
                object Id = Sys.GenId();

                AddRow(tblFiscalPeriod, ("Id", Id), ("YearId", YearId), ("Code", $"FY{Year}-{Month:00}"), ("Name", $"{MonthNames[Month - 1]} {Year}"), ("PeriodNo", Month), ("StartDate", StartDate), ("EndDate", EndDate), ("IsClosed", false), ("Remarks", DBNull.Value));
                AddRow(tblSource, ("Id", Id), ("YearId", YearId), ("Code", $"FY{Year}-{Month:00}"), ("Name", $"{MonthNames[Month - 1]} {Year}"), ("PeriodNo", Month), ("StartDate", StartDate), ("EndDate", EndDate), ("IsClosed", false), ("Remarks", DBNull.Value));
            }

            Module.Commit();
        }
    }
    static void Add_DocumentType()
    {
        string ModuleName = "DocumentType";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        object SalesInvoiceId = Sys.GenId();

        AddRow(tblSource, ("Id", SalesInvoiceId), ("Code", "SAL-INV"), ("Name", "Sales Invoice"), ("TradeTypeId", (int)TradeType.Sales), ("NumberSeriesId", DBNull.Value), ("IsActive", true), ("AffectsStock", true), ("AffectsFinancial", true), ("AffectsAccounting", true), ("StockDirection", -1), ("FinancialDirection", 1), ("AccountingDirection", 0), ("IsCancellation", false), ("TargetDocumentTypeId", DBNull.Value), ("RequiresApproval", false), ("AutoComplete", true), ("Color", "#2563EB"), ("IconName", "FileText"), ("PrintTemplate", "SalesInvoice"), ("ReportName", "SalesInvoice"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PUR-INV"), ("Name", "Purchase Invoice"), ("TradeTypeId", (int)TradeType.Purchases), ("NumberSeriesId", DBNull.Value), ("IsActive", true), ("AffectsStock", true), ("AffectsFinancial", true), ("AffectsAccounting", true), ("StockDirection", 1), ("FinancialDirection", -1), ("AccountingDirection", 0), ("IsCancellation", false), ("TargetDocumentTypeId", DBNull.Value), ("RequiresApproval", false), ("AutoComplete", true), ("Color", "#16A34A"), ("IconName", "FileInput"), ("PrintTemplate", "PurchaseInvoice"), ("ReportName", "PurchaseInvoice"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RETAIL"), ("Name", "Retail Receipt"), ("TradeTypeId", (int)TradeType.Sales), ("NumberSeriesId", DBNull.Value), ("IsActive", true), ("AffectsStock", true), ("AffectsFinancial", true), ("AffectsAccounting", true), ("StockDirection", -1), ("FinancialDirection", 1), ("AccountingDirection", 0), ("IsCancellation", false), ("TargetDocumentTypeId", DBNull.Value), ("RequiresApproval", false), ("AutoComplete", true), ("Color", "#F59E0B"), ("IconName", "Receipt"), ("PrintTemplate", "RetailReceipt"), ("ReportName", "RetailReceipt"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SAL-CREDIT"), ("Name", "Sales Credit Note"), ("TradeTypeId", (int)TradeType.Sales), ("NumberSeriesId", DBNull.Value), ("IsActive", true), ("AffectsStock", true), ("AffectsFinancial", true), ("AffectsAccounting", true), ("StockDirection", 1), ("FinancialDirection", -1), ("AccountingDirection", 0), ("IsCancellation", true), ("TargetDocumentTypeId", SalesInvoiceId), ("RequiresApproval", false), ("AutoComplete", true), ("Color", "#DC2626"), ("IconName", "FileMinus"), ("PrintTemplate", "SalesCreditNote"), ("ReportName", "SalesCreditNote"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_Person()
    {
        string ModuleName = "Person";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblTaxOffice = SampleTables["TaxOffice"];
        MemTable tblCountry = SampleTables["Country"];
        MemTable tblCurrency = SampleTables["Currency"];
        MemTable tblLanguage = SampleTables["Language"];

        object CentralTaxOfficeId = tblTaxOffice.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("TAX-001"))["Id"];
        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object GermanyId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];
        object EuroId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];
        object EnglishId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EN"))["Id"];
        object GreekId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EL"))["Id"];
        object GermanId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-ACME"), ("Name", "Acme Retail SA"), ("Title", "Retail Customer"), ("TaxNumber", "123456789"), ("TaxOfficeId", CentralTaxOfficeId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", "10 Ermou Street"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("Phone", "+30 210 1000001"), ("Mobile", DBNull.Value), ("Email", "info@acmeretail.example"), ("Website", "https://acmeretail.example"), ("ContactPerson", "Maria Antoniou"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Building2"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-NORTH"), ("Name", "Northwind Traders Ltd"), ("Title", "Wholesale Customer"), ("TaxNumber", "987654321"), ("TaxOfficeId", CentralTaxOfficeId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", EnglishId), ("AddressLine1", "25 Kifisias Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "11523"), ("Phone", "+30 210 1000002"), ("Mobile", DBNull.Value), ("Email", "orders@northwind.example"), ("Website", "https://northwind.example"), ("ContactPerson", "Alex Morgan"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Store"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP-HELIOS"), ("Name", "Helios Supplies OE"), ("Title", "Supplier"), ("TaxNumber", "456789123"), ("TaxOfficeId", CentralTaxOfficeId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", "8 Piraeus Street"), ("AddressLine2", DBNull.Value), ("City", "Piraeus"), ("PostalCode", "18531"), ("Phone", "+30 210 1000003"), ("Mobile", DBNull.Value), ("Email", "sales@helios.example"), ("Website", "https://helios.example"), ("ContactPerson", "Nikos Papadopoulos"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "Truck"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP-BERLIN"), ("Name", "Berlin Components GmbH"), ("Title", "International Supplier"), ("TaxNumber", "DE123456789"), ("TaxOfficeId", DBNull.Value), ("CountryId", GermanyId), ("CurrencyId", EuroId), ("LanguageId", GermanId), ("AddressLine1", "42 Alexanderplatz"), ("AddressLine2", DBNull.Value), ("City", "Berlin"), ("PostalCode", "10178"), ("Phone", "+49 30 1000004"), ("Mobile", DBNull.Value), ("Email", "info@berlincomponents.example"), ("Website", "https://berlincomponents.example"), ("ContactPerson", "Hans Becker"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#9333EA"), ("IconName", "Factory"));

        Module.BatchInsert(tblSource);
    }
    static void Add_Category()
    {
        string ModuleName = "Category";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblVatRate = SampleTables["VatRate"];
        object Vat24Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT24"))["Id"];
        object Vat13Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT13"))["Id"];

        object ElectronicsId = Sys.GenId();
        object FoodId = Sys.GenId();

        AddRow(tblSource, ("Id", ElectronicsId), ("ParentId", DBNull.Value), ("Code", "ELEC"), ("Name", "Electronics"), ("LevelNo", 0), ("SortNo", 10), ("VatRateId", Vat24Id), ("RevenueAccount", "70-1000"), ("ExpenseAccount", "20-1000"), ("IsSystem", false), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "MonitorSmartphone"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", ElectronicsId), ("Code", "ELEC-LAP"), ("Name", "Laptops"), ("LevelNo", 1), ("SortNo", 10), ("VatRateId", Vat24Id), ("RevenueAccount", "70-1100"), ("ExpenseAccount", "20-1100"), ("IsSystem", false), ("IsActive", true), ("Color", "#3B82F6"), ("IconName", "Laptop"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", ElectronicsId), ("Code", "ELEC-MON"), ("Name", "Monitors"), ("LevelNo", 1), ("SortNo", 20), ("VatRateId", Vat24Id), ("RevenueAccount", "70-1200"), ("ExpenseAccount", "20-1200"), ("IsSystem", false), ("IsActive", true), ("Color", "#0EA5E9"), ("IconName", "Monitor"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", FoodId), ("ParentId", DBNull.Value), ("Code", "FOOD"), ("Name", "Food"), ("LevelNo", 0), ("SortNo", 20), ("VatRateId", Vat13Id), ("RevenueAccount", "70-2000"), ("ExpenseAccount", "20-2000"), ("IsSystem", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "ShoppingBasket"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", FoodId), ("Code", "FOOD-COF"), ("Name", "Coffee"), ("LevelNo", 1), ("SortNo", 10), ("VatRateId", Vat13Id), ("RevenueAccount", "70-2100"), ("ExpenseAccount", "20-2100"), ("IsSystem", false), ("IsActive", true), ("Color", "#92400E"), ("IconName", "Coffee"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", FoodId), ("Code", "FOOD-DRK"), ("Name", "Drinks"), ("LevelNo", 1), ("SortNo", 20), ("VatRateId", Vat13Id), ("RevenueAccount", "70-2200"), ("ExpenseAccount", "20-2200"), ("IsSystem", false), ("IsActive", true), ("Color", "#06B6D4"), ("IconName", "CupSoda"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_CompanyBranch()
    {
        string TableName = "CompanyBranch";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Company").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblCompanyBranch = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblCompanyBranch);

        MemTable tblCompany = SampleTables["Company"];
        MemTable tblCountry = SampleTables["Country"];

        object CompanyId = tblCompany.Rows[0]["Id"];
        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];

        Module.Edit(CompanyId);
        tblCompanyBranch = Module.GetTable(TableName);

        object Id = Sys.GenId();

        AddRow(tblCompanyBranch, ("Id", Id), ("CompanyId", CompanyId), ("Code", "MAIN"), ("Name", "Head Office"), ("AddressLine1", "1 Central Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("CountryId", GreeceId), ("Phone", "+30 210 1000000"), ("Email", "info@company.example"), ("IsPrimary", true), ("IsActive", true));
        AddRow(tblSource, ("Id", Id), ("CompanyId", CompanyId), ("Code", "MAIN"), ("Name", "Head Office"), ("AddressLine1", "1 Central Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("CountryId", GreeceId), ("Phone", "+30 210 1000000"), ("Email", "info@company.example"), ("IsPrimary", true), ("IsActive", true));

        Module.Commit();
    }
    static void Add_CompanyBankAccount()
    {
        string TableName = "CompanyBankAccount";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Company").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblCompanyBankAccount = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblCompanyBankAccount);

        MemTable tblCompany = SampleTables["Company"];
        MemTable tblCurrency = SampleTables["Currency"];

        object CompanyId = tblCompany.Rows[0]["Id"];
        object EuroId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];

        Module.Edit(CompanyId);
        tblCompanyBankAccount = Module.GetTable(TableName);

        object Id = Sys.GenId();

        AddRow(tblCompanyBankAccount, ("Id", Id), ("CompanyId", CompanyId), ("Code", "MAIN-EUR"), ("Name", "Main EUR Account"), ("BankName", "First National Bank"), ("Iban", "GR1601101250000000012300695"), ("SwiftBic", "ETHNGRAA"), ("CurrencyId", EuroId), ("IsDefault", true), ("IsActive", true));
        AddRow(tblSource, ("Id", Id), ("CompanyId", CompanyId), ("Code", "MAIN-EUR"), ("Name", "Main EUR Account"), ("BankName", "First National Bank"), ("Iban", "GR1601101250000000012300695"), ("SwiftBic", "ETHNGRAA"), ("CurrencyId", EuroId), ("IsDefault", true), ("IsActive", true));

        Module.Commit();
    }
    static void Add_PersonRole()
    {
        string TableName = "PersonRole";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Person").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblPersonRole = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblPersonRole);

        MemTable tblPerson = SampleTables["Person"];
        MemTable tblPersonRoleType = SampleTables["PersonRoleType"];

        object CustomerRoleId = tblPersonRoleType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUS"))["Id"];
        object SupplierRoleId = tblPersonRoleType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SUP"))["Id"];
        object CarrierRoleId = tblPersonRoleType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CAR"))["Id"];

        void AddRole(DataRow PersonRow, object RoleTypeId)
        {
            object PersonId = PersonRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(PersonId);
            tblPersonRole = Module.GetTable(TableName);

            AddRow(tblPersonRole, ("Id", Id), ("PersonId", PersonId), ("RoleTypeId", RoleTypeId), ("IsActive", true), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("Remarks", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("PersonId", PersonId), ("RoleTypeId", RoleTypeId), ("IsActive", true), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("Remarks", DBNull.Value));

            Module.Commit();
        }

        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE")), SupplierRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Berlin Components GmbH")), SupplierRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Berlin Components GmbH")), CarrierRoleId);
    }
    static void Add_CostCenter()
    {
        string ModuleName = "CostCenter";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblPerson = SampleTables["Person"];

        object SalesManagerId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd"))["Id"];
        object SupportManagerId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA"))["Id"];

        object AdministrationId = Sys.GenId();

        AddRow(tblSource, ("Id", AdministrationId), ("Code", "ADM"), ("Name", "Administration"), ("ParentCostCenterId", DBNull.Value), ("ManagerPersonId", DBNull.Value), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#64748B"), ("IconName", "BriefcaseBusiness"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SALES"), ("Name", "Sales Department"), ("ParentCostCenterId", AdministrationId), ("ManagerPersonId", SalesManagerId), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "ChartNoAxesCombined"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PROD"), ("Name", "Production"), ("ParentCostCenterId", AdministrationId), ("ManagerPersonId", DBNull.Value), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Factory"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUPPORT"), ("Name", "Technical Support"), ("ParentCostCenterId", AdministrationId), ("ManagerPersonId", SupportManagerId), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "Headset"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_Product()
    {
        string ModuleName = "Product";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblCategory = SampleTables["Category"];
        MemTable tblVatRate = SampleTables["VatRate"];
        MemTable tblUnitOfMeasure = SampleTables["UnitOfMeasure"];

        object CoffeeCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-COF"))["Id"];
        object DrinksCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-DRK"))["Id"];
        object Vat24Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT24"))["Id"];
        object Vat13Id = tblVatRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT13"))["Id"];
        object PieceId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("H87"))["Id"];
        object KilogramId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("KGM"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Coffee Machine"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", DrinksCategoryId), ("VatRateId", Vat24Id), ("PrimaryUnitOfMeasureId", PieceId), ("Barcode", "5200000000011"), ("Weight", 6.500m), ("Volume", 0.045m), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Coffee"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Espresso Beans"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", CoffeeCategoryId), ("VatRateId", Vat13Id), ("PrimaryUnitOfMeasureId", KilogramId), ("Barcode", "5200000000028"), ("Weight", 1.000m), ("Volume", 0.004m), ("IsActive", true), ("Color", "#92400E"), ("IconName", "Bean"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_PriceList()
    {
        string ModuleName = "PriceList";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblPriceListType = SampleTables["PriceListType"];
        MemTable tblDiscountCategory = SampleTables["DiscountCategory"];
        MemTable tblPerson = SampleTables["Person"];
        MemTable tblProduct = SampleTables["Product"];
        MemTable tblUnitOfMeasure = SampleTables["UnitOfMeasure"];

        object RetailPriceListTypeId = tblPriceListType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("RETAIL"))["Id"];
        object WholesalePriceListTypeId = tblPriceListType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("WHOLESALE"))["Id"];
        object StandardDiscountCategoryId = tblDiscountCategory.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Standard"))["Id"];
        object PreferredDiscountCategoryId = tblDiscountCategory.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Preferred"))["Id"];
        object AcmeRetailId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA"))["Id"];
        object CoffeeMachineId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"))["Id"];
        object EspressoBeansId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans"))["Id"];
        object PieceId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("H87"))["Id"];
        object KilogramId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("KGM"))["Id"];

        DateTime ValidFrom = new(DateTime.Today.Year, 1, 1);

        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", CoffeeMachineId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 249.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", EspressoBeansId), ("UnitOfMeasureId", KilogramId), ("MinQuantity", 1.0000m), ("UnitPrice", 18.5000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", CoffeeMachineId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 5.0000m), ("UnitPrice", 219.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", EspressoBeansId), ("UnitOfMeasureId", KilogramId), ("MinQuantity", 10.0000m), ("UnitPrice", 15.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductGroups()
    {
        string TableName = "ProductGroups";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductGroups = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductGroups);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblProductGroup = SampleTables["ProductGroup"];

        object ConsumerGroupId = tblProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CONSUMER"))["Id"];
        object SeasonalGroupId = tblProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SEASONAL"))["Id"];

        void AddGroup(DataRow ProductRow, object GroupId)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductGroups = Module.GetTable(TableName);

            AddRow(tblProductGroups, ("Id", Id), ("ProductId", ProductId), ("GroupId", GroupId), ("Remarks", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("GroupId", GroupId), ("Remarks", DBNull.Value));

            Module.Commit();
        }

        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));

        AddGroup(CoffeeMachineRow, ConsumerGroupId);
        AddGroup(CoffeeMachineRow, SeasonalGroupId);
    }
    static void Add_Warehouse()
    {
        string ModuleName = "Warehouse";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblCompany = SampleTables["Company"];
        MemTable tblCompanyBranch = SampleTables["CompanyBranch"];
        MemTable tblCountry = SampleTables["Country"];
        MemTable tblPerson = SampleTables["Person"];

        object CompanyId = tblCompany.Rows[0]["Id"];
        object BranchId = tblCompanyBranch.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("MAIN"))["Id"];
        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object ResponsiblePersonId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Main Warehouse"), ("CompanyId", CompanyId), ("BranchId", BranchId), ("WarehouseTypeId", (int)WarehouseType.Main), ("AddressLine1", "1 Central Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("CountryId", GreeceId), ("Phone", "+30 210 1000100"), ("Email", "warehouse@company.example"), ("ResponsiblePersonId", ResponsiblePersonId), ("IsActive", true), ("IsVirtual", false), ("AllowNegativeStock", false), ("AffectsAvailability", true), ("Color", "#2563EB"), ("IconName", "Warehouse"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Retail Store"), ("CompanyId", CompanyId), ("BranchId", BranchId), ("WarehouseTypeId", (int)WarehouseType.Store), ("AddressLine1", "25 Ermou Street"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("CountryId", GreeceId), ("Phone", "+30 210 1000101"), ("Email", "store01@company.example"), ("ResponsiblePersonId", ResponsiblePersonId), ("IsActive", true), ("IsVirtual", false), ("AllowNegativeStock", false), ("AffectsAvailability", true), ("Color", "#16A34A"), ("IconName", "Store"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Goods In Transit"), ("CompanyId", CompanyId), ("BranchId", DBNull.Value), ("WarehouseTypeId", (int)WarehouseType.Transit), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", DBNull.Value), ("PostalCode", DBNull.Value), ("CountryId", GreeceId), ("Phone", DBNull.Value), ("Email", DBNull.Value), ("ResponsiblePersonId", ResponsiblePersonId), ("IsActive", true), ("IsVirtual", true), ("AllowNegativeStock", false), ("AffectsAvailability", true), ("Color", "#F59E0B"), ("IconName", "Truck"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Scrap / Damaged Stock"), ("CompanyId", CompanyId), ("BranchId", BranchId), ("WarehouseTypeId", (int)WarehouseType.Scrap), ("AddressLine1", "1 Central Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("CountryId", GreeceId), ("Phone", DBNull.Value), ("Email", DBNull.Value), ("ResponsiblePersonId", ResponsiblePersonId), ("IsActive", true), ("IsVirtual", false), ("AllowNegativeStock", true), ("AffectsAvailability", false), ("Color", "#DC2626"), ("IconName", "Trash2"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_Project()
    {
        string ModuleName = "Project";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblPerson = SampleTables["Person"];
        MemTable tblCostCenter = SampleTables["CostCenter"];

        object AcmeRetailId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA"))["Id"];
        object NorthwindId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd"))["Id"];
        object SalesCostCenterId = tblCostCenter.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SALES"))["Id"];
        object SupportCostCenterId = tblCostCenter.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SUPPORT"))["Id"];

        DateTime StartDate = new(DateTime.Today.Year, 1, 1);
        DateTime EndDate = new(DateTime.Today.Year, 12, 31);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "ERP Installation"), ("CustomerId", AcmeRetailId), ("ProjectStatusId", (int)ProjectStatus.Active), ("StartDate", StartDate), ("EndDate", EndDate), ("CostCenterId", SupportCostCenterId), ("ManagerPersonId", NorthwindId), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "BriefcaseBusiness"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "CRM Migration"), ("CustomerId", NorthwindId), ("ProjectStatusId", (int)ProjectStatus.Draft), ("StartDate", StartDate.AddMonths(2)), ("EndDate", DBNull.Value), ("CostCenterId", SalesCostCenterId), ("ManagerPersonId", AcmeRetailId), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "DatabaseZap"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Warehouse Automation"), ("CustomerId", AcmeRetailId), ("ProjectStatusId", (int)ProjectStatus.Active), ("StartDate", StartDate.AddMonths(4)), ("EndDate", DBNull.Value), ("CostCenterId", SupportCostCenterId), ("ManagerPersonId", NorthwindId), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "Warehouse"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductCategory()
    {
        string TableName = "ProductCategory";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductCategory = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductCategory);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblCategory = SampleTables["Category"];

        object CoffeeCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-COF"))["Id"];
        object DrinksCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-DRK"))["Id"];

        void AddCategory(DataRow ProductRow, object CategoryId)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductCategory = Module.GetTable(TableName);

            AddRow(tblProductCategory, ("Id", Id), ("ProductId", ProductId), ("CategoryId", CategoryId), ("IsActive", true));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("CategoryId", CategoryId), ("IsActive", true));

            Module.Commit();
        }

        AddCategory(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine")), DrinksCategoryId);
        AddCategory(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans")), CoffeeCategoryId);
    }
    static void Add_ProductUnitOfMeasure()
    {
        string TableName = "ProductUnitOfMeasure";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductUnitOfMeasure = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductUnitOfMeasure);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblUnitOfMeasure = SampleTables["UnitOfMeasure"];

        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));
        object CoffeeMachineId = CoffeeMachineRow["Id"];
        object PieceId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("H87"))["Id"];
        object BoxId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BX"))["Id"];

        Module.Edit(CoffeeMachineId);
        tblProductUnitOfMeasure = Module.GetTable(TableName);

        object PieceRowId = Sys.GenId();
        object BoxRowId = Sys.GenId();

        AddRow(tblProductUnitOfMeasure, ("Id", PieceRowId), ("ProductId", CoffeeMachineId), ("UnitId", PieceId), ("Ratio", 1.0000m), ("Barcode", "5200000000011"), ("IsSalesDefault", true), ("IsPurchaseDefault", true), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblProductUnitOfMeasure, ("Id", BoxRowId), ("ProductId", CoffeeMachineId), ("UnitId", BoxId), ("Ratio", 12.0000m), ("Barcode", "5200000001018"), ("IsSalesDefault", false), ("IsPurchaseDefault", false), ("IsActive", true), ("Remarks", DBNull.Value));

        AddRow(tblSource, ("Id", PieceRowId), ("ProductId", CoffeeMachineId), ("UnitId", PieceId), ("Ratio", 1.0000m), ("Barcode", "5200000000011"), ("IsSalesDefault", true), ("IsPurchaseDefault", true), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", BoxRowId), ("ProductId", CoffeeMachineId), ("UnitId", BoxId), ("Ratio", 12.0000m), ("Barcode", "5200000001018"), ("IsSalesDefault", false), ("IsPurchaseDefault", false), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.Commit();
    }
    
    static public void AddSampleData()
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
        Add_PaymentTerm();
        Add_ProductGroup();
        Add_FiscalYear();
        Add_Language();
        Add_PersonRoleType();
        Add_StockReason();
        Add_PriceListType();
        Add_TaxCategory();
        Add_FiscalPeriod();
        Add_DocumentType();
        Add_Person();
        Add_Category();
        Add_CompanyBranch();
        Add_CompanyBankAccount();
        Add_PersonRole();
        Add_CostCenter();
        Add_Product();
        Add_PriceList();
        Add_ProductGroups();
        Add_Warehouse();
        Add_Project();
        Add_ProductCategory();
        Add_ProductUnitOfMeasure();
        
        
        
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

        AddRow(tblSource,
            ("Id", Sys.StandardCompanyGuid),
            ("Name", "Default"),
            ("Title", "Default Company"),
            ("TaxNumber", "0123456789"),
            ("TaxOfficeId", DBNull.Value),
            ("CountryId", DBNull.Value),
            ("CurrencyId", DBNull.Value),
            ("AddressLine1", ""),
            ("AddressLine2", ""),
            ("City", ""),
            ("PostalCode", ""),
            ("Phone", ""),
            ("Email", ""),
            ("Website", "")
        );

        Module.BatchInsert(tblSource);
    }
    static void AddDefaultInitialData()
    {
        AddCodeProviderPatterns();
        Add_Company();
    }


}

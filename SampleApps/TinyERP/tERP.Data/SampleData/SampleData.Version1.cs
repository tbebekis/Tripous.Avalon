/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public partial class SampleData1: SampleData
{
    static void Add_Company()
    {
        string ModuleName = "Company";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);
        MemTable tblTaxOffice = SampleTables["TaxOffice"];
        MemTable tblCountry = SampleTables["Country"];
        MemTable tblCurrency = SampleTables["Currency"];
        object TaxOfficeId = tblTaxOffice.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("TAX-001"))["Id"];
        object CountryId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object CurrencyId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];

        AddRow(tblSource,
            ("Id", Sys.StandardCompanyGuid),
            ("Name", "Tripous Demo SA"),
            ("Title", "Tripous Demo Company"),
            ("TaxNumber", "0123456789"),
            ("TaxOfficeId", TaxOfficeId),
            ("CountryId", CountryId),
            ("CurrencyId", CurrencyId),
            ("AddressLine1", "1 Central Avenue"),
            ("AddressLine2", DBNull.Value),
            ("City", "Athens"),
            ("PostalCode", "10563"),
            ("Phone", "+30 210 1000000"),
            ("Email", "info@tripous-demo.example"),
            ("Website", "https://tripous-demo.example")
        );

        Module.BatchInsert(tblSource);
    }
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
    static void Add_TaxRate()
    {
        string ModuleName = "TaxRate";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT00"), ("Name", "Zero VAT"), ("TaxTypeId", (int)TaxType.Vat), ("Percent", 0.00m), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT06"), ("Name", "Reduced VAT 6%"), ("TaxTypeId", (int)TaxType.Vat), ("Percent", 6.00m), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT13"), ("Name", "Reduced VAT 13%"), ("TaxTypeId", (int)TaxType.Vat), ("Percent", 13.00m), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT17"), ("Name", "Reduced VAT 17%"), ("TaxTypeId", (int)TaxType.Vat), ("Percent", 17.00m), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "VAT24"), ("Name", "Standard VAT 24%"), ("TaxTypeId", (int)TaxType.Vat), ("Percent", 24.00m), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "US-CA-0725"), ("Name", "California Sales Tax 7.25%"), ("TaxTypeId", (int)TaxType.SalesTax), ("Percent", 7.25m), ("IsActive", true), ("Remarks", DBNull.Value));

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

        Module.BatchInsert(tblSource);
    }
    static void Add_ResourceStrings()
    {
        string ModuleName = "ResourceStrings";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblLanguage = SampleTables["SYS_LANG"];
        object EnglishId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EN"))["Id"];
        object GreekId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EL"))["Id"];

        (string Key, string English, string Greek)[] Resources = new (string Key, string English, string Greek)[]
        {
            ("ActiveConnectionChangedTo", "Active connection changed to", "Η ενεργή σύνδεση άλλαξε σε"),
            ("AddingSampleDataPleaseWait", "Adding sample data. Please wait...", "Προσθήκη δοκιμαστικών δεδομένων. Παρακαλώ περιμένετε..."),
            ("AffectedRows", "Affected rows", "Επηρεασμένες γραμμές"),
            ("AllPasswordFieldsAreRequired", "All password fields are required.", "Όλα τα πεδία κωδικού πρόσβασης είναι υποχρεωτικά."),
            ("ApplicationSettings", "Application Settings", "Ρυθμίσεις Εφαρμογής"),
            ("ApplicationSettingsFailed", "Application settings failed", "Οι ρυθμίσεις εφαρμογής απέτυχαν"),
            ("ApplicationStarted", "Application Started.", "Η εφαρμογή ξεκίνησε."),
            ("ApplicationWillTerminateRestart", "The application will now terminate. Please restart the application.", "Η εφαρμογή θα τερματιστεί τώρα. Παρακαλώ εκκινήστε την ξανά."),
            ("Calculated", "Calculated", "Υπολογίστηκε"),
            ("Cancel", "Cancel", "Άκυρο"),
            ("CannotCreateServerDialogNoClass", "Cannot create server dialog. No JavaScript class type is specified.", "Δεν είναι δυνατή η δημιουργία server dialog. Δεν έχει οριστεί τύπος κλάσης JavaScript."),
            ("CannotCreateServerDialogNoRoot", "Cannot create server dialog. No root element found.", "Δεν είναι δυνατή η δημιουργία server dialog. Δεν βρέθηκε root element."),
            ("CannotCreateServerDialogNoShowAsync", "Cannot create server dialog. The specified class has no ShowAsync() method.", "Δεν είναι δυνατή η δημιουργία server dialog. Η καθορισμένη κλάση δεν έχει μέθοδο ShowAsync()."),
            ("CashBank", "Cash / Bank", "Ταμείο / Τράπεζα"),
            ("ChangePassword", "Change Password", "Αλλαγή Κωδικού"),
            ("ChangePasswordFailed", "Change password failed", "Η αλλαγή κωδικού απέτυχε"),
            ("CheckingStartupState", "Checking startup state...", "Έλεγχος κατάστασης εκκίνησης..."),
            ("Clear", "Clear", "Καθαρισμός"),
            ("ClearFilters", "Clear Filters", "Καθαρισμός Φίλτρων"),
            ("ClearLog", "Clear Log", "Καθαρισμός Log"),
            ("Close", "Close", "Κλείσιμο"),
            ("CloseFailed", "Close failed", "Το κλείσιμο απέτυχε"),
            ("Collapse", "Collapse", "Σύμπτυξη"),
            ("Columns", "Columns", "Στήλες"),
            ("CommandExecuted", "Command executed", "Η εντολή εκτελέστηκε"),
            ("ConfirmAddSampleDataVersions", "Do you want to add those versions of sample data to the database?", "Θέλετε να προσθέσετε αυτές τις εκδόσεις δοκιμαστικών δεδομένων στη βάση;"),
            ("ConfirmNonSelectSqlExecution", "You are about to execute a non-SELECT SQL statement.", "Πρόκειται να εκτελέσετε SQL εντολή που δεν είναι SELECT."),
            ("ConfirmPassword", "Confirm Password", "Επιβεβαίωση Κωδικού"),
            ("ConfirmRegenerateDatabase", "This will delete and recreate the sample Sqlite database.{0}{0}{1}{0}{0}Continue?", "Αυτό θα διαγράψει και θα δημιουργήσει ξανά τη δοκιμαστική βάση Sqlite.{0}{0}{1}{0}{0}Συνέχεια;"),
            ("ConfirmRegenerateWebDatabase", "This will delete and recreate the sample Sqlite database.", "Αυτό θα διαγράψει και θα δημιουργήσει ξανά τη δοκιμαστική βάση Sqlite."),
            ("Connect", "Connect", "Σύνδεση"),
            ("Connection", "Connection", "Σύνδεση"),
            ("ConnectionFailed", "Connection failed.", "Η σύνδεση απέτυχε."),
            ("ConnectionInfo", "Connection Info", "Πληροφορίες Σύνδεσης"),
            ("ConnectionInfoFailed", "Connection info failed", "Οι πληροφορίες σύνδεσης απέτυχαν"),
            ("ConnectionInformationSaved", "Connection information saved.", "Οι πληροφορίες σύνδεσης αποθηκεύτηκαν."),
            ("ConnectionSucceeded", "Connection succeeded.", "Η σύνδεση πέτυχε."),
            ("Constraints", "Constraints", "Περιορισμοί"),
            ("Continue", "Continue?", "Συνέχεια;"),
            ("Create", "Create", "Δημιουργία"),
            ("CreateA", "Create a", "Δημιουργία"),
            ("CreateCancellationFor", "Create a cancellation for", "Δημιουργία ακυρωτικού για"),
            ("CreateCustomerReceipt", "Create Customer Receipt", "Δημιουργία Είσπραξης Πελάτη"),
            ("CreateCustomerReceiptCancellation", "Create Customer Receipt Cancellation", "Δημιουργία Ακύρωσης Είσπραξης Πελάτη"),
            ("CreateCustomerReceiptCancellationFrom", "Create a Customer Receipt Cancellation from", "Δημιουργία Ακύρωσης Είσπραξης Πελάτη από"),
            ("CreateCustomerReceiptFrom", "Create a Customer Receipt from", "Δημιουργία Είσπραξης Πελάτη από"),
            ("CreatePurchaseCancellation", "Create Purchase Cancellation", "Δημιουργία Ακύρωσης Αγοράς"),
            ("CreatePurchaseCancellationFrom", "Create a Purchase Cancellation from", "Δημιουργία Ακύρωσης Αγοράς από"),
            ("CreatePurchaseCreditNote", "Create Purchase Credit Note", "Δημιουργία Πιστωτικού Αγοράς"),
            ("CreatePurchaseCreditNoteFrom", "Create a Purchase Credit Note from", "Δημιουργία Πιστωτικού Αγοράς από"),
            ("CreatePurchaseDeliveryNote", "Create Purchase Delivery Note", "Δημιουργία Δελτίου Παραλαβής"),
            ("CreatePurchaseDeliveryNoteFailed", "Create Purchase Delivery Note failed", "Η δημιουργία Δελτίου Παραλαβής απέτυχε"),
            ("CreatePurchaseDeliveryNoteFrom", "Create a Purchase Delivery Note from", "Δημιουργία Δελτίου Παραλαβής από"),
            ("CreatePurchaseInvoice", "Create Purchase Invoice", "Δημιουργία Τιμολογίου Αγοράς"),
            ("CreatePurchaseInvoiceFrom", "Create a Purchase Invoice from", "Δημιουργία Τιμολογίου Αγοράς από"),
            ("CreatePurchaseReturn", "Create Purchase Return", "Δημιουργία Επιστροφής Αγοράς"),
            ("CreatePurchaseReturnFrom", "Create a Purchase Return from", "Δημιουργία Επιστροφής Αγοράς από"),
            ("CreateSalesCancellation", "Create Sales Cancellation", "Δημιουργία Ακύρωσης Πώλησης"),
            ("CreateSalesCancellationFrom", "Create a Sales Cancellation from", "Δημιουργία Ακύρωσης Πώλησης από"),
            ("CreateSalesCreditNote", "Create Sales Credit Note", "Δημιουργία Πιστωτικού Πώλησης"),
            ("CreateSalesCreditNoteFrom", "Create a Sales Credit Note from", "Δημιουργία Πιστωτικού Πώλησης από"),
            ("CreateSalesDeliveryNote", "Create Sales Delivery Note", "Δημιουργία Δελτίου Αποστολής"),
            ("CreateSalesDeliveryNoteFailed", "Create Sales Delivery Note failed", "Η δημιουργία Δελτίου Αποστολής απέτυχε"),
            ("CreateSalesDeliveryNoteFrom", "Create a Sales Delivery Note from", "Δημιουργία Δελτίου Αποστολής από"),
            ("CreateSalesInvoice", "Create Sales Invoice", "Δημιουργία Τιμολογίου Πώλησης"),
            ("CreateSalesInvoiceFrom", "Create a Sales Invoice from", "Δημιουργία Τιμολογίου Πώλησης από"),
            ("CreateSalesReturn", "Create Sales Return", "Δημιουργία Επιστροφής Πώλησης"),
            ("CreateSalesReturnFrom", "Create a Sales Return from", "Δημιουργία Επιστροφής Πώλησης από"),
            ("CreateStockCancellation", "Create Stock Cancellation", "Δημιουργία Ακύρωσης Αποθήκης"),
            ("CreateStockCancellationFailed", "Create Stock Cancellation failed", "Η δημιουργία Ακύρωσης Αποθήκης απέτυχε"),
            ("CreateSupplierPayment", "Create Supplier Payment", "Δημιουργία Πληρωμής Προμηθευτή"),
            ("CreateSupplierPaymentCancellation", "Create Supplier Payment Cancellation", "Δημιουργία Ακύρωσης Πληρωμής Προμηθευτή"),
            ("CreateSupplierPaymentCancellationFrom", "Create a Supplier Payment Cancellation from", "Δημιουργία Ακύρωσης Πληρωμής Προμηθευτή από"),
            ("CreateSupplierPaymentFrom", "Create a Supplier Payment from", "Δημιουργία Πληρωμής Προμηθευτή από"),
            ("Created", "Created", "Δημιουργήθηκε"),
            ("CreatedPurchaseDeliveryNoteFrom", "Created Purchase Delivery Note from", "Δημιουργήθηκε Δελτίο Παραλαβής από"),
            ("CreatedSalesDeliveryNoteFrom", "Created Sales Delivery Note from", "Δημιουργήθηκε Δελτίο Αποστολής από"),
            ("CreatedStockCancellationFrom", "Created Stock Cancellation from", "Δημιουργήθηκε Ακύρωση Αποθήκης από"),
            ("CurrentPassword", "Current Password", "Τρέχων Κωδικός"),
            ("CustomerReceipt", "Customer Receipt", "Είσπραξη Πελάτη"),
            ("CustomerReceiptCancellation", "Customer Receipt Cancellation", "Ακύρωση Είσπραξης Πελάτη"),
            ("Dashboard", "Dashboard", "Πίνακας Ελέγχου"),
            ("DashboardRefreshed", "Dashboard refreshed.", "Ο πίνακας ελέγχου ανανεώθηκε."),
            ("DataModuleWasNotReturned", "data module was not returned.", "το data module δεν επιστράφηκε."),
            ("DatabaseConnection", "Database Connection", "Σύνδεση Βάσης"),
            ("DatabaseDeletedApplicationWillTerminate", "The sample Sqlite database has been deleted. The application will now terminate. Please restart the application.", "Η δοκιμαστική βάση Sqlite διαγράφηκε. Η εφαρμογή θα τερματιστεί τώρα. Παρακαλώ εκκινήστε την ξανά."),
            ("DatabaseExplorer", "Database Explorer", "Εξερευνητής Βάσης"),
            ("DatabaseRegenerationOnlySqlite", "Database regeneration is supported only for SQLite connections.", "Η αναδημιουργία βάσης υποστηρίζεται μόνο για συνδέσεις SQLite."),
            ("DatabaseWorkbench", "Database Workbench", "Εργαλεία Βάσης"),
            ("DefaultSqliteConnectionCreated", "A default SQLite connection has been created.{0}{0}{1}", "Δημιουργήθηκε προεπιλεγμένη σύνδεση SQLite.{0}{0}{1}"),
            ("DisableSqlWarningFromSettings", "You can disable this warning from Application Settings by changing ShowWarningOnExecStatements.", "Μπορείτε να απενεργοποιήσετε αυτή την προειδοποίηση από τις Ρυθμίσεις Εφαρμογής αλλάζοντας το ShowWarningOnExecStatements."),
            ("Document", "document", "παραστατικό"),
            ("DocumentCannotBeEditedAfterPosting", "After posting, the document can no longer be edited.", "Μετά την οριστικοποίηση, το παραστατικό δεν μπορεί πλέον να τροποποιηθεί."),
            ("DocumentNotificationFailed", "Document notification failed", "Η ειδοποίηση παραστατικού απέτυχε"),
            ("Done", "DONE", "ΕΤΟΙΜΟ"),
            ("EmptyDatabaseCreatedForConnection", "An empty database has been created for connection '{1}'.{0}{0}{2}", "Δημιουργήθηκε κενή βάση για τη σύνδεση '{1}'.{0}{0}{2}"),
            ("Execute", "Execute", "Εκτέλεση"),
            ("ExecuteF5", "Execute (F5)", "Εκτέλεση (F5)"),
            ("Expand", "Expand", "Ανάπτυξη"),
            ("Failed", "failed", "απέτυχε"),
            ("Filters", "Filters", "Φίλτρα"),
            ("Find", "Find", "Εύρεση"),
            ("FirstApplicationRun", "First Application Run", "Πρώτη Εκκίνηση Εφαρμογής"),
            ("FirstRunSetupIsRequired", "First run setup is required.", "Απαιτείται αρχική ρύθμιση."),
            ("From", "from", "από"),
            ("General", "General", "Γενικά"),
            ("GeneralForms", "General Forms", "Γενικές Φόρμες"),
            ("Indexes", "Indexes", "Ευρετήρια"),
            ("InteractiveSQL", "Interactive SQL", "Interactive SQL"),
            ("InvalidUserNameOrPassword", "Invalid user name or password.", "Λάθος όνομα χρήστη ή κωδικός."),
            ("LoadStartupInfoFailed", "Load startup info failed", "Η φόρτωση startup info απέτυχε"),
            ("LoadWebFormsFailed", "Load web forms failed", "Η φόρτωση web φορμών απέτυχε"),
            ("LoadedWebForms", "Loaded web forms", "Οι web φόρμες φορτώθηκαν"),
            ("LoadingWebForms", "Loading web forms...", "Φόρτωση web φορμών..."),
            ("Log", "Log", "Log"),
            ("LogCleared", "Log cleared.", "Το log καθαρίστηκε."),
            ("LogHidden", "Log hidden.", "Το log αποκρύφθηκε."),
            ("LogSql", "Log Sql", "Log SQL"),
            ("LogSqlFailed", "Log Sql failed", "Το Log SQL απέτυχε"),
            ("LogVisible", "Log visible.", "Το log εμφανίστηκε."),
            ("Login", "Login", "Σύνδεση"),
            ("LoginCancelled", "Login cancelled.", "Η σύνδεση ακυρώθηκε."),
            ("LoginFailed", "Login failed.", "Η σύνδεση απέτυχε."),
            ("LoginIsRequired", "Login is required.", "Απαιτείται σύνδεση."),
            ("LoginSucceeded", "Login succeeded.", "Η σύνδεση πέτυχε."),
            ("MissingSampleDataVersions", "The following versions of sample data are not added to the database yet.", "Οι παρακάτω εκδόσεις δοκιμαστικών δεδομένων δεν έχουν προστεθεί ακόμα στη βάση."),
            ("NewPassword", "New Password", "Νέος Κωδικός"),
            ("Next", "Next", "Επόμενο"),
            ("NoAdminUserTerminating", "No Admin user. Terminating...", "Δεν υπάρχει χρήστης Admin. Τερματισμός..."),
            ("NoConnectionSelected", "No connection selected.", "Δεν έχει επιλεγεί σύνδεση."),
            ("NoTableOrViewSelected", "No table or view selected.", "Δεν έχει επιλεγεί πίνακας ή view."),
            ("NoWebFormNameSpecified", "No WebForm name specified.", "Δεν έχει οριστεί όνομα WebForm."),
            ("NonSelectSqlMayChangeData", "This may change data or database structure. Continue only if you accept responsibility for the result.", "Αυτό μπορεί να αλλάξει δεδομένα ή τη δομή της βάσης. Συνεχίστε μόνο αν αποδέχεστε την ευθύνη για το αποτέλεσμα."),
            ("OK", "OK", "OK"),
            ("OpenDatabaseExplorerFailed", "Open database explorer failed", "Το άνοιγμα του εξερευνητή βάσης απέτυχε"),
            ("OpenSidebarFailed", "Open sidebar failed", "Το άνοιγμα του sidebar απέτυχε"),
            ("OpeningMainPage", "Opening main page...", "Άνοιγμα κύριας σελίδας..."),
            ("Password", "Password", "Κωδικός"),
            ("PasswordChanged", "Password changed.", "Ο κωδικός άλλαξε."),
            ("PasswordChangedForUser", "Password changed for user", "Ο κωδικός άλλαξε για τον χρήστη"),
            ("PasswordFieldsAreRequired", "Password fields are required.", "Τα πεδία κωδικού είναι υποχρεωτικά."),
            ("PasswordsDiffer", "Passwords differ.", "Οι κωδικοί διαφέρουν."),
            ("Payables", "Payables", "Υποχρεώσεις"),
            ("Ping", "Ping", "Ping"),
            ("PingFailed", "Ping failed", "Το ping απέτυχε"),
            ("PingOK", "Ping OK", "Ping OK"),
            ("PingResponse", "Ping response", "Απάντηση ping"),
            ("PingSucceeded", "Ping succeeded.", "Το ping πέτυχε."),
            ("PingingServer", "Pinging server...", "Ping στον server..."),
            ("Post", "Post", "Οριστικοποίηση"),
            ("PostDocument", "Post Document", "Οριστικοποίηση Παραστατικού"),
            ("PostFailed", "Post failed", "Η οριστικοποίηση απέτυχε"),
            ("Posted", "Posted", "Οριστικοποιήθηκε"),
            ("Previous", "Previous", "Προηγούμενο"),
            ("PurchaseCancellation", "Purchase Cancellation", "Ακύρωση Αγοράς"),
            ("PurchaseCreditNote", "Purchase Credit Note", "Πιστωτικό Αγοράς"),
            ("PurchaseDeliveryNote", "Purchase Delivery Note", "Δελτίο Παραλαβής"),
            ("PurchaseDeliveryNoteDataModuleNotReturned", "Purchase Delivery Note data module was not returned.", "Το data module Δελτίου Παραλαβής δεν επιστράφηκε."),
            ("PurchaseInvoice", "Purchase Invoice", "Τιμολόγιο Αγοράς"),
            ("PurchaseInvoiceWithPostedCreditNotesCannotBeCancelled", "A Purchase Invoice with posted Credit Notes cannot be cancelled.", "Τιμολόγιο Αγοράς με οριστικοποιημένα Πιστωτικά δεν μπορεί να ακυρωθεί."),
            ("PurchaseOrder", "Purchase Order", "Παραγγελία Αγοράς"),
            ("PurchaseReturn", "Purchase Return", "Επιστροφή Αγοράς"),
            ("Purchases", "Purchases", "Αγορές"),
            ("ReadOnlyViewSelected", "Read-only view selected", "Επιλέχθηκε read-only view"),
            ("Ready", "Ready", "Έτοιμο"),
            ("Receivables", "Receivables", "Απαιτήσεις"),
            ("Refresh", "Refresh", "Ανανέωση"),
            ("RefreshSkippedUnsavedChanges", "Document changed by another form; refresh is skipped because this form has unsaved changes.", "Το παραστατικό άλλαξε από άλλη φόρμα. Η ανανέωση παραλείφθηκε επειδή αυτή η φόρμα έχει μη αποθηκευμένες αλλαγές."),
            ("RegenerateDatabase", "Regenerate Database", "Αναδημιουργία Βάσης"),
            ("RegenerateDatabaseFailed", "Regenerate database failed", "Η αναδημιουργία βάσης απέτυχε"),
            ("ResourceTranslations", "Resource Translations", "Μεταφράσεις Λεκτικών"),
            ("Result", "Result", "Αποτέλεσμα"),
            ("ReturnedRows", "Returned rows", "Επιστρεφόμενες γραμμές"),
            ("Role", "Role", "Ρόλος"),
            ("Sales", "Sales", "Πωλήσεις"),
            ("SalesCancellation", "Sales Cancellation", "Ακύρωση Πώλησης"),
            ("SalesCreditNote", "Sales Credit Note", "Πιστωτικό Πώλησης"),
            ("SalesDeliveryNote", "Sales Delivery Note", "Δελτίο Αποστολής"),
            ("SalesDeliveryNoteDataModuleNotReturned", "Sales Delivery Note data module was not returned.", "Το data module Δελτίου Αποστολής δεν επιστράφηκε."),
            ("SalesInvoice", "Sales Invoice", "Τιμολόγιο Πώλησης"),
            ("SalesInvoiceWithPostedCreditNotesCannotBeCancelled", "A Sales Invoice with posted Credit Notes cannot be cancelled.", "Τιμολόγιο Πώλησης με οριστικοποιημένα Πιστωτικά δεν μπορεί να ακυρωθεί."),
            ("SalesOrder", "Sales Order", "Παραγγελία Πώλησης"),
            ("SalesReturn", "Sales Return", "Επιστροφή Πώλησης"),
            ("SampleDataAdded", "Sample data added.", "Τα δοκιμαστικά δεδομένα προστέθηκαν."),
            ("SampleDataIsMissing", "Sample data is missing.", "Λείπουν δοκιμαστικά δεδομένα."),
            ("SampleDataWasNotAdded", "Sample data was not added.", "Τα δοκιμαστικά δεδομένα δεν προστέθηκαν."),
            ("Save", "Save", "Αποθήκευση"),
            ("SchemaLoaded", "Schema loaded", "Το schema φορτώθηκε"),
            ("SelectTableOrView", "Select Table Or View", "Επιλογή Πίνακα ή View"),
            ("SetPassword", "Set Password", "Ορισμός Κωδικού"),
            ("SettingsSaved", "Settings saved.", "Οι ρυθμίσεις αποθηκεύτηκαν."),
            ("ShowFieldList", "Show Field List", "Εμφάνιση Λίστας Πεδίων"),
            ("ShowSourceCode", "Show Source Code", "Εμφάνιση Πηγαίου Κώδικα"),
            ("SourceDocumentHasNoRemainingQuantityToCredit", "The source document has no remaining quantity to credit.", "Το αρχικό παραστατικό δεν έχει υπόλοιπη ποσότητα για πίστωση."),
            ("SourceDocumentHasNoRemainingQuantityToInvoice", "The source document has no remaining quantity to invoice.", "Το αρχικό παραστατικό δεν έχει υπόλοιπη ποσότητα για τιμολόγηση."),
            ("SourceDocumentHasNoRemainingQuantityToTransform", "The source document has no remaining quantity to transform.", "Το αρχικό παραστατικό δεν έχει υπόλοιπη ποσότητα για μετασχηματισμό."),
            ("SqlExecutionFailed", "SQL execution failed", "Η εκτέλεση SQL απέτυχε"),
            ("SqlStatementsLoggingChanged", "SQL Statements Logging changed.", "Το SQL Statements Logging άλλαξε."),
            ("SqlStatementsLoggingIsNow", "SQL Statements Logging is now", "Το SQL Statements Logging είναι τώρα"),
            ("Starting", "Starting...", "Εκκίνηση..."),
            ("StartupFailed", "Startup failed", "Η εκκίνηση απέτυχε"),
            ("Statement", "Statement", "Εντολή"),
            ("StockCancellation", "Stock Cancellation", "Ακύρωση Αποθήκης"),
            ("StockCancellationDataModuleNotReturned", "Stock Cancellation data module was not returned.", "Το data module Ακύρωσης Αποθήκης δεν επιστράφηκε."),
            ("StockSnapshot", "Stock Snapshot", "Στιγμιότυπο Αποθήκης"),
            ("StockTransaction", "Stock Transaction", "Κίνηση Αποθήκης"),
            ("StockValue", "Stock Value", "Αξία Αποθέματος"),
            ("SuccessfullyExecuted", "successfully executed", "εκτελέστηκε επιτυχώς"),
            ("SupplierPayment", "Supplier Payment", "Πληρωμή Προμηθευτή"),
            ("SupplierPaymentCancellation", "Supplier Payment Cancellation", "Ακύρωση Πληρωμής Προμηθευτή"),
            ("Tables", "Tables", "Πίνακες"),
            ("TestConnection", "Test Connection", "Δοκιμή Σύνδεσης"),
            ("ToggleFilters", "Toggle Filters", "Εναλλαγή Φίλτρων"),
            ("ToggleLog", "Toggle Log", "Εναλλαγή Log"),
            ("TopCustomers", "Top Customers", "Κορυφαίοι Πελάτες"),
            ("TopSuppliers", "Top Suppliers", "Κορυφαίοι Προμηθευτές"),
            ("Triggers", "Triggers", "Triggers"),
            ("User", "User", "Χρήστης"),
            ("UserAccountIsDisabled", "User account is disabled.", "Ο λογαριασμός χρήστη είναι ανενεργός."),
            ("UserRole", "User Role", "Ρόλος Χρήστη"),
            ("Views", "Views", "Views"),
            ("WebDatabaseDeletedRestartServer", "The sample Sqlite database has been deleted. Restart the tERPWeb server process.", "Η δοκιμαστική βάση Sqlite διαγράφηκε. Επανεκκινήστε τη διεργασία του tERPWeb server."),
            ("WebFormIsNotAvailableYet", "Web form is not available yet", "Η web φόρμα δεν είναι ακόμα διαθέσιμη"),
            ("WebFormNotReturned", "WebForm not returned", "Το WebForm δεν επιστράφηκε"),
            ("WebFormPacketHasNoForm", "WebForm packet has no Form.", "Το WebForm packet δεν έχει Form."),
            ("WebFormRootElementNotFound", "WebForm root element not found", "Το root element του WebForm δεν βρέθηκε")
        };

        foreach ((string Key, string English, string Greek) Resource in Resources)
        {
            AddRow(tblSource, ("Id", Sys.GenId()), ("LanguageId", EnglishId), ("ResKey", Resource.Key), ("ResValue", Resource.English));
            AddRow(tblSource, ("Id", Sys.GenId()), ("LanguageId", GreekId), ("ResKey", Resource.Key), ("ResValue", Resource.Greek));
        }

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

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUS"), ("Name", "Customer"), ("Color", "#2563EB"), ("IconName", "UserRound"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP"), ("Name", "Supplier"), ("Color", "#16A34A"), ("IconName", "Truck"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CAR"), ("Name", "Carrier"), ("Color", "#F59E0B"), ("IconName", "PackageCheck"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP"), ("Name", "Employee"), ("Color", "#0EA5E9"), ("IconName", "UserRoundCheck"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "MGR"), ("Name", "Manager"), ("Color", "#7C3AED"), ("IconName", "UserRoundCog"), ("Remarks", DBNull.Value));

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
    static void Add_ContactType()
    {
        string ModuleName = "ContactType";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Sales"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Accounting"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Technical"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Logistics"), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_AssetCategory()
    {
        string ModuleName = "AssetCategory";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Vehicles"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Computers"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Machinery"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Furniture"), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_AssetLocation()
    {
        string ModuleName = "AssetLocation";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Head Office"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Main Warehouse"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Production Line 1"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Branch Office"), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_AssetDepreciationMethod()
    {
        string ModuleName = "AssetDepreciationMethod";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Straight Line"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Declining Balance"), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductDimension()
    {
        string ModuleName = "ProductDimension";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Color"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Size"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Material"), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Package"), ("IsActive", true));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductAttributeGroup()
    {
        string ModuleName = "ProductAttributeGroup";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Technical"), ("DisplayOrder", 10), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Dimensions"), ("DisplayOrder", 20), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Performance"), ("DisplayOrder", 30), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Packaging"), ("DisplayOrder", 40), ("IsActive", true));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "eShop"), ("DisplayOrder", 50), ("IsActive", true));

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
    static void Add_TaxBusinessGroup()
    {
        string ModuleName = "TaxBusinessGroup";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CONSUMER"), ("Name", "Consumer"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "REGISTERED"), ("Name", "Tax Registered Business"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXEMPT"), ("Name", "Tax Exempt Organization"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "RESELLER"), ("Name", "Reseller"), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxProductGroup()
    {
        string ModuleName = "TaxProductGroup";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "STANDARD"), ("Name", "Standard Taxable Goods"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "REDUCED"), ("Name", "Reduced Rate Goods"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "ZERO"), ("Name", "Zero-Rated Goods"), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXEMPT"), ("Name", "Tax Exempt Goods or Services"), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxJurisdiction()
    {
        string ModuleName = "TaxJurisdiction";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblCountry = SampleTables["Country"];
        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object GermanyId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];
        object UnitedStatesId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US"))["Id"];
        object EuropeanUnionId = Sys.GenId();
        object UnitedStatesJurisdictionId = Sys.GenId();

        AddRow(tblSource, ("Id", EuropeanUnionId), ("ParentId", DBNull.Value), ("CountryId", DBNull.Value), ("Code", "EU"), ("Name", "European Union"), ("JurisdictionTypeId", (int)TaxJurisdictionType.TaxZone), ("RegionCode", DBNull.Value), ("PostalCodePattern", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", EuropeanUnionId), ("CountryId", GreeceId), ("Code", "GR"), ("Name", "Greece"), ("JurisdictionTypeId", (int)TaxJurisdictionType.Country), ("RegionCode", DBNull.Value), ("PostalCodePattern", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", EuropeanUnionId), ("CountryId", GermanyId), ("Code", "DE"), ("Name", "Germany"), ("JurisdictionTypeId", (int)TaxJurisdictionType.Country), ("RegionCode", DBNull.Value), ("PostalCodePattern", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", UnitedStatesJurisdictionId), ("ParentId", DBNull.Value), ("CountryId", UnitedStatesId), ("Code", "US"), ("Name", "United States"), ("JurisdictionTypeId", (int)TaxJurisdictionType.Country), ("RegionCode", DBNull.Value), ("PostalCodePattern", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", UnitedStatesJurisdictionId), ("CountryId", UnitedStatesId), ("Code", "US-CA"), ("Name", "California"), ("JurisdictionTypeId", (int)TaxJurisdictionType.State), ("RegionCode", "CA"), ("PostalCodePattern", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxClause()
    {
        string ModuleName = "TaxClause";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EU-REVERSE"), ("Name", "Intra-Community Reverse Charge"), ("ClauseText", "Intra-Community supply subject to reverse charge."), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXPORT"), ("Name", "Export Outside Tax Territory"), ("ClauseText", "Export transaction taxed at zero rate."), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EXEMPT"), ("Name", "Tax Exemption"), ("ClauseText", "Transaction exempt from indirect tax."), ("IsActive", true), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_TaxRule()
    {
        string ModuleName = "TaxRule";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblBusinessGroup = SampleTables["TaxBusinessGroup"];
        MemTable tblProductGroup = SampleTables["TaxProductGroup"];
        MemTable tblJurisdiction = SampleTables["TaxJurisdiction"];
        MemTable tblTaxRate = SampleTables["TaxRate"];
        MemTable tblTaxClause = SampleTables["TaxClause"];
        object RegisteredId = tblBusinessGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("REGISTERED"))["Id"];
        object ConsumerId = tblBusinessGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CONSUMER"))["Id"];
        object ExemptBusinessId = tblBusinessGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EXEMPT"))["Id"];
        object StandardId = tblProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("STANDARD"))["Id"];
        object ReducedId = tblProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("REDUCED"))["Id"];
        object ZeroId = tblProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("ZERO"))["Id"];
        object GreeceId = tblJurisdiction.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object GermanyId = tblJurisdiction.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];
        object EuropeanUnionId = tblJurisdiction.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EU"))["Id"];
        object UnitedStatesId = tblJurisdiction.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US"))["Id"];
        object CaliforniaId = tblJurisdiction.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US-CA"))["Id"];
        object Vat24Id = tblTaxRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT24"))["Id"];
        object Vat13Id = tblTaxRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT13"))["Id"];
        object Vat00Id = tblTaxRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("VAT00"))["Id"];
        object CaliforniaRateId = tblTaxRate.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US-CA-0725"))["Id"];
        object ReverseChargeId = tblTaxClause.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EU-REVERSE"))["Id"];
        object ExportId = tblTaxClause.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EXPORT"))["Id"];
        object ExemptClauseId = tblTaxClause.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EXEMPT"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-REGISTERED-STANDARD"), ("Name", "Greece Registered Standard VAT"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat24Id), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-REGISTERED-REDUCED"), ("Name", "Greece Registered Reduced VAT"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat13Id), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-CONSUMER-STANDARD"), ("Name", "Greece Consumer Standard VAT"), ("TaxBusinessGroupId", ConsumerId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat24Id), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-CONSUMER-REDUCED"), ("Name", "Greece Consumer Reduced VAT"), ("TaxBusinessGroupId", ConsumerId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat13Id), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-REGISTERED-ZERO"), ("Name", "Greece Registered Zero VAT"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", ZeroId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat00Id), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-EXEMPT-STANDARD"), ("Name", "Greece Exempt Business Standard Goods"), ("TaxBusinessGroupId", ExemptBusinessId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat00Id), ("TaxClauseId", ExemptClauseId), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", true), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-EXEMPT-REDUCED"), ("Name", "Greece Exempt Business Reduced Goods"), ("TaxBusinessGroupId", ExemptBusinessId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat00Id), ("TaxClauseId", ExemptClauseId), ("TradeTypeId", (int)TradeType.None), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", true), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EU-REGISTERED-STANDARD"), ("Name", "EU Registered Reverse Charge"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", EuropeanUnionId), ("TaxRateId", Vat00Id), ("TaxClauseId", ReverseChargeId), ("TradeTypeId", (int)TradeType.Sales), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", true), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EU-REGISTERED-REDUCED"), ("Name", "EU Registered Reduced Reverse Charge"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", EuropeanUnionId), ("TaxRateId", Vat00Id), ("TaxClauseId", ReverseChargeId), ("TradeTypeId", (int)TradeType.Sales), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", true), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DE-GR-REGISTERED-STANDARD"), ("Name", "Germany to Greece Standard Reverse Charge"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GermanyId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat00Id), ("TaxClauseId", ReverseChargeId), ("TradeTypeId", (int)TradeType.Purchases), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", true), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "DE-GR-REGISTERED-REDUCED"), ("Name", "Germany to Greece Reduced Reverse Charge"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GermanyId), ("DestinationTaxJurisdictionId", GreeceId), ("TaxRateId", Vat00Id), ("TaxClauseId", ReverseChargeId), ("TradeTypeId", (int)TradeType.Purchases), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", true), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-US-EXPORT"), ("Name", "Greece Export to United States"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", UnitedStatesId), ("TaxRateId", Vat00Id), ("TaxClauseId", ExportId), ("TradeTypeId", (int)TradeType.Sales), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "GR-US-EXPORT-REDUCED"), ("Name", "Greece Reduced Goods Export to United States"), ("TaxBusinessGroupId", RegisteredId), ("TaxProductGroupId", ReducedId), ("OriginTaxJurisdictionId", GreeceId), ("DestinationTaxJurisdictionId", UnitedStatesId), ("TaxRateId", Vat00Id), ("TaxClauseId", ExportId), ("TradeTypeId", (int)TradeType.Sales), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 80), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "US-CA-CONSUMER-STANDARD"), ("Name", "California Consumer Standard Sales Tax"), ("TaxBusinessGroupId", ConsumerId), ("TaxProductGroupId", StandardId), ("OriginTaxJurisdictionId", CaliforniaId), ("DestinationTaxJurisdictionId", CaliforniaId), ("TaxRateId", CaliforniaRateId), ("TaxClauseId", DBNull.Value), ("TradeTypeId", (int)TradeType.Sales), ("TaxCalculationTypeId", (int)TaxCalculationType.Percentage), ("Priority", 100), ("IsExempt", false), ("IsReverseCharge", false), ("ValidFrom", DBNull.Value), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));

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
        MemTable tblLanguage = SampleTables["SYS_LANG"];
        MemTable tblTaxBusinessGroup = SampleTables["TaxBusinessGroup"];

        object CentralTaxOfficeId = tblTaxOffice.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("TAX-001"))["Id"];
        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object GermanyId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];
        object UnitedStatesId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US"))["Id"];
        object EuroId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];
        object UsDollarId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("USD"))["Id"];
        object EnglishId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EN"))["Id"];
        object GreekId = tblLanguage.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EL"))["Id"];
        object RegisteredId = tblTaxBusinessGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("REGISTERED"))["Id"];
        object ConsumerId = tblTaxBusinessGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CONSUMER"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-ACME"), ("Name", "Acme Retail SA"), ("Title", "Retail Customer"), ("TaxNumber", "123456789"), ("TaxOfficeId", CentralTaxOfficeId), ("TaxBusinessGroupId", RegisteredId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", "10 Ermou Street"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10563"), ("Phone", "+30 210 1000001"), ("Mobile", DBNull.Value), ("Email", "info@acmeretail.example"), ("Website", "https://acmeretail.example"), ("ContactPerson", "Maria Antoniou"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Building2"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-NORTH"), ("Name", "Northwind Traders Ltd"), ("Title", "Wholesale Customer"), ("TaxNumber", "987654321"), ("TaxOfficeId", CentralTaxOfficeId), ("TaxBusinessGroupId", RegisteredId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", EnglishId), ("AddressLine1", "25 Kifisias Avenue"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "11523"), ("Phone", "+30 210 1000002"), ("Mobile", DBNull.Value), ("Email", "orders@northwind.example"), ("Website", "https://northwind.example"), ("ContactPerson", "Alex Morgan"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "Store"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-NIKOS"), ("Name", "Nikos Demo Customer"), ("Title", "Consumer Customer"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("TaxBusinessGroupId", ConsumerId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", "15 Patision Street"), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", "10434"), ("Phone", DBNull.Value), ("Mobile", "+30 694 2000001"), ("Email", "nikos.customer@example.com"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#0EA5E9"), ("IconName", "UserRound"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CUST-LIBERTY"), ("Name", "Liberty Retail LLC"), ("Title", "Export Customer"), ("TaxNumber", "US-99887766"), ("TaxOfficeId", DBNull.Value), ("TaxBusinessGroupId", RegisteredId), ("CountryId", UnitedStatesId), ("CurrencyId", UsDollarId), ("LanguageId", EnglishId), ("AddressLine1", "100 Market Street"), ("AddressLine2", DBNull.Value), ("City", "San Francisco"), ("PostalCode", "94105"), ("Phone", "+1 415 555 0100"), ("Mobile", DBNull.Value), ("Email", "orders@liberty-retail.example"), ("Website", "https://liberty-retail.example"), ("ContactPerson", "John Miller"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#DC2626"), ("IconName", "Globe"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP-HELIOS"), ("Name", "Helios Supplies OE"), ("Title", "Supplier"), ("TaxNumber", "456789123"), ("TaxOfficeId", CentralTaxOfficeId), ("TaxBusinessGroupId", RegisteredId), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", "8 Piraeus Street"), ("AddressLine2", DBNull.Value), ("City", "Piraeus"), ("PostalCode", "18531"), ("Phone", "+30 210 1000003"), ("Mobile", DBNull.Value), ("Email", "sales@helios.example"), ("Website", "https://helios.example"), ("ContactPerson", "Nikos Papadopoulos"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "Truck"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SUP-BERLIN"), ("Name", "Berlin Components GmbH"), ("Title", "International Supplier"), ("TaxNumber", "DE123456789"), ("TaxOfficeId", DBNull.Value), ("TaxBusinessGroupId", RegisteredId), ("CountryId", GermanyId), ("CurrencyId", EuroId), ("LanguageId", EnglishId), ("AddressLine1", "42 Alexanderplatz"), ("AddressLine2", DBNull.Value), ("City", "Berlin"), ("PostalCode", "10178"), ("Phone", "+49 30 1000004"), ("Mobile", DBNull.Value), ("Email", "info@berlincomponents.example"), ("Website", "https://berlincomponents.example"), ("ContactPerson", "Hans Becker"), ("Notes", DBNull.Value), ("IsCompany", true), ("IsActive", true), ("Color", "#9333EA"), ("IconName", "Factory"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP-ELENA"), ("Name", "Elena Papadopoulou"), ("Title", "Sales Manager"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", DBNull.Value), ("Phone", DBNull.Value), ("Mobile", "+30 694 1000001"), ("Email", "elena.papadopoulou@company.example"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#0EA5E9"), ("IconName", "UserRound"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP-DIMITRIS"), ("Name", "Dimitris Nikolaou"), ("Title", "Warehouse Manager"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", "Piraeus"), ("PostalCode", DBNull.Value), ("Phone", DBNull.Value), ("Mobile", "+30 694 1000002"), ("Email", "dimitris.nikolaou@company.example"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#7C3AED"), ("IconName", "UserRound"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP-SOFIA"), ("Name", "Sofia Georgiou"), ("Title", "Accountant"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", DBNull.Value), ("Phone", DBNull.Value), ("Mobile", "+30 694 1000003"), ("Email", "sofia.georgiou@company.example"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "UserRound"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP-ANDREAS"), ("Name", "Andreas Ioannou"), ("Title", "Sales Representative"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", DBNull.Value), ("Phone", DBNull.Value), ("Mobile", "+30 694 1000004"), ("Email", "andreas.ioannou@company.example"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "UserRound"));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "EMP-KATERINA"), ("Name", "Katerina Markou"), ("Title", "Support Specialist"), ("TaxNumber", DBNull.Value), ("TaxOfficeId", DBNull.Value), ("CountryId", GreeceId), ("CurrencyId", EuroId), ("LanguageId", GreekId), ("AddressLine1", DBNull.Value), ("AddressLine2", DBNull.Value), ("City", "Athens"), ("PostalCode", DBNull.Value), ("Phone", DBNull.Value), ("Mobile", "+30 694 1000005"), ("Email", "katerina.markou@company.example"), ("Website", DBNull.Value), ("ContactPerson", DBNull.Value), ("Notes", DBNull.Value), ("IsCompany", false), ("IsActive", true), ("Color", "#DC2626"), ("IconName", "UserRound"));

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

        MemTable tblTaxProductGroup = SampleTables["TaxProductGroup"];
        object StandardId = tblTaxProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("STANDARD"))["Id"];
        object ReducedId = tblTaxProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("REDUCED"))["Id"];

        object ElectronicsId = Sys.GenId();
        object FoodId = Sys.GenId();

        AddRow(tblSource, ("Id", ElectronicsId), ("ParentId", DBNull.Value), ("Code", "ELEC"), ("Name", "Electronics"), ("LevelNo", 0), ("SortNo", 10), ("TaxProductGroupId", StandardId), ("RevenueAccount", "70-1000"), ("ExpenseAccount", "20-1000"), ("IsSystem", false), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "MonitorSmartphone"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", ElectronicsId), ("Code", "ELEC-LAP"), ("Name", "Laptops"), ("LevelNo", 1), ("SortNo", 10), ("TaxProductGroupId", StandardId), ("RevenueAccount", "70-1100"), ("ExpenseAccount", "20-1100"), ("IsSystem", false), ("IsActive", true), ("Color", "#3B82F6"), ("IconName", "Laptop"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", ElectronicsId), ("Code", "ELEC-MON"), ("Name", "Monitors"), ("LevelNo", 1), ("SortNo", 20), ("TaxProductGroupId", StandardId), ("RevenueAccount", "70-1200"), ("ExpenseAccount", "20-1200"), ("IsSystem", false), ("IsActive", true), ("Color", "#0EA5E9"), ("IconName", "Monitor"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", FoodId), ("ParentId", DBNull.Value), ("Code", "FOOD"), ("Name", "Food"), ("LevelNo", 0), ("SortNo", 20), ("TaxProductGroupId", ReducedId), ("RevenueAccount", "70-2000"), ("ExpenseAccount", "20-2000"), ("IsSystem", false), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "ShoppingBasket"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", FoodId), ("Code", "FOOD-COF"), ("Name", "Coffee"), ("LevelNo", 1), ("SortNo", 10), ("TaxProductGroupId", ReducedId), ("RevenueAccount", "70-2100"), ("ExpenseAccount", "20-2100"), ("IsSystem", false), ("IsActive", true), ("Color", "#92400E"), ("IconName", "Coffee"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("ParentId", FoodId), ("Code", "FOOD-DRK"), ("Name", "Drinks"), ("LevelNo", 1), ("SortNo", 20), ("TaxProductGroupId", ReducedId), ("RevenueAccount", "70-2200"), ("ExpenseAccount", "20-2200"), ("IsSystem", false), ("IsActive", true), ("Color", "#06B6D4"), ("IconName", "CupSoda"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_FixedAsset()
    {
        string ModuleName = "FixedAsset";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblAssetCategory = SampleTables["AssetCategory"];
        MemTable tblAssetLocation = SampleTables["AssetLocation"];
        MemTable tblAssetDepreciationMethod = SampleTables["AssetDepreciationMethod"];

        object ComputersId = tblAssetCategory.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Computers"))["Id"];
        object VehiclesId = tblAssetCategory.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Vehicles"))["Id"];
        object HeadOfficeId = tblAssetLocation.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Head Office"))["Id"];
        object WarehouseId = tblAssetLocation.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Main Warehouse"))["Id"];
        object StraightLineId = tblAssetDepreciationMethod.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Straight Line"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AST-LAP-001"), ("Name", "Office Laptop"), ("AssetCategoryId", ComputersId), ("AssetLocationId", HeadOfficeId), ("AssetDepreciationMethodId", StraightLineId), ("PurchaseDate", DateTime.Today.AddMonths(-8)), ("PurchaseValue", 1250.0000m), ("UsefulLifeMonths", 36), ("DepreciationRate", 33.3300m), ("SerialNumber", "LAP-2025-001"), ("Manufacturer", "Apex"), ("Model", "Book Pro 14"), ("IsActive", true), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "AST-VAN-001"), ("Name", "Delivery Van"), ("AssetCategoryId", VehiclesId), ("AssetLocationId", WarehouseId), ("AssetDepreciationMethodId", StraightLineId), ("PurchaseDate", DateTime.Today.AddYears(-1)), ("PurchaseValue", 24500.0000m), ("UsefulLifeMonths", 60), ("DepreciationRate", 20.0000m), ("SerialNumber", "VAN-2024-001"), ("Manufacturer", "Orion"), ("Model", "Cargo 2.0"), ("IsActive", true), ("Notes", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductDimensionValue()
    {
        string TableName = "ProductDimensionValue";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("ProductDimension").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductDimensionValue = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductDimensionValue);

        MemTable tblProductDimension = SampleTables["ProductDimension"];

        void AddValue(DataRow ProductDimensionRow, string Name)
        {
            object ProductDimensionId = ProductDimensionRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductDimensionId);
            tblProductDimensionValue = Module.GetTable(TableName);

            AddRow(tblProductDimensionValue, ("Id", Id), ("ProductDimensionId", ProductDimensionId), ("Name", Name), ("IsActive", true));
            AddRow(tblSource, ("Id", Id), ("ProductDimensionId", ProductDimensionId), ("Name", Name), ("IsActive", true));

            Module.Commit();
        }

        DataRow ColorRow = tblProductDimension.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Color"));
        DataRow SizeRow = tblProductDimension.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Size"));
        DataRow MaterialRow = tblProductDimension.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Material"));
        DataRow PackageRow = tblProductDimension.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Package"));

        AddValue(ColorRow, "Black");
        AddValue(ColorRow, "Silver");
        AddValue(SizeRow, "Small");
        AddValue(SizeRow, "Large");
        AddValue(MaterialRow, "Steel");
        AddValue(MaterialRow, "Plastic");
        AddValue(PackageRow, "Box");
        AddValue(PackageRow, "Bag");
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
        object EmployeeRoleId = tblPersonRoleType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP"))["Id"];
        object ManagerRoleId = tblPersonRoleType.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("MGR"))["Id"];

        void AddRole(DataRow PersonRow, object RoleTypeId)
        {
            object PersonId = PersonRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(PersonId);
            tblPersonRole = Module.GetTable(TableName);

            AddRow(tblPersonRole, ("Id", Id), ("PersonId", PersonId), ("RoleTypeId", RoleTypeId), ("Remarks", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("PersonId", PersonId), ("RoleTypeId", RoleTypeId), ("Remarks", DBNull.Value));

            Module.Commit();
        }

        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-NIKOS")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-LIBERTY")), CustomerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE")), SupplierRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Berlin Components GmbH")), SupplierRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Berlin Components GmbH")), CarrierRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Elena Papadopoulou")), EmployeeRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Elena Papadopoulou")), ManagerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Dimitris Nikolaou")), EmployeeRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Dimitris Nikolaou")), ManagerRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Sofia Georgiou")), EmployeeRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Andreas Ioannou")), EmployeeRoleId);
        AddRole(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Katerina Markou")), EmployeeRoleId);
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

        object SalesManagerId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-ELENA"))["Id"];
        object WarehouseManagerId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-DIMITRIS"))["Id"];
        object SupportManagerId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-KATERINA"))["Id"];

        object AdministrationId = Sys.GenId();

        AddRow(tblSource, ("Id", AdministrationId), ("Code", "ADM"), ("Name", "Administration"), ("ParentCostCenterId", DBNull.Value), ("ManagerPersonId", DBNull.Value), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#64748B"), ("IconName", "BriefcaseBusiness"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "SALES"), ("Name", "Sales Department"), ("ParentCostCenterId", AdministrationId), ("ManagerPersonId", SalesManagerId), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "ChartNoAxesCombined"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "PURCHASES"), ("Name", "Purchasing Department"), ("ParentCostCenterId", AdministrationId), ("ManagerPersonId", WarehouseManagerId), ("StartDate", DateTime.Today), ("EndDate", DBNull.Value), ("IsActive", true), ("Color", "#7C3AED"), ("IconName", "ShoppingBag"), ("Remarks", DBNull.Value));
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
        MemTable tblTaxProductGroup = SampleTables["TaxProductGroup"];
        MemTable tblUnitOfMeasure = SampleTables["UnitOfMeasure"];

        object LaptopCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("ELEC-LAP"))["Id"];
        object MonitorCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("ELEC-MON"))["Id"];
        object CoffeeCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-COF"))["Id"];
        object DrinksCategoryId = tblCategory.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("FOOD-DRK"))["Id"];
        object StandardId = tblTaxProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("STANDARD"))["Id"];
        object ReducedId = tblTaxProductGroup.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("REDUCED"))["Id"];
        object PieceId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("H87"))["Id"];
        object KilogramId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("KGM"))["Id"];
        object LiterId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("LTR"))["Id"];
        object BoxId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BX"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Coffee Machine"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", DrinksCategoryId), ("TaxProductGroupId", StandardId), ("PrimaryUnitOfMeasureId", PieceId), ("Barcode", "5200000000011"), ("Weight", 6.500m), ("Volume", 0.045m), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "Coffee"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Espresso Beans"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", CoffeeCategoryId), ("TaxProductGroupId", ReducedId), ("PrimaryUnitOfMeasureId", KilogramId), ("Barcode", "5200000000028"), ("Weight", 1.000m), ("Volume", 0.004m), ("IsActive", true), ("Color", "#92400E"), ("IconName", "Bean"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Laptop Computer 14"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", LaptopCategoryId), ("TaxProductGroupId", StandardId), ("PrimaryUnitOfMeasureId", PieceId), ("Barcode", "5200000000035"), ("Weight", 1.450m), ("Volume", 0.008m), ("IsActive", true), ("Color", "#3B82F6"), ("IconName", "Laptop"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Monitor 27 Inch"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", MonitorCategoryId), ("TaxProductGroupId", StandardId), ("PrimaryUnitOfMeasureId", PieceId), ("Barcode", "5200000000042"), ("Weight", 5.200m), ("Volume", 0.055m), ("IsActive", true), ("Color", "#0EA5E9"), ("IconName", "Monitor"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Wireless Keyboard"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", MonitorCategoryId), ("TaxProductGroupId", StandardId), ("PrimaryUnitOfMeasureId", PieceId), ("Barcode", "5200000000059"), ("Weight", 0.650m), ("Volume", 0.006m), ("IsActive", true), ("Color", "#64748B"), ("IconName", "Keyboard"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Coffee Capsules"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", CoffeeCategoryId), ("TaxProductGroupId", ReducedId), ("PrimaryUnitOfMeasureId", BoxId), ("Barcode", "5200000000066"), ("Weight", 0.120m), ("Volume", 0.002m), ("IsActive", true), ("Color", "#A16207"), ("IconName", "Package"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Mineral Water"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", DrinksCategoryId), ("TaxProductGroupId", ReducedId), ("PrimaryUnitOfMeasureId", LiterId), ("Barcode", "5200000000073"), ("Weight", 1.000m), ("Volume", 0.001m), ("IsActive", true), ("Color", "#06B6D4"), ("IconName", "Bottle"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Orange Juice"), ("ProductTypeId", (int)ProductType.Goods), ("CategoryId", DrinksCategoryId), ("TaxProductGroupId", ReducedId), ("PrimaryUnitOfMeasureId", LiterId), ("Barcode", "5200000000080"), ("Weight", 1.050m), ("Volume", 0.001m), ("IsActive", true), ("Color", "#F97316"), ("IconName", "CupSoda"), ("Remarks", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_PersonAddress()
    {
        string TableName = "PersonAddress";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Person").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblPersonAddress = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblPersonAddress);

        MemTable tblPerson = SampleTables["Person"];
        MemTable tblCountry = SampleTables["Country"];

        object GreeceId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("GR"))["Id"];
        object GermanyId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("DE"))["Id"];
        object UnitedStatesId = tblCountry.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("US"))["Id"];

        void AddAddress(DataRow PersonRow, int AddressTypeId, string Code, string Name, object CountryId, string City, string PostalCode, string AddressLine1)
        {
            object PersonId = PersonRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(PersonId);
            tblPersonAddress = Module.GetTable(TableName);

            AddRow(tblPersonAddress, ("Id", Id), ("PersonId", PersonId), ("AddressTypeId", AddressTypeId), ("Code", Code), ("Name", Name), ("CountryId", CountryId), ("Region", DBNull.Value), ("City", City), ("PostalCode", PostalCode), ("AddressLine1", AddressLine1), ("AddressLine2", DBNull.Value), ("IsDefault", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("PersonId", PersonId), ("AddressTypeId", AddressTypeId), ("Code", Code), ("Name", Name), ("CountryId", CountryId), ("Region", DBNull.Value), ("City", City), ("PostalCode", PostalCode), ("AddressLine1", AddressLine1), ("AddressLine2", DBNull.Value), ("IsDefault", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        DataRow AcmeRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-ACME"));
        DataRow NorthwindRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-NORTH"));
        DataRow HeliosRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SUP-HELIOS"));
        DataRow BerlinRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SUP-BERLIN"));
        DataRow ConsumerRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-NIKOS"));
        DataRow ExportRow = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-LIBERTY"));
        AddAddress(AcmeRow, (int)AddressType.Billing, "ADR-ACME-BILL", "Billing Address", GreeceId, "Athens", "10563", "10 Ermou Street");
        AddAddress(AcmeRow, (int)AddressType.Shipping, "ADR-ACME-SHIP", "Shipping Address", GreeceId, "Athens", "10435", "20 Iera Odos");
        AddAddress(NorthwindRow, (int)AddressType.Billing, "ADR-NORTH-BILL", "Billing Address", GreeceId, "Athens", "11523", "25 Kifisias Avenue");
        AddAddress(NorthwindRow, (int)AddressType.Shipping, "ADR-NORTH-SHIP", "Shipping Address", GreeceId, "Marousi", "15124", "80 Kifisias Avenue");
        AddAddress(HeliosRow, (int)AddressType.Billing, "ADR-HELIOS-BILL", "Billing Address", GreeceId, "Piraeus", "18531", "8 Piraeus Street");
        AddAddress(HeliosRow, (int)AddressType.Shipping, "ADR-HELIOS-SHIP", "Shipping Address", GreeceId, "Aspropyrgos", "19300", "12 Industrial Road");
        AddAddress(BerlinRow, (int)AddressType.Billing, "ADR-BERLIN-BILL", "Billing Address", GermanyId, "Berlin", "10178", "42 Alexanderplatz");
        AddAddress(BerlinRow, (int)AddressType.Shipping, "ADR-BERLIN-SHIP", "Shipping Address", GermanyId, "Berlin", "10179", "18 Holzmarktstrasse");
        AddAddress(ConsumerRow, (int)AddressType.Billing, "ADR-NIKOS-BILL", "Home Address", GreeceId, "Athens", "10434", "15 Patision Street");
        AddAddress(ConsumerRow, (int)AddressType.Shipping, "ADR-NIKOS-SHIP", "Delivery Address", GreeceId, "Athens", "10434", "15 Patision Street");
        AddAddress(ExportRow, (int)AddressType.Billing, "ADR-LIBERTY-BILL", "Billing Address", UnitedStatesId, "San Francisco", "94105", "100 Market Street");
        AddAddress(ExportRow, (int)AddressType.Shipping, "ADR-LIBERTY-SHIP", "Shipping Address", UnitedStatesId, "San Francisco", "94107", "250 King Street");
    }
    static void Add_PersonContact()
    {
        string TableName = "PersonContact";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Person").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblPersonContact = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblPersonContact);

        MemTable tblPerson = SampleTables["Person"];
        MemTable tblContactType = SampleTables["ContactType"];

        object SalesId = tblContactType.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Sales"))["Id"];
        object AccountingId = tblContactType.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Accounting"))["Id"];
        object TechnicalId = tblContactType.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Technical"))["Id"];

        void AddContact(DataRow PersonRow, object ContactTypeId, string Name, string JobTitle, string Phone, string Email, bool IsDefault)
        {
            object PersonId = PersonRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(PersonId);
            tblPersonContact = Module.GetTable(TableName);

            AddRow(tblPersonContact, ("Id", Id), ("PersonId", PersonId), ("ContactTypeId", ContactTypeId), ("Name", Name), ("JobTitle", JobTitle), ("Phone", Phone), ("Mobile", DBNull.Value), ("Email", Email), ("IsDefault", IsDefault), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("PersonId", PersonId), ("ContactTypeId", ContactTypeId), ("Name", Name), ("JobTitle", JobTitle), ("Phone", Phone), ("Mobile", DBNull.Value), ("Email", Email), ("IsDefault", IsDefault), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddContact(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA")), SalesId, "Maria Antoniou", "Sales Manager", "+30 210 1000001", "sales@acmeretail.example", true);
        AddContact(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd")), AccountingId, "Alex Morgan", "Accounting Manager", "+30 210 1000002", "accounts@northwind.example", true);
        AddContact(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE")), TechnicalId, "Nikos Papadopoulos", "Technical Contact", "+30 210 1000003", "support@helios.example", true);
    }
    static void Add_PersonBankAccount()
    {
        string TableName = "PersonBankAccount";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Person").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblPersonBankAccount = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblPersonBankAccount);

        MemTable tblPerson = SampleTables["Person"];
        MemTable tblBank = SampleTables["Bank"];

        object FirstBankId = tblBank.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BNK-001"))["Id"];
        object UnionBankId = tblBank.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BNK-003"))["Id"];

        void AddBankAccount(DataRow PersonRow, object BankId, string Name, string Iban, string SwiftCode)
        {
            object PersonId = PersonRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(PersonId);
            tblPersonBankAccount = Module.GetTable(TableName);

            AddRow(tblPersonBankAccount, ("Id", Id), ("PersonId", PersonId), ("BankId", BankId), ("Name", Name), ("Iban", Iban), ("SwiftCode", SwiftCode), ("IsDefault", true), ("IsActive", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("PersonId", PersonId), ("BankId", BankId), ("Name", Name), ("Iban", Iban), ("SwiftCode", SwiftCode), ("IsDefault", true), ("IsActive", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddBankAccount(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Acme Retail SA")), FirstBankId, "Main Account", "GR1601101250000000012300701", "ETHNGRAA");
        AddBankAccount(tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE")), UnionBankId, "Settlement Account", "GR1601101250000000012300702", "UNBNGRAA");
    }
    static void Add_AssetAssignment()
    {
        string TableName = "AssetAssignment";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("FixedAsset").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblAssetAssignment = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblAssetAssignment);

        MemTable tblFixedAsset = SampleTables["FixedAsset"];
        MemTable tblPerson = SampleTables["Person"];

        object EmployeeId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-ELENA"))["Id"];

        void AddAssignment(DataRow FixedAssetRow, object PersonId)
        {
            object FixedAssetId = FixedAssetRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(FixedAssetId);
            tblAssetAssignment = Module.GetTable(TableName);

            AddRow(tblAssetAssignment, ("Id", Id), ("FixedAssetId", FixedAssetId), ("PersonId", PersonId), ("AssignmentDate", DateTime.Today.AddMonths(-3)), ("ReturnDate", DBNull.Value), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("FixedAssetId", FixedAssetId), ("PersonId", PersonId), ("AssignmentDate", DateTime.Today.AddMonths(-3)), ("ReturnDate", DBNull.Value), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddAssignment(tblFixedAsset.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Office Laptop")), EmployeeId);
    }
    static void Add_AssetMaintenance()
    {
        string TableName = "AssetMaintenance";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("FixedAsset").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblAssetMaintenance = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblAssetMaintenance);

        MemTable tblFixedAsset = SampleTables["FixedAsset"];
        DataRow VanRow = tblFixedAsset.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Delivery Van"));
        object FixedAssetId = VanRow["Id"];
        object Id = Sys.GenId();

        Module.Edit(FixedAssetId);
        tblAssetMaintenance = Module.GetTable(TableName);

        AddRow(tblAssetMaintenance, ("Id", Id), ("FixedAssetId", FixedAssetId), ("Date", DateTime.Today.AddMonths(-2)), ("Description", "Scheduled service"), ("Cost", 320.0000m), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Id), ("FixedAssetId", FixedAssetId), ("Date", DateTime.Today.AddMonths(-2)), ("Description", "Scheduled service"), ("Cost", 320.0000m), ("Notes", DBNull.Value));

        Module.Commit();
    }
    static void Add_AssetDocument()
    {
        string TableName = "AssetDocument";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("FixedAsset").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblAssetDocument = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblAssetDocument);

        MemTable tblFixedAsset = SampleTables["FixedAsset"];
        DataRow LaptopRow = tblFixedAsset.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Office Laptop"));
        object FixedAssetId = LaptopRow["Id"];
        object Id = Sys.GenId();

        Module.Edit(FixedAssetId);
        tblAssetDocument = Module.GetTable(TableName);

        AddRow(tblAssetDocument, ("Id", Id), ("FixedAssetId", FixedAssetId), ("Name", "Purchase Invoice"), ("FileName", "office-laptop-invoice.pdf"), ("Description", "Purchase invoice"), ("BlobText", DBNull.Value));
        AddRow(tblSource, ("Id", Id), ("FixedAssetId", FixedAssetId), ("Name", "Purchase Invoice"), ("FileName", "office-laptop-invoice.pdf"), ("Description", "Purchase invoice"), ("BlobText", DBNull.Value));

        Module.Commit();
    }
    static void Add_AssetInsurance()
    {
        string TableName = "AssetInsurance";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("FixedAsset").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblAssetInsurance = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblAssetInsurance);

        MemTable tblFixedAsset = SampleTables["FixedAsset"];
        DataRow VanRow = tblFixedAsset.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Delivery Van"));
        object FixedAssetId = VanRow["Id"];
        object Id = Sys.GenId();
        DateTime StartDate = new(DateTime.Today.Year, 1, 1);

        Module.Edit(FixedAssetId);
        tblAssetInsurance = Module.GetTable(TableName);

        AddRow(tblAssetInsurance, ("Id", Id), ("FixedAssetId", FixedAssetId), ("PolicyNumber", "POL-VAN-001"), ("StartDate", StartDate), ("EndDate", StartDate.AddYears(1).AddDays(-1)), ("Amount", 850.0000m), ("IsActive", true), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Id), ("FixedAssetId", FixedAssetId), ("PolicyNumber", "POL-VAN-001"), ("StartDate", StartDate), ("EndDate", StartDate.AddYears(1).AddDays(-1)), ("Amount", 850.0000m), ("IsActive", true), ("Notes", DBNull.Value));

        Module.Commit();
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
        object LaptopComputerId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Laptop Computer 14"))["Id"];
        object MonitorId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Monitor 27 Inch"))["Id"];
        object WirelessKeyboardId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Wireless Keyboard"))["Id"];
        object CoffeeCapsulesId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Capsules"))["Id"];
        object MineralWaterId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Mineral Water"))["Id"];
        object OrangeJuiceId = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Orange Juice"))["Id"];
        object PieceId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("H87"))["Id"];
        object KilogramId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("KGM"))["Id"];
        object LiterId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("LTR"))["Id"];
        object BoxId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BX"))["Id"];

        DateTime ValidFrom = new(DateTime.Today.Year, 1, 1);

        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", CoffeeMachineId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 249.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", EspressoBeansId), ("UnitOfMeasureId", KilogramId), ("MinQuantity", 1.0000m), ("UnitPrice", 18.5000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", LaptopComputerId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 1299.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", MonitorId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 329.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", WirelessKeyboardId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 49.9500m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", CoffeeCapsulesId), ("UnitOfMeasureId", BoxId), ("MinQuantity", 1.0000m), ("UnitPrice", 8.7500m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", MineralWaterId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 1.0000m), ("UnitPrice", 1.2000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", RetailPriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", OrangeJuiceId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 1.0000m), ("UnitPrice", 2.8500m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", CoffeeMachineId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 229.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", EspressoBeansId), ("UnitOfMeasureId", KilogramId), ("MinQuantity", 1.0000m), ("UnitPrice", 16.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", LaptopComputerId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 1249.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", MonitorId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 309.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", WirelessKeyboardId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 1.0000m), ("UnitPrice", 45.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", CoffeeCapsulesId), ("UnitOfMeasureId", BoxId), ("MinQuantity", 1.0000m), ("UnitPrice", 7.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", MineralWaterId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 1.0000m), ("UnitPrice", 1.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", StandardDiscountCategoryId), ("CustomerId", DBNull.Value), ("ProductId", OrangeJuiceId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 1.0000m), ("UnitPrice", 2.5000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", CoffeeMachineId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 5.0000m), ("UnitPrice", 219.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", EspressoBeansId), ("UnitOfMeasureId", KilogramId), ("MinQuantity", 10.0000m), ("UnitPrice", 15.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", LaptopComputerId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 3.0000m), ("UnitPrice", 1199.0000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", MonitorId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 5.0000m), ("UnitPrice", 289.9000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", WirelessKeyboardId), ("UnitOfMeasureId", PieceId), ("MinQuantity", 10.0000m), ("UnitPrice", 42.5000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", CoffeeCapsulesId), ("UnitOfMeasureId", BoxId), ("MinQuantity", 20.0000m), ("UnitPrice", 7.2500m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", MineralWaterId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 48.0000m), ("UnitPrice", 0.8500m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("PriceListTypeId", WholesalePriceListTypeId), ("DiscountCategoryId", PreferredDiscountCategoryId), ("CustomerId", AcmeRetailId), ("ProductId", OrangeJuiceId), ("UnitOfMeasureId", LiterId), ("MinQuantity", 24.0000m), ("UnitPrice", 2.2000m), ("ValidFrom", ValidFrom), ("ValidTo", DBNull.Value), ("IsActive", true), ("Remarks", DBNull.Value));

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
        object ResponsiblePersonId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-DIMITRIS"))["Id"];

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

        object AcmeRetailId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("CUST-ACME"))["Id"];
        object NorthwindId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Northwind Traders Ltd"))["Id"];
        object ElenaId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-ELENA"))["Id"];
        object DimitrisId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-DIMITRIS"))["Id"];
        object KaterinaId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EMP-KATERINA"))["Id"];
        object SalesCostCenterId = tblCostCenter.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SALES"))["Id"];
        object SupportCostCenterId = tblCostCenter.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("SUPPORT"))["Id"];

        DateTime StartDate = new(DateTime.Today.Year, 1, 1);
        DateTime EndDate = new(DateTime.Today.Year, 12, 31);

        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "ERP Installation"), ("CustomerId", AcmeRetailId), ("ProjectStatusId", (int)ProjectStatus.Active), ("StartDate", StartDate), ("EndDate", EndDate), ("CostCenterId", SupportCostCenterId), ("ManagerPersonId", KaterinaId), ("IsActive", true), ("Color", "#2563EB"), ("IconName", "BriefcaseBusiness"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "CRM Migration"), ("CustomerId", NorthwindId), ("ProjectStatusId", (int)ProjectStatus.Draft), ("StartDate", StartDate.AddMonths(2)), ("EndDate", DBNull.Value), ("CostCenterId", SalesCostCenterId), ("ManagerPersonId", ElenaId), ("IsActive", true), ("Color", "#16A34A"), ("IconName", "DatabaseZap"), ("Remarks", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Name", "Warehouse Automation"), ("CustomerId", AcmeRetailId), ("ProjectStatusId", (int)ProjectStatus.Active), ("StartDate", StartDate.AddMonths(4)), ("EndDate", DBNull.Value), ("CostCenterId", SupportCostCenterId), ("ManagerPersonId", DimitrisId), ("IsActive", true), ("Color", "#F59E0B"), ("IconName", "Warehouse"), ("Remarks", DBNull.Value));

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

        object BoxId = tblUnitOfMeasure.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("BX"))["Id"];

        void AddUnit(DataRow ProductRow, object UnitId, decimal Ratio, string Barcode, bool IsSalesDefault, bool IsPurchaseDefault)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductUnitOfMeasure = Module.GetTable(TableName);

            AddRow(tblProductUnitOfMeasure, ("Id", Id), ("ProductId", ProductId), ("UnitId", UnitId), ("Ratio", Ratio), ("Barcode", Barcode), ("IsSalesDefault", IsSalesDefault), ("IsPurchaseDefault", IsPurchaseDefault), ("IsActive", true), ("Remarks", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("UnitId", UnitId), ("Ratio", Ratio), ("Barcode", Barcode), ("IsSalesDefault", IsSalesDefault), ("IsPurchaseDefault", IsPurchaseDefault), ("IsActive", true), ("Remarks", DBNull.Value));

            Module.Commit();
        }

        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));
        AddUnit(CoffeeMachineRow, CoffeeMachineRow["PrimaryUnitOfMeasureId"], 1.0000m, "5200000000011", true, true);
        AddUnit(CoffeeMachineRow, BoxId, 12.0000m, "5200000001018", false, false);
        foreach (DataRow ProductRow in tblProduct.Rows.Cast<DataRow>().Where(x => !x.AsString("Name").IsSameText("Coffee Machine")))
            AddUnit(ProductRow, ProductRow["PrimaryUnitOfMeasureId"], 1.0000m, ProductRow.AsString("Barcode"), true, true);
    }
    static void Add_ProductBarcode()
    {
        string TableName = "ProductBarcode";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductBarcode = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductBarcode);

        MemTable tblProduct = SampleTables["Product"];

        void AddBarcode(DataRow ProductRow, string Barcode, string Name, bool IsDefault)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductBarcode = Module.GetTable(TableName);

            AddRow(tblProductBarcode, ("Id", Id), ("ProductId", ProductId), ("Barcode", Barcode), ("Name", Name), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("Barcode", Barcode), ("Name", Name), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine")), "5200000000011", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine")), "5200000001018", "Box Barcode", false);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans")), "5200000000028", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Laptop Computer 14")), "5200000000035", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Monitor 27 Inch")), "5200000000042", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Wireless Keyboard")), "5200000000059", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Capsules")), "5200000000066", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Mineral Water")), "5200000000073", "Retail Barcode", true);
        AddBarcode(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Orange Juice")), "5200000000080", "Retail Barcode", true);
    }
    static void Add_ProductSupplier()
    {
        string TableName = "ProductSupplier";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductSupplier = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductSupplier);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblPerson = SampleTables["Person"];

        object HeliosId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Helios Supplies OE"))["Id"];
        object BerlinId = tblPerson.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Berlin Components GmbH"))["Id"];

        void AddSupplier(DataRow ProductRow, object SupplierId, string SupplierCode, int LeadDays, decimal LastCost, bool IsDefault)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductSupplier = Module.GetTable(TableName);

            AddRow(tblProductSupplier, ("Id", Id), ("ProductId", ProductId), ("SupplierId", SupplierId), ("SupplierCode", SupplierCode), ("LeadDays", LeadDays), ("LastCost", LastCost), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("SupplierId", SupplierId), ("SupplierCode", SupplierCode), ("LeadDays", LeadDays), ("LastCost", LastCost), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine")), BerlinId, "CM-2000", 14, 175.0000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans")), HeliosId, "EB-1KG", 5, 11.2000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Laptop Computer 14")), BerlinId, "LAP-14", 10, 900.0000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Monitor 27 Inch")), BerlinId, "MON-27", 10, 210.0000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Wireless Keyboard")), BerlinId, "KEY-WL", 7, 25.0000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Capsules")), HeliosId, "CAP-10", 5, 4.5000m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Mineral Water")), HeliosId, "WAT-1L", 3, 0.3500m, true);
        AddSupplier(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Orange Juice")), HeliosId, "JUI-1L", 3, 1.1000m, true);
    }
    static void Add_BillOfMaterial()
    {
        string TableName = "BillOfMaterial";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblBillOfMaterial = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblBillOfMaterial);

        MemTable tblProduct = SampleTables["Product"];
        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));
        object ProductId = CoffeeMachineRow["Id"];
        object Id = Sys.GenId();

        Module.Edit(ProductId);
        tblBillOfMaterial = Module.GetTable(TableName);

        AddRow(tblBillOfMaterial, ("Id", Id), ("ProductId", ProductId), ("Code", "BOM-CM-001"), ("Name", "Coffee Machine Bundle"), ("Quantity", 1.0000m), ("IsDefault", true), ("IsActive", true), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("Code", "BOM-CM-001"), ("Name", "Coffee Machine Bundle"), ("Quantity", 1.0000m), ("IsDefault", true), ("IsActive", true), ("Notes", DBNull.Value));

        Module.Commit();
    }
    static void Add_CashAccount()
    {
        string ModuleName = "CashAccount";
        if (!CanAdd(ModuleName, out DataModule Module))
            return;

        MemTable tblSource = new() { TableName = Module.tblItem.TableName };
        SampleTables[tblSource.TableName] = tblSource;

        tblSource.CopyColumnsFrom(Module.tblItem);

        MemTable tblCurrency = SampleTables["Currency"];
        MemTable tblCompanyBranch = SampleTables["CompanyBranch"];

        object EurId = tblCurrency.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("EUR"))["Id"];
        object BranchId = tblCompanyBranch.Rows.Cast<DataRow>().First(x => x.AsString("Code").IsSameText("MAIN"))["Id"];

        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CASH-MAIN"), ("Name", "Main Cash"), ("CurrencyId", EurId), ("CompanyBranchId", BranchId), ("Balance", 1000.0000m), ("IsActive", true), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CASH-STORE"), ("Name", "Store Cash"), ("CurrencyId", EurId), ("CompanyBranchId", BranchId), ("Balance", 350.0000m), ("IsActive", true), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Sys.GenId()), ("Code", "CASH-PETTY"), ("Name", "Petty Cash"), ("CurrencyId", EurId), ("CompanyBranchId", BranchId), ("Balance", 150.0000m), ("IsActive", true), ("Notes", DBNull.Value));

        Module.BatchInsert(tblSource);
    }
    static void Add_ProductImage()
    {
        string TableName = "ProductImage";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductImage = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductImage);

        MemTable tblProduct = SampleTables["Product"];

        void AddImage(DataRow ProductRow, string Name, int DisplayOrder, bool IsDefault)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductImage = Module.GetTable(TableName);

            AddRow(tblProductImage, ("Id", Id), ("ProductId", ProductId), ("Name", Name), ("ImageBlob", DBNull.Value), ("IsDefault", IsDefault), ("IsActive", true), ("DisplayOrder", DisplayOrder), ("Remarks", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("Name", Name), ("ImageBlob", DBNull.Value), ("IsDefault", IsDefault), ("IsActive", true), ("DisplayOrder", DisplayOrder), ("Remarks", DBNull.Value));

            Module.Commit();
        }

        AddImage(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine")), "Catalog Image", 10, true);
        AddImage(tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans")), "Package Image", 10, true);
    }
    static void Add_ProductAttribute()
    {
        string TableName = "ProductAttribute";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductAttribute = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductAttribute);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblProductAttributeGroup = SampleTables["ProductAttributeGroup"];

        object TechnicalId = tblProductAttributeGroup.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Technical"))["Id"];
        object DimensionsId = tblProductAttributeGroup.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Dimensions"))["Id"];
        object PackagingId = tblProductAttributeGroup.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Packaging"))["Id"];

        void AddAttribute(DataRow ProductRow, object ProductAttributeGroupId, string Name, int TypeId, string TextValue, object UnitOfMeasure, int DisplayOrder, bool IsFilter)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductAttribute = Module.GetTable(TableName);

            AddRow(tblProductAttribute, ("Id", Id), ("ProductId", ProductId), ("ProductAttributeGroupId", ProductAttributeGroupId), ("Name", Name), ("TypeId", TypeId), ("TextValue", TextValue), ("UnitOfMeasure", UnitOfMeasure), ("DisplayOrder", DisplayOrder), ("IsSpec", true), ("IsFilter", IsFilter), ("IsActive", true));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("ProductAttributeGroupId", ProductAttributeGroupId), ("Name", Name), ("TypeId", TypeId), ("TextValue", TextValue), ("UnitOfMeasure", UnitOfMeasure), ("DisplayOrder", DisplayOrder), ("IsSpec", true), ("IsFilter", IsFilter), ("IsActive", true));

            Module.Commit();
        }

        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));
        DataRow EspressoBeansRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans"));

        AddAttribute(CoffeeMachineRow, TechnicalId, "Power", (int)ProductAttributeType.Integer, "1450", "W", 10, true);
        AddAttribute(CoffeeMachineRow, DimensionsId, "Weight", (int)ProductAttributeType.Decimal, "6.5", "Kg", 20, true);
        AddAttribute(EspressoBeansRow, PackagingId, "Package", (int)ProductAttributeType.Option, "Bag", DBNull.Value, 10, true);
    }
    static void Add_ProductWarehouse()
    {
        string TableName = "ProductWarehouse";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblProductWarehouse = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblProductWarehouse);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblWarehouse = SampleTables["Warehouse"];

        object MainWarehouseId = tblWarehouse.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Main Warehouse"))["Id"];
        object RetailStoreId = tblWarehouse.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Retail Store"))["Id"];

        void AddWarehouse(DataRow ProductRow, object WarehouseId, decimal MinStock, decimal MaxStock, decimal ReorderPoint, bool IsDefault)
        {
            object ProductId = ProductRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(ProductId);
            tblProductWarehouse = Module.GetTable(TableName);

            AddRow(tblProductWarehouse, ("Id", Id), ("ProductId", ProductId), ("WarehouseId", WarehouseId), ("MinStock", MinStock), ("MaxStock", MaxStock), ("ReorderPoint", ReorderPoint), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("ProductId", ProductId), ("WarehouseId", WarehouseId), ("MinStock", MinStock), ("MaxStock", MaxStock), ("ReorderPoint", ReorderPoint), ("IsDefault", IsDefault), ("IsActive", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        foreach (DataRow ProductRow in tblProduct.Rows)
        {
            AddWarehouse(ProductRow, MainWarehouseId, 5.0000m, 500.0000m, 20.0000m, true);
            AddWarehouse(ProductRow, RetailStoreId, 2.0000m, 100.0000m, 10.0000m, false);
        }
    }
    static void Add_WarehouseLocation()
    {
        string TableName = "WarehouseLocation";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Warehouse").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblWarehouseLocation = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblWarehouseLocation);

        MemTable tblWarehouse = SampleTables["Warehouse"];

        void AddLocation(DataRow WarehouseRow, string Code, string Name, string Zone, string Aisle, string Rack, string Shelf, string Bin)
        {
            object WarehouseId = WarehouseRow["Id"];
            object Id = Sys.GenId();

            Module.Edit(WarehouseId);
            tblWarehouseLocation = Module.GetTable(TableName);

            AddRow(tblWarehouseLocation, ("Id", Id), ("WarehouseId", WarehouseId), ("Code", Code), ("Name", Name), ("Zone", Zone), ("Aisle", Aisle), ("Rack", Rack), ("Shelf", Shelf), ("Bin", Bin), ("IsActive", true), ("Notes", DBNull.Value));
            AddRow(tblSource, ("Id", Id), ("WarehouseId", WarehouseId), ("Code", Code), ("Name", Name), ("Zone", Zone), ("Aisle", Aisle), ("Rack", Rack), ("Shelf", Shelf), ("Bin", Bin), ("IsActive", true), ("Notes", DBNull.Value));

            Module.Commit();
        }

        AddLocation(tblWarehouse.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Main Warehouse")), "LOC-A-01-01", "Zone A Rack 01", "A", "01", "01", "01", "A-01-01");
        AddLocation(tblWarehouse.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Retail Store")), "LOC-R-01-01", "Retail Shelf 01", "R", "01", "01", "01", "R-01-01");
    }
    static void Add_BillOfMaterialLine()
    {
        string TableName = "BillOfMaterialLine";
        if (!Store.TableExists(TableName) || !Store.TableIsEmpty(TableName))
            return;

        DataModule Module = DataRegistry.Modules.Get("Product").Create();
        MemTable tblSource = new() { TableName = TableName };
        SampleTables[tblSource.TableName] = tblSource;

        MemTable tblBillOfMaterialLine = Module.GetTable(TableName);
        tblSource.CopyColumnsFrom(tblBillOfMaterialLine);

        MemTable tblProduct = SampleTables["Product"];
        MemTable tblBillOfMaterial = SampleTables["BillOfMaterial"];

        DataRow BillOfMaterialRow = tblBillOfMaterial.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine Bundle"));
        DataRow CoffeeMachineRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Coffee Machine"));
        DataRow EspressoBeansRow = tblProduct.Rows.Cast<DataRow>().First(x => x.AsString("Name").IsSameText("Espresso Beans"));
        object ProductId = CoffeeMachineRow["Id"];
        object BillOfMaterialId = BillOfMaterialRow["Id"];
        object Id = Sys.GenId();

        Module.Edit(ProductId);
        tblBillOfMaterialLine = Module.GetTable(TableName);

        AddRow(tblBillOfMaterialLine, ("Id", Id), ("BillOfMaterialId", BillOfMaterialId), ("ProductId", EspressoBeansRow["Id"]), ("Quantity", 1.0000m), ("Notes", DBNull.Value));
        AddRow(tblSource, ("Id", Id), ("BillOfMaterialId", BillOfMaterialId), ("ProductId", EspressoBeansRow["Id"]), ("Quantity", 1.0000m), ("Notes", DBNull.Value));

        Module.Commit();
    }
    
    protected override void AddSampleDataInternal()
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
        Add_Company();
        Add_TaxRate();
        Add_TaxBusinessGroup();
        Add_TaxProductGroup();
        Add_TaxJurisdiction();
        Add_TaxClause();
        Add_TaxRule();
        Add_PaymentTerm();
        Add_ProductGroup();
        Add_FiscalYear();
        Add_Language();
        Add_ResourceStrings();
        Add_PersonRoleType();
        Add_StockReason();
        Add_ContactType();
        Add_AssetCategory();
        Add_AssetLocation();
        Add_AssetDepreciationMethod();
        Add_ProductDimension();
        Add_ProductAttributeGroup();
        Add_PriceListType();
        Add_FiscalPeriod();
        Add_Person();
        Add_Category();
        Add_FixedAsset();
        Add_ProductDimensionValue();
        Add_CompanyBranch();
        Add_CompanyBankAccount();
        Add_PersonRole();
        Add_CostCenter();
        Add_Product();
        Add_PersonAddress();
        Add_PersonContact();
        Add_PersonBankAccount();
        Add_AssetAssignment();
        Add_AssetMaintenance();
        Add_AssetDocument();
        Add_AssetInsurance();
        Add_PriceList();
        Add_ProductGroups();
        Add_Warehouse();
        Add_Project();
        Add_ProductCategory();
        Add_ProductUnitOfMeasure();
        Add_ProductBarcode();
        Add_ProductSupplier();
        Add_BillOfMaterial();
        Add_CashAccount();
        Add_ProductImage();
        Add_ProductAttribute();
        Add_ProductWarehouse();
        Add_WarehouseLocation();
        Add_BillOfMaterialLine();

        SetIsAdded();
    }

    // ● construction
    public SampleData1()
    {
    }
    
    public override int VersionNumber => 1;
}

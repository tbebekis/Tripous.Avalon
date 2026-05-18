namespace tERP;

static internal partial class AppHost
{
    static void AddCompany()
    {
        //string SqlText = "select * from Company";
        string TableName = "Company";
        if (Store.TableExists(TableName) && Store.TableIsEmpty(TableName))
        {
            DataModule dmCompany = DataRegistry.CreateModule(TableName);
            dmCompany.Insert();
            MemTable tblItem = dmCompany.tblItem;
            DataRow Row = tblItem.Rows[0];
            Row["Id"] = Sys.StandardCompanyGuid;
            Row["Code"] = "001";
            Row["Name"] = "Default";
            Row["Title"] = "Default";
            Row["TaxNumber"] = "0123456789";
            Row["TaxOfficeId"] = "";
            Row["CountryId"] = "";
            Row["CurrencyId"] = "";
            dmCompany.Commit();
        }
    }

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
            CodeProviderEntries.Module.SeedPatterns(CodeProviderPatters);
        }
    }
    static void AddInitialData()
    {
        AddCompany();
        AddCodeProviderPatterns();
    }
}
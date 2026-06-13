/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;


/// <summary>
/// Represents this library.
/// </summary>
static public partial class DataLib
{
    static DbLogListener_tERP LogListener;
    
    // ● public
    /// <summary>
    /// We need to call this first of all in order for .Net to load the assembly.
    /// <para>Otherwise is not "visible" to <see cref="TypeStore.RegisterLoadedAssemblies()"/> which registers types marked with the <see cref="TypeStoreAttribute"/>.</para>
    /// </summary>
    static public void Load()
    {
        // fake, must be called for the assembly to be loaded in the domain.
    }
    /// <summary>
    /// Initializes this library.
    /// </summary>
    static public void Initialize()
    {
        LogListener = new();
    }
    
    // ● defaults
    static string GetDefaultId(string SqlText, string CodeValue)
    {
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", CodeValue, LocateOptions.CaseInsensitive);    
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }
        return ""; 
    }
    static public string GetDefaultWarehouseId()
    {
        string SqlText = @"
select
    Id
from
    Warehouse
where
    IsActive <> 0
order by
    Code
";
 
        DataRow Row = Db.DefaultStore.SelectResults(SqlText);
        return Row != null ? Row.AsString("Id") : "";
    }
    static public string GetDefaultBranchId()
    {
        string SqlText = $@"
select
    Id
from
    CompanyBranch
where
    IsActive <> 0
    and IsPrimary = 1
    and CompanyId = '{DbConfig.CompanyId}'
order by
    Code
";
 
        DataRow Row = Db.DefaultStore.SelectResults(SqlText);
        return Row != null ? Row.AsString("Id") : "";
    }
    static public string GetDefaultSalesCostCenterId()
    {
        string SqlText = @$"select * from CostCenter where IsActive = 1 order by Code";
        string CodeValue = "SALES";
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultPurchaseCostCenterId()
    {
        string SqlText = @$"select * from CostCenter where IsActive = 1 order by Code";
        string CodeValue = "PURCHASES";
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultCurrencyId()
    {
        string SqlText = @$"select * from Currency order by Code";
        string CodeValue = "EUR";
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultPaymentMethodId()
    {
        string SqlText = @$"select * from PaymentMethod where IsActive = 1 order by Code";
        string CodeValue = "42"; // Payment To Bank Account
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultPaymentTermId()  
    {
        string SqlText = @$"select * from PaymentTerm where IsActive = 1 order by Code";
        string CodeValue = "NET30"; // 30 Days
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultPriceListTypeId()
    {
        string SqlText = @$"select * from PriceListType where IsActive = 1 order by Code";
        string CodeValue = "WHOLESALE";
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultTaxBusinessGroupId()
    {
        string SqlText = @$"select * from TaxBusinessGroup  where IsActive = 1 order by Code";
        string CodeValue = "REGISTERED";
        return GetDefaultId(SqlText, CodeValue);
    }
    static public string GetDefaultTaxJurisdictionId()
    {
        string SqlText = @$"select * from TaxJurisdiction where IsActive = 1 order by Code";
        string CodeValue = "GR";
        return GetDefaultId(SqlText, CodeValue);
    }
    // 
    
    // ● miscs
    static public List<PersonAddress> LoadPersonAddressList(string PersonId)
    {
        List<PersonAddress> Result = new();

        string SqlText = $@"
select 
	t.Id, 
	t.PersonId, 
	t.AddressTypeId, 
	t.Code, 
	t.Name, 
	t.CountryId, 
	c.Code 			CountryCode,
	c.Name 			Country,
	t.Region, 
	t.City, 
	t.PostalCode, 
	t.AddressLine1, 
	t.AddressLine2, 
	t.IsDefault, 
	t.Notes
from 
	PersonAddress t
		left join Country c on t.CountryId = c.Id
where
    t.PersonId = '{PersonId}'
";

        DataTable Table = Db.DefaultStore.Select(SqlText);
        foreach (DataRow Row in Table.Rows)
            Result.Add(new PersonAddress(Row));

        return Result;
    }
    
    // ● properties
#if DEBUG
    static public string DebugUserName => "teo";
#else
    static public string DebugUserName => string.Empty;
#endif
    static public string[] SupportedCultures { get; } =  ["en-US", "el-GR"];
}

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
    static public string GetDefaultSalesCostCenterId()
    {
        string SqlText = @$"select Id, Code from CostCenter where IsActive = 1 order by Code";
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", "SALES", LocateOptions.CaseInsensitive);
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }

        return "";
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
    static public string GetDefaultCurrencyId()
    {
        string SqlText = @$"select Id, Code from Currency order by Code";
 
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", "EUR", LocateOptions.CaseInsensitive);
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }

        return "";
    }
    static public string GetDefaultPaymentMethodId()
    {
        string SqlText = @$"select Id, Code from PaymentMethod order by Code";
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", "42", LocateOptions.CaseInsensitive);    // Payment To Bank Account
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }
        return "";
    }
    static public string GetDefaultPaymentTermId()  
    {
        string SqlText = @$"select Id, Code from PaymentTerm order by Code";
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", "NET30", LocateOptions.CaseInsensitive);    // 30 Days
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }
        return "";    
    }
    static public string GetDefaultPriceListTypeId()
    {
        string SqlText = @$"select Id, Code from PriceListType order by Code";
        MemTable Table = Db.DefaultStore.Select(SqlText);
        if (Table.Rows.Count > 0)
        {
            DataRow Row = Table.Locate("Code", "WHOLESALE", LocateOptions.CaseInsensitive);    // WHOLESALE
            if (Row != null)
                return Row.AsString("Id");
            return Table.Rows[0].AsString("Id");
        }
        return "";  
    }
    
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
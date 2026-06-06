/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// The base <see cref="DataModule"/> class for all modules of this application.
/// </summary>
public class AppDataModule: DataModule
{
    // ● protected
    protected virtual string GetDefaultCurrencyId()
    {
        string SqlText = """
                         select
                           Id
                         from
                           Currency
                         where
                           Code = 'EUR'
                         order by
                           Code
                         """;
        DataRow Row = Store.SelectResults(SqlText);
        if (Row == null)
        {
            SqlText = """
                      select
                        Id
                      from
                        Currency
                      order by
                        Code
                      """;
            Row = Store.SelectResults(SqlText);
        }
        if (Row == null)
            throw new TripousDataException("Default currency not found.");
        return Row.AsString("Id");
    }
    protected virtual string GetDefaultBranchId()
    {
        string SqlText = """
                         select
                           Id
                         from
                           CompanyBranch
                         where
                           IsActive <> 0
                         order by
                           IsPrimary desc,
                           Code
                         """;
        DataRow Row = Store.SelectResults(SqlText);
        return Row != null ? Row.AsString("Id") : null;
    }
    protected virtual string GetDefaultWarehouseId()
    {
        string SqlText = """
                         select
                           Id
                         from
                           Warehouse
                         where
                           IsActive <> 0
                         order by
                           Code
                         """;
        DataRow Row = Store.SelectResults(SqlText);
        return Row != null ? Row.AsString("Id") : null;
    }
    protected virtual string GetCurrentAppUserId()
    {
        if (Sys.Context.CurrentUser == null || string.IsNullOrWhiteSpace(Sys.Context.CurrentUser.Id))
            throw new TripousDataException("Current user not found.");
        return Sys.Context.CurrentUser.Id;
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
          return;

        if (Table == tblItem)
        {
            if (IsInserting)
            {
                if (Table.ContainsColumn("CreatedBy"))
                    Row["CreatedBy"] = GetCurrentAppUserId();
                
                if (Table.ContainsColumn("CreatedAt"))
                    Row["CreatedAt"] = DateTime.UtcNow;
            }

            if (Table.ContainsColumn("ModifiedBy"))
                Row["ModifiedBy"] = GetCurrentAppUserId();
            
            if (Table.ContainsColumn("ModifiedAt"))
                Row["ModifiedAt"] = DateTime.UtcNow;
            
            if (Table.ContainsColumn("CurrencyId") && Sys.IsNull(Row["CurrencyId"]))
                Row["CurrencyId"] = GetDefaultCurrencyId();
            
            if (Table.ContainsColumn("BranchId") && Sys.IsNull(Row["BranchId"]))
            {
                string BranchId = GetDefaultBranchId();
                if (!string.IsNullOrWhiteSpace(BranchId))
                    Row["BranchId"] = BranchId;
            }
            
            if (Table.ContainsColumn("WarehouseId") &&  Sys.IsNull(Row["WarehouseId"]))
            {
                string WarehouseId = GetDefaultWarehouseId();
                if (!string.IsNullOrWhiteSpace(WarehouseId))
                    Row["WarehouseId"] = WarehouseId;
            }
        }
    }
    
    // ● construction
    public AppDataModule()
    {
    }
}
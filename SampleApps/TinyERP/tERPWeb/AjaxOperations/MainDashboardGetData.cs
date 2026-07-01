/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns data used by the main dashboard form.
/// </summary>
[AjaxOperation("App.MainDashboard.GetData")]
public class MainDashboardGetData: AppAjaxOperation
{
    // ● private
    JsonDataTable Select(string Name, string SqlText)
    {
        DataTable Table = Db.DefaultStore.Select(SqlText);
        Table.TableName = Name;
        return new JsonDataTable(Table);
    }
    string GetMetricsSql() => """
        select 'Sales' as Name, coalesce(sum(Trade.TotalAmount), 0) as Value
        from Trade
          inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
        where DocumentType.ModuleName = 'SalesInvoice'
          and Trade.TradeStatusId = 2
          and Trade.IsCancelled = 0
        union all
        select 'Purchases' as Name, coalesce(sum(Trade.TotalAmount), 0) as Value
        from Trade
          inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
        where DocumentType.ModuleName = 'PurchaseInvoice'
          and Trade.TradeStatusId = 2
          and Trade.IsCancelled = 0
        union all
        select 'StockValue' as Name, coalesce(sum(TotalCostAmount), 0) as Value
        from StockBalance
        union all
        select 'Receivables' as Name, coalesce(sum(Balance), 0) as Value
        from FinanceBalance
        where TradeTypeId = 1
          and PersonId is not null
        union all
        select 'Payables' as Name, coalesce(sum(Balance), 0) as Value
        from FinanceBalance
        where TradeTypeId = 2
          and PersonId is not null
        union all
        select 'CashBank' as Name, coalesce(sum(Balance), 0) as Value
        from FinanceBalance
        where PersonId is null
          and (CashAccountId is not null or CompanyBankAccountId is not null)
        """;
    string GetCustomersSql() => """
        select
          Person.Code as CustomerCode,
          Person.Name as CustomerName,
          count(*) as DocumentCount,
          sum(Trade.TotalAmount) as TotalAmount
        from Trade
          inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
          inner join Person on Person.Id = Trade.PersonId
        where DocumentType.ModuleName = 'SalesInvoice'
          and Trade.TradeStatusId = 2
          and Trade.IsCancelled = 0
        group by
          Person.Code,
          Person.Name
        order by TotalAmount desc
        limit 10
        """;
    string GetSuppliersSql() => """
        select
          Person.Code as SupplierCode,
          Person.Name as SupplierName,
          count(*) as DocumentCount,
          sum(Trade.TotalAmount) as TotalAmount
        from Trade
          inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
          inner join Person on Person.Id = Trade.PersonId
        where DocumentType.ModuleName = 'PurchaseInvoice'
          and Trade.TradeStatusId = 2
          and Trade.IsCancelled = 0
        group by
          Person.Code,
          Person.Name
        order by TotalAmount desc
        limit 10
        """;
    string GetStockSql() => """
        select
          Product.Code as ProductCode,
          Product.Name as ProductName,
          Warehouse.Code as WarehouseCode,
          Warehouse.Name as WarehouseName,
          StockBalance.PrimaryQuantity,
          StockBalance.TotalCostAmount
        from StockBalance
          inner join Product on Product.Id = StockBalance.ProductId
          inner join Warehouse on Warehouse.Id = StockBalance.WarehouseId
        order by StockBalance.TotalCostAmount desc
        limit 20
        """;

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["Metrics"] = Select("DashboardMetrics", GetMetricsSql());
        Result["Customers"] = Select("DashboardCustomers", GetCustomersSql());
        Result["Suppliers"] = Select("DashboardSuppliers", GetSuppliersSql());
        Result["Stock"] = Select("DashboardStock", GetStockSql());
        return Result;
    }
}

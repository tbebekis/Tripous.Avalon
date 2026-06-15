/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Displays a fixed demo dashboard for the tERP sample application.
/// </summary>
public partial class DashboardForm : AppForm
{
    // ● private fields
    ToolBar fToolBar;

    // ● private methods
    void CreateToolBar()
    {
        fToolBar = new();
        fToolBar.Panel = pnlToolBar;
        fToolBar.AddButton("table_refresh.png", "Refresh", async () => await RefreshDashboard());
        fToolBar.AddSeparator();
        fToolBar.AddButton("door_out.png", "Close", CloseForm);
    }
    decimal GetDecimal(string SqlText)
    {
        object Value = AppHost.Store.SelectResult(SqlText, 0m);
        return Sys.IsNull(Value) ? 0m : Convert.ToDecimal(Value);
    }
    string FormatAmount(decimal Value)
    {
        return Value.ToString("N2", CultureInfo.CurrentCulture);
    }
    void BindGrid(DataGrid Grid, string SqlText)
    {
        DataGridBinder.UnBindGrid(Grid);
        MemTable Table = AppHost.Store.Select(SqlText);
        DataGridBinder.BindGrid(Grid, Table.DataView, SupportsRecycling: false, GoToFirst: true);
    }
    async Task RefreshDashboard()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            txtSalesTotal.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(Trade.TotalAmount), 0)
                from Trade
                  inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
                where DocumentType.ModuleName = 'SalesInvoice'
                  and Trade.TradeStatusId = 2
                  and Trade.IsCancelled = 0
                """));
            txtPurchasesTotal.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(Trade.TotalAmount), 0)
                from Trade
                  inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
                where DocumentType.ModuleName = 'PurchaseInvoice'
                  and Trade.TradeStatusId = 2
                  and Trade.IsCancelled = 0
                """));
            txtStockValue.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(TotalCostAmount), 0)
                from StockBalance
                """));
            txtReceivables.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(Balance), 0)
                from FinanceBalance
                where TradeTypeId = 1
                  and PersonId is not null
                """));
            txtPayables.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(Balance), 0)
                from FinanceBalance
                where TradeTypeId = 2
                  and PersonId is not null
                """));
            txtCashBank.Text = FormatAmount(GetDecimal("""
                select coalesce(sum(Balance), 0)
                from FinanceBalance
                where PersonId is null
                  and (CashAccountId is not null or CompanyBankAccountId is not null)
                """));
            BindGrid(gridCustomers, """
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
                """);
            BindGrid(gridSuppliers, """
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
                """);
            BindGrid(gridStock, """
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
                """);
            AppHost.Log("Dashboard refreshed.");
        });
    }

    // ● protected methods
    /// <summary>
    /// Initializes the dashboard controls.
    /// </summary>
    protected override void FormInitialize()
    {
        CreateToolBar();
    }
    /// <summary>
    /// Loads dashboard data when the form opens.
    /// </summary>
    protected override async Task Start()
    {
        await RefreshDashboard();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public DashboardForm()
    {
        InitializeComponent();
    }
}

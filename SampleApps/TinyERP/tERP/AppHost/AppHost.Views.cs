/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class AppHost
{
    // ● private methods
    /// <summary>
    /// Opens a read-only view in the main content tab control.
    /// </summary>
    static object ShowReadOnlyViewFunc(Command Cmd)
    {
        SelectDef SelectDef = Cmd.Tag as SelectDef;
        if (SelectDef == null)
            throw new TripousException("Read-only view command has no view definition.");

        FormContext Context = FormContext.Create($"ReadOnlyView.{SelectDef.Name}", typeof(ReadOnlyViewForm).FullName, FormDisplayMode.TabItem, AppHost.MainWindow, SelectDef);
        Context.Title = SelectDef.Title;
        return AppHost.ContentHandler.ShowAppForm(Context);
    }
    /// <summary>
    /// Creates a read-only view definition.
    /// </summary>
    static SelectDef CreateReadOnlyView(string Name, string Title, string SqlText)
    {
        SelectDef Result = new();
        Result.Name = Name;
        Result.TitleKey = Title;
        Result.SqlText = SqlText;
        Result.UseFilters = true;
        return Result;
    }
    /// <summary>
    /// Adds common partner filters to a read-only view.
    /// </summary>
    static void AddPartnerFilters(SelectDef SelectDef, string Prefix)
    {
        SelectDef.AddFilter($"{Prefix}Code", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter($"{Prefix}Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
    }
    /// <summary>
    /// Adds a date filter to a read-only view.
    /// </summary>
    static void AddDateFilter(SelectDef SelectDef, string Name)
    {
        SelectDef.AddFilter(Name, FilterDataType: DataFieldType.Date, ConditionOp: ConditionOp.Between);
    }
    /// <summary>
    /// Adds a decimal filter to a read-only view.
    /// </summary>
    static void AddDecimalFilter(SelectDef SelectDef, string Name)
    {
        SelectDef.AddFilter(Name, FilterDataType: DataFieldType.Decimal, ConditionOp: ConditionOp.GreaterOrEqual);
    }
    /// <summary>
    /// Creates the read-only view definitions.
    /// </summary>
    static List<SelectDef> CreateReadOnlyViews()
    {
        List<SelectDef> Result = new();

        SelectDef View = CreateReadOnlyView("SalesByCustomer", "Sales By Customer", """
            select
              Person.Code as CustomerCode,
              Person.Name as CustomerName,
              Currency.Code as CurrencyCode,
              count(*) as DocumentCount,
              sum(Trade.NetAmount) as NetAmount,
              sum(Trade.TaxAmount) as TaxAmount,
              sum(Trade.TotalAmount) as TotalAmount
            from Trade
              inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
              inner join Person on Person.Id = Trade.PersonId
              inner join Currency on Currency.Id = Trade.CurrencyId
            where DocumentType.ModuleName = 'SalesInvoice'
              and Trade.TradeStatusId = 2
              and Trade.IsCancelled = 0
            group by
              Person.Code,
              Person.Name,
              Currency.Code
            order by TotalAmount desc
            """);
        AddPartnerFilters(View, "Customer");
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "TotalAmount");
        Result.Add(View);

        View = CreateReadOnlyView("SalesByMonth", "Sales By Month", """
            select
              strftime('%Y-%m', Trade.TradeDate) as YearMonth,
              Currency.Code as CurrencyCode,
              count(*) as DocumentCount,
              sum(Trade.NetAmount) as NetAmount,
              sum(Trade.TaxAmount) as TaxAmount,
              sum(Trade.TotalAmount) as TotalAmount
            from Trade
              inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
              inner join Currency on Currency.Id = Trade.CurrencyId
            where DocumentType.ModuleName = 'SalesInvoice'
              and Trade.TradeStatusId = 2
              and Trade.IsCancelled = 0
            group by
              strftime('%Y-%m', Trade.TradeDate),
              Currency.Code
            order by YearMonth desc
            """);
        View.AddFilter("YearMonth", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "TotalAmount");
        Result.Add(View);

        View = CreateReadOnlyView("PurchasesBySupplier", "Purchases By Supplier", """
            select
              Person.Code as SupplierCode,
              Person.Name as SupplierName,
              Currency.Code as CurrencyCode,
              count(*) as DocumentCount,
              sum(Trade.NetAmount) as NetAmount,
              sum(Trade.TaxAmount) as TaxAmount,
              sum(Trade.TotalAmount) as TotalAmount
            from Trade
              inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
              inner join Person on Person.Id = Trade.PersonId
              inner join Currency on Currency.Id = Trade.CurrencyId
            where DocumentType.ModuleName = 'PurchaseInvoice'
              and Trade.TradeStatusId = 2
              and Trade.IsCancelled = 0
            group by
              Person.Code,
              Person.Name,
              Currency.Code
            order by TotalAmount desc
            """);
        AddPartnerFilters(View, "Supplier");
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "TotalAmount");
        Result.Add(View);

        View = CreateReadOnlyView("PurchasesByMonth", "Purchases By Month", """
            select
              strftime('%Y-%m', Trade.TradeDate) as YearMonth,
              Currency.Code as CurrencyCode,
              count(*) as DocumentCount,
              sum(Trade.NetAmount) as NetAmount,
              sum(Trade.TaxAmount) as TaxAmount,
              sum(Trade.TotalAmount) as TotalAmount
            from Trade
              inner join DocumentType on DocumentType.Id = Trade.DocumentTypeId
              inner join Currency on Currency.Id = Trade.CurrencyId
            where DocumentType.ModuleName = 'PurchaseInvoice'
              and Trade.TradeStatusId = 2
              and Trade.IsCancelled = 0
            group by
              strftime('%Y-%m', Trade.TradeDate),
              Currency.Code
            order by YearMonth desc
            """);
        View.AddFilter("YearMonth", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "TotalAmount");
        Result.Add(View);

        View = CreateReadOnlyView("StockBalance", "Stock Balance", """
            select
              Product.Code as ProductCode,
              Product.Name as ProductName,
              Warehouse.Code as WarehouseCode,
              Warehouse.Name as WarehouseName,
              StockBalance.PrimaryQuantity,
              StockBalance.AverageUnitCost,
              StockBalance.TotalCostAmount,
              StockBalance.LastMovementDate
            from StockBalance
              inner join Product on Product.Id = StockBalance.ProductId
              inner join Warehouse on Warehouse.Id = StockBalance.WarehouseId
            order by
              Product.Code,
              Warehouse.Code
            """);
        View.AddFilter("ProductCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("ProductName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("WarehouseCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "PrimaryQuantity");
        AddDecimalFilter(View, "TotalCostAmount");
        AddDateFilter(View, "LastMovementDate");
        Result.Add(View);

        View = CreateReadOnlyView("FinanceBalance", "Finance Balance", """
            select
              case
                when Person.Id is not null then 'Person'
                when CashAccount.Id is not null then 'Cash'
                when CompanyBankAccount.Id is not null then 'Bank'
              end as OwnerType,
              coalesce(Person.Code, CashAccount.Code, CompanyBankAccount.Code) as OwnerCode,
              coalesce(Person.Name, CashAccount.Name, CompanyBankAccount.Name) as OwnerName,
              case FinanceBalance.TradeTypeId
                when 1 then 'Sales'
                when 2 then 'Purchases'
                when 3 then 'Warehouse'
                when 4 then 'Financial'
                when 5 then 'Accounting'
                else 'None'
              end as TradeType,
              Currency.Code as CurrencyCode,
              FinanceBalance.Balance,
              FinanceBalance.LastMovementDate
            from FinanceBalance
              inner join Currency on Currency.Id = FinanceBalance.CurrencyId
              left join Person on Person.Id = FinanceBalance.PersonId
              left join CashAccount on CashAccount.Id = FinanceBalance.CashAccountId
              left join CompanyBankAccount on CompanyBankAccount.Id = FinanceBalance.CompanyBankAccountId
            order by
              OwnerType,
              OwnerCode,
              TradeType
            """);
        View.AddFilter("OwnerType", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("OwnerCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("OwnerName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("TradeType", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "Balance");
        AddDateFilter(View, "LastMovementDate");
        Result.Add(View);

        View = CreateReadOnlyView("CashBankBalance", "Cash Bank Balance", """
            select
              case
                when CashAccount.Id is not null then 'Cash'
                when CompanyBankAccount.Id is not null then 'Bank'
              end as AccountType,
              coalesce(CashAccount.Code, CompanyBankAccount.Code) as AccountCode,
              coalesce(CashAccount.Name, CompanyBankAccount.Name) as AccountName,
              Currency.Code as CurrencyCode,
              FinanceBalance.Balance,
              FinanceBalance.LastMovementDate
            from FinanceBalance
              inner join Currency on Currency.Id = FinanceBalance.CurrencyId
              left join CashAccount on CashAccount.Id = FinanceBalance.CashAccountId
              left join CompanyBankAccount on CompanyBankAccount.Id = FinanceBalance.CompanyBankAccountId
            where FinanceBalance.PersonId is null
              and (FinanceBalance.CashAccountId is not null or FinanceBalance.CompanyBankAccountId is not null)
            order by
              AccountType,
              AccountCode
            """);
        View.AddFilter("AccountType", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("AccountCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("AccountName", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        View.AddFilter("CurrencyCode", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        AddDecimalFilter(View, "Balance");
        AddDateFilter(View, "LastMovementDate");
        Result.Add(View);

        return Result;
    }
    /// <summary>
    /// Registers read-only view commands.
    /// </summary>
    static void RegisterReadOnlyViewCommands()
    {
        Command Group = AppRegistry.FindCommand("Views");
        if (Group == null)
        {
            Group = new Command("Views");
            AppRegistry.MenuCommands.Add(Group);
        }

        foreach (SelectDef View in CreateReadOnlyViews())
        {
            Command Cmd = Command.Create(View.Name, "table.png", ShowReadOnlyViewFunc, View.TitleKey);
            Cmd.Tag = View;
            Group.Commands.Add(Cmd);
        }
    }
}

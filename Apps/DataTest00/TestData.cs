using System;
using System.Collections.Generic;
using System.Data;

namespace Tests;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public double UnitPrice { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CountryId { get; set; }
}

public enum TradeType
{
    None,
    Order,
    Sales,
}

public enum TradeStatus
{
    None,
    Cancelled,
    Pending,
    Completed,
}

public class Trade
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TradeType TradeType { get; set; }
    public TradeStatus Status { get; set; }
    public List<TradeLine> Lines { get; set; } = new();
}

public class TradeLine
{
    public int Id { get; set; }
    public int TradeId { get; set; }
    public int ProductId { get; set; }
    public double Qty { get; set; }
    public double UnitPrice { get; set; }
    public double LineAmount { get; set; }
}

public class TestData
{
    static readonly string[] CategoryList = { "Furniture", "Office Supplies", "Technology" };
    static readonly List<string[]> ProductsByCategoryList = new List<string[]> {
        new[] { "Executive Chair", "Standing Desk", "Ergonomic Bookcase", "Conference Table", "Leather Sofa" },
        new[] { "A4 Copy Paper", "Ring Binders", "Thermal Labels", "Gel Pens", "Steel Storage Cabinet" },
        new[] { "Workstation Laptop", "4K Monitor", "IP Phone", "Laser Printer", "Wireless Keyboard" }
    };

    static readonly string[] CountryList = { "Greece", "Germany", "United Kingdom", "France", "Italy", "Spain", "USA", "Canada" };
    static readonly List<string[]> CustomersByCountryList = new List<string[]> {
        new[] { "Alpha Delta S.A.", "Papadopoulos & Co", "Aegean Solutions" },
        new[] { "Berlin Tech GmbH", "Munich Logistics", "Hansa Group" },
        new[] { "London Bridge Ltd", "Skyline Trading", "Oxford Retail" },
        new[] { "Parisienne S.A.R.L.", "Lyon Industrial", "Cote d'Azur" },
        new[] { "Milano Fashion", "Roma Digital", "Venice Imports" },
        new[] { "Madrid Solar", "Barcelona Apps", "Iberica Foods" },
        new[] { "Global Dynamics", "Pacific Systems", "Liberty Bell Corp" },
        new[] { "Maple Leaf Inc", "Ontario Resources", "Quebec Logistics" }
    };
    
    static void SyncDataTables()
    {
        tblCountries = CreateTable<Country>(Countries);
        tblCategories = CreateTable<Category>(Categories);
        tblProducts = CreateTable<Product>(Products);
        tblCustomers = CreateTable<Customer>(Customers);
            
        // 1. Master: Trades
        tblTrades = new DataTable("Trades");
        tblTrades.Columns.Add("Id", typeof(int));
        tblTrades.Columns.Add("Date", typeof(DateTime));
        tblTrades.Columns.Add("TradeType", typeof(int));
        tblTrades.Columns.Add("Status", typeof(int));

        // 2. Detail: TradeLines
        tblTradeLines = new DataTable("TradeLines");
        tblTradeLines.Columns.Add("Id", typeof(int));
        tblTradeLines.Columns.Add("TradeId", typeof(int));
        tblTradeLines.Columns.Add("ProductId", typeof(int));
        tblTradeLines.Columns.Add("Qty", typeof(double));
        tblTradeLines.Columns.Add("UnitPrice", typeof(double));
        tblTradeLines.Columns.Add("LineAmount", typeof(double));

        foreach (var t in Trades)
        {
            tblTrades.Rows.Add(t.Id, t.Date, (int)t.TradeType, (int)t.Status);

            foreach (var l in t.Lines)
            {
                tblTradeLines.Rows.Add(
                    l.Id, 
                    l.TradeId, 
                    l.ProductId, 
                    l.Qty, 
                    l.UnitPrice, 
                    l.LineAmount
                );
            }
        }
    }

    static DataTable CreateTable<T>(List<T> list)
    {
        DataTable dt = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties();
        foreach (var p in props)
            dt.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

        foreach (var item in list)
        {
            var row = dt.NewRow();
            foreach (var p in props)
                row[p.Name] = p.GetValue(item) ?? DBNull.Value;
            dt.Rows.Add(row);
        }
        return dt;
    }

    // ● public properties
    static public List<Country> Countries { get; private set; } = new();
    static public List<Category> Categories { get; private set; } = new();
    static public List<Product> Products { get; private set; } = new();
    static public List<Customer> Customers { get; private set; } = new();
    static public List<Trade> Trades { get; private set; } = new();

    static public DataTable tblCountries { get; private set; }
    static public DataTable tblCategories { get; private set; }
    static public DataTable tblProducts { get; private set; }
    static public DataTable tblCustomers { get; private set; }
    static public DataTable tblTrades { get; private set; }
    static public DataTable tblTradeLines { get; private set; }

    static public void Initialize(int TradeCount = 500)
    {
        Random rnd = new Random();

        // 1. Categories & Products
        Categories.Clear();
        Products.Clear();
        int prodId = 1;
        for (int i = 0; i < CategoryList.Length; i++)
        {
            int catId = i + 1;
            Categories.Add(new Category { Id = catId, Name = CategoryList[i] });

            foreach (var prodName in ProductsByCategoryList[i])
            {
                Products.Add(new Product { 
                    Id = prodId++, 
                    Name = prodName, 
                    CategoryId = catId, 
                    UnitPrice = rnd.Next(10, 1500) + rnd.NextDouble() 
                });
            }
        }

        // 2. Countries & Customers
        Countries.Clear();
        Customers.Clear();
        int custId = 1;
        for (int i = 0; i < CountryList.Length; i++)
        {
            int countryId = i + 1;
            Countries.Add(new Country { Id = countryId, Name = CountryList[i] });

            foreach (var custName in CustomersByCountryList[i])
            {
                Customers.Add(new Customer { 
                    Id = custId++, 
                    Name = custName, 
                    CountryId = countryId 
                });
            }
        }

        // 3. Trades & TradeLines
        Trades.Clear();
        int lineId = 1;
        for (int i = 1; i <= TradeCount; i++)
        {
            var trade = new Trade
            {
                Id = i,
                Date = DateTime.Today.AddDays(-rnd.Next(0, 365)),
                TradeType = (TradeType)rnd.Next(1, 3),
                Status = (TradeStatus)rnd.Next(1, 4)
            };

            int lineCount = rnd.Next(3, 11);
            for (int j = 0; j < lineCount; j++)
            {
                var product = Products[rnd.Next(Products.Count)];
                var qty = rnd.Next(1, 20);
                trade.Lines.Add(new TradeLine
                {
                    Id = lineId++,
                    TradeId = i,
                    ProductId = product.Id,
                    Qty = qty,
                    UnitPrice = product.UnitPrice,
                    LineAmount = qty * product.UnitPrice
                });
            }
            Trades.Add(trade);
        }

        // 4. Create DataTables (Sync with Lists)
        SyncDataTables();
    }


}
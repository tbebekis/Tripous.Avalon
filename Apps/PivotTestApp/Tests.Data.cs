using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Avalonia.Controls.DataGridPivoting;

namespace PivotTestApp;

static public partial class Tests
{
    static readonly string[] s_regions = { "North", "South", "East", "West" };
    static readonly string[] s_segments = { "Consumer", "Corporate", "Home Office" };
    static readonly string[] s_categories = { "Furniture", "Office Supplies", "Technology" };
    static readonly Dictionary<string, string[]> s_productsByCategory = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Furniture"] = new[] { "Chair", "Desk", "Bookcase", "Table", "Sofa" },
        ["Office Supplies"] = new[] { "Paper", "Binders", "Labels", "Pens", "Storage" },
        ["Technology"] = new[] { "Laptop", "Monitor", "Phone", "Printer", "Accessories" }
    };
    
    static public ObservableCollection<SalesLine> CreatePocoSalesLines(int count, int seed = 1729)
    {
        ObservableCollection<SalesLine> Result = new();
        
        var random = new Random(seed);
        var start = new DateTime(DateTime.Today.Year - 2, 1, 1);
        var days = Math.Max(1, (DateTime.Today - start).Days);

        for (var i = 0; i < count; i++)
        {
            var region = s_regions[random.Next(s_regions.Length)];
            var segment = s_segments[random.Next(s_segments.Length)];
            var category = s_categories[random.Next(s_categories.Length)];
            var product = s_productsByCategory[category][random.Next(s_productsByCategory[category].Length)];
            var orderDate = start.AddDays(random.Next(days));

            var quantity = random.Next(1, 12);
            var unitPrice = random.NextDouble() * 900 + 25;
            var sales = Math.Round(unitPrice * quantity, 2);
            var margin = random.NextDouble() * 0.35 - 0.05;
            var profit = Math.Round(sales * margin, 2);

            SalesLine Line = new SalesLine
            {
                OrderDate = orderDate,
                Region = region,
                Segment = segment,
                Category = category,
                Product = product,
                Sales = sales,
                Profit = profit,
                Quantity = quantity
            };
            Result.Add(Line);
 
        }

        return Result;
    }

    static public DataTable CreateTableSalesLines(int count, int seed = 1729)
    {
 
        DataTable Table = new DataTable("SalesLines");
        Table.Columns.Add("OrderDate", typeof(DateTime));
        Table.Columns.Add("Region", typeof(string));
        Table.Columns.Add("Segment", typeof(string));
        Table.Columns.Add("Category", typeof(string));
        Table.Columns.Add("Product", typeof(string));
        Table.Columns.Add("Sales", typeof(double));
        Table.Columns.Add("Profit", typeof(double));
        Table.Columns.Add("Quantity", typeof(double));
        
        var random = new Random(seed);
        var start = new DateTime(DateTime.Today.Year - 2, 1, 1);
        var days = Math.Max(1, (DateTime.Today - start).Days);

        for (var i = 0; i < count; i++)
        {
            var region = s_regions[random.Next(s_regions.Length)];
            var segment = s_segments[random.Next(s_segments.Length)];
            var category = s_categories[random.Next(s_categories.Length)];
            var product = s_productsByCategory[category][random.Next(s_productsByCategory[category].Length)];
            var orderDate = start.AddDays(random.Next(days));

            var quantity = random.Next(1, 12);
            var unitPrice = random.NextDouble() * 900 + 25;
            var sales = Math.Round(unitPrice * quantity, 2);
            var margin = random.NextDouble() * 0.35 - 0.05;
            var profit = Math.Round(sales * margin, 2);

            DataRow Row = Table.NewRow();
            Table.Rows.Add(Row);
            Row["OrderDate"] = orderDate;
            Row["Region"] = region;
            Row["Segment"] = segment;
            Row["Category"] = category;
            Row["Product"] = product;
            Row["Sales"] = sales;
            Row["Profit"] = profit;
            Row["Quantity"] = quantity;
        }


        return Table;
    }


    

}
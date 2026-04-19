using System;
using System.Collections.Generic;
using System.Data;
using Tripous.Data;
 
using System.Collections;
 

namespace TestDataBinderApp00;

public enum LineStatus
{
    None,
    New,
    Active,
    Done
}

public sealed class SalesLine
{
    public DateTime? OrderDate { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public int? CategoryId { get; set; } 
    public string Product { get; set; } = string.Empty;
    public double Sales { get; set; }
    public double Profit { get; set; }
    public int Quantity { get; set; }
    public LineStatus? Status { get; set; }
    public bool Done { get; set; }
}

public class Category
{
    public override string ToString() => Name;
    public int Id { get; set; }
    public string Name { get; set; }
}

public class CategoryLookupSourcePoco : ILookupSource
{
    public string Name { get; } = "Category";
    public IEnumerable Items { get; } = TestData.Categories;
}

public class CategoryLookupSourceDataView : ILookupSource
{
    public string Name { get; } = "Category";
    public IEnumerable Items { get; } = TestData.tblCategory.DefaultView;
}

static public class TestData
{
    static public readonly string[] s_regions = { "North", "South", "East", "West" };
    static readonly string[] s_segments = { "Consumer", "Corporate", "Home Office" };
    static readonly string[] s_categories = { "Furniture", "Office Supplies", "Technology" };
    static readonly List<string[]> s_productsByCategory = new List<string[]>() {
        new[] { "Chair", "Desk", "Bookcase", "Table", "Sofa" },
        new[] { "Paper", "Binders", "Labels", "Pens", "Storage" },
        new[] { "Laptop", "Monitor", "Phone", "Printer", "Accessories" }
    };
        
    static List<SalesLine> CreatePocoSalesLines(int count, int seed = 1729)
    {
        List<SalesLine> Result = new();
        
        var random = new Random(seed);
        var start = new DateTime(DateTime.Today.Year - 2, 1, 1);
        var days = Math.Max(1, (DateTime.Today - start).Days);

        for (var i = 0; i < count; i++)
        {
            var region = s_regions[random.Next(s_regions.Length)];
            var segment = s_segments[random.Next(s_segments.Length)];
            var category = random.Next(s_categories.Length) + 1; //s_categories[random.Next(s_categories.Length)];
            var product = s_productsByCategory[category - 1][random.Next(s_productsByCategory[category - 1].Length)];
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
                CategoryId = category,
                Product = product,
                Sales = sales,
                Profit = profit,
                Quantity = quantity
            };
            Result.Add(Line);
 
        }

        return Result;
    }
    static List<Category> CreatePocoCategories()
    {
        List<Category> List = new();
        int Id = 0;
        foreach (string S in s_categories)
        {
            Id++;
            List.Add(new Category() {Id = Id, Name = S});
        }
        return List;
    }
    
    static DataTable CreateTableSalesLines(int count, int seed = 1729)
    {
 
        DataTable Table = new DataTable("SalesLines");
        Table.Columns.Add("OrderDate", typeof(DateTime));
        Table.Columns.Add("Region", typeof(string));
        Table.Columns.Add("Segment", typeof(string));
        Table.Columns.Add("CategoryId", typeof(string));
        Table.Columns.Add("Product", typeof(string));
        Table.Columns.Add("Sales", typeof(double)).AllowDBNull = false;
        Table.Columns.Add("Profit", typeof(double)).AllowDBNull = false;
        Table.Columns.Add("Quantity", typeof(int)).AllowDBNull = false;
        Table.Columns.Add("Status", typeof(LineStatus));
        Table.Columns.Add("Done", typeof(bool));
        
        var random = new Random(seed);
        var start = new DateTime(DateTime.Today.Year - 2, 1, 1);
        var days = Math.Max(1, (DateTime.Today - start).Days);

        for (var i = 0; i < count; i++)
        {
            var region = s_regions[random.Next(s_regions.Length)];
            var segment = s_segments[random.Next(s_segments.Length)];
            var category = random.Next(s_categories.Length) + 1; //s_categories[random.Next(s_categories.Length)];
            var product = s_productsByCategory[category - 1][random.Next(s_productsByCategory[category - 1].Length)];
            var orderDate = start.AddDays(random.Next(days));

            var quantity = random.Next(1, 12);
            var unitPrice = random.NextDouble() * 900 + 25;
            var sales = Math.Round(unitPrice * quantity, 2);
            var margin = random.NextDouble() * 0.35 - 0.05;
            var profit = Math.Round(sales * margin, 2);

            DataRow Row = Table.NewRow();
            Row["OrderDate"] = orderDate;
            Row["Region"] = region;
            Row["Segment"] = segment;
            Row["CategoryId"] = category;
            Row["Product"] = product;
            Row["Sales"] = sales;
            Row["Profit"] = profit;
            Row["Quantity"] = quantity;
            Row["Status"] = LineStatus.Active;
            Row["Done"] = false;
            Table.Rows.Add(Row);
        }


        return Table;
    }
    static DataTable CreateTableCategories()
    {
        DataTable Table = new DataTable("Category");
        Table.Columns.Add("Id", typeof(int));
        Table.Columns.Add("Name");

        int Id = 0;
        foreach (string S in s_categories)
        {
            Id++;
            DataRow Row = Table.NewRow();
            Row["Id"] = Id;
            Row["Name"] = S;
            Table.Rows.Add(Row);
        }
        
        return Table;
    }
    

    // ● public
    static public void Initialize(int ItemCount = 100)
    {
        SalesLines = CreatePocoSalesLines(ItemCount);
        Categories = CreatePocoCategories();

        tblSalesLines = CreateTableSalesLines(ItemCount);
        tblCategory = CreateTableCategories();
    }

    // ● properties
    static public List<SalesLine> SalesLines { get; private set; }
    static public List<Category> Categories { get; private set; }
    static public DataTable tblSalesLines { get; private set; }
    static public DataTable tblCategory { get; private set; }
 
}
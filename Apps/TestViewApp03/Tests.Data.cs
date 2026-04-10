using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;


namespace Tripous.Data;

public enum LineStatus
{
    None,
    New,
    Active,
    Done
}

public class CategoryLookupSourcePoco : ILookupSource
{
    public string Name { get; } = "Category";
    public IEnumerable Items { get; } = Tests.Categories;
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

public class CategoryLookupSourceDataView : ILookupSource
{
    public string Name { get; } = "Category";
    public IEnumerable Items { get; } = Tests.tblCategory.DefaultView;
}


static public partial class Tests
{
    static public readonly string[] s_regions = { "North", "South", "East", "West" };
    static public readonly string[] s_segments = { "Consumer", "Corporate", "Home Office" };
    static public readonly string[] s_categories = { "Furniture", "Office Supplies", "Technology" };
    static public readonly List<string[]> s_productsByCategory = new List<string[]>() {
        new[] { "Chair", "Desk", "Bookcase", "Table", "Sofa" },
        new[] { "Paper", "Binders", "Labels", "Pens", "Storage" },
        new[] { "Laptop", "Monitor", "Phone", "Printer", "Accessories" }
    };

    /*
    static public readonly Dictionary<string, string[]> s_productsByCategory = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Furniture"] = new[] { "Chair", "Desk", "Bookcase", "Table", "Sofa" },
        ["Office Supplies"] = new[] { "Paper", "Binders", "Labels", "Pens", "Storage" },
        ["Technology"] = new[] { "Laptop", "Monitor", "Phone", "Printer", "Accessories" }
    };
    */

        
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
        foreach (string S in Tests.s_categories)
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
    
 
    static GridViewDef CreateDefaultGridViewDef()
    {
        GridViewDef Result = new();

        Result.ShowGroupColumnsAsDataColumns = false;
        
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "OrderDate"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Region"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Segment"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "CategoryId"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Product"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Sales"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Profit"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Quantity"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Status"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Done"});

        Result["CategoryId"].GroupIndex = 0;
        Result["Product"].GroupIndex = 1;
        Result["Sales"].Aggregate = AggregateType.Sum;
 

        return Result;
    }

    // ● public
    static public void Initialize(int ItemCount = 100)
    {
        SalesLines = CreatePocoSalesLines(ItemCount);
        Categories = CreatePocoCategories();

        tblSalesLines = CreateTableSalesLines(ItemCount);
        tblCategory = CreateTableCategories();

        ViewDef = CreateDefaultGridViewDef();
    }

    // ● properties
    static public List<SalesLine> SalesLines { get; private set; }
    static public List<Category> Categories { get; private set; }
    static public DataTable tblSalesLines { get; private set; }
    static public DataTable tblCategory { get; private set; }
    static public GridViewDef ViewDef { get; private set; }


}
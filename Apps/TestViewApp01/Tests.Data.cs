using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
 

namespace Tripous.Data;

public enum LineStatus
{
    None,
    New,
    Active,
    Done
}

public sealed class SalesLine
{
    public DateTime OrderDate { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public double Sales { get; set; }
    public double Profit { get; set; }
    public int Quantity { get; set; }
    public LineStatus Status { get; set; }
}

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
    
    static public List<SalesLine> CreatePocoSalesLines(int count, int seed = 1729)
    {
        List<SalesLine> Result = new();
        
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
        Table.Columns.Add("Quantity", typeof(int));
        Table.Columns.Add("Status", typeof(LineStatus));
        
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
            Row["Status"] = LineStatus.Active;
        }


        return Table;
    }

    static public PivotDef CreateDefaultPivotDef()
    {
        PivotDef def = new PivotDef();

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Region",
            Axis = PivotAxis.Row,
            
            //SortDescending = true
        });

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Category",
            Axis = PivotAxis.Row
        });

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Segment",
            Axis = PivotAxis.Column
            
        });
        
        def.Columns.Add(new PivotColumnDef { FieldName = "Product", Axis = PivotAxis.Column });
        

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Sales",
            IsValue = true,
            ValueAggregateType = PivotValueAggregateType.Sum,
            Caption = "Sales",
            Format = "N2"
        });

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Profit",
            IsValue = true,
            ValueAggregateType = PivotValueAggregateType.Sum,
            Caption = "Profit",
            Format = "N2"
        });

        def.Columns.Add(new PivotColumnDef
        {
            FieldName = "Quantity",
            IsValue = true,
            ValueAggregateType = PivotValueAggregateType.Sum,
            Caption = "Qty",
            Format = "N0"
        });

        return def;
    }

    static public GridViewDef CreateDefaultGridViewDef()
    {
        GridViewDef Result = new();

        Result.ShowGroupColumnsAsDataColumns = false;
        
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "OrderDate"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Region"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Segment"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Category"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Product"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Sales"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Profit"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Quantity"});
        Result.Columns.Add(new GridViewColumnDef() { FieldName = "Status"});

        Result["Category"].GroupIndex = 0;
        Result["Product"].GroupIndex = 1;
        Result["Sales"].Aggregate = AggregateType.Sum;
 

        return Result;
    }
}
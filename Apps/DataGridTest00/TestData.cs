using System;
using System.Collections.Generic;
using System.Data;

namespace DataGridTest00;

public class Product
{
    public string Name { get; set; }
    public double Amount { get; set; }
    public bool Flag { get; set; }
    public int CategoryId { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

static public class TestData
{
    static readonly string[] s_categories = { "Furniture", "Office Supplies", "Technology" };
    static readonly List<string[]> s_productsByCategory = new List<string[]>() {
        new[] { "Chair", "Desk", "Bookcase", "Table", "Sofa" },
        new[] { "Paper", "Binders", "Labels", "Pens", "Storage" },
        new[] { "Laptop", "Monitor", "Phone", "Printer", "Accessories" }
    };
    
        
    static List<Product> CreatePocoProducts(int count, int seed = 1729)
    {
        List<Product> Result = new();
        
        var random = new Random(seed);

        for (var i = 0; i < count; i++)
        { 
            var CategoryId = random.Next(s_categories.Length) + 1; 
            var Name = s_productsByCategory[CategoryId - 1][random.Next(s_productsByCategory[CategoryId - 1].Length)];
            var Amount = random.NextDouble() * 900 + 25;

            Product Line = new Product
            {
                Name = Name,
                Amount = Amount,
                Flag = i % 3 == 0,
                CategoryId = CategoryId,
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
    
    static DataTable CreateTableProducts(int count, int seed = 1729)
    {
 
        DataTable Table = new DataTable("Products");
 
        Table.Columns.Add("Name", typeof(string));
        Table.Columns.Add("Amount", typeof(double)).AllowDBNull = false;
        Table.Columns.Add("Flag", typeof(bool));
        Table.Columns.Add("CategoryId", typeof(int));
        //Table.Columns.Add("Status", typeof(ProductType));
 
        
        var random = new Random(seed);
 

        for (var i = 0; i < count; i++)
        {
            var CategoryId = random.Next(s_categories.Length) + 1; 
            var Name = s_productsByCategory[CategoryId - 1][random.Next(s_productsByCategory[CategoryId - 1].Length)];
            var Amount = random.NextDouble() * 900 + 25;

            DataRow Row = Table.NewRow();
            Row["Name"] = Name;
            Row["Amount"] = Amount;
            Row["Flag"] =  i % 3 == 0;
            Row["CategoryId"] = CategoryId;
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
        Products = CreatePocoProducts(ItemCount);
        Categories = CreatePocoCategories();

        tblProduct = CreateTableProducts(ItemCount);
        tblCategory = CreateTableCategories();
    }

    // ● properties
    static public List<Product> Products { get; private set; }
    static public List<Category> Categories { get; private set; }
    static public DataTable tblProduct { get; private set; }
    static public DataTable tblCategory { get; private set; }
}
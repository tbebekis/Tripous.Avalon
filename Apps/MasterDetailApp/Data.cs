using System;
using System.Collections.Generic;
using System.Data;

namespace MasterDetailApp;

public class Customer
{
    public int Id { get; set; } 
    public string Name { get; set; } 
}
public class Order 
{ 
    public int Id { get; set; } 
    public int CustomerId { get; set; } 
    public DateTime OrderDate { get; set; } 
    public double Total { get; set; } 
    public string Status { get; set; } 
}

public static class MockData
{
    public static DataSet CreateDataSet()
    {
        DataSet Ds = new DataSet();
        
        // Customers table
        DataTable Customers = Ds.Tables.Add("Customers");
        Customers.Columns.Add("Id", typeof(int));
        Customers.Columns.Add("Name", typeof(string));
        
        Customers.Rows.Add(1, "Alpha Corp");
        Customers.Rows.Add(2, "Beta Systems");
        Customers.Rows.Add(3, "Gamma Solutions");
        
        // Orders table
        DataTable Orders = Ds.Tables.Add("Orders");
        Orders.Columns.Add("Id", typeof(int));
        Orders.Columns.Add("CustomerId", typeof(int)); // Relation field
        Orders.Columns.Add("OrderDate", typeof(DateTime));
        Orders.Columns.Add("Total", typeof(double));
        Orders.Columns.Add("Status", typeof(string)); // Filter field (e.g. "Pending", "Shipped")
        
        // Data for Alpha Corp (Id 1)
        Orders.Rows.Add(101, 1, DateTime.Now.AddDays(-10), 1500.0, "Shipped");
        Orders.Rows.Add(102, 1, DateTime.Now.AddDays(-5), 2500.0, "Pending");
        
        // Data for Beta Systems (Id 2)
        Orders.Rows.Add(201, 2, DateTime.Now.AddDays(-2), 450.0, "Shipped");
        return Ds;
    }
    
    public static List<Customer> GetCustomers() => new List<Customer>
    {
        new Customer { Id = 1, Name = "Alpha Corp" },
        new Customer { Id = 2, Name = "Beta Systems" }
    };
    public static List<Order> GetOrders() => new List<Order>
    {
        new Order { Id = 101, CustomerId = 1, OrderDate = DateTime.Now, Total = 120, Status = "Shipped" },
        new Order { Id = 102, CustomerId = 1, OrderDate = DateTime.Now, Total = 80, Status = "Pending" },
        new Order { Id = 201, CustomerId = 2, OrderDate = DateTime.Now, Total = 300, Status = "Shipped" }
    };
}


    
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Data;
using AvaloniaEdit.Utils;

namespace DataApp00;

public class Country
{
    public string Id { get; set; }
    public string Name { get; set; }

    static public List<Country>  GetList()
    {
        List<Country> List = new();
        
        List.Add(new  Country{Id = "GR", Name = "Greece" });
        List.Add(new  Country{Id = "US", Name = "United States" });
        List.Add(new  Country{Id = "UK", Name = "United Kingdom" });
        return List;
    }
} 

public class Customer
{
 
    public string Id { get; set; }
    public string Name { get; set; }
    public string CountryId  { get; set; }
    
    static public List<Customer> GetList()
    {
        List<Customer> List = new();
        
        List.Add(new  Customer{Id = "1", Name = "Theo", CountryId = "GR" });
        List.Add(new  Customer{Id = "2", Name = "George" , CountryId = "US" });
        List.Add(new  Customer{Id = "3", Name = "Nick" , CountryId = "GR" });
        List.Add(new  Customer{Id = "4", Name = "John" , CountryId = "UK" });
        
        return List;
    }
}

static public class Tests
{
    static public DataTable CreateCountryTable()
    {
        DataTable Table = new();
        
        Table.Columns.Add("Id", typeof(string));
        Table.Columns.Add("Name", typeof(string));
       
        
        Table.Rows.Add("GR", "Greece");
        Table.Rows.Add("US", "United States");
        Table.Rows.Add("UK", "United Kingdom");
        
        return Table;
    }
    
    static public DataTable CreateCustomerTable()
    {
        DataTable Table = new();
        
        Table.Columns.Add("Id", typeof(string));
        Table.Columns.Add("Name", typeof(string));
        Table.Columns.Add("CountryId", typeof(string));
        
        Table.Rows.Add("1", "Theo", "GR");
        Table.Rows.Add("2", "George", "US");
        Table.Rows.Add("3", "Nick", "GR");
        Table.Rows.Add("4", "John", "UK");
        
        return Table;
    }
}
 

 

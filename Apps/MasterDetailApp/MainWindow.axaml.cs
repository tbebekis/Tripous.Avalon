using Avalonia.Controls;
using Tripous.Avalon.Data;
using Tripous.Avalon.Controls;
using System;
using System.Linq;

namespace MasterDetailApp;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    string[] StatusList = new[] { "Pending", "Shipped", "Cancelled" };
    BindingSource bsMaster;
    BindingSource bsDetail;
    
    // ● event handlers
    void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        
        if (btnInsertMaster == sender)
            AddMaster();
        else if (btnDeleteMaster == sender)
            DeleteMaster();
        else if (btnInsertDetail == sender)
            AddDetail();
        else if (btnDeleteDetail == sender)
            DeleteDetail();
    }

    void AddMaster()
    {
        /* 1. Create the new row via BindingSource */
        var NewRow = bsMaster.NewRow();
        
        /* 2. Manual assignment from entry controls (No Data Binding) */
        NewRow["Id"] = edtIdMaster.Text;
        NewRow["Name"] = edtName.Text;
        
        bsMaster.AddRow(NewRow);
    }
    
    void DeleteMaster()
    {
        /* Deletes the current master row */
        bsMaster.Delete();
    }

    void AddDetail()
    {
        /* 1. Ensure we have a master record first */
        if (bsMaster.Current == null) return;

        var NewRow = bsDetail.NewRow();

        /* 2. Manual assignment from entry controls */
        NewRow["Id"] = edtIdDetail.Text;
        NewRow["OrderDate"] = edtOrderDate.SelectedDate?.DateTime ?? DateTime.Now;
        NewRow["Total"] = double.TryParse(edtTotal.Text, out double total) ? total : 0.0;
        NewRow["Status"] = cboStatus.SelectedItem?.ToString() ?? "Pending";
        
        bsDetail.AddRow(NewRow);
        
        /* 3. Crucial: The CustomerId must match the current Master's Id */
        //NewRow["CustomerId"] = bsMaster.Current["Id"];

        // a detail BindingSource needs to know that the insert/edit is ended
        // in order to apply the master-detail and/or the filter
        //bsDetail.UpdateVisibleRows();
        
        
    }

    void DeleteDetail()
    {
        /* Deletes the current detail row */
        bsDetail.Delete();
    }
    
    // ● private
    void WindowInitialize()
    {
        /* Initialize data sources (choose DataTableTest or ListTest) */
        DataTableTest();

        cboStatus.ItemsSource = StatusList;

        /* Setup Filters for real-time searching */
        edtFilterMaster.TextChanged += (s, e) => 
        {
   
            string text = edtFilterMaster.Text;
            if (string.IsNullOrWhiteSpace(text))
                bsMaster.CancelFilter();
            else
                bsMaster.SetFilter("Name", text); // Now this performs a 'Contains' search!
        };

        edtFilterDetail.TextChanged += (s, e) => 
        {
            if (string.IsNullOrWhiteSpace(edtFilterDetail.Text))
                bsDetail.CancelFilter();
            else
                bsDetail.SetFilter("Status", edtFilterDetail.Text);
        };
        
        /* Bind DataGrids to BindingSource row collections */
        bsMaster.Bind(gridMaster, true);
        bsDetail.Bind(gridDetail, true);
 
        /* Establish master-detail relationship between BindingSources */
        bsMaster.AddDetail("CustOrders", "Id", bsDetail, "CustomerId");
        bsMaster.ActivateDetails(true);
        
        /* Wire up buttons */
        btnInsertMaster.Click += AnyClick;
        btnDeleteMaster.Click += AnyClick;
        btnInsertDetail.Click += AnyClick;
        btnDeleteDetail.Click += AnyClick;
        
        /* Initial selection */
        bsMaster.First();
    }

    void DataTableTest()
    {
        /* Loads data using ADO.NET DataTables */
        var ds = MockData.CreateDataSet();
        bsMaster = BindingSource.FromTable(ds.Tables["Customers"]);
        bsDetail = BindingSource.FromTable(ds.Tables["Orders"]);
    }

    void ListTest()
    {
        /* Loads data using POCO lists and Reflection-based sources */
        var customers = MockData.GetCustomers();
        var orders = MockData.GetOrders();
        
        bsMaster = BindingSource.FromList(customers);
        bsDetail = BindingSource.FromList(orders);
    }

    public MainWindow()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) =>
        {
            if (IsWindowInitialized)
                return;
            WindowInitialize();
            IsWindowInitialized = true;
        };
    }
}
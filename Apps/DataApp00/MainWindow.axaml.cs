using Avalonia.Controls;
using Tripous.Avalon.Data;
using Tripous.Avalon.Controls;
 

namespace DataApp00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private BindingSource dsCustomer;
    private BindingSource dsCountry;
    
    // ● event handlers
    void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is null)
            return;
 
        if (btnInsert == sender)
            Insert();
        else if (btnDelete == sender)
            Delete();
    }

    void ListTest()
    {
        dsCustomer = BindingSource.FromList(Customer.GetList()); // new DataSource(Customer.GetList());
        dsCustomer.Bind(edtName, "Name");
        dsCustomer.Bind(gridCustomers, true);
        dsCountry = BindingSource.FromList(Country.GetList()); 
        dsCustomer.Bind(cboCountry, dsCountry, "Name", "Id",  "CountryId");
    }

    void DataTableTest()
    {
        dsCustomer = BindingSource.FromTable(Tests.CreateCustomerTable()); // new DataSource(Customer.GetList());
        dsCustomer.Bind(edtName, "Name");
        dsCustomer.Bind(gridCustomers, true);
        dsCountry = BindingSource.FromTable(Tests.CreateCountryTable()); 
        dsCustomer.Bind(cboCountry, dsCountry, "Name", "Id",  "CountryId");
    }
    
    // ● private
    void WindowInitialize()
    {
        // ListTest()
        DataTableTest();
    }

    void Insert()
    {
        BindingSourceRow Row = dsCustomer.NewRow();
        Row["Id"] = edtNewId.GetText();
        Row["Name"] = edtNewName.GetText();
        Row["CountryId"] = edtNewCountryId.GetText();
        dsCustomer.AddRow(Row);
    }

    void Delete()
    {
        dsCustomer.Delete();
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
using Avalonia.Controls;
using Tripous.Avalon;

namespace DataApp00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private DataSource dsCustomer;
    private DataSource dsCountry;
    
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
        dsCustomer = DataSource.FromList(Customer.GetList()); // new DataSource(Customer.GetList());
        dsCustomer.Bind(edtName, "Name");
        dsCustomer.Bind(gridCustomers, true);
        dsCountry = DataSource.FromList(Country.GetList()); 
        dsCustomer.Bind(cboCountry, dsCountry, "Name", "Id",  "CountryId");
    }

    void DataTableTest()
    {
        dsCustomer = DataSource.FromTable(Tests.CreateCustomerTable()); // new DataSource(Customer.GetList());
        dsCustomer.Bind(edtName, "Name");
        dsCustomer.Bind(gridCustomers, true);
        dsCountry = DataSource.FromTable(Tests.CreateCountryTable()); 
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
        DataSourceRow Row = dsCustomer.Add();
        Row["Id"] = edtNewId.GetText();
        Row["Name"] = edtNewName.GetText();
        Row["CountryId"] = edtNewCountryId.GetText();
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
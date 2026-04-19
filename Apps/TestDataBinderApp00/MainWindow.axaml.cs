using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Tripous;
using Tripous.Avalon;
using Tripous.Data;

namespace TestDataBinderApp00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private DataSource DS;
    private DataBinder Binder;
    private DataGridHost GridHost;

    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
 
    }

    // ● private
    void WindowInitialize()
    {
        TestData.Initialize(50);
        
        btnFirst.Click += (s, ea) =>
        {
            DS.MoveFirst();
            EnableCommands();
        };
        btnNext.Click += (s, ea) =>
        {
            DS.MoveNext();
            EnableCommands();
        };
        btnPrior.Click += (s, ea) =>
        {
            DS.MovePrevious();
            EnableCommands();
        };
        btnLast.Click += (s, ea) =>
        {
            DS.MoveLast();
            EnableCommands();
        };
        btnNew.Click += (s, ea) =>
        {
            var Row = Binder.CreateNew();
            Row["Region"] = "Oti nanai";
            Row["Segment"] = "Οτι νάναι";
            Row["Sales"] = 0m;
            Row["Profit"] = 0m;
            Row["Quantity"] = 1;
            Row["CategoryId"] = 2;
            
            Binder.Add(Row);
            EnableCommands();
        };
        btnDelete.Click += (s, ea) =>
        {
            DS.DeleteCurrent();
            EnableCommands();
        };
        
        Test_DataView();
        Bind();
        EnableCommands();
    }
    void EnableCommands()
    {
        btnFirst.IsEnabled = !DS.IsBof;
        btnNext.IsEnabled = !DS.IsEof;
        btnPrior.IsEnabled = !DS.IsBof;
        btnLast.IsEnabled = !DS.IsEof;
    }
    void Bind()
    {
        Binder = new DataBinder(DS);
        Binder.Bind(edtRegion, "Region");
        Binder.Bind(edtSegment, "Segment");
        //cboRegion.ItemsSource = TestData.s_regions;
        Binder.Bind(cboCategory, "Category", "CategoryId", "Name", "Id", AllowNullSelection: true);
        Binder.Bind(gridSales);
        
        GridHost = new DataGridHost(gridSales);
        GridHost.Initialize();
    }

    void Test_DataView()
    {
        DS = DataViewSource.Create(TestData.tblSalesLines.DefaultView);
        DS.Lookups.Add(new CategoryLookupSourceDataView());
    }
    void Test_Poco()
    {
    }
 
    // ● construction
    public MainWindow()
    {
        InitializeComponent();
        Ui.MainWindow = this;
        this.Loaded += (s, e) =>
        {
            if (IsWindowInitialized)
                return;
            WindowInitialize();
            IsWindowInitialized = true;
        };
    }
}
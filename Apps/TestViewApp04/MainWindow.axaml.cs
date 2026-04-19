using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Tripous;
using Tripous.Avalon;
using Tripous.Data;
namespace TestViewApp04;
 
public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
 

    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
 
    }

    // ● private
    void WindowInitialize()
    {
        Tests.Initialize(50);
        Test_Poco();
    }
 
    void Test_Poco()
    {
        List<SalesLine> List = Tests.SalesLines;
        ucGridView.ViewDef = GridViewDef.Create(typeof(SalesLine));
        ucGridView.SetSource(List);
    }
    void Test_DataView()
    {
        DataView DataView = Tests.tblSalesLines.DefaultView;
        ucGridView.DataView = DataView;
        ucGridView.ViewDef = GridViewDef.Create(DataView);
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
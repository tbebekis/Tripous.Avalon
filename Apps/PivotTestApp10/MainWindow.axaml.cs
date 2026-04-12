using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;

using Tripous.Data;
using Tripous.Avalon;

namespace PivotTestApp10;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    
    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (btnTest == sender)
            Test_Poco();
        else if (btnTest2 == sender)
            Test_DataView();
    }
    
    // ● private
    void WindowInitialize()
    {
        Test_DataView2();
    }

    private int LineCount = 300;
    
    void Test_Poco()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        List<SalesLine> source = Tests.CreatePocoSalesLines(LineCount);
        PivotData Result = PivotEngine.Execute(source, PivotDef);
        PivotGridRenderer.Show(Grid, Result,  PivotDef);
    }

    void Test_Poco2()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        List<SalesLine> Source = Tests.CreatePocoSalesLines(LineCount);
        
        PivotView PivotView = PivotView.Create(Grid, Source, PivotDef);
        PivotView.Menu.IsEnabled = true;
    }
    void Test_DataView()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        DataTable table = Tests.CreateTableSalesLines(LineCount);
        PivotData Result = PivotEngine.Execute(table.DefaultView, PivotDef);
        PivotGridRenderer.Show(Grid, Result, PivotDef);
    }
    void Test_DataView2()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        DataTable Table = Tests.CreateTableSalesLines(LineCount);
        
        PivotView PivotView = PivotView.Create(Grid, Table.DefaultView, PivotDef);
        PivotView.Menu.IsEnabled = true;
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
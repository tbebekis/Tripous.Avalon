using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using Tripous.Avalon;
using Tripous.Data;

namespace PivotTestApp08;


public partial class MainWindow : Window
{
    
    bool IsWindowInitialized = false;

    // ● event handlers
    void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btnTest == sender)
            Test();
        else if (btnTest2 == sender)
            Test2();
    }

    // ● private
    void WindowInitialize()
    {
    }

    void Test()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        List<SalesLine> source = Tests.CreatePocoSalesLines(5000);
        PivotData Result = PivotEngine.Execute(source, PivotDef);
        PivotGridRenderer.Show(Grid, Result);
    }

    void Test2()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        DataTable table = Tests.CreateTableSalesLines(5000);
        PivotData Result = PivotEngine.Execute(table.DefaultView, PivotDef);
        PivotGridRenderer.Show(Grid, Result);
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
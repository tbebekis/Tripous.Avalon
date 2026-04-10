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
            Test();
        else if (btnTest2 == sender)
            Test2();
    }
    
    // ● private
    void WindowInitialize()
    {
        Test2();
    }

    private int LineCount = 300;
    
    void Test()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        List<SalesLine> source = Tests.CreatePocoSalesLines(LineCount);
        PivotData Result = PivotEngine.Execute(source, PivotDef);
        PivotGridRenderer.Show(Grid, Result,  PivotDef);
    }

    void Test2()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        DataTable table = Tests.CreateTableSalesLines(LineCount);
        PivotData Result = PivotEngine.Execute(table.DefaultView, PivotDef);
        PivotGridRenderer.Show(Grid, Result, PivotDef);
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
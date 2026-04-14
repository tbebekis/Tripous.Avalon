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
            Test_Poco2();
        else if (btnTest2 == sender)
            Test_DataView2();
    }
    
    // ● private
    void WindowInitialize()
    {
        Test_DataView2();
    }

    private int LineCount = 300;
 
    void Test_Poco2()
    {
        PivotViewDef PivotViewDef = Tests.CreateDefaultPivotDef();
        List<SalesLine> Source = Tests.CreatePocoSalesLines(LineCount);
        
        PivotView PivotView = PivotView.Create(Grid, Source, PivotViewDef);

        PivotView.ToolBar.Panel = pnlToolBar;
        PivotView.ToolBar.IsMultiDef = true;
        //PivotView.ToolBar.IsReadOnlyView = true;
        PivotView.Menu.IsEnabled = true;
    }
    void Test_DataView2()
    {
        PivotViewDef PivotViewDef = Tests.CreateDefaultPivotDef();
        DataTable Table = Tests.CreateTableSalesLines(LineCount);
        
        PivotView PivotView = PivotView.Create(Grid, Table.DefaultView, PivotViewDef);
 
        PivotView.ToolBar.Panel = pnlToolBar;
        PivotView.ToolBar.IsMultiDef = true;
        //PivotView.ToolBar.IsReadOnlyView = true;
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
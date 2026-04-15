using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;

using Tripous.Data;
using Tripous.Avalon;

namespace PivotTestApp10;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    const int LineCount = 300;
    DataTable Table = Tests.CreateTableSalesLines(LineCount);
    List<SalesLine> EnumerableSource = Tests.CreatePocoSalesLines(LineCount);
    PivotViewDef Def = Tests.CreateDefaultPivotDef();
    PivotView PivotView = new();
    
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
        PivotView.Grid = Grid;
        PivotView.ViewDef = Def;
        
        PivotView.ToolBar.Panel = pnlToolBar;
        PivotView.ToolBar.IsMultiDef = true;
        //PivotView.ToolBar.IsReadOnlyView = true;
        PivotView.Menu.IsEnabled = true;
        
        Test_DataView();
    }

    
 
    void Test_Poco()
    {
        PivotView.SetSource(EnumerableSource);
    }

    void Test_DataView()
    {
        PivotView.DataView = Table.DefaultView;
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
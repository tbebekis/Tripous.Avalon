using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace PivotTestApp;

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
        ObservableCollection<SalesLine> Data = Tests.CreatePocoSalesLines(900);
        Tests.PocoTest(Grid, PivotDef, typeof(SalesLine), Data);
    }

    void Test2()
    {
        PivotDef PivotDef = Tests.CreateDefaultPivotDef();
        DataView DataView = Tests.CreateTableSalesLines(5000).DefaultView;
        Tests.DataTableTest(Grid, PivotDef, DataView);
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
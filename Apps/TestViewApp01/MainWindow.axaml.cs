using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using Tripous;
using Tripous.Avalon;
using Tripous.Data;
namespace TestViewApp01;
 
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
        //GridViewDef Def = Tests.CreateDefaultGridViewDef();
        //List<SalesLine> Data = Tests.CreatePocoSalesLines(200);
        //GridViewData ViewData = GridViewEngine.Execute(Data, Def);

    }

    void Test2()
    {
        //GridViewDef Def = Tests.CreateDefaultGridViewDef();
        //DataTable Data = Tests.CreateTableSalesLines(100);
        //GridViewData ViewData = GridViewEngine.Execute(Data.DefaultView, Def);
        //GridViewGridBinder.Apply(gridView, ViewData, Def);
 
        DataTable Table = Tests.CreateTableSalesLines(100);
        GridViewDef Def = GridViewEngine.CreateDefaultDef(Table.DefaultView);
        Def["Category"].GroupIndex = 0;
        Def["Product"].GroupIndex = 1;
        Def["Sales"].Aggregate = AggregateType.Sum;
        
        GridViewController Controller = new(Table.DefaultView, Def);
        GridViewGridBinder.Bind(gridView, Controller);
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
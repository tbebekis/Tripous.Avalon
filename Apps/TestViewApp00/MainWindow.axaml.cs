using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;

using Tripous.Avalon;
using Tripous.Data;
namespace TestViewApp00;
 
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
        GridViewDef Def = Tests.CreateDefaultGridViewDef();
        List<SalesLine> Data = Tests.CreatePocoSalesLines(200);
        GridViewData ViewData = GridViewEngine.Execute(Data, Def);
        //edtLog.Text = GridViewDiagnosticRenderer.RenderText(ViewData);
        
        //var RenderData = GridViewRenderer.Render(ViewData, Def);
        //GridViewGridBinder.Apply(gridView, RenderData);
    }

    void Test2()
    {
        GridViewDef Def = Tests.CreateDefaultGridViewDef();
        DataTable Data = Tests.CreateTableSalesLines(100);
        GridViewData ViewData = GridViewEngine.Execute(Data.DefaultView, Def);
 
 
        //var RenderData = GridViewRenderer.Render(ViewData, Def);
        //GridViewGridBinder.Apply(gridView, RenderData, Def);
        
        
        GridViewGridBinder.Apply(gridView, ViewData, Def);
 
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
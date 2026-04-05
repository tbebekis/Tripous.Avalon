using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using Avalonia.Threading;
using Tripous;
using Tripous.Avalon;
using Tripous.Data;
namespace TestViewApp02;
 
public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private GridViewController Controller;
    private DataView DataView;

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
        Tests.Initialize(150);
        Test2();
    }

    void Test()
    {
        GridViewDef Def = GridViewEngine.CreateDefaultDef(typeof(SalesLine));
        Def.ShowGroupColumnsAsDataColumns = true;
        
        Def["Product"].GroupIndex = 0;
        Def["Sales"].Aggregate = AggregateType.Sum;
        Def["Sales"].DisplayFormat = "N3";

        //Def["CategoryId"].GroupIndex = 1;
        Def["CategoryId"].Title = "Category";
        Def["CategoryId"].ValueMember = "Id";
        Def["CategoryId"].DisplayMember = "Name";
        Def["CategoryId"].LookupItemsSource = Tests.Categories;
        
        Controller = new();
        Controller.Open(Tests.SalesLines, Def);
        GridViewGridBinder.Bind(gridView, Controller);
    }

    void TestValues()
    {
        var R = Controller.Rows[2];
        R.SetValue("Sales", 999999m);
        //gridView.CommitEdit();
        Controller.Position = 4; //.MoveTo(2);
    }

    void Test2()
    {
        DataView = Tests.tblSalesLines.DefaultView;
        
        GridViewDef Def = GridViewEngine.CreateDefaultDef(DataView);
        Def.ShowGroupColumnsAsDataColumns = true;
        
        Def["CategoryId"].GroupIndex = 0;
        Def["Product"].GroupIndex = 1;
        
        Def["Sales"].Aggregate = AggregateType.Sum;
        Def["Sales"].DisplayFormat = "N3";
        
        //Def["CategoryId"].GroupIndex = 1;
        Def["CategoryId"].Title = "Category";
        Def["CategoryId"].ValueMember = "Id";
        Def["CategoryId"].DisplayMember = "Name";
        Def["CategoryId"].LookupItemsSource = Tests.tblCategory.DefaultView;
        
        Controller = new(DataView, Def);
         
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
using Avalonia.Controls;
using Tripous;
using Tripous.Data;

namespace TestApp00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
 
    
    // ● event handlers
    void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btnTest == sender)
            Test();
    }
    
    // ● private
    void WindowInitialize()
    {
        
    }

    void Test()
    {
        RowFilterItemList RowFilterList = new();

        //RowFilterList.Add(BoolOp.And, ConditionOp.Equal, "Customer", "Takis");
        //RowFilterList.Add(BoolOp.AndNot, ConditionOp.GreaterOrEqual, "Amount", 123.5m);
        
        RowFilterList.Add(BoolOp.And, ConditionOp.Equal, "Customer", "Takis");
        RowFilterList.Add(BoolOp.And, ConditionOp.GreaterOrEqual, "Amount", 100);
        RowFilterList.Add(BoolOp.And, ConditionOp.LessOrEqual, "Amount", 500);
        RowFilterList.Add(BoolOp.AndNot, ConditionOp.Null, "DeletedOn", null);
        RowFilterList.Add(BoolOp.And, ConditionOp.Like, "Customer", "%Νίκος%");
        RowFilterList.Add(BoolOp.And, ConditionOp.In, "Status", new[] { "Open", "Closed", "Pending" });

        //edtLog.Text = RowFilterList.Text;

        string JsonText = Json.Serialize(RowFilterList);
        //edtLog.Text = JsonText;
        
        RowFilterList = Json.Deserialize<RowFilterItemList>(JsonText);
        JsonText = Json.Serialize(RowFilterList);
        edtLog.Text = JsonText;
    }

    void Test2()
    {
        
    }
    
    // ● construction
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
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Tripous;
using Tripous.Avalon;
using Tripous.Data;
namespace TestViewApp03;
 
public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private ToolBar ToolBar;
    private GridView GridView;

    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btnTest_DataView == sender)
            Test_DataView();
        else if (btnTest_Poco == sender)
            Test_Poco();
        else if (btnCloseGridView == sender)
            CloseGridView();
        else if (btnTestExport == sender)
            await TestExport();
    }

    // ● private
    void WindowInitialize()
    {
        //CreateToolBar();
        Tests.Initialize(50);
 
        Test_DataView();
    }

    void CreateToolBar()
    {
        ToolBar = new ToolBar();
        ToolBar.Panel = pnlToolBar;
        
        Button btnClose = ToolBar.AddButton("door_out.png", "Exit", (s,e) => Close());
        ToolBar.AddSeparator();
        ToolBar.AddButton("application_add.png", "App", (s,e) => Close());

        ContextMenu Menu = new ContextMenu();
        MenuItem mnuItem1 = Menu.AddMenuItem("Ena Menu",
            async (sender, args) => await MessageBox.Info("Hi there")); // Menu.Items.Add(new MenuItem() { Header = "Ενα Μενού" });
        MenuItem mnuItem2 = Menu.AddMenuItem("Κι άλλο Μενού",
            async (sender, args) => await MessageBox.Info("Hi there mnuItem2")); 
    
        ToolBar.AddDropDownButton("application_add.png", "Menu", Menu,
            (sender, args) =>
            {
                mnuItem2.IsEnabled = false; 
            });

        ToolBar.AddComboBox(new string[] { "one", "two", "three" });
    }
    void TestValues()
    {
        if (GridView != null)
        {
            var R = GridView.Rows[2];
            R.SetValue("Sales", 999999m);
            GridView.Position = 4; //.MoveTo(2); 
        }
    }

    void CloseGridView()
    {
        if (GridView != null)
            GridView.Close();
        pnlToolBar.Children.Clear();
    }
    void Test_Poco()
    {
        CloseGridView();
        
        GridViewDefs ViewDefs = new();
        GridViewDef Def = ViewDefs.Add(typeof(SalesLine));  
        Def.ShowGroupColumnsAsDataColumns = true;
        
        Def["Product"].GroupIndex = 0;
        Def["CategoryId"].GroupIndex = 1;
        
        Def["Sales"].Aggregate = AggregateType.Sum;
        Def["Sales"].DisplayFormat = "N3";
        
        Def["CategoryId"].Title = "Category";
        Def["CategoryId"].ValueMember = "Id";
        Def["CategoryId"].DisplayMember = "Name";
        Def["CategoryId"].LookupItemsSource = Tests.Categories;
        
        GridView = new GridView();
        GridView.Grid = gridView;
        GridView.SetSource(Tests.SalesLines);
        GridView.ViewDefs = ViewDefs;
        GridView.ToolBar.Panel = pnlToolBar;
        GridView.ToolBar.IsMultiDef = true;
    }
    void Test_DataView()
    {
        CloseGridView();
        
        DataView DataView = Tests.tblSalesLines.DefaultView;
        
        GridViewDefs ViewDefs = new();
        GridViewDef Def = ViewDefs.Add(DataView); 
        Def.ShowGroupColumnsAsDataColumns = true;
        
        Def["CategoryId"].GroupIndex = 0;
        Def["Product"].GroupIndex = 1;
        
        Def["Sales"].Aggregate = AggregateType.Sum;
        Def["Sales"].DisplayFormat = "N3";
 
        Def["CategoryId"].Title = "Category";
        Def["CategoryId"].ValueMember = "Id";
        Def["CategoryId"].DisplayMember = "Name";
        Def["CategoryId"].LookupItemsSource = Tests.tblCategory.DefaultView;

        GridView = new GridView();
        GridView.Grid = gridView;
        GridView.DataView = DataView;
        GridView.ViewDefs = ViewDefs;
        GridView.ToolBar.Panel = pnlToolBar;
        GridView.ToolBar.IsMultiDef = true;
        //GridView.ToolBar.IsReadOnlyView = true;
    }

    async Task TestExport()
    {
        string FilePath = await Ui.SaveFileDialog(this,"html");
        if (!string.IsNullOrWhiteSpace(FilePath))
        {
            GridViewExportOptions Options = new();
            Options.Format = GridViewExportFormat.HtmlGrouped;
            Options.ExportFilePath = FilePath;
            GridViewExporter.Export(GridView, Options);
        }
        
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
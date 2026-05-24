namespace TestBindingApp;

static public class Test_RowItemBinding
{
    // ● private
    static ObservableCollection<RowItem> fItems;
    static bool fBindingAssigned;

    static void PrepareItems()
    {
        if (fItems != null)
            return;

        fItems = new();

        foreach (DataRowView RowView in Tests.tblCustomer.DefaultView)
            fItems.Add(new RowItem(RowView));
    }
    static void EnsureBinding()
    {
        if (!fBindingAssigned)
            Execute();
    }
    static void BindSimpleControls()
    {
        TextBox edtId = Tests.MainWindow.FindControl<TextBox>("edtId");
        TextBox edtCode = Tests.MainWindow.FindControl<TextBox>("edtCode");
        TextBox edtName = Tests.MainWindow.FindControl<TextBox>("edtName");
        TextBox edtCountryId = Tests.MainWindow.FindControl<TextBox>("edtCountryId");
        TextBox edtCountryCode = Tests.MainWindow.FindControl<TextBox>("edtCountry__Code");
        TextBox edtCountryName = Tests.MainWindow.FindControl<TextBox>("edtCountry__Name");
        TextBox edtIsActive = Tests.MainWindow.FindControl<TextBox>("edtIsActive");

        if (fItems.Count == 0)
            return;

        RowItem Item = fItems[0];

        edtId.Bind(TextBox.TextProperty, new Binding("Id") { Source = Item });
        edtCode.Bind(TextBox.TextProperty, new Binding("Code") { Source = Item, Mode = BindingMode.TwoWay });
        edtName.Bind(TextBox.TextProperty, new Binding("Name") { Source = Item, Mode = BindingMode.TwoWay });
        edtCountryId.Bind(TextBox.TextProperty, new Binding("CountryId") { Source = Item, Mode = BindingMode.TwoWay });
        edtCountryCode.Bind(TextBox.TextProperty, new Binding("Country__Code") { Source = Item, Mode = BindingMode.TwoWay });
        edtCountryName.Bind(TextBox.TextProperty, new Binding("Country__Name") { Source = Item, Mode = BindingMode.TwoWay });
        edtIsActive.Bind(TextBox.TextProperty, new Binding("IsActive") { Source = Item, Mode = BindingMode.TwoWay });
    }
    static void AddTextColumn(DataGrid Grid, string Header, string Path)
    {
        DataGridTextColumn Column = new();
        Column.Header = Header;
        Column.Binding = new Binding(Path);
        Grid.Columns.Add(Column);
    }
    static void AddCheckBoxColumn(DataGrid Grid, string Header, string Path)
    {
        DataGridCheckBoxColumn Column = new();
        Column.Header = Header;
        Column.Binding = new Binding(Path);
        Grid.Columns.Add(Column);
    }
    static void ChangeByRowItem()
    {
        if (fItems == null || fItems.Count == 0)
            return;

        RowItem Item = fItems[0];

        Item["Code"] = "RI-001";
        Item["Name"] = "Changed by RowItem";
        Item["CountryId"] = 500;
        Item["Country__Code"] = "PT";
        Item["Country__Name"] = "Portugal";
        Item["IsActive"] = false;
    }
    static void ChangeByDataRowView()
    {
        if (Tests.tblCustomer.DefaultView.Count == 0)
            return;

        DataRowView RowView = Tests.tblCustomer.DefaultView[0];

        RowView["Code"] = "RI-DRV-001";
        RowView["Name"] = "Changed by DataRowView";
        RowView["CountryId"] = 600;
        RowView["Country__Code"] = "SE";
        RowView["Country__Name"] = "Sweden";
        RowView["IsActive"] = true;
    }
    static void ChangeByDataRow()
    {
        if (Tests.tblCustomer.Rows.Count == 0)
            return;

        DataRow Row = Tests.tblCustomer.Rows[0];

        Row["Code"] = "RI-DR-001";
        Row["Name"] = "Changed by DataRow";
        Row["CountryId"] = 700;
        Row["Country__Code"] = "DK";
        Row["Country__Name"] = "Denmark";
        Row["IsActive"] = false;
    }

    // ● public
    static public void Execute()
    {
        if (fBindingAssigned)
            return;

        DataGrid Grid = Tests.MainWindow.FindControl<DataGrid>("gridList");

        if (Grid == null)
        {
            Tests.Log("gridList not found.");
            return;
        }

        PrepareItems();

        Grid.AutoGenerateColumns = false;
        Grid.Columns.Clear();

        AddTextColumn(Grid, "Id", "Id");
        AddTextColumn(Grid, "Code", "Code");
        AddTextColumn(Grid, "Name", "Name");
        AddTextColumn(Grid, "Country Id", "CountryId");
        AddTextColumn(Grid, "Country Code", "Country__Code");
        AddTextColumn(Grid, "Country Name", "Country__Name");
        AddCheckBoxColumn(Grid, "Is Active", "IsActive");

        Grid.ItemsSource = fItems;
        BindSimpleControls();
        fBindingAssigned = true;
        Tests.Log("RowItem binding: grid and simple controls assigned.");
    }
    static public void Execute_ChangeByRowItem()
    {
        Tests.Log("TEST: Execute_ChangeByRowItem");
        EnsureBinding();
        ChangeByRowItem();
        Tests.Log("RowItem binding: changed first row through RowItem.");
        Tests.Log("-------------------------------------------------------------");
    }
    static public void Execute_ChangeByDataRowView()
    {
        Tests.Log("TEST: Execute_ChangeByDataRowView");
        EnsureBinding();
        ChangeByDataRowView();
        Tests.Log("RowItem binding: changed first row through DataRowView.");
        Tests.Log("-------------------------------------------------------------");
    }
    static public void Execute_ChangeByDataRow()
    {
        Tests.Log("TEST: Execute_ChangeByDataRow");
        EnsureBinding();
        ChangeByDataRow();
        Tests.Log("RowItem binding: changed first row through DataRow.");
        Tests.Log("-------------------------------------------------------------");
    }
}

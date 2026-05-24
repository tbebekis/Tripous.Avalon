namespace TestBindingApp;

static public class Test_DataRowViewTemplateColumn
{
    static public void Execute()
    {
        // ---------------------------------------------------------------
        void AddDataRowViewTextColumn(DataGrid Grid, string Header, string FieldName)
        {
            DataGridTemplateColumn Column = new();
            Column.Header = Header;
            Column.CellTemplate = new FuncDataTemplate<DataRowView>((RowView, Namescope) =>
            {
                TextBlock TextBlock = new();
                TextBlock.Text = RowView == null ? string.Empty : RowView[FieldName].ToString();
                return TextBlock;
            });
            Grid.Columns.Add(Column);
        }
        // ---------------------------------------------------------------
        void AddDataRowViewCheckBoxColumn(DataGrid Grid, string Header, string FieldName)
        {
            DataGridTemplateColumn Column = new();
            Column.Header = Header;
            Column.CellTemplate = new FuncDataTemplate<DataRowView>((RowView, Namescope) =>
            {
                CheckBox CheckBox = new();
                CheckBox.IsHitTestVisible = false;
                CheckBox.IsChecked = RowView != null && RowView[FieldName] is bool Value && Value;
                return CheckBox;
            });
            Grid.Columns.Add(Column);
        }
        // ---------------------------------------------------------------
        void BindSimpleControls()
        {
            TextBox edtId = Tests.MainWindow.FindControl<TextBox>("edtId");
            TextBox edtCode = Tests.MainWindow.FindControl<TextBox>("edtCode");
            TextBox edtName = Tests.MainWindow.FindControl<TextBox>("edtName");
            TextBox edtCountryId = Tests.MainWindow.FindControl<TextBox>("edtCountryId");
            TextBox edtCountryCode = Tests.MainWindow.FindControl<TextBox>("edtCountry__Code");
            TextBox edtCountryName = Tests.MainWindow.FindControl<TextBox>("edtCountry__Name");
            TextBox edtIsActive = Tests.MainWindow.FindControl<TextBox>("edtIsActive");

            if (Tests.tblCustomer.DefaultView.Count == 0)
                return;

            DataRowView RowView = Tests.tblCustomer.DefaultView[0];

            edtId.Text = RowView["Id"].ToString();
            edtCode.Text = RowView["Code"].ToString();
            edtName.Text = RowView["Name"].ToString();
            edtCountryId.Text = RowView["CountryId"].ToString();
            edtCountryCode.Text = RowView["Country__Code"].ToString();
            edtCountryName.Text = RowView["Country__Name"].ToString();
            edtIsActive.Text = RowView["IsActive"].ToString();
        }
        // ---------------------------------------------------------------
        DataGrid Grid = Tests.MainWindow.FindControl<DataGrid>("gridList");

        if (Grid == null)
        {
            Tests.Log("gridList not found.");
            return;
        }

        Grid.AutoGenerateColumns = false;
        Grid.Columns.Clear();

        AddDataRowViewTextColumn(Grid, "Id", "Id");
        AddDataRowViewTextColumn(Grid, "Code", "Code");
        AddDataRowViewTextColumn(Grid, "Name", "Name");
        AddDataRowViewTextColumn(Grid, "Country Id", "CountryId");
        AddDataRowViewTextColumn(Grid, "Country Code", "Country__Code");
        AddDataRowViewTextColumn(Grid, "Country Name", "Country__Name");
        AddDataRowViewCheckBoxColumn(Grid, "Is Active", "IsActive");

        Grid.ItemsSource = Tests.tblCustomer.DefaultView;
        BindSimpleControls();
        Tests.Log("DataRowView template column: custom snapshot columns assigned.");
    }
    
    static public void Execute_ChangeByDataRowView()
    {
        Execute();

        if (Tests.tblCustomer.DefaultView.Count == 0)
            return;

        DataRowView RowView = Tests.tblCustomer.DefaultView[0];

        RowView["Code"] = "DRV-001";
        RowView["Name"] = "Changed by DataRowView";
        RowView["CountryId"] = 100;
        RowView["Country__Code"] = "CY";
        RowView["Country__Name"] = "Cyprus";
        RowView["IsActive"] = false;

        Tests.Log("DataRowView change: first row changed through DataRowView indexer.");
    }
    
    static public void Execute_ChangeByDataRow()
    {
        Execute();

        if (Tests.tblCustomer.Rows.Count == 0)
            return;

        DataRow Row = Tests.tblCustomer.Rows[0];

        Row["Code"] = "DR-001";
        Row["Name"] = "Changed by DataRow";
        Row["CountryId"] = 200;
        Row["Country__Code"] = "US";
        Row["Country__Name"] = "United States";
        Row["IsActive"] = true;

        Tests.Log("DataRow change: first row changed through DataRow indexer.");
    }
}

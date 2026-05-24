namespace TestBindingApp;

static public class Test_DataRowViewIndexerBinding
{
    static public void Execute()
    {
        // ---------------------------------------------------------------
        void AddDataRowViewTextColumn(DataGrid Grid, string Header, string Path)
        {
            DataGridTextColumn Column = new();
            Column.Header = Header;
            Column.Binding = new Binding(Path);
            Grid.Columns.Add(Column);
        }
        // ---------------------------------------------------------------
        void AddDataRowViewCheckBoxColumn(DataGrid Grid, string Header, string Path)
        {
            DataGridCheckBoxColumn Column = new();
            Column.Header = Header;
            Column.Binding = new Binding(Path);
            Grid.Columns.Add(Column);
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

        AddDataRowViewTextColumn(Grid, "Id", "[Id]");
        AddDataRowViewTextColumn(Grid, "Code", "[Code]");
        AddDataRowViewTextColumn(Grid, "Name", "[Name]");
        AddDataRowViewTextColumn(Grid, "Country Id", "[CountryId]");
        AddDataRowViewTextColumn(Grid, "Country Code", "[Country__Code]");
        AddDataRowViewTextColumn(Grid, "Country Name", "[Country__Name]");
        AddDataRowViewCheckBoxColumn(Grid, "Is Active", "[IsActive]");

        Grid.ItemsSource = Tests.tblCustomer.DefaultView;
        Tests.Log("DataRowView indexer binding: custom columns assigned.");
    }
}
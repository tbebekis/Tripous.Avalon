namespace TestBindingApp;

static public class Test_DataRowViewEvents
{
    // ● private
    static bool fInitialized;

    static void InitializeEvents()
    {
        if (fInitialized)
            return;

        Tests.tblCustomer.DefaultView.ListChanged += DefaultView_ListChanged;
        Tests.tblCustomer.ColumnChanged += tblCustomer_ColumnChanged;

        foreach (DataRowView RowView in Tests.tblCustomer.DefaultView)
            RowView.PropertyChanged += RowView_PropertyChanged;

        fInitialized = true;
        Tests.Log("DataRowView events: subscriptions assigned.");
    }
    static void ChangeByDataRowView()
    {
        if (Tests.tblCustomer.DefaultView.Count == 0)
            return;

        DataRowView RowView = Tests.tblCustomer.DefaultView[0];

        RowView["Code"] = "EV-DRV-001";
        RowView["Name"] = "Event by DataRowView";
        RowView["CountryId"] = 300;
        RowView["Country__Code"] = "NL";
        RowView["Country__Name"] = "Netherlands";
        RowView["IsActive"] = false;
    }
    static void ChangeByDataRow()
    {
        if (Tests.tblCustomer.Rows.Count == 0)
            return;

        DataRow Row = Tests.tblCustomer.Rows[0];

        Row["Code"] = "EV-DR-001";
        Row["Name"] = "Event by DataRow";
        Row["CountryId"] = 400;
        Row["Country__Code"] = "BE";
        Row["Country__Name"] = "Belgium";
        Row["IsActive"] = true;
    }
    static void RowView_PropertyChanged(object Sender, PropertyChangedEventArgs e)
    {
        Tests.Log("DataRowView.PropertyChanged: " + e.PropertyName);
    }
    static void tblCustomer_ColumnChanged(object Sender, DataColumnChangeEventArgs e)
    {
        Tests.Log("DataTable.ColumnChanged: " + e.Column.ColumnName);
    }
    static void DefaultView_ListChanged(object Sender, ListChangedEventArgs e)
    {
        Tests.Log("DataView.ListChanged: " + e.ListChangedType + ", Index=" + e.NewIndex + ", Property=" + e.PropertyDescriptor?.Name);
    }

    // ● public
    static public void Execute_ChangeByDataRowView()
    {
        Tests.Log("TEST: Execute_ChangeByDataRowView");
        Test_DataRowViewTemplateColumn.Execute();
        InitializeEvents();
        ChangeByDataRowView();
        Tests.Log("DataRowView events: changed first row through DataRowView indexer.");
        Tests.Log("------------------------------------------------------------------");
    }
    static public void Execute_ChangeByDataRow()
    {
        Tests.Log("TEST: Execute_ChangeByDataRow");
        Test_DataRowViewTemplateColumn.Execute();
        InitializeEvents();
        ChangeByDataRow();
        Tests.Log("DataRowView events: changed first row through DataRow indexer.");
        Tests.Log("------------------------------------------------------------------");
    }
}

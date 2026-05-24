namespace BindingStudyApp_01;

static public class DataSourceTest
{
    // ● private fields
    static bool fBindingAssigned;

    // ● private
    static void EnsureBinding()
    {
        if (!fBindingAssigned)
            Execute();
    }
    static void BindGrid()
    {
        DataGrid Grid = Tests.MainWindow.FindControl<DataGrid>("gridList");

        if (Grid == null)
        {
            Tests.Log("gridList not found.");
            return;
        }

        dsCustomer.Bind(Grid);
    }
    static void BindDetailGrid()
    {
        DataGrid Grid = Tests.MainWindow.FindControl<DataGrid>("gridDetail");

        if (Grid == null)
        {
            Tests.Log("gridDetail not found.");
            return;
        }

        dsOrder.Bind(Grid);
    }
    static void BindControls()
    {
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtId"), "Id");
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtCode"), "Code");
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtName"), "Name");
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtCountryId"), "CountryId");
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtCountry__Code"), "Country__Code");
        dsCustomer.Bind(Tests.MainWindow.FindControl<TextBox>("edtCountry__Name"), "Country__Name");
        dsCustomer.Bind(Tests.MainWindow.FindControl<CheckBox>("chkIsActive"), "IsActive");
    }
    static void ChangeByDataSourceRow()
    {
        if (dsCustomer.Current == null)
            return;

        dsCustomer.Current["Code"] = "DSR-001";
        dsCustomer.Current["Name"] = "Changed by DataSourceRow";
        dsCustomer.Current["CountryId"] = 500;
        dsCustomer.Current["Country__Code"] = "PT";
        dsCustomer.Current["Country__Name"] = "Portugal";
        dsCustomer.Current["IsActive"] = false;
    }
    static void ChangeByDataRowView()
    {
        if (dsCustomer.Current == null || !(dsCustomer.Current.InnerObject is DataRowView RowView))
            return;

        RowView["Code"] = "DRV-001";
        RowView["Name"] = "Changed by DataRowView";
        RowView["CountryId"] = 600;
        RowView["Country__Code"] = "SE";
        RowView["Country__Name"] = "Sweden";
        RowView["IsActive"] = true;
    }
    static void ChangeByDataRow()
    {
        if (dsCustomer.Current == null)
            return;

        DataRowView RowView = dsCustomer.Current.InnerObject as DataRowView;
        DataRow Row = RowView != null ? RowView.Row : dsCustomer.Current.InnerObject as DataRow;

        if (Row == null)
            return;

        Row["Code"] = "DR-001";
        Row["Name"] = "Changed by DataRow";
        Row["CountryId"] = 700;
        Row["Country__Code"] = "DK";
        Row["Country__Name"] = "Denmark";
        Row["IsActive"] = false;
    }
    static int GetNextId(DataTable Table)
    {
        int Result = 0;

        foreach (DataRow Row in Table.Rows)
        {
            if (Row.RowState == DataRowState.Deleted)
                continue;

            object Value = Row["Id"];

            if (Value != DBNull.Value)
                Result = Math.Max(Result, Convert.ToInt32(Value));
        }

        return Result + 1;
    }

    // ● public
    static public void Execute()
    {
        if (fBindingAssigned)
            return;

        dsCustomer = DataSource.FromTable(Tests.tblCustomer);
        dsOrder = DataSource.FromTable(Tests.tblOrder);
        dsCustomer.AddDetail("CustomerOrders", dsOrder, "Id", "CustomerId");
        BindGrid();
        BindDetailGrid();
        BindControls();
        
        dsCustomer.BindingComplete();
        //dsOrder.BindingComplete();
        //dsCustomer.MoveFirst();
        //dsCustomer.RefreshCurrent();
        
        fBindingAssigned = true;
        
        Tests.Log("DataSource/DataTable: grid and controls assigned.");
    }
    static public void Execute_ChangeByDataSourceRow()
    {
        Tests.Log("TEST: Execute_ChangeByDataSourceRow");
        EnsureBinding();
        ChangeByDataSourceRow();
        Tests.Log("DataSource/DataTable: changed current row through DataSourceRow.");
        Tests.Log("-------------------------------------------------------------");
    }
    static public void Execute_ChangeByDataRowView()
    {
        Tests.Log("TEST: Execute_ChangeByDataRowView");
        EnsureBinding();
        ChangeByDataRowView();
        Tests.Log("DataSource/DataTable: changed current row through DataRowView.");
        Tests.Log("-------------------------------------------------------------");
    }
    static public void Execute_ChangeByDataRow()
    {
        Tests.Log("TEST: Execute_ChangeByDataRow");
        EnsureBinding();
        ChangeByDataRow();
        Tests.Log("DataSource/DataTable: changed current row through DataRow.");
        Tests.Log("-------------------------------------------------------------");
    }
    static public void Execute_AddCustomer()
    {
        EnsureBinding();
        DataSourceRow Row = dsCustomer.AddNew();
        Row["Id"] = GetNextId(Tests.tblCustomer);
        Tests.Log("DataSource/DataTable: added customer row.");
    }
    static public void Execute_DeleteCustomer()
    {
        EnsureBinding();
        dsCustomer.DeleteCurrent();
        Tests.Log("DataSource/DataTable: deleted current customer row.");
    }
    static public void Execute_AddOrder()
    {
        EnsureBinding();
        DataSourceRow Row = dsOrder.AddNew();
        Row["Id"] = GetNextId(Tests.tblOrder);
        Tests.Log("DataSource/DataTable: added order row.");
    }
    static public void Execute_DeleteOrder()
    {
        EnsureBinding();
        dsOrder.DeleteCurrent();
        Tests.Log("DataSource/DataTable: deleted current order row.");
    }

    static public DataSource dsCustomer { get; private set; }
    static public DataSource dsOrder { get; private set; }
}

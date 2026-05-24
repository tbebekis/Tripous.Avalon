namespace TestBindingApp;

static public class Test_DefaultBinding
{
    static public void Execute()
    {
        DataGrid Grid = Tests.MainWindow.FindControl<DataGrid>("gridList");
        TextBox edtId = Tests.MainWindow.FindControl<TextBox>("edtId");
        TextBox edtCode = Tests.MainWindow.FindControl<TextBox>("edtCode");
        TextBox edtName = Tests.MainWindow.FindControl<TextBox>("edtName");
        TextBox edtCountryId = Tests.MainWindow.FindControl<TextBox>("edtCountryId");
        TextBox edtCountryCode = Tests.MainWindow.FindControl<TextBox>("edtCountry__Code");
        TextBox edtCountryName = Tests.MainWindow.FindControl<TextBox>("edtCountry__Name");
        TextBox edtIsActive = Tests.MainWindow.FindControl<TextBox>("edtIsActive");

        if (Grid == null)
        {
            Tests.Log("gridList not found.");
            return;
        }

        Grid.ItemsSource = Tests.tblCustomer.DefaultView;

        if (Tests.tblCustomer.DefaultView.Count > 0)
        {
            DataRowView RowView = Tests.tblCustomer.DefaultView[0];

            edtId.Text = RowView["Id"].ToString();
            edtCode.Text = RowView["Code"].ToString();
            edtName.Text = RowView["Name"].ToString();
            edtCountryId.Text = RowView["CountryId"].ToString();
            edtCountryCode.Text = RowView["Country__Code"].ToString();
            edtCountryName.Text = RowView["Country__Name"].ToString();
            edtIsActive.Text = RowView["IsActive"].ToString();
        }

        Tests.Log("Default binding: DataView assigned to grid and first row assigned to editors.");
    }
}
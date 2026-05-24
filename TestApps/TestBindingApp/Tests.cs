namespace TestBindingApp;

static public class Tests
{
    // ● private
    
    static Action<string> LogProc;

    static void PrepareDataTable()
    {
        tblCustomer = new();
        tblCustomer.TableName = "Customer";
        tblCustomer.Columns.Add("Id", typeof(int));
        tblCustomer.Columns.Add("Code", typeof(string));
        tblCustomer.Columns.Add("Name", typeof(string));
        tblCustomer.Columns.Add("CountryId", typeof(int));
        tblCustomer.Columns.Add("Country__Code", typeof(string));
        tblCustomer.Columns.Add("Country__Name", typeof(string));
        tblCustomer.Columns.Add("IsActive", typeof(bool));
        tblCustomer.Rows.Add(1, "CUST-001", "Acme Stores", 1, "GR", "Greece", true);
        tblCustomer.Rows.Add(2, "CUST-002", "Northwind Traders", 2, "IT", "Italy", true);
        tblCustomer.Rows.Add(3, "CUST-003", "Contoso Retail", 3, "DE", "Germany", false);
        tblCustomer.Rows.Add(4, "CUST-004", "Alpine Market", 4, "FR", "France", true);
        tblCustomer.Rows.Add(5, "CUST-005", "Blue Ocean Supplies", 5, "ES", "Spain", false);
    }
    
    // ● construction
    static Tests()
    {
        PrepareDataTable();
    }

    // ● initialization
    static public void Initialize(MainWindow MainWindow, Action<string> LogProc)
    {
        Tests.MainWindow = MainWindow;
        Tests.LogProc = LogProc;
    }
    static public void Log(string Text)
    {
        if (LogProc != null) LogProc(Text);
    }
    
    // ● tests


    // ● properties
    static public MainWindow MainWindow { get; private set; }
    static public DataTable tblCustomer { get; private set; }
}

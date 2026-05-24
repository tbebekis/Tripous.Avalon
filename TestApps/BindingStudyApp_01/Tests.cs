namespace BindingStudyApp_01;

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

        tblOrder = new();
        tblOrder.TableName = "Order";
        tblOrder.Columns.Add("Id", typeof(int));
        tblOrder.Columns.Add("CustomerId", typeof(int));
        tblOrder.Columns.Add("Code", typeof(string));
        tblOrder.Columns.Add("Amount", typeof(decimal));
        tblOrder.Columns.Add("OrderDate", typeof(DateTime));
        tblOrder.Rows.Add(1, 1, "ORD-001", 120.50m, new DateTime(2026, 1, 10));
        tblOrder.Rows.Add(2, 1, "ORD-002", 340.00m, new DateTime(2026, 1, 12));
        tblOrder.Rows.Add(3, 2, "ORD-003", 90.25m, new DateTime(2026, 2, 5));
        tblOrder.Rows.Add(4, 2, "ORD-004", 840.90m, new DateTime(2026, 2, 8));
        tblOrder.Rows.Add(5, 3, "ORD-005", 72.00m, new DateTime(2026, 3, 14));
        tblOrder.Rows.Add(6, 4, "ORD-006", 410.75m, new DateTime(2026, 4, 2));
        tblOrder.Rows.Add(7, 4, "ORD-007", 66.10m, new DateTime(2026, 4, 9));
        tblOrder.Rows.Add(8, 5, "ORD-008", 150.00m, new DateTime(2026, 5, 20));
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
    static public DataTable tblOrder { get; private set; }
}

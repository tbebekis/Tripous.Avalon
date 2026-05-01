namespace DesktopApp;
 
static public class SampleData
{
    // ● private
    static Random fRandom = new Random(1);
    static List<string> fCountryIds = new List<string>();
    static List<string> fCategoryIds = new List<string>();
    static List<string> fProductIds = new List<string>();
    static List<string> fCustomerIds = new List<string>();
    static List<string> fTradeIds = new List<string>();

    // ● private
    static int Rand(int Min, int Max) => fRandom.Next(Min, Max + 1);
    static double RandDouble(double Min, double Max) => Min + fRandom.NextDouble() * (Max - Min);
    static double Round(double Value) => Math.Round(Value, 2);
    static string RandId(List<string> List) => List[Rand(0, List.Count - 1)];
    static void Exec(SqlStore Store, DbTransaction Transaction, string SqlText, params object[] Params)
    {
        Store.ExecSql(Transaction, SqlText, Params);
    }

    // ● static public
    static public void Execute(SqlStore Store, int TradeCount = 500)
    {
        fRandom = new Random(1);
        fCountryIds.Clear();
        fCategoryIds.Clear();
        fProductIds.Clear();
        fCustomerIds.Clear();
        fTradeIds.Clear();

        DbTransaction Transaction = Store.BeginTransaction();

        try
        {
            InsertMasterData(Store, Transaction);
            Transaction.Commit();

            Transaction = Store.BeginTransaction();
            int BatchCount = 0;

            for (int Index = 0; Index < TradeCount; Index++)
            {
                InsertTrade(Store, Transaction);
                BatchCount++;

                if (BatchCount >= 100)
                {
                    Transaction.Commit();
                    Transaction = Store.BeginTransaction();
                    BatchCount = 0;
                }
            }

            Transaction.Commit();
        }
        catch
        {
            Transaction.Rollback();
            throw;
        }
    }

    // ● private
    static void InsertMasterData(SqlStore Store, DbTransaction Transaction)
    {
        InsertCountry(Store, Transaction, "USA");
        InsertCountry(Store, Transaction, "United Kingdom");
        InsertCountry(Store, Transaction, "Germany");
        InsertCountry(Store, Transaction, "France");
        InsertCountry(Store, Transaction, "Italy");
        InsertCountry(Store, Transaction, "Spain");
        InsertCountry(Store, Transaction, "Canada");
        InsertCountry(Store, Transaction, "Australia");

        InsertCategory(Store, Transaction, "Beverages");
        InsertCategory(Store, Transaction, "Food");
        InsertCategory(Store, Transaction, "Office Supplies");
        InsertCategory(Store, Transaction, "Hardware");
        InsertCategory(Store, Transaction, "Electronics");
        InsertCategory(Store, Transaction, "Services");

        InsertProduct(Store, Transaction, "Coffee Beans", 0, 12.50, 8.20);
        InsertProduct(Store, Transaction, "Black Tea", 0, 6.80, 4.10);
        InsertProduct(Store, Transaction, "Orange Juice", 0, 4.90, 3.20);
        InsertProduct(Store, Transaction, "Mineral Water", 0, 1.20, 0.60);
        InsertProduct(Store, Transaction, "Energy Drink", 0, 2.80, 1.70);
        InsertProduct(Store, Transaction, "Pasta", 1, 2.40, 1.20);
        InsertProduct(Store, Transaction, "Olive Oil", 1, 13.90, 9.50);
        InsertProduct(Store, Transaction, "Rice", 1, 3.10, 1.80);
        InsertProduct(Store, Transaction, "Tomato Sauce", 1, 2.20, 1.10);
        InsertProduct(Store, Transaction, "Biscuits", 1, 3.70, 2.00);
        InsertProduct(Store, Transaction, "A4 Paper", 2, 5.90, 3.40);
        InsertProduct(Store, Transaction, "Notebook", 2, 2.60, 1.20);
        InsertProduct(Store, Transaction, "Ballpoint Pen", 2, 0.80, 0.30);
        InsertProduct(Store, Transaction, "Desk Organizer", 2, 8.50, 5.20);
        InsertProduct(Store, Transaction, "Printer Cartridge", 2, 29.90, 21.00);
        InsertProduct(Store, Transaction, "Hammer", 3, 11.50, 7.00);
        InsertProduct(Store, Transaction, "Screwdriver Set", 3, 16.80, 10.20);
        InsertProduct(Store, Transaction, "Drill Bits", 3, 9.90, 5.70);
        InsertProduct(Store, Transaction, "Measuring Tape", 3, 6.40, 3.20);
        InsertProduct(Store, Transaction, "Work Gloves", 3, 4.70, 2.30);
        InsertProduct(Store, Transaction, "Keyboard", 4, 24.00, 15.50);
        InsertProduct(Store, Transaction, "Mouse", 4, 14.50, 8.80);
        InsertProduct(Store, Transaction, "USB Cable", 4, 5.20, 2.40);
        InsertProduct(Store, Transaction, "Monitor Stand", 4, 32.00, 22.00);
        InsertProduct(Store, Transaction, "Web Camera", 4, 48.00, 31.00);
        InsertProduct(Store, Transaction, "Installation Service", 5, 60.00, 35.00);
        InsertProduct(Store, Transaction, "Maintenance Service", 5, 80.00, 45.00);
        InsertProduct(Store, Transaction, "Support Package", 5, 120.00, 70.00);
        InsertProduct(Store, Transaction, "Training Session", 5, 150.00, 90.00);
        InsertProduct(Store, Transaction, "Consulting Hour", 5, 95.00, 55.00);

        InsertCustomer(Store, Transaction, "Northwind Traders");
        InsertCustomer(Store, Transaction, "Blue River Stores");
        InsertCustomer(Store, Transaction, "Green Valley Market");
        InsertCustomer(Store, Transaction, "London Retail Group");
        InsertCustomer(Store, Transaction, "Oxford Office Supplies");
        InsertCustomer(Store, Transaction, "Berlin Wholesale");
        InsertCustomer(Store, Transaction, "Munich Equipment");
        InsertCustomer(Store, Transaction, "Paris Central Market");
        InsertCustomer(Store, Transaction, "Lyon Food Services");
        InsertCustomer(Store, Transaction, "Roma Trading");
        InsertCustomer(Store, Transaction, "Milano Distribution");
        InsertCustomer(Store, Transaction, "Madrid Supply Co");
        InsertCustomer(Store, Transaction, "Barcelona Retail");
        InsertCustomer(Store, Transaction, "Toronto Business Center");
        InsertCustomer(Store, Transaction, "Vancouver Office Market");
        InsertCustomer(Store, Transaction, "Sydney Commercial");
        InsertCustomer(Store, Transaction, "Melbourne Supplies");
        InsertCustomer(Store, Transaction, "Atlantic Imports");
        InsertCustomer(Store, Transaction, "European Goods Ltd");
        InsertCustomer(Store, Transaction, "Global Service Partners");
    }
    static void InsertCountry(SqlStore Store, DbTransaction Transaction, string Name)
    {
        string Id = Sys.GenId();
        fCountryIds.Add(Id);
        Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", Id, Name);
    }
    static void InsertCategory(SqlStore Store, DbTransaction Transaction, string Name)
    {
        string Id = Sys.GenId();
        fCategoryIds.Add(Id);
        Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", Id, Name);
    }
    static void InsertProduct(SqlStore Store, DbTransaction Transaction, string Name, int CategoryIndex, double UnitPrice, double UnitCost)
    {
        string Id = Sys.GenId();
        string CategoryId = fCategoryIds[CategoryIndex];
        fProductIds.Add(Id);

        Exec(Store, Transaction, "insert into Product (Id, Name, CategoryId, UnitPrice, UnitCost) values (:Id, :Name, :CategoryId, :UnitPrice, :UnitCost)", Id, Name, CategoryId, UnitPrice, UnitCost);
    }
    static void InsertCustomer(SqlStore Store, DbTransaction Transaction, string Name)
    {
        string Id = Sys.GenId();
        string CountryId = RandId(fCountryIds);
        fCustomerIds.Add(Id);

        Exec(Store, Transaction, "insert into Customer (Id, Name, CountryId) values (:Id, :Name, :CountryId)", Id, Name, CountryId);
    }
    static void InsertTrade(SqlStore Store, DbTransaction Transaction)
    {
        string TradeId = Sys.GenId();
        fTradeIds.Add(TradeId);

        DateTime Date = DateTime.Today.AddDays(-Rand(0, 365));
        string CustomerId = RandId(fCustomerIds);
        int TradeType = Rand(1, 2);
        int Status = Rand(1, 3);

        Exec(Store, Transaction, "insert into Trade (Id, Date, CustomerId, TradeType, Status) values (:Id, :Date, :CustomerId, :TradeType, :Status)", TradeId, Date, CustomerId, TradeType, Status);

        int LineCount = Rand(2, 20);

        for (int Index = 0; Index < LineCount; Index++)
            InsertTradeLine(Store, Transaction, TradeId);
    }
    static void InsertTradeLine(SqlStore Store, DbTransaction Transaction, string TradeId)
    {
        string Id = Sys.GenId();
        string ProductId = RandId(fProductIds);

        double Qty = Rand(1, 20);
        double UnitPrice = Round(RandDouble(2, 150));
        double NetAmount = Round(Qty * UnitPrice);
        double TaxPercent = Round(RandDouble(5, 15));
        double TaxAmount = Round((NetAmount / 100) * TaxPercent);
        double LineAmount = Round(NetAmount + TaxAmount);

        Exec(Store, Transaction, "insert into TradeLines (Id, TradeId, ProductId, Qty, UnitPrice, NetAmount, TaxPercent, TaxAmount, LineAmount) values (:Id, :TradeId, :ProductId, :Qty, :UnitPrice, :NetAmount, :TaxPercent, :TaxAmount, :LineAmount)", Id, TradeId, ProductId, Qty, UnitPrice, NetAmount, TaxPercent, TaxAmount, LineAmount);
    }
}
 
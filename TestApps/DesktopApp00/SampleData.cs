namespace Tripous.Data
{
    // ● public
    static public class SampleData
    {
        // ● private
        static Random fRandom = new Random(1);
        static int fTradeId;
        static int fLineId;

        // ● private
        static int NextTradeId() => ++fTradeId;
        static int NextLineId() => ++fLineId;
        static int Rand(int Min, int Max) => fRandom.Next(Min, Max + 1);
        static double RandDouble(double Min, double Max) => Min + fRandom.NextDouble() * (Max - Min);
        static double Round(double Value) => Math.Round(Value, 2);
        static void Exec(SqlStore Store, DbTransaction Transaction, string SqlText, params object[] Params)
        {
            Store.ExecSql(Transaction, SqlText, Params);
        }

        // ● static public
        static public void Execute(SqlStore Store, int TradeCount = 500)
        {
            fRandom = new Random(1);
            fTradeId = 0;
            fLineId = 0;

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
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 1, "USA");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 2, "United Kingdom");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 3, "Germany");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 4, "France");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 5, "Italy");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 6, "Spain");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 7, "Canada");
            Exec(Store, Transaction, "insert into Country (Id, Name) values (:Id, :Name)", 8, "Australia");

            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 1, "Beverages");
            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 2, "Food");
            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 3, "Office Supplies");
            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 4, "Hardware");
            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 5, "Electronics");
            Exec(Store, Transaction, "insert into Category (Id, Name) values (:Id, :Name)", 6, "Services");

            InsertProduct(Store, Transaction, 1, "Coffee Beans", 1, 12.50, 8.20);
            InsertProduct(Store, Transaction, 2, "Black Tea", 1, 6.80, 4.10);
            InsertProduct(Store, Transaction, 3, "Orange Juice", 1, 4.90, 3.20);
            InsertProduct(Store, Transaction, 4, "Mineral Water", 1, 1.20, 0.60);
            InsertProduct(Store, Transaction, 5, "Energy Drink", 1, 2.80, 1.70);
            InsertProduct(Store, Transaction, 6, "Pasta", 2, 2.40, 1.20);
            InsertProduct(Store, Transaction, 7, "Olive Oil", 2, 13.90, 9.50);
            InsertProduct(Store, Transaction, 8, "Rice", 2, 3.10, 1.80);
            InsertProduct(Store, Transaction, 9, "Tomato Sauce", 2, 2.20, 1.10);
            InsertProduct(Store, Transaction, 10, "Biscuits", 2, 3.70, 2.00);
            InsertProduct(Store, Transaction, 11, "A4 Paper", 3, 5.90, 3.40);
            InsertProduct(Store, Transaction, 12, "Notebook", 3, 2.60, 1.20);
            InsertProduct(Store, Transaction, 13, "Ballpoint Pen", 3, 0.80, 0.30);
            InsertProduct(Store, Transaction, 14, "Desk Organizer", 3, 8.50, 5.20);
            InsertProduct(Store, Transaction, 15, "Printer Cartridge", 3, 29.90, 21.00);
            InsertProduct(Store, Transaction, 16, "Hammer", 4, 11.50, 7.00);
            InsertProduct(Store, Transaction, 17, "Screwdriver Set", 4, 16.80, 10.20);
            InsertProduct(Store, Transaction, 18, "Drill Bits", 4, 9.90, 5.70);
            InsertProduct(Store, Transaction, 19, "Measuring Tape", 4, 6.40, 3.20);
            InsertProduct(Store, Transaction, 20, "Work Gloves", 4, 4.70, 2.30);
            InsertProduct(Store, Transaction, 21, "Keyboard", 5, 24.00, 15.50);
            InsertProduct(Store, Transaction, 22, "Mouse", 5, 14.50, 8.80);
            InsertProduct(Store, Transaction, 23, "USB Cable", 5, 5.20, 2.40);
            InsertProduct(Store, Transaction, 24, "Monitor Stand", 5, 32.00, 22.00);
            InsertProduct(Store, Transaction, 25, "Web Camera", 5, 48.00, 31.00);
            InsertProduct(Store, Transaction, 26, "Installation Service", 6, 60.00, 35.00);
            InsertProduct(Store, Transaction, 27, "Maintenance Service", 6, 80.00, 45.00);
            InsertProduct(Store, Transaction, 28, "Support Package", 6, 120.00, 70.00);
            InsertProduct(Store, Transaction, 29, "Training Session", 6, 150.00, 90.00);
            InsertProduct(Store, Transaction, 30, "Consulting Hour", 6, 95.00, 55.00);

            InsertCustomer(Store, Transaction, 1, "Northwind Traders", 1);
            InsertCustomer(Store, Transaction, 2, "Blue River Stores", 1);
            InsertCustomer(Store, Transaction, 3, "Green Valley Market", 1);
            InsertCustomer(Store, Transaction, 4, "London Retail Group", 2);
            InsertCustomer(Store, Transaction, 5, "Oxford Office Supplies", 2);
            InsertCustomer(Store, Transaction, 6, "Berlin Wholesale", 3);
            InsertCustomer(Store, Transaction, 7, "Munich Equipment", 3);
            InsertCustomer(Store, Transaction, 8, "Paris Central Market", 4);
            InsertCustomer(Store, Transaction, 9, "Lyon Food Services", 4);
            InsertCustomer(Store, Transaction, 10, "Roma Trading", 5);
            InsertCustomer(Store, Transaction, 11, "Milano Distribution", 5);
            InsertCustomer(Store, Transaction, 12, "Madrid Supply Co", 6);
            InsertCustomer(Store, Transaction, 13, "Barcelona Retail", 6);
            InsertCustomer(Store, Transaction, 14, "Toronto Business Center", 7);
            InsertCustomer(Store, Transaction, 15, "Vancouver Office Market", 7);
            InsertCustomer(Store, Transaction, 16, "Sydney Commercial", 8);
            InsertCustomer(Store, Transaction, 17, "Melbourne Supplies", 8);
            InsertCustomer(Store, Transaction, 18, "Atlantic Imports", 1);
            InsertCustomer(Store, Transaction, 19, "European Goods Ltd", 2);
            InsertCustomer(Store, Transaction, 20, "Global Service Partners", 3);
        }
        static void InsertProduct(SqlStore Store, DbTransaction Transaction, int Id, string Name, int CategoryId, double UnitPrice, double UnitCost)
        {
            Exec(Store, Transaction, "insert into Product (Id, Name, CategoryId, UnitPrice, UnitCost) values (:Id, :Name, :CategoryId, :UnitPrice, :UnitCost)", Id, Name, CategoryId, UnitPrice, UnitCost);
        }
        static void InsertCustomer(SqlStore Store, DbTransaction Transaction, int Id, string Name, int CountryId)
        {
            Exec(Store, Transaction, "insert into Customer (Id, Name, CountryId) values (:Id, :Name, :CountryId)", Id, Name, CountryId);
        }
        static void InsertTrade(SqlStore Store, DbTransaction Transaction)
        {
            int TradeId = NextTradeId();
            DateTime Date = DateTime.Today.AddDays(-Rand(0, 365));
            int CustomerId = Rand(1, 20);
            int TradeType = Rand(1, 2);
            int Status = Rand(1, 3);

            Exec(Store, Transaction, "insert into Trade (Id, Date, CustomerId, TradeType, Status) values (:Id, :Date, :CustomerId, :TradeType, :Status)", TradeId, Date, CustomerId, TradeType, Status);

            int LineCount = Rand(2, 20);

            for (int Index = 0; Index < LineCount; Index++)
                InsertTradeLine(Store, Transaction, TradeId);
        }
        static void InsertTradeLine(SqlStore Store, DbTransaction Transaction, int TradeId)
        {
            int ProductId = Rand(1, 30);
            double Qty = Rand(1, 20);
            double UnitPrice = Round(RandDouble(2, 150));
            double NetAmount = Round(Qty * UnitPrice);
            double TaxPercent = Round(RandDouble(5, 15));
            double TaxAmount = Round((NetAmount / 100) * TaxPercent);
            double LineAmount = Round(NetAmount + TaxAmount);

            Exec(Store, Transaction, "insert into TradeLines (Id, TradeId, ProductId, Qty, UnitPrice, NetAmount, TaxPercent, TaxAmount, LineAmount) values (:Id, :TradeId, :ProductId, :Qty, :UnitPrice, :NetAmount, :TaxPercent, :TaxAmount, :LineAmount)", NextLineId(), TradeId, ProductId, Qty, UnitPrice, NetAmount, TaxPercent, TaxAmount, LineAmount);
        }
    }
}
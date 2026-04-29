namespace DesktopApp;

static public class DataTests
{
    const string SMsSql =
        @"Data Source=localhost; Initial Catalog=DVD; User ID=sa; Password={0}; TrustServerCertificate=true;";
 
  
    // ● static public
    static public void TestSqlParamScanner()
    {
        void Run(string Title, string Sql)
        {
            var List = SqlParamScanner.Scan(Sql);

            LogBox.AppendLine("--------------------------------------------------");
            LogBox.AppendLine(Title);
            LogBox.AppendLine(Sql);
            LogBox.AppendLine($"Count = {List.Count}");

            for (int i = 0; i < List.Count; i++)
                LogBox.AppendLine($"  [{i}] Name = {List[i].Name}, Index = {List[i].Index}");
        }

        // ● tests

        Run("Basic",
            "select * from Customer where Id = :Id and Name = :Name");

        Run("Ignore Strings",
            "select * from Customer where Note = ':Id' and Id = :Id");

        Run("Ignore Comments",
            @"
        select * from Customer 
        -- where Id = :Id
        where Name = :Name
        /* and Code = :Code */
        ");

        Run("Postgres Cast",
            "select now()::timestamp, Id from Table where Id = :Id");

        Run("Repeated",
            "select * from T where Id = :Id or ParentId = :Id");
    }
    static public void TestSqlParamScanner_Edges()
    {
        void Run(string Title, string Sql)
        {
            var List = SqlParamScanner.Scan(Sql);

            LogBox.AppendLine("--------------------------------------------------");
            LogBox.AppendLine(Title);
            LogBox.AppendLine(Sql);
            LogBox.AppendLine($"Count = {List.Count}");

            for (int i = 0; i < List.Count; i++)
                LogBox.AppendLine($"  [{i}] Name = {List[i].Name}, Index = {List[i].Index}");
        }

        // ● tests

        Run("Id vs Id2",
            "select * from T where Id = :Id and Id2 = :Id2");

        Run("Ends With Param",
            "select * from T where Id = :Id");

        Run("Adjacent Params",
            "select * from T where A = :A and B = :B and C = :C");

        Run("Underscore Name",
            "select * from T where Value = :My_Param1");
    }

    static public DbConnectionInfo CreateMsSqlConnectionInfo(string Password)
    {
        DbConnectionInfo CI = new();
        CI.Name = "MsSql.Default";
        CI.DbServerType = DbServerType.MsSql;
        CI.ConnectionString = string.Format(SMsSql, Password);
        return CI;
    }
    static public void TestConnection(string Password) // M!r0d@t0
    {
        DbConnectionInfo CI = CreateMsSqlConnectionInfo(Password);
        var Provider = CI.GetSqlProvider();
        var Flag = Provider.CanConnect(CI.ConnectionString);
        LogBox.AppendLine($"CanConnect() - Psw: {Password}  = {Flag}");
    }
    static public void TestMsSqlStore(string Password)
    {
        DbConnectionInfo CI = CreateMsSqlConnectionInfo(Password);
        SqlStore Store = new SqlStore(CI);
        
        LogBox.AppendLine("==================================================");

        // ● test 1 - scalar params (positional)
        {
            string Sql = @"
            select top 5 Id, first_name, last_name
            from dbo.actor
            where last_name like :LastName
        ";

            DataTable T = Store.Select(Sql, "A%");

            LogBox.AppendLine("Test 1 - scalar");
            LogBox.AppendLine($"Rows = {T.Rows.Count}");
        }

        // ● test 2 - dictionary params
        {
            string Sql = @"
            select top 5 f.title, c.name
            from dbo.film f
            join dbo.film_category fc on fc.film_id = f.Id
            join dbo.category c on c.Id = fc.category_id
            where c.name = :Category
        ";

            Dictionary<string, object> D = new Dictionary<string, object>();
            D["Category"] = "Action";

            DataTable T = Store.Select(Sql, D);

            LogBox.AppendLine("Test 2 - dictionary");
            LogBox.AppendLine($"Rows = {T.Rows.Count}");
        }
    }
    static public void TestConnectionStringBuilder()
    {
        void Log(string Title, string Text)
        {
            LogBox.AppendLine("--------------------------------------------------");
            LogBox.AppendLine(Title);
            LogBox.AppendLine(Text);
        }

        // ● test 1 - basic parse
        {
            string Cs = "Server=localhost;Database=DVD;User Id=sa;Password=123;";
            var B = new ConnectionStringBuilder(Cs);

            Log("Basic",
                $"Server={B.GetFirst(new[] { "Server", "Data Source" })}\n" +
                $"Database={B.GetFirst(new[] { "Database", "Initial Catalog" })}\n" +
                $"User={B.GetFirst(new[] { "User Id", "User ID" })}");
        }

        // ● test 2 - alias
        {
            string Cs = "Alias=TestDb;Server=localhost;Database=DVD;";
            string Alias = ConnectionStringBuilder.GetAlias(Cs);
            string Clean = ConnectionStringBuilder.RemoveAliasEntry(Cs);

            Log("Alias",
                $"Alias={Alias}\n" +
                $"Clean={Clean}");
        }

        // ● test 3 - normalize paths
        {
            string Cs = "Data Source=[AppPath]/data.db;";
            string Norm = ConnectionStringBuilder.NormalizeConnectionString(Cs);

            Log("Normalize",
                Norm);
        }

        // ● test 4 - remove keys
        {
            string Cs = "Server=localhost;Database=DVD;User Id=sa;";
            var B = new ConnectionStringBuilder(Cs);

            B.RemoveKeys(new[] { "Database" });

            Log("Remove Database",
                B.ConnectionString);
        }

        // ● test 5 - to/from table
        {
            string Cs = "Server=localhost;Database=DVD;";
            var B = new ConnectionStringBuilder(Cs);

            DataTable T = B.ToDataTable();

            var B2 = new ConnectionStringBuilder();
            B2.FromDataTable(T);

            Log("To/From Table",
                B2.ConnectionString);
        }
    }
}
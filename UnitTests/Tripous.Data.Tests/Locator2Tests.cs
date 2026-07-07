namespace Tripous.Data.Tests;

/// <summary>
/// Tests for the <see cref="Locator2"/> runtime locator.
/// </summary>
public class Locator2Tests
{
    // ● private
    void ResetDefaultStore()
    {
        typeof(Db)
            .GetField("fDefaultStore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .SetValue(null, null);
    }
    string CreateTestDatabase()
    {
        string FilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{System.Guid.NewGuid():N}.db3");
        using System.Data.SQLite.SQLiteConnection Connection = new($"Data Source=\"{FilePath}\"");
        Connection.Open();
        using System.Data.SQLite.SQLiteCommand Command = Connection.CreateCommand();
        Command.CommandText = """
            create table UnitOfMeasure (
                Id nvarchar(40) not null primary key,
                Code nvarchar(40) not null,
                Name nvarchar(96) not null
            );
            insert into UnitOfMeasure (Id, Code, Name) values ('1', 'H87', 'Piece');
            insert into UnitOfMeasure (Id, Code, Name) values ('2', 'KGM', 'Kilogram');
            insert into UnitOfMeasure (Id, Code, Name) values ('3', 'LTR', 'Liter');
            insert into UnitOfMeasure (Id, Code, Name) values ('4', 'MTR', 'Meter');
            insert into UnitOfMeasure (Id, Code, Name) values ('5', 'CMT', 'Centimeter');
            """;
        Command.ExecuteNonQuery();
        return FilePath;
    }
    LocatorDef2 RegisterUnitOfMeasureLocator()
    {
        DataRegistry.Locators2.Clear();
        LocatorDef2 LocatorDef = DataRegistry.AddLocator2("UnitOfMeasure");
        LocatorDef.Add("Id");
        LocatorDef.Add("Code");
        LocatorDef.Add("Name");
        LocatorDef.AddSearchFields("Code", "Name");
        LocatorDef.AddResultFields("Id", "Code", "Name");
        return LocatorDef;
    }
    void SetupTestDatabase(string FilePath)
    {
        Db.Connections = new DbConnections();
        Db.Connections.List.Add(new DbConnectionInfo()
        {
            Name = DbConfig.DefaultConnectionName,
            DbServerType = DbServerType.Sqlite,
            ConnectionString = $"Data Source=\"{FilePath}\"",
            CommandTimeoutSeconds = 30,
        });
        ResetDefaultStore();
        RegisterUnitOfMeasureLocator();
    }

    // ● public
    /// <summary>
    /// Executes a SQL locator and returns a <see cref="MemTable"/> result.
    /// </summary>
    [Fact]
    public void Execute_ReturnsMemTableForSqlSource()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchTerm = "kilo",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.SingleResult, Result.Status);
            Assert.NotNull(Result.Table);
            Assert.NotNull(Result.View);
            Assert.Equal(1, Result.Count);
            Assert.Equal("KGM", Result.Table.Rows[0]["Code"]);
            Assert.Equal("Kilogram", Result.Table.Rows[0]["Name"]);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and returns no result when the search term is not found.
    /// </summary>
    [Fact]
    public void Execute_ReturnsNoResultWhenTermIsNotFound()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchTerm = "missing",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.NoResult, Result.Status);
            Assert.NotNull(Result.Table);
            Assert.Equal(0, Result.Count);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and returns multiple results when the search term is ambiguous.
    /// </summary>
    [Fact]
    public void Execute_ReturnsMultipleResultsWhenTermIsAmbiguous()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchTerm = "meter",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.MultipleResults, Result.Status);
            Assert.NotNull(Result.Table);
            Assert.True(Result.Count > 1);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and returns too many results when row count exceeds the maximum result count.
    /// </summary>
    [Fact]
    public void Execute_ReturnsTooManyResultsWhenCountExceedsMaximum()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);
            LocatorDef2 LocatorDef = DataRegistry.GetLocator2("UnitOfMeasure");
            LocatorDef.MaximumResultCount = 1;

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchTerm = "meter",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.TooManyResults, Result.Status);
            Assert.Null(Result.Table);
            Assert.Equal(0, Result.Count);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and returns invalid request when the search field is not registered.
    /// </summary>
    [Fact]
    public void Execute_ReturnsInvalidRequestWhenSearchFieldIsUnknown()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchField = "Unknown",
                SearchTerm = "kilo",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.InvalidRequest, Result.Status);
            Assert.Contains("Search field", Result.Message, System.StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and applies the descriptor ORDER BY clause.
    /// </summary>
    [Fact]
    public void Execute_AppliesOrderBy()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);
            LocatorDef2 LocatorDef = DataRegistry.GetLocator2("UnitOfMeasure");
            LocatorDef.OrderBy = "Name desc";

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                SearchTerm = "meter",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.MultipleResults, Result.Status);
            Assert.Equal("Meter", Result.Table.Rows[0]["Name"]);
            Assert.Equal("Centimeter", Result.Table.Rows[1]["Name"]);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Executes a SQL locator and resolves a row by exact key value.
    /// </summary>
    [Fact]
    public void Execute_ResolvesByKeyValue()
    {
        string FilePath = CreateTestDatabase();
        try
        {
            SetupTestDatabase(FilePath);

            LocatorRequest2 Request = new()
            {
                Context = new LocatorContext2("UnitOfMeasure"),
                KeyValue = "2",
            };

            LocatorResult2 Result = Locators2.Execute(Request);

            Assert.Equal(LocatorResultStatus2.SingleResult, Result.Status);
            Assert.Equal("KGM", Result.Table.Rows[0]["Code"]);
            Assert.Equal("Kilogram", Result.Table.Rows[0]["Name"]);
        }
        finally
        {
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Creates a locator mapping plan using key, snapshot and join fields.
    /// </summary>
    [Fact]
    public void Mapper_CreatePlan_UsesKeySnapshotAndJoinFields()
    {
        LocatorDef2 LocatorDef = new() { Name = "Product" };
        LocatorDef.Add("Id");
        LocatorDef.Add("Code");
        LocatorDef.Add("Name");
        LocatorDef.AddResultFields("Id", "Code", "Name");

        TableDef TargetTable = new() { Name = "TradeLine" };
        TargetTable.AddStringId();
        FieldDef ReferenceField = TargetTable.AddField("ProductId", DataFieldType.String, Locator: "Product");
        TargetTable.AddString("ProductCode").SnapshotOf = "Product.Code";

        TableDef JoinTable = TargetTable.AddJoin(OwnKeyField: "ProductId", Locator: "Product", ForeignTable: "Product");
        JoinTable.AddStringId();
        JoinTable.AddString("Code");
        JoinTable.AddString("Name");
        TargetTable.UpdateReferences();

        LocatorMapPlan2 Plan = new LocatorMapper2().CreatePlan(LocatorDef, TargetTable, ReferenceField);

        Assert.Equal("Product", Plan.LocatorName);
        Assert.Equal("ProductId", Plan.ReferenceField);
        Assert.Contains(Plan.Items, x => x.SourceField == "Id" && x.TargetField == "ProductId");
        Assert.Contains(Plan.Items, x => x.SourceField == "Code" && x.TargetField == "ProductCode");
        Assert.Contains(Plan.Items, x => x.SourceField == "Name" && x.TargetField == "Product__Name");
    }
    /// <summary>
    /// Applies a locator mapping plan to a target row.
    /// </summary>
    [Fact]
    public void Mapper_Apply_WritesMappedTargetFields()
    {
        MemTable SourceTable = new("Product");
        SourceTable.Columns.Add("Id", typeof(string));
        SourceTable.Columns.Add("Code", typeof(string));
        SourceTable.Columns.Add("Name", typeof(string));
        DataRow SourceRow = SourceTable.NewRow();
        SourceRow["Id"] = "P1";
        SourceRow["Code"] = "PRD-001";
        SourceRow["Name"] = "Coffee Machine";
        SourceTable.Rows.Add(SourceRow);

        MemTable TargetTable = new("TradeLine");
        TargetTable.Columns.Add("ProductId", typeof(string));
        TargetTable.Columns.Add("ProductCode", typeof(string));
        TargetTable.Columns.Add("Product__Name", typeof(string));
        DataRow TargetRow = TargetTable.NewRow();
        TargetTable.Rows.Add(TargetRow);

        LocatorMapPlan2 Plan = new()
        {
            LocatorName = "Product",
            ReferenceField = "ProductId",
        };
        Plan.Add("Id", "ProductId");
        Plan.Add("Code", "ProductCode");
        Plan.Add("Name", "Product__Name");

        new LocatorMapper2().Apply(Plan, SourceRow, TargetRow);

        Assert.Equal("P1", TargetRow["ProductId"]);
        Assert.Equal("PRD-001", TargetRow["ProductCode"]);
        Assert.Equal("Coffee Machine", TargetRow["Product__Name"]);
    }
}

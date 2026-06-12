namespace tERP.Tests;

[Collection(TestCollection.Name)]
public class DatabaseSmokeTests
{
    // ● private fields
    readonly TestDatabaseFixture fFixture;

    // ● construction
    public DatabaseSmokeTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    [Fact]
    public void DatabaseContainsSchemaAndSampleProducts()
    {
        Assert.True(File.Exists(fFixture.DatabasePath));
        Assert.True(fFixture.Store.TableExists("Trade"));
        Assert.Equal(8, fFixture.Store.IntegerResult("select count(*) from Product", 0));
        Assert.Equal(16, fFixture.Store.IntegerResult("select count(*) from PriceList", 0));
    }
}

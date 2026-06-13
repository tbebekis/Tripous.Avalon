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
        Assert.Equal(24, fFixture.Store.IntegerResult("select count(*) from PriceList", 0));
        Assert.Equal(9, fFixture.Store.IntegerResult("select count(*) from ProductUnitOfMeasure", 0));
        Assert.Equal(9, fFixture.Store.IntegerResult("select count(*) from ProductBarcode", 0));
        Assert.Equal(8, fFixture.Store.IntegerResult("select count(*) from ProductSupplier", 0));
        Assert.Equal(16, fFixture.Store.IntegerResult("select count(*) from ProductWarehouse", 0));
    }
    [Fact]
    public void DatabaseContainsCompleteBusinessTestData()
    {
        Assert.Equal(11, fFixture.Store.IntegerResult("select count(*) from Person", 0));
        Assert.Equal(5, fFixture.Store.IntegerResult("select count(*) from CostCenter", 0));
        Assert.Equal(14, fFixture.Store.IntegerResult("select count(*) from TaxRule", 0));
        Assert.Equal(21, fFixture.Store.IntegerResult("select count(*) from Account", 0));
        Assert.Equal(2, fFixture.Store.IntegerResult("select count(*) from Asset", 0));
        Assert.Equal(16, fFixture.Store.IntegerResult("select count(*) from StockMovement", 0));
        Assert.Equal(16, fFixture.Store.IntegerResult("select count(*) from StockBalance", 0));
        Assert.Equal(0, fFixture.Store.IntegerResult("select count(*) from (select ProductId from StockBalance group by ProductId having count(*) <> 2) X", 0));
        Assert.Equal(0, fFixture.Store.IntegerResult("select count(*) from StockBalance B left join StockMovement M on M.Id = B.LastMovementId where M.Id is null or B.PrimaryQuantity <> M.PrimaryQuantity or B.TotalCostAmount <> M.CostAmount or B.AverageUnitCost <> M.UnitCost", 0));
    }
}

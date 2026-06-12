namespace tERP.Tests;

[CollectionDefinition("tERP database tests")]
public class TestCollection: ICollectionFixture<TestDatabaseFixture>
{
    // ● constants
    public const string Name = "tERP database tests";
}

namespace Tripous.Data.Tests;

public class TableDefTests
{
    [Fact]
    public void BuildSql_SetsEmptyInsertAndUpdateSqlWhenNoWriteableFields()
    {
        TableDef TableDef = new();
        TableDef.Name = "TestTable";
        TableDef.AddIntegerId();

        TableSqls Sqls = TableDef.BuildSql(BuildSqlFlags.None);

        Assert.Equal(string.Empty, Sqls.InsertRowSql);
        Assert.Equal(string.Empty, Sqls.UpdateRowSql);
    }
}

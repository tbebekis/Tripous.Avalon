namespace Tripous.Data.Tests;

public class SelectSqlTests
{
    [Fact]
    public void Constructor_ParsesBasicClauses()
    {
        SelectSql Sql = new("select Id, Name from Customer where IsActive = 1 order by Name");

        Assert.Equal("Id, Name", Sql.Select);
        Assert.Equal("Customer", Sql.From);
        Assert.Equal("IsActive = 1", Sql.Where);
        Assert.Equal("Name", Sql.OrderBy);
    }
    [Fact]
    public void Parser_ParsesOuterClausesWhenWhereContainsNestedSelect()
    {
        SelectSqlParser Parser = new("select C.Id from Customer C where C.Id in (select O.CustomerId from Orders O where O.Total > 100) order by C.Name");

        Assert.Equal("C.Id", Parser.Select);
        Assert.Equal("Customer C", Parser.From);
        Assert.Equal("C.Id in (select O.CustomerId from Orders O where O.Total > 100)", Parser.Where);
        Assert.Equal("C.Name", Parser.OrderBy);
    }
    [Fact]
    public void Constructor_ParsesOuterClausesWhenWhereContainsNestedSelect()
    {
        SelectSql Sql = new("select C.Id from Customer C where C.Id in (select O.CustomerId from Orders O where O.Total > 100) order by C.Name");

        Assert.Equal("C.Id", Sql.Select);
        Assert.Equal("Customer C", Sql.From);
        Assert.Equal("C.Id in (select O.CustomerId from Orders O where O.Total > 100)", Sql.Where);
        Assert.Equal("C.Name", Sql.OrderBy);
    }
    [Fact]
    public void Text_PutsWhereBeforeOrderBy()
    {
        SelectSql Sql = new("select Id, Name from Customer order by Name");

        Sql.Where = "Customer.Id in (1, 2)";
        string Text = Sql.Text;

        Assert.Contains("Customer.Id in (1, 2)", Text);
        Assert.True(Text.IndexOf("where", StringComparison.OrdinalIgnoreCase) < Text.IndexOf("order by", StringComparison.OrdinalIgnoreCase));
    }
    [Fact]
    public void AddToWhere_AppendsWithAnd()
    {
        SelectSql Sql = new("select Id from Customer where IsActive = 1");

        Sql.AddToWhere("CountryId = 5");

        Assert.Contains("IsActive = 1", Sql.Where);
        Assert.Contains("and CountryId = 5", Sql.Where);
    }
    [Fact]
    public void GetMainTableName_ReturnsFirstFromTable()
    {
        SelectSql Sql = new("select C.Id from Customer C left join Country R on R.Id = C.CountryId");

        string TableName = Sql.GetMainTableName();

        Assert.Equal("Customer", TableName);
    }
    [Fact]
    public void Clone_CopiesClausesWithoutSharingInstance()
    {
        SelectSql Source = new("select Id from Customer where IsActive = 1");
        SelectSql Clone = (SelectSql)Source.Clone();

        Clone.Where = "IsActive = 0";

        Assert.Equal("IsActive = 1", Source.Where);
        Assert.Equal("IsActive = 0", Clone.Where);
    }
}

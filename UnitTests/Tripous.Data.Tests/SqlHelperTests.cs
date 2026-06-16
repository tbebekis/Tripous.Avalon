namespace Tripous.Data.Tests;

public class SqlHelperTests
{
    [Fact]
    public void Format_ReturnsDecimalSqlText()
    {
        string Result = SqlHelper.Format(12.34m);

        Assert.Equal("12.34", Result);
    }
    [Fact]
    public void Format_ReturnsDateTimeSqlTextWithSeconds()
    {
        DateTime Value = new DateTime(2026, 6, 16, 10, 20, 35);

        string Result = SqlHelper.Format(Value);

        Assert.Equal("'2026-06-16 10:20:35'", Result);
    }
    [Fact]
    public void FormatId_ReturnsLargeNumericIdWithoutQuotes()
    {
        string Result = SqlHelper.FormatId(3000000000L);

        Assert.Equal("3000000000", Result);
    }
}

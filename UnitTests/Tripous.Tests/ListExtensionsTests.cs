namespace Tripous.Tests;

/// <summary>
/// Tests for list extension methods.
/// </summary>
public class ListExtensionsTests
{
    // ● public
    /// <summary>
    /// Ensures key values are split into chunks.
    /// </summary>
    [Fact]
    public void GetKeyValuesList_SplitsNumericValuesIntoChunks()
    {
        List<object> Source = new List<object> { 1, 2, 3 };
        string[] Result = Source.GetKeyValuesList("Id", 2, false);
        Assert.Equal(new[] { "1, 2", "3" }, Result);
    }
    /// <summary>
    /// Ensures null values are skipped without creating empty chunks.
    /// </summary>
    [Fact]
    public void GetKeyValuesList_SkipsNullValues()
    {
        List<object> Source = new List<object> { null, 1, null, 2 };
        string[] Result = Source.GetKeyValuesList("Id", 2, false);
        Assert.Equal(new[] { "1, 2" }, Result);
    }
    /// <summary>
    /// Ensures zero and negative values are skipped when requested.
    /// </summary>
    [Fact]
    public void GetKeyValuesList_DiscardsNonPositiveNumericValues()
    {
        List<object> Source = new List<object> { -1, 0, 1, 2 };
        string[] Result = Source.GetKeyValuesList("Id", 2, true);
        Assert.Equal(new[] { "1, 2" }, Result);
    }
    /// <summary>
    /// Ensures string values are quoted and escaped.
    /// </summary>
    [Fact]
    public void GetKeyValuesList_QuotesAndEscapesStringValues()
    {
        List<object> Source = new List<object> { "A", "B'B", "C" };
        string[] Result = Source.GetKeyValuesList("Code", 2, false);
        Assert.Equal(new[] { "'A', 'B''B'", "'C'" }, Result);
    }
    /// <summary>
    /// Ensures the first item cannot move toward index zero.
    /// </summary>
    [Fact]
    public void CanMove_ReturnsFalseForFirstItemMovingDown()
    {
        List<string> Source = new List<string> { "A", "B" };
        Assert.False(((IList)Source).CanMove(0, true));
    }
    /// <summary>
    /// Ensures the last item cannot move past the list end.
    /// </summary>
    [Fact]
    public void CanMove_ReturnsFalseForLastItemMovingUp()
    {
        List<string> Source = new List<string> { "A", "B" };
        Assert.False(((IList)Source).CanMove(1, false));
    }
    /// <summary>
    /// Ensures move returns false and leaves the list unchanged when movement is invalid.
    /// </summary>
    [Fact]
    public void Move_ReturnsFalseAndKeepsListWhenMovementIsInvalid()
    {
        List<string> Source = new List<string> { "A", "B" };
        bool Result = ((IList)Source).Move(1, false);
        Assert.False(Result);
        Assert.Equal(new[] { "A", "B" }, Source);
    }
    /// <summary>
    /// Ensures a null source still returns a table with the source type columns.
    /// </summary>
    [Fact]
    public void ToDataTable_ReturnsColumnsForNullSource()
    {
        List<TestRow> Source = null;
        DataTable Result = Source.ToDataTable();
        Assert.Equal(2, Result.Columns.Count);
        Assert.Equal("Id", Result.Columns[0].ColumnName);
        Assert.Equal("Name", Result.Columns[1].ColumnName);
        Assert.Empty(Result.Rows);
    }
    /// <summary>
    /// Ensures null items are skipped when converting a list to a table.
    /// </summary>
    [Fact]
    public void ToDataTable_SkipsNullItems()
    {
        List<TestRow> Source = new List<TestRow> { new TestRow { Id = 1, Name = "A" }, null };
        DataTable Result = Source.ToDataTable();
        Assert.Single(Result.Rows);
        Assert.Equal(1, Result.Rows[0]["Id"]);
        Assert.Equal("A", Result.Rows[0]["Name"]);
    }

    // ● private types
    /// <summary>
    /// Test row used by ToDataTable tests.
    /// </summary>
    class TestRow
    {
        // ● properties
        /// <summary>
        /// Test row id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Test row name.
        /// </summary>
        public string Name { get; set; }
    }
}

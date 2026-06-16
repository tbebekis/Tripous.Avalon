namespace Tripous.Tests;

/// <summary>
/// Tests for string extension methods.
/// </summary>
public class StringExtensionsTests
{
    // ● public
    /// <summary>
    /// Ensures Quote escapes single quotes for SQL string literals.
    /// </summary>
    [Fact]
    public void Quote_EscapesSingleQuotes()
    {
        string Result = "O'Brien".Quote();
        Assert.Equal("'O''Brien'", Result);
    }
    /// <summary>
    /// Ensures Quote returns a SQL null literal for null text.
    /// </summary>
    [Fact]
    public void Quote_ReturnsSqlNullForNullText()
    {
        string Text = null;
        string Result = Text.Quote();
        Assert.Equal("null", Result);
    }
    /// <summary>
    /// Ensures ToLines handles all common line endings.
    /// </summary>
    [Fact]
    public void ToLines_HandlesMixedLineEndings()
    {
        string[] Result = "A\r\nB\nC\rD".ToLines();
        Assert.Equal(new[] { "A", "B", "C", "D" }, Result);
    }
    /// <summary>
    /// Ensures SplitCamelCase handles acronym boundaries.
    /// </summary>
    [Fact]
    public void SplitCamelCase_HandlesAcronymBoundaries()
    {
        string Result = "ABCamelDECase".SplitCamelCase();
        Assert.Equal("AB Camel DE Case", Result);
    }
    /// <summary>
    /// Ensures SplitToWords uses acronym-aware word splitting.
    /// </summary>
    [Fact]
    public void SplitToWords_HandlesAcronymBoundaries()
    {
        string Result = "ABCamelDECase".SplitToWords();
        Assert.Equal("AB Camel DE Case", Result);
    }
    /// <summary>
    /// Ensures ToPlural handles words ending in us.
    /// </summary>
    [Fact]
    public void ToPlural_HandlesUsEnding()
    {
        string Result = "Status".ToPlural();
        Assert.Equal("Statuses", Result);
    }
}

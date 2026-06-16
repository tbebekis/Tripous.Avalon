namespace Tripous.Tests;

/// <summary>
/// Tests for dictionary extension methods.
/// </summary>
public class DictionaryExtensionsTests
{
    // ● public
    /// <summary>
    /// Ensures AsFloat preserves decimal values.
    /// </summary>
    [Fact]
    public void AsFloat_PreservesDecimalValue()
    {
        Dictionary<string, object> Source = new Dictionary<string, object>
        {
            ["Amount"] = "12.50"
        };

        double Result = ((IDictionary)Source).AsFloat("Amount", 0);
        Assert.Equal(12.50, Result);
    }
    /// <summary>
    /// Ensures TextToDic keeps equals characters inside values.
    /// </summary>
    [Fact]
    public void TextToDic_KeepsEqualsCharactersInsideValues()
    {
        Dictionary<string, string> Result = new Dictionary<string, string>();
        Result.TextToDic("Connection=A=B");
        Assert.Equal("A=B", Result["Connection"]);
    }
    /// <summary>
    /// Ensures TextToDic ignores empty lines and empty keys.
    /// </summary>
    [Fact]
    public void TextToDic_IgnoresEmptyLinesAndEmptyKeys()
    {
        Dictionary<string, string> Result = new Dictionary<string, string>();
        Result.TextToDic("\r\n=NoKey\nName=Tripous\n");
        Assert.Single(Result);
        Assert.Equal("Tripous", Result["Name"]);
    }
    /// <summary>
    /// Ensures SaveToXml uses the item node name for dictionary entries.
    /// </summary>
    [Fact]
    public void SaveToXml_UsesItemNodeNameForEntries()
    {
        Dictionary<string, string> Source = new Dictionary<string, string>
        {
            ["Name"] = "Tripous"
        };

        XmlDocument Document = new XmlDocument();
        XmlElement Root = Document.CreateElement("root");
        Document.AppendChild(Root);
        Source.SaveToXml(Root, "items", "item");

        XmlNode ItemsNode = Root.SelectSingleNode("items");
        Assert.NotNull(ItemsNode);
        Assert.NotNull(ItemsNode.SelectSingleNode("item"));
        Assert.Null(ItemsNode.SelectSingleNode("items"));
    }
    /// <summary>
    /// Ensures ValuesToRow writes DBNull for null dictionary values.
    /// </summary>
    [Fact]
    public void ValuesToRow_WritesDbNullForNullValues()
    {
        DataTable Table = new DataTable();
        Table.Columns.Add("Name", typeof(string));
        DataRow Row = Table.NewRow();
        Table.Rows.Add(Row);
        Hashtable Values = new Hashtable
        {
            ["Name"] = null
        };

        Values.ValuesToRow(Row);
        Assert.Equal(DBNull.Value, Row["Name"]);
    }
    /// <summary>
    /// Ensures assigning a dictionary to itself does not clear it.
    /// </summary>
    [Fact]
    public void Assign_SameDictionaryDoesNotClear()
    {
        Dictionary<string, string> Source = new Dictionary<string, string>
        {
            ["Name"] = "Tripous"
        };

        Source.Assign(Source);
        Assert.Single(Source);
        Assert.Equal("Tripous", Source["Name"]);
    }
}

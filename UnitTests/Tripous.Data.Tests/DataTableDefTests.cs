namespace Tripous.Data.Tests;

public class DataTableDefTests
{
    [Fact]
    public void Check_ThrowsOnDuplicateFieldNames()
    {
        DataTableDef TableDef = new();
        TableDef.Name = "TestTable";
        TableDef.AddIntegerId();
        TableDef.AddString("Name");
        TableDef.AddString("name");

        Exception Ex = Assert.Throws<Exception>(() => TableDef.Check());
        Assert.Contains("Duplicate column name", Ex.Message);
    }
    [Fact]
    public void Check_ThrowsOnMissingPrimaryKey()
    {
        DataTableDef TableDef = new();
        TableDef.Name = "TestTable";
        TableDef.AddString("Name");

        Exception Ex = Assert.Throws<Exception>(() => TableDef.Check());
        Assert.Contains("No primary key", Ex.Message);
    }
    [Fact]
    public void UniqueConstraint_GetDefText_UsesFieldNames()
    {
        UniqueConstraintDef Def = new();
        Def.Name = "UC_Test";
        Def.FieldNames = "CustomerId, Code";

        string Text = Def.GetDefText();

        Assert.Equal("constraint UC_Test unique (CustomerId, Code)", Text);
    }
    [Fact]
    public void AddStringId_UsesSpecifiedLength()
    {
        DataTableDef TableDef = new();

        DataFieldDef FieldDef = TableDef.AddStringId(Length: 64);

        Assert.Equal(64, FieldDef.Length);
    }
}

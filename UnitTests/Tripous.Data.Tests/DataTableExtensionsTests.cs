namespace Tripous.Data.Tests;

public class DataTableExtensionsTests
{
    [Fact]
    public void ToTable_ReturnsCopiedRows()
    {
        DataTable Table = new();
        Table.Columns.Add("Id", typeof(int));
        Table.Rows.Add(1);
        Table.Rows.Add(2);

        DataTable Result = Table.Rows.Cast<DataRow>().ToTable();

        Assert.Equal(2, Result.Rows.Count);
        Assert.Equal(1, Result.Rows[0]["Id"]);
        Assert.Equal(2, Result.Rows[1]["Id"]);
    }
    [Fact]
    public void CopyExactState_DoesNotDuplicateDeletedRows()
    {
        DataTable Source = new();
        Source.Columns.Add("Id", typeof(int));
        Source.Rows.Add(1);
        Source.AcceptChanges();
        Source.Rows[0].Delete();
        DataTable Dest = new();

        Source.CopyExactState(Dest, CopySchemaToo: true);

        Assert.Single(Dest.Rows.Cast<DataRow>());
        Assert.Equal(DataRowState.Deleted, Dest.Rows[0].RowState);
    }
    [Fact]
    public void Locate_IgnoresDeletedRows()
    {
        DataTable Table = new();
        Table.Columns.Add("Id", typeof(int));
        Table.Rows.Add(1);
        Table.AcceptChanges();
        Table.Rows[0].Delete();

        DataRow Row = Table.Locate("Id", 1, LocateOptions.None);

        Assert.Null(Row);
    }
}

using System.Data;
using Avalonia.Controls;
using Avalonia.Threading;
using Tripous.Data;
using Tripous.Desktop;

namespace BindingStudyApp_01_Test;

public class DesktopBindingSmokeTests
{
    // ● private
    DataTable CreateCustomerTable()
    {
        DataTable Table = new("Customer");
        Table.Columns.Add("Id", typeof(int));
        Table.Columns.Add("Code", typeof(string));
        Table.Columns.Add("Name", typeof(string));
        Table.Columns.Add("IsActive", typeof(bool));
        Table.Rows.Add(1, "CUST-001", "Acme Stores", true);
        Table.Rows.Add(2, "CUST-002", "Northwind Traders", true);
        return Table;
    }

    // ● public
    [Fact]
    public void TextBoxBinding_ReturnsControlBindingInfo()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        TextBox Control = new();

        DataSourceBinding Binding = Source.Bind(Control, "Code");

        Assert.Equal(Source, Binding.Source);
        Assert.Equal(Control, Binding.Control);
        Assert.Equal("Code", Binding.FieldName);
        Assert.Equal(DataSourceBindingKind.Control, Binding.Kind);
        Assert.True(Binding.IsControlBinding);
        Assert.False(Binding.IsGridColumnBinding);

        Binding.Dispose();

        Assert.True(Binding.IsDisposed);
    }
    [Fact]
    public void CheckBoxBinding_ReturnsControlBindingInfo()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        CheckBox Control = new();

        DataSourceBinding Binding = Source.Bind(Control, "IsActive");

        Assert.Equal(DataSourceBindingKind.Control, Binding.Kind);
        Assert.Equal(CheckBox.IsCheckedProperty, Binding.TargetProperty);

        Binding.Dispose();

        Assert.True(Binding.IsDisposed);
    }
    [Fact]
    public void GridBinding_ReturnsGridAndColumnBindingInfo()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        DataGrid Grid = new();

        List<DataSourceBinding> Bindings = Source.Bind(Grid);

        Assert.Equal(6, Bindings.Count);
        Assert.Equal(4, Grid.Columns.Count);
        Assert.Equal(2, Bindings.Count(Binding => Binding.Kind == DataSourceBindingKind.Grid));
        Assert.Equal(4, Bindings.Count(Binding => Binding.Kind == DataSourceBindingKind.GridColumn));
        Assert.Contains(Bindings, Binding => Binding.FieldName == "Code" && Binding.IsGridColumnBinding);

        foreach (DataSourceBinding Binding in Bindings)
            Binding.Dispose();

        Assert.All(Bindings, Binding => Assert.True(Binding.IsDisposed));
    }
    [Fact]
    public void CreateGridColumns_GeneratesColumnsFromProviderFields()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        DataGrid Grid = new();

        List<DataSourceBinding> Bindings = Source.CreateGridColumns(Grid);

        Assert.Equal(4, Bindings.Count);
        Assert.Equal(4, Grid.Columns.Count);
        Assert.Equal("Id", Bindings[0].FieldName);
        Assert.Equal("Code", Bindings[1].FieldName);
        Assert.Equal("Name", Bindings[2].FieldName);
        Assert.Equal("IsActive", Bindings[3].FieldName);
        Assert.IsType<DataGridCheckBoxColumn>(Grid.Columns[3]);
    }
    [Fact]
    public void BindingComplete_MovesToFirstRowWhenCurrentIsEmpty()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        Source.Current = null;

        Source.BindingComplete();
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Background);

        Assert.NotNull(Source.Current);
        Assert.Equal(0, Source.Position);
        Assert.Equal("CUST-001", Source.Current.AsString("Code"));
    }
    [Fact]
    public void BindingComplete_PreservesExistingCurrentRow()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        Source.MoveLast();

        Source.BindingComplete();
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Background);

        Assert.NotNull(Source.Current);
        Assert.Equal(1, Source.Position);
        Assert.Equal("CUST-002", Source.Current.AsString("Code"));
    }
}

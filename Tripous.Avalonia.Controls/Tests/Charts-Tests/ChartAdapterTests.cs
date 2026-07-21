// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Charts.Tests;

/// <summary>
/// Tests chart data adapters.
/// </summary>
public class ChartAdapterTests
{
    // ● private methods
    DataTable CreateTable()
    {
        DataTable Result = new("Sales");
        Result.Columns.Add("Region", typeof(string));
        Result.Columns.Add("Amount", typeof(decimal));
        Result.Columns.Add("Payload", typeof(byte[]));
        Result.Rows.Add("North", 10m, new byte[] { 1 });
        Result.Rows.Add("South", 20m, new byte[] { 2 });
        return Result;
    }

    // ● tests
    /// <summary>
    /// Verifies DataTable/DataView access.
    /// </summary>
    [Fact]
    public void ChartDataViewDataAdapter_WithDataTable_ExposesRowsAndFields()
    {
        ChartDataViewDataAdapter Adapter = new(CreateTable().DefaultView);

        Assert.Equal(2, Adapter.RowCount);
        Assert.Equal("North", Adapter.GetValue(0, "Region"));
        Assert.Equal(10m, Adapter.GetValue(0, "Amount"));
        Assert.Contains(Adapter.SourceFields, Field => Field.Name == "Region" && Field.CanUseAsDimension);
        Assert.Contains(Adapter.SourceFields, Field => Field.Name == "Amount" && Field.CanUseAsMeasure);
        Assert.DoesNotContain(Adapter.SourceFields, Field => Field.Name == "Payload");
    }
    /// <summary>
    /// Verifies POCO list access.
    /// </summary>
    [Fact]
    public void ChartListDataAdapter_WithRows_ExposesProperties()
    {
        List<ChartTestRow> Rows = new()
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m },
        };

        ChartListDataAdapter Adapter = new(Rows);

        Assert.Equal(1, Adapter.RowCount);
        Assert.Equal("Q1", Adapter.GetValue(0, nameof(ChartTestRow.Quarter)));
        Assert.Equal(10m, Adapter.GetValue(0, nameof(ChartTestRow.Amount)));
        Assert.Contains(Adapter.SourceFields, Field => Field.Name == nameof(ChartTestRow.Amount) && Field.IsNumeric);
    }
}

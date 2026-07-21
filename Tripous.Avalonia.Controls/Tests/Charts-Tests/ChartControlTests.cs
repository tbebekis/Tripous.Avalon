// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Charts.Tests;

/// <summary>
/// Tests chart control public APIs that do not require a live window.
/// </summary>
public class ChartControlTests
{
    // ● tests
    /// <summary>
    /// Verifies that the control accepts settings and list data.
    /// </summary>
    [Fact]
    public void ChartControl_WithSettingsAndItemsSource_RebuildsProjection()
    {
        ChartControl Control = new();
        Control.Settings = new ChartSettings
        {
            ChartType = ChartType.Column,
            CategoryFieldName = nameof(ChartTestRow.Region),
            ValueFieldName = nameof(ChartTestRow.Amount),
            AggregateKind = ChartAggregateKind.Sum,
        };
        Control.ItemsSource = new List<ChartTestRow>
        {
            new() { Region = "North", Amount = 10m },
            new() { Region = "North", Amount = 15m },
        };

        Assert.Single(Control.Engine.Series);
        Assert.Single(Control.Engine.CategoryTexts);
        Assert.Equal(25m, Control.Engine.Series[0].Points[0].Value);
        Assert.Equal(ChartType.Column, Control.CreateSettings().ChartType);
    }
    /// <summary>
    /// Verifies that the control accepts DataTable data.
    /// </summary>
    [Fact]
    public void ChartControl_WithDataTableItemsSource_CreatesAdapter()
    {
        DataTable Table = new("Sales");
        Table.Columns.Add("Region", typeof(string));
        Table.Columns.Add("Amount", typeof(decimal));
        Table.Rows.Add("North", 10m);

        ChartControl Control = new();
        Control.ApplySettings(new ChartSettings { CategoryFieldName = "Region", ValueFieldName = "Amount" });
        Control.ItemsSource = Table;

        Assert.IsType<ChartDataViewDataAdapter>(Control.DataAdapter);
        Assert.Equal(10m, Control.Engine.Series[0].Points[0].Value);
    }
    /// <summary>
    /// Verifies the settings menu visibility flag.
    /// </summary>
    [Fact]
    public void IsSettingsMenuItemsVisible_WhenChanged_StoresValue()
    {
        ChartControl Control = new();

        Assert.True(Control.IsSettingsMenuItemsVisible);

        Control.IsSettingsMenuItemsVisible = false;

        Assert.False(Control.IsSettingsMenuItemsVisible);
    }
}

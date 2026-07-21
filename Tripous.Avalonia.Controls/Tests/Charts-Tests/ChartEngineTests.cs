// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Charts.Tests;

/// <summary>
/// Tests the non-visual chart engine.
/// </summary>
public class ChartEngineTests
{
    // ● private methods
    List<ChartTestRow> CreateRows()
    {
        return new List<ChartTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m },
            new() { Region = "North", Quarter = "Q1", Salesperson = "Bianca", Amount = 15m },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m },
            new() { Region = "South", Quarter = "Q1", Salesperson = "Alex", Amount = 7m },
        };
    }
    ChartEngine CreateEngine(ChartAggregateKind AggregateKind = ChartAggregateKind.Sum)
    {
        ChartEngine Result = new();
        Result.ApplySettings(new ChartSettings
        {
            CategoryFieldName = nameof(ChartTestRow.Region),
            SeriesFieldName = nameof(ChartTestRow.Quarter),
            ValueFieldName = nameof(ChartTestRow.Amount),
            AggregateKind = AggregateKind,
            ValueFormat = "N2",
        });
        Result.DataAdapter = new ChartListDataAdapter(CreateRows());
        return Result;
    }

    // ● tests
    /// <summary>
    /// Verifies category and series grouping.
    /// </summary>
    [Fact]
    public void Rebuild_WithCategoryAndSeries_CreatesSeriesPoints()
    {
        ChartEngine Engine = CreateEngine();

        Assert.Equal(new[] { "North", "South" }, Engine.CategoryTexts);
        Assert.Equal(new[] { "Q1", "Q2" }, Engine.Series.Select(Series => Series.Text));
        Assert.Equal(25m, Engine.Series[0].Points[0].Value);
        Assert.Equal(20m, Engine.Series[1].Points[0].Value);
        Assert.Equal(7m, Engine.Series[0].Points[1].Value);
    }
    /// <summary>
    /// Verifies all aggregate kinds.
    /// </summary>
    [Fact]
    public void Rebuild_WithAllAggregateKinds_ReturnsExpectedValues()
    {
        Dictionary<ChartAggregateKind, decimal> Expected = new()
        {
            [ChartAggregateKind.Count] = 2m,
            [ChartAggregateKind.Sum] = 25m,
            [ChartAggregateKind.Min] = 10m,
            [ChartAggregateKind.Max] = 15m,
            [ChartAggregateKind.Average] = 12.5m,
            [ChartAggregateKind.StdDev] = 3.5355m,
            [ChartAggregateKind.StdDevP] = 2.5m,
            [ChartAggregateKind.Variance] = 12.5m,
            [ChartAggregateKind.VarianceP] = 6.25m,
            [ChartAggregateKind.CountDistinct] = 2m,
            [ChartAggregateKind.Product] = 150m,
        };

        foreach (KeyValuePair<ChartAggregateKind, decimal> Entry in Expected)
        {
            ChartEngine Engine = CreateEngine(Entry.Key);
            decimal Value = Engine.Series[0].Points[0].NumericValue;
            Assert.Equal(Entry.Value, Math.Round(Value, 4));
        }
    }
    /// <summary>
    /// Verifies sorting and TopN category selection.
    /// </summary>
    [Fact]
    public void Rebuild_WithTopNAndSort_AppliesCategoryProjectionRules()
    {
        ChartEngine Engine = new();
        Engine.ApplySettings(new ChartSettings
        {
            CategoryFieldName = nameof(ChartTestRow.Region),
            ValueFieldName = nameof(ChartTestRow.Amount),
            AggregateKind = ChartAggregateKind.Sum,
            TopN = 1,
            SortDirection = ChartSortDirection.Descending,
        });
        Engine.DataAdapter = new ChartListDataAdapter(CreateRows());

        Assert.Equal(new[] { "North" }, Engine.CategoryTexts);
        Assert.Equal(45m, Engine.Series[0].Points[0].Value);
    }
}

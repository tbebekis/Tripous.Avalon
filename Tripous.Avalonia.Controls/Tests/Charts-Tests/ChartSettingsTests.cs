// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Charts.Tests;

/// <summary>
/// Tests chart settings serialization.
/// </summary>
public class ChartSettingsTests
{
    // ● tests
    /// <summary>
    /// Verifies settings JSON round-trip.
    /// </summary>
    [Fact]
    public void ChartSettings_WithJson_RoundTrips()
    {
        ChartSettings Source = new()
        {
            Name = "Sales",
            Title = "Sales by Region",
            ChartType = ChartType.Donut,
            CategoryFieldName = "Region",
            SeriesFieldName = "Quarter",
            ValueFieldName = "Amount",
            AggregateKind = ChartAggregateKind.Average,
            SortDirection = ChartSortDirection.Descending,
            TopN = 5,
            ShowLegend = false,
            ShowValueLabels = true,
            ValueFormat = "C2",
            PaletteName = "Signal",
        };

        string Json = JsonSerializer.Serialize(Source);
        ChartSettings Target = JsonSerializer.Deserialize<ChartSettings>(Json);

        Assert.NotNull(Target);
        Assert.Equal(Source.Name, Target.Name);
        Assert.Equal(Source.Title, Target.Title);
        Assert.Equal(Source.ChartType, Target.ChartType);
        Assert.Equal(Source.CategoryFieldName, Target.CategoryFieldName);
        Assert.Equal(Source.SeriesFieldName, Target.SeriesFieldName);
        Assert.Equal(Source.ValueFieldName, Target.ValueFieldName);
        Assert.Equal(Source.AggregateKind, Target.AggregateKind);
        Assert.Equal(Source.SortDirection, Target.SortDirection);
        Assert.Equal(Source.TopN, Target.TopN);
        Assert.Equal(Source.ShowLegend, Target.ShowLegend);
        Assert.Equal(Source.ShowValueLabels, Target.ShowValueLabels);
        Assert.Equal(Source.ValueFormat, Target.ValueFormat);
        Assert.Equal(Source.PaletteName, Target.PaletteName);
    }
}

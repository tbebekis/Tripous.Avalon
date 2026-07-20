// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Pivot.Tests;

/// <summary>
/// Tests built-in pivot grid exporters.
/// </summary>
public class PivotGridExporterTests
{
    // ● private types
    class TestExporter: PivotGridExporter
    {
        /// <inheritdoc />
        public override void Export(PivotGrid Grid, PivotGridExportSnapshot Snapshot, string FilePath)
        {
            WriteText(FilePath, Snapshot.ValueColumns.Count.ToString());
        }
        /// <inheritdoc />
        public override string Name => "Test";
        /// <inheritdoc />
        public override string DefaultExtension => "test";
    }

    // ● private methods
    PivotGrid CreateGrid()
    {
        PivotGrid Result = new();
        Result.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Region), Header = "Region" });
        Result.ColumnFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Quarter), Header = "Quarter" });
        Result.Measures.Add(new PivotGridMeasure { Name = "Amount", Header = "Amount", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.Sum });
        Result.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North, \"A\"", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North, \"A\"", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "South", Quarter = "Q1", Salesperson = "Bianca", Amount = 7m, Units = 1 },
        });
        return Result;
    }
    string TempPath(string Extension)
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "." + Extension);
    }

    // ● tests
    /// <summary>
    /// Verifies that the export snapshot mirrors the visible pivot matrix including totals.
    /// </summary>
    [Fact]
    public void CreateExportSnapshot_WithTotals_ReturnsMatrixRowsAndColumns()
    {
        PivotGrid Grid = CreateGrid();

        PivotGridExportSnapshot Snapshot = Grid.CreateExportSnapshot();

        Assert.Equal(new[] { "Region" }, Snapshot.RowFields.Select(Field => Field.Header));
        Assert.Equal(new[] { "Quarter" }, Snapshot.ColumnFields.Select(Field => Field.Header));
        Assert.Equal(3, Snapshot.ValueColumns.Count);
        Assert.Equal(new[] { "Q1 / Amount", "Q2 / Amount", "Total / Amount" }, Snapshot.ValueColumns.Select(Column => Column.Header));
        Assert.Equal(3, Snapshot.Rows.Count);
        Assert.Equal("North, \"A\"", Snapshot.Rows[0].RowTexts[0]);
        Assert.Equal(30m, Snapshot.Rows[0].Cells[2].Value);
        Assert.True(Snapshot.Rows[2].IsColumnTotal);
        Assert.Equal("Total", Snapshot.Rows[2].RowTexts[0]);
        Assert.Equal(37m, Snapshot.Rows[2].Cells[2].Value);
    }
    /// <summary>
    /// Verifies clipboard text uses a tab-separated visible pivot matrix.
    /// </summary>
    [Fact]
    public void CreateClipboardText_WithVisibleMatrix_ReturnsTabSeparatedText()
    {
        PivotGrid Grid = CreateGrid();

        string Text = Grid.CreateClipboardText();
        string[] Lines = Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Region\tQ1 / Amount\tQ2 / Amount\tTotal / Amount", Lines[0]);
        Assert.Equal("North, \"A\"\t10\t20\t30", Lines[1]);
        Assert.Equal("Total\t17\t20\t37", Lines[3]);
    }
    /// <summary>
    /// Verifies CSV escaping for pivot matrix headers and row values.
    /// </summary>
    [Fact]
    public void CsvExporter_WithSpecialText_EscapesCsvCells()
    {
        string FilePath = TempPath("csv");
        try
        {
            new PivotGridCsvExporter().Export(CreateGrid(), null, FilePath);
            string Text = File.ReadAllText(FilePath);

            Assert.Contains("Region,Q1 / Amount,Q2 / Amount,Total / Amount", Text);
            Assert.Contains("\"North, \"\"A\"\"\",10,20,30", Text);
            Assert.Contains("Total,17,20,37", Text);
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies JSON export includes pivot metadata and value cells.
    /// </summary>
    [Fact]
    public void JsonExporter_WithGrid_WritesFormattedPivotJson()
    {
        string FilePath = TempPath("json");
        try
        {
            new PivotGridJsonExporter().Export(CreateGrid(), null, FilePath);
            string Text = File.ReadAllText(FilePath);
            using JsonDocument Document = JsonDocument.Parse(Text);

            Assert.Contains(Environment.NewLine, Text);
            Assert.Equal("Region", Document.RootElement.GetProperty("RowFields")[0].GetProperty("Name").GetString());
            Assert.Equal("Q1 / Amount", Document.RootElement.GetProperty("ValueColumns")[0].GetProperty("Header").GetString());
            Assert.Equal("North, \"A\"", Document.RootElement.GetProperty("Rows")[0].GetProperty("RowTexts")[0].GetString());
            Assert.Equal(30m, Document.RootElement.GetProperty("Rows")[0].GetProperty("Values")[2].GetProperty("Value").GetDecimal());
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies HTML export encodes pivot matrix cells.
    /// </summary>
    [Fact]
    public void HtmlExporter_WithSpecialText_EncodesMatrix()
    {
        string FilePath = TempPath("html");
        try
        {
            new PivotGridHtmlExporter().Export(CreateGrid(), null, FilePath);
            string Text = File.ReadAllText(FilePath);

            Assert.Contains("<table>", Text);
            Assert.Contains("Q1 / Amount", Text);
            Assert.Contains("North, &quot;A&quot;", Text);
            Assert.Contains("<th class=\"total\">Total / Amount</th>", Text);
            Assert.Contains("<tr class=\"total\">", Text);
            Assert.Contains("<td class=\"value total\">37</td>", Text);
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies exporter registry instance and factory registration.
    /// </summary>
    [Fact]
    public void ExporterRegistry_WithInstanceAndFactory_ReturnsRegisteredExporters()
    {
        PivotGridExporter Instance = new TestExporter();

        PivotGridExporters.Register(Instance);
        PivotGridExporters.Register(() => new TestExporter());
        IReadOnlyList<PivotGridExporter> Exporters = PivotGridExporters.CreateExporters();

        Assert.Contains(Exporters, Exporter => ReferenceEquals(Exporter, Instance));
        Assert.Contains(Exporters, Exporter => Exporter.Name == "Test" && !ReferenceEquals(Exporter, Instance));
    }
    /// <summary>
    /// Verifies save export uses the selected exporter and current snapshot.
    /// </summary>
    [Fact]
    public void SaveExport_WithExporter_WritesExportFile()
    {
        string FilePath = TempPath("test");
        try
        {
            PivotGrid Grid = CreateGrid();
            Grid.SaveExport(new TestExporter(), FilePath);

            Assert.Equal("3", File.ReadAllText(FilePath));
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}

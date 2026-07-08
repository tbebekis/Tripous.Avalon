// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests built-in group grid exporters.
/// </summary>
public class GroupGridExporterTests
{
    // ● private types
    class TestExporter: GroupGridExporter
    {
        /// <inheritdoc />
        public override void Export(GroupGrid Grid, GroupGridExportSnapshot Snapshot, string FilePath)
        {
            WriteText(FilePath, Snapshot.Columns.Count.ToString());
        }
        /// <inheritdoc />
        public override string Name => "Test";
        /// <inheritdoc />
        public override string DefaultExtension => "test";
    }

    // ● private methods
    GroupGridExportSnapshot CreateSnapshot()
    {
        GroupGridExportColumn NameColumn = new(new GroupGridTextColumn { Name = "Name", Header = "Name" });
        GroupGridExportColumn AmountColumn = new(new GroupGridNumberColumn { Name = "Amount", Header = "Amount" });
        GroupGridExportRow GroupRow = new(
            new GroupGridRowInfo(GroupGridRowKind.Group, 0, -1, 0, NameColumn.Column, "Group", null, true),
            "Name: Group",
            Array.Empty<GroupGridExportCell>());
        GroupGridExportRow Row = new(
            new GroupGridRowInfo(GroupGridRowKind.DataRow, 1, 0, 1, null, null, null, false),
            string.Empty,
            new[]
            {
                new GroupGridExportCell(NameColumn, "Alpha, \"One\"", "Alpha, \"One\""),
                new GroupGridExportCell(AmountColumn, 12.5m, "12.5"),
            });
        GroupGridExportRow GroupSummaryRow = new(
            new GroupGridRowInfo(GroupGridRowKind.GroupSummary, 2, -1, 1, null, null, null, false),
            string.Empty,
            new[]
            {
                new GroupGridExportCell(NameColumn, null, string.Empty),
                new GroupGridExportCell(AmountColumn, 12.5m, "sum=12.5"),
            });

        return new GroupGridExportSnapshot(
            new[] { NameColumn, AmountColumn },
            new[] { GroupRow, Row, GroupSummaryRow },
            new[]
            {
                new GroupGridExportCell(NameColumn, null, string.Empty),
                new GroupGridExportCell(AmountColumn, 12.5m, "sum=12.5"),
            });
    }
    string TempPath(string Extension)
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "." + Extension);
    }

    // ● tests
    /// <summary>
    /// Verifies CSV escaping for delimiters and quotes.
    /// </summary>
    [Fact]
    public void CsvExporter_WithSpecialText_EscapesCsvCells()
    {
        string FilePath = TempPath("csv");
        try
        {
            new GroupGridCsvExporter().Export(null, CreateSnapshot(), FilePath);
            string Text = File.ReadAllText(FilePath);

            Assert.Contains("Name,Amount", Text);
            Assert.Contains("\"Alpha, \"\"One\"\"\",12.5", Text);
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies formatted JSON export with column metadata and values.
    /// </summary>
    [Fact]
    public void JsonExporter_WithSnapshot_WritesFormattedJson()
    {
        string FilePath = TempPath("json");
        try
        {
            new GroupGridJsonExporter().Export(null, CreateSnapshot(), FilePath);
            string Text = File.ReadAllText(FilePath);
            using JsonDocument Document = JsonDocument.Parse(Text);

            Assert.Contains(Environment.NewLine, Text);
            Assert.Equal("Name", Document.RootElement.GetProperty("Columns")[0].GetProperty("Name").GetString());
            Assert.Equal("Alpha, \"One\"", Document.RootElement.GetProperty("Rows")[0].GetProperty("Name").GetString());
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies HTML escaping and grouped export rows.
    /// </summary>
    [Fact]
    public void HtmlExporter_WithSpecialTextAndGroups_EncodesFullGrid()
    {
        string FilePath = TempPath("html");
        try
        {
            new GroupGridHtmlExporter().Export(null, CreateSnapshot(), FilePath);
            string Text = File.ReadAllText(FilePath);

            Assert.Contains("<table>", Text);
            Assert.Contains("class=\"group-row\"", Text);
            Assert.Contains("Name: Group", Text);
            Assert.Contains("class=\"data-row\"", Text);
            Assert.Contains("Alpha, &quot;One&quot;", Text);
            Assert.Contains("class=\"group-summary-row\"", Text);
            Assert.Contains("class=\"total-summary-row\"", Text);
            Assert.Contains("sum=12.5", Text);
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
        GroupGridExporter Instance = new TestExporter();

        GroupGridExporters.Register(Instance);
        GroupGridExporters.Register(() => new TestExporter());
        IReadOnlyList<GroupGridExporter> Exporters = GroupGridExporters.CreateExporters();

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
            GroupGrid Grid = new();
            Grid.Columns.Add(new GroupGridTextColumn { Name = "Name" });
            Grid.SaveExport(new TestExporter(), FilePath);

            Assert.Equal("1", File.ReadAllText(FilePath));
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}

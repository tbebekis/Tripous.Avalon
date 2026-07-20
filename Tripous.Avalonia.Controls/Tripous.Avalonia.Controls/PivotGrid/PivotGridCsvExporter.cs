// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports pivot grid matrix rows to CSV.
/// </summary>
public class PivotGridCsvExporter: PivotGridExporter
{
    // ● private methods
    string Escape(string Text)
    {
        Text ??= string.Empty;
        string DelimiterText = Delimiter.ToString();
        bool MustQuote = Text.Contains(DelimiterText, StringComparison.Ordinal)
                         || Text.Contains('"', StringComparison.Ordinal)
                         || Text.Contains('\r', StringComparison.Ordinal)
                         || Text.Contains('\n', StringComparison.Ordinal);

        return MustQuote ? "\"" + Text.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"" : Text;
    }
    PivotGridExportSnapshot GetSnapshot(PivotGrid Grid, PivotGridExportSnapshot Snapshot)
    {
        return Snapshot ?? Grid?.CreateExportSnapshot() ?? new PivotGridExportSnapshot(null, null, null, null, null);
    }

    // ● public methods
    /// <inheritdoc />
    public override void Export(PivotGrid Grid, PivotGridExportSnapshot Snapshot, string FilePath)
    {
        Snapshot = GetSnapshot(Grid, Snapshot);
        StringBuilder Builder = new();
        List<string> Headers = Snapshot.RowFields.Select(Field => Field.Header).ToList();
        Headers.AddRange(Snapshot.ValueColumns.Select(Column => Column.Header));
        Builder.AppendLine(string.Join(Delimiter.ToString(), Headers.Select(Escape)));
        foreach (PivotGridExportRow Row in Snapshot.Rows)
        {
            List<string> Values = Row.RowTexts.ToList();
            Values.AddRange(Row.Cells.Select(Cell => Cell.Text));
            Builder.AppendLine(string.Join(Delimiter.ToString(), Values.Select(Escape)));
        }

        WriteText(FilePath, Builder.ToString());
    }

    // ● properties
    /// <inheritdoc />
    public override string Name => "CSV";
    /// <inheritdoc />
    public override string DefaultExtension => "csv";
    /// <summary>
    /// Gets or sets the CSV delimiter.
    /// </summary>
    public char Delimiter { get; set; } = ',';
}

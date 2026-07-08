// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports group grid data rows to CSV.
/// </summary>
public class GroupGridCsvExporter: GroupGridExporter
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

    // ● public methods
    /// <inheritdoc />
    public override void Export(GroupGrid Grid, GroupGridExportSnapshot Snapshot, string FilePath)
    {
        Snapshot ??= Grid?.CreateExportSnapshot();
        if (Snapshot == null)
            Snapshot = new GroupGridExportSnapshot(Array.Empty<GroupGridExportColumn>(), Array.Empty<GroupGridExportRow>(), Array.Empty<GroupGridExportCell>());

        StringBuilder Builder = new();
        Builder.AppendLine(string.Join(Delimiter.ToString(), Snapshot.Columns.Select(Column => Escape(Column.Header))));
        foreach (GroupGridExportRow Row in Snapshot.GetDataRows())
            Builder.AppendLine(string.Join(Delimiter.ToString(), Row.Cells.Select(Cell => Escape(Cell.Text))));

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

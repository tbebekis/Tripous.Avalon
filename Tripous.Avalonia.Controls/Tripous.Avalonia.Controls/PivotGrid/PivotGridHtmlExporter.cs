// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports pivot grid matrix rows to an HTML table.
/// </summary>
public class PivotGridHtmlExporter: PivotGridExporter
{
    // ● private methods
    string Html(string Text)
    {
        return WebUtility.HtmlEncode(Text ?? string.Empty);
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
        Builder.AppendLine("<!doctype html>");
        Builder.AppendLine("<html>");
        Builder.AppendLine("<head>");
        Builder.AppendLine("<meta charset=\"utf-8\">");
        Builder.AppendLine("<style>");
        Builder.AppendLine("body{font-family:Arial,sans-serif;font-size:14px}");
        Builder.AppendLine("table{border-collapse:collapse}");
        Builder.AppendLine("th,td{border:1px solid #999;padding:4px 8px;text-align:left}");
        Builder.AppendLine("td.value{text-align:right}");
        Builder.AppendLine("th{background:#eee}");
        Builder.AppendLine(".total th,.total td,th.total,td.total{background:#fafafa;font-weight:bold}");
        Builder.AppendLine("</style>");
        Builder.AppendLine("</head>");
        Builder.AppendLine("<body>");
        Builder.AppendLine("<table>");
        Builder.AppendLine("<thead>");
        Builder.AppendLine("<tr>");
        foreach (PivotGridExportField Field in Snapshot.RowFields)
            Builder.Append("<th>").Append(Html(Field.Header)).AppendLine("</th>");
        foreach (PivotGridExportValueColumn Column in Snapshot.ValueColumns)
            Builder.Append(Column.IsTotal ? "<th class=\"total\">" : "<th>")
                .Append(Html(Column.Header))
                .AppendLine("</th>");
        Builder.AppendLine("</tr>");
        Builder.AppendLine("</thead>");
        Builder.AppendLine("<tbody>");
        foreach (PivotGridExportRow Row in Snapshot.Rows)
        {
            Builder.Append(Row.IsColumnTotal ? "<tr class=\"total\">" : "<tr>").AppendLine();
            foreach (string Text in Row.RowTexts)
                Builder.Append("<th>").Append(Html(Text)).AppendLine("</th>");
            foreach (PivotGridExportCell Cell in Row.Cells)
                Builder.Append(Cell.Column.IsTotal ? "<td class=\"value total\">" : "<td class=\"value\">")
                    .Append(Html(Cell.Text))
                    .AppendLine("</td>");
            Builder.AppendLine("</tr>");
        }
        Builder.AppendLine("</tbody>");
        Builder.AppendLine("</table>");
        Builder.AppendLine("</body>");
        Builder.AppendLine("</html>");

        WriteText(FilePath, Builder.ToString());
    }

    // ● properties
    /// <inheritdoc />
    public override string Name => "HTML";
    /// <inheritdoc />
    public override string DefaultExtension => "html";
}

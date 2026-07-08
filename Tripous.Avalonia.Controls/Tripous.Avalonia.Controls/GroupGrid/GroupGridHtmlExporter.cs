// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports group grid data rows to an HTML table.
/// </summary>
public class GroupGridHtmlExporter: GroupGridExporter
{
    // ● private methods
    string Html(string Text)
    {
        return WebUtility.HtmlEncode(Text ?? string.Empty);
    }
    void AppendCells(StringBuilder Builder, IEnumerable<GroupGridExportCell> Cells, string Tag)
    {
        foreach (GroupGridExportCell Cell in Cells)
            Builder.Append('<').Append(Tag).Append('>')
                .Append(Html(Cell.Text))
                .Append("</").Append(Tag).AppendLine(">");
    }
    void AppendDataRow(StringBuilder Builder, GroupGridExportRow Row)
    {
        Builder.AppendLine("<tr class=\"data-row\">");
        AppendCells(Builder, Row.Cells, "td");
        Builder.AppendLine("</tr>");
    }
    void AppendGroupRow(StringBuilder Builder, GroupGridExportRow Row, int ColumnCount)
    {
        int ColSpan = Math.Max(1, ColumnCount);
        int Padding = Math.Max(0, Row.Level) * 18;
        Builder.Append("<tr class=\"group-row\"><td colspan=\"")
            .Append(ColSpan.ToString(CultureInfo.InvariantCulture))
            .Append("\" style=\"padding-left:")
            .Append(Padding.ToString(CultureInfo.InvariantCulture))
            .Append("px\">")
            .Append(Html(Row.GroupText))
            .AppendLine("</td></tr>");
    }
    void AppendSummaryRow(StringBuilder Builder, GroupGridExportRow Row)
    {
        Builder.AppendLine("<tr class=\"group-summary-row\">");
        AppendCells(Builder, Row.Cells, "td");
        Builder.AppendLine("</tr>");
    }
    void AppendTotalSummary(StringBuilder Builder, GroupGridExportSnapshot Snapshot)
    {
        if (Snapshot.TotalSummaryCells.Count == 0)
            return;

        Builder.AppendLine("<tfoot>");
        Builder.AppendLine("<tr class=\"total-summary-row\">");
        AppendCells(Builder, Snapshot.TotalSummaryCells, "td");
        Builder.AppendLine("</tr>");
        Builder.AppendLine("</tfoot>");
    }

    // ● public methods
    /// <inheritdoc />
    public override void Export(GroupGrid Grid, GroupGridExportSnapshot Snapshot, string FilePath)
    {
        Snapshot ??= Grid?.CreateExportSnapshot();
        if (Snapshot == null)
            Snapshot = new GroupGridExportSnapshot(Array.Empty<GroupGridExportColumn>(), Array.Empty<GroupGridExportRow>(), Array.Empty<GroupGridExportCell>());

        StringBuilder Builder = new();
        Builder.AppendLine("<!doctype html>");
        Builder.AppendLine("<html>");
        Builder.AppendLine("<head>");
        Builder.AppendLine("<meta charset=\"utf-8\">");
        Builder.AppendLine("<style>");
        Builder.AppendLine("body{font-family:Arial,sans-serif;font-size:14px}");
        Builder.AppendLine("table{border-collapse:collapse}");
        Builder.AppendLine("th,td{border:1px solid #999;padding:4px 8px;text-align:left}");
        Builder.AppendLine("th{background:#eee}");
        Builder.AppendLine(".group-row td{background:#f3f3f3;font-weight:bold}");
        Builder.AppendLine(".group-summary-row td,.total-summary-row td{background:#fafafa;font-weight:bold}");
        Builder.AppendLine("</style>");
        Builder.AppendLine("</head>");
        Builder.AppendLine("<body>");
        Builder.AppendLine("<table>");
        Builder.AppendLine("<thead>");
        Builder.AppendLine("<tr>");
        foreach (GroupGridExportColumn Column in Snapshot.Columns)
            Builder.Append("<th>").Append(Html(Column.Header)).AppendLine("</th>");
        Builder.AppendLine("</tr>");
        Builder.AppendLine("</thead>");
        Builder.AppendLine("<tbody>");
        foreach (GroupGridExportRow Row in Snapshot.Rows)
        {
            if (Row.IsGroup)
                AppendGroupRow(Builder, Row, Snapshot.Columns.Count);
            else if (Row.IsGroupSummary)
                AppendSummaryRow(Builder, Row);
            else if (Row.IsDataRow)
                AppendDataRow(Builder, Row);
        }
        Builder.AppendLine("</tbody>");
        AppendTotalSummary(Builder, Snapshot);
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

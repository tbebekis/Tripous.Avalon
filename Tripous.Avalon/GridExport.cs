namespace Tripous.Avalon;

public class GridExport
{
    private DataGrid _grid;
    private TopLevel _topLevel;

    public GridExport()
    {
    }

    // Η σωστή έκδοση
    public async Task ExportToAsync(DataGrid grid, DataGridClipboardExportFormat exportFormat)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _topLevel = TopLevel.GetTopLevel(grid)
                    ?? throw new InvalidOperationException("Grid is not attached to a TopLevel.");
        
        var fileType = GetFileType(exportFormat);

        var file = await _topLevel.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = $"Export to {exportFormat}",
                SuggestedFileName = $"Export.{GetExtension(exportFormat)}",
                DefaultExtension = GetExtension(exportFormat),
                ShowOverwritePrompt = true,
                FileTypeChoices = new[] { fileType }
            });

        if (file is null)
            return;

        string content = BuildContent(exportFormat);

        await using var stream = await file.OpenWriteAsync();
        stream.SetLength(0);

        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        await writer.WriteAsync(content);
        await writer.FlushAsync();
    }

    // Αν επιμένεις να έχεις void signature για event handler-style χρήση
    public async void ExportTo(DataGrid grid, DataGridClipboardExportFormat exportFormat)
    {
        await ExportToAsync(grid, exportFormat);
    }

    private string BuildContent(DataGridClipboardExportFormat format)
    {
        var columns = GetExportColumns();
        var rows = GetRows()
            .Select(item => columns.ToDictionary(
                c => c.Header,
                c => FormatCellValue(GetCellValue(item, c.Column))))
            .ToList();

        return format switch
        {
            DataGridClipboardExportFormat.Text => BuildText(rows, columns),
            DataGridClipboardExportFormat.Csv => BuildCsv(rows, columns),
            DataGridClipboardExportFormat.Html => BuildHtml(rows, columns),
            DataGridClipboardExportFormat.Markdown => BuildMarkdown(rows, columns),
            DataGridClipboardExportFormat.Xml => BuildXml(rows, columns),
            DataGridClipboardExportFormat.Yaml => BuildYaml(rows, columns),
            DataGridClipboardExportFormat.Json => BuildJson(rows),
            _ => throw new NotSupportedException($"Unsupported export format: {format}")
        };
    }

    private List<ExportColumn> GetExportColumns()
    {
        return _grid.Columns
            .Where(c => c.IsVisible)
            .OrderBy(c => c.DisplayIndex)
            .Select(c => new ExportColumn(c, GetColumnHeader(c)))
            .ToList();
    }

    private IEnumerable<object> GetRows()
    {
        if (_grid.ItemsSource is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                yield return item;
        }
    }

    private static string GetColumnHeader(DataGridColumn column)
    {
        return column.Header?.ToString() ?? string.Empty;
    }

    private static string FormatCellValue(object value)
    {
        if (value is null || value == DBNull.Value)
            return string.Empty;

        return value switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static object GetCellValue(object item, DataGridColumn column)
    {
        if (item is null)
            return null;

        string memberName = GetMemberName(column);

        if (string.IsNullOrWhiteSpace(memberName))
            return null;

        // DataRowView
        if (item is DataRowView drv)
        {
            return drv.Row.Table.Columns.Contains(memberName)
                ? drv[memberName]
                : null;
        }

        // DataRow
        if (item is DataRow dr)
        {
            return dr.Table.Columns.Contains(memberName)
                ? dr[memberName]
                : null;
        }

        // IDictionary<string, object>
        if (item is IDictionary<string, object> dict)
        {
            return dict.TryGetValue(memberName, out var v) ? v : null;
        }

        // IDictionary
        if (item is IDictionary nonGenericDict && nonGenericDict.Contains(memberName))
        {
            return nonGenericDict[memberName];
        }

        // POCO property
        var prop = item.GetType().GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (prop != null)
            return prop.GetValue(item);

        // POCO field
        var field = item.GetType().GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (field != null)
            return field.GetValue(item);

        return null;
    }

    private static string GetMemberName(DataGridColumn column)
    {
        if (!string.IsNullOrWhiteSpace(column.SortMemberPath))
            return column.SortMemberPath;

        if (column is DataGridBoundColumn boundColumn && boundColumn.Binding is Avalonia.Data.Binding binding)
        {
            if (!string.IsNullOrWhiteSpace(binding.Path))
                return binding.Path;
        }

        return null;
    }

    private static string BuildText(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Join('\t', columns.Select(c => c.Header)));

        foreach (var row in rows)
            sb.AppendLine(string.Join('\t', columns.Select(c => row[c.Header])));

        return sb.ToString();
    }

    private static string BuildCsv(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", columns.Select(c => CsvEscape(c.Header))));

        foreach (var row in rows)
            sb.AppendLine(string.Join(",", columns.Select(c => CsvEscape(row[c.Header]))));

        return sb.ToString();
    }

    private static string BuildHtml(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<table>");
        sb.AppendLine("  <thead>");
        sb.AppendLine("    <tr>");
        foreach (var col in columns)
            sb.AppendLine($"      <th>{HtmlEncode(col.Header)}</th>");
        sb.AppendLine("    </tr>");
        sb.AppendLine("  </thead>");
        sb.AppendLine("  <tbody>");

        foreach (var row in rows)
        {
            sb.AppendLine("    <tr>");
            foreach (var col in columns)
                sb.AppendLine($"      <td>{HtmlEncode(row[col.Header])}</td>");
            sb.AppendLine("    </tr>");
        }

        sb.AppendLine("  </tbody>");
        sb.AppendLine("</table>");

        return sb.ToString();
    }

    private static string BuildMarkdown(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var sb = new StringBuilder();

        sb.AppendLine("| " + string.Join(" | ", columns.Select(c => MdEscape(c.Header))) + " |");
        sb.AppendLine("| " + string.Join(" | ", columns.Select(_ => "---")) + " |");

        foreach (var row in rows)
            sb.AppendLine("| " + string.Join(" | ", columns.Select(c => MdEscape(row[c.Header]))) + " |");

        return sb.ToString();
    }

    private static string BuildXml(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var doc = new XDocument(
            new XElement("rows",
                rows.Select(row =>
                    new XElement("row",
                        columns.Select(col =>
                            new XElement(XmlSafeName(col.Header), row[col.Header]))))));

        return doc.ToString();
    }

    private static string BuildYaml(List<Dictionary<string, string>> rows, List<ExportColumn> columns)
    {
        var sb = new StringBuilder();

        foreach (var row in rows)
        {
            sb.AppendLine("-");
            foreach (var col in columns)
                sb.AppendLine($"  {YamlKey(col.Header)}: {YamlScalar(row[col.Header])}");
        }

        return sb.ToString();
    }

    private static string BuildJson(List<Dictionary<string, string>> rows)
    {
        return JsonSerializer.Serialize(rows, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static string CsvEscape(string s)
    {
        s ??= string.Empty;

        bool mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\r') || s.Contains('\n');
        if (mustQuote)
            return "\"" + s.Replace("\"", "\"\"") + "\"";

        return s;
    }

    private static string HtmlEncode(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        return s.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
    }

    private static string MdEscape(string s)
    {
        return (s ?? string.Empty).Replace("|", "\\|").Replace("\r", " ").Replace("\n", "<br/>");
    }

    private static string XmlSafeName(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return "Column";

        var chars = s.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray();
        var name = new string(chars);

        if (!char.IsLetter(name[0]) && name[0] != '_')
            name = "_" + name;

        return name;
    }

    private static string YamlKey(string s)
    {
        return "\"" + (s ?? string.Empty).Replace("\"", "\\\"") + "\"";
    }

    private static string YamlScalar(string s)
    {
        return "\"" + (s ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n") + "\"";
    }

    private static string GetExtension(DataGridClipboardExportFormat format)
    {
        return format switch
        {
            DataGridClipboardExportFormat.Text => "txt",
            DataGridClipboardExportFormat.Csv => "csv",
            DataGridClipboardExportFormat.Html => "html",
            DataGridClipboardExportFormat.Markdown => "md",
            DataGridClipboardExportFormat.Xml => "xml",
            DataGridClipboardExportFormat.Yaml => "yaml",
            DataGridClipboardExportFormat.Json => "json",
            _ => "txt"
        };
    }

    private static FilePickerFileType GetFileType(DataGridClipboardExportFormat format)
    {
        return format switch
        {
            DataGridClipboardExportFormat.Text => new FilePickerFileType("Text")
            {
                Patterns = new[] { "*.txt" }
            },
            DataGridClipboardExportFormat.Csv => new FilePickerFileType("CSV")
            {
                Patterns = new[] { "*.csv" }
            },
            DataGridClipboardExportFormat.Html => new FilePickerFileType("HTML")
            {
                Patterns = new[] { "*.html", "*.htm" }
            },
            DataGridClipboardExportFormat.Markdown => new FilePickerFileType("Markdown")
            {
                Patterns = new[] { "*.md" }
            },
            DataGridClipboardExportFormat.Xml => new FilePickerFileType("XML")
            {
                Patterns = new[] { "*.xml" }
            },
            DataGridClipboardExportFormat.Yaml => new FilePickerFileType("YAML")
            {
                Patterns = new[] { "*.yaml", "*.yml" }
            },
            DataGridClipboardExportFormat.Json => new FilePickerFileType("JSON")
            {
                Patterns = new[] { "*.json" }
            },
            _ => new FilePickerFileType("Text")
            {
                Patterns = new[] { "*.txt" }
            }
        };
    }

    private sealed record ExportColumn(DataGridColumn Column, string Header);
}
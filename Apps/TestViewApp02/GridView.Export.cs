using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;
using Tripous.Data;

namespace Tripous.Avalon;

public enum GridViewExportFormat
{
    Csv,
    Json,
    HtmlTable,
    HtmlGrouped,
    XlsxTable,
    XlsxGrouped,
}

public enum GridViewExportRowType
{
    Data,
    Group,
    Footer,
    GrandTotal,
}

public enum GridViewBlobExportMode
{
    Skip,
    Placeholder,
    InlineBase64
}

public class GridViewExportOptions: SettingsBase
{
    // ● construction
    public GridViewExportOptions()
    {
    }

    // ● public methods
    public GridViewExportOptions Clone()
    {
        return (GridViewExportOptions)MemberwiseClone();
    }
    public void Check()
    {
        if (string.IsNullOrWhiteSpace(ExportFilePath))
            throw new ApplicationException("Export file path is required.");
    }

    public CultureInfo GetCulture() => CultureInfo.GetCultureInfo(Culture);
    public Encoding GetEncoding() => System.Text.Encoding.GetEncoding(Encoding);
    
    // ● properties
    public GridViewExportFormat Format { get; set; }
    public string ExportFilePath { get; set; }
    public string Culture { get; set; } = "en-US";
    public string Encoding { get; set; } = "utf-8";
    //public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    //public Encoding Encoding { get; set; } = new UTF8Encoding(true);
    public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm";
    public string NumberFormat { get; set; } = "N2";
    public bool IncludeHeaders { get; set; } = true;
    public bool IncludeGroups { get; set; } = true;
    public bool IncludeFooters { get; set; } = true;
    public bool IncludeGrandTotal { get; set; } = true;
    public bool OnlyVisibleColumns { get; set; } = true;
    public bool UseFormattedValues { get; set; } = true;
    public bool IndentJson { get; set; } = true;
    public string CsvDelimiter { get; set; } = ",";
    public bool CsvQuoteAll { get; set; } = true;
    public string HtmlTitle { get; set; } = "Grid Export";
    public string ExcelSheetName { get; set; } = "Sheet1";

    public string BooleanTrueValue { get; set; } = "x";
    public string BooleanFalseValue { get; set; } = "";

    public GridViewBlobExportMode BlobMode { get; set; } = GridViewBlobExportMode.Placeholder;
    public string BlobPlaceholderText { get; set; } = "[BLOB]";
    public string MemoPlaceholderText { get; set; } = "[MEMO]";
}

public class GridViewExportColumn
{
    // ● construction
    public GridViewExportColumn()
    {
    }

    // ● public methods
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName) ? FieldName : base.ToString();

    // ● properties
    public string FieldName { get; set; }
    public string Title { get; set; }
    public Type DataType { get; set; }
    public int VisibleIndex { get; set; } = -1;
    public int GroupIndex { get; set; } = -1;
    public bool IsReadOnly { get; set; }
    public bool IsBlob { get; set; }
    public BlobType BlobType { get; set; }
    public GridViewColumnDef ColumnDef { get; set; }
}

public class GridViewExportRow
{
    // ● construction
    public GridViewExportRow()
    {
    }

    // ● public methods
    public override string ToString() => RowType.ToString();
    public object GetValue(string FieldName)
    {
        if (Values == null || Columns == null || string.IsNullOrWhiteSpace(FieldName))
            return null;

        for (int i = 0; i < Columns.Count; i++)
        {
            if (string.Equals(Columns[i].FieldName, FieldName, StringComparison.OrdinalIgnoreCase))
                return i < Values.Count ? Values[i] : null;
        }

        return null;
    }

    // ● properties
    internal List<GridViewExportColumn> Columns { get; set; }
    public GridViewExportRowType RowType { get; set; }
    public int Level { get; set; }
    public int LabelColumnIndex { get; set; } = -1;
    public List<object> Values { get; set; } = new();
    public GridDataRow SourceRow { get; set; }
    public object Tag { get; set; }
    public bool IsData => RowType == GridViewExportRowType.Data;
    public bool IsGroup => RowType == GridViewExportRowType.Group;
    public bool IsFooter => RowType == GridViewExportRowType.Footer;
    public bool IsGrandTotal => RowType == GridViewExportRowType.GrandTotal;
}

public class GridViewExportData
{
    // ● construction
    public GridViewExportData()
    {
    }

    // ● public methods
    public override string ToString() => $"Columns: {Columns.Count}, Rows: {Rows.Count}";

    // ● properties
    public List<GridViewExportColumn> Columns { get; set; } = new();
    public List<GridViewExportRow> Rows { get; set; } = new();
    public GridViewData SourceData { get; set; }
    public GridViewExportOptions Options { get; set; }
}

static internal class GridViewExportFormatHelper
{
    // ● static public methods
    static public string FormatExportValue(object Value, GridViewColumnDef ColumnDef, GridViewExportOptions Options)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        CultureInfo Culture = Options?.GetCulture() ?? CultureInfo.InvariantCulture;
        Type DataType = ColumnDef != null ? Nullable.GetUnderlyingType(ColumnDef.DataType) ?? ColumnDef.DataType : null;

        if (ColumnDef != null && ColumnDef.IsLookup)
            return GridViewLookupHelper.GetDisplayTextByValue(ColumnDef, Value);

        if (ColumnDef != null && ColumnDef.IsBlob)
        {
            return Options != null && Options.BlobMode == GridViewBlobExportMode.InlineBase64
                ? GetBlobBase64(Value) ?? GetBlobPlaceholder(ColumnDef, Options)
                : GetBlobPlaceholder(ColumnDef, Options);
        }

        if (DataType != null && DataType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(DataType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        if (DataType == typeof(bool))
        {
            bool BoolValue = false;

            try
            {
                BoolValue = Convert.ToBoolean(Value, Culture);
            }
            catch
            {
            }

            return BoolValue
                ? (Options?.BooleanTrueValue ?? "x")
                : (Options?.BooleanFalseValue ?? string.Empty);
        }

        if (DataType == typeof(DateTime))
        {
            string Format = !string.IsNullOrWhiteSpace(Options?.DateTimeFormat)
                ? Options.DateTimeFormat
                : !string.IsNullOrWhiteSpace(ColumnDef?.DisplayFormat) ? ColumnDef.DisplayFormat : "yyyy-MM-dd HH:mm";

            return ((DateTime)Value).ToString(Format, Culture);
        }

        if (DataType == typeof(DateTimeOffset))
        {
            string Format = !string.IsNullOrWhiteSpace(Options?.DateTimeFormat)
                ? Options.DateTimeFormat
                : !string.IsNullOrWhiteSpace(ColumnDef?.DisplayFormat) ? ColumnDef.DisplayFormat : "yyyy-MM-dd HH:mm";

            return ((DateTimeOffset)Value).ToString(Format, Culture);
        }

        if (DataType != null && DataType.IsNumeric())
        {
            string Format = !string.IsNullOrWhiteSpace(Options?.NumberFormat)
                ? Options.NumberFormat
                : ColumnDef?.DisplayFormat;

            if (!string.IsNullOrWhiteSpace(Format) && Value is IFormattable Formattable)
                return Formattable.ToString(Format, Culture);
        }

        if (Value is IFormattable FormattableValue && !string.IsNullOrWhiteSpace(ColumnDef?.DisplayFormat))
            return FormattableValue.ToString(ColumnDef.DisplayFormat, Culture);

        return Convert.ToString(Value, Culture) ?? string.Empty;
    }
    static public string GetBlobPlaceholder(GridViewColumnDef ColumnDef, GridViewExportOptions Options)
    {
        if (ColumnDef != null && ColumnDef.BlobType == BlobType.Text)
            return Options?.MemoPlaceholderText ?? "[MEMO]";

        return Options?.BlobPlaceholderText ?? "[BLOB]";
    }
    static public byte[] GetBlobBytes(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (Value is byte[] Bytes)
            return Bytes;

        if (Value is Stream Stream)
        {
            long Position = 0;
            bool CanSeek = Stream.CanSeek;

            if (CanSeek)
            {
                Position = Stream.Position;
                Stream.Position = 0;
            }

            try
            {
                using MemoryStream MS = new();
                Stream.CopyTo(MS);
                return MS.ToArray();
            }
            finally
            {
                if (CanSeek)
                    Stream.Position = Position;
            }
        }

        return null;
    }
    static public string GetBlobBase64(object Value)
    {
        byte[] Bytes = GetBlobBytes(Value);
        return Bytes != null && Bytes.Length > 0
            ? Convert.ToBase64String(Bytes)
            : null;
    }

    static public string GetExportFileExtension(this GridViewExportFormat Format)
    {
        switch (Format)
        {
            case  GridViewExportFormat.Csv : return "csv";
            case  GridViewExportFormat.Json: return  "json";
            case  GridViewExportFormat.HtmlTable: return  "html";
            case  GridViewExportFormat.HtmlGrouped: return  "html";
            case  GridViewExportFormat.XlsxTable: return   "xlsx";
            case  GridViewExportFormat.XlsxGrouped: return   "xlsx";       
        }

        return "txt";
    }
}

static internal class GridViewExportBuilder
{
    // ● private methods
    static private List<GridViewColumnDef> GetColumns(GridViewData Data, GridViewExportOptions Options)
    {
        if (Data?.ViewDef == null)
            return new List<GridViewColumnDef>();

        GridViewDef Def = Data.ViewDef;

        if (Options != null && Options.OnlyVisibleColumns)
        {
            List<GridViewColumnDef> GroupColumns = Def.GetGroupColumns().OrderBy(x => x.GroupIndex).ToList();
            List<GridViewColumnDef> VisibleColumns = Def.GetVisibleColumns().ToList();
            List<GridViewColumnDef> NonGroupColumns = VisibleColumns
                .Where(x => x.GroupIndex < 0)
                .OrderBy(x => x.VisibleIndex)
                .ToList();

            return GroupColumns.Concat(NonGroupColumns).ToList();
        }

        List<GridViewColumnDef> VisibleGroupColumns = Def.Columns
            .Where(x => x.GroupIndex >= 0)
            .OrderBy(x => x.GroupIndex)
            .ToList();

        List<GridViewColumnDef> OtherColumns = Def.Columns
            .Where(x => x.GroupIndex < 0)
            .OrderBy(x => x.VisibleIndex >= 0 ? x.VisibleIndex : int.MaxValue)
            .ToList();

        return VisibleGroupColumns.Concat(OtherColumns).ToList();
    }
    static private object GetCellValue(GridDataRow Row, GridViewColumnDef ColumnDef, GridViewExportOptions Options)
    {
        if (Row == null || ColumnDef == null)
            return null;

        object Value = Row.GetValue(ColumnDef.FieldName);
        if (Options != null && Options.UseFormattedValues)
            return GridViewExportFormatHelper.FormatExportValue(Value, ColumnDef, Options);

        return Value;
    }

    static private GridViewExportRowType GetRowType(GridDataRow Row)
    {
        if (Row == null)
            return GridViewExportRowType.Data;

        if (Row.IsGroup)
            return GridViewExportRowType.Group;

        if (Row.IsFooter)
            return GridViewExportRowType.Footer;

        if (Row.IsGrandTotal)
            return GridViewExportRowType.GrandTotal;

        return GridViewExportRowType.Data;
    }
    static private bool IncludeRow(GridDataRow Row, GridViewExportOptions Options, bool Grouped)
    {
        if (Row == null)
            return false;

        if (!Grouped)
            return Row.IsData;

        if (Row.IsGroup)
            return Options == null || Options.IncludeGroups;

        if (Row.IsFooter)
            return Options == null || Options.IncludeFooters;

        if (Row.IsGrandTotal)
            return Options == null || Options.IncludeGrandTotal;

        return true;
    }
    static private GridViewExportColumn CreateColumn(GridViewColumnDef Source)
    {
        return new GridViewExportColumn()
        {
            FieldName = Source.FieldName,
            Title = Source.Caption,
            DataType = Source.DataType,
            VisibleIndex = Source.VisibleIndex,
            GroupIndex = Source.GroupIndex,
            IsReadOnly = Source.IsReadOnly,
            IsBlob = Source.IsBlob,
            BlobType = Source.BlobType,
            ColumnDef = Source,
        };
    }
    
    // ● static public methods
    static public GridViewExportData CreateTabular(GridViewData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        GridViewExportData Result = new()
        {
            SourceData = Data,
            Options = Options,
        };

        List<GridViewColumnDef> SourceColumns = GetColumns(Data, Options);
        Dictionary<string, GridViewColumnDef> ColumnMap = SourceColumns.ToDictionary(x => x.FieldName, StringComparer.OrdinalIgnoreCase);

        foreach (GridViewColumnDef ColumnDef in SourceColumns)
            Result.Columns.Add(CreateColumn(ColumnDef));

        foreach (GridDataRow SourceRow in Data.Rows)
        {
            if (!IncludeRow(SourceRow, Options, false))
                continue;

            GridViewExportRow Row = new()
            {
                Columns = Result.Columns,
                SourceRow = SourceRow,
                RowType = GridViewExportRowType.Data,
                Level = SourceRow.Level,
            };

            foreach (GridViewExportColumn ExportColumn in Result.Columns)
            {
                GridViewColumnDef ColumnDef = ColumnMap[ExportColumn.FieldName];
                Row.Values.Add(GetCellValue(SourceRow, ColumnDef, Options));
            }

            Result.Rows.Add(Row);
        }

        return Result;
    }
    static public GridViewExportData CreateGrouped(GridViewData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        GridViewExportData Result = new()
        {
            SourceData = Data,
            Options = Options,
        };

        List<GridViewColumnDef> SourceColumns = GetColumns(Data, Options);
        Dictionary<string, GridViewColumnDef> ColumnMap = SourceColumns.ToDictionary(x => x.FieldName, StringComparer.OrdinalIgnoreCase);

        foreach (GridViewColumnDef ColumnDef in SourceColumns)
            Result.Columns.Add(CreateColumn(ColumnDef));

        foreach (GridDataRow SourceRow in Data.Rows)
        {
            if (!IncludeRow(SourceRow, Options, true))
                continue;

            GridViewExportRow Row = new()
            {
                Columns = Result.Columns,
                SourceRow = SourceRow,
                RowType = GetRowType(SourceRow),
                Level = SourceRow.Level,
            };

            for (int i = 0; i < Result.Columns.Count; i++)
                Row.Values.Add(null);

            if (SourceRow.IsData)
            {
                for (int i = 0; i < Result.Columns.Count; i++)
                {
                    GridViewExportColumn ExportColumn = Result.Columns[i];
                    GridViewColumnDef ColumnDef = ColumnMap[ExportColumn.FieldName];
                    Row.Values[i] = GetCellValue(SourceRow, ColumnDef, Options);
                }
            }
            else if (SourceRow.IsGroup)
            {
                GridViewNode Node = SourceRow.Node;
                string FieldName = Node != null ? Node.FieldName : string.Empty;

                int LabelColumnIndex = Result.Columns.FindIndex(x => string.Equals(x.FieldName, FieldName, StringComparison.OrdinalIgnoreCase));
                if (LabelColumnIndex < 0)
                    LabelColumnIndex = Math.Min(Math.Max(SourceRow.Level, 0), Result.Columns.Count - 1);

                Row.LabelColumnIndex = LabelColumnIndex;

                GridViewColumnDef ColumnDef = !string.IsNullOrWhiteSpace(FieldName) && ColumnMap.ContainsKey(FieldName)
                    ? ColumnMap[FieldName]
                    : null;

                string Title = LabelColumnIndex >= 0 && LabelColumnIndex < Result.Columns.Count
                    ? Result.Columns[LabelColumnIndex].Title
                    : FieldName;

                string KeyText = GridViewExportFormatHelper.FormatExportValue(Node != null ? Node.Key : null, ColumnDef, Options);

                if (LabelColumnIndex >= 0 && LabelColumnIndex < Row.Values.Count)
                    Row.Values[LabelColumnIndex] = $"{Title} = {KeyText}";
            }
            else if (SourceRow.IsFooter || SourceRow.IsGrandTotal)
            {
                GridViewNode OwnerGroup = SourceRow.Node != null ? SourceRow.Node.OwnerGroup : null;

                if (SourceRow.IsGrandTotal || OwnerGroup == null || OwnerGroup.IsRoot)
                {
                    Row.LabelColumnIndex = 0;

                    if (Row.LabelColumnIndex >= 0 && Row.LabelColumnIndex < Row.Values.Count)
                        Row.Values[Row.LabelColumnIndex] = "Grand Total";
                }
                else
                {
                    string FieldName = OwnerGroup.FieldName ?? string.Empty;

                    int LabelColumnIndex = Result.Columns.FindIndex(x => string.Equals(x.FieldName, FieldName, StringComparison.OrdinalIgnoreCase));
                    if (LabelColumnIndex < 0)
                        LabelColumnIndex = 0;

                    Row.LabelColumnIndex = LabelColumnIndex;

                    GridViewColumnDef ColumnDef = !string.IsNullOrWhiteSpace(FieldName) && ColumnMap.ContainsKey(FieldName)
                        ? ColumnMap[FieldName]
                        : null;

                    string Title = LabelColumnIndex >= 0 && LabelColumnIndex < Result.Columns.Count
                        ? Result.Columns[LabelColumnIndex].Title
                        : FieldName;

                    string KeyText = GridViewExportFormatHelper.FormatExportValue(OwnerGroup.Key, ColumnDef, Options);

                    if (LabelColumnIndex >= 0 && LabelColumnIndex < Row.Values.Count)
                        Row.Values[LabelColumnIndex] = $"Total ({Title} = {KeyText})";
                }

                if (SourceRow.Node != null)
                {
                    foreach (GridViewSummary Summary in SourceRow.Node.Summaries)
                    {
                        int Index = Result.Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));

                        if (Index >= 0 && Index < Row.Values.Count)
                        {
                            GridViewColumnDef ColumnDef = ColumnMap[Summary.FieldName];
                            Row.Values[Index] = Options != null && Options.UseFormattedValues
                                ? GridViewExportFormatHelper.FormatExportValue(Summary.Value, ColumnDef, Options)
                                : Summary.Value;
                        }
                    }
                }
            }

            Result.Rows.Add(Row);
        }

        return Result;
    }
}

static internal class GridViewCsvExporter
{
    // ● private methods
    static private string Escape(object Value, GridViewExportOptions Options)
    {
        string Text = Convert.ToString(Value, Options?.GetCulture() ?? CultureInfo.InvariantCulture) ?? string.Empty;
        string Delimiter = Options != null && !string.IsNullOrWhiteSpace(Options.CsvDelimiter) ? Options.CsvDelimiter : ",";
        bool MustQuote = Options != null && Options.CsvQuoteAll;

        if (Text.Contains('"'))
        {
            Text = Text.Replace("\"", "\"\"");
            MustQuote = true;
        }

        if (Text.Contains("\r") || Text.Contains("\n") || Text.Contains(Delimiter))
            MustQuote = true;

        return MustQuote ? $"\"{Text}\"" : Text;
    }

    // ● static public methods
    static public void Export(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        StringBuilder SB = new();
        string Delimiter = !string.IsNullOrWhiteSpace(Options.CsvDelimiter) ? Options.CsvDelimiter : ",";

        if (Options.IncludeHeaders)
        {
            SB.AppendLine(string.Join(Delimiter, Data.Columns.Select(x => Escape(x.Title, Options))));
        }

        foreach (GridViewExportRow Row in Data.Rows)
            SB.AppendLine(string.Join(Delimiter, Row.Values.Select(x => Escape(x, Options))));

        File.WriteAllText(Options.ExportFilePath, SB.ToString(), Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class GridViewJsonExporter
{
    // ● private methods
    static private object ProcessValue(object Value, GridViewColumnDef ColumnDef, GridViewExportOptions Options)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (ColumnDef != null && ColumnDef.IsBlob)
        {
            switch (Options.BlobMode)
            {
                case GridViewBlobExportMode.Skip:
                    return null;

                case GridViewBlobExportMode.Placeholder:
                    return GridViewExportFormatHelper.GetBlobPlaceholder(ColumnDef, Options);

                case GridViewBlobExportMode.InlineBase64:
                    return GridViewExportFormatHelper.GetBlobBase64(Value) ?? GridViewExportFormatHelper.GetBlobPlaceholder(ColumnDef, Options);

                default:
                    return null;
            }
        }

        if (Options.UseFormattedValues)
            return GridViewExportFormatHelper.FormatExportValue(Value, ColumnDef, Options);

        return Value;
    }
    static private List<Dictionary<string, object>> CreateList(GridViewExportData Data, GridViewExportOptions Options)
    {
        List<Dictionary<string, object>> Result = new();

        foreach (GridViewExportRow Row in Data.Rows)
        {
            Dictionary<string, object> Item = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Data.Columns.Count; i++)
            {
                GridViewExportColumn ExportColumn = Data.Columns[i];
                object Value = i < Row.Values.Count ? Row.Values[i] : null;

                Item[ExportColumn.FieldName] = ProcessValue(Value, ExportColumn.ColumnDef, Options);
            }

            Result.Add(Item);
        }

        return Result;
    }

    // ● static public methods
    static public void Export(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = Options.IndentJson,
        };

        string Text = JsonSerializer.Serialize(CreateList(Data, Options), JsonOptions);
        File.WriteAllText(Options.ExportFilePath, Text, Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class GridViewHtmlExporter
{
    // ● private methods
    static private string Encode(object Value, GridViewExportOptions Options, GridViewExportColumn Column = null)
    {
        if (Value is bool B)
        {
            string Text = B
                ? (Options?.BooleanTrueValue ?? "x")
                : (Options?.BooleanFalseValue ?? string.Empty);

            return System.Net.WebUtility.HtmlEncode(Text);
        }

        string Text2 = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
        return System.Net.WebUtility.HtmlEncode(Text2);
    }
    static private string GetCssClass(GridViewExportRow Row)
    {
        if (Row == null)
            return "data";

        if (Row.IsGrandTotal)
            return "grand-total";

        return Row.RowType.ToString().ToLowerInvariant();
    }
    static private string GetStyle()
    {
        return """
               <style>
               body { font-family: Segoe UI, Arial, sans-serif; font-size: 13px; }
               table.grid { border-collapse: collapse; border-spacing: 0; }
               table.grid th, table.grid td { border: 1px solid #777; padding: 4px 6px; vertical-align: top; }
               table.grid th { background: #efefef; font-weight: bold; }
               tr.group td { background: #f5f5f5; font-weight: bold; }
               tr.footer td, tr.grand-total td { background: #fafafa; font-weight: bold; }
               td.group-label,
               tr.group td.group-label,
               tr.footer td.group-label,
               tr.grand-total td.group-label { text-align: left !important; padding-left: 4px; }
               td.group-label.level-0 { padding-left: 6px; }
               td.group-label.level-1 { padding-left: 4px; }
               td.group-label.level-2 { padding-left: 42px; }
               td.group-label.level-3 { padding-left: 60px; }
               td.group-label.level-4 { padding-left: 78px; }
               td.num { text-align: right; }
               td.date { text-align: right; white-space: nowrap; }
               td.bool { text-align: center; }
               </style>
               """;
    }
    static private string GetCellCssClass(GridViewExportColumn Column)
    {
        if (Column?.DataType == null)
            return string.Empty;

        Type DataType = Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType;

        if (DataType == typeof(bool))
            return "bool";

        if (DataType.IsDateTime())
            return "date";

        if (DataType.IsNumeric())
            return "num";

        return string.Empty;
    }
    static private void AppendHeader(StringBuilder SB, GridViewExportData Data)
    {
        SB.AppendLine("<thead>");
        SB.AppendLine("<tr>");

        foreach (GridViewExportColumn Column in Data.Columns)
            SB.AppendLine($"<th>{Encode(Column.Title, Data.Options)}</th>");

        SB.AppendLine("</tr>");
        SB.AppendLine("</thead>");
    }
    static private void AppendTableRows(StringBuilder SB, GridViewExportData Data)
    {
        SB.AppendLine("<tbody>");

        foreach (GridViewExportRow Row in Data.Rows)
        {
            SB.AppendLine("<tr class=\"data\">");

            for (int i = 0; i < Data.Columns.Count; i++)
            {
                GridViewExportColumn Column = Data.Columns[i];
                object Value = i < Row.Values.Count ? Row.Values[i] : null;
                string CssClass = GetCellCssClass(Column);

                if (string.IsNullOrWhiteSpace(CssClass))
                    SB.AppendLine($"<td>{Encode(Value, Data.Options, Column)}</td>");
                else
                    SB.AppendLine($"<td class=\"{Encode(CssClass, Data.Options)}\">{Encode(Value, Data.Options, Column)}</td>");
            }

            SB.AppendLine("</tr>");
        }

        SB.AppendLine("</tbody>");
    }
    static private void AppendGroupedRows(StringBuilder SB, GridViewExportData Data)
    {
        SB.AppendLine("<tbody>");

        foreach (GridViewExportRow Row in Data.Rows)
        {
            string RowCssClass = GetCssClass(Row);
            SB.AppendLine($"<tr class=\"{RowCssClass}\">");

            for (int i = 0; i < Data.Columns.Count; i++)
            {
                GridViewExportColumn Column = Data.Columns[i];
                object Value = i < Row.Values.Count ? Row.Values[i] : null;
                string Text = Encode(Value, Data.Options, Column);
                string CssClass = string.Empty;

                if (i == Row.LabelColumnIndex)
                    CssClass = $"group-label level-{Row.Level}";
                else
                    CssClass = GetCellCssClass(Column);

                if (string.IsNullOrWhiteSpace(CssClass))
                    SB.AppendLine($"<td>{Text}</td>");
                else
                    SB.AppendLine($"<td class=\"{CssClass}\">{Text}</td>");
            }

            SB.AppendLine("</tr>");
        }

        SB.AppendLine("</tbody>");
    }

    // ● static public methods
    static public void ExportTable(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        StringBuilder SB = new();
        SB.AppendLine("<!doctype html>");
        SB.AppendLine("<html>");
        SB.AppendLine("<head>");
        SB.AppendLine("<meta charset=\"utf-8\">");
        SB.AppendLine($"<title>{Encode(Options.HtmlTitle, Options)}</title>");
        SB.AppendLine(GetStyle());
        SB.AppendLine("</head>");
        SB.AppendLine("<body>");
        SB.AppendLine("<table class=\"grid\">");

        if (Options.IncludeHeaders)
            AppendHeader(SB, Data);

        AppendTableRows(SB, Data);

        SB.AppendLine("</table>");
        SB.AppendLine("</body>");
        SB.AppendLine("</html>");

        File.WriteAllText(Options.ExportFilePath, SB.ToString(), Options.GetEncoding() ?? new UTF8Encoding(true));
    }
    static public void ExportGrouped(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        StringBuilder SB = new();
        SB.AppendLine("<!doctype html>");
        SB.AppendLine("<html>");
        SB.AppendLine("<head>");
        SB.AppendLine("<meta charset=\"utf-8\">");
        SB.AppendLine($"<title>{Encode(Options.HtmlTitle, Options)}</title>");
        SB.AppendLine(GetStyle());
        SB.AppendLine("</head>");
        SB.AppendLine("<body>");
        SB.AppendLine("<table class=\"grid\">");

        if (Options.IncludeHeaders)
            AppendHeader(SB, Data);

        AppendGroupedRows(SB, Data);

        SB.AppendLine("</table>");
        SB.AppendLine("</body>");
        SB.AppendLine("</html>");

        File.WriteAllText(Options.ExportFilePath, SB.ToString(), Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class GridViewXlsxExporter
{
    // ● private methods
    static private string GetBoolText(bool Value, GridViewExportOptions Options)
    {
        return Value
            ? (Options?.BooleanTrueValue ?? "x")
            : (Options?.BooleanFalseValue ?? string.Empty);
    }
    static private bool IsDateColumn(GridViewExportColumn Column)
    {
        if (Column?.DataType == null)
            return false;

        Type DataType = Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType;
        return DataType == typeof(DateTime) || DataType == typeof(DateTimeOffset);
    }
    static private bool IsNumericColumn(GridViewExportColumn Column)
    {
        if (Column?.DataType == null)
            return false;

        Type DataType = Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType;
        return DataType.IsNumeric();
    }
    static private byte GetOutlineLevel(GridViewExportRow Row)
    {
        if (Row == null)
            return 0;

        int Level = Math.Max(Row.Level, 0);
        Level = Math.Min(Level + 1, 8);
        return (byte)Level;
    }
    static private void ApplyBorderAndBaseAlignment(IXLCell Cell)
    {
        Cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }
    static private void ApplyDateStyle(IXLCell Cell, GridViewExportOptions Options)
    {
        string Format = !string.IsNullOrWhiteSpace(Options.DateTimeFormat)
            ? Options.DateTimeFormat
            : "yyyy-MM-dd HH:mm";

        Cell.Style.DateFormat.Format = Format;
        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }
    static private void ApplyNumericStyle(IXLCell Cell)
    {
        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }
    static private void ApplyBoolStyle(IXLCell Cell)
    {
        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }
    static private bool TryGetDateValue(object Value, GridViewExportColumn Column, out DateTime Result)
    {
        Result = default;

        if (!IsDateColumn(Column) || Value == null || Value == DBNull.Value)
            return false;

        try
        {
            if (Value is DateTime DT)
            {
                Result = DT;
                return true;
            }

            if (Value is DateTimeOffset DTO)
            {
                Result = DTO.DateTime;
                return true;
            }

            if (Value is string S && !string.IsNullOrWhiteSpace(S))
            {
                if (DateTime.TryParse(S, CultureInfo.InvariantCulture, DateTimeStyles.None, out DT))
                {
                    Result = DT;
                    return true;
                }

                if (DateTimeOffset.TryParse(S, CultureInfo.InvariantCulture, DateTimeStyles.None, out DTO))
                {
                    Result = DTO.DateTime;
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }
    static private bool TryGetNumericValue(object Value, GridViewExportColumn Column, out double Result)
    {
        Result = 0;

        if (!IsNumericColumn(Column) || Value == null || Value == DBNull.Value)
            return false;

        try
        {
            if (Value is byte B8)
            {
                Result = B8;
                return true;
            }

            if (Value is short S16)
            {
                Result = S16;
                return true;
            }

            if (Value is int S32)
            {
                Result = S32;
                return true;
            }

            if (Value is long S64)
            {
                Result = S64;
                return true;
            }

            if (Value is float F)
            {
                Result = F;
                return true;
            }

            if (Value is double D)
            {
                Result = D;
                return true;
            }

            if (Value is decimal M)
            {
                Result = Convert.ToDouble(M, CultureInfo.InvariantCulture);
                return true;
            }

            if (Value is string S && !string.IsNullOrWhiteSpace(S))
            {
                if (double.TryParse(S, NumberStyles.Any, CultureInfo.InvariantCulture, out D))
                {
                    Result = D;
                    return true;
                }

                if (double.TryParse(S, NumberStyles.Any, CultureInfo.CurrentCulture, out D))
                {
                    Result = D;
                    return true;
                }
            }

            Result = Convert.ToDouble(Value, CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
        }

        return false;
    }
    static private void SetCellValue(IXLCell Cell, object Value, GridViewExportColumn Column, GridViewExportOptions Options)
    {
        ApplyBorderAndBaseAlignment(Cell);

        if (Value == null || Value == DBNull.Value)
        {
            Cell.Clear();
            return;
        }

        if (Value is bool B)
        {
            Cell.Value = GetBoolText(B, Options);
            ApplyBoolStyle(Cell);
            return;
        }

        if (TryGetDateValue(Value, Column, out DateTime DT))
        {
            Cell.Value = DT;
            ApplyDateStyle(Cell, Options);
            return;
        }

        if (TryGetNumericValue(Value, Column, out double D))
        {
            Cell.Value = D;
            ApplyNumericStyle(Cell);
            return;
        }

        switch (Value)
        {
            case string S:
                Cell.Value = S;
                break;
            case DateTimeOffset DTO:
                Cell.Value = DTO.DateTime;
                ApplyDateStyle(Cell, Options);
                break;
            case DateTime DT2:
                Cell.Value = DT2;
                ApplyDateStyle(Cell, Options);
                break;
            default:
                Cell.Value = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
                break;
        }
    }
    static private void WriteHeader(IXLWorksheet WS, GridViewExportData Data, int RowIndex)
    {
        for (int c = 0; c < Data.Columns.Count; c++)
        {
            IXLCell Cell = WS.Cell(RowIndex, c + 1);
            Cell.Value = Data.Columns[c].Title;
            Cell.Style.Font.Bold = true;
            Cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFEFEF");
        }
    }
    static private void WriteDataRow(IXLWorksheet WS, GridViewExportData Data, GridViewExportRow Row, int RowIndex, GridViewExportOptions Options)
    {
        for (int c = 0; c < Data.Columns.Count; c++)
        {
            GridViewExportColumn Column = Data.Columns[c];
            object Value = c < Row.Values.Count ? Row.Values[c] : null;
            IXLCell Cell = WS.Cell(RowIndex, c + 1);
            SetCellValue(Cell, Value, Column, Options);
        }
    }
    static private void WriteGroupedRow(IXLWorksheet WS, GridViewExportData Data, GridViewExportRow Row, int RowIndex, GridViewExportOptions Options)
    {
        for (int c = 0; c < Data.Columns.Count; c++)
        {
            GridViewExportColumn Column = Data.Columns[c];
            object Value = c < Row.Values.Count ? Row.Values[c] : null;
            IXLCell Cell = WS.Cell(RowIndex, c + 1);

            SetCellValue(Cell, Value, Column, Options);

            if (c == Row.LabelColumnIndex)
            {
                Cell.Style.Font.Bold = true;
                Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                int Indent = Math.Min(Math.Max(Row.Level, 0), 15);
                Cell.Style.Alignment.Indent = Indent;
            }
            else if (Row.IsFooter || Row.IsGrandTotal)
            {
                if (Value != null && Value != DBNull.Value)
                    Cell.Style.Font.Bold = true;
            }
        }

        if (Row.IsGroup)
            WS.Row(RowIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
        else if (Row.IsFooter || Row.IsGrandTotal)
            WS.Row(RowIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");

        WS.Row(RowIndex).OutlineLevel = GetOutlineLevel(Row);
    }

    // ● static public methods
    static public void ExportTable(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        using XLWorkbook WB = new();
        string SheetName = !string.IsNullOrWhiteSpace(Options.ExcelSheetName) ? Options.ExcelSheetName : "Sheet1";
        IXLWorksheet WS = WB.Worksheets.Add(SheetName);

        int RowIndex = 1;

        if (Options.IncludeHeaders)
        {
            WriteHeader(WS, Data, RowIndex);
            WS.SheetView.FreezeRows(1);
            RowIndex++;
        }

        foreach (GridViewExportRow Row in Data.Rows.Where(x => x.IsData))
        {
            WriteDataRow(WS, Data, Row, RowIndex, Options);
            RowIndex++;
        }

        WS.Columns().AdjustToContents();
        WB.SaveAs(Options.ExportFilePath);
    }
    static public void ExportGrouped(GridViewExportData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        using XLWorkbook WB = new();
        string SheetName = !string.IsNullOrWhiteSpace(Options.ExcelSheetName) ? Options.ExcelSheetName : "Sheet1";
        IXLWorksheet WS = WB.Worksheets.Add(SheetName);

        int RowIndex = 1;

        if (Options.IncludeHeaders)
        {
            WriteHeader(WS, Data, RowIndex);
            WS.SheetView.FreezeRows(1);
            RowIndex++;
        }

        WS.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Bottom;

        foreach (GridViewExportRow Row in Data.Rows)
        {
            WriteGroupedRow(WS, Data, Row, RowIndex, Options);
            RowIndex++;
        }

        WS.Columns().AdjustToContents();
        WB.SaveAs(Options.ExportFilePath);
    }
}

static public class GridViewExporter
{
    // ● private methods
    static private GridViewExportOptions NormalizeOptions(GridViewExportOptions Options)
    {
        Options ??= new GridViewExportOptions();
        Options.Check();
        return Options;
    }
    static private GridViewData GetData(GridView GridView)
    {
        if (GridView == null)
            throw new ArgumentNullException(nameof(GridView));

        if (GridView.Controller == null)
            throw new ApplicationException("GridView has no controller.");

        GridViewData Result = GridView.Controller.Data;
        if (Result == null)
            throw new ApplicationException("GridView has no data.");

        return Result;
    }
    static private GridViewExportData CreateExportData(GridViewData Data, GridViewExportOptions Options)
    {
        if (Options.Format == GridViewExportFormat.Json)
        {
            GridViewExportOptions JsonBuildOptions = Options.Clone();
            JsonBuildOptions.UseFormattedValues = false;
            return GridViewExportBuilder.CreateTabular(Data, JsonBuildOptions);
        }

        return Options.Format switch
        {
            GridViewExportFormat.Csv => GridViewExportBuilder.CreateTabular(Data, Options),
            GridViewExportFormat.HtmlTable => GridViewExportBuilder.CreateTabular(Data, Options),
            GridViewExportFormat.HtmlGrouped => GridViewExportBuilder.CreateGrouped(Data, Options),
            GridViewExportFormat.XlsxTable => GridViewExportBuilder.CreateTabular(Data, Options),
            GridViewExportFormat.XlsxGrouped => GridViewExportBuilder.CreateGrouped(Data, Options),
            _ => throw new NotSupportedException($"Unsupported export format: {Options.Format}"),
        };
    }
    
    // ● static public methods
    static public void Export(GridView GridView, GridViewExportOptions Options)
    {
        Export(GetData(GridView), Options);
    }
    static public void Export(GridViewData Data, GridViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        Options = NormalizeOptions(Options);
        GridViewExportData ExportData = CreateExportData(Data, Options);

        switch (Options.Format)
        {
            case GridViewExportFormat.Csv:
                GridViewCsvExporter.Export(ExportData, Options);
                break;
            case GridViewExportFormat.Json:
                GridViewJsonExporter.Export(ExportData, Options);
                break;
            case GridViewExportFormat.HtmlTable:
                GridViewHtmlExporter.ExportTable(ExportData, Options);
                break;
            case GridViewExportFormat.HtmlGrouped:
                GridViewHtmlExporter.ExportGrouped(ExportData, Options);
                break;
            case GridViewExportFormat.XlsxTable:
                GridViewXlsxExporter.ExportTable(ExportData, Options);
                break;
            case GridViewExportFormat.XlsxGrouped:
                GridViewXlsxExporter.ExportGrouped(ExportData, Options);
                break;
            default:
                throw new NotSupportedException($"Unsupported export format: {Options.Format}");
        }
    }
}

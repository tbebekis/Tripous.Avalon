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

public enum PivotViewExportFormat
{
    Csv,
    Xlsx,
    Html,
    Json,
}

public class PivotViewExportOptions: SettingsBase
{
    // ● construction
    public PivotViewExportOptions()
    {
    }

    // ● public methods
    public PivotViewExportOptions Clone()
    {
        return (PivotViewExportOptions)MemberwiseClone();
    }
    public void Check()
    {
        if (string.IsNullOrWhiteSpace(ExportFilePath))
            throw new ApplicationException("Export file path is required.");
    }
    
    public CultureInfo GetCulture() => CultureInfo.GetCultureInfo(Culture);
    public Encoding GetEncoding() => System.Text.Encoding.GetEncoding(Encoding);

    // ● properties
    public PivotViewExportFormat Format { get; set; } = PivotViewExportFormat.Xlsx;
    public string ExportFilePath { get; set; }
    public string Culture { get; set; } = "en-US";
    public string Encoding { get; set; } = "utf-8";
    public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm";
    public string NumberFormat { get; set; } = "N2";
    public bool IncludeHeaders { get; set; } = true;
    public bool UseFormattedValues { get; set; } = true;
    public string CsvDelimiter { get; set; } = ",";
    public bool CsvQuoteAll { get; set; } = true;
    public string HtmlTitle { get; set; } = "Pivot Export";
    public string ExcelSheetName { get; set; } = "Pivot";
    public bool IndentJson { get; set; } = true;
    public bool BooleanAsCheckMark { get; set; } = true;
    public string BooleanTrueValue { get; set; } = "x";
    public string BooleanFalseValue { get; set; } = "";
}

public class PivotViewExportColumn
{
    // ● construction
    public PivotViewExportColumn()
    {
    }

    // ● public methods
    public override string ToString() => !string.IsNullOrWhiteSpace(Key) ? Key : base.ToString();

    // ● properties
    public string Key { get; set; }
    public string Title { get; set; }
    public Type DataType { get; set; }
    public string Format { get; set; }
    public PivotDataColumnKind Kind { get; set; }
    public int RowLevel { get; set; } = -1;
    public PivotFieldDef SourceField { get; set; }
    public PivotDataColumn Column { get; set; }
}

public class PivotViewExportRow
{
    // ● construction
    public PivotViewExportRow()
    {
    }

    // ● public methods
    public override string ToString() => RowType.ToString();
    public object GetValue(string Key)
    {
        if (Values == null || Columns == null || string.IsNullOrWhiteSpace(Key))
            return null;

        for (int i = 0; i < Columns.Count; i++)
        {
            if (string.Equals(Columns[i].Key, Key, StringComparison.OrdinalIgnoreCase))
                return i < Values.Count ? Values[i] : null;
        }

        return null;
    }

    // ● properties
    internal List<PivotViewExportColumn> Columns { get; set; }
    public PivotDataRowType RowType { get; set; }
    public int Level { get; set; }
    public List<object> Values { get; set; } = new();
    public PivotDataRow SourceRow { get; set; }
    public object Tag { get; set; }
    public bool IsNormal => RowType == PivotDataRowType.Normal;
    public bool IsSubtotal => RowType == PivotDataRowType.Subtotal;
    public bool IsGrandTotal => RowType == PivotDataRowType.GrandTotal;
}

public class PivotViewExportData
{
    // ● construction
    public PivotViewExportData()
    {
    }

    // ● public methods
    public override string ToString() => $"Columns: {Columns.Count}, Rows: {Rows.Count}";

    // ● properties
    public List<PivotViewExportColumn> Columns { get; set; } = new();
    public List<PivotViewExportRow> Rows { get; set; } = new();
    public PivotData SourceData { get; set; }
    public PivotViewExportOptions Options { get; set; }
}

static internal class PivotViewExportFormatHelper
{
    // ● private methods
    static private Type GetDataType(PivotViewExportColumn Column)
    {
        return Column != null
            ? Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType
            : null;
    }

    // ● static public methods
    static public string FormatExportValue(object Value, PivotViewExportColumn Column, PivotViewExportOptions Options)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        CultureInfo Culture = Options?.GetCulture() ?? CultureInfo.InvariantCulture;
        Type DataType = GetDataType(Column);

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

            if (Options != null && Options.BooleanAsCheckMark)
                return BoolValue ? (Options.BooleanTrueValue ?? "x") : (Options.BooleanFalseValue ?? string.Empty);

            return BoolValue.ToString(CultureInfo.InvariantCulture);
        }

        if (DataType == typeof(DateTime))
        {
            string Format = !string.IsNullOrWhiteSpace(Column?.Format)
                ? Column.Format
                : !string.IsNullOrWhiteSpace(Options?.DateTimeFormat) ? Options.DateTimeFormat : "yyyy-MM-dd HH:mm";

            return ((DateTime)Value).ToString(Format, Culture);
        }

        if (DataType == typeof(DateTimeOffset))
        {
            string Format = !string.IsNullOrWhiteSpace(Column?.Format)
                ? Column.Format
                : !string.IsNullOrWhiteSpace(Options?.DateTimeFormat) ? Options.DateTimeFormat : "yyyy-MM-dd HH:mm";

            return ((DateTimeOffset)Value).ToString(Format, Culture);
        }

        if (DataType != null && DataType.IsNumeric())
        {
            string Format = !string.IsNullOrWhiteSpace(Column?.Format)
                ? Column.Format
                : Options?.NumberFormat;

            if (!string.IsNullOrWhiteSpace(Format) && Value is IFormattable Formattable)
                return Formattable.ToString(Format, Culture);
        }

        if (Value is IFormattable FormattableValue && !string.IsNullOrWhiteSpace(Column?.Format))
            return FormattableValue.ToString(Column.Format, Culture);

        return Convert.ToString(Value, Culture) ?? string.Empty;
    }
    static public object GetJsonValue(object Value, PivotViewExportColumn Column, PivotViewExportOptions Options)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (Options != null && Options.UseFormattedValues)
            return FormatExportValue(Value, Column, Options);

        Type DataType = GetDataType(Column);

        if (DataType == typeof(bool))
        {
            try
            {
                return Convert.ToBoolean(Value, Options?.GetCulture() ?? CultureInfo.InvariantCulture);
            }
            catch
            {
                return Value;
            }
        }

        if (DataType == typeof(DateTime))
        {
            try
            {
                return Convert.ToDateTime(Value, Options?.GetCulture() ?? CultureInfo.InvariantCulture);
            }
            catch
            {
                return Value;
            }
        }

        if (DataType == typeof(DateTimeOffset))
        {
            if (Value is DateTimeOffset DTO)
                return DTO;

            try
            {
                return DateTimeOffset.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty, CultureInfo.InvariantCulture);
            }
            catch
            {
                return Value;
            }
        }

        return Value;
    }
    static public string GetExportFileExtension(this PivotViewExportFormat Format)
    {
        switch (Format)
        {
            case PivotViewExportFormat.Csv: return "csv";
            case PivotViewExportFormat.Xlsx: return "xlsx";
            case PivotViewExportFormat.Html: return "html";
            case PivotViewExportFormat.Json: return "json";
        }

        return "txt";
    }
}

static internal class PivotViewExportBuilder
{
    // ● private methods
    static private PivotViewExportColumn CreateColumn(PivotDataColumn Source)
    {
        return new PivotViewExportColumn
        {
            Key = Source.Key,
            Title = Source.Caption,
            DataType = Source.DataType,
            Format = Source.Format,
            Kind = Source.Kind,
            RowLevel = Source.RowLevel,
            SourceField = Source.SourceField,
            Column = Source,
        };
    }

    // ● static public methods
    static public PivotViewExportData Create(PivotData Data, PivotViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        PivotViewExportData Result = new()
        {
            SourceData = Data,
            Options = Options,
        };

        foreach (PivotDataColumn Column in Data.Columns)
            Result.Columns.Add(CreateColumn(Column));

        foreach (PivotDataRow SourceRow in Data.Rows)
        {
            PivotViewExportRow Row = new()
            {
                Columns = Result.Columns,
                SourceRow = SourceRow,
                RowType = SourceRow.RowType,
                Level = SourceRow.Level,
                Tag = SourceRow.Tag,
            };

            if (SourceRow.Values != null)
                Row.Values.AddRange(SourceRow.Values);

            Result.Rows.Add(Row);
        }

        return Result;
    }
}

static internal class PivotViewCsvExporter
{
    // ● private methods
    static private string Escape(object Value, PivotViewExportOptions Options)
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
    static private object GetCellValue(PivotViewExportRow Row, PivotViewExportColumn Column, int ColumnIndex, PivotViewExportOptions Options)
    {
        object Value = ColumnIndex < Row.Values.Count ? Row.Values[ColumnIndex] : null;

        if (Options != null && Options.UseFormattedValues)
            return PivotViewExportFormatHelper.FormatExportValue(Value, Column, Options);

        return Value;
    }

    // ● static public methods
    static public void Export(PivotViewExportData Data, PivotViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));
        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        StringBuilder SB = new();
        string Delimiter = !string.IsNullOrWhiteSpace(Options.CsvDelimiter) ? Options.CsvDelimiter : ",";

        if (Options.IncludeHeaders)
            SB.AppendLine(string.Join(Delimiter, Data.Columns.Select(x => Escape(x.Title, Options))));

        foreach (PivotViewExportRow Row in Data.Rows)
        {
            List<string> Values = new();

            for (int i = 0; i < Data.Columns.Count; i++)
                Values.Add(Escape(GetCellValue(Row, Data.Columns[i], i, Options), Options));

            SB.AppendLine(string.Join(Delimiter, Values));
        }

        File.WriteAllText(Options.ExportFilePath, SB.ToString(), Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class PivotViewJsonExporter
{
    // ● private methods
    static private Dictionary<string, object> CreateRowObject(PivotViewExportData Data, PivotViewExportRow Row, PivotViewExportOptions Options)
    {
        Dictionary<string, object> Result = new(StringComparer.OrdinalIgnoreCase)
        {
            ["RowType"] = Row.RowType.ToString(),
            ["Level"] = Row.Level,
        };

        for (int i = 0; i < Data.Columns.Count; i++)
        {
            PivotViewExportColumn Column = Data.Columns[i];
            object Value = i < Row.Values.Count ? Row.Values[i] : null;
            Result[Column.Key] = PivotViewExportFormatHelper.GetJsonValue(Value, Column, Options);
        }

        return Result;
    }
    static private object CreatePayload(PivotViewExportData Data, PivotViewExportOptions Options)
    {
        return new
        {
            Columns = Data.Columns.Select(x => new
            {
                x.Key,
                Title = x.Title,
                DataType = x.DataType != null ? x.DataType.FullName : null,
                x.Format,
                Kind = x.Kind.ToString(),
                x.RowLevel,
            }).ToList(),
            Rows = Data.Rows.Select(x => CreateRowObject(Data, x, Options)).ToList(),
        };
    }

    // ● static public methods
    static public void Export(PivotViewExportData Data, PivotViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));
        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = Options.IndentJson,
        };

        string Text = JsonSerializer.Serialize(CreatePayload(Data, Options), JsonOptions);
        File.WriteAllText(Options.ExportFilePath, Text, Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class PivotViewHtmlExporter
{
    // ● private methods
    static private string Encode(object Value)
    {
        string Text = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
        return System.Net.WebUtility.HtmlEncode(Text);
    }
    static private string EncodeMultiline(object Value)
    {
        string Text = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
        Text = System.Net.WebUtility.HtmlEncode(Text);
        return Text.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
    }
    static private string GetRowCssClass(PivotViewExportRow Row)
    {
        if (Row == null)
            return "normal";

        return Row.RowType switch
        {
            PivotDataRowType.Subtotal => "subtotal",
            PivotDataRowType.GrandTotal => "grand-total",
            _ => "normal"
        };
    }
    static private string GetCellCssClass(PivotViewExportColumn Column)
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
    static private string GetStyle()
    {
        return """
               <style>
               body { font-family: Segoe UI, Arial, sans-serif; font-size: 13px; }
               table.pivot { border-collapse: collapse; border-spacing: 0; }
               table.pivot th, table.pivot td { border: 1px solid #777; padding: 4px 6px; vertical-align: middle; }
               table.pivot th { background: #efefef; font-weight: bold; white-space: normal; }
               tr.subtotal td { background: #f7f7f7; font-weight: bold; }
               tr.grand-total td { background: #e6e6e6; font-weight: bold; }
               td.num { text-align: right; }
               td.date { text-align: right; white-space: nowrap; }
               td.bool { text-align: center; }
               </style>
               """;
    }
    static private void AppendHeader(StringBuilder SB, PivotViewExportData Data)
    {
        SB.AppendLine("<thead>");
        SB.AppendLine("<tr>");

        foreach (PivotViewExportColumn Column in Data.Columns)
            SB.AppendLine($"<th>{EncodeMultiline(Column.Title)}</th>");

        SB.AppendLine("</tr>");
        SB.AppendLine("</thead>");
    }
    static private void AppendBody(StringBuilder SB, PivotViewExportData Data, PivotViewExportOptions Options)
    {
        SB.AppendLine("<tbody>");

        foreach (PivotViewExportRow Row in Data.Rows)
        {
            string RowCssClass = GetRowCssClass(Row);
            SB.AppendLine($"<tr class=\"{RowCssClass}\">");

            for (int i = 0; i < Data.Columns.Count; i++)
            {
                PivotViewExportColumn Column = Data.Columns[i];
                object RawValue = i < Row.Values.Count ? Row.Values[i] : null;
                object Value = Options.UseFormattedValues
                    ? PivotViewExportFormatHelper.FormatExportValue(RawValue, Column, Options)
                    : RawValue;
                string CssClass = GetCellCssClass(Column);
                string Text = EncodeMultiline(Value);

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
    static public void Export(PivotViewExportData Data, PivotViewExportOptions Options)
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
        SB.AppendLine($"<title>{Encode(Options.HtmlTitle)}</title>");
        SB.AppendLine(GetStyle());
        SB.AppendLine("</head>");
        SB.AppendLine("<body>");
        SB.AppendLine("<table class=\"pivot\">");

        if (Options.IncludeHeaders)
            AppendHeader(SB, Data);

        AppendBody(SB, Data, Options);

        SB.AppendLine("</table>");
        SB.AppendLine("</body>");
        SB.AppendLine("</html>");

        File.WriteAllText(Options.ExportFilePath, SB.ToString(), Options.GetEncoding() ?? new UTF8Encoding(true));
    }
}

static internal class PivotViewXlsxExporter
{
    // ● private methods
    static private bool IsDateColumn(PivotViewExportColumn Column)
    {
        if (Column?.DataType == null)
            return false;

        Type DataType = Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType;
        return DataType == typeof(DateTime) || DataType == typeof(DateTimeOffset);
    }
    static private bool IsNumericColumn(PivotViewExportColumn Column)
    {
        if (Column?.DataType == null)
            return false;

        Type DataType = Nullable.GetUnderlyingType(Column.DataType) ?? Column.DataType;
        return DataType.IsNumeric();
    }
    static private void ApplyBaseStyle(IXLCell Cell)
    {
        Cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }
    static private void ApplyHeaderStyle(IXLCell Cell)
    {
        Cell.Style.Font.Bold = true;
        Cell.Style.Alignment.WrapText = true;
        Cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFEFEF");
    }
    static private void ApplyDateStyle(IXLCell Cell, PivotViewExportOptions Options, PivotViewExportColumn Column)
    {
        string Format = !string.IsNullOrWhiteSpace(Column?.Format)
            ? Column.Format
            : !string.IsNullOrWhiteSpace(Options?.DateTimeFormat) ? Options.DateTimeFormat : "yyyy-MM-dd HH:mm";

        Cell.Style.DateFormat.Format = Format;
        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }
    static private void ApplyNumericStyle(IXLCell Cell, PivotViewExportOptions Options, PivotViewExportColumn Column)
    {
        string Format = !string.IsNullOrWhiteSpace(Column?.Format)
            ? Column.Format
            : Options?.NumberFormat;

        if (!string.IsNullOrWhiteSpace(Format))
            Cell.Style.NumberFormat.Format = Format;

        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }
    static private void ApplyBoolStyle(IXLCell Cell)
    {
        Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }
    static private void ApplyRowStyle(IXLWorksheet WS, PivotViewExportRow Row, int RowIndex)
    {
        if (Row.IsSubtotal)
        {
            WS.Row(RowIndex).Style.Font.Bold = true;
            WS.Row(RowIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF7F7F7");
        }
        else if (Row.IsGrandTotal)
        {
            WS.Row(RowIndex).Style.Font.Bold = true;
            WS.Row(RowIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6E6");
        }
    }
    static private bool TryGetDateValue(object Value, PivotViewExportColumn Column, out DateTime Result)
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
    static private bool TryGetNumericValue(object Value, PivotViewExportColumn Column, out double Result)
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
    static private void SetCellValue(IXLCell Cell, object Value, PivotViewExportColumn Column, PivotViewExportOptions Options)
    {
        ApplyBaseStyle(Cell);

        if (Value == null || Value == DBNull.Value)
        {
            Cell.Clear();
            return;
        }

        if (!Options.UseFormattedValues)
        {
            if (Value is bool B)
            {
                Cell.Value = B;
                ApplyBoolStyle(Cell);
                return;
            }

            if (TryGetDateValue(Value, Column, out DateTime DT))
            {
                Cell.Value = DT;
                ApplyDateStyle(Cell, Options, Column);
                return;
            }

            if (TryGetNumericValue(Value, Column, out double D))
            {
                Cell.Value = D;
                ApplyNumericStyle(Cell, Options, Column);
                return;
            }
        }

        string Text = PivotViewExportFormatHelper.FormatExportValue(Value, Column, Options);
        Cell.Value = Text;

        if (IsNumericColumn(Column) || IsDateColumn(Column))
            Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }
    static private void WriteHeader(IXLWorksheet WS, PivotViewExportData Data, int RowIndex)
    {
        for (int c = 0; c < Data.Columns.Count; c++)
        {
            IXLCell Cell = WS.Cell(RowIndex, c + 1);
            Cell.Value = Data.Columns[c].Title;
            ApplyHeaderStyle(Cell);
        }
    }
    static private void WriteRow(IXLWorksheet WS, PivotViewExportData Data, PivotViewExportRow Row, int RowIndex, PivotViewExportOptions Options)
    {
        for (int c = 0; c < Data.Columns.Count; c++)
        {
            IXLCell Cell = WS.Cell(RowIndex, c + 1);
            object Value = c < Row.Values.Count ? Row.Values[c] : null;
            SetCellValue(Cell, Value, Data.Columns[c], Options);
        }

        ApplyRowStyle(WS, Row, RowIndex);
    }

    // ● static public methods
    static public void Export(PivotViewExportData Data, PivotViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));
        if (Options == null)
            throw new ArgumentNullException(nameof(Options));

        using XLWorkbook WB = new();
        string SheetName = !string.IsNullOrWhiteSpace(Options.ExcelSheetName) ? Options.ExcelSheetName : "Pivot";
        IXLWorksheet WS = WB.Worksheets.Add(SheetName);

        int RowIndex = 1;

        if (Options.IncludeHeaders)
        {
            WriteHeader(WS, Data, RowIndex);
            WS.SheetView.FreezeRows(1);
            RowIndex++;
        }

        foreach (PivotViewExportRow Row in Data.Rows)
        {
            WriteRow(WS, Data, Row, RowIndex, Options);
            RowIndex++;
        }

        WS.Columns().AdjustToContents();
        WB.SaveAs(Options.ExportFilePath);
    }
}

static public class PivotViewExporter
{
    // ● private methods
    static private PivotViewExportOptions NormalizeOptions(PivotViewExportOptions Options)
    {
        Options ??= new PivotViewExportOptions();
        Options.Check();
        return Options;
    }
    static private PivotData GetData(PivotView PivotView)
    {
        if (PivotView == null)
            throw new ArgumentNullException(nameof(PivotView));
        if (PivotView.PivotData == null)
            throw new ApplicationException("PivotView has no pivot data.");

        return PivotView.PivotData;
    }
    static private PivotViewExportData CreateExportData(PivotData Data, PivotViewExportOptions Options)
    {
        return PivotViewExportBuilder.Create(Data, Options);
    }

    // ● static public methods
    static public void Export(PivotView PivotView, PivotViewExportOptions Options)
    {
        Export(GetData(PivotView), Options);
    }
    static public void Export(PivotData Data, PivotViewExportOptions Options)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        Options = NormalizeOptions(Options);
        PivotViewExportData ExportData = CreateExportData(Data, Options);

        switch (Options.Format)
        {
            case PivotViewExportFormat.Csv:
                PivotViewCsvExporter.Export(ExportData, Options);
                break;
            case PivotViewExportFormat.Xlsx:
                PivotViewXlsxExporter.Export(ExportData, Options);
                break;
            case PivotViewExportFormat.Html:
                PivotViewHtmlExporter.Export(ExportData, Options);
                break;
            case PivotViewExportFormat.Json:
                PivotViewJsonExporter.Export(ExportData, Options);
                break;
            default:
                throw new NotSupportedException($"Unsupported export format: {Options.Format}");
        }
    }
}

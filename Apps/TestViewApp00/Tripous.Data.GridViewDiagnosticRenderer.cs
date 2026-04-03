using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tripous.Data;

static public class GridViewDiagnosticRenderer
{
    // ● private methods
    static private void AddNodeLines(GridViewNode Node, List<string> Lines)
    {
        if (Node == null)
            return;

        string Indent = new string(' ', Math.Max(0, Node.Level) * 4);
        string Text = Node.NodeType switch
        {
            GridNodeType.Group => $"{Indent}[Group] {Node.FieldName} = {FormatValue(Node.Key)} {(Node.IsExpanded ? "(Expanded)" : "(Collapsed)")}",
            GridNodeType.Row => $"{Indent}[Row] {FormatRow(Node.DataItem)}",
            GridNodeType.Footer => $"{Indent}[Footer] {FormatSummaries(Node.Summaries)}",
            _ => $"{Indent}[None]",
        };

        Lines.Add(Text);
    }
    static private string FormatRow(object DataItem)
    {
        return DataItem != null ? DataItem.ToString() : "null";
    }
    static private string FormatSummaries(IEnumerable<GridViewSummary> Summaries)
    {
        if (Summaries == null)
            return string.Empty;

        return string.Join(", ", Summaries.Select(x => $"{x.FieldName}:{x.AggregateType}={FormatValue(x.Value)}"));
    }
    static private string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return "null";

        if (Value is DateTime DateTimeValue)
            return DateTimeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        if (Value is IFormattable Formattable)
            return Formattable.ToString(null, CultureInfo.InvariantCulture);

        return Value.ToString();
    }

    // ● static public methods
    static public List<string> RenderLines(GridViewData Data)
    {
        List<string> Result = new();

        if (Data == null)
            return Result;

        Result.Add("[Root]");

        foreach (GridViewNode Node in Data.VisibleNodes)
            AddNodeLines(Node, Result);

        return Result;
    }
    static public string RenderText(GridViewData Data)
    {
        StringBuilder SB = new();

        foreach (string Line in RenderLines(Data))
            SB.AppendLine(Line);

        return SB.ToString();
    }
}

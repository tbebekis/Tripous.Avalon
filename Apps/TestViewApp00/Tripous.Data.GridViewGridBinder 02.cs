using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Tripous.Data;

static public class GridViewGridBinder
{
    // ● private methods
    static private string FormatValue(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        return Convert.ToString(Value) ?? string.Empty;
    }
    static private bool HasValue(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return false;

        return Row.Values[ColumnIndex] != null;
    }
    static private bool IsLabelCell(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null)
            return false;

        return Row.LabelColumnIndex == ColumnIndex;
    }
    static private HorizontalAlignment GetHorizontalAlignment(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && IsLabelCell(Row, ColumnIndex))
            return HorizontalAlignment.Left;

        if (ColumnDef != null && (ColumnDef.IsNumeric || ColumnDef.IsDate))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    static private FontWeight GetFontWeight(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null)
            return FontWeight.Normal;

        if ((Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && IsLabelCell(Row, ColumnIndex))
            return FontWeight.Bold;

        if ((Row.IsFooter || Row.IsGrandTotal) && HasValue(Row, ColumnIndex))
            return FontWeight.Bold;

        return FontWeight.Normal;
    }
    static private IBrush GetRowBackground(GridViewRenderRow Row)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal))
            return Brushes.Gainsboro;

        return Brushes.Transparent;
    }
    static private Control CreateGroupCell(DataGrid Grid, GridViewData Data, GridViewDef Def, GridViewRenderRow Row, int ColumnIndex)
    {
        string Glyph = Row.Node != null && Row.Node.IsExpanded ? "▼ " : "▶ ";

        TextBlock Text = new()
        {
            Text = Glyph + FormatValue(Row, ColumnIndex),
            Margin = new Thickness(6, 2, 6, 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Text,
            Cursor = new Cursor(StandardCursorType.Hand),
        };

        Result.PointerPressed += (s, e) =>
        {
            if (Data == null || Row?.Node == null)
                return;

            if (Row.Node.Toggle())
            {
                Data.RebuildVisibleNodes();
                Apply(Grid, Data, Def);
            }
        };

        return Result;
    }
    static private Control CreateCell(DataGrid Grid, GridViewData Data, GridViewDef Def, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && Row.IsGroup && IsLabelCell(Row, ColumnIndex))
            return CreateGroupCell(Grid, Data, Def, Row, ColumnIndex);

        TextBlock Text = new()
        {
            Text = FormatValue(Row, ColumnIndex),
            Margin = new Thickness(6, 2, 6, 2),
            HorizontalAlignment = GetHorizontalAlignment(Row, ColumnDef, ColumnIndex),
            FontWeight = GetFontWeight(Row, ColumnIndex),
            VerticalAlignment = VerticalAlignment.Center,
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Text,
        };

        return Result;
    }

    // ● static public methods
    static public void Apply(DataGrid Grid, GridViewData Data, GridViewDef Def)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridViewRenderData RenderData = GridViewRenderer.Render(Data, Def);
        Apply(Grid, RenderData, Data, Def);
    }
    static public void Apply(DataGrid Grid, GridViewRenderData RenderData)
    {
        Apply(Grid, RenderData, null, null);
    }
    static public void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data, GridViewDef Def)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (RenderData == null)
            throw new ArgumentNullException(nameof(RenderData));

        Grid.AutoGenerateColumns = false;
        Grid.Columns.Clear();

        for (int i = 0; i < RenderData.Columns.Count; i++)
        {
            int ColumnIndex = i;
            GridViewColumnDef ColumnDef = RenderData.Columns[i];

            DataGridTemplateColumn Column = new()
            {
                Header = ColumnDef.FieldName,
                Width = DataGridLength.Auto,
                CellTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateCell(Grid, Data, Def, Row, ColumnDef, ColumnIndex);
                }),
                SortMemberPath = ColumnDef.FieldName,
            };

            Grid.Columns.Add(Column);
        }

        Grid.ItemsSource = RenderData.Rows;
    }
}

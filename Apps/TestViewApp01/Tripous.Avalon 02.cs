using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Tripous.Data;

namespace Tripous.Avalon;

public class GridViewRenderRow
{
    // ● constructors
    public GridViewRenderRow()
    {
    }

    // ● public methods
    public override string ToString() => RowType.ToString();

    // ● properties
    public GridDataRow DataRow { get; set; }
    public int LabelColumnIndex { get; set; } = -1;
    public GridViewNode Node => DataRow != null ? DataRow.Node : null;
    public int Level => DataRow != null ? DataRow.Level : 0;
    public GridDataRowType RowType => DataRow != null ? DataRow.RowType : GridDataRowType.Data;
    public object[] Values { get; set; }
    public bool IsData => DataRow != null && DataRow.IsData;
    public bool IsGroup => DataRow != null && DataRow.IsGroup;
    public bool IsFooter => DataRow != null && DataRow.IsFooter;
    public bool IsGrandTotal => DataRow != null && DataRow.IsGrandTotal;
}

public class GridViewRenderData
{
    // ● constructors
    public GridViewRenderData()
    {
    }

    // ● properties
    public List<GridViewColumnDef> Columns { get; set; } = new();
    public List<GridViewRenderRow> Rows { get; set; } = new();
}

public class GridViewGridLink: IDisposable
{
    // ● private fields
    private GridViewController fController;
    private bool fDisposed;
    private DataGrid fGrid;

    // ● private methods
    private void Controller_DataChanged(object Sender, GridViewDataChangedEventArgs e)
    {
        Refresh();
    }
    private void DetachController()
    {
        if (fController != null)
            fController.DataChanged -= Controller_DataChanged;
    }
    private void OnBoundChanged()
    {
        Refresh();
    }

    // ● constructors
    public GridViewGridLink()
    {
    }
    public GridViewGridLink(DataGrid Grid, GridViewController Controller)
    {
        Bind(Grid, Controller);
    }

    // ● public methods
    public void Bind(DataGrid Grid, GridViewController Controller)
    {
        if (ReferenceEquals(fGrid, Grid) && ReferenceEquals(fController, Controller))
            return;

        DetachController();

        fGrid = Grid;
        fController = Controller;

        if (fController != null)
            fController.DataChanged += Controller_DataChanged;

        OnBoundChanged();
    }
    public void Dispose()
    {
        if (!fDisposed)
        {
            DetachController();
            fGrid = null;
            fController = null;
            fDisposed = true;
        }
    }
    public void Refresh()
    {
        if (fGrid == null)
            return;

        GridViewGridBinder.Apply(fGrid, fController);
    }

    // ● properties
    public GridViewController Controller => fController;
    public bool IsDisposed => fDisposed;
    public DataGrid Grid => fGrid;
}

static public class GridViewRenderer
{
    // ● private methods
    static private List<GridViewColumnDef> GetDisplayColumns(GridViewDef Def)
    {
        List<GridViewColumnDef> GroupColumns = Def.GetGroupColumns().OrderBy(x => x.GroupIndex).ToList();
        List<GridViewColumnDef> VisibleColumns = Def.GetVisibleColumns().ToList();
        List<GridViewColumnDef> NonGroupColumns = VisibleColumns.Where(x => x.GroupIndex < 0).OrderBy(x => x.VisibleIndex).ToList();
        return GroupColumns.Concat(NonGroupColumns).ToList();
    }
    static private string GetFooterLabel(GridViewNode Node)
    {
        if (Node == null)
            return string.Empty;

        if (Node.OwnerGroup == null)
            return "Grand Total";

        return $"Total ({Node.OwnerGroup.FieldName} = {Node.OwnerGroup.Key})";
    }
    static private int GetGroupLabelColumnIndex(GridViewNode Node, int ColumnCount)
    {
        if (ColumnCount <= 0)
            return -1;

        return Math.Min(Math.Max(Node != null ? Node.Level : 0, 0), ColumnCount - 1);
    }
    static private object GetRowValue(GridDataRow Row, string FieldName)
    {
        return Row != null ? Row.GetValue(FieldName) : null;
    }

    // ● static public methods
    static public GridViewRenderData Render(GridViewData Data)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Data.ViewDef == null)
            throw new ArgumentNullException(nameof(Data.ViewDef));

        GridViewRenderData Result = new();
        List<GridViewColumnDef> Columns = GetDisplayColumns(Data.ViewDef);

        Result.Columns.AddRange(Columns);

        foreach (GridDataRow DataRow in Data.Rows)
        {
            GridViewRenderRow Row = new()
            {
                DataRow = DataRow,
                Values = new object[Columns.Count],
            };

            if (DataRow.IsGroup)
            {
                int ColumnIndex = GetGroupLabelColumnIndex(DataRow.Node, Columns.Count);
                Row.LabelColumnIndex = ColumnIndex;

                if (ColumnIndex >= 0)
                    Row.Values[ColumnIndex] = $"{DataRow.Node.FieldName} = {DataRow.Node.Key}";
            }
            else if (DataRow.IsFooter || DataRow.IsGrandTotal)
            {
                if (Columns.Count > 0)
                {
                    Row.LabelColumnIndex = 0;
                    Row.Values[0] = GetFooterLabel(DataRow.Node);
                }

                if (DataRow.Node != null)
                {
                    foreach (GridViewSummary Summary in DataRow.Node.Summaries)
                    {
                        int ColumnIndex = Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));
                        if (ColumnIndex >= 0)
                            Row.Values[ColumnIndex] = Summary.Value;
                    }
                }
            }
            else if (DataRow.IsData)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    GridViewColumnDef ColumnDef = Columns[i];

                    if (!Data.ViewDef.ShowGroupColumnsAsDataColumns && ColumnDef.GroupIndex >= 0)
                        Row.Values[i] = null;
                    else
                        Row.Values[i] = GetRowValue(DataRow, ColumnDef.FieldName);
                }
            }

            Result.Rows.Add(Row);
        }

        return Result;
    }
}

static public class GridViewGridBinder
{
    // ● private fields
    static private ConditionalWeakTable<DataGrid, GridViewGridLink> fLinks = new();

    // ● private methods
    static private void AddOrReplaceLink(DataGrid Grid, GridViewGridLink Link)
    {
        if (Grid == null)
            return;

        if (fLinks.TryGetValue(Grid, out GridViewGridLink OldLink))
        {
            OldLink.Dispose();
            fLinks.Remove(Grid);
        }

        if (Link != null)
            fLinks.Add(Grid, Link);
    }
    static private bool CanEditCell(GridViewController Controller, GridViewRenderRow Row, GridViewColumnDef ColumnDef)
    {
        if (Controller == null || Row == null || Row.DataRow == null || ColumnDef == null)
            return false;

        return Controller.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
    static private void CommitCellEdit(DataGrid Grid, GridViewController Controller, GridViewData Data, GridViewRenderRow Row, GridViewColumnDef ColumnDef, string Text)
    {
        if (Row == null || Row.DataRow == null || ColumnDef == null)
            return;

        if (Controller != null)
        {
            Controller.SetValue(Row.DataRow, ColumnDef.FieldName, Text);
            return;
        }

        if (Row.DataRow.SetValue(ColumnDef.FieldName, Text) && Data != null)
            Apply(Grid, Data);
    }
    static private Control CreateCell(DataGrid Grid, GridViewData Data, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && Row.IsGroup && IsLabelCell(Row, ColumnIndex))
            return CreateGroupCell(Grid, Data, Row, ColumnIndex);

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
    static private Control CreateEditingCell(DataGrid Grid, GridViewController Controller, GridViewData Data, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (!CanEditCell(Controller, Row, ColumnDef))
            return CreateCell(Grid, Data, Row, ColumnDef, ColumnIndex);

        TextBox Editor = new()
        {
            Text = FormatValue(Row, ColumnIndex),
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = GetHorizontalAlignment(Row, ColumnDef, ColumnIndex),
            IsReadOnly = !CanEditCell(Controller, Row, ColumnDef),
        };

        bool Committed = false;

        void Commit()
        {
            if (Committed)
                return;

            Committed = true;
            CommitCellEdit(Grid, Controller, Data, Row, ColumnDef, Editor.Text);
        }

        Editor.LostFocus += (s, e) => Commit();
        Editor.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter)
                Commit();
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Editor,
        };

        return Result;
    }
    static private Control CreateGroupCell(DataGrid Grid, GridViewData Data, GridViewRenderRow Row, int ColumnIndex)
    {
        string Glyph = Row != null && Row.Node != null && Row.Node.IsExpanded ? "▼ " : "▶ ";

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
            if (Data == null || Row == null || Row.Node == null)
                return;

            if (Row.Node.Toggle())
                Apply(Grid, Data);
        };

        return Result;
    }
    static private string FormatValue(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        return Convert.ToString(Value) ?? string.Empty;
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
    static private HorizontalAlignment GetHorizontalAlignment(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && IsLabelCell(Row, ColumnIndex))
            return HorizontalAlignment.Left;

        if (ColumnDef != null && (ColumnDef.IsNumeric || ColumnDef.IsDate))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    static private IBrush GetRowBackground(GridViewRenderRow Row)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal))
            return Brushes.Gainsboro;

        return Brushes.Transparent;
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

    // ● static public methods
    static public void Apply(DataGrid Grid, GridViewController Controller)
    {
        if (Controller == null)
            throw new ArgumentNullException(nameof(Controller));

        Apply(Grid, Controller.Data, Controller);
    }
    static public void Apply(DataGrid Grid, GridViewData Data)
    {
        Apply(Grid, Data, null);
    }
    static public void Apply(DataGrid Grid, GridViewData Data, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (Data == null)
        {
            Grid.AutoGenerateColumns = false;
            Grid.Columns.Clear();
            Grid.ItemsSource = null;
            return;
        }

        GridViewRenderData RenderData = GridViewRenderer.Render(Data);
        Apply(Grid, RenderData, Data, Controller);
    }
    static public void Apply(DataGrid Grid, GridViewRenderData RenderData)
    {
        Apply(Grid, RenderData, null, null);
    }
    static public void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data)
    {
        Apply(Grid, RenderData, Data, null);
    }
    static public void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data, GridViewController Controller)
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
                Header = !string.IsNullOrWhiteSpace(ColumnDef.Title) ? ColumnDef.Title : ColumnDef.FieldName,
                Width = DataGridLength.Auto,
                IsReadOnly = ColumnDef.IsReadOnly,
                CellTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateCell(Grid, Data, Row, ColumnDef, ColumnIndex);
                }),
                CellEditingTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateEditingCell(Grid, Controller, Data, Row, ColumnDef, ColumnIndex);
                }),
                SortMemberPath = ColumnDef.FieldName,
            };

            Grid.Columns.Add(Column);
        }

        Grid.ItemsSource = RenderData.Rows;
    }
    static public GridViewGridLink Bind(DataGrid Grid, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridViewGridLink Result = new(Grid, Controller);
        AddOrReplaceLink(Grid, Result);
        return Result;
    }
    static public void Refresh(DataGrid Grid)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (fLinks.TryGetValue(Grid, out GridViewGridLink Link))
            Link.Refresh();
    }
    static public void Unbind(DataGrid Grid)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (fLinks.TryGetValue(Grid, out GridViewGridLink Link))
        {
            Link.Dispose();
            fLinks.Remove(Grid);
        }
    }
}

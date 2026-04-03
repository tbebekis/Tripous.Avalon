using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Tripous.Data;

public enum GridViewRowType
{
    Data,
    Group,
    Footer,
    GrandTotal,
}

public class GridViewRenderRow
{
    public GridViewNode Node { get; set; }
    public int Level { get; set; }
    public GridViewRowType RowType { get; set; }
    public object[] Values { get; set; }
    public int LabelColumnIndex { get; set; } = -1;

    public bool IsData => RowType == GridViewRowType.Data;
    public bool IsGroup => RowType == GridViewRowType.Group;
    public bool IsFooter => RowType == GridViewRowType.Footer;
    public bool IsGrandTotal => RowType == GridViewRowType.GrandTotal;
}

public class GridViewRenderData
{
    public List<GridViewColumnDef> Columns { get; set; } = new();
    public List<GridViewRenderRow> Rows { get; set; } = new();
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
    static private GridViewRowType GetRowType(GridViewNode Node)
    {
        if (Node == null)
            return GridViewRowType.Data;

        if (Node.IsGroup)
            return GridViewRowType.Group;

        if (Node.IsFooter)
            return Node.OwnerGroup == null ? GridViewRowType.GrandTotal : GridViewRowType.Footer;

        return GridViewRowType.Data;
    }
    static private object GetValue(object Instance, string FieldName)
    {
        if (Instance == null || string.IsNullOrWhiteSpace(FieldName))
            return null;

        if (Instance is DataRowView RowView)
            return RowView[FieldName];

        if (Instance is DataRow Row)
            return Row[FieldName];

        PropertyInfo Prop = Instance.GetType().GetProperty(FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (Prop != null)
            return Prop.GetValue(Instance);

        return null;
    }

    // ● static public methods
    static public GridViewRenderData Render(GridViewData Data, GridViewDef Def)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Def == null)
            throw new ArgumentNullException(nameof(Def));

        GridViewRenderData Result = new();
        List<GridViewColumnDef> Columns = GetDisplayColumns(Def);

        Result.Columns.AddRange(Columns);

        foreach (GridViewNode Node in Data.VisibleNodes)
        {
            GridViewRenderRow Row = new()
            {
                Node = Node,
                Level = Node.Level,
                RowType = GetRowType(Node),
                Values = new object[Columns.Count],
            };

            if (Node.IsGroup)
            {
                int ColumnIndex = Math.Min(Math.Max(Node.Level, 0), Math.Max(Columns.Count - 1, 0));
                Row.LabelColumnIndex = ColumnIndex;
                if (Columns.Count > 0)
                    Row.Values[ColumnIndex] = $"{Node.FieldName} = {Node.Key}";
            }
            else if (Node.IsFooter)
            {
                Row.LabelColumnIndex = 0;

                if (Columns.Count > 0)
                {
                    if (Row.IsGrandTotal)
                        Row.Values[0] = "Grand Total";
                    else
                        Row.Values[0] = $"Total ({Node.OwnerGroup.FieldName} = {Node.OwnerGroup.Key})";
                }

                foreach (GridViewSummary Summary in Node.Summaries)
                {
                    int ColumnIndex = Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));
                    if (ColumnIndex >= 0)
                        Row.Values[ColumnIndex] = Summary.Value;
                }
            }
            else if (Node.IsRow)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    GridViewColumnDef ColumnDef = Columns[i];

                    if (!Def.ShowGroupColumnsAsDataColumns && ColumnDef.GroupIndex >= 0)
                        Row.Values[i] = null;
                    else
                        Row.Values[i] = GetValue(Node.DataItem, ColumnDef.FieldName);
                }
            }

            Result.Rows.Add(Row);
        }

        return Result;
    }
}

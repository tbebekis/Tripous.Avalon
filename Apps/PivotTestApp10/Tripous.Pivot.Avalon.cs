using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Tripous.Data;

namespace Tripous.Avalon;


/*
public class PivotBinder: ObservableObject
{


    private DataGrid fGrid;
    private DataView fDataView;
    private PivotDef fPivotDef;
    private PivotData fPivotDataModel;

    // ● constructors
    public PivotBinder(DataGrid Grid, DataTable Table)
        : this(Grid, Table?.DefaultView)
    {
    }
    public PivotBinder(DataGrid Grid, DataView DataView)
    {
        this.Grid = Grid;
        this.DataView = DataView;
    }

    // ● static public methods
    /// <summary>
    /// Defines the attached <see cref="PivotBinder"/> property to the <see cref="DataGrid"/> class.
    /// </summary>
    static public readonly AttachedProperty<PivotBinder> GridPivotBinderProperty =
        AvaloniaProperty.RegisterAttached<PivotBinder, DataGrid, PivotBinder>("GridPivotBinder");

    /// <summary>
    /// Returns the <see cref="PivotBinder"/> of a <see cref="DataGrid"/>.
    /// </summary>
    static public PivotBinder GetGridPivotBinder(DataGrid Element) => Element.GetValue(GridPivotBinderProperty);

    // ● public methods
    public virtual void Refresh()
    {
        if (Grid == null || DataView == null || PivotDef == null)
            return;

        PivotDataModel = PivotEngine.Execute(DataView, PivotDef);
    }
    public virtual void ResetPivotDef()
    {
        if (DataView == null)
            return;

        PivotDef = DataView.CreateDefaultPivotDef();
    }

    // ● properties
    /// <summary>
    /// The grid where this instance presents the pivot.
    /// </summary>
    public DataGrid Grid
    {
        get => fGrid;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(Grid));

            if (fGrid == value)
                return;

            fPivotDataModel = null;
            fGrid = value;
            fGrid.Tag = this;
            fGrid.SetValue(GridPivotBinderProperty, this);

            // CHECK: ViewMenu = new PivotViewMenu(fGrid);
        }
    }
    /// <summary>
    /// The source table.
    /// </summary>
    public DataTable DataTable
    {
        get => DataView?.Table;
        set
        {
            if (value != null && value != DataTable)
                DataView = value.DefaultView;
        }
    }
    /// <summary>
    /// The source view.
    /// </summary>
    public DataView DataView
    {
        get => fDataView;
        set
        {
            if (value == null || fDataView == value)
                return;

            if (Grid == null)
                throw new ApplicationException("Cannot set DataView without a Grid.");

            fDataView = value;
            fPivotDef = null;
            fPivotDataModel = null;

            ResetPivotDef();
        }
    }
    /// <summary>
    /// The pivot definition.
    /// </summary>
    public PivotDef PivotDef
    {
        get => fPivotDef;
        set
        {
            if (value == null || fPivotDef == value)
                return;

            if (Grid == null)
                throw new ApplicationException("Cannot set PivotDef without a Grid.");
            if (DataView == null)
                throw new ApplicationException("Cannot set PivotDef without a DataView.");

            fPivotDef = value;
            fPivotDataModel = null;

            Refresh();
        }
    }
    /// <summary>
    /// The rendered pivot data model.
    /// </summary>
    public PivotData PivotDataModel
    {
        get => fPivotDataModel;
        set
        {
            if (fPivotDataModel == value)
                return;

            fPivotDataModel = value;

            if (Grid != null && fPivotDataModel != null)
                PivotGridRenderer.Show(Grid, fPivotDataModel);
        }
    }
    /// <summary>
    /// The supported source columns of the current source view.
    /// </summary>
    public List<DataColumn> SupportedColumns => DataView?.GetPivotSupportedColumns();
    /// <summary>
    /// Handles the UI menus, etc.
    /// </summary>
    public PivotViewMenu ViewMenu { get; protected set; }
}
*/ 

/// <summary>
/// Renders PivotData to an Avalonia DataGrid / ProDataGrid.
/// </summary>
/// <summary>
/// Renders PivotData to an Avalonia DataGrid / ProDataGrid.
/// </summary>
public static class PivotGridRenderer
{
    // ● private methods
    private static Dictionary<string, int> CreateIndexes(List<PivotDataColumn> Columns)
    {
        Dictionary<string, int> Result = new(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < Columns.Count; i++)
            Result[Columns[i].Key] = i;

        return Result;
    }
    private static List<PivotGridRow> CreateGridRows(PivotData PivotData, PivotDef PivotDef, Dictionary<string, int> Indexes)
    {
        List<PivotDataRow> SourceRows = PivotDef != null && !PivotDef.RepeatRowHeaders
            ? CreateDisplayRows(PivotData)
            : PivotData.Rows;

        return SourceRows
            .Select(x => new PivotGridRow(x, Indexes))
            .ToList();
    }
    private static List<PivotDataRow> CreateDisplayRows(PivotData PivotData)
    {
        List<PivotDataRow> Result = new();
        List<int> RowHeaderIndexes = GetRowHeaderIndexes(PivotData.Columns);
        object[] PreviousNormalHeaderValues = null;

        foreach (PivotDataRow SourceRow in PivotData.Rows)
        {
            PivotDataRow Row = CloneRow(SourceRow);

            if (SourceRow.RowType == PivotDataRowType.Normal)
            {
                if (PreviousNormalHeaderValues != null)
                {
                    for (int i = 0; i < RowHeaderIndexes.Count; i++)
                    {
                        int ValueIndex = RowHeaderIndexes[i];

                        if (Equals(Row.Values[ValueIndex], PreviousNormalHeaderValues[i]))
                            Row.Values[ValueIndex] = null;
                        else
                            break;
                    }
                }

                PreviousNormalHeaderValues = RowHeaderIndexes
                    .Select(x => SourceRow.Values[x])
                    .ToArray();
            }
            else
            {
                PreviousNormalHeaderValues = null;
            }

            Result.Add(Row);
        }

        return Result;
    }
    private static List<int> GetRowHeaderIndexes(List<PivotDataColumn> Columns)
    {
        List<int> Result = new();

        for (int i = 0; i < Columns.Count; i++)
        {
            if (Columns[i].Kind == PivotDataColumnKind.RowHeader)
                Result.Add(i);
        }

        return Result;
    }
    private static PivotDataRow CloneRow(PivotDataRow SourceRow)
    {
        return new PivotDataRow
        {
            Values = SourceRow.Values != null ? (object[])SourceRow.Values.Clone() : null,
            RowType = SourceRow.RowType,
            Level = SourceRow.Level,
            Tag = SourceRow.Tag
        };
    }
    private static DataGridColumn CreateColumn(PivotDataColumn Col)
    {
        DataGridTemplateColumn Result = new()
        {
            Header = CreateHeaderControl(Col.Caption),
            IsReadOnly = true,
            CellTemplate = CreateCellTemplate(Col)
        };

        Result.Tag = Col.SourceField;
        return Result;
    }
    private static Control CreateHeaderControl(string Caption)
    {
        return new Border
        {
            Padding = new Thickness(6, 4, 6, 4),
            Child = new TextBlock
            {
                Text = Caption,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }
    private static IDataTemplate CreateCellTemplate(PivotDataColumn Col)
    {
        bool IsValueColumn = Col.Kind == PivotDataColumnKind.Value;

        return new FuncDataTemplate<object>((Item, _) =>
        {
            PivotGridRow Row = Item as PivotGridRow;

            Border Border = new()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Padding = IsValueColumn
                    ? new Thickness(4, 0, 6, 0)
                    : new Thickness(6, 0, 6, 0)
            };

            TextBlock TextBlock = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = IsValueColumn ? TextAlignment.Right : TextAlignment.Left
            };

            ApplyRowStyle(Row, Border, TextBlock);

            Binding Binding = new($"[{Col.Key}]");
            if (!string.IsNullOrWhiteSpace(Col.Format))
                Binding.StringFormat = Col.Format;

            TextBlock.Bind(TextBlock.TextProperty, Binding);

            Border.Child = TextBlock;
            return Border;
        });
    }
    private static void ApplyRowStyle(PivotGridRow Row, Border Border, TextBlock TextBlock)
    {
        if (Row == null)
            return;

        switch (Row.RowType)
        {
            case PivotDataRowType.Subtotal:
                TextBlock.FontWeight = FontWeight.Bold;
                Border.Background = new SolidColorBrush(Color.Parse("#FFF7F7F7"));
                break;
            case PivotDataRowType.GrandTotal:
                TextBlock.FontWeight = FontWeight.Bold;
                Border.Background = new SolidColorBrush(Color.Parse("#FFE6E6E6"));
                break;
        }
    }

    // ● public methods
    /// <summary>
    /// Shows pivot data in a grid.
    /// </summary>
    public static void Show(DataGrid Grid, PivotData PivotData, PivotDef PivotDef)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));
        if (PivotData == null)
            throw new ArgumentNullException(nameof(PivotData));

        Grid.Columns.Clear();
        Grid.AutoGenerateColumns = false;
        Grid.IsReadOnly = true;

        Dictionary<string, int> Indexes = CreateIndexes(PivotData.Columns);
        List<PivotGridRow> Rows = CreateGridRows(PivotData, PivotDef, Indexes);

        foreach (PivotDataColumn Col in PivotData.Columns)
            Grid.Columns.Add(CreateColumn(Col));

        Grid.ItemsSource = Rows;
    }
    /// <summary>
    /// Shows pivot data in a grid.
    /// </summary>
    public static void Show(DataGrid Grid, PivotData PivotData)
    {
        Show(Grid, PivotData, null);
    }
}

/// <summary>
/// A bindable row adapter used by the pivot grid renderer.
/// </summary>
public class PivotGridRow
{
    // ● private fields
    private readonly PivotDataRow fRow;
    private readonly Dictionary<string, int> fIndexes;

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public PivotGridRow(PivotDataRow Row, Dictionary<string, int> Indexes)
    {
        fRow = Row ?? throw new ArgumentNullException(nameof(Row));
        fIndexes = Indexes ?? throw new ArgumentNullException(nameof(Indexes));
    }

    // ● properties
    /// <summary>
    /// Gets a cell value by pivot column key.
    /// </summary>
    public object this[string Key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Key))
                return null;

            return fIndexes.TryGetValue(Key, out int Index) ? fRow.Values[Index] : null;
        }
    }
    /// <summary>
    /// The source pivot row.
    /// </summary>
    public PivotDataRow Row => fRow;
    /// <summary>
    /// The row type.
    /// </summary>
    public PivotDataRowType RowType => fRow.RowType;
    /// <summary>
    /// The subtotal level or -1.
    /// </summary>
    public int Level => fRow.Level;
}
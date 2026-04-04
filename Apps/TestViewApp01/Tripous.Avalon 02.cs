using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Tripous.Data;

namespace Tripous.Avalon;

internal class GridViewRenderRow
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

internal class GridViewRenderData
{
    // ● constructors
    public GridViewRenderData()
    {
    }

    // ● properties
    public List<GridViewColumnDef> Columns { get; set; } = new();
    public List<GridViewRenderRow> Rows { get; set; } = new();
}
 
internal class GridViewGridLink: IDisposable
{
    // ● private fields
    private GridViewController fController;
    private GridViewData fData;
    private bool fDisposed;
    private DataGrid fGrid;
    private bool fSyncingPosition;

    // ● private methods
    private void AttachData(GridViewData Data)
    {
        if (ReferenceEquals(fData, Data))
            return;

        DetachData();

        fData = Data;

        if (fData != null)
            fData.PositionChanged += Data_PositionChanged;
    }
    private void AttachGrid(DataGrid Grid)
    {
        if (Grid != null)
            Grid.SelectionChanged += Grid_SelectionChanged;
    }
    private void Controller_DataChanged(object Sender, GridViewDataChangedEventArgs e)
    {
        AttachData(e != null ? e.Data : null);
        Refresh();
    }
    private void Data_PositionChanged(object Sender, EventArgs e)
    {
        SyncDataToGrid();
    }
    private void DetachController()
    {
        if (fController != null)
            fController.DataChanged -= Controller_DataChanged;
    }
    private void DetachData()
    {
        if (fData != null)
            fData.PositionChanged -= Data_PositionChanged;

        fData = null;
    }
    private void DetachGrid(DataGrid Grid)
    {
        if (Grid != null)
            Grid.SelectionChanged -= Grid_SelectionChanged;
    }
    private int GetSelectedPosition()
    {
        return fGrid != null ? fGrid.SelectedIndex : -1;
    }
    private object GetSelectedRowItemByPosition(int Position)
    {
        if (fGrid == null || Position < 0 || fGrid.ItemsSource is not IEnumerable List)
            return null;

        int Index = 0;
        foreach (object Item in List)
        {
            if (Index == Position)
                return Item;

            Index++;
        }

        return null;
    }
    private void Grid_SelectionChanged(object Sender, SelectionChangedEventArgs e)
    {
        SyncGridToData();
    }
    private void OnBoundChanged()
    {
        AttachData(fController != null ? fController.Data : null);
        Refresh();
    }
    private void SyncDataToGrid()
    {
        if (fSyncingPosition || fGrid == null || fData == null)
            return;

        object SelectedItem = GetSelectedRowItemByPosition(fData.Position);

        if (!ReferenceEquals(fGrid.SelectedItem, SelectedItem))
        {
            try
            {
                fSyncingPosition = true;
                fGrid.SelectedItem = SelectedItem;
            }
            finally
            {
                fSyncingPosition = false;
            }
        }
    }
    private void SyncGridToData()
    {
        if (fSyncingPosition || fData == null || fGrid == null)
            return;

        int Position = GetSelectedPosition();

        try
        {
            fSyncingPosition = true;
            fData.MoveTo(Position);
        }
        finally
        {
            fSyncingPosition = false;
        }
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
        DetachData();
        DetachGrid(fGrid);

        fGrid = Grid;
        fController = Controller;

        AttachGrid(fGrid);

        if (fController != null)
            fController.DataChanged += Controller_DataChanged;

        OnBoundChanged();
    }
    public void Dispose()
    {
        if (!fDisposed)
        {
            DetachController();
            DetachData();
            DetachGrid(fGrid);

            fGrid = null;
            fController = null;
            fDisposed = true;
        }
    }
    public void Refresh()
    {
        if (fGrid == null)
            return;

        AttachData(fController != null ? fController.Data : null);

        try
        {
            fSyncingPosition = true;
            GridViewGridBinder.Apply(fGrid, fController);
        }
        finally
        {
            fSyncingPosition = false;
        }

        SyncDataToGrid();
    }

    // ● properties
    public GridViewController Controller => fController;
    public GridViewData Data => fData;
    public bool IsDisposed => fDisposed;
    public DataGrid Grid => fGrid;
}
 
static internal class GridViewRenderer
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
        if (Node == null || Node.OwnerGroup == null || Node.OwnerGroup.IsRoot)
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

                if (DataRow.Node != null && !DataRow.Node.IsExpanded)
                {
                    foreach (GridViewSummary Summary in DataRow.Node.Summaries)
                    {
                        int SummaryColumnIndex = Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));
                        if (SummaryColumnIndex >= 0 && SummaryColumnIndex != Row.LabelColumnIndex)
                            Row.Values[SummaryColumnIndex] = Summary.Value;
                    }
                }
            }
            else if (DataRow.IsFooter || DataRow.IsGrandTotal)
            {
                if (Columns.Count > 0)
                {
                    int LabelColumnIndex = 0;

                    if (DataRow.IsFooter && DataRow.Node?.OwnerGroup != null && !DataRow.Node.OwnerGroup.IsRoot)
                    {
                        LabelColumnIndex = Columns.FindIndex(x =>
                            string.Equals(x.FieldName, DataRow.Node.OwnerGroup.FieldName, StringComparison.OrdinalIgnoreCase));

                        if (LabelColumnIndex < 0)
                            LabelColumnIndex = 0;
                    }

                    Row.LabelColumnIndex = LabelColumnIndex;
                    Row.Values[LabelColumnIndex] = GetFooterLabel(DataRow.Node);
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
    static private ConditionalWeakTable<DataGrid, GridState> fStates = new();

    // ● private methods
    static private Type GetCoreDataType(GridViewColumnDef ColumnDef)
    {
        if (ColumnDef?.DataType == null)
            return null;

        return Nullable.GetUnderlyingType(ColumnDef.DataType) ?? ColumnDef.DataType;
    }
    static private Control CreateEnumEditingCell(DataGrid Grid, GridState State, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        GridViewController Controller = State != null ? State.Controller : null;
        GridViewData Data = State != null ? State.Data : null;

        Type EnumType = GetCoreDataType(ColumnDef);
        object OriginalValue = Row?.Values != null && ColumnIndex >= 0 && ColumnIndex < Row.Values.Length
            ? GetEnumEditorValue(ColumnDef, Row.Values[ColumnIndex])
            : null;

        ComboBox Editor = new()
        {
            ItemsSource = Enum.GetValues(EnumType),
            SelectedItem = OriginalValue,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            IsEnabled = CanEditCell(Controller, Row, ColumnDef),
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.IsDropDownOpen = true;
                }
            }, DispatcherPriority.Input);
        };

        bool Completed = false;
        bool Canceled = false;

        void Commit()
        {
            if (Completed || Canceled)
                return;

            Completed = true;

            object NewValue = Editor.SelectedItem;
            if (Equals(NewValue, OriginalValue))
                return;

            CommitCellEdit(Grid, Controller, Data, Row, ColumnDef, NewValue);
        }

        void Cancel()
        {
            if (Completed)
                return;

            Canceled = true;
            Completed = true;
        }

        Editor.DropDownClosed += (s, e) => Commit();
        //Editor.LostFocus += (s, e) => Commit();
        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    Commit();
                    e.Handled = true;
                    break;

                case Key.Tab:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    Commit();
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Editor,
        };

        return Result;
    }
    static private object GetEnumEditorValue(GridViewColumnDef ColumnDef, object Value)
    {
        Type EnumType = GetCoreDataType(ColumnDef);

        if (EnumType == null || !EnumType.IsEnum || Value == null || Value == DBNull.Value)
            return null;

        if (Value.GetType().IsEnum)
            return Value;

        try
        {
            if (Value is string S && !string.IsNullOrWhiteSpace(S))
                return Enum.Parse(EnumType, S, true);

            return Enum.ToObject(EnumType, Value);
        }
        catch
        {
            return null;
        }
    }
    
    static private void AttachEditHandlers(DataGrid Grid, GridState State)
    {
        if (Grid == null || State == null)
            return;

        if (State.EditHandlersAttached)
            return;

        Grid.KeyDown += Grid_KeyDown;
        State.EditHandlersAttached = true;
    }
    static private void RestoreCurrentColumn(DataGrid Grid, int ColumnIndex)
    {
        if (Grid == null || ColumnIndex < 0)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (Grid.Columns != null && ColumnIndex >= 0 && ColumnIndex < Grid.Columns.Count)
                Grid.CurrentColumn = Grid.Columns[ColumnIndex];
        }, DispatcherPriority.Input);
    }
    static private bool IsEditStartKey(KeyEventArgs e)
    {
        if (e == null || e.Handled)
            return false;

        Key Key = e.Key;

        if (Key >= Key.A && Key <= Key.Z)
            return true;

        if (Key >= Key.D0 && Key <= Key.D9)
            return true;

        if (Key >= Key.NumPad0 && Key <= Key.NumPad9)
            return true;

        return Key switch
        {
            Key.Space => true,
            Key.OemPlus => true,
            Key.OemMinus => true,
            Key.OemComma => true,
            Key.OemPeriod => true,
            _ => false,
        };
    }
    static private void Grid_KeyDown(object Sender, KeyEventArgs e)
    {
        if (Sender is not DataGrid Grid)
            return;

        GridState State = GetState(Grid);
        if (!IsEditStartKey(e))
            return;

        if (!CanBeginEditFromKeyboard(Grid, State))
            return;

        Grid.BeginEdit();
    }
    static private bool CanBeginEditFromKeyboard(DataGrid Grid, GridState State)
    {
        if (Grid == null || State == null || State.Controller == null)
            return false;

        if (Grid.IsReadOnly)
            return false;

        if (Grid.SelectedItem is not GridViewRenderRow Row)
            return false;

        if (!Row.IsData || Row.DataRow == null)
            return false;

        DataGridColumn GridColumn = Grid.CurrentColumn;
        if (GridColumn == null)
            return false;

        int ColumnIndex = Grid.Columns.IndexOf(GridColumn);
        if (ColumnIndex < 0)
            return false;

        GridViewRenderData RenderData = State.RenderData;
        if (RenderData == null || ColumnIndex >= RenderData.Columns.Count)
            return false;

        GridViewColumnDef ColumnDef = RenderData.Columns[ColumnIndex];
        if (ColumnDef == null)
            return false;

        return State.Controller.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
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
    static private void AddRows(ObservableCollection<GridViewRenderRow> TargetRows, IList<GridViewRenderRow> SourceRows, int StartIndex)
    {
        for (int i = StartIndex; i < SourceRows.Count; i++)
            TargetRows.Add(SourceRows[i]);
    }
    static private void BindRows(DataGrid Grid, GridViewRenderData RenderData, GridState State)
    {
        if (Grid == null)
            return;

        if (State == null)
        {
            Grid.ItemsSource = RenderData != null ? RenderData.Rows : null;
            return;
        }

        if (State.Rows == null)
            State.Rows = new ObservableCollection<GridViewRenderRow>();

        UpdateRows(State.Rows, RenderData != null ? RenderData.Rows : null);

        if (!ReferenceEquals(Grid.ItemsSource, State.Rows))
            Grid.ItemsSource = State.Rows;
    }
    static private void BuildStructure(DataGrid Grid, GridViewRenderData RenderData, GridState State)
    {
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
                    return CreateCell(Grid, State, Row, ColumnDef, ColumnIndex);
                }),
                CellEditingTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateEditingCell(Grid, State, Row, ColumnDef, ColumnIndex);
                }),
                SortMemberPath = ColumnDef.FieldName,
            };

            Grid.Columns.Add(Column);
        }

        if (State != null)
            State.StructureKey = GetStructureKey(RenderData);
        
        if (State != null)
        {
            State.StructureKey = GetStructureKey(RenderData);
            State.RenderData = RenderData;
            AttachEditHandlers(Grid, State);
        }
    }
    static private bool CanEditCell(GridViewController Controller, GridViewRenderRow Row, GridViewColumnDef ColumnDef)
    {
        if (Controller == null || Row == null || Row.DataRow == null || ColumnDef == null)
            return false;

        return Controller.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
    static private void ClearState(GridState State)
    {
        if (State == null)
            return;

        State.Controller = null;
        State.Data = null;
        State.StructureKey = null;

        if (State.Rows != null)
            State.Rows.Clear();
    }
    static private void CommitCellEdit(DataGrid Grid, GridViewController Controller, GridViewData Data, GridViewRenderRow Row, GridViewColumnDef ColumnDef, object Value)
    {
        if (Row == null || Row.DataRow == null || ColumnDef == null)
            return;

        int ColumnIndex = Grid != null && Grid.CurrentColumn != null
            ? Grid.Columns.IndexOf(Grid.CurrentColumn)
            : -1;

        if (Controller != null)
        {
            Controller.SetValue(Row.DataRow, ColumnDef.FieldName, Value);
            RestoreCurrentColumn(Grid, ColumnIndex);
            return;
        }

        if (Row.DataRow.SetValue(ColumnDef.FieldName, Value) && Data != null)
        {
            Apply(Grid, Data);
            RestoreCurrentColumn(Grid, ColumnIndex);
        }
    }
    static private Control CreateCell(DataGrid Grid, GridState State, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && Row.IsGroup && IsLabelCell(Row, ColumnIndex))
            return CreateGroupCell(Grid, State, Row, ColumnIndex);

        TextBlock Text = new()
        {
            Text = FormatValue(Row, ColumnDef, ColumnIndex),
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
    
    static private Control CreateEditingCell(DataGrid Grid, GridState State, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        GridViewController Controller = State != null ? State.Controller : null;
        GridViewData Data = State != null ? State.Data : null;

        if (!CanEditCell(Controller, Row, ColumnDef))
            return CreateCell(Grid, State, Row, ColumnDef, ColumnIndex);
        
        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
            return CreateEnumEditingCell(Grid, State, Row, ColumnDef, ColumnIndex);

        string OriginalText = FormatValue(Row, ColumnDef, ColumnIndex);

        TextBox Editor = new()
        {
            Text = OriginalText,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = GetHorizontalAlignment(Row, ColumnDef, ColumnIndex),
            IsReadOnly = !CanEditCell(Controller, Row, ColumnDef),
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.SelectAll();
                }
            }, DispatcherPriority.Input);
        };

        bool Completed = false;
        bool Canceled = false;

        void Commit()
        {
            if (Completed || Canceled)
                return;

            Completed = true;

            string NewText = Editor.Text ?? string.Empty;
            if (string.Equals(NewText, OriginalText, StringComparison.Ordinal))
                return;

            CommitCellEdit(Grid, Controller, Data, Row, ColumnDef, NewText);
        }

        void Cancel()
        {
            if (Completed)
                return;

            Canceled = true;
            Completed = true;
        }

        Editor.LostFocus += (s, e) => Commit();
        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;
            
            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    break;

                case Key.Enter:
                    Commit();
                    e.Handled = true;
                    break; 
                
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    Commit();
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Editor,
        };

        return Result;
    }
    static private Control CreateGroupCell(DataGrid Grid, GridState State, GridViewRenderRow Row, int ColumnIndex)
    {
        string Glyph = Row != null && Row.Node != null && Row.Node.IsExpanded ? "▼ " : "▶ ";

        TextBlock Text = new()
        {
            Text = Glyph + FormatValue(Row, null, ColumnIndex),
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
            if (Row == null || Row.Node == null)
                return;

            if (Row.Node.Toggle() && State != null && State.Data != null)
                Apply(Grid, State.Data, State.Controller);
        };

        return Result;
    }
 
    static private string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

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
        
        if (Row.IsGroup && Row.Node != null && !Row.Node.IsExpanded && HasValue(Row, ColumnIndex))
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
    static private GridState GetState(DataGrid Grid)
    {
        if (Grid == null)
            return null;

        if (!fStates.TryGetValue(Grid, out GridState State))
        {
            State = new GridState();
            fStates.Add(Grid, State);
        }

        return State;
    }
    static private string GetStructureKey(GridViewRenderData RenderData)
    {
        if (RenderData == null || RenderData.Columns == null || RenderData.Columns.Count == 0)
            return string.Empty;

        return string.Join("\u001F", RenderData.Columns.Select(x =>
        {
            string FieldName = x.FieldName ?? string.Empty;
            string Title = x.Title ?? string.Empty;
            string DataType = x.DataType != null ? x.DataType.FullName : string.Empty;
            return $"{FieldName}|{Title}|{x.IsReadOnly}|{x.VisibleIndex}|{x.GroupIndex}|{x.Aggregate}|{DataType}";
        }));
    }
    static private bool HasStructureChanged(GridState State, GridViewRenderData RenderData)
    {
        string NewKey = GetStructureKey(RenderData);

        if (State == null)
            return true;

        return !string.Equals(State.StructureKey, NewKey, StringComparison.Ordinal);
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
    static private void RemoveRows(ObservableCollection<GridViewRenderRow> TargetRows, int StartIndex)
    {
        while (TargetRows.Count > StartIndex)
            TargetRows.RemoveAt(TargetRows.Count - 1);
    }
    static private void UpdateRows(ObservableCollection<GridViewRenderRow> TargetRows, IList<GridViewRenderRow> SourceRows)
    {
        SourceRows ??= Array.Empty<GridViewRenderRow>();

        int CommonCount = Math.Min(TargetRows.Count, SourceRows.Count);

        for (int i = 0; i < CommonCount; i++)
        {
            if (!ReferenceEquals(TargetRows[i], SourceRows[i]))
                TargetRows[i] = SourceRows[i];
        }

        if (TargetRows.Count > SourceRows.Count)
            RemoveRows(TargetRows, SourceRows.Count);
        else if (SourceRows.Count > TargetRows.Count)
            AddRows(TargetRows, SourceRows, TargetRows.Count);
    }
    static private void UpdateState(GridState State, GridViewData Data, GridViewController Controller)
    {
        if (State == null)
            return;

        State.Data = Data;
        State.Controller = Controller;
    }
 
    static internal void Apply(DataGrid Grid, GridViewController Controller)
    {
        if (Controller == null)
            throw new ArgumentNullException(nameof(Controller));

        Apply(Grid, Controller.Data, Controller);
    }
    static private void Apply(DataGrid Grid, GridViewData Data)
    {
        Apply(Grid, Data, null);
    }
    static private void Apply(DataGrid Grid, GridViewData Data, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridState State = GetState(Grid);
        UpdateState(State, Data, Controller);

        if (Data == null)
        {
            Grid.AutoGenerateColumns = false;
            Grid.Columns.Clear();
            Grid.ItemsSource = null;
            ClearState(State);
            return;
        }

        GridViewRenderData RenderData = GridViewRenderer.Render(Data);
        Apply(Grid, RenderData, Data, Controller);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData)
    {
        Apply(Grid, RenderData, null, null);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data)
    {
        Apply(Grid, RenderData, Data, null);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (RenderData == null)
            throw new ArgumentNullException(nameof(RenderData));

        GridState State = GetState(Grid);
        UpdateState(State, Data, Controller);
        State.RenderData = RenderData;
        
        if (HasStructureChanged(State, RenderData))
            BuildStructure(Grid, RenderData, State);

        BindRows(Grid, RenderData, State);
    }
    
    // ● static public methods
    static public void Bind(DataGrid Grid, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridViewGridLink Result = new(Grid, Controller);
        AddOrReplaceLink(Grid, Result);
        //return Result;
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

        if (fStates.TryGetValue(Grid, out GridState State))
        {
            ClearState(State);
            fStates.Remove(Grid);
        }
    }

    // ● private classes
    private class GridState
    {
        // ● properties
        public GridViewController Controller { get; set; }
        public GridViewData Data { get; set; }
        public ObservableCollection<GridViewRenderRow> Rows { get; set; }
        public string StructureKey { get; set; }
        public bool EditHandlersAttached { get; set; }
        public GridViewRenderData RenderData { get; set; }
    }
}
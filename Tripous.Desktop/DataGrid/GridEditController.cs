namespace Tripous.Desktop;

/// <summary>
/// Controls keyboard editing behavior for a <see cref="DataGrid"/>.
/// </summary>
public class GridEditController
{
    // ● fields
    readonly DataGrid fGrid;
    bool fIsAttached;
    bool fIsEditing;

    // ● private
    GridEditController(DataGrid Grid)
    {
        fGrid = Grid ?? throw new TripousArgumentNullException(nameof(Grid));
    }
    void Attach()
    {
        if (fIsAttached)
            return;

        fGrid.AddHandler(InputElement.TextInputEvent, Grid_TextInput, RoutingStrategies.Tunnel, handledEventsToo: true);
        fGrid.AddHandler(InputElement.KeyDownEvent, Grid_KeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        fGrid.BeginningEdit += Grid_BeginningEdit;
        fGrid.CellEditEnded += Grid_CellEditEnded;
        fIsAttached = true;
    }
    void Detach()
    {
        if (!fIsAttached)
            return;

        fGrid.RemoveHandler(InputElement.TextInputEvent, Grid_TextInput);
        fGrid.RemoveHandler(InputElement.KeyDownEvent, Grid_KeyDown);
        fGrid.BeginningEdit -= Grid_BeginningEdit;
        fGrid.CellEditEnded -= Grid_CellEditEnded;
        fIsAttached = false;
    }
    bool IsFocusedEditor()
    {
        IInputElement FocusedElement = TopLevel.GetTopLevel(fGrid)?.FocusManager?.GetFocusedElement();
        if (FocusedElement is not Visual Visual)
            return false;

        return Visual is TextBox
            || Visual is ComboBox
            || Visual is CheckBox
            || Visual.FindAncestorOfType<TextBox>() != null
            || Visual.FindAncestorOfType<ComboBox>() != null
            || Visual.FindAncestorOfType<CheckBox>() != null;
    }
    ComboBox GetFocusedComboBox()
    {
        IInputElement FocusedElement = TopLevel.GetTopLevel(fGrid)?.FocusManager?.GetFocusedElement();
        if (FocusedElement is not Visual Visual)
            return null;

        return Visual as ComboBox ?? Visual.FindAncestorOfType<ComboBox>();
    }
    ComboBox GetOpenComboBox()
    {
        return fGrid.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault(ComboBox => ComboBox.IsDropDownOpen);
    }
    ComboBox GetCurrentCellComboBox()
    {
        DataGridCell Cell = FindCurrentCell();
        return Cell?.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault();
    }
    bool CanBeginEdit()
    {
        if (fGrid.IsReadOnly || fGrid.SelectedItem == null || fGrid.CurrentColumn == null)
            return false;

        return !fGrid.CurrentColumn.IsReadOnly;
    }
    bool IsPrintableText(string Text)
    {
        return !string.IsNullOrEmpty(Text) && !Text.Any(c => char.IsControl(c));
    }
    List<DataGridColumn> GetVisibleColumns()
    {
        return fGrid.Columns.Where(Column => Column.IsVisible).ToList();
    }
    int GetCurrentColumnIndex(List<DataGridColumn> Columns)
    {
        if (fGrid.CurrentColumn == null)
            return Columns.Count > 0 ? 0 : -1;

        return Columns.IndexOf(fGrid.CurrentColumn);
    }
    bool MoveCurrentCell(int Delta)
    {
        List<DataGridColumn> Columns = GetVisibleColumns();
        int Index = GetCurrentColumnIndex(Columns);
        if (Index < 0)
            return false;

        int NewIndex = Index + Delta;
        if (NewIndex < 0 || NewIndex >= Columns.Count)
            return false;

        fGrid.CurrentColumn = Columns[NewIndex];
        FocusCurrentCell();
        return true;
    }
    bool MoveCurrentRow(int Delta)
    {
        if (fGrid.ItemsSource is not IList Items || Items.Count == 0)
            return false;

        int Index = fGrid.SelectedIndex >= 0 ? fGrid.SelectedIndex : 0;
        int NewIndex = Index + Delta;
        if (NewIndex < 0 || NewIndex >= Items.Count)
            return false;

        fGrid.SelectedIndex = NewIndex;
        fGrid.SelectedItem = Items[NewIndex];
        FocusCurrentCell();
        return true;
    }
    void EnsureCurrentColumn()
    {
        if (fGrid.CurrentColumn != null)
            return;

        fGrid.CurrentColumn = GetVisibleColumns().FirstOrDefault();
    }
    void FocusCurrentCell()
    {
        EnsureCurrentColumn();
        fGrid.ScrollIntoView(fGrid.SelectedItem, fGrid.CurrentColumn);
        fGrid.Focus(NavigationMethod.Tab, KeyModifiers.None);
        Dispatcher.UIThread.Post(() =>
        {
            DataGridCell Cell = FindCurrentCell();
            Cell?.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }, DispatcherPriority.Input);
    }
    DataGridCell FindCurrentCell()
    {
        if (fGrid.SelectedItem == null || fGrid.CurrentColumn == null)
            return null;

        foreach (DataGridCell Cell in fGrid.GetVisualDescendants().OfType<DataGridCell>())
        {
            if (!object.Equals(Cell.DataContext, fGrid.SelectedItem))
                continue;
            if (DataGridColumn.GetColumnContainingElement(Cell) == fGrid.CurrentColumn)
                return Cell;
        }

        return null;
    }
    bool CommitCellEdit()
    {
        return fGrid.CommitEdit(DataGridEditingUnit.Cell, true);
    }
    bool OpenCurrentLookupDropDown()
    {
        if (!CanBeginEdit())
            return false;

        GridColumnBinding Binding = fGrid.CurrentColumn.GetInfo();
        if (Binding == null || Binding.LookupSource == null)
            return false;
        if (!fIsEditing && !fGrid.BeginEdit())
            return false;

        Dispatcher.UIThread.Post(() =>
        {
            ComboBox ComboBox = GetFocusedComboBox() ?? GetCurrentCellComboBox();
            if (ComboBox == null)
                return;

            ComboBox.Focus(NavigationMethod.Tab, KeyModifiers.None);
            ComboBox.IsDropDownOpen = true;
        }, DispatcherPriority.Input);

        return true;
    }
    bool CancelCellEdit()
    {
        return fGrid.CancelEdit(DataGridEditingUnit.Cell);
    }
    bool ToggleCurrentBooleanCell()
    {
        if (fGrid.SelectedItem is not DataRowView RowView || fGrid.CurrentColumn == null)
            return false;

        GridColumnBinding Binding = fGrid.CurrentColumn.GetInfo();
        if (Binding == null || Binding.DataColumn == null)
            return false;

        DataColumn Column = Binding.DataColumn;
        DataColumnType ColumnType = Column.ExtendedProperties.ContainsKey("ColumnType")
            ? (DataColumnType)Column.ExtendedProperties["ColumnType"]
            : DataColumnType.None;
        bool IsBoolean = ColumnType.HasFlag(DataColumnType.Boolean) || Column.DataType == typeof(bool) || Column.IsCheckBox();
        if (!IsBoolean || Column.ReadOnly)
            return false;

        bool Value = RowView.AsBoolean(Column.ColumnName);
        RowView.BeginEdit();
        RowView[Column.ColumnName] = Column.DataType == typeof(bool) ? !Value : !Value ? 1 : 0;
        FocusCurrentCell();
        return true;
    }
    void Grid_TextInput(object Sender, TextInputEventArgs Args)
    {
        if (fIsEditing || !IsPrintableText(Args.Text) || IsFocusedEditor() || !CanBeginEdit())
            return;

        if (fGrid.BeginEdit())
            Args.Handled = true;
    }
    void Grid_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Tab)
        {
            if (fIsEditing)
                CommitCellEdit();

            MoveCurrentCell(Args.KeyModifiers.HasFlag(KeyModifiers.Shift) ? -1 : 1);
            Args.Handled = true;
            return;
        }

        if (fIsEditing)
        {
            ComboBox ComboBox = GetFocusedComboBox() ?? GetOpenComboBox();
            bool IsComboDropDownKey = Args.Key switch
            {
                Key.Enter => true,
                Key.Escape => true,
                Key.Up => true,
                Key.Down => true,
                _ => false
            };
            if (ComboBox != null && ComboBox.IsDropDownOpen && IsComboDropDownKey)
                return;

            if (Args.Key == Key.Enter)
            {
                CommitCellEdit();
                Dispatcher.UIThread.Post(FocusCurrentCell, DispatcherPriority.Input);
                Args.Handled = true;
                return;
            }
            if (Args.Key == Key.Escape)
            {
                CancelCellEdit();
                Dispatcher.UIThread.Post(FocusCurrentCell, DispatcherPriority.Input);
                Args.Handled = true;
                return;
            }
            return;
        }

        if (Args.Key == Key.Left || Args.Key == Key.Right)
        {
            MoveCurrentCell(Args.Key == Key.Left ? -1 : 1);
            Args.Handled = true;
            return;
        }
        if (Args.Key == Key.Down && Args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            if (OpenCurrentLookupDropDown())
                Args.Handled = true;
            return;
        }
        if (Args.Key == Key.Up || Args.Key == Key.Down)
        {
            MoveCurrentRow(Args.Key == Key.Up ? -1 : 1);
            Args.Handled = true;
            return;
        }
        if (Args.Key == Key.Space && ToggleCurrentBooleanCell())
        {
            Args.Handled = true;
            return;
        }

        if (IsFocusedEditor() || !CanBeginEdit())
            return;

        if (Args.Key == Key.Enter || Args.Key == Key.F2)
        {
            if (fGrid.BeginEdit())
                Args.Handled = true;
        }
    }
    void Grid_BeginningEdit(object Sender, DataGridBeginningEditEventArgs Args)
    {
        fIsEditing = true;
    }
    void Grid_CellEditEnded(object Sender, DataGridCellEditEndedEventArgs Args)
    {
        fIsEditing = false;
    }

    // ● static public
    static public readonly AttachedProperty<GridEditController> ControllerProperty =
        AvaloniaProperty.RegisterAttached<GridEditController, DataGrid, GridEditController>("Controller");
    static public GridEditController Attach(DataGrid Grid)
    {
        GridEditController Result = GetController(Grid);
        if (Result != null)
            return Result;

        Result = new GridEditController(Grid);
        SetController(Grid, Result);
        Result.Attach();
        return Result;
    }
    static public void Detach(DataGrid Grid)
    {
        GridEditController Controller = GetController(Grid);
        if (Controller == null)
            return;

        Controller.Detach();
        SetController(Grid, null);
    }
    static public GridEditController GetController(DataGrid Grid) => Grid.GetValue(ControllerProperty);
    static public void SetController(DataGrid Grid, GridEditController Value) => Grid.SetValue(ControllerProperty, Value);

    // ● properties
    public DataGrid Grid => fGrid;
    public bool IsEditing => fIsEditing;
}

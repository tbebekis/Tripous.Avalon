/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Controls keyboard editing behavior for a data grid.
/// </summary>
public class GridEditController
{
    // ● private fields
    /// <summary>
    /// The controlled data grid.
    /// </summary>
    readonly DataGrid fGrid;
    /// <summary>
    /// True when this controller is attached to the grid.
    /// </summary>
    bool fIsAttached;
    /// <summary>
    /// True while a grid cell is being edited.
    /// </summary>
    bool fIsEditing;

    // ● private
    /// <summary>
    /// Initializes a new instance of the <see cref="GridEditController"/> class.
    /// </summary>
    /// <param name="Grid">The data grid.</param>
    GridEditController(DataGrid Grid)
    {
        fGrid = Grid ?? throw new TripousArgumentNullException(nameof(Grid));
    }
    /// <summary>
    /// Attaches this controller to the grid.
    /// </summary>
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
    /// <summary>
    /// Detaches this controller from the grid.
    /// </summary>
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
    /// <summary>
    /// Returns true when the focused element is an editor control.
    /// </summary>
    /// <returns>True if the focused element is an editor control; otherwise, false.</returns>
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
    /// <summary>
    /// Returns the focused combo box.
    /// </summary>
    /// <returns>The focused combo box, if any; otherwise, null.</returns>
    ComboBox GetFocusedComboBox()
    {
        IInputElement FocusedElement = TopLevel.GetTopLevel(fGrid)?.FocusManager?.GetFocusedElement();
        if (FocusedElement is not Visual Visual)
            return null;

        return Visual as ComboBox ?? Visual.FindAncestorOfType<ComboBox>();
    }
    /// <summary>
    /// Returns an open combo box in the grid.
    /// </summary>
    /// <returns>The open combo box, if any; otherwise, null.</returns>
    ComboBox GetOpenComboBox()
    {
        return fGrid.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault(ComboBox => ComboBox.IsDropDownOpen);
    }
    /// <summary>
    /// Returns the combo box in the current cell.
    /// </summary>
    /// <returns>The current cell combo box, if any; otherwise, null.</returns>
    ComboBox GetCurrentCellComboBox()
    {
        DataGridCell Cell = FindCurrentCell();
        return Cell?.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault();
    }
    /// <summary>
    /// Returns true when the current cell can enter edit mode.
    /// </summary>
    /// <returns>True if editing can begin; otherwise, false.</returns>
    bool CanBeginEdit()
    {
        if (fGrid.IsReadOnly || fGrid.SelectedItem == null || fGrid.CurrentColumn == null)
            return false;

        return !fGrid.CurrentColumn.IsReadOnly;
    }
    /// <summary>
    /// Returns true when text contains printable characters.
    /// </summary>
    /// <param name="Text">The text to check.</param>
    /// <returns>True if the text is printable; otherwise, false.</returns>
    bool IsPrintableText(string Text)
    {
        return !string.IsNullOrEmpty(Text) && !Text.Any(c => char.IsControl(c));
    }
    /// <summary>
    /// Returns the visible grid columns ordered by display index.
    /// </summary>
    /// <returns>The visible grid columns.</returns>
    List<DataGridColumn> GetVisibleColumns()
    {
        return fGrid.Columns
            .Where(Column => Column.IsVisible)
            .OrderBy(Column => Column.DisplayIndex)
            .ToList();
    }
    /// <summary>
    /// Returns the index of the current column in a column list.
    /// </summary>
    /// <param name="Columns">The column list.</param>
    /// <returns>The current column index.</returns>
    int GetCurrentColumnIndex(List<DataGridColumn> Columns)
    {
        if (fGrid.CurrentColumn == null)
            return Columns.Count > 0 ? 0 : -1;

        return Columns.IndexOf(fGrid.CurrentColumn);
    }
    /// <summary>
    /// Moves the current cell horizontally.
    /// </summary>
    /// <param name="Delta">The column offset.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
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
    /// <summary>
    /// Moves the current row vertically.
    /// </summary>
    /// <param name="Delta">The row offset.</param>
    /// <returns>True if the current row moved; otherwise, false.</returns>
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
    /// <summary>
    /// Ensures that the grid has a current column.
    /// </summary>
    /// <returns>True if the current column exists; otherwise, false.</returns>
    bool EnsureCurrentColumn()
    {
        if (fGrid.SelectedItem == null)
            return false;
        if (fGrid.CurrentColumn != null)
            return true;

        fGrid.CurrentColumn = GetVisibleColumns().FirstOrDefault();
        return fGrid.CurrentColumn != null;
    }
    /// <summary>
    /// Focuses the current grid cell.
    /// </summary>
    void FocusCurrentCell()
    {
        if (!EnsureCurrentColumn())
            return;

        fGrid.ScrollIntoView(fGrid.SelectedItem, fGrid.CurrentColumn);
        fGrid.Focus(NavigationMethod.Tab, KeyModifiers.None);
        Dispatcher.UIThread.Post(() =>
        {
            DataGridCell Cell = FindCurrentCell();
            Cell?.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }, DispatcherPriority.Input);
    }
    /// <summary>
    /// Finds the current grid cell.
    /// </summary>
    /// <returns>The current grid cell, if any; otherwise, null.</returns>
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
    /// <summary>
    /// Commits the current cell edit.
    /// </summary>
    /// <returns>True if the edit was committed; otherwise, false.</returns>
    bool CommitCellEdit()
    {
        return fGrid.CommitEdit(DataGridEditingUnit.Cell, true);
    }
    /// <summary>
    /// Opens the lookup drop-down of the current cell.
    /// </summary>
    /// <returns>True if the drop-down was opened; otherwise, false.</returns>
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
    /// <summary>
    /// Cancels the current cell edit.
    /// </summary>
    /// <returns>True if the edit was cancelled; otherwise, false.</returns>
    bool CancelCellEdit()
    {
        return fGrid.CancelEdit(DataGridEditingUnit.Cell);
    }
    /// <summary>
    /// Toggles the boolean value of the current cell.
    /// </summary>
    /// <returns>True if the value was toggled; otherwise, false.</returns>
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
    /// <summary>
    /// Handles text input and starts editing when needed.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The text input event arguments.</param>
    void Grid_TextInput(object Sender, TextInputEventArgs Args)
    {
        if (fIsEditing || !IsPrintableText(Args.Text) || IsFocusedEditor() || !CanBeginEdit())
            return;

        if (fGrid.BeginEdit())
            Args.Handled = true;
    }
    /// <summary>
    /// Handles grid key down events.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The key event arguments.</param>
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
    /// <summary>
    /// Handles the beginning edit event.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The beginning edit event arguments.</param>
    void Grid_BeginningEdit(object Sender, DataGridBeginningEditEventArgs Args)
    {
        fIsEditing = true;
    }
    /// <summary>
    /// Handles the cell edit ended event.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The cell edit ended event arguments.</param>
    void Grid_CellEditEnded(object Sender, DataGridCellEditEndedEventArgs Args)
    {
        fIsEditing = false;
    }

    // ● static public
    /// <summary>
    /// Identifies the attached controller property.
    /// </summary>
    static public readonly AttachedProperty<GridEditController> ControllerProperty =
        AvaloniaProperty.RegisterAttached<GridEditController, DataGrid, GridEditController>("Controller");
    /// <summary>
    /// Attaches a grid edit controller to a data grid.
    /// </summary>
    /// <param name="Grid">The data grid.</param>
    /// <returns>The attached grid edit controller.</returns>
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
    /// <summary>
    /// Detaches the grid edit controller from a data grid.
    /// </summary>
    /// <param name="Grid">The data grid.</param>
    static public void Detach(DataGrid Grid)
    {
        GridEditController Controller = GetController(Grid);
        if (Controller == null)
            return;

        Controller.Detach();
        SetController(Grid, null);
    }
    /// <summary>
    /// Returns the grid edit controller attached to a data grid.
    /// </summary>
    /// <param name="Grid">The data grid.</param>
    /// <returns>The attached grid edit controller, if any; otherwise, null.</returns>
    static public GridEditController GetController(DataGrid Grid) => Grid.GetValue(ControllerProperty);
    /// <summary>
    /// Sets the grid edit controller attached to a data grid.
    /// </summary>
    /// <param name="Grid">The data grid.</param>
    /// <param name="Value">The grid edit controller.</param>
    static public void SetController(DataGrid Grid, GridEditController Value) => Grid.SetValue(ControllerProperty, Value);

    // ● properties
    /// <summary>
    /// Gets the controlled data grid.
    /// </summary>
    public DataGrid Grid => fGrid;
    /// <summary>
    /// Gets a value indicating whether a grid cell is being edited.
    /// </summary>
    public bool IsEditing => fIsEditing;
}

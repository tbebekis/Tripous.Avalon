/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// A lightweight locator editor for data grid cells.
/// </summary>
public class GridLocatorBox: UserControl
{
    // ● private fields
    Grid fRoot;
    TextBox fTextBox;
    Popup fPopup;
    Border fPopupBorder;
    DataGrid fGrid;
    DataViewItemsSource fPopupItemsSource;
    Locator fLocator;
    LocatorDef fLocatorDef;
    LocatorFieldDef fLocatorFieldDef;
    DataRowView fRowView;
    DataGrid fOwnerGrid;
    DataGridColumn fOwnerColumn;
    string fKeyFieldName;
    Dictionary<string, string> fTargetFieldMap = [];

    // ● private methods
    void Build()
    {
        fRoot = new Grid();
        fTextBox = new TextBox();
        fTextBox.Padding = new Thickness(6, 2, 6, 2);
        fTextBox.VerticalContentAlignment = VerticalAlignment.Center;
        fTextBox.TextChanged += TextBox_TextChanged;
        fTextBox.KeyDown += TextBox_KeyDown;
        fRoot.Children.Add(fTextBox);
        CreatePopup();
        Content = fRoot;
    }
    void CreatePopup()
    {
        fGrid = new DataGrid();
        fGrid.AutoGenerateColumns = false;
        fGrid.IsReadOnly = true;
        fGrid.Background = Brushes.White;
        fGrid.MinWidth = 300;
        fGrid.MaxHeight = 260;
        fGrid.DoubleTapped += Grid_DoubleTapped;
        fGrid.KeyDown += Grid_KeyDown;
        fGrid.AddHandler(KeyDownEvent, Grid_PreviewKeyDown, RoutingStrategies.Tunnel);
        fPopupBorder = new Border();
        fPopupBorder.Background = Brushes.White;
        fPopupBorder.BorderBrush = Brushes.Gray;
        fPopupBorder.BorderThickness = new Thickness(1);
        fPopupBorder.Child = fGrid;
        fPopup = new Popup();
        fPopup.PlacementTarget = this;
        fPopup.Placement = PlacementMode.Bottom;
        fPopup.IsLightDismissEnabled = true;
        fPopup.Child = fPopupBorder;
        fRoot.Children.Add(fPopup);
    }
    void CreatePopupColumns()
    {
        fGrid.Columns.Clear();
        if (Locator == null || Locator.LocatorDef == null || Locator.SourceTable == null)
            return;

        foreach (LocatorFieldDef FieldDef in Locator.LocatorDef.Fields.Where(item => item.IsVisible))
        {
            DataColumn Column = Locator.SourceTable.FindColumn(FieldDef.Alias);
            if (Column != null)
                fGrid.Columns.Add(DataGridBinder.CreateGridColumn(Column, IsReadOnly: true));
        }
    }
    void OpenPopup()
    {
        if (fPopup == null || fGrid == null || Locator == null)
            return;

        fPopupItemsSource?.Dispose();
        fPopupItemsSource = new DataViewItemsSource(Locator.SourceTable.DataView);
        double Width = Bounds.Width > 0 ? Bounds.Width : 300;
        fGrid.Width = Math.Max(Width, 300);
        CreatePopupColumns();
        fGrid.ItemsSource = fPopupItemsSource;
        fGrid.SelectedIndex = fPopupItemsSource.Count > 0 ? 0 : -1;
        fPopup.PlacementTarget = this;
        fPopup.IsOpen = true;
        Ui.Post(() => fGrid.Focus());
    }
    void ClosePopup()
    {
        if (fPopup != null)
            fPopup.IsOpen = false;
    }
    void CommitCell()
    {
        DataGrid Grid = GetOwnerGrid();
        object SelectedItem = RowView;
        DataGridColumn CurrentColumn = fOwnerColumn ?? Grid?.CurrentColumn;
        Grid?.CommitEdit(DataGridEditingUnit.Cell, true);
        UpdateLocatorDisplayCells(Grid, RowView);
        RestoreGridCellFocus(Grid, SelectedItem, CurrentColumn);
    }
    void CancelCell()
    {
        DataGrid Grid = GetOwnerGrid();
        RowView?.CancelEdit();
        Grid?.CancelEdit();
    }
    DataGrid GetOwnerGrid()
    {
        fOwnerGrid ??= this.FindAncestorOfType<DataGrid>();
        return fOwnerGrid;
    }
    string GetLogSearchTerm(string Term)
    {
        return !string.IsNullOrWhiteSpace(Term) ? Term.Trim().TrimEnd('?').Trim() : string.Empty;
    }
    void AssignSourceRow(DataRow SourceRow)
    {
        if (SourceRow == null || RowView?.Row == null || RowView.Row.RowState.In(DataRowState.Deleted | DataRowState.Detached) || Locator == null)
            return;

        RowView.BeginEdit();
        Locator.Assign(SourceRow, RowView.Row, KeyFieldName, TargetFieldMap);
        RowView.EndEdit();
    }
    void UpdateLocatorDisplayCells(DataGrid Grid, DataRowView TargetRowView)
    {
        if (Grid == null || TargetRowView?.Row == null)
            return;

        foreach (DataGridCell Cell in Grid.GetVisualDescendants().OfType<DataGridCell>())
        {
            if (Cell.DataContext is not DataRowView RowView || !ReferenceEquals(RowView.Row, TargetRowView.Row))
                continue;

            DataGridColumn Column = DataGridColumn.GetColumnContainingElement(Cell);
            GridColumnBinding Binding = Column.GetInfo();
            if (Binding == null || Binding.LocatorDef != LocatorDef || !Binding.FieldName.IsSameText(KeyFieldName) || string.IsNullOrWhiteSpace(Binding.DisplayFieldName))
                continue;
            if (!RowView.Row.Table.Columns.Contains(Binding.DisplayFieldName))
                continue;

            object Value = RowView[Binding.DisplayFieldName];
            foreach (TextBlock TextBlock in Cell.GetVisualDescendants().OfType<TextBlock>())
                TextBlock.Text = DataGridBinder.FormatValue(Value == DBNull.Value ? null : Value, null);
        }
    }
    void RestoreGridCellFocus(DataGrid Grid, object SelectedItem, DataGridColumn CurrentColumn)
    {
        if (Grid == null || SelectedItem == null || CurrentColumn == null)
            return;

        void Apply()
        {
            Grid.SelectedItem = SelectedItem;
            Grid.CurrentColumn = CurrentColumn;
            Grid.ScrollIntoView(SelectedItem, CurrentColumn);
            UpdateLocatorDisplayCells(Grid, RowView);
        }

        Dispatcher.UIThread.Post(() =>
        {
            Apply();
            Dispatcher.UIThread.Post(() =>
            {
                Apply();
                foreach (DataGridCell Cell in Grid.GetVisualDescendants().OfType<DataGridCell>())
                {
                    if (!object.Equals(Cell.DataContext, SelectedItem))
                        continue;
                    if (DataGridColumn.GetColumnContainingElement(Cell) != CurrentColumn)
                        continue;

                    Cell.Focus(NavigationMethod.Tab, KeyModifiers.None);
                    break;
                }
            }, DispatcherPriority.Input);
        }, DispatcherPriority.Background);
    }
    void SelectCurrentRow()
    {
        if (fGrid?.SelectedItem is DataRowView SourceRowView)
        {
            AssignSourceRow(SourceRowView.Row);
            ClosePopup();
            CommitCell();
        }
    }
    void Search()
    {
        if (Locator == null)
            return;

        string Term = fTextBox.Text;
        string LogTerm = GetLogSearchTerm(Term);
        LogBox.AppendLine($"Grid Locator: Searching for term: {LogTerm}");
        try
        {
            LocatorSearchResult Result = Locator.Execute(Term);
            if (Result.TooManyRows)
            {
                ClosePopup();
                LogBox.AppendLine($"Grid Locator: Too many rows for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info(Result.Message, this));
            }
            else if (Result.IsEmpty)
            {
                ClosePopup();
                LogBox.AppendLine($"Grid Locator: No rows found for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info("No rows found.", this));
            }
            else if (Result.IsSingleRow)
            {
                ClosePopup();
                LogBox.AppendLine($"Grid Locator: Found 1 row for term: {LogTerm}");
                AssignSourceRow(Result.SourceTable.Rows[0]);
                CommitCell();
            }
            else
            {
                LogBox.AppendLine($"Grid Locator: Found {Result.RowCount} rows for term: {LogTerm}");
                OpenPopup();
            }
        }
        catch (Exception e)
        {
            ClosePopup();
            LogBox.AppendLine($"Grid Locator: {e.Message}");
            Ui.Post(async () => await MessageBox.Error(e, this));
        }
    }
    void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        if (Sender is TextBox TextBox && Locator != null && Locator.ContainsSearchTrigger(TextBox.Text))
            Search();
    }
    void TextBox_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Escape)
        {
            if (fPopup != null && fPopup.IsOpen)
            {
                ClosePopup();
                FocusEditor();
            }
            else
            {
                CancelCell();
            }
            Args.Handled = true;
        }
    }
    void Grid_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            ClosePopup();
            FocusEditor();
            Args.Handled = true;
        }
    }
    void Grid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        SelectCurrentRow();
    }
    void Grid_PreviewKeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
    }

    // ● overridables
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        fOwnerGrid = this.FindAncestorOfType<DataGrid>();
        fOwnerColumn = DataGridColumn.GetColumnContainingElement(this);
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public GridLocatorBox()
    {
        Build();
    }

    // ● public methods
    /// <summary>
    /// Initializes this editor.
    /// </summary>
    public void Initialize(LocatorDef LocatorDef, LocatorFieldDef LocatorFieldDef, DataRowView RowView, string KeyFieldName, Dictionary<string, string> TargetFieldMap)
    {
        this.LocatorDef = LocatorDef;
        this.LocatorFieldDef = LocatorFieldDef;
        this.RowView = RowView;
        this.KeyFieldName = KeyFieldName;
        this.TargetFieldMap = TargetFieldMap ?? [];
        this.Locator = LocatorDef?.Create();
    }
    /// <summary>
    /// Focuses the editor textbox.
    /// </summary>
    public void FocusEditor()
    {
        fTextBox.Focus(NavigationMethod.Tab, KeyModifiers.None);
        Dispatcher.UIThread.Post(() => fTextBox.SelectAll(), DispatcherPriority.Input);
    }
    /// <summary>
    /// Sets the editor text.
    /// </summary>
    public void SetText(string Text)
    {
        fTextBox.Text = Text;
    }

    // ● properties
    /// <summary>
    /// The locator definition.
    /// </summary>
    public LocatorDef LocatorDef
    {
        get => fLocatorDef;
        set => fLocatorDef = value;
    }
    /// <summary>
    /// The locator field definition.
    /// </summary>
    public LocatorFieldDef LocatorFieldDef
    {
        get => fLocatorFieldDef;
        set => fLocatorFieldDef = value;
    }
    /// <summary>
    /// The row view edited by this control.
    /// </summary>
    public DataRowView RowView
    {
        get => fRowView;
        set => fRowView = value;
    }
    /// <summary>
    /// The target key field name.
    /// </summary>
    public string KeyFieldName
    {
        get => fKeyFieldName;
        set => fKeyFieldName = value;
    }
    /// <summary>
    /// Maps locator field names to target row field names.
    /// </summary>
    public Dictionary<string, string> TargetFieldMap
    {
        get => fTargetFieldMap;
        set => fTargetFieldMap = value ?? [];
    }
    /// <summary>
    /// The locator used by this editor.
    /// </summary>
    public Locator Locator
    {
        get => fLocator;
        set => fLocator = value;
    }
}

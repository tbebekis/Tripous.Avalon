/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Event arguments for locator row selection.
/// </summary>
public class LocatorBoxRowEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorBoxRowEventArgs(DataRow Row)
    {
        this.Row = Row;
    }

    // ● properties
    /// <summary>
    /// The selected source row.
    /// </summary>
    public DataRow Row { get; }
}

/// <summary>
/// A composite locator editor.
/// </summary>
public class LocatorBox: UserControl
{
    // ● private fields
    protected Grid fRoot;
    protected Popup fPopup;
    protected Border fPopupBorder;
    protected DataGrid fGrid;
    protected Button fMenuButton;
    protected DataViewItemsSource fPopupItemsSource;
    protected List<TextBox> fTextBoxes = [];
    protected Dictionary<LocatorFieldDef, TextBox> fTextBoxMap = [];

    // ● protected methods
    protected virtual void ApplyReadOnly()
    {
        foreach (KeyValuePair<LocatorFieldDef, TextBox> Entry in fTextBoxMap)
            Entry.Value.IsReadOnly = IsReadOnly || !Entry.Key.IsSearchable;
    }
    /// <summary>
    /// Returns the column width of a locator field.
    /// </summary>
    protected virtual GridLength GetFieldWidth(LocatorFieldDef FieldDef, int Index, List<LocatorFieldDef> Fields)
    {
        bool IsLast = Index == Fields.Count - 1;
        if (IsLast)
            return new GridLength(1, GridUnitType.Star);

        return new GridLength(LocatorControlHelper.GetFieldWidth(FieldDef));
    }
    /// <summary>
    /// Builds the control UI.
    /// </summary>
    protected virtual void Build()
    {
        fRoot = new Grid();
        if (!Classes.Contains("LocatorBox"))
            Classes.Add("LocatorBox");
        fTextBoxes.Clear();
        fTextBoxMap.Clear();
        if (Locator == null || Locator.LocatorDef == null)
        {
            Content = fRoot;
            return;
        }
        List<LocatorFieldDef> Fields = Locator.LocatorDef.Fields
            .Where(item => item.IsVisible && !item.Name.IsSameText(Locator.LocatorDef.KeyField))
            .ToList();
        for (int Index = 0; Index < Fields.Count; Index++)
        {
            LocatorFieldDef FieldDef = Fields[Index];
            ColumnDefinition Column = new();
            Column.Width = GetFieldWidth(FieldDef, Index, Fields);
            fRoot.ColumnDefinitions.Add(Column);
            TextBox TextBox = CreateTextBox(FieldDef);
            Grid.SetColumn(TextBox, Index);
            fRoot.Children.Add(TextBox);
            fTextBoxes.Add(TextBox);
            fTextBoxMap[FieldDef] = TextBox;
        }
        fRoot.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(28)));
        fMenuButton = CreateMenuButton();
        Grid.SetColumn(fMenuButton, Fields.Count);
        fRoot.Children.Add(fMenuButton);
        CreatePopup();
        Content = fRoot;
    }
    /// <summary>
    /// Creates a textbox for a locator field.
    /// </summary>
    protected virtual TextBox CreateTextBox(LocatorFieldDef FieldDef)
    {
        TextBox Result = new();
        Result.Tag = FieldDef;
        Result.Watermark = FieldDef.Title;
        Result.IsReadOnly = IsReadOnly || !FieldDef.IsSearchable;
        Result.KeyDown += TextBox_KeyDown;
        Result.TextChanged += TextBox_TextChanged;
        return Result;
    }
    /// <summary>
    /// Creates the reference menu button.
    /// </summary>
    protected virtual Button CreateMenuButton()
    {
        Button Result = new();
        Result.Content = "...";
        Result.Width = 26;
        Result.MinWidth = 26;
        Result.Height = 32;
        Result.MinHeight = 32;
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.VerticalAlignment = VerticalAlignment.Stretch;
        Result.IsTabStop = false;
        Result.Classes.Add("LocatorBoxMenuButton");
        Result.Padding = new Thickness(0);
        Result.Margin = new Thickness(2, 0, 0, 0);
        return Result;
    }
    /// <summary>
    /// Creates the popup.
    /// </summary>
    protected virtual void CreatePopup()
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
        Grid.SetColumnSpan(fPopup, Math.Max(1, fRoot.ColumnDefinitions.Count));
        fRoot.Children.Add(fPopup);
    }
    /// <summary>
    /// Creates the popup grid columns.
    /// </summary>
    protected virtual void CreatePopupColumns()
    {
        fGrid.Columns.Clear();
        if (Locator == null || Locator.LocatorDef == null || Locator.SourceTable == null)
            return;

        foreach (LocatorFieldDef FieldDef in Locator.LocatorDef.Fields.Where(item => item.IsVisible && !item.Name.IsSameText(Locator.LocatorDef.KeyField)))
        {
            DataColumn Column = Locator.SourceTable.FindColumn(FieldDef.Alias);
            if (Column != null)
            {
                DataGridColumn GridColumn = DataGridBinder.CreateGridColumn(Column, IsReadOnly: true);
                GridColumn.Width = new DataGridLength(LocatorControlHelper.GetFieldWidth(FieldDef));
                fGrid.Columns.Add(GridColumn);
            }
        }
    }
    /// <summary>
    /// Opens the popup.
    /// </summary>
    protected virtual void OpenPopup(Control Target)
    {
        if (fPopup != null && fGrid != null && Locator != null)
        {
            fPopupItemsSource?.Dispose();
            fPopupItemsSource = new DataViewItemsSource(Locator.SourceTable.DataView);
            List<LocatorFieldDef> Fields = Locator.LocatorDef.Fields
                .Where(item => item.IsVisible && !item.Name.IsSameText(Locator.LocatorDef.KeyField))
                .ToList();
            double MinimumWidth = Bounds.Width > 0 ? Math.Clamp(Bounds.Width, 300, 800) : 300;
            fGrid.Width = LocatorControlHelper.GetPopupWidth(Fields, MinimumWidth);
            CreatePopupColumns();
            fGrid.ItemsSource = fPopupItemsSource;
            fGrid.SelectedIndex = fPopupItemsSource.Count > 0 ? 0 : -1;
            Point? Offset = Target?.TranslatePoint(new Point(0, 0), this);
            fPopup.PlacementTarget = this;
            fPopup.HorizontalOffset = Offset.HasValue ? Offset.Value.X : 0;
            fPopup.IsOpen = true;
            Ui.Post(() => fGrid.Focus());
        }
    }
    /// <summary>
    /// Closes the popup.
    /// </summary>
    protected virtual void ClosePopup()
    {
        if (fPopup != null)
            fPopup.IsOpen = false;
    }
    /// <summary>
    /// Assigns a source row to this control.
    /// </summary>
    protected virtual void AssignRow(DataRow Row)
    {
        if (Row == null || Locator == null || Locator.LocatorDef == null)
        {
            KeyValue = DBNull.Value;
            ClearTargetBoxes();
            RowSelected?.Invoke(this, new LocatorBoxRowEventArgs(null));
            return;
        }

        DataColumn KeyColumn = Row.Table.FindColumn(Locator.LocatorDef.KeyField);
        KeyValue = KeyColumn != null ? Row[KeyColumn] : DBNull.Value;
        RefreshTargetBoxes(Row);
        RowSelected?.Invoke(this, new LocatorBoxRowEventArgs(Row));
    }
    /// <summary>
    /// Returns a log-friendly search term.
    /// </summary>
    protected virtual string GetLogSearchTerm(string Term)
    {
        return !string.IsNullOrWhiteSpace(Term) ? Term.Trim().TrimEnd('?').Trim() : string.Empty;
    }
    /// <summary>
    /// Performs search using the specified textbox text.
    /// </summary>
    protected virtual void Search(TextBox TextBox)
    {
        if (TextBox == null || Locator == null)
            return;

        string Term = TextBox.Text;
        string LogTerm = GetLogSearchTerm(Term);
        LogBox.AppendLine($"Locator: Searching for term: {LogTerm}");
        try
        {
            LocatorSearchResult Result = Locator.Execute(Term);
            if (Result.TooManyRows)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator: Too many rows for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info(Result.Message, this));
            }
            else if (Result.IsEmpty)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator: No rows found for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info("No rows found.", this));
            }
            else if (Result.IsSingleRow)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator: Found 1 row for term: {LogTerm}");
                AssignRow(Result.SourceTable.Rows[0]);
            }
            else
            {
                LogBox.AppendLine($"Locator: Found {Result.RowCount} rows for term: {LogTerm}");
                OpenPopup(TextBox);
            }
        }
        catch (Exception e)
        {
            ClosePopup();
            LogBox.AppendLine($"Locator: {e.Message}");
            Ui.Post(async () => await MessageBox.Error(e, this));
        }
    }
    /// <summary>
    /// Selects the current popup row.
    /// </summary>
    protected virtual void SelectCurrentRow()
    {
        if (fGrid?.SelectedItem is DataRowView RowView)
            AssignRow(RowView.Row);
        ClosePopup();
    }
    /// <summary>
    /// Clears the current value.
    /// </summary>
    protected virtual void ClearValue()
    {
        AssignRow(null);
    }
    protected virtual void TextBox_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter && fPopup != null && fPopup.IsOpen)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            ClosePopup();
            Args.Handled = true;
        }
    }
    protected virtual void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        if (Sender is TextBox TextBox && Locator != null && Locator.ContainsSearchTrigger(TextBox.Text))
            Search(TextBox);
    }
    protected virtual void Grid_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            ClosePopup();
            Args.Handled = true;
        }
    }
    protected virtual void Grid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        SelectCurrentRow();
    }
    protected virtual void Grid_PreviewKeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
    }
    protected virtual void ItemZoom_Click(object Sender, RoutedEventArgs Args)
    {
        ZoomRequested?.Invoke(this, EventArgs.Empty);
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorBox()
    {
        Build();
    }

    // ● static public methods
    /// <summary>
    /// Locator property.
    /// </summary>
    static public readonly StyledProperty<Locator> LocatorProperty = AvaloniaProperty.Register<LocatorBox, Locator>(nameof(Locator));
    /// <summary>
    /// IsReadOnly property.
    /// </summary>
    static public readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<LocatorBox, bool>(nameof(IsReadOnly));
    /// <summary>
    /// KeyValue property.
    /// </summary>
    static public readonly StyledProperty<object> KeyValueProperty = AvaloniaProperty.Register<LocatorBox, object>(nameof(KeyValue));

    // ● public methods
    /// <summary>
    /// Rebuilds the control.
    /// </summary>
    public virtual void Rebuild()
    {
        Build();
    }
    /// <summary>
    /// Sets a textbox value.
    /// </summary>
    public virtual void SetTargetBoxValue(LocatorFieldDef FieldDef, object Value)
    {
        if (FieldDef != null && fTextBoxMap.TryGetValue(FieldDef, out TextBox TextBox))
            TextBox.Text = Sys.IsNull(Value) ? string.Empty : Value.ToString();
    }
    /// <summary>
    /// Clears all target textboxes.
    /// </summary>
    public virtual void ClearTargetBoxes()
    {
        foreach (TextBox TextBox in fTextBoxes)
            TextBox.Text = string.Empty;
    }
    /// <summary>
    /// Refreshes all target textboxes from a row.
    /// </summary>
    public virtual void RefreshTargetBoxes(DataRow Row) => RefreshTargetBoxes(Row, null);
    /// <summary>
    /// Refreshes all target textboxes from a row using an optional target field map.
    /// </summary>
    public virtual void RefreshTargetBoxes(DataRow Row, Dictionary<string, string> TargetFieldMap)
    {
        if (Row == null || Locator == null || Locator.LocatorDef == null)
        {
            ClearTargetBoxes();
            return;
        }
        foreach (LocatorFieldDef FieldDef in Locator.LocatorDef.Fields)
        {
            if (!FieldDef.IsVisible)
                continue;

            string TargetField = null;
            if (TargetFieldMap != null)
            {
                if (!TargetFieldMap.TryGetValue(FieldDef.Name, out TargetField))
                    TargetFieldMap.TryGetValue(FieldDef.Alias, out TargetField);
            }
            if (string.IsNullOrWhiteSpace(TargetField))
                TargetField = FieldDef.TargetField;
            if (string.IsNullOrWhiteSpace(TargetField))
                continue;

            DataColumn Column = Row.Table.FindColumn(TargetField);
            object Value = Column != null ? Row[Column] : DBNull.Value;
            if (Column == null && !Sys.IsNull(KeyValue) && Locator.LocateByKey(KeyValue))
                Value = Locator.SelectedRow != null && Locator.SelectedRow.Table.Columns.Contains(FieldDef.Name)
                    ? Locator.SelectedRow[FieldDef.Name]
                    : DBNull.Value;
            SetTargetBoxValue(FieldDef, Value);
        }
    }

    // ● properties
    /// <summary>
    /// The locator.
    /// </summary>
    public Locator Locator
    {
        get => GetValue(LocatorProperty);
        set => SetValue(LocatorProperty, value);
    }
    /// <summary>
    /// True when this control is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
    /// <summary>
    /// The selected key value.
    /// </summary>
    public object KeyValue
    {
        get => GetValue(KeyValueProperty);
        set => SetValue(KeyValueProperty, value);
    }
    /// <summary>
    /// The reference menu button.
    /// </summary>
    public Button MenuButton => fMenuButton;

    // ● events
    /// <summary>
    /// Occurs when zoom is requested.
    /// </summary>
    public event EventHandler ZoomRequested;
    /// <summary>
    /// Occurs when a locator source row is selected or cleared.
    /// </summary>
    public event EventHandler<LocatorBoxRowEventArgs> RowSelected;

    // ● static constructor
    /// <summary>
    /// Static constructor.
    /// </summary>
    static LocatorBox()
    {
        LocatorProperty.Changed.AddClassHandler<LocatorBox>((Sender, Args) => Sender.Rebuild());
        IsReadOnlyProperty.Changed.AddClassHandler<LocatorBox>((Sender, Args) => Sender.ApplyReadOnly());
    }
}

/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// A composite locator editor based on Locator2.
/// </summary>
public class LocatorBox2: UserControl
{
    // ● private fields
    /// <summary>
    /// The root panel.
    /// </summary>
    protected Grid fRoot;
    /// <summary>
    /// The result popup.
    /// </summary>
    protected Popup fPopup;
    /// <summary>
    /// The popup border.
    /// </summary>
    protected Border fPopupBorder;
    /// <summary>
    /// The popup result grid.
    /// </summary>
    protected GroupGrid fGrid;
    /// <summary>
    /// The context menu button.
    /// </summary>
    protected Button fMenuButton;
    /// <summary>
    /// The display text boxes.
    /// </summary>
    protected List<TextBox> fTextBoxes = [];
    /// <summary>
    /// The display text boxes by field name.
    /// </summary>
    protected Dictionary<string, TextBox> fTextBoxMap = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// The last locator result.
    /// </summary>
    protected LocatorResult fLastResult;

    // ● protected methods
    /// <summary>
    /// Returns true when a field can be used as a search field.
    /// </summary>
    protected virtual bool IsSearchField(string FieldName) => LocatorDef != null && LocatorDef.GetAllSearchFields().Any(item => item.IsSameText(FieldName));
    /// <summary>
    /// Returns true when a term contains the locator search trigger.
    /// </summary>
    protected virtual bool ContainsSearchTrigger(string Term) => !string.IsNullOrWhiteSpace(Term) && Term.TrimEnd().EndsWith("?");
    /// <summary>
    /// Returns a normalized search term without the trigger.
    /// </summary>
    protected virtual string GetLogSearchTerm(string Term) => !string.IsNullOrWhiteSpace(Term) ? Term.Trim().TrimEnd('?').Trim() : string.Empty;
    /// <summary>
    /// Applies the read-only state to child editors.
    /// </summary>
    protected virtual void ApplyReadOnly()
    {
        foreach (KeyValuePair<string, TextBox> Entry in fTextBoxMap)
            Entry.Value.IsReadOnly = IsReadOnly || !IsSearchField(Entry.Key);
    }
    /// <summary>
    /// Returns the display width for a locator field.
    /// </summary>
    protected virtual GridLength GetFieldWidth(string FieldName, int Index, List<string> Fields)
    {
        bool IsLast = Index == Fields.Count - 1;
        return IsLast ? new GridLength(1, GridUnitType.Star) : new GridLength(120);
    }
    /// <summary>
    /// Returns the locator fields displayed by this control.
    /// </summary>
    protected virtual List<string> GetDisplayFields()
    {
        if (LocatorDef == null)
            return [];

        return LocatorDef.GetResultFields()
            .Where(item => !item.IsSameText(LocatorDef.KeyField))
            .ToList();
    }
    /// <summary>
    /// Builds the visual tree.
    /// </summary>
    protected virtual void Build()
    {
        fRoot = new Grid();
        if (!Classes.Contains("LocatorBox"))
            Classes.Add("LocatorBox");
        fTextBoxes.Clear();
        fTextBoxMap.Clear();
        if (LocatorDef == null)
        {
            Content = fRoot;
            return;
        }

        List<string> Fields = GetDisplayFields();
        for (int Index = 0; Index < Fields.Count; Index++)
        {
            string FieldName = Fields[Index];
            ColumnDefinition Column = new();
            Column.Width = GetFieldWidth(FieldName, Index, Fields);
            fRoot.ColumnDefinitions.Add(Column);
            TextBox TextBox = CreateTextBox(FieldName);
            Grid.SetColumn(TextBox, Index);
            fRoot.Children.Add(TextBox);
            fTextBoxes.Add(TextBox);
            fTextBoxMap[FieldName] = TextBox;
        }

        fRoot.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(28)));
        fMenuButton = CreateMenuButton();
        Grid.SetColumn(fMenuButton, Fields.Count);
        fRoot.Children.Add(fMenuButton);
        CreatePopup();
        Content = fRoot;
    }
    /// <summary>
    /// Creates a field text box.
    /// </summary>
    protected virtual TextBox CreateTextBox(string FieldName)
    {
        TextBox Result = new();
        Result.Tag = FieldName;
        Result.PlaceholderText = FieldName;
        Result.IsReadOnly = IsReadOnly || !IsSearchField(FieldName);
        Result.KeyDown += TextBox_KeyDown;
        Result.TextChanged += TextBox_TextChanged;
        return Result;
    }
    /// <summary>
    /// Creates the context menu button.
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
    /// Creates the popup and result grid.
    /// </summary>
    protected virtual void CreatePopup()
    {
        fGrid = new GroupGrid()
        {
            AutoGenerateColumns = false,
            IsReadOnly = true,
            MinWidth = 300,
            MaxHeight = 260,
            IsToolBarVisible = false,
            IsGroupPanelVisible = false,
            IsColumnHeadersVisible = true,
            IsFilterPanelVisible = false,
            IsTotalsSummaryVisible = false,
            IsColumnManagerMenuItemVisible = false,
            IsSettingsMenuItemsVisible = false,
        };
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
    /// Creates popup grid columns.
    /// </summary>
    protected virtual void CreatePopupColumns(DataTable Table)
    {
        fGrid.Columns.Clear();
        if (LocatorDef == null || Table == null)
            return;

        foreach (string FieldName in GetDisplayFields())
        {
            DataColumn Column = Table.FindColumn(FieldName);
            if (Column != null)
                fGrid.Columns.Add(GroupGridBinder.CreateGridColumn(Column, IsReadOnly: true));
        }
    }
    /// <summary>
    /// Returns the popup grid height.
    /// </summary>
    protected virtual double GetPopupGridHeight(DataView DataView)
    {
        if (fGrid == null || DataView == null)
            return 0;

        double HeaderHeight = fGrid.IsColumnHeadersVisible ? fGrid.LayoutMetrics.ColumnHeaderHeight : 0;
        double RowHeight = fGrid.LayoutMetrics.RowHeight;
        double BodyHeight = Math.Max(1, Math.Min(DataView.Count, 8)) * RowHeight;
        double ScrollHeight = DataView.Count > 8 ? fGrid.LayoutMetrics.HorizontalScrollBarHeight : 0;
        return Math.Clamp(HeaderHeight + BodyHeight + ScrollHeight + 2, 80, 260);
    }
    /// <summary>
    /// Opens the result popup.
    /// </summary>
    protected virtual void OpenPopup(Control Target)
    {
        DataTable Table = fLastResult?.Table;
        if (fPopup != null && fGrid != null && Table != null)
        {
            DataView DataView = Table.DefaultView;
            fGrid.Width = Bounds.Width > 0 ? Math.Clamp(Bounds.Width, 300, 800) : 300;
            CreatePopupColumns(Table);
            fGrid.ItemsSource = DataView;
            fGrid.Height = GetPopupGridHeight(DataView);
            fPopupBorder.Height = fGrid.Height;
            if (DataView.Count > 0)
                GroupGridBinder.SelectRow(fGrid, 0);
            fGrid.BestFitColumns();
            Point? Offset = Target?.TranslatePoint(new Point(0, 0), this);
            fPopup.PlacementTarget = this;
            fPopup.HorizontalOffset = Offset.HasValue ? Offset.Value.X : 0;
            fPopup.IsOpen = true;
            Ui.Post(() => fGrid.Focus());
        }
    }
    /// <summary>
    /// Closes the result popup.
    /// </summary>
    protected virtual void ClosePopup()
    {
        if (fPopup != null)
            fPopup.IsOpen = false;
    }
    /// <summary>
    /// Creates a locator request.
    /// </summary>
    protected virtual LocatorRequest CreateRequest(TextBox TextBox)
    {
        string FieldName = TextBox?.Tag as string;
        LocatorRequest Result = new()
        {
            Context = new LocatorContext(LocatorDef?.Name),
            SearchField = FieldName,
            SearchTerm = GetLogSearchTerm(TextBox?.Text),
            IsMultiRow = true,
        };
        DataRow Row = ContextRowProvider?.CurrentRow;
        if (Row != null)
        {
            Result.Context.Params["Row"] = Row;
            Result.Context.Params["DataRow"] = Row;
        }
        return Result;
    }
    /// <summary>
    /// Assigns a selected source row.
    /// </summary>
    protected virtual void AssignRow(DataRow Row)
    {
        if (Row == null || LocatorDef == null)
        {
            KeyValue = DBNull.Value;
            ClearTargetBoxes();
            RowSelected?.Invoke(this, new LocatorBoxRowEventArgs(null));
            return;
        }

        DataColumn KeyColumn = Row.Table.FindColumn(LocatorDef.KeyField);
        KeyValue = KeyColumn != null ? Row[KeyColumn] : DBNull.Value;
        RefreshTargetBoxes(Row);
        RowSelected?.Invoke(this, new LocatorBoxRowEventArgs(Row));
    }
    /// <summary>
    /// Executes locator search.
    /// </summary>
    protected virtual void Search(TextBox TextBox)
    {
        if (TextBox == null || LocatorDef == null)
            return;

        string LogTerm = GetLogSearchTerm(TextBox.Text);
        LogBox.AppendLine($"Locator2: Searching for term: {LogTerm}");
        try
        {
            fLastResult = Locators.Execute(CreateRequest(TextBox));
            if (fLastResult.HasTooManyResults)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator2: Too many rows for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info(fLastResult.Message, this));
            }
            else if (fLastResult.Status == LocatorResultStatus.NoResult)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator2: No rows found for term: {LogTerm}");
                Ui.Post(async () => await MessageBox.Info("No rows found.", this));
            }
            else if (fLastResult.HasSingleResult)
            {
                ClosePopup();
                LogBox.AppendLine($"Locator2: Found 1 row for term: {LogTerm}");
                AssignRow(fLastResult.Table.Rows[0]);
            }
            else
            {
                LogBox.AppendLine($"Locator2: Found {fLastResult.Count} rows for term: {LogTerm}");
                OpenPopup(TextBox);
            }
        }
        catch (Exception e)
        {
            ClosePopup();
            LogBox.AppendLine($"Locator2: {e.Message}");
            Ui.Post(async () => await MessageBox.Error(e, this));
        }
    }
    /// <summary>
    /// Selects the current popup row.
    /// </summary>
    protected virtual void SelectCurrentRow()
    {
        if (fGrid?.CurrentRow is DataRowView RowView)
            AssignRow(RowView.Row);
        ClosePopup();
    }
    /// <summary>
    /// Handles text box key down.
    /// </summary>
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
    /// <summary>
    /// Handles text changes.
    /// </summary>
    protected virtual void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        if (Sender is TextBox TextBox && ContainsSearchTrigger(TextBox.Text))
            Search(TextBox);
    }
    /// <summary>
    /// Handles result grid key down.
    /// </summary>
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
    /// <summary>
    /// Handles result grid double tap.
    /// </summary>
    protected virtual void Grid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        SelectCurrentRow();
    }
    /// <summary>
    /// Handles result grid preview key down.
    /// </summary>
    protected virtual void Grid_PreviewKeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorBox2()
    {
        Build();
    }

    // ● static public methods
    /// <summary>
    /// LocatorDef property.
    /// </summary>
    static public readonly StyledProperty<LocatorDef> LocatorDefProperty = AvaloniaProperty.Register<LocatorBox2, LocatorDef>(nameof(LocatorDef));
    /// <summary>
    /// IsReadOnly property.
    /// </summary>
    static public readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<LocatorBox2, bool>(nameof(IsReadOnly));
    /// <summary>
    /// KeyValue property.
    /// </summary>
    static public readonly StyledProperty<object> KeyValueProperty = AvaloniaProperty.Register<LocatorBox2, object>(nameof(KeyValue));

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
    public virtual void SetTargetBoxValue(string FieldName, object Value)
    {
        if (!string.IsNullOrWhiteSpace(FieldName) && fTextBoxMap.TryGetValue(FieldName, out TextBox TextBox))
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
    public virtual void RefreshTargetBoxes(DataRow Row)
    {
        if (Row == null || LocatorDef == null)
        {
            ClearTargetBoxes();
            return;
        }
        foreach (string FieldName in GetDisplayFields())
        {
            DataColumn Column = Row.Table.FindColumn(FieldName);
            object Value = Column != null ? Row[Column] : DBNull.Value;
            SetTargetBoxValue(FieldName, Value);
        }
    }
    /// <summary>
    /// Refreshes all target textboxes from a target row and mapping plan.
    /// </summary>
    public virtual void RefreshTargetBoxes(DataRow Row, LocatorMapPlan Plan)
    {
        if (Row == null || Plan == null)
        {
            ClearTargetBoxes();
            return;
        }

        foreach (LocatorMapItem Item in Plan.Items)
        {
            if (Item.SourceField.IsSameText(LocatorDef?.KeyField))
                continue;

            DataColumn Column = Row.Table.FindColumn(Item.TargetField);
            object Value = Column != null ? Row[Column] : DBNull.Value;
            SetTargetBoxValue(Item.SourceField, Value);
        }
    }

    // ● properties
    /// <summary>
    /// The locator definition.
    /// </summary>
    public LocatorDef LocatorDef
    {
        get => GetValue(LocatorDefProperty);
        set => SetValue(LocatorDefProperty, value);
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
    /// Provides the current row context.
    /// </summary>
    public IRowProvider ContextRowProvider { get; set; }
    /// <summary>
    /// The reference menu button.
    /// </summary>
    public Button MenuButton => fMenuButton;

    // ● events
    /// <summary>
    /// Occurs when a locator source row is selected or cleared.
    /// </summary>
    public event EventHandler<LocatorBoxRowEventArgs> RowSelected;

    // ● static constructor
    /// <summary>
    /// Static constructor.
    /// </summary>
    static LocatorBox2()
    {
        LocatorDefProperty.Changed.AddClassHandler<LocatorBox2>((Sender, Args) => Sender.Rebuild());
        IsReadOnlyProperty.Changed.AddClassHandler<LocatorBox2>((Sender, Args) => Sender.ApplyReadOnly());
    }
}

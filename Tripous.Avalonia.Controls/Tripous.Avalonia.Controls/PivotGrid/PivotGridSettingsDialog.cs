// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a layout settings dialog for pivot grid fields.
/// </summary>
public class PivotGridSettingsDialog: Window
{
    // ● private fields
    readonly List<PivotGridFieldSettingsItem> fItems;
    readonly ListBox lboAvailable;
    readonly ListBox lboRows;
    readonly ListBox lboColumns;
    readonly ListBox lboValues;
    readonly ComboBox cboAggregate;
    readonly ComboBox cboDisplayFormat;
    readonly TextBox edtWidth;
    readonly CheckBox chkShowFieldPanel;
    readonly CheckBox chkShowRowGrandTotals;
    readonly CheckBox chkShowColumnGrandTotals;
    readonly CheckBox chkShowToolTips;
    readonly Button btnRows;
    readonly Button btnColumns;
    readonly Button btnValues;
    readonly Button btnAvailable;
    readonly Button btnUp;
    readonly Button btnDown;
    readonly Button btnOk;
    readonly Button btnCancel;
    bool fIsUpdatingAggregateEditor;
    bool fIsUpdatingValueEditor;

    // ● private methods
    string GetItemText(PivotGridFieldSettingsItem Item)
    {
        if (Item == null)
            return string.Empty;

        string Text = string.IsNullOrWhiteSpace(Item.Header) ? Item.Name : Item.Header;
        return Item.Role == PivotGridFieldRole.Measure ? $"{Text} ({Item.AggregateKind})" : Text;
    }
    ListBox GetRoleListBox(PivotGridFieldRole Role)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                return lboRows;
            case PivotGridFieldRole.Column:
                return lboColumns;
            case PivotGridFieldRole.Measure:
                return lboValues;
        }

        return lboAvailable;
    }
    PivotGridFieldSettingsItem GetSelectedItem(out ListBox SourceListBox)
    {
        foreach (ListBox ListBox in new[] { lboAvailable, lboRows, lboColumns, lboValues })
            if (ListBox.SelectedItem is ListBoxItem Item)
            {
                SourceListBox = ListBox;
                return Item.Tag as PivotGridFieldSettingsItem;
            }

        SourceListBox = null;
        return null;
    }
    ListBoxItem CreateListBoxItem(PivotGridFieldSettingsItem Item)
    {
        return new ListBoxItem
        {
            Content = GetItemText(Item),
            Tag = Item,
        };
    }
    void RefreshList(ListBox ListBox, PivotGridFieldRole Role, PivotGridFieldSettingsItem SelectedItem)
    {
        ListBox.Items.Clear();
        foreach (PivotGridFieldSettingsItem Item in fItems.Where(Item => Item.Role == Role))
        {
            ListBoxItem ListBoxItem = CreateListBoxItem(Item);
            ListBox.Items.Add(ListBoxItem);
            if (ReferenceEquals(Item, SelectedItem))
                ListBox.SelectedItem = ListBoxItem;
        }
    }
    void Refresh(PivotGridFieldSettingsItem SelectedItem = null)
    {
        RefreshList(lboAvailable, PivotGridFieldRole.Available, SelectedItem);
        RefreshList(lboRows, PivotGridFieldRole.Row, SelectedItem);
        RefreshList(lboColumns, PivotGridFieldRole.Column, SelectedItem);
        RefreshList(lboValues, PivotGridFieldRole.Measure, SelectedItem);
        UpdateValueEditor();
    }
    bool CanMoveToRole(PivotGridFieldSettingsItem Item, PivotGridFieldRole Role)
    {
        if (Item == null)
            return false;
        if (Role == PivotGridFieldRole.Row || Role == PivotGridFieldRole.Column)
            return Item.CanUseAsAxis;
        if (Role == PivotGridFieldRole.Measure)
            return Item.CanUseAsMeasure;

        return true;
    }
    void MoveSelectedTo(PivotGridFieldRole Role)
    {
        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        if (!CanMoveToRole(Item, Role))
            return;

        fItems.Remove(Item);
        Item.Role = Role;
        fItems.Add(Item);
        Refresh(Item);
        GetRoleListBox(Role).Focus();
    }
    void MoveSelected(int Delta)
    {
        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        if (Item == null || ListBox == null)
            return;

        List<PivotGridFieldSettingsItem> Items = fItems.Where(Candidate => Candidate.Role == Item.Role).ToList();
        int OldIndex = Items.IndexOf(Item);
        int NewIndex = OldIndex + Delta;
        if (OldIndex < 0 || NewIndex < 0 || NewIndex >= Items.Count)
            return;

        PivotGridFieldSettingsItem Neighbor = Items[NewIndex];
        int ItemIndex = fItems.IndexOf(Item);
        int NeighborIndex = fItems.IndexOf(Neighbor);
        fItems.RemoveAt(ItemIndex);
        fItems.Insert(NeighborIndex, Item);
        Refresh(Item);
        ListBox.Focus();
    }
    void ClearSelections(object Sender)
    {
        foreach (ListBox ListBox in new[] { lboAvailable, lboRows, lboColumns, lboValues })
            if (!ReferenceEquals(ListBox, Sender))
                ListBox.SelectedItem = null;
    }
    void UpdateValueEditor()
    {
        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        bool IsMeasure = Item != null && ReferenceEquals(ListBox, lboValues);
        fIsUpdatingAggregateEditor = true;
        fIsUpdatingValueEditor = true;
        cboAggregate.IsEnabled = IsMeasure;
        cboAggregate.SelectedItem = IsMeasure ? Item.AggregateKind : null;
        cboDisplayFormat.IsEnabled = IsMeasure;
        cboDisplayFormat.Text = IsMeasure ? Item.DisplayFormat : string.Empty;
        edtWidth.IsEnabled = IsMeasure;
        edtWidth.Text = IsMeasure && Item.Width > 0 ? Item.Width.ToString(CultureInfo.CurrentCulture) : string.Empty;
        fIsUpdatingAggregateEditor = false;
        fIsUpdatingValueEditor = false;
    }
    void ListBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        ClearSelections(Sender);
        UpdateValueEditor();
    }
    void AggregateComboBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (fIsUpdatingAggregateEditor)
            return;

        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        if (Item == null || !ReferenceEquals(ListBox, lboValues) || cboAggregate.SelectedItem is not PivotGridAggregateKind AggregateKind)
            return;

        Item.AggregateKind = AggregateKind;
        if (ListBox.SelectedItem is ListBoxItem ListBoxItem)
            ListBoxItem.Content = GetItemText(Item);
    }
    void DisplayFormat_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (fIsUpdatingValueEditor)
            return;

        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        if (Item == null || !ReferenceEquals(ListBox, lboValues))
            return;

        Item.DisplayFormat = cboDisplayFormat.Text ?? string.Empty;
    }
    void DisplayFormat_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        DisplayFormat_SelectionChanged(Sender, null);
    }
    void DisplayFormat_PropertyChanged(object Sender, AvaloniaPropertyChangedEventArgs Args)
    {
        if (Args.Property == ComboBox.TextProperty)
            DisplayFormat_SelectionChanged(Sender, null);
    }
    void Width_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        if (fIsUpdatingValueEditor)
            return;

        PivotGridFieldSettingsItem Item = GetSelectedItem(out ListBox ListBox);
        if (Item == null || !ReferenceEquals(ListBox, lboValues))
            return;

        if (double.TryParse(edtWidth.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out double Width))
            Item.Width = Math.Max(0, Width);
        else if (string.IsNullOrWhiteSpace(edtWidth.Text))
            Item.Width = 0;
    }
    void RoleListBox_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        if (ReferenceEquals(Sender, lboRows) || ReferenceEquals(Sender, lboColumns) || ReferenceEquals(Sender, lboValues))
            MoveSelectedTo(PivotGridFieldRole.Available);
    }
    Border CreateRolePanel(string Header, ListBox ListBox)
    {
        TextBlock HeaderTextBlock = new()
        {
            Text = Header,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(8, 6),
        };
        DockPanel.SetDock(HeaderTextBlock, Dock.Top);
        return new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.LightGray,
            Margin = new Thickness(0),
            Child = new DockPanel
            {
                Children =
                {
                    HeaderTextBlock,
                    ListBox,
                },
            },
        };
    }
    StackPanel CreateCommandPanel()
    {
        return new StackPanel
        {
            Width = 130,
            Spacing = 7,
            Margin = new Thickness(8, 0),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Children =
            {
                btnRows,
                btnColumns,
                btnValues,
                btnAvailable,
                new Separator(),
                new TextBlock
                {
                    Text = "Aggregate",
                    FontWeight = FontWeight.SemiBold,
                },
                cboAggregate,
                new TextBlock
                {
                    Text = "Format",
                    FontWeight = FontWeight.SemiBold,
                },
                cboDisplayFormat,
                new TextBlock
                {
                    Text = "Width",
                    FontWeight = FontWeight.SemiBold,
                },
                edtWidth,
                new Separator(),
                btnUp,
                btnDown,
            },
        };
    }
    Grid CreateListsPanel()
    {
        Grid Result = new()
        {
            Margin = new Thickness(12),
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
            },
        };
        Control Available = CreateRolePanel("Available", lboAvailable);
        Control Commands = CreateCommandPanel();
        Control Rows = CreateRolePanel("Rows", lboRows);
        Control Columns = CreateRolePanel("Columns", lboColumns);
        Control Values = CreateRolePanel("Values", lboValues);
        Grid.SetColumn(Available, 0);
        Grid.SetColumn(Commands, 1);
        Grid.SetColumn(Rows, 2);
        Grid.SetColumn(Columns, 3);
        Grid.SetColumn(Values, 4);
        Result.Children.Add(Available);
        Result.Children.Add(Commands);
        Result.Children.Add(Rows);
        Result.Children.Add(Columns);
        Result.Children.Add(Values);
        return Result;
    }
    DockPanel CreateButtonPanel()
    {
        StackPanel TotalsPanel = new()
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Children =
            {
                chkShowFieldPanel,
                chkShowRowGrandTotals,
                chkShowColumnGrandTotals,
                chkShowToolTips,
            },
        };
        StackPanel ButtonsPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children =
            {
                btnCancel,
                btnOk,
            },
        };
        DockPanel.SetDock(ButtonsPanel, Dock.Right);
        return new DockPanel
        {
            LastChildFill = true,
            Margin = new Thickness(12),
            Children =
            {
                ButtonsPanel,
                TotalsPanel,
            },
        };
    }
    Control CreateContent()
    {
        DockPanel Result = new()
        {
            LastChildFill = true,
            Children =
            {
                CreateButtonPanel(),
                CreateListsPanel(),
            },
        };
        DockPanel.SetDock(Result.Children[0], Dock.Bottom);
        return Result;
    }
    void Button_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        if (ReferenceEquals(Sender, btnRows))
            MoveSelectedTo(PivotGridFieldRole.Row);
        else if (ReferenceEquals(Sender, btnColumns))
            MoveSelectedTo(PivotGridFieldRole.Column);
        else if (ReferenceEquals(Sender, btnValues))
            MoveSelectedTo(PivotGridFieldRole.Measure);
        else if (ReferenceEquals(Sender, btnAvailable))
            MoveSelectedTo(PivotGridFieldRole.Available);
        else if (ReferenceEquals(Sender, btnUp))
            MoveSelected(-1);
        else if (ReferenceEquals(Sender, btnDown))
            MoveSelected(1);
    }
    void Ok_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Close(true);
    }
    void Cancel_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Close(false);
    }

    // ● protected methods
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs Args)
    {
        base.OnKeyDown(Args);

        if (Args.Key == Key.Escape)
        {
            Close(false);
            Args.Handled = true;
        }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridSettingsDialog"/> class.
    /// </summary>
    /// <param name="Items">The editable field settings items.</param>
    /// <param name="ShowFieldPanel">True to display the top field panel.</param>
    /// <param name="ShowRowGrandTotals">True to display row grand totals as a total column.</param>
    /// <param name="ShowColumnGrandTotals">True to display column grand totals as a total row.</param>
    /// <param name="ShowToolTips">True to display hover tooltips.</param>
    public PivotGridSettingsDialog(IEnumerable<PivotGridFieldSettingsItem> Items, bool ShowFieldPanel, bool ShowRowGrandTotals, bool ShowColumnGrandTotals, bool ShowToolTips)
    {
        fItems = Items == null ? new List<PivotGridFieldSettingsItem>() : Items.ToList();
        Title = "Pivot Grid Settings";
        Width = 780;
        Height = 560;
        MinWidth = 760;
        MinHeight = 500;
        CanMinimize = false;
        CanMaximize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        lboAvailable = new ListBox();
        lboRows = new ListBox();
        lboColumns = new ListBox();
        lboValues = new ListBox();
        foreach (ListBox ListBox in new[] { lboAvailable, lboRows, lboColumns, lboValues })
            ListBox.SelectionChanged += ListBox_SelectionChanged;
        foreach (ListBox ListBox in new[] { lboRows, lboColumns, lboValues })
            ListBox.DoubleTapped += RoleListBox_DoubleTapped;
        cboAggregate = new ComboBox
        {
            ItemsSource = Enum.GetValues(typeof(PivotGridAggregateKind)).Cast<PivotGridAggregateKind>().ToList(),
            IsEnabled = false,
            MinWidth = 100,
        };
        cboDisplayFormat = new ComboBox
        {
            ItemsSource = new List<string> { string.Empty, "N0", "N2", "N4", "C0", "C2", "P0", "P2", "0", "0.00", "#,##0", "#,##0.00" },
            IsEnabled = false,
            IsEditable = true,
            PlaceholderText = "Format",
            MinWidth = 100,
        };
        edtWidth = new TextBox
        {
            IsEnabled = false,
            MinWidth = 100,
        };
        chkShowFieldPanel = new CheckBox
        {
            Content = "Field panel",
            IsChecked = ShowFieldPanel,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        chkShowRowGrandTotals = new CheckBox
        {
            Content = "Row totals",
            IsChecked = ShowRowGrandTotals,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        chkShowColumnGrandTotals = new CheckBox
        {
            Content = "Column totals",
            IsChecked = ShowColumnGrandTotals,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        chkShowToolTips = new CheckBox
        {
            Content = "Tooltips",
            IsChecked = ShowToolTips,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        btnRows = new Button { Content = "Rows", MinWidth = 120 };
        btnColumns = new Button { Content = "Columns", MinWidth = 120 };
        btnValues = new Button { Content = "Values", MinWidth = 120 };
        btnAvailable = new Button { Content = "Available", MinWidth = 120 };
        btnUp = new Button { Content = "Up", MinWidth = 120 };
        btnDown = new Button { Content = "Down", MinWidth = 120 };
        btnOk = new Button { Content = "OK", MinWidth = 80 };
        btnCancel = new Button { Content = "Cancel", MinWidth = 80 };
        foreach (Button Button in new[] { btnRows, btnColumns, btnValues, btnAvailable, btnUp, btnDown })
            Button.Click += Button_Click;
        cboAggregate.SelectionChanged += AggregateComboBox_SelectionChanged;
        cboDisplayFormat.SelectionChanged += DisplayFormat_SelectionChanged;
        cboDisplayFormat.PropertyChanged += DisplayFormat_PropertyChanged;
        edtWidth.TextChanged += Width_TextChanged;
        btnOk.Click += Ok_Click;
        btnCancel.Click += Cancel_Click;
        Content = CreateContent();
        Refresh();
    }

    // ● properties
    /// <summary>
    /// Gets the edited field settings items.
    /// </summary>
    public IReadOnlyList<PivotGridFieldSettingsItem> Items => fItems;
    /// <summary>
    /// Gets a value indicating whether the top field panel should be displayed.
    /// </summary>
    public bool ShowFieldPanel => chkShowFieldPanel.IsChecked == true;
    /// <summary>
    /// Gets a value indicating whether row grand totals should be displayed as a total column.
    /// </summary>
    public bool ShowRowGrandTotals => chkShowRowGrandTotals.IsChecked == true;
    /// <summary>
    /// Gets a value indicating whether column grand totals should be displayed as a total row.
    /// </summary>
    public bool ShowColumnGrandTotals => chkShowColumnGrandTotals.IsChecked == true;
    /// <summary>
    /// Gets a value indicating whether hover tooltips should be displayed.
    /// </summary>
    public bool ShowToolTips => chkShowToolTips.IsChecked == true;
}

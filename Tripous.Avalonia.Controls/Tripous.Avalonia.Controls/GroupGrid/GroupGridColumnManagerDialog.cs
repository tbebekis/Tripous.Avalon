// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the default column manager dialog for a <see cref="GroupGrid"/>.
/// </summary>
public class GroupGridColumnManagerDialog: Window
{
    // ● private fields
    const double RowsViewportHeight = 400;
    const double ListHeight = 380;
    const double ListWidth = 155;
    readonly Dictionary<string, GroupGridColumn> fSourceColumns;
    readonly List<GroupGridColumnSettings> fVisibleColumns = new();
    readonly List<GroupGridColumnSettings> fHiddenColumns = new();
    readonly List<GroupGridColumnSettings> fGroupedColumns = new();
    readonly List<GroupGridColumnSettings> fAvailableGroupColumns = new();
    readonly ListBox fVisibleListBox = new();
    readonly ListBox fHiddenListBox = new();
    readonly ListBox fGroupedListBox = new();
    readonly ListBox fAvailableGroupListBox = new();
    readonly TextBox fNameTextBox = new();

    // ● private methods
    GroupGridColumnSettings CloneColumnSettings(GroupGridColumnSettings Source)
    {
        return new GroupGridColumnSettings
        {
            Name = Source.Name,
            Header = Source.Header,
            IsVisible = Source.IsVisible,
            VisibleIndex = Source.VisibleIndex,
            GroupIndex = Source.GroupIndex,
            FilterText = Source.FilterText,
            GroupSummary = Source.GroupSummary,
            TotalSummary = Source.TotalSummary,
        };
    }
    GroupGridSettings CloneSettings(GroupGridSettings Source)
    {
        GroupGridSettings Result = new()
        {
            Name = string.IsNullOrWhiteSpace(Source?.Name) ? "Default" : Source.Name,
        };
        if (Source?.Columns != null)
            Result.Columns = Source.Columns.Select(CloneColumnSettings).ToList();

        return Result;
    }
    string GetColumnText(GroupGridColumnSettings Column)
    {
        return string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header;
    }
    GroupGridColumnSettings GetSelectedColumn(ListBox ListBox)
    {
        return ListBox.SelectedItem is ListBoxItem Item ? Item.Tag as GroupGridColumnSettings : null;
    }
    ListBoxItem CreateColumnItem(GroupGridColumnSettings Column)
    {
        return new ListBoxItem
        {
            Content = GetColumnText(Column),
            Tag = Column,
        };
    }
    void RefreshList(ListBox ListBox, IEnumerable<GroupGridColumnSettings> Columns, GroupGridColumnSettings SelectedColumn = null)
    {
        ListBox.Items.Clear();
        foreach (GroupGridColumnSettings Column in Columns)
        {
            ListBoxItem Item = CreateColumnItem(Column);
            ListBox.Items.Add(Item);
            if (ReferenceEquals(Column, SelectedColumn))
                ListBox.SelectedItem = Item;
        }
    }
    void RefreshVisibleTab(GroupGridColumnSettings SelectedColumn = null)
    {
        RefreshList(fVisibleListBox, fVisibleColumns, SelectedColumn);
        RefreshList(fHiddenListBox, fHiddenColumns, SelectedColumn);
    }
    void RefreshGroupTab(GroupGridColumnSettings SelectedColumn = null)
    {
        RefreshList(fGroupedListBox, fGroupedColumns, SelectedColumn);
        RefreshList(fAvailableGroupListBox, fAvailableGroupColumns, SelectedColumn);
    }
    void MoveBetweenLists(List<GroupGridColumnSettings> Source, List<GroupGridColumnSettings> Target, ListBox SourceListBox, bool IsVisible)
    {
        GroupGridColumnSettings Column = GetSelectedColumn(SourceListBox);
        if (Column == null || !Source.Remove(Column))
            return;

        Column.IsVisible = IsVisible;
        Target.Add(Column);
        if (!IsVisible)
        {
            fGroupedColumns.Remove(Column);
            if (!fAvailableGroupColumns.Contains(Column))
                fAvailableGroupColumns.Add(Column);
        }

        RefreshVisibleTab(Column);
        RefreshGroupTab(Column);
    }
    void MoveItem(List<GroupGridColumnSettings> List, ListBox ListBox, int Delta)
    {
        GroupGridColumnSettings Column = GetSelectedColumn(ListBox);
        if (Column == null)
            return;

        int Index = List.IndexOf(Column);
        int NewIndex = Index + Delta;
        if (Index < 0 || NewIndex < 0 || NewIndex >= List.Count)
            return;

        List.RemoveAt(Index);
        List.Insert(NewIndex, Column);
        RefreshVisibleTab(Column);
        RefreshGroupTab(Column);
    }
    void MoveGroupColumn(bool ToGroup)
    {
        List<GroupGridColumnSettings> Source = ToGroup ? fAvailableGroupColumns : fGroupedColumns;
        List<GroupGridColumnSettings> Target = ToGroup ? fGroupedColumns : fAvailableGroupColumns;
        ListBox SourceListBox = ToGroup ? fAvailableGroupListBox : fGroupedListBox;
        GroupGridColumnSettings Column = GetSelectedColumn(SourceListBox);
        if (Column == null || !Source.Remove(Column))
            return;

        Target.Add(Column);
        RefreshGroupTab(Column);
    }
    void AttachListBoxDoubleClickHandlers()
    {
        fHiddenListBox.DoubleTapped += (Sender, Args) =>
        {
            Args.Handled = true;
            MoveBetweenLists(fHiddenColumns, fVisibleColumns, fHiddenListBox, true);
        };
        fVisibleListBox.DoubleTapped += (Sender, Args) =>
        {
            Args.Handled = true;
            MoveBetweenLists(fVisibleColumns, fHiddenColumns, fVisibleListBox, false);
        };
        fAvailableGroupListBox.DoubleTapped += (Sender, Args) =>
        {
            Args.Handled = true;
            MoveGroupColumn(true);
        };
        fGroupedListBox.DoubleTapped += (Sender, Args) =>
        {
            Args.Handled = true;
            MoveGroupColumn(false);
        };
    }
    GroupGridColumn FindSourceColumn(GroupGridColumnSettings Settings)
    {
        return Settings != null && fSourceColumns.TryGetValue(Settings.Name, out GroupGridColumn Column) ? Column : null;
    }
    List<GroupGridAggregateKind> GetAggregateList(GroupGridColumnSettings Settings)
    {
        GroupGridColumn Column = FindSourceColumn(Settings);
        return Enum.GetValues(typeof(GroupGridAggregateKind))
            .Cast<GroupGridAggregateKind>()
            .Where(AggregateKind => Column == null || Column.CanAggregate(AggregateKind))
            .ToList();
    }
    ComboBox CreateAggregateComboBox(GroupGridColumnSettings Settings, bool IsGroupSummary)
    {
        ComboBox Result = new()
        {
            Width = 110,
            ItemsSource = GetAggregateList(Settings),
        };
        GroupGridAggregateKind SelectedAggregate = IsGroupSummary ? Settings.GroupSummary : Settings.TotalSummary;
        Result.SelectedItem = Result.Items.Cast<object>().Contains(SelectedAggregate) ? SelectedAggregate : GroupGridAggregateKind.None;
        Result.SelectionChanged += (Sender, Args) =>
        {
            if (Result.SelectedItem is not GroupGridAggregateKind AggregateKind)
                return;
            if (IsGroupSummary)
                Settings.GroupSummary = AggregateKind;
            else
                Settings.TotalSummary = AggregateKind;
        };
        return Result;
    }
    Control CreateVisibleTab()
    {
        Button ShowButton = new() { Content = "←", MinWidth = 28 };
        Button HideButton = new() { Content = "→", MinWidth = 28 };
        Button UpButton = new() { Content = "↑", MinWidth = 28 };
        Button DownButton = new() { Content = "↓", MinWidth = 28 };
        ShowButton.Click += (Sender, Args) => MoveBetweenLists(fHiddenColumns, fVisibleColumns, fHiddenListBox, true);
        HideButton.Click += (Sender, Args) => MoveBetweenLists(fVisibleColumns, fHiddenColumns, fVisibleListBox, false);
        UpButton.Click += (Sender, Args) => MoveItem(fVisibleColumns, fVisibleListBox, -1);
        DownButton.Click += (Sender, Args) => MoveItem(fVisibleColumns, fVisibleListBox, 1);

        return new StackPanel
        {
            Margin = new Thickness(8),
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 6,
                    Children =
                    {
                        CreateNamedList("Visible", fVisibleListBox),
                        new StackPanel
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Spacing = 4,
                            Children =
                            {
                                ShowButton,
                                HideButton,
                                UpButton,
                                DownButton,
                            },
                        },
                        CreateNamedList("Hidden", fHiddenListBox),
                    },
                },
            },
        };
    }
    Control CreateGroupTab()
    {
        Button AddButton = new() { Content = "←", MinWidth = 28 };
        Button RemoveButton = new() { Content = "→", MinWidth = 28 };
        Button UpButton = new() { Content = "↑", MinWidth = 28 };
        Button DownButton = new() { Content = "↓", MinWidth = 28 };
        AddButton.Click += (Sender, Args) => MoveGroupColumn(true);
        RemoveButton.Click += (Sender, Args) => MoveGroupColumn(false);
        UpButton.Click += (Sender, Args) => MoveItem(fGroupedColumns, fGroupedListBox, -1);
        DownButton.Click += (Sender, Args) => MoveItem(fGroupedColumns, fGroupedListBox, 1);

        return new StackPanel
        {
            Margin = new Thickness(8),
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 6,
                    Children =
                    {
                        CreateNamedList("Group", fGroupedListBox),
                        new StackPanel
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Spacing = 4,
                            Children =
                            {
                                AddButton,
                                RemoveButton,
                                UpButton,
                                DownButton,
                            },
                        },
                        CreateNamedList("Available", fAvailableGroupListBox),
                    },
                },
            },
        };
    }
    Control CreateFiltersTab()
    {
        StackPanel Rows = new()
        {
            Spacing = 6,
        };
        Rows.Children.Add(CreateTwoColumnHeader("Field", "Filter"));
        foreach (GroupGridColumnSettings Column in Settings.Columns)
        {
            TextBox TextBox = new()
            {
                Width = 180,
                Text = Column.FilterText,
            };
            TextBox.TextChanged += (Sender, Args) => Column.FilterText = TextBox.Text ?? string.Empty;
            Rows.Children.Add(CreateTwoColumnRow(GetColumnText(Column), TextBox));
        }

        return CreateRowsPanel(Rows);
    }
    Control CreateSummariesTab()
    {
        StackPanel Rows = new()
        {
            Spacing = 6,
        };
        Rows.Children.Add(CreateThreeColumnHeader("Field", "Group", "Total"));
        foreach (GroupGridColumnSettings Column in Settings.Columns)
            Rows.Children.Add(CreateThreeColumnRow(GetColumnText(Column), CreateAggregateComboBox(Column, true), CreateAggregateComboBox(Column, false)));

        return CreateRowsPanel(Rows);
    }
    Control CreateNamedList(string Header, ListBox ListBox)
    {
        ListBox.Width = ListWidth;
        ListBox.Height = ListHeight;
        return new StackPanel
        {
            Children =
            {
                new TextBlock { Text = Header, FontWeight = FontWeight.SemiBold, Margin = new Thickness(0, 0, 0, 6) },
                ListBox,
            },
        };
    }
    Control CreateTwoColumnHeader(string Left, string Right)
    {
        return CreateTwoColumnRow(Left, new TextBlock { Text = Right, FontWeight = FontWeight.SemiBold }, true);
    }
    Control CreateTwoColumnRow(string Left, Control Right, bool IsHeader = false)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = Left, Width = 120, FontWeight = IsHeader ? FontWeight.SemiBold : FontWeight.Normal },
                Right,
            },
        };
    }
    Control CreateThreeColumnHeader(string Left, string Middle, string Right)
    {
        return CreateThreeColumnRow(Left, new TextBlock { Text = Middle, FontWeight = FontWeight.SemiBold }, new TextBlock { Text = Right, FontWeight = FontWeight.SemiBold }, true);
    }
    Control CreateThreeColumnRow(string Left, Control Middle, Control Right, bool IsHeader = false)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = Left, Width = 100, FontWeight = IsHeader ? FontWeight.SemiBold : FontWeight.Normal },
                Middle,
                Right,
            },
        };
    }
    Control CreateRowsPanel(Control Rows)
    {
        return new Border
        {
            Margin = new Thickness(8),
            Height = RowsViewportHeight,
            Child = new ScrollViewer
            {
                Content = Rows,
            },
        };
    }
    void InitializeLists()
    {
        fVisibleColumns.Clear();
        fHiddenColumns.Clear();
        fGroupedColumns.Clear();
        fAvailableGroupColumns.Clear();

        foreach (GroupGridColumnSettings Column in Settings.Columns.OrderBy(Column => Column.VisibleIndex))
        {
            if (Column.IsVisible)
                fVisibleColumns.Add(Column);
            else
                fHiddenColumns.Add(Column);
        }

        foreach (GroupGridColumnSettings Column in Settings.Columns.Where(Column => Column.GroupIndex >= 0).OrderBy(Column => Column.GroupIndex))
            fGroupedColumns.Add(Column);
        foreach (GroupGridColumnSettings Column in Settings.Columns.Where(Column => Column.GroupIndex < 0))
            fAvailableGroupColumns.Add(Column);
    }
    void ApplyListsToSettings()
    {
        int VisibleIndex = 0;
        foreach (GroupGridColumnSettings Column in fVisibleColumns)
        {
            Column.IsVisible = true;
            Column.VisibleIndex = VisibleIndex++;
        }
        foreach (GroupGridColumnSettings Column in fHiddenColumns)
        {
            Column.IsVisible = false;
            Column.VisibleIndex = VisibleIndex++;
        }

        foreach (GroupGridColumnSettings Column in Settings.Columns)
            Column.GroupIndex = -1;
        for (int Index = 0; Index < fGroupedColumns.Count; Index++)
            fGroupedColumns[Index].GroupIndex = Index;
    }
    void ResetDialog()
    {
        Settings = CloneSettings(InitialSettings);
        InitializeLists();
        Content = CreateContent();
    }
    Control CreateContent()
    {
        fNameTextBox.Width = 180;
        fNameTextBox.Text = Settings.Name;
        fNameTextBox.TextChanged += (Sender, Args) => Settings.Name = string.IsNullOrWhiteSpace(fNameTextBox.Text) ? "Default" : fNameTextBox.Text;

        TabControl TabControl = new()
        {
            Items =
            {
                new TabItem { Header = "Visible", Content = CreateVisibleTab() },
                new TabItem { Header = "Group", Content = CreateGroupTab() },
                new TabItem { Header = "Filters", Content = CreateFiltersTab() },
                new TabItem { Header = "Summaries", Content = CreateSummariesTab() },
            },
        };
        Button ResetButton = new() { Content = "Reset", MinWidth = 80 };
        Button CancelButton = new() { Content = "Cancel", MinWidth = 80 };
        Button ApplyButton = new() { Content = "Apply", MinWidth = 80 };
        ResetButton.Click += (Sender, Args) => ResetDialog();
        CancelButton.Click += (Sender, Args) => Close(false);
        ApplyButton.Click += (Sender, Args) =>
        {
            ApplyListsToSettings();
            Close(true);
        };

        RefreshVisibleTab();
        RefreshGroupTab();
        StackPanel NamePanel = new()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(8, 8, 8, 4),
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = "Name", VerticalAlignment = VerticalAlignment.Center },
                fNameTextBox,
            },
        };
        StackPanel ButtonPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(8, 4, 8, 8),
            Children =
            {
                ResetButton,
                CancelButton,
                ApplyButton,
            },
        };
        Grid Root = new()
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
        };
        Grid.SetRow(NamePanel, 0);
        Grid.SetRow(TabControl, 1);
        Grid.SetRow(ButtonPanel, 2);
        Root.Children.Add(NamePanel);
        Root.Children.Add(TabControl);
        Root.Children.Add(ButtonPanel);
        return Root;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridColumnManagerDialog"/> class.
    /// </summary>
    /// <param name="Settings">The settings edited by the dialog.</param>
    /// <param name="Columns">The source grid columns.</param>
    public GroupGridColumnManagerDialog(GroupGridSettings Settings, IEnumerable<GroupGridColumn> Columns)
    {
        this.Settings = CloneSettings(Settings ?? new GroupGridSettings());
        this.InitialSettings = CloneSettings(this.Settings);
        fSourceColumns = (Columns ?? Enumerable.Empty<GroupGridColumn>()).ToDictionary(Column => Column.Name, StringComparer.OrdinalIgnoreCase);
        InitializeLists();
        AttachListBoxDoubleClickHandlers();

        Title = "Column Manager";
        Width = 390;
        Height = 590;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Content = CreateContent();
    }

    // ● properties
    /// <summary>
    /// Gets the initial settings snapshot used by reset.
    /// </summary>
    public GroupGridSettings InitialSettings { get; private set; }
    /// <summary>
    /// Gets the edited settings.
    /// </summary>
    public GroupGridSettings Settings { get; private set; }
}

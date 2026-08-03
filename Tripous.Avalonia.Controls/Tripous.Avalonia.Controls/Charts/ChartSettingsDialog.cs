// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a settings dialog for <see cref="ChartControl"/>.
/// </summary>
public class ChartSettingsDialog: Window
{
    // ● private fields
    readonly IReadOnlyList<ChartSourceField> fSourceFields;
    readonly ComboBox cboChartType;
    readonly ComboBox cboCategoryField;
    readonly ComboBox cboSeriesField;
    readonly ComboBox cboValueField;
    readonly ComboBox cboAggregate;
    readonly ComboBox cboSortDirection;
    readonly ComboBox cboPalette;
    readonly TextBox edtTitle;
    readonly TextBox edtTopN;
    readonly TextBox edtValueFormat;
    readonly CheckBox chkShowLegend;
    readonly CheckBox chkShowValueLabels;
    readonly Button btnOk;
    readonly Button btnCancel;

    // ● private methods
    ComboBoxItem CreateComboItem(string Text, object Value)
    {
        return new ComboBoxItem
        {
            Content = Text ?? string.Empty,
            Tag = Value,
        };
    }
    ComboBox CreateComboBox()
    {
        return new ComboBox
        {
            MinHeight = 28,
            HorizontalAlignment = Layout.HorizontalAlignment.Stretch,
        };
    }
    TextBlock CreateLabel(string Text)
    {
        return new TextBlock
        {
            Text = Text,
            VerticalAlignment = Layout.VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0),
        };
    }
    TextBox CreateTextBox()
    {
        return new TextBox
        {
            MinHeight = 28,
            HorizontalAlignment = Layout.HorizontalAlignment.Stretch,
        };
    }
    Panel CreateRow(string Label, Control Editor)
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(130)),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
            },
            Children =
            {
                CreateLabel(Label),
                Editor,
            },
        };
    }
    object GetSelectedValue(ComboBox ComboBox)
    {
        return ComboBox?.SelectedItem is ComboBoxItem Item ? Item.Tag : null;
    }
    string GetSelectedFieldName(ComboBox ComboBox)
    {
        return GetSelectedValue(ComboBox) is ChartSourceField Field ? Field.Name : string.Empty;
    }
    void SelectValue(ComboBox ComboBox, object Value)
    {
        foreach (object ItemObject in ComboBox.Items)
            if (ItemObject is ComboBoxItem Item && Equals(Item.Tag, Value))
            {
                ComboBox.SelectedItem = Item;
                return;
            }

        ComboBox.SelectedIndex = ComboBox.Items.Count > 0 ? 0 : -1;
    }
    void SelectField(ComboBox ComboBox, string FieldName)
    {
        foreach (object ItemObject in ComboBox.Items)
            if (ItemObject is ComboBoxItem Item && Item.Tag is ChartSourceField Field && string.Equals(Field.Name, FieldName, StringComparison.OrdinalIgnoreCase))
            {
                ComboBox.SelectedItem = Item;
                return;
            }

        ComboBox.SelectedIndex = ComboBox.Items.Count > 0 ? 0 : -1;
    }
    void FillEnumCombo<T>(ComboBox ComboBox, T SelectedValue)
    {
        ComboBox.Items.Clear();
        foreach (T Value in Enum.GetValues(typeof(T)).Cast<T>())
            ComboBox.Items.Add(CreateComboItem(Value.ToString(), Value));
        SelectValue(ComboBox, SelectedValue);
    }
    void FillFieldCombos(ChartSettings Settings)
    {
        cboCategoryField.Items.Clear();
        cboSeriesField.Items.Clear();
        cboValueField.Items.Clear();
        cboSeriesField.Items.Add(CreateComboItem("(None)", null));
        foreach (ChartSourceField Field in fSourceFields)
        {
            if (Field.CanUseAsDimension)
            {
                cboCategoryField.Items.Add(CreateComboItem(Field.Header, Field));
                cboSeriesField.Items.Add(CreateComboItem(Field.Header, Field));
            }
            if (Field.CanUseAsMeasure)
                cboValueField.Items.Add(CreateComboItem(Field.Header, Field));
        }

        SelectField(cboCategoryField, Settings.CategoryFieldName);
        SelectField(cboSeriesField, Settings.SeriesFieldName);
        SelectField(cboValueField, Settings.ValueFieldName);
    }
    void FillPaletteCombo(string PaletteName)
    {
        cboPalette.Items.Clear();
        foreach (string Name in new[] { "Business", "Muted", "Signal" })
            cboPalette.Items.Add(CreateComboItem(Name, Name));
        SelectValue(cboPalette, string.IsNullOrWhiteSpace(PaletteName) ? "Business" : PaletteName);
    }
    void OkButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Settings.Title = edtTitle.Text ?? string.Empty;
        Settings.ChartType = GetSelectedValue(cboChartType) is ChartType ChartType ? ChartType : ChartType.Column;
        Settings.CategoryFieldName = GetSelectedFieldName(cboCategoryField);
        Settings.SeriesFieldName = GetSelectedFieldName(cboSeriesField);
        Settings.ValueFieldName = GetSelectedFieldName(cboValueField);
        Settings.AggregateKind = GetSelectedValue(cboAggregate) is ChartAggregateKind AggregateKind ? AggregateKind : ChartAggregateKind.Sum;
        Settings.SortDirection = GetSelectedValue(cboSortDirection) is ChartSortDirection SortDirection ? SortDirection : ChartSortDirection.None;
        Settings.TopN = int.TryParse(edtTopN.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int TopN) ? Math.Max(0, TopN) : 0;
        Settings.ShowLegend = chkShowLegend.IsChecked == true;
        Settings.ShowValueLabels = chkShowValueLabels.IsChecked == true;
        Settings.ValueFormat = edtValueFormat.Text ?? string.Empty;
        Settings.PaletteName = GetSelectedValue(cboPalette) as string ?? "Business";
        Close(true);
    }
    void CancelButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Close(false);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSettingsDialog"/> class.
    /// </summary>
    /// <param name="Settings">The settings to edit.</param>
    /// <param name="SourceFields">The available source fields.</param>
    public ChartSettingsDialog(ChartSettings Settings, IReadOnlyList<ChartSourceField> SourceFields)
    {
        this.Settings = Settings ?? new ChartSettings();
        fSourceFields = SourceFields ?? new List<ChartSourceField>();
        Title = "Chart Settings";
        Width = 480;
        Height = 560;
        MinWidth = 460;
        MinHeight = 560;
        CanResize = true;
        CanMinimize = false;
        CanMaximize = false;
        ShowInTaskbar = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        edtTitle = CreateTextBox();
        cboChartType = CreateComboBox();
        cboCategoryField = CreateComboBox();
        cboSeriesField = CreateComboBox();
        cboValueField = CreateComboBox();
        cboAggregate = CreateComboBox();
        cboSortDirection = CreateComboBox();
        edtTopN = CreateTextBox();
        edtValueFormat = CreateTextBox();
        cboPalette = CreateComboBox();
        chkShowLegend = new CheckBox { Content = "Show legend" };
        chkShowValueLabels = new CheckBox { Content = "Show value labels" };
        btnOk = new Button { Content = "OK", Width = 88, HorizontalContentAlignment = Layout.HorizontalAlignment.Center, IsDefault = true };
        btnCancel = new Button { Content = "Cancel", Width = 88, HorizontalContentAlignment = Layout.HorizontalAlignment.Center, IsCancel = true };

        edtTitle.Text = this.Settings.Title;
        FillEnumCombo(cboChartType, this.Settings.ChartType);
        FillFieldCombos(this.Settings);
        FillEnumCombo(cboAggregate, this.Settings.AggregateKind);
        FillEnumCombo(cboSortDirection, this.Settings.SortDirection);
        edtTopN.Text = this.Settings.TopN.ToString(CultureInfo.CurrentCulture);
        edtValueFormat.Text = this.Settings.ValueFormat;
        FillPaletteCombo(this.Settings.PaletteName);
        chkShowLegend.IsChecked = this.Settings.ShowLegend;
        chkShowValueLabels.IsChecked = this.Settings.ShowValueLabels;

        Grid.SetColumn(edtTitle, 1);
        Grid.SetColumn(cboChartType, 1);
        Grid.SetColumn(cboCategoryField, 1);
        Grid.SetColumn(cboSeriesField, 1);
        Grid.SetColumn(cboValueField, 1);
        Grid.SetColumn(cboAggregate, 1);
        Grid.SetColumn(cboSortDirection, 1);
        Grid.SetColumn(edtTopN, 1);
        Grid.SetColumn(edtValueFormat, 1);
        Grid.SetColumn(cboPalette, 1);

        StackPanel ButtonPanel = new()
        {
            Orientation = Layout.Orientation.Horizontal,
            HorizontalAlignment = Layout.HorizontalAlignment.Right,
            Margin = new Thickness(12),
            Spacing = 8,
            Children =
            {
                btnOk,
                btnCancel,
            },
        };
        StackPanel MainPanel = new()
        {
            Spacing = 8,
            Margin = new Thickness(12),
            HorizontalAlignment = Layout.HorizontalAlignment.Stretch,
            Children =
            {
                CreateRow("Title", edtTitle),
                CreateRow("Chart Type", cboChartType),
                CreateRow("Category Field", cboCategoryField),
                CreateRow("Series Field", cboSeriesField),
                CreateRow("Value Field", cboValueField),
                CreateRow("Aggregate", cboAggregate),
                CreateRow("Sort", cboSortDirection),
                CreateRow("TopN", edtTopN),
                CreateRow("Value Format", edtValueFormat),
                CreateRow("Palette", cboPalette),
                chkShowLegend,
                chkShowValueLabels,
            },
        };
        DockPanel RootPanel = new();
        DockPanel.SetDock(ButtonPanel, Dock.Bottom);
        RootPanel.Children.Add(ButtonPanel);
        RootPanel.Children.Add(MainPanel);

        Content = RootPanel;
        btnOk.Click += OkButton_Click;
        btnCancel.Click += CancelButton_Click;
    }

    // ● properties
    /// <summary>
    /// Gets the edited settings.
    /// </summary>
    public ChartSettings Settings { get; }
}

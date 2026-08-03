// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.Charts;

/// <summary>
/// Provides the main demo window.
/// </summary>
public partial class MainWindow: Window
{
    // ● private fields
    bool fIsWindowInitialized;

    // ● private
    List<SalesRow> CreateRows()
    {
        List<SalesRow> Result = new();
        string[] Regions = { "North", "South", "East", "West", "Central", "Coastal", "Mountain", "Island" };
        string[] Salespersons = { "Alex", "Bianca", "Chris", "Diana", "Eleni", "Felix" };
        string[] Categories = { "Office", "Lighting", "Hardware", "Storage" };

        for (int Index = 0; Index < 192; Index++)
        {
            int Quantity = 1 + (Index % 9);
            Result.Add(new SalesRow
            {
                Region = Regions[Index % Regions.Length],
                Salesperson = Salespersons[Index % Salespersons.Length],
                Category = Categories[Index % Categories.Length],
                Quarter = "Q" + (1 + (Index % 4)),
                Quantity = Quantity,
                Amount = Quantity * (35m + (Index % 17)),
            });
        }

        return Result;
    }
    DataTable CreateTable(string Source)
    {
        DataTable Result = new("Sales");
        Result.Columns.Add("Source", typeof(string));
        Result.Columns.Add("Region", typeof(string));
        Result.Columns.Add("Salesperson", typeof(string));
        Result.Columns.Add("Category", typeof(string));
        Result.Columns.Add("Quarter", typeof(string));
        Result.Columns.Add("Quantity", typeof(int));
        Result.Columns.Add("Amount", typeof(decimal));

        foreach (SalesRow Row in CreateRows())
            Result.Rows.Add(Source, Row.Region, Row.Salesperson, Row.Category, Row.Quarter, Row.Quantity, Row.Amount);

        return Result;
    }
    object CreateSelectedItemsSource()
    {
        switch (SourceComboBox.SelectedIndex)
        {
            case 1:
                return CreateTable("DataTable");
            case 2:
                return CreateTable("DataView").DefaultView;
        }

        return CreateRows();
    }
    void LoadChart()
    {
        Chart.ApplySettings(new ChartSettings
        {
            Title = "Sales Amount by Region and Quarter",
            ChartType = Chart.Settings.ChartType,
            CategoryFieldName = nameof(SalesRow.Region),
            SeriesFieldName = nameof(SalesRow.Quarter),
            ValueFieldName = nameof(SalesRow.Amount),
            AggregateKind = ChartAggregateKind.Sum,
            SortDirection = ChartSortDirection.Ascending,
            TopN = Chart.Settings.TopN,
            ShowLegend = Chart.Settings.ShowLegend,
            ShowValueLabels = Chart.Settings.ShowValueLabels,
            ValueFormat = "N0",
            PaletteName = Chart.Settings.PaletteName,
        });
        Chart.ItemsSource = CreateSelectedItemsSource();
        SetStatus($"{SourceComboBox.SelectionBoxItem}: {Chart.Engine.CategoryTexts.Count} categories, {Chart.Engine.Series.Count} series, {Chart.Settings.ChartType}.");
    }
    void SetChartType(ChartType ChartType)
    {
        ChartSettings Settings = Chart.CreateSettings();
        Settings.ChartType = ChartType;
        if (ChartType == ChartType.Pie || ChartType == ChartType.Donut)
        {
            Settings.SeriesFieldName = string.Empty;
            Settings.ShowLegend = false;
        }
        else
            Settings.SeriesFieldName = nameof(SalesRow.Quarter);

        Chart.ApplySettings(Settings);
        SetStatus($"{Chart.Settings.ChartType}: {Chart.Engine.CategoryTexts.Count} categories, {Chart.Engine.Series.Count} series.");
    }
    void SetStatus(string Text)
    {
        StatusTextBlock.Text = Text ?? string.Empty;
    }
    void SourceComboBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (!fIsWindowInitialized)
            return;

        LoadChart();
    }
    void ColumnButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Column);
    }
    void BarButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Bar);
    }
    void LineButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Line);
    }
    void AreaButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Area);
    }
    void PieButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Pie);
    }
    void DonutButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.Donut);
    }
    void StackedColumnButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.StackedColumn);
    }
    void StackedBarButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetChartType(ChartType.StackedBar);
    }
    void LabelsButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        ChartSettings Settings = Chart.CreateSettings();
        Settings.ShowValueLabels = !Settings.ShowValueLabels;
        Chart.ApplySettings(Settings);
        SetStatus($"Labels: {Settings.ShowValueLabels}.");
    }
    void LegendButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        ChartSettings Settings = Chart.CreateSettings();
        Settings.ShowLegend = !Settings.ShowLegend;
        Chart.ApplySettings(Settings);
        SetStatus($"Legend: {Settings.ShowLegend}.");
    }
    void Top5Button_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        ChartSettings Settings = Chart.CreateSettings();
        Settings.TopN = Settings.TopN == 0 ? 5 : 0;
        Chart.ApplySettings(Settings);
        SetStatus(Settings.TopN == 0 ? "TopN disabled." : "TopN: 5.");
    }
    async void SettingsButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        if (await Chart.ShowSettingsDialogAsync())
            SetStatus("Applied chart settings.");
    }
    void ReloadButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        LoadChart();
    }
    void DefaultThemeButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Application.Current.RequestedThemeVariant = ThemeVariant.Default;
        SetStatus("Default theme applied.");
    }
    void LightThemeButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        SetStatus("Light theme applied.");
    }
    void DarkThemeButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        SetStatus("Dark theme applied.");
    }
    void LocalBackgroundButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Chart.ChartBackgroundBrush = new SolidColorBrush(Color.FromRgb(255, 248, 220));
        SetStatus("Local chart background brush applied.");
    }
    void ClearLocalBackgroundButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Chart.ClearValue(global::Avalonia.Controls.ChartControl.ChartBackgroundBrushProperty);
        SetStatus("Local chart background brush cleared.");
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SourceComboBox.SelectedIndex = 0;
        LoadChart();
        fIsWindowInitialized = true;
    }
}

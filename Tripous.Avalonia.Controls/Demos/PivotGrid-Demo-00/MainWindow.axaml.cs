// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.PivotGrid;

/// <summary>
/// Provides the main demo window.
/// </summary>
public partial class MainWindow: Window
{
    // ● private fields
    bool fIsWindowInitialized;

    // ● private
    bool IsLargeDatasetSelected()
    {
        return DatasetComboBox.SelectedIndex == 1;
    }
    List<SalesRow> CreateRows()
    {
        return IsLargeDatasetSelected() ? CreateLargeRows() : CreateSmallRows();
    }
    List<SalesRow> CreateSmallRows()
    {
        List<SalesRow> Result = new();
        string[] Regions = { "North", "South", "East", "West" };
        string[] Salespersons = { "Alex", "Bianca", "Chris", "Diana" };
        string[] Products = { "Desk", "Chair", "Lamp" };
        string[] Categories = { "Office", "Lighting", "Storage" };

        for (int Index = 0; Index < 96; Index++)
        {
            int Quantity = 1 + (Index % 8);
            int Month = 1 + (Index % 12);
            Result.Add(new SalesRow
            {
                Region = Regions[Index % Regions.Length],
                Salesperson = Salespersons[Index % Salespersons.Length],
                Product = Products[Index % Products.Length],
                Category = Categories[Index % Categories.Length],
                Quarter = "Q" + (1 + (Index % 4)),
                Month = Month.ToString("00"),
                Quantity = Quantity,
                Amount = Quantity * (25m + (Index % 9)),
            });
        }

        return Result;
    }
    List<SalesRow> CreateLargeRows()
    {
        List<SalesRow> Result = new();
        string[] Regions = { "North", "South", "East", "West", "Central", "Coastal", "Mountain", "Island" };
        string[] Salespersons =
        {
            "Alex", "Bianca", "Chris", "Diana", "Eleni", "Felix", "Georgia", "Harris",
            "Iris", "Jonas", "Katerina", "Luca", "Maya", "Nikos", "Olivia", "Petros",
            "Rania", "Sofia", "Theo", "Vera",
        };
        string[] Products =
        {
            "Executive Standing Desk with Cable Management",
            "Ergonomic Mesh Chair with Adjustable Lumbar Support",
            "Architect Task Lamp with Wireless Charging Base",
            "Ultra Wide Monitor for Financial Dashboards",
            "Mechanical Keyboard with Programmable Shortcuts",
            "Precision Wireless Mouse for Design Workstations",
            "Locking File Cabinet with Fire Resistant Drawers",
            "Modular Storage Shelf for Shared Office Spaces",
            "High Volume Network Printer with Secure Release",
            "Document Scanner with Automatic Batch Feeder",
            "Conference Phone with Beamforming Microphones",
            "Rugged Tablet for Warehouse Inventory Counts",
            "Premium Notebook Bundle for Field Sales Teams",
            "Managed Router for Branch Office Installations",
            "Portable Projector for Training Room Presentations",
        };
        string[] Categories = { "Office", "Lighting", "Hardware", "Storage", "Mobile" };

        int Index = 0;
        foreach (string Region in Regions)
            foreach (string Salesperson in Salespersons)
                foreach (string Product in Products)
                {
                    int Month = 1 + (Index % 12);
                    int Quantity = 1 + ((Index * 7) % 18);
                    Result.Add(new SalesRow
                    {
                        Region = Region,
                        Salesperson = Salesperson,
                        Product = Product,
                        Category = Categories[Index % Categories.Length],
                        Quarter = "Q" + (1 + ((Month - 1) / 3)),
                        Month = Month.ToString("00"),
                        Quantity = Quantity,
                        Amount = Quantity * (18m + (Index % 37)),
                    });
                    Index++;
                }

        return Result;
    }
    DataTable CreateTable(string Source)
    {
        DataTable Result = new("Sales");
        Result.Columns.Add("Source", typeof(string));
        Result.Columns.Add("Region", typeof(string));
        Result.Columns.Add("Salesperson", typeof(string));
        Result.Columns.Add("Product", typeof(string));
        Result.Columns.Add("Category", typeof(string));
        Result.Columns.Add("Quarter", typeof(string));
        Result.Columns.Add("Month", typeof(string));
        Result.Columns.Add("Quantity", typeof(int));
        Result.Columns.Add("Amount", typeof(decimal));
        Result.Columns.Add("Payload", typeof(byte[]));

        foreach (SalesRow Row in CreateRows())
            Result.Rows.Add(Source, Row.Region, Row.Salesperson, Row.Product, Row.Category, Row.Quarter, Row.Month, Row.Quantity, Row.Amount, new byte[] { 1, 2, 3 });

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
    void LoadGrid()
    {
        Grid.ItemsSource = null;
        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.Measures.Clear();
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Region), Header = "Region" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Quarter), Header = "Quarter" });
        Grid.Measures.Add(new PivotGridMeasure { Name = "Amount", Header = "Amount", SourceFieldName = nameof(SalesRow.Amount), DisplayFormat = "N2", AggregateKind = PivotGridAggregateKind.Sum });
        Grid.Measures.Add(new PivotGridMeasure { Name = "Quantity", Header = "Qty", SourceFieldName = nameof(SalesRow.Quantity), AggregateKind = PivotGridAggregateKind.Sum, Width = 80 });
        Grid.ItemsSource = CreateSelectedItemsSource();
        SetStatus($"Loaded {SourceComboBox.SelectionBoxItem}, {DatasetComboBox.SelectionBoxItem}: {Grid.Engine.VisibleRowNodes.Count} visible rows, {Grid.Engine.ColumnItems.Count} columns.");
    }
    void SetStatus(string Text)
    {
        StatusTextBlock.Text = Text ?? string.Empty;
    }
    void RegionQuarterButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Region), Header = "Region" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Quarter), Header = "Quarter" });
        Grid.Engine.Rebuild();
        SetStatus("Showing Region x Quarter.");
    }
    void RegionSalespersonQuarterButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Region), Header = "Region" });
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Salesperson), Header = "Salesperson" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Quarter), Header = "Quarter" });
        Grid.Engine.Rebuild();
        SetStatus("Showing Region + Salesperson x Quarter.");
    }
    void LargeNavigationButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        if (!IsLargeDatasetSelected())
            DatasetComboBox.SelectedIndex = 1;

        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Region), Header = "Region" });
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Salesperson), Header = "Salesperson" });
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Product), Header = "Product" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Month), Header = "Month" });
        Grid.Engine.Rebuild();
        SetStatus($"Large navigation: {Grid.Engine.VisibleRowNodes.Count} visible rows, {Grid.Engine.ColumnItems.Count} month columns. Try PageUp/PageDown, Home/End, Ctrl+End.");
    }
    void LongRowHeaderButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        if (!IsLargeDatasetSelected())
            DatasetComboBox.SelectedIndex = 1;

        Grid.ItemsSource = null;
        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.Measures.Clear();
        Grid.LayoutMetrics.RowHeaderWidth = 130;
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Product), Header = "Product" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Month), Header = "Month" });
        Grid.Measures.Add(new PivotGridMeasure { Name = "Amount", Header = "Amount", SourceFieldName = nameof(SalesRow.Amount), DisplayFormat = "N2", AggregateKind = PivotGridAggregateKind.Sum });
        Grid.ItemsSource = CreateSelectedItemsSource();
        SetStatus("Long row header preset: right-click the grid and choose Auto Fit Row Header.");
    }
    void SalespersonProductButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Grid.RowFields.Clear();
        Grid.ColumnFields.Clear();
        Grid.RowFields.Add(new PivotGridField { Name = nameof(SalesRow.Salesperson), Header = "Salesperson" });
        Grid.ColumnFields.Add(new PivotGridField { Name = nameof(SalesRow.Product), Header = "Product" });
        Grid.Engine.Rebuild();
        SetStatus("Showing Salesperson x Product.");
    }
    async void SettingsButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        if (await Grid.ShowSettingsDialogAsync())
            SetStatus("Applied pivot settings.");
    }
    void ReloadButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        LoadGrid();
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
    void LocalSelectionButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Grid.SelectedCellBrush = new SolidColorBrush(Color.FromRgb(255, 188, 92));
        SetStatus("Local selected cell brush applied.");
    }
    void ClearLocalSelectionButton_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Grid.ClearValue(global::Avalonia.Controls.PivotGrid.SelectedCellBrushProperty);
        SetStatus("Local selected cell brush cleared.");
    }
    void SourceComboBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (!fIsWindowInitialized)
            return;

        LoadGrid();
    }
    void DatasetComboBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (!fIsWindowInitialized)
            return;

        LoadGrid();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SourceComboBox.SelectedIndex = 0;
        DatasetComboBox.SelectedIndex = 0;
        fIsWindowInitialized = true;
        LoadGrid();
    }
}

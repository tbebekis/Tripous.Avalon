// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Displays an aggregated BI chart using native Avalonia drawing.
/// </summary>
public class ChartControl: Control
{
    // ● private fields
    readonly ChartEngine fEngine = new();
    readonly List<(Rect Rect, ChartHitTestResult Hit)> fHitRegions = new();
    readonly Pen fAxisPen = new(new SolidColorBrush(Color.Parse("#D1D5DB")), 1);
    readonly Pen fGridPen = new(new SolidColorBrush(Color.Parse("#E5E7EB")), 1);
    readonly IBrush fTextBrush = new SolidColorBrush(Color.Parse("#111827"));
    readonly IBrush fMutedTextBrush = new SolidColorBrush(Color.Parse("#6B7280"));
    readonly IBrush fBackgroundBrush = new SolidColorBrush(Color.Parse("#FFFFFF"));
    object fItemsSource;
    IChartDataAdapter fDataAdapter;
    IChartDataAdapter fOwnedAdapter;
    ChartSettings fSettings = new();
    bool fIsSettingsMenuItemsVisible = true;

    // ● private methods
    void Engine_ProjectionChanged(object Sender, EventArgs Args)
    {
        InvalidateVisual();
    }
    void DisposeOwnedAdapter()
    {
        if (fOwnedAdapter is IDisposable Disposable)
            Disposable.Dispose();

        fOwnedAdapter = null;
    }
    IChartDataAdapter CreateAdapter(object Source)
    {
        if (Source == null)
            return null;
        if (Source is IChartDataAdapter Adapter)
            return Adapter;
        if (Source is DataTable Table)
            return new ChartDataViewDataAdapter(Table.DefaultView);
        if (Source is DataView View)
            return new ChartDataViewDataAdapter(View);
        if (Source is IList List)
            return new ChartListDataAdapter(List);

        return null;
    }
    FormattedText CreateText(string Text, IBrush Brush, double MaxWidth, double FontSize = 12, FontWeight Weight = FontWeight.Normal)
    {
        FormattedText Result = new(Text ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI", FontStyle.Normal, Weight, FontStretch.Normal), FontSize, Brush);
        Result.MaxTextWidth = Math.Max(0, MaxWidth);
        Result.Trimming = TextTrimming.CharacterEllipsis;
        return Result;
    }
    void DrawText(DrawingContext Context, string Text, Rect Rect, IBrush Brush, double FontSize = 12, FontWeight Weight = FontWeight.Normal, bool AlignRight = false, bool Center = false)
    {
        if (Rect.Width <= 2 || Rect.Height <= 2)
            return;

        using (Context.PushClip(Rect))
        {
            FormattedText FormattedText = CreateText(Text, Brush, Rect.Width - 6, FontSize, Weight);
            double X = AlignRight ? Rect.Right - FormattedText.Width - 3 : Rect.X + 3;
            if (Center)
                X = Rect.X + Math.Max(0, (Rect.Width - FormattedText.Width) / 2);
            double Y = Rect.Y + Math.Max(0, (Rect.Height - FormattedText.Height) / 2);
            Context.DrawText(FormattedText, new Point(X, Y));
        }
    }
    Rect GetContentRect()
    {
        return new Rect(0, 0, Bounds.Width, Bounds.Height).Deflate(12);
    }
    Rect GetTitleRect(Rect ContentRect)
    {
        return string.IsNullOrWhiteSpace(fSettings.Title)
            ? new Rect(ContentRect.X, ContentRect.Y, ContentRect.Width, 0)
            : new Rect(ContentRect.X, ContentRect.Y, ContentRect.Width, 30);
    }
    Rect GetLegendRect(Rect ContentRect, Rect TitleRect)
    {
        bool ShowLegend = fSettings.ShowLegend && fEngine.Series.Count > 1;
        return ShowLegend
            ? new Rect(ContentRect.Right - 150, TitleRect.Bottom + 4, 150, Math.Max(0, ContentRect.Bottom - TitleRect.Bottom - 4))
            : new Rect(ContentRect.Right, TitleRect.Bottom, 0, 0);
    }
    Rect GetPlotRect(Rect ContentRect, Rect TitleRect, Rect LegendRect, bool IsCircular)
    {
        double Right = LegendRect.Width > 0 ? LegendRect.X - 10 : ContentRect.Right;
        double BottomMargin = IsCircular ? 8 : 44;
        double LeftMargin = IsCircular ? 8 : 54;
        double Top = TitleRect.Bottom + 8;
        return new Rect(ContentRect.X + LeftMargin, Top + 8, Math.Max(0, Right - ContentRect.X - LeftMargin), Math.Max(0, ContentRect.Bottom - Top - BottomMargin));
    }
    bool IsCircularChart()
    {
        return fSettings.ChartType == ChartType.Pie || fSettings.ChartType == ChartType.Donut;
    }
    bool IsStackedChart()
    {
        return fSettings.ChartType == ChartType.StackedColumn || fSettings.ChartType == ChartType.StackedBar;
    }
    decimal GetMaximumValue()
    {
        if (IsStackedChart())
        {
            decimal Max = 0m;
            for (int CategoryIndex = 0; CategoryIndex < fEngine.CategoryKeys.Count; CategoryIndex++)
            {
                decimal Sum = 0m;
                foreach (ChartSeries Series in fEngine.Series)
                    if (CategoryIndex < Series.Points.Count)
                        Sum += Math.Max(0m, Series.Points[CategoryIndex].NumericValue);
                Max = Math.Max(Max, Sum);
            }
            return Max;
        }

        return fEngine.Series.SelectMany(Series => Series.Points).Select(Point => Math.Max(0m, Point.NumericValue)).DefaultIfEmpty(0m).Max();
    }
    void DrawEmpty(DrawingContext Context, Rect ContentRect)
    {
        DrawText(Context, "No chart data", ContentRect, fMutedTextBrush, 13, FontWeight.SemiBold, false, true);
    }
    void DrawTitle(DrawingContext Context, Rect TitleRect)
    {
        if (TitleRect.Height > 0)
            DrawText(Context, fSettings.Title, TitleRect, fTextBrush, 16, FontWeight.SemiBold, false, true);
    }
    void DrawLegend(DrawingContext Context, Rect LegendRect, ChartPalette Palette)
    {
        if (LegendRect.Width <= 0 || LegendRect.Height <= 0)
            return;

        double Y = LegendRect.Y + 4;
        for (int Index = 0; Index < fEngine.Series.Count && Y < LegendRect.Bottom - 18; Index++)
        {
            ChartSeries Series = fEngine.Series[Index];
            Rect SwatchRect = new(LegendRect.X + 4, Y + 5, 10, 10);
            Rect TextRect = new(SwatchRect.Right + 6, Y, LegendRect.Width - 24, 20);
            Context.DrawRectangle(new SolidColorBrush(Palette.GetColor(Index)), null, SwatchRect);
            DrawText(Context, Series.Text, TextRect, fMutedTextBrush);
            fHitRegions.Add((new Rect(LegendRect.X, Y, LegendRect.Width, 20), new ChartHitTestResult { Kind = ChartHitTestKind.Legend, SeriesIndex = Index, Series = Series }));
            Y += 22;
        }
    }
    void DrawAxes(DrawingContext Context, Rect PlotRect, decimal MaxValue)
    {
        if (PlotRect.Width <= 0 || PlotRect.Height <= 0)
            return;

        for (int Tick = 0; Tick <= 4; Tick++)
        {
            double Y = PlotRect.Bottom - (PlotRect.Height * Tick / 4);
            Context.DrawLine(fGridPen, new Point(PlotRect.Left, Y), new Point(PlotRect.Right, Y));
            decimal Value = MaxValue * Tick / 4m;
            DrawText(Context, Value.ToString(fSettings.ValueFormat, CultureInfo.CurrentCulture), new Rect(0, Y - 10, PlotRect.Left - 4, 20), fMutedTextBrush, 11, FontWeight.Normal, true);
        }

        Context.DrawLine(fAxisPen, new Point(PlotRect.Left, PlotRect.Top), new Point(PlotRect.Left, PlotRect.Bottom));
        Context.DrawLine(fAxisPen, new Point(PlotRect.Left, PlotRect.Bottom), new Point(PlotRect.Right, PlotRect.Bottom));
    }
    void DrawCategoryLabels(DrawingContext Context, Rect PlotRect)
    {
        int Count = fEngine.CategoryTexts.Count;
        if (Count == 0)
            return;

        double SlotWidth = PlotRect.Width / Count;
        for (int Index = 0; Index < Count; Index++)
        {
            if (Count > 12 && Index % Math.Ceiling(Count / 12.0) != 0)
                continue;

            Rect LabelRect = new(PlotRect.Left + Index * SlotWidth, PlotRect.Bottom + 4, SlotWidth, 34);
            DrawText(Context, fEngine.CategoryTexts[Index], LabelRect, fMutedTextBrush, 11, FontWeight.Normal, false, true);
        }
    }
    void DrawBarCategoryLabels(DrawingContext Context, Rect PlotRect)
    {
        int Count = fEngine.CategoryTexts.Count;
        if (Count == 0)
            return;

        double SlotHeight = PlotRect.Height / Count;
        for (int Index = 0; Index < Count; Index++)
        {
            Rect LabelRect = new(0, PlotRect.Top + Index * SlotHeight, PlotRect.Left - 6, SlotHeight);
            DrawText(Context, fEngine.CategoryTexts[Index], LabelRect, fMutedTextBrush, 11, FontWeight.Normal, true);
        }
    }
    void AddPointHit(Rect Rect, int SeriesIndex, int PointIndex)
    {
        ChartSeries Series = fEngine.Series[SeriesIndex];
        ChartDataPoint Point = Series.Points[PointIndex];
        fHitRegions.Add((Rect, new ChartHitTestResult
        {
            Kind = ChartHitTestKind.DataPoint,
            SeriesIndex = SeriesIndex,
            PointIndex = PointIndex,
            Series = Series,
            DataPoint = Point,
        }));
    }
    string GetCircularLabelText(ChartDataPoint Point)
    {
        if (Point == null)
            return string.Empty;

        string CategoryText = string.IsNullOrWhiteSpace(Point.CategoryText) ? "(Blank)" : Point.CategoryText;
        return string.IsNullOrWhiteSpace(Point.Text) ? CategoryText : CategoryText + ": " + Point.Text;
    }
    MenuItem CreateMenuItem(string Header, bool IsEnabled, Action Click)
    {
        MenuItem Result = new()
        {
            Header = Header,
            IsEnabled = IsEnabled,
        };
        Result.Click += (Sender, Args) => Click?.Invoke();
        return Result;
    }
    MenuItem CreateCheckedMenuItem(string Header, bool IsChecked, Action Click)
    {
        MenuItem Result = CreateMenuItem(Header, true, Click);
        Result.ToggleType = MenuItemToggleType.CheckBox;
        Result.IsChecked = IsChecked;
        return Result;
    }
    MenuItem CreateSubMenuItem(string Header, IEnumerable<object> Items)
    {
        return new MenuItem
        {
            Header = Header,
            ItemsSource = Items?.ToList() ?? new List<object>(),
        };
    }
    MenuItem CreateChartTypeMenuItem()
    {
        List<object> Items = Enum.GetValues(typeof(ChartType))
            .Cast<ChartType>()
            .Select(ChartType =>
            {
                string Header = ChartType == fSettings.ChartType ? "* " + ChartType : ChartType.ToString();
                return (object)CreateMenuItem(Header, true, () => SetChartType(ChartType));
            })
            .ToList();

        return CreateSubMenuItem("Chart Type", Items);
    }
    MenuItem CreateSortMenuItem()
    {
        List<object> Items = Enum.GetValues(typeof(ChartSortDirection))
            .Cast<ChartSortDirection>()
            .Select(Direction =>
            {
                string Header = Direction == fSettings.SortDirection ? "* " + Direction : Direction.ToString();
                return (object)CreateMenuItem(Header, true, () => SetSortDirection(Direction));
            })
            .ToList();

        return CreateSubMenuItem("Sort", Items);
    }
    MenuItem CreateTopNMenuItem()
    {
        int[] Values = { 0, 5, 10, 20 };
        List<object> Items = Values
            .Select(Value =>
            {
                string Text = Value == 0 ? "No TopN" : "Top " + Value.ToString(CultureInfo.CurrentCulture);
                string Header = Value == fSettings.TopN ? "* " + Text : Text;
                return (object)CreateMenuItem(Header, true, () => SetTopN(Value));
            })
            .ToList();

        return CreateSubMenuItem("TopN", Items);
    }
    MenuItem CreatePaletteMenuItem()
    {
        List<object> Items = new[] { "Business", "Muted", "Signal" }
            .Select(Name =>
            {
                string Header = string.Equals(Name, fSettings.PaletteName, StringComparison.OrdinalIgnoreCase) ? "* " + Name : Name;
                return (object)CreateMenuItem(Header, true, () => SetPalette(Name));
            })
            .ToList();

        return CreateSubMenuItem("Palette", Items);
    }
    bool ShowChartContextMenu(Point Point)
    {
        ContextMenu Menu = new()
        {
            Placement = PlacementMode.Pointer,
        };
        List<object> Items = new()
        {
            CreateMenuItem("Settings...", true, () => _ = ShowSettingsDialogAsync()),
            new Separator(),
            CreateChartTypeMenuItem(),
            CreateSortMenuItem(),
            CreateTopNMenuItem(),
            CreatePaletteMenuItem(),
            new Separator(),
            CreateCheckedMenuItem("Legend", fSettings.ShowLegend, ToggleLegend),
            CreateCheckedMenuItem("Value Labels", fSettings.ShowValueLabels, ToggleValueLabels),
            new Separator(),
            CreateMenuItem("Export to PNG...", true, ExportToPngAsync),
        };
        if (fIsSettingsMenuItemsVisible)
        {
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Save Settings...", true, SaveSettingsAsync));
            Items.Add(CreateMenuItem("Load Settings...", true, LoadSettingsAsync));
        }

        Menu.ItemsSource = Items;
        Menu.Open(this);
        return true;
    }
    ChartSettings CloneSettings()
    {
        string Json = JsonSerializer.Serialize(fSettings);
        return JsonSerializer.Deserialize<ChartSettings>(Json) ?? new ChartSettings();
    }
    void SetChartType(ChartType ChartType)
    {
        ChartSettings Settings = CloneSettings();
        Settings.ChartType = ChartType;
        ApplySettings(Settings);
    }
    void SetSortDirection(ChartSortDirection Direction)
    {
        ChartSettings Settings = CloneSettings();
        Settings.SortDirection = Direction;
        ApplySettings(Settings);
    }
    void SetTopN(int TopN)
    {
        ChartSettings Settings = CloneSettings();
        Settings.TopN = Math.Max(0, TopN);
        ApplySettings(Settings);
    }
    void SetPalette(string PaletteName)
    {
        ChartSettings Settings = CloneSettings();
        Settings.PaletteName = PaletteName ?? "Business";
        ApplySettings(Settings);
    }
    void ToggleLegend()
    {
        ChartSettings Settings = CloneSettings();
        Settings.ShowLegend = !Settings.ShowLegend;
        ApplySettings(Settings);
    }
    void ToggleValueLabels()
    {
        ChartSettings Settings = CloneSettings();
        Settings.ShowValueLabels = !Settings.ShowValueLabels;
        ApplySettings(Settings);
    }
    async void SaveSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerSaveOptions Options = new()
        {
            Title = "Save Chart Settings",
            SuggestedFileName = string.IsNullOrWhiteSpace(SettingsSuggestedFileName) ? "chart-settings.json" : SettingsSuggestedFileName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
            },
        };
        IStorageFile File = await Owner.StorageProvider.SaveFilePickerAsync(Options);
        if (File == null)
            return;

        SaveSettings(File.Path.LocalPath);
    }
    async void ExportToPngAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerSaveOptions Options = new()
        {
            Title = "Export Chart to PNG",
            SuggestedFileName = "chart.png",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PNG") { Patterns = new[] { "*.png" } },
            },
        };
        IStorageFile File = await Owner.StorageProvider.SaveFilePickerAsync(Options);
        if (File == null)
            return;

        ExportToPng(File.Path.LocalPath);
    }
    async void LoadSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerOpenOptions Options = new()
        {
            Title = "Load Chart Settings",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
            },
        };
        IReadOnlyList<IStorageFile> Files = await Owner.StorageProvider.OpenFilePickerAsync(Options);
        if (Files == null || Files.Count == 0)
            return;

        LoadSettings(Files[0].Path.LocalPath);
    }
    void DrawColumnCharts(DrawingContext Context, Rect PlotRect, ChartPalette Palette, decimal MaxValue)
    {
        int CategoryCount = fEngine.CategoryKeys.Count;
        int SeriesCount = Math.Max(1, fEngine.Series.Count);
        if (CategoryCount == 0 || MaxValue <= 0m)
            return;

        double SlotWidth = PlotRect.Width / CategoryCount;
        double GroupPadding = Math.Min(12, SlotWidth * 0.18);
        for (int CategoryIndex = 0; CategoryIndex < CategoryCount; CategoryIndex++)
        {
            double X = PlotRect.Left + CategoryIndex * SlotWidth + GroupPadding / 2;
            double AvailableWidth = Math.Max(1, SlotWidth - GroupPadding);
            double StackTop = PlotRect.Bottom;
            for (int SeriesIndex = 0; SeriesIndex < fEngine.Series.Count; SeriesIndex++)
            {
                ChartDataPoint Point = fEngine.Series[SeriesIndex].Points[CategoryIndex];
                double Height = PlotRect.Height * (double)(Math.Max(0m, Point.NumericValue) / MaxValue);
                double BarWidth = IsStackedChart() ? AvailableWidth : AvailableWidth / SeriesCount;
                double BarX = IsStackedChart() ? X : X + SeriesIndex * BarWidth;
                double BarY = IsStackedChart() ? StackTop - Height : PlotRect.Bottom - Height;
                Rect BarRect = new(BarX + 1, BarY, Math.Max(1, BarWidth - 2), Math.Max(1, Height));
                Context.DrawRectangle(new SolidColorBrush(Palette.GetColor(SeriesIndex)), null, BarRect);
                AddPointHit(BarRect, SeriesIndex, CategoryIndex);
                if (fSettings.ShowValueLabels && Height > 14)
                    DrawText(Context, Point.Text, BarRect, Brushes.White, 10, FontWeight.SemiBold, false, true);
                if (IsStackedChart())
                    StackTop = BarY;
            }
        }
    }
    void DrawBarCharts(DrawingContext Context, Rect PlotRect, ChartPalette Palette, decimal MaxValue)
    {
        int CategoryCount = fEngine.CategoryKeys.Count;
        int SeriesCount = Math.Max(1, fEngine.Series.Count);
        if (CategoryCount == 0 || MaxValue <= 0m)
            return;

        double SlotHeight = PlotRect.Height / CategoryCount;
        double GroupPadding = Math.Min(10, SlotHeight * 0.18);
        for (int CategoryIndex = 0; CategoryIndex < CategoryCount; CategoryIndex++)
        {
            double Y = PlotRect.Top + CategoryIndex * SlotHeight + GroupPadding / 2;
            double AvailableHeight = Math.Max(1, SlotHeight - GroupPadding);
            double StackLeft = PlotRect.Left;
            for (int SeriesIndex = 0; SeriesIndex < fEngine.Series.Count; SeriesIndex++)
            {
                ChartDataPoint Point = fEngine.Series[SeriesIndex].Points[CategoryIndex];
                double Width = PlotRect.Width * (double)(Math.Max(0m, Point.NumericValue) / MaxValue);
                double BarHeight = IsStackedChart() ? AvailableHeight : AvailableHeight / SeriesCount;
                double BarX = IsStackedChart() ? StackLeft : PlotRect.Left;
                double BarY = IsStackedChart() ? Y : Y + SeriesIndex * BarHeight;
                Rect BarRect = new(BarX, BarY + 1, Math.Max(1, Width), Math.Max(1, BarHeight - 2));
                Context.DrawRectangle(new SolidColorBrush(Palette.GetColor(SeriesIndex)), null, BarRect);
                AddPointHit(BarRect, SeriesIndex, CategoryIndex);
                if (fSettings.ShowValueLabels && Width > 24)
                    DrawText(Context, Point.Text, BarRect, Brushes.White, 10, FontWeight.SemiBold, false, true);
                if (IsStackedChart())
                    StackLeft += Width;
            }
        }
    }
    void DrawLineCharts(DrawingContext Context, Rect PlotRect, ChartPalette Palette, decimal MaxValue)
    {
        int CategoryCount = fEngine.CategoryKeys.Count;
        if (CategoryCount == 0 || MaxValue <= 0m)
            return;

        double SlotWidth = CategoryCount == 1 ? PlotRect.Width : PlotRect.Width / (CategoryCount - 1);
        for (int SeriesIndex = 0; SeriesIndex < fEngine.Series.Count; SeriesIndex++)
        {
            ChartSeries Series = fEngine.Series[SeriesIndex];
            List<Point> Points = new();
            for (int PointIndex = 0; PointIndex < Series.Points.Count; PointIndex++)
            {
                ChartDataPoint DataPoint = Series.Points[PointIndex];
                double X = CategoryCount == 1 ? PlotRect.Left + PlotRect.Width / 2 : PlotRect.Left + PointIndex * SlotWidth;
                double Y = PlotRect.Bottom - PlotRect.Height * (double)(Math.Max(0m, DataPoint.NumericValue) / MaxValue);
                Points.Add(new Point(X, Y));
            }

            Color Color = Palette.GetColor(SeriesIndex);
            if (fSettings.ChartType == ChartType.Area && Points.Count > 0)
            {
                StreamGeometry Area = new();
                using (StreamGeometryContext GeometryContext = Area.Open())
                {
                    GeometryContext.BeginFigure(new Point(Points[0].X, PlotRect.Bottom), true);
                    foreach (Point Point in Points)
                        GeometryContext.LineTo(Point);
                    GeometryContext.LineTo(new Point(Points[^1].X, PlotRect.Bottom));
                    GeometryContext.EndFigure(true);
                }
                Context.DrawGeometry(new SolidColorBrush(Color.FromArgb(60, Color.R, Color.G, Color.B)), null, Area);
            }

            Pen LinePen = new(new SolidColorBrush(Color), 2);
            for (int Index = 1; Index < Points.Count; Index++)
                Context.DrawLine(LinePen, Points[Index - 1], Points[Index]);
            for (int Index = 0; Index < Points.Count; Index++)
            {
                Context.DrawEllipse(new SolidColorBrush(Color), null, Points[Index], 4, 4);
                Rect HitRect = new(Points[Index].X - 7, Points[Index].Y - 7, 14, 14);
                AddPointHit(HitRect, SeriesIndex, Index);
                if (fSettings.ShowValueLabels)
                    DrawText(Context, Series.Points[Index].Text, new Rect(Points[Index].X - 28, Points[Index].Y - 24, 56, 18), fMutedTextBrush, 10, FontWeight.SemiBold, false, true);
            }
        }
    }
    void DrawPieCharts(DrawingContext Context, Rect PlotRect, ChartPalette Palette)
    {
        List<(ChartDataPoint Point, int SeriesIndex, int PointIndex)> Points = new();
        for (int SeriesIndex = 0; SeriesIndex < fEngine.Series.Count; SeriesIndex++)
            for (int PointIndex = 0; PointIndex < fEngine.Series[SeriesIndex].Points.Count; PointIndex++)
            {
                ChartDataPoint Point = fEngine.Series[SeriesIndex].Points[PointIndex];
                if (Point.NumericValue > 0m)
                    Points.Add((Point, SeriesIndex, PointIndex));
            }

        decimal Total = Points.Sum(Item => Item.Point.NumericValue);
        if (Total <= 0m)
            return;

        double Radius = Math.Max(1, Math.Min(PlotRect.Width, PlotRect.Height) / 2 - 4);
        Point Center = new(PlotRect.Left + PlotRect.Width / 2, PlotRect.Top + PlotRect.Height / 2);
        double StartAngle = -90;
        for (int Index = 0; Index < Points.Count; Index++)
        {
            (ChartDataPoint DataPoint, int SeriesIndex, int PointIndex) Item = Points[Index];
            double Sweep = 360.0 * (double)(Item.DataPoint.NumericValue / Total);
            double EndAngle = StartAngle + Sweep;
            Point StartPoint = new(Center.X + Radius * Math.Cos(StartAngle * Math.PI / 180), Center.Y + Radius * Math.Sin(StartAngle * Math.PI / 180));
            Point EndPoint = new(Center.X + Radius * Math.Cos(EndAngle * Math.PI / 180), Center.Y + Radius * Math.Sin(EndAngle * Math.PI / 180));
            StreamGeometry Geometry = new();
            using (StreamGeometryContext GeometryContext = Geometry.Open())
            {
                GeometryContext.BeginFigure(Center, true);
                GeometryContext.LineTo(StartPoint);
                GeometryContext.ArcTo(EndPoint, new Size(Radius, Radius), 0, Sweep > 180, SweepDirection.Clockwise);
                GeometryContext.LineTo(Center);
                GeometryContext.EndFigure(true);
            }
            Context.DrawGeometry(new SolidColorBrush(Palette.GetColor(Index)), null, Geometry);
            if (fSettings.ShowValueLabels && Sweep > 18)
            {
                double MidAngle = (StartAngle + EndAngle) / 2;
                Point LabelPoint = new(Center.X + Radius * 0.62 * Math.Cos(MidAngle * Math.PI / 180), Center.Y + Radius * 0.62 * Math.Sin(MidAngle * Math.PI / 180));
                DrawText(Context, GetCircularLabelText(Item.DataPoint), new Rect(LabelPoint.X - 58, LabelPoint.Y - 18, 116, 36), Brushes.White, 10, FontWeight.SemiBold, false, true);
            }
            AddPointHit(new Rect(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2), Item.SeriesIndex, Item.PointIndex);
            StartAngle = EndAngle;
        }

        if (fSettings.ChartType == ChartType.Donut)
            Context.DrawEllipse(fBackgroundBrush, null, Center, Radius * 0.48, Radius * 0.48);
    }
    void DrawChart(DrawingContext Context, Rect PlotRect, ChartPalette Palette)
    {
        decimal MaxValue = GetMaximumValue();
        if (!IsCircularChart())
            DrawAxes(Context, PlotRect, MaxValue <= 0m ? 1m : MaxValue);

        switch (fSettings.ChartType)
        {
            case ChartType.Bar:
            case ChartType.StackedBar:
                DrawBarCharts(Context, PlotRect, Palette, MaxValue);
                DrawBarCategoryLabels(Context, PlotRect);
                break;
            case ChartType.Line:
            case ChartType.Area:
                DrawLineCharts(Context, PlotRect, Palette, MaxValue);
                DrawCategoryLabels(Context, PlotRect);
                break;
            case ChartType.Pie:
            case ChartType.Donut:
                DrawPieCharts(Context, PlotRect, Palette);
                break;
            default:
                DrawColumnCharts(Context, PlotRect, Palette, MaxValue);
                DrawCategoryLabels(Context, PlotRect);
                break;
        }
    }

    // ● overridables
    /// <inheritdoc />
    protected override void OnPointerMoved(PointerEventArgs Args)
    {
        base.OnPointerMoved(Args);
        ToolTip.SetTip(this, GetToolTipText(Args.GetPosition(this)));
    }
    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs Args)
    {
        base.OnPointerPressed(Args);

        PointerPoint PointProperties = Args.GetCurrentPoint(this);
        if (PointProperties.Properties.IsRightButtonPressed)
        {
            ShowChartContextMenu(Args.GetPosition(this));
            Args.Handled = true;
        }
    }
    /// <inheritdoc />
    protected override void OnPointerExited(PointerEventArgs Args)
    {
        base.OnPointerExited(Args);
        ToolTip.SetTip(this, null);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    public ChartControl()
    {
        Focusable = true;
        fEngine.ProjectionChanged += Engine_ProjectionChanged;
        fEngine.ApplySettings(fSettings);
    }

    // ● public methods
    /// <summary>
    /// Creates a serializable settings snapshot.
    /// </summary>
    /// <returns>The settings snapshot.</returns>
    public ChartSettings CreateSettings()
    {
        return CloneSettings();
    }
    /// <summary>
    /// Applies settings to the control.
    /// </summary>
    /// <param name="Settings">The settings.</param>
    public void ApplySettings(ChartSettings Settings)
    {
        fSettings = Settings ?? new ChartSettings();
        fEngine.ApplySettings(fSettings);
        InvalidateVisual();
    }
    /// <summary>
    /// Saves settings as JSON.
    /// </summary>
    /// <param name="FilePath">The full file path.</param>
    public void SaveSettings(string FilePath)
    {
        JsonSerializerOptions Options = new() { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(fSettings, Options), Encoding.UTF8);
    }
    /// <summary>
    /// Exports the currently displayed chart to a PNG image using the current control size.
    /// </summary>
    /// <param name="FilePath">The full file path.</param>
    public void ExportToPng(string FilePath)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ArgumentNullException(nameof(FilePath));

        int Width = Math.Max(1, (int)Math.Ceiling(Bounds.Width));
        int Height = Math.Max(1, (int)Math.Ceiling(Bounds.Height));
        RenderTargetBitmap Bitmap = new(new PixelSize(Width, Height));
        Bitmap.Render(this);
        Bitmap.Save(FilePath);
    }
    /// <summary>
    /// Loads settings from JSON.
    /// </summary>
    /// <param name="FilePath">The full file path.</param>
    public void LoadSettings(string FilePath)
    {
        ApplySettings(JsonSerializer.Deserialize<ChartSettings>(File.ReadAllText(FilePath, Encoding.UTF8)) ?? new ChartSettings());
    }
    /// <summary>
    /// Shows the chart settings dialog.
    /// </summary>
    /// <returns>True if settings were applied; otherwise, false.</returns>
    public async Task<bool> ShowSettingsDialogAsync()
    {
        ChartSettings Settings = CreateSettings();
        ChartSettingsDialog Dialog = new(Settings, DataAdapter?.SourceFields ?? new List<ChartSourceField>());
        bool Result;
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner is Window Window)
            Result = await Dialog.ShowDialog<bool>(Window);
        else
        {
            Dialog.Show();
            return false;
        }

        if (!Result)
            return false;

        ApplySettings(Dialog.Settings);
        return true;
    }
    /// <summary>
    /// Hit-tests the chart.
    /// </summary>
    /// <param name="Point">The control point.</param>
    /// <returns>The hit-test result.</returns>
    public ChartHitTestResult HitTest(Point Point)
    {
        for (int Index = fHitRegions.Count - 1; Index >= 0; Index--)
            if (fHitRegions[Index].Rect.Contains(Point))
                return fHitRegions[Index].Hit;

        return ChartHitTestResult.Empty();
    }
    /// <summary>
    /// Returns tooltip text for a control point.
    /// </summary>
    /// <param name="Point">The control point.</param>
    /// <returns>The tooltip text.</returns>
    public string GetToolTipText(Point Point)
    {
        ChartHitTestResult Hit = HitTest(Point);
        if (Hit.Kind == ChartHitTestKind.Legend && Hit.Series != null)
            return Hit.Series.Text;
        if (Hit.Kind == ChartHitTestKind.DataPoint && Hit.DataPoint != null)
        {
            string SeriesText = fEngine.Series.Count > 1 ? Hit.DataPoint.SeriesText + Environment.NewLine : string.Empty;
            return SeriesText + Hit.DataPoint.CategoryText + Environment.NewLine + Hit.DataPoint.Text;
        }

        return string.Empty;
    }
    /// <inheritdoc />
    public override void Render(DrawingContext Context)
    {
        base.Render(Context);
        fHitRegions.Clear();
        Rect ContentRect = GetContentRect();
        Context.DrawRectangle(fBackgroundBrush, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        Rect TitleRect = GetTitleRect(ContentRect);
        Rect LegendRect = GetLegendRect(ContentRect, TitleRect);
        Rect PlotRect = GetPlotRect(ContentRect, TitleRect, LegendRect, IsCircularChart());
        DrawTitle(Context, TitleRect);
        if (fEngine.Series.Count == 0 || fEngine.CategoryKeys.Count == 0)
        {
            DrawEmpty(Context, ContentRect);
            return;
        }

        ChartPalette Palette = ChartPalette.Get(fSettings.PaletteName);
        DrawChart(Context, PlotRect, Palette);
        DrawLegend(Context, LegendRect, Palette);
    }

    // ● properties
    /// <summary>
    /// Gets the non-visual chart engine.
    /// </summary>
    public ChartEngine Engine => fEngine;
    /// <summary>
    /// Gets or sets the chart settings.
    /// </summary>
    public ChartSettings Settings
    {
        get => fSettings;
        set => ApplySettings(value);
    }
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IChartDataAdapter DataAdapter
    {
        get => fDataAdapter;
        set
        {
            if (ReferenceEquals(fDataAdapter, value))
                return;

            DisposeOwnedAdapter();
            fDataAdapter = value;
            fEngine.DataAdapter = fDataAdapter;
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets the items source.
    /// </summary>
    public object ItemsSource
    {
        get => fItemsSource;
        set
        {
            if (ReferenceEquals(fItemsSource, value))
                return;

            fItemsSource = value;
            DisposeOwnedAdapter();
            fOwnedAdapter = CreateAdapter(value);
            fDataAdapter = fOwnedAdapter;
            fEngine.DataAdapter = fDataAdapter;
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether Save Settings and Load Settings context menu items are visible.
    /// </summary>
    public bool IsSettingsMenuItemsVisible
    {
        get => fIsSettingsMenuItemsVisible;
        set => fIsSettingsMenuItemsVisible = value;
    }
    /// <summary>
    /// Gets or sets the suggested settings file name used by the Save Settings file picker.
    /// </summary>
    public string SettingsSuggestedFileName { get; set; } = "chart-settings.json";
}

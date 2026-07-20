// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the Avalonia visual surface for a <see cref="PivotGridEngine"/>.
/// </summary>
public class PivotGrid: Control
{
    // ● public fields
    /// <summary>
    /// Defines the <see cref="GridBackgroundBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GridBackgroundBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(GridBackgroundBrush), CreateBrush(255, 255, 255));
    /// <summary>
    /// Defines the <see cref="HeaderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> HeaderBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(HeaderBrush), CreateBrush(241, 243, 245));
    /// <summary>
    /// Defines the <see cref="TextBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> TextBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(TextBrush), CreateBrush(32, 37, 42));
    /// <summary>
    /// Defines the <see cref="MutedTextBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> MutedTextBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(MutedTextBrush), CreateBrush(84, 91, 99));
    /// <summary>
    /// Defines the <see cref="GridLineBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GridLineBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(GridLineBrush), CreateBrush(211, 216, 222));
    /// <summary>
    /// Defines the <see cref="SelectedCellBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> SelectedCellBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(SelectedCellBrush), CreateBrush(218, 236, 255));
    /// <summary>
    /// Defines the <see cref="SelectedCellBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> SelectedCellBorderBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(SelectedCellBorderBrush), CreateBrush(61, 132, 232));
    /// <summary>
    /// Defines the <see cref="ScrollBarTrackBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ScrollBarTrackBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(ScrollBarTrackBrush), CreateBrush(244, 246, 248));
    /// <summary>
    /// Defines the <see cref="ScrollBarThumbBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ScrollBarThumbBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(ScrollBarThumbBrush), CreateBrush(188, 196, 205));
    /// <summary>
    /// Defines the <see cref="ResizeGuideBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ResizeGuideBrushProperty = AvaloniaProperty.Register<PivotGrid, IBrush>(nameof(ResizeGuideBrush), CreateBrush(80, 120, 170));

    // ● private fields
    static readonly JsonSerializerOptions fSettingsJsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };
    readonly Dictionary<string, double> fValueColumnWidths = new(StringComparer.Ordinal);
    PivotGridEngine fEngine;
    object fItemsSource;
    IDisposable fOwnedDataAdapter;
    Pen fLinePen;
    Pen fSelectedCellBorderPen;
    Pen fResizePen;
    bool fIsFieldDragging;
    bool fIsFilterDialogOpen;
    bool fIsSettingsDialogOpen;
    bool fIsSettingsMenuItemsVisible = true;
    bool fIsExportMenuItemVisible = true;
    bool fShowFieldPanel = true;
    bool fShowToolTips = true;
    string fToolTipText = string.Empty;
    bool fIsVerticalScrollDragging;
    bool fIsHorizontalScrollDragging;
    bool fIsMeasureResizing;
    bool fIsRowHeaderResizing;
    int fCurrentRowIndex = -1;
    int fCurrentColumnIndex = -1;
    int fCurrentMeasureIndex = -1;
    int fMeasureResizeColumnIndex = -1;
    int fMeasureResizeIndex = -1;
    bool fShowRowGrandTotals = true;
    bool fShowColumnGrandTotals = true;
    double fHorizontalOffset;
    double fVerticalOffset;
    double fVerticalScrollDragOffset;
    double fHorizontalScrollDragOffset;
    double fMeasureResizeStartX;
    double fMeasureResizeCurrentX;
    double fMeasureResizeStartWidth;
    double fRowHeaderResizeStartX;
    double fRowHeaderResizeCurrentX;
    double fRowHeaderResizeStartWidth;
    PivotGridFieldRole fFieldDragSourceRole;
    PivotGridFieldRole fFieldDragTargetRole;
    int fFieldDragTargetIndex = -1;
    string fFieldDragName = string.Empty;
    string fFieldDragText = string.Empty;
    string fSettingsSuggestedFileName = "pivot-grid-settings.json";
    Point fFieldDragStartPoint;
    Point fFieldDragPoint;
    const double FieldChipGlyphWidth = 10;
    const double FieldChipGlyphGap = 5;
    const double FieldChipGlyphRightPadding = 8;
    const double FieldChipGlyphReservedWidth = 34;
    const double MeasureResizeGripWidth = 5;
    const double MeasureMinWidth = 40;
    const double MeasureAutoFitMaxWidth = 900;
    const double RowHeaderMinWidth = 120;
    const double RowHeaderAutoFitMaxWidth = 640;

    // ● private methods
    static IBrush CreateBrush(byte Red, byte Green, byte Blue)
    {
        return new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
    }
    Pen CreateLinePen()
    {
        return new Pen(GridLineBrush, 1);
    }
    Pen CreateSelectedCellBorderPen()
    {
        return new Pen(SelectedCellBorderBrush, 1);
    }
    Pen CreateResizePen()
    {
        return new Pen(ResizeGuideBrush, 1);
    }
    void UpdateThemePens()
    {
        fLinePen = CreateLinePen();
        fSelectedCellBorderPen = CreateSelectedCellBorderPen();
        fResizePen = CreateResizePen();
    }
    void Engine_Changed(object Sender, EventArgs Args)
    {
        EnsureCurrentCellInRange();
        ClampScrollOffsets();
        InvalidateVisual();
    }
    void AttachEngine(PivotGridEngine Engine)
    {
        if (Engine == null)
            return;

        Engine.DataAdapterChanged += Engine_Changed;
        Engine.ProjectionChanged += Engine_Changed;
        Engine.SortingChanged += Engine_Changed;
        Engine.FiltersChanged += Engine_Changed;
    }
    void DetachEngine(PivotGridEngine Engine)
    {
        if (Engine == null)
            return;

        Engine.DataAdapterChanged -= Engine_Changed;
        Engine.ProjectionChanged -= Engine_Changed;
        Engine.SortingChanged -= Engine_Changed;
        Engine.FiltersChanged -= Engine_Changed;
    }
    Type FindListItemType(object ItemsSource)
    {
        if (ItemsSource == null)
            return null;

        Type SourceType = ItemsSource.GetType();
        Type ListType = SourceType
            .GetInterfaces()
            .Concat(new[] { SourceType })
            .FirstOrDefault(Item => Item.IsGenericType && Item.GetGenericTypeDefinition() == typeof(IList<>));

        return ListType == null ? null : ListType.GetGenericArguments()[0];
    }
    DataView FindDataView(object ItemsSource)
    {
        if (ItemsSource is DataTable Table)
            return Table.DefaultView;
        if (ItemsSource is DataView View)
            return View;

        return null;
    }
    IPivotGridDataAdapter CreateDataAdapter(object ItemsSource)
    {
        if (ItemsSource == null)
            return null;

        DataView View = FindDataView(ItemsSource);
        if (View != null)
            return new PivotGridDataViewDataAdapter(View);

        Type ItemType = FindListItemType(ItemsSource);
        if (ItemType == null)
            throw new ArgumentException("ItemsSource must be a DataTable, DataView, or implement IList<T>.", nameof(ItemsSource));

        Type AdapterType = typeof(PivotGridListDataAdapter<>).MakeGenericType(ItemType);
        return (IPivotGridDataAdapter)Activator.CreateInstance(AdapterType, ItemsSource);
    }
    void CreateDefaultLayout()
    {
        if (DataAdapter == null || (RowFields.Count > 0 || ColumnFields.Count > 0 || Measures.Count > 0))
            return;

        PivotGridSourceField RowField = SourceFields.FirstOrDefault(Field => Field.CanUseAsAxis && !Field.CanUseAsMeasure);
        PivotGridSourceField ColumnField = SourceFields.FirstOrDefault(Field => Field.CanUseAsAxis && !ReferenceEquals(Field, RowField));
        PivotGridSourceField MeasureField = SourceFields.FirstOrDefault(Field => Field.IsNumeric);

        if (RowField != null)
            RowFields.Add(new PivotGridField { Name = RowField.Name, Header = RowField.Header });
        if (ColumnField != null)
            ColumnFields.Add(new PivotGridField { Name = ColumnField.Name, Header = ColumnField.Header });
        if (MeasureField != null)
            Measures.Add(new PivotGridMeasure { Name = MeasureField.Name, Header = MeasureField.Header, SourceFieldName = MeasureField.Name, AggregateKind = PivotGridAggregateKind.Sum });
    }
    void SetItemsSource(object Value)
    {
        if (ReferenceEquals(fItemsSource, Value))
            return;

        if (fOwnedDataAdapter != null)
        {
            fOwnedDataAdapter.Dispose();
            fOwnedDataAdapter = null;
        }

        fItemsSource = Value;
        IPivotGridDataAdapter Adapter = CreateDataAdapter(Value);
        fOwnedDataAdapter = Adapter as IDisposable;
        DataAdapter = Adapter;
        CreateDefaultLayout();
        Engine.Rebuild();
    }
    void ValidateSettingsFilePath(string FilePath)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ArgumentException("A settings file path is required.", nameof(FilePath));

        string Folder = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(Folder) && !Directory.Exists(Folder))
            throw new DirectoryNotFoundException(Folder);
    }
    bool ContainsFieldName(IEnumerable<string> FieldNames, string FieldName)
    {
        return !string.IsNullOrWhiteSpace(FieldName)
               && FieldNames.Any(Item => string.Equals(Item, FieldName, StringComparison.OrdinalIgnoreCase));
    }
    IReadOnlyList<PivotGridSourceField> GetAvailableFields()
    {
        List<string> UsedFieldNames = RowFields.Select(Field => Field.Name)
            .Concat(ColumnFields.Select(Field => Field.Name))
            .Concat(Measures.Select(Measure => Measure.SourceFieldName))
            .ToList();
        return SourceFields
            .Where(Field => !ContainsFieldName(UsedFieldNames, Field.Name))
            .ToList();
    }
    PivotGridSourceField FindSourceField(string FieldName)
    {
        return string.IsNullOrWhiteSpace(FieldName)
            ? null
            : SourceFields.FirstOrDefault(Field => string.Equals(Field.Name, FieldName, StringComparison.OrdinalIgnoreCase));
    }
    PivotGridFieldRole FindFieldRole(string FieldName, out int Index)
    {
        Index = -1;
        if (string.IsNullOrWhiteSpace(FieldName))
            return PivotGridFieldRole.None;

        for (int ItemIndex = 0; ItemIndex < RowFields.Count; ItemIndex++)
            if (string.Equals(RowFields[ItemIndex].Name, FieldName, StringComparison.OrdinalIgnoreCase))
            {
                Index = ItemIndex;
                return PivotGridFieldRole.Row;
            }
        for (int ItemIndex = 0; ItemIndex < ColumnFields.Count; ItemIndex++)
            if (string.Equals(ColumnFields[ItemIndex].Name, FieldName, StringComparison.OrdinalIgnoreCase))
            {
                Index = ItemIndex;
                return PivotGridFieldRole.Column;
            }
        for (int ItemIndex = 0; ItemIndex < Measures.Count; ItemIndex++)
            if (string.Equals(Measures[ItemIndex].SourceFieldName, FieldName, StringComparison.OrdinalIgnoreCase))
            {
                Index = ItemIndex;
                return PivotGridFieldRole.Measure;
            }

        return PivotGridFieldRole.Available;
    }
    void RemoveFieldFromRole(PivotGridFieldRole Role, int Index)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                if (Index >= 0 && Index < RowFields.Count)
                    RowFields.RemoveAt(Index);
                break;
            case PivotGridFieldRole.Column:
                if (Index >= 0 && Index < ColumnFields.Count)
                    ColumnFields.RemoveAt(Index);
                break;
            case PivotGridFieldRole.Measure:
                if (Index >= 0 && Index < Measures.Count)
                    Measures.RemoveAt(Index);
                break;
        }
    }
    int NormalizeInsertIndex(int Index, int Count)
    {
        if (Index < 0 || Index > Count)
            return Count;

        return Index;
    }
    int GetRoleItemCount(PivotGridFieldRole Role)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                return RowFields.Count;
            case PivotGridFieldRole.Column:
                return ColumnFields.Count;
            case PivotGridFieldRole.Measure:
                return Measures.Count;
        }

        return 0;
    }
    bool IsValidCellIndex(int RowIndex, int ColumnIndex, int MeasureIndex)
    {
        return RowIndex >= 0
               && RowIndex < GetSelectableRowCount()
               && ColumnIndex >= 0
               && ColumnIndex < GetSelectableColumnCount()
               && MeasureIndex >= 0
               && MeasureIndex < Engine.Measures.Count;
    }
    int GetSelectableRowCount()
    {
        return Engine.VisibleRowNodes.Count + (fShowColumnGrandTotals ? 1 : 0);
    }
    int GetSelectableColumnCount()
    {
        return Engine.ColumnItems.Count + (fShowRowGrandTotals ? 1 : 0);
    }
    bool SetCurrentCellCore(int RowIndex, int ColumnIndex, int MeasureIndex)
    {
        if (RowIndex != -1 || ColumnIndex != -1 || MeasureIndex != -1)
            if (!IsValidCellIndex(RowIndex, ColumnIndex, MeasureIndex))
                return false;

        if (fCurrentRowIndex == RowIndex && fCurrentColumnIndex == ColumnIndex && fCurrentMeasureIndex == MeasureIndex)
            return false;

        fCurrentRowIndex = RowIndex;
        fCurrentColumnIndex = ColumnIndex;
        fCurrentMeasureIndex = MeasureIndex;
        CurrentCellChanged?.Invoke(this, EventArgs.Empty);
        InvalidateVisual();
        return true;
    }
    double GetTotalValueWidth()
    {
        double Result = 0;
        foreach (PivotGridAxisItem ColumnItem in Engine.ColumnItems)
            Result += GetColumnGroupWidth(ColumnItem);
        if (fShowRowGrandTotals)
            Result += GetTotalGroupWidth();

        return Result;
    }
    double GetTotalRowHeight()
    {
        int RowCount = Engine.VisibleRowNodes.Count + (fShowColumnGrandTotals ? 1 : 0);
        return RowCount * LayoutMetrics.RowHeight;
    }
    double GetBodyTop()
    {
        return GetGridTop() + LayoutMetrics.ColumnHeaderHeight;
    }
    double GetHorizontalScrollBarHeight(bool HasHorizontalScrollBar)
    {
        return HasHorizontalScrollBar ? LayoutMetrics.HorizontalScrollBarHeight : 0;
    }
    double GetVerticalScrollBarWidth(bool HasVerticalScrollBar)
    {
        return HasVerticalScrollBar ? LayoutMetrics.VerticalScrollBarWidth : 0;
    }
    void GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect)
    {
        double RowHeaderWidth = GetRowHeaderWidth();
        double BodyTop = GetBodyTop();
        double AvailableWidth = Math.Max(0, Bounds.Width - RowHeaderWidth);
        double AvailableHeight = Math.Max(0, Bounds.Height - BodyTop);
        double TotalValueWidth = GetTotalValueWidth();
        double TotalRowHeight = GetTotalRowHeight();

        HasVerticalScrollBar = TotalRowHeight > AvailableHeight && LayoutMetrics.VerticalScrollBarWidth > 0;
        HasHorizontalScrollBar = TotalValueWidth > Math.Max(0, AvailableWidth - GetVerticalScrollBarWidth(HasVerticalScrollBar)) && LayoutMetrics.HorizontalScrollBarHeight > 0;
        if (!HasVerticalScrollBar)
            HasVerticalScrollBar = TotalRowHeight > Math.Max(0, AvailableHeight - GetHorizontalScrollBarHeight(HasHorizontalScrollBar)) && LayoutMetrics.VerticalScrollBarWidth > 0;
        if (!HasHorizontalScrollBar)
            HasHorizontalScrollBar = TotalValueWidth > Math.Max(0, AvailableWidth - GetVerticalScrollBarWidth(HasVerticalScrollBar)) && LayoutMetrics.HorizontalScrollBarHeight > 0;

        double BodyWidth = Math.Max(0, AvailableWidth - GetVerticalScrollBarWidth(HasVerticalScrollBar));
        double BodyHeight = Math.Max(0, AvailableHeight - GetHorizontalScrollBarHeight(HasHorizontalScrollBar));
        BodyRect = new Rect(RowHeaderWidth, BodyTop, BodyWidth, BodyHeight);
        HorizontalTrackRect = HasHorizontalScrollBar ? new Rect(RowHeaderWidth, BodyRect.Bottom, BodyWidth, LayoutMetrics.HorizontalScrollBarHeight) : default;
        VerticalTrackRect = HasVerticalScrollBar ? new Rect(BodyRect.Right, BodyTop, LayoutMetrics.VerticalScrollBarWidth, BodyHeight) : default;
    }
    double GetMaxHorizontalOffset()
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        return HasHorizontalScrollBar ? Math.Max(0, GetTotalValueWidth() - BodyRect.Width) : 0;
    }
    double GetMaxVerticalOffset()
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        return HasVerticalScrollBar ? Math.Max(0, GetTotalRowHeight() - BodyRect.Height) : 0;
    }
    Rect GetVerticalScrollThumbRect()
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (!HasVerticalScrollBar || VerticalTrackRect.Width <= 0 || VerticalTrackRect.Height <= 0)
            return default;

        double TotalHeight = GetTotalRowHeight();
        double ThumbHeight = Math.Max(LayoutMetrics.VerticalScrollThumbMinHeight, VerticalTrackRect.Height * VerticalTrackRect.Height / TotalHeight);
        ThumbHeight = Math.Min(VerticalTrackRect.Height, ThumbHeight);
        double MaxOffset = Math.Max(0, TotalHeight - VerticalTrackRect.Height);
        double Top = VerticalTrackRect.Y;
        if (MaxOffset > 0 && VerticalTrackRect.Height > ThumbHeight)
            Top += (VerticalTrackRect.Height - ThumbHeight) * fVerticalOffset / MaxOffset;

        return new Rect(VerticalTrackRect.X + 2, Top + 2, Math.Max(0, VerticalTrackRect.Width - 4), Math.Max(0, ThumbHeight - 4));
    }
    Rect GetHorizontalScrollThumbRect()
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (!HasHorizontalScrollBar || HorizontalTrackRect.Width <= 0 || HorizontalTrackRect.Height <= 0)
            return default;

        double TotalWidth = GetTotalValueWidth();
        double ThumbWidth = Math.Max(LayoutMetrics.HorizontalScrollThumbMinWidth, HorizontalTrackRect.Width * HorizontalTrackRect.Width / TotalWidth);
        ThumbWidth = Math.Min(HorizontalTrackRect.Width, ThumbWidth);
        double MaxOffset = Math.Max(0, TotalWidth - HorizontalTrackRect.Width);
        double Left = HorizontalTrackRect.X;
        if (MaxOffset > 0 && HorizontalTrackRect.Width > ThumbWidth)
            Left += (HorizontalTrackRect.Width - ThumbWidth) * fHorizontalOffset / MaxOffset;

        return new Rect(Left + 2, HorizontalTrackRect.Y + 2, Math.Max(0, ThumbWidth - 4), Math.Max(0, HorizontalTrackRect.Height - 4));
    }
    void ClampScrollOffsets()
    {
        fHorizontalOffset = Math.Clamp(fHorizontalOffset, 0, GetMaxHorizontalOffset());
        fVerticalOffset = Math.Clamp(fVerticalOffset, 0, GetMaxVerticalOffset());
    }
    bool SetHorizontalOffsetCore(double Value)
    {
        double NewValue = Math.Clamp(Value, 0, GetMaxHorizontalOffset());
        if (Math.Abs(NewValue - fHorizontalOffset) < 0.1)
            return false;

        fHorizontalOffset = NewValue;
        InvalidateVisual();
        return true;
    }
    bool SetVerticalOffsetFromScroll(double Y)
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        Rect ThumbRect = GetVerticalScrollThumbRect();
        double MaxOffset = Math.Max(0, GetTotalRowHeight() - VerticalTrackRect.Height);
        double Range = VerticalTrackRect.Height - ThumbRect.Height;
        if (!HasVerticalScrollBar || Range <= 0 || MaxOffset <= 0)
            return false;

        double Ratio = (Y - VerticalTrackRect.Y - fVerticalScrollDragOffset) / Range;
        return SetVerticalOffsetCore(Math.Clamp(Ratio, 0, 1) * MaxOffset);
    }
    bool SetHorizontalOffsetFromScroll(double X)
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        Rect ThumbRect = GetHorizontalScrollThumbRect();
        double MaxOffset = Math.Max(0, GetTotalValueWidth() - HorizontalTrackRect.Width);
        double Range = HorizontalTrackRect.Width - ThumbRect.Width;
        if (!HasHorizontalScrollBar || Range <= 0 || MaxOffset <= 0)
            return false;

        double Ratio = (X - HorizontalTrackRect.X - fHorizontalScrollDragOffset) / Range;
        return SetHorizontalOffsetCore(Math.Clamp(Ratio, 0, 1) * MaxOffset);
    }
    bool SetVerticalOffsetCore(double Value)
    {
        double NewValue = Math.Clamp(Value, 0, GetMaxVerticalOffset());
        if (Math.Abs(NewValue - fVerticalOffset) < 0.1)
            return false;

        fVerticalOffset = NewValue;
        InvalidateVisual();
        return true;
    }
    void EnsureCurrentCellInRange()
    {
        if (fCurrentRowIndex == -1 && fCurrentColumnIndex == -1 && fCurrentMeasureIndex == -1)
            return;
        if (IsValidCellIndex(fCurrentRowIndex, fCurrentColumnIndex, fCurrentMeasureIndex))
            return;

        fCurrentRowIndex = -1;
        fCurrentColumnIndex = -1;
        fCurrentMeasureIndex = -1;
        CurrentCellChanged?.Invoke(this, EventArgs.Empty);
    }
    bool ReorderFieldInRole(PivotGridFieldRole Role, int SourceIndex, int TargetIndex)
    {
        int Count = GetRoleItemCount(Role);
        if (SourceIndex < 0 || SourceIndex >= Count)
            return false;

        TargetIndex = NormalizeInsertIndex(TargetIndex, Count);
        if (TargetIndex > SourceIndex)
            TargetIndex--;
        if (TargetIndex == SourceIndex)
            return false;

        switch (Role)
        {
            case PivotGridFieldRole.Row:
                RowFields.Move(SourceIndex, TargetIndex);
                break;
            case PivotGridFieldRole.Column:
                ColumnFields.Move(SourceIndex, TargetIndex);
                break;
            case PivotGridFieldRole.Measure:
                Measures.Move(SourceIndex, TargetIndex);
                break;
            default:
                return false;
        }

        Engine.Rebuild();
        InvalidateVisual();
        return true;
    }
    bool MoveFieldToRole(string FieldName, PivotGridFieldRole Role, int TargetIndex)
    {
        PivotGridSourceField SourceField = FindSourceField(FieldName);
        if (SourceField == null)
            return false;

        switch (Role)
        {
            case PivotGridFieldRole.Available:
                Engine.Rebuild();
                InvalidateVisual();
                return true;
            case PivotGridFieldRole.Row:
                if (!SourceField.CanUseAsAxis || ContainsFieldName(RowFields.Select(Field => Field.Name), FieldName))
                    return false;
                RowFields.Insert(NormalizeInsertIndex(TargetIndex, RowFields.Count), new PivotGridField { Name = SourceField.Name, Header = SourceField.Header });
                break;
            case PivotGridFieldRole.Column:
                if (!SourceField.CanUseAsAxis || ContainsFieldName(ColumnFields.Select(Field => Field.Name), FieldName))
                    return false;
                ColumnFields.Insert(NormalizeInsertIndex(TargetIndex, ColumnFields.Count), new PivotGridField { Name = SourceField.Name, Header = SourceField.Header });
                break;
            case PivotGridFieldRole.Measure:
                if (!SourceField.CanUseAsMeasure || ContainsFieldName(Measures.Select(Measure => Measure.SourceFieldName), FieldName))
                    return false;
                Measures.Insert(NormalizeInsertIndex(TargetIndex, Measures.Count), new PivotGridMeasure { Name = SourceField.Name, Header = SourceField.Header, SourceFieldName = SourceField.Name, AggregateKind = PivotGridAggregateKind.Sum });
                break;
            default:
                return false;
        }

        Engine.Rebuild();
        InvalidateVisual();
        return true;
    }
    bool MoveFieldCore(string FieldName, PivotGridFieldRole TargetRole, int TargetIndex)
    {
        if (TargetRole == PivotGridFieldRole.None)
            return false;

        PivotGridFieldRole SourceRole = FindFieldRole(FieldName, out int SourceIndex);
        if (SourceRole == TargetRole)
            return ReorderFieldInRole(SourceRole, SourceIndex, TargetIndex);

        RemoveFieldFromRole(SourceRole, SourceIndex);
        if (TargetRole == PivotGridFieldRole.Available)
        {
            Engine.Rebuild();
            InvalidateVisual();
            return true;
        }

        if (MoveFieldToRole(FieldName, TargetRole, TargetIndex))
            return true;

        MoveFieldToRole(FieldName, SourceRole, SourceIndex);
        return false;
    }
    FormattedText CreateText(string Text, IBrush Brush, double MaxWidth, FontWeight Weight = FontWeight.Normal)
    {
        FormattedText Result = new(Text ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI", FontStyle.Normal, Weight, FontStretch.Normal), 12, Brush);
        Result.MaxTextWidth = Math.Max(0, MaxWidth);
        Result.MaxLineCount = 1;
        Result.Trimming = TextTrimming.CharacterEllipsis;
        return Result;
    }
    double MeasureTextWidth(string Text, FontWeight Weight = FontWeight.Normal)
    {
        try
        {
            return CreateText(Text, TextBrush, 10000, Weight).Width;
        }
        catch (InvalidOperationException)
        {
            double Factor = Weight >= FontWeight.SemiBold ? 7.2 : 6.8;
            return (Text ?? string.Empty).Length * Factor;
        }
    }
    void DrawText(DrawingContext Context, string Text, Rect Rect, IBrush Brush, FontWeight Weight = FontWeight.Normal, bool AlignRight = false)
    {
        if (string.IsNullOrEmpty(Text) || Rect.Width <= 4 || Rect.Height <= 4)
            return;

        using (Context.PushClip(Rect))
        {
            FormattedText FormattedText = CreateText(Text, Brush, Rect.Width - 8, Weight);
            double X = AlignRight ? Rect.Right - FormattedText.Width - 4 : Rect.X + 4;
            double Y = Rect.Y + Math.Max(2, (Rect.Height - FormattedText.Height) / 2);
            Context.DrawText(FormattedText, new Point(X, Y));
        }
    }
    void DrawBand(DrawingContext Context, Rect Rect, IBrush Brush)
    {
        Context.DrawRectangle(Brush, fLinePen, Rect);
    }
    double GetFieldPanelHeight()
    {
        return ShowFieldPanel ? LayoutMetrics.FieldPanelHeight : 0;
    }
    double GetGridTop()
    {
        return GetFieldPanelHeight() + LayoutMetrics.AxisPanelHeight;
    }
    string GetSourceFieldText(PivotGridSourceField Field)
    {
        return Field == null ? string.Empty : string.IsNullOrWhiteSpace(Field.Header) ? Field.Name : Field.Header;
    }
    string GetAxisFieldText(PivotGridField Field)
    {
        return Field == null ? string.Empty : string.IsNullOrWhiteSpace(Field.Header) ? Field.Name : Field.Header;
    }
    string GetMeasureText(PivotGridMeasure Measure)
    {
        if (Measure == null)
            return string.Empty;

        string Text = string.IsNullOrWhiteSpace(Measure.Header) ? Measure.Name : Measure.Header;
        return $"{Text} ({Measure.AggregateKind})";
    }
    bool IsRightAlignedType(Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return PivotGridFieldRules.IsNumericType(Type)
               || Type == typeof(DateTime)
               || Type == typeof(DateTimeOffset);
    }
    bool IsRightAlignedField(string FieldName)
    {
        PivotGridSourceField Field = FindSourceField(FieldName);
        return Field != null && IsRightAlignedType(Field.ValueType);
    }
    double GetFieldChipWidth(string Text)
    {
        return Math.Max(72, Math.Min(210, (Text ?? string.Empty).Length * 8 + 26 + FieldChipGlyphReservedWidth));
    }
    double GetChipBandWidth(IEnumerable<string> Texts)
    {
        double Width = 84;
        foreach (string Text in Texts)
            Width += GetFieldChipWidth(Text) + 6;

        return Width;
    }
    double GetLeftHeaderWidth()
    {
        return Math.Max(LayoutMetrics.RowHeaderWidth, GetLeftHeaderContentWidth());
    }
    double GetRowHeaderWidth()
    {
        return GetLeftHeaderWidth();
    }
    double GetLeftHeaderContentWidth()
    {
        double RowWidth = GetChipBandWidth(RowFields.Select(GetAxisFieldText));
        double MeasureWidth = GetChipBandWidth(Measures.Select(GetMeasureText));
        return Math.Max(RowWidth, MeasureWidth);
    }
    Rect GetFieldChipRect(double X, double Y, string Text)
    {
        return new Rect(X, Y, GetFieldChipWidth(Text), 24);
    }
    void DrawFieldChip(DrawingContext Context, Rect Rect, string Text)
    {
        Context.DrawRectangle(GridBackgroundBrush, fLinePen, Rect, 3, 3);
        DrawText(Context, Text, new Rect(Rect.X, Rect.Y, Math.Max(0, Rect.Width - FieldChipGlyphReservedWidth), Rect.Height), TextBrush, FontWeight.SemiBold, false);
    }
    bool IsSortedField(PivotGridFieldRole Role, string FieldName)
    {
        return Engine.SortRole == Role
               && Engine.SortDirection != PivotGridSortDirection.None
               && !string.IsNullOrWhiteSpace(FieldName)
               && string.Equals(Engine.SortFieldName, FieldName, StringComparison.OrdinalIgnoreCase);
    }
    double GetFieldGlyphCenterX(Rect Rect, int SlotIndex)
    {
        return Rect.Right - FieldChipGlyphRightPadding - (FieldChipGlyphWidth / 2) - (SlotIndex * (FieldChipGlyphWidth + FieldChipGlyphGap));
    }
    void DrawSortGlyph(DrawingContext Context, Rect Rect, PivotGridSortDirection Direction, int SlotIndex)
    {
        if (Direction == PivotGridSortDirection.None)
            return;

        double CenterX = GetFieldGlyphCenterX(Rect, SlotIndex);
        double CenterY = Rect.Y + (Rect.Height / 2);
        double Size = 8;
        StreamGeometry Geometry = new();

        using (StreamGeometryContext GeometryContext = Geometry.Open())
        {
            if (Direction == PivotGridSortDirection.Ascending)
            {
                GeometryContext.BeginFigure(new Point(CenterX, CenterY - (Size / 2)), true);
                GeometryContext.LineTo(new Point(CenterX + (Size / 2), CenterY + (Size / 2)));
                GeometryContext.LineTo(new Point(CenterX - (Size / 2), CenterY + (Size / 2)));
            }
            else
            {
                GeometryContext.BeginFigure(new Point(CenterX - (Size / 2), CenterY - (Size / 2)), true);
                GeometryContext.LineTo(new Point(CenterX + (Size / 2), CenterY - (Size / 2)));
                GeometryContext.LineTo(new Point(CenterX, CenterY + (Size / 2)));
            }

            GeometryContext.EndFigure(true);
        }

        Context.DrawGeometry(MutedTextBrush, null, Geometry);
    }
    void DrawSortableFieldChip(DrawingContext Context, Rect Rect, string Text, PivotGridFieldRole Role, string FieldName)
    {
        bool IsSorted = IsSortedField(Role, FieldName);
        bool IsFiltered = Engine.IsFieldFiltered(FieldName);
        DrawFieldChip(Context, Rect, Text);
        if (IsSorted)
            DrawSortGlyph(Context, Rect, Engine.SortDirection, IsFiltered ? 1 : 0);
        if (IsFiltered)
            DrawFilterGlyph(Context, Rect, 0);
    }
    void DrawFilterGlyph(DrawingContext Context, Rect Rect, int SlotIndex)
    {
        double X = GetFieldGlyphCenterX(Rect, SlotIndex) - (FieldChipGlyphWidth / 2);
        double Y = Rect.Y + 8;
        StreamGeometry Geometry = new();

        using (StreamGeometryContext GeometryContext = Geometry.Open())
        {
            GeometryContext.BeginFigure(new Point(X, Y), true);
            GeometryContext.LineTo(new Point(X + 10, Y));
            GeometryContext.LineTo(new Point(X + 6, Y + 5));
            GeometryContext.LineTo(new Point(X + 6, Y + 9));
            GeometryContext.LineTo(new Point(X + 4, Y + 11));
            GeometryContext.LineTo(new Point(X + 4, Y + 5));
            GeometryContext.EndFigure(true);
        }

        Context.DrawGeometry(MutedTextBrush, null, Geometry);
    }
    void DrawAvailableFieldsPanel(DrawingContext Context)
    {
        if (!ShowFieldPanel)
            return;

        Rect PanelRect = new(0, 0, Bounds.Width, GetFieldPanelHeight());
        DrawBand(Context, PanelRect, HeaderBrush);
        DrawText(Context, "Available Fields", new Rect(8, 0, 120, PanelRect.Height), MutedTextBrush, FontWeight.SemiBold);

        double X = 130;
        double Y = Math.Max(0, (PanelRect.Height - 24) / 2);
        foreach (PivotGridSourceField Field in AvailableFields)
        {
            string Text = GetSourceFieldText(Field);
            Rect ChipRect = GetFieldChipRect(X, Y, Text);
            if (ChipRect.X >= Bounds.Width)
                break;
            DrawFieldChip(Context, ChipRect, Text);
            if (Engine.IsFieldFiltered(Field.Name))
                DrawFilterGlyph(Context, ChipRect, 0);
            X = ChipRect.Right + 6;
        }
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
    IEnumerable<MenuItem> CreateAggregateMenuItems(PivotGridMeasure Measure)
    {
        return Enum.GetValues(typeof(PivotGridAggregateKind))
            .Cast<PivotGridAggregateKind>()
            .Select(AggregateKind =>
            {
                string Header = AggregateKind == Measure?.AggregateKind ? "* " + AggregateKind : AggregateKind.ToString();
                return CreateMenuItem("Aggregate " + Header, CanMeasureAggregate(Measure, AggregateKind), () => SetMeasureAggregate(Measure, AggregateKind));
            });
    }
    IEnumerable<MenuItem> CreateExportMenuItems()
    {
        return PivotGridExporters.CreateExporters()
            .Select(Exporter => CreateMenuItem("Export " + Exporter.Name, true, () => ExportAsync(Exporter)));
    }
    bool ShowGridContextMenu(Point Point)
    {
        PivotGridHitTestResult Hit = HitTest(Point);
        bool HasValueCell = Hit.Kind == PivotGridHitTestKind.ValueCell;
        if (HasValueCell)
            SetCurrentCellCore(Hit.RowIndex, Hit.ColumnIndex, Hit.MeasureIndex);

        ContextMenu Menu = new()
        {
            Placement = PlacementMode.Pointer,
        };
        List<object> Items = new()
        {
            CreateMenuItem("Copy Cell", HasValueCell && CurrentCell != null, () => _ = CopyCurrentCellTextAsync()),
            CreateMenuItem("Copy Pivot", Engine.Measures.Count > 0, () => _ = CopyPivotTextAsync()),
            new Separator(),
            CreateMenuItem("Expand All Rows", Engine.CanExpandRows, () => ExpandAllRows()),
            CreateMenuItem("Collapse All Rows", Engine.CanCollapseRows, () => CollapseAllRows()),
            new Separator(),
            CreateMenuItem("Auto Fit Row Header", Engine.VisibleRowNodes.Count > 0, () => AutoFitRowHeaderWidth()),
            CreateMenuItem("Auto Fit Column Widths", Engine.Measures.Count > 0, () => AutoFitValueColumnWidths()),
            CreateMenuItem("Reset Row Header Width", CanResetRowHeaderWidth(), () => ResetRowHeaderWidth()),
            CreateMenuItem("Reset Column Widths", fValueColumnWidths.Count > 0, () => ClearValueColumnWidths()),
            new Separator(),
            CreateCheckedMenuItem("Field Panel", ShowFieldPanel, () => ShowFieldPanel = !ShowFieldPanel),
            CreateCheckedMenuItem("Row Totals", ShowRowGrandTotals, () => ShowRowGrandTotals = !ShowRowGrandTotals),
            CreateCheckedMenuItem("Column Totals", ShowColumnGrandTotals, () => ShowColumnGrandTotals = !ShowColumnGrandTotals),
            CreateCheckedMenuItem("Tooltips", ShowToolTips, () => ShowToolTips = !ShowToolTips),
        };
        if (fIsSettingsMenuItemsVisible)
        {
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Settings...", true, () => ShowSettingsDialogAsync()));
            Items.Add(CreateMenuItem("Save Settings...", true, SaveSettingsAsync));
            Items.Add(CreateMenuItem("Load Settings...", true, LoadSettingsAsync));
        }
        if (fIsExportMenuItemVisible)
        {
            Items.Add(new Separator());
            Items.AddRange(CreateExportMenuItems());
        }

        Menu.ItemsSource = Items;
        Menu.Open(this);
        return true;
    }
    bool GetHitField(PivotGridHitTestResult Hit, out PivotGridFieldRole Role, out string FieldName, out string Text)
    {
        Role = PivotGridFieldRole.None;
        FieldName = string.Empty;
        Text = string.Empty;
        if (Hit == null)
            return false;

        switch (Hit.Kind)
        {
            case PivotGridHitTestKind.AvailableField:
                Role = PivotGridFieldRole.Available;
                FieldName = Hit.SourceField?.Name;
                Text = GetSourceFieldText(Hit.SourceField);
                break;
            case PivotGridHitTestKind.RowField:
                Role = PivotGridFieldRole.Row;
                FieldName = Hit.RowIndex >= 0 && Hit.RowIndex < RowFields.Count ? RowFields[Hit.RowIndex].Name : string.Empty;
                Text = Hit.RowIndex >= 0 && Hit.RowIndex < RowFields.Count ? GetAxisFieldText(RowFields[Hit.RowIndex]) : string.Empty;
                break;
            case PivotGridHitTestKind.ColumnField:
                Role = PivotGridFieldRole.Column;
                FieldName = Hit.ColumnIndex >= 0 && Hit.ColumnIndex < ColumnFields.Count ? ColumnFields[Hit.ColumnIndex].Name : string.Empty;
                Text = Hit.ColumnIndex >= 0 && Hit.ColumnIndex < ColumnFields.Count ? GetAxisFieldText(ColumnFields[Hit.ColumnIndex]) : string.Empty;
                break;
            case PivotGridHitTestKind.MeasureField:
                Role = PivotGridFieldRole.Measure;
                FieldName = Hit.Measure?.SourceFieldName;
                Text = GetMeasureText(Hit.Measure);
                break;
        }

        return Role != PivotGridFieldRole.None && !string.IsNullOrWhiteSpace(FieldName);
    }
    bool IsFieldSorted(PivotGridFieldRole Role, string FieldName)
    {
        return Engine.SortRole == Role
               && Engine.SortDirection != PivotGridSortDirection.None
               && string.Equals(Engine.SortFieldName, FieldName, StringComparison.OrdinalIgnoreCase);
    }
    bool ShowFieldContextMenu(Point Point)
    {
        PivotGridHitTestResult Hit = HitTest(Point);
        if (!GetHitField(Hit, out PivotGridFieldRole Role, out string FieldName, out string Text))
            return false;

        PivotGridSourceField SourceField = FindSourceField(FieldName);
        bool CanUseAsAxis = SourceField != null && SourceField.CanUseAsAxis;
        bool CanUseAsMeasure = SourceField != null && SourceField.CanUseAsMeasure;
        bool CanSort = Role == PivotGridFieldRole.Row || Role == PivotGridFieldRole.Column;
        bool IsSorted = IsFieldSorted(Role, FieldName);
        ContextMenu Menu = new()
        {
            Placement = PlacementMode.Pointer,
        };
        List<object> Items = new()
        {
            CreateMenuItem(Text, false, null),
            new Separator(),
        };

        if (CanSort)
        {
            Items.Add(CreateMenuItem("Sort Ascending", !IsSorted || Engine.SortDirection != PivotGridSortDirection.Ascending, () => SetSort(Role, FieldName, PivotGridSortDirection.Ascending)));
            Items.Add(CreateMenuItem("Sort Descending", !IsSorted || Engine.SortDirection != PivotGridSortDirection.Descending, () => SetSort(Role, FieldName, PivotGridSortDirection.Descending)));
            Items.Add(CreateMenuItem("Clear Sorting", IsSorted, () => ClearSort()));
            Items.Add(new Separator());
        }
        if (Role == PivotGridFieldRole.Measure)
        {
            Items.AddRange(CreateAggregateMenuItems(Hit.Measure));
            Items.Add(new Separator());
        }

        Items.Add(CreateMenuItem("Filter...", SourceField != null, () => RequestFieldFilter(SourceField)));
        Items.Add(CreateMenuItem("Clear Field Filter", SourceField != null && Engine.IsFieldFiltered(FieldName), () => ClearFieldFilter(FieldName)));
        Items.Add(CreateMenuItem("Clear All Filters", Engine.HasFilters, () => ClearFilters()));
        Items.Add(new Separator());
        Items.Add(CreateMenuItem("Move to Rows", CanUseAsAxis && Role != PivotGridFieldRole.Row, () => MoveField(FieldName, PivotGridFieldRole.Row)));
        Items.Add(CreateMenuItem("Move to Columns", CanUseAsAxis && Role != PivotGridFieldRole.Column, () => MoveField(FieldName, PivotGridFieldRole.Column)));
        Items.Add(CreateMenuItem("Move to Values", CanUseAsMeasure && Role != PivotGridFieldRole.Measure, () => MoveField(FieldName, PivotGridFieldRole.Measure)));
        Items.Add(CreateMenuItem("Move to Available", Role != PivotGridFieldRole.Available, () => MoveField(FieldName, PivotGridFieldRole.Available)));
        Items.Add(new Separator());
        Items.Add(CreateMenuItem("Auto Fit Row Header", Engine.VisibleRowNodes.Count > 0, () => AutoFitRowHeaderWidth()));
        Items.Add(CreateMenuItem("Auto Fit Column Widths", Engine.Measures.Count > 0, () => AutoFitValueColumnWidths()));
        Items.Add(CreateMenuItem("Reset Row Header Width", CanResetRowHeaderWidth(), () => ResetRowHeaderWidth()));
        Items.Add(CreateMenuItem("Reset Column Widths", fValueColumnWidths.Count > 0, () => ClearValueColumnWidths()));
        Items.Add(new Separator());
        Items.Add(CreateCheckedMenuItem("Field Panel", ShowFieldPanel, () => ShowFieldPanel = !ShowFieldPanel));
        Items.Add(CreateCheckedMenuItem("Row Totals", ShowRowGrandTotals, () => ShowRowGrandTotals = !ShowRowGrandTotals));
        Items.Add(CreateCheckedMenuItem("Column Totals", ShowColumnGrandTotals, () => ShowColumnGrandTotals = !ShowColumnGrandTotals));
        Items.Add(CreateCheckedMenuItem("Tooltips", ShowToolTips, () => ShowToolTips = !ShowToolTips));
        if (fIsSettingsMenuItemsVisible)
        {
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Settings...", true, () => ShowSettingsDialogAsync()));
            Items.Add(CreateMenuItem("Save Settings...", true, SaveSettingsAsync));
            Items.Add(CreateMenuItem("Load Settings...", true, LoadSettingsAsync));
        }
        if (fIsExportMenuItemVisible)
        {
            Items.Add(new Separator());
            Items.AddRange(CreateExportMenuItems());
        }

        Menu.ItemsSource = Items;
        Menu.Open(this);
        return true;
    }
    PivotGridFieldSettingsItem CreateFieldSettingsItem(PivotGridSourceField SourceField)
    {
        PivotGridFieldRole Role = FindFieldRole(SourceField.Name, out int Index);
        PivotGridAggregateKind AggregateKind = Role == PivotGridFieldRole.Measure && Index >= 0 && Index < Measures.Count
            ? Measures[Index].AggregateKind
            : PivotGridAggregateKind.Sum;
        string Header = SourceField.Header;
        string DisplayFormat = string.Empty;
        double Width = 0;
        if (Role == PivotGridFieldRole.Row && Index >= 0 && Index < RowFields.Count)
        {
            Header = RowFields[Index].Header;
            DisplayFormat = RowFields[Index].DisplayFormat;
            Width = RowFields[Index].Width;
        }
        else if (Role == PivotGridFieldRole.Column && Index >= 0 && Index < ColumnFields.Count)
        {
            Header = ColumnFields[Index].Header;
            DisplayFormat = ColumnFields[Index].DisplayFormat;
            Width = ColumnFields[Index].Width;
        }
        else if (Role == PivotGridFieldRole.Measure && Index >= 0 && Index < Measures.Count)
        {
            Header = Measures[Index].Header;
            DisplayFormat = Measures[Index].DisplayFormat;
            Width = Measures[Index].Width;
        }

        return new PivotGridFieldSettingsItem
        {
            Name = SourceField.Name,
            Header = Header,
            CanUseAsAxis = SourceField.CanUseAsAxis,
            CanUseAsMeasure = SourceField.CanUseAsMeasure,
            Role = Role,
            AggregateKind = AggregateKind,
            DisplayFormat = DisplayFormat,
            Width = Width,
        };
    }
    List<PivotGridFieldSettingsItem> CreateFieldSettingsItems()
    {
        Dictionary<string, PivotGridSourceField> SourceMap = SourceFields.ToDictionary(Field => Field.Name, StringComparer.OrdinalIgnoreCase);
        List<string> OrderedNames = RowFields.Select(Field => Field.Name)
            .Concat(ColumnFields.Select(Field => Field.Name))
            .Concat(Measures.Select(Measure => Measure.SourceFieldName))
            .Concat(SourceFields.Select(Field => Field.Name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        List<PivotGridFieldSettingsItem> Result = new();
        foreach (string Name in OrderedNames)
            if (SourceMap.TryGetValue(Name, out PivotGridSourceField Field))
                Result.Add(CreateFieldSettingsItem(Field));

        return Result;
    }
    void ApplyFieldSettingsItems(IEnumerable<PivotGridFieldSettingsItem> Items)
    {
        if (Items == null)
            return;

        RowFields.Clear();
        ColumnFields.Clear();
        Measures.Clear();
        foreach (PivotGridFieldSettingsItem Item in Items)
        {
            switch (Item.Role)
            {
                case PivotGridFieldRole.Row:
                    if (Item.CanUseAsAxis)
                        RowFields.Add(new PivotGridField { Name = Item.Name, Header = Item.Header, DisplayFormat = Item.DisplayFormat, Width = Item.Width });
                    break;
                case PivotGridFieldRole.Column:
                    if (Item.CanUseAsAxis)
                        ColumnFields.Add(new PivotGridField { Name = Item.Name, Header = Item.Header, DisplayFormat = Item.DisplayFormat, Width = Item.Width });
                    break;
                case PivotGridFieldRole.Measure:
                    if (Item.CanUseAsMeasure)
                        Measures.Add(new PivotGridMeasure { Name = Item.Name, Header = Item.Header, SourceFieldName = Item.Name, AggregateKind = Item.AggregateKind, DisplayFormat = Item.DisplayFormat, Width = Item.Width });
                    break;
            }
        }

        Engine.Rebuild();
        InvalidateVisual();
    }
    bool CanMeasureAggregate(PivotGridMeasure Measure, PivotGridAggregateKind AggregateKind)
    {
        if (Measure == null)
            return false;
        if (AggregateKind == PivotGridAggregateKind.Count)
            return true;

        PivotGridSourceField SourceField = FindSourceField(Measure.SourceFieldName);
        Type ValueType = SourceField?.ValueType;
        if (ValueType == null)
            return false;

        if (AggregateKind == PivotGridAggregateKind.Sum || AggregateKind == PivotGridAggregateKind.Average)
            return PivotGridFieldRules.IsNumericType(ValueType);

        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return typeof(IComparable).IsAssignableFrom(Type);
    }
    bool SetMeasureAggregate(PivotGridMeasure Measure, PivotGridAggregateKind AggregateKind)
    {
        if (!CanMeasureAggregate(Measure, AggregateKind))
            return false;
        if (Measure.AggregateKind == AggregateKind)
            return false;

        Measure.AggregateKind = AggregateKind;
        Engine.Rebuild();
        InvalidateVisual();
        return true;
    }
    async Task<bool> ShowSettingsDialogCoreAsync()
    {
        if (fIsSettingsDialogOpen)
            return false;

        Window Owner = TopLevel.GetTopLevel(this) as Window;
        if (Owner == null)
            return false;

        PivotGridSettingsDialog Dialog = new(CreateFieldSettingsItems(), ShowFieldPanel, ShowRowGrandTotals, ShowColumnGrandTotals, ShowToolTips);
        fIsSettingsDialogOpen = true;
        try
        {
            bool Result = await Dialog.ShowDialog<bool>(Owner);
            if (Result)
            {
                ApplyFieldSettingsItems(Dialog.Items);
                ShowFieldPanel = Dialog.ShowFieldPanel;
                ShowRowGrandTotals = Dialog.ShowRowGrandTotals;
                ShowColumnGrandTotals = Dialog.ShowColumnGrandTotals;
                ShowToolTips = Dialog.ShowToolTips;
            }

            return Result;
        }
        finally
        {
            fIsSettingsDialogOpen = false;
        }
    }
    async void RequestFieldFilter(PivotGridSourceField Field)
    {
        if (Field == null || fIsFilterDialogOpen)
            return;

        Window Owner = TopLevel.GetTopLevel(this) as Window;
        if (Owner == null)
            return;

        List<object> Values = GetDistinctFieldValues(Field.Name);
        List<PivotGridFilterValueItem> Items = Values
            .Select(Value => new PivotGridFilterValueItem
            {
                Value = Value,
                Text = FormatSourceValue(Field, Value),
                IsChecked = !Engine.IsFieldFiltered(Field.Name) || Engine.IsFilterValueAccepted(Field.Name, Value),
            })
            .ToList();
        PivotGridFilterDialog Dialog = new(GetSourceFieldText(Field), Items);
        fIsFilterDialogOpen = true;
        try
        {
            bool Result = await Dialog.ShowDialog<bool>(Owner);
            if (!Result)
                return;

            IReadOnlyList<object> SelectedValues = Dialog.SelectedValues;
            if (SelectedValues.Count == Values.Count)
                ClearFieldFilter(Field.Name);
            else
                SetFieldFilter(Field.Name, SelectedValues);
        }
        finally
        {
            fIsFilterDialogOpen = false;
        }
    }
    async void SaveSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerSaveOptions Options = new()
        {
            Title = "Save Settings",
            SuggestedFileName = string.IsNullOrWhiteSpace(SettingsSuggestedFileName) ? "pivot-grid-settings.json" : SettingsSuggestedFileName,
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
    async void LoadSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerOpenOptions Options = new()
        {
            Title = "Load Settings",
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
    async void ExportAsync(PivotGridExporter Exporter)
    {
        if (Exporter == null)
            return;

        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        string Extension = (Exporter.DefaultExtension ?? string.Empty).Trim().TrimStart('.');
        string Pattern = string.IsNullOrWhiteSpace(Extension) ? "*.*" : "*." + Extension;
        FilePickerFileType FileType = new(Exporter.Name)
        {
            Patterns = new[] { Pattern },
        };
        FilePickerSaveOptions Options = new()
        {
            Title = "Export",
            SuggestedFileName = string.IsNullOrWhiteSpace(Extension) ? "PivotGrid" : "PivotGrid." + Extension,
            DefaultExtension = Extension,
            FileTypeChoices = new[] { FileType },
        };
        IStorageFile File = await Owner.StorageProvider.SaveFilePickerAsync(Options);
        if (File == null || !File.Path.IsFile)
            return;

        SaveExport(Exporter, File.Path.LocalPath);
    }
    string FormatSourceValue(PivotGridSourceField Field, object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return "(blank)";
        if (Field != null && (Field.ValueType == typeof(DateTime) || Field.ValueType == typeof(DateTimeOffset)) && Value is IFormattable Formattable)
            return Formattable.ToString("d", CultureInfo.CurrentCulture);

        return Convert.ToString(Value, CultureInfo.CurrentCulture);
    }
    List<object> GetDistinctFieldValues(string FieldName)
    {
        if (DataAdapter == null || string.IsNullOrWhiteSpace(FieldName))
            return new List<object>();

        List<object> Result = new();
        HashSet<string> Keys = new(StringComparer.Ordinal);
        for (int RowIndex = 0; RowIndex < DataAdapter.RowCount; RowIndex++)
        {
            object Value = DataAdapter.GetValue(RowIndex, FieldName);
            string Key = Value == null || Value == DBNull.Value ? string.Empty : Convert.ToString(Value, CultureInfo.InvariantCulture);
            if (Keys.Add(Key))
                Result.Add(Value);
        }

        return Result
            .OrderBy(Value => FormatSourceValue(FindSourceField(FieldName), Value), StringComparer.CurrentCulture)
            .ToList();
    }
    IEnumerable<(string Text, string FieldName)> GetRowFieldChips()
    {
        return RowFields.Select(Field => (GetAxisFieldText(Field), Field.Name));
    }
    IEnumerable<(string Text, string FieldName)> GetColumnFieldChips()
    {
        return ColumnFields.Select(Field => (GetAxisFieldText(Field), Field.Name));
    }
    IEnumerable<(string Text, string FieldName)> GetMeasureChips()
    {
        return Measures.Select(Measure => (GetMeasureText(Measure), Measure.SourceFieldName));
    }
    string FormatExportFieldValue(PivotGridField Field, object Value)
    {
        if (Field == null)
            return string.Empty;

        return Field.FormatValue(Value);
    }
    List<string> CreateExportRowTexts(PivotGridAxisNode RowNode, bool IsColumnTotal)
    {
        List<string> Result = new();
        for (int Index = 0; Index < RowFields.Count; Index++)
        {
            if (IsColumnTotal)
                Result.Add(Index == 0 ? "Total" : string.Empty);
            else if (RowNode != null && RowNode.Item != null && RowNode.Item.Values.Count > Index)
                Result.Add(FormatExportFieldValue(RowFields[Index], RowNode.Item.Values[Index]));
            else
                Result.Add(string.Empty);
        }

        return Result;
    }
    string CreateExportRowHeaderText(PivotGridAxisNode RowNode, bool IsColumnTotal)
    {
        if (IsColumnTotal)
            return "Total";
        if (RowNode == null)
            return string.Empty;

        return RowNode.Item?.Text ?? string.Empty;
    }
    string SanitizeClipboardCellText(string Text)
    {
        return (Text ?? string.Empty)
            .Replace('\t', ' ')
            .Replace('\r', ' ')
            .Replace('\n', ' ');
    }
    string CreateClipboardText(PivotGridExportSnapshot Snapshot)
    {
        Snapshot ??= new PivotGridExportSnapshot(null, null, null, null, null);
        StringBuilder Builder = new();
        List<string> Headers = Snapshot.RowFields.Select(Field => Field.Header).ToList();
        Headers.AddRange(Snapshot.ValueColumns.Select(Column => Column.Header));
        Builder.AppendLine(string.Join('\t', Headers.Select(SanitizeClipboardCellText)));
        foreach (PivotGridExportRow Row in Snapshot.Rows)
        {
            List<string> Values = Row.RowTexts.ToList();
            Values.AddRange(Row.Cells.Select(Cell => Cell.Text));
            Builder.AppendLine(string.Join('\t', Values.Select(SanitizeClipboardCellText)));
        }

        return Builder.ToString();
    }
    IReadOnlyList<string> GetRoleChipTexts(PivotGridFieldRole Role)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                return RowFields.Select(GetAxisFieldText).ToList();
            case PivotGridFieldRole.Column:
                return ColumnFields.Select(GetAxisFieldText).ToList();
            case PivotGridFieldRole.Measure:
                return Measures.Select(GetMeasureText).ToList();
        }

        return Array.Empty<string>();
    }
    double GetRoleChipZoneX(PivotGridFieldRole Role)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Column:
                return GetRowHeaderWidth();
            case PivotGridFieldRole.Row:
            case PivotGridFieldRole.Measure:
                return 0;
        }

        return 0;
    }
    double GetRoleChipY(PivotGridFieldRole Role)
    {
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                return GetGridTop() + Math.Max(0, (LayoutMetrics.ColumnHeaderHeight - 24) / 2);
            case PivotGridFieldRole.Column:
            case PivotGridFieldRole.Measure:
                return GetFieldPanelHeight() + Math.Max(0, (LayoutMetrics.AxisPanelHeight - 24) / 2);
        }

        return 0;
    }
    int GetFieldDropIndex(PivotGridFieldRole Role, Point Point)
    {
        IReadOnlyList<string> Items = GetRoleChipTexts(Role);
        if (Role == PivotGridFieldRole.Available)
            return -1;

        double X = GetRoleChipZoneX(Role) + 78;
        for (int Index = 0; Index < Items.Count; Index++)
        {
            Rect ChipRect = GetFieldChipRect(X, GetRoleChipY(Role), Items[Index]);
            if (Point.X < ChipRect.X + (ChipRect.Width / 2))
                return Index;

            X = ChipRect.Right + 6;
        }

        return Items.Count;
    }
    void DrawAxisZone(DrawingContext Context, string Header, IEnumerable<(string Text, string FieldName)> Items, Rect Rect, PivotGridFieldRole Role)
    {
        DrawBand(Context, Rect, HeaderBrush);
        DrawText(Context, Header, new Rect(Rect.X + 6, Rect.Y, 72, Rect.Height), MutedTextBrush, FontWeight.SemiBold);

        double X = Rect.X + 78;
        double Y = Rect.Y + Math.Max(0, (Rect.Height - 24) / 2);
        foreach ((string Text, string FieldName) in Items)
        {
            Rect ChipRect = GetFieldChipRect(X, Y, Text);
            if (ChipRect.Right > Rect.Right)
                break;
            if (Role == PivotGridFieldRole.Row || Role == PivotGridFieldRole.Column)
                DrawSortableFieldChip(Context, ChipRect, Text, Role, FieldName);
            else
            {
                DrawFieldChip(Context, ChipRect, Text);
                if (Engine.IsFieldFiltered(FieldName))
                    DrawFilterGlyph(Context, ChipRect, 0);
            }
            X = ChipRect.Right + 6;
        }
    }
    void DrawAxisPanel(DrawingContext Context)
    {
        double Y = GetFieldPanelHeight();
        double RowHeaderWidth = GetRowHeaderWidth();
        Rect MeasureRect = new(0, Y, RowHeaderWidth, LayoutMetrics.AxisPanelHeight);
        Rect ColumnRect = new(RowHeaderWidth, Y, Math.Max(0, Bounds.Width - RowHeaderWidth), LayoutMetrics.AxisPanelHeight);
        DrawAxisZone(Context, "Values", GetMeasureChips(), MeasureRect, PivotGridFieldRole.Measure);
        DrawAxisZone(Context, "Columns", GetColumnFieldChips(), ColumnRect, PivotGridFieldRole.Column);
    }
    Rect GetAxisZoneRect(PivotGridFieldRole Role)
    {
        double Y = GetFieldPanelHeight();
        double RowHeaderWidth = GetRowHeaderWidth();
        switch (Role)
        {
            case PivotGridFieldRole.Row:
                return new Rect(0, GetGridTop(), RowHeaderWidth, Math.Max(0, Bounds.Height - GetGridTop()));
            case PivotGridFieldRole.Column:
                return new Rect(RowHeaderWidth, Y, Math.Max(0, Bounds.Width - RowHeaderWidth), LayoutMetrics.AxisPanelHeight);
            case PivotGridFieldRole.Measure:
                return new Rect(0, Y, RowHeaderWidth, LayoutMetrics.AxisPanelHeight);
        }

        return default;
    }
    bool HitTestAxisFields(Point Point, double ZoneX, double ChipY, IReadOnlyList<string> Items, out int Index)
    {
        Index = -1;
        double X = ZoneX + 78;
        for (int ItemIndex = 0; ItemIndex < Items.Count; ItemIndex++)
        {
            Rect ChipRect = GetFieldChipRect(X, ChipY, Items[ItemIndex]);
            if (ChipRect.Contains(Point))
            {
                Index = ItemIndex;
                return true;
            }

            X = ChipRect.Right + 6;
        }

        return false;
    }
    PivotGridFieldRole GetDropRole(Point Point)
    {
        double RowHeaderWidth = GetRowHeaderWidth();
        if (ShowFieldPanel && Point.Y >= 0 && Point.Y < GetFieldPanelHeight())
            return PivotGridFieldRole.Available;
        if (Point.Y >= GetFieldPanelHeight() && Point.Y < GetFieldPanelHeight() + LayoutMetrics.AxisPanelHeight)
            return Point.X < RowHeaderWidth ? PivotGridFieldRole.Measure : PivotGridFieldRole.Column;
        if (Point.X < RowHeaderWidth && Point.Y >= GetGridTop())
            return PivotGridFieldRole.Row;

        return PivotGridFieldRole.None;
    }
    bool CanDropDraggedField(PivotGridFieldRole TargetRole)
    {
        PivotGridSourceField SourceField = FindSourceField(fFieldDragName);
        if (SourceField == null)
            return false;

        switch (TargetRole)
        {
            case PivotGridFieldRole.Available:
                return fFieldDragSourceRole != PivotGridFieldRole.Available;
            case PivotGridFieldRole.Row:
            case PivotGridFieldRole.Column:
                return SourceField.CanUseAsAxis && (fFieldDragSourceRole != TargetRole || ReorderFieldInRoleWouldMove(TargetRole, fFieldDragName, fFieldDragTargetIndex));
            case PivotGridFieldRole.Measure:
                return SourceField.CanUseAsMeasure && (fFieldDragSourceRole != TargetRole || ReorderFieldInRoleWouldMove(TargetRole, fFieldDragName, fFieldDragTargetIndex));
        }

        return false;
    }
    bool ReorderFieldInRoleWouldMove(PivotGridFieldRole Role, string FieldName, int TargetIndex)
    {
        PivotGridFieldRole SourceRole = FindFieldRole(FieldName, out int SourceIndex);
        if (SourceRole != Role)
            return false;

        int Count = GetRoleItemCount(Role);
        TargetIndex = NormalizeInsertIndex(TargetIndex, Count);
        if (TargetIndex > SourceIndex)
            TargetIndex--;

        return TargetIndex != SourceIndex;
    }
    void DrawFieldDropInsertionGuide(DrawingContext Context)
    {
        double X = GetRoleChipZoneX(fFieldDragTargetRole) + 78;
        IReadOnlyList<string> Items = GetRoleChipTexts(fFieldDragTargetRole);
        int TargetIndex = NormalizeInsertIndex(fFieldDragTargetIndex, Items.Count);
        for (int Index = 0; Index < TargetIndex; Index++)
            X = GetFieldChipRect(X, GetRoleChipY(fFieldDragTargetRole), Items[Index]).Right + 6;

        double Y = GetRoleChipY(fFieldDragTargetRole);
        Context.DrawLine(new Pen(MutedTextBrush, 2), new Point(X, Y - 3), new Point(X, Y + 27));
    }
    void DrawFieldDropGuide(DrawingContext Context)
    {
        if (!fIsFieldDragging || !CanDropDraggedField(fFieldDragTargetRole))
            return;

        if (fFieldDragTargetRole == PivotGridFieldRole.Row
            || fFieldDragTargetRole == PivotGridFieldRole.Column
            || fFieldDragTargetRole == PivotGridFieldRole.Measure)
        {
            DrawFieldDropInsertionGuide(Context);
            return;
        }

        Rect Rect = fFieldDragTargetRole == PivotGridFieldRole.Available
            ? new Rect(0, 0, Bounds.Width, GetFieldPanelHeight())
            : GetAxisZoneRect(fFieldDragTargetRole);
        if (Rect.Width > 0 && Rect.Height > 0)
            Context.DrawRectangle(null, new Pen(MutedTextBrush, 2), Rect.Deflate(2), 4, 4);
    }
    void DrawFieldDragGhost(DrawingContext Context)
    {
        if (!fIsFieldDragging)
            return;

        Rect Rect = GetFieldChipRect(fFieldDragPoint.X - 18, fFieldDragPoint.Y - 12, fFieldDragText);
        Context.DrawRectangle(GridBackgroundBrush, new Pen(MutedTextBrush, 1), Rect, 3, 3);
        DrawText(Context, fFieldDragText, Rect, TextBrush, FontWeight.SemiBold);
    }
    void DrawExpander(DrawingContext Context, Rect Rect, bool IsExpanded)
    {
        double Size = Math.Min(10, Math.Max(6, Math.Min(Rect.Width, Rect.Height) - 8));
        double CenterX = Rect.X + (Rect.Width / 2);
        double CenterY = Rect.Y + (Rect.Height / 2);
        StreamGeometry Geometry = new();

        using (StreamGeometryContext GeometryContext = Geometry.Open())
        {
            if (IsExpanded)
            {
                GeometryContext.BeginFigure(new Point(CenterX - (Size / 2), CenterY - (Size / 4)), true);
                GeometryContext.LineTo(new Point(CenterX + (Size / 2), CenterY - (Size / 4)));
                GeometryContext.LineTo(new Point(CenterX, CenterY + (Size / 2)));
            }
            else
            {
                GeometryContext.BeginFigure(new Point(CenterX - (Size / 4), CenterY - (Size / 2)), true);
                GeometryContext.LineTo(new Point(CenterX - (Size / 4), CenterY + (Size / 2)));
                GeometryContext.LineTo(new Point(CenterX + (Size / 2), CenterY));
            }

            GeometryContext.EndFigure(true);
        }

        Context.DrawGeometry(MutedTextBrush, null, Geometry);
    }
    bool BeginFieldDrag(PointerPressedEventArgs Args, Point Point)
    {
        PivotGridHitTestResult Hit = HitTest(Point);
        PivotGridFieldRole Role = PivotGridFieldRole.None;
        string FieldName = string.Empty;
        string Text = string.Empty;

        if (!GetHitField(Hit, out Role, out FieldName, out Text))
            return false;

        fIsFieldDragging = true;
        fFieldDragSourceRole = Role;
        fFieldDragTargetRole = PivotGridFieldRole.None;
        fFieldDragTargetIndex = -1;
        fFieldDragName = FieldName;
        fFieldDragText = Text;
        fFieldDragStartPoint = Point;
        fFieldDragPoint = Point;
        Args.Pointer.Capture(this);
        InvalidateVisual();
        return true;
    }
    bool UpdateFieldDrag(Point Point)
    {
        if (!fIsFieldDragging)
            return false;

        fFieldDragPoint = Point;
        fFieldDragTargetRole = GetDropRole(Point);
        fFieldDragTargetIndex = GetFieldDropIndex(fFieldDragTargetRole, Point);
        InvalidateVisual();
        return true;
    }
    bool EndFieldDrag(PointerReleasedEventArgs Args, Point Point)
    {
        if (!fIsFieldDragging)
            return false;

        UpdateFieldDrag(Point);
        PivotGridFieldRole TargetRole = fFieldDragTargetRole;
        int TargetIndex = fFieldDragTargetIndex;
        string FieldName = fFieldDragName;
        bool CanDrop = CanDropDraggedField(TargetRole);
        bool IsClick = Math.Abs(Point.X - fFieldDragStartPoint.X) < 4 && Math.Abs(Point.Y - fFieldDragStartPoint.Y) < 4;
        PivotGridFieldRole SourceRole = fFieldDragSourceRole;
        fIsFieldDragging = false;
        fFieldDragSourceRole = PivotGridFieldRole.None;
        fFieldDragTargetRole = PivotGridFieldRole.None;
        fFieldDragTargetIndex = -1;
        fFieldDragName = string.Empty;
        fFieldDragText = string.Empty;
        fFieldDragStartPoint = default;
        fFieldDragPoint = default;
        Args.Pointer.Capture(null);
        if (IsClick && (SourceRole == PivotGridFieldRole.Row || SourceRole == PivotGridFieldRole.Column))
            Engine.ToggleSort(SourceRole, FieldName);
        else if (CanDrop)
            MoveFieldCore(FieldName, TargetRole, TargetIndex);
        InvalidateVisual();
        return true;
    }
    bool HandleRowExpanderPointerPressed(Point Point)
    {
        PivotGridHitTestResult Hit = HitTest(Point);
        return Hit.Kind == PivotGridHitTestKind.RowExpander && Engine.ToggleRowExpanded(Hit.RowIndex);
    }
    bool HandleValueCellPointerPressed(Point Point)
    {
        PivotGridHitTestResult Hit = HitTest(Point);
        return Hit.Kind == PivotGridHitTestKind.ValueCell && SetCurrentCellCore(Hit.RowIndex, Hit.ColumnIndex, Hit.MeasureIndex);
    }
    string GetHitToolTipText(PivotGridHitTestResult Hit)
    {
        if (Hit == null)
            return string.Empty;

        switch (Hit.Kind)
        {
            case PivotGridHitTestKind.AvailableField:
                return GetSourceFieldText(Hit.SourceField);
            case PivotGridHitTestKind.RowField:
                return Hit.RowIndex >= 0 && Hit.RowIndex < RowFields.Count ? GetAxisFieldText(RowFields[Hit.RowIndex]) : string.Empty;
            case PivotGridHitTestKind.ColumnField:
                return Hit.ColumnIndex >= 0 && Hit.ColumnIndex < ColumnFields.Count ? GetAxisFieldText(ColumnFields[Hit.ColumnIndex]) : string.Empty;
            case PivotGridHitTestKind.MeasureField:
                return GetMeasureText(Hit.Measure);
            case PivotGridHitTestKind.RowHeader:
            case PivotGridHitTestKind.RowExpander:
                return Hit.RowNode != null ? CreateExportRowHeaderText(Hit.RowNode, false) : Hit.RowIndex == Engine.VisibleRowNodes.Count ? "Total" : string.Empty;
            case PivotGridHitTestKind.ColumnHeader:
                return Hit.ColumnItem != null ? Hit.ColumnItem.Text : Hit.ColumnIndex == Engine.ColumnItems.Count ? "Total" : string.Empty;
            case PivotGridHitTestKind.ValueCell:
                return GetValueCellToolTipText(Hit);
        }

        return string.Empty;
    }
    string GetValueCellToolTipText(PivotGridHitTestResult Hit)
    {
        if (Hit == null || Hit.Cell == null)
            return string.Empty;

        string RowText = Hit.RowNode != null ? CreateExportRowHeaderText(Hit.RowNode, false) : Hit.RowIndex == Engine.VisibleRowNodes.Count ? "Total" : string.Empty;
        string ColumnText = Hit.ColumnItem != null ? Hit.ColumnItem.Text : Hit.ColumnIndex == Engine.ColumnItems.Count ? "Total" : string.Empty;
        string MeasureText = GetMeasureText(Hit.Measure);
        List<string> Lines = new();
        if (!string.IsNullOrWhiteSpace(RowText))
            Lines.Add(RowText);
        if (!string.IsNullOrWhiteSpace(ColumnText))
            Lines.Add(ColumnText);
        if (!string.IsNullOrWhiteSpace(MeasureText))
            Lines.Add(MeasureText);
        Lines.Add(Hit.Cell.Text ?? string.Empty);
        return string.Join(Environment.NewLine, Lines);
    }
    void UpdateToolTip(Point Point)
    {
        if (!ShowToolTips)
        {
            if (!string.IsNullOrEmpty(fToolTipText))
            {
                fToolTipText = string.Empty;
                ToolTip.SetTip(this, null);
            }
            return;
        }

        string Text = GetToolTipText(Point);
        if (string.Equals(fToolTipText, Text, StringComparison.Ordinal))
            return;

        fToolTipText = Text;
        ToolTip.SetTip(this, string.IsNullOrWhiteSpace(Text) ? null : Text);
    }
    PivotGridValueCell GetCurrentCellCore()
    {
        if (!IsValidCellIndex(fCurrentRowIndex, fCurrentColumnIndex, fCurrentMeasureIndex))
            return null;

        PivotGridMeasure Measure = Engine.Measures[fCurrentMeasureIndex];
        bool IsTotalRow = fShowColumnGrandTotals && fCurrentRowIndex == Engine.VisibleRowNodes.Count;
        bool IsTotalColumn = fShowRowGrandTotals && fCurrentColumnIndex == Engine.ColumnItems.Count;
        if (IsTotalRow && IsTotalColumn)
            return Engine.GetGrandTotalCell(Measure);
        if (IsTotalRow)
            return Engine.GetColumnTotalCell(Engine.ColumnItems[fCurrentColumnIndex], Measure);
        if (IsTotalColumn)
            return Engine.GetRowTotalCell(Engine.VisibleRowNodes[fCurrentRowIndex].Item, Measure);

        return Engine.GetCell(Engine.VisibleRowNodes[fCurrentRowIndex].Item, Engine.ColumnItems[fCurrentColumnIndex], Measure);
    }
    async Task<bool> CopyCurrentCellTextAsync()
    {
        PivotGridValueCell Cell = GetCurrentCellCore();
        if (Cell == null)
            return false;

        TopLevel TopLevel = TopLevel.GetTopLevel(this);
        if (TopLevel?.Clipboard == null)
            return false;

        await TopLevel.Clipboard.SetTextAsync(Cell.Text ?? string.Empty);
        return true;
    }
    async Task<bool> CopyPivotTextAsync()
    {
        TopLevel TopLevel = TopLevel.GetTopLevel(this);
        if (TopLevel?.Clipboard == null)
            return false;

        await TopLevel.Clipboard.SetTextAsync(CreateClipboardText());
        return true;
    }
    bool MoveCurrentCell(int RowOffset, int CellOffset)
    {
        int RowCount = GetSelectableRowCount();
        int ColumnCount = GetSelectableColumnCount();
        int MeasureCount = Engine.Measures.Count;
        if (RowCount == 0 || ColumnCount == 0 || MeasureCount == 0)
            return false;

        int RowIndex = fCurrentRowIndex;
        int CellIndex = fCurrentColumnIndex >= 0 && fCurrentMeasureIndex >= 0
            ? (fCurrentColumnIndex * MeasureCount) + fCurrentMeasureIndex
            : -1;

        if (RowIndex < 0 || CellIndex < 0)
            return SetCurrentCellCore(0, 0, 0);

        RowIndex = Math.Max(0, Math.Min(RowCount - 1, RowIndex + RowOffset));
        CellIndex = Math.Max(0, Math.Min((ColumnCount * MeasureCount) - 1, CellIndex + CellOffset));
        int ColumnIndex = CellIndex / MeasureCount;
        int MeasureIndex = CellIndex % MeasureCount;
        bool Result = SetCurrentCellCore(RowIndex, ColumnIndex, MeasureIndex);
        ScrollCurrentCellIntoViewCore();
        return Result;
    }
    bool SetCurrentCellFromFlatIndex(int RowIndex, int CellIndex)
    {
        int RowCount = GetSelectableRowCount();
        int ColumnCount = GetSelectableColumnCount();
        int MeasureCount = Engine.Measures.Count;
        if (RowCount == 0 || ColumnCount == 0 || MeasureCount == 0)
            return false;

        RowIndex = Math.Max(0, Math.Min(RowCount - 1, RowIndex));
        CellIndex = Math.Max(0, Math.Min((ColumnCount * MeasureCount) - 1, CellIndex));
        bool Result = SetCurrentCellCore(RowIndex, CellIndex / MeasureCount, CellIndex % MeasureCount);
        ScrollCurrentCellIntoViewCore();
        return Result;
    }
    int GetCurrentFlatCellIndex()
    {
        int MeasureCount = Engine.Measures.Count;
        if (fCurrentColumnIndex < 0 || fCurrentMeasureIndex < 0 || MeasureCount == 0)
            return 0;

        return (fCurrentColumnIndex * MeasureCount) + fCurrentMeasureIndex;
    }
    int GetLastFlatCellIndex()
    {
        int ColumnCount = GetSelectableColumnCount();
        int MeasureCount = Engine.Measures.Count;
        return Math.Max(0, (ColumnCount * MeasureCount) - 1);
    }
    int GetKeyboardPageRowCount()
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (BodyRect.Height <= 0 || LayoutMetrics.RowHeight <= 0)
            return 10;

        return Math.Max(1, (int)Math.Floor(BodyRect.Height / LayoutMetrics.RowHeight) - 1);
    }
    bool MoveCurrentCellToRowStart()
    {
        int RowIndex = fCurrentRowIndex < 0 ? 0 : fCurrentRowIndex;
        return SetCurrentCellFromFlatIndex(RowIndex, 0);
    }
    bool MoveCurrentCellToRowEnd()
    {
        int RowIndex = fCurrentRowIndex < 0 ? 0 : fCurrentRowIndex;
        return SetCurrentCellFromFlatIndex(RowIndex, GetLastFlatCellIndex());
    }
    bool MoveCurrentCellToGridStart()
    {
        return SetCurrentCellFromFlatIndex(0, 0);
    }
    bool MoveCurrentCellToGridEnd()
    {
        return SetCurrentCellFromFlatIndex(GetSelectableRowCount() - 1, GetLastFlatCellIndex());
    }
    bool MoveCurrentCellPage(int Direction)
    {
        int RowIndex = fCurrentRowIndex < 0 ? 0 : fCurrentRowIndex;
        int CellIndex = GetCurrentFlatCellIndex();
        return SetCurrentCellFromFlatIndex(RowIndex + (Direction * GetKeyboardPageRowCount()), CellIndex);
    }
    bool ScrollCurrentCellIntoViewCore()
    {
        if (!IsValidCellIndex(fCurrentRowIndex, fCurrentColumnIndex, fCurrentMeasureIndex))
            return false;

        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        bool Result = false;
        double RowTop = fCurrentRowIndex * LayoutMetrics.RowHeight;
        double RowBottom = RowTop + LayoutMetrics.RowHeight;
        if (HasVerticalScrollBar && RowTop < fVerticalOffset)
            Result = SetVerticalOffsetCore(RowTop) || Result;
        else if (HasVerticalScrollBar && RowBottom > fVerticalOffset + BodyRect.Height)
            Result = SetVerticalOffsetCore(RowBottom - BodyRect.Height) || Result;

        double CellLeft = 0;
        for (int ColumnIndex = 0; ColumnIndex < fCurrentColumnIndex; ColumnIndex++)
        {
            PivotGridAxisItem ColumnItem = ColumnIndex < Engine.ColumnItems.Count ? Engine.ColumnItems[ColumnIndex] : null;
            for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
                CellLeft += GetValueCellWidth(ColumnItem, Engine.Measures[MeasureIndex]);
        }
        PivotGridAxisItem CurrentColumnItem = fCurrentColumnIndex < Engine.ColumnItems.Count ? Engine.ColumnItems[fCurrentColumnIndex] : null;
        for (int MeasureIndex = 0; MeasureIndex < fCurrentMeasureIndex; MeasureIndex++)
            CellLeft += GetValueCellWidth(CurrentColumnItem, Engine.Measures[MeasureIndex]);
        double CellRight = CellLeft + GetValueCellWidth(CurrentColumnItem, Engine.Measures[fCurrentMeasureIndex]);
        if (HasHorizontalScrollBar && CellLeft < fHorizontalOffset)
            Result = SetHorizontalOffsetCore(CellLeft) || Result;
        else if (HasHorizontalScrollBar && CellRight > fHorizontalOffset + BodyRect.Width)
            Result = SetHorizontalOffsetCore(CellRight - BodyRect.Width) || Result;

        return Result;
    }
    double GetValueCellWidth(PivotGridMeasure Measure)
    {
        if (Measure != null && Measure.Width > 0)
            return Measure.Width;

        return LayoutMetrics.ValueCellWidth;
    }
    double GetColumnGroupWidth(PivotGridAxisItem ColumnItem)
    {
        double Result = 0;
        foreach (PivotGridMeasure Measure in Engine.Measures)
            Result += GetValueCellWidth(ColumnItem, Measure);

        return Result > 0 ? Result : LayoutMetrics.ValueCellWidth;
    }
    double GetTotalGroupWidth()
    {
        double Result = 0;
        foreach (PivotGridMeasure Measure in Engine.Measures)
            Result += GetValueCellWidth(null, Measure);

        return Result > 0 ? Result : LayoutMetrics.ValueCellWidth;
    }
    string CreateValueColumnWidthKey(PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)
    {
        return (ColumnItem == null ? "$TOTAL$" : ColumnItem.Key) + "\u001E" + (Measure == null ? string.Empty : Measure.Name);
    }
    bool SetMeasureWidth(PivotGridMeasure Measure, double Width)
    {
        if (Measure == null)
            return false;

        double NewWidth = Math.Max(MeasureMinWidth, Width);
        if (Math.Abs(Measure.Width - NewWidth) < 0.1)
            return false;

        Measure.Width = NewWidth;
        ClampScrollOffsets();
        InvalidateVisual();
        return true;
    }
    double GetValueCellWidth(PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)
    {
        if (Measure != null && fValueColumnWidths.TryGetValue(CreateValueColumnWidthKey(ColumnItem, Measure), out double Width) && Width > 0)
            return Width;

        return GetValueCellWidth(Measure);
    }
    bool SetValueColumnWidth(PivotGridAxisItem ColumnItem, PivotGridMeasure Measure, double Width)
    {
        if (Measure == null)
            return false;

        string Key = CreateValueColumnWidthKey(ColumnItem, Measure);
        double NewWidth = Math.Max(MeasureMinWidth, Width);
        if (fValueColumnWidths.TryGetValue(Key, out double CurrentWidth) && Math.Abs(CurrentWidth - NewWidth) < 0.1)
            return false;

        fValueColumnWidths[Key] = NewWidth;
        ClampScrollOffsets();
        InvalidateVisual();
        return true;
    }
    double CalculateAutoFitValueColumnWidth(PivotGridExportSnapshot Snapshot, PivotGridExportValueColumn Column)
    {
        double Result = MeasureTextWidth(Column?.Header, FontWeight.SemiBold) + 18;
        Result = Math.Max(Result, MeasureTextWidth(GetMeasureText(Column?.Measure?.Measure), FontWeight.SemiBold) + 18);
        foreach (PivotGridExportRow Row in Snapshot.Rows)
        {
            PivotGridExportCell Cell = Row.Cells.FirstOrDefault(Item => ReferenceEquals(Item.Column, Column));
            if (Cell == null)
                continue;

            FontWeight Weight = Row.IsColumnTotal || Column.IsTotal || (Row.RowNode != null && Row.RowNode.HasChildren && Row.RowNode.IsExpanded)
                ? FontWeight.SemiBold
                : FontWeight.Normal;
            Result = Math.Max(Result, MeasureTextWidth(Cell.Text, Weight) + 18);
        }

        return Math.Max(MeasureMinWidth, Math.Min(MeasureAutoFitMaxWidth, Math.Ceiling(Result)));
    }
    Dictionary<PivotGridExportValueColumn, double> CalculateAutoFitValueColumnWidths(PivotGridExportSnapshot Snapshot)
    {
        Dictionary<PivotGridExportValueColumn, double> Result = Snapshot.ValueColumns.ToDictionary(Column => Column, Column => CalculateAutoFitValueColumnWidth(Snapshot, Column));
        foreach (IGrouping<int, PivotGridExportValueColumn> Group in Snapshot.ValueColumns.GroupBy(Column => Column.ColumnIndex))
        {
            List<PivotGridExportValueColumn> Columns = Group.ToList();
            if (Columns.Count == 0)
                continue;

            double CurrentWidth = Columns.Sum(Column => Result[Column]);
            double RequiredWidth = MeasureTextWidth(Columns[0].ColumnText, FontWeight.SemiBold) + 18;
            if (RequiredWidth <= CurrentWidth)
                continue;

            double ExtraWidth = Math.Ceiling((RequiredWidth - CurrentWidth) / Columns.Count);
            foreach (PivotGridExportValueColumn Column in Columns)
                Result[Column] = Math.Min(MeasureAutoFitMaxWidth, Result[Column] + ExtraWidth);
        }

        return Result;
    }
    bool AutoFitValueColumnWidthsCore()
    {
        PivotGridExportSnapshot Snapshot = CreateExportSnapshot();
        if (Snapshot.ValueColumns.Count == 0)
            return false;

        bool Result = false;
        Dictionary<PivotGridExportValueColumn, double> Widths = CalculateAutoFitValueColumnWidths(Snapshot);
        foreach (PivotGridExportValueColumn Column in Snapshot.ValueColumns)
        {
            string Key = CreateValueColumnWidthKey(Column.ColumnItem, Column.Measure?.Measure);
            double Width = Widths[Column];
            if (!fValueColumnWidths.TryGetValue(Key, out double CurrentWidth) || Math.Abs(CurrentWidth - Width) >= 0.1)
            {
                fValueColumnWidths[Key] = Width;
                Result = true;
            }
        }

        if (!Result)
            return false;

        ClampScrollOffsets();
        InvalidateVisual();
        return true;
    }
    bool AutoFitValueColumnWidthCore(int ColumnIndex, int MeasureIndex)
    {
        PivotGridExportSnapshot Snapshot = CreateExportSnapshot();
        PivotGridExportValueColumn Column = Snapshot.ValueColumns.FirstOrDefault(Item => Item.ColumnIndex == ColumnIndex && Item.MeasureIndex == MeasureIndex);
        if (Column == null)
            return false;

        double Width = CalculateAutoFitValueColumnWidths(Snapshot)[Column];
        return SetValueColumnWidth(Column.ColumnItem, Column.Measure?.Measure, Width);
    }
    double CalculateAutoFitRowHeaderWidth()
    {
        double Result = GetLeftHeaderContentWidth();
        foreach (PivotGridAxisNode Node in Engine.VisibleRowNodes)
        {
            double Indent = Math.Max(0, Node.Level) * LayoutMetrics.RowIndentWidth;
            double ExpanderWidth = LayoutMetrics.RowExpanderWidth;
            double TextWidth = MeasureTextWidth(Node.Item?.Text, FontWeight.SemiBold);
            Result = Math.Max(Result, Indent + ExpanderWidth + TextWidth + 16);
        }

        if (ShowColumnGrandTotals)
            Result = Math.Max(Result, MeasureTextWidth("Total", FontWeight.SemiBold) + 16);

        return Math.Max(RowHeaderMinWidth, Math.Min(RowHeaderAutoFitMaxWidth, Math.Ceiling(Result)));
    }
    bool AutoFitRowHeaderWidthCore()
    {
        if (Engine.VisibleRowNodes.Count == 0)
            return false;

        double Width = CalculateAutoFitRowHeaderWidth();
        return SetRowHeaderWidthCore(Width);
    }
    bool SetRowHeaderWidthCore(double Width)
    {
        double NewWidth = Math.Max(RowHeaderMinWidth, Width);
        if (Math.Abs(LayoutMetrics.RowHeaderWidth - NewWidth) < 0.1)
            return false;

        LayoutMetrics.RowHeaderWidth = NewWidth;
        ClampScrollOffsets();
        InvalidateVisual();
        return true;
    }
    double GetDefaultRowHeaderWidth()
    {
        return new PivotGridLayoutMetrics().RowHeaderWidth;
    }
    bool CanResetRowHeaderWidth()
    {
        return Math.Abs(LayoutMetrics.RowHeaderWidth - GetDefaultRowHeaderWidth()) >= 0.1;
    }
    bool ResetRowHeaderWidthCore()
    {
        return SetRowHeaderWidthCore(GetDefaultRowHeaderWidth());
    }
    bool ClearValueColumnWidthsCore()
    {
        if (fValueColumnWidths.Count == 0)
            return false;

        fValueColumnWidths.Clear();
        ClampScrollOffsets();
        InvalidateVisual();
        return true;
    }
    bool TryGetRowHeaderResizeHit(Point Point, out double BoundaryX)
    {
        BoundaryX = GetRowHeaderWidth();
        if (Point.Y < GetFieldPanelHeight())
            return false;
        if (Bounds.Height > 0 && Point.Y >= Bounds.Height)
            return false;

        return Math.Abs(Point.X - BoundaryX) <= MeasureResizeGripWidth;
    }
    bool TryGetMeasureResizeHit(Point Point, out int ColumnIndex, out int MeasureIndex, out double BoundaryX)
    {
        ColumnIndex = -1;
        MeasureIndex = -1;
        BoundaryX = 0;
        if (Engine.Measures.Count == 0)
            return false;
        if (Point.Y < GetGridTop() || Point.Y >= GetBodyTop())
            return false;

        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        Rect GridRect = new(GetRowHeaderWidth(), GetGridTop(), BodyRect.Width, LayoutMetrics.ColumnHeaderHeight);
        if (Bounds.Width > 0 && Bounds.Height > 0 && !GridRect.Contains(Point))
            return false;

        double X = GetRowHeaderWidth() - fHorizontalOffset;
        for (int ItemIndex = 0; ItemIndex < Engine.ColumnItems.Count; ItemIndex++)
            for (int Index = 0; Index < Engine.Measures.Count; Index++)
            {
                X += GetValueCellWidth(Engine.ColumnItems[ItemIndex], Engine.Measures[Index]);
                if (Math.Abs(Point.X - X) <= MeasureResizeGripWidth)
                {
                    ColumnIndex = ItemIndex;
                    MeasureIndex = Index;
                    BoundaryX = X;
                    return true;
                }
            }

        if (fShowRowGrandTotals)
        {
            int TotalColumnIndex = Engine.ColumnItems.Count;
            for (int Index = 0; Index < Engine.Measures.Count; Index++)
            {
                X += GetValueCellWidth(null, Engine.Measures[Index]);
                if (Math.Abs(Point.X - X) <= MeasureResizeGripWidth)
                {
                    ColumnIndex = TotalColumnIndex;
                    MeasureIndex = Index;
                    BoundaryX = X;
                    return true;
                }
            }
        }

        return false;
    }
    void DrawColumnHeaders(DrawingContext Context)
    {
        double Top = GetGridTop();
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        double X = GetRowHeaderWidth() - fHorizontalOffset;
        double MeasureHeaderHeight = Engine.Measures.Count > 1 ? LayoutMetrics.ColumnHeaderHeight / 2 : LayoutMetrics.ColumnHeaderHeight;
        using (Context.PushClip(new Rect(BodyRect.X, Top, BodyRect.Width, LayoutMetrics.ColumnHeaderHeight)))
        {
            foreach (PivotGridAxisItem ColumnItem in Engine.ColumnItems)
            {
                double GroupWidth = GetColumnGroupWidth(ColumnItem);
                Rect GroupRect = new(X, Top, GroupWidth, MeasureHeaderHeight);
                DrawBand(Context, GroupRect, HeaderBrush);
                PivotGridField DisplayField = ColumnFields.Count == 0 ? null : ColumnFields[Math.Min(ColumnFields.Count - 1, ColumnItem.Values.Count - 1)];
                DrawText(Context, ColumnItem.Text, GroupRect, TextBrush, FontWeight.SemiBold, IsRightAlignedField(DisplayField?.Name));

                double MeasureX = X;
                double MeasureY = Top + (Engine.Measures.Count > 1 ? MeasureHeaderHeight : 0);
                foreach (PivotGridMeasure Measure in Engine.Measures)
                {
                    double Width = GetValueCellWidth(ColumnItem, Measure);
                    Rect MeasureRect = new(MeasureX, MeasureY, Width, Engine.Measures.Count > 1 ? MeasureHeaderHeight : LayoutMetrics.ColumnHeaderHeight);
                    if (Engine.Measures.Count > 1)
                    {
                        DrawBand(Context, MeasureRect, HeaderBrush);
                        DrawText(Context, GetMeasureText(Measure), MeasureRect, MutedTextBrush, FontWeight.SemiBold);
                    }
                    MeasureX += Width;
                }

                X += GroupWidth;
            }

            if (fShowRowGrandTotals)
            {
                double GroupWidth = GetTotalGroupWidth();
                Rect GroupRect = new(X, Top, GroupWidth, MeasureHeaderHeight);
                DrawBand(Context, GroupRect, HeaderBrush);
                DrawText(Context, "Total", GroupRect, TextBrush, FontWeight.SemiBold);

                double MeasureX = X;
                double MeasureY = Top + (Engine.Measures.Count > 1 ? MeasureHeaderHeight : 0);
                foreach (PivotGridMeasure Measure in Engine.Measures)
                {
                    double Width = GetValueCellWidth(null, Measure);
                    Rect MeasureRect = new(MeasureX, MeasureY, Width, Engine.Measures.Count > 1 ? MeasureHeaderHeight : LayoutMetrics.ColumnHeaderHeight);
                    if (Engine.Measures.Count > 1)
                    {
                        DrawBand(Context, MeasureRect, HeaderBrush);
                        DrawText(Context, GetMeasureText(Measure), MeasureRect, MutedTextBrush, FontWeight.SemiBold);
                    }
                    MeasureX += Width;
                }
            }
        }
    }
    void DrawRows(DrawingContext Context)
    {
        double RowHeaderWidth = GetRowHeaderWidth();
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        double Y = GetBodyTop() - fVerticalOffset;
        for (int RowIndex = 0; RowIndex < Engine.VisibleRowNodes.Count; RowIndex++)
        {
            if (Y + LayoutMetrics.RowHeight < BodyRect.Y)
            {
                Y += LayoutMetrics.RowHeight;
                continue;
            }
            if (Y > BodyRect.Bottom)
                break;

            PivotGridAxisNode RowNode = Engine.VisibleRowNodes[RowIndex];
            PivotGridAxisItem RowItem = RowNode.Item;
            FontWeight ValueFontWeight = RowNode.HasChildren && RowNode.IsExpanded ? FontWeight.SemiBold : FontWeight.Normal;
            Rect HeaderRect = new(0, Y, RowHeaderWidth, LayoutMetrics.RowHeight);
            using (Context.PushClip(new Rect(0, BodyRect.Y, RowHeaderWidth, BodyRect.Height)))
            {
                DrawBand(Context, HeaderRect, HeaderBrush);
                double Indent = Math.Max(0, RowNode.Level) * LayoutMetrics.RowIndentWidth;
                Rect ExpanderRect = new(Indent, Y, LayoutMetrics.RowExpanderWidth, LayoutMetrics.RowHeight);
                if (RowNode.HasChildren)
                    DrawExpander(Context, ExpanderRect, RowNode.IsExpanded);
                PivotGridField RowField = RowNode.Level >= 0 && RowNode.Level < RowFields.Count ? RowFields[RowNode.Level] : null;
                DrawText(Context, RowItem.Text, new Rect(ExpanderRect.Right, Y, Math.Max(0, HeaderRect.Width - ExpanderRect.Right), HeaderRect.Height), TextBrush, FontWeight.SemiBold, IsRightAlignedField(RowField?.Name));
            }

            double X = RowHeaderWidth - fHorizontalOffset;
            using (Context.PushClip(BodyRect))
            {
                for (int ColumnIndex = 0; ColumnIndex < Engine.ColumnItems.Count; ColumnIndex++)
                {
                    PivotGridAxisItem ColumnItem = Engine.ColumnItems[ColumnIndex];
                    for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
                    {
                        PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                        double Width = GetValueCellWidth(ColumnItem, Measure);
                        Rect CellRect = new(X, Y, Width, LayoutMetrics.RowHeight);
                        bool IsCurrentCell = RowIndex == fCurrentRowIndex && ColumnIndex == fCurrentColumnIndex && MeasureIndex == fCurrentMeasureIndex;
                        DrawBand(Context, CellRect, IsCurrentCell ? SelectedCellBrush : GridBackgroundBrush);
                        PivotGridValueCell Cell = Engine.GetCell(RowItem, ColumnItem, Measure);
                        DrawText(Context, Cell == null ? string.Empty : Cell.Text, CellRect, TextBrush, ValueFontWeight, true);
                        if (IsCurrentCell)
                            Context.DrawRectangle(null, fSelectedCellBorderPen, CellRect.Deflate(0.5));
                        X += Width;
                    }
                }

                if (fShowRowGrandTotals)
                {
                    int TotalColumnIndex = Engine.ColumnItems.Count;
                    for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
                    {
                        PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                        double Width = GetValueCellWidth(null, Measure);
                        Rect CellRect = new(X, Y, Width, LayoutMetrics.RowHeight);
                        bool IsCurrentCell = RowIndex == fCurrentRowIndex && TotalColumnIndex == fCurrentColumnIndex && MeasureIndex == fCurrentMeasureIndex;
                        DrawBand(Context, CellRect, IsCurrentCell ? SelectedCellBrush : HeaderBrush);
                        PivotGridValueCell Cell = Engine.GetRowTotalCell(RowItem, Measure);
                        DrawText(Context, Cell == null ? string.Empty : Cell.Text, CellRect, TextBrush, FontWeight.SemiBold, true);
                        if (IsCurrentCell)
                            Context.DrawRectangle(null, fSelectedCellBorderPen, CellRect.Deflate(0.5));
                        X += Width;
                    }
                }
            }

            Y += LayoutMetrics.RowHeight;
        }

        DrawColumnGrandTotalRow(Context);
    }
    void DrawColumnGrandTotalRow(DrawingContext Context)
    {
        if (!fShowColumnGrandTotals || Engine.Measures.Count == 0)
            return;

        double RowHeaderWidth = GetRowHeaderWidth();
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        double Y = GetBodyTop() - fVerticalOffset + (Engine.VisibleRowNodes.Count * LayoutMetrics.RowHeight);
        if (Y + LayoutMetrics.RowHeight < BodyRect.Y || Y > BodyRect.Bottom)
            return;

        using (Context.PushClip(new Rect(0, BodyRect.Y, RowHeaderWidth, BodyRect.Height)))
        {
            Rect HeaderRect = new(0, Y, RowHeaderWidth, LayoutMetrics.RowHeight);
            DrawBand(Context, HeaderRect, HeaderBrush);
            DrawText(Context, "Total", new Rect(4, Y, RowHeaderWidth - 8, LayoutMetrics.RowHeight), TextBrush, FontWeight.SemiBold);
        }

        double X = RowHeaderWidth - fHorizontalOffset;
        using (Context.PushClip(BodyRect))
        {
            int TotalRowIndex = Engine.VisibleRowNodes.Count;
            for (int ColumnIndex = 0; ColumnIndex < Engine.ColumnItems.Count; ColumnIndex++)
            {
                PivotGridAxisItem ColumnItem = Engine.ColumnItems[ColumnIndex];
                for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
                {
                    PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                    double Width = GetValueCellWidth(ColumnItem, Measure);
                    Rect CellRect = new(X, Y, Width, LayoutMetrics.RowHeight);
                    bool IsCurrentCell = TotalRowIndex == fCurrentRowIndex && ColumnIndex == fCurrentColumnIndex && MeasureIndex == fCurrentMeasureIndex;
                    DrawBand(Context, CellRect, IsCurrentCell ? SelectedCellBrush : HeaderBrush);
                    PivotGridValueCell Cell = Engine.GetColumnTotalCell(ColumnItem, Measure);
                    DrawText(Context, Cell == null ? string.Empty : Cell.Text, CellRect, TextBrush, FontWeight.SemiBold, true);
                    if (IsCurrentCell)
                        Context.DrawRectangle(null, fSelectedCellBorderPen, CellRect.Deflate(0.5));
                    X += Width;
                }
            }

            if (fShowRowGrandTotals)
            {
                int TotalColumnIndex = Engine.ColumnItems.Count;
                for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
                {
                    PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                    double Width = GetValueCellWidth(null, Measure);
                    Rect CellRect = new(X, Y, Width, LayoutMetrics.RowHeight);
                    bool IsCurrentCell = TotalRowIndex == fCurrentRowIndex && TotalColumnIndex == fCurrentColumnIndex && MeasureIndex == fCurrentMeasureIndex;
                    DrawBand(Context, CellRect, IsCurrentCell ? SelectedCellBrush : HeaderBrush);
                    PivotGridValueCell Cell = Engine.GetGrandTotalCell(Measure);
                    DrawText(Context, Cell == null ? string.Empty : Cell.Text, CellRect, TextBrush, FontWeight.SemiBold, true);
                    if (IsCurrentCell)
                        Context.DrawRectangle(null, fSelectedCellBorderPen, CellRect.Deflate(0.5));
                    X += Width;
                }
            }
        }
    }
    void DrawScrollBars(DrawingContext Context)
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (HasVerticalScrollBar)
        {
            Context.DrawRectangle(ScrollBarTrackBrush, fLinePen, VerticalTrackRect);
            Context.DrawRectangle(ScrollBarThumbBrush, null, GetVerticalScrollThumbRect(), 4, 4);
        }
        if (HasHorizontalScrollBar)
        {
            Context.DrawRectangle(ScrollBarTrackBrush, fLinePen, HorizontalTrackRect);
            Context.DrawRectangle(ScrollBarThumbBrush, null, GetHorizontalScrollThumbRect(), 4, 4);
        }
        if (HasVerticalScrollBar && HasHorizontalScrollBar)
            Context.DrawRectangle(ScrollBarTrackBrush, fLinePen, new Rect(VerticalTrackRect.X, HorizontalTrackRect.Y, VerticalTrackRect.Width, HorizontalTrackRect.Height));
    }
    bool HandleVerticalScrollPointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (!HasVerticalScrollBar || !VerticalTrackRect.Contains(Point))
            return false;

        Rect ThumbRect = GetVerticalScrollThumbRect();
        if (ThumbRect.Contains(Point))
        {
            fIsVerticalScrollDragging = true;
            fVerticalScrollDragOffset = Point.Y - ThumbRect.Y;
            Args.Pointer.Capture(this);
            return true;
        }

        double Delta = Point.Y < ThumbRect.Y ? -BodyRect.Height : BodyRect.Height;
        SetVerticalOffsetCore(fVerticalOffset + Delta);
        return true;
    }
    bool HandleHorizontalScrollPointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        if (!HasHorizontalScrollBar || !HorizontalTrackRect.Contains(Point))
            return false;

        Rect ThumbRect = GetHorizontalScrollThumbRect();
        if (ThumbRect.Contains(Point))
        {
            fIsHorizontalScrollDragging = true;
            fHorizontalScrollDragOffset = Point.X - ThumbRect.X;
            Args.Pointer.Capture(this);
            return true;
        }

        double Delta = Point.X < ThumbRect.X ? -BodyRect.Width : BodyRect.Width;
        SetHorizontalOffsetCore(fHorizontalOffset + Delta);
        return true;
    }
    bool HandleMeasureResizePointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        if (!TryGetMeasureResizeHit(Point, out int ColumnIndex, out int MeasureIndex, out double BoundaryX))
            return false;
        if (MeasureIndex < 0 || MeasureIndex >= Engine.Measures.Count)
            return false;
        if (Args.ClickCount > 1)
        {
            AutoFitValueColumnWidthCore(ColumnIndex, MeasureIndex);
            return true;
        }

        fIsMeasureResizing = true;
        fMeasureResizeColumnIndex = ColumnIndex;
        fMeasureResizeIndex = MeasureIndex;
        fMeasureResizeStartX = Point.X;
        fMeasureResizeCurrentX = BoundaryX;
        PivotGridAxisItem ColumnItem = ColumnIndex >= 0 && ColumnIndex < Engine.ColumnItems.Count ? Engine.ColumnItems[ColumnIndex] : null;
        fMeasureResizeStartWidth = GetValueCellWidth(ColumnItem, Engine.Measures[MeasureIndex]);
        Args.Pointer.Capture(this);
        Cursor = new Cursor(StandardCursorType.SizeWestEast);
        InvalidateVisual();
        return true;
    }
    bool HandleMeasureResizePointerMoved(Point Point)
    {
        if (!fIsMeasureResizing || fMeasureResizeIndex < 0 || fMeasureResizeIndex >= Engine.Measures.Count)
            return false;

        fMeasureResizeCurrentX = Point.X;
        PivotGridAxisItem ColumnItem = fMeasureResizeColumnIndex >= 0 && fMeasureResizeColumnIndex < Engine.ColumnItems.Count ? Engine.ColumnItems[fMeasureResizeColumnIndex] : null;
        SetValueColumnWidth(ColumnItem, Engine.Measures[fMeasureResizeIndex], fMeasureResizeStartWidth + Point.X - fMeasureResizeStartX);
        return true;
    }
    void EndMeasureResize()
    {
        fIsMeasureResizing = false;
        fMeasureResizeColumnIndex = -1;
        fMeasureResizeIndex = -1;
        fMeasureResizeStartX = 0;
        fMeasureResizeCurrentX = 0;
        fMeasureResizeStartWidth = 0;
        Cursor = null;
        InvalidateVisual();
    }
    bool HandleRowHeaderResizePointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        if (!TryGetRowHeaderResizeHit(Point, out double BoundaryX))
            return false;
        if (Args.ClickCount > 1)
        {
            AutoFitRowHeaderWidthCore();
            return true;
        }

        fIsRowHeaderResizing = true;
        fRowHeaderResizeStartX = Point.X;
        fRowHeaderResizeCurrentX = BoundaryX;
        fRowHeaderResizeStartWidth = GetRowHeaderWidth();
        Args.Pointer.Capture(this);
        Cursor = new Cursor(StandardCursorType.SizeWestEast);
        InvalidateVisual();
        return true;
    }
    bool HandleRowHeaderResizePointerMoved(Point Point)
    {
        if (!fIsRowHeaderResizing)
            return false;

        fRowHeaderResizeCurrentX = Point.X;
        SetRowHeaderWidthCore(fRowHeaderResizeStartWidth + Point.X - fRowHeaderResizeStartX);
        return true;
    }
    void EndRowHeaderResize()
    {
        fIsRowHeaderResizing = false;
        fRowHeaderResizeStartX = 0;
        fRowHeaderResizeCurrentX = 0;
        fRowHeaderResizeStartWidth = 0;
        Cursor = null;
        InvalidateVisual();
    }
    void UpdatePointerCursor(Point Point)
    {
        Cursor = fIsMeasureResizing || fIsRowHeaderResizing || TryGetRowHeaderResizeHit(Point, out double RowHeaderBoundaryX) || TryGetMeasureResizeHit(Point, out int ColumnIndex, out int MeasureIndex, out double BoundaryX)
            ? new Cursor(StandardCursorType.SizeWestEast)
            : null;
    }
    void DrawMeasureResizeGuide(DrawingContext Context)
    {
        if (!fIsMeasureResizing)
            return;

        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        double X = Math.Clamp(fMeasureResizeCurrentX, BodyRect.X, BodyRect.Right);
        Context.DrawLine(fResizePen, new Point(X, GetGridTop()), new Point(X, BodyRect.Bottom));
    }
    void DrawRowHeaderResizeGuide(DrawingContext Context)
    {
        if (!fIsRowHeaderResizing)
            return;

        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        double RightLimit = Bounds.Width - GetVerticalScrollBarWidth(HasVerticalScrollBar);
        double X = Math.Clamp(fRowHeaderResizeCurrentX, RowHeaderMinWidth, Math.Max(RowHeaderMinWidth, RightLimit));
        Context.DrawLine(fResizePen, new Point(X, GetFieldPanelHeight()), new Point(X, BodyRect.Bottom));
    }

    // ● protected methods
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs Args)
    {
        base.OnPropertyChanged(Args);

        if (Args.Property == GridLineBrushProperty || Args.Property == SelectedCellBorderBrushProperty || Args.Property == ResizeGuideBrushProperty)
            UpdateThemePens();
    }
    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs Args)
    {
        base.OnPointerPressed(Args);

        PointerPoint PointProperties = Args.GetCurrentPoint(this);
        if (PointProperties.Properties.IsRightButtonPressed)
        {
            Point MenuPoint = Args.GetPosition(this);
            if (ShowFieldContextMenu(MenuPoint) || ShowGridContextMenu(MenuPoint))
                Args.Handled = true;
            return;
        }

        if (!PointProperties.Properties.IsLeftButtonPressed)
            return;

        Focus();
        Point Point = Args.GetPosition(this);
        if (HandleVerticalScrollPointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }
        if (HandleHorizontalScrollPointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }
        if (HandleRowHeaderResizePointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }
        if (HandleMeasureResizePointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }

        if (HandleRowExpanderPointerPressed(Point))
        {
            Args.Handled = true;
            return;
        }

        if (BeginFieldDrag(Args, Point))
        {
            Args.Handled = true;
            return;
        }

        if (HandleValueCellPointerPressed(Point))
            Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnPointerMoved(PointerEventArgs Args)
    {
        base.OnPointerMoved(Args);

        Point Point = Args.GetPosition(this);
        if (fIsVerticalScrollDragging && SetVerticalOffsetFromScroll(Point.Y))
            Args.Handled = true;
        if (fIsHorizontalScrollDragging && SetHorizontalOffsetFromScroll(Point.X))
            Args.Handled = true;
        if (Args.Handled)
            return;

        if (HandleRowHeaderResizePointerMoved(Point))
        {
            Args.Handled = true;
            return;
        }
        if (HandleMeasureResizePointerMoved(Point))
        {
            Args.Handled = true;
            return;
        }

        if (UpdateFieldDrag(Point))
            Args.Handled = true;
        if (!Args.Handled)
        {
            UpdatePointerCursor(Point);
            UpdateToolTip(Point);
        }
    }
    /// <inheritdoc />
    protected override void OnPointerExited(PointerEventArgs Args)
    {
        base.OnPointerExited(Args);

        fToolTipText = string.Empty;
        ToolTip.SetTip(this, null);
    }
    /// <inheritdoc />
    protected override void OnPointerReleased(PointerReleasedEventArgs Args)
    {
        base.OnPointerReleased(Args);

        if (fIsVerticalScrollDragging || fIsHorizontalScrollDragging)
        {
            fIsVerticalScrollDragging = false;
            fIsHorizontalScrollDragging = false;
            fVerticalScrollDragOffset = 0;
            fHorizontalScrollDragOffset = 0;
            Args.Pointer.Capture(null);
            Args.Handled = true;
            return;
        }
        if (fIsMeasureResizing)
        {
            EndMeasureResize();
            Args.Pointer.Capture(null);
            Args.Handled = true;
            return;
        }
        if (fIsRowHeaderResizing)
        {
            EndRowHeaderResize();
            Args.Pointer.Capture(null);
            Args.Handled = true;
            return;
        }

        if (EndFieldDrag(Args, Args.GetPosition(this)))
            Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnPointerWheelChanged(PointerWheelEventArgs Args)
    {
        base.OnPointerWheelChanged(Args);

        bool Handled = false;
        if ((Args.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
            Handled = SetHorizontalOffsetCore(fHorizontalOffset - (Args.Delta.Y * LayoutMetrics.ValueCellWidth / 2));
        else
            Handled = SetVerticalOffsetCore(fVerticalOffset - (Args.Delta.Y * LayoutMetrics.RowHeight * 3));

        if (Handled)
            Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs Args)
    {
        base.OnKeyDown(Args);

        bool Handled = false;
        bool IsControl = (Args.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control;
        bool IsShift = (Args.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift;
        if (IsControl && Args.Key == Key.C)
        {
            _ = IsShift ? CopyPivotTextAsync() : CopyCurrentCellTextAsync();
            Handled = true;
        }
        else switch (Args.Key)
        {
            case Key.Left:
                Handled = MoveCurrentCell(0, -1);
                break;
            case Key.Right:
                Handled = MoveCurrentCell(0, 1);
                break;
            case Key.Up:
                Handled = MoveCurrentCell(-1, 0);
                break;
            case Key.Down:
                Handled = MoveCurrentCell(1, 0);
                break;
            case Key.Home:
                Handled = IsControl ? MoveCurrentCellToGridStart() : MoveCurrentCellToRowStart();
                break;
            case Key.End:
                Handled = IsControl ? MoveCurrentCellToGridEnd() : MoveCurrentCellToRowEnd();
                break;
            case Key.PageUp:
                Handled = MoveCurrentCellPage(-1);
                break;
            case Key.PageDown:
                Handled = MoveCurrentCellPage(1);
                break;
        }

        if (Handled)
            Args.Handled = true;
    }

    // ● constructor
    static PivotGrid()
    {
        AffectsRender<PivotGrid>(
            GridBackgroundBrushProperty,
            HeaderBrushProperty,
            TextBrushProperty,
            MutedTextBrushProperty,
            GridLineBrushProperty,
            SelectedCellBrushProperty,
            SelectedCellBorderBrushProperty,
            ScrollBarTrackBrushProperty,
            ScrollBarThumbBrushProperty,
            ResizeGuideBrushProperty);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGrid"/> class.
    /// </summary>
    public PivotGrid()
    {
        Focusable = true;
        UpdateThemePens();
        Engine = new PivotGridEngine();
    }

    // ● public methods
    /// <summary>
    /// Moves a source field to a pivot role.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="TargetRole">The target field role.</param>
    /// <returns>True if the field moved; otherwise, false.</returns>
    public bool MoveField(string FieldName, PivotGridFieldRole TargetRole)
    {
        return MoveFieldCore(FieldName, TargetRole, -1);
    }
    /// <summary>
    /// Moves a source field to a pivot role at a target insertion index.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="TargetRole">The target field role.</param>
    /// <param name="TargetIndex">The insertion index, or -1 to append.</param>
    /// <returns>True if the field moved; otherwise, false.</returns>
    public bool MoveField(string FieldName, PivotGridFieldRole TargetRole, int TargetIndex)
    {
        return MoveFieldCore(FieldName, TargetRole, TargetIndex);
    }
    /// <summary>
    /// Toggles sorting for a row or column field using the none, ascending, descending cycle.
    /// </summary>
    /// <param name="Role">The field role.</param>
    /// <param name="FieldName">The field name.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ToggleSort(PivotGridFieldRole Role, string FieldName)
    {
        return Engine.ToggleSort(Role, FieldName);
    }
    /// <summary>
    /// Sets sorting for a row or column field.
    /// </summary>
    /// <param name="Role">The field role.</param>
    /// <param name="FieldName">The field name.</param>
    /// <param name="Direction">The sort direction.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool SetSort(PivotGridFieldRole Role, string FieldName, PivotGridSortDirection Direction)
    {
        return Engine.SetSort(Role, FieldName, Direction);
    }
    /// <summary>
    /// Clears active sorting.
    /// </summary>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ClearSort()
    {
        return Engine.ClearSort();
    }
    /// <summary>
    /// Sets a value-list filter for a source field.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="Values">The accepted field values.</param>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool SetFieldFilter(string FieldName, IEnumerable<object> Values)
    {
        return Engine.SetFieldFilter(FieldName, Values);
    }
    /// <summary>
    /// Clears a value-list filter from a source field.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool ClearFieldFilter(string FieldName)
    {
        return Engine.ClearFieldFilter(FieldName);
    }
    /// <summary>
    /// Clears all value-list filters.
    /// </summary>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool ClearFilters()
    {
        return Engine.ClearFilters();
    }
    /// <summary>
    /// Expands all row-axis nodes.
    /// </summary>
    /// <returns>True if any expanded state changed; otherwise, false.</returns>
    public bool ExpandAllRows()
    {
        return Engine.ExpandAllRows();
    }
    /// <summary>
    /// Collapses all row-axis nodes.
    /// </summary>
    /// <returns>True if any expanded state changed; otherwise, false.</returns>
    public bool CollapseAllRows()
    {
        return Engine.CollapseAllRows();
    }
    /// <summary>
    /// Sets the current value cell.
    /// </summary>
    /// <param name="RowIndex">The visible row index.</param>
    /// <param name="ColumnIndex">The column item index.</param>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool SetCurrentCell(int RowIndex, int ColumnIndex, int MeasureIndex)
    {
        if (RowIndex != -1 || ColumnIndex != -1 || MeasureIndex != -1)
            if (!IsValidCellIndex(RowIndex, ColumnIndex, MeasureIndex))
                return false;

        bool Result = SetCurrentCellCore(RowIndex, ColumnIndex, MeasureIndex);
        return ScrollCurrentCellIntoViewCore() || Result;
    }
    /// <summary>
    /// Scrolls the current value cell into view.
    /// </summary>
    /// <returns>True if any scroll offset changed; otherwise, false.</returns>
    public bool ScrollCurrentCellIntoView()
    {
        return ScrollCurrentCellIntoViewCore();
    }
    /// <summary>
    /// Clears the current value cell.
    /// </summary>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool ClearCurrentCell()
    {
        return SetCurrentCellCore(-1, -1, -1);
    }
    /// <summary>
    /// Sets the horizontal scroll offset.
    /// </summary>
    /// <param name="HorizontalOffset">The horizontal scroll offset.</param>
    /// <returns>True if the offset changed; otherwise, false.</returns>
    public bool SetHorizontalOffset(double HorizontalOffset)
    {
        return SetHorizontalOffsetCore(HorizontalOffset);
    }
    /// <summary>
    /// Sets the vertical scroll offset.
    /// </summary>
    /// <param name="VerticalOffset">The vertical scroll offset.</param>
    /// <returns>True if the offset changed; otherwise, false.</returns>
    public bool SetVerticalOffset(double VerticalOffset)
    {
        return SetVerticalOffsetCore(VerticalOffset);
    }
    /// <summary>
    /// Sets the width of a measure value column.
    /// </summary>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <param name="Width">The new width.</param>
    /// <returns>True if the width changed; otherwise, false.</returns>
    public bool SetMeasureWidth(int MeasureIndex, double Width)
    {
        if (MeasureIndex < 0 || MeasureIndex >= Engine.Measures.Count)
            return false;

        return SetMeasureWidth(Engine.Measures[MeasureIndex], Width);
    }
    /// <summary>
    /// Sets the width of a specific visible value column.
    /// </summary>
    /// <param name="ColumnIndex">The visible column item index, or <see cref="PivotGridEngine.ColumnItems"/> count for the total column.</param>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <param name="Width">The new width.</param>
    /// <returns>True if the width changed; otherwise, false.</returns>
    public bool SetValueColumnWidth(int ColumnIndex, int MeasureIndex, double Width)
    {
        if (ColumnIndex < 0 || ColumnIndex >= GetSelectableColumnCount())
            return false;
        if (MeasureIndex < 0 || MeasureIndex >= Engine.Measures.Count)
            return false;

        PivotGridAxisItem ColumnItem = ColumnIndex < Engine.ColumnItems.Count ? Engine.ColumnItems[ColumnIndex] : null;
        return SetValueColumnWidth(ColumnItem, Engine.Measures[MeasureIndex], Width);
    }
    /// <summary>
    /// Sets the aggregate kind of a measure.
    /// </summary>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <param name="AggregateKind">The aggregate kind.</param>
    /// <returns>True if the aggregate kind changed; otherwise, false.</returns>
    public bool SetMeasureAggregate(int MeasureIndex, PivotGridAggregateKind AggregateKind)
    {
        if (MeasureIndex < 0 || MeasureIndex >= Engine.Measures.Count)
            return false;

        return SetMeasureAggregate(Engine.Measures[MeasureIndex], AggregateKind);
    }
    /// <summary>
    /// Auto-fits visible value column widths using the current pivot content.
    /// </summary>
    /// <returns>True if any width changed; otherwise, false.</returns>
    public bool AutoFitValueColumnWidths()
    {
        return AutoFitValueColumnWidthsCore();
    }
    /// <summary>
    /// Auto-fits a visible value column width using the current pivot content.
    /// </summary>
    /// <param name="ColumnIndex">The visible column item index, or the row total column index.</param>
    /// <param name="MeasureIndex">The measure index.</param>
    /// <returns>True if the width changed; otherwise, false.</returns>
    public bool AutoFitValueColumnWidth(int ColumnIndex, int MeasureIndex)
    {
        return AutoFitValueColumnWidthCore(ColumnIndex, MeasureIndex);
    }
    /// <summary>
    /// Auto-fits the row header width using the current visible row-axis content.
    /// </summary>
    /// <returns>True if the row header width changed; otherwise, false.</returns>
    public bool AutoFitRowHeaderWidth()
    {
        return AutoFitRowHeaderWidthCore();
    }
    /// <summary>
    /// Sets the row header width.
    /// </summary>
    /// <param name="Width">The row header width.</param>
    /// <returns>True if the row header width changed; otherwise, false.</returns>
    public bool SetRowHeaderWidth(double Width)
    {
        return SetRowHeaderWidthCore(Width);
    }
    /// <summary>
    /// Resets the row header width to the default layout value.
    /// </summary>
    /// <returns>True if the row header width changed; otherwise, false.</returns>
    public bool ResetRowHeaderWidth()
    {
        return ResetRowHeaderWidthCore();
    }
    /// <summary>
    /// Clears all visible value column width overrides.
    /// </summary>
    /// <returns>True if any width override was cleared; otherwise, false.</returns>
    public bool ClearValueColumnWidths()
    {
        return ClearValueColumnWidthsCore();
    }
    /// <summary>
    /// Opens the default pivot grid settings dialog.
    /// </summary>
    /// <returns>True if the dialog applied changes; otherwise, false.</returns>
    public Task<bool> ShowSettingsDialogAsync()
    {
        return ShowSettingsDialogCoreAsync();
    }
    /// <summary>
    /// Creates a serializable snapshot of the current pivot layout settings.
    /// </summary>
    /// <param name="Name">The settings name.</param>
    /// <returns>The settings snapshot.</returns>
    public PivotGridSettings CreateSettings(string Name = "Default")
    {
        PivotGridSettings Result = new()
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "Default" : Name,
            ShowFieldPanel = ShowFieldPanel,
            ShowRowGrandTotals = ShowRowGrandTotals,
            ShowColumnGrandTotals = ShowColumnGrandTotals,
            ShowToolTips = ShowToolTips,
            RowHeaderWidth = LayoutMetrics.RowHeaderWidth,
            SortRole = Engine.SortRole,
            SortFieldName = Engine.SortFieldName,
            SortDirection = Engine.SortDirection,
        };
        foreach (PivotGridField Field in RowFields)
            Result.RowFields.Add(new PivotGridFieldSettings { Name = Field.Name, Header = Field.Header, DisplayFormat = Field.DisplayFormat, Width = Field.Width });
        foreach (PivotGridField Field in ColumnFields)
            Result.ColumnFields.Add(new PivotGridFieldSettings { Name = Field.Name, Header = Field.Header, DisplayFormat = Field.DisplayFormat, Width = Field.Width });
        foreach (PivotGridMeasure Measure in Measures)
            Result.Measures.Add(new PivotGridMeasureSettings
            {
                Name = Measure.Name,
                Header = Measure.Header,
                SourceFieldName = Measure.SourceFieldName,
                AggregateKind = Measure.AggregateKind,
                DisplayFormat = Measure.DisplayFormat,
                Width = Measure.Width,
            });
        foreach (KeyValuePair<string, double> Entry in fValueColumnWidths)
            if (!string.IsNullOrWhiteSpace(Entry.Key) && Entry.Value > 0)
                Result.ValueColumnWidths[Entry.Key] = Entry.Value;
        Result.CollapsedRowKeys.AddRange(Engine.GetCollapsedRowKeys());
        foreach (string FieldName in Engine.FilterFieldNames)
            Result.Filters.Add(new PivotGridFilterSettings
            {
                FieldName = FieldName,
                AcceptedValueKeys = Engine.GetFieldFilterKeys(FieldName).ToList(),
            });

        return Result;
    }
    /// <summary>
    /// Applies serialized pivot layout settings.
    /// </summary>
    /// <param name="Settings">The settings to apply.</param>
    public void ApplySettings(PivotGridSettings Settings)
    {
        if (Settings == null)
            return;

        Dictionary<string, PivotGridSourceField> SourceMap = SourceFields.ToDictionary(Field => Field.Name, StringComparer.OrdinalIgnoreCase);
        Engine.ClearFilters();
        ShowFieldPanel = Settings.ShowFieldPanel;
        ShowRowGrandTotals = Settings.ShowRowGrandTotals;
        ShowColumnGrandTotals = Settings.ShowColumnGrandTotals;
        ShowToolTips = Settings.ShowToolTips;
        if (Settings.RowHeaderWidth > 0)
            LayoutMetrics.RowHeaderWidth = Settings.RowHeaderWidth;
        fValueColumnWidths.Clear();
        foreach (KeyValuePair<string, double> Entry in Settings.ValueColumnWidths)
            if (!string.IsNullOrWhiteSpace(Entry.Key) && Entry.Value > 0)
                fValueColumnWidths[Entry.Key] = Entry.Value;
        RowFields.Clear();
        ColumnFields.Clear();
        Measures.Clear();
        foreach (PivotGridFieldSettings Item in Settings.RowFields)
            if (Item != null && SourceMap.TryGetValue(Item.Name, out PivotGridSourceField SourceField) && SourceField.CanUseAsAxis)
                RowFields.Add(new PivotGridField { Name = SourceField.Name, Header = string.IsNullOrWhiteSpace(Item.Header) ? SourceField.Header : Item.Header, DisplayFormat = Item.DisplayFormat, Width = Item.Width });
        foreach (PivotGridFieldSettings Item in Settings.ColumnFields)
            if (Item != null && SourceMap.TryGetValue(Item.Name, out PivotGridSourceField SourceField) && SourceField.CanUseAsAxis)
                ColumnFields.Add(new PivotGridField { Name = SourceField.Name, Header = string.IsNullOrWhiteSpace(Item.Header) ? SourceField.Header : Item.Header, DisplayFormat = Item.DisplayFormat, Width = Item.Width });
        foreach (PivotGridMeasureSettings Item in Settings.Measures)
            if (Item != null && SourceMap.TryGetValue(Item.SourceFieldName, out PivotGridSourceField SourceField) && SourceField.CanUseAsMeasure)
                Measures.Add(new PivotGridMeasure
                {
                    Name = string.IsNullOrWhiteSpace(Item.Name) ? SourceField.Name : Item.Name,
                    Header = string.IsNullOrWhiteSpace(Item.Header) ? SourceField.Header : Item.Header,
                    SourceFieldName = SourceField.Name,
                    AggregateKind = Item.AggregateKind,
                    DisplayFormat = Item.DisplayFormat,
                    Width = Item.Width,
                });

        Engine.ClearSort();
        if (Settings.SortDirection != PivotGridSortDirection.None)
            Engine.SetSort(Settings.SortRole, Settings.SortFieldName, Settings.SortDirection);
        foreach (PivotGridFilterSettings Filter in Settings.Filters)
            if (Filter != null && SourceMap.ContainsKey(Filter.FieldName))
                Engine.SetFieldFilterKeys(Filter.FieldName, Filter.AcceptedValueKeys);
        Engine.Rebuild();
        Engine.SetCollapsedRowKeys(Settings.CollapsedRowKeys);
        InvalidateVisual();
    }
    /// <summary>
    /// Saves current pivot layout settings to a JSON file.
    /// </summary>
    /// <param name="FilePath">The target JSON file path.</param>
    /// <param name="Name">The settings name.</param>
    public void SaveSettings(string FilePath, string Name = "Default")
    {
        ValidateSettingsFilePath(FilePath);
        string Json = JsonSerializer.Serialize(CreateSettings(Name), fSettingsJsonOptions);
        File.WriteAllText(FilePath, Json, Encoding.UTF8);
    }
    /// <summary>
    /// Loads pivot layout settings from a JSON file.
    /// </summary>
    /// <param name="FilePath">The source JSON file path.</param>
    /// <returns>True if settings loaded; otherwise, false.</returns>
    public bool LoadSettings(string FilePath)
    {
        ValidateSettingsFilePath(FilePath);
        if (!File.Exists(FilePath))
            return false;

        string Json = File.ReadAllText(FilePath, Encoding.UTF8);
        PivotGridSettings Settings = JsonSerializer.Deserialize<PivotGridSettings>(Json, fSettingsJsonOptions);
        if (Settings == null)
            return false;

        ApplySettings(Settings);
        return true;
    }
    /// <summary>
    /// Creates an export snapshot from the current visible pivot projection.
    /// </summary>
    /// <returns>The export snapshot.</returns>
    public PivotGridExportSnapshot CreateExportSnapshot()
    {
        List<PivotGridExportField> ExportRowFields = RowFields
            .Select(Field => new PivotGridExportField(Field))
            .ToList();
        List<PivotGridExportField> ExportColumnFields = ColumnFields
            .Select(Field => new PivotGridExportField(Field))
            .ToList();
        List<PivotGridExportMeasure> ExportMeasures = Measures
            .Select(Measure => new PivotGridExportMeasure(Measure))
            .ToList();
        Dictionary<PivotGridMeasure, PivotGridExportMeasure> MeasureMap = ExportMeasures.ToDictionary(Measure => Measure.Measure);
        List<PivotGridExportValueColumn> ValueColumns = new();

        for (int ColumnIndex = 0; ColumnIndex < Engine.ColumnItems.Count; ColumnIndex++)
            for (int MeasureIndex = 0; MeasureIndex < Measures.Count; MeasureIndex++)
            {
                PivotGridMeasure Measure = Measures[MeasureIndex];
                ValueColumns.Add(new PivotGridExportValueColumn(Engine.ColumnItems[ColumnIndex], ColumnIndex, MeasureMap[Measure], MeasureIndex, false));
            }

        if (ShowRowGrandTotals)
            for (int MeasureIndex = 0; MeasureIndex < Measures.Count; MeasureIndex++)
            {
                PivotGridMeasure Measure = Measures[MeasureIndex];
                ValueColumns.Add(new PivotGridExportValueColumn(null, Engine.ColumnItems.Count, MeasureMap[Measure], MeasureIndex, true));
            }

        List<PivotGridExportRow> Rows = new();
        for (int RowIndex = 0; RowIndex < Engine.VisibleRowNodes.Count; RowIndex++)
        {
            PivotGridAxisNode RowNode = Engine.VisibleRowNodes[RowIndex];
            List<PivotGridExportCell> Cells = new();
            foreach (PivotGridExportValueColumn Column in ValueColumns)
            {
                PivotGridValueCell Cell = Column.IsTotal
                    ? Engine.GetRowTotalCell(RowNode.Item, Column.Measure.Measure)
                    : Engine.GetCell(RowNode.Item, Column.ColumnItem, Column.Measure.Measure);
                Cells.Add(new PivotGridExportCell(Column, Cell?.Value, Cell?.Text));
            }

            Rows.Add(new PivotGridExportRow(RowNode, RowIndex, CreateExportRowHeaderText(RowNode, false), CreateExportRowTexts(RowNode, false), Cells, false));
        }

        if (ShowColumnGrandTotals)
        {
            List<PivotGridExportCell> Cells = new();
            foreach (PivotGridExportValueColumn Column in ValueColumns)
            {
                PivotGridValueCell Cell = Column.IsTotal
                    ? Engine.GetGrandTotalCell(Column.Measure.Measure)
                    : Engine.GetColumnTotalCell(Column.ColumnItem, Column.Measure.Measure);
                Cells.Add(new PivotGridExportCell(Column, Cell?.Value, Cell?.Text));
            }

            Rows.Add(new PivotGridExportRow(null, Engine.VisibleRowNodes.Count, CreateExportRowHeaderText(null, true), CreateExportRowTexts(null, true), Cells, true));
        }

        return new PivotGridExportSnapshot(ExportRowFields, ExportColumnFields, ExportMeasures, ValueColumns, Rows);
    }
    /// <summary>
    /// Creates tab-separated text from the current visible pivot projection.
    /// </summary>
    /// <returns>The tab-separated pivot matrix text.</returns>
    public string CreateClipboardText()
    {
        return CreateClipboardText(CreateExportSnapshot());
    }
    /// <summary>
    /// Gets the tooltip text for a point in grid coordinates.
    /// </summary>
    /// <param name="Point">The point in grid coordinates.</param>
    /// <returns>The tooltip text, or an empty string when no tooltip applies.</returns>
    public string GetToolTipText(Point Point)
    {
        if (!ShowToolTips)
            return string.Empty;

        return GetHitToolTipText(HitTest(Point));
    }
    /// <summary>
    /// Exports the current pivot grid using a specified exporter.
    /// </summary>
    /// <param name="Exporter">The exporter.</param>
    /// <param name="FilePath">The full export file path.</param>
    public void SaveExport(PivotGridExporter Exporter, string FilePath)
    {
        if (Exporter == null)
            throw new ArgumentNullException(nameof(Exporter));

        Exporter.Export(this, CreateExportSnapshot(), FilePath);
    }
    /// <summary>
    /// Performs a control-level hit test.
    /// </summary>
    /// <param name="Point">The point in grid coordinates.</param>
    /// <returns>The hit-test result.</returns>
    public PivotGridHitTestResult HitTest(Point Point)
    {
        ClampScrollOffsets();
        if (Point.X < 0 || Point.Y < 0)
            return PivotGridHitTestResult.Empty;

        double RowHeaderWidth = GetRowHeaderWidth();
        GetScrollLayout(out bool HasVerticalScrollBar, out bool HasHorizontalScrollBar, out Rect BodyRect, out Rect HorizontalTrackRect, out Rect VerticalTrackRect);
        bool HasBounds = Bounds.Width > 0 && Bounds.Height > 0;

        if (TryGetRowHeaderResizeHit(Point, out double RowHeaderBoundaryX))
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.RowHeaderResizer };
        if (TryGetMeasureResizeHit(Point, out int ResizeColumnIndex, out int ResizeMeasureIndex, out double BoundaryX))
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.MeasureResizer, ColumnIndex = ResizeColumnIndex, MeasureIndex = ResizeMeasureIndex, Measure = Engine.Measures[ResizeMeasureIndex] };

        if (Point.Y < GetFieldPanelHeight())
        {
            double X = 130;
            double Y = Math.Max(0, (GetFieldPanelHeight() - 24) / 2);
            IReadOnlyList<PivotGridSourceField> Fields = AvailableFields;
            for (int Index = 0; Index < Fields.Count; Index++)
            {
                PivotGridSourceField Field = Fields[Index];
                Rect ChipRect = GetFieldChipRect(X, Y, GetSourceFieldText(Field));
                if (ChipRect.Contains(Point))
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.FieldPanel, Kind = PivotGridHitTestKind.AvailableField, ColumnIndex = Index, SourceField = Field };

                X = ChipRect.Right + 6;
            }

            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.FieldPanel, Kind = PivotGridHitTestKind.Band };
        }

        if (Point.Y < GetFieldPanelHeight() + LayoutMetrics.AxisPanelHeight)
        {
            PivotGridBand Band = PivotGridBand.AxisPanel;
            double ChipY = GetFieldPanelHeight() + Math.Max(0, (LayoutMetrics.AxisPanelHeight - 24) / 2);
            if (Point.X < RowHeaderWidth)
            {
                if (HitTestAxisFields(Point, 0, ChipY, Measures.Select(GetMeasureText).ToList(), out int MeasureIndex))
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = Band, Kind = PivotGridHitTestKind.MeasureField, MeasureIndex = MeasureIndex, Measure = Measures[MeasureIndex] };
            }
            else if (HitTestAxisFields(Point, RowHeaderWidth, ChipY, ColumnFields.Select(GetAxisFieldText).ToList(), out int Index))
            {
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = Band, Kind = PivotGridHitTestKind.ColumnField, ColumnIndex = Index };
            }

            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = Band, Kind = PivotGridHitTestKind.Band };
        }

        double MatrixY = Point.Y - GetGridTop();
        if (Point.X < RowHeaderWidth && MatrixY < LayoutMetrics.ColumnHeaderHeight)
        {
            double ChipY = GetGridTop() + Math.Max(0, (LayoutMetrics.ColumnHeaderHeight - 24) / 2);
            if (HitTestAxisFields(Point, 0, ChipY, RowFields.Select(GetAxisFieldText).ToList(), out int Index))
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Corner, Kind = PivotGridHitTestKind.RowField, RowIndex = Index };

            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Corner, Kind = PivotGridHitTestKind.Band };
        }
        if (HasBounds && HasVerticalScrollBar && VerticalTrackRect.Contains(Point))
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.Band };
        if (HasBounds && HasHorizontalScrollBar && HorizontalTrackRect.Contains(Point))
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.Band };
        if (Point.X < RowHeaderWidth)
        {
            if (HasBounds && (Point.Y < BodyRect.Y || Point.Y >= BodyRect.Bottom))
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.RowHeader, Kind = PivotGridHitTestKind.Band };

            int RowIndex = (int)((MatrixY - LayoutMetrics.ColumnHeaderHeight + fVerticalOffset) / LayoutMetrics.RowHeight);
            if (RowIndex >= 0 && RowIndex < Engine.VisibleRowNodes.Count)
            {
                PivotGridAxisNode Node = Engine.VisibleRowNodes[RowIndex];
                double RowTop = GetGridTop() + LayoutMetrics.ColumnHeaderHeight + (RowIndex * LayoutMetrics.RowHeight) - fVerticalOffset;
                double ExpanderLeft = Math.Max(0, Node.Level) * LayoutMetrics.RowIndentWidth;
                Rect ExpanderRect = new(ExpanderLeft, RowTop, LayoutMetrics.RowExpanderWidth, LayoutMetrics.RowHeight);
                PivotGridHitTestKind Kind = Node.HasChildren && ExpanderRect.Contains(Point)
                    ? PivotGridHitTestKind.RowExpander
                    : PivotGridHitTestKind.RowHeader;
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.RowHeader, Kind = Kind, RowIndex = RowIndex, RowNode = Node, RowItem = Node.Item };
            }
            if (fShowColumnGrandTotals && RowIndex == Engine.VisibleRowNodes.Count)
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.RowHeader, Kind = PivotGridHitTestKind.RowHeader, RowIndex = RowIndex };

            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.RowHeader, Kind = PivotGridHitTestKind.Band };
        }
        if (MatrixY < LayoutMetrics.ColumnHeaderHeight)
        {
            Rect ColumnHeaderRect = new(BodyRect.X, GetGridTop(), BodyRect.Width, LayoutMetrics.ColumnHeaderHeight);
            if (HasBounds && !ColumnHeaderRect.Contains(Point))
                return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.ColumnHeader, Kind = PivotGridHitTestKind.Band };

            double X = RowHeaderWidth - fHorizontalOffset;
            for (int ColumnIndex = 0; ColumnIndex < Engine.ColumnItems.Count; ColumnIndex++)
            {
                PivotGridAxisItem ColumnItem = Engine.ColumnItems[ColumnIndex];
                double Width = GetColumnGroupWidth(ColumnItem);
                if (Point.X >= X && Point.X < X + Width)
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.ColumnHeader, Kind = PivotGridHitTestKind.ColumnHeader, ColumnIndex = ColumnIndex, ColumnItem = ColumnItem };

                X += Width;
            }
            if (fShowRowGrandTotals)
            {
                double Width = GetTotalGroupWidth();
                if (Point.X >= X && Point.X < X + Width)
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.ColumnHeader, Kind = PivotGridHitTestKind.ColumnHeader, ColumnIndex = Engine.ColumnItems.Count };
            }

            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.ColumnHeader, Kind = PivotGridHitTestKind.Band };
        }

        if (HasBounds && !BodyRect.Contains(Point) && Point.X >= RowHeaderWidth)
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.Band };

        int BodyRowIndex = (int)((MatrixY - LayoutMetrics.ColumnHeaderHeight + fVerticalOffset) / LayoutMetrics.RowHeight);
        bool IsColumnTotalRow = fShowColumnGrandTotals && BodyRowIndex == Engine.VisibleRowNodes.Count;
        if (BodyRowIndex < 0 || BodyRowIndex >= GetSelectableRowCount())
            return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.Band };

        double CellX = RowHeaderWidth - fHorizontalOffset;
        PivotGridAxisNode RowNode = IsColumnTotalRow ? null : Engine.VisibleRowNodes[BodyRowIndex];
        PivotGridAxisItem RowItem = RowNode?.Item;
        for (int ColumnIndex = 0; ColumnIndex < Engine.ColumnItems.Count; ColumnIndex++)
        {
            PivotGridAxisItem ColumnItem = Engine.ColumnItems[ColumnIndex];
            for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
            {
                PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                double Width = GetValueCellWidth(ColumnItem, Measure);
                if (Point.X >= CellX && Point.X < CellX + Width)
                {
                    PivotGridValueCell Cell = IsColumnTotalRow ? Engine.GetColumnTotalCell(ColumnItem, Measure) : Engine.GetCell(RowItem, ColumnItem, Measure);
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.ValueCell, RowIndex = BodyRowIndex, ColumnIndex = ColumnIndex, MeasureIndex = MeasureIndex, RowNode = RowNode, RowItem = RowItem, ColumnItem = ColumnItem, Measure = Measure, Cell = Cell };
                }

                CellX += Width;
            }
        }
        if (fShowRowGrandTotals)
        {
            int TotalColumnIndex = Engine.ColumnItems.Count;
            for (int MeasureIndex = 0; MeasureIndex < Engine.Measures.Count; MeasureIndex++)
            {
                PivotGridMeasure Measure = Engine.Measures[MeasureIndex];
                double Width = GetValueCellWidth(null, Measure);
                if (Point.X >= CellX && Point.X < CellX + Width)
                {
                    PivotGridValueCell Cell = IsColumnTotalRow ? Engine.GetGrandTotalCell(Measure) : Engine.GetRowTotalCell(RowItem, Measure);
                    return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.ValueCell, RowIndex = BodyRowIndex, ColumnIndex = TotalColumnIndex, MeasureIndex = MeasureIndex, RowNode = RowNode, RowItem = RowItem, Measure = Measure, Cell = Cell };
                }

                CellX += Width;
            }
        }

        return new PivotGridHitTestResult { X = Point.X, Y = Point.Y, Band = PivotGridBand.Body, Kind = PivotGridHitTestKind.Band, RowIndex = BodyRowIndex, RowNode = RowNode, RowItem = RowItem };
    }
    /// <inheritdoc />
    public override void Render(DrawingContext Context)
    {
        base.Render(Context);

        ClampScrollOffsets();
        Rect BoundsRect = new(0, 0, Bounds.Width, Bounds.Height);
        DrawBand(Context, BoundsRect, GridBackgroundBrush);
        if (Engine == null)
            return;

        DrawAvailableFieldsPanel(Context);
        DrawAxisPanel(Context);

        double Top = GetGridTop();
        double RowHeaderWidth = GetRowHeaderWidth();
        Rect CornerRect = new(0, Top, RowHeaderWidth, LayoutMetrics.ColumnHeaderHeight);
        DrawAxisZone(Context, "Rows", GetRowFieldChips(), CornerRect, PivotGridFieldRole.Row);
        DrawColumnHeaders(Context);
        DrawRows(Context);
        DrawScrollBars(Context);
        DrawMeasureResizeGuide(Context);
        DrawRowHeaderResizeGuide(Context);
        DrawFieldDropGuide(Context);
        DrawFieldDragGhost(Context);
    }

    // ● properties
    /// <summary>
    /// Gets or sets the pivot grid engine rendered by this control.
    /// </summary>
    public PivotGridEngine Engine
    {
        get => fEngine;
        set
        {
            value ??= new PivotGridEngine();
            if (ReferenceEquals(fEngine, value))
                return;

            DetachEngine(fEngine);
            fEngine = value;
            AttachEngine(fEngine);
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets the row axis fields.
    /// </summary>
    public ObservableCollection<PivotGridField> RowFields => Engine.RowFields;
    /// <summary>
    /// Gets the column axis fields.
    /// </summary>
    public ObservableCollection<PivotGridField> ColumnFields => Engine.ColumnFields;
    /// <summary>
    /// Gets the value measures.
    /// </summary>
    public ObservableCollection<PivotGridMeasure> Measures => Engine.Measures;
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IPivotGridDataAdapter DataAdapter
    {
        get => Engine.DataAdapter;
        set
        {
            if (fOwnedDataAdapter != null && !ReferenceEquals(fOwnedDataAdapter, value))
            {
                fOwnedDataAdapter.Dispose();
                fOwnedDataAdapter = null;
                fItemsSource = null;
            }

            Engine.DataAdapter = value;
        }
    }
    /// <summary>
    /// Gets or sets an item source that is a <see cref="DataTable"/>, <see cref="DataView"/>, or implements <see cref="IList{T}"/>.
    /// </summary>
    public object ItemsSource
    {
        get => fItemsSource;
        set => SetItemsSource(value);
    }
    /// <summary>
    /// Gets the valid source fields exposed by the current adapter.
    /// </summary>
    public IReadOnlyList<PivotGridSourceField> SourceFields
    {
        get
        {
            return DataAdapter == null ? Array.Empty<PivotGridSourceField>() : DataAdapter.SourceFields;
        }
    }
    /// <summary>
    /// Gets the valid source fields not currently used by row fields, column fields, or measures.
    /// </summary>
    public IReadOnlyList<PivotGridSourceField> AvailableFields => GetAvailableFields();
    /// <summary>
    /// Gets the layout metrics used by the grid.
    /// </summary>
    public PivotGridLayoutMetrics LayoutMetrics { get; } = new();
    /// <summary>
    /// Gets the current row header width after dynamic field chip sizing is applied.
    /// </summary>
    public double ActualRowHeaderWidth => GetRowHeaderWidth();
    /// <summary>
    /// Gets or sets a value indicating whether settings menu items are visible in the context menu.
    /// </summary>
    public bool IsSettingsMenuItemsVisible
    {
        get => fIsSettingsMenuItemsVisible;
        set => fIsSettingsMenuItemsVisible = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether export menu items are visible in the context menu.
    /// </summary>
    public bool IsExportMenuItemVisible
    {
        get => fIsExportMenuItemVisible;
        set => fIsExportMenuItemVisible = value;
    }
    /// <summary>
    /// Gets or sets the suggested settings file name.
    /// </summary>
    public string SettingsSuggestedFileName
    {
        get => fSettingsSuggestedFileName;
        set => fSettingsSuggestedFileName = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the top field panel is displayed.
    /// </summary>
    public bool ShowFieldPanel
    {
        get => fShowFieldPanel;
        set
        {
            if (fShowFieldPanel == value)
                return;

            fShowFieldPanel = value;
            ClampScrollOffsets();
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether row grand totals are displayed as a total column.
    /// </summary>
    public bool ShowRowGrandTotals
    {
        get => fShowRowGrandTotals;
        set
        {
            if (fShowRowGrandTotals == value)
                return;

            fShowRowGrandTotals = value;
            EnsureCurrentCellInRange();
            ClampScrollOffsets();
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether column grand totals are displayed as a total row.
    /// </summary>
    public bool ShowColumnGrandTotals
    {
        get => fShowColumnGrandTotals;
        set
        {
            if (fShowColumnGrandTotals == value)
                return;

            fShowColumnGrandTotals = value;
            EnsureCurrentCellInRange();
            ClampScrollOffsets();
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether hover tooltips are displayed.
    /// </summary>
    public bool ShowToolTips
    {
        get => fShowToolTips;
        set
        {
            if (fShowToolTips == value)
                return;

            fShowToolTips = value;
            if (!fShowToolTips)
            {
                fToolTipText = string.Empty;
                ToolTip.SetTip(this, null);
            }
        }
    }
    /// <summary>
    /// Gets or sets the grid background brush.
    /// </summary>
    public IBrush GridBackgroundBrush { get => GetValue(GridBackgroundBrushProperty); set => SetValue(GridBackgroundBrushProperty, value); }
    /// <summary>
    /// Gets or sets the header background brush.
    /// </summary>
    public IBrush HeaderBrush { get => GetValue(HeaderBrushProperty); set => SetValue(HeaderBrushProperty, value); }
    /// <summary>
    /// Gets or sets the primary text brush.
    /// </summary>
    public IBrush TextBrush { get => GetValue(TextBrushProperty); set => SetValue(TextBrushProperty, value); }
    /// <summary>
    /// Gets or sets the muted text brush.
    /// </summary>
    public IBrush MutedTextBrush { get => GetValue(MutedTextBrushProperty); set => SetValue(MutedTextBrushProperty, value); }
    /// <summary>
    /// Gets or sets the grid line brush.
    /// </summary>
    public IBrush GridLineBrush { get => GetValue(GridLineBrushProperty); set => SetValue(GridLineBrushProperty, value); }
    /// <summary>
    /// Gets or sets the selected cell background brush.
    /// </summary>
    public IBrush SelectedCellBrush { get => GetValue(SelectedCellBrushProperty); set => SetValue(SelectedCellBrushProperty, value); }
    /// <summary>
    /// Gets or sets the selected cell border brush.
    /// </summary>
    public IBrush SelectedCellBorderBrush { get => GetValue(SelectedCellBorderBrushProperty); set => SetValue(SelectedCellBorderBrushProperty, value); }
    /// <summary>
    /// Gets or sets the scroll bar track brush.
    /// </summary>
    public IBrush ScrollBarTrackBrush { get => GetValue(ScrollBarTrackBrushProperty); set => SetValue(ScrollBarTrackBrushProperty, value); }
    /// <summary>
    /// Gets or sets the scroll bar thumb brush.
    /// </summary>
    public IBrush ScrollBarThumbBrush { get => GetValue(ScrollBarThumbBrushProperty); set => SetValue(ScrollBarThumbBrushProperty, value); }
    /// <summary>
    /// Gets or sets the measure resize guide brush.
    /// </summary>
    public IBrush ResizeGuideBrush { get => GetValue(ResizeGuideBrushProperty); set => SetValue(ResizeGuideBrushProperty, value); }
    /// <summary>
    /// Gets the horizontal scroll offset.
    /// </summary>
    public double HorizontalOffset => fHorizontalOffset;
    /// <summary>
    /// Gets the vertical scroll offset.
    /// </summary>
    public double VerticalOffset => fVerticalOffset;
    /// <summary>
    /// Gets the current value cell.
    /// </summary>
    public PivotGridValueCell CurrentCell => GetCurrentCellCore();
    /// <summary>
    /// Gets the current value cell text.
    /// </summary>
    public string CurrentCellText => CurrentCell?.Text ?? string.Empty;
    /// <summary>
    /// Gets the current visible row index.
    /// </summary>
    public int CurrentRowIndex => fCurrentRowIndex;
    /// <summary>
    /// Gets the current column item index.
    /// </summary>
    public int CurrentColumnIndex => fCurrentColumnIndex;
    /// <summary>
    /// Gets the current measure index.
    /// </summary>
    public int CurrentMeasureIndex => fCurrentMeasureIndex;

    // ● events
    /// <summary>
    /// Occurs when the current value cell changes.
    /// </summary>
    public event EventHandler CurrentCellChanged;
}

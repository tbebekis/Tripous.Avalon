// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the Avalonia visual surface for a <see cref="GroupGridEngine"/>.
/// </summary>
public class GroupGrid: Control, IGroupGridDropDownEditorHost
{
    // ● public fields
    // ● theme brush properties
    /// <summary>
    /// Defines the <see cref="GridBackgroundBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GridBackgroundBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(GridBackgroundBrush), CreateBrush(255, 255, 255));
    /// <summary>
    /// Defines the <see cref="ToolBarBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ToolBarBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ToolBarBrush), CreateBrush(246, 247, 248));
    /// <summary>
    /// Defines the <see cref="ToolButtonBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ToolButtonBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ToolButtonBrush), CreateBrush(255, 255, 255));
    /// <summary>
    /// Defines the <see cref="ToolButtonHoverBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ToolButtonHoverBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ToolButtonHoverBrush), CreateBrush(235, 241, 248));
    /// <summary>
    /// Defines the <see cref="DisabledTextBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> DisabledTextBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(DisabledTextBrush), CreateBrush(150, 156, 164));
    /// <summary>
    /// Defines the <see cref="GroupPanelBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GroupPanelBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(GroupPanelBrush), CreateBrush(235, 239, 244));
    /// <summary>
    /// Defines the <see cref="HeaderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> HeaderBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(HeaderBrush), CreateBrush(241, 243, 245));
    /// <summary>
    /// Defines the <see cref="FilterBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> FilterBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(FilterBrush), CreateBrush(250, 250, 250));
    /// <summary>
    /// Defines the <see cref="FilteredCellBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> FilteredCellBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(FilteredCellBrush), CreateBrush(255, 250, 228));
    /// <summary>
    /// Defines the <see cref="ActiveFilterBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ActiveFilterBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ActiveFilterBrush), CreateBrush(255, 246, 211));
    /// <summary>
    /// Defines the <see cref="GroupRowBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GroupRowBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(GroupRowBrush), CreateBrush(232, 238, 246));
    /// <summary>
    /// Defines the <see cref="GroupSummaryBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GroupSummaryBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(GroupSummaryBrush), CreateBrush(247, 249, 252));
    /// <summary>
    /// Defines the <see cref="SelectedBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> SelectedBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(SelectedBrush), CreateBrush(218, 236, 255));
    /// <summary>
    /// Defines the <see cref="CurrentBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> CurrentBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(CurrentBrush), CreateBrush(255, 247, 213));
    /// <summary>
    /// Defines the <see cref="EditingBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> EditingBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(EditingBrush), CreateBrush(255, 238, 204));
    /// <summary>
    /// Defines the <see cref="FooterBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> FooterBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(FooterBrush), CreateBrush(238, 240, 242));
    /// <summary>
    /// Defines the <see cref="TextBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> TextBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(TextBrush), CreateBrush(32, 37, 42));
    /// <summary>
    /// Defines the <see cref="MutedTextBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> MutedTextBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(MutedTextBrush), CreateBrush(84, 91, 99));
    /// <summary>
    /// Defines the <see cref="ScrollBarTrackBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ScrollBarTrackBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ScrollBarTrackBrush), CreateBrush(244, 246, 248));
    /// <summary>
    /// Defines the <see cref="ScrollBarThumbBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ScrollBarThumbBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ScrollBarThumbBrush), CreateBrush(188, 196, 205));
    /// <summary>
    /// Defines the <see cref="ColumnDragBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ColumnDragBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ColumnDragBrush), CreateBrush(220, 250, 252, 255));
    /// <summary>
    /// Defines the <see cref="GridLineBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> GridLineBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(GridLineBrush), CreateBrush(211, 216, 222));
    /// <summary>
    /// Defines the <see cref="CurrentBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> CurrentBorderBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(CurrentBorderBrush), CreateBrush(64, 122, 190));
    /// <summary>
    /// Defines the <see cref="EditingBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> EditingBorderBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(EditingBorderBrush), CreateBrush(194, 105, 0));
    /// <summary>
    /// Defines the <see cref="ResizeGuideBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> ResizeGuideBrushProperty = AvaloniaProperty.Register<GroupGrid, IBrush>(nameof(ResizeGuideBrush), CreateBrush(80, 120, 170));

    // ● private fields
    // ● toolbar constants
    const string InsertToolButtonName = "Insert";
    const string DeleteToolButtonName = "Delete";
    const string EditToolButtonName = "Edit";

    // ● core state
    static readonly JsonSerializerOptions fSettingsJsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };
    readonly ObservableCollection<GroupGridToolButton> fToolButtons = new();
    GroupGridEngine fEngine;
    object fItemsSource;
    IDisposable fOwnedDataAdapter;
    int fFirstVisibleNodeIndex;
    bool fAutoGenerateColumns;

    // ● interaction flags
    bool fIsVerticalScrollDragging;
    bool fIsHorizontalScrollDragging;
    bool fIsColumnResizing;
    bool fIsColumnDragging;
    bool fIsColumnDragActive;
    bool fIsDeleteConfirmationOpen;
    bool fIsColumnManagerOpen;

    // ● band visibility
    bool fIsToolBarVisible = true;
    bool fIsFilterPanelVisible = true;
    bool fIsColumnHeadersVisible = true;
    bool fIsGroupPanelVisible = true;
    bool fIsTotalsSummaryVisible = true;
    bool fAreIdColumnsVisible = true;

    // ● toolbar state
    GroupGridToolButton fPointerOverToolButton;

    // ● column interaction state
    bool fColumnDragFromGroupPanel;
    bool fIsColumnManagerMenuItemVisible = true;
    bool fIsSettingsMenuItemsVisible = true;
    GroupGridDropDownPlacementMode fDropDownPlacementMode = GroupGridDropDownPlacementMode.Auto;
    string fSettingsSuggestedFileName = "group-grid-settings.json";
    string fEmptyGroupPanelText = "Drag a column header here to create a group";
    double fVerticalScrollDragOffset;
    double fHorizontalScrollDragOffset;
    double fHorizontalOffset;

    // ● column resize state
    double fColumnResizeStartX;
    double fColumnResizeStartWidth;
    double fColumnResizeCurrentX;
    GroupGridColumn fColumnResizeColumn;

    // ● column drag state
    double fColumnDragStartX;
    double fColumnDragCurrentX;
    double fColumnDragCurrentY;
    double fColumnDragDropX;
    int fColumnDragDropIndex = -1;
    int fColumnDragGroupDropIndex = -1;
    int fColumnDragSourceGroupIndex = -1;
    GroupGridColumn fColumnDragColumn;

    // ● editing state
    GroupGridColumn fFilterEditColumn;
    GroupGridInplaceEditorBase fActiveEditor;
    Border fActiveDropDownHost;
    Control fActiveDropDownControl;
    ListBox fActiveDropDownListBox;
    Popup fActiveDropDownPopup;
    Rect fLastEditorRect;
    Rect fLastDropDownRect;
    int fActiveDropDownItemCount;
    string fFilterEditText = string.Empty;
    string fCellEditText = string.Empty;
    bool fIsClosingEditor;
    bool fHasHorizontalScrollBar;

    // ● theme state
    Pen fLinePen;
    Pen fCurrentPen;
    Pen fEditingPen;
    Pen fResizePen;

    // ● private methods
    // ● theme helpers
    static IBrush CreateBrush(byte Red, byte Green, byte Blue)
    {
        return new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
    }
    static IBrush CreateBrush(byte Alpha, byte Red, byte Green, byte Blue)
    {
        return new SolidColorBrush(Color.FromArgb(Alpha, Red, Green, Blue));
    }
    Pen CreateLinePen()
    {
        return new Pen(GridLineBrush, 1);
    }
    Pen CreateCurrentPen()
    {
        return new Pen(CurrentBorderBrush, 1);
    }
    Pen CreateEditingPen()
    {
        return new Pen(EditingBorderBrush, 2);
    }
    Pen CreateResizePen()
    {
        return new Pen(ResizeGuideBrush, 1);
    }
    void UpdateThemePens()
    {
        fLinePen = CreateLinePen();
        fCurrentPen = CreateCurrentPen();
        fEditingPen = CreateEditingPen();
        fResizePen = CreateResizePen();
    }

    // ● engine event handlers
    void Engine_Changed(object Sender, EventArgs Args)
    {
        if (fEngine != null && !fEngine.IsEditing)
            fCellEditText = string.Empty;
        UpdateViewport(Bounds.Size);
        InvalidateVisual();
    }
    void Engine_ColumnsChanged(object Sender, EventArgs Args)
    {
        ApplyIdColumnsVisibility();
        Engine_Changed(Sender, Args);
    }
    void Engine_CurrentRowChanged(object Sender, EventArgs Args)
    {
        Engine_Changed(Sender, Args);
        CurrentRowChanged?.Invoke(this, EventArgs.Empty);
    }
    void Engine_BeginningEdit(object Sender, GroupGridCellEditEventArgs Args)
    {
        BeginningEdit?.Invoke(this, Args);
    }
    void Engine_CellValidating(object Sender, GroupGridCellEditEventArgs Args)
    {
        CellValidating?.Invoke(this, Args);
    }
    void Engine_CellValueCommitting(object Sender, GroupGridCellEditEventArgs Args)
    {
        CellValueCommitting?.Invoke(this, Args);
    }
    void Engine_CellValueCommitted(object Sender, GroupGridCellEditEventArgs Args)
    {
        CellValueCommitted?.Invoke(this, Args);
    }
    void Engine_EditCanceled(object Sender, GroupGridCellEditEventArgs Args)
    {
        EditCanceled?.Invoke(this, Args);
    }
    void Engine_InsertingRow(object Sender, GroupGridRowOperationEventArgs Args)
    {
        InsertingRow?.Invoke(this, Args);
    }
    void Engine_RowInserted(object Sender, GroupGridRowOperationEventArgs Args)
    {
        RowInserted?.Invoke(this, Args);
    }
    void Engine_DeletingRow(object Sender, GroupGridRowOperationEventArgs Args)
    {
        DeletingRow?.Invoke(this, Args);
    }
    void Engine_RowDeleted(object Sender, GroupGridRowOperationEventArgs Args)
    {
        RowDeleted?.Invoke(this, Args);
    }
    void Engine_EditRequested(object Sender, EventArgs Args)
    {
        EditRequested?.Invoke(this, Args);
    }
    void AttachEngine(GroupGridEngine Engine)
    {
        if (Engine == null)
            return;

        Engine.DataAdapterChanged += Engine_Changed;
        Engine.DataChanged += Engine_Changed;
        Engine.ColumnsChanged += Engine_ColumnsChanged;
        Engine.GroupColumnsChanged += Engine_Changed;
        Engine.VisibleNodesChanged += Engine_Changed;
        Engine.CurrentCellChanged += Engine_Changed;
        Engine.CurrentRowChanged += Engine_CurrentRowChanged;
        Engine.SelectionChanged += Engine_Changed;
        Engine.BeginningEdit += Engine_BeginningEdit;
        Engine.CellValidating += Engine_CellValidating;
        Engine.CellValueCommitting += Engine_CellValueCommitting;
        Engine.CellValueCommitted += Engine_CellValueCommitted;
        Engine.EditCanceled += Engine_EditCanceled;
        Engine.EditingCellChanged += Engine_Changed;
        Engine.ViewportChanged += Engine_Changed;
        Engine.SummariesChanged += Engine_Changed;
        Engine.SortingChanged += Engine_Changed;
        Engine.FiltersChanged += Engine_Changed;
        Engine.InsertingRow += Engine_InsertingRow;
        Engine.RowInserted += Engine_RowInserted;
        Engine.DeletingRow += Engine_DeletingRow;
        Engine.RowDeleted += Engine_RowDeleted;
        Engine.EditRequested += Engine_EditRequested;
    }
    void DetachEngine(GroupGridEngine Engine)
    {
        if (Engine == null)
            return;

        Engine.DataAdapterChanged -= Engine_Changed;
        Engine.DataChanged -= Engine_Changed;
        Engine.ColumnsChanged -= Engine_ColumnsChanged;
        Engine.GroupColumnsChanged -= Engine_Changed;
        Engine.VisibleNodesChanged -= Engine_Changed;
        Engine.CurrentCellChanged -= Engine_Changed;
        Engine.CurrentRowChanged -= Engine_CurrentRowChanged;
        Engine.SelectionChanged -= Engine_Changed;
        Engine.BeginningEdit -= Engine_BeginningEdit;
        Engine.CellValidating -= Engine_CellValidating;
        Engine.CellValueCommitting -= Engine_CellValueCommitting;
        Engine.CellValueCommitted -= Engine_CellValueCommitted;
        Engine.EditCanceled -= Engine_EditCanceled;
        Engine.EditingCellChanged -= Engine_Changed;
        Engine.ViewportChanged -= Engine_Changed;
        Engine.SummariesChanged -= Engine_Changed;
        Engine.SortingChanged -= Engine_Changed;
        Engine.FiltersChanged -= Engine_Changed;
        Engine.InsertingRow -= Engine_InsertingRow;
        Engine.RowInserted -= Engine_RowInserted;
        Engine.DeletingRow -= Engine_DeletingRow;
        Engine.RowDeleted -= Engine_RowDeleted;
        Engine.EditRequested -= Engine_EditRequested;
    }

    // ● toolbar helpers
    void ToolButtons_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        if (Args.OldItems != null)
            foreach (GroupGridToolButton Button in Args.OldItems)
                Button.PropertyChanged -= ToolButton_PropertyChanged;
        if (Args.NewItems != null)
            foreach (GroupGridToolButton Button in Args.NewItems)
                Button.PropertyChanged += ToolButton_PropertyChanged;

        InvalidateVisual();
    }
    void ToolButton_PropertyChanged(object Sender, PropertyChangedEventArgs Args)
    {
        InvalidateVisual();
    }
    void AddStandardToolButtons()
    {
        fToolButtons.Add(new GroupGridToolButton { Name = InsertToolButtonName, Text = "+", ToolTip = "Insert row" });
        fToolButtons.Add(new GroupGridToolButton { Name = DeleteToolButtonName, Text = "-", ToolTip = "Delete row" });
        fToolButtons.Add(new GroupGridToolButton { Name = EditToolButtonName, Text = "E", ToolTip = "Edit row", IsVisible = false });
    }
    GroupGridToolButton FindToolButton(string Name)
    {
        return string.IsNullOrWhiteSpace(Name)
            ? null
            : fToolButtons.FirstOrDefault(Button => string.Equals(Button.Name, Name, StringComparison.OrdinalIgnoreCase));
    }
    int IndexOfToolButton(string Name)
    {
        GroupGridToolButton Button = FindToolButton(Name);
        return Button == null ? -1 : fToolButtons.IndexOf(Button);
    }
    void PrepareToolButton(GroupGridToolButton Button, GroupGridToolButtonAlignment Alignment)
    {
        if (Button == null)
            throw new ArgumentNullException(nameof(Button));

        Button.Alignment = Alignment;
    }

    // ● column visibility helpers
    GroupGridColumn FindColumn(string FieldName)
    {
        return string.IsNullOrWhiteSpace(FieldName) || fEngine == null
            ? null
            : fEngine.Columns.FirstOrDefault(Column => string.Equals(Column.Name, FieldName, StringComparison.OrdinalIgnoreCase));
    }
    bool IsIdColumn(GroupGridColumn Column)
    {
        string Name = Column == null ? string.Empty : Column.Name;
        return !string.IsNullOrWhiteSpace(Name)
               && (string.Equals(Name, "Id", StringComparison.OrdinalIgnoreCase)
                   || Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
    }
    void ApplyIdColumnsVisibility()
    {
        if (fEngine == null)
            return;

        foreach (GroupGridColumn Column in fEngine.Columns.Where(IsIdColumn).ToList())
            if (Column.IsVisible != fAreIdColumnsVisible)
                fEngine.SetColumnVisible(Column, fAreIdColumnsVisible);
    }
    bool SetBandVisible(ref bool Field, bool Value)
    {
        if (Field == Value)
            return false;

        Field = Value;
        UpdateViewport(Bounds.Size);
        InvalidateMeasure();
        InvalidateVisual();
        return true;
    }
    bool GetDefaultToolButtonVisible(string Name)
    {
        GroupGridToolButton Button = FindToolButton(Name);
        return Button != null && Button.IsVisible;
    }
    void SetDefaultToolButtonVisible(string Name, bool Value)
    {
        GroupGridToolButton Button = FindToolButton(Name);
        if (Button != null)
            Button.IsVisible = Value;
    }

    // ● viewport and layout helpers
    void UpdateViewport(Size Size)
    {
        if (fEngine == null)
            return;

        double ContentWidth = Math.Max(0, Size.Width);
        double BodyHeight = Math.Max(0, Size.Height - FixedBandHeight);
        fHasHorizontalScrollBar = NeedsHorizontalScrollBar(ContentWidth);
        if (fHasHorizontalScrollBar)
            BodyHeight = Math.Max(0, BodyHeight - fEngine.LayoutMetrics.HorizontalScrollBarHeight);

        int Count = fEngine.LayoutMetrics.RowHeight <= 0
            ? 0
            : Math.Min(fEngine.VisibleNodeCount, (int)Math.Floor(BodyHeight / fEngine.LayoutMetrics.RowHeight));
        bool HasVerticalScrollBar = fEngine.VisibleNodeCount > Count && fEngine.LayoutMetrics.VerticalScrollBarWidth > 0;
        ContentWidth = Math.Max(0, Size.Width - (HasVerticalScrollBar ? fEngine.LayoutMetrics.VerticalScrollBarWidth : 0));
        bool NeedsHorizontal = NeedsHorizontalScrollBar(ContentWidth);
        if (NeedsHorizontal != fHasHorizontalScrollBar)
        {
            fHasHorizontalScrollBar = NeedsHorizontal;
            BodyHeight = Math.Max(0, Size.Height - FixedBandHeight - (fHasHorizontalScrollBar ? fEngine.LayoutMetrics.HorizontalScrollBarHeight : 0));
            Count = fEngine.LayoutMetrics.RowHeight <= 0
                ? 0
                : Math.Min(fEngine.VisibleNodeCount, (int)Math.Floor(BodyHeight / fEngine.LayoutMetrics.RowHeight));
        }

        fFirstVisibleNodeIndex = ClampFirstVisibleNodeIndex(fFirstVisibleNodeIndex, Count);
        ClampHorizontalOffset(ContentWidth);
        GroupGridViewport Viewport = Count <= 0
            ? GroupGridViewport.Empty
            : new GroupGridViewport(fFirstVisibleNodeIndex, fFirstVisibleNodeIndex + Count - 1);

        fEngine.SetViewport(Viewport, BodyHeight);
    }
    int ClampFirstVisibleNodeIndex(int Value, int ViewportCount)
    {
        if (fEngine == null || fEngine.VisibleNodeCount == 0 || ViewportCount <= 0)
            return 0;

        int Max = Math.Max(0, fEngine.VisibleNodeCount - ViewportCount);
        if (Value < 0)
            return 0;
        if (Value > Max)
            return Max;

        return Value;
    }
    bool ScrollViewport(int Delta)
    {
        if (fEngine == null || Delta == 0)
            return false;

        return SetFirstVisibleNodeIndexCore(fFirstVisibleNodeIndex + Delta);
    }
    bool ScrollCurrentCellIntoViewCore()
    {
        if (fEngine == null || fEngine.CurrentCell.IsEmpty || fEngine.Viewport.IsEmpty)
            return false;

        bool Result = false;
        int VisibleNodeIndex = fEngine.IndexOfVisibleRow(fEngine.CurrentCell.RowIndex);
        if (VisibleNodeIndex >= 0)
        {
            if (VisibleNodeIndex < fEngine.Viewport.FirstVisibleNodeIndex)
                Result = SetFirstVisibleNodeIndexCore(VisibleNodeIndex);
            else if (VisibleNodeIndex > fEngine.Viewport.LastVisibleNodeIndex)
                Result = SetFirstVisibleNodeIndexCore(VisibleNodeIndex - fEngine.Viewport.Count + 1);
        }

        Result = ScrollCurrentColumnIntoViewCore() || Result;
        return Result;
    }
    double GetColumnLeft(GroupGridColumn Column)
    {
        double Result = 0;
        foreach (GroupGridColumn Item in fEngine.GetVisibleValueColumns())
        {
            if (ReferenceEquals(Item, Column))
                return Result;

            Result += Math.Max(Item.MinWidth, Item.Width);
        }

        return -1;
    }
    bool ScrollCurrentColumnIntoViewCore()
    {
        if (fEngine == null || fEngine.CurrentCell.IsEmpty || !HasHorizontalScrollBar())
            return false;

        double ColumnLeft = GetColumnLeft(fEngine.CurrentCell.Column);
        if (ColumnLeft < 0)
            return false;

        double ColumnWidth = Math.Max(fEngine.CurrentCell.Column.MinWidth, fEngine.CurrentCell.Column.Width);
        double ColumnRight = ColumnLeft + ColumnWidth;
        double ContentWidth = GetBodyContentWidth();
        if (ColumnLeft < fHorizontalOffset)
            return SetHorizontalOffsetCore(ColumnLeft);
        if (ColumnRight > fHorizontalOffset + ContentWidth)
            return SetHorizontalOffsetCore(ColumnRight - ContentWidth);

        return false;
    }
    bool SetFirstVisibleNodeIndexCore(int Value)
    {
        if (fEngine == null)
            return false;

        int NewFirstVisibleNodeIndex = ClampFirstVisibleNodeIndex(Value, fEngine.Viewport.Count);
        if (NewFirstVisibleNodeIndex == fFirstVisibleNodeIndex)
            return false;

        CancelCellEdit();
        fFirstVisibleNodeIndex = NewFirstVisibleNodeIndex;
        UpdateViewport(Bounds.Size);
        InvalidateVisual();
        return true;
    }
    double GetTotalColumnsWidth()
    {
        if (fEngine == null)
            return 0;

        return fEngine.GetVisibleValueColumns().Sum(Column => Math.Max(Column.MinWidth, Column.Width));
    }
    bool NeedsHorizontalScrollBar(double ContentWidth)
    {
        return fEngine != null
               && fEngine.LayoutMetrics.HorizontalScrollBarHeight > 0
               && GetTotalColumnsWidth() > Math.Max(0, ContentWidth) + 0.1;
    }
    bool HasHorizontalScrollBar()
    {
        return fHasHorizontalScrollBar;
    }
    void ClampHorizontalOffset(double ContentWidth)
    {
        double Max = Math.Max(0, GetTotalColumnsWidth() - Math.Max(0, ContentWidth));
        if (fHorizontalOffset < 0)
            fHorizontalOffset = 0;
        if (fHorizontalOffset > Max)
            fHorizontalOffset = Max;
    }
    bool SetHorizontalOffsetCore(double Value)
    {
        if (fEngine == null)
            return false;

        double ContentWidth = GetBodyContentWidth();
        double Max = Math.Max(0, GetTotalColumnsWidth() - ContentWidth);
        double NewValue = Math.Clamp(Value, 0, Max);
        if (Math.Abs(NewValue - fHorizontalOffset) < 0.1)
            return false;

        CancelCellEdit();
        fHorizontalOffset = NewValue;
        InvalidateVisual();
        return true;
    }
    double GetToolBarHeight()
    {
        return fIsToolBarVisible ? fEngine.LayoutMetrics.ToolBarHeight : 0;
    }
    double GetGroupPanelHeight()
    {
        return fIsGroupPanelVisible ? fEngine.LayoutMetrics.GroupPanelHeight : 0;
    }
    double GetColumnHeaderHeight()
    {
        return fIsColumnHeadersVisible ? fEngine.LayoutMetrics.ColumnHeaderHeight : 0;
    }
    double GetFilterPanelHeight()
    {
        return fIsFilterPanelVisible ? fEngine.LayoutMetrics.FilterRowHeight : 0;
    }
    double GetTotalsSummaryHeight()
    {
        return fIsTotalsSummaryVisible ? fEngine.LayoutMetrics.FooterSummaryHeight : 0;
    }
    double GetBodyFixedTopHeight()
    {
        return GetToolBarHeight()
               + GetGroupPanelHeight()
               + GetColumnHeaderHeight()
               + GetFilterPanelHeight();
    }
    bool HasVerticalScrollBar()
    {
        return fEngine != null
               && !fEngine.Viewport.IsEmpty
               && fEngine.VisibleNodeCount > fEngine.Viewport.Count
               && fEngine.LayoutMetrics.VerticalScrollBarWidth > 0;
    }
    double GetBodyTop()
    {
        if (fEngine == null)
            return 0;

        return GetBodyFixedTopHeight();
    }
    double GetGroupPanelTop()
    {
        if (fEngine == null)
            return 0;

        return GetToolBarHeight();
    }
    Rect GetGroupPanelRect()
    {
        if (fEngine == null)
            return default;

        return new Rect(0, GetGroupPanelTop(), GetBodyContentWidth(), GetGroupPanelHeight());
    }
    Rect GetColumnHeaderRect()
    {
        if (fEngine == null)
            return default;

        double Y = GetToolBarHeight() + GetGroupPanelHeight();
        return new Rect(0, Y, GetBodyContentWidth(), GetColumnHeaderHeight());
    }
    double GetBodyHeight()
    {
        if (fEngine == null)
            return 0;

        return Math.Max(0, Bounds.Height - GetBodyTop() - GetTotalsSummaryHeight() - GetHorizontalScrollBarHeight());
    }
    double GetBodyContentWidth()
    {
        if (fEngine == null)
            return Bounds.Width;

        double ScrollBarWidth = HasVerticalScrollBar() ? fEngine.LayoutMetrics.VerticalScrollBarWidth : 0;
        return Math.Max(0, Bounds.Width - ScrollBarWidth);
    }
    double GetHorizontalScrollBarHeight()
    {
        return HasHorizontalScrollBar() ? fEngine.LayoutMetrics.HorizontalScrollBarHeight : 0;
    }

    // ● scrollbar geometry helpers
    Rect GetVerticalScrollTrackRect()
    {
        if (!HasVerticalScrollBar())
            return default;

        double Width = fEngine.LayoutMetrics.VerticalScrollBarWidth;
        return new Rect(Math.Max(0, Bounds.Width - Width), GetBodyTop(), Width, GetBodyHeight());
    }
    Rect GetVerticalScrollThumbRect()
    {
        Rect TrackRect = GetVerticalScrollTrackRect();
        if (TrackRect.Width <= 0 || TrackRect.Height <= 0)
            return default;

        double ThumbHeight = Math.Max(fEngine.LayoutMetrics.VerticalScrollThumbMinHeight, TrackRect.Height * fEngine.Viewport.Count / fEngine.VisibleNodeCount);
        ThumbHeight = Math.Min(TrackRect.Height, ThumbHeight);
        int MaxFirstVisibleNodeIndex = Math.Max(0, fEngine.VisibleNodeCount - fEngine.Viewport.Count);
        double Top = TrackRect.Y;
        if (MaxFirstVisibleNodeIndex > 0 && TrackRect.Height > ThumbHeight)
            Top += (TrackRect.Height - ThumbHeight) * fFirstVisibleNodeIndex / MaxFirstVisibleNodeIndex;

        return new Rect(TrackRect.X + 2, Top + 2, Math.Max(0, TrackRect.Width - 4), Math.Max(0, ThumbHeight - 4));
    }
    bool SetFirstVisibleNodeIndexFromVerticalScroll(double Y)
    {
        Rect TrackRect = GetVerticalScrollTrackRect();
        Rect ThumbRect = GetVerticalScrollThumbRect();
        int MaxFirstVisibleNodeIndex = Math.Max(0, fEngine.VisibleNodeCount - fEngine.Viewport.Count);
        double Range = TrackRect.Height - ThumbRect.Height;
        if (Range <= 0 || MaxFirstVisibleNodeIndex <= 0)
            return false;

        double Ratio = (Y - TrackRect.Y - fVerticalScrollDragOffset) / Range;
        int NewFirstVisibleNodeIndex = (int)Math.Round(Math.Clamp(Ratio, 0, 1) * MaxFirstVisibleNodeIndex);
        return SetFirstVisibleNodeIndexCore(NewFirstVisibleNodeIndex);
    }
    Rect GetHorizontalScrollTrackRect()
    {
        if (!HasHorizontalScrollBar())
            return default;

        return new Rect(0, GetBodyTop() + GetBodyHeight() + GetTotalsSummaryHeight(), GetBodyContentWidth(), fEngine.LayoutMetrics.HorizontalScrollBarHeight);
    }
    Rect GetHorizontalScrollThumbRect()
    {
        Rect TrackRect = GetHorizontalScrollTrackRect();
        if (TrackRect.Width <= 0 || TrackRect.Height <= 0)
            return default;

        double TotalWidth = GetTotalColumnsWidth();
        double ThumbWidth = Math.Max(fEngine.LayoutMetrics.HorizontalScrollThumbMinWidth, TrackRect.Width * TrackRect.Width / TotalWidth);
        ThumbWidth = Math.Min(TrackRect.Width, ThumbWidth);
        double MaxOffset = Math.Max(0, TotalWidth - TrackRect.Width);
        double Left = TrackRect.X;
        if (MaxOffset > 0 && TrackRect.Width > ThumbWidth)
            Left += (TrackRect.Width - ThumbWidth) * fHorizontalOffset / MaxOffset;

        return new Rect(Left + 2, TrackRect.Y + 2, Math.Max(0, ThumbWidth - 4), Math.Max(0, TrackRect.Height - 4));
    }
    bool SetHorizontalOffsetFromScroll(double X)
    {
        Rect TrackRect = GetHorizontalScrollTrackRect();
        Rect ThumbRect = GetHorizontalScrollThumbRect();
        double MaxOffset = Math.Max(0, GetTotalColumnsWidth() - TrackRect.Width);
        double Range = TrackRect.Width - ThumbRect.Width;
        if (Range <= 0 || MaxOffset <= 0)
            return false;

        double Ratio = (X - TrackRect.X - fHorizontalScrollDragOffset) / Range;
        return SetHorizontalOffsetCore(Math.Clamp(Ratio, 0, 1) * MaxOffset);
    }

    // ● column resize and drag helpers
    bool SetColumnWidth(GroupGridColumn Column, double Width)
    {
        if (Column == null)
            return false;

        double NewWidth = Math.Max(Column.MinWidth, Width);
        if (Math.Abs(NewWidth - Column.Width) < 0.1)
            return false;

        CancelCellEdit();
        Column.Width = NewWidth;
        UpdateViewport(Bounds.Size);
        InvalidateVisual();
        return true;
    }
    double MeasureBestFitTextWidth(string Text, FontWeight Weight = FontWeight.Normal)
    {
        if (string.IsNullOrEmpty(Text))
            return 0;

        try
        {
            return CreateText(Text, TextBrush, 100000, Weight).Width;
        }
        catch (InvalidOperationException)
        {
            return Text.Length * (Weight == FontWeight.Normal ? 7 : 8);
        }
    }
    double GetBestFitColumnWidth(GroupGridColumn Column, int MaxRows)
    {
        if (Column == null)
            return 0;

        double Width = Math.Max(Column.MinWidth, MeasureBestFitTextWidth(string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header, FontWeight.SemiBold) + 28);
        string FilterText = fEngine.GetColumnFilter(Column);
        if (!string.IsNullOrEmpty(FilterText))
            Width = Math.Max(Width, MeasureBestFitTextWidth(FilterText) + 16);

        int DataRowsMeasured = 0;
        for (int Index = 0; Index < fEngine.VisibleNodeCount; Index++)
        {
            GroupGridRowInfo RowInfo = fEngine.GetVisibleRowInfo(Index);
            if (!RowInfo.IsDataRow)
                continue;

            Width = Math.Max(Width, MeasureBestFitTextWidth(fEngine.GetDisplayText(Index, Column)) + 16);
            DataRowsMeasured++;
            if (MaxRows > 0 && DataRowsMeasured >= MaxRows)
                break;
        }

        return Math.Ceiling(Width);
    }
    bool GetColumnDropInfo(Point Point, out int ColumnIndex, out double X)
    {
        ColumnIndex = -1;
        X = 0;

        IReadOnlyList<GroupGridColumn> Columns = fEngine.GetVisibleValueColumns();
        if (Columns.Count == 0)
            return false;

        double LogicalX = Math.Clamp(Point.X, 0, GetBodyContentWidth()) + fHorizontalOffset;
        double Left = 0;
        GroupGridColumn LastColumn = null;
        double LastRight = 0;

        foreach (GroupGridColumn Column in Columns)
        {
            double Width = Math.Max(Column.MinWidth, Column.Width);
            double Right = Left + Width;
            if (LogicalX < Left + (Width / 2))
            {
                ColumnIndex = fEngine.Columns.IndexOf(Column);
                X = Math.Clamp(Left - fHorizontalOffset, 0, GetBodyContentWidth());
                return ColumnIndex >= 0;
            }

            LastColumn = Column;
            LastRight = Right;
            Left = Right;
        }

        ColumnIndex = fEngine.Columns.IndexOf(LastColumn) + 1;
        X = Math.Clamp(LastRight - fHorizontalOffset, 0, GetBodyContentWidth());
        return ColumnIndex > 0;
    }
    bool GetGroupDropInfo(Point Point, out int GroupIndex, out double X)
    {
        GroupIndex = -1;
        X = 0;
        Rect PanelRect = GetGroupPanelRect();
        if (!PanelRect.Contains(Point) || fColumnDragColumn == null || !fColumnDragColumn.CanUserGroup)
            return false;

        X = 6;
        for (int Index = 0; Index < fEngine.GroupColumns.Count; Index++)
        {
            GroupGridColumn Column = fEngine.GroupColumns[Index];
            string Text = string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header;
            double ItemWidth = Math.Max(80, Text.Length * 8 + 24);
            if (Point.X < X + (ItemWidth / 2))
            {
                GroupIndex = Index;
                return true;
            }

            X += ItemWidth + 6;
        }

        GroupIndex = fEngine.GroupColumns.Count;
        return true;
    }
    bool GetGroupPanelColumnAt(Point Point, out GroupGridColumn Column, out int GroupIndex)
    {
        Column = null;
        GroupIndex = -1;
        Rect PanelRect = GetGroupPanelRect();
        if (!PanelRect.Contains(Point))
            return false;

        double X = 6;
        for (int Index = 0; Index < fEngine.GroupColumns.Count; Index++)
        {
            GroupGridColumn Item = fEngine.GroupColumns[Index];
            string Text = string.IsNullOrWhiteSpace(Item.Header) ? Item.Name : Item.Header;
            double ItemWidth = Math.Max(80, Text.Length * 8 + 24);
            Rect Rect = new(X, PanelRect.Y + 5, ItemWidth, Math.Max(0, PanelRect.Height - 10));
            if (Rect.Contains(Point))
            {
                Column = Item;
                GroupIndex = Index;
                return true;
            }

            X += ItemWidth + 6;
        }

        return false;
    }
    bool IsInsideGrid(Point Point)
    {
        return Point.X >= 0 && Point.Y >= 0 && Point.X < Bounds.Width && Point.Y < Bounds.Height;
    }
    bool IsColumnDragRejectTarget(Point Point)
    {
        return fIsColumnDragging
               && fIsColumnDragActive
               && fColumnDragColumn != null
               && !IsInsideGrid(Point)
               && fColumnDragDropIndex < 0
               && fColumnDragGroupDropIndex < 0;
    }
    int IndexOfGroupColumn(GroupGridColumn Column)
    {
        for (int Index = 0; Index < fEngine.GroupColumns.Count; Index++)
            if (ReferenceEquals(fEngine.GroupColumns[Index], Column))
                return Index;

        return -1;
    }
    bool MoveDraggedColumn()
    {
        if (fColumnDragColumn == null || fColumnDragDropIndex < 0)
            return false;

        int OldIndex = fEngine.Columns.IndexOf(fColumnDragColumn);
        if (OldIndex < 0)
            return false;

        int NewIndex = fColumnDragDropIndex;
        if (NewIndex > OldIndex)
            NewIndex--;
        NewIndex = Math.Clamp(NewIndex, 0, fEngine.Columns.Count - 1);

        return fEngine.MoveColumn(fColumnDragColumn, NewIndex);
    }
    bool MoveGroupedColumn()
    {
        if (fColumnDragColumn == null || fColumnDragGroupDropIndex < 0)
            return false;

        int OldIndex = IndexOfGroupColumn(fColumnDragColumn);
        if (OldIndex < 0)
            return false;

        int NewIndex = fColumnDragGroupDropIndex;
        if (NewIndex > OldIndex)
            NewIndex--;
        NewIndex = Math.Clamp(NewIndex, 0, fEngine.GroupColumns.Count - 1);
        return fEngine.MoveGroupedColumn(fColumnDragColumn, NewIndex);
    }
    bool UngroupDraggedColumnToHeader()
    {
        if (fColumnDragColumn == null || fColumnDragDropIndex < 0)
            return false;

        int NewIndex = Math.Clamp(fColumnDragDropIndex, 0, fEngine.Columns.Count - 1);
        bool Ungrouped = fEngine.UngroupColumn(fColumnDragColumn);
        bool Moved = fEngine.MoveColumn(fColumnDragColumn, NewIndex);
        return Ungrouped || Moved;
    }

    // ● data source helpers
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
    IGroupGridDataAdapter CreateDataAdapter(object ItemsSource)
    {
        if (ItemsSource == null)
            return null;

        DataView View = FindDataView(ItemsSource);
        if (View != null)
            return new GroupGridDataViewDataAdapter(View);

        Type ItemType = FindListItemType(ItemsSource);
        if (ItemType == null)
            throw new ArgumentException("ItemsSource must be a DataTable, DataView, or implement IList<T>.", nameof(ItemsSource));

        Type AdapterType = typeof(GroupGridListDataAdapter<>).MakeGenericType(ItemType);
        return (IGroupGridDataAdapter)Activator.CreateInstance(AdapterType, ItemsSource);
    }
    GroupGridColumn CreateAutoColumn(PropertyInfo Property)
    {
        Type Type = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
        GroupGridColumn Column;

        if (Type == typeof(bool))
            Column = new GroupGridCheckBoxColumn();
        else if (Type == typeof(DateTime) || Type == typeof(DateTimeOffset))
            Column = new GroupGridDateColumn();
        else if (Type == typeof(byte)
                 || Type == typeof(short)
                 || Type == typeof(int)
                 || Type == typeof(long)
                 || Type == typeof(float)
                 || Type == typeof(double)
                 || Type == typeof(decimal))
            Column = new GroupGridNumberColumn();
        else
            Column = new GroupGridTextColumn();

        Column.Name = Property.Name;
        Column.Header = Property.Name;
        Column.ValueType = Type;
        Column.IsReadOnly = !Property.CanWrite;
        return Column;
    }
    GroupGridColumn CreateAutoColumn(DataColumn DataColumn)
    {
        Type Type = Nullable.GetUnderlyingType(DataColumn.DataType) ?? DataColumn.DataType;
        GroupGridColumn Column;

        if (Type == typeof(bool))
            Column = new GroupGridCheckBoxColumn();
        else if (Type == typeof(DateTime) || Type == typeof(DateTimeOffset))
            Column = new GroupGridDateColumn();
        else if (Type == typeof(byte)
                 || Type == typeof(short)
                 || Type == typeof(int)
                 || Type == typeof(long)
                 || Type == typeof(float)
                 || Type == typeof(double)
                 || Type == typeof(decimal))
            Column = new GroupGridNumberColumn();
        else
            Column = new GroupGridTextColumn();

        Column.Name = DataColumn.ColumnName;
        Column.Header = string.IsNullOrWhiteSpace(DataColumn.Caption) ? DataColumn.ColumnName : DataColumn.Caption;
        Column.ValueType = Type;
        Column.IsReadOnly = DataColumn.ReadOnly;
        return Column;
    }
    void GenerateColumnsFromItemType(Type ItemType)
    {
        if (!fAutoGenerateColumns || ItemType == null || Columns.Count > 0)
            return;

        foreach (PropertyInfo Property in ItemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (Property.GetIndexParameters().Length == 0)
                Columns.Add(CreateAutoColumn(Property));
    }
    void GenerateColumnsFromDataView(DataView View)
    {
        if (!fAutoGenerateColumns || View == null || View.Table == null || Columns.Count > 0)
            return;

        foreach (DataColumn DataColumn in View.Table.Columns)
            Columns.Add(CreateAutoColumn(DataColumn));
    }
    void GenerateColumnsFromItemsSource(object ItemsSource)
    {
        DataView View = FindDataView(ItemsSource);
        if (View != null)
        {
            GenerateColumnsFromDataView(View);
            return;
        }

        GenerateColumnsFromItemType(FindListItemType(ItemsSource));
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
        GenerateColumnsFromItemsSource(Value);

        IGroupGridDataAdapter Adapter = CreateDataAdapter(Value);
        fOwnedDataAdapter = Adapter as IDisposable;
        DataAdapter = Adapter;
    }

    // ● settings file helpers
    void ValidateSettingsFilePath(string FilePath)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ArgumentException("A settings file path is required.", nameof(FilePath));
        if (!Path.IsPathFullyQualified(FilePath))
            throw new ArgumentException("The settings file path must be a full file path.", nameof(FilePath));
    }

    // ● cell state helpers
    bool IsCurrentCell(GroupGridRowInfo RowInfo, GroupGridColumn Column)
    {
        return RowInfo.IsDataRow && fEngine.CurrentCell == new GroupGridCell(RowInfo.RowIndex, Column);
    }
    bool IsSelectedRow(GroupGridRowInfo RowInfo)
    {
        return RowInfo.IsDataRow && fEngine.IsSelectedRow(RowInfo.RowIndex);
    }
    bool IsEditingCell(GroupGridRowInfo RowInfo, GroupGridColumn Column)
    {
        return RowInfo.IsDataRow && fEngine.EditingCell == new GroupGridCell(RowInfo.RowIndex, Column);
    }
    bool IsCheckBoxToggleColumn(GroupGridColumn Column)
    {
        return Column is GroupGridCheckBoxColumn || Column?.ValueType == typeof(bool);
    }
    bool ToggleCheckBoxCell(GroupGridHitTestResult Hit)
    {
        if (Hit == null
            || Hit.RowKind != GroupGridRowKind.DataRow
            || !Hit.HasCell)
            return false;

        return ToggleCheckBoxCell(Hit.Cell);
    }
    bool ToggleCheckBoxCell(GroupGridCell Cell)
    {
        if (Cell.IsEmpty
            || !IsCheckBoxToggleColumn(Cell.Column)
            || fEngine.IsReadOnly
            || Cell.Column.IsReadOnly
            || !fEngine.CanSetValue(Cell.RowIndex, Cell.Column))
            return false;

        object Value = fEngine.GetValue(Cell.RowIndex, Cell.Column);
        bool IsChecked = Value != null && Value != DBNull.Value && Convert.ToBoolean(Value, CultureInfo.CurrentCulture);
        fEngine.SetValue(Cell.RowIndex, Cell.Column, !IsChecked);
        return true;
    }
    object GetAdapterRow(int RowIndex)
    {
        IGroupGridDataAdapter Adapter = DataAdapter;
        if (Adapter == null || RowIndex < 0 || RowIndex >= Adapter.RowCount)
            return null;

        return Adapter.GetRow(RowIndex);
    }
    string GetCellEditText(GroupGridCell Cell)
    {
        return Cell.IsEmpty
            ? string.Empty
            : Convert.ToString(fEngine.GetValue(Cell.RowIndex, Cell.Column), CultureInfo.CurrentCulture) ?? string.Empty;
    }
    Rect GetCellRect(GroupGridCell Cell)
    {
        if (Cell.IsEmpty || fEngine == null || fEngine.Viewport.IsEmpty)
            return default;

        int VisibleNodeIndex = fEngine.IndexOfVisibleRow(Cell.RowIndex);
        if (VisibleNodeIndex < fEngine.Viewport.FirstVisibleNodeIndex || VisibleNodeIndex > fEngine.Viewport.LastVisibleNodeIndex)
            return default;

        double ColumnLeft = GetColumnLeft(Cell.Column);
        if (ColumnLeft < 0)
            return default;

        double RowHeight = fEngine.LayoutMetrics.RowHeight;
        double X = ColumnLeft - fHorizontalOffset;
        double Y = GetBodyTop() + ((VisibleNodeIndex - fEngine.Viewport.FirstVisibleNodeIndex) * RowHeight);
        double Width = Math.Max(Cell.Column.MinWidth, Cell.Column.Width);
        double ContentWidth = GetBodyContentWidth();
        Rect ClipRect = new(0, GetBodyTop(), ContentWidth, GetBodyHeight());
        Rect CellRect = new(X, Y, Width, RowHeight);
        return CellRect.Intersect(ClipRect);
    }

    // ● editor geometry helpers
    Rect GetEditorRect()
    {
        Rect CellRect = GetCellRect(fEngine.EditingCell);
        return CellRect.Width <= 0 || CellRect.Height <= 0 ? default : InsetRect(CellRect, 2);
    }
    bool UsesPopupDropDown()
    {
        return fDropDownPlacementMode != GroupGridDropDownPlacementMode.Inline;
    }
    double GetDesiredDropDownHeight()
    {
        if (fActiveEditor != null && fActiveEditor.DropDownHeight > 0)
            return fActiveEditor.DropDownHeight;

        double ItemHeight = 26;
        int Count = fActiveDropDownItemCount;
        return Math.Max(ItemHeight, Math.Min(220, Count * ItemHeight + 2));
    }
    double GetDropDownHeight(Rect EditorRect)
    {
        double DesiredHeight = GetDesiredDropDownHeight();
        if (UsesPopupDropDown())
            return DesiredHeight;

        double ItemHeight = 26;
        double AvailableBelow = Math.Max(0, Bounds.Height - EditorRect.Bottom);
        return Math.Max(ItemHeight, Math.Min(DesiredHeight, AvailableBelow));
    }
    double GetDropDownWidth(Rect EditorRect)
    {
        double Width = fActiveEditor != null && fActiveEditor.DropDownWidth > 0
            ? fActiveEditor.DropDownWidth
            : EditorRect.Width;
        return Math.Max(EditorRect.Width, Width);
    }
    int GetDropDownItemCount(IEnumerable Items)
    {
        if (Items == null)
            return 0;
        if (Items is ICollection Collection)
            return Collection.Count;

        int Result = 0;
        foreach (object Item in Items)
            Result++;
        return Result;
    }
    void SetDropDownListBoxHeight(double Height)
    {
        if (fActiveDropDownListBox == null)
            return;

        double ListBoxHeight = Math.Max(24, Height - 2);
        fActiveDropDownListBox.Height = ListBoxHeight;
        fActiveDropDownListBox.MaxHeight = ListBoxHeight;
    }
    Rect GetDropDownRect(Rect EditorRect)
    {
        if (EditorRect.Width <= 0 || EditorRect.Height <= 0)
            return default;

        double Height = GetDropDownHeight(EditorRect);
        double Width = GetDropDownWidth(EditorRect);
        return new Rect(EditorRect.X, EditorRect.Bottom, Width, Height);
    }
    void ConfigureActiveDropDownPopup(Rect EditorRect)
    {
        if (fActiveDropDownPopup == null || EditorRect.Width <= 0 || EditorRect.Height <= 0)
            return;

        fActiveDropDownPopup.PlacementTarget = this;
        fActiveDropDownPopup.Placement = PlacementMode.AnchorAndGravity;
        fActiveDropDownPopup.PlacementRect = EditorRect;
        fActiveDropDownPopup.PlacementAnchor = PopupAnchor.BottomLeft;
        fActiveDropDownPopup.PlacementGravity = PopupGravity.BottomRight;
        fActiveDropDownPopup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.FlipY | PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.SlideY;
    }
    void ActiveDropDownPopup_Closed(object Sender, EventArgs Args)
    {
        if (fActiveDropDownPopup == null)
            return;

        CloseActiveDropDown();
        fActiveEditor?.FocusEditor();
    }
    void IGroupGridDropDownEditorHost.CloseDropDown()
    {
        CloseActiveDropDown();
    }
    bool IGroupGridDropDownEditorHost.CommitDropDownValue(object Value)
    {
        if (fActiveEditor == null)
            return false;

        fActiveEditor.SelectDropDownItem(Value);
        CloseActiveDropDown();
        return CommitCellEdit();
    }
    bool IGroupGridDropDownEditorHost.CancelDropDown()
    {
        if (fActiveEditor == null)
            return false;

        CloseActiveDropDown();
        fActiveEditor.FocusEditor();
        return true;
    }
    void IGroupGridDropDownEditorHost.RestoreEditorFocus()
    {
        fActiveEditor?.FocusEditor();
    }
    bool CanEditTextCell(GroupGridCell Cell)
    {
        return !Cell.IsEmpty
               && !IsCheckBoxToggleColumn(Cell.Column)
               && fEngine.CanSetValue(Cell.RowIndex, Cell.Column);
    }

    // ● cell editing helpers
    bool BeginCellEdit(bool ReplaceText, string Text)
    {
        GroupGridCell Cell = fEngine.CurrentCell;
        if (!CanEditTextCell(Cell))
            return false;

        if (!fEngine.BeginEdit(Cell))
            return false;

        fCellEditText = ReplaceText ? Text ?? string.Empty : GetCellEditText(Cell);
        ShowCellEditor(Cell);
        InvalidateVisual();
        return true;
    }
    bool BeginCellEdit(GroupGridCell Cell, bool ReplaceText, string Text)
    {
        if (Cell.IsEmpty)
            return false;

        if (fEngine.SetCurrentCell(Cell))
            ScrollCurrentCellIntoViewCore();
        return BeginCellEdit(ReplaceText, Text);
    }
    void ShowCellEditor(GroupGridCell Cell)
    {
        CloseActiveEditor();
        GroupGridInplaceEditorBase Editor = CreateCellEditor(Cell.Column);
        Editor.Background = EditingBrush;
        if (Editor is GroupGridDropDownInplaceEditorBase DropDownEditor)
            DropDownEditor.DropDownHost = this;
        if (Editor is GroupGridDateInplaceEditor DateEditor)
        {
            DateEditor.Foreground = TextBrush;
            DateEditor.Value = fEngine.GetValue(Cell.RowIndex, Cell.Column);
        }
        else if (Editor is GroupGridTextInplaceEditor TextEditor)
        {
            TextEditor.Foreground = TextBrush;
            TextEditor.Value = fCellEditText;
        }
        else if (Editor is GroupGridLookupInplaceEditor)
        {
            Editor.Value = fEngine.GetValue(Cell.RowIndex, Cell.Column);
        }
        else
        {
            Editor.Value = fCellEditText;
        }
        Editor.ValueChanged += Editor_ValueChanged;
        Editor.KeyDown += Editor_KeyDown;
        Editor.LostFocus += Editor_LostFocus;
        if (Editor.EditorControl != null)
        {
            Editor.EditorControl.KeyDown += Editor_KeyDown;
            Editor.EditorControl.LostFocus += Editor_LostFocus;
        }
        Editor.DropDownRequested += Editor_DropDownRequested;
        fActiveEditor = Editor;
        VisualChildren.Add(Editor);
        LogicalChildren.Add(Editor);
        InvalidateMeasure();
        InvalidateArrange();
        Editor.FocusEditor();
        Editor.SelectAll();
    }

    // ● editor drop-down helpers
    void Editor_DropDownRequested(object Sender, EventArgs Args)
    {
        if (fActiveDropDownHost != null)
            CloseActiveDropDown();
        else
            ShowActiveDropDown();
    }
    void ShowActiveDropDown()
    {
        if (fActiveEditor == null || !fActiveEditor.HasDropDown)
            return;

        Control DropDownControl = fActiveEditor.CreateDropDownControl();
        if (DropDownControl == null)
        {
            if (fActiveEditor.DropDownItems == null)
                return;

            fActiveDropDownItemCount = GetDropDownItemCount(fActiveEditor.DropDownItems);
            ListBox ListBox = new()
            {
                ItemsSource = fActiveEditor.DropDownItems,
                SelectedItem = fActiveEditor.SelectedDropDownItem,
                MinHeight = 24,
                Focusable = true,
            };
            ScrollViewer.SetHorizontalScrollBarVisibility(ListBox, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(ListBox, ScrollBarVisibility.Visible);
            ListBox.AddHandler(InputElement.KeyDownEvent, ActiveDropDownListBox_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
            ListBox.SelectionChanged += ActiveDropDownListBox_SelectionChanged;
            ListBox.PointerReleased += ActiveDropDownListBox_PointerReleased;
            fActiveDropDownListBox = ListBox;
            DropDownControl = ListBox;
        }
        else if (DropDownControl is Calendar Calendar)
        {
            Calendar.AddHandler(InputElement.KeyDownEvent, ActiveDropDownCalendar_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
            Calendar.AddHandler(InputElement.DoubleTappedEvent, ActiveDropDownCalendar_DoubleTapped, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        }

        Border Host = new()
        {
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(180, 185, 192)),
            BorderThickness = new Thickness(1),
            Child = DropDownControl,
        };
        fActiveDropDownControl = DropDownControl;
        fActiveDropDownHost = Host;
        if (UsesPopupDropDown())
        {
            Rect EditorRect = GetEditorRect();
            Popup Popup = new()
            {
                Child = Host,
                IsLightDismissEnabled = true,
                OverlayDismissEventPassThrough = true,
                ShouldUseOverlayLayer = true,
            };
            Popup.Closed += ActiveDropDownPopup_Closed;
            fActiveDropDownPopup = Popup;
            LogicalChildren.Add(Popup);
            ConfigureActiveDropDownPopup(EditorRect);
            Popup.IsOpen = true;
        }
        else
        {
            VisualChildren.Add(Host);
            LogicalChildren.Add(Host);
        }
        InvalidateMeasure();
        InvalidateArrange();
        Dispatcher.UIThread.Post(() =>
        {
            fActiveDropDownListBox?.ScrollIntoView(fActiveDropDownListBox.SelectedItem);
            if (fActiveDropDownListBox != null)
                fActiveDropDownListBox.Focus(NavigationMethod.Pointer, KeyModifiers.None);
            else
                fActiveEditor?.DropDownOpened(fActiveDropDownControl);
        }, DispatcherPriority.Background);
    }
    void CloseActiveDropDown()
    {
        if (fActiveDropDownHost == null)
            return;

        if (fActiveDropDownListBox != null)
        {
            fActiveDropDownListBox.RemoveHandler(InputElement.KeyDownEvent, ActiveDropDownListBox_KeyDown);
            fActiveDropDownListBox.SelectionChanged -= ActiveDropDownListBox_SelectionChanged;
            fActiveDropDownListBox.PointerReleased -= ActiveDropDownListBox_PointerReleased;
        }
        if (fActiveDropDownControl is Calendar Calendar)
        {
            Calendar.RemoveHandler(InputElement.KeyDownEvent, ActiveDropDownCalendar_KeyDown);
            Calendar.RemoveHandler(InputElement.DoubleTappedEvent, ActiveDropDownCalendar_DoubleTapped);
        }
        fActiveEditor?.DropDownClosed();
        if (fActiveDropDownPopup != null)
        {
            fActiveDropDownPopup.Closed -= ActiveDropDownPopup_Closed;
            fActiveDropDownPopup.IsOpen = false;
            fActiveDropDownPopup.Child = null;
            LogicalChildren.Remove(fActiveDropDownPopup);
            fActiveDropDownPopup = null;
        }
        else
        {
            LogicalChildren.Remove(fActiveDropDownHost);
            VisualChildren.Remove(fActiveDropDownHost);
        }
        fActiveDropDownControl = null;
        fActiveDropDownListBox = null;
        fActiveDropDownHost = null;
        fActiveDropDownItemCount = 0;
        InvalidateMeasure();
        InvalidateArrange();
    }
    void ActiveDropDownListBox_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (fActiveDropDownListBox?.SelectedItem != null)
            fActiveDropDownListBox.ScrollIntoView(fActiveDropDownListBox.SelectedItem);
    }
    bool CommitSelectedDropDownItem()
    {
        if (fActiveEditor == null || fActiveDropDownListBox?.SelectedItem == null)
            return false;

        return ((IGroupGridDropDownEditorHost)this).CommitDropDownValue(fActiveDropDownListBox.SelectedItem);
    }
    void ActiveDropDownListBox_KeyDown(object Sender, KeyEventArgs Args)
    {
        switch (Args.Key)
        {
            case Key.Enter:
                Args.Handled = CommitSelectedDropDownItem();
                break;
            case Key.Tab:
                if (!CommitSelectedDropDownItem())
                    return;
                fEngine.MoveCurrentToNextEditableCell(!Args.KeyModifiers.HasFlag(KeyModifiers.Shift));
                ScrollCurrentCellIntoViewCore();
                Args.Handled = true;
                break;
            case Key.Escape:
                Args.Handled = ((IGroupGridDropDownEditorHost)this).CancelDropDown();
                break;
        }
    }
    void ActiveDropDownListBox_PointerReleased(object Sender, PointerReleasedEventArgs Args)
    {
        if (!CommitSelectedDropDownItem())
            fActiveEditor?.FocusEditor();
        Args.Handled = true;
    }
    bool CommitSelectedCalendarDate()
    {
        if (fActiveEditor == null || fActiveDropDownControl is not Calendar Calendar || Calendar.SelectedDate == null)
            return false;

        return ((IGroupGridDropDownEditorHost)this).CommitDropDownValue(Calendar.SelectedDate.Value);
    }
    void ActiveDropDownCalendar_KeyDown(object Sender, KeyEventArgs Args)
    {
        switch (Args.Key)
        {
            case Key.Enter:
                Args.Handled = CommitSelectedCalendarDate();
                break;
            case Key.Tab:
                if (!CommitSelectedCalendarDate())
                    return;
                fEngine.MoveCurrentToNextEditableCell(!Args.KeyModifiers.HasFlag(KeyModifiers.Shift));
                ScrollCurrentCellIntoViewCore();
                Args.Handled = true;
                break;
            case Key.Escape:
                Args.Handled = ((IGroupGridDropDownEditorHost)this).CancelDropDown();
                break;
        }
    }
    void ActiveDropDownCalendar_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!CommitSelectedCalendarDate())
                fActiveEditor?.FocusEditor();
        }, DispatcherPriority.Background);
        Args.Handled = true;
    }

    // ● editor factory helpers
    GroupGridInplaceEditorBase CreateCellEditor(GroupGridColumn Column)
    {
        GroupGridCreateInplaceEditorEventArgs Args = new(Column);
        CreateInplaceEditor?.Invoke(this, Args);
        if (Args.Handled && Args.Editor != null)
            return Args.Editor;

        if (Column is GroupGridLookupColumn LookupColumn)
            return new GroupGridLookupInplaceEditor(LookupColumn);

        Type ValueType = Nullable.GetUnderlyingType(Column.ValueType) ?? Column.ValueType;
        if (Column is GroupGridNumberColumn
            || ValueType == typeof(byte)
            || ValueType == typeof(sbyte)
            || ValueType == typeof(short)
            || ValueType == typeof(ushort)
            || ValueType == typeof(int)
            || ValueType == typeof(uint)
            || ValueType == typeof(long)
            || ValueType == typeof(ulong)
            || ValueType == typeof(float)
            || ValueType == typeof(double)
            || ValueType == typeof(decimal))
            return new GroupGridNumberInplaceEditor();

        if (Column is GroupGridDateColumn || ValueType == typeof(DateTime) || ValueType == typeof(DateTimeOffset))
            return new GroupGridDateInplaceEditor(Column as GroupGridDateColumn);

        return new GroupGridTextInplaceEditor();
    }
    void CloseActiveEditor()
    {
        CloseActiveEditor(true);
    }
    void CloseActiveEditor(bool RestoreFocus)
    {
        if (fActiveEditor == null)
            return;

        fIsClosingEditor = true;
        fActiveEditor.LostFocus -= Editor_LostFocus;
        fActiveEditor.KeyDown -= Editor_KeyDown;
        if (fActiveEditor.EditorControl != null)
        {
            fActiveEditor.EditorControl.LostFocus -= Editor_LostFocus;
            fActiveEditor.EditorControl.KeyDown -= Editor_KeyDown;
        }
        fActiveEditor.ValueChanged -= Editor_ValueChanged;
        fActiveEditor.DropDownRequested -= Editor_DropDownRequested;
        CloseActiveDropDown();
        fActiveEditor.Cleanup();
        if (fActiveEditor is GroupGridDropDownInplaceEditorBase DropDownEditor)
            DropDownEditor.DropDownHost = null;
        LogicalChildren.Remove(fActiveEditor);
        VisualChildren.Remove(fActiveEditor);
        fActiveEditor = null;
        fIsClosingEditor = false;
        InvalidateMeasure();
        InvalidateArrange();
        if (RestoreFocus)
            Focus(NavigationMethod.Pointer, KeyModifiers.None);
    }
    void Editor_ValueChanged(object Sender, EventArgs Args)
    {
        if (fActiveEditor != null)
            fCellEditText = Convert.ToString(fActiveEditor.Value, CultureInfo.CurrentCulture) ?? string.Empty;
    }
    void Editor_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Handled)
            return;

        switch (Args.Key)
        {
            case Key.Enter:
                Args.Handled = CommitCellEdit();
                break;
            case Key.Escape:
                Args.Handled = CancelCellEdit();
                break;
            case Key.Tab:
                if (!CommitCellEdit())
                    return;
                fEngine.MoveCurrentToNextEditableCell(!Args.KeyModifiers.HasFlag(KeyModifiers.Shift));
                ScrollCurrentCellIntoViewCore();
                Args.Handled = true;
                break;
        }
    }
    void Editor_LostFocus(object Sender, RoutedEventArgs Args)
    {
        if (fIsClosingEditor)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (fIsClosingEditor || fActiveEditor == null || fActiveDropDownHost != null)
                return;
            if (fActiveEditor.IsKeyboardFocusWithin)
                return;

            if (fActiveEditor is GroupGridNumberInplaceEditor)
                CommitCellEdit();
            else
                CancelCellEdit();
        }, DispatcherPriority.Background);
    }
    object NormalizeEditorValue(GroupGridCell Cell, object Value)
    {
        if (Cell.IsEmpty || Value == null || Value == DBNull.Value)
            return Value;

        Type ValueType = Nullable.GetUnderlyingType(Cell.Column.ValueType) ?? Cell.Column.ValueType;
        if ((Cell.Column is GroupGridDateColumn || ValueType == typeof(DateTime)) && Value is string Text)
        {
            GroupGridDateNormalizeEventArgs Args = new(Cell.Column, Text, CultureInfo.CurrentCulture);
            DateNormalize?.Invoke(this, Args);
            if (Args.Handled)
                return Args.Value;
        }

        return Value;
    }

    // ● edit commit helpers
    bool CommitCellEdit()
    {
        if (!fEngine.IsEditing)
            return false;

        try
        {
            object EditorValue = fCellEditText;
            if (fActiveEditor != null)
            {
                EditorValue = fActiveEditor.Value;
                if (fActiveEditor is GroupGridTextInplaceEditor)
                    fCellEditText = Convert.ToString(EditorValue, CultureInfo.CurrentCulture) ?? string.Empty;
            }
            object Value = NormalizeEditorValue(fEngine.EditingCell, EditorValue);
            bool Result = fEngine.CommitEdit(Value);
            if (Result)
            {
                fCellEditText = string.Empty;
                CloseActiveEditor();
            }
            return Result;
        }
        catch
        {
            if (fActiveEditor is GroupGridNumberInplaceEditor)
                CancelCellEdit();

            // Invalid editor text remains pending so the user can correct it.
            return true;
        }
    }
    bool CancelCellEdit()
    {
        if (!fEngine.CancelEdit())
            return false;

        fCellEditText = string.Empty;
        CloseActiveEditor();
        InvalidateVisual();
        return true;
    }
    bool AppendCellEditText(string Text)
    {
        if (!fEngine.IsEditing || fActiveEditor != null || !IsPrintableText(Text))
            return false;

        fCellEditText += Text;
        InvalidateVisual();
        return true;
    }
    bool BackspaceCellEditText()
    {
        if (!fEngine.IsEditing || fActiveEditor != null || string.IsNullOrEmpty(fCellEditText))
            return false;

        fCellEditText = fCellEditText.Substring(0, fCellEditText.Length - 1);
        InvalidateVisual();
        return true;
    }
    bool ClearCellEditText()
    {
        if (!fEngine.IsEditing || fActiveEditor != null)
            return false;

        fCellEditText = string.Empty;
        InvalidateVisual();
        return true;
    }

    // ● menu helpers
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
    MenuItem CreateExportMenuItem()
    {
        List<GroupGridExporter> Exporters = GroupGridExporters.CreateExporters().ToList();
        MenuItem Result = new()
        {
            Header = "Export",
            IsEnabled = Exporters.Count > 0,
        };
        Result.ItemsSource = Exporters
            .Select(Exporter => CreateMenuItem(Exporter.Name, true, () => ExportAsync(Exporter)))
            .ToList();
        return Result;
    }
    string GetSortMenuHeader(GroupGridColumn Column)
    {
        if (!ReferenceEquals(fEngine.SortColumn, Column) || fEngine.SortDirection == GroupGridSortDirection.None)
            return "Sort Ascending";
        if (fEngine.SortDirection == GroupGridSortDirection.Ascending)
            return "Sort Descending";

        return "Clear Sorting";
    }
    bool IsPrintableText(string Text)
    {
        return !string.IsNullOrEmpty(Text) && !Text.Any(Character => char.IsControl(Character));
    }

    // ● filter edit helpers
    bool SetFilterEditColumn(GroupGridColumn Column)
    {
        if (ReferenceEquals(fFilterEditColumn, Column))
            return false;

        fFilterEditColumn = Column;
        fFilterEditText = Column == null ? string.Empty : fEngine.GetColumnFilter(Column);
        InvalidateVisual();
        return true;
    }
    bool AppendFilterText(string Text)
    {
        if (fFilterEditColumn == null || !IsPrintableText(Text))
            return false;

        fFilterEditText += Text;
        InvalidateVisual();
        return true;
    }
    bool BackspaceFilterText()
    {
        if (fFilterEditColumn == null)
            return false;

        if (string.IsNullOrEmpty(fFilterEditText))
            return false;

        fFilterEditText = fFilterEditText.Substring(0, fFilterEditText.Length - 1);
        InvalidateVisual();
        return true;
    }
    bool ApplyFilterEdit()
    {
        if (fFilterEditColumn == null)
            return false;

        GroupGridColumn Column = fFilterEditColumn;
        string Text = fFilterEditText;
        SetFilterEditColumn(null);
        CancelCellEdit();
        return fEngine.SetColumnFilter(Column, Text);
    }
    bool CancelFilterEdit()
    {
        return SetFilterEditColumn(null);
    }
    bool ClearFilterEdit()
    {
        if (fFilterEditColumn == null)
            return false;

        GroupGridColumn Column = fFilterEditColumn;
        SetFilterEditColumn(null);
        CancelCellEdit();
        return fEngine.ClearColumnFilter(Column);
    }

    // ● context menu helpers
    bool ShowColumnContextMenu(Point Point)
    {
        GroupGridHitTestResult Hit = HitTestCore(Point);
        if (Hit == null || Hit.Column == null || Hit.Kind != GroupGridHitTestKind.ColumnHeader)
            return false;

        GroupGridColumn Column = Hit.Column;
        bool IsGrouped = fEngine.IsGroupedColumn(Column);
        ContextMenu Menu = new()
        {
            Placement = PlacementMode.Pointer,
        };
        List<object> Items = new()
        {
            CreateMenuItem(GetSortMenuHeader(Column), true, () => ToggleSort(Column)),
            new Separator(),
            CreateMenuItem("Group by This Column", !IsGrouped && Column.CanUserGroup, () => GroupColumn(Column)),
            CreateMenuItem("Ungroup Column", IsGrouped, () => UngroupColumn(Column)),
            new Separator(),
            CreateMenuItem("Hide Column", Column.IsVisible && Column.CanUserHide, () => SetColumnVisible(Column, false)),
            new Separator(),
            CreateMenuItem("Clear Column Filter", !string.IsNullOrEmpty(fEngine.GetColumnFilter(Column)), () => ClearColumnFilter(Column)),
            CreateMenuItem("Clear All Filters", fEngine.HasFilters, () => ClearFilters()),
        };

        if (fIsColumnManagerMenuItemVisible)
        {
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Column Manager...", true, RequestColumnManager));
        }

        if (fIsSettingsMenuItemsVisible)
        {
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Save Settings...", true, SaveSettingsAsync));
            Items.Add(CreateMenuItem("Load Settings...", true, LoadSettingsAsync));
        }

        Items.Add(new Separator());
        Items.Add(CreateExportMenuItem());

        Menu.ItemsSource = Items;
        Menu.Open(this);
        return true;
    }
    bool ShowSummaryContextMenu(Point Point)
    {
        GroupGridHitTestResult Hit = HitTestCore(Point);
        if (Hit == null || Hit.Column == null)
            return false;

        bool IsGroupSummary = Hit.Kind == GroupGridHitTestKind.BodyCell && Hit.RowKind == GroupGridRowKind.GroupSummary;
        bool IsTotalSummary = Hit.Kind == GroupGridHitTestKind.FooterSummaryCell;
        if (!IsGroupSummary && !IsTotalSummary)
            return false;

        GroupGridColumn Column = Hit.Column;
        GroupGridAggregateKind Current = IsGroupSummary ? Column.GroupSummary : Column.TotalSummary;
        ContextMenu Menu = new()
        {
            Placement = PlacementMode.Pointer,
        };
        List<object> Items = new()
        {
            CreateMenuItem(IsGroupSummary ? "Group Summary" : "Total Summary", false, null),
            new Separator(),
        };

        foreach (GroupGridAggregateKind AggregateKind in Enum.GetValues(typeof(GroupGridAggregateKind)))
        {
            string Header = AggregateKind == Current ? "* " + AggregateKind : AggregateKind.ToString();
            Items.Add(CreateMenuItem(Header, Column.CanAggregate(AggregateKind), () => SetSummaryAggregate(Column, IsGroupSummary, AggregateKind)));
        }

        Menu.ItemsSource = Items;
        Menu.Open(this);
        return true;
    }
    void SetSummaryAggregate(GroupGridColumn Column, bool IsGroupSummary, GroupGridAggregateKind AggregateKind)
    {
        if (Column == null || !Column.CanAggregate(AggregateKind))
            return;

        CancelCellEdit();
        if (IsGroupSummary)
        {
            Column.GroupSummary = AggregateKind;
            if (Column.TotalSummary == GroupGridAggregateKind.None)
                Column.TotalSummary = AggregateKind;
        }
        else
        {
            Column.TotalSummary = AggregateKind;
        }

        fEngine.RebuildProjection();
        InvalidateVisual();
    }

    // ● toolbar layout and command helpers
    List<(GroupGridToolButton Button, Rect Rect)> LayoutToolButtons(double Width)
    {
        List<(GroupGridToolButton Button, Rect Rect)> Result = new();
        if (fEngine == null || !fIsToolBarVisible)
            return Result;

        double ButtonSize = 24;
        double Gap = 4;
        double Margin = 4;
        double Y = Math.Max(0, (fEngine.LayoutMetrics.ToolBarHeight - ButtonSize) / 2);
        double X = Margin;

        foreach (GroupGridToolButton Button in fToolButtons.Where(Button => Button.IsVisible && Button.Alignment == GroupGridToolButtonAlignment.Left))
        {
            Result.Add((Button, new Rect(X, Y, ButtonSize, ButtonSize)));
            X += ButtonSize + Gap;
        }

        List<GroupGridToolButton> RightButtons = fToolButtons
            .Where(Button => Button.IsVisible && Button.Alignment == GroupGridToolButtonAlignment.Right)
            .ToList();
        double RightWidth = RightButtons.Count == 0 ? 0 : (RightButtons.Count * ButtonSize) + ((RightButtons.Count - 1) * Gap);
        X = Math.Max(Margin, Width - Margin - RightWidth);
        foreach (GroupGridToolButton Button in RightButtons)
        {
            Result.Add((Button, new Rect(X, Y, ButtonSize, ButtonSize)));
            X += ButtonSize + Gap;
        }

        return Result;
    }
    bool IsToolButtonEnabled(GroupGridToolButton Button)
    {
        if (Button == null || !Button.IsEnabled || fEngine == null)
            return false;

        switch (Button.Name)
        {
            case InsertToolButtonName:
                return fEngine.CanInsertRow();
            case DeleteToolButtonName:
                return fEngine.CanDeleteCurrentRow();
        }

        return true;
    }
    GroupGridToolButton GetToolButtonAt(Point Point)
    {
        foreach ((GroupGridToolButton Button, Rect Rect) in LayoutToolButtons(Bounds.Width))
            if (Rect.Contains(Point))
                return Button;

        return null;
    }
    void UpdateToolButtonPointerOver(Point Point)
    {
        GroupGridToolButton Button = GetToolButtonAt(Point);
        if (ReferenceEquals(fPointerOverToolButton, Button))
            return;

        fPointerOverToolButton = Button;
        ToolTip.SetTip(this, string.IsNullOrWhiteSpace(Button?.ToolTip) ? null : Button.ToolTip);
        InvalidateVisual();
    }
    bool HandleToolButtonPointerPressed(Point Point)
    {
        GroupGridToolButton Button = GetToolButtonAt(Point);
        if (Button == null)
            return false;

        if (!IsToolButtonEnabled(Button))
            return true;

        switch (Button.Name)
        {
            case InsertToolButtonName:
                CancelCellEdit();
                if (fEngine.InsertRow())
                {
                    ScrollCurrentCellIntoViewCore();
                    BeginCellEdit(false, null);
                }
                return true;
            case DeleteToolButtonName:
                CancelCellEdit();
                DeleteCurrentRowFromToolBarAsync();
                return true;
            case EditToolButtonName:
                fEngine.RequestEdit();
                return true;
        }

        ToolButtonClicked?.Invoke(this, new GroupGridToolButtonEventArgs(Button));
        return true;
    }
    async void DeleteCurrentRowFromToolBarAsync()
    {
        if (fEngine == null || fIsDeleteConfirmationOpen || !fEngine.CanDeleteCurrentRow())
            return;

        if (DeletingRow == null)
        {
            fIsDeleteConfirmationOpen = true;
            bool Confirmed;
            try
            {
                Confirmed = await ShowDefaultDeleteConfirmationAsync();
            }
            finally
            {
                fIsDeleteConfirmationOpen = false;
            }

            if (!Confirmed)
                return;
        }

        if (fEngine.DeleteCurrentRow())
            ScrollCurrentCellIntoViewCore();
    }

    // ● dialog helpers
    Task<bool> ShowDefaultDeleteConfirmationAsync()
    {
        Window Owner = TopLevel.GetTopLevel(this) as Window;
        if (Owner == null)
            return Task.FromResult(false);

        TextBlock Text = new()
        {
            Text = "Delete current row?",
            Margin = new Thickness(12, 12, 12, 8),
        };
        Button CancelButton = new()
        {
            Content = "Cancel",
            MinWidth = 80,
        };
        Button DeleteButton = new()
        {
            Content = "Delete",
            MinWidth = 80,
        };
        StackPanel ButtonPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(12, 0, 12, 12),
            Children =
            {
                CancelButton,
                DeleteButton,
            },
        };
        StackPanel Panel = new()
        {
            Children =
            {
                Text,
                ButtonPanel,
            },
        };
        Window Dialog = new()
        {
            Title = "Confirm Delete",
            Width = 280,
            Height = 120,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = Panel,
        };

        CancelButton.Click += (Sender, Args) => Dialog.Close(false);
        DeleteButton.Click += (Sender, Args) => Dialog.Close(true);
        return Dialog.ShowDialog<bool>(Owner);
    }
    async void RequestColumnManager()
    {
        if (ColumnManagerRequested != null)
        {
            ColumnManagerRequested(this, EventArgs.Empty);
            return;
        }

        if (fIsColumnManagerOpen)
            return;

        fIsColumnManagerOpen = true;
        try
        {
            await ShowDefaultColumnManagerAsync();
        }
        finally
        {
            fIsColumnManagerOpen = false;
        }
    }
    async Task<bool> ShowDefaultColumnManagerAsync()
    {
        Window Owner = TopLevel.GetTopLevel(this) as Window;
        if (Owner == null)
            return false;

        GroupGridSettings Settings = CreateSettings();
        GroupGridColumnManagerDialog Dialog = new(Settings, fEngine.Columns);
        bool Result = await Dialog.ShowDialog<bool>(Owner);
        if (Result)
            ApplySettings(Dialog.Settings);

        return Result;
    }
    async void SaveSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerFileType FileType = new("JSON")
        {
            Patterns = new[] { "*.json" },
        };
        FilePickerSaveOptions Options = new()
        {
            Title = "Save Settings",
            SuggestedFileName = string.IsNullOrWhiteSpace(SettingsSuggestedFileName) ? "group-grid-settings.json" : SettingsSuggestedFileName,
            DefaultExtension = "json",
            FileTypeChoices = new[] { FileType },
        };
        IStorageFile File = await Owner.StorageProvider.SaveFilePickerAsync(Options);
        if (File == null || !File.Path.IsFile)
            return;

        SaveSettings(File.Path.LocalPath);
    }
    async void LoadSettingsAsync()
    {
        TopLevel Owner = TopLevel.GetTopLevel(this);
        if (Owner == null)
            return;

        FilePickerFileType FileType = new("JSON")
        {
            Patterns = new[] { "*.json" },
        };
        FilePickerOpenOptions Options = new()
        {
            Title = "Load Settings",
            AllowMultiple = false,
            FileTypeFilter = new[] { FileType },
        };
        IReadOnlyList<IStorageFile> Files = await Owner.StorageProvider.OpenFilePickerAsync(Options);
        IStorageFile File = Files.FirstOrDefault();
        if (File == null || !File.Path.IsFile)
            return;

        LoadSettings(File.Path.LocalPath);
    }
    async void ExportAsync(GroupGridExporter Exporter)
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
            SuggestedFileName = string.IsNullOrWhiteSpace(Extension) ? "GroupGrid" : "GroupGrid." + Extension,
            DefaultExtension = Extension,
            FileTypeChoices = new[] { FileType },
        };
        IStorageFile File = await Owner.StorageProvider.SaveFilePickerAsync(Options);
        if (File == null || !File.Path.IsFile)
            return;

        SaveExport(Exporter, File.Path.LocalPath);
    }

    // ● drawing helpers
    void DrawToolBar(DrawingContext Context, Rect Rect)
    {
        DrawBand(Context, Rect, ToolBarBrush);

        foreach ((GroupGridToolButton Button, Rect ButtonRect) in LayoutToolButtons(Rect.Width))
        {
            bool IsEnabled = IsToolButtonEnabled(Button);
            bool IsPointerOver = ReferenceEquals(fPointerOverToolButton, Button);
            IBrush ButtonBrush = IsPointerOver && IsEnabled ? ToolButtonHoverBrush : ToolButtonBrush;
            IBrush ForegroundBrush = IsEnabled ? TextBrush : DisabledTextBrush;
            Context.DrawRectangle(ButtonBrush, fLinePen, ButtonRect, 3, 3);
            DrawText(Context, Button.Text, ButtonRect, ForegroundBrush, FontWeight.SemiBold, GroupGridCellHorizontalAlignment.Center);
        }
    }
    Rect InsetRect(Rect Rect, double Value)
    {
        return new Rect(Rect.X + Value, Rect.Y + Value, Math.Max(0, Rect.Width - (Value * 2)), Math.Max(0, Rect.Height - (Value * 2)));
    }
    FormattedText CreateText(string Text, IBrush Brush, double MaxWidth, FontWeight Weight = FontWeight.Normal)
    {
        FormattedText Result = new(Text ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI", FontStyle.Normal, Weight, FontStretch.Normal), 12, Brush);
        Result.MaxTextWidth = Math.Max(0, MaxWidth);
        Result.MaxLineCount = 1;
        Result.Trimming = TextTrimming.CharacterEllipsis;
        return Result;
    }
    double GetTextX(Rect Rect, FormattedText Text, GroupGridCellHorizontalAlignment Alignment)
    {
        switch (Alignment)
        {
            case GroupGridCellHorizontalAlignment.Center:
                return Rect.X + Math.Max(4, (Rect.Width - Text.Width) / 2);
            case GroupGridCellHorizontalAlignment.Right:
                return Rect.Right - Text.Width - 4;
        }

        return Rect.X + 4;
    }
    void DrawText(DrawingContext Context, string Text, Rect Rect, IBrush Brush, FontWeight Weight = FontWeight.Normal, GroupGridCellHorizontalAlignment Alignment = GroupGridCellHorizontalAlignment.Left)
    {
        if (string.IsNullOrEmpty(Text) || Rect.Width <= 4 || Rect.Height <= 4)
            return;

        using (Context.PushClip(Rect))
        {
            FormattedText FormattedText = CreateText(Text, Brush, Rect.Width - 8, Weight);
            double X = GetTextX(Rect, FormattedText, Alignment);
            double Y = Rect.Y + Math.Max(2, (Rect.Height - FormattedText.Height) / 2);
            Context.DrawText(FormattedText, new Point(X, Y));
        }
    }
    void DrawBand(DrawingContext Context, Rect Rect, IBrush Brush)
    {
        Context.DrawRectangle(Brush, fLinePen, Rect);
    }
    void DrawColumnHeaderCell(DrawingContext Context, GroupGridColumn Column, Rect Rect, IBrush Brush)
    {
        DrawBand(Context, Rect, Brush);

        string Text = string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header;
        Rect TextRect = Rect;
        if (ReferenceEquals(fEngine.SortColumn, Column) && fEngine.SortDirection != GroupGridSortDirection.None)
            TextRect = new Rect(Rect.X, Rect.Y, Math.Max(0, Rect.Width - 18), Rect.Height);

        DrawText(Context, Text, TextRect, TextBrush, FontWeight.SemiBold);
        DrawSortGlyph(Context, Column, Rect);
    }
    void DrawSortGlyph(DrawingContext Context, GroupGridColumn Column, Rect Rect)
    {
        if (!ReferenceEquals(fEngine.SortColumn, Column) || fEngine.SortDirection == GroupGridSortDirection.None)
            return;

        double CenterX = Rect.Right - 10;
        double CenterY = Rect.Y + (Rect.Height / 2);
        double Size = 8;
        StreamGeometry Geometry = new();

        using (StreamGeometryContext GeometryContext = Geometry.Open())
        {
            if (fEngine.SortDirection == GroupGridSortDirection.Ascending)
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
    void DrawFilterCell(DrawingContext Context, GroupGridColumn Column, Rect Rect, IBrush Brush)
    {
        bool IsActive = ReferenceEquals(fFilterEditColumn, Column);
        string FilterText = IsActive
            ? fFilterEditText
            : fEngine.GetColumnFilter(Column);
        bool HasFilter = !string.IsNullOrEmpty(FilterText);
        IBrush CellBrush = IsActive ? ActiveFilterBrush : HasFilter ? FilteredCellBrush : Brush;
        DrawBand(Context, Rect, CellBrush);

        if (!string.IsNullOrEmpty(FilterText))
            DrawText(Context, FilterText, Rect, MutedTextBrush);

        if (IsActive)
        {
            Context.DrawRectangle(null, fCurrentPen, InsetRect(Rect, 1));
            DrawFilterCaret(Context, FilterText, Rect);
        }
    }
    void DrawFilterCaret(DrawingContext Context, string Text, Rect Rect)
    {
        double X = Rect.X + 4;
        if (!string.IsNullOrEmpty(Text))
        {
            FormattedText FormattedText = CreateText(Text, MutedTextBrush, Math.Max(0, Rect.Width - 8));
            X += Math.Min(FormattedText.Width, Math.Max(0, Rect.Width - 10));
        }

        double Top = Rect.Y + 4;
        double Bottom = Rect.Bottom - 4;
        Context.DrawLine(fCurrentPen, new Point(X, Top), new Point(X, Bottom));
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
    double DrawColumns(DrawingContext Context, double Y, double Height, double ContentWidth, IBrush Brush, bool DrawFilterText)
    {
        double X = -fHorizontalOffset;
        using (Context.PushClip(new Rect(0, Y, ContentWidth, Height)))
        {
            foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
            {
                double ColumnWidth = Math.Max(Column.MinWidth, Column.Width);
                if (X >= ContentWidth)
                    break;

                if (X + ColumnWidth > 0)
                {
                    Rect Rect = new(X, Y, ColumnWidth, Height);
                    if (DrawFilterText)
                        DrawFilterCell(Context, Column, Rect, Brush);
                    else
                        DrawColumnHeaderCell(Context, Column, Rect, Brush);
                }

                X += ColumnWidth;
            }
        }

        return X;
    }

    // ● group and body drawing helpers
    void DrawGroupPanel(DrawingContext Context, double Y, double Height, double Width)
    {
        DrawBand(Context, new Rect(0, Y, Width, Height), GroupPanelBrush);

        if (fEngine.GroupColumns.Count == 0)
        {
            DrawText(Context, EmptyGroupPanelText, new Rect(8, Y, Math.Max(0, Width - 16), Height), MutedTextBrush, FontWeight.SemiBold);
            return;
        }

        double X = 6;
        foreach (GroupGridColumn Column in fEngine.GroupColumns)
        {
            string Text = string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header;
            double ItemWidth = Math.Max(80, Text.Length * 8 + 24);
            Rect Rect = new(X, Y + 5, ItemWidth, Math.Max(0, Height - 10));
            Context.DrawRectangle(HeaderBrush, fLinePen, Rect, 3, 3);
            DrawText(Context, Text, Rect, TextBrush, FontWeight.SemiBold);
            X += ItemWidth + 6;
        }
    }
    void DrawBody(DrawingContext Context, double Y, double Width)
    {
        double RowHeight = fEngine.LayoutMetrics.RowHeight;
        if (RowHeight <= 0 || fEngine.Viewport.IsEmpty)
            return;

        for (int Index = 0; Index < fEngine.Viewport.Count; Index++)
        {
            int VisibleNodeIndex = fEngine.Viewport.FirstVisibleNodeIndex + Index;
            GroupGridRowInfo RowInfo = fEngine.GetVisibleRowInfo(VisibleNodeIndex);
            Rect RowRect = new(0, Y + (Index * RowHeight), Width, RowHeight);

            if (RowInfo.IsGroup)
                DrawGroupRow(Context, RowRect, VisibleNodeIndex, RowInfo);
            else
                DrawValueRow(Context, RowRect, VisibleNodeIndex, RowInfo);
        }
    }
    string GetGroupSummaryText(int VisibleNodeIndex, GroupGridColumn Column)
    {
        GroupGridSummaryValue Summary = fEngine.GetGroupSummary(VisibleNodeIndex, Column);
        return Summary.AggregateKind == GroupGridAggregateKind.None
            ? string.Empty
            : Column.FormatSummaryValue(Summary);
    }
    double GetFirstCollapsedGroupSummaryX(int VisibleNodeIndex, double RowWidth)
    {
        double X = -fHorizontalOffset;
        foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
        {
            double Width = Math.Max(Column.MinWidth, Column.Width);
            if (X >= RowWidth)
                break;

            if (X + Width > 0 && !string.IsNullOrWhiteSpace(GetGroupSummaryText(VisibleNodeIndex, Column)))
                return Math.Max(0, X);

            X += Width;
        }

        return RowWidth;
    }
    void DrawCollapsedGroupSummaries(DrawingContext Context, Rect RowRect, int VisibleNodeIndex)
    {
        double X = -fHorizontalOffset;
        using (Context.PushClip(RowRect))
        {
            foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
            {
                double Width = Math.Max(Column.MinWidth, Column.Width);
                if (X >= RowRect.Width)
                    break;

                if (X + Width > 0)
                {
                    string Text = GetGroupSummaryText(VisibleNodeIndex, Column);
                    if (!string.IsNullOrWhiteSpace(Text))
                    {
                        Rect CellRect = new(X, RowRect.Y, Width, RowRect.Height);
                        DrawText(Context, Text, CellRect, MutedTextBrush, FontWeight.SemiBold, Column.HorizontalAlignment);
                    }
                }

                X += Width;
            }
        }
    }
    void DrawGroupRow(DrawingContext Context, Rect RowRect, int VisibleNodeIndex, GroupGridRowInfo RowInfo)
    {
        DrawBand(Context, RowRect, GroupRowBrush);

        double X = Math.Max(0, RowInfo.Level) * fEngine.LayoutMetrics.GroupIndentWidth;
        Rect ExpanderRect = new(X, RowRect.Y, fEngine.LayoutMetrics.GroupExpanderWidth, RowRect.Height);
        double HeaderRight = RowRect.Width;
        if (!RowInfo.IsExpanded)
            HeaderRight = Math.Min(HeaderRight, GetFirstCollapsedGroupSummaryX(VisibleNodeIndex, RowRect.Width));

        DrawExpander(Context, ExpanderRect, RowInfo.IsExpanded);
        DrawText(Context, fEngine.GetGroupHeaderText(VisibleNodeIndex), new Rect(ExpanderRect.Right, RowRect.Y, Math.Max(0, HeaderRight - ExpanderRect.Right), RowRect.Height), TextBrush, FontWeight.SemiBold);

        if (!RowInfo.IsExpanded)
            DrawCollapsedGroupSummaries(Context, RowRect, VisibleNodeIndex);
    }
    void DrawValueRow(DrawingContext Context, Rect RowRect, int VisibleNodeIndex, GroupGridRowInfo RowInfo)
    {
        IBrush RowBrush = RowInfo.IsGroupSummary ? GroupSummaryBrush : GridBackgroundBrush;
        if (IsSelectedRow(RowInfo))
            RowBrush = SelectedBrush;

        DrawBand(Context, RowRect, RowBrush);

        double X = -fHorizontalOffset;
        using (Context.PushClip(RowRect))
        {
            foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
            {
                double Width = Math.Max(Column.MinWidth, Column.Width);
                if (X >= RowRect.Width)
                    break;

                if (X + Width > 0)
                {
                    Rect CellRect = new(X, RowRect.Y, Width, RowRect.Height);
                    IBrush CellBrush = RowBrush;
                    if (IsSelectedRow(RowInfo))
                        CellBrush = SelectedBrush;
                    if (IsCurrentCell(RowInfo, Column))
                        CellBrush = CurrentBrush;
                    if (IsEditingCell(RowInfo, Column))
                        CellBrush = EditingBrush;

                    Context.DrawRectangle(CellBrush, fLinePen, CellRect);
                    string Text = IsEditingCell(RowInfo, Column) ? fCellEditText : fEngine.GetDisplayText(VisibleNodeIndex, Column);
                    DrawText(Context, Text, CellRect, RowInfo.IsGroupSummary ? MutedTextBrush : TextBrush, FontWeight.Normal, Column.HorizontalAlignment);

                    if (IsCurrentCell(RowInfo, Column) && !(IsEditingCell(RowInfo, Column) && fActiveEditor != null))
                        Context.DrawRectangle(null, fCurrentPen, CellRect);
                    if (IsEditingCell(RowInfo, Column) && fActiveEditor == null)
                    {
                        Context.DrawRectangle(null, fEditingPen, InsetRect(CellRect, 1));
                        DrawFilterCaret(Context, Text, CellRect);
                    }
                }

                X += Width;
            }
        }
    }
    void DrawVerticalScrollBar(DrawingContext Context)
    {
        if (!HasVerticalScrollBar())
            return;

        Rect TrackRect = GetVerticalScrollTrackRect();
        Rect ThumbRect = GetVerticalScrollThumbRect();
        Context.DrawRectangle(ScrollBarTrackBrush, fLinePen, TrackRect);
        Context.DrawRectangle(ScrollBarThumbBrush, null, ThumbRect, 4, 4);
    }
    void DrawHorizontalScrollBar(DrawingContext Context)
    {
        if (!HasHorizontalScrollBar())
            return;

        Rect TrackRect = GetHorizontalScrollTrackRect();
        Rect ThumbRect = GetHorizontalScrollThumbRect();
        Context.DrawRectangle(ScrollBarTrackBrush, fLinePen, TrackRect);
        Context.DrawRectangle(ScrollBarThumbBrush, null, ThumbRect, 4, 4);

        if (HasVerticalScrollBar())
            DrawBand(Context, new Rect(TrackRect.Right, TrackRect.Y, Bounds.Width - TrackRect.Right, TrackRect.Height), ScrollBarTrackBrush);
    }
    void DrawColumnResizeGuide(DrawingContext Context)
    {
        if (!fIsColumnResizing)
            return;

        double X = Math.Clamp(fColumnResizeCurrentX, 0, GetBodyContentWidth());
        double Top = GetToolBarHeight() + GetGroupPanelHeight();
        double Bottom = GetBodyTop() + GetBodyHeight() + GetTotalsSummaryHeight();
        Context.DrawLine(fResizePen, new Point(X, Top), new Point(X, Bottom));
    }
    void DrawColumnDragGuide(DrawingContext Context)
    {
        if (!fIsColumnDragActive)
            return;

        if (fColumnDragGroupDropIndex < 0 && fColumnDragDropIndex < 0)
            return;

        double X = Math.Clamp(fColumnDragDropX, 0, GetBodyContentWidth());
        if (fColumnDragGroupDropIndex >= 0)
        {
            Rect PanelRect = GetGroupPanelRect();
            Context.DrawLine(fResizePen, new Point(X, PanelRect.Y + 4), new Point(X, PanelRect.Bottom - 4));
            return;
        }

        double Top = GetToolBarHeight() + GetGroupPanelHeight();
        double Bottom = GetBodyTop() + GetBodyHeight() + GetTotalsSummaryHeight();
        Context.DrawLine(fResizePen, new Point(X, Top), new Point(X, Bottom));
    }
    void DrawColumnDragGhost(DrawingContext Context)
    {
        if (!fIsColumnDragActive || fColumnDragColumn == null)
            return;

        double Width = Math.Max(fColumnDragColumn.MinWidth, fColumnDragColumn.Width);
        double Height = fEngine.LayoutMetrics.ColumnHeaderHeight;
        double X = fColumnDragCurrentX - (Width / 2);
        double Y = fColumnDragCurrentY - (Height / 2);
        Rect Rect = new(X, Y, Width, Height);
        DrawColumnHeaderCell(Context, fColumnDragColumn, Rect, ColumnDragBrush);
    }
    void DrawFooter(DrawingContext Context, double Y, double Height, double ContentWidth)
    {
        double X = -fHorizontalOffset;
        using (Context.PushClip(new Rect(0, Y, ContentWidth, Height)))
        {
            foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
            {
                double ColumnWidth = Math.Max(Column.MinWidth, Column.Width);
                if (X >= ContentWidth)
                    break;

                if (X + ColumnWidth > 0)
                {
                    Rect Rect = new(X, Y, ColumnWidth, Height);
                    DrawBand(Context, Rect, FooterBrush);
                    DrawText(Context, fEngine.GetTotalSummaryText(Column), Rect, MutedTextBrush, FontWeight.SemiBold);
                }

                X += ColumnWidth;
            }
        }
    }

    // ● hit-test action helpers
    void HandleHitTest(GroupGridHitTestResult Hit)
    {
        if (Hit == null || fEngine == null)
            return;

        if (Hit.Kind == GroupGridHitTestKind.GroupExpander)
        {
            SetFilterEditColumn(null);
            fEngine.ToggleGroupExpanded(Hit.VisibleNodeIndex);
            return;
        }

        if (Hit.Kind == GroupGridHitTestKind.FilterCell && Hit.Column != null)
        {
            SetFilterEditColumn(Hit.Column);
            return;
        }

        if (Hit.Kind == GroupGridHitTestKind.BodyCell && Hit.RowKind == GroupGridRowKind.DataRow && Hit.HasCell)
        {
            SetFilterEditColumn(null);
            fEngine.SetCurrentCell(Hit.Cell);
            fEngine.SetSelectedCell(Hit.Cell);
            if (!ToggleCheckBoxCell(Hit))
                BeginCellEdit(false, null);
        }
    }
    bool RaiseCellPointerPressed(GroupGridHitTestResult Hit, Point Point, PointerPressedEventArgs Args, bool IsRightButton)
    {
        if (Hit == null || Hit.Kind != GroupGridHitTestKind.BodyCell || Hit.RowKind != GroupGridRowKind.DataRow || !Hit.HasCell)
            return false;

        SetFilterEditColumn(null);
        fEngine.SetCurrentCell(Hit.Cell);
        fEngine.SetSelectedCell(Hit.Cell);
        GroupGridCellPointerEventArgs CellArgs = new(Hit, GetAdapterRow(Hit.RowIndex), Point, Args, IsRightButton);
        CellPointerPressed?.Invoke(this, CellArgs);
        if (CellArgs.Handled)
            Args.Handled = true;
        return CellArgs.Handled || Args.Handled;
    }

    // ● pointer handling helpers
    bool HandleVerticalScrollPointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        if (!HasVerticalScrollBar())
            return false;

        Rect TrackRect = GetVerticalScrollTrackRect();
        if (!TrackRect.Contains(Point))
            return false;

        Rect ThumbRect = GetVerticalScrollThumbRect();
        if (ThumbRect.Contains(Point))
        {
            fIsVerticalScrollDragging = true;
            fVerticalScrollDragOffset = Point.Y - ThumbRect.Y;
            Args.Pointer.Capture(this);
            return true;
        }

        int Delta = Point.Y < ThumbRect.Y ? -fEngine.Viewport.Count : fEngine.Viewport.Count;
        ScrollViewport(Delta);
        return true;
    }
    bool HandleHorizontalScrollPointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        if (!HasHorizontalScrollBar())
            return false;

        Rect TrackRect = GetHorizontalScrollTrackRect();
        if (!TrackRect.Contains(Point))
            return false;

        Rect ThumbRect = GetHorizontalScrollThumbRect();
        if (ThumbRect.Contains(Point))
        {
            fIsHorizontalScrollDragging = true;
            fHorizontalScrollDragOffset = Point.X - ThumbRect.X;
            Args.Pointer.Capture(this);
            return true;
        }

        double Delta = Point.X < ThumbRect.X ? -TrackRect.Width : TrackRect.Width;
        SetHorizontalOffsetCore(fHorizontalOffset + Delta);
        return true;
    }
    bool HandleColumnResizePointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        GroupGridHitTestResult Hit = HitTestCore(Point);
        if (Hit == null || Hit.Kind != GroupGridHitTestKind.ColumnResizer || Hit.Column == null)
            return false;

        CancelCellEdit();
        fIsColumnResizing = true;
        fColumnResizeColumn = Hit.Column;
        fColumnResizeStartX = Point.X;
        fColumnResizeCurrentX = Point.X;
        fColumnResizeStartWidth = Hit.Column.Width;
        Args.Pointer.Capture(this);
        return true;
    }
    bool HandleColumnResizePointerMoved(Point Point)
    {
        if (!fIsColumnResizing || fColumnResizeColumn == null)
            return false;

        fColumnResizeCurrentX = Point.X;
        SetColumnWidth(fColumnResizeColumn, fColumnResizeStartWidth + Point.X - fColumnResizeStartX);
        return true;
    }
    void EndColumnResize()
    {
        fIsColumnResizing = false;
        fColumnResizeColumn = null;
        fColumnResizeStartX = 0;
        fColumnResizeCurrentX = 0;
        fColumnResizeStartWidth = 0;
        InvalidateVisual();
    }
    bool HandleColumnDragPointerPressed(PointerPressedEventArgs Args, Point Point)
    {
        if (GetGroupPanelColumnAt(Point, out GroupGridColumn GroupColumn, out int GroupIndex))
        {
            CancelCellEdit();
            fIsColumnDragging = true;
            fIsColumnDragActive = false;
            fColumnDragFromGroupPanel = true;
            fColumnDragColumn = GroupColumn;
            fColumnDragSourceGroupIndex = GroupIndex;
            fColumnDragStartX = Point.X;
            fColumnDragCurrentX = Point.X;
            fColumnDragCurrentY = Point.Y;
            fColumnDragDropX = Point.X;
            fColumnDragGroupDropIndex = GroupIndex;
            Args.Pointer.Capture(this);
            return true;
        }

        GroupGridHitTestResult Hit = HitTestCore(Point);
        if (Hit == null || Hit.Kind != GroupGridHitTestKind.ColumnHeader || Hit.Column == null || !Hit.Column.CanUserReorder)
            return false;

        CancelCellEdit();
        fIsColumnDragging = true;
        fIsColumnDragActive = false;
        fColumnDragFromGroupPanel = false;
        fColumnDragColumn = Hit.Column;
        fColumnDragSourceGroupIndex = -1;
        fColumnDragStartX = Point.X;
        fColumnDragCurrentX = Point.X;
        fColumnDragCurrentY = Point.Y;
        fColumnDragDropX = Point.X;
        fColumnDragDropIndex = fEngine.Columns.IndexOf(Hit.Column);
        Args.Pointer.Capture(this);
        return true;
    }
    bool HandleColumnDragPointerMoved(Point Point)
    {
        if (!fIsColumnDragging || fColumnDragColumn == null)
            return false;

        fColumnDragCurrentX = Point.X;
        fColumnDragCurrentY = Point.Y;
        if (!fIsColumnDragActive && Math.Abs(Point.X - fColumnDragStartX) < 4)
            return true;

        fIsColumnDragActive = true;
        if (GetGroupDropInfo(Point, out fColumnDragGroupDropIndex, out fColumnDragDropX))
        {
            fColumnDragDropIndex = -1;
        }
        else if (GetColumnHeaderRect().Contains(Point))
        {
            fColumnDragGroupDropIndex = -1;
            GetColumnDropInfo(Point, out fColumnDragDropIndex, out fColumnDragDropX);
        }
        else
        {
            fColumnDragGroupDropIndex = -1;
            fColumnDragDropIndex = -1;
            fColumnDragDropX = Point.X;
        }

        UpdatePointerCursor(Point);
        InvalidateVisual();
        return true;
    }
    void EndColumnDrag()
    {
        if (fIsColumnDragActive)
        {
            if (fColumnDragFromGroupPanel && fColumnDragDropIndex >= 0)
                UngroupDraggedColumnToHeader();
            else if (fColumnDragFromGroupPanel && fColumnDragGroupDropIndex >= 0)
                MoveGroupedColumn();
            else if (fColumnDragFromGroupPanel)
                fEngine.UngroupColumn(fColumnDragColumn);
            else if (fColumnDragGroupDropIndex >= 0)
                fEngine.GroupColumn(fColumnDragColumn, fColumnDragGroupDropIndex);
            else if (!IsInsideGrid(new Point(fColumnDragCurrentX, fColumnDragCurrentY)))
                fEngine.SetColumnVisible(fColumnDragColumn, false);
            else
                MoveDraggedColumn();
        }

        fIsColumnDragging = false;
        fIsColumnDragActive = false;
        fColumnDragFromGroupPanel = false;
        fColumnDragColumn = null;
        fColumnDragStartX = 0;
        fColumnDragCurrentX = 0;
        fColumnDragCurrentY = 0;
        fColumnDragDropX = 0;
        fColumnDragDropIndex = -1;
        fColumnDragGroupDropIndex = -1;
        fColumnDragSourceGroupIndex = -1;
        InvalidateVisual();
    }
    void UpdatePointerCursor(Point Point)
    {
        if (fIsColumnResizing)
        {
            Cursor = new Cursor(StandardCursorType.SizeWestEast);
            return;
        }
        if (fIsColumnDragging)
        {
            Cursor = IsColumnDragRejectTarget(Point)
                ? new Cursor(StandardCursorType.No)
                : null;
            return;
        }

        GroupGridHitTestResult Hit = HitTestCore(Point);
        Cursor = Hit != null && Hit.Kind == GroupGridHitTestKind.ColumnResizer
            ? new Cursor(StandardCursorType.SizeWestEast)
            : null;
    }
    bool IsHorizontalScrollableBand(Point Point)
    {
        if (!HasHorizontalScrollBar())
            return false;

        double Top = GetToolBarHeight() + GetGroupPanelHeight();
        double Bottom = GetBodyTop() + GetBodyHeight() + GetTotalsSummaryHeight();
        return Point.X >= 0 && Point.X < GetBodyContentWidth() && Point.Y >= Top && Point.Y < Bottom;
    }
    bool TryMapVisibleYToEngineY(double VisibleY, out double EngineY)
    {
        EngineY = VisibleY;
        double VisibleTop = 0;
        double EngineOffset = 0;

        if (!MapVisibleBandY(VisibleY, fEngine.LayoutMetrics.ToolBarHeight, GetToolBarHeight(), ref VisibleTop, ref EngineOffset, out EngineY))
            return false;
        if (!MapVisibleBandY(VisibleY, fEngine.LayoutMetrics.GroupPanelHeight, GetGroupPanelHeight(), ref VisibleTop, ref EngineOffset, out EngineY))
            return false;
        if (!MapVisibleBandY(VisibleY, fEngine.LayoutMetrics.ColumnHeaderHeight, GetColumnHeaderHeight(), ref VisibleTop, ref EngineOffset, out EngineY))
            return false;
        if (!MapVisibleBandY(VisibleY, fEngine.LayoutMetrics.FilterRowHeight, GetFilterPanelHeight(), ref VisibleTop, ref EngineOffset, out EngineY))
            return false;

        double BodyHeight = GetBodyHeight();
        if (VisibleY < VisibleTop + BodyHeight)
        {
            EngineY = VisibleY + EngineOffset;
            return true;
        }

        VisibleTop += BodyHeight;
        if (!MapVisibleBandY(VisibleY, fEngine.LayoutMetrics.FooterSummaryHeight, GetTotalsSummaryHeight(), ref VisibleTop, ref EngineOffset, out EngineY))
            return false;

        EngineY = VisibleY + EngineOffset;
        return true;
    }

    // ● hit-test mapping helpers
    bool MapVisibleBandY(double VisibleY, double MetricHeight, double VisibleHeight, ref double VisibleTop, ref double EngineOffset, out double EngineY)
    {
        EngineY = VisibleY + EngineOffset;
        if (VisibleHeight > 0)
        {
            if (VisibleY < VisibleTop + VisibleHeight)
                return true;

            VisibleTop += VisibleHeight;
            return true;
        }

        EngineOffset += MetricHeight;
        return true;
    }
    GroupGridHitTestResult HitTestCore(Point Point)
    {
        double X = Point.X;
        double Y = Point.Y;
        double HorizontalScrollTop = GetBodyTop() + GetBodyHeight() + GetTotalsSummaryHeight();
        if (HasHorizontalScrollBar() && Point.Y >= HorizontalScrollTop && Point.Y < HorizontalScrollTop + fEngine.LayoutMetrics.HorizontalScrollBarHeight)
            return GroupGridHitTestResult.Empty;

        if (!TryMapVisibleYToEngineY(Y, out double EngineY))
            return GroupGridHitTestResult.Empty;

        GroupGridHitTestResult Result = fEngine.HitTest(X, EngineY);
        if (!IsHorizontalScrollableBand(Point))
            return Result;

        if (Result.Kind == GroupGridHitTestKind.GroupExpander)
            return Result;

        return fEngine.HitTest(X + fHorizontalOffset, EngineY);
    }

    // ● keyboard helpers
    bool HandleKey(KeyEventArgs Args)
    {
        if (fEngine == null)
            return false;

        if (fActiveEditor != null)
            return false;

        if (fFilterEditColumn != null)
        {
            switch (Args.Key)
            {
                case Key.Back:
                    BackspaceFilterText();
                    return true;
                case Key.Delete:
                    ClearFilterEdit();
                    return true;
                case Key.Enter:
                    ApplyFilterEdit();
                    return true;
                case Key.Escape:
                    CancelFilterEdit();
                    return true;
            }
        }

        if (fEngine.IsEditing)
        {
            switch (Args.Key)
            {
                case Key.Back:
                    BackspaceCellEditText();
                    return true;
                case Key.Delete:
                    ClearCellEditText();
                    return true;
                case Key.Enter:
                    return CommitCellEdit();
                case Key.Escape:
                    return CancelCellEdit();
                case Key.Tab:
                    if (!CommitCellEdit())
                        return false;
                    return fEngine.MoveCurrentToNextEditableCell(!Args.KeyModifiers.HasFlag(KeyModifiers.Shift));
            }
        }

        switch (Args.Key)
        {
            case Key.Left:
                return fEngine.MoveCurrentColumn(-1);
            case Key.Right:
                return fEngine.MoveCurrentColumn(1);
            case Key.Up:
                return fEngine.MoveCurrentRow(-1);
            case Key.Down:
                return fEngine.MoveCurrentRow(1);
            case Key.Home:
                return fEngine.MoveCurrentToFirstColumn();
            case Key.End:
                return fEngine.MoveCurrentToLastColumn();
            case Key.Tab:
                return fEngine.MoveCurrentToNextEditableCell(!Args.KeyModifiers.HasFlag(KeyModifiers.Shift));
            case Key.Space:
                return ToggleCheckBoxCell(fEngine.CurrentCell);
            case Key.Enter:
            case Key.F2:
                return BeginCellEdit(false, null);
        }

        return false;
    }

    // ● protected methods
    // ● theme change hooks
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs Args)
    {
        base.OnPropertyChanged(Args);

        if (Args.Property == GridLineBrushProperty
            || Args.Property == CurrentBorderBrushProperty
            || Args.Property == EditingBorderBrushProperty
            || Args.Property == ResizeGuideBrushProperty)
            UpdateThemePens();

        if (Args.Property == EditingBrushProperty && fActiveEditor != null)
            fActiveEditor.Background = EditingBrush;
        if (Args.Property == TextBrushProperty && fActiveEditor != null)
        {
            if (fActiveEditor is GroupGridDateInplaceEditor DateEditor)
                DateEditor.Foreground = TextBrush;
            else if (fActiveEditor is GroupGridTextInplaceEditor TextEditor)
                TextEditor.Foreground = TextBrush;
        }
        if (Args.Property == BoundsProperty)
            UpdateViewport(Bounds.Size);
    }

    // ● pointer overrides
    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs Args)
    {
        base.OnPointerPressed(Args);

        if (fEngine == null)
            return;

        PointerPoint PointProperties = Args.GetCurrentPoint(this);
        if (PointProperties.Properties.IsRightButtonPressed)
        {
            Point MenuPoint = Args.GetPosition(this);
            GroupGridHitTestResult Hit = HitTestCore(MenuPoint);
            if (RaiseCellPointerPressed(Hit, MenuPoint, Args, IsRightButton: true))
                return;

            if (ShowColumnContextMenu(MenuPoint) || ShowSummaryContextMenu(MenuPoint))
                Args.Handled = true;
            return;
        }

        if (!PointProperties.Properties.IsLeftButtonPressed)
            return;

        Focus(NavigationMethod.Pointer, KeyModifiers.None);

        Point Point = Args.GetPosition(this);
        if (HandleToolButtonPointerPressed(Point))
        {
            Args.Handled = true;
            return;
        }
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
        if (HandleColumnResizePointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }
        if (HandleColumnDragPointerPressed(Args, Point))
        {
            Args.Handled = true;
            return;
        }

        HandleHitTest(HitTestCore(Point));
        Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnPointerMoved(PointerEventArgs Args)
    {
        base.OnPointerMoved(Args);

        Point Point = Args.GetPosition(this);
        UpdateToolButtonPointerOver(Point);
        if (HandleColumnResizePointerMoved(Point))
        {
            UpdatePointerCursor(Point);
            Args.Handled = true;
            return;
        }
        if (HandleColumnDragPointerMoved(Point))
        {
            Args.Handled = true;
            return;
        }
        if (fIsVerticalScrollDragging && SetFirstVisibleNodeIndexFromVerticalScroll(Point.Y))
            Args.Handled = true;
        if (fIsHorizontalScrollDragging && SetHorizontalOffsetFromScroll(Point.X))
            Args.Handled = true;
        if (!Args.Handled)
        {
            UpdatePointerCursor(Point);
            if (fPointerOverToolButton != null && IsToolButtonEnabled(fPointerOverToolButton))
                Cursor = new Cursor(StandardCursorType.Hand);
        }
    }
    /// <inheritdoc />
    protected override void OnPointerReleased(PointerReleasedEventArgs Args)
    {
        base.OnPointerReleased(Args);

        if (!fIsVerticalScrollDragging && !fIsHorizontalScrollDragging && !fIsColumnResizing && !fIsColumnDragging)
            return;

        bool WasColumnResizing = fIsColumnResizing;
        bool WasColumnDragging = fIsColumnDragging;
        Point Point = Args.GetPosition(this);
        fIsVerticalScrollDragging = false;
        fIsHorizontalScrollDragging = false;
        fVerticalScrollDragOffset = 0;
        fHorizontalScrollDragOffset = 0;
        if (WasColumnResizing)
            EndColumnResize();
        if (WasColumnDragging)
            HandleColumnDragPointerMoved(Point);
        if (WasColumnDragging)
            EndColumnDrag();
        Args.Pointer.Capture(null);
        UpdatePointerCursor(Point);
        Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnPointerExited(PointerEventArgs Args)
    {
        base.OnPointerExited(Args);

        if (!fIsColumnResizing)
            Cursor = null;
        fPointerOverToolButton = null;
        ToolTip.SetTip(this, null);
        InvalidateVisual();
    }

    // ● keyboard and text input overrides
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs Args)
    {
        base.OnKeyDown(Args);

        if (HandleKey(Args))
        {
            ScrollCurrentCellIntoViewCore();
            Args.Handled = true;
        }
    }
    /// <inheritdoc />
    protected override void OnTextInput(TextInputEventArgs Args)
    {
        base.OnTextInput(Args);

        if (AppendFilterText(Args.Text))
        {
            Args.Handled = true;
            return;
        }

        if (AppendCellEditText(Args.Text))
        {
            Args.Handled = true;
            return;
        }

        if (IsPrintableText(Args.Text) && BeginCellEdit(true, Args.Text))
            Args.Handled = true;
    }
    /// <inheritdoc />
    protected override void OnPointerWheelChanged(PointerWheelEventArgs Args)
    {
        base.OnPointerWheelChanged(Args);

        if (Args.Delta.Y == 0)
            return;

        int Delta = Args.Delta.Y < 0 ? 3 : -3;
        if (ScrollViewport(Delta))
            Args.Handled = true;
    }

    // ● layout overrides
    /// <inheritdoc />
    protected override Size ArrangeOverride(Size FinalSize)
    {
        UpdateViewport(FinalSize);
        Size Result = base.ArrangeOverride(FinalSize);
        Rect EditorRect = GetEditorRect();
        fLastEditorRect = default;
        fLastDropDownRect = default;
        if (fActiveEditor != null)
        {
            fLastEditorRect = EditorRect;
            fActiveEditor.Arrange(EditorRect);
        }
        if (fActiveDropDownHost != null)
        {
            fLastEditorRect = EditorRect;
            Rect DropDownRect = GetDropDownRect(EditorRect);
            double Height = DropDownRect.Height;
            double Width = DropDownRect.Width;
            SetDropDownListBoxHeight(Height);
            fLastDropDownRect = DropDownRect;
            if (fActiveDropDownPopup != null)
            {
                fActiveDropDownHost.Width = Width;
                fActiveDropDownHost.Height = Height;
                ConfigureActiveDropDownPopup(EditorRect);
            }
            else
            {
                fActiveDropDownHost.Arrange(DropDownRect);
            }
        }
        return Result;
    }
    /// <inheritdoc />
    protected override Size MeasureOverride(Size AvailableSize)
    {
        if (fActiveEditor != null)
        {
            Rect Rect = GetEditorRect();
            fActiveEditor.Measure(Rect.Size);
        }
        if (fActiveDropDownHost != null)
        {
            Rect Rect = GetEditorRect();
            Rect DropDownRect = GetDropDownRect(Rect);
            double Height = DropDownRect.Height;
            double Width = DropDownRect.Width;
            SetDropDownListBoxHeight(Height);
            if (fActiveDropDownPopup != null)
            {
                fActiveDropDownHost.Width = Width;
                fActiveDropDownHost.Height = Height;
            }
            else
            {
                fActiveDropDownHost.Measure(new Size(Width, Height));
            }
        }

        return base.MeasureOverride(AvailableSize);
    }

    // ● constructor
    static GroupGrid()
    {
        AffectsRender<GroupGrid>(
            GridBackgroundBrushProperty,
            ToolBarBrushProperty,
            ToolButtonBrushProperty,
            ToolButtonHoverBrushProperty,
            DisabledTextBrushProperty,
            GroupPanelBrushProperty,
            HeaderBrushProperty,
            FilterBrushProperty,
            FilteredCellBrushProperty,
            ActiveFilterBrushProperty,
            GroupRowBrushProperty,
            GroupSummaryBrushProperty,
            SelectedBrushProperty,
            CurrentBrushProperty,
            EditingBrushProperty,
            FooterBrushProperty,
            TextBrushProperty,
            MutedTextBrushProperty,
            ScrollBarTrackBrushProperty,
            ScrollBarThumbBrushProperty,
            ColumnDragBrushProperty,
            GridLineBrushProperty,
            CurrentBorderBrushProperty,
            EditingBorderBrushProperty,
            ResizeGuideBrushProperty);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGrid"/> class.
    /// </summary>
    public GroupGrid()
    {
        Focusable = true;
        UpdateThemePens();
        fToolButtons.CollectionChanged += ToolButtons_CollectionChanged;
        AddStandardToolButtons();
        Engine = new GroupGridEngine();
    }

    // ● public methods
    // ● viewport API
    /// <summary>
    /// Sets the first visible node index in the virtual viewport.
    /// </summary>
    /// <param name="VisibleNodeIndex">The first visible node index.</param>
    /// <returns>True if the viewport changed; otherwise, false.</returns>
    public bool SetFirstVisibleNodeIndex(int VisibleNodeIndex)
    {
        return SetFirstVisibleNodeIndexCore(VisibleNodeIndex);
    }
    /// <summary>
    /// Sets the horizontal scroll offset in pixels.
    /// </summary>
    /// <param name="HorizontalOffset">The horizontal scroll offset.</param>
    /// <returns>True if the offset changed; otherwise, false.</returns>
    public bool SetHorizontalOffset(double HorizontalOffset)
    {
        return SetHorizontalOffsetCore(HorizontalOffset);
    }
    /// <summary>
    /// Scrolls an adapter row into the virtual viewport.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <returns>True if the row exists in the current projection and scrolling succeeded or was not needed; otherwise, false.</returns>
    public bool ScrollToRow(int RowIndex)
    {
        int VisibleNodeIndex = fEngine.IndexOfVisibleRow(RowIndex);
        if (VisibleNodeIndex < 0)
            return false;

        if (fEngine.Viewport.IsEmpty)
        {
            fFirstVisibleNodeIndex = Math.Clamp(VisibleNodeIndex, 0, Math.Max(0, fEngine.VisibleNodeCount - 1));
            InvalidateVisual();
            return true;
        }

        if (VisibleNodeIndex >= fEngine.Viewport.FirstVisibleNodeIndex && VisibleNodeIndex <= fEngine.Viewport.LastVisibleNodeIndex)
            return true;

        return SetFirstVisibleNodeIndexCore(VisibleNodeIndex);
    }
    /// <summary>
    /// Scrolls the current cell into the virtual viewport.
    /// </summary>
    /// <returns>True if the viewport changed; otherwise, false.</returns>
    public bool ScrollCurrentCellIntoView()
    {
        return ScrollCurrentCellIntoViewCore();
    }

    // ● column query API
    /// <summary>
    /// Returns the visible columns in display order.
    /// </summary>
    /// <returns>The visible columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetVisibleColumns()
    {
        return fEngine.GetVisibleColumns();
    }
    /// <summary>
    /// Returns the visible value columns in display order.
    /// </summary>
    /// <returns>The visible value columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetVisibleValueColumns()
    {
        return fEngine.GetVisibleValueColumns();
    }
    /// <summary>
    /// Returns all columns in grid column order.
    /// </summary>
    /// <returns>A snapshot list with all columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetAllColumns()
    {
        return fEngine.Columns.ToList();
    }
    /// <summary>
    /// Returns visible value columns in the order they are displayed in the header band.
    /// </summary>
    /// <returns>A snapshot list with visible value columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetVisibleOrderedColumns()
    {
        return fEngine.GetVisibleValueColumns().ToList();
    }
    /// <summary>
    /// Returns hidden columns in grid column order.
    /// </summary>
    /// <returns>A snapshot list with hidden columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetHiddenColumns()
    {
        return fEngine.Columns.Where(Column => !Column.IsVisible).ToList();
    }
    /// <summary>
    /// Returns grouped columns in grouping order.
    /// </summary>
    /// <returns>A snapshot list with grouped columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetGroupedColumns()
    {
        return fEngine.GroupColumns.ToList();
    }

    // ● settings API
    /// <summary>
    /// Creates a serializable settings object from the current grid layout.
    /// </summary>
    /// <param name="Name">The settings name.</param>
    /// <returns>The grid settings.</returns>
    public GroupGridSettings CreateSettings(string Name = "Default")
    {
        GroupGridSettings Result = new()
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "Default" : Name,
            SortColumnName = fEngine.SortColumn == null ? string.Empty : fEngine.SortColumn.Name,
            SortDirection = fEngine.SortDirection,
            IsToolBarVisible = IsToolBarVisible,
            IsGroupPanelVisible = IsGroupPanelVisible,
            IsColumnHeadersVisible = IsColumnHeadersVisible,
            IsFilterPanelVisible = IsFilterPanelVisible,
            IsTotalsSummaryVisible = IsTotalsSummaryVisible,
            IsInsertButtonVisible = IsInsertButtonVisible,
            IsDeleteButtonVisible = IsDeleteButtonVisible,
            IsEditButtonVisible = IsEditButtonVisible,
        };

        int Index = 0;
        foreach (GroupGridColumn Column in fEngine.Columns)
        {
            Result.Columns.Add(new GroupGridColumnSettings
            {
                Name = Column.Name,
                Header = string.IsNullOrWhiteSpace(Column.Header) ? Column.Name : Column.Header,
                IsVisible = Column.IsVisible,
                VisibleIndex = Index++,
                Width = Math.Max(Column.MinWidth, Column.Width),
                GroupIndex = IndexOfGroupColumn(Column),
                FilterText = fEngine.GetColumnFilter(Column),
                GroupSummary = Column.GroupSummary,
                TotalSummary = Column.TotalSummary,
            });
        }

        return Result;
    }
    /// <summary>
    /// Saves the current grid settings to a JSON file.
    /// </summary>
    /// <param name="FilePath">The full file path where settings are saved.</param>
    /// <param name="Name">The settings name.</param>
    public void SaveSettings(string FilePath, string Name = "Default")
    {
        ValidateSettingsFilePath(FilePath);
        string DirectoryPath = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        string Json = JsonSerializer.Serialize(CreateSettings(Name), fSettingsJsonOptions);
        File.WriteAllText(FilePath, Json);
    }
    /// <summary>
    /// Loads grid settings from a JSON file and applies them to the current grid layout.
    /// </summary>
    /// <param name="FilePath">The full file path where settings are loaded from.</param>
    /// <returns>True if settings were loaded and applied; otherwise, false.</returns>
    public bool LoadSettings(string FilePath)
    {
        ValidateSettingsFilePath(FilePath);
        if (!File.Exists(FilePath))
            return false;

        string Json = File.ReadAllText(FilePath);
        GroupGridSettings Settings = JsonSerializer.Deserialize<GroupGridSettings>(Json, fSettingsJsonOptions);
        if (Settings == null)
            return false;

        ApplySettings(Settings);
        return true;
    }

    // ● export API
    /// <summary>
    /// Creates an export snapshot from the current visible grid projection.
    /// </summary>
    /// <returns>The export snapshot.</returns>
    public GroupGridExportSnapshot CreateExportSnapshot()
    {
        List<GroupGridExportColumn> Columns = fEngine.GetVisibleValueColumns()
            .Select(Column => new GroupGridExportColumn(Column))
            .ToList();
        Dictionary<GroupGridColumn, GroupGridExportColumn> ColumnMap = Columns.ToDictionary(Column => Column.Column);
        List<GroupGridExportRow> Rows = new();

        for (int Index = 0; Index < fEngine.VisibleNodeCount; Index++)
        {
            GroupGridRowInfo RowInfo = fEngine.GetVisibleRowInfo(Index);
            string GroupText = RowInfo.IsGroup ? fEngine.GetGroupHeaderText(Index) : string.Empty;
            List<GroupGridExportCell> Cells = new();

            if (RowInfo.IsDataRow || RowInfo.IsGroupSummary)
            {
                foreach (GroupGridExportColumn Column in Columns)
                {
                    object Value = RowInfo.IsDataRow
                        ? fEngine.GetValue(RowInfo.RowIndex, Column.Column)
                        : fEngine.GetGroupSummary(Index, Column.Column).Value;
                    string Text = fEngine.GetDisplayText(Index, Column.Column);
                    Cells.Add(new GroupGridExportCell(ColumnMap[Column.Column], Value, Text));
                }
            }

            Rows.Add(new GroupGridExportRow(RowInfo, GroupText, Cells));
        }

        List<GroupGridExportCell> TotalSummaryCells = Columns
            .Select(Column =>
            {
                GroupGridSummaryValue Summary = fEngine.GetTotalSummary(Column.Column);
                return new GroupGridExportCell(ColumnMap[Column.Column], Summary.Value, fEngine.GetTotalSummaryText(Column.Column));
            })
            .ToList();

        return new GroupGridExportSnapshot(Columns, Rows, TotalSummaryCells);
    }
    /// <summary>
    /// Exports the current grid using a specified exporter.
    /// </summary>
    /// <param name="Exporter">The exporter.</param>
    /// <param name="FilePath">The full export file path.</param>
    public void SaveExport(GroupGridExporter Exporter, string FilePath)
    {
        if (Exporter == null)
            throw new ArgumentNullException(nameof(Exporter));

        Exporter.Export(this, CreateExportSnapshot(), FilePath);
    }
    /// <summary>
    /// Applies a serializable settings object to the current grid layout.
    /// </summary>
    /// <param name="Settings">The grid settings.</param>
    public void ApplySettings(GroupGridSettings Settings)
    {
        if (Settings == null)
            return;

        CancelCellEdit();
        IsToolBarVisible = Settings.IsToolBarVisible;
        IsGroupPanelVisible = Settings.IsGroupPanelVisible;
        IsColumnHeadersVisible = Settings.IsColumnHeadersVisible;
        IsFilterPanelVisible = Settings.IsFilterPanelVisible;
        IsTotalsSummaryVisible = Settings.IsTotalsSummaryVisible;
        IsInsertButtonVisible = Settings.IsInsertButtonVisible;
        IsDeleteButtonVisible = Settings.IsDeleteButtonVisible;
        IsEditButtonVisible = Settings.IsEditButtonVisible;

        Dictionary<string, GroupGridColumn> ColumnMap = fEngine.Columns.ToDictionary(Column => Column.Name, StringComparer.OrdinalIgnoreCase);
        List<GroupGridColumnSettings> OrderedSettings = Settings.Columns
            .Where(Item => Item != null && ColumnMap.ContainsKey(Item.Name))
            .OrderBy(Item => Item.VisibleIndex)
            .ToList();

        for (int Index = 0; Index < OrderedSettings.Count; Index++)
        {
            GroupGridColumn Column = ColumnMap[OrderedSettings[Index].Name];
            int OldIndex = fEngine.Columns.IndexOf(Column);
            if (OldIndex >= 0 && OldIndex != Index)
                fEngine.Columns.Move(OldIndex, Index);
        }

        foreach (GroupGridColumnSettings Item in OrderedSettings)
        {
            GroupGridColumn Column = ColumnMap[Item.Name];
            if (Item.Width > 0)
                Column.Width = Math.Max(Column.MinWidth, Item.Width);

            fEngine.SetColumnVisible(Column, Item.IsVisible);
            if (string.IsNullOrWhiteSpace(Item.FilterText))
                fEngine.ClearColumnFilter(Column);
            else
                fEngine.SetColumnFilter(Column, Item.FilterText);

            Column.GroupSummary = Column.CanAggregate(Item.GroupSummary) ? Item.GroupSummary : GroupGridAggregateKind.None;
            Column.TotalSummary = Column.CanAggregate(Item.TotalSummary) ? Item.TotalSummary : GroupGridAggregateKind.None;
        }

        foreach (GroupGridColumn Column in fEngine.GroupColumns.ToList())
            fEngine.UngroupColumn(Column);
        foreach (GroupGridColumnSettings Item in OrderedSettings.Where(Item => Item.GroupIndex >= 0).OrderBy(Item => Item.GroupIndex))
        {
            GroupGridColumn Column = ColumnMap[Item.Name];
            if (Item.IsVisible && Column.CanUserGroup)
                fEngine.GroupColumn(Column);
        }

        fEngine.ClearSort();
        if (!string.IsNullOrWhiteSpace(Settings.SortColumnName)
            && Settings.SortDirection != GroupGridSortDirection.None
            && ColumnMap.TryGetValue(Settings.SortColumnName, out GroupGridColumn SortColumn))
        {
            fEngine.ToggleSort(SortColumn);
            if (Settings.SortDirection == GroupGridSortDirection.Descending)
                fEngine.ToggleSort(SortColumn);
        }

        fEngine.RebuildProjection();
        UpdateViewport(Bounds.Size);
        InvalidateVisual();
    }

    // ● toolbar API
    /// <summary>
    /// Returns a toolbar button by name.
    /// </summary>
    /// <param name="Name">The button name.</param>
    /// <returns>The toolbar button, or null when not found.</returns>
    public GroupGridToolButton GetToolButton(string Name)
    {
        return FindToolButton(Name);
    }
    /// <summary>
    /// Adds a toolbar button to the requested side.
    /// </summary>
    /// <param name="Button">The button to add.</param>
    /// <param name="Alignment">The toolbar side.</param>
    /// <returns>The added button.</returns>
    public GroupGridToolButton AddButton(GroupGridToolButton Button, GroupGridToolButtonAlignment Alignment = GroupGridToolButtonAlignment.Left)
    {
        PrepareToolButton(Button, Alignment);
        fToolButtons.Add(Button);
        return Button;
    }
    /// <summary>
    /// Creates and adds a toolbar button to the requested side.
    /// </summary>
    /// <param name="Alignment">The toolbar side.</param>
    /// <param name="Name">The button name.</param>
    /// <param name="Text">The compact display text.</param>
    /// <param name="ToolTip">The tooltip text.</param>
    /// <returns>The added button.</returns>
    public GroupGridToolButton AddButton(GroupGridToolButtonAlignment Alignment, string Name, string Text, string ToolTip = "")
    {
        return AddButton(new GroupGridToolButton { Name = Name, Text = Text, ToolTip = ToolTip }, Alignment);
    }
    /// <summary>
    /// Inserts a toolbar button before an existing button.
    /// </summary>
    /// <param name="ExistingButtonName">The existing button name.</param>
    /// <param name="Button">The button to insert.</param>
    /// <returns>True if the button was inserted; otherwise, false.</returns>
    public bool InsertButtonBefore(string ExistingButtonName, GroupGridToolButton Button)
    {
        int Index = IndexOfToolButton(ExistingButtonName);
        if (Index < 0 || Button == null)
            return false;

        PrepareToolButton(Button, fToolButtons[Index].Alignment);
        fToolButtons.Insert(Index, Button);
        return true;
    }
    /// <summary>
    /// Inserts a toolbar button after an existing button.
    /// </summary>
    /// <param name="ExistingButtonName">The existing button name.</param>
    /// <param name="Button">The button to insert.</param>
    /// <returns>True if the button was inserted; otherwise, false.</returns>
    public bool InsertButtonAfter(string ExistingButtonName, GroupGridToolButton Button)
    {
        int Index = IndexOfToolButton(ExistingButtonName);
        if (Index < 0 || Button == null)
            return false;

        PrepareToolButton(Button, fToolButtons[Index].Alignment);
        fToolButtons.Insert(Index + 1, Button);
        return true;
    }
    /// <summary>
    /// Creates and inserts a toolbar button before an existing button.
    /// </summary>
    /// <param name="ExistingButtonName">The existing button name.</param>
    /// <param name="Name">The button name.</param>
    /// <param name="Text">The compact display text.</param>
    /// <param name="ToolTip">The tooltip text.</param>
    /// <returns>The inserted button, or null when the existing button is not found.</returns>
    public GroupGridToolButton InsertButtonBefore(string ExistingButtonName, string Name, string Text, string ToolTip = "")
    {
        GroupGridToolButton Button = new() { Name = Name, Text = Text, ToolTip = ToolTip };
        return InsertButtonBefore(ExistingButtonName, Button) ? Button : null;
    }
    /// <summary>
    /// Creates and inserts a toolbar button after an existing button.
    /// </summary>
    /// <param name="ExistingButtonName">The existing button name.</param>
    /// <param name="Name">The button name.</param>
    /// <param name="Text">The compact display text.</param>
    /// <param name="ToolTip">The tooltip text.</param>
    /// <returns>The inserted button, or null when the existing button is not found.</returns>
    public GroupGridToolButton InsertButtonAfter(string ExistingButtonName, string Name, string Text, string ToolTip = "")
    {
        GroupGridToolButton Button = new() { Name = Name, Text = Text, ToolTip = ToolTip };
        return InsertButtonAfter(ExistingButtonName, Button) ? Button : null;
    }

    // ● column grouping and layout API
    /// <summary>
    /// Adds a column to the grouping list.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="GroupIndex">The group index, or -1 to append.</param>
    /// <returns>True if the column was grouped; otherwise, false.</returns>
    public bool GroupColumn(GroupGridColumn Column, int GroupIndex = -1)
    {
        CancelCellEdit();
        return fEngine.GroupColumn(Column, GroupIndex);
    }
    /// <summary>
    /// Removes a column from the grouping list.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the column was ungrouped; otherwise, false.</returns>
    public bool UngroupColumn(GroupGridColumn Column)
    {
        CancelCellEdit();
        return fEngine.UngroupColumn(Column);
    }
    /// <summary>
    /// Moves a grouped column to a new group index.
    /// </summary>
    /// <param name="Column">The grouped column.</param>
    /// <param name="GroupIndex">The new group index.</param>
    /// <returns>True if the column moved; otherwise, false.</returns>
    public bool MoveGroupedColumn(GroupGridColumn Column, int GroupIndex)
    {
        CancelCellEdit();
        return fEngine.MoveGroupedColumn(Column, GroupIndex);
    }
    /// <summary>
    /// Moves a column to a new display index in the column collection.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="ColumnIndex">The new column index.</param>
    /// <returns>True if the column moved; otherwise, false.</returns>
    public bool MoveColumn(GroupGridColumn Column, int ColumnIndex)
    {
        CancelCellEdit();
        return fEngine.MoveColumn(Column, ColumnIndex);
    }
    /// <summary>
    /// Resizes a visible value column to fit its header, filter text, and projected data row text.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="MaxRows">The maximum number of projected data rows to measure. Use zero or a negative value to measure all rows.</param>
    /// <returns>True if the column width changed; otherwise, false.</returns>
    public bool BestFitColumn(GroupGridColumn Column, int MaxRows = 2500)
    {
        if (Column == null || !fEngine.GetVisibleValueColumns().Contains(Column))
            return false;

        return SetColumnWidth(Column, GetBestFitColumnWidth(Column, MaxRows));
    }
    /// <summary>
    /// Resizes a visible value column to fit its header, filter text, and projected data row text.
    /// </summary>
    /// <param name="ColumnName">The column name.</param>
    /// <param name="MaxRows">The maximum number of projected data rows to measure. Use zero or a negative value to measure all rows.</param>
    /// <returns>True if the column was found and its width changed; otherwise, false.</returns>
    public bool BestFitColumn(string ColumnName, int MaxRows = 2500)
    {
        return BestFitColumn(FindColumn(ColumnName), MaxRows);
    }
    /// <summary>
    /// Resizes all visible value columns to fit their headers, filter text, and projected data row text.
    /// </summary>
    /// <param name="MaxRows">The maximum number of projected data rows to measure per column. Use zero or a negative value to measure all rows.</param>
    /// <returns>The number of columns whose width changed.</returns>
    public int BestFitColumns(int MaxRows = 2500)
    {
        int Result = 0;
        foreach (GroupGridColumn Column in fEngine.GetVisibleValueColumns())
            if (BestFitColumn(Column, MaxRows))
                Result++;

        return Result;
    }
    /// <summary>
    /// Shows or hides a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="IsVisible">True to show the column; false to hide it.</param>
    /// <returns>True if the column visibility changed; otherwise, false.</returns>
    public bool SetColumnVisible(GroupGridColumn Column, bool IsVisible)
    {
        CancelCellEdit();
        return fEngine.SetColumnVisible(Column, IsVisible);
    }
    /// <summary>
    /// Shows or hides a column by field name.
    /// </summary>
    /// <param name="FieldName">The column field name.</param>
    /// <param name="IsVisible">True to show the column; false to hide it.</param>
    /// <returns>True if the column visibility changed; otherwise, false.</returns>
    public bool SetColumnVisible(string FieldName, bool IsVisible)
    {
        return SetColumnVisible(FindColumn(FieldName), IsVisible);
    }
    /// <summary>
    /// Shows or hides columns by field name.
    /// </summary>
    /// <param name="FieldNames">The column field names.</param>
    /// <param name="IsVisible">True to show the columns; false to hide them.</param>
    /// <returns>True if any column visibility changed; otherwise, false.</returns>
    public bool SetColumnsVisible(string[] FieldNames, bool IsVisible)
    {
        bool Result = false;
        foreach (string FieldName in FieldNames ?? Array.Empty<string>())
            Result = SetColumnVisible(FieldName, IsVisible) || Result;

        return Result;
    }
    /// <summary>
    /// Sets a column read-only state by field name.
    /// </summary>
    /// <param name="FieldName">The column field name.</param>
    /// <param name="IsReadOnly">True to make the column read-only; false to make it editable.</param>
    /// <returns>True if the column read-only state changed; otherwise, false.</returns>
    public bool SetColumnReadOnly(string FieldName, bool IsReadOnly)
    {
        GroupGridColumn Column = FindColumn(FieldName);
        if (Column == null || Column.IsReadOnly == IsReadOnly)
            return false;

        CancelCellEdit();
        Column.IsReadOnly = IsReadOnly;
        InvalidateVisual();
        return true;
    }
    /// <summary>
    /// Sets columns read-only state by field name.
    /// </summary>
    /// <param name="FieldNames">The column field names.</param>
    /// <param name="IsReadOnly">True to make the columns read-only; false to make them editable.</param>
    /// <returns>True if any column read-only state changed; otherwise, false.</returns>
    public bool SetColumnsReadOnly(string[] FieldNames, bool IsReadOnly)
    {
        bool Result = false;
        foreach (string FieldName in FieldNames ?? Array.Empty<string>())
            Result = SetColumnReadOnly(FieldName, IsReadOnly) || Result;

        return Result;
    }

    // ● sorting and filtering API
    /// <summary>
    /// Clears the active column sorting.
    /// </summary>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ClearSort()
    {
        CancelCellEdit();
        return fEngine.ClearSort();
    }
    /// <summary>
    /// Toggles sorting for a column using the None, Ascending, Descending cycle.
    /// </summary>
    /// <param name="Column">The column to sort by.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ToggleSort(GroupGridColumn Column)
    {
        CancelCellEdit();
        return fEngine.ToggleSort(Column);
    }
    /// <summary>
    /// Returns the text filter applied to a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The filter text, or an empty string when no filter is applied.</returns>
    public string GetColumnFilter(GroupGridColumn Column)
    {
        return fEngine.GetColumnFilter(Column);
    }
    /// <summary>
    /// Sets the contains-text filter for a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="FilterText">The filter text.</param>
    /// <returns>True if the filter changed; otherwise, false.</returns>
    public bool SetColumnFilter(GroupGridColumn Column, string FilterText)
    {
        CancelCellEdit();
        return fEngine.SetColumnFilter(Column, FilterText);
    }
    /// <summary>
    /// Clears the text filter for a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the filter changed; otherwise, false.</returns>
    public bool ClearColumnFilter(GroupGridColumn Column)
    {
        CancelCellEdit();
        return fEngine.ClearColumnFilter(Column);
    }
    /// <summary>
    /// Clears all column filters.
    /// </summary>
    /// <returns>True if any filter was cleared; otherwise, false.</returns>
    public bool ClearFilters()
    {
        CancelCellEdit();
        return fEngine.ClearFilters();
    }

    // ● row command API
    /// <summary>
    /// Returns true when a new row can be inserted after the current row.
    /// </summary>
    /// <returns>True when a row can be inserted; otherwise, false.</returns>
    public bool CanInsertRow()
    {
        return fEngine.CanInsertRow();
    }
    /// <summary>
    /// Inserts a new row after the current row.
    /// </summary>
    /// <returns>True if a row was inserted; otherwise, false.</returns>
    public bool InsertRow()
    {
        CancelCellEdit();
        bool Result = fEngine.InsertRow();
        if (Result)
            ScrollCurrentCellIntoViewCore();
        return Result;
    }
    /// <summary>
    /// Returns true when the current row can be deleted.
    /// </summary>
    /// <returns>True when the current row can be deleted; otherwise, false.</returns>
    public bool CanDeleteCurrentRow()
    {
        return fEngine.CanDeleteCurrentRow();
    }
    /// <summary>
    /// Deletes the current row.
    /// </summary>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public bool DeleteCurrentRow()
    {
        CancelCellEdit();
        bool Result = fEngine.DeleteCurrentRow();
        if (Result)
            ScrollCurrentCellIntoViewCore();
        return Result;
    }
    /// <summary>
    /// Raises the edit requested event.
    /// </summary>
    public void RequestEdit()
    {
        fEngine.RequestEdit();
    }

    // ● group expansion API
    /// <summary>
    /// Sets the expanded state of a group node by visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <param name="IsExpanded">True to expand; false to collapse.</param>
    /// <returns>True if the group state changed; otherwise, false.</returns>
    public bool SetGroupExpanded(int VisibleNodeIndex, bool IsExpanded)
    {
        CancelCellEdit();
        return fEngine.SetGroupExpanded(VisibleNodeIndex, IsExpanded);
    }
    /// <summary>
    /// Toggles the expanded state of a group node by visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>True if the group state changed; otherwise, false.</returns>
    public bool ToggleGroupExpanded(int VisibleNodeIndex)
    {
        CancelCellEdit();
        return fEngine.ToggleGroupExpanded(VisibleNodeIndex);
    }

    // ● current cell and selection API
    /// <summary>
    /// Clears the current cell.
    /// </summary>
    public void ClearCurrentCell()
    {
        CancelCellEdit();
        fEngine.ClearCurrentCell();
    }
    /// <summary>
    /// Sets the current cell.
    /// </summary>
    /// <param name="Cell">The current cell.</param>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool SetCurrentCell(GroupGridCell Cell)
    {
        CancelCellEdit();
        bool Result = fEngine.SetCurrentCell(Cell);
        ScrollCurrentCellIntoViewCore();
        return Result;
    }
    /// <summary>
    /// Sets the current cell by row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool SetCurrentCell(int RowIndex, GroupGridColumn Column) => SetCurrentCell(new GroupGridCell(RowIndex, Column));
    /// <summary>
    /// Clears the selected cell.
    /// </summary>
    public void ClearSelection()
    {
        fEngine.ClearSelection();
    }
    /// <summary>
    /// Sets the selected cell.
    /// </summary>
    /// <param name="Cell">The selected cell.</param>
    /// <returns>True if the selected cell changed; otherwise, false.</returns>
    public bool SetSelectedCell(GroupGridCell Cell)
    {
        return fEngine.SetSelectedCell(Cell);
    }
    /// <summary>
    /// Selects the current cell.
    /// </summary>
    /// <returns>True if the selected cell changed; otherwise, false.</returns>
    public bool SelectCurrentCell()
    {
        return fEngine.SelectCurrentCell();
    }
    /// <summary>
    /// Performs a control-level hit test using visible bands, scrollbars, fixed areas, and horizontal scroll offset.
    /// </summary>
    /// <param name="Point">The point in grid coordinates.</param>
    /// <returns>The hit-test result.</returns>
    public GroupGridHitTestResult HitTest(Point Point)
    {
        return HitTestCore(Point);
    }

    // ● editing API
    /// <summary>
    /// Begins editing the current cell.
    /// </summary>
    /// <returns>True if editing started; otherwise, false.</returns>
    public bool BeginEdit()
    {
        return BeginCellEdit(false, null);
    }
    /// <summary>
    /// Commits the edited value.
    /// </summary>
    /// <param name="Value">The editor value.</param>
    /// <returns>True if the edit was committed; otherwise, false.</returns>
    public bool CommitEdit(object Value)
    {
        bool Result = fEngine.CommitEdit(NormalizeEditorValue(fEngine.EditingCell, Value));
        if (Result)
        {
            fCellEditText = string.Empty;
            CloseActiveEditor();
        }
        return Result;
    }
    /// <summary>
    /// Cancels the current edit.
    /// </summary>
    /// <returns>True if an edit was canceled; otherwise, false.</returns>
    public bool CancelEdit()
    {
        return CancelCellEdit();
    }

    // ● navigation API
    /// <summary>
    /// Moves the current cell by visible row and column deltas.
    /// </summary>
    /// <param name="RowDelta">The visible row delta.</param>
    /// <param name="ColumnDelta">The visible column delta.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentCell(int RowDelta, int ColumnDelta)
    {
        CancelCellEdit();
        bool Result = fEngine.MoveCurrentCell(RowDelta, ColumnDelta);
        ScrollCurrentCellIntoViewCore();
        return Result;
    }

    // ● rendering API
    /// <inheritdoc />
    public override void Render(DrawingContext Context)
    {
        base.Render(Context);

        Rect BoundsRect = new(0, 0, Bounds.Width, Bounds.Height);
        DrawBand(Context, BoundsRect, GridBackgroundBrush);

        if (fEngine == null)
            return;

        double ContentWidth = GetBodyContentWidth();
        double Y = 0;
        if (fIsToolBarVisible)
        {
            DrawToolBar(Context, new Rect(0, Y, Bounds.Width, fEngine.LayoutMetrics.ToolBarHeight));
            Y += fEngine.LayoutMetrics.ToolBarHeight;
        }

        if (fIsGroupPanelVisible)
        {
            DrawGroupPanel(Context, Y, fEngine.LayoutMetrics.GroupPanelHeight, ContentWidth);
            if (HasVerticalScrollBar())
                DrawBand(Context, new Rect(ContentWidth, Y, Bounds.Width - ContentWidth, fEngine.LayoutMetrics.GroupPanelHeight), GroupPanelBrush);
            Y += fEngine.LayoutMetrics.GroupPanelHeight;
        }

        if (fIsColumnHeadersVisible)
        {
            DrawColumns(Context, Y, fEngine.LayoutMetrics.ColumnHeaderHeight, ContentWidth, HeaderBrush, false);
            if (HasVerticalScrollBar())
                DrawBand(Context, new Rect(ContentWidth, Y, Bounds.Width - ContentWidth, fEngine.LayoutMetrics.ColumnHeaderHeight), HeaderBrush);
            Y += fEngine.LayoutMetrics.ColumnHeaderHeight;
        }

        if (fIsFilterPanelVisible)
        {
            DrawColumns(Context, Y, fEngine.LayoutMetrics.FilterRowHeight, ContentWidth, FilterBrush, true);
            if (HasVerticalScrollBar())
                DrawBand(Context, new Rect(ContentWidth, Y, Bounds.Width - ContentWidth, fEngine.LayoutMetrics.FilterRowHeight), FilterBrush);
            Y += fEngine.LayoutMetrics.FilterRowHeight;
        }

        double BodyHeight = GetBodyHeight();
        DrawBody(Context, Y, ContentWidth);
        DrawVerticalScrollBar(Context);
        Y += BodyHeight;

        if (fIsTotalsSummaryVisible)
        {
            DrawFooter(Context, Y, fEngine.LayoutMetrics.FooterSummaryHeight, ContentWidth);
            if (HasVerticalScrollBar())
                DrawBand(Context, new Rect(ContentWidth, Y, Bounds.Width - ContentWidth, fEngine.LayoutMetrics.FooterSummaryHeight), FooterBrush);
            Y += fEngine.LayoutMetrics.FooterSummaryHeight;
        }

        DrawHorizontalScrollBar(Context);
        DrawColumnResizeGuide(Context);
        DrawColumnDragGuide(Context);
        DrawColumnDragGhost(Context);
    }

    // ● properties
    // ● core properties
    /// <summary>
    /// Gets or sets the group grid engine rendered by this control.
    /// </summary>
    public GroupGridEngine Engine
    {
        get => fEngine;
        set
        {
            value ??= new GroupGridEngine();

            if (ReferenceEquals(fEngine, value))
                return;

            DetachEngine(fEngine);
            fEngine = value;
            AttachEngine(fEngine);
            ApplyIdColumnsVisibility();
            InvalidateMeasure();
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets the grid columns.
    /// </summary>
    public ObservableCollection<GroupGridColumn> Columns => fEngine.Columns;
    /// <summary>
    /// Gets the toolbar buttons. The built-in buttons are named Insert, Delete, and Edit.
    /// </summary>
    public ObservableCollection<GroupGridToolButton> ToolButtons => fToolButtons;
    /// <summary>
    /// Gets the grouped columns in grouping order.
    /// </summary>
    public IReadOnlyList<GroupGridColumn> GroupColumns => fEngine.GroupColumns;

    // ● data properties
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IGroupGridDataAdapter DataAdapter
    {
        get => fEngine.DataAdapter;
        set
        {
            if (fOwnedDataAdapter != null && !ReferenceEquals(fOwnedDataAdapter, value))
            {
                fOwnedDataAdapter.Dispose();
                fOwnedDataAdapter = null;
                fItemsSource = null;
            }

            fEngine.DataAdapter = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether editing is disabled for the whole grid.
    /// </summary>
    public bool IsReadOnly
    {
        get => fEngine.IsReadOnly;
        set => fEngine.IsReadOnly = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the column manager menu item is visible in the column context menu.
    /// </summary>
    public bool IsColumnManagerMenuItemVisible
    {
        get => fIsColumnManagerMenuItemVisible;
        set => fIsColumnManagerMenuItemVisible = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the save/load settings menu items are visible in the column context menu.
    /// </summary>
    public bool IsSettingsMenuItemsVisible
    {
        get => fIsSettingsMenuItemsVisible;
        set => fIsSettingsMenuItemsVisible = value;
    }
    /// <summary>
    /// Gets or sets how in-place editor drop-down controls are hosted.
    /// </summary>
    public GroupGridDropDownPlacementMode DropDownPlacementMode
    {
        get => fDropDownPlacementMode;
        set => fDropDownPlacementMode = value;
    }
    /// <summary>
    /// Gets or sets the suggested file name used by the built-in save settings file picker.
    /// </summary>
    public string SettingsSuggestedFileName
    {
        get => fSettingsSuggestedFileName;
        set => fSettingsSuggestedFileName = value;
    }
    /// <summary>
    /// Gets or sets the text displayed in the group panel when no columns are grouped.
    /// </summary>
    public string EmptyGroupPanelText
    {
        get => fEmptyGroupPanelText;
        set => fEmptyGroupPanelText = value;
    }
    /// <summary>
    /// Gets or sets an item source that is a <see cref="DataTable"/>, <see cref="DataView"/>, or implements <see cref="IList{T}"/>.
    /// When a <see cref="DataTable"/> is assigned, its <see cref="DataTable.DefaultView"/> is used.
    /// </summary>
    public object ItemsSource
    {
        get => fItemsSource;
        set => SetItemsSource(value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether columns are generated from public item properties or data-view columns when columns are empty.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get => fAutoGenerateColumns;
        set
        {
            if (fAutoGenerateColumns == value)
                return;

            fAutoGenerateColumns = value;
            GenerateColumnsFromItemsSource(fItemsSource);
        }
    }
    /// <summary>
    /// Gets the layout metrics used by the grid.
    /// </summary>
    public GroupGridLayoutMetrics LayoutMetrics => fEngine.LayoutMetrics;

    // ● theme properties
    /// <summary>
    /// Gets or sets the grid body background brush.
    /// </summary>
    public IBrush GridBackgroundBrush { get => GetValue(GridBackgroundBrushProperty); set => SetValue(GridBackgroundBrushProperty, value); }
    /// <summary>
    /// Gets or sets the toolbar background brush.
    /// </summary>
    public IBrush ToolBarBrush { get => GetValue(ToolBarBrushProperty); set => SetValue(ToolBarBrushProperty, value); }
    /// <summary>
    /// Gets or sets the toolbar button background brush.
    /// </summary>
    public IBrush ToolButtonBrush { get => GetValue(ToolButtonBrushProperty); set => SetValue(ToolButtonBrushProperty, value); }
    /// <summary>
    /// Gets or sets the toolbar button hover background brush.
    /// </summary>
    public IBrush ToolButtonHoverBrush { get => GetValue(ToolButtonHoverBrushProperty); set => SetValue(ToolButtonHoverBrushProperty, value); }
    /// <summary>
    /// Gets or sets the disabled text brush.
    /// </summary>
    public IBrush DisabledTextBrush { get => GetValue(DisabledTextBrushProperty); set => SetValue(DisabledTextBrushProperty, value); }
    /// <summary>
    /// Gets or sets the group panel background brush.
    /// </summary>
    public IBrush GroupPanelBrush { get => GetValue(GroupPanelBrushProperty); set => SetValue(GroupPanelBrushProperty, value); }
    /// <summary>
    /// Gets or sets the column header background brush.
    /// </summary>
    public IBrush HeaderBrush { get => GetValue(HeaderBrushProperty); set => SetValue(HeaderBrushProperty, value); }
    /// <summary>
    /// Gets or sets the filter row background brush.
    /// </summary>
    public IBrush FilterBrush { get => GetValue(FilterBrushProperty); set => SetValue(FilterBrushProperty, value); }
    /// <summary>
    /// Gets or sets the filtered cell background brush.
    /// </summary>
    public IBrush FilteredCellBrush { get => GetValue(FilteredCellBrushProperty); set => SetValue(FilteredCellBrushProperty, value); }
    /// <summary>
    /// Gets or sets the active filter cell background brush.
    /// </summary>
    public IBrush ActiveFilterBrush { get => GetValue(ActiveFilterBrushProperty); set => SetValue(ActiveFilterBrushProperty, value); }
    /// <summary>
    /// Gets or sets the group row background brush.
    /// </summary>
    public IBrush GroupRowBrush { get => GetValue(GroupRowBrushProperty); set => SetValue(GroupRowBrushProperty, value); }
    /// <summary>
    /// Gets or sets the group summary row background brush.
    /// </summary>
    public IBrush GroupSummaryBrush { get => GetValue(GroupSummaryBrushProperty); set => SetValue(GroupSummaryBrushProperty, value); }
    /// <summary>
    /// Gets or sets the selected row or cell background brush.
    /// </summary>
    public IBrush SelectedBrush { get => GetValue(SelectedBrushProperty); set => SetValue(SelectedBrushProperty, value); }
    /// <summary>
    /// Gets or sets the current cell background brush.
    /// </summary>
    public IBrush CurrentBrush { get => GetValue(CurrentBrushProperty); set => SetValue(CurrentBrushProperty, value); }
    /// <summary>
    /// Gets or sets the editing cell background brush.
    /// </summary>
    public IBrush EditingBrush { get => GetValue(EditingBrushProperty); set => SetValue(EditingBrushProperty, value); }
    /// <summary>
    /// Gets or sets the footer summary background brush.
    /// </summary>
    public IBrush FooterBrush { get => GetValue(FooterBrushProperty); set => SetValue(FooterBrushProperty, value); }
    /// <summary>
    /// Gets or sets the primary text brush.
    /// </summary>
    public IBrush TextBrush { get => GetValue(TextBrushProperty); set => SetValue(TextBrushProperty, value); }
    /// <summary>
    /// Gets or sets the muted text brush.
    /// </summary>
    public IBrush MutedTextBrush { get => GetValue(MutedTextBrushProperty); set => SetValue(MutedTextBrushProperty, value); }
    /// <summary>
    /// Gets or sets the scrollbar track brush.
    /// </summary>
    public IBrush ScrollBarTrackBrush { get => GetValue(ScrollBarTrackBrushProperty); set => SetValue(ScrollBarTrackBrushProperty, value); }
    /// <summary>
    /// Gets or sets the scrollbar thumb brush.
    /// </summary>
    public IBrush ScrollBarThumbBrush { get => GetValue(ScrollBarThumbBrushProperty); set => SetValue(ScrollBarThumbBrushProperty, value); }
    /// <summary>
    /// Gets or sets the column drag ghost brush.
    /// </summary>
    public IBrush ColumnDragBrush { get => GetValue(ColumnDragBrushProperty); set => SetValue(ColumnDragBrushProperty, value); }
    /// <summary>
    /// Gets or sets the grid line brush.
    /// </summary>
    public IBrush GridLineBrush { get => GetValue(GridLineBrushProperty); set => SetValue(GridLineBrushProperty, value); }
    /// <summary>
    /// Gets or sets the current cell border brush.
    /// </summary>
    public IBrush CurrentBorderBrush { get => GetValue(CurrentBorderBrushProperty); set => SetValue(CurrentBorderBrushProperty, value); }
    /// <summary>
    /// Gets or sets the editing cell border brush.
    /// </summary>
    public IBrush EditingBorderBrush { get => GetValue(EditingBorderBrushProperty); set => SetValue(EditingBorderBrushProperty, value); }
    /// <summary>
    /// Gets or sets the resize and drop guide brush.
    /// </summary>
    public IBrush ResizeGuideBrush { get => GetValue(ResizeGuideBrushProperty); set => SetValue(ResizeGuideBrushProperty, value); }

    // ● visibility properties
    /// <summary>
    /// Gets or sets a value indicating whether the toolbar band is visible.
    /// </summary>
    public bool IsToolBarVisible
    {
        get => fIsToolBarVisible;
        set => SetBandVisible(ref fIsToolBarVisible, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether the filter panel band is visible.
    /// </summary>
    public bool IsFilterPanelVisible
    {
        get => fIsFilterPanelVisible;
        set => SetBandVisible(ref fIsFilterPanelVisible, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether column headers are visible.
    /// </summary>
    public bool IsColumnHeadersVisible
    {
        get => fIsColumnHeadersVisible;
        set => SetBandVisible(ref fIsColumnHeadersVisible, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether the group panel band is visible.
    /// </summary>
    public bool IsGroupPanelVisible
    {
        get => fIsGroupPanelVisible;
        set => SetBandVisible(ref fIsGroupPanelVisible, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether the totals summary band is visible.
    /// </summary>
    public bool IsTotalsSummaryVisible
    {
        get => fIsTotalsSummaryVisible;
        set => SetBandVisible(ref fIsTotalsSummaryVisible, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether columns named Id or ending with Id are visible.
    /// </summary>
    public bool AreIdColumnsVisible
    {
        get => fAreIdColumnsVisible;
        set
        {
            if (fAreIdColumnsVisible == value)
                return;

            fAreIdColumnsVisible = value;
            ApplyIdColumnsVisibility();
            InvalidateVisual();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the default insert toolbar button is visible.
    /// </summary>
    public bool IsInsertButtonVisible
    {
        get => GetDefaultToolButtonVisible(InsertToolButtonName);
        set => SetDefaultToolButtonVisible(InsertToolButtonName, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether the default delete toolbar button is visible.
    /// </summary>
    public bool IsDeleteButtonVisible
    {
        get => GetDefaultToolButtonVisible(DeleteToolButtonName);
        set => SetDefaultToolButtonVisible(DeleteToolButtonName, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether the default edit toolbar button is visible.
    /// </summary>
    public bool IsEditButtonVisible
    {
        get => GetDefaultToolButtonVisible(EditToolButtonName);
        set => SetDefaultToolButtonVisible(EditToolButtonName, value);
    }

    // ● state properties
    /// <summary>
    /// Gets the current cell.
    /// </summary>
    public GroupGridCell CurrentCell => fEngine.CurrentCell;
    /// <summary>
    /// Gets the current adapter row index, or -1 when there is no current row.
    /// </summary>
    public int CurrentRowIndex => CurrentCell.IsEmpty ? -1 : CurrentCell.RowIndex;
    /// <summary>
    /// Gets the current adapter row object, or null when there is no current row.
    /// </summary>
    public object CurrentRow
    {
        get
        {
            return GetAdapterRow(CurrentRowIndex);
        }
    }
    /// <summary>
    /// Gets the selected cell.
    /// </summary>
    public GroupGridCell SelectedCell => fEngine.SelectedCell;
    /// <summary>
    /// Gets the editing cell.
    /// </summary>
    public GroupGridCell EditingCell => fEngine.EditingCell;
    /// <summary>
    /// Gets the sorted column, or null when sorting is not active.
    /// </summary>
    public GroupGridColumn SortColumn => fEngine.SortColumn;
    /// <summary>
    /// Gets the active sort direction.
    /// </summary>
    public GroupGridSortDirection SortDirection => fEngine.SortDirection;
    /// <summary>
    /// Gets the number of active column filters.
    /// </summary>
    public int FilterCount => fEngine.FilterCount;
    /// <summary>
    /// Gets a value indicating whether any column filter is active.
    /// </summary>
    public bool HasFilters => fEngine.HasFilters;
    /// <summary>
    /// Gets a value indicating whether the grid has a current cell.
    /// </summary>
    public bool HasCurrentCell => fEngine.HasCurrentCell;
    /// <summary>
    /// Gets a value indicating whether the grid has a selected cell.
    /// </summary>
    public bool HasSelectedCell => fEngine.HasSelectedCell;
    /// <summary>
    /// Gets a value indicating whether the grid is editing a cell.
    /// </summary>
    public bool IsEditing => fEngine.IsEditing;
    /// <summary>
    /// Gets the last arranged editor rectangle used for drop-down geometry diagnostics.
    /// </summary>
    public Rect LastEditorRect => fLastEditorRect;
    /// <summary>
    /// Gets the last arranged drop-down host rectangle used for drop-down geometry diagnostics.
    /// </summary>
    public Rect LastDropDownRect => fLastDropDownRect;

    // ● viewport state properties
    /// <summary>
    /// Gets the viewport window into the logical visible-node list.
    /// </summary>
    public GroupGridViewport Viewport => fEngine.Viewport;
    /// <summary>
    /// Gets the number of visible projected nodes.
    /// </summary>
    public int VisibleNodeCount => fEngine.VisibleNodeCount;
    /// <summary>
    /// Gets the number of adapter rows.
    /// </summary>
    public int RowCount => fEngine.RowCount;
    /// <summary>
    /// Gets the first visible node index in the virtual viewport.
    /// </summary>
    public int FirstVisibleNodeIndex => fFirstVisibleNodeIndex;
    /// <summary>
    /// Gets the horizontal scroll offset in pixels.
    /// </summary>
    public double HorizontalOffset => fHorizontalOffset;
    /// <summary>
    /// Gets the total height of the fixed non-body bands.
    /// </summary>
    public double FixedBandHeight
    {
        get
        {
            if (fEngine == null)
                return 0;

            return GetBodyFixedTopHeight() + GetTotalsSummaryHeight();
        }
    }

    // ● events
    // ● state events
    /// <summary>
    /// Occurs when the current adapter row changes.
    /// </summary>
    public event EventHandler CurrentRowChanged;

    // ● editing events
    /// <summary>
    /// Occurs before cell editing begins. Set Cancel to true to stop editing.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> BeginningEdit;
    /// <summary>
    /// Occurs before an edited value is committed. Set Cancel to true to keep editing active.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValidating;
    /// <summary>
    /// Occurs after validation and before the edited value is written to the adapter.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValueCommitting;
    /// <summary>
    /// Occurs after the edited value is written to the adapter.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValueCommitted;
    /// <summary>
    /// Occurs when cell editing is canceled.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> EditCanceled;
    /// <summary>
    /// Occurs when a pointer press targets a data body cell.
    /// </summary>
    public event EventHandler<GroupGridCellPointerEventArgs> CellPointerPressed;

    // ● command events
    /// <summary>
    /// Occurs when the column manager menu item is clicked.
    /// </summary>
    public event EventHandler ColumnManagerRequested;
    /// <summary>
    /// Occurs when a custom toolbar button is clicked.
    /// </summary>
    public event EventHandler<GroupGridToolButtonEventArgs> ToolButtonClicked;

    // ● row operation events
    /// <summary>
    /// Occurs before a row is inserted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> InsertingRow;
    /// <summary>
    /// Occurs after a row is inserted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> RowInserted;
    /// <summary>
    /// Occurs before a row is deleted. Set Cancel to true to stop the deletion.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> DeletingRow;
    /// <summary>
    /// Occurs after a row is deleted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> RowDeleted;
    /// <summary>
    /// Occurs when the edit toolbar command is requested.
    /// </summary>
    public event EventHandler EditRequested;

    // ● editing extension events
    /// <summary>
    /// Occurs when the grid needs an in-place editor for a cell.
    /// </summary>
    public event EventHandler<GroupGridCreateInplaceEditorEventArgs> CreateInplaceEditor;
    /// <summary>
    /// Occurs when the grid needs to normalize date editor text before committing a value.
    /// </summary>
    public event EventHandler<GroupGridDateNormalizeEventArgs> DateNormalize;
}

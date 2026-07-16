/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Handles a tab control which displays forms (UserControls) embedded in TabItems.
/// </summary>
public class AppFormPagerHandler
{
    // ● private fields
    /// <summary>
    /// The tab page currently being dragged.
    /// </summary>
    TabItem fDraggedTabPage;
    /// <summary>
    /// The tab drag start position.
    /// </summary>
    Point fDragStartPoint;
    /// <summary>
    /// True while the user is dragging a tab page.
    /// </summary>
    bool fIsDraggingTabPage;
    /// <summary>
    /// The original opacity of the dragged tab page.
    /// </summary>
    double fDraggedTabPageOpacity = 1;
    /// <summary>
    /// The tab page currently marked as drop target.
    /// </summary>
    TabItem fDropTargetTabPage;
    /// <summary>
    /// The target insertion index during a tab drag operation.
    /// </summary>
    int fDropTargetIndex = -1;
    /// <summary>
    /// The popup used as a tab drop marker.
    /// </summary>
    Popup fTabDropMarker;
    /// <summary>
    /// Field for the MaxAllowedTabPages property.
    /// </summary>
    int fMaxAllowedTabPages = -1;

    // ● private
    /// <summary>
    /// Creates a menu item.
    /// </summary>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The menu item action.</param>
    /// <returns>The created menu item.</returns>
    MenuItem CreateMenuItem(string Header, Action Action)
    {
        MenuItem Result = new MenuItem { Header = Header };
        Result.Click += (Sender, Args) => Action();
        return Result;
    }
    /// <summary>
    /// Creates a tab header context menu.
    /// </summary>
    /// <param name="TabPage">The tab page.</param>
    /// <returns>The created context menu.</returns>
    ContextMenu CreateTabHeaderContextMenu(TabItem TabPage)
    {
        ContextMenu Result = new ContextMenu();
        Result.Items.Add(CreateMenuItem("Close", () => CloseTab(TabPage)));
        Result.Items.Add(CreateMenuItem("Close Others", () => CloseOtherTabs(TabPage)));
        Result.Items.Add(CreateMenuItem("Close All", CloseAllTabs));
        Result.Items.Add(CreateMenuItem("Close All Right", () => CloseTabsRight(TabPage)));
        return Result;
    }
    /// <summary>
    /// Opens the tab header context menu.
    /// </summary>
    /// <param name="TabPage">The tab page.</param>
    void OpenTabHeaderContextMenu(TabItem TabPage)
    {
        if (TabPage == null)
            return;

        TabPage.ContextMenu = CreateTabHeaderContextMenu(TabPage);
        TabPage.ContextMenu.Open(TabPage);
    }
    /// <summary>
    /// Closes a tab page.
    /// </summary>
    /// <param name="TabPage">The tab page.</param>
    void CloseTab(TabItem TabPage)
    {
        AppForm Form = GetForm(TabPage);
        if (Form != null)
            Form.CloseForm();
    }
    /// <summary>
    /// Closes all tab pages except a specified tab page.
    /// </summary>
    /// <param name="TabPage">The tab page to keep open.</param>
    void CloseOtherTabs(TabItem TabPage)
    {
        foreach (TabItem Item in Pager.Items.Cast<TabItem>().ToList())
        {
            if (!ReferenceEquals(Item, TabPage))
                CloseTab(Item);
        }
    }
    /// <summary>
    /// Closes all tab pages.
    /// </summary>
    void CloseAllTabs()
    {
        foreach (TabItem Item in Pager.Items.Cast<TabItem>().ToList())
            CloseTab(Item);
    }
    /// <summary>
    /// Closes all tab pages to the right of a specified tab page.
    /// </summary>
    /// <param name="TabPage">The reference tab page.</param>
    void CloseTabsRight(TabItem TabPage)
    {
        int Index = Pager.Items.IndexOf(TabPage);
        if (Index < 0)
            return;

        foreach (TabItem Item in Pager.Items.Cast<TabItem>().Skip(Index + 1).ToList())
            CloseTab(Item);
    }
    /// <summary>
    /// Returns the form assigned to a tab item.
    /// </summary>
    /// <param name="TabPage">The tab item.</param>
    /// <returns>The assigned form, if any; otherwise, null.</returns>
    AppForm GetForm(TabItem TabPage)
    {
        return (TabPage.Tag is AppForm)? TabPage.Tag as AppForm : null;
    }
    /// <summary>
    /// Returns true if a pointer move should start a tab reorder operation.
    /// </summary>
    /// <param name="CurrentPoint">The current pointer point.</param>
    /// <returns>True if a tab reorder operation should start; otherwise false.</returns>
    bool ShouldStartTabDrag(Point CurrentPoint)
    {
        const double DragThreshold = 6;
        return Math.Abs(CurrentPoint.X - fDragStartPoint.X) > DragThreshold
            || Math.Abs(CurrentPoint.Y - fDragStartPoint.Y) > DragThreshold;
    }
    /// <summary>
    /// Returns the tab item at a pointer position.
    /// </summary>
    /// <param name="Point">The pointer position relative to the pager.</param>
    /// <returns>The tab item, if any; otherwise null.</returns>
    TabItem GetTabItemAt(Point Point)
    {
        foreach (object Item in Pager.Items)
        {
            TabItem TabPage = Item as TabItem;
            if (TabPage == null)
                continue;

            Point? TabPoint = Pager.TranslatePoint(Point, TabPage);
            if (TabPoint != null && new Rect(TabPage.Bounds.Size).Contains(TabPoint.Value))
                return TabPage;
        }

        return null;
    }
    /// <summary>
    /// Moves the dragged tab page to an insertion index.
    /// </summary>
    /// <param name="TargetIndex">The target insertion index.</param>
    void MoveDraggedTabPage(int TargetIndex)
    {
        if (fDraggedTabPage == null || TargetIndex < 0)
            return;

        int SourceIndex = Pager.Items.IndexOf(fDraggedTabPage);
        if (SourceIndex < 0)
            return;

        if (SourceIndex < TargetIndex)
            TargetIndex--;

        if (SourceIndex == TargetIndex)
            return;

        Pager.Items.RemoveAt(SourceIndex);
        TargetIndex = Math.Max(0, Math.Min(TargetIndex, Pager.Items.Count));
        Pager.Items.Insert(TargetIndex, fDraggedTabPage);
        Pager.SelectedItem = fDraggedTabPage;
    }
    /// <summary>
    /// Creates the tab drop marker.
    /// </summary>
    void CreateTabDropMarker()
    {
        if (fTabDropMarker != null)
            return;

        Border Marker = new Border
        {
            Width = 3,
            Background = Brushes.DodgerBlue,
            CornerRadius = new CornerRadius(2),
            IsHitTestVisible = false
        };

        fTabDropMarker = new Popup
        {
            PlacementTarget = Pager,
            Placement = PlacementMode.AnchorAndGravity,
            PlacementAnchor = PopupAnchor.TopLeft,
            PlacementGravity = PopupGravity.BottomRight,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.SlideY,
            Child = Marker,
            IsLightDismissEnabled = false
        };
    }
    /// <summary>
    /// Shows the tab drop marker at an insertion point.
    /// </summary>
    /// <param name="TargetTabPage">The target tab page.</param>
    /// <param name="PointerPoint">The pointer point relative to the pager.</param>
    void ShowTabDropMarker(TabItem TargetTabPage, Point PointerPoint)
    {
        if (TargetTabPage == null)
        {
            HideTabDropMarker();
            return;
        }

        CreateTabDropMarker();

        Point? Point = TargetTabPage.TranslatePoint(new Point(0, 0), Pager);
        if (Point == null)
            return;

        bool InsertAfter = PointerPoint.X > Point.Value.X + TargetTabPage.Bounds.Width / 2;
        int TargetIndex = Pager.Items.IndexOf(TargetTabPage);
        if (TargetIndex < 0)
        {
            HideTabDropMarker();
            return;
        }

        if (InsertAfter)
            TargetIndex++;

        double MarkerX = (InsertAfter ? Point.Value.X + TargetTabPage.Bounds.Width : Point.Value.X) - 2;
        double MarkerY = Point.Value.Y;
        double MarkerHeight = Math.Max(18, TargetTabPage.Bounds.Height);

        fDropTargetTabPage = TargetTabPage;
        fDropTargetIndex = TargetIndex;
        if (fTabDropMarker.Child is Border Marker)
            Marker.Height = MarkerHeight;

        fTabDropMarker.PlacementRect = new Rect(MarkerX, MarkerY, 3, MarkerHeight);
        fTabDropMarker.IsOpen = true;
    }
    /// <summary>
    /// Hides the tab drop marker.
    /// </summary>
    void HideTabDropMarker()
    {
        fDropTargetTabPage = null;
        fDropTargetIndex = -1;

        if (fTabDropMarker != null)
            fTabDropMarker.IsOpen = false;
    }
    /// <summary>
    /// Starts the current tab drag operation.
    /// </summary>
    /// <param name="Pointer">The pointer to capture.</param>
    void StartTabDrag(IPointer Pointer)
    {
        if (fDraggedTabPage == null)
            return;

        fIsDraggingTabPage = true;
        fDraggedTabPageOpacity = fDraggedTabPage.Opacity;
        fDraggedTabPage.Opacity = 0.65;
        Pointer?.Capture(Pager);
    }
    /// <summary>
    /// Clears the current tab drag operation.
    /// </summary>
    /// <param name="Pointer">The pointer to release.</param>
    void ClearTabDrag(IPointer Pointer)
    {
        Pointer?.Capture(null);

        if (fDraggedTabPage != null)
            fDraggedTabPage.Opacity = fDraggedTabPageOpacity;

        HideTabDropMarker();
        fDraggedTabPage = null;
        fIsDraggingTabPage = false;
        fDraggedTabPageOpacity = 1;
        fDragStartPoint = default;
    }
    /// <summary>
    /// Wires pager pointer handlers.
    /// </summary>
    void WirePager()
    {
        Pager.AddHandler(InputElement.PointerPressedEvent, Pager_PointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        Pager.AddHandler(InputElement.PointerMovedEvent, Pager_PointerMoved, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        Pager.AddHandler(InputElement.PointerReleasedEvent, Pager_PointerReleased, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
    }
    /// <summary>
    /// Handles pointer press events on the pager.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The pointer event arguments.</param>
    void Pager_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        TabItem TabPage = GetTabItemAt(e.GetPosition(Pager));
        if (TabPage == null)
            return;

        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            AppForm Form = GetForm(TabPage);
            if ((Form != null) && Form.ClosableByUser)
            {
                Form.CloseForm();
                e.Handled = true;
            }
        }
        else if (IsTabHeaderContextMenuVisible && e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            Pager.SelectedItem = TabPage;
            OpenTabHeaderContextMenu(TabPage);
            e.Handled = true;
        }
        else if (CanUserReorderTabs && e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            fDraggedTabPage = TabPage;
            fDragStartPoint = e.GetPosition(Pager);
            fIsDraggingTabPage = false;
        }
    }
    /// <summary>
    /// Handles pointer move events on the pager.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The pointer event arguments.</param>
    void Pager_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!CanUserReorderTabs || fDraggedTabPage == null)
            return;

        if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            ClearTabDrag(fIsDraggingTabPage ? e.Pointer : null);
            return;
        }

        Point Point = e.GetPosition(Pager);
        if (!fIsDraggingTabPage)
        {
            if (!ShouldStartTabDrag(Point))
                return;

            StartTabDrag(e.Pointer);
        }

        ShowTabDropMarker(GetTabItemAt(Point), Point);
    }
    /// <summary>
    /// Handles pointer release events on the pager.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The pointer event arguments.</param>
    void Pager_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (fDraggedTabPage != null || fIsDraggingTabPage)
        {
            if (fIsDraggingTabPage && fDropTargetTabPage != null)
                MoveDraggedTabPage(fDropTargetIndex);

            ClearTabDrag(fIsDraggingTabPage ? e.Pointer : null);
        }
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFormPagerHandler"/> class.
    /// </summary>
    /// <param name="Pager">The tab control handled by this instance.</param>
    public AppFormPagerHandler(TabControl Pager)
    {
        this.Pager = Pager;
        WirePager();
    }
    
    // ● public
    /// <summary>
    /// Finds a tab item by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    /// <returns>The matching tab item, if any; otherwise, null.</returns>
    public TabItem FindTabItem(string FormId)
    {
        var TabItems =  Pager.Items.Cast<TabItem>();
        foreach (TabItem Item in TabItems)
        {
            if (Item.Tag is AppForm)
            {
                AppForm  Form = (AppForm)Item.Tag;
                if (Sys.IsSameText(FormId, Form.FormId))
                {
                    return Item;
                }
            }
        }
      
        return null;
    }
    /// <summary>
    /// Finds an application form by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    /// <returns>The matching form, if any; otherwise, null.</returns>
    public AppForm FindAppForm(string FormId)
    {
        var TabItems =  Pager.Items.Cast<TabItem>();
        foreach (TabItem Item in TabItems)
        {
            if (Item.Tag is AppForm)
            {
                AppForm Form = (AppForm)Item.Tag;
                if (Sys.IsSameText(FormId, Form.FormId))
                {
                    return Form;
                }
            }
        }
      
        return null;
    }
 
    /// <summary>
    /// Shows an application form in the pager.
    /// </summary>
    /// <param name="Context">The form context.</param>
    /// <returns>The shown form.</returns>
    public AppForm ShowAppForm(FormContext Context)
    {
        if (Context == null)
            throw new TripousArgumentNullException(nameof(Context));

        AppForm Form = FindAppForm(Context.FormId);
        if (Form == null)
        {
            if (MaxAllowedTabPages > 0 && Pager.Items.Count >= MaxAllowedTabPages)
                throw new InvalidOperationException($"The maximum number of allowed tab pages has been reached: {MaxAllowedTabPages}");

            Context.DisplayMode = FormDisplayMode.TabItem;
            Form = Context.CreateForm();
            
            TabItem TabPage = new TabItem();
            TabPage.Tag = Form;
            Pager.Items.Add(TabPage);
            
            Context.ParentControl = TabPage;
            Form.Setup(Context);
        }

        Form.ParentTabControl.SelectedItem = Form.ParentTabPage;
        return Form;
    }
    /// <summary>
    /// Shows a data form in the pager.
    /// </summary>
    /// <param name="Context">The data form context.</param>
    /// <returns>The shown data form.</returns>
    public DataForm ShowDataForm(DataFormContext Context) => ShowAppForm(Context) as DataForm;
    /// <summary>
    /// Shows a data form in the pager.
    /// </summary>
    /// <param name="RegistryName">The registry name of the data form.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The shown data form.</returns>
    public DataForm ShowDataForm(string RegistryName, Control Caller = null) => ShowDataForm(DataFormContext.Create(RegistryName, Caller)); 
 
    /// <summary>
    /// Closes a form by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    public void CloseForm(string FormId)
    {
        AppForm Form = FindAppForm(FormId);
        if (Form != null)
            Form.CloseForm();
    }

    // ● properties
    /// <summary>
    /// Gets the tab control handled by this instance.
    /// </summary>
    public TabControl Pager { get; private set; }
    /// <summary>
    /// Gets or sets a value indicating whether the user can reorder tabs by dragging them.
    /// </summary>
    public bool CanUserReorderTabs { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether tab header context menus are visible.
    /// </summary>
    public bool IsTabHeaderContextMenuVisible { get; set; }
    /// <summary>
    /// Gets or sets the maximum allowed tab pages. A value of -1 means unlimited.
    /// </summary>
    public int MaxAllowedTabPages
    {
        get => fMaxAllowedTabPages;
        set
        {
            if (value == 0 || value < -1)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be -1 or greater than zero.");

            fMaxAllowedTabPages = value;
        }
    }
}

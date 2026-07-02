// ● tab page
/**
 * Represents a tab page, a child of a tp.TabControl.
 */
tp.TabPage = class extends tp.Component {
    // ● constructor
    /**
     * Creates a tab page.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The tab page create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.TabPage);
    }
    /**
     * Applies explicit create params to this tab page.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (Params.Tab instanceof HTMLElement)
            this.Tab = Params.Tab;
    }
    /**
     * Destroys the handle and the tab element.
     * @returns {void}
     */
    DoDispose() {
        if (tp.IsHTMLElement(this.Tab) && this.Tab.parentNode)
            this.Tab.parentNode.removeChild(this.Tab);
        super.DoDispose();
    }

    // ● properties
    /**
     * Gets or sets the title of the tab.
     * @returns {string} Returns the title.
     */
    get Title() {
        return tp.IsHTMLElement(this.Tab) ? this.Tab.innerHTML : "";
    }
    /**
     * Gets or sets the title of the tab.
     * @param {string} Value The title.
     * @returns {void}
     */
    set Title(Value) {
        if (tp.IsHTMLElement(this.Tab))
            this.Tab.innerHTML = tp.IsNil(Value) ? "" : String(Value);
    }
};

// ● prototype
/**
 * The tab caption element.
 * @type {HTMLElement|null}
 */
tp.TabPage.prototype.Tab = null;
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.TabPage.prototype.tpClass = "tp.TabPage";

// ● tab control
/**
 * A tab control with a responsive tab bar.
 * It uses tp.ItemBar for its tab bar, and tp.ResizeDetector through tp.ItemBar to detect whether tabs fit.
 * The tab bar can render in Normal, Toggle, or NextPrev mode.
 *
 * Events:
 * - SelectedIndexChanging
 * - SelectedIndexChanged
 * - PageAdded
 * - PageCloseRequested
 * - PageRemoving
 * - PageRemoved
 * - PageReordered
 *
 * @example
 * <div id="TabControl" class="tp-TabControl">
 *     <div><div>Page 1</div><div>Page 2</div></div>
 *     <div><div>Content 1</div><div>Content 2</div></div>
 * </div>
 */
tp.TabControl = class extends tp.Component {
    // ● constructor
    /**
     * Creates a tab control.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The tab control create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.TabControl);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Applies explicit create params to this tab control.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (Params) {
            if (tp.IsBoolean(Params.CanClosePages))
                this.CanClosePages = Params.CanClosePages;
            if (tp.IsBoolean(Params.CanReorderPages))
                this.CanReorderPages = Params.CanReorderPages;
        }
        this.ApplyTabBehaviors();
        if (Params && tp.IsNumber(Params.SelectedIndex))
            this.SelectedIndex = Params.SelectedIndex;
        else if (this.GetPageCount() > 0)
            this.SelectedIndex = 0;
    }
    /**
     * Creates child controls.
     * @returns {void}
     */
    CreateControls() {
        var List = this.GetElementList();
        var TabBarElement;
        var TabList;
        var PageList;
        var Index;
        var Tab;
        var PageElement;
        if (List.length === 2) {
            TabBarElement = List[0];
            this.PageContainer = List[1];
        } else if (List.length === 0) {
            TabBarElement = this.Document.createElement("div");
            this.PageContainer = this.Document.createElement("div");
            this.Handle.appendChild(TabBarElement);
            this.Handle.appendChild(this.PageContainer);
        } else {
            tp.Throw("Wrong TabControl structure. Should be empty or have two child DIVs.");
        }
        tp.AddClass(TabBarElement, tp.Classes.TabBar);
        tp.AddClass(this.PageContainer, tp.Classes.List);
        this.TabBar = new tp.ItemBar(TabBarElement);
        this.TabBar.On("SelectedIndexChanging", this.OnSelectedIndexChanging, this);
        this.TabBar.On("SelectedIndexChanged", this.OnSelectedIndexChanged, this);
        this.TabBar.On("ItemClicked", this.HandleTabBarItemClicked, this);
        this.fDragStartHandler = this.FuncBind(this.HandleTabDragStart);
        this.fDragOverHandler = this.FuncBind(this.HandleTabDragOver);
        this.fDropHandler = this.FuncBind(this.HandleTabDrop);
        this.fDragEndHandler = this.FuncBind(this.HandleTabDragEnd);
        TabList = this.GetTabElementList();
        PageList = this.GetPageElementList();
        if (TabList.length !== PageList.length)
            tp.Throw("Tabs and pages should be equal in number.");
        for (Index = 0; Index < PageList.length; Index++) {
            Tab = TabList[Index];
            PageElement = PageList[Index];
            this.CreateTabPage(PageElement, Tab);
        }
    }
    /**
     * Creates a tp.TabPage from a page element and tab element.
     * @param {HTMLElement} PageElement The page element.
     * @param {HTMLElement} TabElement The tab element.
     * @returns {tp.TabPage} Returns the tab page.
     */
    CreateTabPage(PageElement, TabElement) {
        var Page = new tp.TabPage({ ElementOrSelector: PageElement, Tab: TabElement });
        TabElement.TabPage = Page;
        PageElement.TabPage = Page;
        this.ApplyTabBehavior(TabElement);
        return Page;
    }
    /**
     * Applies optional behavior to a tab element.
     * @param {HTMLElement} TabElement The tab element.
     * @returns {void}
     */
    ApplyTabBehavior(TabElement) {
        if (!tp.IsHTMLElement(TabElement))
            return;
        TabElement.draggable = this.CanReorderPages === true;
        TabElement.removeEventListener("dragstart", this.fDragStartHandler);
        TabElement.removeEventListener("dragover", this.fDragOverHandler);
        TabElement.removeEventListener("drop", this.fDropHandler);
        TabElement.removeEventListener("dragend", this.fDragEndHandler);
        if (this.CanReorderPages === true) {
            TabElement.addEventListener("dragstart", this.fDragStartHandler);
            TabElement.addEventListener("dragover", this.fDragOverHandler);
            TabElement.addEventListener("drop", this.fDropHandler);
            TabElement.addEventListener("dragend", this.fDragEndHandler);
        }
    }
    /**
     * Creates and returns a new page without adding it.
     * @param {string|null|undefined} Title Optional title.
     * @returns {tp.TabPage} Returns the new page.
     */
    CreatePage(Title) {
        var Tab = this.Document.createElement("div");
        var PageElement = this.Document.createElement("div");
        Tab.innerHTML = tp.IsString(Title) && !tp.IsBlank(Title) ? Title : "no-name";
        return this.CreateTabPage(PageElement, Tab);
    }
    /**
     * Shows the selected page and hides all other pages without triggering ItemBar selection events.
     * @param {number} Index The selected index.
     * @returns {void}
     */
    SetSelectedIndex(Index) {
        var PageList = this.GetPageElementList();
        var Page;
        var i;
        this.TabBar.SetSelectedIndex(Index);
        for (i = 0; i < PageList.length; i++) {
            Page = PageList[i];
            Page.style.display = i === Index ? "" : "none";
        }
    }
    /**
     * Event trigger called before SelectedIndex changes.
     * @param {tp.EventArgs} Args The item bar event arguments.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanging(Args) {
        return this.Trigger("SelectedIndexChanging", Args);
    }
    /**
     * Event trigger called after SelectedIndex changes.
     * @param {tp.EventArgs} Args The item bar event arguments.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanged(Args) {
        this.SetSelectedIndex(Args.NewIndex);
        return this.Trigger("SelectedIndexChanged", Args);
    }
    /**
     * Event trigger called after a page is added.
     * @param {tp.TabPage} Page The page.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnPageAdded(Page) {
        return this.Trigger("PageAdded", { Child: Page, Page: Page });
    }
    /**
     * Event trigger called when a page close is requested.
     * @param {tp.TabPage} Page The page.
     * @param {number} Index The page index.
     * @param {Event|null|undefined} DomEvent The DOM event.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnPageCloseRequested(Page, Index, DomEvent) {
        return this.Trigger("PageCloseRequested", { Child: Page, Page: Page, Index: Index, e: DomEvent || null });
    }
    /**
     * Event trigger called before a page is removed.
     * @param {tp.TabPage} Page The page.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnPageRemoving(Page) {
        return this.Trigger("PageRemoving", { Child: Page, Page: Page });
    }
    /**
     * Event trigger called after a page is removed.
     * @param {tp.TabPage} Page The page.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnPageRemoved(Page) {
        return this.Trigger("PageRemoved", { Child: Page, Page: Page });
    }
    /**
     * Event trigger called after a page is reordered.
     * @param {tp.TabPage} Page The page.
     * @param {number} OldIndex The previous index.
     * @param {number} NewIndex The new index.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnPageReordered(Page, OldIndex, NewIndex) {
        return this.Trigger("PageReordered", { Child: Page, Page: Page, OldIndex: OldIndex, NewIndex: NewIndex });
    }
    /**
     * Handles tab bar item clicks.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleTabBarItemClicked(Args) {
        var Page;
        var CloseArgs;
        if (this.CanClosePages === true && Args && Args.MouseButton === tp.Mouse.MID && Args.ItemIndex >= 0) {
            Args.e.preventDefault();
            Args.e.stopPropagation();
            Page = this.PageAt(Args.ItemIndex);
            CloseArgs = this.OnPageCloseRequested(Page, Args.ItemIndex, Args.e);
            if (!CloseArgs || CloseArgs.Handled !== true)
                this.RemovePageAt(Args.ItemIndex);
        }
    }
    /**
     * Handles tab drag start.
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    HandleTabDragStart(e) {
        var Tab = this.FindTabElement(e.target);
        var Index = this.IndexOfTab(Tab);
        if (Index < 0)
            return;
        this.fDragPageIndex = Index;
        if (e.dataTransfer) {
            e.dataTransfer.effectAllowed = "move";
            e.dataTransfer.setData("text/plain", String(Index));
        }
    }
    /**
     * Handles tab drag over.
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    HandleTabDragOver(e) {
        if (this.CanReorderPages === true && this.fDragPageIndex >= 0) {
            e.preventDefault();
            if (e.dataTransfer)
                e.dataTransfer.dropEffect = "move";
        }
    }
    /**
     * Handles tab drop.
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    HandleTabDrop(e) {
        var Tab = this.FindTabElement(e.target);
        var Index = this.IndexOfTab(Tab);
        e.preventDefault();
        if (this.fDragPageIndex >= 0 && Index >= 0)
            this.MovePage(this.fDragPageIndex, Index);
        this.fDragPageIndex = -1;
    }
    /**
     * Handles tab drag end.
     * @returns {void}
     */
    HandleTabDragEnd() {
        this.fDragPageIndex = -1;
    }

    // ● public
    /**
     * Shows or hides the tab bar.
     * @param {boolean} Flag True to show; false to hide.
     * @returns {void}
     */
    ShowTabBar(Flag) {
        this.TabBar.Visible = Flag === true;
    }
    /**
     * Returns tab elements.
     * @returns {HTMLElement[]} Returns tab elements.
     */
    GetTabElementList() {
        return this.TabBar ? this.TabBar.GetItemElementList() : [];
    }
    /**
     * Returns page elements.
     * @returns {HTMLElement[]} Returns page elements.
     */
    GetPageElementList() {
        return tp.IsHTMLElement(this.PageContainer) ? tp.ChildHTMLElements(this.PageContainer) : [];
    }
    /**
     * Returns tab pages.
     * @returns {tp.TabPage[]} Returns tab pages.
     */
    GetPageList() {
        var Result = [];
        this.GetPageElementList().forEach(function (Element) {
            if (Element.TabPage instanceof tp.TabPage)
                Result.push(Element.TabPage);
        });
        return Result;
    }
    /**
     * Returns page count.
     * @returns {number} Returns page count.
     */
    GetPageCount() {
        return this.GetPageList().length;
    }
    /**
     * Applies optional tab behavior to all current tabs.
     * @returns {void}
     */
    ApplyTabBehaviors() {
        this.GetTabElementList().forEach(function (Tab) {
            this.ApplyTabBehavior(Tab);
        }, this);
    }
    /**
     * Finds a tab element from a descendant event target.
     * @param {EventTarget} Target The event target.
     * @returns {HTMLElement|null} Returns the tab element or null.
     */
    FindTabElement(Target) {
        var List = this.GetTabElementList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.ContainsEventTarget(List[Index], Target))
                return List[Index];
        }
        return null;
    }
    /**
     * Returns the index of a tab element.
     * @param {HTMLElement|null|undefined} Tab The tab element.
     * @returns {number} Returns the tab index or -1.
     */
    IndexOfTab(Tab) {
        return this.GetTabElementList().indexOf(Tab);
    }
    /**
     * Adds a page.
     * @param {string|null|undefined} Title Optional title.
     * @returns {tp.TabPage|null} Returns the added page.
     */
    AddPage(Title) {
        return this.InsertPage(this.GetPageCount(), Title);
    }
    /**
     * Inserts a page.
     * @param {number} Index The insert index.
     * @param {string|null|undefined} Title Optional title.
     * @returns {tp.TabPage|null} Returns the inserted page.
     */
    InsertPage(Index, Title) {
        var PageList;
        var Page;
        var ReferencePage;
        if (!this.HasHandle)
            return null;
        PageList = this.GetPageElementList();
        Page = this.CreatePage(Title);
        if (PageList.length === 0 || Index < 0 || Index >= PageList.length) {
            Index = PageList.length;
            this.TabBar.AddItem(Page.Tab);
            this.PageContainer.appendChild(Page.Handle);
        } else {
            ReferencePage = PageList[Index];
            this.TabBar.InsertItem(Page.Tab, Index);
            this.PageContainer.insertBefore(Page.Handle, ReferencePage);
        }
        this.OnPageAdded(Page);
        this.SelectedIndex = Index;
        return Page;
    }
    /**
     * Removes a page by index.
     * @param {number} Index The page index.
     * @returns {void}
     */
    RemovePageAt(Index) {
        var List = this.GetPageList();
        var Page;
        var NewIndex;
        if (tp.IsNumber(Index) && Index >= 0 && Index < List.length) {
            Page = List[Index];
            this.OnPageRemoving(Page);
            this.ClearTabBehavior(Page.Tab);
            this.TabBar.RemoveItemAt(Index);
            Page.Dispose();
            this.OnPageRemoved(Page);
            List = this.GetPageList();
            if (List.length === 0) {
                this.SelectedIndex = -1;
            } else {
                NewIndex = Math.min(Index, List.length - 1);
                this.SelectedIndex = NewIndex;
            }
        }
    }
    /**
     * Removes a page.
     * @param {tp.TabPage} Page The page.
     * @returns {void}
     */
    RemovePage(Page) {
        var Index = this.GetPageList().indexOf(Page);
        this.RemovePageAt(Index);
    }
    /**
     * Moves a page from one index to another.
     * @param {number} OldIndex The old index.
     * @param {number} NewIndex The new index.
     * @returns {void}
     */
    MovePage(OldIndex, NewIndex) {
        var PageList = this.GetPageList();
        var Page;
        var ReferencePage;
        if (!tp.IsNumber(OldIndex) || !tp.IsNumber(NewIndex) || OldIndex < 0 || NewIndex < 0 || OldIndex >= PageList.length || NewIndex >= PageList.length || OldIndex === NewIndex)
            return;
        Page = PageList[OldIndex];
        this.TabBar.RemoveItemAt(OldIndex);
        this.TabBar.InsertItem(Page.Tab, NewIndex);
        if (Page.Handle.parentNode === this.PageContainer)
            this.PageContainer.removeChild(Page.Handle);
        PageList = this.GetPageList();
        ReferencePage = PageList[NewIndex];
        if (ReferencePage)
            this.PageContainer.insertBefore(Page.Handle, ReferencePage.Handle);
        else
            this.PageContainer.appendChild(Page.Handle);
        this.SelectedPage = Page;
        this.OnPageReordered(Page, OldIndex, NewIndex);
    }
    /**
     * Returns a page by index.
     * @param {number} Index The page index.
     * @returns {tp.TabPage|null} Returns the page or null.
     */
    PageAt(Index) {
        var List = this.GetPageList();
        return Index >= 0 && Index < List.length ? List[Index] : null;
    }
    /**
     * Returns the title at an index.
     * @param {number} Index The page index.
     * @returns {string} Returns the title.
     */
    GetTitleAt(Index) {
        var Page = this.PageAt(Index);
        return Page ? Page.Title : "";
    }
    /**
     * Sets the title at an index.
     * @param {number} Index The page index.
     * @param {string} Text The title text.
     * @returns {void}
     */
    SetTitleAt(Index, Text) {
        var Page = this.PageAt(Index);
        if (Page)
            Page.Title = Text;
    }
    /**
     * Disposes this tab control.
     * @returns {void}
     */
    Dispose() {
        this.GetTabElementList().forEach(function (Tab) {
            this.ClearTabBehavior(Tab);
        }, this);
        this.GetPageList().forEach(function (Page) {
            Page.Dispose();
        });
        if (this.TabBar) {
            this.TabBar.Dispose();
            this.TabBar = null;
        }
        super.Dispose();
    }
    /**
     * Clears optional behavior from a tab element.
     * @param {HTMLElement} TabElement The tab element.
     * @returns {void}
     */
    ClearTabBehavior(TabElement) {
        if (!tp.IsHTMLElement(TabElement))
            return;
        TabElement.removeEventListener("dragstart", this.fDragStartHandler);
        TabElement.removeEventListener("dragover", this.fDragOverHandler);
        TabElement.removeEventListener("drop", this.fDropHandler);
        TabElement.removeEventListener("dragend", this.fDragEndHandler);
        TabElement.draggable = false;
    }

    // ● properties
    /**
     * Gets current tab render mode.
     * @returns {number} Returns a tp.ItemBarRenderMode value.
     */
    get RenderMode() {
        return this.TabBar ? this.TabBar.RenderMode : tp.ItemBarRenderMode.None;
    }
    /**
     * Gets or sets the responsive render mode.
     * @returns {number} Returns a tp.ItemBarRenderMode value.
     */
    get ResponsiveMode() {
        return this.TabBar ? this.TabBar.ResponsiveMode : tp.ItemBarRenderMode.NextPrev;
    }
    /**
     * Gets or sets the responsive render mode.
     * @param {number} Value The responsive render mode.
     * @returns {void}
     */
    set ResponsiveMode(Value) {
        if (this.TabBar)
            this.TabBar.ResponsiveMode = Value;
    }
    /**
     * Gets or sets selected page index.
     * @returns {number} Returns selected page index.
     */
    get SelectedIndex() {
        return this.TabBar ? this.TabBar.SelectedIndex : -1;
    }
    /**
     * Gets or sets selected page index.
     * @param {number} Value The selected page index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        if (this.TabBar)
            this.TabBar.SelectedIndex = Value;
    }
    /**
     * Gets or sets the selected page.
     * @returns {tp.TabPage|null} Returns the selected page.
     */
    get SelectedPage() {
        return this.PageAt(this.SelectedIndex);
    }
    /**
     * Gets or sets the selected page.
     * @param {tp.TabPage|null|undefined} Value The selected page.
     * @returns {void}
     */
    set SelectedPage(Value) {
        var Index;
        if (Value instanceof tp.TabPage) {
            Index = this.GetPageList().indexOf(Value);
            if (Index >= 0)
                this.SelectedIndex = Index;
        }
    }
};

// ● prototype
/**
 * The tab bar.
 * @type {tp.ItemBar|null}
 */
tp.TabControl.prototype.TabBar = null;
/**
 * The page container.
 * @type {HTMLElement|null}
 */
tp.TabControl.prototype.PageContainer = null;
/**
 * When true, pages may be closed by middle-clicking their tabs.
 * @type {boolean}
 */
tp.TabControl.prototype.CanClosePages = false;
/**
 * When true, pages may be reordered by dragging their tabs.
 * @type {boolean}
 */
tp.TabControl.prototype.CanReorderPages = false;
/**
 * The index of the dragged page.
 * @type {number}
 */
tp.TabControl.prototype.fDragPageIndex = -1;

tp.Ui.RegisterType(["TabControl", "tp-TabControl"], tp.TabControl);

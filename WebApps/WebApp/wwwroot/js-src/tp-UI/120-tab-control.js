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
        this.tpClass = "tp.TabPage";
        tp.AddClass(this.Handle, tp.Classes.TabPage);
        this.Tab = this.CreateParams.Tab;
    }

    // ● protected
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
 * - PageRemoving
 * - PageRemoved
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
        this.tpClass = "tp.TabControl";
        tp.AddClass(this.Handle, tp.Classes.TabControl);
        this.CreateControls();
        if (tp.IsNumber(this.CreateParams.SelectedIndex))
            this.SelectedIndex = this.CreateParams.SelectedIndex;
        else if (this.GetPageCount() > 0)
            this.SelectedIndex = 0;
    }

    // ● protected
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
        return Page;
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
        this.GetPageList().forEach(function (Page) {
            Page.Dispose();
        });
        if (this.TabBar) {
            this.TabBar.Dispose();
            this.TabBar = null;
        }
        super.Dispose();
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

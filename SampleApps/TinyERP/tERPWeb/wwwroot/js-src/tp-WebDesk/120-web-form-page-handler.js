// ● web form page handler
/**
 * Handles a tab control which displays web forms embedded in tab pages.
 */
tp.WebFormPageHandler = class {
    // ● constructor
    /**
     * Creates a web form page handler.
     * @param {object|null|undefined} Params The create parameters.
     */
    constructor(Params) {
        Params = Params || {};
        /**
         * The tab control handled by this instance.
         * @type {tp.TabControl|null}
         */
        this.TabControl = Params.TabControl instanceof tp.TabControl ? Params.TabControl : null;
        /**
         * Callback that returns a Promise resolving to a server web form packet.
         * @type {Function|null}
         */
        this.GetWebFormFunc = tp.IsFunction(Params.GetWebFormFunc) ? Params.GetWebFormFunc : null;
        /**
         * Optional callback used to report non-throwing errors.
         * @type {Function|null}
         */
        this.ErrorFunc = tp.IsFunction(Params.ErrorFunc) ? Params.ErrorFunc : null;
    }

    // ● protected
    /**
     * Returns the handled tab control.
     * @returns {tp.TabControl|null} Returns the tab control.
     */
    GetTabControl() {
        return this.TabControl;
    }
    /**
     * Calls the configured error callback or writes to the log box.
     * @param {string} Text The error text.
     * @returns {void}
     */
    ReportError(Text) {
        if (this.ErrorFunc)
            this.ErrorFunc(Text);
        else if (tp.LogBox)
            tp.LogBox.AppendLine(Text);
    }
    /**
     * Returns a server web form packet.
     * @param {string} WebFormName The web form name.
     * @returns {Promise<object>} Returns a Promise resolving with the packet.
     */
    async GetWebFormPacketAsync(WebFormName) {
        if (!this.GetWebFormFunc)
            throw new Error("No GetWebFormFunc callback is assigned.");
        return await this.GetWebFormFunc(WebFormName);
    }
    /**
     * Finds the root element of a web form inside a tab page.
     * @param {tp.TabPage} Page The tab page.
     * @returns {HTMLElement|null} Returns the form element or null.
     */
    FindFormElement(Page) {
        var Index;
        var Element;
        var Children = Page && Page.Handle ? Page.Handle.children : [];
        for (Index = 0; Index < Children.length; Index++) {
            Element = Children[Index];
            if (Element instanceof HTMLElement)
                return Element;
        }
        return null;
    }
    /**
     * Creates a web form context for a tab page.
     * @param {tp.TabPage} Page The tab page.
     * @param {object} Form The server form packet.
     * @param {object} Packet The server packet.
     * @returns {tp.WebFormContext} Returns the web form context.
     */
    CreateFormContext(Page, Form, Packet) {
        return new tp.WebFormContext({
            FormId: Form.Name,
            ClassName: Form.JsFormClassType,
            DisplayMode: tp.WebFormDisplayMode.TabPage,
            ParentControl: Page,
            Title: !tp.IsBlankString(Form.Title) ? Form.Title : Form.Name,
            WebFormDef: Form,
            Packet: Packet,
            CssFiles: Form.CssFiles || [],
            JavaScriptFiles: Form.JavaScriptFiles || []
        });
    }
    /**
     * Creates the client component that handles a web form page.
     * @param {tp.TabPage} Page The tab page.
     * @param {tp.WebFormContext} Context The web form context.
     * @returns {Promise<tp.WebForm>} Returns a Promise resolving with the client form component.
     */
    async CreateFormComponent(Page, Context) {
        var Element = this.FindFormElement(Page);
        if (!(Element instanceof HTMLElement))
            throw new Error("WebForm root element not found: " + Context.FormId);
        return await Context.CreateForm(Element);
    }
    /**
     * Handles a web form close request.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleFormCloseRequested(Args) {
        if (Args && Args.Context instanceof tp.WebFormContext && Args.Context.ParentControl instanceof tp.TabPage)
            this.ClosePage(Args.Context.ParentControl);
    }

    // ● public
    /**
     * Finds a tab page by web form identifier.
     * @param {string} FormId The form identifier.
     * @returns {tp.TabPage|null} Returns the tab page or null.
     */
    FindPage(FormId) {
        var TabControl = this.GetTabControl();
        var Pages;
        var Index;
        if (!TabControl)
            return null;
        Pages = TabControl.GetPageList();
        for (Index = 0; Index < Pages.length; Index++) {
            if (tp.IsSameText(Pages[Index].AppPageName, FormId))
                return Pages[Index];
        }
        return null;
    }
    /**
     * Finds a web form by form identifier.
     * @param {string} FormId The form identifier.
     * @returns {tp.WebForm|null} Returns the form or null.
     */
    FindForm(FormId) {
        var Page = this.FindPage(FormId);
        return Page && Page.AppComponent instanceof tp.WebForm ? Page.AppComponent : null;
    }
    /**
     * Opens a web form page.
     * @param {string} WebFormName The web form name.
     * @returns {Promise<tp.TabPage|null>} Returns a Promise with the opened page.
     */
    async OpenAsync(WebFormName) {
        var TabControl = this.GetTabControl();
        var Page;
        var Packet;
        var Form;
        var Component;
        var Context;
        if (!TabControl || tp.IsBlankString(WebFormName))
            return null;
        Page = this.FindPage(WebFormName);
        if (Page) {
            TabControl.SelectedPage = Page;
            if (Page.AppComponent instanceof tp.WebForm)
                Page.AppComponent.LoadData();
            else if (Page.AppComponent && tp.IsFunction(Page.AppComponent.Refresh))
                Page.AppComponent.Refresh();
            return Page;
        }
        Packet = await this.GetWebFormPacketAsync(WebFormName);
        Form = Packet ? Packet.Form : null;
        if (!Form)
            throw new Error("WebForm not returned: " + WebFormName);
        Page = TabControl.AddPage(!tp.IsBlankString(Form.Title) ? Form.Title : Form.Name);
        Page.AppPageName = Form.Name;
        Page.AppPageHandler = this;
        Page.Handle.innerHTML = Form.Html || "";
        Context = this.CreateFormContext(Page, Form, Packet);
        Page.AppContext = Context;
        Component = await this.CreateFormComponent(Page, Context);
        Component.On("CloseRequested", this.HandleFormCloseRequested, this);
        Page.AppComponent = Component;
        if (Component instanceof tp.WebForm)
            Component.LoadData();
        else if (Component && tp.IsFunction(Component.Refresh))
            Component.Refresh();
        return Page;
    }
    /**
     * Opens a web form page and logs failures without throwing.
     * @param {string} WebFormName The web form name.
     * @returns {void}
     */
    Open(WebFormName) {
        var Handler = this;
        this.OpenAsync(WebFormName).catch(function (e) {
            Handler.ReportError("Open web form failed: " + tp.ExceptionText(e));
        });
    }
    /**
     * Closes a web form page.
     * @param {tp.TabPage|null|undefined} Page The page to close.
     * @returns {void}
     */
    ClosePage(Page) {
        var TabControl = this.GetTabControl();
        var Component;
        if (!(TabControl && Page instanceof tp.TabPage))
            return;
        Component = Page.AppComponent;
        if (Component instanceof tp.WebForm && !Component.IsClosing) {
            if (Component.ClosableByUser)
                Component.CloseForm();
            return;
        }
        if (Component instanceof tp.Component) {
            Component.Dispose();
            Page.AppComponent = null;
        }
        Page.AppContext = null;
        TabControl.RemovePage(Page);
    }
    /**
     * Closes a web form page by form identifier.
     * @param {string} FormId The form identifier.
     * @returns {void}
     */
    CloseForm(FormId) {
        var Page = this.FindPage(FormId);
        if (Page)
            this.ClosePage(Page);
    }
};

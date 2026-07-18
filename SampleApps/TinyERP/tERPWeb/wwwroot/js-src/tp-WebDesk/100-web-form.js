// ● web form display mode
/**
 * Indicates how a WebDesk form is displayed.
 * @enum {string}
 */
tp.WebFormDisplayMode = {
    TabPage: "TabPage",
    Dialog: "Dialog"
};
Object.freeze(tp.WebFormDisplayMode);

// ● web form context
/**
 * Holds the created objects and result data of a web form opening operation.
 */
tp.WebFormContext = class {
    // ● constructor
    /**
     * Creates a web form context.
     * @param {object|null|undefined} Source The optional source object.
     */
    constructor(Source) {
        this.Params = {};
        if (tp.IsObject(Source))
            tp.MergePropsShallow(this, Source);
        if (!tp.IsObject(this.Params))
            this.Params = {};
    }

    // ● public
    /**
     * Creates a web form instance.
     * @param {HTMLElement|string|null|undefined} ElementOrSelectorOrHtmlText The element, selector, or HTML markup text.
     * @returns {Promise<tp.WebForm>} Returns a Promise resolving with the created form.
     */
    async CreateForm(ElementOrSelectorOrHtmlText) {
        if (!(this.Form instanceof tp.WebForm))
            this.Form = await tp.WebForm.CreateWebForm(ElementOrSelectorOrHtmlText, null, this);
        return this.Form;
    }
};

// ● prototype
/**
 * A string that uniquely identifies the form among all forms.
 * @type {string}
 */
tp.WebFormContext.prototype.FormId = "";
/**
 * The JavaScript class name used to create the form.
 * @type {string}
 */
tp.WebFormContext.prototype.ClassName = "";
/**
 * Indicates how the form is displayed.
 * @type {string}
 */
tp.WebFormContext.prototype.DisplayMode = tp.WebFormDisplayMode.TabPage;
/**
 * The host control that contains the form.
 * @type {*}
 */
tp.WebFormContext.prototype.ParentControl = null;
/**
 * The created web form instance.
 * @type {tp.WebForm|null}
 */
tp.WebFormContext.prototype.Form = null;
/**
 * Optional title override.
 * @type {string}
 */
tp.WebFormContext.prototype.Title = "";
/**
 * The modal result of the dialog displaying the form.
 * @type {number}
 */
tp.WebFormContext.prototype.ModalResult = tp.DialogResult.None;
/**
 * Optional result data returned by the form.
 * @type {*}
 */
tp.WebFormContext.prototype.ResultData = null;
/**
 * Optional options.
 * @type {*}
 */
tp.WebFormContext.prototype.Options = null;
/**
 * Optional parameter bag.
 * @type {object}
 */
tp.WebFormContext.prototype.Params = null;
/**
 * Optional user data.
 * @type {*}
 */
tp.WebFormContext.prototype.Tag = null;
/**
 * The server web form definition.
 * @type {object|null}
 */
tp.WebFormContext.prototype.WebFormDef = null;
/**
 * The server web form packet.
 * @type {object|null}
 */
tp.WebFormContext.prototype.Packet = null;
/**
 * The dynamic CSS files loaded for the form.
 * @type {string[]|null}
 */
tp.WebFormContext.prototype.CssFiles = null;
/**
 * The dynamic JavaScript files loaded for the form.
 * @type {string[]|null}
 */
tp.WebFormContext.prototype.JavaScriptFiles = null;

// ● web form
/**
 * Base class for WebDesk forms.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 * - CloseRequested
 */
tp.WebForm = class extends tp.Component {
    // ● private
    /**
     * Resolves an element, selector, or HTML markup text to an element.
     * @param {HTMLElement|string|null|undefined} ElementOrSelectorOrHtmlText The element, selector, or HTML markup text.
     * @returns {HTMLElement|null} Returns the resolved element or null.
     */
    static ResolveFormElement(ElementOrSelectorOrHtmlText) {
        if (ElementOrSelectorOrHtmlText instanceof HTMLElement)
            return ElementOrSelectorOrHtmlText;
        if (tp.IsString(ElementOrSelectorOrHtmlText) && tp.IsHtml(ElementOrSelectorOrHtmlText))
            return tp.HtmlToElement(ElementOrSelectorOrHtmlText);
        if (tp.IsString(ElementOrSelectorOrHtmlText))
            return tp(ElementOrSelectorOrHtmlText);
        return null;
    }
    /**
     * Resolves a dotted global object name.
     * @param {string} Name The dotted name.
     * @returns {*} Returns the resolved value or null.
     */
    static ResolveGlobalName(Name) {
        var Parts;
        var Index;
        var Result = window;
        if (tp.IsBlankString(Name))
            return null;
        Parts = String(Name).split(".");
        for (Index = 0; Index < Parts.length; Index++) {
            if (tp.IsBlankString(Parts[Index]) || tp.IsNil(Result[Parts[Index]]))
                return null;
            Result = Result[Parts[Index]];
        }
        return Result;
    }
    /**
     * Resolves the JavaScript class type for a web form.
     * @param {object} Params The create params.
     * @param {tp.WebFormContext|null} Context The optional web form context.
     * @returns {Function|null} Returns the class type or null.
     */
    static ResolveFormType(Params, Context) {
        var ClassType = Params ? (Params.ClassType || Params.JsFormClassType) : null;
        var ClassName = Params ? (Params.ClassName || "") : "";
        if (tp.IsBlankString(ClassName) && Context)
            ClassName = Context.ClassName;
        if (tp.IsBlankString(ClassName) && Context && Context.WebFormDef)
            ClassName = Context.WebFormDef.JsFormClassType || "";
        if (tp.IsFunction(ClassType))
            return ClassType;
        if (tp.IsString(ClassType) && !tp.IsBlankString(ClassType))
            ClassName = ClassType;
        return tp.WebForm.ResolveGlobalName(ClassName);
    }

    // ● static public
    /**
     * Creates and returns a web form.
     * @param {HTMLElement|string|null|undefined} ElementOrSelectorOrHtmlText The element, selector, or HTML markup text.
     * @param {object|null|undefined} CreateParams Optional create params. When null, data-setup is used.
     * @param {tp.WebFormContext|null|undefined} Context Optional web form context.
     * @returns {Promise<tp.WebForm>} Returns a Promise resolving with the created form.
     */
    static async CreateWebForm(ElementOrSelectorOrHtmlText, CreateParams, Context) {
        var Element = tp.WebForm.ResolveFormElement(ElementOrSelectorOrHtmlText);
        var Params;
        var Type;
        var Result;
        var CssFiles;
        var JavaScriptFiles;
        if (!(Element instanceof HTMLElement))
            throw new Error("Cannot create web form. No element, selector, or HTML markup text is passed.");
        Params = tp.IsObject(CreateParams) ? new tp.CreateParams(CreateParams) : new tp.CreateParams(tp.GetDataSetupObject(Element));
        Params.ElementOrSelector = Element;
        CssFiles = tp.IsArray(Params.CssFiles) ? Params.CssFiles : (tp.IsArray(Params.CSS) ? Params.CSS : []);
        JavaScriptFiles = tp.IsArray(Params.JavaScriptFiles) ? Params.JavaScriptFiles : (tp.IsArray(Params.JS) ? Params.JS : []);
        if (Context instanceof tp.WebFormContext) {
            Params.Context = Context;
            if (tp.IsBlankString(Context.ClassName))
                Context.ClassName = Params.ClassName || Params.JsFormClassType || "";
            CssFiles = CssFiles.length > 0 ? CssFiles : (Context.CssFiles || []);
            JavaScriptFiles = JavaScriptFiles.length > 0 ? JavaScriptFiles : (Context.JavaScriptFiles || []);
            Context.CssFiles = CssFiles;
            Context.JavaScriptFiles = JavaScriptFiles;
        }
        await tp.StaticFiles.LoadCssFiles(CssFiles);
        await tp.StaticFiles.LoadJavascriptFiles(JavaScriptFiles);
        try {
            Type = tp.WebForm.ResolveFormType(Params, Context);
            if (!tp.IsFunction(Type))
                throw new Error("Cannot create web form. No JavaScript class type is specified.");
            Result = new Type(Params);
            if (!(Result instanceof tp.WebForm))
                throw new Error("Cannot create web form. The specified class does not extend tp.WebForm.");
        } catch (e) {
            tp.StaticFiles.UnLoadJavascriptFiles(JavaScriptFiles);
            tp.StaticFiles.UnLoadCssFiles(CssFiles);
            throw e;
        }
        return Result;
    }

    // ● constructor
    /**
     * Creates a WebDesk form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
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
        tp.AddClass(this.Handle, tp.Classes.WebForm);
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.Context = null;
        this.fTitleText = "";
        this.fModalResult = tp.DialogResult.None;
        this.IsSetupDone = false;
        this.IsFormInitialized = false;
        this.ClosableByUser = true;
        this.IsClosing = false;
        this.fBroadcasterRegistered = false;
    }
    /**
     * Applies explicit create params to this component.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (Params && Params.Context instanceof tp.WebFormContext)
            this.Setup(Params.Context);
    }
    /**
     * Destroys the component handle and releases resources.
     * @returns {void}
     */
    DoDispose() {
        this.UnregisterBroadcaster();
        if (this.Context instanceof tp.WebFormContext) {
            tp.StaticFiles.UnLoadJavascriptFiles(this.Context.JavaScriptFiles || []);
            tp.StaticFiles.UnLoadCssFiles(this.Context.CssFiles || []);
        }
        super.DoDispose();
    }

    // ● overridables
    /**
     * Called just before form initialization.
     * @returns {void}
     */
    FormInitializing() {
        this.ProcessFormOptions();
    }
    /**
     * Called in order to initialize the form.
     * @returns {void}
     */
    FormInitialize() {
    }
    /**
     * Called just after form initialization.
     * @returns {void}
     */
    FormInitialized() {
    }
    /**
     * Executes any first operation on the form.
     * @returns {Promise<void>} Returns a Promise.
     */
    async StartAsync() {
    }
    /**
     * Called just after the context is assigned.
     * @returns {void}
     */
    SetupContext() {
    }
    /**
     * Processes form options.
     * @returns {void}
     */
    ProcessFormOptions() {
    }
    /**
     * Called just before the form is closed.
     * @returns {void}
     */
    Closing() {
    }
    /**
     * Called just after the form is closed.
     * @returns {void}
     */
    Closed() {
        this.UnregisterBroadcaster();
    }
    /**
     * Handles a broadcaster event.
     * @param {string} EventName The event name.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleBroadcasterEvent(EventName, Args) {
    }
    /**
     * Called when the TitleText property changes.
     * @returns {void}
     */
    TitleTextChanged() {
        if (this.Context instanceof tp.WebFormContext) {
            this.Context.Title = this.TitleText;
            if (this.ParentControl instanceof tp.TabPage)
                this.ParentControl.Title = this.TitleText;
            else if (this.ParentControl instanceof tp.Window)
                this.ParentControl.Title = this.TitleText;
        }
    }
    /**
     * Loads form data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
    }

    // ● public
    /**
     * Sets up this form using a web form context.
     * @param {tp.WebFormContext} Context The web form context.
     * @returns {void}
     */
    Setup(Context) {
        if (!this.IsSetupDone && Context instanceof tp.WebFormContext) {
            this.Context = Context;
            Context.Form = this;
            this.TitleText = Context.Title;
            this.SetupContext();
            this.IsSetupDone = true;
            this.RegisterBroadcaster();
            this.FormInitializing();
            this.FormInitialize();
            this.IsFormInitialized = true;
            this.FormInitialized();
            this.Start();
        }
    }
    /**
     * Executes the asynchronous start operation without throwing.
     * @returns {void}
     */
    Start() {
        var ShowSpinner = tp.IsFunction(tp.ShowSpinner);
        if (ShowSpinner)
            tp.ShowSpinner(true);
        this.StartAsync()
            .catch(function (e) {
                if (tp.LogBox)
                    tp.LogBox.AppendLine("WebForm start failed: " + tp.ExceptionText(e));
            })
            .finally(function () {
                if (ShowSpinner)
                    tp.ShowSpinner(false);
            });
    }
    /**
     * Loads form data without throwing.
     * @returns {void}
     */
    LoadData() {
        this.LoadDataAsync().catch(function (e) {
            if (tp.LogBox)
                tp.LogBox.AppendLine("WebForm load data failed: " + tp.ExceptionText(e));
        });
    }
    /**
     * Returns true if this form can close.
     * @returns {boolean} Returns true if this form can close.
     */
    CanCloseForm() {
        return true;
    }
    /**
     * Closes this form.
     * @returns {void}
     */
    CloseForm() {
        if (this.CanCloseForm()) {
            this.IsClosing = true;
            try {
                this.Closing();
                this.Trigger("CloseRequested", { Form: this, Context: this.Context });
            } finally {
                this.IsClosing = false;
            }
            this.Closed();
        }
    }
    /**
     * Closes this form.
     * @returns {void}
     */
    Close() {
        this.CloseForm();
    }
    /**
     * Registers this form to the broadcaster.
     * @returns {void}
     */
    RegisterBroadcaster() {
        if (this.fBroadcasterRegistered !== true && tp.Broadcaster && tp.Broadcaster.Add) {
            tp.Broadcaster.Add(this);
            this.fBroadcasterRegistered = true;
        }
    }
    /**
     * Unregisters this form from the broadcaster.
     * @returns {void}
     */
    UnregisterBroadcaster() {
        if (this.fBroadcasterRegistered === true && tp.Broadcaster && tp.Broadcaster.Remove) {
            tp.Broadcaster.Remove(this);
            this.fBroadcasterRegistered = false;
        }
    }
    /**
     * Handles broadcaster notifications.
     * @param {tp.EventArgs} Args The broadcaster arguments.
     * @returns {void}
     */
    BroadcasterFunc(Args) {
        if (Args && Args.Sender !== this)
            this.HandleBroadcasterEvent(Args.EventName, Args);
    }

    // ● properties
    /**
     * Gets the form identifier.
     * @returns {string} Returns the form identifier.
     */
    get FormId() {
        return this.Context instanceof tp.WebFormContext ? this.Context.FormId : "";
    }
    /**
     * Gets the parent control.
     * @returns {*} Returns the parent control.
     */
    get ParentControl() {
        return this.Context instanceof tp.WebFormContext ? this.Context.ParentControl : null;
    }
    /**
     * Gets true when this form is displayed in a modal dialog.
     * @returns {boolean} Returns true when this form is displayed in a modal dialog.
     */
    get IsModal() {
        return this.Context instanceof tp.WebFormContext && this.Context.DisplayMode === tp.WebFormDisplayMode.Dialog;
    }
    /**
     * Gets the form title text.
     * @returns {string} Returns the form title text.
     */
    get TitleText() {
        return this.fTitleText;
    }
    /**
     * Sets the form title text.
     * @param {string} Value The title text.
     */
    set TitleText(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fTitleText !== Value) {
            this.fTitleText = Value;
            this.TitleTextChanged();
        }
    }
    /**
     * Gets the modal result.
     * @returns {number} Returns the modal result.
     */
    get ModalResult() {
        return this.IsModal ? this.fModalResult : tp.DialogResult.None;
    }
    /**
     * Sets the modal result.
     * @param {number} Value The modal result.
     */
    set ModalResult(Value) {
        if (this.IsModal && this.fModalResult !== Value) {
            this.fModalResult = Value;
            if (this.Context instanceof tp.WebFormContext)
                this.Context.ModalResult = Value;
            if (Value !== tp.DialogResult.None)
                this.CloseForm();
        }
    }
};

// ● prototype
/**
 * The form context.
 * @type {tp.WebFormContext|null}
 */
tp.WebForm.prototype.Context = null;
/**
 * The title text of the form.
 * @type {string}
 */
tp.WebForm.prototype.fTitleText = "";
/**
 * The modal result of the dialog displaying this form.
 * @type {number}
 */
tp.WebForm.prototype.fModalResult = tp.DialogResult.None;
/**
 * True when the setup of this form is done.
 * @type {boolean}
 */
tp.WebForm.prototype.IsSetupDone = false;
/**
 * True when form initialization is done.
 * @type {boolean}
 */
tp.WebForm.prototype.IsFormInitialized = false;
/**
 * True when the user can close this form directly from the host.
 * @type {boolean}
 */
tp.WebForm.prototype.ClosableByUser = true;
/**
 * True while closing.
 * @type {boolean}
 */
tp.WebForm.prototype.IsClosing = false;
/**
 * True when this form is registered to the broadcaster.
 * @type {boolean}
 */
tp.WebForm.prototype.fBroadcasterRegistered = false;

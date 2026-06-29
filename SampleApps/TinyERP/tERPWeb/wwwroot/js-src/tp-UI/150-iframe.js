// ● iframe
/**
 * Wraps an iframe element.
 *
 * Events:
 * - Loaded
 *
 * @example
 * <iframe id="IFrame"></iframe>
 * <script>
 *     var Frame = new tp.IFrame("#IFrame", {
 *         Width: 800,
 *         Height: 600,
 *         Url: "/demo/tp-ready"
 *     });
 * </script>
 */
tp.IFrame = class extends tp.Component {
    // ● constructor
    /**
     * Creates an iframe component.
     * @param {tp.CreateParams|object|HTMLIFrameElement|string} CreateParams The iframe create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = arguments.length > 1 ? tp.IFrame.CreateParams(CreateParams, Options) : tp.IFrame.CreateParams(CreateParams);
        super(Params);
    }

    // ● protected
    /**
     * Creates normalized iframe create parameters.
     * @param {tp.CreateParams|object|HTMLIFrameElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        if (arguments.length > 1 && !tp.IsNil(Options)) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        return Params;
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fLoadHandler = this.FuncBind(this.DocumentLoaded);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        if (!(this.Handle instanceof HTMLIFrameElement))
            tp.Throw("tp.IFrame requires an HTMLIFrameElement handle.");
        tp.AddClass(this.Handle, tp.Classes.Frame);
        tp.FrameRemoveBorder(this.Handle);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.Handle.addEventListener("load", this.fLoadHandler);
    }
    /**
     * Applies explicit create params to this iframe.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        this.ApplyIFrameParams(Params);
    }
    /**
     * Applies create parameters specific to tp.IFrame.
     * @param {tp.CreateParams|object|null|undefined} Params The create parameters.
     * @returns {void}
     */
    ApplyIFrameParams(Params) {
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
        if (!tp.IsNil(Params.UseSpinner))
            this.UseSpinner = Params.UseSpinner === true;
        if (!tp.IsNil(Params.Content))
            this.Content = Params.Content;
        if (!tp.IsNil(Params.Url))
            this.Url = Params.Url;
    }
    /**
     * Handles the iframe load event.
     * @param {Event} e The event object.
     * @returns {void}
     */
    DocumentLoaded(e) {
        this.HideLoadSpinner();
        this.OnLoaded(e);
    }
    /**
     * Event trigger called after the iframe document is loaded.
     * @param {Event} e The load event.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnLoaded(e) {
        return this.Trigger("Loaded", { e: e });
    }
    /**
     * Shows the global spinner when an external load starts.
     * @param {string} Url The URL to load.
     * @returns {void}
     */
    ShowLoadSpinner(Url) {
        if (tp.IsString(Url) && !tp.IsBlank(Url) && this.UseSpinner === true && this.fSpinnerVisible !== true) {
            tp.ShowSpinner(true);
            this.fSpinnerVisible = true;
        }
    }
    /**
     * Hides the global spinner when this instance has shown it.
     * @returns {void}
     */
    HideLoadSpinner() {
        if (this.fSpinnerVisible === true) {
            tp.ShowSpinner(false);
            this.fSpinnerVisible = false;
        }
    }

    // ● public
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        this.HideLoadSpinner();
        if (this.HasHandle && this.fLoadHandler)
            this.Handle.removeEventListener("load", this.fLoadHandler);
        this.fLoadHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Retrieves the document object of the page or frame.
     * @returns {Document|null} Returns the iframe content document.
     */
    get ContentDoc() {
        return this.Handle instanceof HTMLIFrameElement ? this.Handle.contentDocument : null;
    }
    /**
     * Returns the Window object of the iframe element.
     * @returns {Window|null} Returns the iframe window.
     */
    get Window() {
        return this.Handle instanceof HTMLIFrameElement ? this.Handle.contentWindow : null;
    }
    /**
     * Gets or sets the iframe width.
     * @returns {string|null} Returns the width attribute.
     */
    get Width() {
        return this.Handle instanceof HTMLIFrameElement ? this.Handle.width : null;
    }
    /**
     * Gets or sets the iframe width.
     * @param {string|number} Value The width value. A number is treated as pixels.
     * @returns {void}
     */
    set Width(Value) {
        if (this.Handle instanceof HTMLIFrameElement)
            this.Handle.width = tp.IsNumber(Value) ? tp.px(Value) : Value;
    }
    /**
     * Gets or sets the iframe height.
     * @returns {string|null} Returns the height attribute.
     */
    get Height() {
        return this.Handle instanceof HTMLIFrameElement ? this.Handle.height : null;
    }
    /**
     * Gets or sets the iframe height.
     * @param {string|number} Value The height value. A number is treated as pixels.
     * @returns {void}
     */
    set Height(Value) {
        if (this.Handle instanceof HTMLIFrameElement)
            this.Handle.height = tp.IsNumber(Value) ? tp.px(Value) : Value;
    }
    /**
     * Gets or sets the URL loaded by this iframe.
     * @returns {string} Returns the URL.
     */
    get Url() {
        return this.fUrl;
    }
    /**
     * Gets or sets the URL loaded by this iframe.
     * @param {string} Value The URL.
     * @returns {void}
     */
    set Url(Value) {
        if (this.Handle instanceof HTMLIFrameElement) {
            if (tp.IsBlank(Value))
                this.HideLoadSpinner();
            else
                this.ShowLoadSpinner(Value);
            this.fUrl = Value;
            this.Handle.src = Value;
            if (!tp.IsBlank(Value))
                this.Handle.removeAttribute("srcdoc");
        }
    }
    /**
     * Returns true when the browser supports the iframe srcdoc attribute.
     * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/HTMLIFrameElement/srcdoc|MDN HTMLIFrameElement.srcdoc}
     * @see {@link https://stackoverflow.com/questions/19739001/which-is-the-difference-between-srcdoc-and-src-datatext-html-in-an|stackoverflow}
     * @returns {boolean} Returns true when srcdoc is supported.
     */
    get SupportsSrcDoc() {
        return this.Handle instanceof HTMLIFrameElement && "srcdoc" in this.Handle;
    }
    /**
     * Gets or sets the HTML content of the page shown in the iframe.
     * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/HTMLIFrameElement/srcdoc|MDN HTMLIFrameElement.srcdoc}
     * @see {@link https://stackoverflow.com/questions/19739001/which-is-the-difference-between-srcdoc-and-src-datatext-html-in-an|stackoverflow}
     * @returns {string} Returns the iframe HTML content.
     */
    get Content() {
        return this.SupportsSrcDoc ? this.Handle.srcdoc : this.Handle.src;
    }
    /**
     * Gets or sets the HTML content of the page shown in the iframe.
     * @param {string} Value The HTML content.
     * @returns {void}
     */
    set Content(Value) {
        var Doc;
        if (this.Handle instanceof HTMLIFrameElement) {
            this.HideLoadSpinner();
            this.fUrl = "";
            if (this.SupportsSrcDoc) {
                this.Handle.srcdoc = Value;
            } else {
                Doc = this.ContentDoc;
                if (Doc) {
                    Doc.open();
                    Doc.write(Value);
                    Doc.close();
                }
            }
        }
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.IFrame.prototype.tpClass = "tp.IFrame";
/**
 * Private field.
 * @type {string}
 */
tp.IFrame.prototype.fUrl = "";
/**
 * A value indicating whether to display a global spinner while loading a document to the iframe.
 * @type {boolean}
 */
tp.IFrame.prototype.UseSpinner = true;
/**
 * Private field.
 * @type {Function|null}
 */
tp.IFrame.prototype.fLoadHandler = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.IFrame.prototype.fSpinnerVisible = false;

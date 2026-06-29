// ● image box
/**
 * A background-image based image control.
 *
 * The control is a div that uses background-image, background-size, background-position,
 * and background-repeat instead of an img element.
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 */
tp.ImageBox = class extends tp.Control {
    // ● private
    /**
     * Creates image-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateImageBoxParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "div";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● constructor
    /**
     * Creates an image box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.ImageBox.CreateImageBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Url";
    }
    /**
     * Applies explicit create params to this image box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
        if (!tp.IsNil(Params.Url))
            this.Url = Params.Url;
        if (!tp.IsNil(Params.ImageMode))
            this.ImageMode = Params.ImageMode;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.ImageBox);
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
        super.Bind();
        this.ReadDataValue();
    }

    // ● properties
    /**
     * Gets or sets the image url.
     * @returns {string} Returns the image url or an empty string.
     */
    get Url() {
        var Value = this.Handle instanceof HTMLElement ? this.Handle.style.backgroundImage || "" : "";
        if (Value.indexOf("url(") === 0)
            Value = Value.substring(4, Value.length - 1).replace(/^["']|["']$/g, "");
        return Value === "none" ? "" : Value;
    }
    /**
     * Gets or sets the image url.
     * @param {string} Value The image url or data url.
     * @returns {void}
     */
    set Url(Value) {
        if (this.Handle instanceof HTMLElement) {
            Value = tp.IsNil(Value) ? "" : String(Value);
            this.Handle.style.backgroundImage = tp.IsBlank(Value) ? "" : "url(\"" + Value + "\")";
        }
    }
    /**
     * Gets or sets the image size mode.
     * @returns {number} Returns a tp.ImageSizeMode value.
     */
    get ImageMode() {
        var Value = this.Handle instanceof HTMLElement ? this.Handle.style.backgroundSize || "" : "";
        if (Value === "cover")
            return tp.ImageSizeMode.Crop;
        if (Value === "contain")
            return tp.ImageSizeMode.Scale;
        if (Value === "100% 100%")
            return tp.ImageSizeMode.Stretch;
        return tp.ImageSizeMode.Unknown;
    }
    /**
     * Gets or sets the image size mode.
     * @param {number|string} Value The tp.ImageSizeMode value or enum name.
     * @returns {void}
     */
    set ImageMode(Value) {
        if (tp.IsString(Value)) {
            if (tp.IsSameText(Value, "Crop"))
                Value = tp.ImageSizeMode.Crop;
            else if (tp.IsSameText(Value, "Scale"))
                Value = tp.ImageSizeMode.Scale;
            else if (tp.IsSameText(Value, "Stretch"))
                Value = tp.ImageSizeMode.Stretch;
            else
                Value = tp.ImageSizeMode.Stretch;
        }
        if (this.Handle instanceof HTMLElement) {
            if (Value === tp.ImageSizeMode.Crop)
                this.Handle.style.backgroundSize = "cover";
            else if (Value === tp.ImageSizeMode.Scale)
                this.Handle.style.backgroundSize = "contain";
            else if (Value === tp.ImageSizeMode.Stretch)
                this.Handle.style.backgroundSize = "100% 100%";
        }
    }
    /**
     * Gets or sets CSS width.
     * @returns {string} Returns the width.
     */
    get Width() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.width || "" : "";
    }
    /**
     * Gets or sets CSS width.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Gets or sets CSS height.
     * @returns {string} Returns the height.
     */
    get Height() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.height || "" : "";
    }
    /**
     * Gets or sets CSS height.
     * @param {number|string} Value The height.
     * @returns {void}
     */
    set Height(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.height = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
};

tp.Ui.RegisterType(["ImageBox", "tp-ImageBox"], tp.ImageBox);

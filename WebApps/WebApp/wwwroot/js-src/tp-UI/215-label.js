// ● label
/**
 * Label control.
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
tp.Label = class extends tp.Control {
    // ● private
    /**
     * Creates label create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateLabelParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "label";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "label";
        return Args;
    }

    // ● constructor
    /**
     * Creates a label.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.Label.CreateLabelParams(CreateParams));
        this.tpClass = "tp.Label";
        tp.AddClass(this.Handle, tp.Classes.Label);
        this.Handle.style.display = "inline-block";
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Text";
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
     * Returns the first text node of this label, if any.
     * @returns {Text|null} Returns the first text node or null.
     */
    get TextNode() {
        return tp.FindTextNode(this.Handle);
    }
    /**
     * Gets or sets the label text.
     * @returns {string} Returns the label text.
     */
    get Text() {
        var TextNode = this.TextNode;
        return TextNode ? TextNode.nodeValue : "";
    }
    /**
     * Gets or sets the label text.
     * @param {*} Value The label text.
     * @returns {void}
     */
    set Text(Value) {
        var TextNode = this.TextNode;
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (!TextNode) {
            TextNode = this.Document.createTextNode(Value);
            this.Handle.appendChild(TextNode);
        }
        if (TextNode)
            TextNode.nodeValue = Value;
    }
    /**
     * Gets or sets the id of the associated control.
     * @returns {string} Returns the associated control id.
     */
    get AssociateId() {
        return this.Handle instanceof HTMLLabelElement ? this.Handle.htmlFor : "";
    }
    /**
     * Gets or sets the id of the associated control.
     * @param {string} Value The associated control id.
     * @returns {void}
     */
    set AssociateId(Value) {
        if (this.Handle instanceof HTMLLabelElement)
            this.Handle.htmlFor = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets the associated control element.
     * @returns {HTMLElement|null} Returns the associated control element.
     */
    get Associate() {
        return this.Handle instanceof HTMLLabelElement ? this.Handle.control : null;
    }
};

tp.Ui.RegisterType(["Label", "tp-Label"], tp.Label);

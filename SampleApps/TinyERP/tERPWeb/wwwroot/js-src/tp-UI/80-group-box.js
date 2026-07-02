// ● group box
/**
 * A fieldset container with a legend title element.
 * @example
 * <fieldset id="GroupBox"></fieldset>
 * <script>
 *     var GroupBox = new tp.GroupBox("#GroupBox");
 *     GroupBox.Text = "Title";
 * </script>
 */
tp.GroupBox = class extends tp.Component {
    // ● constructor
    /**
     * Creates a group box.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The group box create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
        this.tpClass = "tp.GroupBox";
        tp.AddClass(this.Handle, tp.Classes.GroupBox);
    }

    // ● protected
    /**
     * Ensures this group box has a legend element.
     * @returns {void}
     */
    OnHandleCreated() {
        var Legend;
        super.OnHandleCreated();
        Legend = tp.Select(this.Handle, "legend");
        if (Legend instanceof HTMLLegendElement) {
            this.fLegend = Legend;
        } else {
            this.fLegend = this.Document.createElement("legend");
            this.Handle.insertBefore(this.fLegend, this.Handle.firstChild);
        }
        this.fLegend.style.display = tp.IsBlank(this.Text) ? "none" : "";
    }

    // ● properties
    /**
     * Gets or sets the group box title.
     * @returns {string} Returns the title.
     */
    get Text() {
        return this.fLegend instanceof HTMLLegendElement ? this.fLegend.innerHTML : "";
    }
    /**
     * Gets or sets the group box title.
     * @param {string} Value The title.
     * @returns {void}
     */
    set Text(Value) {
        if (tp.IsString(Value) && this.fLegend instanceof HTMLLegendElement) {
            this.fLegend.innerHTML = Value;
            this.fLegend.style.display = tp.IsBlank(Value) ? "none" : "";
        }
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.GroupBox.prototype.tpClass = "tp.GroupBox";
/**
 * The legend element.
 * @type {HTMLLegendElement|null}
 */
tp.GroupBox.prototype.fLegend = null;

tp.Ui.RegisterType(["GroupBox", "tp-GroupBox"], tp.GroupBox);

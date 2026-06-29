// ● log box
/**
 * Provides static logging to a text area.
 * @type {object}
 */
tp.LogBox = {
    // ● fields
    /**
     * The separator line text.
     * @type {string}
     */
    SeparatorLine: "-------------------------------------------------------------------",
    /**
     * The line break used when rendering text.
     * @type {string}
     */
    LineBreak: "\n",
    /**
     * True to prefix appended lines with the current date and time.
     * @type {boolean}
     */
    Timestamp: true,
    /**
     * True to scroll the target to the end after rendering.
     * @type {boolean}
     */
    AutoScroll: true,

    fElement: null,
    fLines: [],
    fMaxLines: 1000,

    // ● private
    /**
     * Resolves a log target to a text area element.
     * @param {HTMLTextAreaElement|string|null|undefined} Target The target text area or selector.
     * @returns {HTMLTextAreaElement|null} Returns the resolved text area or null.
     */
    ResolveElement: function (Target) {
        var Element;
        if (Target instanceof HTMLTextAreaElement)
            return Target;
        if (tp.IsString(Target)) {
            Element = tp.Select(Target);
            return Element instanceof HTMLTextAreaElement ? Element : null;
        }
        return null;
    },
    /**
     * Applies log box style constraints to the text area.
     * @returns {void}
     */
    PrepareElement: function () {
        if (this.fElement instanceof HTMLTextAreaElement) {
            this.fElement.style.resize = "none";
            this.fElement.style.overflow = "auto";
            this.fElement.wrap = "off";
        }
    },
    /**
     * Returns the text currently displayed by the target.
     * @returns {string} Returns the current target text.
     */
    ReadTargetText: function () {
        if (this.fElement instanceof HTMLTextAreaElement)
            return this.fElement.value || "";
        return "";
    },
    /**
     * Writes text to the target.
     * @param {string} Text The text to write.
     * @returns {void}
     */
    WriteTargetText: function (Text) {
        Text = tp.IsNil(Text) ? "" : String(Text);
        if (this.fElement instanceof HTMLTextAreaElement)
            this.fElement.value = Text;
    },
    /**
     * Formats the current timestamp.
     * @returns {string} Returns the timestamp text.
     */
    FormatTimestamp: function () {
        var Value = new Date();
        var Pad = function (NumberValue) { return String(NumberValue).padStart(2, "0"); };
        return Value.getFullYear()
            + "-" + Pad(Value.getMonth() + 1)
            + "-" + Pad(Value.getDate())
            + " " + Pad(Value.getHours())
            + ":" + Pad(Value.getMinutes())
            + ":" + Pad(Value.getSeconds());
    },
    /**
     * Normalizes text to a string array.
     * @param {*} Text The source text.
     * @returns {string[]} Returns the normalized lines.
     */
    ToLines: function (Text) {
        Text = tp.IsNil(Text) ? "" : String(Text);
        Text = Text.replace(/\r\n/g, "\n").replace(/\r/g, "\n");
        return Text.split("\n");
    },
    /**
     * Trims old lines when the buffer exceeds MaxLines.
     * @returns {void}
     */
    Trim: function () {
        if (this.fMaxLines > 0 && this.fLines.length > this.fMaxLines)
            this.fLines.splice(0, this.fLines.length - this.fMaxLines);
    },
    /**
     * Renders the internal line buffer to the target.
     * @returns {void}
     */
    Render: function () {
        this.Trim();
        this.WriteTargetText(this.fLines.join(this.LineBreak));
        if (this.AutoScroll === true)
            this.ScrollToEnd();
    },
    /**
     * Adds the specified lines to the internal line buffer.
     * @param {string[]} Lines The lines to add.
     * @returns {void}
     */
    AddLines: function (Lines) {
        if (!tp.IsArray(Lines))
            return;
        Lines.forEach(function (Line) {
            this.fLines.push(tp.IsNil(Line) ? "" : String(Line));
        }, this);
    },

    // ● public
    /**
     * Initializes this log box service.
     * @param {HTMLTextAreaElement|string|null|undefined} Target The target text area or selector.
     * @param {object|null|undefined} Options The optional settings.
     * @returns {void}
     */
    Initialize: function (Target, Options) {
        this.fElement = this.ResolveElement(Target);
        if (!this.fElement)
            return;
        this.PrepareElement();
        if (tp.IsObject(Options)) {
            if (!tp.IsNil(Options.MaxLines))
                this.MaxLines = Options.MaxLines;
            if (!tp.IsNil(Options.Timestamp))
                this.Timestamp = Options.Timestamp === true;
            if (!tp.IsNil(Options.AutoScroll))
                this.AutoScroll = Options.AutoScroll === true;
        }
        this.fLines = this.ToLines(this.ReadTargetText());
        if (this.fLines.length === 1 && this.fLines[0] === "")
            this.fLines = [];
        this.Render();
    },
    /**
     * Clears the log target and the internal line buffer.
     * @returns {void}
     */
    Clear: function () {
        this.fLines = [];
        if (this.IsInitialized)
            this.WriteTargetText("");
    },
    /**
     * Appends text to the last existing line.
     * @param {*} Text The text to append.
     * @returns {void}
     */
    Append: function (Text) {
        if (!this.IsInitialized || tp.IsNil(Text))
            return;
        Text = String(Text);
        if (Text.length === 0)
            return;
        if (this.fLines.length === 0)
            this.fLines.push(Text);
        else
            this.fLines[this.fLines.length - 1] += Text;
        this.Render();
    },
    /**
     * Appends a new text line.
     * @param {*} Text The text to append.
     * @returns {void}
     */
    AppendLine: function (Text) {
        var Lines;
        var Index;
        if (!this.IsInitialized)
            return;
        if (tp.IsNil(Text) || Text === "") {
            this.fLines.push("");
            this.Render();
            return;
        }
        Text = String(Text);
        Lines = this.ToLines(Text);
        for (Index = 0; Index < Lines.length; Index++) {
            if (this.Timestamp === true && Lines[Index] !== this.SeparatorLine)
                this.fLines.push("[" + this.FormatTimestamp() + "] " + Lines[Index] + " ");
            else
                this.fLines.push(Lines[Index]);
        }
        this.Render();
    },
    /**
     * Appends a new empty text line.
     * @returns {void}
     */
    AppendLineEmpty: function () {
        this.AppendLine("");
    },
    /**
     * Appends a separator line.
     * @returns {void}
     */
    Separator: function () {
        this.AppendLine(this.SeparatorLine);
    },
    /**
     * Scrolls the target to the end.
     * @returns {void}
     */
    ScrollToEnd: function () {
        if (this.fElement instanceof HTMLTextAreaElement)
            this.fElement.scrollTop = this.fElement.scrollHeight;
    },

    // ● properties
    /**
     * Returns true when this service has a target.
     * @returns {boolean} Returns true when this service has a target.
     */
    get IsInitialized() {
        return this.fElement instanceof HTMLTextAreaElement;
    },
    /**
     * Gets the current target.
     * @returns {HTMLTextAreaElement|null} Returns the target text area.
     */
    get Target() {
        return this.fElement;
    },
    /**
     * Gets the current line count.
     * @returns {number} Returns the line count.
     */
    get LineCount() {
        return this.fLines.length;
    },
    /**
     * Gets or sets the maximum number of log lines kept.
     * @returns {number} Returns the maximum line count.
     */
    get MaxLines() {
        return this.fMaxLines;
    },
    /**
     * Gets or sets the maximum number of log lines kept.
     * @param {number} Value The maximum line count.
     * @returns {void}
     */
    set MaxLines(Value) {
        Value = tp.ToInt(Value);
        this.fMaxLines = Value > 0 ? Value : 0;
        this.Trim();
        if (this.IsInitialized)
            this.Render();
    }
};

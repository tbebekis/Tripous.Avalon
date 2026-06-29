// ● code editor
/**
 * Represents a source code editor wrapper.
 * It uses Ace Editor when it can be loaded and falls back to a simple browser element when Ace is not available.
 */
tp.CodeEditor = class extends tp.Component {
    // ● constructor
    /**
     * Creates a code editor wrapper.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● static public
    /**
     * Creates and initializes a code editor wrapper.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     * @returns {Promise<tp.CodeEditor>} Returns the initialized code editor wrapper.
     */
    static async CreateAsync(CreateParams) {
        var Result = new tp.CodeEditor(CreateParams);
        await Result.InitializeEditorAsync();
        return Result;
    }
    /**
     * Loads Ace Editor when needed.
     * @param {string} Url The Ace Editor script URL.
     * @returns {Promise<boolean>} Returns true when Ace Editor is available.
     */
    static async LoadAceAsync(Url) {
        if (typeof ace !== "undefined")
            return true;
        try {
            await tp.StaticFiles.LoadJavascriptFile(Url);
            return typeof ace !== "undefined";
        } catch (e) {
            return false;
        }
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fEditor = null;
        this.fIsAce = false;
        this.fMode = "text";
        this.fTheme = "twilight";
        this.fText = "";
        this.fReadOnly = false;
        this.fUseAce = true;
        this.fAceUrl = tp.CodeEditor.AceCdnUrl;
        this.fFontSize = 14;
        this.fShowPrintMargin = false;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.CodeEditor);
    }
    /**
     * Applies explicit create params to this component.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Mode))
            this.fMode = String(Params.Mode);
        if (!tp.IsNil(Params.Theme))
            this.fTheme = String(Params.Theme);
        if (!tp.IsNil(Params.Text))
            this.fText = String(Params.Text);
        if (!tp.IsNil(Params.ReadOnly))
            this.fReadOnly = Params.ReadOnly === true;
        if (!tp.IsNil(Params.UseAce))
            this.fUseAce = Params.UseAce === true;
        if (!tp.IsNil(Params.AceUrl))
            this.fAceUrl = String(Params.AceUrl);
        if (!tp.IsNil(Params.FontSize))
            this.fFontSize = tp.StrToInt(Params.FontSize, this.fFontSize);
        if (!tp.IsNil(Params.ShowPrintMargin))
            this.fShowPrintMargin = Params.ShowPrintMargin === true;
    }
    /**
     * Initializes the inner editor implementation.
     * @returns {Promise<void>} Returns a promise resolved when initialization is complete.
     */
    async InitializeEditorAsync() {
        var AceAvailable = false;
        if (this.fUseAce === true)
            AceAvailable = await tp.CodeEditor.LoadAceAsync(this.fAceUrl);
        if (AceAvailable === true)
            this.CreateAceEditor();
        else
            this.CreateFallbackEditor();
    }
    /**
     * Creates the Ace Editor implementation.
     * @returns {void}
     */
    CreateAceEditor() {
        var Text = this.fText;
        if (tp.IsBlank(Text))
            Text = this.GetFallbackText();
        var Editor = ace.edit(this.Handle);
        tp.RemoveClass(this.Handle, tp.Classes.CodeEditorFallback);
        Editor.setTheme("ace/theme/" + this.Theme);
        Editor.session.setMode("ace/mode/" + this.Mode);
        Editor.setFontSize(this.FontSize);
        Editor.setReadOnly(this.ReadOnly);
        Editor.setShowPrintMargin(this.ShowPrintMargin);
        Editor.setValue(Text, -1);
        this.fText = Text;
        this.fEditor = Editor;
        this.fIsAce = true;
        this.Handle.__Editor = Editor;
    }
    /**
     * Creates the fallback editor implementation.
     * @returns {void}
     */
    CreateFallbackEditor() {
        tp.AddClass(this.Handle, tp.Classes.CodeEditorFallback);
        this.fEditor = null;
        this.fIsAce = false;
        this.Handle.__Editor = null;
        this.ApplyFallbackReadOnly();
        this.SetFallbackText(this.Text);
    }
    /**
     * Applies the read-only state to the fallback element.
     * @returns {void}
     */
    ApplyFallbackReadOnly() {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.readOnly = this.ReadOnly;
        else
            this.Handle.contentEditable = this.ReadOnly === true ? "false" : "true";
    }
    /**
     * Gets text from the fallback element.
     * @returns {string} Returns the fallback text.
     */
    GetFallbackText() {
        if (this.Handle instanceof HTMLTextAreaElement)
            return this.Handle.value || "";
        return this.Handle.textContent || "";
    }
    /**
     * Sets text to the fallback element.
     * @param {string} Value The text.
     * @returns {void}
     */
    SetFallbackText(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.value = Value;
        else
            this.Handle.textContent = Value;
    }

    // ● public
    /**
     * Sets the editor text.
     * @param {string} Value The text.
     * @returns {void}
     */
    SetValue(Value) {
        this.Text = Value;
    }
    /**
     * Gets the editor text.
     * @returns {string} Returns the editor text.
     */
    GetValue() {
        return this.Text;
    }
    /**
     * Sets the read-only state.
     * @param {boolean} Value True for read-only.
     * @returns {void}
     */
    SetReadOnly(Value) {
        this.ReadOnly = Value === true;
    }
    /**
     * Sets the editor mode.
     * @param {string} Value The mode name, such as html, javascript, css, sql, or csharp.
     * @returns {void}
     */
    SetMode(Value) {
        this.Mode = Value;
    }
    /**
     * Sets input focus to the editor.
     * @returns {void}
     */
    Focus() {
        if (this.IsAce && this.Editor)
            this.Editor.focus();
        else
            super.Focus();
    }

    // ● properties
    /**
     * Gets the wrapped Ace Editor instance when Ace is active.
     * @returns {object|null} Returns the Ace Editor instance or null.
     */
    get Editor() {
        return this.fEditor;
    }
    /**
     * Returns true when Ace Editor is active.
     * @returns {boolean} Returns true when Ace Editor is active.
     */
    get IsAce() {
        return this.fIsAce === true;
    }
    /**
     * Gets or sets the editor mode.
     * @returns {string} Returns the mode name.
     */
    get Mode() {
        return this.fMode;
    }
    /**
     * Gets or sets the editor mode.
     * @param {string} Value The mode name.
     * @returns {void}
     */
    set Mode(Value) {
        this.fMode = tp.IsBlank(Value) ? "text" : String(Value);
        if (this.IsAce && this.Editor)
            this.Editor.session.setMode("ace/mode/" + this.fMode);
    }
    /**
     * Gets or sets the Ace theme name.
     * @returns {string} Returns the theme name.
     */
    get Theme() {
        return this.fTheme;
    }
    /**
     * Gets or sets the Ace theme name.
     * @param {string} Value The theme name.
     * @returns {void}
     */
    set Theme(Value) {
        this.fTheme = tp.IsBlank(Value) ? "twilight" : String(Value);
        if (this.IsAce && this.Editor)
            this.Editor.setTheme("ace/theme/" + this.fTheme);
    }
    /**
     * Gets or sets the editor text.
     * @returns {string} Returns the editor text.
     */
    get Text() {
        if (this.IsAce && this.Editor)
            return this.Editor.getValue();
        if (this.HasHandle)
            return this.GetFallbackText();
        return this.fText;
    }
    /**
     * Gets or sets the editor text.
     * @param {string} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        this.fText = tp.IsNil(Value) ? "" : String(Value);
        if (this.IsAce && this.Editor)
            this.Editor.setValue(this.fText, -1);
        else if (this.HasHandle)
            this.SetFallbackText(this.fText);
    }
    /**
     * Gets or sets the read-only state.
     * @returns {boolean} Returns true when read-only.
     */
    get ReadOnly() {
        return this.fReadOnly === true;
    }
    /**
     * Gets or sets the read-only state.
     * @param {boolean} Value True for read-only.
     * @returns {void}
     */
    set ReadOnly(Value) {
        this.fReadOnly = Value === true;
        if (this.IsAce && this.Editor)
            this.Editor.setReadOnly(this.fReadOnly);
        else if (this.HasHandle)
            this.ApplyFallbackReadOnly();
    }
    /**
     * Gets or sets the Ace Editor script URL.
     * @returns {string} Returns the Ace Editor script URL.
     */
    get AceUrl() {
        return this.fAceUrl;
    }
    /**
     * Gets or sets the Ace Editor script URL.
     * @param {string} Value The Ace Editor script URL.
     * @returns {void}
     */
    set AceUrl(Value) {
        this.fAceUrl = String(Value || "");
    }
    /**
     * Gets or sets the editor font size.
     * @returns {number} Returns the font size.
     */
    get FontSize() {
        return this.fFontSize;
    }
    /**
     * Gets or sets the editor font size.
     * @param {number|string} Value The font size.
     * @returns {void}
     */
    set FontSize(Value) {
        this.fFontSize = tp.StrToInt(Value, this.fFontSize);
        if (this.IsAce && this.Editor)
            this.Editor.setFontSize(this.fFontSize);
    }
    /**
     * Gets or sets whether Ace shows the print margin.
     * @returns {boolean} Returns true when the print margin is shown.
     */
    get ShowPrintMargin() {
        return this.fShowPrintMargin === true;
    }
    /**
     * Gets or sets whether Ace shows the print margin.
     * @param {boolean} Value True to show the print margin.
     * @returns {void}
     */
    set ShowPrintMargin(Value) {
        this.fShowPrintMargin = Value === true;
        if (this.IsAce && this.Editor)
            this.Editor.setShowPrintMargin(this.fShowPrintMargin);
    }
};

// ● static fields
/**
 * Gets or sets the default Ace Editor CDN URL.
 * @type {string}
 */
tp.CodeEditor.AceCdnUrl = "https://cdnjs.cloudflare.com/ajax/libs/ace/1.36.2/ace.js";

tp.Ui.RegisterType("CodeEditor", tp.CodeEditor);

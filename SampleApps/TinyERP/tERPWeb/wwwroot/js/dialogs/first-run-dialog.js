/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● first run dialog
/**
 * Displays the first application run administrator dialog.
 */
app.FirstRunDialog = class {
    // ● constructor
    /**
     * Creates the first run dialog helper.
     * @param {object|null|undefined} Params Optional dialog params.
     */
    constructor(Params) {
        Params = Params || {};
        /**
         * Dialog create params.
         * @type {object}
         */
        this.Params = Params;
    }

    // ● protected
    /**
     * Sets the message text of the dialog.
     * @param {tp.Window} Window The dialog window.
     * @param {string} Text The message text.
     * @returns {void}
     */
    SetMessage(Window, Text) {
        var Element = Window && Window.Handle ? Window.Handle.querySelector("[data-role='message']") : null;
        if (Element)
            Element.textContent = Text || "";
    }
    /**
     * Collects values from the dialog.
     * @param {tp.Window} Window The dialog window.
     * @returns {object} Returns a value object.
     */
    CollectData(Window) {
        var Result = {};
        var Elements = Window && Window.Handle ? Window.Handle.querySelectorAll("input[name], select[name]") : [];
        var Index;
        var Element;
        for (Index = 0; Index < Elements.length; Index++) {
            Element = Elements[Index];
            Result[Element.name] = Element.value;
        }
        return Result;
    }
    /**
     * Handles dialog key presses.
     * @param {KeyboardEvent} e The keyboard event.
     * @param {tp.Window} Window The dialog window.
     * @returns {void}
     */
    HandleKeyDown(e, Window) {
        if (tp.IsKey(e, tp.Keys.Enter)) {
            e.preventDefault();
            Window.DialogResult = tp.DialogResult.OK;
        }
    }

    // ● public
    /**
     * Shows the dialog.
     * @param {object|null|undefined} Packet Optional startup information packet.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null when cancelled.
     */
    async ShowAsync(Packet, Message) {
        Packet = Packet || {};
        var Self = this;
        var Args = {
            Text: "First Application Run",
            Width: 420,
            Height: 400,
            ResizeEdges: tp.Edge.None,
            InitialFocusSelector: "input[autofocus], input",
            ShowFunc: function (Window) {
                Self.SetMessage(Window, Message);
                Window.FirstRunDialogKeyDownHandler = function (e) {
                    Self.HandleKeyDown(e, Window);
                };
                Window.Handle.addEventListener("keydown", Window.FirstRunDialogKeyDownHandler);
            },
            CloseFunc: function (Window) {
                if (Window.FirstRunDialogKeyDownHandler)
                    Window.Handle.removeEventListener("keydown", Window.FirstRunDialogKeyDownHandler);
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectData(Window);
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Packet.FirstRunHtml || "", Args);
        return Window.DialogResult === tp.DialogResult.OK ? Window.ResultData : null;
    }
};

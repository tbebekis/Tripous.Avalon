/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● login dialog
/**
 * Displays the login dialog.
 */
app.LoginDialog = class {
    // ● constructor
    /**
     * Creates the login dialog helper.
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
            Text: "Login",
            Width: 400,
            Height: 300,
            ResizeEdges: tp.Edge.None,
            InitialFocusSelector: "input[autofocus], input",
            ShowFunc: function (Window) {
                Self.SetMessage(Window, Message);
                Window.LoginDialogKeyDownHandler = function (e) {
                    Self.HandleKeyDown(e, Window);
                };
                Window.Handle.addEventListener("keydown", Window.LoginDialogKeyDownHandler);
            },
            CloseFunc: function (Window) {
                if (Window.LoginDialogKeyDownHandler)
                    Window.Handle.removeEventListener("keydown", Window.LoginDialogKeyDownHandler);
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectData(Window);
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Packet.LoginHtml || "", Args);
        return Window.DialogResult === tp.DialogResult.OK ? Window.ResultData : null;
    }
};

/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● change password dialog
/**
 * Displays the change password dialog.
 */
app.ChangePasswordDialog = class {
    // ● constructor
    /**
     * Creates the change password dialog helper.
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
     * Sets the user caption text.
     * @param {tp.Window} Window The dialog window.
     * @param {object|null|undefined} Packet The server packet.
     * @returns {void}
     */
    SetUser(Window, Packet) {
        var Element = Window && Window.Handle ? Window.Handle.querySelector("[data-role='user']") : null;
        if (Element)
            Element.textContent = tp._L("User", "User") + ": " + (Packet && Packet.UserName ? Packet.UserName : "");
    }
    /**
     * Collects values from the dialog.
     * @param {tp.Window} Window The dialog window.
     * @returns {object} Returns a value object.
     */
    CollectData(Window) {
        var Result = {};
        var Elements = Window && Window.Handle ? Window.Handle.querySelectorAll("input[name]") : [];
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
    /**
     * Shows the modal dialog once.
     * @param {object|null|undefined} Packet The server packet.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null when cancelled.
     */
    async ShowDialogAsync(Packet, Message) {
        Packet = Packet || {};
        var Self = this;
        var Args = {
            Text: tp._L("ChangePassword", "Change Password"),
            Width: 420,
            Height: 340,
            ResizeEdges: tp.Edge.None,
            InitialFocusSelector: "input[autofocus], input",
            ShowFunc: function (Window) {
                tp.AddClass(Window.Handle, "app-change-password-window");
                Self.SetUser(Window, Packet);
                Self.SetMessage(Window, Message);
                Window.ChangePasswordDialogKeyDownHandler = function (e) {
                    Self.HandleKeyDown(e, Window);
                };
                Window.Handle.addEventListener("keydown", Window.ChangePasswordDialogKeyDownHandler);
            },
            CloseFunc: function (Window) {
                if (Window.ChangePasswordDialogKeyDownHandler)
                    Window.Handle.removeEventListener("keydown", Window.ChangePasswordDialogKeyDownHandler);
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectData(Window);
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Packet.Html || "", Args);
        return Window.DialogResult === tp.DialogResult.OK ? Window.ResultData : null;
    }

    // ● public
    /**
     * Shows the dialog and saves the new password.
     * @param {object|null|undefined} Packet The server packet.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ShowAsync(Packet) {
        var Message = "";
        var Data;
        var Result;
        while (true) {
            Data = await this.ShowDialogAsync(Packet, Message);
            if (Data === null)
                return;
            Result = await app.App.ChangePasswordAsync(Data);
            Message = Result && Result.Message ? Result.Message : "";
            if (Result && Result.Success === true) {
                if (tp.LogBox)
                    tp.LogBox.AppendLine(Message || tp._L("PasswordChanged", "Password changed."));
                if (app.App.MainPage && app.App.MainPage.StatusBar)
                    app.App.MainPage.StatusBar.Message = Message || tp._L("PasswordChanged", "Password changed.");
                return;
            }
        }
    }
};
